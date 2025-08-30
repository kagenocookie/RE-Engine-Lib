namespace ReeLib;

using System;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using ReeLib.Common;
using ReeLib.Pak;

/// <summary>
/// Abstraction of common file operations across PAK files.
/// </summary>
public class PakReader
{
    /// <summary>
    /// List of PAK files to handle. Should be in chronological order.
    /// </summary>
    public List<string> PakFilePriority { get; set; } = new();

    /// <summary>
    /// The upper limit for many threads should be queued up for file extraction. 1 = disable multithreading.<br/>
    /// The actual used thread count will be determined by the .NET ThreadPool.
    /// </summary>
    public int MaxThreads { get; set; } = 32;

    /// <summary>
    /// A regex to use for filtering which files should get handled. The filter is applied during every AddFiles* call and should therefore be set before adding files.
    /// </summary>
    public Regex? Filter { get; set; }

    public bool EnableConsoleLogging { get; set; } = true;

    protected readonly Dictionary<ulong, string> searchedPaths = new();

    public void ResetFileList()
    {
        searchedPaths.Clear();
    }

    public void AddFilesFromListFile(string fileList)
    {
        using var stream = File.OpenRead(fileList);
        AddFilesFromListFile(stream);
    }

    public void AddFilesFromListFile(Stream stream)
    {
        var f = new StreamReader(stream);
        while (!f.EndOfStream) {
            var line = f.ReadLine();
            if (!string.IsNullOrWhiteSpace(line)) {
                if (Filter != null && !Filter.IsMatch(line)) continue;

                searchedPaths.TryAdd(PakUtils.GetFilepathHash(line), line);
            }
        }
    }

    private const string ManifestFilepath = "__MANIFEST/MANIFEST.TXT";

    public void AddFiles(params string[] files) => AddFiles((IEnumerable<string>)files);

    public bool TryReadManifestFileList(string pakFile)
    {
        PakFilePriority = [pakFile];
        AddFiles(ManifestFilepath);
        var file = FindFiles().SingleOrDefault();
        if (file.stream == null) return false;
        AddFilesFromListFile(file.stream);
        return true;
    }

    public void AddFiles(IEnumerable<string> files)
    {
        foreach (var file in files) {
            if (Filter != null && !Filter.IsMatch(file)) continue;

            searchedPaths.Add(PakUtils.GetFilepathHash(file), file);
        }
    }

    private int CalculateChunkCount(int entryCount)
    {
        // reduce the number of threads to have at least 64 entries per thread, otherwise it's meaningless
        return MaxThreads <= 1 ? 1 : Math.Max(Math.Min(MaxThreads, entryCount / 64), 1);
    }

    /// <summary>
    /// Finds requested files and returns a memory stream for each of them.
    /// The returned stream is transient, do not hold a permanent reference to it - copy any data you need somewhere else and don't Dispose() it.
    /// </summary>
    public IEnumerable<(string path, MemoryStream stream)> FindFiles(CancellationToken cancellationToken = default)
    {
        var pak = new PakFile();
        foreach (var pakfile in EnumerateTempPaksWithSearchedFiles(pak)) {
            if (cancellationToken.IsCancellationRequested) yield break;
            var ctx = new ChunkContextBase(pak, 0, pak.Entries.Count);
            foreach (var (entry, data) in FindEntriesInChunk(ctx)) {
                data.Seek(0, SeekOrigin.Begin);
                yield return (entry.path!, data);
            }

            int _ = 0;
            UpdateSearchedPaths(searchedPaths, [ctx], ref _);
            if (EnableConsoleLogging) Log.Info("Finished searching " + pakfile);
            if (searchedPaths.Count == 0) break;
        }
    }

    /// <summary>
    /// Extracts requested files to the given path.
    /// </summary>
    /// <param name="outputDirectory">The base file output directory.</param>
    /// <param name="missingFiles">An optional list to populate with any listed files that weren't found.</param>
    /// <returns>The number of extracted files.</returns>
    public int UnpackFilesTo(string outputDirectory, List<string>? missingFiles = null)
    {
        var unpackedCount = 0;

        var pak = new PakFile();
        foreach (var pakfile in EnumerateTempPaksWithSearchedFiles(pak)) {
            var threads = CalculateChunkCount(pak.Entries.Count);
            var chunks = PartitionPakEntryChunks(pak, threads, (start, end) => new ExtractionChunkContext(pak, start, end, outputDirectory));
            if (chunks.Count == 1) {
                HandleChunkUnpack(chunks[0]);
            } else {
                foreach (var chunk in chunks) {
                    var success = ThreadPool.QueueUserWorkItem(ExtractFileChunkSafe, chunk);
                    if (!success) {
                        throw new Exception("Failed to queue ThreadPool task");
                    }
                }

                while (chunks.Any(c => !c.finished)) Thread.Sleep(25);
            }

            UpdateSearchedPaths(searchedPaths, chunks, ref unpackedCount);
            if (EnableConsoleLogging) Log.Info("Finished unpacking " + pakfile);
            if (searchedPaths.Count == 0) break;
        }

        missingFiles?.AddRange(searchedPaths.Values);
        return unpackedCount;
    }

    /// <summary>
    /// Unpack requested files to the given path.
    /// </summary>
    /// <param name="outputDirectory">The base file output directory.</param>
    /// <param name="missingFiles">An optional list to populate with any listed files that weren't found.</param>
    /// <returns>The number of unpacked files.</returns>
    public async Task<int> UnpackFilesAsyncTo(string outputDirectory, List<string>? missingFiles = null)
    {
        var unpackedCount = 0;

        var pak = new PakFile();
        foreach (var pakfile in EnumerateTempPaksWithSearchedFiles(pak)) {
            var threads = CalculateChunkCount(pak.Entries.Count);
            var chunks = PartitionPakEntryChunks(pak, threads, (start, end) => new ExtractionChunkContext(pak, start, end, outputDirectory));
            foreach (var chunk in chunks) {
                var success = ThreadPool.QueueUserWorkItem(ExtractFileChunkSafe, chunk);
                if (!success) {
                    throw new Exception("Failed to queue ThreadPool task");
                }
            }

            while (chunks.Any(c => !c.finished)) await Task.Delay(25);

            UpdateSearchedPaths(searchedPaths, chunks, ref unpackedCount);
            if (EnableConsoleLogging) Log.Info("Finished unpacking " + pakfile);
            if (searchedPaths.Count == 0) break;
        }

        missingFiles?.AddRange(searchedPaths.Values);
        return unpackedCount;
    }

    protected static void UpdateSearchedPaths<TContext>(Dictionary<ulong, string> paths, List<TContext> chunks, ref int fileCount)
        where TContext : ChunkContextBase
    {
        foreach (var chunk in chunks) {
            fileCount += chunk.fileCount;
            foreach (var item in chunk.foundHashes) paths.Remove(item);
        }
    }

    protected IEnumerable<PakFile> EnumeratePaks(bool assignEntryPaths)
    {
        for (var i = PakFilePriority.Count - 1; i >= 0; i--) {
            var pakfile = PakFilePriority[i];
            var filesize = new FileInfo(pakfile).Length;
            if (filesize <= 16) continue;

            var pak = new PakFile();
            pak.filepath = pakfile;
            pak.ReadContents(pakfile, assignEntryPaths ? searchedPaths : null);

            if (pak.Entries.Count > 0) {
                yield return pak;
            }
        }
    }

    protected IEnumerable<string> EnumerateTempPaksWithSearchedFiles(PakFile pak)
    {
        for (var i = PakFilePriority.Count - 1; i >= 0; i--) {
            var pakfile = PakFilePriority[i];
            var filesize = new FileInfo(pakfile).Length;
            if (filesize <= 16) continue;

            pak.filepath = pakfile;
            pak.ReadContents(pakfile, searchedPaths);

            if (pak.Entries.Count > 0) {
                yield return pakfile;
            }
        }
    }

    private List<TChunkType> PartitionPakEntryChunks<TChunkType>(PakFile pak, int chunkCount, Func<int, int, TChunkType> factory)
        where TChunkType : ChunkContextBase
    {
        var chunks = new List<TChunkType>();
        var entryCount = pak.Entries.Count;
        for (int k = 0; k < chunkCount; k++) {
            var start = (int)Math.Floor(entryCount * ((float)k / chunkCount));
            var end = (int)Math.Floor(entryCount * ((float)(k + 1) / chunkCount));
            if (end == start) continue;
            // var ctx = new ExtractionChunkContext(pakfile, pak.Entries, start, end, outputDirectory);
            var ctx = factory.Invoke(start, end);
            chunks.Add(ctx);
        }

        return chunks;
    }

    protected record class ChunkContextBase(PakFile file, int startOffset, int endOffset)
    {
        public int fileCount;
        public volatile bool finished;
        public readonly List<ulong> foundHashes = new(endOffset - startOffset);
    }

    protected record class ExtractionChunkContext(PakFile file, int startOffset, int endOffset, string outputDir)
        : ChunkContextBase(file, startOffset, endOffset)
    {
    }

    protected void ExtractFileChunkSafe(object? context)
    {
        var ctx = (ExtractionChunkContext)context!;
        try {
            HandleChunkUnpack(ctx);
        } finally {
            ctx.finished = true;
        }
    }

    protected IEnumerable<(PakEntry entry, MemoryStream stream)> FindEntriesInChunk(ChunkContextBase ctx)
    {
        return ReadEntriesInChunk(ctx, GetToMemoryUnpacker());
    }

    protected void HandleChunkUnpack(ExtractionChunkContext ctx)
    {
        foreach (var (entry, stream) in ReadEntriesInChunk(ctx, UnpackEntryToFile)) {
            stream.Dispose();
        }
    }

    protected static Func<ChunkContextBase, string, MemoryStream> GetToMemoryUnpacker()
    {
        var stream = new MemoryStream();
        return (_, _) => {
            stream.Seek(0, SeekOrigin.Begin);
            stream.SetLength(0);
            return stream;
        };
    }

    protected static FileStream UnpackEntryToFile(ExtractionChunkContext ctx, string path)
    {
        var outputFile = Path.Combine(ctx.outputDir, path);
        Directory.CreateDirectory(Path.GetDirectoryName(outputFile)!);
        return File.Create(outputFile);
    }

    protected IEnumerable<(PakEntry entry, TStream stream)> ReadEntriesInChunk<TStream, TContextType>(TContextType ctx, Func<TContextType, string, TStream> writeStreamProvider)
        where TStream : Stream
        where TContextType : ChunkContextBase
    {
        var chunkId = ctx.endOffset == ctx.startOffset + 1 ? ctx.startOffset : ctx.startOffset / (ctx.endOffset - ctx.startOffset - 1);
        var hasMultipleChunks = (ctx.startOffset > 0 || ctx.endOffset < ctx.file.Entries.Count - 1);
        if (EnableConsoleLogging && hasMultipleChunks) {
            Log.Info($"Starting chunk {chunkId}: entries {ctx.startOffset} - {ctx.endOffset} for file {ctx.file.filepath}");
        }
        using var fs = File.OpenRead(ctx.file.filepath);
        for (int i = ctx.startOffset; i < ctx.endOffset; ++i) {
            var entry = ctx.file.Entries[i];
            var hash = entry.CombinedHash;
            if (!searchedPaths.TryGetValue(hash, out var path)) continue;
            if (entry.compressedSize > int.MaxValue || entry.decompressedSize > int.MaxValue) {
                throw new Exception("PAK entry size exceeds int.MaxValue - likely read error");
            }

            var outStream = writeStreamProvider.Invoke(ctx, path);
            PakFile.ReadEntry(entry, fs, outStream);
            ctx.foundHashes.Add(entry.CombinedHash);
            ctx.fileCount++;
            yield return (entry, outStream);
        }

        if (EnableConsoleLogging && hasMultipleChunks) Log.Info("Finished chunk " + chunkId);
    }
}
