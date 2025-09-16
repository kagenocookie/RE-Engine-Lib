using System.Numerics;
using ReeLib.Clip;
using ReeLib.Common;
using ReeLib.InternalAttributes;
using ReeLib.Mot;

namespace ReeLib.Mot
{
    public enum MotVersion
    {
        RE7 = 43,
        RE2_DMC5 = 65,
        RE3 = 78,
        MHR_DEMO = 456,
        RE8 = 458,
        RE2_RT = 492,
        RE3_RT = RE2_RT,
        RE7_RT = RE2_RT,
        MHR = 495,
        SF6 = 603,
        RE4 = 613,
        DD2 = 698,
    }


    public class MotHeader : ReadWriteModel
    {
        public MotVersion version;
        public uint magic;
        public uint ukn00;
        public uint motSize;
        public long boneHeaderOffsetStart; // BoneBaseDataPointer
        public long boneClipHeaderOffset; // BoneDataPointer

        public long clipFileOffset;
        public long jmapOffset;
        public long motEndClipDataOffset;

        // public long namesOffset;
        public float frameCount;
        public float blending; // Set to 0 to enable repeating
        public float startFrame;
        public float endFrame;
        public ushort boneCount;
        public ushort boneClipCount;
        public byte clipCount;
        public byte motEndClipCount;
        public ushort FrameRate;
        public ushort uknPointerCount;
        public ushort uknShort;

        public string motName = string.Empty;
        public string? jointMapPath = string.Empty;

        internal long BoneHeaderOffsetStartOffset => Start + 16;

        protected override bool ReadWrite(IFileHandlerAction action)
        {
            action.Then(ref version)
                 ?.Then(ref magic)
                 ?.Then(ref ukn00)
                 ?.Then(ref motSize)
                 ?.Then(ref boneHeaderOffsetStart)
                 ?.Then(ref boneClipHeaderOffset)
                 ?.Skip(8);
            if (version >= MotVersion.MHR_DEMO)
            {
                action.Skip(8)
                     ?.Then(ref clipFileOffset)
                     ?.Then(ref jmapOffset)
                     ?.Then(ref motEndClipDataOffset)
                     ?.Skip(16);
            }
            else
            {
                action.Then(ref jmapOffset)
                     ?.Then(ref clipFileOffset)
                     ?.Skip(16)
                     ?.Then(ref motEndClipDataOffset);
            }
            action.HandleOffsetWString(ref motName)
                 ?.Then(ref frameCount)
                 ?.Then(ref blending)
                 ?.Then(ref startFrame)
                 ?.Then(ref endFrame)
                 ?.Then(ref boneCount)
                 ?.Then(ref boneClipCount)
                 ?.Then(ref clipCount)
                 ?.Then(ref motEndClipCount)
                 ?.Then(ref FrameRate)
                 ?.Then(ref uknPointerCount)
                 ?.Then(ref uknShort);
            return true;
        }
    }


    public class MotBone(BoneHeader header)
    {
        public BoneHeader Header { get; set; } = header;

        public string Name { get => Header.boneName; set => Header.boneName = value; }
        public int Index { get => Header.Index; set => Header.Index = value; }
        public Vector4 Translation { get => Header.translation; set => Header.translation = value; }
        public Quaternion Quaternion { get => Header.quaternion; set => Header.quaternion = value; }

        public MotBone? Parent { get; set; }
        public List<MotBone> Children { get; } = new();

        public override string ToString() => Header.boneName;
    }

    [RszGenerate, RszAutoReadWrite]
    public partial class BoneHeader : BaseModel
    {
        [RszOffsetWString]
        public string boneName = string.Empty;
        public long parentOffs;
        public long childOffs;
        public long nextSiblingOffs;
        public Vector4 translation;
        public Quaternion quaternion;
        public int Index;
        public uint boneHash;
        public long padding;

        public const int StructSize = 80;

        public override string ToString() => $"[{Index}] {boneName}";
    }


    // public class BoneHeaders : BaseModel
    // {
    //     public long boneHeaderOffset;
    //     public long boneHeaderCount;

    //     public List<BoneHeader> Headers { get; } = new();

    //     protected override bool DoRead(FileHandler handler)
    //     {
    //         handler.Read(ref boneHeaderOffset);
    //         handler.Read(ref boneHeaderCount);

    //         if (boneHeaderCount > 1000) throw new InvalidDataException($"boneHeaderCount {boneHeaderCount} > 1000");
    //         Headers.Clear();
    //         for (int i = 0; i < boneHeaderCount; i++)
    //         {
    //             BoneHeader boneHeader = new();
    //             boneHeader.Read(handler);
    //             Headers.Add(boneHeader);
    //         }
    //         return true;
    //     }

    //     protected override bool DoWrite(FileHandler handler)
    //     {
    //         boneHeaderCount = Headers.Count;
    //         handler.Write(ref boneHeaderOffset);
    //         handler.Write(ref boneHeaderCount);
    //         foreach (var header in Headers)
    //         {
    //             header.Write(handler);
    //         }
    //         handler.StringTableFlush();
    //         return true;
    //     }
    // }


    [Flags]
    public enum TrackFlag : ushort
    {
        None,
        Translation = 1,
        Rotation = 2,
        Scale = 4
    }

    public class BoneClipHeader : BaseModel
    {
        public ushort boneIndex;
        public TrackFlag trackFlags;
        public uint boneHash;  // MurMur3
        public float uknFloat;  // always 1.0?
        public uint padding;
        public long trackHeaderOffset; // keysPointer
        public string? boneName;

        public MotVersion MotVersion { get; set; }

        public BoneClipHeader(MotVersion motVersion)
        {
            MotVersion = motVersion;
        }

        protected override bool DoRead(FileHandler handler)
        {
            handler.Read(ref boneIndex);
            handler.Read(ref trackFlags);
            handler.Read(ref boneHash);
            if (MotVersion == MotVersion.RE2_DMC5)
            {
                handler.Read(ref uknFloat);
                handler.Read(ref padding);
                handler.Read(ref trackHeaderOffset);
            }
            else
            {
                if (MotVersion == MotVersion.RE7)
                    handler.Read(ref trackHeaderOffset);
                else
                    trackHeaderOffset = handler.ReadUInt();
            }
            return true;
        }

        protected override bool DoWrite(FileHandler handler)
        {
            if (!string.IsNullOrEmpty(boneName)) boneHash = MurMur3HashUtils.GetHash(boneName);
            if (MotVersion == MotVersion.RE2_DMC5)
            {
                handler.WriteRange(ref boneIndex, ref trackHeaderOffset);
            }
            else
            {
                handler.Write(ref boneIndex);
                handler.Write(ref trackFlags);
                handler.Write(ref boneHash);
                if (MotVersion == MotVersion.RE7)
                    handler.Write(ref trackHeaderOffset);
                else
                    handler.WriteUInt((uint)trackHeaderOffset);
            }
            return true;
        }

        public override string ToString() => boneName ?? $"Hash: [{boneHash}]";
    }


    public enum Vector3Decompression
    {
        UnknownType,
        LoadVector3sFull,
        LoadVector3s5BitA,
        LoadVector3s10BitA,
        LoadScalesXYZ,
        LoadVector3s21BitA,
        LoadVector3sXAxis,
        LoadVector3sYAxis,
        LoadVector3sZAxis,
        LoadVector3sXAxis16Bit,
        LoadVector3sYAxis16Bit,
        LoadVector3sZAxis16Bit,
        LoadVector3s5BitB,
        LoadVector3s10BitB,
        LoadVector3s21BitB,
        LoadVector3sXYZAxis16Bit,
        LoadVector3sXYZAxis,
    }


    public enum QuaternionDecompression
    {
        UnknownType,
        LoadQuaternionsFull,
        LoadQuaternions3Component,
        LoadQuaternions5Bit,
        LoadQuaternions8Bit,
        LoadQuaternions10Bit,
        LoadQuaternions13Bit,
        LoadQuaternions16Bit,
        LoadQuaternions18Bit,
        LoadQuaternions21Bit,
        LoadQuaternionsXAxis16Bit,
        LoadQuaternionsYAxis16Bit,
        LoadQuaternionsZAxis16Bit,
        LoadQuaternionsXAxis,
        LoadQuaternionsYAxis,
        LoadQuaternionsZAxis,
    }

    /// <summary>
    /// Packed 16-Bit Vector 3
    /// </summary>
    public struct PackedVector3
    {
        public ushort X;
        public ushort Y;
        public ushort Z;

        public PackedVector3() {}

        public PackedVector3(ushort x, ushort y, ushort z)
        {
            X = x;
            Y = y;
            Z = z;
        }
    }


    public class Track : BaseModel
    {
        public uint flags;
        public int keyCount;
        public uint frameRate;
        public float maxFrame;
        public long frameIndexOffset;
        public long frameDataOffset;
        public long unpackDataOffset;

        public int[]? frameIndexs;
        private readonly float[] unpackData = new float[8];

        /// <summary>
        /// NOTE: can be either translations or scales.
        /// </summary>
        public Vector3[]? translations;
        public Quaternion[]? rotations;

        private long offsetStart;

        public MotVersion MotVersion { get; set; }
        public TrackFlag TrackFlag { get; set; }

        public Track(MotVersion motVersion, TrackFlag trackFlag)
        {
            MotVersion = motVersion;
            TrackFlag = trackFlag;
        }

        public byte FrameIndexType
        {
            get => (byte)(flags >> 20);
            set => flags = (flags & 0xFFFFF) | (uint)(value << 20);
        }

        public uint Compression
        {
            get => flags & 0xFF000;
            set => flags = (flags & 0xFFF00FFF) | (value & 0xFF000);
        }

        public uint UnkFlag
        {
            get => flags & 0xFFF;
            set => flags = (flags & 0xFFFFF000) | (value & 0xFFF);
        }

        protected override bool DoRead(FileHandler handler)
        {
            handler.Read(ref flags);
            handler.Read(ref keyCount);
            if (MotVersion >= MotVersion.RE3)
            {
                frameIndexOffset = handler.ReadUInt();
                frameDataOffset = handler.ReadUInt();
                unpackDataOffset = handler.ReadUInt();
            }
            else
            {
                handler.Read(ref frameRate);
                handler.Read(ref maxFrame);
                handler.Read(ref frameIndexOffset);
                handler.Read(ref frameDataOffset);
                handler.Read(ref unpackDataOffset);
            }

            if (frameIndexOffset > 0)
            {
                using var frameIndexSeek = handler.SeekJumpBack(frameIndexOffset);
                frameIndexs = new int[keyCount];
                handler.Seek(frameIndexOffset);
                switch (FrameIndexType)
                {
                    case 2:
                        for (int i = 0; i < keyCount; i++) frameIndexs[i] = handler.ReadByte();
                        break;
                    case 4:
                        for (int i = 0; i < keyCount; i++) frameIndexs[i] = handler.ReadShort();
                        break;
                    case 5:
                        for (int i = 0; i < keyCount; i++) frameIndexs[i] = handler.ReadInt();
                        break;
                    default:
                        throw new InvalidDataException($"Unknown track compression type {FrameIndexType}");
                }
            }
            if (unpackDataOffset > 0)
            {
                using var unpackDataSeek = handler.SeekJumpBack(unpackDataOffset);
                handler.ReadArray(unpackData);
            }
            return true;
        }

        protected override bool DoWrite(FileHandler handler)
        {
            handler.Write(ref flags);
            handler.Write(ref keyCount);
            if (MotVersion >= MotVersion.RE3)
            {
                handler.WriteUInt((uint)frameIndexOffset);
                handler.WriteUInt((uint)frameDataOffset);
                handler.WriteUInt((uint)unpackDataOffset);
            }
            else
            {
                handler.Write(ref frameRate);
                handler.Write(ref maxFrame);
                offsetStart = handler.Tell();
                handler.Write(ref frameIndexOffset);
                handler.Write(ref frameDataOffset);
                handler.Write(ref unpackDataOffset);
            }

            if (frameIndexOffset > 0)
            {
                using var frameIndexSeek = handler.SeekJumpBack(frameIndexOffset);
                frameIndexs = new int[keyCount];
                handler.Seek(frameIndexOffset);
                switch (FrameIndexType)
                {
                    case 2:
                        for (int i = 0; i < keyCount; i++) frameIndexs[i] = handler.ReadByte();
                        break;
                    case 4:
                        for (int i = 0; i < keyCount; i++) frameIndexs[i] = handler.ReadShort();
                        break;
                    case 5:
                        for (int i = 0; i < keyCount; i++) frameIndexs[i] = handler.ReadInt();
                        break;
                    default:
                        throw new InvalidDataException($"Unknown track compression type {FrameIndexType}");
                }
            }
            if (unpackDataOffset > 0)
            {
                using var unpackDataSeek = handler.SeekJumpBack(unpackDataOffset);
                handler.ReadArray(unpackData);
            }
            return true;
        }

        public void WriteOffsetContents(FileHandler handler)
        {
            // FrameIndex
            if (frameIndexs == null) return;
            handler.WriteInt64(offsetStart, handler.Tell());
            switch (FrameIndexType)
            {
                case 2:
                    for (int i = 0; i < keyCount; i++) handler.WriteByte((byte)frameIndexs[i]);
                    break;
                case 4:
                    for (int i = 0; i < keyCount; i++) handler.WriteShort((short)frameIndexs[i]);
                    break;
                case 5:
                    for (int i = 0; i < keyCount; i++) handler.WriteInt(frameIndexs[i]);
                    break;
                default:
                    throw new InvalidDataException($"Unknown track compression type {FrameIndexType}");
            }

            // FrameData
            handler.WriteInt64(offsetStart + 8, handler.Tell());
            if (TrackFlag == TrackFlag.Translation || TrackFlag == TrackFlag.Scale)
            {
                WriteFrameDataTranslation(handler);
            }
            else if (TrackFlag == TrackFlag.Rotation)
            {
                WriteFrameDataRotation(handler);
            }

            // UnpackData
            if (unpackData != null)
            {
                handler.WriteInt64(offsetStart + 16, handler.Tell());
                handler.WriteArray(unpackData);
            }
        }

        public Vector3Decompression TranslationDecompression
        {
            get
            {
                if (MotVersion == MotVersion.RE2_DMC5 || MotVersion == MotVersion.RE7)
                {
                    // RE2 and RE7
                    return Compression switch
                    {
                        0x00000 => Vector3Decompression.LoadVector3sFull,
                        0x20000 => Vector3Decompression.LoadVector3s5BitA,
                        0x30000 => Vector3Decompression.LoadVector3s10BitA,
                        0x40000 => Vector3Decompression.LoadVector3s10BitA,
                        0x70000 => Vector3Decompression.LoadVector3s21BitA,
                        0x31000 => Vector3Decompression.LoadVector3sXAxis,
                        0x32000 => Vector3Decompression.LoadVector3sYAxis,
                        0x33000 => Vector3Decompression.LoadVector3sZAxis,
                        0x21000 => Vector3Decompression.LoadVector3sXAxis16Bit,
                        0x22000 => Vector3Decompression.LoadVector3sYAxis16Bit,
                        0x23000 => Vector3Decompression.LoadVector3sZAxis16Bit,
                        0x34000 => Vector3Decompression.LoadScalesXYZ,
                        0x24000 => Vector3Decompression.LoadVector3sXYZAxis16Bit, // dmc5 m06_200_pl0200_ev01.motlist.85
                        _ => Vector3Decompression.UnknownType
                    };
                }
                else
                {
                    // RE3+
                    return Compression switch
                    {
                        0x00000 => Vector3Decompression.LoadVector3sFull,
                        0x20000 => Vector3Decompression.LoadVector3s5BitB,
                        0x30000 => Vector3Decompression.LoadVector3s5BitB,
                        0x40000 => Vector3Decompression.LoadVector3s10BitB,
                        0x80000 => Vector3Decompression.LoadVector3s21BitB,
                        0x21000 => Vector3Decompression.LoadVector3sXAxis16Bit,
                        0x22000 => Vector3Decompression.LoadVector3sYAxis16Bit,
                        0x23000 => Vector3Decompression.LoadVector3sZAxis16Bit,
                        0x24000 => Vector3Decompression.LoadVector3sXYZAxis16Bit,
                        0x41000 => Vector3Decompression.LoadVector3sXAxis,
                        0x42000 => Vector3Decompression.LoadVector3sYAxis,
                        0x43000 => Vector3Decompression.LoadVector3sZAxis,
                        0x44000 => Vector3Decompression.LoadVector3sXYZAxis,
                        _ => Vector3Decompression.UnknownType
                    };
                }
            }
        }

        public QuaternionDecompression RotationDecompression
        {
            get
            {
                if (MotVersion == MotVersion.RE2_DMC5 || MotVersion == MotVersion.RE7)
                {
                    // RE2 and RE7
                    return Compression switch
                    {
                        0x00000 => QuaternionDecompression.LoadQuaternionsFull,
                        0xB0000 => QuaternionDecompression.LoadQuaternions3Component,
                        0xC0000 => QuaternionDecompression.LoadQuaternions3Component,
                        0x30000 => QuaternionDecompression.LoadQuaternions10Bit,
                        0x40000 => QuaternionDecompression.LoadQuaternions10Bit,
                        0x50000 => QuaternionDecompression.LoadQuaternions16Bit,
                        0x70000 => QuaternionDecompression.LoadQuaternions21Bit,
                        0x21000 => QuaternionDecompression.LoadQuaternionsXAxis16Bit,
                        0x22000 => QuaternionDecompression.LoadQuaternionsYAxis16Bit,
                        0x23000 => QuaternionDecompression.LoadQuaternionsZAxis16Bit,
                        0x31000 => QuaternionDecompression.LoadQuaternionsXAxis,
                        0x41000 => QuaternionDecompression.LoadQuaternionsXAxis,
                        0x32000 => QuaternionDecompression.LoadQuaternionsYAxis,
                        0x42000 => QuaternionDecompression.LoadQuaternionsYAxis,
                        0x33000 => QuaternionDecompression.LoadQuaternionsZAxis,
                        0x43000 => QuaternionDecompression.LoadQuaternionsZAxis,
                        _ => QuaternionDecompression.UnknownType
                    };
                }
                else
                {
                    // RE3+
                    return Compression switch
                    {
                        0x00000 => QuaternionDecompression.LoadQuaternionsFull,
                        0xB0000 => QuaternionDecompression.LoadQuaternions3Component,
                        0xC0000 => QuaternionDecompression.LoadQuaternions3Component,
                        0x20000 => QuaternionDecompression.LoadQuaternions5Bit,
                        0x30000 => QuaternionDecompression.LoadQuaternions8Bit,
                        0x40000 => QuaternionDecompression.LoadQuaternions10Bit,
                        0x50000 => QuaternionDecompression.LoadQuaternions13Bit,
                        0x60000 => QuaternionDecompression.LoadQuaternions16Bit,
                        0x70000 => QuaternionDecompression.LoadQuaternions18Bit,
                        0x80000 => QuaternionDecompression.LoadQuaternions21Bit,
                        0x21000 => QuaternionDecompression.LoadQuaternionsXAxis16Bit,
                        0x22000 => QuaternionDecompression.LoadQuaternionsYAxis16Bit,
                        0x23000 => QuaternionDecompression.LoadQuaternionsZAxis16Bit,
                        0x31000 => QuaternionDecompression.LoadQuaternionsXAxis,
                        0x41000 => QuaternionDecompression.LoadQuaternionsXAxis,
                        0x32000 => QuaternionDecompression.LoadQuaternionsYAxis,
                        0x42000 => QuaternionDecompression.LoadQuaternionsYAxis,
                        0x33000 => QuaternionDecompression.LoadQuaternionsZAxis,
                        0x43000 => QuaternionDecompression.LoadQuaternionsZAxis,
                        _ => QuaternionDecompression.UnknownType
                    };
                }
            }
        }

        public void ReadFrameDataTranslation(FileHandler handler)
        {
            using var defer = handler.SeekJumpBack(frameDataOffset);
            // Vector3Decompression type = TrackFlag == TrackFlag.Translation ? TranslationDecompression : ScaleDecompression;
            Vector3Decompression type = TranslationDecompression;
            translations = new Vector3[keyCount];
            for (int i = 0; i < keyCount; i++)
            {
                Vector3 translation = new();
                switch (type)
                {
                    case Vector3Decompression.LoadVector3sFull:
                        handler.Read(ref translation);
                        break;
                    case Vector3Decompression.LoadVector3s5BitA:
                        {
                            var data = handler.Read<ushort>();
                            translation.X = (unpackData[0] * ((data >> 00) & 0x1F) / 31.0f) + unpackData[4];
                            translation.Y = (unpackData[1] * ((data >> 05) & 0x1F) / 31.0f) + unpackData[5];
                            translation.Z = (unpackData[2] * ((data >> 10) & 0x1F) / 31.0f) + unpackData[6];
                            break;
                        }
                    case Vector3Decompression.LoadVector3s5BitB:
                        {
                            var data = handler.Read<ushort>();
                            translation.X = (unpackData[0] * ((data >> 00) & 0x1F) / 31.0f) + unpackData[3];
                            translation.Y = (unpackData[1] * ((data >> 05) & 0x1F) / 31.0f) + unpackData[4];
                            translation.Z = (unpackData[2] * ((data >> 10) & 0x1F) / 31.0f) + unpackData[5];
                            break;
                        }
                    case Vector3Decompression.LoadScalesXYZ:
                        translation.X = translation.Y = translation.Z = handler.Read<float>();
                        break;
                    case Vector3Decompression.LoadVector3s10BitA:
                        {
                            var data = handler.Read<uint>();
                            translation.X = unpackData[0] * (((data >> 00) & 0x3FF) * (1.0f / 0x3FF)) + unpackData[4];
                            translation.Y = unpackData[1] * (((data >> 10) & 0x3FF) * (1.0f / 0x3FF)) + unpackData[5];
                            translation.Z = unpackData[2] * (((data >> 20) & 0x3FF) * (1.0f / 0x3FF)) + unpackData[6];
                            break;
                        }
                    case Vector3Decompression.LoadVector3s10BitB:
                        {
                            var data = handler.Read<uint>();
                            translation.X = unpackData[0] * (((data >> 00) & 0x3FF) * (1.0f / 0x3FF)) + unpackData[3];
                            translation.Y = unpackData[1] * (((data >> 10) & 0x3FF) * (1.0f / 0x3FF)) + unpackData[4];
                            translation.Z = unpackData[2] * (((data >> 20) & 0x3FF) * (1.0f / 0x3FF)) + unpackData[5];
                            break;
                        }
                    case Vector3Decompression.LoadVector3s21BitA:
                        {
                            var data = handler.Read<ulong>();
                            translation.X = (unpackData[0] * ((data >> 00) & 0x1FFFFF) / 2097151.0f) + unpackData[4];
                            translation.Y = (unpackData[1] * ((data >> 21) & 0x1FFFFF) / 2097151.0f) + unpackData[5];
                            translation.Z = (unpackData[2] * ((data >> 42) & 0x1FFFFF) / 2097151.0f) + unpackData[6];
                            break;
                        }
                    case Vector3Decompression.LoadVector3s21BitB:
                        {
                            var data = handler.Read<ulong>();
                            translation.X = (unpackData[0] * ((data >> 00) & 0x1FFFFF) / 2097151.0f) + unpackData[3];
                            translation.Y = (unpackData[1] * ((data >> 21) & 0x1FFFFF) / 2097151.0f) + unpackData[4];
                            translation.Z = (unpackData[2] * ((data >> 42) & 0x1FFFFF) / 2097151.0f) + unpackData[5];
                            break;
                        }
                    case Vector3Decompression.LoadVector3sXAxis:
                        {
                            translation.X = handler.Read<float>();
                            translation.Y = unpackData[1];
                            translation.Z = unpackData[2];
                            break;
                        }
                    case Vector3Decompression.LoadVector3sYAxis:
                        {
                            translation.X = unpackData[0];
                            translation.Y = handler.Read<float>();
                            translation.Z = unpackData[2];
                            break;
                        }
                    case Vector3Decompression.LoadVector3sZAxis:
                        {
                            translation.X = unpackData[0];
                            translation.Y = unpackData[1];
                            translation.Z = handler.Read<float>();
                            break;
                        }
                    case Vector3Decompression.LoadVector3sXAxis16Bit:
                        {
                            var data = handler.Read<ushort>();
                            translation.X = unpackData[0] * (data / 65535.0f) + unpackData[1];
                            translation.Y = unpackData[2];
                            translation.Z = unpackData[3];
                            break;
                        }
                    case Vector3Decompression.LoadVector3sYAxis16Bit:
                        {
                            var data = handler.Read<ushort>();
                            translation.X = unpackData[1];
                            translation.Y = unpackData[0] * (data / 65535.0f) + unpackData[2];
                            translation.Z = unpackData[3];
                            break;
                        }
                    case Vector3Decompression.LoadVector3sZAxis16Bit:
                        {
                            var data = handler.Read<ushort>();
                            translation.X = unpackData[1];
                            translation.Y = unpackData[2];
                            translation.Z = unpackData[0] * (data / 65535.0f) + unpackData[3];
                            break;
                        }
                    case Vector3Decompression.LoadVector3sXYZAxis16Bit:
                        {
                            var data = handler.Read<ushort>();
                            translation.X = translation.Y = translation.Z = unpackData[0] * (data / 65535.0f) + unpackData[3];
                            break;
                        }
                    case Vector3Decompression.LoadVector3sXYZAxis:
                        translation.X = translation.Y = translation.Z = handler.Read<float>();
                        break;
                    default:
                        throw new InvalidOperationException($"Invalid type {type}");
                }
                translations[i] = translation;
            }
        }

        public void WriteFrameDataTranslation(FileHandler handler)
        {
            if (translations == null) throw new NullReferenceException($"{nameof(translations)} is null");
            Vector3Decompression type = TranslationDecompression;
            for (int i = 0; i < keyCount; i++)
            {
                Vector3 translation = translations[i];
                switch (type)
                {
                    case Vector3Decompression.LoadVector3sFull:
                        handler.Write(translation);
                        break;
                    case Vector3Decompression.LoadVector3s5BitA:
                        {
                            ushort data = (ushort)(
                                ((ushort)((translation.X - unpackData[4]) / unpackData[0] * 31.0f) << 00) |
                                ((ushort)((translation.Y - unpackData[5]) / unpackData[1] * 31.0f) << 05) |
                                ((ushort)((translation.Z - unpackData[6]) / unpackData[2] * 31.0f) << 10));
                            handler.Write(data);
                            break;
                        }
                    case Vector3Decompression.LoadVector3s5BitB:
                        {
                            ushort data = (ushort)(
                                ((ushort)((translation.X - unpackData[3]) / unpackData[0] * 31.0f) << 00) |
                                ((ushort)((translation.Y - unpackData[4]) / unpackData[1] * 31.0f) << 05) |
                                ((ushort)((translation.Z - unpackData[5]) / unpackData[2] * 31.0f) << 10));
                            handler.Write(data);
                            break;
                        }
                    case Vector3Decompression.LoadScalesXYZ:
                        handler.Write(translation.X);
                        break;
                    case Vector3Decompression.LoadVector3s10BitA:
                        {
                            uint data =
                                ((uint)((translation.X - unpackData[4]) / unpackData[0] * 0x3FF) << 00) |
                                ((uint)((translation.Y - unpackData[5]) / unpackData[1] * 0x3FF) << 10) |
                                ((uint)((translation.Z - unpackData[6]) / unpackData[2] * 0x3FF) << 20);
                            handler.Write(data);
                            break;
                        }
                    case Vector3Decompression.LoadVector3s10BitB:
                        {
                            uint data = (
                                ((uint)((translation.X - unpackData[3]) / unpackData[0] * 0x3FF) << 00) |
                                ((uint)((translation.Y - unpackData[4]) / unpackData[1] * 0x3FF) << 10) |
                                ((uint)((translation.Z - unpackData[5]) / unpackData[2] * 0x3FF) << 20)
                            );
                            handler.Write(data);
                            break;
                        }
                    case Vector3Decompression.LoadVector3s21BitA:
                        {
                            ulong data =
                                ((ulong)((translation.X - unpackData[4]) / unpackData[0] * 2097151.0f) << 00) |
                                ((ulong)((translation.Y - unpackData[5]) / unpackData[1] * 2097151.0f) << 21) |
                                ((ulong)((translation.Z - unpackData[6]) / unpackData[2] * 2097151.0f) << 42);
                            handler.Write(data);
                            break;
                        }
                    case Vector3Decompression.LoadVector3s21BitB:
                        {
                            var data =
                                ((int)((translation.X - unpackData[3]) / unpackData[0]) & 0x1FFFFF) << 00 |
                                ((int)((translation.Y - unpackData[4]) / unpackData[1]) & 0x1FFFFF) << 21 |
                                ((int)((translation.Z - unpackData[5]) / unpackData[2]) & 0x1FFFFF) << 42;
                            handler.Write((ulong)data);
                            break;
                        }
                    case Vector3Decompression.LoadVector3sXAxis:
                        {
                            handler.Write(translation.X);
                            break;
                        }
                    case Vector3Decompression.LoadVector3sYAxis:
                        {
                            handler.Write(translation.Y);
                            break;
                        }
                    case Vector3Decompression.LoadVector3sZAxis:
                        {
                            handler.Write(translation.Z);
                            break;
                        }
                    case Vector3Decompression.LoadVector3sXAxis16Bit:
                        {
                            var data = (ushort)((translation.X - unpackData[1]) / unpackData[0] * 65535);
                            handler.Write(data);
                            break;
                        }
                    case Vector3Decompression.LoadVector3sYAxis16Bit:
                        {
                            var data = (ushort)((translation.Y - unpackData[2]) / unpackData[0] * 65535);
                            handler.Write(data);
                            break;
                        }
                    case Vector3Decompression.LoadVector3sZAxis16Bit:
                        {
                            var data = (ushort)((translation.Z - unpackData[3]) / unpackData[0] * 65535);
                            handler.Write(data);
                            break;
                        }
                    case Vector3Decompression.LoadVector3sXYZAxis16Bit:
                        {
                            var data = (ushort)((translation.X - unpackData[3]) / unpackData[0] * 65535);
                            handler.Write(data);
                            break;
                        }
                    case Vector3Decompression.LoadVector3sXYZAxis:
                        {
                            handler.Write(translation.X);
                            break;
                        }
                    default:
                        throw new InvalidOperationException($"Invalid type {type}");
                }
            }
        }

        public void ReadFrameDataRotation(FileHandler handler)
        {
            using var defer = handler.SeekJumpBack(frameDataOffset);
            QuaternionDecompression type = RotationDecompression;
            rotations = new Quaternion[keyCount];
            for (int i = 0; i < keyCount; i++)
            {
                Quaternion quaternion = new();
                switch (type)
                {
                    case QuaternionDecompression.LoadQuaternionsFull:
                        handler.Read(ref quaternion);
                        break;
                    case QuaternionDecompression.LoadQuaternions3Component:
                        {
                            Vector3 vector = handler.Read<Vector3>();
                            quaternion = new(vector, 0.0f);
                            break;
                        }
                    case QuaternionDecompression.LoadQuaternions5Bit:
                        {
                            var data = handler.Read<ushort>();
                            quaternion.X = (unpackData[0] * ((data >> 00) & 0x1F) * (1.0f / 0x1F)) + unpackData[4];
                            quaternion.Y = (unpackData[1] * ((data >> 05) & 0x1F) * (1.0f / 0x1F)) + unpackData[5];
                            quaternion.Z = (unpackData[2] * ((data >> 10) & 0x1F) * (1.0f / 0x1F)) + unpackData[6];
                            break;
                        }
                    case QuaternionDecompression.LoadQuaternions8Bit:
                        {
                            quaternion.X = (unpackData[0] * (handler.ReadByte() * 0.000015259022f)) + unpackData[4];
                            quaternion.Y = (unpackData[1] * (handler.ReadByte() * 0.000015259022f)) + unpackData[5];
                            quaternion.Z = (unpackData[2] * (handler.ReadByte() * 0.000015259022f)) + unpackData[6];
                            break;
                        }
                    case QuaternionDecompression.LoadQuaternions10Bit:
                        {
                            var data = handler.Read<uint>();
                            quaternion.X = (unpackData[0] * ((data >> 00) & 0x3FF) / 1023.0f) + unpackData[4];
                            quaternion.Y = (unpackData[1] * ((data >> 10) & 0x3FF) / 1023.0f) + unpackData[5];
                            quaternion.Z = (unpackData[2] * ((data >> 20) & 0x3FF) / 1023.0f) + unpackData[6];
                            break;
                        }
                    case QuaternionDecompression.LoadQuaternions13Bit:
                        {
                            var data = handler.ReadBytes(5);
                            ulong val = 0;
                            for (int j = 0; j < 5; j++)
                            {
                                val = data[j] | (val << 8);
                            }
                            quaternion.X = (unpackData[0] * ((val >> 00) & 0x1FFF) * 0.00012208521f) + unpackData[4];
                            quaternion.Y = (unpackData[1] * ((val >> 13) & 0x1FFF) * 0.00012208521f) + unpackData[5];
                            quaternion.Z = (unpackData[2] * ((val >> 26) & 0x1FFF) * 0.00012208521f) + unpackData[6];
                            break;
                        }
                    case QuaternionDecompression.LoadQuaternions16Bit:
                        {
                            var data = handler.Read<PackedVector3>();
                            quaternion.X = (unpackData[0] * (data.X / 65535.0f)) + unpackData[4];
                            quaternion.Y = (unpackData[1] * (data.Y / 65535.0f)) + unpackData[5];
                            quaternion.Z = (unpackData[2] * (data.Z / 65535.0f)) + unpackData[6];
                            break;
                        }
                    case QuaternionDecompression.LoadQuaternions18Bit:
                        {
                            var data = handler.ReadBytes(7);
                            ulong val = 0;
                            for (int j = 0; j < 7; j++)
                            {
                                val = data[j] | (val << 8);
                            }
                            quaternion.X = (unpackData[0] * ((val >> 00) & 0x1FFF) * 0.00012208521f) + unpackData[4];
                            quaternion.Y = (unpackData[1] * ((val >> 13) & 0x1FFF) * 0.00012208521f) + unpackData[5];
                            quaternion.Z = (unpackData[2] * ((val >> 26) & 0x1FFF) * 0.00012208521f) + unpackData[6];
                            break;
                        }
                    case QuaternionDecompression.LoadQuaternions21Bit:
                        {
                            var data = handler.Read<ulong>();
                            quaternion.X = (unpackData[0] * ((data >> 00) & 0x1FFFFF) / 2097151.0f) + unpackData[4];
                            quaternion.Y = (unpackData[1] * ((data >> 21) & 0x1FFFFF) / 2097151.0f) + unpackData[5];
                            quaternion.Z = (unpackData[2] * ((data >> 42) & 0x1FFFFF) / 2097151.0f) + unpackData[6];
                            break;
                        }
                    case QuaternionDecompression.LoadQuaternionsXAxis16Bit:
                        quaternion.X = unpackData[0] * (handler.Read<float>() / 65535.0f) + unpackData[1];
                        quaternion.Y = 0.0f;
                        quaternion.Z = 0.0f;
                        break;
                    case QuaternionDecompression.LoadQuaternionsYAxis16Bit:
                        quaternion.X = 0.0f;
                        quaternion.Y = unpackData[0] * (handler.Read<float>() / 65535.0f) + unpackData[1];
                        quaternion.Z = 0.0f;
                        break;
                    case QuaternionDecompression.LoadQuaternionsZAxis16Bit:
                        quaternion.X = 0.0f;
                        quaternion.Y = 0.0f;
                        quaternion.Z = unpackData[0] * (handler.Read<float>() / 65535.0f) + unpackData[1];
                        break;
                    case QuaternionDecompression.LoadQuaternionsXAxis:
                        quaternion.X = handler.Read<float>();
                        quaternion.Y = 0.0f;
                        quaternion.Z = 0.0f;
                        break;
                    case QuaternionDecompression.LoadQuaternionsYAxis:
                        quaternion.X = 0.0f;
                        quaternion.Y = handler.Read<float>();
                        quaternion.Z = 0.0f;
                        break;
                    case QuaternionDecompression.LoadQuaternionsZAxis:
                        quaternion.X = 0.0f;
                        quaternion.Y = 0.0f;
                        quaternion.Z = handler.Read<float>();
                        break;
                    default:
                        throw new InvalidOperationException($"Invalid type {type}");
                }
                if (type != QuaternionDecompression.LoadQuaternionsFull)
                {
                    SetRotationW(ref quaternion);
                }
                rotations[i] = quaternion;
            }
        }

        public void WriteFrameDataRotation(FileHandler handler)
        {
            if (rotations == null) throw new NullReferenceException($"{nameof(rotations)} is null");
            QuaternionDecompression type = RotationDecompression;
            for (int i = 0; i < keyCount; i++)
            {
                Quaternion quaternion = rotations[i];
                switch (type)
                {
                    case QuaternionDecompression.LoadQuaternionsFull:
                        handler.Write(quaternion);
                        break;
                    case QuaternionDecompression.LoadQuaternions3Component:
                        {
                            Vector3 vector = new(quaternion.X, quaternion.Y, quaternion.Z);
                            handler.Write(vector);
                            break;
                        }
                    case QuaternionDecompression.LoadQuaternions5Bit:
                        {
                            ushort data = (ushort)(
                                (ushort)((quaternion.X - unpackData[4]) / unpackData[0] * 0x1F) << 00 |
                                (ushort)((quaternion.Y - unpackData[5]) / unpackData[1] * 0x1F) << 05 |
                                (ushort)((quaternion.Z - unpackData[6]) / unpackData[2] * 0x1F) << 10);
                            handler.Write(data);
                            break;
                        }
                    case QuaternionDecompression.LoadQuaternions8Bit:
                        {
                            byte x = (byte)((quaternion.X - unpackData[4]) / unpackData[0] * (1 / 0.000015259022f));
                            byte y = (byte)((quaternion.Y - unpackData[5]) / unpackData[1] * (1 / 0.000015259022f));
                            byte z = (byte)((quaternion.Z - unpackData[6]) / unpackData[2] * (1 / 0.000015259022f));
                            handler.Write(x);
                            handler.Write(y);
                            handler.Write(z);
                            break;
                        }
                    case QuaternionDecompression.LoadQuaternions10Bit:
                        {
                            uint data =
                                (uint)((quaternion.X - unpackData[4]) / unpackData[0] * 1023.0f) << 00 |
                                (uint)((quaternion.Y - unpackData[5]) / unpackData[1] * 1023.0f) << 10 |
                                (uint)((quaternion.Z - unpackData[6]) / unpackData[2] * 1023.0f) << 20;
                            handler.Write(data);
                            break;
                        }
                    case QuaternionDecompression.LoadQuaternions13Bit:
                        {
                            ulong val =
                                (ulong)((quaternion.X - unpackData[4]) / unpackData[0] * (1 / 0.00012208521f)) << 00 |
                                (ulong)((quaternion.Y - unpackData[5]) / unpackData[1] * (1 / 0.00012208521f)) << 13 |
                                (ulong)((quaternion.Z - unpackData[6]) / unpackData[2] * (1 / 0.00012208521f)) << 26;
                            byte[] data = new byte[5];
                            for (int j = 0; j < 5; j++)
                            {
                                data[j] = (byte)(val & 0xFF);
                                val >>= 8;
                            }
                            handler.WriteBytes(data);
                            break;
                        }
                    case QuaternionDecompression.LoadQuaternions16Bit:
                        {
                            var data = new PackedVector3(
                                (ushort)((quaternion.X - unpackData[4]) / unpackData[0] * 65535.0f),
                                (ushort)((quaternion.Y - unpackData[5]) / unpackData[1] * 65535.0f),
                                (ushort)((quaternion.Z - unpackData[6]) / unpackData[2] * 65535.0f)
                            );
                            handler.Write(data);
                            break;
                        }
                    case QuaternionDecompression.LoadQuaternions18Bit:
                        {
                            ulong val = (((ulong)((quaternion.X - unpackData[4]) / unpackData[0] * 8191.0f) & 0x1FFF) << 00) |
                                        (((ulong)((quaternion.Y - unpackData[5]) / unpackData[1] * 8191.0f) & 0x1FFF) << 13) |
                                        (((ulong)((quaternion.Z - unpackData[6]) / unpackData[2] * 8191.0f) & 0x1FFF) << 26);
                            byte[] data = new byte[7];
                            for (int j = 0; j < 7; j++)
                            {
                                data[j] = (byte)((val >> (8 * j)) & 0xFF);
                            }
                            handler.WriteBytes(data);
                            break;
                        }
                    case QuaternionDecompression.LoadQuaternions21Bit:
                        {
                            ulong val = (((ulong)((quaternion.X - unpackData[4]) / unpackData[0] * 1048575.0f) & 0x1FFFFF) << 00) |
                                        (((ulong)((quaternion.Y - unpackData[5]) / unpackData[1] * 1048575.0f) & 0x1FFFFF) << 21) |
                                        (((ulong)((quaternion.Z - unpackData[6]) / unpackData[2] * 1048575.0f) & 0x1FFFFF) << 42);
                            handler.Write(val);
                            break;
                        }
                    case QuaternionDecompression.LoadQuaternionsXAxis16Bit:
                        {
                            float data = (quaternion.X - unpackData[1]) / unpackData[0] * 65535.0f;
                            handler.Write(data);
                            break;
                        }
                    case QuaternionDecompression.LoadQuaternionsYAxis16Bit:
                        {
                            float data = (quaternion.Y - unpackData[1]) / unpackData[0] * 65535.0f;
                            handler.Write(data);
                            break;
                        }
                    case QuaternionDecompression.LoadQuaternionsZAxis16Bit:
                        {
                            float data = (quaternion.Z - unpackData[1]) / unpackData[0] * 65535.0f;
                            handler.Write(data);
                            break;
                        }
                    case QuaternionDecompression.LoadQuaternionsXAxis:
                        handler.Write(quaternion.X);
                        break;
                    case QuaternionDecompression.LoadQuaternionsYAxis:
                        handler.Write(quaternion.Y);
                        break;
                    case QuaternionDecompression.LoadQuaternionsZAxis:
                        handler.Write(quaternion.Z);
                        break;
                    default:
                        throw new InvalidOperationException($"Invalid type {type}");
                }
            }
        }

        public static void SetRotationW(ref Quaternion quaternion)
        {
            float w = 1.0f - (quaternion.X * quaternion.X + quaternion.Y * quaternion.Y + quaternion.Z * quaternion.Z);
            quaternion.W = w > 0.0f ? (float)Math.Sqrt(w) : 0.0f;
        }
    }


    public class BoneMotionClip : BaseModel
    {
        public BoneClipHeader ClipHeader { get; }

        public BoneMotionClip(BoneClipHeader clipHeader)
        {
            ClipHeader = clipHeader;
        }

        public Track? Translation { get; set; }
        public Track? Rotation { get; set; }
        public Track? Scale { get; set; }

        protected override bool DoRead(FileHandler handler)
        {
            var header = ClipHeader;
            handler.Seek(header.trackHeaderOffset);
            if (header.trackFlags.HasFlag(TrackFlag.Translation))
            {
                Translation = new(header.MotVersion, TrackFlag.Translation);
                Translation.Read(handler);
            }
            if (header.trackFlags.HasFlag(TrackFlag.Rotation))
            {
                Rotation = new(header.MotVersion, TrackFlag.Rotation);
                Rotation.Read(handler);
            }
            if (header.trackFlags.HasFlag(TrackFlag.Scale))
            {
                Scale = new(header.MotVersion, TrackFlag.Scale);
                Scale.Read(handler);
            }
            Translation?.ReadFrameDataTranslation(handler);
            Rotation?.ReadFrameDataRotation(handler);
            Scale?.ReadFrameDataTranslation(handler);
            return true;
        }

        protected override bool DoWrite(FileHandler handler)
        {
            ClipHeader.trackHeaderOffset = handler.Tell();
            Translation?.Write(handler);
            Rotation?.Write(handler);
            Scale?.Write(handler);
            return true;
        }

        public void WriteOffsetContents(FileHandler handler)
        {
            Translation?.WriteOffsetContents(handler);
            Rotation?.WriteOffsetContents(handler);
            Scale?.WriteOffsetContents(handler);
        }

        public override string ToString() => $"{ClipHeader} [{(Translation == null ? "": "T")}{(Rotation == null ? "": "R")}{(Scale == null ? "": "S")}]";
    }


    public class MotClip : BaseModel
    {
        public long clipOffset;
        public long endClipStructsRelocation;
        public uint uknIntA;
        public uint uknIntB;
        public byte[] uknBytes1C = new byte[0x1C];
        public ClipEntry ClipEntry { get; set; } = new();
        public EndClipStruct[]? EndClipStructs { get; set; }

        protected override bool DoRead(FileHandler handler)
        {
            handler.Skip(8);
            handler.Read(ref clipOffset);
            handler.Read(ref endClipStructsRelocation);
            handler.Skip(4);
            handler.Read(ref uknIntA);
            handler.Read(ref uknIntB);
            handler.ReadBytes(uknBytes1C);
            ClipEntry.Read(handler);
            if (ClipEntry.Header.version > ClipVersion.RE7 && ClipEntry.Header.numNodes > 1)
            {
                handler.Seek(endClipStructsRelocation);
                EndClipStructs = new EndClipStruct[ClipEntry.Header.numNodes - 1];
                for (int i = 0; i < ClipEntry.Header.numNodes - 1; ++i)
                {
                    EndClipStructs[i] = new EndClipStruct() { Version = ClipEntry.Header.version };
                    EndClipStructs[i].Read(handler);
                }
            }

            return true;
        }

        protected override bool DoWrite(FileHandler handler)
        {
            handler.Skip(8);
            handler.Write(ref clipOffset);
            handler.Write(ref endClipStructsRelocation);
            handler.Skip(4);
            handler.Write(ref uknIntA);
            handler.Write(ref uknIntB);
            handler.WriteBytes(uknBytes1C);

            clipOffset = handler.Tell();
            ClipEntry.Write(handler);

            if (ClipEntry.Header.version > ClipVersion.RE7 && ClipEntry.Header.numNodes > 1)
            {
                endClipStructsRelocation = handler.Tell();
                for (int i = 0; i < EndClipStructs!.Length; ++i)
                {
                    EndClipStructs[i].Write(handler);
                }
            }
            handler.Write(Start + 8, clipOffset);
            handler.Write(Start + 16, endClipStructsRelocation);
            return true;
        }

        public override string ToString() => $"MotClip: {ClipEntry}";
    }


    public class MotEndClipHeader(MotVersion version) : BaseModel
    {
        public ulong unknown;
        public long dataSize;
        public int objectCount;
        public int endHashCount;
        public byte count1;
        public byte type1;
        public byte endFlags1;
        public byte endFlags2;

        public float[]? frameValues1;
        public float[]? frameValues2;

        public uint[] EndHashes = [];

        public struct ExtraDataSubstruct1
        {
            public int ukn1;
            public int ukn2;
            public int hash;
            public int ukn4;
            public int ukn5;
            public int hash3;
            public int ukn7;
        }

        public MotVersion Version { get; set; } = version;

        protected override bool DoRead(FileHandler handler)
        {
            if (Version > MotVersion.MHR_DEMO)
            {
                handler.Read(ref count1);
                handler.Read(ref type1);
                endFlags1 = handler.Read<byte>();
                endFlags2 = handler.Read<byte>();

                // 4 2 0 count => 8 or 9 (ch20_000_comem.motlist.751 mot 300)
                // 1 5 0 count => 6 (ch53_000_low_com.motlist.751)
                // 1 7 0 count => 8 or 9 (ch53_000_com.motlist.751 mot 206)
                // 2 2 1 count => 5 (ch51_000_combm.motlist.751 mot 181)
                // 1 2 1 count => 3 (ch51_000_com_catch.motlist.751 mot 1)
                // 2 2 0 1 => 5 (re3rt base_cmn_move.motlist.524 mot 170)
                // 2 2 1 1 => 5 (re3rt em9400_attack.motlist.524 mot 1)
                // 3 4 2 0 => 13 (re8 ch01_6000_handsrose.motlist.486 mot 2)

                // note: this may not be fully correct, but the values are consistently correct with this logic, for DD2, and at least not failing for the other games
                if (type1 == 2 || type1 == 4) {
                    frameValues1 = handler.ReadArray<float>(count1 * type1);
                } else {
                    frameValues1 = handler.ReadArray<float>(count1 + type1);
                }
                if (endFlags1 > 0 || endFlags2 > 0) {
                    frameValues2 = handler.ReadArray<float>(1);
                }
            }
            else
            {
                handler.Read(ref unknown);
                handler.Read(ref dataSize);
                handler.Read(ref objectCount);
                handler.Read(ref endHashCount);
                var skip = handler.Read<long>();
                if (skip != 0)
                {
                    throw new DataInterpretationException("Not padding!");
                }
                var dataSize1 = handler.Read<long>(); // size from startof(dataSize) to startof(hashStruct[6] thing); in other words, sizeof(structs1) + 32
                var dataSize2 = handler.Read<long>(); // above + 32 (== unknown?)
                // TODO

                // hash structs (seem to mark "end of data" offsets):
                // 1925441204 => end of this godforsaken struct
                // 1904925210 => end of frame values
                // 535309294 => end of frame indexes
                // 3914920078 => end of frame indexes again (maybe some extra data set that isn't present in the file i'm looking at rn)
                // 3591947802 => -||-
                // 327324368 => end of clip list (also start of this struct)
                if (objectCount > 1)
                {
                    Log.Warn("Found multi object: " + handler.FilePath);
                }
                handler.Seek(dataSize);
                EndHashes = handler.ReadArray<uint>(endHashCount * 2);
            }
            return true;
        }

        protected override bool DoWrite(FileHandler handler)
        {
            handler.Write(ref unknown);
            if (Version > MotVersion.MHR_DEMO)
            {
            }
            else
            {
                throw new NotImplementedException();
            }
            return true;
        }
    }

    public class MotEndClip(MotEndClipHeader header) : BaseModel
    {
        public MotEndClipHeader Header { get; } = header;

        public FrameValue[]? FrameValues;

        protected override bool DoRead(FileHandler handler)
        {
            var count = handler.Read<int>();
            FrameValues = handler.ReadArray<FrameValue>(count);
            return true;
        }

        protected override bool DoWrite(FileHandler handler)
        {
            FrameValues ??= [];
            handler.Write<int>(FrameValues.Length);
            handler.WriteArray<FrameValue>(FrameValues);
            return true;
        }
    }
}


namespace ReeLib
{
    public abstract class MotFileBase(FileHandler fileHandler)
        : BaseFile(fileHandler)
    {
    }

    public class MotFile(FileHandler fileHandler)
        : MotFileBase(fileHandler)
    {
        public const uint Magic = 0x20746F6D;
        public const string Extension = ".mot";

        public MotHeader Header { get; } = new();
        public List<BoneHeader>? BoneHeaders { get; set; }
        public List<BoneMotionClip> BoneClips { get; } = new();
        public List<MotClip> Clips { get; } = new();
        public List<MotEndClip> EndClips { get; } = new();

        public List<MotBone> Bones { get; } = new();
        public List<MotBone> RootBones { get; } = new();

        public MotBone? GetBoneByHash(uint hash) => Bones.FirstOrDefault(b => b.Header.boneHash == hash);

        private bool IsMotlist => PathUtils.GetFilenameExtensionWithoutSuffixes(FileHandler.FilePath ?? string.Empty).SequenceEqual("motlist") == true;

        protected override bool DoRead()
        {
            Bones.Clear();
            Clips.Clear();
            var handler = FileHandler;
            var header = Header;
            if (!header.Read(handler)) return false;
            if (header.magic != Magic)
            {
                throw new InvalidDataException($"{handler.FilePath} Not a MOT file");
            }

            if (header.boneClipCount > header.boneCount)
                throw new InvalidDataException($"boneClipCount {header.boneClipCount} > boneCount {header.boneCount}");

            if (header.boneClipCount > 0)
            {
                handler.Seek(header.boneClipHeaderOffset);
                BoneClipHeader[] boneClipHeaders = new BoneClipHeader[header.boneClipCount];
                for (int i = 0; i < header.boneClipCount; i++)
                {
                    BoneClipHeader clipHeader = new(header.version);
                    clipHeader.Read(handler);
                    boneClipHeaders[i] = clipHeader;
                }

                foreach (var clipHeader in boneClipHeaders)
                {
                    BoneMotionClip clip = new(clipHeader);
                    clip.Read(handler);
                    // bones[clipHeader.boneIndex].Tracks = tracks;
                    BoneClips.Add(clip);
                }
            }

            if (!IsMotlist)
            {
                ReadBones(null);
            }

            if (header.jmapOffset > 0)
            {
                header.jointMapPath = handler.ReadWString(header.jmapOffset);
            }

            if (header.clipFileOffset > 0 && header.clipCount > 0)
            {
                handler.Seek(header.clipFileOffset);
                long[] clipOffset = handler.ReadArray<long>(header.clipCount);
                for (int i = 0; i < header.clipCount; i++)
                {
                    handler.Seek(clipOffset[i]);
                    MotClip motClip = new();
                    motClip.Read(handler);
                    Clips.Add(motClip);
                }
                if (header.motEndClipDataOffset > 0)
                {
                    handler.Seek(header.motEndClipDataOffset);

                    if (header.version > MotVersion.MHR_DEMO)
                    {
                        ReadMotEndClipsModern();
                    }
                    else
                    {
                        ReadMotEndClipsLegacy();
                    }
                }
            }
            return true;
        }

        private void ReadMotEndClipsLegacy()
        {

        }
        private void ReadMotEndClipsModern()
        {
            var handler = FileHandler;
            if (Header.motEndClipCount >= 3) throw new DataInterpretationException("Not necessarily an error - I just need this example file to verify correctness");

            var endClipHeaders = new MotEndClipHeader[Header.motEndClipCount];
            var headerOffsets = handler.ReadArray<long>(Header.motEndClipCount);
            for (int i = 0; i < Header.motEndClipCount; ++i)
            {
                handler.Seek(headerOffsets[i]);
                var motClipHeader = new MotEndClipHeader(Header.version);
                motClipHeader.Read(handler);
                endClipHeaders[i] = motClipHeader;
            }
            handler.Align(8);

            var frameDataOffsets = handler.ReadArray<long>(Header.motEndClipCount);
            for (int i = 0; i < Header.motEndClipCount; ++i)
            {
                handler.Seek(frameDataOffsets[i]);
                var endClip = new MotEndClip(endClipHeaders[i]);
                endClip.Read(handler);
                EndClips.Add(endClip);
            }
        }

        public void ReadBones(MotFile? mainSourceMot)
        {
            var header = Header;
            var handler = FileHandler;
            var hasOwnBoneList = header.motSize == 0 || header.boneHeaderOffsetStart > 0 && header.boneHeaderOffsetStart < header.motSize;
            if (!hasOwnBoneList)
            {
                if (mainSourceMot?.BoneHeaders == null)
                {
                    throw new InvalidDataException("Mot file bones not found");
                }

                BoneHeaders = mainSourceMot.BoneHeaders;
                Bones.AddRange(mainSourceMot.Bones);
                RootBones.AddRange(mainSourceMot.RootBones);
            }
            else
            {
                BoneHeaders = new(header.boneCount);
                handler.Seek(header.boneHeaderOffsetStart);
                var boneHeaderOffset = handler.Read<long>();
                var boneHeaderCount = handler.Read<long>();
                for (int i = 0; i < boneHeaderCount; i++)
                {
                    BoneHeader boneHeader = new();
                    boneHeader.Read(handler);
                    BoneHeaders.Add(boneHeader);
                    Bones.Add(new MotBone(boneHeader));
                }
                foreach (var boneHeader in BoneHeaders)
                {
                    // TODO how does this behave with motlists with varying bone lists?
                    var bone = GetBoneByHash(boneHeader.boneHash);
                    if (bone == null)
                    {
                        // there's no matches sometimes, for motlists with varying bone header lists.
                        // unsure if these are actually valid and functional (and ignored), or just obsolete unused mots
                        // or if there's some setting that makes it use the original mesh bones instead of motlist provided ones
                        continue;
                    }

                    if (boneHeader.parentOffs == 0) {
                        RootBones.Add(bone);
                    } else {
                        var parentBoneIndex = (int)((boneHeader.parentOffs - boneHeaderOffset) / BoneHeader.StructSize);
                        var parentBone = GetBoneByHash(BoneHeaders[parentBoneIndex].boneHash);
                        if (parentBone == null)
                        {
                            Log.Warn("Parent bone not found");
                            continue;
                        }

                        bone.Parent = parentBone;
                        parentBone.Children.Add(bone);
                    }
                }
            }

            foreach (var bone in BoneHeaders)
            {
                // could be sped up with a dictionary - but then we need to maintain both the list and dict, maybe later
                var clip = BoneClips.FirstOrDefault(c => c.ClipHeader.boneHash == bone.boneHash);
                if (clip != null)
                {
                    clip.ClipHeader.boneName = bone.boneName;
                }
            }
        }

        protected override bool DoWrite()
        {
            FileHandler handler = FileHandler;
            handler.Clear();
            var header = Header;
            header.Write(handler);
            handler.StringTable?.Clear(); // we'll be re-writing the header's string table at the end
            handler.Align(16);

            var isFirstEntry = handler.Stream.Position < 200;
            // "we only need one bone list header per motlist even if some mots use different bones" - capcom dev, apparently
            var skipBoneList = IsMotlist && !isFirstEntry && header.version >= MotVersion.RE3;
            if (!skipBoneList)
            {
                if (BoneHeaders == null) throw new Exception("Missing bone headers for MOT");

                header.boneHeaderOffsetStart = handler.Tell();
                handler.Write(header.boneHeaderOffsetStart + 16);
                handler.Write((long)BoneHeaders.Count);
                foreach (var bone in BoneHeaders)
                {
                    bone.Write(handler);
                }
                handler.StringTableFlush();
            }

            foreach (var clip in BoneClips)
            {
                clip.ClipHeader.Write(handler);
            }

            foreach (var clip in BoneClips)
            {
                clip.ClipHeader.trackHeaderOffset = handler.Tell();
                clip.Write(handler);
            }

            header.clipCount = (byte)Clips.Count;
            if (Clips.Count > 0)
            {
                header.clipFileOffset = handler.Tell();
                handler.Skip(header.clipCount * sizeof(long));
                for (int i = 0; i < Clips.Count; i++)
                {
                    var clip = Clips[i];
                    handler.Write(header.clipFileOffset + i * sizeof(long), handler.Tell());
                    clip.Write(handler);
                }
            }

            Header.Write(handler, 0);
            handler.StringTableFlush();
            handler.Align(16);
            if (skipBoneList)
            {
                Header.boneHeaderOffsetStart = handler.Tell();
                handler.Write(header.BoneHeaderOffsetStartOffset, Header.boneHeaderOffsetStart);
            }

            return true;
        }

        public override string ToString() => $"{Header.motName}";
    }
}
