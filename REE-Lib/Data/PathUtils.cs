using ReeLib.Common;

namespace ReeLib;

public static class PathUtils
{
    public static REFileFormat ParseFileFormat(ReadOnlySpan<char> filename)
    {
        filename = GetFilenameExtensionWithSuffixes(filename);
        if (filename.IsEmpty) return REFileFormat.Unknown;
        // filename: ".mesh" OR ".mesh.123" OR ".sbnk.1.x64" OR ".sbnk.1.x64.en"

        var versionDot = filename.IndexOf('.');
        if (versionDot == -1) return new REFileFormat(GetFileFormatFromExtension(filename), -1);

        var versionEnd = filename[(versionDot + 1)..].IndexOf('.');
        if (versionEnd == -1) versionEnd = filename.Length;
        else versionEnd += versionDot + 1;

        if (!int.TryParse(filename[(versionDot + 1)..versionEnd], out var version)) {
            return new REFileFormat(GetFileFormatFromExtension(filename), -1);
        }

        var fmt = GetFileFormatFromExtension(filename[..versionDot]);
        return new REFileFormat(fmt, version);
    }

    public static KnownFileFormats GetFileFormatFromExtension(string extension) => GetFileFormatFromExtension(extension.AsSpan());
    public static KnownFileFormats GetFileFormatFromExtension(ReadOnlySpan<char> extension)
    {
        var hash = MurMur3HashUtils.GetHash(extension);
        return FileFormatExtensions.ExtensionHashToEnum(hash);
    }

    public static ReadOnlySpan<char> GetFilepathWithoutSuffixes(ReadOnlySpan<char> filename)
    {
        var extIndex = GetFilenameExtensionStartIndex(filename);
        if (extIndex == -1) return filename;
        var nextDot = filename.Slice(extIndex + 1).IndexOf('.') + extIndex + 1;
        if (nextDot > extIndex) {
            return filename.Slice(0, nextDot);
        }
        return filename;
    }

    public static string GetFilepathWithoutVersion(string filepath)
    {
        var versionDot = filepath.LastIndexOf('.');
        if (versionDot != -1 && int.TryParse(filepath.AsSpan().Slice(versionDot + 1), out _)) {
            return filepath[..versionDot];
        }

        return filepath;
    }

    public static bool IsSameExtension(string? path1, string? path2)
    {
        if (path1 == null || path2 == null) return false;

        var p1 = GetFilenameExtensionWithoutSuffixes(path1);
        var p2 = GetFilenameExtensionWithoutSuffixes(path2);
        return p1.SequenceEqual(p2);
    }

    public static string NormalizeFilepath(this string str)
    {
        return str.Replace('\\', '/');
    }

    public static ReadOnlySpan<char> GetFilenameExtensionWithSuffixes(ReadOnlySpan<char> filename)
    {
        var extIndex = GetFilenameExtensionStartIndex(filename);
        if (extIndex == -1) return filename;
        return filename[extIndex] == '.' ? filename[(extIndex + 1)..] : filename[extIndex..];
    }

    public static ReadOnlySpan<char> GetFilenameExtensionWithoutSuffixes(ReadOnlySpan<char> filename)
    {
        var fullExt = GetFilenameExtensionWithSuffixes(filename);
        var dot = fullExt.IndexOf('.');
        return dot == -1 ? fullExt : fullExt[0..dot];
    }

    public static ReadOnlySpan<char> GetFilepathWithoutExtensionOrVersion(ReadOnlySpan<char> filepath)
    {
        var extIndex = GetFilenameExtensionStartIndex(filepath);
        if (extIndex == -1) return filepath;
        return filepath[extIndex] == '.' ? filepath[..extIndex] : filepath[..(extIndex + 1)];
    }
    public static ReadOnlySpan<char> GetFilenameWithoutExtensionOrVersion(ReadOnlySpan<char> filepath)
    {
        return Path.GetFileName(GetFilepathWithoutExtensionOrVersion(filepath));
    }

    private static int GetFilenameExtensionStartIndex(ReadOnlySpan<char> filename)
    {
        var lastSlash = filename.LastIndexOfAny(['\\', '/']);
        if (lastSlash == -1) {
            lastSlash = 0;
        }

        var period = filename[lastSlash..].IndexOf('.');
        if (period == -1) return filename.IsEmpty || filename.Contains('/') ? -1 : 0;
        return period + lastSlash;
    }

    public static string CombineChunkSubpath(string chunkBasePath, string filepath, bool isX64)
    {
        var leftIsNatives = chunkBasePath.Contains("natives/x64") || chunkBasePath.Contains("natives/stm");
        var rightIsNatives = filepath.StartsWith("natives/x64") || filepath.StartsWith("natives/stm");
        if (leftIsNatives && rightIsNatives) return Path.Combine(chunkBasePath, filepath[("natives/stm".Length + 1)..]);
        if (!leftIsNatives && !rightIsNatives) return Path.Combine(chunkBasePath, isX64 ? "natives/x64" : "natives/stm", filepath);
        return Path.Combine(chunkBasePath, filepath);
    }

    public static string GetExtensionWithoutPeriod(this string path) => path.AsSpan().GetExtensionWithoutPeriod();
    public static string GetExtensionWithoutPeriod(this ReadOnlySpan<char> path)
    {
        var ext = Path.GetExtension(path);
        return ext.IsEmpty ? string.Empty : ext[1..].ToString();
    }

    public static bool IsNativePath(this string path)
    {
        return path.StartsWith("natives/") && ParseFileFormat(path).version != -1;
    }

    /// <summary>
    /// Converts an absolute path into a relative native path (e.g. C:/files/my-game/natives/stm/subpath/file.scn.20 => natives/stm/subpath/file.scn.20).
    /// </summary>
    public static string? GetNativeFromFullFilepath(string filepath)
    {
        filepath = filepath.Replace('\\', '/');
        var nativesStart = filepath.IndexOf("/natives/", StringComparison.OrdinalIgnoreCase);
        if (nativesStart == -1) {
            return null;
        }

        return filepath.Substring(nativesStart + 1);
    }

    /// <summary>
    /// Converts a native path into an internal path (e.g. natives/stm/subpath/file.scn.20 => subpath/file.scn). Returns the same path if it's already an internal path.
    /// </summary>
    public static string GetInternalFromNativePath(string nativePath)
    {
        return RemoveNativesFolder(GetFilepathWithoutSuffixes(nativePath).ToString());
    }

    /// <summary>
    /// Removes a natives/ prefix or suffix from the path and normalizes it with forward slashes.
    /// </summary>
    /// <param name="path"></param>
    /// <returns></returns>
    public static string RemoveNativesFolder(string path)
    {
        path = path.Replace('\\', '/');
        if (path.EndsWith('/')) {
            path = path[..^1];
        }
        if (path.EndsWith("/natives/x64", StringComparison.OrdinalIgnoreCase)) {
            path = path.Substring(0, path.IndexOf("/natives/x64", StringComparison.OrdinalIgnoreCase));
        }
        if (path.EndsWith("/natives/stm", StringComparison.OrdinalIgnoreCase)) {
            path = path.Substring(0, path.IndexOf("/natives/stm", StringComparison.OrdinalIgnoreCase));
        }
        if (path.StartsWith("natives/x64/", StringComparison.OrdinalIgnoreCase)) {
            path = path.Substring("natives/x64/".Length);
        }
        if (path.StartsWith("natives/stm/", StringComparison.OrdinalIgnoreCase)) {
            path = path.Substring("natives/stm/".Length);
        }
        return path;
    }

    public static string ChangeFileVersion(ReadOnlySpan<char> filepath, int version)
    {
        var fmt = ParseFileFormat(filepath);
        if (fmt.version == version) return filepath.ToString();
        if (fmt.version == -1) return $"{filepath}.{version}";

        var extIndex = GetFilenameExtensionStartIndex(filepath);
        var prevVersionStr = $".{fmt.version}";
        var newVersionStr = $".{version}";

        return string.Concat(filepath.Slice(0, extIndex), filepath.Slice(extIndex).ToString().Replace(prevVersionStr, newVersionStr));
    }
}

public record struct REFileFormat(KnownFileFormats format, int version)
{
    public static readonly REFileFormat Unknown = new REFileFormat(KnownFileFormats.Unknown, -1);
}
