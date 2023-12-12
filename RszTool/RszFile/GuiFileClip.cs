using System.Numerics;

namespace RszTool.Gui
{
    public enum InterpolationType
    {
        Unknown = 0x0,
        Discrete = 0x1,
        Linear = 0x2,
        Event = 0x3,
        Slerp = 0x4,
        Hermite = 0x5,
        AutoHermite = 0x6,
        Bezier = 0x7,
        AutoBezier = 0x8,
        OffsetFrame = 0x9,
        OffsetSec = 0xA,
        PassEvent = 0xB,
        Bezier3D = 0xC,
        Range = 0xD,
        DiscreteToEnd = 0xE,
    }


    public enum PropertyType : byte
    {
        Unknown = 0x0,
        Bool = 0x1,
        S8 = 0x2,
        U8 = 0x3,
        S16 = 0x4,
        U16 = 0x5,
        S32 = 0x6,
        U32 = 0x7,
        S64 = 0x8,
        U64 = 0x9,
        F32 = 0xA,
        F64 = 0xB,
        Str8 = 0xC,
        Str16 = 0xD,
        Enum = 0xE,
        Quaternion = 0xF,
        Array = 0x10,
        NativeArray = 0x11,
        Class = 0x12,
        NativeClass = 0x13,
        Struct = 0x14,
        Vec2 = 0x15,
        Vec3 = 0x16,
        Vec4 = 0x17,
        Color = 0x18,
        Range = 0x19,
        Float2 = 0x1A,
        Float3 = 0x1B,
        Float4 = 0x1C,
        RangeI = 0x1D,
        Point = 0x1E,
        Size = 0x1F,
        Asset = 0x20,
        Action = 0x21,
        Guid = 0x22,
        Uint2 = 0x23,
        Uint3 = 0x24,
        Uint4 = 0x25,
        Int2 = 0x26,
        Int3 = 0x27,
        Int4 = 0x28,
        OBB = 0x29,
        Mat4 = 0x2A,
        Rect = 0x2B,
        PathPoint3D = 0x2C,
        Plane = 0x2D,
        Sphere = 0x2E,
        Capsule = 0x2F,
        AABB = 0x30,
        Nullable = 0x31,
        Sfix = 0x32,
        Sfix2 = 0x33,
        Sfix3 = 0x34,
        Sfix4 = 0x35,
        AnimationCurve = 0x36,
        KeyFrame = 0x37,
        GameObjectRef = 0x38,
    }

    public static class PropertyTypeUtils
    {
        public static RszFieldType ToRszFieldType(PropertyType type)
        {
            return type switch
            {
                PropertyType.Bool => RszFieldType.Bool,
                PropertyType.S8 => RszFieldType.S8,
                PropertyType.U8 => RszFieldType.U8,
                PropertyType.S16 => RszFieldType.S16,
                PropertyType.U16 => RszFieldType.U16,
                PropertyType.S32 => RszFieldType.S32,
                PropertyType.U32 => RszFieldType.U32,
                PropertyType.S64 => RszFieldType.S64,
                PropertyType.U64 => RszFieldType.U64,
                PropertyType.F32 => RszFieldType.F32,
                PropertyType.F64 => RszFieldType.F64,
                PropertyType.Str8 => RszFieldType.String,
                PropertyType.Str16 => RszFieldType.String,
                PropertyType.Enum => RszFieldType.Enum,
                PropertyType.Quaternion => RszFieldType.Quaternion,
                PropertyType.Array => RszFieldType.ukn_type,
                PropertyType.NativeArray => RszFieldType.ukn_type,
                PropertyType.Class => RszFieldType.ukn_type,
                PropertyType.NativeClass => RszFieldType.ukn_type,
                PropertyType.Struct => RszFieldType.Struct,
                PropertyType.Vec2 => RszFieldType.Vec2,
                PropertyType.Vec3 => RszFieldType.Vec3,
                PropertyType.Vec4 => RszFieldType.Vec4,
                PropertyType.Color => RszFieldType.Color,
                PropertyType.Range => RszFieldType.Range,
                PropertyType.Float2 => RszFieldType.Float2,
                PropertyType.Float3 => RszFieldType.Float3,
                PropertyType.Float4 => RszFieldType.Float4,
                PropertyType.RangeI => RszFieldType.RangeI,
                PropertyType.Point => RszFieldType.Point,
                PropertyType.Size => RszFieldType.Size,
                PropertyType.Asset => RszFieldType.ukn_type,
                PropertyType.Action => RszFieldType.Action,
                PropertyType.Guid => RszFieldType.Guid,
                PropertyType.Uint2 => RszFieldType.Uint2,
                PropertyType.Uint3 => RszFieldType.Uint3,
                PropertyType.Uint4 => RszFieldType.Uint4,
                PropertyType.Int2 => RszFieldType.Int2,
                PropertyType.Int3 => RszFieldType.Int3,
                PropertyType.Int4 => RszFieldType.Int4,
                PropertyType.OBB => RszFieldType.OBB,
                PropertyType.Mat4 => RszFieldType.Mat4,
                PropertyType.Rect => RszFieldType.Rect,
                PropertyType.PathPoint3D => RszFieldType.ukn_type,
                PropertyType.Plane => RszFieldType.Plane,
                PropertyType.Sphere => RszFieldType.Sphere,
                PropertyType.Capsule => RszFieldType.Capsule,
                PropertyType.AABB => RszFieldType.AABB,
                PropertyType.Nullable => RszFieldType.ukn_type,
                PropertyType.Sfix => RszFieldType.Sfix,
                PropertyType.Sfix2 => RszFieldType.Sfix2,
                PropertyType.Sfix3 => RszFieldType.Sfix3,
                PropertyType.Sfix4 => RszFieldType.Sfix4,
                PropertyType.AnimationCurve => RszFieldType.ukn_type,
                PropertyType.KeyFrame => RszFieldType.KeyFrame,
                PropertyType.GameObjectRef => RszFieldType.GameObjectRef,
                _ => RszFieldType.ukn_type,
            };
        }
    }


    public class Key : BaseModel
    {
        public float frame;
        public float rate;
        public InterpolationType interpolation;
        public bool instanceValue;
        public ushort reserved;
        public uint reserved2;
        public object Value { get; set; } = null!;

        public PropertyType PropertyType { get; }

        public Key(PropertyType type)
        {
            PropertyType = type;
        }

        protected override bool DoRead(FileHandler handler)
        {
            handler.Read(ref frame);
            handler.Read(ref rate);
            handler.Read(ref interpolation);
            handler.Read(ref instanceValue);
            handler.Read(ref reserved);
            handler.Read(ref reserved2);
            // Value
            switch (PropertyType)
            {
                case PropertyType.Bool:
                    Value = handler.Read<bool>();
                    break;
                case PropertyType.S8:
                    Value = handler.Read<sbyte>();
                    break;
                case PropertyType.U8:
                    Value = handler.Read<byte>();
                    break;
                case PropertyType.S16:
                    Value = handler.Read<short>();
                    break;
                case PropertyType.U16:
                    Value = handler.Read<ushort>();
                    break;
                case PropertyType.S32:
                    Value = handler.Read<int>();
                    break;
                case PropertyType.U32:
                    Value = handler.Read<uint>();
                    break;
                case PropertyType.S64:
                    Value = handler.Read<long>();
                    break;
                case PropertyType.U64:
                    Value = handler.Read<ulong>();
                    break;
                case PropertyType.F32:
                    Value = handler.Read<float>();
                    break;
                case PropertyType.F64:
                    Value = handler.Read<double>();
                    break;
                case PropertyType.Str8:
                case PropertyType.Enum:
                    {
                        long offset = handler.Read<long>();
                        // clipHeader.namesOffsExtra[1] + start + offset
                        // Value = handler.ReadAsciiString(offset);
                    }
                    break;
                case PropertyType.Str16:
                case PropertyType.Asset:
                case PropertyType.Guid:
                    {
                        long offset = handler.Read<long>();
                        // clipHeader.unicodeNamesOffs + start + offset*2
                        // Value = handler.ReadWString(offset);
                    }
                    break;
                case PropertyType.Quaternion:
                    Value = handler.Read<Quaternion>();
                    break;
                case PropertyType.Array:
                case PropertyType.NativeArray:
                case PropertyType.Class:
                case PropertyType.NativeClass:
                case PropertyType.Struct:
                    throw new Exception($"Unsupported PropertyType: {PropertyType}");
                case PropertyType.Vec2:
                case PropertyType.Float2:
                    Value = handler.Read<Vector2>();
                    break;
                case PropertyType.Vec3:
                case PropertyType.Float3:
                    Value = handler.Read<Vector3>();
                    break;
                case PropertyType.Vec4:
                case PropertyType.Float4:
                    Value = handler.Read<Vector4>();
                    break;
                case PropertyType.Color:
                    Value = handler.Read<via.Color>();
                    break;
                case PropertyType.Range:
                    Value = handler.Read<via.Range>();
                    break;
                case PropertyType.RangeI:
                    Value = handler.Read<via.RangeI>();
                    break;
                case PropertyType.Point:
                    Value = handler.Read<via.Point>();
                    break;
                case PropertyType.Size:
                    Value = handler.Read<via.Size>();
                    break;
                case PropertyType.Action:
                    throw new Exception($"Unsupported PropertyType: {PropertyType}");
                case PropertyType.Uint2:
                    Value = handler.Read<via.Uint2>();
                    break;
                case PropertyType.Uint3:
                    Value = handler.Read<via.Uint3>();
                    break;
                case PropertyType.Uint4:
                    Value = handler.Read<via.Uint4>();
                    break;
                case PropertyType.Int2:
                    Value = handler.Read<via.Int2>();
                    break;
                case PropertyType.Int3:
                    Value = handler.Read<via.Int3>();
                    break;
                case PropertyType.Int4:
                    Value = handler.Read<via.Int4>();
                    break;
                case PropertyType.OBB:
                    Value = handler.Read<via.OBB>();
                    break;
                case PropertyType.Mat4:
                    Value = handler.Read<via.mat4>();
                    break;
                case PropertyType.Rect:
                    Value = handler.Read<via.Rect>();
                    break;
                case PropertyType.PathPoint3D:
                    throw new Exception($"Unsupported PropertyType: {PropertyType}");
                case PropertyType.Plane:
                    Value = handler.Read<via.Plane>();
                    break;
                case PropertyType.Sphere:
                    Value = handler.Read<via.Sphere>();
                    break;
                case PropertyType.Capsule:
                    Value = handler.Read<via.Capsule>();
                    break;
                case PropertyType.AABB:
                    Value = handler.Read<via.AABB>();
                    break;
                case PropertyType.Nullable:
                    throw new Exception($"Unsupported PropertyType: {PropertyType}");
                case PropertyType.Sfix:
                    Value = handler.Read<via.sfix>();
                    break;
                case PropertyType.Sfix2:
                    Value = handler.Read<via.Sfix2>();
                    break;
                case PropertyType.Sfix3:
                    Value = handler.Read<via.Sfix3>();
                    break;
                case PropertyType.Sfix4:
                    Value = handler.Read<via.Sfix4>();
                    break;
                case PropertyType.AnimationCurve:
                case PropertyType.KeyFrame:
                case PropertyType.GameObjectRef:
                    throw new Exception($"Unsupported PropertyType: {PropertyType}");
                default:
                    throw new Exception($"Unknown PropertyType: {PropertyType}");
            }
            return true;
        }

        protected override bool DoWrite(FileHandler handler)
        {
            handler.Write(frame);
            handler.Write(rate);
            handler.Write(interpolation);
            handler.Write(instanceValue);
            handler.Write(reserved);
            handler.Write(reserved2);
            // Value
            return true;
        }
    }


    public class PropertyInfo : BaseModel
    {
        public GuiVersion Version { get; set; }

        private uint pad;
        private int uknRE7_1;
        public float ValueA;  // Start
        public float ValueB;  // End
        private uint U32_1;
        private ulong U64_1;

        public long nameOffset;
        public long dataOffset;
        public long ChildStartIndex;
        public ushort ChildMembershipCount;
        public short arrayIndex;
        public short speedPointNum;
        public PropertyType DataType;
        public byte uknByte;
        public long lastKeyOffset;
        public long speedPointOffset;
        public long clipPropertyOffset;

        public byte uknCount;
        public ulong RE3hash;
        public ulong uknRE7_2;
        public ulong uknRE7_3;
        public ulong uknRE7_4;
        public long nameOffset2;


        public PropertyInfo(GuiVersion version)
        {
            Version = version;
        }

        protected override bool DoRead(FileHandler handler)
        {
            if (Version < GuiVersion.RE8) handler.Read(ref pad);
            if (Version == GuiVersion.RE7) handler.Read(ref uknRE7_1);
            handler.Read(ref ValueA);
            handler.Read(ref ValueB);
            if (Version == GuiVersion.RE7)
                handler.Read(ref U32_1);
            else
                handler.Read(ref U64_1);
            if (Version >= GuiVersion.RE8)
            {
                handler.Read(ref nameOffset);
                handler.Read(ref dataOffset);
                handler.Read(ref ChildStartIndex);
                handler.Read(ref ChildMembershipCount);
                handler.Read(ref arrayIndex);
                handler.Read(ref speedPointNum);
                handler.Read(ref DataType);
                handler.Read(ref uknByte);
                handler.Read(ref lastKeyOffset);
                handler.Read(ref speedPointOffset);
                handler.Read(ref clipPropertyOffset);
            }
            else
            {
                handler.Read(ref DataType);
                handler.Read(ref uknCount);
                handler.Skip(2);
                if (Version == GuiVersion.RE3)
                    handler.Read(ref RE3hash);
                else
                    handler.Skip(8);
                if (Version == GuiVersion.RE7)
                {
                    handler.Skip(8);
                    handler.Read(ref uknRE7_2);
                }
                handler.Read(ref nameOffset);
                handler.Read(ref nameOffset2);  // why?
                if (Version == GuiVersion.RE7)
                {
                    handler.Read(ref uknRE7_3);
                    handler.Skip(8);
                }
                handler.Skip(8);
                if (Version == GuiVersion.RE7)
                {
                    handler.Skip(16);
                }
                handler.Read(ref ChildStartIndex);
                handler.Read(ref ChildMembershipCount);
                if (Version == GuiVersion.RE7)
                {
                    handler.Skip(8);
                    handler.Read(ref uknRE7_4);
                }
            }

            // TODO
            // FSeek(clipHeader.namesOffsExtra[1] + start + PropInfo.nameOffset[0]);
            // string FunctionName  <hidden=false>;
            // if (Version != 486 && PropInfo.nameOffset[1] > 0) {
            //     FSeek(clipHeader.unicodeNamesOffs + start + PropInfo.nameOffset[1]*2);
            //     wstring wFunctionName <hidden=false>;
            // }
            // FSeek(startof(this)+propSize);
            return true;
        }

        protected override bool DoWrite(FileHandler handler)
        {
            throw new NotImplementedException();
        }
    }


    public class Property : BaseModel
    {
        public PropertyInfo? PropertyInfo;

        protected override bool DoRead(FileHandler handler)
        {
            throw new NotImplementedException();
        }

        protected override bool DoWrite(FileHandler handler)
        {
            throw new NotImplementedException();
        }
    }


    public class CTRACKS : BaseModel
    {
        public uint nodeCount;
        public uint propCount;
        public float Start_Frame;
        public float End_Frame;
        public Guid guid1;
        public Guid guid2;

        public byte nodeType;

        public long hash;
        public long nameOffset;
        public long nameOffset2;
        public long firstPropIdx;

        public GuiVersion Version { get; set; }

        public CTRACKS(GuiVersion version)
        {
            Version = version;
        }

        protected override bool DoRead(FileHandler handler)
        {
            if (Version >= GuiVersion.RE8)
            {
                handler.Read(ref nodeCount);
                handler.Read(ref propCount);
                handler.Read(ref nodeType);
                handler.Skip(3);
            }
            else
            {
                handler.Read(ref nodeCount);
                handler.Read(ref propCount);
                handler.Read(ref Start_Frame);
                handler.Read(ref End_Frame);
                handler.Read(ref guid1);
                handler.Read(ref guid2);
                handler.Skip(8);
            }
            handler.Read(ref hash);
            handler.Read(ref nameOffset);
            handler.Read(ref nameOffset2);
            handler.Read(ref firstPropIdx);
            if (Version == GuiVersion.RE2_DMC5)
            {
                handler.Read(ref firstPropIdx);  // need check
            }
            /*
            pos = FTell();
            if (Version == 99) {
                FSeek(clipHeader.unicodeNamesOffs + start + nameOffset[0] * 2);
                wstring name;
            } else if (Version == 486) {
                FSeek(clipHeader.unicodeNamesOffs + start + nameOffset[0] * 2);
                wstring name;
            } else {
                FSeek(clipHeader.namesOffsExtra[1] + start + nameOffset[0]);
                string name;
            }
            FSeek(pos);
            */
            throw new NotImplementedException();
        }

        protected override bool DoWrite(FileHandler handler)
        {
            throw new NotImplementedException();
        }
    }


    public class ClipHeader : BaseModel
    {
        public Guid guid;
        public string? Name;
        public uint Magic;
        public uint version;
        public float NumFrames;
        public int numNodes;
        public int numProperties;
        public int numKeys;

        protected override bool DoRead(FileHandler handler)
        {
            throw new NotImplementedException();
        }

        protected override bool DoWrite(FileHandler handler)
        {
            throw new NotImplementedException();
        }
    }


    public class CLIP_ENTRY : BaseModel
    {


        public GuiVersion Version { get; set; }

        public CLIP_ENTRY(GuiVersion version)
        {
            Version = version;
        }

        public int PropertySize => Version switch
        {
            GuiVersion.RE3 => 32,
            GuiVersion.RE7 => 120,
            GuiVersion.RE8 => 72,
            _ => 112,
        };

        public int KeySize => Version switch
        {
            GuiVersion.RE8 => 32,
            _ => 40,
        };

        protected override bool DoRead(FileHandler handler)
        {
            throw new NotImplementedException();
        }

        protected override bool DoWrite(FileHandler handler)
        {
            throw new NotImplementedException();
        }
    }
}
