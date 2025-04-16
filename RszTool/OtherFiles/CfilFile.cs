using RszTool.Common;
using System.Collections.ObjectModel;

namespace RszTool
{
    public class CfilFile : BaseFile
    {
        public int guidCount;
        public Guid LayerGuid;
        public int ukn3;
        public int ukn4;
        public int ukn5;
        public int ukn6;
        public long guidListOffset;
        public long uknOffset;
        public Guid[]? Masks;

        private const int Magic = 0x4c494643;

        public CfilFile(FileHandler fileHandler) : base(fileHandler)
        {
        }

        protected override bool DoRead()
        {
            var handler = FileHandler;
            var magic = handler.Read<int>();
            if (magic != Magic)
            {
                throw new Exception("Invalid CFIL file");
            }
            handler.Read(ref guidCount);
            handler.Skip(8);
            handler.Read(ref LayerGuid);
            handler.Read(ref ukn3);
            handler.Read(ref ukn4);
            handler.Read(ref ukn5);
            handler.Read(ref ukn6);
            handler.Read(ref guidListOffset);
            handler.Read(ref uknOffset);
            if (Masks == null || Masks.Length != guidCount)
            {
                Masks = new Guid[guidCount];
            }
            handler.ReadArray(Masks);
            return true;
        }

        protected override bool DoWrite()
        {
            var handler = FileHandler;
            handler.Write(Magic);
            guidCount = Masks?.Length ?? 0;
            handler.Write(ref guidCount);
            handler.Skip(8);
            handler.Write(ref LayerGuid);
            handler.Write(ref ukn3);
            handler.Write(ref ukn4);
            handler.Write(ref ukn5);
            handler.Write(ref ukn6);
            // since there's only one cfil header structure and everything else seems to just be 0, guids always start at 64
            guidListOffset = 64L;
            handler.Write(ref guidListOffset);
            handler.Write(guidListOffset + guidCount * sizeof(long) * 2);
            handler.WriteArray(Masks ?? Array.Empty<Guid>());
            return true;
        }
    }
}