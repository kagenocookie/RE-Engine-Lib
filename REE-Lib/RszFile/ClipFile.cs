using System.Collections;
using System.Numerics;
using System.Runtime.CompilerServices;
using ReeLib.Common;
using ReeLib.InternalAttributes;
using ReeLib.via;

namespace ReeLib.Clip
{
    public enum ClipVersion
    {
        RE7 = 18,
        RE2_DMC5 = 27,
        RE3 = 34,
        RE8 = 40,
        RE_RT = 43,
        SF6 = 53,
        RE4 = 54,
        DD2 = 62,
        MHWilds = 85,
        Pragmata = 91,
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

    public static class ClipPropertyExtensions
    {
        public static object Read(this PropertyType type, FileHandler handler, IKeyValueContainer? relativeOffsets = null)
        {
            if (type == PropertyType.Unknown) return handler.Read<long>();

            switch (type)
            {
                case PropertyType.Bool: return handler.Read<bool>();
                case PropertyType.S8:  return handler.Read<sbyte>();
                case PropertyType.U8:  return handler.Read<byte>();
                case PropertyType.S16: return handler.Read<short>();
                case PropertyType.U16: return handler.Read<ushort>();
                case PropertyType.S32: return handler.Read<int>();
                case PropertyType.U32: return handler.Read<uint>();
                case PropertyType.S64: return handler.Read<long>();
                case PropertyType.U64: return handler.Read<ulong>();
                case PropertyType.F64: return handler.Read<double>();
                case PropertyType.F64_alt: { // yes they both look like doubles ¯\_(ツ)_/¯
                        var value = handler.Read<double>();
                        return value;
                    }
                case PropertyType.Str8:
                case PropertyType.Enum:
                    {
                        long offset = handler.Read<long>();
                        return handler.ReadAsciiString(relativeOffsets == null ? offset : relativeOffsets.AsciiStringOffset + offset);
                    }
                case PropertyType.Str16:
                case PropertyType.Asset:
                case PropertyType.GameObjectRef:
                    {
                        long offset = handler.Read<long>();
                        return handler.ReadWString(relativeOffsets == null ? offset : relativeOffsets.UnicodeStringOffset + offset * 2);
                    }
                case PropertyType.Guid:
                    {
                        long offset = handler.Read<long>();
                        if (relativeOffsets == null)
                        {
                            return handler.Read<Guid>(offset);
                        }
                        return Guid.Parse(handler.ReadWString(relativeOffsets.UnicodeStringOffset + offset * 2));
                    }
                case PropertyType.Vec2:
                case PropertyType.Float2:
                    return handler.Read<Vector2>(handler.Read<long>());
                case PropertyType.Float3:
                case PropertyType.Vec3:
                case PropertyType.PathPoint3D:
                case PropertyType.Point:
                    {
                        var offset = handler.Read<long>();
                        return handler.Read<Vector3>(relativeOffsets == null ? offset : relativeOffsets.DataOffset16B + offset * 16);
                    }
                case PropertyType.Float4:
                case PropertyType.Vec4:
                    return handler.Read<Vector4>(handler.Read<long>());
                case PropertyType.Position:
                    // yes, you read that right. Position seems to be a plain Vec3 except not written with offset
                    return handler.Read<Vector3>();
                case PropertyType.Int2: return handler.Read<Int2>(handler.Read<long>());
                case PropertyType.Int3: return handler.Read<Int3>(handler.Read<long>());
                case PropertyType.Int4: return handler.Read<Int4>(handler.Read<long>());
                case PropertyType.Uint2: return handler.Read<Uint2>(handler.Read<long>());
                case PropertyType.Uint3: return handler.Read<Uint3>(handler.Read<long>());
                case PropertyType.Uint4: return handler.Read<Uint4>(handler.Read<long>());
                case PropertyType.Color: return handler.Read<Color>(handler.Read<long>());
                case PropertyType.Rect: return handler.Read<Rect>(handler.Read<long>());
                case PropertyType.RangeI: return handler.Read<RangeI>(handler.Read<long>());
                case PropertyType.Size: return handler.Read<Size>(handler.Read<long>());
                case PropertyType.Action: return handler.Read<long>();
                case PropertyType.UserDataAsset: return handler.Read<long>(); // most likely an RSZ ObjectTable index, but which RSZ?
                default:
                    throw new Exception($"Unsupported PropertyType: {type}");
            }
        }

        private static void WriteOffsetValue<T>(T value, FileHandler handler, bool relativeOffsets) where T : unmanaged
        {
            if (relativeOffsets)
            {
                handler.OffsetContentTableAdd((h) => h.Write(value), false);
                var offset = handler.OffsetContentTable!.Items.Count - 1;
                handler.Write((long)offset);
            }
            else
            {
                handler.WriteOffsetContent((h) => h.Write(value));
            }
        }

        public static void Write(this PropertyType type, object Value, FileHandler handler, bool relativeOffsets)
        {
            if (type == PropertyType.Unknown)
            {
                handler.Write((long)(Value ?? 0L));
                return;
            }

            var endOffset = handler.Tell() + 8;
            switch (type)
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
                    handler.Write((long)(int)Value);
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
                case PropertyType.F64_alt:
                    handler.Write((double)Value);
                    break;
                case PropertyType.Str8:
                case PropertyType.Enum:
                    if (relativeOffsets)
                    {
                        var stringItem = handler.AsciiStringTableAdd((string)Value, false);
                        handler.Write((long)stringItem.TableOffset);
                    }
                    else
                    {
                        handler.WriteOffsetAsciiString((string)Value);
                    }
                    break;
                case PropertyType.Str16:
                case PropertyType.Asset:
                case PropertyType.GameObjectRef:
                    if (relativeOffsets)
                    {
                        var stringItem = handler.StringTableAdd((string)Value, false);
                        handler.Write((long)stringItem.TableOffset);
                    }
                    else
                    {
                        handler.WriteOffsetWString((string)Value);
                    }
                    break;
                case PropertyType.Guid:
                    if (relativeOffsets)
                    {
                        var stringItem = handler.StringTableAdd(((Guid)Value).ToString(), false);
                        handler.Write((long)stringItem.TableOffset);
                    }
                    else
                    {
                        handler.WriteOffsetContent(h => h.Write((Guid)Value));
                    }
                    break;
                case PropertyType.Vec2:
                case PropertyType.Float2:
                    WriteOffsetValue((Vector2)Value, handler, relativeOffsets);
                    break;
                case PropertyType.PathPoint3D:
                case PropertyType.Float3:
                case PropertyType.Vec3:
                case PropertyType.Point:
                    {
                        var vec = (Vector3)Value;
                        if (relativeOffsets)
                        {
                            handler.OffsetContentTableAdd((h) => {
                                h.Write(vec);
                                h.WriteNull(4);
                            }, false);
                            var offset = handler.OffsetContentTable!.Items.Count - 1;
                            handler.Write((long)offset);
                        }
                        else
                        {
                            handler.WriteOffsetContent(h => {
                                h.Write(vec);
                                h.WriteNull(4);
                            });
                        }
                    }
                    break;
                case PropertyType.Float4:
                case PropertyType.Vec4:
                    WriteOffsetValue((Vector4)Value, handler, relativeOffsets);
                    break;
                case PropertyType.Position:
                    handler.Write((Vector3)Value);
                    break;
                case PropertyType.Uint2: WriteOffsetValue((Uint2)Value, handler, relativeOffsets); break;
                case PropertyType.Uint3: WriteOffsetValue((Uint3)Value, handler, relativeOffsets); break;
                case PropertyType.Uint4: WriteOffsetValue((Uint4)Value, handler, relativeOffsets); break;
                case PropertyType.Int2: WriteOffsetValue((Int2)Value, handler, relativeOffsets); break;
                case PropertyType.Int3: WriteOffsetValue((Int3)Value, handler, relativeOffsets); break;
                case PropertyType.Int4: WriteOffsetValue((Int4)Value, handler, relativeOffsets); break;
                case PropertyType.Color: WriteOffsetValue((Color)Value, handler, relativeOffsets); break;
                case PropertyType.Rect: WriteOffsetValue((Rect)Value, handler, relativeOffsets); break;
                case PropertyType.RangeI: WriteOffsetValue((RangeI)Value, handler, relativeOffsets); break;
                case PropertyType.Size: WriteOffsetValue((Size)Value, handler, relativeOffsets); break;
                case PropertyType.Action: handler.Write((long)Value); break;
                case PropertyType.UserDataAsset: handler.Write((long)Value); break;
                default:
                    throw new Exception($"Unsupported PropertyType: {type}");
            }
            handler.WritePaddingUntil(endOffset);
        }
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
        F64_alt = 0xB,
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
        Position = 0x39,
        UserDataAsset = 0x3A,
    }

    public static class PropertyTypeUtils
    {
        public static RszFieldType ToRszFieldType(this PropertyType type)
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
                PropertyType.Enum => RszFieldType.String,
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
                PropertyType.Asset => RszFieldType.String,
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
                PropertyType.PathPoint3D => RszFieldType.Vec3,
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
                PropertyType.Position => RszFieldType.Vec3,
                PropertyType.UserDataAsset => RszFieldType.S64,
                _ => RszFieldType.ukn_type,
            };
        }
    }

    public interface IKeyValueContainer
    {
        long AsciiStringOffset { get; }
        long UnicodeStringOffset { get; }
        long DataOffset16B { get; }
    }

    public class Key : BaseModel
    {
        public float frame;
        public float rate;
        public InterpolationType interpolation;
        public KeyFlags flags;
        public uint unknown; // might be some sort of frame length?
        public int hermiteDataIndex;
        public object Value { get; set; } = null!;

        [Flags]
        public enum KeyFlags : byte
        {
            InstanceValue = 1,
            Ukn2 = 2,
        }

        public PropertyType PropertyType { get; set; }
        public ClipVersion Version { get; set; }

        private int CommonKeySize => GetKeySize(Version);

        public static int GetKeySize(ClipVersion version) => version switch {
            < ClipVersion.RE3 => 40,
            _ => 32,
        };

        public Key(ClipVersion version)
        {
            Version = version;
        }

        public void ResetValue()
        {
            var rszType = PropertyType.ToRszFieldType();
            if (rszType == RszFieldType.String)
            {
                if (Value is not string) Value = "";
                return;
            }

            var expectedType = RszInstance.RszFieldTypeToCSharpType(rszType);
            if (Value?.GetType() == expectedType) return;

            try
            {
                // try and maintain the original value when possible
                Value = Convert.ChangeType(Value, expectedType)!;
            }
            catch
            {
                Value = Activator.CreateInstance(expectedType)!;
            }
        }

        protected override bool DoRead(FileHandler handler)
        {
            handler.Read(ref frame);
            handler.Read(ref rate);
            handler.Read(ref interpolation);
            handler.Read(ref flags);
            handler.ReadNull(2);
            handler.Read(ref unknown);
            handler.Seek(Start + CommonKeySize);
            return true;
        }

        protected override bool DoWrite(FileHandler handler)
        {
            handler.Write(ref frame);
            handler.Write(ref rate);
            handler.Write(ref interpolation);
            handler.Write(ref flags);
            handler.WriteNull(2);
            handler.Write(ref unknown);
            handler.Seek(Start + 16);
            PropertyType.Write(Value, handler, true);
            handler.Write(ref hermiteDataIndex);
            handler.WriteNull(4);
            if (Version < ClipVersion.RE3) {
                handler.WriteNull(8);
            }
            var end = Start + CommonKeySize;
            handler.WriteNull((int)(end - handler.Tell()));
            return true;
        }

        protected void ReadValueOnly(FileHandler handler, Property property, IKeyValueContainer offsets)
        {
            handler.Seek(Start + (Version < ClipVersion.Pragmata ? 16 : 8));
            PropertyType = property.Info.DataType;
            Value = PropertyType.Read(handler, offsets);
        }

        public virtual void ReadValue(FileHandler handler, Property property, IKeyValueContainer offsets)
        {
            ReadValueOnly(handler, property, offsets);
            if (Version < ClipVersion.MHWilds)
            {
                handler.Seek(Start + 24);
                handler.Read(ref hermiteDataIndex);
                handler.ReadNull(4);
                if (Version < ClipVersion.RE3) {
                    handler.ReadNull(8);
                }
            }
        }

        public override string ToString() => $"[Frame {frame}]: {Value}";
    }

    public class UknKey1(ClipVersion version) : Key(version)
    {
        protected override bool DoRead(FileHandler handler)
        {
            throw new NotImplementedException();
            // handler.Read(ref frame);
            // handler.Read(ref interpolation);
            // handler.Read(ref flags);
            // Value = handler.Read<short>() != 0;
            // return true;
        }

        protected override bool DoWrite(FileHandler handler)
        {
            throw new NotImplementedException();
        }
    }

    public class ShortKey(ClipVersion version) : Key(version)
    {
        protected override bool DoRead(FileHandler handler)
        {
            handler.Read(ref frame);
            handler.Read(ref interpolation);
            handler.Read(ref flags);
            unknown = (uint)handler.Read<short>();
            return true;
        }

        protected override bool DoWrite(FileHandler handler)
        {
            handler.Write(ref frame);
            handler.Write(ref interpolation);
            handler.Write(ref flags);
            handler.Write((short)unknown);
            return true;
        }

        public override void ReadValue(FileHandler handler, Property property, IKeyValueContainer offsets)
        {
            // value already in header read
        }
    }

    public class ShortValueKey(ClipVersion version) : ShortKey(version)
    {
        protected override bool DoRead(FileHandler handler)
        {
            base.DoRead(handler);
            handler.Skip(8); // skip 8 bytes for ReadValue later
            return true;
        }

        protected override bool DoWrite(FileHandler handler)
        {
            base.DoWrite(handler);
            PropertyType.Write(Value, handler, true);
            return true;
        }

        public override void ReadValue(FileHandler handler, Property property, IKeyValueContainer offsets)
        {
            ReadValueOnly(handler, property, offsets);
        }
    }

    public class PropertyInfo : ReadWriteModel
    {
        public ClipVersion Version { get; set; }

        private int uknValue;
        public float startFrame;
        public float endFrame;
        public uint nameUtf16Hash;
        public uint wildsNameHash;
        public uint nameAsciiHash;

        internal long nameOffset;
        internal long dataOffset;
        public long ChildStartIndex;
        public ushort ChildMembershipCount;
        public short arrayIndex;
        public short speedPointNum;
        public PropertyType DataType;
        public byte uknByte00;
        public byte uknByte;
        internal long lastKeyOffset;
        internal long speedPointOffset;
        internal long clipPropertyOffset;

        public byte uknCount;
        public ulong uknRE7_2;
        public ulong uknRE7_3;
        public ulong uknRE7_4;
        internal long unicodeNameOffset;
        public string FunctionName { get; set; } = string.Empty;

        public long timelineUkn;
        public long timelineUkn2;

        public PropertyInfo(ClipVersion version)
        {
            Version = version;
        }

        protected override sealed bool ReadWrite<THandler>(THandler action)
        {
            if (Version == ClipVersion.RE7)
            {
                action.Do(ref DataType);
                action.Do(ref uknCount);
                action.Null(2);
                action.Do(ref uknValue);
            }
            else if (Version < ClipVersion.RE8)
            {
                action.Do(ref uknValue);
            }
            action.Do(ref startFrame);
            action.Do(ref endFrame);
            if (Version is ClipVersion.RE2_DMC5 or ClipVersion.RE3)
            {
                int two = 2;
                action.Do(ref two);
                action.Do(ref speedPointNum);
                action.Null(2);
                DataInterpretationException.DebugThrowIf(two != 2);
            }
            else if (Version == ClipVersion.RE7)
            {
                int two = 2, speedPoints = speedPointNum;
                action.Do(ref two);
                action.Do(ref speedPoints);
                speedPointNum = (short)speedPoints;
                DataInterpretationException.DebugThrowIf(two != 2 || speedPoints != 2);
            }
            else if (Version < ClipVersion.MHWilds)
            {
                action.Do(ref nameAsciiHash);
                action.Do(ref nameUtf16Hash);
            }
            else
            {
                action.Do(ref wildsNameHash);
                action.Do(ref nameUtf16Hash);
            }

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
            else // Version <= RE3
            {
                if (Version > ClipVersion.RE7)
                {
                    action.Do(ref DataType);
                    action.Do(ref uknCount);
                    action.Null(2);
                }

                if (Version == ClipVersion.RE3)
                {
                    action.Do(ref nameAsciiHash);
                    action.Do(ref nameUtf16Hash);
                }
                else if (Version > ClipVersion.RE7)
                    action.Null(8);
                else
                {
                    action.Do(ref uknRE7_2); // TODO RE7
                    action.Null(16);
                }
                action.Do(ref nameOffset);
                action.Do(ref unicodeNameOffset);
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
                    action.Null(6 + 8);
                    action.Do(ref uknRE7_4);
                }
                else
                {
                    // dmc5+
                    action.Null(6);
                    action.Do(ref timelineUkn);
                    action.Do(ref timelineUkn2);
                    action.Null(8);
                }
            }
            DataInterpretationException.DebugThrowIf(DataType == PropertyType.Unknown);
            return true;
        }

        protected override bool DoWrite(FileHandler handler)
        {
            nameUtf16Hash = MurMur3HashUtils.GetHash(FunctionName);
            nameAsciiHash = MurMur3HashUtils.GetAsciiHash(FunctionName);
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
                    case PropertyType.Size:        // 0x1F
                    case PropertyType.Uint2:       // 0x23
                    case PropertyType.Uint3:       // 0x24 (assumption)
                    case PropertyType.Uint4:       // 0x25 (assumption)
                    case PropertyType.OBB:         // 0x29
                    case PropertyType.Mat4:        // 0x2A
                    case PropertyType.Rect:        // 0x2B
                    case PropertyType.Plane:       // 0x2D
                    case PropertyType.Sphere:      // 0x2E
                    case PropertyType.Capsule:     // 0x2F
                    case PropertyType.AABB:        // 0x30
                    case PropertyType.Nullable:    // 0x31
                    case PropertyType.AnimationCurve:// 0x36
                    case PropertyType.KeyFrame:    // 0x37
                    case PropertyType.Position:    // 0x39
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
        internal int nodeCount;
        internal int propCount;
        public float Start_Frame;
        public float End_Frame;
        public Guid guid1;
        public Guid guid2;

        public byte nodeType;

        public ulong nameHash;
        internal long nameOffset;
        internal long childNodeIndex;
        internal long firstPropIdx;

        /// <summary>
        /// For embedded clips, there is always exactly 1 root track and all other tracks are a child of that one track, no multi level nesting
        /// </summary>
        public List<CTrack> ChildTracks { get; } = new();
        public List<Property> Properties { get; } = new();

        public ClipVersion Version { get; set; }
        public string Name { get; set; } = string.Empty;

        public CTrack(ClipVersion version)
        {
            Version = version;
        }

        protected override sealed bool ReadWrite<THandler>(THandler action)
        {
            if (Version >= ClipVersion.RE8)
            {
                action.Do(ref Unsafe.As<int, short>(ref nodeCount));
                action.Do(ref Unsafe.As<int, short>(ref propCount));
                action.Do(ref nodeType);
                action.Null(3);
                action.Do(ref nameHash);
                action.Do(ref nameOffset);
            }
            else
            {
                action.Do(ref nodeCount);
                action.Do(ref propCount);
                action.Do(ref Start_Frame);
                action.Do(ref End_Frame);
                action.Do(ref guid1);
                action.Do(ref guid2);
                if (Version == ClipVersion.RE3) {
                    action.Null(8);
                    action.Do(ref nameHash);
                    action.Do(ref nameOffset);
                } else {
                    if (Version == ClipVersion.RE7) {
                        action.Null(8);
                        action.Do(ref nameOffset);
                    } else {
                        action.Null(16);
                        action.Do(ref nameOffset);
                    }
                    long offset2 = nameOffset;
                    action.Do(ref offset2);
                    DataInterpretationException.DebugWarnIf(offset2 != nameOffset);
                }
            }
            action.Do(ref childNodeIndex);
            action.Do(ref firstPropIdx);
            return true;
        }

        protected override bool DoWrite(FileHandler handler)
        {
            nameHash = MurMur3HashUtils.GetCombineHash(Name);
            return base.DoWrite(handler);
        }

        public void ReadName(FileHandler handler, ClipHeader header)
        {
            ReadName(handler, header.unicodeNamesOffset, header.namesOffset);
        }

        public void ReadName(FileHandler handler, long unicodeOffset, long asciiOffset)
        {
            if (Version >= ClipVersion.RE3)
            {
                Name = handler.ReadWString(unicodeOffset + nameOffset * 2);
            }
            else
            {
                Name = handler.ReadAsciiString(asciiOffset + nameOffset);
                DataInterpretationException.DebugWarnIf(handler.ReadWString(unicodeOffset + nameOffset * 2) != Name);
            }
        }

        public void WriteName(FileHandler handler)
        {
            if (Version >= ClipVersion.RE3)
            {
                var stringItem = handler.StringTableAdd(Name, false);
                nameOffset = stringItem.TableOffset;
            }
            else
            {
                var stringItem = handler.AsciiStringTableAdd(Name, false);
                var stringItemUnicode = handler.StringTableAdd(Name, false);
                nameOffset = stringItem.TableOffset;
                DataInterpretationException.DebugWarnIf(stringItemUnicode.TableOffset != nameOffset);
            }
        }

        public override string ToString() => Name ?? $"Hash: {nameHash}";
    }


    /// <summary>
    /// 这是Embedded Clip的结构，用在motlists和gui文件中，单独的clip结构不一样，多几个字段
    /// 暂时没想好怎么做兼容
    /// </summary>
    public class ClipHeader : BaseModel, IKeyValueContainer
    {
        public uint magic = ClipFile.Magic;
        public ClipVersion version;
        public float numFrames;
        public int numNodes;
        public int numProperties;
        public int keysCount;
        public int boolKeysCount;
        public int uknKeysCount1;
        public int uknKeysCount2;
        public int uknCount3;

        public Guid guid;
        internal long clipDataOffset;
        internal long propertiesOffset;
        internal long uknKeysOffset1;
        internal long boolKeysOffset;
        internal long uknKeysOffset2;
        internal long keysOffset;
        internal long speedPointOffset;
        internal long hermiteDataOffset;

        // note: one of these is likely bezier3D data (based on .clip/.tml files), but doesn't seem used in motlists)
        internal long[] unknownOffsets = [];
        internal long[] wildsOffsets = [];

        internal long namesOffset;
        internal long unicodeNamesOffset;
        internal long stringsEndOffset;
        internal long endClipStructsOffset;

        internal int UnknownOffsetsCount => version switch
        {
            ClipVersion.RE7 => 3,
            ClipVersion.RE4 or ClipVersion.SF6 => 1,
            >= ClipVersion.MHWilds => 2,
            ClipVersion.RE2_DMC5 => 3,
            _ => 2,
        };

        long IKeyValueContainer.AsciiStringOffset => namesOffset;
        long IKeyValueContainer.UnicodeStringOffset => unicodeNamesOffset;
        long IKeyValueContainer.DataOffset16B => throw new NotImplementedException();

        protected override bool DoRead(FileHandler handler)
        {
            handler.Read(ref magic);
            handler.Read(ref version);
            handler.Read(ref numFrames);
            handler.Read(ref numNodes);
            handler.Read(ref numProperties);
            handler.Read(ref keysCount);
            if (version >= ClipVersion.MHWilds)
            {
                handler.Read(ref boolKeysCount);
                handler.Read(ref uknKeysCount1);
                handler.Read(ref uknKeysCount2);
                handler.Read(ref uknCount3);
                DataInterpretationException.DebugThrowIf(uknKeysCount1 > 0);
                DataInterpretationException.DebugThrowIf(uknCount3 > 0);
            }
            if (version < ClipVersion.RE3)
            {
                handler.Read(ref guid);
            }
            handler.Read(ref clipDataOffset);
            handler.Read(ref propertiesOffset);
            handler.Read(ref keysOffset);
            if (version >= ClipVersion.MHWilds)
            {
                handler.Read(ref boolKeysOffset);
                handler.Read(ref uknKeysOffset1);
                handler.Read(ref uknKeysOffset2);
            }
            handler.Read(ref speedPointOffset);
            handler.Read(ref hermiteDataOffset);
            unknownOffsets = handler.ReadArray<long>(UnknownOffsetsCount);
            DataInterpretationException.ThrowIfNotEqualValues<long>(unknownOffsets);
            handler.Read(ref namesOffset);
            handler.Read(ref unicodeNamesOffset);
            handler.Read(ref stringsEndOffset);
            handler.Read(ref endClipStructsOffset);
            return true;
        }

        protected override bool DoWrite(FileHandler handler)
        {
            handler.Write(magic);
            handler.Write(ref version);
            handler.Write(ref numFrames);
            handler.Write(ref numNodes);
            handler.Write(ref numProperties);
            handler.Write(ref keysCount);
            if (version >= ClipVersion.MHWilds)
            {
                handler.Write(ref boolKeysCount);
                handler.Write(ref uknKeysCount1);
                handler.Write(ref uknKeysCount2);
            }
            if (version >= ClipVersion.MHWilds)
            {
                handler.Write(ref uknCount3);
            }
            if (version != ClipVersion.RE3 && version < ClipVersion.RE8)
            {
                handler.Write(ref guid);
            }
            handler.Write(ref clipDataOffset);
            handler.Write(ref propertiesOffset);
            handler.Write(ref keysOffset);
            if (version >= ClipVersion.MHWilds)
            {
                handler.Write(ref boolKeysOffset);
                handler.Write(ref uknKeysOffset1);
                handler.Write(ref uknKeysOffset2);
            }
            handler.Write(ref speedPointOffset);
            handler.Write(ref hermiteDataOffset);
            handler.WriteArray(unknownOffsets!);
            handler.Write(ref namesOffset);
            handler.Write(ref unicodeNamesOffset);
            handler.Write(ref stringsEndOffset);
            handler.Write(ref endClipStructsOffset);
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

    public struct SpeedPointData
    {
        public float f1;
        public float f2;
        public long a;
        public long b;

        public override string ToString() => $"{f1} {f2}  {a} {b}";
    }

    public struct HermiteInterpolationData
    {
        public float x1;
        public float y1;
        public float x2;
        public float y2;

        public override string ToString() => $"{x1},{y1}  {x2},{y2}";
    }

    public struct Bezier3DKeys
    {
        public float x1;
        public float y1;
        public float x2;
        public float y2;
        public float x3;
        public float y3;
        public float x4;
        public float y4;

        public override string ToString() => $"{x1},{y1}  {x2},{y2}  {x3},{y3}  {x4},{y4}";
    }

    [RszGenerate, RszAutoReadWrite, RszVersionedObject(typeof(ClipVersion))]
    public partial class ClipInfoStruct : BaseModel
    {
        public float f1;
        public UndeterminedFieldType f2;
        public uint a;
        public UndeterminedFieldType b;
        public uint c;

        public float x1;
        public UndeterminedFieldType x2;
        public UndeterminedFieldType x3;
        [RszVersion(nameof(Version), "<", ClipVersion.RE3, EndAt = nameof(dmc5_x5))]
        public UndeterminedFieldType dmc5_x4;
        public UndeterminedFieldType dmc5_x5;

        [RszIgnore] public ClipVersion Version;

        public static int GetSize(ClipVersion version) => version switch {
            < ClipVersion.RE3 => 40,
            _ => 32,
        };

        public override string ToString() => $"{f1} {f2} / {a} {b} {c}";
    }
    public class EmbeddedClip : BaseModel
    {
        public ClipHeader Header { get; } = new();
        public List<CTrack> Tracks { get; } = new();
        public List<Property> Properties { get; } = new();
        public List<Key> ClipKeys { get; } = new();
        public List<ShortKey> ShortKeys { get; } = new();
        public List<UknKey1> UknKeys1 { get; } = new();
        public List<ShortValueKey> ShortValueKeys { get; } = new();
        public List<HermiteInterpolationData> HermiteData { get; } = new();
        public List<Bezier3DKeys> Bezier3DData { get; } = new();
        public List<SpeedPointData> SpeedPointData { get; } = new();
        public List<ClipInfoStruct> ClipInfoList { get; } = new();

        public float FrameCount { get => Header.numFrames; set => Header.numFrames = value; }
        public Guid Guid { get => Header.guid; set => Header.guid = value; }

        public ClipExtraPropertyData ExtraPropertyData = new();

        public ClipVersion Version { get => Header.version; set => Header.version = value; }

        protected override bool DoRead(FileHandler handler)
        {
            Tracks.Clear();
            Properties.Clear();
            ClipKeys.Clear();
            ShortKeys.Clear();
            UknKeys1.Clear();
            ShortValueKeys.Clear();
            var clipHeader = Header;
            if (!clipHeader.Read(handler)) return false;
            if (clipHeader.magic != ClipFile.Magic)
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

            var speedPointCount = 0;
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
                    if (Header.version <= ClipVersion.RE2_DMC5)
                    {
                        speedPointCount += (int)property.Info.speedPointNum;
                    }
                }
            }

            var hermiteKeys = 0;
            var bezier3dCount = 0;
            if (clipHeader.keysCount > 0)
            {
                handler.Seek(clipHeader.keysOffset);
                for (int i = 0; i < clipHeader.keysCount; i++)
                {
                    Key key = new(Version);
                    key.Read(handler);
                    ClipKeys.Add(key);
                    if (key.interpolation == InterpolationType.Hermite) hermiteKeys++;
                    if (key.interpolation == InterpolationType.Bezier3D) bezier3dCount++;
                }
            }

            if (clipHeader.boolKeysCount > 0)
            {
                handler.Seek(clipHeader.boolKeysOffset);
                for (int i = 0; i < clipHeader.boolKeysCount; i++)
                {
                    ShortKey key = new(Version);
                    key.Read(handler);
                    ShortKeys.Add(key);
                }
            }

            if (clipHeader.uknKeysCount1 > 0)
            {
                handler.Seek(clipHeader.uknKeysOffset1);
                for (int i = 0; i < clipHeader.uknKeysCount1; i++)
                {
                    UknKey1 key = new(Version);
                    key.Read(handler);
                    UknKeys1.Add(key);
                }
            }

            if (clipHeader.uknKeysCount2 > 0)
            {
                handler.Seek(clipHeader.uknKeysOffset2);
                for (int i = 0; i < clipHeader.uknKeysCount2; i++)
                {
                    ShortValueKey key = new(Version);
                    key.Read(handler);
                    ShortValueKeys.Add(key);
                }
            }

            // sub properties or sub keys
            foreach (var property in Properties)
            {
                if (property.Info.ChildMembershipCount == 0) continue;
                if (property.IsPropertyContainer)
                {
                    property.ChildProperties ??= new();
                    property.ChildProperties.AddRange(Properties.Slice((int)property.Info.ChildStartIndex, property.Info.ChildMembershipCount));
                    continue;
                }

                property.Keys ??= new();
                if (Version >= ClipVersion.MHWilds)
                {
                    if (property.Info.uknByte == 16)
                    {
                        for (long i = property.Info.ChildStartIndex; i < property.Info.ChildMembershipCount + property.Info.ChildStartIndex; i++)
                        {
                            var key = ShortKeys[(int)i];
                            property.Keys.Add(key);
                            DataInterpretationException.DebugThrowIf(property.Info.DataType != PropertyType.Bool);
                        }

                        continue;
                    }

                    if (property.Info.uknByte == 48)
                    {
                        for (long i = property.Info.ChildStartIndex; i < property.Info.ChildMembershipCount + property.Info.ChildStartIndex; i++)
                        {
                            Key key = ShortValueKeys[(int)i];
                            property.Keys.Add(key);
                            key.ReadValue(handler, property, Header);
                        }

                        continue;
                    }
                    DataInterpretationException.DebugThrowIf(property.Info.uknByte != 0);
                }

                for (long i = property.Info.ChildStartIndex; i < property.Info.ChildMembershipCount + property.Info.ChildStartIndex; i++)
                {
                    Key key = ClipKeys[(int)i];
                    property.Keys.Add(key);
                    key.ReadValue(handler, property, Header);
                }
            }

            if (hermiteKeys > 0)
            {
                handler.Seek(Header.hermiteDataOffset);
                HermiteData.ReadStructList(handler, hermiteKeys);
                DataInterpretationException.DebugThrowIf((Header.unknownOffsets[0] - Header.hermiteDataOffset) / 16 != hermiteKeys);
            }

            if (bezier3dCount > 0)
            {
                throw new NotImplementedException("Offset for clip Bezier3D data unknown");
                // handler.Seek(Header.bezier3DDataOffset);
                // Bezier3DData.ReadStructList(handler, bezier3dCount);
            }

            if (clipHeader.unknownOffsets.Length > 0)
            {
                var clipInfoOffset = clipHeader.unknownOffsets[0];
                if (clipHeader.namesOffset != clipInfoOffset)
                {
                    handler.Seek(clipInfoOffset);
                    var clipInfoCount = (int)(Header.namesOffset - clipInfoOffset) / ClipInfoStruct.GetSize(Header.version);
                    DataInterpretationException.DebugThrowIf((Header.namesOffset - clipInfoOffset) % ClipInfoStruct.GetSize(Header.version) != 0);
                    for (int i = 0; i < clipInfoCount; ++i)
                    {
                        var clip = new ClipInfoStruct() { Version = Header.version };
                        clip.Read(handler);
                        ClipInfoList.Add(clip);
                    }
                }
            }

            if (speedPointCount > 0)
            {
                handler.Seek(Header.speedPointOffset);
                SpeedPointData.ReadStructList(handler, speedPointCount);
                DataInterpretationException.DebugThrowIf((Header.hermiteDataOffset - Header.speedPointOffset) / 24 != speedPointCount);
            }

            ExtraPropertyData.Version = Header.version;
            if (Version != ClipVersion.RE7 && clipHeader.endClipStructsOffset > 0)
            {
                handler.Seek(clipHeader.endClipStructsOffset);
                // TODO figure out what and why this section exists
                ExtraPropertyData.Read(handler);
            }

            // setup prop/key hierarchy
            foreach (var track in Tracks)
            {
                if (track.propCount != 0)
                    track.Properties.AddRange(Properties.Slice((int)track.firstPropIdx, track.propCount));

                if (track.nodeCount != 0)
                    track.ChildTracks.AddRange(Tracks.Slice((int)track.childNodeIndex, track.nodeCount));

                DataInterpretationException.DebugThrowIf(track == Tracks[0] && track.ChildTracks.Any(ct => ct.ChildTracks.Count != 0));
                DataInterpretationException.DebugThrowIf(track != Tracks[0] && !Tracks[0].ChildTracks.Contains(track));
            }

            return true;
        }

        protected override bool DoWrite(FileHandler handler)
        {
            long start = Start;
            var clipHeader = Header;
            clipHeader.magic = ClipFile.Magic;
            handler.Seek(start + clipHeader.Size);

            clipHeader.numNodes = Tracks.Count;
            clipHeader.clipDataOffset = handler.Tell();
            ReFlattenProperties(clipHeader);
            foreach (var track in Tracks)
            {
                track.WriteName(handler);
                track.Write(handler);
            }

            handler.Align(8);
            clipHeader.numProperties = Properties.Count;
            clipHeader.propertiesOffset = handler.Tell();
            foreach (var property in Properties)
            {
                var stringItem = handler.AsciiStringTableAdd(property.Info.FunctionName, false);
                property.Info.nameOffset = stringItem.TableOffset;

                if (Version <= ClipVersion.RE3)
                {
                    stringItem = handler.StringTableAdd(property.Info.FunctionName, false);
                    property.Info.unicodeNameOffset = stringItem.TableOffset;
                }

                property.Info.Write(handler);
            }

            clipHeader.keysCount = ClipKeys.Count;
            clipHeader.boolKeysCount = ShortKeys.Count;
            clipHeader.uknKeysCount1 = UknKeys1.Count;
            clipHeader.uknKeysCount2 = ShortValueKeys.Count;
            clipHeader.keysOffset = handler.Tell();
            foreach (var key in ClipKeys)
            {
                key.Write(handler);
            }
            clipHeader.boolKeysOffset = handler.Tell();
            foreach (var key in ShortKeys)
            {
                key.Write(handler);
            }
            clipHeader.uknKeysOffset1 = handler.Tell();
            foreach (var key in UknKeys1)
            {
                key.Write(handler);
            }
            clipHeader.uknKeysOffset2 = handler.Tell();
            foreach (var key in ShortValueKeys)
            {
                key.Write(handler);
            }

            clipHeader.speedPointOffset = handler.Tell();
            SpeedPointData.Write(handler);

            clipHeader.hermiteDataOffset = handler.Tell();
            HermiteData.Write(handler);

            if (clipHeader.unknownOffsets?.Length != clipHeader.UnknownOffsetsCount) clipHeader.unknownOffsets = new long[clipHeader.UnknownOffsetsCount];
            ((Span<long>)clipHeader.unknownOffsets).Fill(handler.Tell());

            ClipInfoList.Write(handler);

            clipHeader.namesOffset = handler.Tell();
            handler.AsciiStringTableFlush();

            handler.Align(8);
            clipHeader.unicodeNamesOffset = handler.Tell();
            handler.StringTableFlush();

            handler.Align(8);
            clipHeader.stringsEndOffset = handler.Tell();
            handler.Align(16);
            clipHeader.endClipStructsOffset = handler.Tell();
            if (Version != ClipVersion.RE7)
            {
                ExtraPropertyData.Write(handler);
            }

            clipHeader.Write(handler, start);
            return true;
        }

        private void ReFlattenProperties(ClipHeader clipHeader)
        {
            Properties.Clear();
            ClipKeys.Clear();
            ShortKeys.Clear();
            UknKeys1.Clear();
            ShortValueKeys.Clear();
            foreach (var track in Tracks)
            {
                track.propCount = track.Properties.Count;
                if (track.propCount == 0)
                {
                    track.firstPropIdx = 0;
                    if (track == Tracks[0])
                    {
                        track.nodeCount = Tracks.Count - 1;
                    }
                    continue;
                }

                track.firstPropIdx = Properties.Count;
                static void InsertProperties(List<Property> allProps, EmbeddedClip clip, List<Property> props)
                {
                    allProps.AddRange(props);
                    foreach (var prop in props)
                    {
                        if (!prop.IsPropertyContainer)
                        {
                            prop.Keys ??= new();
                            prop.Info.ChildMembershipCount = (ushort)prop.Keys.Count;
                            if (prop.Keys.Count == 0)
                            {
                                prop.Info.ChildStartIndex = 0;
                                continue;
                            }
                            IList keyType = prop.Keys.First() switch {
                                ShortValueKey => clip.ShortValueKeys,
                                ShortKey => clip.ShortKeys,
                                UknKey1 => clip.UknKeys1,
                                Key => clip.ClipKeys,
                                _ => throw new NotImplementedException("Unsupported key type " + prop.Keys.First().GetType()),
                            };
                            prop.Info.ChildStartIndex = keyType.Count;

                            foreach (var key in prop.Keys) {
                                keyType.Add(key);
                            }
                            continue;
                        }

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
                        InsertProperties(allProps, clip, prop.ChildProperties);
                    }
                }
                InsertProperties(Properties, this, track.Properties);
            }
        }

        public override string ToString() => $"Clip {Header.guid}";
    }
}


namespace ReeLib
{
    public class ClipFile(FileHandler fileHandler) : TmlFile(fileHandler)
    {
        public const int Magic = 0x50494C43;
    }

    public class UcurveFile(FileHandler fileHandler) : TmlFile(fileHandler)
    {
    }
}

