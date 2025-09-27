using System.Runtime.CompilerServices;
using ReeLib.Clip;
using ReeLib.Common;
using ReeLib.InternalAttributes;

namespace ReeLib.Clip
{
    public enum ClipVersion
    {
        RE7 = 18,
        RE2_DMC5 = 27,
        RE3 = 34,
        MHR_DEMO = 40,
        RE8 = MHR_DEMO,
        RE7_RT = 43,
        RE2_RT = RE7_RT,
        RE3_RT = RE7_RT,
        MHR = RE7_RT,
        SF6 = 53,
        RE4 = 54,
        DD2 = 62,
        MHWilds = 85,
    }


    public enum InterpolationType : byte
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


    /// <summary>
    /// source: via.timeline.PropertyType
    /// </summary>
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
        // yes, it's actually a double even though the ingame enum says F32 for this value, maybe it's not the right enum, or the devs are dumb; haven't found a F64 instance in the wild yet
        F64 = 0xA,
        F64_invalid = 0xB,
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
                PropertyType.F64 => RszFieldType.F64,
                // PropertyType.F64_invalid => RszFieldType.F32,
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
        public uint unknown;
        public object Value { get; set; } = null!;

        public PropertyType PropertyType { get; set; }
        public WeakReference<ClipEntry> ClipFile { get; set; }

        public Key(ClipEntry clipFile)
        {
            ClipFile = new(clipFile);
        }

        protected override bool DoRead(FileHandler handler)
        {
            handler.Read(ref frame);
            handler.Read(ref rate);
            handler.Read(ref interpolation);
            handler.Read(ref instanceValue);
            handler.ReadNull(2);
            handler.Read(ref unknown);
            handler.Seek(Start + ClipFile.GetTarget()!.KeySize);
            return true;
        }

        protected override bool DoWrite(FileHandler handler)
        {
            handler.Write(ref frame);
            handler.Write(ref rate);
            handler.Write(ref interpolation);
            handler.Write(ref instanceValue);
            handler.WriteNull(2);
            handler.Write(ref unknown);
            WriteValue(handler);
            var end = Start + ClipFile.GetTarget()!.KeySize;
            handler.WriteNull((int)(end - handler.Tell()));
            return true;
        }

        public void ReadValue(FileHandler handler, Property property)
        {
            handler.Seek(Start + 16);
            var clipFile = ClipFile.GetTarget()!;
            PropertyType = property.Info.DataType;
            if (PropertyType == PropertyType.Unknown)
            {
                Value = handler.Read<long>();
            }
            else switch (PropertyType)
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
                case PropertyType.F64:
                    Value = handler.Read<double>();
                    break;
                case PropertyType.F64_invalid:
                    throw new DataInterpretationException("F64_invalid, formerly known as non-existent F64, has been found.");
                    // Value = handler.Read<double>();
                    // break;
                case PropertyType.Str8:
                case PropertyType.Enum:
                    {
                        long offset = handler.Read<long>();
                        Value = handler.ReadAsciiString(clipFile.Header.namesOffset + offset);
                    }
                    break;
                case PropertyType.Str16:
                case PropertyType.Asset:
                case PropertyType.Guid:
                    {
                        long offset = handler.Read<long>();
                        // clipHeader.unicodeNamesOffs + start + offset*2
                        Value = handler.ReadWString(clipFile.Header.unicodeNamesOffset + offset * 2);
                    }
                    break;
                default:
                    throw new Exception($"Unsupported PropertyType: {PropertyType}");
            }
        }

        public void WriteValue(FileHandler handler)
        {
            handler.Seek(Start + 16);
            if (PropertyType == PropertyType.Unknown)
            {
                if (Value != null)
                {
                    handler.Write((long)Value);
                }
            }
            else switch (PropertyType)
            {
                case PropertyType.Bool:
                    handler.Write((bool)Value);
                    break;
                case PropertyType.S8:
                    handler.Write((sbyte)Value);
                    break;
                case PropertyType.U8:
                    handler.Write((byte)Value);
                    break;
                case PropertyType.S16:
                    handler.Write((short)Value);
                    break;
                case PropertyType.U16:
                    handler.Write((ushort)Value);
                    break;
                case PropertyType.S32:
                    handler.Write((int)Value);
                    break;
                case PropertyType.U32:
                    handler.Write((uint)Value);
                    break;
                case PropertyType.S64:
                    handler.Write((long)Value);
                    break;
                case PropertyType.U64:
                    handler.Write((ulong)Value);
                    break;
                case PropertyType.F64:
                    handler.Write((double)Value);
                    break;
                case PropertyType.F64_invalid:
                    handler.Write((double)Value);
                    break;
                case PropertyType.Str8:
                case PropertyType.Enum:
                    {
                        string text = (string)Value;
                        var stringItem = handler.AsciiStringTableAdd(text, false);
                        handler.Write(stringItem.TableOffset);
                    }
                    break;
                case PropertyType.Str16:
                case PropertyType.Asset:
                case PropertyType.Guid:
                    {
                        string text = (string)Value;
                        var stringItem = handler.StringTableAdd(text, false);
                        handler.Write(stringItem.TableOffset);
                    }
                    break;
                default:
                    throw new Exception($"Unsupported PropertyType: {PropertyType}");
            }
        }

        public override string ToString() => $"[{frame}]: {Value}";
    }


    public class PropertyInfo : ReadWriteModel
    {
        public ClipVersion Version { get; set; }

        private uint pad;
        private int uknRE7_1;
        public float startFrame;  // Start
        public float endFrame;  // End
        public uint nameHashRe7;
        public ulong nameCombineHash; // UTF16 + ascii hash

        public long nameOffset;
        public long dataOffset;
        public long ChildStartIndex;
        public ushort ChildMembershipCount;
        public short arrayIndex;
        public short speedPointNum;
        public PropertyType DataType;
        public byte uknByte00;
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
        public string FunctionName { get; set; } = string.Empty;


        public PropertyInfo(ClipVersion version)
        {
            Version = version;
        }

        protected override bool ReadWrite(IFileHandlerAction action)
        {
            if (Version < ClipVersion.RE8) action.Do(ref pad);
            if (Version == ClipVersion.RE7) action.Do(ref uknRE7_1);
            action.Do(ref startFrame);
            action.Do(ref endFrame);
            if (Version == ClipVersion.RE7)
                action.Do(ref nameHashRe7);
            else
                action.Do(ref nameCombineHash);
            if (Version >= ClipVersion.RE8)
            {
                action.Do(ref nameOffset);
                action.Do(ref dataOffset);
                action.Do(ref ChildStartIndex);
                action.Do(ref ChildMembershipCount);
                action.Do(ref arrayIndex);
                if (Version > ClipVersion.RE8)
                    action.Do(ref Unsafe.As<short, byte>(ref speedPointNum));
                else
                    action.Do(ref speedPointNum);
                action.Do(ref DataType);
                if (Version > ClipVersion.RE8)
                    action.Do(ref uknByte00);
                action.Do(ref uknByte);
                action.Do(ref lastKeyOffset);
                if (Version < ClipVersion.RE4)
                {
                    action.Do(ref speedPointOffset);
                    action.Do(ref clipPropertyOffset);
                }
            }
            else
            {
                action.Do(ref DataType);
                action.Do(ref uknCount);
                action.Null(2);
                if (Version == ClipVersion.RE3)
                    action.Do(ref RE3hash);
                else
                    action.Null(8); // TODO RE7 not null
                if (Version == ClipVersion.RE7)
                {
                    action.Skip(8); // TODO RE7 not null natives/x64/animation/weapon/wp1230/motlist/wp1230.motlist.60
                    action.Do(ref uknRE7_2);
                }
                action.Do(ref nameOffset);
                action.Do(ref nameOffset2);  // why?
                if (Version == ClipVersion.RE7)
                {
                    action.Do(ref uknRE7_3);
                    action.Null(8);
                }
                action.Null(8);
                if (Version > ClipVersion.RE7)
                {
                    action.Null(16);
                }
                action.Do(ref ChildStartIndex);
                action.Do(ref ChildMembershipCount);
                if (Version == ClipVersion.RE7)
                {
                    action.Skip(8); // TODO RE7 not null natives/x64/animation/enemy/em3000/motlist/upperblend/em3000upperblend.motlist.60
                    action.Do(ref uknRE7_4);
                }
                else
                {
                    // dmc5+
                    action.Null(30);
                }
            }
            // action.Handler.Seek(Start + 112);
            return true;
        }

        protected override bool DoWrite(FileHandler handler)
        {
            if (Version == ClipVersion.RE7)
            {
                nameHashRe7 = MurMur3HashUtils.GetHash(FunctionName);
            }
            else
            {
                nameCombineHash = MurMur3HashUtils.GetCombineHash(FunctionName);
            }
            return base.DoWrite(handler);
        }

        public override string ToString() => $"[{FunctionName}]";
    }


    public class Property
    {
        public PropertyInfo Info { get; }
        public List<Property>? ChildProperties { get; set; }
        public List<Key>? Keys { get; set; }

        public Property(ClipVersion version)
        {
            Info = new(version);
        }

        public bool IsPropertyContainer
        {
            get
            {
                switch (Info.DataType)
                {
                    case PropertyType.Quaternion:  // 0x0F
                    case PropertyType.Array:       // 0x10
                    case PropertyType.NativeArray: // 0x11
                    case PropertyType.Class:       // 0x12
                    case PropertyType.NativeClass: // 0x13
                    case PropertyType.Struct:      // 0x14
                    case PropertyType.Vec2:        // 0x15
                    case PropertyType.Vec3:        // 0x16
                    case PropertyType.Vec4:        // 0x17
                    case PropertyType.Color:       // 0x18
                    case PropertyType.Range:       // 0x19
                    case PropertyType.Float2:      // 0x1A
                    case PropertyType.Float3:      // 0x1B
                    case PropertyType.Float4:      // 0x1C
                    case PropertyType.RangeI:      // 0x1D
                    case PropertyType.Point:       // 0x1E
                    case PropertyType.OBB:         // 0x29
                    case PropertyType.Mat4:        // 0x2A
                    case PropertyType.Nullable:    // 0x31
                        return true;
                    default:
                        return false;
                }
            }
        }

        public override string ToString() => $"{Info.FunctionName} [{Info.startFrame}-{Info.endFrame}]";
    }


    public class CTrack : ReadWriteModel
    {
        public int nodeCount;
        public int propCount;
        public float Start_Frame;
        public float End_Frame;
        public Guid guid1;
        public Guid guid2;

        public byte nodeType;

        public ulong nameHash;
        public long nameOffset;
        public long nameOffset2;
        public long firstPropIdx;

        public List<Property> Properties { get; } = new();

        public ClipVersion Version { get; set; }
        public string Name { get; set; } = string.Empty;

        public CTrack(ClipVersion version)
        {
            Version = version;
        }

        protected override bool ReadWrite(IFileHandlerAction action)
        {
            if (Version >= ClipVersion.RE8)
            {
                action.Do(ref Unsafe.As<int, short>(ref nodeCount));
                action.Do(ref Unsafe.As<int, short>(ref propCount));
                action.Do(ref nodeType);
                action.Null(3);
            }
            else
            {
                action.Do(ref nodeCount);
                action.Do(ref propCount);
                action.Do(ref Start_Frame);
                action.Do(ref End_Frame);
                action.Do(ref guid1);
                action.Do(ref guid2);
                action.Null(8);
            }
            action.Do(ref nameHash);
            action.Do(ref nameOffset);
            action.Do(ref nameOffset2);
            action.Do(ref firstPropIdx);
            if (Version == ClipVersion.RE2_DMC5)
            {
                action.Do(ref firstPropIdx);  // need check
            }
            return true;
        }

        protected override bool DoWrite(FileHandler handler)
        {
            nameHash = MurMur3HashUtils.GetCombineHash(Name);
            return base.DoWrite(handler);
        }

        public void ReadName(FileHandler handler, ClipHeader header)
        {
            if (Version >= ClipVersion.RE3)
            {
                Name = handler.ReadWString(header.unicodeNamesOffset + nameOffset * 2);
            }
            else if (header.unknownOffsets != null)
            {
                Name = handler.ReadAsciiString(header.namesOffset + nameOffset);
            }
        }

        public void WriteName(FileHandler handler, ClipHeader header)
        {
            if (Version >= ClipVersion.RE3)
            {
                var stringItem = handler.StringTableAdd(Name, false);
                nameOffset = stringItem.TableOffset;
            }
            else if (header.unknownOffsets != null)
            {
                var stringItem = handler.AsciiStringTableAdd(Name, false);
                nameOffset = stringItem.TableOffset;
            }
        }

        public override string ToString() => Name ?? $"Hash: {nameHash}";
    }


    /// <summary>
    /// 这是Embedded Clip的结构，用在motlists和gui文件中，单独的clip结构不一样，多几个字段
    /// 暂时没想好怎么做兼容
    /// </summary>
    public class ClipHeader : BaseModel
    {
        public uint magic = ClipEntry.Magic;
        public ClipVersion version;
        public float numFrames;
        public int numNodes;
        public int numProperties;
        public int numKeys;
        public int numUnknownWilds1;
        public int numUnknownWilds2;
        public int numUnknownWilds3;
        public int numUnknownWilds4;

        public Guid guid;
        internal long clipDataOffset;
        internal long propertiesOffset;
        internal long keysOffset;
        internal long uknOffset1;
        internal long hermiteDataOffset;

        internal long[]? unknownOffsets;

        internal long namesOffset;
        internal long unicodeNamesOffset;
        internal long endClipStructsOffset1;
        internal long endClipStructsOffset2;

        internal int UnknownOffsetsCount => version switch
        {
            ClipVersion.RE7 => 3,
            ClipVersion.RE4 or ClipVersion.SF6 => 1,
            >= ClipVersion.MHWilds => 5,
            ClipVersion.RE2_DMC5 => 3,
            _ => 2,
        };

        protected override bool DoRead(FileHandler handler)
        {
            handler.Read(ref magic);
            handler.Read(ref version);
            handler.Read(ref numFrames);
            handler.Read(ref numNodes);
            handler.Read(ref numProperties);
            handler.Read(ref numKeys);
            if (version < ClipVersion.RE3)
            {
                handler.Read(ref guid);
            }
            if (version >= ClipVersion.MHWilds)
            {
                handler.Read(ref numUnknownWilds1);
                handler.Read(ref numUnknownWilds2);
                handler.Read(ref numUnknownWilds3);
                handler.Read(ref numUnknownWilds4);
            }
            handler.Read(ref clipDataOffset);
            handler.Read(ref propertiesOffset);
            handler.Read(ref keysOffset);
            handler.Read(ref uknOffset1);
            handler.Read(ref hermiteDataOffset);
            DataInterpretationException.ThrowIf(uknOffset1 != hermiteDataOffset);
            unknownOffsets = handler.ReadArray<long>(UnknownOffsetsCount);
            DataInterpretationException.ThrowIfNotEqualValues<long>(unknownOffsets);
            handler.Read(ref namesOffset);
            handler.Read(ref unicodeNamesOffset);
            handler.Read(ref endClipStructsOffset1);
            // if (version <= ClipVersion.RE4)
            handler.Read(ref endClipStructsOffset2);
            return true;
        }

        protected override bool DoWrite(FileHandler handler)
        {
            handler.Write(magic);
            handler.Write(ref version);
            handler.Write(ref numFrames);
            handler.Write(ref numNodes);
            handler.Write(ref numProperties);
            handler.Write(ref numKeys);
            if (version != ClipVersion.RE3 && version < ClipVersion.RE8)
            {
                handler.Write(ref guid);
            }
            if (version >= ClipVersion.MHWilds)
            {
                handler.Write(ref numUnknownWilds1);
                handler.Write(ref numUnknownWilds2);
                handler.Write(ref numUnknownWilds3);
                handler.Write(ref numUnknownWilds4);
            }
            handler.Write(ref clipDataOffset);
            handler.Write(ref propertiesOffset);
            handler.Write(ref keysOffset);
            handler.Write(ref uknOffset1);
            handler.Write(ref hermiteDataOffset);
            handler.WriteArray(unknownOffsets!);
            handler.Write(ref namesOffset);
            handler.Write(ref unicodeNamesOffset);
            handler.Write(ref endClipStructsOffset1);
            // if (version <= ClipVersion.RE4)
            handler.Write(ref endClipStructsOffset2);
            return true;
        }
    }


    public class EndClipStruct : BaseModel
    {
        public int ukn = -1;
        // these values could be byte flags of some sort, no idea
        public uint ukn1;
        public uint ukn2;
        public uint ukn3;
        public uint ukn4;
        public uint ukn5;
        public uint ukn6;

        public ClipVersion Version;

        public override string ToString() => $"{ukn} {ukn1} {ukn2} {ukn3}";

        protected override bool DoRead(FileHandler handler)
        {
            handler.Read(ref ukn);
            handler.Read(ref ukn1);
            handler.Read(ref ukn2);
            handler.Read(ref ukn3);
            if (Version >= ClipVersion.RE8)
            {
                handler.Read(ref ukn4);
                handler.Read(ref ukn5);
                handler.Read(ref ukn6);
            }
            return true;
        }

        protected override bool DoWrite(FileHandler handler)
        {
            handler.Write(ref ukn);
            handler.Write(ref ukn1);
            handler.Write(ref ukn2);
            handler.Write(ref ukn3);
            if (Version >= ClipVersion.RE8)
            {
                handler.Write(ref ukn4);
                handler.Write(ref ukn5);
                handler.Write(ref ukn6);
            }
            return true;
        }
    }

    [RszGenerate, RszAutoReadWrite]
    public partial class ExtraPropertyInfo : BaseModel
    {
        public uint propertyUTF16Hash;
        public short flags;
        public short count;
        public long valueOffset;

        [RszIgnore] public readonly List<FrameValue> values = new();

        public override string ToString() => $"[{propertyUTF16Hash}]: {values.FirstOrDefault()}";
    }

    public struct FrameValue
    {
        public float frame;
        public int value;

        public readonly override string ToString() => $"[{frame}] {value}";
    }

    public class ClipExtraPropertyData : BaseModel
    {
        public List<ExtraPropertyInfo> Props1 = new();
        public List<ExtraPropertyInfo> Props2 = new();

        public ClipVersion Version;

        public bool HasTwoLists => Version > ClipVersion.RE4;

        protected override bool DoRead(FileHandler handler)
        {
            // TODO figure out why there's two sets of property infos
            // I'm thinking it might be a "start" values and "end" / "after" values
            handler.Align(16);
            var count1 = handler.ReadInt();
            var count2 = handler.ReadInt(); // for versions with a single prop list, this is just padding
            var offset1 = handler.Read<long>();
            var offset2 = HasTwoLists ? handler.Read<long>() : 0;
            handler.Seek(offset1);
            Props1.Read(handler, count1);
            if (offset2 > 0) {
                handler.Seek(offset2);
                Props2.Read(handler, count2);
            }

            foreach (var prop in Props1)
            {
                handler.Seek(prop.valueOffset);
                for (int x = 0; x < prop.count; ++x) {
                    var n = handler.Read<int>();
                    var value = n == -1 ? -1f : BitConverter.Int32BitsToSingle(n);
                    // value2 seems to only be either 1 or 2
                    prop.values.Add(new FrameValue() {
                        frame = value,
                        value = handler.Read<int>(),
                    });
                }
            }
            foreach (var prop in Props2)
            {
                handler.Seek(prop.valueOffset);
                for (int x = 0; x < prop.count; ++x) {
                    var n = handler.Read<int>();
                    var value = n == -1 ? -1f : BitConverter.Int32BitsToSingle(n);
                    // value2 seems to be within frame counts as well
                    prop.values.Add(new FrameValue() {
                        frame = value,
                        value = handler.Read<int>(),
                    });
                }
            }
            return true;
        }

        protected override bool DoWrite(FileHandler handler)
        {
            handler.Align(16);
            handler.Write(Props1.Count);
            handler.Write(Props2.Count);
            long offset1;
            if (HasTwoLists)
            {
                handler.Write(offset1 = handler.Tell() + 16);
                handler.Write(handler.Tell() + 8 + Props1.Count * 16);
            }
            else
            {
                handler.Write(offset1 = handler.Tell() + 8);
                Props2.Clear();
            }
            handler.Skip((Props1.Count + Props2.Count) * 16);

            foreach (var prop in Props1)
            {
                prop.valueOffset = handler.Tell();
                prop.count = (short)prop.values.Count;
                foreach (var frame in prop.values)
                {
                    if (frame.frame == -1)
                    {
                        handler.Write(-1);
                    }
                    else
                    {
                        handler.Write(frame.frame);
                    }
                    handler.Write(frame.value);
                }
            }

            foreach (var prop in Props2)
            {
                prop.valueOffset = handler.Tell();
                prop.count = (short)prop.values.Count;
                foreach (var frame in prop.values)
                {
                    if (frame.frame == -1)
                    {
                        handler.Write(-1);
                    }
                    else
                    {
                        handler.Write(frame.frame);
                    }
                    handler.Write(frame.value);
                }
            }

            var endOffset = handler.Tell();

            handler.Seek(offset1);
            Props1.Write(handler);
            Props2.Write(handler);

            handler.Seek(endOffset);
            return true;
        }

        public override string ToString() => $"[Props: {Props1.Count} + {Props2.Count}]";
    }

    public struct HermiteInterpolationData
    {
        public float x1;
        public float y1;
        public float x2;
        public float y2;

        public override string ToString() => $"{x1},{y1}  {x2},{y2}";
    }

    public class ClipEntry : BaseModel
    {
        public ClipHeader Header { get; } = new();
        public List<CTrack> Tracks { get; } = new();
        public List<Property> Properties { get; } = new();
        public List<Key> ClipKeys { get; } = new();
        public HermiteInterpolationData[]? HermiteData { get; set; }
        public uint[]? UnknownData { get; set; }

        public float FrameCount { get => Header.numFrames; set => Header.numFrames = value; }
        public Guid Guid { get => Header.guid; set => Header.guid = value; }

        public ClipExtraPropertyData ExtraPropertyData = new();

        public const uint Magic = 0x50494C43;
        public const string Extension = ".clip";

        public ClipVersion Version { get => Header.version; set => Header.version = value; }

        internal int PropertySize => Version switch
        {
            ClipVersion.RE3 => 32,
            ClipVersion.RE7 => 120,
            ClipVersion.MHR_DEMO or ClipVersion.RE8 => 72,
            ClipVersion.SF6 or ClipVersion.RE4 => 56,
            _ => 112,
        };

        internal int KeySize => Version switch
        {
            < ClipVersion.RE3 => 40,
            _ => 32,
        };

        protected override bool DoRead(FileHandler handler)
        {
            Tracks.Clear();
            Properties.Clear();
            ClipKeys.Clear();
            var clipHeader = Header;
            if (!clipHeader.Read(handler)) return false;
            if (clipHeader.magic != Magic)
            {
                throw new InvalidDataException($"{handler.FilePath} Not a CLIP file");
            }
            if (clipHeader.numNodes > 0)
            {
                handler.Seek(clipHeader.clipDataOffset);
                for (int i = 0; i < clipHeader.numNodes; i++)
                {
                    CTrack cTrack = new(Version);
                    cTrack.Read(handler);
                    cTrack.ReadName(handler, clipHeader);
                    Tracks.Add(cTrack);
                }
            }

            if (clipHeader.numProperties > 0)
            {
                handler.Seek(clipHeader.propertiesOffset);
                for (int i = 0; i < clipHeader.numProperties; i++)
                {
                    Property property = new(Version);
                    property.Info.Read(handler);
                    property.Info.FunctionName = handler.ReadAsciiString(clipHeader.namesOffset + property.Info.nameOffset);
                    DataInterpretationException.ThrowIf(string.IsNullOrEmpty(property.Info.FunctionName));
                    Properties.Add(property);
                }
            }

            if (clipHeader.numKeys > 0)
            {
                handler.Seek(clipHeader.keysOffset);
                for (int i = 0; i < clipHeader.numKeys; i++)
                {
                    Key key = new(this);
                    key.Read(handler);
                    ClipKeys.Add(key);
                }
            }

            // sub properties or sub keys
            foreach (var property in Properties)
            {
                if (property.Info.ChildMembershipCount == 0) continue;
                if (property.IsPropertyContainer)
                {
                    property.ChildProperties ??= new();
                    for (long i = property.Info.ChildStartIndex; i < property.Info.ChildMembershipCount + property.Info.ChildStartIndex; i++)
                    {
                        property.ChildProperties.Add(Properties[(int)i]);
                    }
                }
                else
                {
                    property.Keys ??= new();
                    for (long i = property.Info.ChildStartIndex; i < property.Info.ChildMembershipCount + property.Info.ChildStartIndex; i++)
                    {
                        Key key = ClipKeys[(int)i];
                        property.Keys.Add(key);
                        key.ReadValue(handler, property);
                    }
                }
            }

            if (clipHeader.unknownOffsets != null)
            {
                if (clipHeader.unknownOffsets[0] > clipHeader.hermiteDataOffset)
                {
                    handler.Seek(clipHeader.hermiteDataOffset);
                    HermiteData = handler.ReadArray<HermiteInterpolationData>((int)(clipHeader.unknownOffsets[0] - clipHeader.hermiteDataOffset) / 16);
                }

                if (clipHeader.namesOffset != clipHeader.unknownOffsets[0])
                {
                    // TODO: exists in dmc5 pl0100_maken.motlist.85, many re8 files
                    // re8: last offset points to 32 byte blocks @ property count
                    UnknownData = handler.ReadArray<uint>((int)((clipHeader.namesOffset - clipHeader.unknownOffsets[0]) / 4));
                }
            }

            ExtraPropertyData.Version = Header.version;
            if (Version != ClipVersion.RE7 && clipHeader.endClipStructsOffset1 > 0)
            {
                handler.Seek(clipHeader.endClipStructsOffset1);
                // TODO figure out what and why this section exists
                ExtraPropertyData.Read(handler);
            }

            // setup prop/key hierarchy
            foreach (var track in Tracks)
            {
                if (track.propCount == 0) continue;
                track.Properties.AddRange(Properties.Slice((int)track.firstPropIdx, track.propCount));
            }

            return true;
        }

        protected override bool DoWrite(FileHandler handler)
        {
            long start = Start;
            var clipHeader = Header;
            clipHeader.magic = Magic;
            handler.Seek(start + clipHeader.Size);

            clipHeader.numNodes = Tracks.Count;
            clipHeader.clipDataOffset = handler.Tell();
            ReFlattenProperties(clipHeader);
            foreach (var track in Tracks)
            {
                track.WriteName(handler, clipHeader);
                track.Write(handler);
            }

            handler.Align(8);
            clipHeader.numProperties = Properties.Count;
            clipHeader.propertiesOffset = handler.Tell();
            foreach (var property in Properties)
            {
                var stringItem = handler.AsciiStringTableAdd(property.Info.FunctionName, false);
                property.Info.nameOffset = stringItem.TableOffset;

                stringItem = handler.StringTableAdd(property.Info.FunctionName, false);
                property.Info.nameOffset2 = stringItem.TableOffset;

                property.Info.Write(handler);
            }

            clipHeader.numKeys = ClipKeys.Count;
            clipHeader.keysOffset = handler.Tell();
            foreach (var key in ClipKeys)
            {
                key.Write(handler);
            }

            clipHeader.uknOffset1 = handler.Tell();
            clipHeader.hermiteDataOffset = handler.Tell();
            if (HermiteData?.Length > 0)
            {
                handler.WriteArray(HermiteData);
            }
            if (clipHeader.unknownOffsets?.Length != clipHeader.UnknownOffsetsCount) clipHeader.unknownOffsets = new long[clipHeader.UnknownOffsetsCount];
            ((Span<long>)clipHeader.unknownOffsets).Fill(handler.Tell());

            if (UnknownData?.Length > 0) handler.WriteArray(UnknownData);
            clipHeader.namesOffset = handler.Tell();
            handler.AsciiStringTableFlush();

            handler.Align(8);
            clipHeader.unicodeNamesOffset = handler.Tell();
            handler.StringTableFlush();

            handler.Align(16);
            clipHeader.endClipStructsOffset1 = handler.Tell() - 8;
            clipHeader.endClipStructsOffset2 = clipHeader.endClipStructsOffset1 + 8;
            if (Version != ClipVersion.RE7)
            {
                ExtraPropertyData.Write(handler);
            }

            clipHeader.Write(handler, start);
            return true;
        }

        private void ReFlattenProperties(ClipHeader clipHeader)
        {
            var pendingProps = new List<Property>(Properties.Count);
            var expectedcount = Properties.Count;
            Properties.Clear();
            // TODO handle Key flattening as well
            foreach (var track in Tracks)
            {
                track.propCount = track.Properties.Count;
                if (track.Properties.Count == 0)
                {
                    track.firstPropIdx = 0;
                    track.propCount = 0;
                    continue;
                }

                track.firstPropIdx = Properties.Count;
                static void InsertProperties(List<Property> allProps, List<Property> props)
                {
                    allProps.AddRange(props);
                    foreach (var prop in props)
                    {
                        if (!(prop.ChildProperties?.Count > 0))
                        {
                            if (prop.Info.DataType is PropertyType.NativeArray or PropertyType.NativeClass or PropertyType.Class or PropertyType.Struct or PropertyType.Array)
                            {
                                prop.Info.ChildStartIndex = 0;
                                prop.Info.ChildMembershipCount = 0;
                            }
                            continue;
                        }

                        prop.Info.ChildStartIndex = allProps.Count;
                        prop.Info.ChildMembershipCount = (ushort)prop.ChildProperties.Count;
                        InsertProperties(allProps, prop.ChildProperties);
                    }
                }
                InsertProperties(Properties, track.Properties);
            }
        }

        public override string ToString() => $"Clip {Header.guid}";
    }
}


namespace ReeLib
{
    public class ClipFile(RszFileOption option, FileHandler fileHandler) : BaseRszFile(option, fileHandler)
    {
        public ClipEntry ClipEntry { get; } = new();

        protected override bool DoRead()
        {
            return ClipEntry.Read(FileHandler);
        }

        protected override bool DoWrite()
        {
            return ClipEntry.Write(FileHandler);
        }
    }
}
