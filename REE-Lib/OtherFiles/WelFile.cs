using ReeLib.InternalAttributes;

namespace ReeLib.Wel
{
    [RszGenerate, RszAutoReadWrite]
    public partial class EventInfo : BaseModel
    {
        public uint eventId;
        public uint hash;

        public uint val1;
        public int val2;

        public short val3;
        public short val4;
        public short val5;
        public short val6;

        public byte byte7;
        [RszFixedSizeArray(11)] public short[] data1 = new short[11];
        public byte byte9;
        [RszFixedSizeArray(7)] public short[] data2 = new short[7];

        public override string ToString() => $"{eventId} / {hash}";
    }
}

namespace ReeLib
{
    using ReeLib.Wel;

    public class WelFile(FileHandler fileHandler) : BaseFile(fileHandler)
    {
        public string BankPath = "";
        public List<EventInfo> Events { get; } = new();

        protected override bool DoRead()
        {
            var handler = FileHandler;
            BankPath = handler.ReadWString();
            handler.Seek(512);

            int count = handler.Read<int>();
            Events.Read(handler, count);
            return true;
        }

        protected override bool DoWrite()
        {
            var handler = FileHandler;
            handler.WriteWString(BankPath);
            handler.WritePaddingUntil(512);

            handler.Write(Events.Count);
            Events.Write(handler);

            return true;
        }
    }
}

