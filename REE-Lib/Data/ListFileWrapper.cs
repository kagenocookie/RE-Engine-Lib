using System.Text.RegularExpressions;
using ReeLib.Common;

namespace ReeLib;

public class ListFileWrapper
{
    public string[] Files { get; private set; } = Array.Empty<string>();

    private Dictionary<string, string[]> folderListCache = new();

    private PathFilter? _filter;
    public PathFilter? Filter
    {
        get => _filter;
        set {
            _filter = value;
            folderListCache.Clear();
        }
    }

    public delegate bool PathFilter(ReadOnlySpan<char> path);

    public IEnumerable<string> GetRecursiveFileList(string folder, int limit = -1)
    {
        var list = new List<string>();
        var queueFolders = new Queue<string>();
        queueFolders.Enqueue(folder);
        while (queueFolders.Count > 0) {
            foreach (var file in GetFilesInFolder(queueFolders.Dequeue())) {
                if (Path.GetExtension(file.AsSpan()).Length == 0) {
                    queueFolders.Enqueue(file);
                } else if (Filter?.Invoke(file) != false) {
                    list.Add(file);
                    if (limit > 0 && list.Count >= limit) return list;
                }
            }
        }

        return list;
    }
    public ListFileWrapper()
    {
    }

    public ListFileWrapper(string fileList)
    {
        ReadFileList(fileList);
    }

    public ListFileWrapper(IEnumerable<string> files)
    {
        Files = files.Where(f => !string.IsNullOrEmpty(f)).Select(f => NormalizePath(f).ToLowerInvariant()).Order().ToArray();
    }

    public void ReadFileList(string listFilepath)
    {
        if (!File.Exists(listFilepath)) {
            throw new ArgumentException("List file does not exist", nameof(listFilepath));
        }

        using var f = new StreamReader(File.OpenRead(listFilepath));
        var list = new SortedSet<string>();
        while (!f.EndOfStream) {
            var line = f.ReadLine();
            if (!string.IsNullOrWhiteSpace(line)) {
                var norm = NormalizePath(line);
                list.Add(norm);
            }
        }

        Files = list.ToArray();
    }

    public bool FileExists(string path) => Array.BinarySearch(Files, path) > 0;
    public IEnumerable<string> GetFileExtensionVariants(string path)
    {
        path = PathUtils.GetFilepathWithoutSuffixes(path).ToString();
        path = NormalizePath(path).ToLowerInvariant();
        var index = Array.BinarySearch(Files, path);
        if (index < 0) {
            index = ~index;
            if (index >= Files.Length || !Files[index].StartsWith(path)) yield break;
        }
        while(index > 0) {
            yield return Files[index];
            ++index;
            // TODO verify correctness
            if (index >= Files.Length || !Files[index].StartsWith(path)) {
                yield break;
            }
        }
    }


    public string[] GetFiles(string filter, int limit, bool bypassCache = false)
    {
        if (filter.Contains('*')) {
            return FilterAllFiles(filter, limit, bypassCache);
        } else {
            return GetFilesInFolder(filter, bypassCache);
        }
    }

    /// <summary>
    /// Filter paths by a regex pattern. The matcher converts ** into .* to allow for "standard" glob patterns.
    /// </summary>
    public string[] FilterAllFiles(string pattern, int limit, bool bypassCache = false)
    {
        pattern = NormalizePath(pattern);
        var cacheKey = pattern.ToLowerInvariant();
        if (!bypassCache && folderListCache.TryGetValue(cacheKey, out var names)) {
            return names;
        }

        try {
            var regex = new Regex("^" + pattern.Replace("/.", "\\.").Replace("**", ".*") + "$");
            var results = FilterAllFiles(regex, limit).ToArray();
            if (!bypassCache) folderListCache[cacheKey] = results;
            return results;
        } catch (Exception) {
            Log.Error("Failed to parse regex pattern: " + pattern);
            if (!bypassCache) folderListCache[cacheKey] = [];
            return [];
        }
    }

    public void ResetResultCache()
    {
        folderListCache.Clear();
    }

    public List<string> FilterAllFiles(Regex regex, int limit)
    {
        var list = new List<string>();
        foreach (var path in Files) {
            if (regex.IsMatch(path)) {
                list.Add(path);
                if (list.Count >= limit) break;
            }
        }

        return list;
    }

    public string[] GetFilesInFolder(string folder, bool bypassCache = false)
    {
        folder = NormalizePath(folder);
        var lower = folder.ToLowerInvariant();
        if (string.IsNullOrEmpty(lower)) {
            return GetFolderFileNames(string.Empty, bypassCache);
        }
        return GetFolderFileNames(lower, bypassCache);
    }

    private string[] GetFolderFileNames(string folderNormalized, bool bypassCache = false)
    {
        if (folderNormalized.EndsWith('/')) {
            folderNormalized = folderNormalized[..^1];
        }
        if (folderListCache.TryGetValue(folderNormalized, out var names)) {
            return names;
        }

        var cacheKey = folderNormalized;
        var startIndex = Array.BinarySearch(Files, folderNormalized);
        if (startIndex >= 0) {
            return folderListCache[folderNormalized] = names = [folderNormalized];
        } else {
            if (folderNormalized.Length > 0 && !folderNormalized.EndsWith('/')) {
                folderNormalized += "/";
            }
            startIndex = ~startIndex;
            if (startIndex > Files.Length) return [];
        }
        // TODO may have to handle invalid filepaths here as well - maybe a StartsWith check
        var count = Files.Length;
        var list = new List<string>();

        if (startIndex >= 0 && startIndex < Files.Length && Filter?.Invoke(Files[startIndex]) != false) {
            list.Add(GetSubfolderPath(Files[startIndex], folderNormalized).ToString());
        }
        int endIndex = startIndex + 1;
        while (endIndex < count) {
            var next = Files[endIndex++];
            if (!next.StartsWith(folderNormalized)) break;
            // possible optimization: use binary search to find the next non-matching entry instead of doing sequential iteration
            if ((list.Count == 0 || !next.StartsWith(list.Last()) || next[list.Last().Length] != '/') && Filter?.Invoke(next) != false) {
                list.Add(GetSubfolderPath(next, folderNormalized).ToString());
            }
        }

        return folderListCache[cacheKey] = names = list.ToArray();
    }

    private static ReadOnlySpan<char> GetSubfolderPath(string path, string fromFolder)
    {
        if (path.Length < fromFolder.Length) return path;
        var nextSlash = fromFolder.EndsWith('/') ? path.IndexOf('/', fromFolder.Length) : path.IndexOf('/', fromFolder.Length + 1);
        if (nextSlash == -1) return path;
        return path.AsSpan().Slice(0, nextSlash);
    }

    private static string NormalizePath(string path)
    {
        path = path.Replace('\\', '/');
        if (path.StartsWith('/')) path = path[1..];
        return path.ToLowerInvariant();
    }

    public virtual string? GetPathInfo(string path, string field)
    {
        if (field.Equals("path", StringComparison.OrdinalIgnoreCase)) return path;
        if (field.Equals("name", StringComparison.OrdinalIgnoreCase)) return Path.GetFileName(path);
        if (field.Equals("extension", StringComparison.OrdinalIgnoreCase)) return Path.GetExtension(path);
        if (field.Equals("type", StringComparison.OrdinalIgnoreCase)) return GetPathType(path).ToString();

        return null;
    }

    private enum PathType { File, Folder }

    private PathType GetPathType(string path)
    {
        var exactMatch = Array.BinarySearch(Files, NormalizePath(path));
        return exactMatch < 0 ? PathType.Folder : PathType.File;
    }
}
