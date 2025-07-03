using System.Diagnostics;
using System.Text.Json;
using ReeLib.Common;
using ReeLib.Il2cpp;

namespace ReeLib;

public sealed partial class Workspace : IDisposable
{
    private TypeCache LoadTypeCache()
    {
        var time = Stopwatch.StartNew();
        _typeCache = new TypeCache();
        if (!config.Resources.TryGetIl2cppCachePath(out var baseCacheFile) || !TryDeserialize(baseCacheFile, out _typeCache)) {
            RegenerateTypeCache(Config.GuessIl2cppDumpPath(), baseCacheFile);
            Console.WriteLine("Regenerated source il2cpp data in " + time.Elapsed);
        } else {
            var cacheLastUpdate = File.GetLastWriteTimeUtc(baseCacheFile);
            var il2cppPath = Config.GuessIl2cppDumpPath();
            var il2cppLastUpdate = !File.Exists(il2cppPath) ? DateTime.MinValue : File.GetLastWriteTimeUtc(il2cppPath);
            if (il2cppLastUpdate > cacheLastUpdate) {
                RegenerateTypeCache(Config.GuessIl2cppDumpPath(), baseCacheFile);
                Console.WriteLine("Regenerated source il2cpp data in " + time.Elapsed);
            }
        }
        time.Restart();

        _typeCache ??= new();
        var success = TryApplyTypeCache(_typeCache, baseCacheFile);
        if (!success) {
            RegenerateTypeCache(Config.GuessIl2cppDumpPath(), baseCacheFile);
            Console.WriteLine("Regenerated source il2cpp data in " + time.Elapsed);
            success = TryApplyTypeCache(_typeCache, baseCacheFile);
        }
        // TryApplyEnumOverrides(_il2cpp, paths.EnumOverridesDir);
        if (success) {
            Console.WriteLine("Loaded cached il2cpp data in " + time.Elapsed);
        } else {
            Console.Error.WriteLine("Failed to load il2cpp cache data from " + baseCacheFile);
        }
        // TryApplyTypePatches(_il2cpp, paths.TypePatchFilepath);
        return _typeCache;
    }

    private void RegenerateTypeCache(string? il2cppPath, string cachePath)
    {
        if (!File.Exists(il2cppPath)) {
            Console.Error.WriteLine($"Il2cpp file does not exist, nor do we have a valid cache file for {Config.Game}. Enums and class names won't resolve properly.");
            return;
        }

        _typeCache ??= new();
        var entries = DeserializeOrNull<Il2cppDump>(il2cppPath)
            ?? throw new Exception("File is not a valid il2cpp dump json file");
        _typeCache.ApplyIl2cppData(entries);
        // TryApplyTypePatches(_il2cpp, paths.TypePatchFilepath);

        Console.WriteLine("Updating il2cpp cache... " + cachePath);
        Directory.CreateDirectory(Path.GetDirectoryName(cachePath)!);
        using var outfs = File.Create(cachePath);
        JsonSerializer.Serialize(outfs, _typeCache.ToCacheData(), jsonOptions);
        outfs.Close();
    }

    private bool TryApplyTypePatches(TypeCache target, string patchFilename)
    {
        // TODO
        if (!File.Exists(patchFilename)) return false;

        if (TryDeserialize<Dictionary<string, TypePatch>>(patchFilename, out var patches)) {
            target.ApplyPatches(patches);
            return true;
        }
        return false;
    }

    private bool TryApplyTypeCache(TypeCache target, string cacheFilename)
    {
        if (TryDeserialize<TypeCacheData>(cacheFilename, out var data)) {
            if (data.CacheVersion < TypeCacheData.CurrentCacheVersion) {
                Console.Error.WriteLine("Il2cpp cache data is out of date, needs a rebuild.");
                return false;
            }
            target.ApplyCacheData(data);
            return true;
        }
        return false;
    }

    private bool TryApplyEnumOverrides(TypeCache target, string sourceDir)
    {
        // TODO
        if (!Directory.Exists(sourceDir)) return false;

        var files = Directory.EnumerateFiles(sourceDir, "*.json");
        foreach (var file in files) {
            if (TryDeserialize<EnumOverrideRoot>(file, out var root)) {
                var classname = Path.GetFileNameWithoutExtension(file);
                if (target.enums.TryGetValue(classname, out var enumDesc)) {
                    if (root.DisplayLabels != null) {
                        enumDesc.ParseCacheData(root.DisplayLabels);
                    }
                }
            }
        }
        return true;
    }
}