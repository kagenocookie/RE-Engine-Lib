namespace ReeLib
{
    public class VtxaFile(FileHandler handler) : BaseFile(handler)
    {
        public int type;
        public string MeshPath { get; set; } = "";
        public byte[] Buffer { get; set; } = [];

        protected override bool DoRead()
        {
            var handler = FileHandler;
            handler.Read(ref type);
            var size = handler.Read<int>();
            var bufferOffset = handler.Read<long>();
            MeshPath = handler.ReadOffsetWString();
            Buffer = new byte[size];
            return true;
        }

        protected override bool DoWrite()
        {
            var handler = FileHandler;
            handler.Write(type);
            handler.Write(Buffer.Length);
            var bufferOffset = handler.Tell() + 16;
            handler.Write(bufferOffset);
            handler.WriteOffsetWString(MeshPath);
            handler.WriteArray(Buffer);
            handler.Align(8);
            handler.StringTableFlush();
            return true;
        }
    }
}