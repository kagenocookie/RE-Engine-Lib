using RszTool.Motbank;

namespace RszTool.Motbank
{
    public class MotlistItem : BaseModel
    {
        public long offset;
        public long BankID;
        public int uknInt;
        public long ukn;

        public int Version { get; set; }
        public string Path { get; set; } = string.Empty;

        public MotlistItem(int version)
        {
            Version = version;
        }

        protected override bool DoRead(FileHandler handler)
        {
            handler.Read(ref offset);
            if (Version == 3)
            {
                BankID = handler.ReadInt();
                handler.Read(ref uknInt);
            }
            else
            {
                handler.Read(ref BankID);
            }
            handler.Read(ref ukn);
            Path = handler.ReadWString(offset);
            return true;
        }

        protected override bool DoWrite(FileHandler handler)
        {
            handler.WriteOffsetWString(Path);
            if (Version == 3)
            {
                handler.WriteInt((int)BankID);
                handler.Write(ref uknInt);
            }
            else
            {
                handler.Write(ref BankID);
            }
            handler.Write(ref ukn);
            return true;
        }
    }
}


namespace RszTool
{
    public class MotbankFile(RszFileOption option, FileHandler fileHandler) : BaseRszFile(option, fileHandler)
    {
        public int version;
        public uint magic;
        // Skip(8);
        public long motlistsOffset;
        public long uvarOffset;
        public long ukn;
        public int motlistCount;

        public string UvarPath { get; set; } = string.Empty;
        public List<MotlistItem> MotlistItems { get; } = new();

        public const uint Magic = 0x6B6E626D;
        public const string Extension = ".motbank";

        protected override bool DoRead()
        {
            FileHandler handler = FileHandler;
            handler.Read(ref version);
            handler.Read(ref magic);
            if (magic != Magic)
            {
                throw new InvalidDataException($"{handler.FilePath} Not a motbank file");
            }
            handler.Skip(8);
            handler.Read(ref motlistsOffset);
            handler.Read(ref uvarOffset);
            if (version == 3)
            {
                handler.Read(ref ukn);
            }
            handler.Read(ref motlistCount);

            UvarPath = handler.ReadWString(uvarOffset);
            handler.Seek(motlistsOffset);
            for (int i = 0; i < motlistCount; i++)
            {
                MotlistItem item = new(version);
                item.Read(handler);
                MotlistItems.Add(item);
            }

            return true;
        }

        protected override bool DoWrite()
        {
            FileHandler handler = FileHandler;
            handler.Clear();

            motlistCount = MotlistItems.Count;

            handler.Write(ref version);
            handler.Write(ref magic);
            handler.Skip(8);

            long motlistsOffsetStart = handler.Tell();
            handler.Write(ref motlistsOffset);
            handler.WriteOffsetWString(UvarPath);
            if (version == 3)
            {
                handler.Write(ref ukn);
            }
            handler.Write(ref motlistCount);
            handler.StringTableFlush();

            handler.Align(16);
            motlistsOffset = handler.Tell();
            handler.Write(motlistsOffsetStart, motlistsOffset);
            MotlistItems.Write(handler);
            handler.StringTableFlush();
            return true;
        }
    }
}
