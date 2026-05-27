using System.Collections;
using System.Text.RegularExpressions;
using ReeLib.Common;

namespace ReeLib;

public class ListFileWrapper
{
    public string[] Files { get; private set; } = Array.Empty<string>();

    private Dictionary<ulong, string[]> folderListCache = new();

    private PathFilter? _filter;

    private readonly PlatformIdentifier platform;

    public PathFilter? Filter
    {
        get => _filter;
        set {
            _filter = value;
            folderListCache.Clear();
        }
    }

    public delegate bool PathFilter(ReadOnlySpan<char> path);

    private List<PathFilter> PatternFilters { get; } = new();

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

    public ListFileWrapper(PlatformIdentifier platform)
    {
        this.platform = platform;
    }

    public ListFileWrapper(string fileList, PlatformIdentifier platform)
    {
        this.platform = platform;
        ReadFileList(fileList);
    }

    public ListFileWrapper(IEnumerable<string> files, PlatformIdentifier platform, bool ensureUniqueEntries = false)
    {
        this.platform = platform;
        var ordered = new List<string>();
        var platformPrefix = platform.basePath;
        bool? isCorrectPlatform = platformPrefix == null ? true : null;
        foreach (var f in files) {
            if (string.IsNullOrEmpty(f)) continue;

            var norm = NormalizePath(f);
            isCorrectPlatform ??= norm.StartsWith(platformPrefix!);
            if (isCorrectPlatform == false) {
                norm = PathUtils.SwapPlatformPrefix(norm, platform);
            }
            ordered.Add(norm);
        }
        ordered.Sort(StringComparer.OrdinalIgnoreCase);
        if (ensureUniqueEntries) {
            Files = ordered.Distinct(StringComparer.OrdinalIgnoreCase).ToArray();
        } else {
            Files = ordered.ToArray();
        }
    }

    public void ReadFileList(string listFilepath)
    {
        if (!File.Exists(listFilepath)) {
            throw new ArgumentException("List file does not exist", nameof(listFilepath));
        }

        using var f = new StreamReader(File.OpenRead(listFilepath));
        var list = new SortedSet<string>(StringComparer.OrdinalIgnoreCase);
        var platformPrefix = platform.basePath;
        bool? isCorrectPlatform = platformPrefix == null ? true : null;
        while (!f.EndOfStream) {
            var line = f.ReadLine();
            if (!string.IsNullOrWhiteSpace(line)) {
                var norm = NormalizePath(line);
                isCorrectPlatform ??= norm.StartsWith(platformPrefix!, StringComparison.OrdinalIgnoreCase);
                if (isCorrectPlatform == false) {
                    norm = PathUtils.SwapPlatformPrefix(norm, platform);
                }
                list.Add(norm);
            }
        }

        Files = list.ToArray();
    }

    public bool FileExists(string path) => Array.BinarySearch(Files, path, StringComparer.OrdinalIgnoreCase) >= 0;

    public IEnumerable<string> GetFileExtensionVariants(string path)
    {
        static bool CheckIsSubpath(string path, string comparePath)
        {
            var lastSlash = comparePath.LastIndexOf('/');
            if (lastSlash == -1) return comparePath.StartsWith(path, StringComparison.OrdinalIgnoreCase);

            var compareDir = comparePath.AsSpan(0, lastSlash + 1);
            return path.StartsWith(compareDir, StringComparison.OrdinalIgnoreCase) && comparePath.StartsWith(path, StringComparison.OrdinalIgnoreCase);
        }

        path = PathUtils.GetFilepathWithoutSuffixes(path).ToString();
        path = NormalizePath(path);
        var index = Array.BinarySearch(Files, path, StringComparer.OrdinalIgnoreCase);
        if (index < 0) {
            index = ~index;
            if (index >= Files.Length || !CheckIsSubpath(path, Files[index])) yield break;
        }

        while(index > 0) {
            yield return Files[index];
            ++index;
            if (index >= Files.Length || !CheckIsSubpath(path, Files[index])) {
                yield break;
            }
        }
    }


    public string[] GetFiles(string filter)
    {
        if (QueryHasPatterns(filter)) {
            return FilterAllFiles(filter);
        } else {
            return GetFilesInFolder(filter);
        }
    }

    /// <summary>
    /// Filter paths by a regex pattern. The matcher converts ** into .* to allow for "standard" glob patterns.
    /// </summary>
    public string[] FilterAllFiles(string pattern)
    {
        pattern = pattern.Trim();
        var cacheKey = MurMur3HashUtils.GetPakFilepathHash(pattern);
        if (folderListCache.TryGetValue(cacheKey, out var names)) {
            return names;
        }

        try {
            ParsePattern(PatternFilters, pattern);
            var results = FilterAllFiles(Files, PatternFilters, int.MaxValue).ToArray();
            folderListCache[cacheKey] = results;
            return results;
        } catch (Exception) {
            Log.Error("Failed to parse regex pattern: " + pattern);
            folderListCache[cacheKey] = [];
            return [];
        }
    }

    public static string[] FilterFiles(string[] fileList, string pattern)
    {
        try {
            var patterns = new List<PathFilter>();
            ParsePattern(patterns, pattern);
            return FilterAllFiles(fileList, patterns, int.MaxValue).ToArray();
        } catch (Exception) {
            Log.Error("Failed to parse regex pattern: " + pattern);
            return [];
        }
    }

    public void ResetResultCache()
    {
        folderListCache.Clear();
    }

    public static bool QueryHasPatterns(string pattern)
    {
        if (pattern.StartsWith('+') || pattern.StartsWith('!')) return true;
        if (pattern.Contains('*') || pattern.Contains(" +") || pattern.Contains(" !")) return true;
        return false;
    }


    private static void ParsePattern(List<PathFilter> PatternFilters, string pattern)
    {
        PatternFilters.Clear();
        var span = pattern.AsSpan();
        var space = pattern.IndexOf(' ');
        var segmentStart = 0;

        while (true)
        {
            char next;
            if (space == -1)
            {
                next = 'e';
                space = pattern.Length;
            }
            else
            {
                var nonspace = space;
                while (nonspace < pattern.Length && pattern[nonspace] == ' ') nonspace++;
                next = nonspace >= pattern.Length ? 'e' : pattern[nonspace];
                space = nonspace;
            }
            var segment = span.Slice(segmentStart, space - segmentStart).TrimEnd();
            if (segment.Length == 0)
            {
                if (next == 'e') break;
            }
            var curSegmentType = segment[0];
            if (curSegmentType == '!')
            {
                var segmentStr = segment.Slice(1).ToString();
                PatternFilters.Add((path) => path.IndexOf(segmentStr, StringComparison.OrdinalIgnoreCase) == -1);
                segmentStart = space;
            }
            else if (curSegmentType == '+')
            {
                var segmentStr = segment.Slice(1).ToString();
                PatternFilters.Add((path) => path.IndexOf(segmentStr, StringComparison.OrdinalIgnoreCase) != -1);
                segmentStart = space;
            }
            else if (curSegmentType == 'e' || next == '!' || next == '+' || next == 'e')
            {
                var segmentStr = segment.ToString();
                if (!QueryHasPatterns(segmentStr)) {
                    segmentStr = segmentStr + ".*";
                } else {
                    segmentStr = segmentStr.Replace("**", ".*");
                }

                var regex = new Regex("^" + segmentStr + "$", RegexOptions.IgnoreCase);
                PatternFilters.Add((path) => regex.IsMatch(path));
                if (curSegmentType == 'e') break;
                else
                {
                    segmentStart = space;
                }
            }

            if (next == 'e') break;
            space = pattern.IndexOf(' ', space + 1);
        }
    }

    private static List<string> FilterAllFiles(string[] files, List<PathFilter> filters, int limit)
    {
        var list = new List<string>();
        foreach (var path in files)
        {
            var allow = true;
            foreach (var filter in filters)
            {
                if (!filter.Invoke(path))
                {
                    allow = false;
                    break;
                }
            }
            if (allow)
            {
                list.Add(path);
                if (list.Count >= limit) break;
            }
        }

        return list;
    }

    public string[] GetFilesInFolder(string folder)
    {
        folder = NormalizePath(folder);
        if (string.IsNullOrEmpty(folder)) {
            return GetFolderFileNames(string.Empty);
        }
        return GetFolderFileNames(folder);
    }

    private string[] GetFolderFileNames(string folderNormalized)
    {
        if (folderNormalized.EndsWith('/')) {
            folderNormalized = folderNormalized[..^1];
        }
        var cacheKey = MurMur3HashUtils.GetPakFilepathHash(folderNormalized);
        if (folderListCache.TryGetValue(cacheKey, out var names)) {
            return names;
        }

        var startIndex = Array.BinarySearch(Files, folderNormalized, StringComparer.OrdinalIgnoreCase);
        if (startIndex < 0 && folderNormalized.Length > 0 && !folderNormalized.EndsWith('/')) {
            folderNormalized += "/";
            startIndex = Array.BinarySearch(Files, folderNormalized, StringComparer.OrdinalIgnoreCase);
        }
        if (startIndex >= 0) {
            return folderListCache[cacheKey] = names = [Files[startIndex]];
        } else {
            startIndex = ~startIndex;
            if (startIndex > Files.Length) return [];
        }
        var count = Files.Length;
        var list = new List<string>();

        if (startIndex >= 0 && startIndex < Files.Length && Filter?.Invoke(Files[startIndex]) != false) {
            var subpath = GetSubfolderPath(Files[startIndex], folderNormalized).ToString();
            var commonLen = Math.Min(subpath.Length, folderNormalized.Length) - 1;
            if (commonLen != -1 && subpath.Substring(0, commonLen).Equals(folderNormalized.Substring(0, commonLen), StringComparison.OrdinalIgnoreCase)) {
                list.Add(subpath);
            }
        }
        int endIndex = startIndex + 1;
        while (endIndex < count) {
            var next = Files[endIndex++];
            if (!next.StartsWith(folderNormalized, StringComparison.OrdinalIgnoreCase)) break;
            // possible optimization: use binary search to find the next non-matching entry instead of doing sequential iteration
            if ((list.Count == 0 || !next.StartsWith(list.Last(), StringComparison.OrdinalIgnoreCase) || next.Length > list.Last().Length && next[list.Last().Length] != '/') && Filter?.Invoke(next) != false) {
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
        return path.Replace('\\', '/').TrimEnd().TrimStart('/');
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
        var exactMatch = Array.BinarySearch(Files, NormalizePath(path), StringComparer.OrdinalIgnoreCase);
        return exactMatch < 0 ? PathType.Folder : PathType.File;
    }
}
