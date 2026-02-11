using ReeLib.InternalAttributes;

namespace ReeLib.Wel
{
    [RszGenerate, RszAutoReadWrite]
    public partial class EventInfo : BaseModel
    {
        public uint triggerId;
        public uint eventId;

        public uint hash1;
        public uint hash2;

        public byte byte1;
        public byte byte2;

        [RszFixedSizeArray(5)] public short[] data1 = new short[5];
        [RszFixedSizeArray(14)] public sbyte[] data2 = new sbyte[14];
        public uint data3;
        public uint data4;
        public uint data5;
        public uint data6;
        public uint data7;

        public override string ToString() => $"{triggerId} / {eventId}";
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
            if (handler.FileVersion == 10)
            {
                // format 10 has direct strings instead of doing hashes only which is nice but also useless for the rest of the formats that don't have the strings
                throw new NotSupportedException("WEL file format 10 is not supported");
            }

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

            // items need to be sorted to work correctly
            Events.Sort((a, b) => a.triggerId.CompareTo(b.triggerId));
            handler.Write(Events.Count);
            Events.Write(handler);

            return true;
        }
    }
}

