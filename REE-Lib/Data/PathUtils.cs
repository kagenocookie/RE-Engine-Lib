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

    public static ReadOnlySpan<char> GetFilepathWithoutExtensionOrVersion(ReadOnlySpan<char> filename)
    {
        var extIndex = GetFilenameExtensionStartIndex(filename);
        if (extIndex == -1) return filename;
        return filename[extIndex] == '.' ? filename[..extIndex] : filename[..(extIndex + 1)];
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
}

public record struct REFileFormat(KnownFileFormats format, int version)
{
    public static readonly REFileFormat Unknown = new REFileFormat(KnownFileFormats.Unknown, -1);
}
