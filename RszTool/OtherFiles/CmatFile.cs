using RszTool.Common;
using System.Collections.ObjectModel;

namespace RszTool
{
    public class CmatFile : BaseFile
    {
        public Guid materialGuid;
        public Guid[]? Attributes;

        private const int Magic = 0x54414D43;

        public CmatFile(FileHandler fileHandler) : base(fileHandler)
        {
        }

        protected override bool DoRead()
        {
            var handler = FileHandler;
            var magic = handler.Read<int>();
            if (magic != Magic)
            {
                throw new Exception("Invalid CMAT file");
            }

            var attributeCount = handler.Read<int>();
            handler.Read(ref materialGuid);
            handler.Skip(8); // filesize
            if (Attributes == null || Attributes.Length != attributeCount)
            {
                Attributes = new Guid[attributeCount];
            }
            handler.ReadArray(Attributes);
            return true;
        }

        protected override bool DoWrite()
        {
            var attributeCount = Attributes?.Length ?? 0;

            var handler = FileHandler;
            handler.Write(Magic);
            handler.Write(ref attributeCount);
            handler.Write(16 + attributeCount * sizeof(long) * 2);
            if (attributeCount > 0)
            {
                handler.WriteArray(Attributes!);
            }
            return true;
        }
    }
}