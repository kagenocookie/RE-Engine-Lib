using System.Text.Json.Serialization;
using System.Text.RegularExpressions;
using ReeLib.Common;

namespace ReeLib;

public sealed partial class FileExtensionCache
{
    public Dictionary<string, int> Versions { get; set; }
    public Dictionary<string, FileExtensionInfo> Info { get; set; }

    public FileExtensionCache()
    {
        Versions = new();
        Info = new();
    }

    public FileExtensionCache(Dictionary<string, int> versions)
    {
        Versions = versions;
        Info = versions.ToDictionary(k => k.Key, v => new FileExtensionInfo() { Version = v.Value });
    }

    public FileExtensionCache(Dictionary<string, FileExtensionInfo> info)
    {
        Info = info;
        Versions = info.ToDictionary(k => k.Key, v => v.Value.Version);
    }

    // locale can be en, pt, ptbr, es, es419, zhcn, zhtw, ...
    [GeneratedRegex("\\.(?:x64|stm)\\.([a-z]{2}[a-zA-Z0-9]*)$")]
    private static partial Regex IsLocalizedFileRegex();

    [GeneratedRegex("\\.(?:\\d+)\\.([a-z]{2}[a-zA-Z0-9]*)$")]
    private static partial Regex IsLocalizedNonPlatformFileRegex();

    public void HandleFilepathsFromList(string listFilepath)
    {
        if (!File.Exists(listFilepath)) {
            throw new Exception($"List file '{listFilepath}' not found");
        }
        using var file = new StreamReader(File.OpenRead(listFilepath));
        while (!file.EndOfStream) {
            var line = file.ReadLine();
            if (string.IsNullOrWhiteSpace(line)) continue;

            HandleFilepath(line);
        }
        foreach (var ext in Info) {
            Versions[ext.Key] = ext.Value.Version;
        }
    }

    public void HandleFilepath(string filepath)
    {
        var isLocalized = false;
        string? locale = null;
        var hasStm = false;
        var hasX64 = false;
        if (IsLocalizedFileRegex().IsMatch(filepath)) {
            if (filepath.Contains(".x64.")) {
                hasX64 = true;
            } else {
                hasStm = true;
            }
            isLocalized = true;
            locale = IsLocalizedFileRegex().Match(filepath).Groups[1].Value;
            filepath = Path.GetFileNameWithoutExtension(Path.GetFileNameWithoutExtension(filepath));
        } else if (IsLocalizedNonPlatformFileRegex().IsMatch(filepath)) {
            isLocalized = true;
            locale = IsLocalizedNonPlatformFileRegex().Match(filepath).Groups[1].Value;
            filepath = Path.GetFileNameWithoutExtension(filepath);
        } else if (filepath.EndsWith(".x64")) {
            hasX64 = true;
            filepath = Path.GetFileNameWithoutExtension(filepath);
        } else if (filepath.EndsWith(".stm")) {
            hasStm = true;
            filepath = Path.GetFileNameWithoutExtension(filepath);
        }

        var versionStr = Path.GetExtension(filepath).Replace(".", "");
        var ext = Path.GetExtension(Path.GetFileNameWithoutExtension(filepath)).Replace(".", "");
        if (!string.IsNullOrEmpty(ext)) {
            if (!Info.TryGetValue(ext, out var info)) {
                Info[ext] = info = new FileExtensionInfo() {
                    Type = FileFormatExtensions.ExtensionToEnum(ext),
                };
            }

            info.CanHaveX64 = hasX64 || info.CanHaveX64;
            info.CanHaveStm = hasStm || info.CanHaveStm;
            info.CanNotHaveX64 = !hasX64 && !hasStm || info.CanNotHaveX64;
            info.CanHaveLang = isLocalized || info.CanHaveLang;
            info.CanNotHaveLang = !isLocalized || info.CanNotHaveLang;

            if (locale != null && !info.Locales.Contains(locale)) {
                if (locale == "en") {
                    info.Locales.Insert(0, locale);
                } else {
                    info.Locales.Add(locale);
                }
            }
            if (int.TryParse(versionStr, out var version)) {
                if (info.Version != 0 && info.Version != version) {
                    Log.Error($"Warning: updating .{ext} file version from {info.Version} to {version}");
                }
                info.Version = version;
            }
        }
    }

    public sealed class FileExtensionInfo
    {
        public List<string> Locales { get; set; } = new();
        [JsonConverter(typeof(JsonStringEnumConverter<KnownFileFormats>))]
        public KnownFileFormats Type { get; set; } = KnownFileFormats.Unknown;
        public int Version { get; set; }
        public bool CanHaveX64 { get; set; }
        public bool CanHaveStm { get; set; }
        public bool CanNotHaveX64 { get; set; }
        public bool CanHaveLang { get; set; }
        public bool CanNotHaveLang { get; set; }
    }
}

