using ReeLib.Gcf;
using ReeLib.InternalAttributes;

using Header = ReeLib.StructModel<ReeLib.Msg.HeaderStruct>;

namespace ReeLib.Gcf
{
    [RszGenerate, RszAutoReadWrite, RszVersionedObject(typeof(int)), RszAssignVersion]
    public partial class GcfHeader : BaseModel
    {
        public uint version;
        public uint magic;
        public long dataOffset;
        public long msgListOffset;
        public long offset3;
        public long offset4;
        [RszVersion(19)]
        public long gcpOffset;
        [RszVersion(24)]
        public long uknOffset;
    }
}

namespace ReeLib
{
    public class GcfFile(FileHandler fileHandler) : BaseFile(fileHandler)
    {
        public GcfHeader Header { get; } = new();
        public string DefaultFont = string.Empty;
        public string[] Fonts = Array.Empty<string>();
        public List<string> MessageFiles { get; } = new();

        public const int Magic = 0x47464347;

        protected override bool DoRead()
        {
            var handler = FileHandler;
            Header.Read(handler);
            if (Header.magic != Magic) {
                throw new Exception("Invalid GCF file");
            }
            var version = Header.version;

            handler.Seek(Header.dataOffset);

            int langCount;
            int extraFontCount;
            if (version <= 19) {
                var ukn = handler.Read<short>();
                langCount = handler.Read<short>();
            } else {
                langCount = handler.Read<int>();
            }
            if (version <= 15) {
                // re7: total font count, including the base font array, is langCount * ukn2 * ukn3
                var ukn2 = handler.Read<short>();
                var ukn3 = handler.Read<short>();
                extraFontCount = ukn2 * ukn3 * langCount - langCount;
            }

            if (version == 15) {
                var flt = handler.Read<float>();
                var ukn1 = handler.Read<int>();
            } else if (version == 19) {
                var flt = handler.Read<float>();
            } else if (version >= 24) {
                // ver 27
                // dd2: 33 base + 528 extra (528 = 33 * 16)
                // re4: 33 base + 528 extra
                // maybe the 16 is hardcoded?
                var floats = handler.ReadArray<float>(4);
                var ukn = handler.Read<int>();
            }

            DefaultFont = handler.ReadOffsetWString();
            // if (Fonts.Length != langCount) Fonts = new string[langCount];
            // for (int i = 0; i < langCount; ++i) Fonts[i] = handler.ReadOffsetWString();
            // TODO font variants / data

            handler.Seek(Header.msgListOffset);
            MessageFiles.Clear();
            var msgCount = (int)handler.Read<long>();
            MessageFiles.EnsureCapacity(msgCount);
            for (int i = 0; i < msgCount; ++i) MessageFiles.Add(handler.ReadOffsetWString());

            // the rest of the file is unsupported for now

            return true;
        }

        protected override bool DoWrite()
        {
            throw new NotImplementedException();
        }
    }
}