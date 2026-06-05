using ReeLib.InternalAttributes;

namespace ReeLib.Gml
{
    [RszGenerate, RszAutoReadWrite]
    public partial class Header : BaseModel
    {
        public uint magic = GmlFile.Magic;
        public uint version;
        public int dataCount;
        [RszPaddingAfter(8, nameof(version), "!=", 858728217)] // not in DD2
        public int texCount;
        public long texOffset;
        public long dataOffset;
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
        public int bufferCount;
        public int uknNum1;
        public int bufferCount2;
        public int uknNum2;

        public List<GroundMaterialBufferLevel> Buffers { get; } = new();
        public List<GroundMaterialBufferLevel> Buffers2 { get; } = new();

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
                    buffer.Buffers.Add(handler.ReadArray<byte>(realRowSize));
                    handler.Skip(rowSize - realRowSize);
                }
            }
            var bufCount2 = bufferCount2 * lodCount;
            Buffers2.EnsureCapacity(bufCount2);
            for (int i = 0; i < bufCount2; ++i)
            {
                using var _ = handler.SeekJumpBack(handler.Read<long>());
                var offset = handler.Read<long>();
                var rowSize = handler.Read<int>();
                var fullSize = handler.Read<int>();
                var realRowSize = fullSize / count1;
                var rowCount = fullSize / rowSize;
                var buffer = new GroundMaterialBufferLevel();
                Buffers2.Add(buffer);

                handler.Seek(offset);
                buffer.Buffers.EnsureCapacity(rowCount);
                for (int k = 0; k < rowCount; ++k)
                {
                    buffer.Buffers.Add(handler.ReadArray<byte>(realRowSize));
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

        public const int Magic = 0x004c4d47;

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

            return true;
        }

        protected override bool DoWrite()
        {
            var handler = FileHandler;
            Header.Write(handler);
            Header.texCount = Textures.Count;
            Header.dataCount = Data.Count;

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

            Header.Write(handler, 0);
            return true;
        }
    }
}
