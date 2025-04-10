using System.Collections.ObjectModel;
using System.Numerics;
using RszTool.Gui;

namespace RszTool
{
    public enum GuiVersion
    {
        RE7 = 60,
        RE2_DMC5 = 85,
        RE3 = 99,
        RE8 = 486,
    }


    public class GuiFile : BaseRszFile
    {
        // 内存结构顺序：
        // 1. Header
        // 2. ElementHeader[]
        // 3. SubElementHeader[]
        // 4. ClipHeader[]

        public HeaderStruct Header { get; } = new();
        public List<Element> Elements { get; } = new();

        public GuiFile(RszFileOption option, FileHandler fileHandler) : base(option, fileHandler)
        {
        }

        public const uint Magic = 0x52495547;  // GUIR
        public const string Extension2 = ".gui";

        protected override bool DoRead()
        {
            var handler = FileHandler;
            var header = Header;
            if (!header.Read(handler)) return false;
            if (header.magic != Magic)
            {
                throw new InvalidDataException($"{handler.FilePath} Not a GUI file");
            }

            if (header.version < 430000)
            {
                handler.Seek(header.endOffs[0]);
                throw new NotImplementedException("Not implemented for version < 430000");
            }

            handler.Seek(header.elementOffsets[0]);

            return true;
        }

        protected override bool DoWrite()
        {
            throw new NotImplementedException();
        }
    }
}


namespace RszTool.Gui
{
    public class HeaderStruct : BaseModel {
        public uint version;
        public uint magic;
        public long offsetsStartOffset;
        public long[] endOffs = new long[4];
        public ulong ukn;
        public long offsetsStart;
        public long viewOffset;
        public long numOffs;
        public List<long> elementOffsets = new();

        public GuiVersion GuiVersion { get; private set; }

        protected override bool DoRead(FileHandler handler)
        {
            handler.Read(ref version);
            handler.Read(ref magic);
            handler.Read(ref offsetsStartOffset);
            handler.ReadArray(endOffs);
            if (version > 430000)
            {
                handler.Read(ref ukn);
            }
            handler.Seek(offsetsStartOffset);
            handler.Read(ref offsetsStart);
            handler.Read(ref viewOffset);
            handler.Read(ref numOffs);
            elementOffsets.Clear();
            for (int i = 0; i < numOffs; i++)
            {
                elementOffsets.Add(handler.ReadInt64());
            }

            if (version == 180014) // RE7
                GuiVersion = GuiVersion.RE7;
            else if (version == 340020) // RE3
                GuiVersion = GuiVersion.RE3;
            else if (version  >= 400022) // RE8 and MHRise
                GuiVersion = GuiVersion.RE8;
            return true;
        }

        protected override bool DoWrite(FileHandler handler)
        {
            handler.Write(ref version);
            handler.Write(ref magic);
            var addrOffset = handler.Tell();
            handler.Write(ref offsetsStartOffset);
            handler.ReadArray(endOffs);
            if (version > 430000)
            {
                handler.Write(ref ukn);
            }
            offsetsStartOffset = handler.Tell();
            handler.Write(addrOffset, offsetsStartOffset);
            handler.Write(ref offsetsStart);
            handler.Write(ref viewOffset);
            numOffs = elementOffsets.Count;
            handler.Write(ref numOffs);
            elementOffsets.Clear();
            for (int i = 0; i < numOffs; i++)
            {
                handler.Write(elementOffsets[i]);
            }
            return true;
        }
    }


    // for header.version < 430000
    public class GPath : BaseModel
    {
        public long count;

        protected override bool DoRead(FileHandler handler)
        {
            throw new NotImplementedException();
        }

        protected override bool DoWrite(FileHandler handler)
        {
            throw new NotImplementedException();
        }
    }


    public enum AttributeType
    {
        Unknown,
        Bool,
        Int,
        Float,
        String,
        WString,
        Vec4,
        Vec3,
        Color,
        ColorFloat,
    }


    public class AttributeInfo : BaseModel
    {
        public uint type;
        private uint padding;
        public long nameOffset;
        public string? Name { get; set; }

        public override string ToString()
        {
            return Name ?? "";
        }

        protected override bool DoRead(FileHandler handler)
        {
            handler.Read(ref type);
            handler.Read(ref padding);
            handler.Read(ref nameOffset);
            Name = handler.ReadWString(nameOffset);
            return true;

        }

        protected override bool DoWrite(FileHandler handler)
        {
            handler.Write(ref type);
            handler.Write(ref padding);
            handler.StringTableAdd(Name);
            handler.Write(ref nameOffset);
            return true;
        }

        public AttributeType AttributeType
        {
            get
            {
                switch (type)
                {
                    case 1:
                        return AttributeType.Bool;
                    case 3:
                    case 4: // UV
                    case 5:
                    case 6: // RE3
                    case 7:
                        return AttributeType.Int;
                    case 10:
                        return AttributeType.Float;
                    case 14:
                    case 43: // RE3
                        return AttributeType.String;
                    case 13:
                    case 32: // filepath
                    case 34: // string RE3
                        return AttributeType.WString;
                    case 22:
                    case 26:
                        return AttributeType.Vec4;
                    case 21: // RE3
                    case 31: // vector3
                        return AttributeType.Vec3;
                    case 24: // color (bytes)
                        return AttributeType.Color;
                    case 27:
                    case 28: // color (floats)
                        return AttributeType.ColorFloat;
                    default:
                        return AttributeType.Unknown;
                }
            }
        }

        public bool IsPointer
        {
            get
            {
                return AttributeType switch
                {
                    AttributeType.String or AttributeType.WString or AttributeType.Vec4 or AttributeType.Vec3
                        or AttributeType.Color or AttributeType.ColorFloat => true,
                    _ => false,
                };
            }
        }
    }


    public struct ColorFloat
    {
        public float Red;
        public float Blue;
        public float Green;
        public float Alpha;
    }


    public class Attribute : BaseModel
    {
        private uint Version { get; }
        public AttributeInfo AttributeInfo { get; }

        public long valueOffset;
        private long ukn;

        // public bool UniqueOffset { get; set; }
        // public int OffsetIdx { get; set; }
        public object Value { get; set; } = 0;


        public Attribute(uint version, AttributeInfo attributeInfo)
        {
            Version = version;
            AttributeInfo = attributeInfo;
        }

        protected override bool DoRead(FileHandler handler)
        {
            handler.Read(ref valueOffset);
            if (Version > 430000) handler.Read(ref ukn);

            long jumpBack = handler.Tell();
            handler.Seek(valueOffset);
            switch (AttributeInfo.AttributeType)
            {
                case AttributeType.Bool:
                    Value = handler.Read<int>();
                    break;
                case AttributeType.Int:
                    Value = handler.Read<int>();
                    break;
                case AttributeType.Float:
                    handler.Skip(4);
                    Value = handler.Read<float>();
                    break;
                case AttributeType.String:
                    Value = handler.ReadAsciiString();
                    break;
                case AttributeType.WString:
                    Value = handler.ReadWString();
                    break;
                case AttributeType.Vec4:
                    Value = handler.Read<Vector4>();
                    break;
                case AttributeType.Vec3:
                    Value = handler.Read<Vector3>();
                    break;
                case AttributeType.Color:
                    Value = handler.Read<via.Color>();
                    break;
                case AttributeType.ColorFloat:
                    Value = handler.Read<ColorFloat>();
                    break;
            }
            handler.Seek(jumpBack);
            return true;
        }

        protected override bool DoWrite(FileHandler handler)
        {
            handler.Write(ref valueOffset);
            if (Version > 430000) handler.Write(ref ukn);
            // TODO write Value
            return true;
        }

        public void WriteValue(FileHandler handler)
        {
            switch (AttributeInfo.AttributeType)
            {
                case AttributeType.Bool:
                    handler.Write((int)Value);
                    break;
                case AttributeType.Int:
                    handler.Write((int)Value);
                    break;
                case AttributeType.Float:
                    handler.Skip(4);
                    handler.Write((float)Value);
                    break;
                case AttributeType.String:
                    handler.WriteAsciiString((string)Value);
                    break;
                case AttributeType.WString:
                    handler.WriteWString((string)Value);
                    break;
                case AttributeType.Vec4:
                    handler.Write((Vector4)Value);
                    break;
                case AttributeType.Vec3:
                    handler.Write((Vector3)Value);
                    break;
                case AttributeType.Color:
                    handler.Write((via.Color)Value);
                    break;
                case AttributeType.ColorFloat:
                    handler.Write((ColorFloat)Value);
                    break;
            }
        }
    }


    public class ElementInfo : BaseModel
    {
        public Guid guid;
        public long nameOffset;
        public long classNameOffset;
        public long subElementOffset;
        public long clipOffset;
        public string? Name { get; set; }
        public string? ClassName { get; set; }

        public override string ToString()
        {
            return ClassName ?? "";
        }

        protected override bool DoRead(FileHandler handler)
        {
            handler.Read(ref guid);
            handler.Read(ref nameOffset);
            handler.Read(ref classNameOffset);
            handler.Read(ref subElementOffset);
            handler.Read(ref clipOffset);

            Name = handler.ReadWString(nameOffset);
            ClassName = handler.ReadWString(classNameOffset);
            return true;
        }

        protected override bool DoWrite(FileHandler handler)
        {
            handler.Write(ref guid);
            handler.StringTableAdd(Name);
            handler.Write(ref nameOffset);
            handler.StringTableAdd(ClassName);
            handler.Write(ref classNameOffset);
            handler.Write(ref subElementOffset);
            handler.Write(ref clipOffset);
            return true;
        }
    }


    public class ElementInfoSubElement : BaseModel
    {
        public long subElementCount;
        public List<long> subElementOffsets = new();

        protected override bool DoRead(FileHandler handler)
        {
            handler.Read(ref subElementCount);
            subElementOffsets.Clear();
            for (int i = 0; i < subElementCount; i++)
            {
                subElementOffsets.Add(handler.ReadInt64());
            }
            return true;
        }

        protected override bool DoWrite(FileHandler handler)
        {
            subElementCount = subElementOffsets.Count;
            handler.Write(ref subElementCount);
            for (int i = 0; i < subElementCount; i++)
            {
                handler.WriteInt64(subElementOffsets[i]);
            }
            return true;
        }
    }


    public class ElementInfoClip : BaseModel
    {
        public uint E;
        public int clipCount;
        public List<long> clipOffsets = new();

        protected override bool DoRead(FileHandler handler)
        {
            handler.Read(ref E);
            handler.Read(ref clipCount);
            clipOffsets.Clear();
            for (int i = 0; i < clipCount; i++)
            {
                clipOffsets.Add(handler.ReadInt64());
            }
            return true;
        }

        protected override bool DoWrite(FileHandler handler)
        {
            handler.Write(ref E);
            clipCount = clipOffsets.Count;
            handler.Write(ref clipCount);
            for (int i = 0; i < clipCount; i++)
            {
                handler.WriteInt64(clipOffsets[i]);
            }
            return true;
        }
    }


    public class Element
    {
        public ElementInfo? Info { get; }
        public ObservableCollection<SubElement> SubElements { get; } = new();
        // Clips
    }


    public class SubElementInfo : BaseModel
    {
        private uint Version { get; }
        public Guid guid1;
        public Guid guid2;
        private ulong ukn1;
        private ulong ukn2;
        public string Name { get; set; } = "";
        public string ClassName { get; set; } = "";
        public long subStructOffs;
        public long subStructEndOffs;
        private ulong padding;

        public SubElementInfo(uint version)
        {
            Version = version;
        }

        protected override bool DoRead(FileHandler handler)
        {
            handler.Read(ref guid1);
            handler.Read(ref guid2);
            if (Version >= 400022)
            {
                handler.Read(ref ukn1);
                handler.Read(ref ukn2);
            }
            Name = handler.ReadWString(handler.ReadInt64());
            ClassName = handler.ReadAsciiString(handler.ReadInt64());
            handler.Read(ref subStructOffs);
            handler.Read(ref subStructEndOffs);
            handler.Read(ref padding);
            return true;
        }

        protected override bool DoWrite(FileHandler handler)
        {
            handler.Write(ref guid1);
            handler.Write(ref guid2);
            if (Version >= 400022)
            {
                handler.Write(ref ukn1);
                handler.Write(ref ukn2);
            }
            handler.WriteWString(Name);
            handler.WriteAsciiString(ClassName);
            handler.Write(ref subStructOffs);
            handler.Write(ref subStructEndOffs);
            handler.Write(ref padding);
            return true;
        }
    }


    public class SubElement : BaseModel
    {
        public SubElementInfo Info { get; }
        public long attributeCount;
        public List<Attribute> Attributes { get; } = new();

        public class UnknownStruct : BaseModel
        {
            public int A, B, C, D;
            public string? sName1;
            public long valueOffset;
            public int E, F;
            public string? sName2;
            public long G;
            public Guid guid;
            public long H;
            public string? wName;

            protected override bool DoRead(FileHandler handler)
            {
                handler.Read(ref A);
                handler.Read(ref B);
                handler.Read(ref C);
                handler.Read(ref D);
                sName1 = handler.ReadAsciiString(handler.ReadInt64());
                handler.Read(ref valueOffset);
                handler.Read(ref E);
                handler.Read(ref F);
                sName2 = handler.ReadAsciiString(handler.ReadInt64());
                handler.Read(ref G);
                handler.Read(ref guid);
                handler.Read(ref H);
                wName = handler.ReadWString(handler.ReadInt64());
                return true;
            }

            protected override bool DoWrite(FileHandler handler)
            {
                handler.Write(ref A);
                handler.Write(ref B);
                handler.Write(ref C);
                handler.Write(ref D);
                handler.WriteAsciiString(sName1 ?? "");
                handler.Write(ref valueOffset);
                handler.Write(ref E);
                handler.Write(ref F);
                handler.WriteAsciiString(sName2 ?? "");
                handler.Write(ref G);
                handler.Write(ref guid);
                handler.Write(ref H);
                handler.WriteWString(wName ?? "");
                return true;
            }
        }

        public UnknownStruct? Unknown;

        public SubElement(SubElementInfo info)
        {
            Info = info;
        }

        protected override bool DoRead(FileHandler handler)
        {
            handler.Seek(Info.subStructOffs);
            handler.Read(ref attributeCount);

            Attributes.Clear();
            for (int i = 0; i < attributeCount; i++)
            {
                // TODO
                // Attribute attribute = new();
                // attribute.Read(handler);
                // Attributes.Add(attribute);
            }

            handler.Seek(Info.subStructEndOffs);
            if (handler.ReadUInt() != 0)
            {
                Unknown = new();
                Unknown.Read(handler);
            }
            return true;
        }

        protected override bool DoWrite(FileHandler handler)
        {
            throw new NotImplementedException();
        }
    }
}
