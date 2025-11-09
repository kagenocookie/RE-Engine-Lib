namespace ReeLib
{
    public class CmatFile : BaseFile
    {
        public Guid materialGuid;
        public List<Guid> Attributes = [];

        private const int Magic = 0x54414D43;

        public CmatFile(FileHandler fileHandler) : base(fileHandler)
        {
        }

        protected override bool DoRead()
        {
            var handler = FileHandler;
            var magic = handler.Read<int>();
            if (magic != Magic)
            {
                throw new Exception("Invalid CMAT file");
            }

            Attributes.Clear();

            var attributeCount = handler.Read<int>();
            var attributesOffset = handler.Read<long>();
            handler.Read(ref materialGuid);
            handler.Seek(attributesOffset);
            Attributes.ReadStructList(handler, attributeCount);
            return true;
        }

        protected override bool DoWrite()
        {
            var handler = FileHandler;
            handler.Write(Magic);
            handler.Write(Attributes.Count);
            handler.Write(handler.Tell() + 24); // attribute offset
            handler.Read(ref materialGuid);
            Attributes.Write(handler);
            return true;
        }
    }
}