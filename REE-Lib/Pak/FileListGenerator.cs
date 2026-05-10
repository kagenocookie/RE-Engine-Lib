namespace ReeLib.Pak;

using System.Buffers;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Text;
using ReeLib.Common;

public class FileListGenerator(string gameDirectory)
{
    public string GameDirectory { get; } = gameDirectory;

    public List<string> PakFiles { get; } = new();

    public string? PreviousListFile { get; set; }
    public string[] ReferenceListFiles { get; set; } = [];

    private string? PlatformBase { get; set; }

    private readonly Dictionary<ulong, string> _knownHashPaths = new();

    public GeneratorPhase Phase { get; set; }

    public float PhaseProgress { get; set; }

    public ScanFlags Flags { get; set; } = ScanFlags.Files|ScanFlags.Executable|ScanFlags.MaintainPreviousList;

    [Flags]
    public enum ScanFlags
    {
        Files = 1,
        Executable = 2,
        MaintainPreviousList = 1 << 30
    }

    public enum GeneratorPhase
    {
        CachingKnownPaths,
        ScanningExecutable,
        ScanningFiles,
        ProcessingPaths,
        Done,
    }

    private static string[] PlatformBases = [
        "natives/STM/",
        "natives/x64/",
        "natives/EGS/"
    ];

    private const int MaxExtensionLength = 16; // "jointlodgroup" = 13
    private const int MinPathLength = 10;

    private HashSet<string> otherFileListInternalPaths = new();

    public void HashKnownFilePaths(CachedMemoryPakReader reader)
    {
        Phase = GeneratorPhase.CachingKnownPaths;
        int count = ReferenceListFiles.Length + (PreviousListFile != null ? 1 : 0);
        int i = 0;
        foreach (var file in ReferenceListFiles.Prepend(PreviousListFile)) {
            if (!File.Exists(file)) continue;

            PhaseProgress = i++ / (float)count;

            using var stream = new StreamReader(File.OpenRead(file));
            var isCurrent = PreviousListFile == file;
            while (!stream.EndOfStream) {
                var line = stream.ReadLine();
                if (string.IsNullOrEmpty(line)) continue;

                var hash = PakUtils.GetFilepathHash(line);
                if (_knownHashPaths.TryAdd(hash, line)) {
                    if (isCurrent) {
                        reader.AddFiles(line);
                    } else {
                        otherFileListInternalPaths.Add(PathUtils.GetInternalFromNativePath(line));
                    }
                }
            }
        }
    }

    public void AutoScanPAKs()
    {
        PakFiles.AddRange(PakUtils.ScanPakFiles(GameDirectory));
    }

    private static readonly string[] FileSuffixes = ["", ".stm", ".x64"];

    private static readonly string[] FileSuffixesLocalized = [
        .. FileSuffixes,

        ".en",
        ".ar",
        ".de",
        ".es",
        ".es419",
        ".fr",
        ".hi",
        ".it",
        ".ja",
        ".ko",
        ".pl",
        ".ptbr",
        ".ru",
        ".th",
        ".zhcn",
        ".zhtw",

        ".x64.en",
        ".x64.ar",
        ".x64.de",
        ".x64.es",
        ".x64.es419",
        ".x64.fr",
        ".x64.hi",
        ".x64.it",
        ".x64.ja",
        ".x64.ko",
        ".x64.pl",
        ".x64.ptbr",
        ".x64.ru",
        ".x64.th",
        ".x64.zhcn",
        ".x64.zhtw",
    ];

    private static readonly string[] TexTypes = [
        "_acot", "_albd", "_nrmr", "_msr", "_mskm", "_scot", "_nrca", "_msk4", "_alb", "_nrma",
        "_nrm", "_albs", "_atos", "_faketex", "_dslut", "_msk3", "_emi", "_msk1", "_colormask.",
        "_selectionmask", "_hgt", "_nrrc", "_hdr", "_mask", "_msk", "_nrra", "_albm", "_albh",
        "_nrro", "_rocm", "_occ", "_lymo", "_alba", "_nrca", "_alp", "_nmr", "_rgh", "_met",
        "_iam", "_lut", "_fbi", "_add", "_emm", "_lym", "_cvt", "_vns", "_lin", "_pos", "_fur", "_im", "_disp"];

    private static readonly HashSet<string> IgnoredExtensions = ["json", "dll", "pdb", "ini", "cpp", "hpp", "h", "cs", "technology", "com", "com0C", "com0\\", "com0A", "fffff", "0"];
    private static readonly HashSet<uint> IgnoreExtHashes = IgnoredExtensions.Select(x => MurMur3HashUtils.GetHash(x)).ToHashSet();
    private static readonly string[] GuessDates = ["251111", "251112", "251121", "250925"];

    public List<string> Scan()
    {
        // create file reader
        var reader = new CachedMemoryPakReader() { IncludeUnknownFilePaths = true };
        reader.PakFilePriority.AddRange(PakFiles);
        HashKnownFilePaths(reader);
        reader.CacheEntries(true);
        var outputPaths = new List<string>(reader.CachedPaths);
        var unknownHashes = reader.UnknownPathHashes.ToHashSet();
        if (unknownHashes.Count == 0) {
            Log.Error("No unknown files found");
            return [];
        }
        var totalFilesCount = outputPaths.Count + unknownHashes.Count;
        var previouslyKnownFilesCount = outputPaths.Count;

        string[] sourceFileList = [];

        if (Flags.HasFlag(ScanFlags.MaintainPreviousList) && File.Exists(PreviousListFile)) {
            var lfw = new ListFileWrapper(PreviousListFile);
            sourceFileList = lfw.Files;
        }
        var knownHashes = sourceFileList.Select(x => PakUtils.GetFilepathHash(x)).ToHashSet();

        var rawPaths = new Dictionary<ulong, string>();
        if (Flags.HasFlag(ScanFlags.Executable)) {
            ScanExecutables(rawPaths);
        }
        if (Flags.HasFlag(ScanFlags.Files)) {
            ScanFiles(reader, totalFilesCount, rawPaths);
        }

        var platformBase = PlatformBase
            ?? PlatformBases.FirstOrDefault(pb => outputPaths.Any(op => op.StartsWith(pb, StringComparison.OrdinalIgnoreCase)));

        if (platformBase == null) {
            platformBase = "natives/STM/";
        }

        Phase = GeneratorPhase.ProcessingPaths;
        PhaseProgress = 0;
        var extVersions = new Dictionary<string, int>();
        foreach (var path in outputPaths.Concat(sourceFileList)) {
            var fmt = PathUtils.ParseFileFormatFull(path);
            if (fmt.version == -1 || extVersions.ContainsKey(fmt.extension)) continue;

            extVersions[fmt.extension] = fmt.version;
        }
        var versionPrefixes = new List<string>();
        versionPrefixes.Add("");
        versionPrefixes.AddRange(GuessDates);
        foreach (var (ext, num) in extVersions) {
            var numStr = num.ToString();
            if (numStr.Length >= 6 + 3) {
                var date = numStr[0..6];
                if (!versionPrefixes.Contains(date)) {
                    versionPrefixes.Add(date);
                }
            }
        }

        var unmatchedPaths = new List<string>();
        var unknownExtFiles = new Dictionary<string, List<string>>();

        var pathsProcessed = 0;
        foreach (var path in rawPaths.Values.Concat(otherFileListInternalPaths)) {
            PhaseProgress = (float)pathsProcessed++ / rawPaths.Count;

            var attemptBase = Path.Combine(platformBase, path);
            var ext = PathUtils.GetExtensionWithoutPeriod(attemptBase);
            if (string.IsNullOrEmpty(ext)) {
                continue;
            }

            if (!extVersions.TryGetValue(ext, out var version)) {
                bool canContinue = false;
                if (!unknownExtFiles.TryGetValue(ext, out var uknList)) {
                    // most file versions are small numbers so we can try these without it taking forever
                    // some version also follow a seemingly YYMMDD000 date-like structure so we can try all the previously known dates as well
                    // TODO try and also fetch versions directly instead of only guessing
                    if (ext.Length <= MaxExtensionLength) {
                        foreach (var prefix in versionPrefixes) {
                            for (int i = 0; i <= 999; i++) {
                                var str = prefix == "" ? $"{attemptBase}.{i}" : $"{attemptBase}.{prefix}{i:000}";
                                var hash = PakUtils.GetFilepathHash(str);
                                if (TryAddFoundFilePath(outputPaths, unknownHashes, KnownFileFormats.Unknown, str, hash)) {
                                    version = prefix == "" ? i : int.Parse($"{prefix}{i:000}");
                                    extVersions[ext] = version;
                                    knownHashes.Add(hash);
                                    canContinue = true;
                                    Log.Info($"Found new file extension version: {ext}.{version}");
                                    break;
                                }
                            }
                            if (canContinue) break;
                        }
                    }
                    if (!canContinue) {
                        Log.Warn($"Unknown version for file extension {ext} (file {path})");
                        unknownExtFiles[ext] = uknList = new ();
                    }
                }
                if (uknList != null) {
                    uknList.Add(path);
                    continue;
                }
            }
            var versionExt = "." + version;

            var format = FileFormatExtensions.ExtensionHashToEnum(MurMur3HashUtils.GetHash(ext));


            var suffixes = IsLocalizableFormat(format) ? FileSuffixesLocalized : FileSuffixes;
            var foundNewFile = false;
            var matchesExisting = false;
            foreach (var suffix in suffixes) {
                var attempt = attemptBase + versionExt + suffix;
                var attemptHash = PakUtils.GetFilepathHash(attempt);
                if (knownHashes.Contains(attemptHash)) {
                    matchesExisting = true;
                } else {
                    foundNewFile |= TryAddFoundFilePath(outputPaths, unknownHashes, format, attempt, attemptHash);
                }

                if (format == KnownFileFormats.Texture) {
                    var lastSub = attempt.LastIndexOf('_');
                    if (lastSub != -1) {
                        var withoutType = attempt.Substring(0, lastSub);
                        foreach (var type in TexTypes) {
                            attempt = $"{Path.ChangeExtension(attemptBase, null)}{type}.{ext}{versionExt}{suffix}";
                            attemptHash = PakUtils.GetFilepathHash(attempt);
                            matchesExisting |= knownHashes.Contains(attemptHash);
                            if (TryAddFoundFilePath(outputPaths, unknownHashes, format, attempt, attemptHash)) {
                                foundNewFile = true;
                            }
                        }
                    }
                }
            }

            if (!foundNewFile && !matchesExisting) {
                unmatchedPaths.Add(path);
            }
        }
        var newFiles = outputPaths.Count - previouslyKnownFilesCount;
        Log.Info($"Found {newFiles} new file paths");
        if (newFiles > 0 && previouslyKnownFilesCount > 0) {
            Log.Info(string.Join("\n", outputPaths[previouslyKnownFilesCount..]));
        }
        if (sourceFileList.Length > 0) {
            outputPaths.AddRange(sourceFileList);
            outputPaths = outputPaths.Distinct().ToList();
        }
        if (unmatchedPaths.Count > 0) {
            // ignore missing PFBs here cause there's way too many of them and they're mostly expected to be missing
            Log.Info("Not found paths:\n\n" + string.Join("\n", unmatchedPaths.Where(p => !p.EndsWith(".pfb"))));
        }
        if (unknownExtFiles.Count > 0) {
            Log.Info("Potentially unknown extensions:\n\n" + string.Join("\n\n", unknownExtFiles.Select(kv => $"  {kv.Key}:\n{string.Join("\n", kv.Value)}")));
        }
        var newKnownFiles = totalFilesCount - unknownHashes.Count;
        Log.Info($"Resolved file paths: {newKnownFiles} / {totalFilesCount} ({((float)newKnownFiles / totalFilesCount * 100):0.00}%)");
        outputPaths.Sort(StringComparer.OrdinalIgnoreCase);
        Phase = GeneratorPhase.Done;
        return outputPaths;
    }

    private static bool TryAddFoundFilePath(List<string> outputPaths, HashSet<ulong> unknownHashes, KnownFileFormats format, string attempt, ulong attemptHash)
    {
        if (!unknownHashes.Remove(attemptHash)) return false;

        outputPaths.Add(attempt);

        if (format is KnownFileFormats.Texture or KnownFileFormats.Mesh or KnownFileFormats.Movie or
            KnownFileFormats.SoundBank or KnownFileFormats.SoundPackage or KnownFileFormats.SoundVoxel or KnownFileFormats.SoundStreamingLQG or
            KnownFileFormats.VibrationSource or KnownFileFormats.WwiseStreamingGeometry or KnownFileFormats.Unknown) {
            // try streaming/
            var streaming = PathUtils.GetStreamingNativePath(attempt);
            var streamingHash = PakUtils.GetFilepathHash(streaming);
            if (unknownHashes.Remove(streamingHash)) {
                outputPaths.Add(streaming);
            }
        }

        return true;
    }

    private int ScanFiles(CachedMemoryPakReader reader, int totalFiles, Dictionary<ulong, string> rawPaths)
    {
        Phase = GeneratorPhase.ScanningFiles;
        PhaseProgress = 0;
        int filesProcessed = 0;
        var files = reader.ReadAllFiles(p => {
            PhaseProgress = (float)filesProcessed++ / totalFiles;
            return string.IsNullOrEmpty(p.path) || !IsPathlessFormat(PathUtils.ParseFileFormat(p.path).format);
        });
        foreach (var (file, entry) in files) {
            if (!string.IsNullOrEmpty(entry.path)) {
                var format = PathUtils.ParseFileFormat(entry.path).format;
                if (IsPathlessFormat(format)) {
                    continue;
                }
            }

            foreach (var path in ExtractPaths(Encoding.Unicode, file, null)) {
                rawPaths.TryAdd(PakUtils.GetFilepathHash(path), path);
            }
        }

        return filesProcessed;
    }

    private void ScanExecutables(Dictionary<ulong, string> rawPathLikes)
    {
        Phase = GeneratorPhase.ScanningExecutable;
        PhaseProgress = -1;
        try {
            foreach (var exe in Directory.EnumerateFiles(GameDirectory, "*.exe")) {
                var name = Path.GetFileName(exe);
                if (name == "CrashReport.exe" || name == "InstallerMessage.exe") continue;
                try {
                    using var fs = File.OpenRead(exe);
                    foreach (var str in ExtractPaths(Encoding.UTF8, fs, exe)) {
                        rawPathLikes.TryAdd(PakUtils.GetFilepathHash(str), str);
                    }

                    fs.Seek(0, SeekOrigin.Begin);
                    foreach (var str in ExtractPaths(Encoding.Unicode, fs, exe)) {
                        rawPathLikes.TryAdd(PakUtils.GetFilepathHash(str), str);
                    }
                } catch (Exception ex) {
                    Log.Error($"Failed to analyze exe '{exe}': {ex.Message}");
                }
            }
        } catch (Exception) {
            // ignore
            Log.Error("Failed to check through executables");
        }
    }


    private static readonly SearchValues<char> PathyChars = SearchValues.Create("abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789 _-.:@\\/");

    private static bool IsPathyCharacter(char ch) => PathyChars.Contains(ch);

    private static bool IsFilePath(string str)
    {
        if (str.Length < MinPathLength) return false;

        var hasSlash = false;
        foreach (var ch in str) {
            if (!IsPathyCharacter(ch)) return false;
            hasSlash |= ch == '/';
        }
        if (!hasSlash) return false;

        var ext = PathUtils.GetExtensionWithoutPeriod(str.AsSpan());
        if (ext.IsEmpty || ext.Length > MaxExtensionLength || int.TryParse(ext, out _) || IgnoreExtHashes.Contains(MurMur3HashUtils.GetHash(ext))) return false;
        return true;
    }

    private static bool IsLocalizableFormat(KnownFileFormats format) =>
        format is KnownFileFormats.SoundBank or KnownFileFormats.SoundPackage or KnownFileFormats.Texture or KnownFileFormats.MotionList or KnownFileFormats.Movie;

    private static bool IsPathlessFormat(KnownFileFormats format)
        => format is KnownFileFormats.Texture or KnownFileFormats.Mesh or KnownFileFormats.Movie or
        KnownFileFormats.SoundPackage or KnownFileFormats.SoundVoxel or KnownFileFormats.SoundBank or
        KnownFileFormats.MasterMaterial or KnownFileFormats.Shader or ReeLib.KnownFileFormats.VfxShader or
        KnownFileFormats.Probes or KnownFileFormats.SparseShadowTree or
        KnownFileFormats.Strands or KnownFileFormats.CollisionMesh;

    private static bool _ignoreStringsLib = false;
    private static IEnumerable<string> ExtractPaths(Encoding encoding, Stream stream, string? filepath)
    {
        if (stream.Length < MinPathLength) yield break;

        uint magic = 0;
        stream.ReadExactly(MemoryUtils.StructureAsBytes(ref magic));
        var fmt = CachedMemoryPakReader.GuessFileFormatFromMagic(magic);
        if (fmt != KnownFileFormats.Unknown && IsPathlessFormat(fmt)) {
            yield break;
        }

        stream.ReadExactly(MemoryUtils.StructureAsBytes(ref magic));
        fmt = CachedMemoryPakReader.GuessFileFormatFromMagic(magic);
        if (fmt != KnownFileFormats.Unknown && IsPathlessFormat(fmt)) {
            yield break;
        }

        if (RuntimeInformation.IsOSPlatform(OSPlatform.Linux) && !_ignoreStringsLib && File.Exists("/usr/bin/strings")) {
            // prefer just using strings cause it's faster (sorry not sorry Windows)
            Process? proc = null;
            try {
                var info = new ProcessStartInfo("/usr/bin/strings") {
                    UseShellExecute = false,
                    RedirectStandardOutput = true,
                    RedirectStandardInput = filepath == null,
                };
                info.ArgumentList.Add("-e");
                info.ArgumentList.Add(encoding == Encoding.Unicode ? "l" : "S");
                // it's still slightly faster to let it read the file on its own so add a separate path here for that case
                if (filepath != null) {
                    info.ArgumentList.Add(filepath);
                }
                info.StandardOutputEncoding = Encoding.UTF8;
                proc = Process.Start(info);
            } catch (Exception e) {
                Log.Error("Failed to /usr/bin/strings, falling back to manual scan: " + e.Message);
                _ignoreStringsLib = true;
            }

            if (proc != null) {
                if (filepath == null) {
                    stream.CopyToAsync(proc.StandardInput.BaseStream).ContinueWith(_ => proc.StandardInput.Close());
                }
                while (!proc.StandardOutput.EndOfStream) {
                    var line = proc.StandardOutput.ReadLine();
                    if (line != null && IsFilePath(line)) {
                        yield return CleanFilePath(line);
                        // sometimes we end up grabbing paths like "aConfig/...", try to handle those as well
                        if (char.IsLower(line[0]) && char.IsUpper(line[1])) {
                            yield return line.Substring(1);
                        }
                    }
                }
                yield break;
            }
        }

        // something like half as fast as strings, not _too_ horrible either
        foreach (var item in ExtractStringsManually(stream, encoding)) {
            yield return item;
        }
    }

    private static string CleanFilePath(string path)
    {
        var colon = path.IndexOf(':');
        if (colon != -1) {
            path = path.Substring(colon + 1);
        }
        if (path.StartsWith('@')) {
            return path.Substring(1);
        } else {
            return path;
        }
    }

    private static List<string> ExtractStringsManually(Stream stream, Encoding encoding)
    {
        var decoder = encoding.GetDecoder();
        var maxByteCount = encoding.GetMaxByteCount(8) - encoding.GetMaxByteCount(0); // negate null terminator
        var maxCharCount = encoding.GetMaxCharCount(maxByteCount);
        Span<byte> bytes = stackalloc byte[maxByteCount];
        Span<char> chars = stackalloc char[maxCharCount];
        var maxSlice = maxCharCount - 1;

        var sb = new StringBuilder();
        var strings = new List<string>();
        while (stream.Position < stream.Length) {
            var readBytes = stream.Read(bytes);
            var chCount = decoder.GetChars(bytes, chars, true);
            var validChars = chars.Slice(0, chCount).IndexOfAnyExcept(PathyChars);

            if (validChars == -1) {
                // chars includes includes a null terminator, we don't want it
                sb.Append(chars.Slice(0, maxSlice));
            } else {
                if (validChars > 0) {
                    sb.Append(chars.Slice(0, validChars));
                }

                if (validChars < chCount) {
                    if (sb.Length >= MinPathLength) {
                        var str = sb.ToString();
                        if (IsFilePath(str)) {
                            strings.Add(CleanFilePath(str));
                            // sometimes we end up grabbing paths like "aConfig/...", try to handle those as well
                            if (char.IsLower(str[0]) && char.IsUpper(str[1])) {
                                strings.Add(str.Substring(1));
                            }
                        }
                    }
                    sb.Clear();

                    var byteCount = validChars == -1 ? maxByteCount : encoding.GetByteCount(chars.Slice(0, validChars));
                    var zeros = bytes.IndexOfAnyExcept((byte)0);
                    var decodeCorrection = -readBytes + Math.Max(1, byteCount + zeros);
                    if (decodeCorrection != 0) stream.Seek(decodeCorrection, SeekOrigin.Current);
                }
            }
        }

        // just in case we still had an unfinished string left over, see if it's a path
        if (sb.Length >= MinPathLength) {
            var str = sb.ToString();
            if (IsFilePath(str)) {
                strings.Add(CleanFilePath(str));
            }
        }

        return strings;
    }
}