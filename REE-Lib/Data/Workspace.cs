using System.Diagnostics.CodeAnalysis;
using System.Text.Json;
using ReeLib.Common;
using ReeLib.Data;
using ReeLib.Il2cpp;

namespace ReeLib;

/// <summary>
/// Represents an abstraction of all game-specific file settings. Should be thread safe.
/// </summary>
/// <param name="config"></param>
public sealed partial class Workspace(GameConfig config) : IDisposable
{
    public GameConfig Config => config;

    /// <summary>
    /// Whether to allow fetching resources directly from the game's PAK files.
    /// </summary>
    public bool AllowUsePackedFiles { get; set; } = true;
    /// <summary>
    /// <para>Whether to _always_ allow fetching resources from the loose file folder in the game path. Loose files are evaluated before packed files, same as if the loose file loader was enabled ingame.</para>
    /// Defaults to false to prevent accidentally fetching modded files.
    /// </summary>
    public bool AllowUseLooseFiles { get; set; } = false;

    private RszParser? _rszParser;
    public RszParser RszParser => _rszParser ??= LoadRszParser();

    private RszFileOption? rszFileOption;
    public RszFileOption RszFileOption => rszFileOption ??= new RszFileOption(config.BuiltInGame, RszParser);

    private JsonSerializerOptions? _jsonOptions;
    public JsonSerializerOptions JsonOptions => _jsonOptions ??= CreateJsonSerializerOptions();

    private TypeCache? _typeCache;
    public TypeCache TypeCache => _typeCache ??= LoadTypeCache();

    private CommonRszClasses? _classes;
    public CommonRszClasses Classes => _classes ??= new CommonRszClasses(RszParser);

    private Dictionary<string, Dictionary<string, PrefabGameObjectRefProperty>>? _refPropCache;
    public Dictionary<string, Dictionary<string, PrefabGameObjectRefProperty>> PfbRefProps => _refPropCache ??= LoadPfbRefData();

    private EfxCacheData? _efxCache;
    public EfxCacheData EfxCacheData => _efxCache ??= LoadEfxDataCache();

    private CachedMemoryPakReader? _baseReader;
    public CachedMemoryPakReader PakReader => _baseReader ??= CreateUnpacker();

    private readonly object _listlock = new();

    [Flags]
    public enum FileSourceType
    {
        Pak = 1,
        Loose = 2,
        ExtractedChunks = 4,
        Original = Pak|ExtractedChunks,
        Any = Pak|Loose|ExtractedChunks,
    }

    public bool CanUsePakFiles => !string.IsNullOrEmpty(config.GamePath);
    public bool CanUseChunkFiles => !string.IsNullOrEmpty(config.ChunkPath);
    public bool CanUseLooseFiles => !string.IsNullOrEmpty(config.GamePath);
    public bool CanExtractPakFiles => (!string.IsNullOrEmpty(config.GamePath) || config.PakFiles.Length > 0) && !string.IsNullOrEmpty(config.ChunkPath);

    /// <summary>
    /// Attempts to load the list file and returns whether there's at least one file in the list.
    /// </summary>
    public bool CanUseListFile => ListFile?.Files.Length > 0;

    public bool UsesEmbeddedInstanceInfoUserdata => config.Game.hash is GameNameHash.re7;
    public bool UsesEmbeddedUserdata => config.Game.hash is GameNameHash.dmc5 or GameNameHash.re2;
    public bool UsesEmbeddedUserdataAny => config.Game.hash is GameNameHash.dmc5 or GameNameHash.re2 or GameNameHash.re7;
    public bool RequiresSubPaksForTextures => TexFile.GetRequiresGDeflateCompression(config.Game);

    public string BasePath => Config.Platform.basePath;

    public IEnumerable<string> GameFileExtensions => FileExtensionCache.Versions.Keys;
    public IEnumerable<REFileFormat> GameFileFormats => FileExtensionCache.Versions.Select(kv => new REFileFormat(PathUtils.ParseFileFormat(kv.Key).format, kv.Value));

    private static readonly JsonSerializerOptions jsonOptions = new() {
        WriteIndented = true,
        DefaultIgnoreCondition = System.Text.Json.Serialization.JsonIgnoreCondition.WhenWritingDefault,
        IncludeFields = true,
        IgnoreReadOnlyFields = true,
        IgnoreReadOnlyProperties = true,
    };
    public static JsonSerializerOptions DefaultJsonOptions { get; set; } = jsonOptions;

    private FileExtensionCache? _fileExtensionCache = null!;
    private FileExtensionCache FileExtensionCache => _fileExtensionCache ??= LoadFileExtensionCache();

    private ListFileWrapper? listFile;
    public ListFileWrapper? ListFile => listFile ??= GetFileList();

    private readonly object _setupLock = new();
    private const int lockTimeout = 15000;

    public bool TryGetFileExtensionVersion(string extension, out int version)
    {
        return FileExtensionCache.Versions.TryGetValue(extension, out version);
    }

    public IEnumerable<string> FindPossibleFilepaths(string filepath)
    {
        filepath = PrependBasePath(filepath);
        var fileList = GetFileList();
        if (fileList != null) {
            foreach (var c in fileList.GetFileExtensionVariants(filepath)) yield return c;
        }

        var basepath = PathUtils.GetFilepathWithoutExtensionOrVersion(filepath);
        var ext = PathUtils.GetFilenameExtensionWithoutSuffixes(filepath);
        var extInfo = FileExtensionCache.Info.GetValueOrDefault(ext.ToString());
        if (extInfo == null) {
            yield return AppendFileVersion(basepath.ToString());
            yield break;
        }

        var withExt = AppendFileVersion($"{basepath}.{ext}");
        if (extInfo.CanNotHaveX64) {
            yield return withExt;
        }

        if (extInfo.CanNotHaveLang && extInfo.CanHaveX64) {
            yield return withExt + ".x64";
        }

        if (extInfo.CanNotHaveLang && extInfo.CanHaveStm) {
            yield return withExt + ".stm";
        }

        if (extInfo.CanHaveLang) {
            foreach (var locale in extInfo.Locales) {
                if (extInfo.CanHaveX64)
                    yield return withExt + $".x64.{locale}";
                if (extInfo.CanHaveStm)
                    yield return withExt + $".stm.{locale}";
            }
        }
    }

    public string AppendFileVersion(string filename)
    {
        var fmt = PathUtils.ParseFileFormat(filename);
        if (fmt.version != -1) {
            return filename;
        }

        var version = GetFileVersion(filename);
        if (version == -1) {
            return filename;
        }

        return $"{filename}.{version}";
    }

    public int GetFileVersion(ReadOnlySpan<char> relativePath)
    {
        var ext = relativePath.GetExtensionWithoutPeriod().ToString();
        if (FileExtensionCache.Info.TryGetValue(ext, out var version)) {
            return version.Version;
        }

        return -1;
    }

    /// <summary>
    /// Attempt to match the filename to a single file and return the filename. Automatically adds any missing file extensions and suffixes.
    /// </summary>
    public string? ResolveFilepath(string filepath)
    {
        filepath = PrependBasePath(filepath);
        if (FileExists(filepath)) {
            return filepath;
        }

        foreach (var candidate in FindPossibleFilepaths(filepath)) {
            if (FileExists(candidate)) {
                return candidate;
            }
        }

        return null;
    }

    /// <summary>
    /// Attempt to match the filename to a single file and return a <see cref="Stream"/> for it. Automatically adds any missing file extensions and suffixes.
    /// </summary>
    public Stream? FindSingleFile(string filepath, FileSourceType sourceTypes = FileSourceType.Original) => FindSingleFile(filepath, out _, sourceTypes);

    /// <summary>
    /// Attempt to match the filename to a single file and return a <see cref="Stream"/> for it. Automatically adds any missing file extensions and suffixes.
    /// </summary>
    public Stream? FindSingleFile(string filepath, [MaybeNull] out string resolvedNativeFilepath, FileSourceType sourceTypes = FileSourceType.Original)
    {
        filepath = PrependBasePath(filepath.NormalizeFilepath());
        var match = GetFile(filepath, sourceTypes);
        if (match != null) {
            resolvedNativeFilepath = filepath.NormalizeFilepath();
            return match;
        }

        foreach (var candidate in FindPossibleFilepaths(filepath)) {
            match = GetFile(candidate, sourceTypes);
            if (match != null) {
                resolvedNativeFilepath = candidate.NormalizeFilepath();
                return match;
            }
        }

        resolvedNativeFilepath = null;
        return null;
    }

    /// <summary>
    /// Gets a <see cref="Stream"/> for a single file. The filename should include the full file extension, version and any suffixes.
    /// </summary>
    public Stream? GetFile(string filepath, FileSourceType sourceTypes = FileSourceType.Original)
    {
        filepath = PrependBasePath(filepath);

        bool didAttempt = false;
        if ((sourceTypes & FileSourceType.Loose) != 0 && AllowUseLooseFiles && CanUseLooseFiles) {
            didAttempt = true;
            var loosePath = Path.Combine(config.GamePath, filepath);
            if (File.Exists(loosePath)) {
                return File.OpenRead(loosePath);
            }
        }
        if ((sourceTypes & FileSourceType.Pak) != 0 && AllowUsePackedFiles && CanUsePakFiles) {
            didAttempt = true;
            var file = PakReader.GetFile(filepath);
            if (file != null) return file;
        }
        if ((sourceTypes & FileSourceType.ExtractedChunks) != 0 && CanUseChunkFiles) {
            var outputPath = GetAbsoluteChunkFilepath(filepath);
            if (File.Exists(outputPath)) {
                return File.OpenRead(outputPath);
            }

            if (!CanExtractPakFiles) return null;

            var stream = PakReader.GetFile(filepath);
            if (stream == null) return null;

            var ofstream = new FileStream(outputPath, FileMode.Create, FileAccess.ReadWrite, FileShare.None);
            stream.CopyTo(ofstream);
            ofstream.Seek(0, SeekOrigin.Begin);
            return ofstream;
        }
        if (didAttempt || (sourceTypes & FileSourceType.Original) == 0) return null;
        throw new Exception("REE-Lib workspace not fully configured, we are unable to access game resources.");
    }

    /// <summary>
    /// Checks if an exact filepath exists in any of the configured and allowed source locations (PAK, loose files, extracted chunk path).
    /// </summary>
    public bool FileExists(string filepath)
    {
        filepath = PrependBasePath(filepath);

        bool didAttempt = false;
        if (AllowUsePackedFiles && CanUsePakFiles) {
            didAttempt = true;
            if (PakReader.FileExists(filepath)) return true;
        }

        if (AllowUseLooseFiles && CanUseLooseFiles) {
            didAttempt = true;
            var loosePath = Path.Combine(config.GamePath, filepath);
            if (File.Exists(loosePath)) {
                return true;
            }
        }

        if (CanUseChunkFiles) {
            var outputPath = GetAbsoluteChunkFilepath(filepath);
            if (File.Exists(outputPath)) return true;
            if (!CanExtractPakFiles) return false;
            return PakReader.FileExists(filepath);
        }
        if (didAttempt) return false;
        throw new Exception("REE-Lib workspace not fully configured, we are unable to access game resources.");
    }

    /// <summary>
    /// Gets a <see cref="Stream"/> for a single file. The filename should include the full file extension, version and any suffixes.
    /// Will throw an exception if the file is not found.
    /// </summary>
    public Stream GetRequiredFile(string filepath)
    {
        var file = GetFile(filepath);
        if (file == null) throw new NullReferenceException($"File not found: {filepath}");
        return file;
    }

    /// <summary>
    /// Extracts a file to the given folder and returns its full filepath. Only returns the filepath if the file already exists (even if it's not up to date).
    /// </summary>
    public string? GetExtractedFilepath(string filepath, string extractionPath)
    {
        var outputPath = CombineChunkSubpath(extractionPath, filepath);
        if (File.Exists(outputPath)) return outputPath;

        var file = GetFile(filepath);
        if (file == null) return null;

        Directory.CreateDirectory(Path.GetDirectoryName(outputPath)!);
        using var fs = File.Create(outputPath);
        file.CopyTo(fs);
        return outputPath;
    }

    /// <summary>
    /// Finds all files with the given file extension.
    /// The returned stream is transient, do not hold a permanent reference to it - copy any data you need somewhere else and don't Dispose() it.
    /// </summary>
    public IEnumerable<(string, MemoryStream)> GetFilesWithExtension(string extension, CancellationToken cancellationToken = default)
    {
        var list = GetFileList();
        if (list == null) {
            Log.Error("No list file for game " + config.Game);
            return [];
        }

        if (!TryGetFileExtensionVersion(extension, out var version)) {
            Log.Error($"File extension {extension} not supported for {config.Game}");
            return [];
        }

        PakReader.CacheEntries();
        if (PakReader.PakFilePriority.Count == 0) {
            Log.Warn("No PAK files found for game " + config.Game);
            return [];
        }
        var reader = PakReader.Clone();
        reader.Filter = new System.Text.RegularExpressions.Regex($"\\.{extension}\\.{version}(?:\\.[^\\/]*)?$", System.Text.RegularExpressions.RegexOptions.Compiled);
        reader.AddFiles(list.Files);
        return reader.FindFiles(cancellationToken);
    }

    public IEnumerable<string> GetFileExtensionsForFormat(KnownFileFormats format)
        => FileExtensionCache.Info.Where(kv => kv.Value.Type == format).Select(kv => kv.Key);

    public KnownFileFormats GetFileFormatFromExtension(string extension)
        => FileExtensionCache.Info.GetValueOrDefault(extension)?.Type ?? KnownFileFormats.Unknown;

    public static KnownFileFormats[] GetFormatIncludingSubtypes(KnownFileFormats format)
        => FileExtensionCache.GetFormatSubTypes(format) ?? [format];

    public string GetAbsoluteChunkFilepath(string filepath)
    {
        if (Path.IsPathFullyQualified(filepath)) return filepath;

        filepath = PrependBasePath(filepath);
        return Path.Combine(config.ChunkPath, filepath);
    }

    public string PrependBasePath(string path)
    {
        if (path.StartsWith('@')) {
            // happens mostly for some sound file resources in scn/pfb files, not sure why it's there
            path = path[1..];
        }

        var basePath = Config.Platform.basePath;
        if (path.StartsWith(basePath, StringComparison.OrdinalIgnoreCase)) {
            return path;
        }

        return basePath + path;
    }

    public ReadOnlySpan<char> RemoveBasePath(ReadOnlySpan<char> path)
    {
        var basePath = Config.Platform.basePath;
        if (path.StartsWith(basePath, StringComparison.OrdinalIgnoreCase)) {
            return path[basePath.Length..];
        }
        return path;
    }

    /// <summary>
    /// Converts a natives or target path into its base resource path.
    /// </summary>
    public ReadOnlySpan<char> GetResourcePath(ReadOnlySpan<char> path)
    {
        var basePath = Config.Platform.basePath;
        if (path.StartsWith(basePath, StringComparison.OrdinalIgnoreCase)) {
            path = path[basePath.Length..];
        }
        return PathUtils.GetFilepathWithoutSuffixes(path);
    }

    /// <summary>
    /// Converts a path into a target path (removes natives prefix and appends file format version).
    /// </summary>
    public string GetTargetPath(ReadOnlySpan<char> path)
    {
        return AppendFileVersion(PathUtils.RemovePlatformPrefix(path).ToString());
    }

    public void ResetListFile()
    {
        listFile = null;
    }

    private string CombineChunkSubpath(string chunkBasePath, string filepath)
    {
        var leftIsNatives = chunkBasePath.Contains(Config.Platform.basePath, StringComparison.OrdinalIgnoreCase);
        var rightIsNatives = filepath.StartsWith(Config.Platform.basePath, StringComparison.OrdinalIgnoreCase);
        if (leftIsNatives && rightIsNatives) return Path.Combine(chunkBasePath, filepath[(Config.Platform.basePath.Length + 1)..]);
        if (!leftIsNatives && !rightIsNatives) return Path.Combine(chunkBasePath, Config.Platform.basePath, filepath);
        return Path.Combine(chunkBasePath, filepath);
    }

    private RszParser LoadRszParser()
    {
        if ((config.RszPatchFiles.Length == 0 || config.RszPatchFiles.Any(x => string.IsNullOrWhiteSpace(x))) && !config.Resources.TryGetRszFiles(out _)) {
            throw new Exception("RSZ json file setting is not configured");
        }
        var pfbRefs = PfbRefProps;
        lock (_setupLock) {
            var parser = RszParser.GetInstance(config.RszPatchFiles[0]);
            for (int i = 1; i < config.RszPatchFiles.Length; ++i) {
                parser.ReadPatch(config.RszPatchFiles[i]);
            }
            parser.SetupPfbGameObjectRefs(pfbRefs);
            return parser;
        }
    }

    private ListFileWrapper? GetFileList()
    {
        if (listFile != null) return listFile;

        lock (_setupLock) {
            if (config.Resources.TryGetListFilePath(out var listfile)) {
                return listFile = new ListFileWrapper(listfile, Config.Platform);
            }
        }
        return null;
    }

    private FileExtensionCache LoadFileExtensionCache()
    {
        if (_fileExtensionCache != null) {
            return _fileExtensionCache;
        }

        _fileExtensionCache = config.Resources.GetResourceFileTypeCache();
        if (_fileExtensionCache == null && !config.Resources.TryGetResourceTypesCache(out var cacheFilepath)) {
            _fileExtensionCache = GenerateFileExtensionCache();
            if (_fileExtensionCache != null) {
                Directory.CreateDirectory(Path.GetDirectoryName(cacheFilepath)!);
                using var fs = File.Create(cacheFilepath);
                JsonSerializer.Serialize(fs, _fileExtensionCache, jsonOptions);
                Log.Info($"File extension cache for {config.Game} successfully generated");
                config.Resources.ResourceTypePath = cacheFilepath;
            }
        }
        return _fileExtensionCache ??= new();
    }

    public FileExtensionCache? GenerateFileExtensionCache()
    {
        if (config.Resources.TryGetListFilePath(out var listfile)) {
            _fileExtensionCache = new();
            _fileExtensionCache.HandleFilepathsFromList(listfile);
            return _fileExtensionCache;
        }

        Log.Error("Missing file extension cache file. Files may not resolve correctly.");
        return null;
    }

    /// <summary>
    /// Initialize the unpacker for this workspace. Does nothing if the method was already called.
    /// </summary>
    private CachedMemoryPakReader CreateUnpacker()
    {
        lock (_setupLock) {
            if (_baseReader != null) {
                return _baseReader;
            }

            Interlocked.Exchange(ref _baseReader, new CachedMemoryPakReader() {
                PakFilePriority = (config.PakFiles?.Length > 0) ? config.PakFiles.ToList() : PakUtils.ScanPakFiles(config.GamePath)
            });

            _ = FileExtensionCache;
        }
        return _baseReader;
    }

    public RszInstance CreateRszInstance(string classname)
    {
        var cls = RszParser.GetRSZClass(classname);
        if (cls == null) {
            Log.Error($"Unknown classname {classname} for game {Config.Game}");
            return RszInstance.NULL;
        }
        return RszInstance.CreateInstance(RszParser, cls);
    }

    public RszInstance CreateRszInstance(RszClass cls) => RszInstance.CreateInstance(RszParser, cls);

    private JsonSerializerOptions CreateJsonSerializerOptions()
    {
        var options = new JsonSerializerOptions(DefaultJsonOptions);
        options.Converters.Add(new RszInstanceJsonConverter(this));
        options.Converters.Add(new PfbGameObjectJsonConverter());
        options.Converters.Add(new ScnGameObjectJsonConverter());
        return options;
    }

    private static bool TryDeserialize<T>(string? filepath, [MaybeNullWhen(false)] out T result)
    {
        if (File.Exists(filepath)) {
            using var fs = File.OpenRead(filepath);
            result = JsonSerializer.Deserialize<T>(fs, jsonOptions)!;
            return result != null;
        }
        result = default!;
        return false;
    }

    private T? DeserializeOrNull<T>(string? filepath) where T : class => DeserializeOrNull<T>(filepath, default);
    private T? DeserializeOrNull<T>(string? filepath, T? _) where T : class
    {
        if (File.Exists(filepath)) {
            using var fs = File.OpenRead(filepath);
            return JsonSerializer.Deserialize<T>(fs, jsonOptions);
        }
        return null;
    }

    public void Dispose()
    {
        _baseReader?.Dispose();
    }

    public override string ToString() => $"Workspace:{config.Game}";
}
