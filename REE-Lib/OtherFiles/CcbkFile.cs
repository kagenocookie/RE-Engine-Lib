namespace ReeLib
{
    public class CcbkFile : BaseFile
    {
        public List<ResourceID> Resources { get; } = new();

        private const int Magic = 0x4B424343;

        public record class ResourceID
        {
            public int ID;
            public string path = "";
        }

        public CcbkFile(FileHandler fileHandler) : base(fileHandler)
        {
        }

        protected override bool DoRead()
        {
            var handler = FileHandler;
            if (handler.Read<int>() != Magic) throw new InvalidDataException("Invalid ccbk file");

            var count = handler.Read<int>();
            var idOffset = handler.Read<long>();
            var pathsOffset = handler.Read<long>();
            handler.Seek(idOffset);
            for (int i = 0; i < count; ++i)
                Resources.Add(new ResourceID() { ID = handler.Read<int>() });

            handler.Seek(pathsOffset);
            for (int i = 0; i < count; ++i)
                Resources[i].path = handler.ReadOffsetWString();

            return true;
        }

        protected override bool DoWrite()
        {
            var handler = FileHandler;
            handler.Write(Magic);
            handler.Write(Resources.Count);
            var idsOffset = handler.Tell();
            handler.Skip(16);
            handler.Write(idsOffset, handler.Tell());
            foreach (var res in Resources) handler.Write(res.ID);

            handler.Write(idsOffset + 8, handler.Tell());
            foreach (var res in Resources) handler.WriteOffsetWString(res.path);

            handler.StringTableFlush();
            return true;
        }
    }
}