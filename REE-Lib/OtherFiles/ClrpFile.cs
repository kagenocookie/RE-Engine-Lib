using System.Numerics;

namespace ReeLib
{
    public class ClrpFile(FileHandler fileHandler) : BaseFile(fileHandler)
    {
        public List<Vector3> Positions = [];

        public const int Magic = 0x50524C43;

        protected override bool DoRead()
        {
            var handler = FileHandler;
            var magic = handler.Read<int>();
            if (magic != Magic)
            {
                throw new Exception("Invalid CLRP file");
            }
            var count = handler.Read<int>();
            var dataOffset = handler.Read<long>();
            handler.Seek(dataOffset);
            Positions.ReadStructList(handler, count);
            return true;
        }

        protected override bool DoWrite()
        {
            var handler = FileHandler;
            handler.Write(Magic);
            handler.Write(Positions.Count);
            handler.Write(handler.Tell() + 8);
            Positions.Write(handler);
            return true;
        }
    }
}