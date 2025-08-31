using System.Text.Json;
using System.Text.Json.Nodes;
using System.Text.Json.Serialization;
using ReeLib.Common;

namespace ReeLib;

public class LocalResourceCache
{
    public DateTime LastUpdateCheckAtUtc { get; set; }
    [JsonIgnore]
    public Dictionary<string, FileExtensionCache> FileExtensions { get; } = new();
    public DatedResourcePath? FileExtensionsPath { get; set; }
    public Dictionary<string, LocalResources> LocalInfo { get; set; } = new();
    public RemoteResourceConfig RemoteInfo { get; set; } = new();
    public bool DisableAutoUpdates { get; set; }

    public static string GetDefaultLocalPathForGame(GameIdentifier game)
        => Path.Combine(Path.GetDirectoryName(ResourceRepository.LocalResourceRepositoryFilepath)!, game.name);

    public bool EnsureUpToDate(GameIdentifier game, out LocalResources localInfo)
    {
        if (IsUpToDate(game)) {
            localInfo = LocalInfo[game.name];
            return true;
        }

        var previousInfo = LocalInfo.GetValueOrDefault(game.name);
        LocalInfo[game.name] = localInfo = new() {
            Game = game,
            DataPath = previousInfo?.DataPath ?? GetDefaultLocalPathForGame(game),
        };
        if (!DisableAutoUpdates && RemoteInfo.Resources.TryGetValue(game.name, out var remote)) {
            localInfo = LocalInfo[game.name];
            if (remote.EfxStructs != null) localInfo.LocalPaths.EfxStructs = null;
            if (remote.RszPatchFiles != null) localInfo.LocalPaths.RszPatchFiles = [];
            if (remote.FileList != null) localInfo.LocalPaths.FileList = null;
            if (remote.Il2cppCache != null) localInfo.LocalPaths.Il2cppCache = null;
            if (remote.ResourceTypes != null) localInfo.LocalPaths.ResourceTypes = null;
            if (remote.PfbRefs != null) localInfo.LocalPaths.PfbRefs = null;
            localInfo.LocalPaths.CollisionDefinition = remote.CollisionDefinition;
            localInfo.LocalPaths.DynamicsDefinition = remote.DynamicsDefinition;
            localInfo.LocalPaths.LastUpdatedAtUtc = DateTime.UtcNow;
            ResourceRepository.UpdateLocalCache();
            return true;
        }

        ResourceRepository.UpdateLocalCache();
        return false;
    }

    public bool IsUpToDate(GameIdentifier game)
    {
        if (!LocalInfo.TryGetValue(game.name, out var local)) {
            return false;
        }

        if (DisableAutoUpdates) return true;

        if (!RemoteInfo.Resources.TryGetValue(game.name, out var remote)) {
            return true;
        }

        return local.LocalPaths.LastUpdatedAtUtc >= remote.LastUpdatedAtUtc;
    }
}

public class LocalResources
{
    public string DataPath { get; set; } = string.Empty;
    [JsonIgnore]
    public GameIdentifier Game { get; set; }

    [JsonInclude]
    public ResourceMetadata LocalPaths { get; set; } = new();

    private ResourceMetadata? RemoteResources => ResourceRepository.Cache.RemoteInfo.Resources.GetValueOrDefault(Game.name);

    private static readonly JsonSerializerOptions jsonOptions = new() {
        WriteIndented = true,
        DefaultIgnoreCondition = JsonIgnoreCondition.Never,
        Encoder = System.Text.Encodings.Web.JavaScriptEncoder.UnsafeRelaxedJsonEscaping,
    };

    [JsonIgnore]
    public string? ResourceTypePath {
        get => LocalPaths.ResourceTypes;
        set => LocalPaths.ResourceTypes = GetStringIfFileExists(value);
    }

    public bool TryGetRszFiles(out string[] paths)
    {
        if (LocalPaths.RszPatchFiles != null && LocalPaths.RszPatchFiles.Length > 0) {
            paths = LocalPaths.RszPatchFiles;
            return true;
        }

        if (DownloadRSZResources(Game, RemoteResources)) {
            paths = LocalPaths.RszPatchFiles!;
            ResourceRepository.UpdateLocalCache();
            return true;
        }

        paths = LocalPaths.RszPatchFiles ??= [];
        return false;
    }

    public FileExtensionCache? GetResourceFileTypeCache()
    {
        var cache = ResourceRepository.Cache;
        if (cache.RemoteInfo.FileExtensions != null && (cache.FileExtensionsPath == null || cache.RemoteInfo.FileExtensions.LastUpdatedAt > cache.FileExtensionsPath.LastUpdatedAt)) {
            var globalCacheLocalPath = Path.GetFullPath(Path.Combine(Path.GetDirectoryName(ResourceRepository.LocalResourceRepositoryFilepath)!, "global/file_extensions.json"));
            if (TryCacheFile(cache.RemoteInfo.FileExtensions.Uri, globalCacheLocalPath)) {
                cache.FileExtensions.Clear();
                cache.FileExtensionsPath = new DatedResourcePath(globalCacheLocalPath, DateTime.UtcNow);
                ResourceRepository.UpdateLocalCache();
            }
        }

        if (TryGetResourceTypesCache(out var gameSpecific)) {
            return TryDeserialize<FileExtensionCache>(gameSpecific, out var data) ? data : null;
        }

        if (!string.IsNullOrEmpty(cache.FileExtensionsPath?.Uri) && cache.FileExtensions.Count == 0) {
            if (TryDeserialize<Dictionary<string, FileExtensionCache>>(cache.FileExtensionsPath.Uri, out var data)) {
                foreach (var p in data) {
                    cache.FileExtensions.TryAdd(p.Key, p.Value);
                }
            }
        }

        return cache.FileExtensions?.GetValueOrDefault(Game.name);
    }

    public bool TryGetResourceTypesCache(out string filepath)
    {
        if (LocalPaths.ResourceTypes != null) {
            filepath = LocalPaths.ResourceTypes;
            return true;
        }
        filepath = Path.Combine(DataPath, "file_extensions.json");
        if (TryCacheFile(RemoteResources?.ResourceTypes, filepath)) {
            LocalPaths.ResourceTypes = filepath;
            ResourceRepository.UpdateLocalCache();
            return true;
        }

        return false;
    }

    public bool TryGetListFilePath(out string filepath)
    {
        if (LocalPaths.FileList != null) {
            filepath = LocalPaths.FileList;
            return true;
        }
        filepath = Path.Combine(DataPath, $"{Game.name}_files.list");
        if (TryCacheFile(RemoteResources?.FileList, filepath)) {
            LocalPaths.FileList = filepath;
            ResourceRepository.UpdateLocalCache();
            return true;
        }

        return false;
    }

    public bool TryGetIl2cppCachePath(out string filepath)
    {
        if (LocalPaths.Il2cppCache != null) {
            filepath = LocalPaths.Il2cppCache;
            return true;
        }
        filepath = Path.Combine(DataPath, $"il2cpp_cache.json");
        if (TryCacheFile(RemoteResources?.Il2cppCache, filepath)) {
            LocalPaths.Il2cppCache = filepath;
            ResourceRepository.UpdateLocalCache();
            return true;
        }

        return false;
    }

    public bool TryGetPfbRefCachePath(out string filepath)
    {
        if (LocalPaths.PfbRefs != null) {
            filepath = LocalPaths.PfbRefs;
            return true;
        }
        filepath = Path.Combine(DataPath, $"pfb_ref_props.json");
        if (TryCacheFile(RemoteResources?.PfbRefs, filepath)) {
            LocalPaths.PfbRefs = filepath;
            ResourceRepository.UpdateLocalCache();
            return true;
        }

        return false;
    }

    public bool TryGetEfxCachePath(out string filepath)
    {
        if (LocalPaths.EfxStructs != null) {
            filepath = LocalPaths.EfxStructs;
            return true;
        }
        filepath = Path.Combine(DataPath, $"efx_structs.json");
        if (TryCacheFile(RemoteResources?.EfxStructs, filepath)) {
            LocalPaths.EfxStructs = filepath;
            ResourceRepository.UpdateLocalCache();
            return true;
        }

        return false;
    }

    private bool DownloadRSZResources(GameIdentifier game, ResourceMetadata? remote)
    {
        if (remote == null) return false;

        Directory.CreateDirectory(DataPath);
        LocalPaths.LastUpdatedAtUtc = DateTime.UtcNow;

        int i = 0;
        LocalPaths.RszPatchFiles = new string[remote.RszPatchFiles.Length];
        foreach (var url in remote.RszPatchFiles) {
            Log.Info($"Fetching {game} RSZ patch file from {url}...");
            var data = ResourceRepository.FetchContentFromUrlOrFile(url);
            if (data == null) {
                Console.Error.WriteLine("Failed to download RSZ patch file from URL " + url);
                return false;
            }
            var localFilename = i switch {
                0 => "rsz_template.json",
                1 => "rsz_patch.json",
                _ => $"rsz_patch{i}.json",
            };
            var localFile = Path.Combine(DataPath, localFilename);
            if (!SaveRSZTemplateFile(data, url, localFile)) {
                Console.Error.WriteLine("Failed to save RSZ patch file from URL " + url);
                return false;
            }
            LocalPaths.RszPatchFiles[i++] = localFile;
        }

        return true;
    }

    private static bool SaveRSZTemplateFile(Stream file, string source, string outputPath)
    {
        var jsondoc = JsonSerializer.Deserialize<JsonObject>(file);
        if (jsondoc == null) {
            Console.Error.WriteLine("Failed to deserialize RSZ JSON file");
            return false;
        }

        var isReasy = source.Contains("reasy", StringComparison.OrdinalIgnoreCase) || jsondoc.ContainsKey("metadata");

        if (!isReasy) {
            Directory.CreateDirectory(Path.GetDirectoryName(outputPath)!);
            using var localFs = File.Create(outputPath);
            JsonSerializer.Serialize(localFs, jsondoc, jsonOptions);
            return true;
        }

        Log.Info("Detected REasy sourced rsz dump json file. Attempting to clean up for REE Lib use...");

        jsondoc.Remove("metadata");
        foreach (var prop in jsondoc) {
            var item = prop.Value!.AsObject();
            if (!item.ContainsKey("fields")) continue;

            var name = item["name"]!.GetValue<string>();
            if (name == "via.physics.SphereShape") {
                // our tooling expects 1 shape == 1 object, otherwise things get complicated and messy
                var sphereField = item["fields"]!.AsArray().Where(f => f!["name"]!.GetValue<string>() == "Center").FirstOrDefault();
                var radiusField = item["fields"]!.AsArray().Where(f => f!["name"]!.GetValue<string>() == "Radius").FirstOrDefault();
                if (radiusField != null) {
                    item["fields"]!.AsArray().Remove(radiusField);
                }
                if (sphereField != null) {
                    if (sphereField["size"]!.GetValue<int>() == 12) {
                        sphereField["name"] = "Sphere";
                        sphereField["size"] = 16;
                        sphereField["type"] = "Vec4";
                        sphereField["original_type"] = "via.vec4";
                    }
                }
            }

            if (name == "via.physics.CylinderShape") {
                // our tooling expects 1 shape == 1 object, otherwise things get complicated and messy
                var fieldlist = item["fields"]!.AsArray();
                var p0 = fieldlist.FirstOrDefault(f => f!["name"]!.GetValue<string>().Contains("p0")) ?? fieldlist.FirstOrDefault(f => f!["type"]!.GetValue<string>() == "Vec3");
                var p1 = fieldlist.FirstOrDefault(f => f!["name"]!.GetValue<string>().Contains("p1")) ?? fieldlist.Where(f => f!["name"]!.GetValue<string>() == "Vec3").Skip(1).FirstOrDefault();
                var r = fieldlist.FirstOrDefault(f => f!["name"]!.GetValue<string>().EndsWith("_r")) ?? fieldlist.FirstOrDefault(f => f!["type"]!.GetValue<string>() == "F32");
                if (p0 != null && p1 != null && r != null) {
                    fieldlist.Remove(p1);
                    fieldlist.Remove(r);
                    p0["name"] = "Cylinder";
                    p0["type"] = "Cylinder";
                    p0["size"] = 48;
                    p0["original_type"] = "";
                }
            }

            // reasy defines some custom types that we don't care about, remap them to default types
            foreach (var field in item["fields"]!.AsArray()) {
                var type = field!["type"]!.GetValue<string>();
                if (type == "MlShape" || type == "RawBytes") {
                    field["type"] = "Data";
                } else if (type == "Bits") {
                    var size = field["size"]!.GetValue<int>();
                    if (size == 4) {
                        field["type"] = "U32";
                    } else {
                        throw new Exception("Unhandled Bits type size");
                    }
                } else if (type == "Float") {
                    field["type"] = "F32";
                }
            }
        }
        using var outfs = File.Create(outputPath);
        JsonSerializer.Serialize(outfs, jsondoc, jsonOptions);
        Log.Info("Saved cleaned RSZ template to " + outputPath);
        return true;
    }

    private static string? GetStringIfFileExists(string? path)
    {
        if (string.IsNullOrEmpty(path)) return null;
        var full = Path.GetFullPath(path);
        return File.Exists(full) ? full : null;
    }

    private static bool TryCacheFile(string? source, string outputPath)
    {
        if (string.IsNullOrEmpty(source)) return false;
        var stream = ResourceRepository.FetchContentFromUrlOrFile(source);
        if (stream == null) {
            return false;
        }
        Directory.CreateDirectory(Path.GetDirectoryName(outputPath)!);
        using var outfs = File.Create(outputPath);
        stream.CopyTo(outfs);
        return true;
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

public class RemoteResourceConfig
{
    [JsonPropertyName("file_extensions")]
    public DatedResourcePath? FileExtensions { get; set; }

    [JsonPropertyName("resources")]
    public Dictionary<string, ResourceMetadata> Resources { get; set; } = new();
}

public record DatedResourcePath(string Uri, DateTime LastUpdatedAt)
{
    public string Uri { get; set; } = Uri;
    public DateTime LastUpdatedAt { get; set; } = LastUpdatedAt;
}

public class ResourceMetadata
{
    public DateTime LastUpdatedAtUtc { get; set; }
    public string[] RszPatchFiles { get; set; } = [];
    public string? FileList { get; set; }
    public string? Il2cppCache { get; set; }
    public string? PfbRefs { get; set; }
    public string? EfxStructs { get; set; }
    public string? ResourceTypes { get; set; }
    public string? CollisionDefinition { get; set; }
    public string? DynamicsDefinition { get; set; }

    [JsonIgnore]
    public bool IsFullySupported => Il2cppCache != null && RszPatchFiles.Length > 0 && EfxStructs != null;
}
