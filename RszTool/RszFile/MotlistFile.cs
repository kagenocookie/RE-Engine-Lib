using RszTool.Mot;
using RszTool.Motlist;

namespace RszTool.Motlist
{
    public enum MotlistVersion
    {
        RE7 = 60,
        RE2_DMC5 = 85,
        RE3 = 99,
        MHR_DEMO = 484,
        RE8 = 486,
        RE2_RT = 524,
        RE3_RT = RE2_RT,
        RE7_RT = RE2_RT,
        MHR = 528,
        SF6 = 653,
        RE4 = 663,
    }


    public class Header : BaseModel
    {
        public uint version;
        public uint magic;
        public long padding;
        public long pointersOffset; // AssetsPointer in Tyrant
        public long colOffset; // UnkPointer
        public long motListNameOffset;
        public long UnkPadding;
        public int numMots;

        public MotlistVersion Version { get; set; }
        public string MotListName { get; set; } = string.Empty;

        protected override bool DoRead(FileHandler handler)
        {
            handler.Read(ref version);
            handler.Read(ref magic);
            handler.Read(ref padding);
            handler.Read(ref pointersOffset);
            handler.Read(ref colOffset);
            handler.Read(ref motListNameOffset);
            Version = (MotlistVersion)version;
            if (Version >= MotlistVersion.RE8)
            {
                handler.Read(ref UnkPadding);
            }
            handler.Read(ref numMots);
            MotListName = handler.ReadWString(motListNameOffset);
            return true;
        }

        protected override bool DoWrite(FileHandler handler)
        {
            handler.Write(ref version);
            handler.Write(ref magic);
            handler.Write(ref padding);
            handler.Write(ref pointersOffset);
            handler.Write(ref colOffset);
            handler.WriteOffsetWString(MotListName);
            Version = (MotlistVersion)version;
            if (Version >= MotlistVersion.RE8)
            {
                handler.Write(ref UnkPadding);
            }
            handler.Write(ref numMots);
            return true;
        }
    }


    public class MotIndex : BaseModel
    {
        public uint motClipOffset;  // may point to MotClip
        public ushort motNumber;
        public ushort Switch;
        uint[]? data;

        public MotlistVersion Version { get; set; }

        public MotIndex(MotlistVersion version)
        {
            Version = version;
        }

        public int DataCount => Version >= MotlistVersion.RE8 ? 15 : Version == MotlistVersion.RE7 ? 0 : 3;

        protected override bool DoRead(FileHandler handler)
        {
            handler.Read(ref motClipOffset);
            handler.Read(ref motNumber);
            handler.Read(ref Switch);
            int dataCount = DataCount;
            if (dataCount > 0)
            {
                data = handler.ReadArray<uint>(dataCount);
            }
            return true;
        }

        protected override bool DoWrite(FileHandler handler)
        {
            handler.Write(ref motClipOffset);
            handler.Write(ref motNumber);
            handler.Write(ref Switch);
            if (data != null)
            {
                handler.WriteArray(data);
            }
            return true;
        }
    }
}


namespace RszTool
{
    public class MotlistFile(RszFileOption option, FileHandler fileHandler) : BaseRszFile(option, fileHandler)
    {
        public const uint Magic = 0x74736C6D;
        public const string Extension = ".motlist";

        public Header Header { get; } = new();
        public List<MotFile> MotFiles { get; } = new();
        public List<MotIndex> MotIndices { get; } = new();

        protected override bool DoRead()
        {
            MotFiles.Clear();
            MotIndices.Clear();
            var handler = FileHandler;
            var header = Header;
            if (!header.Read(handler)) return false;
            if (header.magic != Magic)
            {
                throw new InvalidDataException($"{handler.FilePath} Not a motlist file");
            }
            handler.Seek(header.pointersOffset);
            long[] motOffsets = handler.ReadArray<long>(header.numMots);

            HashSet<long> uniqueOffsets = new();
            BoneHeaders? boneHeaders = null;
            for (int i = 0; i < motOffsets.Length; i++)
            {
                if (uniqueOffsets.Add(motOffsets[i]))
                {
                    MotFile motFile = new(Option, handler.WithOffset(motOffsets[i]), boneHeaders);
                    boneHeaders ??= motFile.BoneHeaders;
                    motFile.Read();
                    MotFiles.Add(motFile);
                }
            }

            handler.Seek(header.colOffset);
            for (int i = 0; i < motOffsets.Length; i++)
            {
                MotIndex motIndex = new(header.Version);
                motIndex.Read(handler);
                MotIndices.Add(motIndex);
            }
            return true;
        }

        protected override bool DoWrite()
        {
            throw new NotImplementedException();
        }
    }
}
