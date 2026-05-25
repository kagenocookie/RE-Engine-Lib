using ReeLib.DDS;

namespace ReeLib.RTex
{
    public class RTexHeader : ReadWriteModel
    {
        public uint magic = RTexFile.Magic;
        public int version = 4;
        public int ukn0;
        public DxgiFormat format;
        public int width;
        public int height;
        public int depth;
        public int mipCount;
        public int arraySize;
        public int ukn1;
        public int ukn2;
        public int ukn3;

        public float widthRate;
        public float heightRate;

        protected override bool ReadWrite<THandler>(THandler action)
        {
            action.Do(ref magic);
            action.Do(ref version);
            action.Do(ref ukn0);
            action.Do(ref format);

            action.Do(ref width);
            action.Do(ref height);
            action.Do(ref depth);
            action.Do(ref mipCount);

            action.Do(ref arraySize);
            action.Do(ref ukn1);
            action.Do(ref ukn2);
            action.Null(4);

            if (version >= 5) {
                action.Do(ref ukn3);
                if (version >= 6) {
                    action.Null(4);
                }
                action.Do(ref widthRate);
                action.Do(ref heightRate);
                DataInterpretationException.DebugWarnIf(widthRate <= 0 || heightRate <= 0);
            } else {
                action.Null(16);
            }

            return true;
        }
    }
}


namespace ReeLib
{
    using ReeLib.RTex;

    public class RTexFile(FileHandler handler) : BaseFile(handler)
    {
        public RTexHeader Header { get; } = new();

        public const uint Magic = 0x58455452;

        protected override bool DoRead()
        {
            var handler = FileHandler;
            Header.Read(handler);
            if (Header.magic != Magic) {
                throw new InvalidDataException("Not a valid RTEX file");
            }
            return true;
        }

        protected override bool DoWrite()
        {
            var handler = FileHandler;
            Header.Write(handler);
            return true;
        }
    }
}