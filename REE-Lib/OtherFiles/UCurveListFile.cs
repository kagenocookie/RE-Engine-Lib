namespace ReeLib
{
    public class UCurveListFile(FileHandler fileHandler) : BaseFile(fileHandler)
    {
        public const int Magic = 0x736C6375;

        public int version;
        public List<string> Paths { get; } = new();
        public int ukn1;
        public int ukn2;

        protected override bool DoRead()
        {
            var handler = FileHandler;
            Paths.Clear();

            var magic = handler.Read<int>();
            if (magic != Magic) throw new InvalidDataException("Invalid ucurvelist file");
            handler.Read(ref version);
            int pathCount = handler.Read<int>();
            handler.ReadNull(4);

            var offset1 = handler.Read<long>();
            var offset2 = handler.Read<long>();
            var pathsOffset = handler.Read<long>();
            handler.ReadNull(8);

            handler.Seek(offset1);
            handler.ReadNull(4);
            handler.Read(ref ukn1);
            handler.ReadNull(8);

            handler.Seek(pathsOffset);
            for (int i = 0; i < pathCount; ++i) Paths.Add(handler.ReadOffsetWString());

            handler.Seek(offset2);
            handler.ReadNull(8);
            handler.Read(ref ukn2);
            handler.ReadNull(4);

            return true;
        }

        protected override bool DoWrite()
        {
            var handler = FileHandler;
            handler.Write(Magic);
            handler.Write(ref version);
            handler.Write(Paths.Count);
            handler.WriteNull(4);

            var offsetsStart = handler.Tell();

            handler.Skip(32);
            handler.Write(offsetsStart, handler.Tell());
            handler.WriteNull(4);
            handler.Write(ref ukn1);
            handler.WriteNull(8);

            handler.Write(offsetsStart + 16, handler.Tell());
            foreach (var path in Paths) handler.WriteOffsetWString(path);
            handler.StringTableFlush();

            handler.Align(16);
            handler.Write(offsetsStart + 8, handler.Tell());
            handler.WriteNull(8);
            handler.Write(ref ukn2);
            handler.WriteNull(4);

            return true;
        }
    }
}

