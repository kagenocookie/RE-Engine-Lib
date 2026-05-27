namespace ReeLib.Pak;

using System.Buffers;
using System.Globalization;
using System.Text;
using ReeLib.Common;

public class FileListGenerator(string gameDirectory, PlatformIdentifier platform)
{
    public string GameDirectory { get; } = gameDirectory;

    public List<string> PakFiles { get; } = new();

    public string? PreviousListFile { get; set; }
    public string[] ReferenceListFiles { get; set; } = [];

    private readonly Dictionary<ulong, string> _knownHashPaths = new();

    public GeneratorPhase Phase { get; set; }

    public float PhaseProgress { get; set; }

    public ScanFlags Flags { get; set; } = ScanFlags.Files|ScanFlags.Executable|ScanFlags.MaintainPreviousList;

    [Flags]
    public enum ScanFlags
    {
        Files = 1,
        Executable = 2,
        BruteforceExtensions = 4,
        UpdateExistingListCasing = 8,
        MaintainPreviousList = 1 << 30
    }

    public enum GeneratorPhase
    {
        CachingKnownPaths,
        ScanningExecutable,
        ScanningFiles,
        ProcessingPaths,
        AdditionalGuesses,
        Done,
    }

    private const int MaxExtensionLength = 16; // "jointlodgroup" = 13
    private const int MinPathLength = 10;

    private HashSet<string> otherFileListResourcePaths = new();

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
                        otherFileListResourcePaths.Add(PathUtils.GetFilepathWithoutSuffixes(PathUtils.RemovePlatformPrefix(line)).ToString());
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
        "_alb", "_alp", "_albd", "_albm", "_alba", "_albs", "_albh", "_scot", "_acot", "_atos",
        "_nrmr", "_msr", "_nrma", "_nrm", "_nrrc", "_nrca", "_nmr", "_nrra", "_nrro",
        "_msk4", "_mskm", "_mask","_msk3", "_msk1", "_msk", "_colormask", "_selectionmask",
        "_faketex", "_dslut", "_emi", "_hgt", "_hdr", "_rocm", "_occ", "_lymo", "_rgh", "_met",
        "_iam", "_lut", "_fbi", "_add", "_emm", "_lym", "_cvt", "_vns", "_lin", "_pos", "_fur", "_im", "_disp"];

    private static readonly HashSet<string> IgnoredExtensions = ["json", "dll", "pdb", "ini", "cpp", "hpp", "h", "cs", "technology", "com", "com0", "com07", "com0N", "com0X", "com0C", "com0\\", "com0A", "fffff", "0", "iconTagEvent", "messageTagEvent"];
    private static readonly HashSet<uint> IgnoreExtHashes = IgnoredExtensions.Select(x => MurMur3HashUtils.GetHash(x)).ToHashSet();
    private static readonly string[] GuessDates = ["1", "251111", "251112", "251121", "250925"];

    private List<string> outputPaths = new();
    private HashSet<ulong> previouslyKnownHashes = new();
    private HashSet<ulong> unknownHashes = new();

    public List<string> Scan()
    {
        // create file reader
        var reader = new CachedMemoryPakReader() { IncludeUnknownFilePaths = true };
        reader.PakFilePriority.AddRange(PakFiles);
        HashKnownFilePaths(reader);
        reader.CacheEntries(true);
        outputPaths.Clear();
        outputPaths.AddRange(reader.CachedPaths);
        unknownHashes = reader.UnknownPathHashes.ToHashSet();
        var totalFilesCount = outputPaths.Count + unknownHashes.Count;
        var previouslyKnownFilesCount = outputPaths.Count;

        if (Flags.HasFlag(ScanFlags.UpdateExistingListCasing)) {
            unknownHashes = outputPaths.Select(p => MurMur3HashUtils.GetPakFilepathHash(p)).Concat(unknownHashes).ToHashSet();
            outputPaths.Clear();
        }
        if (unknownHashes.Count == 0) {
            Log.Error("No unknown files found");
            return [];
        }

        string[] sourceFileList = [];

        if (Flags.HasFlag(ScanFlags.MaintainPreviousList) && File.Exists(PreviousListFile)) {
            var lfw = new ListFileWrapper(PreviousListFile, platform);
            sourceFileList = lfw.Files;
        }
        previouslyKnownHashes = !Flags.HasFlag(ScanFlags.UpdateExistingListCasing) ? sourceFileList.Select(x => PakUtils.GetFilepathHash(x)).ToHashSet() : [];

        var rawPaths = new Dictionary<ulong, string>();
        if (Flags.HasFlag(ScanFlags.Executable)) {
            ScanExecutables(rawPaths);
        }
        if (Flags.HasFlag(ScanFlags.Files)) {
            ScanFiles(reader, totalFilesCount, rawPaths);
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
        GatherVersionPrefixes(extVersions, versionPrefixes);

        var unmatchedPaths = new List<string>();
        var unknownExtFiles = new Dictionary<string, List<string>>();

        var pathsProcessed = 0;
        Span<char> extensionStringSpan = new char[512];
        foreach (var path in rawPaths.Values.Concat(otherFileListResourcePaths)) {
            PhaseProgress = (float)pathsProcessed++ / rawPaths.Count;

            var attemptBase = Path.Combine(platform.basePath, path);
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
                    var sb = new StringBuilder();
                    sb.Append(attemptBase).Append('.');
                    foreach (var prefix in versionPrefixes) {
                        for (int i = 0; i <= 999; i++) {
                            sb.Length = attemptBase.Length + 1;
                            if (prefix == "") {
                                sb.Append(i);
                            } else {
                                sb.Append(prefix).AppendFormat("{0:000}", i);
                            }
                            sb.CopyTo(0, extensionStringSpan, sb.Length);
                            var str = extensionStringSpan.Slice(0, sb.Length);
                            var hash = MurMur3HashUtils.GetPakFilepathHash(str);
                            if (TryAddFoundFilePath(KnownFileFormats.Unknown, str, hash)) {
                                version = prefix == "" ? i : int.Parse($"{prefix}{i:000}");
                                extVersions[ext] = version;
                                canContinue = true;
                                Log.Info($"Found new file extension version: {ext}.{version}");
                                break;
                            }
                        }
                        if (canContinue) break;
                    }
                    if (!canContinue && Flags.HasFlag(ScanFlags.BruteforceExtensions)) {
                        for (uint i = 0; i < uint.MaxValue; i++) {
                            sb.Length = attemptBase.Length + 1;
                            sb.Append((int)i);
                            sb.CopyTo(0, extensionStringSpan, sb.Length);
                            var str = extensionStringSpan.Slice(0, sb.Length);
                            var hash = MurMur3HashUtils.GetPakFilepathHash(str);
                            if (TryAddFoundFilePath(KnownFileFormats.Unknown, str, hash)) {
                                extVersions[ext] = version = (int)i;
                                canContinue = true;
                                Log.Info($"Found new file extension version: {ext}.{version}");
                                break;
                            }
                        }
                    }
                    if (!canContinue) {
                        Log.Warn($"Unknown version for file extension {ext} (file {path})");
                        unknownExtFiles[ext] = uknList = new();
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
                if (previouslyKnownHashes.Contains(attemptHash)) {
                    matchesExisting = true;
                } else {
                    foundNewFile |= TryAddFoundFilePath(format, attempt, attemptHash);
                }

                if (format == KnownFileFormats.Texture) {
                    var lastSub = attempt.LastIndexOf('_');
                    if (lastSub != -1) {
                        var withoutType = attempt.Substring(0, lastSub);
                        foreach (var type in TexTypes) {
                            attempt = $"{Path.ChangeExtension(withoutType, null)}{type}.{ext}{versionExt}{suffix}";
                            attemptHash = PakUtils.GetFilepathHash(attempt);
                            matchesExisting |= previouslyKnownHashes.Contains(attemptHash);
                            if (TryAddFoundFilePath(format, attempt, attemptHash)) {
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

        AttemptAdditionalGuesses(rawPaths, extVersions);

        // in case the previous list file had some paths we didn't find, make sure we add them back in
        if (Flags.HasFlag(ScanFlags.UpdateExistingListCasing)) {
            var outputHashes = outputPaths.Select(p => MurMur3HashUtils.GetPakFilepathHash(p)).ToHashSet();
            foreach (var path in sourceFileList) {
                var hash = MurMur3HashUtils.GetPakFilepathHash(path);
                if (!outputHashes.Contains(hash)) {
                    outputPaths.Add(path);
                    unknownHashes.Remove(hash);
                }
            }

            AttemptAdditionalGuesses(rawPaths, extVersions);
        }

        var newFiles = outputPaths.Count - previouslyKnownFilesCount;
        Log.Info($"Found {newFiles} new file paths");
        // we can't reliably get just the list of new paths in this case because we're re-doing the whole list, so don't print when UpdateExistingListCasing
        if (newFiles > 0 && previouslyKnownFilesCount > 0 && !Flags.HasFlag(ScanFlags.UpdateExistingListCasing)) {
            Log.Info(string.Join("\n", outputPaths[previouslyKnownFilesCount..]));
        }
        if (!Flags.HasFlag(ScanFlags.UpdateExistingListCasing) && sourceFileList.Length > 0) {
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

    private static void GatherVersionPrefixes(Dictionary<string, int> extVersions, List<string> versionPrefixes)
    {
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
        // append all dates from last known date till today as well
        var latestVer = versionPrefixes.Where(v => v.Length == 6 && int.TryParse(v[0..2], out var yy) && yy >= 25 && yy <= DateTime.UtcNow.Year % 100).OrderDescending().First();
        if (DateTime.TryParseExact(latestVer, "yyMMdd", CultureInfo.InvariantCulture, DateTimeStyles.AssumeUniversal, out var latestDate)) {
            var todayDate = DateTime.UtcNow.Date;
            while (latestDate < todayDate) {
                versionPrefixes.Add(latestDate.ToString("yyMMdd"));
                latestDate = latestDate.AddDays(1);
            }
        }
    }

    private void AttemptAdditionalGuesses(Dictionary<ulong, string> rawPaths, Dictionary<string, int> extVersions)
    {
        int pathsProcessed = 0;
        Phase = GeneratorPhase.AdditionalGuesses;
        PhaseProgress = 0;
        var mot = ".mot." + extVersions.GetValueOrDefault("mot").ToString();
        var mesh = ".mesh." + extVersions.GetValueOrDefault("mesh").ToString();
        var sdftex = ".sdftex." + extVersions.GetValueOrDefault("sdftex").ToString();
        var mdf2 = ".mdf2." + extVersions.GetValueOrDefault("mdf2").ToString();
        var skeleton = ".skeleton." + extVersions.GetValueOrDefault("skeleton").ToString();
        var refskel = ".refskel." + extVersions.GetValueOrDefault("refskel").ToString();
        var fbxskel = ".fbxskel." + extVersions.GetValueOrDefault("fbxskel").ToString();
        for (int i = 0; i < outputPaths.Count; i++) {
            var path = outputPaths[i];
            PhaseProgress = (float)pathsProcessed++ / rawPaths.Count;
            var format = PathUtils.ParseFileFormat(path).format;
            var extless = PathUtils.GetFilepathWithoutExtensionOrVersion(path);
            if (path.Equals("natives/stm/systems/rendering/bluenoise256x256/hdr_rgba_0000.tex.251111100", StringComparison.OrdinalIgnoreCase)) {
                Log.Info("a");
            }
            if (format == KnownFileFormats.Mesh) {
                DoAttempt(format, string.Concat(extless, sdftex));
                DoAttempt(format, string.Concat(extless, mdf2));
                DoAttempt(format, string.Concat(extless, mot));
                DoAttempt(format, string.Concat(extless, "_Mat", mdf2));
                DoAttempt(format, string.Concat(extless, "_00", mdf2));
                DoAttempt(format, string.Concat(extless, "_01", mdf2));
            }
            if (format == KnownFileFormats.MeshMaterial) {
                var baseMatPath = extless;
                if (baseMatPath.EndsWith("_Mat", StringComparison.OrdinalIgnoreCase)) baseMatPath = baseMatPath[..^4];
                if (baseMatPath.EndsWith("_00", StringComparison.OrdinalIgnoreCase)) baseMatPath = baseMatPath[..^3];
                DoAttempt(format, string.Concat(baseMatPath, mesh));
            }
            if (format == KnownFileFormats.SDFTexture) {
                DoAttempt(format, string.Concat(extless, mesh));
            }
            if (format is KnownFileFormats.Skeleton or KnownFileFormats.RefSkeleton or KnownFileFormats.FbxSkeleton) {
                DoAttempt(format, string.Concat(extless, skeleton));
                DoAttempt(format, string.Concat(extless, refskel));
                DoAttempt(format, string.Concat(extless, fbxskel));
            }
            if (extless.EndsWith("_00")) {
                var denumbered = extless[..^2];
                var extSuffix = "." + PathUtils.GetFilenameExtensionWithSuffixes(path).ToString();
                for (int n = 1; n <= 20; n++) {
                    DoAttempt(format, string.Concat(denumbered, n.ToString("00"), extSuffix));
                }
            }
            if (extless.EndsWith("_000")) {
                var denumbered = extless[..^3];
                var extSuffix = "." + PathUtils.GetFilenameExtensionWithSuffixes(path).ToString();
                for (int n = 1; n <= 300; n++) {
                    DoAttempt(format, string.Concat(denumbered, n.ToString("000"), extSuffix));
                }
            }
            if (extless.EndsWith("_0000")) {
                var denumbered = extless[..^4];
                var extSuffix = "." + PathUtils.GetFilenameExtensionWithSuffixes(path).ToString();
                for (int n = 1; n <= 300; n++) {
                    DoAttempt(format, string.Concat(denumbered, n.ToString("0000"), extSuffix));
                }
            }
        }
    }

    private bool DoAttempt(KnownFileFormats format, string attempt)
    {
        var attemptHash = MurMur3HashUtils.GetPakFilepathHash(attempt);
        return TryAddFoundFilePath(format, attempt, attemptHash);
    }

    private bool TryAddFoundFilePath(KnownFileFormats format, ReadOnlySpan<char> attempt, ulong attemptHash)
    {
        if (unknownHashes.Contains(attemptHash)) {
            return TryAddFoundFilePath(format, attempt.ToString(), attemptHash);
        }
        return false;
    }

    private bool TryAddFoundFilePath(KnownFileFormats format, string attempt, ulong attemptHash)
    {
        if (!unknownHashes.Remove(attemptHash)) return false;

        outputPaths.Add(attempt);

        if (format is KnownFileFormats.Texture or KnownFileFormats.Mesh or KnownFileFormats.Movie or
            KnownFileFormats.SoundBank or KnownFileFormats.SoundPackage or KnownFileFormats.SoundVoxel or KnownFileFormats.SoundStreamingLQG or
            KnownFileFormats.VibrationSource or KnownFileFormats.WwiseStreamingGeometry or ReeLib.KnownFileFormats.MaterialPointCloud or KnownFileFormats.Unknown) {
            // try streaming/
            var streaming = PathUtils.GetStreamingNativesPath(attempt, platform);
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
    private static readonly SearchValues<char> Numeric = SearchValues.Create("0123456789");
    private static readonly SearchValues<char> ExtensionChars = SearchValues.Create("abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789");

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
        if (ext.Length <= 1 || ext.Length > MaxExtensionLength || !ext.ContainsAnyExcept(Numeric) || ext.ContainsAnyExcept(ExtensionChars) || IgnoreExtHashes.Contains(MurMur3HashUtils.GetHash(ext))) return false;
        return true;
    }

    private static bool IsLocalizableFormat(KnownFileFormats format) =>
        format is KnownFileFormats.SoundBank or KnownFileFormats.SoundPackage or KnownFileFormats.Texture or KnownFileFormats.MotionList or KnownFileFormats.Movie or KnownFileFormats.UserData or KnownFileFormats.Fsm2;

    private static bool IsPathlessFormat(KnownFileFormats format)
        => format is KnownFileFormats.Texture or KnownFileFormats.Mesh or KnownFileFormats.Movie or
        KnownFileFormats.SoundPackage or KnownFileFormats.SoundVoxel or KnownFileFormats.SoundBank or
        KnownFileFormats.MasterMaterial or KnownFileFormats.Shader or ReeLib.KnownFileFormats.VfxShader or
        KnownFileFormats.Probes or KnownFileFormats.SparseShadowTree or
        KnownFileFormats.Strands or KnownFileFormats.CollisionMesh;

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

        foreach (var line in HashUtils.ExtractStrings(encoding, stream, filepath, PathyChars, IsFilePath)) {
            yield return CleanFilePath(line);
            // sometimes we end up grabbing paths like "aConfig/...", try to handle those as well
            if (char.IsLower(line[0]) && char.IsUpper(line[1])) {
                yield return line.Substring(1);
            }
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
}