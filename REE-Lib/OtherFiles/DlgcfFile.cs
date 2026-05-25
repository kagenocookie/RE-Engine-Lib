namespace ReeLib
{
    public class DlgcfFile(FileHandler handler) : BaseFile(handler)
    {
        public long totalDialogueCount;

        public const uint Magic = 0x43474C44;

        protected override bool DoRead()
        {
            var handler = FileHandler;
            var version = handler.Read<int>();
            var magic = handler.Read<int>();
            if (magic != Magic) {
                throw new InvalidDataException($"{handler.FilePath} Not a Dialog Config file");
            }
            handler.ReadNull(8);
            handler.Read(ref totalDialogueCount);
            return true;
        }

        protected override bool DoWrite()
        {
            var handler = FileHandler;
            handler.Write(1);
            handler.Write(Magic);
            handler.WriteNull(8);
            handler.Write(ref totalDialogueCount);
            return true;
        }
    }
}