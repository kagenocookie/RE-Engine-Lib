using System.Runtime.InteropServices;
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

    public static REFileFormatFull ParseFileFormatFull(ReadOnlySpan<char> filename)
    {
        filename = GetFilenameExtensionWithSuffixes(filename);
        if (filename.IsEmpty) return REFileFormatFull.Unknown;
        // filename: ".mesh" OR ".mesh.123" OR ".sbnk.1.x64" OR ".sbnk.1.x64.en" OR ".whatever.png"

        var versionDot = filename.IndexOf('.');
        if (versionDot == -1) return new REFileFormatFull(GetFileFormatFromExtension(filename), -1, filename.ToString());

        var versionEnd = filename[(versionDot + 1)..].IndexOf('.');
        if (versionEnd == -1) versionEnd = filename.Length;
        else versionEnd += versionDot + 1;

        if (!int.TryParse(filename[(versionDot + 1)..versionEnd], out var version)) {
            var fmtEnum = GetFileFormatFromExtension(filename);
            if (fmtEnum == KnownFileFormats.Unknown) {
                return new REFileFormatFull(fmtEnum, -1, filename.Slice(filename.LastIndexOf('.') + 1).ToString());
            } else {
                return new REFileFormatFull(fmtEnum, -1, filename.ToString());
            }
        }

        var fmt = GetFileFormatFromExtension(filename[..versionDot]);
        return new REFileFormatFull(fmt, version, filename[..versionDot].ToString());
    }

    public static KnownFileFormats GetFileFormatFromExtension(ReadOnlySpan<char> extension)
    {
        var hash = MurMur3HashUtils.GetHashLower(extension);
        return FileFormatExtensions.ExtensionHashToEnum(hash);
    }

    /// <summary>
    /// Removes PAK path-specific suffixes from a file path (version, .x64, languages), leaving only the base file path and file extension.
    /// </summary>
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

    public static ReadOnlySpan<char> GetFilepathWithNormalExtensionOnly(ReadOnlySpan<char> filename)
    {
        var dot1 = GetFilenameExtensionStartIndex(filename);
        if (dot1 == -1) return filename;

        var dot2 = filename.Slice(dot1 + 1).IndexOf('.');
        return dot2 == -1 ? filename : filename[0..(dot1 + 1 + dot2)];
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

    public static string GetExtensionWithoutPeriod(this string path) => path.AsSpan().GetExtensionWithoutPeriod().ToString();
    public static ReadOnlySpan<char> GetExtensionWithoutPeriod(this ReadOnlySpan<char> path)
    {
        var ext = Path.GetExtension(path);
        return ext.IsEmpty ? Span<char>.Empty : ext[1..];
    }

    public static bool IsNativePath(this string path)
    {
        return path.StartsWith("natives/") && ParseFileFormat(path).version != -1;
    }

    /// <summary>
    /// Converts an absolute path into a natives-relative target path (e.g. C:/files/my-game/natives/stm/subpath/file.scn.20 => subpath/file.scn.20).
    /// Returns null if it can't reliably determine where the natives/ path starts.
    /// </summary>
    public static string? GetTargetFromFullFilepath(string filepath)
    {
        filepath = filepath.Normalize();
        var nativesStart = filepath.IndexOf("/natives/", StringComparison.OrdinalIgnoreCase);
        if (nativesStart == -1) {
            return null;
        }

        return RemovePlatformPrefix(filepath.AsSpan(nativesStart + 1)).ToString();
    }

    public static string GetStreamingPath(string path)
    {
        if (path.StartsWith("streaming/", StringComparison.OrdinalIgnoreCase)) {
            return path;
        }
        return "streaming/" + path;
    }

    public static string GetStreamingNativesPath(ReadOnlySpan<char> path, PlatformIdentifier platform)
    {
        path = RemoveNativesFolder(path, platform);
        if (path.StartsWith("streaming/", StringComparison.OrdinalIgnoreCase)) {
            return string.Concat(platform.basePath, path);
        }

        return string.Concat(platform.basePath, "streaming/", path);
    }

    /// <summary>
    /// Removes the streaming/ prefix as well as the platform natives/stm/ prefix from a game-relative file path, leaving only the main target path.
    /// </summary>
    public static ReadOnlySpan<char> GetNonStreamingPath(ReadOnlySpan<char> path)
    {
        path = RemovePlatformPrefix(path);
        if (path.StartsWith("streaming/", StringComparison.OrdinalIgnoreCase)) {
            return path.Slice("streaming/".Length);
        }
        return path;
    }

    /// <summary>
    /// Removes the platform specific natives/ prefix or suffix from the path. Keeps any file extension suffixes intact.
    /// </summary>
    public static ReadOnlySpan<char> RemoveNativesFolder(ReadOnlySpan<char> path, PlatformIdentifier platform)
    {
        path = path.TrimEnd('/');
        if (path.EndsWith(platform.basePath, StringComparison.OrdinalIgnoreCase)) {
            path = path.Slice(0, path.IndexOf(platform.basePath, StringComparison.OrdinalIgnoreCase));
        }
        if (path.StartsWith(platform.basePath, StringComparison.OrdinalIgnoreCase)) {
            path = path.Slice(platform.basePath.Length);
        }
        return path;
    }

    public static ReadOnlySpan<char> RemovePlatformPrefix(ReadOnlySpan<char> nativePath)
    {
        if (nativePath.StartsWith("natives/", StringComparison.OrdinalIgnoreCase)) {
            nativePath = nativePath.Slice(nativePath.IndexOf('/') + 1); // remove natives/
            nativePath = nativePath.Slice(nativePath.IndexOf('/') + 1); // remove stm/
            return nativePath;
        } else {
            return nativePath;
        }
    }

    public static string SwapPlatformPrefix(string nativePath, PlatformIdentifier platform)
    {
        if (nativePath.StartsWith(platform.basePath, StringComparison.OrdinalIgnoreCase)) {
            return nativePath;
        }

        if (!nativePath.StartsWith("natives/", StringComparison.OrdinalIgnoreCase)) {
            return platform.basePath + nativePath;
        }

        if (nativePath.StartsWith(platform.basePath)) {
            return nativePath;
        }

        return string.Concat(platform.basePath, PathUtils.RemovePlatformPrefix(nativePath));
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

    public static implicit operator REFileFormat(REFileFormatFull ff) => new REFileFormat(ff.format, ff.version);
}

public record struct REFileFormatFull(KnownFileFormats format, int version, string extension)
{
    public static readonly REFileFormatFull Unknown = new REFileFormatFull(KnownFileFormats.Unknown, -1, "");

    public override string ToString()
    {
        if (format == KnownFileFormats.Unknown)
        {
            if (version == -1) return $".{extension}";
            return $".{extension}.{version}";
        }

        return $".{extension}.{version} ({format})";
    }
}
