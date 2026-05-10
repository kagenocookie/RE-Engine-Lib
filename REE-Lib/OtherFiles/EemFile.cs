namespace ReeLib
{
    public class EemFile(FileHandler fileHandler) : BaseFile(fileHandler)
    {
        public List<string> EmitMasks { get; } = new();

        public const int Magic = 0x726D6565;

        protected override bool DoRead()
        {
            var handler = FileHandler;
            var magic = handler.Read<int>();
            if (magic != Magic) {
                throw new InvalidDataException($"{handler.FilePath} Not an EEM file");
            }

            handler.ReadNull(4);
            var bufLen = handler.Read<int>();
            var strEnd = handler.Tell() + bufLen;
            while (handler.Tell() < strEnd) {
                var str = handler.ReadWString(-1, -1, false);
                EmitMasks.Add(str);
            }

            return true;
        }

        protected override bool DoWrite()
        {
            var handler = FileHandler;
            var version = handler.FileVersion;
            handler.Write(Magic);
            handler.WriteNull(4);
            var bufLen = EmitMasks.Sum(s => s.Length + 1);
            handler.Write(bufLen);
            foreach (var str in EmitMasks) {
                handler.WriteWString(str);
            }
            return true;
        }
    }
}