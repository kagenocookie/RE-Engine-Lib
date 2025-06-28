using System.Text.Json;
using System.Text.Json.Nodes;
using System.Text.Json.Serialization;

namespace ReeLib;

public class LocalResourceCache
{
    public DateTime LastUpdateCheckAtUtc { get; set; }
    public Dictionary<string, LocalResources> LocalInfo { get; set; } = new();
    public RemoteResourceConfig RemoteInfo { get; set; } = new();
    public bool DisableAutoUpdates { get; set; }

    public static string GetDefaultLocalPathForGame(GameIdentifier game)
        => Path.Combine(Path.GetDirectoryName(ResourceRepository.LocalResourceRepositoryFilepath)!, game.name);

    public LocalResources UpdateAndGet(GameIdentifier game)
    {
        EnsureUpToDate(game, out var info);
        return info;
    }

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
        if (!DisableAutoUpdates && DownloadRemoteResources(game)) {
            localInfo = LocalInfo[game.name];
            ResourceRepository.UpdateLocalCache();
            return true;
        }

        ResourceRepository.UpdateLocalCache();
        return false;
    }

    public bool IsUpToDate(GameIdentifier game)
    {
        ResourceRepository.Initialize();
        if (!LocalInfo.TryGetValue(game.name, out var local)) {
            return false;
        }

        if (DisableAutoUpdates) return true;

        if (!RemoteInfo.Resources.TryGetValue(game.name, out var remote)) {
            return true;
        }

        return local.LocalPaths.LastUpdatedAtUtc >= remote.LastUpdatedAtUtc;
    }

    public bool DownloadRemoteResources(GameIdentifier game)
    {
        if (!RemoteInfo.Resources.TryGetValue(game.name, out var remote)) {
            return false;
        }

        var local = LocalInfo.GetValueOrDefault(game.name) ?? (LocalInfo[game.name] = new() { Game = game });
        if (string.IsNullOrEmpty(local.DataPath)) {
            local.DataPath = GetDefaultLocalPathForGame(game);
        }
        Directory.CreateDirectory(local.DataPath);
        local.LocalPaths.LastUpdatedAtUtc = DateTime.UtcNow;

        var http = new HttpClient();

        int i = 0;
        local.LocalPaths.RszPatchFiles = new string[remote.RszPatchFiles.Length];
        foreach (var url in remote.RszPatchFiles) {
            Console.WriteLine($"Fetching {game} RSZ patch file from {url}...");
            // var data = http.Send(new HttpRequestMessage(HttpMethod.Get, url));
            var data = ResourceRepository.FetchContentFromUrlOrFile(url);
            if (data == null) {
                Console.Error.WriteLine("Failed to download RSZ patch file from URL " + url);
                return false;
            }
            var localFile = Path.Combine(local.DataPath, i == 0 ? "rsz_template.json" : $"rsz_patch{i}.json");
            if (!SaveRSZTemplateFile(data, url, localFile)) {
                Console.Error.WriteLine("Failed to save RSZ patch file from URL " + url);
                return false;
            }
            local.LocalPaths.RszPatchFiles[i++] = localFile;
        }

        return true;
    }

    private static bool SaveRSZTemplateFile(Stream file, string source, string outputPath)
    {
        if (!source.Contains("reasy", StringComparison.OrdinalIgnoreCase)) {
            using var localFs = File.Create(outputPath);
            file.CopyTo(localFs);
            return true;
        }

        Console.WriteLine("Detected REasy sourced rsz dump json file. Attempting to clean up for REE Lib use...");

        var jsondoc = JsonSerializer.Deserialize<JsonObject>(file);
        if (jsondoc == null) {
            Console.Error.WriteLine("Failed to deserialize file");
            return false;
        }
        if (jsondoc.ContainsKey("metadata")) {
            jsondoc.Remove("metadata");
        }
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
        using var outfs = File.OpenWrite(outputPath);
        JsonSerializer.Serialize(outfs, jsondoc);
        Console.WriteLine("Saved cleaned RSZ template to " + outputPath);
        return true;
    }
}

public class LocalResources
{
    public string DataPath { get; set; } = string.Empty;
    [JsonIgnore]
    public GameIdentifier Game { get; set; }

    [JsonInclude]
    internal ResourceMetadata LocalPaths { get; set; } = new();

    private ResourceMetadata? RemoteResources => ResourceRepository.Cache.RemoteInfo.Resources.GetValueOrDefault(Game.name);

    private static readonly JsonSerializerOptions jsonOptions = new() {
        WriteIndented = true,
        DefaultIgnoreCondition = JsonIgnoreCondition.Never,
    };

    [JsonIgnore]
    public string? ResourceTypePath {
        get => LocalPaths.ResourceTypes;
        set => LocalPaths.ResourceTypes = GetStringIfFileExists(value);
    }

    public bool TryDownloadResource(out string filepath)
    {
        if (LocalPaths.ResourceTypes != null) {
            filepath = LocalPaths.ResourceTypes;
            return true;
        }
        filepath = Path.Combine(DataPath, "file_extensions.json");
        if (TryCacheFile(RemoteResources?.ResourceTypes, filepath)) {
            LocalPaths.ResourceTypes = filepath;
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
            return true;
        }

        return false;
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
    [JsonPropertyName("resources")]
    public Dictionary<string, ResourceMetadata> Resources { get; set; } = new();
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
}
