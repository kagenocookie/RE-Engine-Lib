namespace ReeLib;

using ReeLib.Common;

public static class PakUtils
{
    public const string ManifestFilepath = "__MANIFEST/MANIFEST.TXT";

    public static ulong GetFilepathHash(string filepath)
    {
        filepath = filepath.Replace('\\', '/');
        var hashLower = MurMur3HashUtils.GetHash(filepath.ToLowerInvariant());
        var hashUpper = MurMur3HashUtils.GetHash(filepath.ToUpperInvariant());
        return (ulong)hashUpper << 32 | hashLower;
    }

    /// <summary>
    /// Scans a directory for PAK files that could be unpacked.
    /// </summary>
    /// <param name="directory"></param>
    /// <param name="ignoreModPaks">Whether modded PAK files should be ignored (based on FMM placeholder PAKs)</param>
    /// <returns></returns>
    public static List<string> ScanPakFiles(string directory, bool ignoreModPaks = true)
    {
        var list = new List<string>();

        foreach (var pak in Directory.EnumerateFiles(directory, "*.pak", SearchOption.TopDirectoryOnly)) {
            var path = Path.Combine(directory, pak);
            // size 16 = FMM placeholder paks, after this is mods which we possibly don't want
            if (new FileInfo(path).Length <= 16) {
                if (ignoreModPaks) break;
                continue;
            }

            list.Add(NormalizePath(path));
        }
        // new dlc structure
        var dlcPath = Path.Combine(directory, "dlc");
        if (Directory.Exists(dlcPath)) {
            foreach (var dlcpak in Directory.EnumerateFiles(dlcPath, "*.pak", SearchOption.TopDirectoryOnly)) {
                list.Add(NormalizePath(Path.Combine(dlcPath, dlcpak)));
            }
        }
        // old dlc structure
        var dlcDirs = Directory.EnumerateDirectories(directory);
        foreach (var subdir in dlcDirs) {
            if (!int.TryParse(subdir, out _)) {
                continue;
            }

            var dlcPakFile = Path.Combine(directory, subdir, "re_dlc_000.pak");
            if (File.Exists(dlcPakFile)) {
                list.Add(dlcPakFile);
            }
        }

        return list;
    }

    public static string GetNextPakFilepath(string directory)
    {
        var paks = Directory.EnumerateFiles(directory, "*.pak", SearchOption.TopDirectoryOnly)
            .Select(pak => Path.GetRelativePath(directory, pak))
            .ToHashSet();

        if (!paks.Contains("re_chunk_000.pak")) {
            throw new NotImplementedException("Base pak file re_chunk_000.pak not found in directory " + directory);
        }

        var pakPatchTemplate = "re_chunk_000.pak.patch_{ID}.pak";
        var nextPakId = 0;
        string nextPak;
        do {
            nextPak = pakPatchTemplate.Replace("{ID}", (++nextPakId).ToString("000"));
        } while (paks.Contains(nextPak) || File.Exists(nextPak));

        return Path.Combine(directory, nextPak);
    }

    internal static string NormalizePath(string path)
    {
        return path.Replace('\\', '/');
    }
}