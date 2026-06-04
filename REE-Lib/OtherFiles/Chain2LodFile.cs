namespace ReeLib
{
    public class Chain2LodFile(FileHandler handler) : BaseFile(handler)
    {
        public List<LodLevel> LODs { get; } = new();
        public List<short> IDs { get; } = new();

        public struct LodLevel
        {
            public ushort nullNum;
            public bool enableMaybe;
            public byte padding;
            public override string ToString() => $"{enableMaybe}";
        }

        public const uint Magic = 0x646C6863;

        protected override bool DoRead()
        {
            var handler = FileHandler;
            var version = handler.Read<int>();
            var magic = handler.Read<uint>();
            if (magic != Magic) {
                throw new InvalidDataException($"{handler.FilePath} Not a Chain2Lod file");
            }
            handler.ReadNull(8);
            var off1 = handler.Read<long>();
            var count = handler.Read<int>();
            handler.ReadNull(4);
            var off2 = handler.Read<long>();
            handler.ReadNull(20);
            var count2 = handler.Read<int>();
            DataInterpretationException.DebugWarnIf(count2 != count);

            handler.Seek(off1);
            LODs.ReadStructList(handler, count);

            handler.Seek(off2);
            IDs.ReadStructList(handler, count);

            return true;
        }

        protected override bool DoWrite()
        {
            var handler = FileHandler;
            handler.Write(1);
            handler.Write(Magic);
            handler.WriteNull(8);
            handler.Skip(8); // off1
            handler.Write(LODs.Count);
            handler.WriteNull(4);
            handler.Skip(8); // off2
            handler.WriteNull(20);
            handler.Write(LODs.Count);

            handler.Write(16, handler.AlignTell());
            LODs.Write(handler);

            handler.Write(32, handler.AlignTell());
            IDs.Write(handler);

            handler.StringTableFlush();
            return true;
        }
    }
}