using ReeLib.InternalAttributes;

namespace ReeLib.Rmesh
{
    [RszGenerate, RszAutoReadWrite, RszAssignVersion, RszVersionedObject(typeof(int))]
    public partial class Header : BaseModel
    {
        [RszPaddingAfter(4)]
        public uint magic = McolFile.Magic;
        public long bvhOffset = 16;
    }
}

namespace ReeLib
{
    using ReeLib.Rmesh;

    public class RmeshFile(FileHandler fileHandler) : BaseFile(fileHandler)
    {
        public readonly Header Header = new();
        public BvhData? bvh;
        public readonly List<string> stringTable = new();

        public const uint Magic = 0x4C4F434D;

        protected override bool DoRead()
        {
            var handler = FileHandler;
            Header.Read(handler);
            bvh = new BvhData(handler.WithOffset(handler.Tell())) { Embedded = true };
            if (handler.Position == handler.FileSize()) return true;

            bvh.Read();

            return true;
        }

        protected override bool DoWrite()
        {
            var handler = FileHandler;
            Header.Write(handler);
            Header.bvhOffset = handler.Tell();
            if (bvh != null) {
                bvh.WriteTo(handler.WithOffset(Header.bvhOffset), false);
            }

            Header.Rewrite(handler);
            return true;
        }
    }
}