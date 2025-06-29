using System.Text.Json;

namespace ReeLib;

public class ResourceRepository
{
    private static LocalResourceCache? _cache;
    /// <summary>
    /// Get a singleton instance of the local resource cache. Will attempt to refresh the cache on first call.
    /// The instance can change if the cache is updated, do not keep a permanent reference to it.
    /// </summary>
    internal static LocalResourceCache Cache => _cache ??= Initialize();

    /// <summary>
    /// The local filepath to store the repository cache JSON file in.
    /// </summary>
    public static string LocalResourceRepositoryFilepath { get; set; } = "resources/resource-cache.json";
    /// <summary>
    /// Disable all online resource cache update features.
    /// </summary>
    public static bool DisableOnlineUpdater { get; set; } = false;

    /// <summary>
    /// An URL or local filepath to the resource info json file to use for downloading and updating deserialization meta resources.
    /// </summary>
    public static string MetadataRemoteSource { get; set; } = "https://raw.githubusercontent.com/kagenocookie/REE-Lib-Resources/refs/heads/master/resource-info.json";

    private const int UpdateCheckIntervalDays = 1;

    public static LocalResourceCache Initialize(bool forceRefresh = false)
    {
        var shouldRefresh = (forceRefresh || _cache == null);
        var localIsSetup = _cache != null;
        if (shouldRefresh) {
            localIsSetup = ReadLocalResourceListing();
        }

        if (MetadataRemoteSource.StartsWith("http") && DisableOnlineUpdater) {
            UpdateLocalCache();
            Console.WriteLine("Resource auto-updater is disabled. Using purely local data.");
            return _cache ??= new();
        }

        var shouldUpdateFromRemote = _cache == null || forceRefresh || (DateTime.UtcNow.Date - _cache.LastUpdateCheckAtUtc.Date).TotalDays >= UpdateCheckIntervalDays;
        if (!shouldUpdateFromRemote) {
            Console.WriteLine("Using cached local resource repository data");
        } else {
            var updateSuccess = TryFetchRemoteResourceListing();
            if (updateSuccess) {
                Console.WriteLine("Updated resource repository from remote data");
            } else {
                Console.Error.WriteLine("Failed to update resource repository from remote data");
            }
        }
        _cache ??= new();
        if (!localIsSetup || shouldUpdateFromRemote || forceRefresh) {
            UpdateLocalCache();
        }

        return _cache ??= new();
    }

    private static bool TryFetchRemoteResourceListing()
    {
        using var stream = FetchContentFromUrlOrFile(MetadataRemoteSource);
        if (stream == null) {
            return false;
        }

        try {
            var data = JsonSerializer.Deserialize<RemoteResourceConfig>(stream);
            if (data == null) {
                return false;
            }

            _cache ??= new();
            _cache.RemoteInfo = data;
            _cache.LastUpdateCheckAtUtc = DateTime.UtcNow;
            return true;
        } catch (Exception e) {
            Console.Error.WriteLine("Failed to update resource info: " + e.Message);
            return false;
        }
    }

    public static Stream? FetchContentFromUrlOrFile(string path)
    {
        var isUrl = path.StartsWith("http://") || path.StartsWith("https://");
        if (isUrl) {
            if (DisableOnlineUpdater) {
                Console.WriteLine("Online resource updater is disabled, can't fetch " + path);
                return null;
            }

            var http = new HttpClient();
            var response = http.Send(new HttpRequestMessage(HttpMethod.Get, path));
            if (!response.IsSuccessStatusCode) {
                return null;
            }
            return response.Content.ReadAsStream();
        }
        if (File.Exists(path)) {
            return File.OpenRead(path);
        }

        return null;
    }

    private static bool ReadLocalResourceListing()
    {
        if (!File.Exists(LocalResourceRepositoryFilepath)) return false;
        using var fs = File.OpenRead(LocalResourceRepositoryFilepath);
        try {
            var data = JsonSerializer.Deserialize<LocalResourceCache>(fs);
            if (data != null) {
                _cache = data;
                foreach (var (name, info) in data.LocalInfo) {
                    info.Game = name;
                }
                return true;
            }
        } catch (Exception e) {
            Console.Error.WriteLine("Failed to load resource info: " + e.Message);
        }
        return false;
    }

    private static JsonSerializerOptions options = new() {
        WriteIndented = true,
        IgnoreReadOnlyProperties = true,
    };

    public static void UpdateLocalCache()
    {
        Directory.CreateDirectory(Path.GetDirectoryName(LocalResourceRepositoryFilepath)!);
        using var fs = File.Create(LocalResourceRepositoryFilepath);
        JsonSerializer.Serialize(fs, _cache, options);
    }
}
