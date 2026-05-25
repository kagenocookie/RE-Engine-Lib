namespace ReeLib.DlgList
{
    public class Header : ReadWriteModel
    {
        public int version;
        public uint magic = DlgFile.Magic;
        public uint hash;
        public int dialogueCount;
        internal long listOffset;
        internal long rszOffset;
        internal long userdataPathOffset;
        public short ukn1;
        public short ukn2;
        public uint userdataPathHash;

        protected override bool ReadWrite<THandler>(THandler action)
        {
            action.Do(ref version);
            action.Do(ref magic);
            action.Do(ref hash);
            action.Do(ref dialogueCount);
            action.Do(ref listOffset);
            action.Do(ref rszOffset);
            action.Null(8);
            action.Do(ref ukn1);
            action.Do(ref ukn2);
            action.Null(4);
            action.Do(ref userdataPathOffset);
            action.Do(ref userdataPathHash);
            action.Null(4);
            return true;
        }
    }

    public class DialogueListItem : ReadWriteModel
    {
        public string name = "";
        public uint nameHash;
        public string dialoguePath = "";

        protected override bool ReadWrite<THandler>(THandler action)
        {
            action.HandleOffsetWString(ref name);
            action.Do(ref nameHash);
            action.Null(4);
            action.Null(8); // offset?
            action.HandleOffsetWString(ref dialoguePath);
            return true;
        }

        public override string ToString() => $"{name} | {dialoguePath}";
    }
}

namespace ReeLib
{
    using ReeLib.DlgList;

    public class DlgListFile(RszFileOption option, FileHandler handler) : BaseRszFile(option, handler)
    {
        public Header Header { get; } = new();
        public List<DialogueListItem> Dialogues { get; } = new();
        public RSZFile UserData { get; } = new(option, handler);
        public string UserDataPath { get; set; } = "";

        public const uint Magic = 0x4C474C44;

        protected override bool DoRead()
        {
            var handler = FileHandler;
            var header = Header;
            header.Read(handler);
            if (header.magic != Magic) {
                throw new InvalidDataException($"{handler.FilePath} Not a Dialog List file");
            }

            UserDataPath = handler.ReadWString(header.userdataPathOffset);

            handler.Seek(header.listOffset);
            Dialogues.Read(handler, header.dialogueCount);

            ReadRsz(UserData, header.rszOffset);
            return true;
        }

        protected override bool DoWrite()
        {
            var handler = FileHandler;
            var header = Header;
            header.Write(handler);

            header.listOffset = handler.AlignTell();
            Dialogues.Write(handler);

            WriteRsz(UserData, header.rszOffset = handler.AlignTell());

            header.userdataPathOffset = handler.AlignTell();
            handler.WriteWString(UserDataPath);
            handler.StringTableFlush();

            header.Rewrite(handler);
            return true;
        }
    }
}