using ReeLib.AimapAttr;
using ReeLib.via;

namespace ReeLib.AimapAttr
{
    public class AimapAttributeEntry
    {
        public string name = "";
        public Color color;
    }
}

namespace ReeLib
{
    public class AimapAttrFile(FileHandler handler) : BaseFile(handler)
    {
        public const int Magic = 0x414D4941;

        public Color color1;
        public Color color2;
        public List<AimapAttributeEntry> Attributes { get; } = new();

        protected override bool DoRead()
        {
            var handler = FileHandler;
            if (handler.Read<int>() != Magic)
            {
                throw new InvalidDataException($"{handler.FilePath} Not a AIMPATTR file");
            }
            Attributes.Clear();

            handler.Read(ref color1);
            handler.Read(ref color2);
            var count = 64;
            for (int i = 0; i < count; ++i)
            {
                var attr = new AimapAttributeEntry();
                attr.name = handler.ReadInlineWString();
                handler.Read(ref attr.color);
                Attributes.Add(attr);
            }
            return true;
        }

        protected override bool DoWrite()
        {
            var handler = FileHandler;
            handler.Write(Magic);
            handler.Write(ref color1);
            handler.Write(ref color2);
            foreach (var attr in Attributes)
            {
                handler.WriteInlineWString(attr.name);
                handler.Write(ref attr.color);
            }
            return true;
        }
    }
}