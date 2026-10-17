using ReeLib.InternalAttributes;

namespace ReeLib.Gml
{
    [RszGenerate, RszAutoReadWrite]
    public partial class Header : BaseModel
    {
        public uint magic = GmlFile.Magic;
        public uint version;
        public int dataCount;
        public int texCount;
        [RszConditional(nameof(version), "!=", 858718217)]
        [RszPaddingAfter(4, nameof(version), "!=", 858718217)]
        public int indicesCount;
        public long texOffset;
        public long dataOffset;

        [RszConditional(nameof(version), "!=", 858718217)]
        public long indicesOffset;
    }

    [RszGenerate, RszAutoReadWrite]
    public partial class GroundMaterialTexture : BaseModel
    {
        [RszOffsetWString] public string albedoPath = "";
        [RszOffsetWString] public string nrrhPath = "";
        public float x;
        public float x2;
        public float y;
        public float y2;
        public float z;
        public float z2;
        public float flt1;
        public float flt2;
        public float flt3;
        public float flt4;
        public float flt5;
        public float flt6;

        public override string ToString() => albedoPath;
    }

    public partial class GroundMaterialBufferLevel
    {
        public List<byte[]> Buffers = new();
    }

    [RszGenerate]
    public partial class GroundMaterialData : BaseModel
    {
        public int count1;
        public int count2;
        public int lodCount;
        public int ukn4;
        public int uknNum1;
        public int uknNum2;
        public int bufferCount;
        public int uknNum3;

        public List<GroundMaterialBufferLevel> Buffers { get; } = new();

        protected override bool DoRead(FileHandler handler)
        {
            DefaultRead(handler);
            var bufCount1 = bufferCount * lodCount;
            Buffers.EnsureCapacity(bufCount1);
            for (int i = 0; i < bufCount1; ++i)
            {
                using var _ = handler.SeekJumpBack(handler.Read<long>());
                var offset = handler.Read<long>();
                var rowSize = handler.Read<int>();
                var fullSize = handler.Read<int>();
                var realRowSize = fullSize / count1;
                var rowCount = fullSize / rowSize;
                var buffer = new GroundMaterialBufferLevel();
                Buffers.Add(buffer);

                handler.Seek(offset);
                buffer.Buffers.EnsureCapacity(rowCount);
                for (int k = 0; k < rowCount; ++k)
                {
                    buffer.Buffers.Add(handler.ReadBytes(realRowSize));
                    handler.Skip(rowSize - realRowSize);
                }
            }
            return true;
        }

        protected override bool DoWrite(FileHandler handler)
        {
            DefaultWrite(handler);
            throw new NotImplementedException();
        }
    }
}

namespace ReeLib
{
    using ReeLib.Gml;

    public class GmlFile : BaseFile
    {
        public readonly Header Header = new();
        public List<GroundMaterialTexture> Textures { get; } = new();
        public List<GroundMaterialData> Data { get; } = new();
        public List<int> Indices { get; } = new();

        public const uint Magic = 0x004c4d47;

        public GmlFile(FileHandler fileHandler) : base(fileHandler)
        {
        }

        protected override bool DoRead()
        {
            var handler = FileHandler;
            Header.Read(handler);

            handler.Seek(Header.texOffset);
            for (int i = 0;i < Header.texCount; ++i)
            {
                var tex = new GroundMaterialTexture();
                tex.Read(handler, handler.Read<long>());
                Textures.Add(tex);
            }

            handler.Seek(Header.dataOffset);
            for (int i = 0;i < Header.dataCount; ++i)
            {
                var tex = new GroundMaterialData();
                tex.Read(handler, handler.Read<long>());
                Data.Add(tex);
            }

            if (Header.indicesOffset > 0 && Header.indicesCount > 0) {
                handler.Seek(Header.indicesOffset);
                Indices.ReadStructList(handler, Header.indicesCount);
            }

            return true;
        }

        protected override bool DoWrite()
        {
            var handler = FileHandler;
            Header.Write(handler);
            Header.texCount = Textures.Count;
            Header.dataCount = Data.Count;
            Header.indicesCount = Indices.Count;

            Header.texOffset = handler.Tell();
            foreach (var tex in Textures)
            {
                handler.WriteOffsetContent((hh) => tex.Write(hh));
            }

            handler.OffsetContentTableFlush();
            handler.StringTableFlush();

            handler.Align(4);
            Header.dataOffset = handler.Tell();
            foreach (var tex in Data)
            {
                handler.WriteOffsetContent((hh) => tex.Write(hh));
            }

            if (Indices.Count > 0) {
                Header.indicesOffset = handler.Tell();
                Indices.Write(handler);
            }

            Header.Write(handler, 0);
            return true;
        }
    }
}
