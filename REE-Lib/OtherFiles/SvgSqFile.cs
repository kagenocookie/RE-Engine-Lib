namespace ReeLib
{
    public class SvgSqFile(FileHandler handler) : BaseFile(handler)
    {
        public List<string> SvgFiles { get; } = new();

        public const uint Magic = 0x53475653;

        protected override bool DoRead()
        {
            var handler = FileHandler;
            var version = handler.Read<int>();
            var magic = handler.Read<uint>();
            if (magic != Magic) {
                throw new InvalidDataException($"{handler.FilePath} Not a Svg sequence file");
            }
            var count = handler.Read<int>();
            handler.ReadNull(4);
            var offsets = handler.ReadArray<long>(count);

            foreach (var off in offsets) {
                SvgFiles.Add(handler.ReadWString(off));
            }

            return true;
        }

        protected override bool DoWrite()
        {
            var handler = FileHandler;
            handler.Write(Magic);
            handler.Write(0);
            handler.Write(SvgFiles.Count);
            handler.WriteNull(4);
            foreach (var file in SvgFiles) {
                handler.WriteOffsetWString(file);
            }
            handler.StringTableFlush();
            return true;
        }
    }
}