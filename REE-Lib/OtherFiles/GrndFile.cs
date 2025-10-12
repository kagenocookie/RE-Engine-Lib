using ReeLib.InternalAttributes;

namespace ReeLib.Grnd
{
    [RszGenerate, RszAutoReadWrite]
    public partial class Header : BaseModel
    {
        public uint magic = GrndFile.Magic;
        public uint hash;
        public float minX;
        public float minZ;
        public float maxX;
        public float maxZ;
        public float minY;
        public float maxY;
        public float unknFlt;
        public int gtlCount;
        public int uknCount;
        public int dataCount;
        public int ukn2;
        public int ukn3;
        public float startX;
        public float startZ;
        public int x;
        public int z;
        public long stringOffset;
        public long dataOffset;
    }

    [RszGenerate, RszAutoReadWrite]
    public partial class GroundDataHeaders : BaseModel
    {
        public int index;
        public int ukn1;
        public int ukn2;
        public int ukn3;
        public int type;
        public int valueCount;
        public int totalBlocksize;
        public int strideBytes;
        public int uknIndex;
        public int ukn9;
    }
}

namespace ReeLib
{
    using ReeLib.Grnd;

    public class GrndFile : BaseFile
    {
        public readonly Header Header = new();
        public List<string> GroundTextures { get; } = new();
        public List<GroundDataHeaders> ContentHeaders { get; } = new();

        public const int Magic = 0x444e5247;

        public GrndFile(FileHandler fileHandler) : base(fileHandler)
        {
        }

        protected override bool DoRead()
        {
            var handler = FileHandler;
            Header.Read(handler);

            handler.Seek(Header.stringOffset);
            for (int i = 0;i < Header.gtlCount; ++i)
            {
                GroundTextures.Add(handler.ReadOffsetWString());
            }

            handler.Seek(Header.dataOffset);
            ContentHeaders.Read(handler, Header.dataCount);

            return true;
        }

        protected override bool DoWrite()
        {
            var handler = FileHandler;
            Header.Write(handler);

            handler.Align(8);
            Header.stringOffset = handler.Tell();
            foreach (var tex in GroundTextures)
            {
                handler.WriteOffsetWString(tex);
            }
            handler.StringTableFlush();

            handler.Align(4);
            Header.dataOffset = handler.Tell();
            ContentHeaders.Write(handler);

            Header.Write(handler, 0);
            return true;
        }
    }
}