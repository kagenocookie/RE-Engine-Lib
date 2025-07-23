using ReeLib.Data;

namespace ReeLib;

public sealed partial class Workspace : IDisposable
{
    private Dictionary<string, Dictionary<string, PrefabGameObjectRefProperty>> LoadPfbRefData()
    {
        if (config.Resources.TryGetPfbRefCachePath(out var baseCacheFile)) {
            TryDeserialize(baseCacheFile, out _refPropCache);
        }

        return _refPropCache ??= new();
    }
}
