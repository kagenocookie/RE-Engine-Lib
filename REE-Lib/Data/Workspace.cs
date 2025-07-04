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
    public bool AllowUsePackedFiles { get; set; } = true;

    private RszParser? _rszParser;
    public RszParser RszParser => _rszParser ??= LoadRszParser();

    private RszFileOption? rszFileOption;
    public RszFileOption RszFileOption => rszFileOption ??= new RszFileOption(config.BuiltInGame, RszParser);

    private JsonSerializerOptions? _jsonOptions;
    public JsonSerializerOptions JsonOptions => _jsonOptions ??= CreateJsonSerializerOptions();

    private TypeCache? _typeCache;
    public TypeCache TypeCache => _typeCache ??= LoadTypeCache();

    private Dictionary<string, Dictionary<string, PrefabGameObjectRefProperty>>? _refPropCache;
    public Dictionary<string, Dictionary<string, PrefabGameObjectRefProperty>> PfbRefProps => _refPropCache ??= LoadPfbRefData();

    private EfxCacheData? _efxCache;
    public EfxCacheData EfxCacheData => _efxCache ??= LoadEfxDataCache();

    private CachedMemoryPakReader? _baseReader;
    public CachedMemoryPakReader PakReader => _baseReader ??= CreateUnpacker();

    private readonly object _listlock = new();

    public bool CanUsePakFiles => !string.IsNullOrEmpty(config.GamePath);
    public bool CanUseChunkFiles => !string.IsNullOrEmpty(config.ChunkPath);
    public bool CanExtractPakFiles => (!string.IsNullOrEmpty(config.GamePath) || config.PakFiles.Length > 0) && !string.IsNullOrEmpty(config.ChunkPath);

    /// <summary>
    /// Attempts to load the list file and returns whether there's at least one file in the list.
    /// </summary>
    public bool CanUseListFile => ListFile?.Files.Length > 0;

    public bool IsInlineUserdata => config.Game.hash is GameNameHash.re7 or GameNameHash.dmc5 or GameNameHash.re2;
    public bool IsEmbeddedUserdata => config.Game.hash is GameNameHash.dmc5 or GameNameHash.re2;

    public bool BasePathIsX64 => config.Game.hash is GameNameHash.re7 or GameNameHash.dmc5 or GameNameHash.re2;
    public string BasePath => BasePathIsX64 ? "natives/x64/" : "natives/stm/";
    public string BasePathBackslash => BasePathIsX64 ? "natives\\x64\\" : "natives\\stm\\";

    public IEnumerable<string> GameFileExtensions => FileExtensionCache.Versions.Keys;
    public IEnumerable<REFileFormat> GameFileFormats => FileExtensionCache.Versions.Select(kv => new REFileFormat(PathUtils.ParseFileFormat(kv.Key).format, kv.Value));

    private static readonly JsonSerializerOptions jsonOptions = new() {
        WriteIndented = true,
        DefaultIgnoreCondition = System.Text.Json.Serialization.JsonIgnoreCondition.WhenWritingDefault,
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
            yield break;
        }

        var basepath = PathUtils.GetFilepathWithoutExtensionOrVersion(filepath);
        var ext = PathUtils.GetFilenameExtensionWithoutSuffixes(filepath);
        var extInfo = FileExtensionCache.Info.GetValueOrDefault(ext.ToString());
        if (extInfo == null) {
            yield return AppendFileVersion(basepath);
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

    public string AppendFileVersion(ReadOnlySpan<char> filename)
    {
        var fmt = PathUtils.ParseFileFormat(filename);
        if (fmt.version != -1) {
            return filename.ToString();
        }

        var version = GetFileVersion(filename);
        if (version == -1) {
            return filename.ToString();
        }

        return $"{filename.ToString()}.{version}";
    }

    public int GetFileVersion(ReadOnlySpan<char> relativePath)
    {
        var ext = relativePath.GetExtensionWithoutPeriod();
        if (FileExtensionCache.Info.TryGetValue(ext, out var version)) {
            return version.Version;
        }

        return -1;
    }

    /// <summary>
    /// Attempt to match the filename to a single file and return a <see cref="Stream"/> for it. Automatically adds any missing file extensions and suffixes.
    /// </summary>
    public Stream? FindSingleFile(string filepath)
    {
        filepath = PrependBasePath(filepath);
        var match = GetFile(filepath);
        if (match != null) return match;

        foreach (var candidate in FindPossibleFilepaths(filepath)) {
            match = GetFile(candidate);
            if (match != null) return match;
        }

        return null;
    }

    /// <summary>
    /// Gets a <see cref="Stream"/> for a single file. The filename should include the full file extension, version and any suffixes.
    /// </summary>
    public Stream? GetFile(string filepath)
    {
        filepath = PrependBasePath(filepath);

        bool didAttempt = false;
        if (AllowUsePackedFiles && CanUsePakFiles) {
            didAttempt = true;
            var file = PakReader.GetFile(filepath);
            if (file != null) return file;
        }
        if (CanUseChunkFiles) {
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
        if (didAttempt) return null;
        throw new Exception("REE-Lib workspace not fully configured, we are unable to access game resources.");
    }

    /// <summary>
    /// Extracts a file to the given folder and returns its full filepath. Only returns the filepath if the file already exists (even if it's not up to date).
    /// </summary>
    public string? GetExtractedFilepath(string filepath, string extractionPath)
    {
        var outputPath = PathUtils.CombineChunkSubpath(extractionPath, filepath, BasePathIsX64);
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
            Console.Error.WriteLine("No list file for game " + config.Game);
            return [];
        }

        if (!TryGetFileExtensionVersion(extension, out var version)) {
            Console.Error.WriteLine($"File extension {extension} not supported for {config.Game}");
            return [];
        }

        PakReader.CacheEntries();
        var reader = PakReader.Clone();
        reader.Filter = new System.Text.RegularExpressions.Regex($"\\.{extension}\\.{version}(?:\\.[^\\/]*)?$", System.Text.RegularExpressions.RegexOptions.Compiled);
        reader.AddFiles(list.Files);
        return reader.FindFiles(cancellationToken);
    }

    public IEnumerable<string> GetFileExtensionsForFormat(KnownFileFormats format)
        => FileExtensionCache.Info.Where(kv => kv.Value.Type == format).Select(kv => kv.Key);

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

        if (path.StartsWith(BasePath) || path.StartsWith(BasePathBackslash)) {
            return path;
        }

        return Path.Combine(BasePath, path);
    }

    public ReadOnlySpan<char> RemoveBasePath(ReadOnlySpan<char> path)
    {
        if (path.StartsWith(BasePath) || path.StartsWith(BasePathBackslash)) return path[BasePath.Length..];
        return path;
    }

    private RszParser LoadRszParser()
    {
        if (config.RszPatchFiles.Length == 0 && !config.Resources.TryGetRszFiles(out _)) {
            throw new Exception("RSZ json file setting is not configured");
        }
        lock (_setupLock) {
            var parser = RszParser.GetInstance(config.RszPatchFiles[0]);
            for (int i = 1; i < config.RszPatchFiles.Length; ++i) {
                parser.ReadPatch(config.RszPatchFiles[i]);
            }
            return parser;
        }
    }

    private ListFileWrapper? GetFileList()
    {
        if (listFile != null) return listFile;

        lock (_setupLock) {
            if (config.Resources.TryGetListFilePath(out var listfile)) {
                return listFile = new ListFileWrapper(listfile);
            }
        }
        return null;
    }

    private FileExtensionCache LoadFileExtensionCache()
    {
        if (!config.Resources.TryGetResourceTypesCache(out var cacheFilepath) || !TryDeserialize(cacheFilepath, out _fileExtensionCache)) {
            if (config.Resources.TryGetListFilePath(out var listfile)) {
                Console.WriteLine("Missing or invalid file extension cache file. Attempting to regenerate from file list ...");
                _fileExtensionCache = new();
                _fileExtensionCache.HandleFilepathsFromList(listfile);

                Directory.CreateDirectory(Path.GetDirectoryName(cacheFilepath)!);
                using var fs = File.Create(cacheFilepath);
                JsonSerializer.Serialize(fs, _fileExtensionCache, jsonOptions);
                Console.WriteLine("File extension cache successfully generated");
                config.Resources.ResourceTypePath = cacheFilepath;
            } else {
                _fileExtensionCache = new();
                Console.Error.WriteLine("Missing file extension cache file. Files may not resolve correctly.");
            }
        }
        return _fileExtensionCache ??= new();
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

    private JsonSerializerOptions CreateJsonSerializerOptions()
    {
        var options = new JsonSerializerOptions(DefaultJsonOptions);
        options.Converters.Add(new RszInstanceJsonConverter(this));
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
