using System.Runtime.InteropServices;
using ReeLib.Grnd;
using ReeLib.InternalAttributes;

namespace ReeLib.Gtl
{
    [RszGenerate, RszAutoReadWrite]
    public partial class Header : BaseModel
    {
        public uint magic = GtlFile.Magic;
        public uint ukn0;
        public uint ukn1;
        public int uknCount;
        public int indicesCount;
        public int count1;
        [RszPaddingAfter(4)]
        public int count2;

        public long indicesOffset;
        public long heightRangesOffset;
        public long dataOffset;
    }

    public struct HeightmapRange
    {
        public float minHeight;
        public float maxHeight;
    }

    public struct HeightmapValueBlock2
    {
        public uint ukn0;
        public uint ukn1;
        public uint ukn2;
        public uint ukn3;
    }

    public struct HeightmapValueBlock3
    {
        public uint ukn0;
        public uint ukn1;
    }

    [RszGenerate, RszAutoReadWrite]
    public partial class GtlData : BaseModel
    {
        public byte index;
        public byte a;
        public byte b;
        public byte c;
        public int ukn1;
        public int ukn2;
        public int ukn3;
        public int ukn4;
        public int ukn5;
        public int ukn6;
        public int ukn7;

        /// <summary>
        /// [y][x] array of height values.
        /// </summary>
        [RszIgnore] public float[][] Heights = [];

        /// <summary>
        /// [y][x] array of whatever this is.
        /// </summary>
        [RszIgnore] public HeightmapValueBlock2[][] Values2 = [];

        /// <summary>
        /// [y][x] array of whatever this is.
        /// </summary>
        [RszIgnore] public HeightmapValueBlock3[][] Values3 = [];

        /// <summary>
        /// [y][x] array of (physics? navigation?) layer indices.
        /// </summary>
        [RszIgnore] public byte[][] Indices = [];

        public void ReadData(FileHandler handler, List<GroundDataHeaders> headers)
        {
            var hh1 = headers[0];
            var hh2 = headers[1];
            var hh3 = headers[2];
            var hh4 = headers[3];

            var cols1 = hh1.valueCount;
            var rows1 = hh1.totalBlocksize / hh1.strideBytes;
            Heights = new float[rows1][];
            for (int y = 0; y < rows1; ++y)
            {
                Heights[y] = new float[cols1];
                for (int x = 0; x < cols1; ++x)
                {
                    Heights[y][x] = handler.Read<ushort>() / (float)ushort.MaxValue;
                }
            }

            var cols2 = hh2.valueCount / 4;
            var rows2 = hh2.totalBlocksize / hh2.strideBytes;
            var skip2 = hh2.strideBytes - Marshal.SizeOf<HeightmapValueBlock2>() * cols2;
            Values2 = new HeightmapValueBlock2[rows2][];
            for (int y = 0; y < rows2; ++y)
            {
                Values2[y] = handler.ReadArray<HeightmapValueBlock2>(cols2);
                handler.Skip(skip2);
            }

            var cols3 = hh3.valueCount / 4;
            var rows3 = hh3.totalBlocksize / hh3.strideBytes;
            Values3 = new HeightmapValueBlock3[rows3][];
            var skip3 = hh3.strideBytes - Marshal.SizeOf<HeightmapValueBlock3>() * cols3;
            for (int y = 0; y < rows3; ++y)
            {
                Values3[y] = handler.ReadArray<HeightmapValueBlock3>(cols3);
                handler.Skip(skip3);
            }

            var cols4 = hh4.valueCount;
            var rows4 = hh4.totalBlocksize / hh4.strideBytes;
            Indices = new byte[rows4][];
            for (int y = 0; y < rows4; ++y)
            {
                Indices[y] = handler.ReadArray<byte>(cols4);
            }
        }
    }
}

namespace ReeLib
{
    using ReeLib.Gtl;

    public class GtlFile : BaseFile
    {
        public readonly Header Header = new();
        public List<int> Indices { get; } = new();
        public List<HeightmapRange> Ranges { get; } = new();
        public List<GtlData> DataItems { get; } = new();

        private readonly List<long> DataOffets = new();

        public const int Magic = 0x444e5247;

        public GtlFile(FileHandler fileHandler) : base(fileHandler)
        {
        }

        protected override bool DoRead()
        {
            var handler = FileHandler;
            Header.Read(handler);

            handler.Seek(Header.indicesOffset);
            Indices.ReadStructList(handler, Header.indicesCount);

            handler.Seek(Header.heightRangesOffset);
            Ranges.ReadStructList(handler, Header.count1);

            handler.Seek(Header.dataOffset);
            DataOffets.ReadStructList(handler, Header.count2);

            return true;
        }

        public void ReadData(GrndFile ground)
        {
            var handler = FileHandler;
            // we need the parent grnd file to get the real content sizes here
            for (int i = 0; i < Header.count2;++i)
            {
                var item = new GtlData();
                handler.Seek(DataOffets[i]);
                item.Read(handler);
                item.ReadData(handler, ground.ContentHeaders);
                DataItems.Add(item);
            }
        }

        protected override bool DoWrite()
        {
            var handler = FileHandler;
            Header.Write(handler);

            Header.Write(handler, 0);
            return false;
        }
    }
}