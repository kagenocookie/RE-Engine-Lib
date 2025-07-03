using ReeLib.Common;

namespace ReeLib.Tools;

public class FileLookupTools
{
    private static readonly byte[] RSZBytes = [0x52, 0x53, 0x5a, 0];

    public struct InstanceInfoStruct {
        public uint typeId;
        public uint CRC;

        public InstanceInfo GetInstanceInfo() => new InstanceInfo(GameVersion.unknown) { typeId = typeId, CRC = CRC };
    }

    public static IEnumerable<InstanceInfoStruct>? GetRSZInstanceInfos(string filepath)
    {
        // read directly from file for finding the RSZ data - so we don't necesarily fully load every single fully into memory
        using var fs = File.OpenRead(filepath);
        var handler = new FileHandler(fs);
        var rszOffset = handler.FindBytes(RSZBytes, new SearchParam() { end = -1 });
        if (rszOffset == -1) {
            yield break;
        }

        var header = new RSZFile.RszHeader();
        handler.Seek(rszOffset);
        header.Read(handler);
        handler.Seek(rszOffset + header.instanceOffset);
        for (int i = 0; i < header.instanceCount; i++)
        {
            InstanceInfoStruct instanceInfo = new();
            handler.Read(ref instanceInfo);
            yield return instanceInfo;
        }
    }

    public static bool FileCanContainRSZData(string filepath)
    {
        var ext = PathUtils.GetFilenameExtensionWithoutSuffixes(filepath);
        var format = PathUtils.GetFileFormatFromExtension(ext);
        return format is KnownFileFormats.Scene
            or KnownFileFormats.Prefab
            or KnownFileFormats.RequestSetCollider
            or KnownFileFormats.UserData
            || ext.StartsWith("ai") // all ai* files (AIMP) have an RSZ section, though it's empty for the most part
            || ext.StartsWith("w") // many wwise related audio files contain a single main rsz instance similar to user files
            || ext == "swms" // one more audio related file
            ;
    }
}
