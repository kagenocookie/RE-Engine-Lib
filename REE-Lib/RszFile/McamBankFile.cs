namespace ReeLib.McamBank
{
    public class McamBankEntry : BaseModel
    {
        public long BankID;
        public uint BankType;
        public long BankTypeMaskBits;

        public int Version { get; set; }
        public string Path { get; set; } = string.Empty;

        public McamBankEntry(int version)
        {
            Version = version;
        }

        protected override bool DoRead(FileHandler handler)
        {
            Path = handler.ReadOffsetWString();
            if (Version >= 3)
            {
                BankID = handler.ReadInt();
                handler.Read(ref BankType);
            }
            else
            {
                handler.Read(ref BankID);
            }
            handler.Read(ref BankTypeMaskBits);
            return true;
        }

        protected override bool DoWrite(FileHandler handler)
        {
            handler.WriteOffsetWString(Path);
            if (Version >= 3)
            {
                handler.WriteInt((int)BankID);
                handler.Write(ref BankType);
            }
            else
            {
                handler.Write(ref BankID);
            }
            handler.Write(ref BankTypeMaskBits);
            return true;
        }

        public override string ToString() => $"{BankID}:{BankType} - {Path}";
    }
}


namespace ReeLib
{
    using ReeLib.McamBank;

    public class McamBankFile(FileHandler fileHandler) : BaseFile(fileHandler)
    {
        public int version = fileHandler.FileVersion;

        public List<McamBankEntry> Items { get; } = new();

        public const uint Magic = 0x6B6E6263;

        protected override bool DoRead()
        {
            FileHandler handler = FileHandler;
            handler.Read(ref version);
            var magic = handler.Read<int>();
            if (magic != Magic)
            {
                throw new InvalidDataException($"{handler.FilePath} Not a mcambank file");
            }
            handler.ReadNull(8);

            var listOffset = handler.Read<long>();
            handler.ReadNull(8);
            if (version >= 3) handler.ReadNull(8);
            var count = handler.Read<int>();

            Items.Clear();
            handler.Seek(listOffset);
            for (int i = 0; i < count; i++)
            {
                McamBankEntry item = new(version);
                item.Read(handler);
                Items.Add(item);
            }

            return true;
        }

        protected override bool DoWrite()
        {
            FileHandler handler = FileHandler;
            handler.Clear();

            handler.Write(ref version);
            handler.Write(Magic);
            handler.WriteNull(8);

            long motlistsOffsetStart = handler.Tell();
            handler.Skip(8);

            handler.WriteNull(8);
            if (version >= 3) handler.WriteNull(8);
            handler.Write(Items.Count);
            handler.Align(16);

            handler.Write(motlistsOffsetStart, handler.Tell());
            Items.Write(handler);
            handler.StringTableFlush();
            return true;
        }
    }
}
