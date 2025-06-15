using System.Numerics;
using RszTool.InternalAttributes;

namespace RszTool.Mcol
{
    [RszGenerate, RszAutoReadWrite, RszAssignVersion, RszVersionedObject(typeof(int))]
    public partial class Header : BaseModel
    {
        public uint magic = McolFile.Magic;
        public uint bvhSize;
        [RszPaddingAfter(4)]
        public int stringCount;
        [RszVersion(3), RszPaddingAfter(8)]
        public long stringTableOffset;
    }
}

namespace RszTool
{
    using RszTool.Mcol;

    public class McolFile : BaseFile
    {
        public readonly Header Header = new();
        public BvhData? bvh;
        public readonly List<string> stringTable = new();

        public const int Magic = 0x4C4F434D;

        public McolFile(FileHandler fileHandler) : base(fileHandler)
        {
        }

        protected override bool DoRead()
        {
            var handler = FileHandler;
            Header.Read(handler);
            bvh = new BvhData(handler.WithOffset(handler.Tell())) { Embedded = true };
            bvh?.Read();

            handler.Seek(Header.stringTableOffset);
            for (int i = 0; i < Header.stringCount * 2; ++i) {
                stringTable.Add(handler.ReadOffsetWString());
            }
            return true;
        }

        protected override bool DoWrite()
        {
            var handler = FileHandler;
            Header.Write(handler);
            Header.stringCount = bvh?.stringTable.Count ?? 0;
            var bvhStart = handler.Tell();
            if (bvh != null) {
                bvh.WriteTo(handler.WithOffset(bvhStart), false);
                Header.bvhSize = (uint)bvh.Size;
            }

            Header.stringTableOffset = handler.Tell();
            // write the bvh string table again, because apparently mcol does that
            if (bvh != null && handler.FileVersion > 2) {
                bvh.FileHandler = handler;
                bvh.WriteBvhStringTable();
            }
            handler.Seek(0);
            Header.Write(handler);

            return true;
        }
    }
}