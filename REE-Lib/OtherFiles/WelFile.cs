using ReeLib.InternalAttributes;

namespace ReeLib.Wel
{
    [RszGenerate, RszAutoReadWrite]
    public partial class EventInfo : BaseModel
    {
        public uint triggerId;
        public uint eventId;

        public uint jointHash;
        public uint gameObjectHash;

        public bool rotation;
        public bool tracking;
        public short priorityId1;
        public short priorityId2;
        public short priorityId3;
        public short bookingTime;
        public short flangingTimer;
        public byte globalId;
        public byte limit;
        public byte priority;
        public uint mode;
        public short releaseTime;
        public bool disableObsOcl;
        public bool updateObsOcl;
        public bool disableMaxObsOcldistance;
        public bool enableSpaceFeature;
        public bool waitUntilFinished;

        public uint listenerMask;
        public uint freeArea;
        public uint freeArea8to11;
        public ulong freeArea12to15;

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
            Events.Clear();
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

