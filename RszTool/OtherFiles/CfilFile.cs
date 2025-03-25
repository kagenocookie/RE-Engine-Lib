using RszTool.Common;
using System.Collections.ObjectModel;

namespace RszTool
{
    public class CfilFile : BaseFile
    {
        public int count;
        public int ukn1;
        public int ukn2;
        public Guid myGuid;
        public int ukn3;
        public int ukn4;
        public int ukn5;
        public int ukn6;
        public long guidListOffset;
        public long uknOffset;
        public Guid[]? Guids;

        private const int Magic = 0x4c494643;

        public CfilFile(FileHandler fileHandler) : base(fileHandler)
        {
        }

        protected override bool DoRead()
        {
            var handler = FileHandler;
            handler.Read<int>();
            handler.Read(ref count);
            handler.Read(ref ukn1);
            handler.Read(ref ukn2);
            handler.Read(ref myGuid);
            handler.Read(ref ukn3);
            handler.Read(ref ukn4);
            handler.Read(ref ukn5);
            handler.Read(ref ukn6);
            handler.Read(ref guidListOffset);
            handler.Read(ref uknOffset);
            if (Guids == null || Guids.Length != count)
            {
                Guids = new Guid[count];
            }
            handler.ReadArray(Guids);
            return true;
        }

        protected override bool DoWrite()
        {
            var handler = FileHandler;
            handler.Write(Magic);
            count = Guids?.Length ?? 0;
            handler.Write(ref count);
            handler.Write(ref ukn1);
            handler.Write(ref ukn2);
            handler.Write(ref myGuid);
            handler.Write(ref ukn3);
            handler.Write(ref ukn4);
            handler.Write(ref ukn5);
            handler.Write(ref ukn6);
            // since there's only one cfil header structure and everything else seems to just be 0, guids always start at 64
            guidListOffset = 64L;
            handler.Write(ref guidListOffset);
            handler.Write(guidListOffset + count * sizeof(long) * 2);
            handler.WriteArray(Guids ?? Array.Empty<Guid>());
            return true;
        }
    }
}