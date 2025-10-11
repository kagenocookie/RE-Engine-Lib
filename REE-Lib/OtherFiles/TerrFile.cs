using ReeLib.InternalAttributes;

namespace ReeLib.Terr
{
    [RszGenerate, RszAutoReadWrite, RszAssignVersion, RszVersionedObject(typeof(int))]
    public partial class Header : BaseModel
    {
        public uint magic = TerrFile.Magic;
        public uint bvhSize;
        public int typeCount;
        public int guidCount;
        [RszVersion(10008, EndAt = nameof(guidsOffset))]
        public Guid guid;
        public long typesOffset;
        public long guidsOffset;
    }

    [RszGenerate, RszAutoReadWrite]
    public partial class TerrainType : BaseModel
    {
        private long unkn;
        private int guid1count;
        private int guid2count;
        public Guid guid;
        private long guids1Offset;
        private long guids2Offset;

        [RszIgnore] public readonly List<Guid> Guids1 = new();
        [RszIgnore] public readonly List<Guid> Guids2 = new();

        public void ReadGuids(FileHandler handler)
        {
            if (guid1count > 0)
            {
                handler.Seek(guids1Offset);
                Guids1.ReadStructList(handler, guid1count);
            }
            if (guid2count > 0)
            {
                handler.Seek(guids2Offset);
                Guids2.ReadStructList(handler, guid2count);
            }
        }

        public void WriteGuids(FileHandler handler)
        {
            guid1count = Guids1.Count;
            guid2count = Guids2.Count;
            guids1Offset = handler.Tell();
            Guids1.Write(handler);
            guids2Offset = handler.Tell();
            Guids2.Write(handler);
        }
    }
}

namespace ReeLib
{
    using ReeLib.Terr;

    public class TerrFile : BaseFile
    {
        public readonly Header Header = new();
        public BvhData? bvh;
        public List<TerrainType> Types { get; } = new();
        public List<Guid> Guids { get; } = new();

        public const int Magic = 0x52524554;

        public TerrFile(FileHandler fileHandler) : base(fileHandler)
        {
        }

        protected override bool DoRead()
        {
            var handler = FileHandler;
            Header.Read(handler);
            bvh = new BvhData(handler.WithOffset(handler.Tell())) { Embedded = true };
            if (handler.Position == handler.FileSize()) return true;

            bvh?.Read();

            Types.Clear();
            Guids.Clear();
            if (Header.typesOffset > 0)
            {
                handler.Seek(Header.typesOffset);
                Types.Read(handler, Header.typeCount);

                foreach (var type in Types)
                {
                    type.ReadGuids(handler);
                }
            }
            if (Header.guidsOffset > 0)
            {
                handler.Seek(Header.guidsOffset);
                Guids.ReadStructList(handler, Header.guidCount);
            }

            return true;
        }

        protected override bool DoWrite()
        {
            var handler = FileHandler;
            Header.Write(handler);

            if (bvh != null)
            {
                bvh.FileHandler = handler;
                bvh.Write();
            }

            if (handler.FileVersion > 4)
            {
                handler.Align(16);
                Header.typesOffset = handler.Tell();
                Types.Write(handler);

                foreach (var type in Types)
                {
                    type.WriteGuids(handler);
                    type.Write(handler, type.Start);
                }

                Header.guidsOffset = handler.Tell();
                Guids.Write(handler);
            }

            Header.Write(handler, 0);

            return true;
        }
    }
}