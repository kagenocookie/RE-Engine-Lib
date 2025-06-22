using System.Buffers;
using System.Numerics;
using System.Runtime.CompilerServices;

namespace RszTool.Pak
{
	public enum CompressionType
	{
        None = 0,
        Deflate = 1,
        Zstd = 2,
	}

    public enum EncryptionType
    {
        None = 0,
        Type1 = 1,
        Type2 = 2,
        Type3 = 3,
        Type4 = 4,
        Invalid = 5,
    }

    public struct Header
    {
        public uint magic;
        public byte majorVersion;
        public byte minorVersion;
        public short featureFlags;
        public int fileCount;
        public uint fingerprint;
    }

    public class PakEntry
    {
        public uint hashLowercase;
        public uint hashUppercase;
        public long offset;
        public long compressedSize;
        public long decompressedSize;
        public long checksum;
        public CompressionType compression;
        public EncryptionType encryption;

        public ulong CombinedHash => (ulong)hashUppercase << 32 | hashLowercase;

        public void ReadVer2(FileHandler handler)
        {
            offset = handler.ReadInt64();
            compressedSize = handler.ReadInt64();
            hashUppercase = handler.ReadUInt();
            hashLowercase = handler.ReadUInt();
        }

        public void ReadVer4(FileHandler handler)
        {
            hashLowercase = handler.ReadUInt();
            hashUppercase = handler.ReadUInt();
            offset = handler.ReadInt64();
            compressedSize = handler.ReadInt64();
            decompressedSize = handler.ReadInt64();
            var attributes = handler.ReadInt64();
            checksum = handler.ReadInt64();

            compression = (CompressionType)(attributes & 0xf);
            encryption = (EncryptionType)((attributes & 0x00ff0000) >> 16);

            if (compression != CompressionType.None && compression != CompressionType.Deflate && compression != CompressionType.Zstd)
            {
                throw new NotImplementedException("Unsupported PAK entry compression type " + compression);
            }
        }
    }

    internal static class Encryption
    {
        // encryption keys and base logic by Ekey/REE.PAK.Tool
#if NET5_0_OR_GREATER
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static BigInteger UnsignedBigInteger(Span<byte> bytes)
        {
            return new BigInteger(bytes, true);
        }
#else
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static BigInteger UnsignedBigInteger(Span<byte> bytes)
        {
            return UnsignedBigInteger(bytes.ToArray());
        }

        private static BigInteger UnsignedBigInteger(byte[] bytes)
        {
            Array.Resize(ref bytes, bytes.Length + 1);
            return new BigInteger(bytes);
        }
#endif

        private static readonly BigInteger keyModulus = UnsignedBigInteger([
            0x7D, 0x0B, 0xF8, 0xC1, 0x7C, 0x23, 0xFD, 0x3B, 0xD4, 0x75, 0x16, 0xD2, 0x33, 0x21, 0xD8, 0x10,
            0x71, 0xF9, 0x7C, 0xD1, 0x34, 0x93, 0xBA, 0x77, 0x26, 0xFC, 0xAB, 0x2C, 0xEE, 0xDA, 0xD9, 0x1C,
            0x89, 0xE7, 0x29, 0x7B, 0xDD, 0x8A, 0xAE, 0x50, 0x39, 0xB6, 0x01, 0x6D, 0x21, 0x89, 0x5D, 0xA5,
            0xA1, 0x3E, 0xA2, 0xC0, 0x8C, 0x93, 0x13, 0x36, 0x65, 0xEB, 0xE8, 0xDF, 0x06, 0x17, 0x67, 0x96,
            0x06, 0x2B, 0xAC, 0x23, 0xED, 0x8C, 0xB7, 0x8B, 0x90, 0xAD, 0xEA, 0x71, 0xC4, 0x40, 0x44, 0x9D,
            0x1C, 0x7B, 0xBA, 0xC4, 0xB6, 0x2D, 0xD6, 0xD2, 0x4B, 0x62, 0xD6, 0x26, 0xFC, 0x74, 0x20, 0x07,
            0xEC, 0xE3, 0x59, 0x9A, 0xE6, 0xAF, 0xB9, 0xA8, 0x35, 0x8B, 0xE0, 0xE8, 0xD3, 0xCD, 0x45, 0x65,
            0xB0, 0x91, 0xC4, 0x95, 0x1B, 0xF3, 0x23, 0x1E, 0xC6, 0x71, 0xCF, 0x3E, 0x35, 0x2D, 0x6B, 0xE3,
        ]);

        private static readonly BigInteger keyExponent = new BigInteger([
            0x01, 0x00, 0x01, 0x00
        ]);

        private static readonly BigInteger resourceModulus = UnsignedBigInteger([
            0x13, 0xD7, 0x9C, 0x89, 0x88, 0x91, 0x48, 0x10, 0xD7, 0xAA, 0x78, 0xAE, 0xF8, 0x59, 0xDF, 0x7D,
            0x3C, 0x43, 0xA0, 0xD0, 0xBB, 0x36, 0x77, 0xB5, 0xF0, 0x5C, 0x02, 0xAF, 0x65, 0xD8, 0x77, 0x03,
        ]);

        private static readonly BigInteger resourceExponent = UnsignedBigInteger([
            0xC0, 0xC2, 0x77, 0x1F, 0x5B, 0x34, 0x6A, 0x01, 0xC7, 0xD4, 0xD7, 0x85, 0x2E, 0x42, 0x2B, 0x3B,
            0x16, 0x3A, 0x17, 0x13, 0x16, 0xEA, 0x83, 0x30, 0x30, 0xDF, 0x3F, 0xF4, 0x25, 0x93, 0x20, 0x01,
        ]);

        public static void DecryptKey(Span<byte> key)
        {
            BigInteger m_EncryptedKey = UnsignedBigInteger(key);
            BigInteger result = BigInteger.ModPow(m_EncryptedKey, keyExponent, keyModulus);
            key.Clear();
#if NET5_0_OR_GREATER
            result.TryWriteBytes(key, out _);
#else
            result.ToByteArray().AsSpan().CopyTo(key);
#endif
        }

#if NET5_0_OR_GREATER
        [SkipLocalsInit]
#endif
        public unsafe static byte[] DecryptResource(byte[] compressedBytes, int size)
        {
            using var reader = new MemoryStream(compressedBytes, 0, size);
            int blockCount = (compressedBytes.Length - 8) / 128;
            var byteSize = blockCount * 8;

            Span<byte> buffer1 = stackalloc byte[8];
            Span<byte> buffer2 = stackalloc byte[8];
            reader.Read(buffer1);
            var decryptedSize = BitConverter.ToInt64(buffer1);

            var outBytes = ArrayPool<byte>.Shared.Rent((int)(decryptedSize + 1));
            var outSpan = outBytes.AsSpan();
            outSpan.Clear();

            for (int offset = 0; offset < byteSize; offset += 8)
            {
                reader.Read(buffer1);
                reader.Read(buffer2);

#if NET5_0_OR_GREATER
                var key = new BigInteger(buffer1);
                var data = new BigInteger(buffer2);
#else
                var key = new BigInteger(buffer1.ToArray());
                var data = new BigInteger(buffer2.ToArray());
#endif

                var mod = BigInteger.ModPow(key, resourceExponent, resourceModulus);
                var result = BigInteger.Divide(data, mod);

#if NET5_0_OR_GREATER
                result.TryWriteBytes(outSpan.Slice(offset), out _);
#else
                var resultBytes = result.ToByteArray();
                Array.Copy(resultBytes, 0, outBytes, offset, resultBytes.Length);
#endif
            }

            return outBytes;
        }
    }

    internal static class Compression
    {
        public static void DecompressZstd(byte[] compressedData, int size, Stream outputStream)
        {
            using var memoryStream = new MemoryStream(compressedData, 0, size);
            using var stream = new ZstdSharp.DecompressionStream(memoryStream);
            stream.CopyTo(outputStream);
        }

        public static void DecompressDeflate(byte[] compressedData, int size, Stream outputStream)
        {
            using var memoryStream = new MemoryStream(compressedData, 0, size);
            using var stream = new System.IO.Compression.DeflateStream(memoryStream, System.IO.Compression.CompressionMode.Decompress);
            stream.CopyTo(outputStream);
        }
    }
}


namespace RszTool
{
    using System;
    using System.Buffers;
    using System.Text.RegularExpressions;
    using System.Threading.Tasks;
    using RszTool.Common;
    using RszTool.Pak;

    public class PakReader
    {
        /// <summary>
        /// List of PAK files to extract from. Should be in chronological order,
        /// </summary>
        public List<string> PakFilePriority { get; set; } = new();

        /// <summary>
        /// The upper limit for many threads should be used queued up for file extraction. 1 = disable multithreading.<br/>
        /// The actual used thread count will be determined by the .NET ThreadPool.
        /// </summary>
        public int MaxThreads { get; set; } = 32;

        /// <summary>
        /// A regex to use for filtering which files should get extracted.
        /// </summary>
        public Regex? Filter { get; set; }

        private readonly Dictionary<ulong, string> HashedPaths = new();

        /// <summary>
        /// Scans a directory for PAK files that could be unpacked.
        /// </summary>
        /// <param name="directory"></param>
        /// <param name="ignoreModPaks">Whether modded PAK files should be ignored (based on FMM placeholder PAKs)</param>
        /// <returns></returns>
        public static List<string> ScanPakFiles(string directory, bool ignoreModPaks = true)
        {
            var list = new List<string>();

            foreach (var pak in Directory.EnumerateFiles(directory, "*.pak", SearchOption.TopDirectoryOnly)) {
                var path = Path.Combine(directory, pak);
                // size 16 = FMM placeholder paks, after this is mods which we possibly don't want
                if (new FileInfo(path).Length <= 16) {
                    if (ignoreModPaks) break;
                    continue;
                }

                list.Add(NormalizePath(path));
            }
            // new dlc structure
            var dlcPath = Path.Combine(directory, "dlc");
            if (Directory.Exists(dlcPath)) {
                foreach (var dlcpak in Directory.EnumerateFiles(dlcPath, "*.pak", SearchOption.TopDirectoryOnly)) {
                    list.Add(NormalizePath(Path.Combine(dlcPath, dlcpak)));
                }
            }
            // old dlc structure
            var dlcDirs = Directory.EnumerateDirectories(directory);
            foreach (var subdir in dlcDirs) {
                if (!int.TryParse(subdir, out _)) {
                    continue;
                }

                var dlcPakFile = Path.Combine(directory, subdir, "re_dlc_000.pak");
                if (File.Exists(dlcPakFile)) {
                    list.Add(dlcPakFile);
                }
            }

            return list;
        }

        public void AddFilesFromListFile(string fileList)
        {
            using var f = new StreamReader(File.OpenRead(fileList));
            while (!f.EndOfStream) {
                var line = f.ReadLine();
                if (!string.IsNullOrWhiteSpace(line)) {
                    var norm = NormalizePath(line);
                    var hashLower = MurMur3HashUtils.GetHash(norm.ToLowerInvariant());
                    var hashUpper = MurMur3HashUtils.GetHash(norm.ToUpperInvariant());
                    var combinedHash = (ulong)hashUpper << 32 | hashLower;
                    HashedPaths.Add(combinedHash, line);
                }
            }
        }

        public void AddFiles(params string[] files) => AddFiles((IEnumerable<string>)files);

        public void AddFiles(IEnumerable<string> files)
        {
            foreach (var file in files) {
                var norm = NormalizePath(file);
                var hashLower = MurMur3HashUtils.GetHash(norm.ToLowerInvariant());
                var hashUpper = MurMur3HashUtils.GetHash(norm.ToUpperInvariant());
                var combinedHash = (ulong)hashUpper << 32 | hashLower;
                HashedPaths.Add(combinedHash, file);
            }
        }

        private static string NormalizePath(string path)
        {
            return path.Replace('\\', '/');
        }

        private int CalculateChunkCount(int entryCount)
        {
            // reduce the number of threads to have at least 64 entries per thread, otherwise it's meaningless
            return MaxThreads <= 1 ? 1 : Math.Max(Math.Min(MaxThreads, entryCount / 64), 1);
        }

        /// <summary>
        /// Extract requested files to the given path.
        /// </summary>
        /// <param name="outputDirectory">The base file output directory.</param>
        /// <param name="missingFiles">An optional list to populate with any listed files that weren't found.</param>
        /// <returns>The number of extracted files.</returns>
        public int UnpackFilesTo(string outputDirectory, List<string>? missingFiles = null)
        {
            var unpackedCount = 0;

            var pak = new PakFile();
            foreach (var pakfile in EnumeratePakFiles(pak))
            {
                var threads = CalculateChunkCount(pak.Entries.Count);
                var chunks = PartitionPakEntryChunks(pak, outputDirectory, pakfile, threads);
                if (chunks.Count == 1)
                {
                    ExtractFileChunk(chunks[0]);
                }
                else
                {
                    foreach (var chunk in chunks)
                    {
                        var success = ThreadPool.QueueUserWorkItem(ExtractFileChunkSafe, chunk);
                        if (!success)
                        {
                            throw new Exception("Failed to queue ThreadPool task");
                        }
                    }

                    while (chunks.Any(c => !c.finished)) Thread.Sleep(25);
                }

                foreach (var chunk in chunks)
                {
                    unpackedCount += chunk.unpackedCount;
                    foreach (var item in chunk.extractedHashes!) HashedPaths.Remove(item);
                }
                Console.WriteLine("Finished unpacking " + pakfile);
                if (HashedPaths.Count == 0) break;
            }

            if (missingFiles != null)
            {
                foreach (var path in HashedPaths.Values)
                {
                    missingFiles.Add(path);
                }
            }

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
            foreach (var pakfile in EnumeratePakFiles(pak))
            {
                var threads = CalculateChunkCount(pak.Entries.Count);
                var chunks = PartitionPakEntryChunks(pak, outputDirectory, pakfile, threads);
                foreach (var chunk in chunks)
                {
                    var success = ThreadPool.QueueUserWorkItem(ExtractFileChunkSafe, chunk);
                    if (!success)
                    {
                        throw new Exception("Failed to queue ThreadPool task");
                    }
                }

                while (chunks.Any(c => !c.finished)) await Task.Delay(25);

                foreach (var chunk in chunks)
                {
                    unpackedCount += chunk.unpackedCount;
                    foreach (var item in chunk.extractedHashes!) HashedPaths.Remove(item);
                }
                Console.WriteLine("Finished unpacking " + pakfile);
                if (HashedPaths.Count == 0) break;
            }

            if (missingFiles != null)
            {
                foreach (var path in HashedPaths.Values)
                {
                    missingFiles.Add(path);
                }
            }

            return unpackedCount;
        }

        private IEnumerable<string> EnumeratePakFiles(PakFile pak)
        {
            for (var i = PakFilePriority.Count - 1; i >= 0; i--)
            {
                var pakfile = PakFilePriority[i];
                var filesize = new FileInfo(pakfile).Length;
                if (filesize <= 16) continue;

                pak.ReadContents(pakfile, HashedPaths.Keys);

                if (pak.Entries.Count > 0)
                {
                    yield return pakfile;
                }
            }
        }

        private List<ExtractionChunkContext> PartitionPakEntryChunks(PakFile pak, string outputDirectory, string pakfile, int count)
        {
            var chunks = new List<ExtractionChunkContext>();
            var entryCount = pak.Entries.Count;
            for (int k = 0; k < count; k++)
            {
                var start = (int)Math.Floor(entryCount * ((float)k / count));
                var end = (int)Math.Floor(entryCount * ((float)(k + 1) / count));
                if (end == start) continue;
                var ctx = new ExtractionChunkContext(pakfile, pak.Entries, start, end, outputDirectory);
                ctx.extractedHashes = new(end - start);
                chunks.Add(ctx);
            }

            return chunks;
        }

        private sealed record class ExtractionChunkContext(string filepath, List<PakEntry> entries, int startOffset, int endOffset, string outputDir)
        {
            public int unpackedCount;
            public List<ulong>? extractedHashes;
            public volatile bool finished;
        }

        private void ExtractFileChunkSafe(object? context)
        {
            var ctx = (ExtractionChunkContext)context!;
            try
            {
                ExtractFileChunk(ctx);
            }
            finally
            {
                ctx.finished = true;
            }
        }

        private void ExtractFileChunk(ExtractionChunkContext ctx)
        {
            var chunkId = ctx.startOffset / (ctx.endOffset - ctx.startOffset - 1);
            Console.WriteLine($"Starting extraction for chunk {chunkId}: entries {ctx.startOffset} - {ctx.endOffset} for file {ctx.filepath}");
            var outputRoot = ctx.outputDir;
            using var fs = File.OpenRead(ctx.filepath);
            for (int i = ctx.startOffset; i < ctx.endOffset; ++i)
            {
                var entry = ctx.entries[i];
                var hash = entry.CombinedHash;
                if (!HashedPaths.TryGetValue(hash, out var path)) continue;
                if (Filter != null && !Filter.IsMatch(path)) continue;
                if (entry.compressedSize > int.MaxValue || entry.decompressedSize > int.MaxValue)
                {
                    throw new Exception("PAK entry size exceeds int.MaxValue - likely read error");
                }

                var outputFile = Path.Combine(outputRoot, path);
                Directory.CreateDirectory(Path.GetDirectoryName(outputFile)!);
                fs.Seek(entry.offset, SeekOrigin.Begin);
                using var outfs = File.OpenWrite(outputFile);
                if (entry.compression == CompressionType.None)
                {
                    var size = (int)entry.decompressedSize;
                    var bytes = ArrayPool<byte>.Shared.Rent(size);
                    fs.Read(bytes, 0, size);
                    outfs.Write(bytes, 0, size);
                    ArrayPool<byte>.Shared.Return(bytes);
                }
                else
                {
                    var size = (int)entry.compressedSize;
                    var bytes = ArrayPool<byte>.Shared.Rent(size);
                    fs.Read(bytes, 0, size);

                    if (entry.encryption != EncryptionType.None)
                    {
                        var decrypted = Encryption.DecryptResource(bytes, size);
                        ArrayPool<byte>.Shared.Return(bytes);
                        bytes = decrypted;
                    }

                    switch (entry.compression)
                    {
                        case CompressionType.Deflate:
                            Compression.DecompressDeflate(bytes, size, outfs);
                            break;
                        case CompressionType.Zstd:
                            Compression.DecompressZstd(bytes, size, outfs);
                            break;
                        default:
                            outfs.Write(bytes, 0, size);
                            break;
                    }

                    ArrayPool<byte>.Shared.Return(bytes);
                }
                ctx.unpackedCount++;
                ctx.extractedHashes?.Add(hash);
            }

            Console.WriteLine("Finished chunk " + chunkId);
        }
    }

    public class PakFile
    {
        public Pak.Header Header;
        public readonly List<PakEntry> Entries = new();

        private static readonly HashSet<(byte, byte)> supportedVersions = new()
        {
            (4, 0),
            (4, 1),
            (2, 0)
        };

        public void ReadContents(string filename, ICollection<ulong>? allowedHashes = null)
        {
            using var fs = File.OpenRead(filename);
            ReadContents(fs, allowedHashes);
            fs.Close();
        }
        public void ReadContents(Stream stream, ICollection<ulong>? allowedHashes = null)
        {
            stream.Read(MemoryUtils.StructureAsBytes(ref Header));

            if (Header.magic != 0x414B504B)
            {
                throw new InvalidDataException("File is not a valid PAK file");
            }

            if (!supportedVersions.Contains((Header.majorVersion, Header.minorVersion)))
            {
                throw new InvalidDataException($"Unsupported PAK version {Header.majorVersion}.{Header.minorVersion}");
            }

            if (Header.featureFlags != 0 && Header.featureFlags != 8 && Header.featureFlags != 24)
            {
                throw new InvalidDataException($"Unsupported PAK encryption type {Header.featureFlags}");
            }

            Entries.Clear();
            if (Header.fileCount == 0) return;

            var entrySize = Header.majorVersion switch
            {
                4 => 48,
                2 => 24,
                _ => throw new Exception()
            };
            var entryListDataSize = Header.fileCount * entrySize;
            var entryTable = ArrayPool<byte>.Shared.Rent(entryListDataSize);
            stream.Read(entryTable, 0, entryListDataSize);

            if ((Header.featureFlags & 16) != 0)
            {
                stream.Seek(4, SeekOrigin.Current);
            }

            if (Header.featureFlags != 0)
            {
                byte[] key = ArrayPool<byte>.Shared.Rent(128);
                stream.Read(key);
                DecryptPakEntryData(entryTable, key);
                ArrayPool<byte>.Shared.Return(key);
            }

            using var reader = new MemoryStream(entryTable);
            var entryHandler = new FileHandler(reader);
            if (Header.majorVersion == 4)
            {
                for (int i = 0; i < Header.fileCount; ++i)
                {
                    var entry = new PakEntry();
                    entry.ReadVer4(entryHandler);
                    if (allowedHashes != null && !allowedHashes.Contains(entry.CombinedHash))
                        continue;
                    Entries.Add(entry);
                }
            }
            else if (Header.majorVersion == 2)
            {
                for (int i = 0; i < Header.fileCount; ++i)
                {
                    var entry = new PakEntry();
                    entry.ReadVer2(entryHandler);
                    if (allowedHashes != null && !allowedHashes.Contains(entry.CombinedHash))
                        continue;
                    Entries.Add(entry);
                }
            }

            ArrayPool<byte>.Shared.Return(entryTable);
        }

        private static void DecryptPakEntryData(byte[] buffer, Span<byte> key)
        {
            Encryption.DecryptKey(key);
            if (key.Length > 0)
            {
                var size = buffer.Length;
                for (int i = 0; i < size; i++)
                {
                    buffer[i] ^= (byte)(i + key[i % 32] * key[i % 29]);
                }
            }
        }
    }
}