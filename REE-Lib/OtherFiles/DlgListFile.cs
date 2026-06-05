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
        public short ukn1;
        public short userFileCount;

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
            action.Do(ref userFileCount);
            action.Null(4);
            return true;
        }
    }

    public class DialogueListItem : ReadWriteModel
    {
        public string dialoguePath = "";

        protected override bool ReadWrite<THandler>(THandler action)
        {
            action.Null(8); // offset?
            action.HandleOffsetWString(ref dialoguePath);
            return true;
        }

        public override string ToString() => dialoguePath;
    }
}

namespace ReeLib
{
    using ReeLib.DlgList;

    public class DlgListFile(RszFileOption option, FileHandler handler) : BaseRszFile(option, handler)
    {
        public Header Header { get; } = new();
        public string Name { get; set; } = "";
        public uint NameHash { get; set; }
        public List<DialogueListItem> Dialogues { get; } = new();
        public RSZFile UserData { get; } = new(option, handler);
        public List<(string path, uint hash)> UserDataPaths { get; } = new();

        public const uint Magic = 0x4C474C44;

        protected override bool DoRead()
        {
            var handler = FileHandler;
            var header = Header;
            header.Read(handler);
            if (header.magic != Magic) {
                throw new InvalidDataException($"{handler.FilePath} Not a Dialog List file");
            }

            var tmlPathOffsets = handler.ReadArray<long>((int)header.userFileCount);
            for (int i = 0; i < header.userFileCount; i++) {
                UserDataPaths.Add((handler.ReadWString(tmlPathOffsets[i]), 0));
            }
            for (int i = 0; i < header.userFileCount; i++) {
                UserDataPaths[i] = UserDataPaths[i] with { hash = handler.Read<uint>() };
            }

            handler.Seek(header.listOffset);
            Name = handler.ReadOffsetWString();
            NameHash = handler.Read<uint>();
            handler.ReadNull(4);
            Dialogues.Read(handler, header.dialogueCount);

            ReadRsz(UserData, header.rszOffset);
            return true;
        }

        protected override bool DoWrite()
        {
            var handler = FileHandler;
            var header = Header;
            header.Write(handler);

            header.userFileCount = (short)UserDataPaths.Count;
            foreach (var path in UserDataPaths) handler.WriteOffsetWString(path.path);
            foreach (var path in UserDataPaths) handler.Write(path.hash);

            header.listOffset = handler.AlignTell();
            handler.WriteOffsetWString(Name);
            handler.Write(NameHash);
            handler.WriteNull(4);
            Dialogues.Write(handler);

            WriteRsz(UserData, header.rszOffset = handler.AlignTell());

            handler.StringTableFlush();

            header.Rewrite(handler);
            return true;
        }
    }
}