using ReeLib.Motbank;

namespace ReeLib.Motbank
{
    public class MotbankEntry : BaseModel
    {
        public long BankID;
        public uint BankType;
        public long BankTypeMaskBits;

        public int Version { get; set; }
        public string Path { get; set; } = string.Empty;

        public MotbankEntry(int version)
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
    public class MotbankFile(FileHandler fileHandler) : BaseFile(fileHandler)
    {
        public int version = fileHandler.FileVersion;
        public uint magic = Magic;

        public string UvarPath { get; set; } = string.Empty;
        public string JmapPath { get; set; } = string.Empty;
        public List<MotbankEntry> MotlistItems { get; } = new();

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
            handler.ReadNull(8);
            var motlistsOffset = handler.Read<long>();
            UvarPath = handler.ReadOffsetWStringNullable() ?? "";
            if (version >= 3)
            {
                JmapPath = handler.ReadOffsetWStringNullable() ?? "";
            }
            var motlistCount = handler.Read<int>();

            MotlistItems.Clear();
            handler.Seek(motlistsOffset);
            for (int i = 0; i < motlistCount; i++)
            {
                MotbankEntry item = new(version);
                item.Read(handler);
                MotlistItems.Add(item);
            }

            return true;
        }

        protected override bool DoWrite()
        {
            FileHandler handler = FileHandler;
            handler.Clear();

            handler.Write(ref version);
            handler.Write(ref magic);
            handler.WriteNull(8);

            long motlistsOffsetStart = handler.Tell();
            handler.Skip(8);
            if (string.IsNullOrEmpty(UvarPath))
            {
                handler.WriteInt64(0);
            }
            else
            {
                handler.WriteOffsetWString(UvarPath);
            }

            if (version >= 3)
            {
                if (string.IsNullOrEmpty(JmapPath))
                {
                    handler.WriteInt64(0);
                }
                else
                {
                    handler.WriteOffsetWString(JmapPath);
                }
            }

            handler.Write(MotlistItems.Count);
            handler.StringTableFlush();

            handler.Align(16);
            handler.Write(motlistsOffsetStart, handler.Tell());
            MotlistItems.Write(handler);
            handler.StringTableFlush();
            return true;
        }
    }
}
