using System.Text.Json;

namespace ReeLib;

/// <summary>
/// Represents an abstraction of all game-specific file settings. Thread safe.
/// </summary>
/// <param name="config"></param>
public sealed class Workspace(GameConfig config) : IDisposable
{
    public GameConfig Config => config;
    public bool UsePackedFiles { get; set; }

    public bool CanUsePakFiles => !string.IsNullOrEmpty(config.GamePath);
    public bool CanUseChunkFiles => !string.IsNullOrEmpty(config.ChunkPath);
    public bool CanExtractPakFiles => (!string.IsNullOrEmpty(config.GamePath) || config.PakFiles.Length > 0) && !string.IsNullOrEmpty(config.ChunkPath);

    public bool BasePathIsX64 => config.Game.hash is GameNameHash.re7 or GameNameHash.dmc5 or GameNameHash.re2;
    public string BasePath => BasePathIsX64 ? "natives/x64/" : "natives/stm/";
    public string BasePathBackslash => BasePathIsX64 ? "natives\\x64\\" : "natives\\stm\\";

    public IEnumerable<string> GameFileExtensions => fileExtensionCache.Versions.Keys;

    public static readonly JsonSerializerOptions jsonOptions = new() {
        WriteIndented = true,
        DefaultIgnoreCondition = System.Text.Json.Serialization.JsonIgnoreCondition.WhenWritingDefault,
    };

    private FileExtensionCache fileExtensionCache = null!;
    private ListFileWrapper? listFile;

    private readonly Mutex _lock = new();

    private volatile CachedMemoryPakReader? _baseReader;
    private CachedMemoryPakReader Reader
    {
        get
        {
            if (_baseReader == null)
            {
                if (_lock.WaitOne(1000))
                {
                    _baseReader = new CachedMemoryPakReader
                    {
                        PakFilePriority = config.PakFiles?.ToList() ?? PakUtils.ScanPakFiles(config.GamePath)
                    };
                    _lock.ReleaseMutex();
                }
                else if (_baseReader == null)
                {
                    throw new Exception("Failed to create pak reader");
                }
            }

            return _baseReader;
        }
    }

    private string GetExtensionCacheFilepath(string cacheDir) => Path.Combine(cacheDir, $"{config.Game.name}/file_extensions.json");
    private string GetRefPropsFilepath(string cacheDir) => Path.Combine(cacheDir, $"{config.Game.name}/pfb_ref_props.json");
    private string GetIl2cppCacheFilepath(string cacheDir) => Path.Combine(cacheDir, $"{config.Game.name}/il2cpp_cache.json");

    public bool TryGetFileExtensionVersion(string extension, out int version)
    {
        return fileExtensionCache.Versions.TryGetValue(extension, out version);
    }

    public void Init(string cacheDirectory)
    {
        if (_lock.WaitOne(10000))
        {
            if (_baseReader != null) return;

            _baseReader = new CachedMemoryPakReader
            {
                PakFilePriority = config.PakFiles?.ToList() ?? PakUtils.ScanPakFiles(config.GamePath)
            };
            var extCacheFilepath = GetExtensionCacheFilepath(cacheDirectory);
            if (File.Exists(extCacheFilepath)) {
                if (!TryDeserialize(extCacheFilepath, out fileExtensionCache)) {
                    throw new Exception("Failed to read file extension cache");
                }
            } else if (!string.IsNullOrEmpty(config.FileListPath)) {
                Console.WriteLine("Missing file extension cache file. Attempting to regenerate from file list ...");
                fileExtensionCache = new();
                fileExtensionCache.HandleFilepathsFromList(config.FileListPath);

                Directory.CreateDirectory(Path.GetDirectoryName(cacheDirectory)!);
                using var fs = File.Create(cacheDirectory);
                JsonSerializer.Serialize(fs, fileExtensionCache, jsonOptions);
                Console.WriteLine("File extension cache successfully generated");
            } else {
                // try and download from github?
                fileExtensionCache = new();
                Console.Error.WriteLine("Missing file extension cache file. Files may not resolve correctly.");
            }
            _lock.ReleaseMutex();
        }
        else if (_baseReader == null)
        {
            throw new Exception("Failed to initialize REE-Lib workspace");
        }
    }

    public IEnumerable<string> FindPossibleFilepaths(string filepath)
    {
        filepath = PrependBasePath(filepath);
        if (!string.IsNullOrEmpty(config.FileListPath)) {
            if (listFile == null) {
                _lock.WaitOne();
                try {
                    listFile = new ListFileWrapper(config.FileListPath);
                } finally {
                    _lock.ReleaseMutex();
                }
            }

            foreach (var c in listFile.GetFileExtensionVariants(filepath)) yield return c;
            yield break;
        }

        var basepath = PathUtils.GetFilepathWithoutExtensionOrVersion(filepath);
        var ext = PathUtils.GetFilenameExtensionWithoutSuffixes(filepath);
        var extInfo = fileExtensionCache.Info.GetValueOrDefault(ext.ToString());
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
        if (fileExtensionCache.Info.TryGetValue(ext, out var version)) {
            return version.Version;
        }

        Console.Error.WriteLine("Could not determine file version for file: " + relativePath.ToString());
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

        if (UsePackedFiles && CanUsePakFiles)
        {
            var file = Reader.GetFile(filepath);
            if (file != null) return file;
        }
        if (CanUseChunkFiles)
        {
            var outputPath = GetAbsoluteChunkFilepath(filepath);
            if (File.Exists(outputPath)) {
                return File.OpenRead(outputPath);
            }

            if (!CanExtractPakFiles) return null;

            var stream = Reader.GetFile(filepath);
            if (stream == null) return null;

            var ofstream = new FileStream(outputPath, FileMode.Create, FileAccess.ReadWrite, FileShare.None);
            stream.CopyTo(ofstream);
            ofstream.Seek(0, SeekOrigin.Begin);
            return ofstream;
        }
        throw new Exception("REE-Lib workspace not fully configured, we are unable to access game resources.");
    }

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

    public void Dispose()
    {
        _baseReader?.Dispose();
    }

    private static bool TryDeserialize<T>(string? filepath, out T result)
    {
        if (File.Exists(filepath)) {
            using var fs = File.OpenRead(filepath);
            result = JsonSerializer.Deserialize<T>(fs, jsonOptions)!;
            return result != null;
        }
        result = default!;
        return false;
    }
}
