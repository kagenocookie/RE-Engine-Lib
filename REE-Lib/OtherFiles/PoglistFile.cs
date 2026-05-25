namespace ReeLib
{
    public class PoglistFile(FileHandler handler) : BaseFile(handler)
    {
        public List<string> PogFiles { get; } = new();

        public const uint Magic = 0x004C4750;
        public uint hash;

        protected override bool DoRead()
        {
            var handler = FileHandler;
            var magic = handler.Read<uint>();
            var version = handler.Read<int>();
            if (magic != Magic) {
                throw new InvalidDataException($"{handler.FilePath} Not a Pog list file");
            }
            var count = handler.Read<int>();
            handler.Read(ref hash);
            handler.Seek(handler.Read<long>());
            var offsets = handler.ReadArray<long>(count);

            foreach (var off in offsets) {
                PogFiles.Add(handler.ReadWString(off));
            }

            return true;
        }

        protected override bool DoWrite()
        {
            var handler = FileHandler;
            handler.Write(Magic);
            handler.Write(0);
            handler.Write(PogFiles.Count);
            handler.Write(hash);
            handler.Write(handler.Tell() + 8);
            foreach (var file in PogFiles) {
                handler.WriteOffsetWString(file);
            }
            handler.StringTableFlush();
            return true;
        }
    }
}