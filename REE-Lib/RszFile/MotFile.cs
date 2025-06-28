using System.Numerics;
using ReeLib.Clip;
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
        public long Offset2; // Extra Data Offset

        // public long namesOffset;
        public float frameCount;
        public float blending; // Set to 0 to enable repeating
        public float uknFloat1;
        public float uknFloat2;
        public ushort boneCount;
        public ushort boneClipCount;
        public byte clipCount;
        public byte uknPointer3Count;
        public ushort FrameRate;
        public ushort uknPointerCount;
        public ushort uknShort;

        public string motName = string.Empty;

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
                     ?.Then(ref Offset2)
                     ?.Skip(16);
            }
            else
            {
                action.Then(ref jmapOffset)
                     ?.Then(ref clipFileOffset)
                     ?.Skip(16)
                     ?.Then(ref Offset2);
            }
            action.HandleOffsetWString(ref motName)
                 ?.Then(ref frameCount)
                 ?.Then(ref blending)
                 ?.Then(ref uknFloat1)
                 ?.Then(ref uknFloat2)
                 ?.Then(ref boneCount)
                 ?.Then(ref boneClipCount)
                 ?.Then(ref clipCount)
                 ?.Then(ref uknPointer3Count)
                 ?.Then(ref FrameRate)
                 ?.Then(ref uknPointerCount)
                 ?.Then(ref uknShort);
            return true;
        }
    }


    public class Bone(BoneHeader header)
    {
        public BoneHeader Header { get; set; } = header;

        public BoneClipHeader? ClipHeader { get; set; }
        public TRACKS? Tracks { get; set; }
    }


    public class BoneHeader : ReadWriteModel
    {
        public string boneName = string.Empty;
        public ulong parentOffs;
        public ulong childOffs;
        public ulong nextSiblingOffs;
        public Vector4 translation;
        public Quaternion quaternion;
        public uint Index;
        public uint boneHash;
        public ulong padding;

        protected override bool ReadWrite(IFileHandlerAction action)
        {
            action.HandleOffsetWString(ref boneName)
                 ?.Then(ref parentOffs)
                 ?.Then(ref childOffs)
                 ?.Then(ref nextSiblingOffs)
                 ?.Then(ref translation)
                 ?.Then(ref quaternion)
                 ?.Then(ref Index)
                 ?.Then(ref boneHash)
                 ?.Then(ref padding);
            return action.Success;
        }
    }


    public class BoneHeaders : BaseModel
    {
        public long boneHeaderOffset;
        public long boneHeaderCount;

        public List<BoneHeader> Headers { get; } = new();

        protected override bool DoRead(FileHandler handler)
        {
            handler.Read(ref boneHeaderOffset);
            handler.Read(ref boneHeaderCount);

            if (boneHeaderCount > 1000) throw new InvalidDataException($"boneHeaderCount {boneHeaderCount} > 1000");
            Headers.Clear();
            for (int i = 0; i < boneHeaderCount; i++)
            {
                BoneHeader boneHeader = new();
                boneHeader.Read(handler);
                Headers.Add(boneHeader);
            }
            return true;
        }

        protected override bool DoWrite(FileHandler handler)
        {
            boneHeaderCount = Headers.Count;
            handler.Write(ref boneHeaderOffset);
            handler.Write(ref boneHeaderCount);
            foreach (var header in Headers)
            {
                header.Write(handler);
            }
            handler.StringTableFlush();
            return true;
        }
    }


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

        public MotVersion MotVersion { get; set; }

        public BoneClipHeader(MotVersion motVersion)
        {
            MotVersion = motVersion;
        }

        protected override bool DoRead(FileHandler handler)
        {
            if (MotVersion == MotVersion.RE2_DMC5)
            {
                handler.ReadRange(ref boneIndex, ref trackHeaderOffset);
            }
            else
            {
                handler.Read(ref boneIndex);
                handler.Read(ref trackFlags);
                handler.Read(ref boneHash);
                if (MotVersion == MotVersion.RE7)
                    handler.Read(ref trackHeaderOffset);
                else
                    trackHeaderOffset = handler.ReadUInt();
            }
            return true;
        }

        protected override bool DoWrite(FileHandler handler)
        {
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

        public Vector3Decompression ScaleDecompression => Compression switch
        {
            0x00000 => Vector3Decompression.LoadVector3sFull,
            0x20000 => Vector3Decompression.LoadVector3s5BitA,
            0x30000 => Vector3Decompression.LoadVector3s10BitA,
            0x34000 => Vector3Decompression.LoadScalesXYZ,
            0x40000 => Vector3Decompression.LoadVector3s10BitA,
            0x70000 => Vector3Decompression.LoadVector3s21BitA,
            0x31000 => Vector3Decompression.LoadVector3sXAxis,
            0x32000 => Vector3Decompression.LoadVector3sYAxis,
            0x33000 => Vector3Decompression.LoadVector3sZAxis,
            0x21000 => Vector3Decompression.LoadVector3sXAxis16Bit,
            0x22000 => Vector3Decompression.LoadVector3sYAxis16Bit,
            0x23000 => Vector3Decompression.LoadVector3sZAxis16Bit,
            _ => Vector3Decompression.UnknownType
        };

        public void ReadFrameDataTranslation(FileHandler handler)
        {
            using var defer = handler.SeekJumpBack(frameDataOffset);
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


    public class TRACKS : BaseModel
    {
        public BoneClipHeader ClipHeader { get; }

        public TRACKS(BoneClipHeader clipHeader)
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
    }


    public class MotClip : BaseModel
    {
        public long clipOffset;
        public long endClipStructsRelocation;
        public uint uknIntA;
        public uint uknIntB;
        public byte[] uknBytes1C = new byte[0x1C];
        public ClipEntry ClipEntry { get; set; } = new();

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
            endClipStructsRelocation = ClipEntry.Header.endClipStructsOffset1;
            handler.Write(Start + 8, clipOffset);
            handler.Write(Start + 16, endClipStructsRelocation);
            return true;
        }
    }


    public class ExtraDataHeader(MotVersion version) : BaseModel
    {
        public uint unknown;

        public MotVersion Version { get; set; } = version;

        protected override bool DoRead(FileHandler handler)
        {
            if (Version > MotVersion.MHR_DEMO)
            {
                handler.Read(ref unknown);
            }
            else
            {
                throw new NotImplementedException();
            }
            return true;
        }

        protected override bool DoWrite(FileHandler handler)
        {
            if (Version > MotVersion.MHR_DEMO)
            {
                handler.Write(ref unknown);
            }
            else
            {
                throw new NotImplementedException();
            }
            return true;
        }
    }
}


namespace ReeLib
{
    public class MotFile(RszFileOption option, FileHandler fileHandler, BoneHeaders? boneHeaders = null)
        : BaseRszFile(option, fileHandler)
    {
        public const uint Magic = 0x20746F6D;
        public const string Extension = ".mot";

        public MotHeader Header { get; } = new();
        public BoneHeaders BoneHeaders { get; } = boneHeaders ?? new();
        public List<Bone> Bones { get; } = new();
        public List<MotClip> MotClips { get; } = new();

        protected override bool DoRead()
        {
            Bones.Clear();
            MotClips.Clear();
            var handler = FileHandler;
            var header = Header;
            if (!header.Read(handler)) return false;
            if (header.magic != Magic)
            {
                throw new InvalidDataException($"{handler.FilePath} Not a MOT file");
            }

            if (header.boneHeaderOffsetStart > 0 && header.motSize == 0 || header.motSize > header.boneHeaderOffsetStart)
            {
                handler.Seek(header.boneHeaderOffsetStart);
                BoneHeaders.Read(handler);
            }

            foreach (var boneHeader in BoneHeaders.Headers)
            {
                Bones.Add(new(boneHeader));
            }

            if (header.boneClipCount > 0)
            {
                if (header.boneClipCount > Bones.Count)
                    throw new InvalidDataException($"boneClipCount {header.boneClipCount} > Bones.Count {Bones.Count}");
                var bones = Bones;
                handler.Seek(header.boneClipHeaderOffset);
                BoneClipHeader[] boneClipHeaders = new BoneClipHeader[header.boneClipCount];
                for (int i = 0; i < header.boneClipCount; i++)
                {
                    BoneClipHeader clipHeader = new(header.version);
                    clipHeader.Read(handler);
                    bones[clipHeader.boneIndex].ClipHeader = clipHeader;
                    boneClipHeaders[i] = clipHeader;
                }
                handler.Align(16);
                foreach (var clipHeader in boneClipHeaders)
                {
                    TRACKS tracks = new(clipHeader);
                    tracks.Read(handler);
                    bones[clipHeader.boneIndex].Tracks = tracks;
                }
            }

            if (header.jmapOffset > 0)
            {
                throw new NotImplementedException("Jmap not supported yet");
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
                    MotClips.Add(motClip);
                }
                ExtraDataHeader extraDataHeader = new(header.version);
                extraDataHeader.Read(handler);
            }
            return true;
        }

        protected override bool DoWrite()
        {
            FileHandler handler = FileHandler;
            handler.Clear();
            var header = Header;
            handler.Seek(Header.Size);
            handler.Align(16);

            foreach (var bone in Bones)
            {
                // BoneHeaders
                // ClipHeader
                // Tracks
            }

            foreach (var bone in Bones)
            {
                bone.Tracks?.Write(handler);
            }
            foreach (var bone in Bones)
            {
                bone.Tracks?.WriteOffsetContents(handler);
            }

            Header.Write(handler, 0);
            return true;
        }
    }
}
