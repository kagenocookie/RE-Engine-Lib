using System.Runtime.CompilerServices;
using ReeLib.Clip;
using ReeLib.Common;
using ReeLib.InternalAttributes;

namespace ReeLib.Clip
{
    public enum ClipVersion
    {
        RE7 = 10,
        RE7_RT = 11,
        RE2_DMC5 = 27,
        RE3 = 34,
        MHR_DEMO = 40,
        RE8 = MHR_DEMO,
        RE2_RT = 43,
        RE3_RT = RE2_RT,
        MHR = RE2_RT,
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
            handler.Read(ref reserved);
            handler.Read(ref reserved2);
            handler.Seek(Start + ClipFile.GetTarget()!.KeySize);
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
            WriteValue(handler);
            handler.Seek(Start + ClipFile.GetTarget()!.KeySize);
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
                        if (clipFile.Header.namesOffsetExtra == null)
                        {
                            throw new Exception("namesOffsetExtra is null");
                        }
                        Value = handler.ReadAsciiString(clipFile.Header.namesOffsetExtra[1] + offset);
                    }
                    break;
                case PropertyType.Str16:
                case PropertyType.Asset:
                case PropertyType.Guid:
                    {
                        long offset = handler.Read<long>();
                        if (clipFile.Header.namesOffsetExtra == null)
                        {
                            throw new Exception("namesOffsetExtra is null");
                        }
                        // clipHeader.unicodeNamesOffs + start + offset*2
                        Value = handler.ReadWString(clipFile.Header.unicodeNamesOffset + offset);
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
                handler.Write((long)Value);
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
                case PropertyType.F32:
                    handler.Write((float)Value);
                    break;
                case PropertyType.F64:
                    handler.Write((double)Value);
                    break;
                case PropertyType.Str8:
                case PropertyType.Enum:
                    {
                        string text = (string)Value;
                        var stringItem = handler.AsciiStringTableAdd(text, false);
                        long offset = stringItem!.TableOffset;
                        handler.Write(offset);
                    }
                    break;
                case PropertyType.Str16:
                case PropertyType.Asset:
                case PropertyType.Guid:
                    {
                        string text = (string)Value;
                        var stringItem = handler.StringTableAdd(text, false);
                        long offset = stringItem!.TableOffset;
                        handler.Write(offset);
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
                action.Skip(2);
                if (Version == ClipVersion.RE3)
                    action.Do(ref RE3hash);
                else
                    action.Skip(8);
                if (Version == ClipVersion.RE7)
                {
                    action.Skip(8);
                    action.Do(ref uknRE7_2);
                }
                action.Do(ref nameOffset);
                action.Do(ref nameOffset2);  // why?
                if (Version == ClipVersion.RE7)
                {
                    action.Do(ref uknRE7_3);
                    action.Skip(8);
                }
                action.Skip(8);
                if (Version > ClipVersion.RE7)
                {
                    action.Skip(16);
                }
                action.Do(ref ChildStartIndex);
                action.Do(ref ChildMembershipCount);
                if (Version == ClipVersion.RE7)
                {
                    action.Skip(8);
                    action.Do(ref uknRE7_4);
                }
                else
                {
                    // dmc5
                    action.Skip(30);
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

        public bool IsProertyContainer
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
                action.Skip(3);
            }
            else
            {
                action.Do(ref nodeCount);
                action.Do(ref propCount);
                action.Do(ref Start_Frame);
                action.Do(ref End_Frame);
                action.Do(ref guid1);
                action.Do(ref guid2);
                action.Skip(8);
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
            else if (header.namesOffsetExtra != null)
            {
                Name = handler.ReadAsciiString(header.namesOffsetExtra[1] + nameOffset);
            }
        }

        public void WriteName(FileHandler handler, ClipHeader header)
        {
            if (Version >= ClipVersion.RE3)
            {
                var stringItem = handler.StringTableAdd(Name, false);
                nameOffset = stringItem!.TableOffset;
            }
            else if (header.namesOffsetExtra != null)
            {
                var stringItem = handler.AsciiStringTableAdd(Name, false);
                nameOffset = stringItem!.TableOffset;
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
        public uint magic;
        public ClipVersion version;
        public float numFrames;
        public int numNodes;
        public int numProperties;
        public int numKeys;

        public Guid guid;
        public long clipDataOffset;
        public long propertiesOffset;
        public long keysOffset;
        public long namesOffset;

        public long namesOffset2;

        public long[]? namesOffsetExtra;

        public long unicodeNamesOffset;
        public long endClipStructsOffset1;
        public long endClipStructsOffset2;

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
            handler.Read(ref clipDataOffset);
            handler.Read(ref propertiesOffset);
            handler.Read(ref keysOffset);
            handler.Read(ref namesOffset);
            if (version == ClipVersion.RE2_DMC5)
            {
                handler.Read(ref namesOffset2);
            }
            namesOffsetExtra = version switch
            {
                ClipVersion.RE7 => handler.ReadArray<long>(5),
                ClipVersion.RE4 or ClipVersion.SF6 => handler.ReadArray<long>(3),
                _ => handler.ReadArray<long>(4),
            };
            handler.Read(ref unicodeNamesOffset);
            handler.Read(ref endClipStructsOffset1);
            if (version <= ClipVersion.RE4)
            {
                handler.Read(ref endClipStructsOffset2);
            }
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
            handler.Write(ref clipDataOffset);
            handler.Write(ref propertiesOffset);
            handler.Write(ref keysOffset);
            handler.Write(ref namesOffset);
            if (version == ClipVersion.RE2_DMC5)
            {
                handler.Write(ref namesOffset2);
            }
            handler.WriteArray(namesOffsetExtra!);
            handler.Write(ref unicodeNamesOffset);
            handler.Write(ref endClipStructsOffset1);
            if (version <= ClipVersion.RE4)
            {
                handler.Write(ref endClipStructsOffset2);
            }
            return true;
        }
    }


    public class EndClipStruct : BaseModel
    {
        public int ukn = -1;
        public uint ukn1;
        public ulong ukn2;

        public uint ukn3;
        public uint ukn4;
        public uint ukn5;

        public ClipVersion Version;

        public override string ToString() => $"{ukn} {ukn1} {ukn2}";

        protected override bool DoRead(FileHandler handler)
        {
            handler.Read(ref ukn);
            handler.Read(ref ukn1);
            handler.Read(ref ukn2);
            if (Version >= ClipVersion.DD2)
            {
                handler.Read(ref ukn3);
                handler.Read(ref ukn4);
                handler.Read(ref ukn5);
            }
            return true;
        }

        protected override bool DoWrite(FileHandler handler)
        {
            handler.Write(ref ukn);
            handler.Write(ref ukn1);
            handler.Write(ref ukn2);
            if (Version >= ClipVersion.DD2)
            {
                handler.Write(ref ukn3);
                handler.Write(ref ukn4);
                handler.Write(ref ukn5);
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

        protected override bool DoRead(FileHandler handler)
        {
            // TODO figure out why there's two sets of property infos
            // I'm thinking it might be a "start" values and "end" / "after" values
            handler.Align(16);
            var count1 = handler.ReadInt();
            var count2 = handler.ReadInt(); // for versions with a single prop list, this is just padding
            var offset1 = handler.Read<long>();
            // TODO find correct version
            var offset2 = Version > ClipVersion.RE4 ? handler.Read<long>() : 0;
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
            throw new NotImplementedException();
        }

        public override string ToString() => $"[Props: {Props1.Count} + {Props2.Count}]";
    }

    public class ClipEntry : BaseModel
    {
        public ClipHeader Header { get; } = new();
        public long endClipOffset;
        public long lastUnicodeNameOffset;
        public List<CTrack> CTrackList { get; } = new();
        public List<Property> Properties { get; } = new();
        public List<Key> ClipKeys { get; } = new();
        public float[]? Unknown_Floats { get; set; }

        public ClipExtraPropertyData ExtraPropertyData = new();


        public const uint Magic = 0x50494C43;
        public const string Extension = ".clip";

        public ClipVersion Version => Header.version;

        public int PropertySize => Version switch
        {
            ClipVersion.RE3 => 32,
            ClipVersion.RE7 => 120,
            ClipVersion.MHR_DEMO or ClipVersion.RE8 => 72,
            ClipVersion.SF6 or ClipVersion.RE4 => 56,
            _ => 112,
        };

        public int KeySize => Version switch
        {
            < ClipVersion.RE3 => 40,
            _ => 32,
        };

        protected override bool DoRead(FileHandler handler)
        {
            CTrackList.Clear();
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
                    CTrackList.Add(cTrack);
                }
            }

            if (clipHeader.numProperties > 0)
            {
                handler.Seek(clipHeader.propertiesOffset);
                for (int i = 0; i < clipHeader.numProperties; i++)
                {
                    Property property = new(Version);
                    property.Info.Read(handler);
                    if (clipHeader.namesOffsetExtra != null && clipHeader.namesOffsetExtra.Length > 1)
                    {
                        property.Info.FunctionName = handler.ReadAsciiString(clipHeader.namesOffsetExtra[1] + property.Info.nameOffset);
                    }
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
                if (property.IsProertyContainer)
                {
                    property.ChildProperties ??= new();
                    for (long i = property.Info.ChildStartIndex; i < property.Info.ChildMembershipCount; i++)
                    {
                        property.ChildProperties.Add(Properties[(int)i]);
                    }
                }
                else
                {
                    property.Keys ??= new();
                    for (long i = property.Info.ChildStartIndex; i < property.Info.ChildMembershipCount; i++)
                    {
                        Key key = ClipKeys[(int)i];
                        property.Keys.Add(key);
                        key.ReadValue(handler, property);
                    }
                }
            }

            if (clipHeader.namesOffsetExtra != null)
            {
                if (clipHeader.namesOffsetExtra[1] - clipHeader.namesOffset > 0)
                {
                    handler.Seek(clipHeader.namesOffset);
                    Unknown_Floats = handler.ReadArray<float>((int)(clipHeader.namesOffsetExtra[1] - clipHeader.namesOffset) / 4);
                }

                /* handler.Seek(clipHeader.namesOffsetExtra[1]);
                for (int i = 0; i < clipHeader.numProperties + clipHeader.numNodes; i++)
                {

                } */
            }

            ExtraPropertyData.Version = Header.version;
            if (Version != ClipVersion.RE7 && clipHeader.numNodes > 1)
            {
                handler.Seek(clipHeader.endClipStructsOffset1);
                // TODO figure out what and why this section exists
                ExtraPropertyData.Read(handler);
            }

            return true;
        }

        protected override bool DoWrite(FileHandler handler)
        {
            long start = Start;
            var clipHeader = Header;
            clipHeader.magic = Magic;
            handler.Seek(start + clipHeader.Size);

            clipHeader.numNodes = CTrackList.Count;
            clipHeader.clipDataOffset = handler.Tell();
            foreach (var cTrack in CTrackList)
            {
                cTrack.WriteName(handler, clipHeader);
                cTrack.Write(handler);
            }

            handler.Align(16);
            clipHeader.numProperties = Properties.Count;
            clipHeader.propertiesOffset = handler.Tell();
            foreach (var property in Properties)
            {
                var stringItem = handler.AsciiStringTableAdd(property.Info.FunctionName, false);
                property.Info.nameOffset = stringItem!.TableOffset;
                property.Info.Write(handler);
            }

            handler.Align(16);
            clipHeader.numKeys = ClipKeys.Count;
            clipHeader.keysOffset = handler.Tell();
            foreach (var key in ClipKeys)
            {
                key.Write(handler);
            }

            // TODO: check namesOffset2?
            clipHeader.namesOffset = handler.Tell();
            if (Unknown_Floats != null)
            {
                handler.WriteArray(Unknown_Floats);
            }
            if (clipHeader.namesOffsetExtra != null)
            {
                ((Span<long>)clipHeader.namesOffsetExtra).Fill(handler.Tell());
            }
            handler.AsciiStringTableFlush();

            handler.Align(8);
            clipHeader.unicodeNamesOffset = handler.Tell();
            handler.StringTableFlush();

            handler.Align(16);


            handler.Align(16);
            clipHeader.endClipStructsOffset1 = handler.Tell();
            clipHeader.endClipStructsOffset2 = clipHeader.endClipStructsOffset1;
            if (Version != ClipVersion.RE7 && clipHeader.numNodes > 1)
            {
                ExtraPropertyData.Write(handler);
            }

            clipHeader.Write(handler, start);
            return true;
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
