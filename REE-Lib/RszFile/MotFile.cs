using System.Diagnostics;
using System.Numerics;
using System.Runtime.InteropServices;
using ReeLib.Clip;
using ReeLib.Common;
using ReeLib.InternalAttributes;
using ReeLib.Mot;
using ReeLib.Motlist;

namespace ReeLib.Mot
{
    public enum MotVersion
    {
        RE7 = 43,
        RE2_DMC5 = 65,
        RE3 = 78,
        MHR_DEMO = 456,
        RE8 = 458,
        RE_RT = 492,
        MHR = 495,
        SF6 = 603,
        RE4 = 613,
        DD2 = 698,
        MHWILDS = 932,
        Pragmata = 993,
    }


    public class MotHeader : ReadWriteModel
    {
        public MotVersion version;
        public uint magic = MotFile.Magic;
        public uint motSize;
        internal long boneHeaderOffsetStart; // BoneBaseDataPointer
        internal long boneClipHeaderOffset; // BoneDataPointer
        internal long motPropertyTracksOffset;

        internal long clipFileOffset;
        internal long motEndClipDataOffset;
        internal long motEndClipFrameValuesOffset;
        internal long propertyTreeOffset;

        // public long namesOffset;
        public float frameCount;
        public float blending = -1; // Set to 0 to enable repeating
        public float startFrame;
        public float endFrame;
        public ushort boneCount;
        public ushort boneClipCount;
        public byte clipCount;
        public byte motEndClipCount;
        public ushort uknExtra; // seems to always be either 0 or 1; natives/stm/animation/ch/ch00/motlist/ch00_001_comnm.motlist.751 - count 1, extra offset 0
        public ushort FrameRate;
        public ushort animatedPropertyCount;

        public string motName = string.Empty;
        public string? jointMapPath = string.Empty;

        internal long BoneHeaderOffsetStartOffset => Start + 16;

        protected override sealed bool ReadWrite<THandler>(THandler action)
        {
            action.Then(ref version)
                 ?.Then(ref magic)
                 ?.Null(4)
                 ?.Then(ref motSize)
                 ?.Then(ref boneHeaderOffsetStart)
                 ?.Then(ref boneClipHeaderOffset)
                 ?.Then(ref motPropertyTracksOffset)
                 ?.Null(8)
                 ?.Then(ref clipFileOffset)
                 // TODO standalone mots _usually_ have some sort of offset data here instead
                 ?.HandleOffsetWString(ref jointMapPath, true)
                 ?.Then(ref motEndClipDataOffset)
                 ?.Then(version >= MotVersion.MHR_DEMO, ref motEndClipFrameValuesOffset)
                 ?.Then(ref propertyTreeOffset)
                 ?.HandleOffsetWString(ref motName)
                 ?.Then(ref frameCount)
                 ?.Then(ref blending)
                 ?.Then(ref startFrame)
                 ?.Then(ref endFrame)
                 ?.Then(ref boneCount)
                 ?.Then(ref boneClipCount)
                 ?.Then(ref clipCount)
                 ?.Then(ref motEndClipCount)
                 ?.Then(version >= MotVersion.RE8, ref uknExtra)
                 ?.Then(ref FrameRate)
                 ?.Then(ref animatedPropertyCount);
            if (version < MotVersion.RE8) {
                action.Then(ref uknExtra);
            } else {
                action.Null(2);
            }

            return true;
        }

        public void CopyValuesFrom(MotHeader source)
        {
            version = source.version;
            motSize = source.motSize;
            FrameRate = source.FrameRate;
            frameCount = source.frameCount;
            endFrame = source.endFrame;
            startFrame = source.startFrame;
            blending = source.blending;
            boneCount = source.boneCount;
            boneClipCount = source.boneClipCount;
            motEndClipCount = source.motEndClipCount;
            uknExtra = source.uknExtra;
            animatedPropertyCount = source.animatedPropertyCount;
            jointMapPath = source.jointMapPath;
            if (string.IsNullOrEmpty(motName)) {
                motName = source.motName;
            }
        }
    }


    public class MotBone(BoneHeader header)
    {
        public BoneHeader Header { get; set; } = header;

        public string Name { get => Header.boneName; set => Header.boneName = value; }
        public int Index { get => Header.Index; set => Header.Index = value; }
        public Vector3 Translation { get => Header.translation; set => Header.translation = value; }
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
        internal long parentOffs;
        internal long childOffs;
        internal long nextSiblingOffs;
        [RszPaddingAfter(4)]
        public Vector3 translation;
        public Quaternion quaternion;
        public int Index;
        public uint boneHash;
        public int uknValue1;
        public int uknValue2;

        public const int StructSize = 80;

        public override string ToString() => $"[{Index}] {boneName}";
    }

    [Flags]
    public enum TrackFlag : byte
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
        public byte uknIndex = byte.MaxValue; // <= re2: always 0, dd2: always 255, other: varying
        public uint boneHash;
        public float uknFloat = 1f; // usually 1, but not always
        public long trackHeaderOffset;
        public string? boneName;

        /// <summary>
        /// Original deserialized name, for easier overview when attempting bone retargeting. Will not be serialized back to the binary format.
        /// </summary>
        public string? OriginalName;

        public MotVersion MotVersion { get; set; }

        public BoneClipHeader(MotVersion motVersion)
        {
            MotVersion = motVersion;
        }

        public override BoneClipHeader Clone() => (BoneClipHeader)base.Clone();

        protected override bool DoRead(FileHandler handler)
        {
            handler.Read(ref boneIndex);
            handler.Read(ref trackFlags);
            handler.Read(ref uknIndex);
            handler.Read(ref boneHash);
            if (MotVersion == MotVersion.RE2_DMC5)
            {
                handler.Read(ref uknFloat);
                handler.ReadNull(4);
                DataInterpretationException.DebugWarnIf(uknFloat <= 0 || uknFloat > 1);
            }

            if (MotVersion <= MotVersion.RE2_DMC5)
            {
                handler.Read(ref trackHeaderOffset);
            }
            else
            {
                trackHeaderOffset = handler.ReadUInt();
            }

            return true;
        }

        protected override bool DoWrite(FileHandler handler)
        {
            if (!string.IsNullOrEmpty(boneName)) boneHash = MurMur3HashUtils.GetHash(boneName);
            handler.Write(ref boneIndex);
            handler.Write(ref trackFlags);
            handler.Write(ref uknIndex);
            handler.Write(ref boneHash);
            if (MotVersion == MotVersion.RE2_DMC5)
            {
                handler.Write(ref uknFloat);
                handler.WriteNull(4);
            }

            if (MotVersion <= MotVersion.RE2_DMC5)
            {
                handler.Write(ref trackHeaderOffset);
            }
            else
            {
                handler.Write((uint)trackHeaderOffset);
            }

            return true;
        }

        public override string ToString() => OriginalName == null || OriginalName == boneName
            ? boneName ?? $"Hash: [{boneHash}]"
            : (boneName ?? $"Hash: [{boneHash}]") + $" (previously: {OriginalName})";
    }


    public enum FloatDecompression
    {
        UnknownType,
        LoadFloatsFull,
        LoadFloats8Bit,
    }

    public enum Vector3Decompression
    {
        UnknownType,
        LoadVector3sFull,
        LoadVector3sXYZAxis,
        LoadVector3sXYZAxis16Bit,
        LoadScalesXYZ,
        LoadVector3s5BitA,
        LoadVector3s10BitA,
        LoadVector3s21BitA,
        LoadVector3s5BitB,
        LoadVector3s8BitB,
        LoadVector3s10BitB,
        LoadVector3s13Bit,
        LoadVector3s16Bit,
        LoadVector3s18Bit,
        LoadVector3s21BitB,
        LoadVector3sXAxis,
        LoadVector3sYAxis,
        LoadVector3sZAxis,
        LoadVector3sXAxis16Bit,
        LoadVector3sYAxis16Bit,
        LoadVector3sZAxis16Bit,
        LoadVector3sXAxis24Bit,
        LoadVector3sYAxis24Bit,
        LoadVector3sZAxis24Bit,
        LoadVector3sXYAxis,
        LoadVector3sYZAxis,
        LoadVector3sZXAxis,
        LoadVector3sXYAxis8Bit,
        LoadVector3sYZAxis8Bit,
        LoadVector3sXZAxis8Bit,
        LoadVector3sXYAxis12Bit,
        LoadVector3sYZAxis12Bit,
        LoadVector3sXZAxis12Bit,
        LoadVector3sXYAxis16Bit,
        LoadVector3sYZAxis16Bit,
        LoadVector3sXZAxis16Bit,
        LoadVector3sXYAxis20Bit,
        LoadVector3sYZAxis20Bit,
        LoadVector3sZXAxis20Bit,
        LoadVector3sXYAxis24Bit,
        LoadVector3sYZAxis24Bit,
        LoadVector3sZXAxis24Bit,
    }

    public enum FrameIndexSize
    {
        Byte = 2,
        Short = 4,
        Int = 5,
    }

    // not sure if it's a flag or just a random arbitrary value, but the value is consistent per track value type across all games.
    public enum TrackValueType
    {
        /// <summary>
        /// Float property tracks (10100010)
        /// </summary>
        Float = 162,

        /// <summary>
        /// Translation/scale tracks (0000 11110010)
        /// </summary>
        Vector3 = 242,

        /// <summary>
        /// Rotation tracks (0001 00010010)
        /// </summary>
        Quaternion = 274,
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
        LoadQuaternionsXAxis24Bit,
        LoadQuaternionsYAxis24Bit,
        LoadQuaternionsZAxis24Bit,
        LoadQuaternionsXAxis,
        LoadQuaternionsYAxis,
        LoadQuaternionsZAxis,
    }

    /// <summary>
    /// Packed 16-Bit Vector 3
    /// </summary>
    public record struct PackedVector3(ushort X, ushort Y, ushort Z);

    public class Track : BaseModel
    {
        public uint flags;
        public int keyCount;
        public uint frameRate;
        public float maxFrame;
        public long frameIndexOffset;
        public long frameDataOffset;
        public long unpackDataOffset;

        public int[]? frameIndexes;
        private readonly float[] unpackData = new float[8];

        /// <summary>
        /// Vector3 value list for this track. Can be either translation or scale depending on track type.
        /// </summary>
        public Vector3[]? translations;

        /// <summary>
        /// Quaternion rotations list for this track.
        /// </summary>
        /// <remarks>
        /// NOTE: all quaternions must be normalized and have a positive W component in order to decompress correctly.
        /// If W &lt; 0, multiply all components by -1 before assigning the values.
        /// </remarks>
        public Quaternion[]? rotations;

        /// <summary>
        /// Float value list for this track. Used by mot properties.
        /// </summary>
        public float[]? floats;

        private long _offsetStart;
        private int DataOffsetSize => MotVersion >= MotVersion.RE3 ? 4 : 8;

        public MotVersion MotVersion { get; set; }

        public int UnpackDataCount
        {
            get {
                if (!RequiresUnpackData) return 0;
                if (TrackType == TrackValueType.Quaternion)
                {
                    if (RotationCompressionType is QuaternionDecompression.LoadQuaternionsXAxis16Bit or QuaternionDecompression.LoadQuaternionsYAxis16Bit or QuaternionDecompression.LoadQuaternionsZAxis16Bit
                        or QuaternionDecompression.LoadQuaternionsXAxis24Bit or QuaternionDecompression.LoadQuaternionsYAxis24Bit or QuaternionDecompression.LoadQuaternionsZAxis24Bit)
                        return 2;

                    return 8;
                }
                if (TrackType == TrackValueType.Vector3)
                {
                    if (TranslationCompressionType is Vector3Decompression.LoadVector3s5BitB or Vector3Decompression.LoadVector3s8BitB or
                        Vector3Decompression.LoadVector3s10BitB or Vector3Decompression.LoadVector3s13Bit or
                        Vector3Decompression.LoadVector3s16Bit or Vector3Decompression.LoadVector3s18Bit or Vector3Decompression.LoadVector3s21BitB)
                        return 6;

                    if (TranslationCompressionType is Vector3Decompression.LoadVector3sXAxis16Bit or Vector3Decompression.LoadVector3sYAxis16Bit or Vector3Decompression.LoadVector3sZAxis16Bit
                        or Vector3Decompression.LoadVector3sXAxis or Vector3Decompression.LoadVector3sYAxis or Vector3Decompression.LoadVector3sZAxis)
                        return 4;

                    if (TranslationCompressionType is Vector3Decompression.LoadVector3sZXAxis or Vector3Decompression.LoadVector3sXYAxis or Vector3Decompression.LoadVector3sYZAxis)
                        return 3;

                    if (TranslationCompressionType is
                        Vector3Decompression.LoadVector3sZXAxis24Bit or Vector3Decompression.LoadVector3sYZAxis24Bit or Vector3Decompression.LoadVector3sXYAxis24Bit or
                        Vector3Decompression.LoadVector3sXYAxis20Bit or Vector3Decompression.LoadVector3sYZAxis20Bit or Vector3Decompression.LoadVector3sZXAxis20Bit or
                        Vector3Decompression.LoadVector3sXYAxis16Bit or Vector3Decompression.LoadVector3sXZAxis16Bit or Vector3Decompression.LoadVector3sYZAxis16Bit or
                        Vector3Decompression.LoadVector3sYZAxis12Bit or Vector3Decompression.LoadVector3sXZAxis12Bit or Vector3Decompression.LoadVector3sXYAxis12Bit or
                        Vector3Decompression.LoadVector3sYZAxis8Bit or Vector3Decompression.LoadVector3sXZAxis8Bit or Vector3Decompression.LoadVector3sXYAxis8Bit
                    )
                        return 5;
                }

                if (TrackType == TrackValueType.Float)
                {
                    if (FloatCompressionType is FloatDecompression.LoadFloats8Bit)
                        return 2;
                }

                return 8;
            }
        }

        public Track(MotVersion motVersion, TrackValueType type)
        {
            MotVersion = motVersion;
            TrackType = type;
            FrameIndexType = FrameIndexSize.Byte;
            if (type == TrackValueType.Quaternion) {
                rotations = [];
            } else if (type == TrackValueType.Vector3) {
                translations = [];
            } else {
                floats = [];
            }
        }

        public FrameIndexSize FrameIndexType
        {
            get => (FrameIndexSize)(flags >> 20);
            set => flags = (flags & 0xFFFFF) | (uint)((byte)value << 20);
        }

        public uint Compression
        {
            get => flags & 0xFF000;
            set => flags = (flags & 0xFFF00FFF) | (value & 0xFF000);
        }

        public TrackValueType TrackType
        {
            get => (TrackValueType)(flags & 0xFFF);
            set => flags = (flags & 0xFFFFF000) | ((uint)value & 0xFFF);
        }

        public void ChangeVersion(MotVersion version)
        {
            // the compression enum is different between games, make sure we keep the same end value
            if (TrackType == TrackValueType.Quaternion)
            {
                var c = RotationCompressionType;
                MotVersion = version;
                RotationCompressionType = c;
            }
            else if (TrackType == TrackValueType.Vector3)
            {
                var c = TranslationCompressionType;
                MotVersion = version;
                TranslationCompressionType = c;
            }
            else if (TrackType == TrackValueType.Float)
            {
                var c = FloatCompressionType;
                MotVersion = version;
                FloatCompressionType = c;
            }
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
                frameIndexes = new int[keyCount];
                handler.Seek(frameIndexOffset);
                switch (FrameIndexType)
                {
                    case FrameIndexSize.Byte:
                        for (int i = 0; i < keyCount; i++) frameIndexes[i] = handler.ReadByte();
                        break;
                    case FrameIndexSize.Short:
                        for (int i = 0; i < keyCount; i++) frameIndexes[i] = handler.ReadShort();
                        break;
                    case FrameIndexSize.Int:
                        for (int i = 0; i < keyCount; i++) frameIndexes[i] = handler.ReadInt();
                        break;
                    default:
                        throw new InvalidDataException($"Unknown frame index type {FrameIndexType}");
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
            EnsureViableFrameIndexSize();
            handler.Write(ref flags);
            handler.Write(ref keyCount);
            if (MotVersion >= MotVersion.RE3)
            {
                _offsetStart = handler.Tell();
                handler.WriteUInt((uint)frameIndexOffset);
                handler.WriteUInt((uint)frameDataOffset);
                handler.WriteUInt((uint)unpackDataOffset);
            }
            else
            {
                handler.Write(ref frameRate);
                handler.Write(ref maxFrame);
                _offsetStart = handler.Tell();
                handler.Write(ref frameIndexOffset);
                handler.Write(ref frameDataOffset);
                handler.Write(ref unpackDataOffset);
            }

            return true;
        }

        private void EnsureViableFrameIndexSize()
        {
            if (frameIndexes == null) return;

            var max = frameIndexes.Last();
            if (max > short.MaxValue)
            {
                FrameIndexType = FrameIndexSize.Int;
            }
            else if (max >= byte.MaxValue && FrameIndexType == FrameIndexSize.Byte)
            {
                FrameIndexType = FrameIndexSize.Short;
            }
        }

        public void WriteOffsetContents(FileHandler handler)
        {
            // FrameIndex
            if (frameIndexes == null)
            {
                handler.WriteInt(_offsetStart, (int)(frameIndexOffset = 0));
            }
            else
            {
                handler.WriteInt(_offsetStart, (int)(frameIndexOffset = handler.Tell()));
                switch (FrameIndexType)
                {
                    case FrameIndexSize.Byte:
                        for (int i = 0; i < keyCount; i++) handler.WriteByte((byte)frameIndexes[i]);
                        break;
                    case FrameIndexSize.Short:
                        for (int i = 0; i < keyCount; i++) handler.WriteShort((short)frameIndexes[i]);
                        break;
                    case FrameIndexSize.Int:
                        for (int i = 0; i < keyCount; i++) handler.WriteInt(frameIndexes[i]);
                        break;
                    default:
                        throw new InvalidDataException($"Unknown frame index type {FrameIndexType}");
                }
            }

            // handle frame data
            // NOTE: even "empty" tracks always have at least one frame data item, but if there's multiple such tracks, they all reuse the same block of zeroes
            // for reference, it seems to relocate every "zero" track to the same sets of 3 zero floats (position/scale) and 3 zero floats (rotation)
            // the block is placed right after the first track that has BOTH (positions ?? scale) and rotations, likely falling back to after last track if there's no such cases
            // sounds needlessly convoluted and it doesn't seem to affect being able to load so we're skipping that
            handler.Align(4);
            handler.WriteInt(_offsetStart + DataOffsetSize, (int)(frameDataOffset = handler.Tell()));
            if (TrackType == TrackValueType.Vector3)
            {
                WriteFrameDataTranslation(handler);
            }
            else if (TrackType == TrackValueType.Quaternion)
            {
                WriteFrameDataRotation(handler);
            }
            else if (TrackType == TrackValueType.Float)
            {
                WriteFrameDataFloats(handler);
            }
            else
            {
                throw new NotImplementedException("Unsupported track type " + TrackType);
            }

            if (RequiresUnpackData)
            {
                handler.Align(4);
                handler.WriteInt(_offsetStart + DataOffsetSize * 2, (int)(unpackDataOffset = handler.Tell()));
                handler.WriteArray(unpackData, 0, UnpackDataCount);
            }
            else
            {
                handler.WriteInt(_offsetStart + DataOffsetSize * 2, (int)(unpackDataOffset = 0));
            }
        }

        private static Dictionary<uint, Vector3Decompression> TranslationDictDmc5 = new() {
            { 0x00000, Vector3Decompression.LoadVector3sFull },
            { 0x20000, Vector3Decompression.LoadVector3s5BitA },
            { 0x21000, Vector3Decompression.LoadVector3sXAxis16Bit },
            { 0x22000, Vector3Decompression.LoadVector3sYAxis16Bit },
            { 0x23000, Vector3Decompression.LoadVector3sZAxis16Bit },
            { 0x24000, Vector3Decompression.LoadVector3sXYZAxis16Bit },
            { 0x30000, Vector3Decompression.LoadVector3s10BitA },
            { 0x31000, Vector3Decompression.LoadVector3sXAxis },
            { 0x32000, Vector3Decompression.LoadVector3sYAxis },
            { 0x33000, Vector3Decompression.LoadVector3sZAxis },
            { 0x34000, Vector3Decompression.LoadScalesXYZ },
            { 0x70000, Vector3Decompression.LoadVector3s21BitA },
        };

        private static Dictionary<uint, Vector3Decompression> TranslationDict = new() {
            { 0x00000, Vector3Decompression.LoadVector3sFull },
            { 0x20000, Vector3Decompression.LoadVector3s5BitB },  // 16 bits per value
            { 0x21000, Vector3Decompression.LoadVector3sXAxis16Bit },
            { 0x22000, Vector3Decompression.LoadVector3sYAxis16Bit },
            { 0x23000, Vector3Decompression.LoadVector3sZAxis16Bit },
            { 0x24000, Vector3Decompression.LoadVector3sXYZAxis16Bit },
            { 0x25000, Vector3Decompression.LoadVector3sXYAxis8Bit },
            { 0x26000, Vector3Decompression.LoadVector3sXZAxis8Bit },
            { 0x27000, Vector3Decompression.LoadVector3sYZAxis8Bit },
            { 0x30000, Vector3Decompression.LoadVector3s8BitB }, // 24 bits per value
            { 0x31000, Vector3Decompression.LoadVector3sXAxis24Bit },
            { 0x32000, Vector3Decompression.LoadVector3sYAxis24Bit },
            { 0x33000, Vector3Decompression.LoadVector3sZAxis24Bit },
            { 0x35000, Vector3Decompression.LoadVector3sYZAxis12Bit },
            { 0x36000, Vector3Decompression.LoadVector3sXZAxis12Bit },
            { 0x37000, Vector3Decompression.LoadVector3sXYAxis12Bit },
            { 0x40000, Vector3Decompression.LoadVector3s10BitB }, // 32 bits per value
            { 0x41000, Vector3Decompression.LoadVector3sXAxis },
            { 0x42000, Vector3Decompression.LoadVector3sYAxis },
            { 0x43000, Vector3Decompression.LoadVector3sZAxis },
            { 0x44000, Vector3Decompression.LoadVector3sXYZAxis },
            { 0x45000, Vector3Decompression.LoadVector3sXYAxis16Bit },
            { 0x46000, Vector3Decompression.LoadVector3sXZAxis16Bit },
            { 0x47000, Vector3Decompression.LoadVector3sYZAxis16Bit },
            { 0x50000, Vector3Decompression.LoadVector3s13Bit }, // 40 bits per value
            { 0x55000, Vector3Decompression.LoadVector3sXYAxis20Bit },
            { 0x56000, Vector3Decompression.LoadVector3sYZAxis20Bit },
            { 0x57000, Vector3Decompression.LoadVector3sZXAxis20Bit },
            { 0x60000, Vector3Decompression.LoadVector3s16Bit }, // 48 bits per value
            { 0x65000, Vector3Decompression.LoadVector3sXYAxis24Bit },
            { 0x66000, Vector3Decompression.LoadVector3sYZAxis24Bit },
            { 0x67000, Vector3Decompression.LoadVector3sZXAxis24Bit },
            { 0x70000, Vector3Decompression.LoadVector3s18Bit }, // 56 bits per value
            { 0x80000, Vector3Decompression.LoadVector3s21BitB }, // 64 bits per value
            { 0x85000, Vector3Decompression.LoadVector3sXYAxis },
            { 0x86000, Vector3Decompression.LoadVector3sYZAxis },
            { 0x87000, Vector3Decompression.LoadVector3sZXAxis },
        };

        private static Dictionary<uint, QuaternionDecompression> RotationDictDmc5 = new() {
            { 0x00000, QuaternionDecompression.LoadQuaternionsFull },
            { 0x21000, QuaternionDecompression.LoadQuaternionsXAxis16Bit },
            { 0x22000, QuaternionDecompression.LoadQuaternionsYAxis16Bit },
            { 0x23000, QuaternionDecompression.LoadQuaternionsZAxis16Bit },
            { 0x30000, QuaternionDecompression.LoadQuaternions10Bit },
            { 0x31000, QuaternionDecompression.LoadQuaternionsXAxis },
            { 0x32000, QuaternionDecompression.LoadQuaternionsYAxis },
            { 0x33000, QuaternionDecompression.LoadQuaternionsZAxis },
            { 0x70000, QuaternionDecompression.LoadQuaternions21Bit },
            { 0xB0000, QuaternionDecompression.LoadQuaternions3Component },
        };

        private static Dictionary<uint, QuaternionDecompression> RotationDict = new() {
            { 0x00000, QuaternionDecompression.LoadQuaternionsFull },
            { 0x20000, QuaternionDecompression.LoadQuaternions5Bit }, // 16 bits
            { 0x21000, QuaternionDecompression.LoadQuaternionsXAxis16Bit },
            { 0x22000, QuaternionDecompression.LoadQuaternionsYAxis16Bit },
            { 0x23000, QuaternionDecompression.LoadQuaternionsZAxis16Bit },
            { 0x30000, QuaternionDecompression.LoadQuaternions8Bit }, // 24 bits
            { 0x31000, QuaternionDecompression.LoadQuaternionsXAxis24Bit },
            { 0x32000, QuaternionDecompression.LoadQuaternionsYAxis24Bit },
            { 0x33000, QuaternionDecompression.LoadQuaternionsZAxis24Bit },
            { 0x40000, QuaternionDecompression.LoadQuaternions10Bit }, // 32 bits
            { 0x41000, QuaternionDecompression.LoadQuaternionsXAxis },
            { 0x42000, QuaternionDecompression.LoadQuaternionsYAxis },
            { 0x43000, QuaternionDecompression.LoadQuaternionsZAxis },
            { 0x50000, QuaternionDecompression.LoadQuaternions13Bit }, // 40 bits
            { 0x60000, QuaternionDecompression.LoadQuaternions16Bit }, // 48 bits
            { 0x70000, QuaternionDecompression.LoadQuaternions18Bit }, // 56 bits
            { 0x80000, QuaternionDecompression.LoadQuaternions21Bit }, // 64 bits
            { 0xC0000, QuaternionDecompression.LoadQuaternions3Component }, // 96 bit
        };

        private static Dictionary<uint, FloatDecompression> FloatDict = new() {
            { 0x00000, FloatDecompression.LoadFloatsFull },
            { 0x10000, FloatDecompression.LoadFloats8Bit },
        };

        public Vector3Decompression TranslationCompressionType
        {
            get => (MotVersion <= MotVersion.RE2_DMC5 ? TranslationDictDmc5 : TranslationDict).GetValueOrDefault(Compression);
            set => Compression = (MotVersion <= MotVersion.RE2_DMC5 ? TranslationDictDmc5 : TranslationDict).FirstOrDefault(kv => kv.Value == value).Key;
        }

        public QuaternionDecompression RotationCompressionType
        {
            get => (MotVersion <= MotVersion.RE2_DMC5 ? RotationDictDmc5 : RotationDict).GetValueOrDefault(Compression);
            set => Compression = (MotVersion <= MotVersion.RE2_DMC5 ? RotationDictDmc5 : RotationDict).FirstOrDefault(kv => kv.Value == value).Key;
        }

        public FloatDecompression FloatCompressionType
        {
            // get => FloatDict.GetValueOrDefault(Compression);
            get => FloatDict[Compression];
            set => Compression = FloatDict.FirstOrDefault(kv => kv.Value == value).Key;
        }

        public bool RequiresUnpackData => TrackType switch {
            TrackValueType.Quaternion => !(RotationCompressionType is QuaternionDecompression.LoadQuaternionsFull or QuaternionDecompression.LoadQuaternions3Component or QuaternionDecompression.LoadQuaternionsXAxis or QuaternionDecompression.LoadQuaternionsYAxis or QuaternionDecompression.LoadQuaternionsZAxis),
            TrackValueType.Vector3 => !(TranslationCompressionType is Vector3Decompression.LoadVector3sFull or Vector3Decompression.LoadScalesXYZ or Vector3Decompression.LoadVector3sXYZAxis),
            TrackValueType.Float => !(FloatCompressionType is FloatDecompression.LoadFloatsFull),
            _ => false,
        };

        private static ulong ReadBytesAsUInt64(FileHandler handler, int bytes)
        {
            var data = handler.ReadBytes(bytes);
            ulong val = 0;
            for (int j = 0; j < bytes; j++)
            {
                val = data[j] | (val << 8);
            }
            return val;
        }
        private static uint ReadBytesAsUInt32(FileHandler handler, int bytes)
        {
            var data = handler.ReadBytes(bytes);
            uint val = 0;
            for (int j = 0; j < bytes; j++)
            {
                val = data[j] | (val << 8);
            }
            return val;
        }
        private static void WriteUnit64AsBytes(FileHandler handler, ulong value, int bytes)
        {
            for (int j = (bytes - 1) * 8; j >= 0; j-= 8)
            {
                handler.Write((byte)((value >> j) & 0xff));
            }
        }

        public void ReadFrameDataTranslation(FileHandler handler)
        {
            using var defer = handler.SeekJumpBack(frameDataOffset);
            Vector3Decompression type = TranslationCompressionType;
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
                            translation.X = (unpackData[0] * ((data >> 00) & 0b11111) / 0b11111) + unpackData[4];
                            translation.Y = (unpackData[1] * ((data >> 05) & 0b11111) / 0b11111) + unpackData[5];
                            translation.Z = (unpackData[2] * ((data >> 10) & 0b11111) / 0b11111) + unpackData[6];
                            break;
                        }
                    case Vector3Decompression.LoadVector3s5BitB:
                        {
                            var data = handler.Read<ushort>();
                            translation.X = unpackData[0] * ((data >> 00) & 0b11111) / 0b11111 + unpackData[3];
                            translation.Y = unpackData[1] * ((data >> 05) & 0b11111) / 0b11111 + unpackData[4];
                            translation.Z = unpackData[2] * ((data >> 10) & 0b11111) / 0b11111 + unpackData[5];
                            break;
                        }
                    case Vector3Decompression.LoadVector3s8BitB:
                        {
                            Span<byte> data = stackalloc byte[3];
                            handler.ReadSpan<byte>(data);
                            translation.X = unpackData[0] * (data[0]) * (1f / 0xFF) + unpackData[3];
                            translation.Y = unpackData[1] * (data[1]) * (1f / 0xFF) + unpackData[4];
                            translation.Z = unpackData[2] * (data[2]) * (1f / 0xFF) + unpackData[5];
                            break;
                        }
                    case Vector3Decompression.LoadVector3s13Bit:
                        {
                            var data = ReadBytesAsUInt64(handler, 5);
                            translation.X = unpackData[0] * ((data >> 00) & 0x1FFF) / 0x1FFF + unpackData[3];
                            translation.Y = unpackData[1] * ((data >> 13) & 0x1FFF) / 0x1FFF + unpackData[4];
                            translation.Z = unpackData[2] * ((data >> 26) & 0x1FFF) / 0x1FFF + unpackData[5];
                            break;
                        }
                    case Vector3Decompression.LoadVector3s18Bit:
                        {
                            var val = ReadBytesAsUInt64(handler, 7);
                            translation.X = unpackData[0] * ((val >> 00) & 0x3FFFF) / (float)0x3FFFF + unpackData[3];
                            translation.Y = unpackData[1] * ((val >> 18) & 0x3FFFF) / (float)0x3FFFF + unpackData[4];
                            translation.Z = unpackData[2] * ((val >> 36) & 0x3FFFF) / (float)0x3FFFF + unpackData[5];
                            break;
                        }
                    case Vector3Decompression.LoadScalesXYZ:
                        translation.X = translation.Y = translation.Z = handler.Read<float>();
                        break;
                    case Vector3Decompression.LoadVector3s10BitA:
                        {
                            var data = handler.Read<uint>();
                            translation.X = unpackData[0] * (((data >> 00) & 0b1111111111) * (1f / 0b1111111111)) + unpackData[4];
                            translation.Y = unpackData[1] * (((data >> 10) & 0b1111111111) * (1f / 0b1111111111)) + unpackData[5];
                            translation.Z = unpackData[2] * (((data >> 20) & 0b1111111111) * (1f / 0b1111111111)) + unpackData[6];
                            break;
                        }
                    case Vector3Decompression.LoadVector3s10BitB:
                        {
                            var data = handler.Read<uint>();
                            translation.X = unpackData[0] * (((data >> 00) & 0b1111111111) * (1f / 0b1111111111)) + unpackData[3];
                            translation.Y = unpackData[1] * (((data >> 10) & 0b1111111111) * (1f / 0b1111111111)) + unpackData[4];
                            translation.Z = unpackData[2] * (((data >> 20) & 0b1111111111) * (1f / 0b1111111111)) + unpackData[5];
                            break;
                        }
                    case Vector3Decompression.LoadVector3s16Bit:
                        {
                            var data = ReadBytesAsUInt64(handler, 6);
                            translation.X = (unpackData[0] * ((data >> 00) & 0xFFFF) / (float)0xFFFF) + unpackData[3];
                            translation.Y = (unpackData[1] * ((data >> 16) & 0xFFFF) / (float)0xFFFF) + unpackData[4];
                            translation.Z = (unpackData[2] * ((data >> 32) & 0xFFFF) / (float)0xFFFF) + unpackData[5];
                            break;
                        }
                    case Vector3Decompression.LoadVector3s21BitA:
                        {
                            var data = handler.Read<ulong>();
                            translation.X = (unpackData[0] * ((data >> 00) & 0x1F_FFFF) * (1f / 0x1F_FFFF)) + unpackData[4];
                            translation.Y = (unpackData[1] * ((data >> 21) & 0x1F_FFFF) * (1f / 0x1F_FFFF)) + unpackData[5];
                            translation.Z = (unpackData[2] * ((data >> 42) & 0x1F_FFFF) * (1f / 0x1F_FFFF)) + unpackData[6];
                            break;
                        }
                    case Vector3Decompression.LoadVector3s21BitB:
                        {
                            var data = handler.Read<ulong>();
                            translation.X = (unpackData[0] * ((data >> 00) & 0x1F_FFFF) * (1f / 0x1F_FFFF)) + unpackData[3];
                            translation.Y = (unpackData[1] * ((data >> 21) & 0x1F_FFFF) * (1f / 0x1F_FFFF)) + unpackData[4];
                            translation.Z = (unpackData[2] * ((data >> 42) & 0x1F_FFFF) * (1f / 0x1F_FFFF)) + unpackData[5];
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
                    case Vector3Decompression.LoadVector3sXYAxis:
                        {
                            translation.X = handler.Read<float>();
                            translation.Y = handler.Read<float>();
                            translation.Z = unpackData[2];
                            break;
                        }
                    case Vector3Decompression.LoadVector3sYZAxis:
                        {
                            translation.X = unpackData[0];
                            translation.Y = handler.Read<float>();
                            translation.Z = handler.Read<float>();
                            break;
                        }
                    case Vector3Decompression.LoadVector3sZXAxis:
                        {
                            translation.Z = handler.Read<float>();
                            translation.Y = unpackData[1];
                            translation.X = handler.Read<float>();
                            break;
                        }
                    case Vector3Decompression.LoadVector3sXAxis16Bit:
                        {
                            var data = handler.Read<ushort>();
                            translation.X = unpackData[0] * (data * (1f / 0xFFFF)) + unpackData[1];
                            translation.Y = unpackData[2];
                            translation.Z = unpackData[3];
                            break;
                        }
                    case Vector3Decompression.LoadVector3sYAxis16Bit:
                        {
                            var data = handler.Read<ushort>();
                            translation.X = unpackData[1];
                            translation.Y = unpackData[0] * (data * (1f / 0xFFFF)) + unpackData[2];
                            translation.Z = unpackData[3];
                            break;
                        }
                    case Vector3Decompression.LoadVector3sZAxis16Bit:
                        {
                            var data = handler.Read<ushort>();
                            translation.X = unpackData[1];
                            translation.Y = unpackData[2];
                            translation.Z = unpackData[0] * (data * (1f / 0xFFFF)) + unpackData[3];
                            break;
                        }
                    case Vector3Decompression.LoadVector3sXAxis24Bit:
                        {
                            var data = ReadBytesAsUInt32(handler, 3);
                            translation.X = unpackData[0] * (data * (1f / 0xFFFFFF)) + unpackData[1];
                            translation.Y = unpackData[2];
                            translation.Z = unpackData[3];
                            break;
                        }
                    case Vector3Decompression.LoadVector3sYAxis24Bit:
                        {
                            var data = ReadBytesAsUInt32(handler, 3);
                            translation.X = unpackData[1];
                            translation.Y = unpackData[0] * (data * (1f / 0xFFFFFF)) + unpackData[2];
                            translation.Z = unpackData[3];
                            break;
                        }
                    case Vector3Decompression.LoadVector3sZAxis24Bit:
                        {
                            var data = ReadBytesAsUInt32(handler, 3);
                            translation.X = unpackData[1];
                            translation.Y = unpackData[2];
                            translation.Z = unpackData[0] * (data * (1f / 0xFFFFFF)) + unpackData[3];
                            break;
                        }
                    case Vector3Decompression.LoadVector3sXYZAxis16Bit:
                        {
                            var data = handler.Read<ushort>();
                            translation.X = translation.Y = translation.Z = unpackData[0] * (data * (1f / 0xFFFF)) + unpackData[3];
                            break;
                        }
                    case Vector3Decompression.LoadVector3sXYAxis8Bit:
                        {
                            translation.X = unpackData[0] * (handler.Read<byte>() * (1f / 0xFF)) + unpackData[2];
                            translation.Y = unpackData[1] * (handler.Read<byte>() * (1f / 0xFF)) + unpackData[3];
                            translation.Z = unpackData[4];
                            break;
                        }
                    case Vector3Decompression.LoadVector3sXZAxis8Bit:
                        {
                            translation.X = unpackData[0] * (handler.Read<byte>() * (1f / 0xFF)) + unpackData[2];
                            translation.Y = unpackData[3];
                            translation.Z = unpackData[1] * (handler.Read<byte>() * (1f / 0xFF)) + unpackData[4];
                            break;
                        }
                    case Vector3Decompression.LoadVector3sYZAxis8Bit:
                        {
                            translation.X = unpackData[2];
                            translation.Y = unpackData[0] * (handler.Read<byte>() * (1f / 0xFF)) + unpackData[3];
                            translation.Z = unpackData[1] * (handler.Read<byte>() * (1f / 0xFF)) + unpackData[4];
                            break;
                        }
                    case Vector3Decompression.LoadVector3sYZAxis12Bit:
                        {
                            var val = ReadBytesAsUInt32(handler, 3);
                            translation.X = unpackData[2];
                            translation.Y = unpackData[0] * ((val >> 00) & 0xFFF) * (1f / 0xFFF) + unpackData[3];
                            translation.Z = unpackData[1] * ((val >> 12) & 0xFFF) * (1f / 0xFFF) + unpackData[4];
                            break;
                        }
                    case Vector3Decompression.LoadVector3sXZAxis12Bit:
                        {
                            var val = ReadBytesAsUInt32(handler, 3);
                            translation.X = unpackData[0] * ((val >> 00) & 0xFFF) * (1f / 0xFFF) + unpackData[2];
                            translation.Y = unpackData[3];
                            translation.Z = unpackData[1] * ((val >> 12) & 0xFFF) * (1f / 0xFFF) + unpackData[4];
                            break;
                        }
                    case Vector3Decompression.LoadVector3sXYAxis12Bit:
                        {
                            var val = ReadBytesAsUInt32(handler, 3);
                            translation.X = unpackData[0] * ((val >> 00) & 0xFFF) * (1f / 0xFFF) + unpackData[2];
                            translation.Y = unpackData[1] * ((val >> 12) & 0xFFF) * (1f / 0xFFF) + unpackData[3];
                            translation.Z = unpackData[4];
                            break;
                        }
                    case Vector3Decompression.LoadVector3sXYAxis16Bit:
                        {
                            translation.X = unpackData[0] * (handler.Read<ushort>() * (1f / 0xFFFF)) + unpackData[2];
                            translation.Y = unpackData[1] * (handler.Read<ushort>() * (1f / 0xFFFF)) + unpackData[3];
                            translation.Z = unpackData[4];
                            break;
                        }
                    case Vector3Decompression.LoadVector3sXZAxis16Bit:
                        {
                            translation.X = unpackData[0] * (handler.Read<ushort>() * (1f / 0xFFFF)) + unpackData[2];
                            translation.Y = unpackData[3];
                            translation.Z = unpackData[1] * (handler.Read<ushort>() * (1f / 0xFFFF)) + unpackData[4];
                            break;
                        }
                    case Vector3Decompression.LoadVector3sYZAxis16Bit:
                        {
                            translation.X = unpackData[2];
                            translation.Y = unpackData[0] * (handler.Read<ushort>() * (1f / 0xFFFF)) + unpackData[3];
                            translation.Z = unpackData[1] * (handler.Read<ushort>() * (1f / 0xFFFF)) + unpackData[4];
                            break;
                        }
                    case Vector3Decompression.LoadVector3sXYAxis20Bit:
                        {
                            // test: ch5800_damage.motlist.1057  ch5800_Damage_0515_body_large_B  Hip
                            var val = ReadBytesAsUInt64(handler, 5);
                            translation.X = unpackData[0] * (((val >> 00) & 0xFFFFF) * (1f / 0xFFFFF)) + unpackData[2];
                            translation.Y = unpackData[1] * (((val >> 20) & 0xFFFFF) * (1f / 0xFFFFF)) + unpackData[3];
                            translation.Z = unpackData[4];
                            break;
                        }
                    case Vector3Decompression.LoadVector3sYZAxis20Bit:
                        {
                            var val = ReadBytesAsUInt64(handler, 5);
                            translation.X = unpackData[2];
                            translation.Y = unpackData[0] * ((val >> 00) & 0xFFFFF) * (1f / 0xFFFFF) + unpackData[3];
                            translation.Z = unpackData[1] * ((val >> 20) & 0xFFFFF) * (1f / 0xFFFFF) + unpackData[4];
                            break;
                        }
                    case Vector3Decompression.LoadVector3sZXAxis20Bit:
                        {
                            // seems to actually be ZX instead of XZ (see ch500_Attack_4005_)
                            var val = ReadBytesAsUInt64(handler, 5);
                            translation.X = unpackData[1] * ((val >> 20) & 0xFFFFF) * (1f / 0xFFFFF) + unpackData[2];
                            translation.Y = unpackData[3];
                            translation.Z = unpackData[0] * ((val >> 00) & 0xFFFFF) * (1f / 0xFFFFF) + unpackData[4];
                            break;
                        }
                    case Vector3Decompression.LoadVector3sXYAxis24Bit:
                        {
                            var data = ReadBytesAsUInt64(handler, 6);
                            translation.X = unpackData[0] * ((data >> 0) & 0xFFFFFF) * (1f / 0xFFFFFF) + unpackData[2];
                            translation.Y = unpackData[1] * ((data >> 24) & 0xFFFFFF) * (1f / 0xFFFFFF) + unpackData[3];
                            translation.Z = unpackData[4];
                            break;
                        }
                    case Vector3Decompression.LoadVector3sYZAxis24Bit:
                        {
                            var data = ReadBytesAsUInt64(handler, 6);
                            translation.X = unpackData[2];
                            translation.Y = unpackData[0] * ((data >> 00) & 0xFFFFFF) * (1f / 0xFFFFFF) + unpackData[3];
                            translation.Z = unpackData[1] * ((data >> 24) & 0xFFFFFF) * (1f / 0xFFFFFF) + unpackData[4];
                            break;
                        }
                    case Vector3Decompression.LoadVector3sZXAxis24Bit:
                        {
                            var data = ReadBytesAsUInt64(handler, 6);
                            translation.X = unpackData[1] * (((data >> 24) & 0xFFFFFF) * (1f / 0xFFFFFF)) + unpackData[2];
                            translation.Y = unpackData[3];
                            translation.Z = unpackData[0] * (((data >> 0) & 0xFFFFFF) * (1f / 0xFFFFFF)) + unpackData[4];
                            break;
                        }
                    case Vector3Decompression.LoadVector3sXYZAxis:
                        translation.X = translation.Y = translation.Z = handler.Read<float>();
                        break;
                    default:
                        throw new InvalidOperationException($"Unknown Vector3 compression type {type} ({Compression.ToString("X")})");
                }
                translations[i] = translation;
            }
        }

        public void WriteFrameDataTranslation(FileHandler handler)
        {
            if (translations == null) throw new NullReferenceException($"{nameof(translations)} is null");
            Vector3Decompression type = TranslationCompressionType;
            RecomputeDenormalizationParams();
            for (int i = 0; i < keyCount; i++)
            {
                Vector3 translation = translations[i];
                switch (type)
                {
                    case Vector3Decompression.LoadVector3sFull:
                        handler.Write(translation);
                        break;
                    case Vector3Decompression.LoadScalesXYZ:
                        handler.Write(translation.X);
                        break;
                    case Vector3Decompression.LoadVector3s5BitA:
                        {
                            ushort data = (ushort)(
                                ((ushort)MathF.Round((translation.X - unpackData[4]) / unpackData[0] * 0b11111) << 00) |
                                ((ushort)MathF.Round((translation.Y - unpackData[5]) / unpackData[1] * 0b11111) << 05) |
                                ((ushort)MathF.Round((translation.Z - unpackData[6]) / unpackData[2] * 0b11111) << 10));
                            handler.Write(data);
                            break;
                        }
                    case Vector3Decompression.LoadVector3s5BitB:
                        {
                            ushort data = (ushort)(
                                ((ushort)MathF.Round((translation.X - unpackData[3]) / unpackData[0] * 0b11111) << 00) |
                                ((ushort)MathF.Round((translation.Y - unpackData[4]) / unpackData[1] * 0b11111) << 05) |
                                ((ushort)MathF.Round((translation.Z - unpackData[5]) / unpackData[2] * 0b11111) << 10));
                            handler.Write(data);
                            break;
                        }
                    case Vector3Decompression.LoadVector3s8BitB:
                        {
                            handler.Write((byte)MathF.Round((translation.X - unpackData[3]) / unpackData[0] * 0xFF));
                            handler.Write((byte)MathF.Round((translation.Y - unpackData[4]) / unpackData[1] * 0xFF));
                            handler.Write((byte)MathF.Round((translation.Z - unpackData[5]) / unpackData[2] * 0xFF));
                            break;
                        }
                    case Vector3Decompression.LoadVector3s10BitA:
                        {
                            uint data =
                                ((uint)MathF.Round((translation.X - unpackData[4]) / unpackData[0] * 0x3FF) << 00) |
                                ((uint)MathF.Round((translation.Y - unpackData[5]) / unpackData[1] * 0x3FF) << 10) |
                                ((uint)MathF.Round((translation.Z - unpackData[6]) / unpackData[2] * 0x3FF) << 20);
                            handler.Write(data);
                            break;
                        }
                    case Vector3Decompression.LoadVector3s10BitB:
                        {
                            uint data = (
                                ((uint)MathF.Round((translation.X - unpackData[3]) / unpackData[0] * 0x3FF) << 00) |
                                ((uint)MathF.Round((translation.Y - unpackData[4]) / unpackData[1] * 0x3FF) << 10) |
                                ((uint)MathF.Round((translation.Z - unpackData[5]) / unpackData[2] * 0x3FF) << 20)
                            );
                            handler.Write(data);
                            break;
                        }
                    case Vector3Decompression.LoadVector3s13Bit:
                        {
                            ulong data = (
                                ((ulong)MathF.Round((translation.X - unpackData[3]) / unpackData[0] * 0x1FFF) << 00) |
                                ((ulong)MathF.Round((translation.Y - unpackData[4]) / unpackData[1] * 0x1FFF) << 13) |
                                ((ulong)MathF.Round((translation.Z - unpackData[5]) / unpackData[2] * 0x1FFF) << 26)
                            );
                            WriteUnit64AsBytes(handler, data, 5);
                            break;
                        }
                    case Vector3Decompression.LoadVector3s16Bit:
                        {
                            var x = (ushort)MathF.Round((translation.X - unpackData[3]) / unpackData[0] * 0xFFFF);
                            var y = (ushort)MathF.Round((translation.Y - unpackData[4]) / unpackData[1] * 0xFFFF);
                            var z = (ushort)MathF.Round((translation.Z - unpackData[5]) / unpackData[2] * 0xFFFF);
                            handler.Write((byte)((z >> 8) & 0xFF)); handler.Write((byte)z);
                            handler.Write((byte)((y >> 8) & 0xFF)); handler.Write((byte)y);
                            handler.Write((byte)((x >> 8) & 0xFF)); handler.Write((byte)x);
                            break;
                        }
                    case Vector3Decompression.LoadVector3s18Bit:
                        {
                            ulong data = (
                                ((ulong)MathF.Round((translation.X - unpackData[3]) / unpackData[0] * 0x3FFFF) << 00) |
                                ((ulong)MathF.Round((translation.Y - unpackData[4]) / unpackData[1] * 0x3FFFF) << 18) |
                                ((ulong)MathF.Round((translation.Z - unpackData[5]) / unpackData[2] * 0x3FFFF) << 36)
                            );
                            WriteUnit64AsBytes(handler, data, 7);
                            break;
                        }
                    case Vector3Decompression.LoadVector3s21BitA:
                        {
                            ulong data =
                                ((ulong)MathF.Round((translation.X - unpackData[4]) / unpackData[0] * 0x1F_FFFF) << 00) |
                                ((ulong)MathF.Round((translation.Y - unpackData[5]) / unpackData[1] * 0x1F_FFFF) << 21) |
                                ((ulong)MathF.Round((translation.Z - unpackData[6]) / unpackData[2] * 0x1F_FFFF) << 42);
                            handler.Write(data);
                            break;
                        }
                    case Vector3Decompression.LoadVector3s21BitB:
                        {
                            var data =
                                ((ulong)MathF.Round((translation.X - unpackData[3]) / unpackData[0] * 0x1F_FFFF) << 00) |
                                ((ulong)MathF.Round((translation.Y - unpackData[4]) / unpackData[1] * 0x1F_FFFF) << 21) |
                                ((ulong)MathF.Round((translation.Z - unpackData[5]) / unpackData[2] * 0x1F_FFFF) << 42);
                            handler.Write((ulong)data);
                            break;
                        }

                    case Vector3Decompression.LoadVector3sXAxis:
                        handler.Write(translation.X);
                        break;
                    case Vector3Decompression.LoadVector3sYAxis:
                        handler.Write(translation.Y);
                        break;
                    case Vector3Decompression.LoadVector3sZAxis:
                        handler.Write(translation.Z);
                        break;
                    case Vector3Decompression.LoadVector3sXYAxis:
                        handler.Write(translation.X);
                        handler.Write(translation.Y);
                        break;
                    case Vector3Decompression.LoadVector3sYZAxis:
                        handler.Write(translation.Y);
                        handler.Write(translation.Z);
                        break;
                    case Vector3Decompression.LoadVector3sZXAxis:
                        handler.Write(translation.Z);
                        handler.Write(translation.X);
                        break;

                    case Vector3Decompression.LoadVector3sXAxis16Bit:
                        {
                            var data = (ushort)MathF.Round((translation.X - unpackData[1]) / unpackData[0] * 0xFFFF);
                            handler.Write(data);
                            break;
                        }
                    case Vector3Decompression.LoadVector3sYAxis16Bit:
                        {
                            var data = (ushort)MathF.Round((translation.Y - unpackData[2]) / unpackData[0] * 0xFFFF);
                            handler.Write(data);
                            break;
                        }
                    case Vector3Decompression.LoadVector3sZAxis16Bit:
                        {
                            var data = (ushort)MathF.Round((translation.Z - unpackData[3]) / unpackData[0] * 0xFFFF);
                            handler.Write(data);
                            break;
                        }
                    case Vector3Decompression.LoadVector3sXAxis24Bit:
                        {
                            uint data = (uint)MathF.Round((translation.X - unpackData[1]) / unpackData[0] * 0xFFFFFF);
                            handler.Write((byte)((data >> 16) & 0xFF));
                            handler.Write((byte)((data >> 8) & 0xFF));
                            handler.Write((byte)((data >> 0) & 0xFF));
                            break;
                        }
                    case Vector3Decompression.LoadVector3sYAxis24Bit:
                        {
                            uint data = (uint)MathF.Round((translation.Y - unpackData[2]) / unpackData[0] * 0xFFFFFF);
                            handler.Write((byte)((data >> 16) & 0xFF));
                            handler.Write((byte)((data >> 8) & 0xFF));
                            handler.Write((byte)((data >> 0) & 0xFF));
                            break;
                        }
                    case Vector3Decompression.LoadVector3sZAxis24Bit:
                        {
                            uint data = (uint)MathF.Round((translation.Z - unpackData[3]) / unpackData[0] * 0xFFFFFF);
                            handler.Write((byte)((data >> 16) & 0xFF));
                            handler.Write((byte)((data >> 8) & 0xFF));
                            handler.Write((byte)((data >> 0) & 0xFF));
                            break;
                        }
                    case Vector3Decompression.LoadVector3sXYZAxis16Bit:
                        {
                            var data = (ushort)MathF.Round((translation.X - unpackData[3]) / unpackData[0] * 0xFFFF);
                            handler.Write(data);
                            break;
                        }

                    case Vector3Decompression.LoadVector3sXYAxis8Bit:
                        {
                            handler.Write((byte)MathF.Round((translation.X - unpackData[2]) / unpackData[0] * 0xFF));
                            handler.Write((byte)MathF.Round((translation.Y - unpackData[3]) / unpackData[1] * 0xFF));
                            break;
                        }
                    case Vector3Decompression.LoadVector3sXZAxis8Bit:
                        {
                            handler.Write((byte)MathF.Round((translation.X - unpackData[2]) / unpackData[0] * 0xFF));
                            handler.Write((byte)MathF.Round((translation.Z - unpackData[4]) / unpackData[1] * 0xFF));
                            break;
                        }
                    case Vector3Decompression.LoadVector3sYZAxis8Bit:
                        {
                            handler.Write((byte)MathF.Round((translation.Y - unpackData[3]) / unpackData[0] * 0xFF));
                            handler.Write((byte)MathF.Round((translation.Z - unpackData[4]) / unpackData[1] * 0xFF));
                            break;
                        }
                    case Vector3Decompression.LoadVector3sYZAxis12Bit:
                        {
                            uint data = (
                                ((uint)MathF.Round((translation.Y - unpackData[3]) / unpackData[0] * 0xFFF) << 00) |
                                ((uint)MathF.Round((translation.Z - unpackData[4]) / unpackData[1] * 0xFFF) << 12)
                            );
                            WriteUnit64AsBytes(handler, data, 3);
                            break;
                        }
                    case Vector3Decompression.LoadVector3sXZAxis12Bit:
                        {
                            uint data = (
                                ((uint)MathF.Round((translation.X - unpackData[2]) / unpackData[0] * 0xFFF) << 00) |
                                ((uint)MathF.Round((translation.Z - unpackData[4]) / unpackData[1] * 0xFFF) << 12)
                            );
                            WriteUnit64AsBytes(handler, data, 3);
                            break;
                        }
                    case Vector3Decompression.LoadVector3sXYAxis12Bit:
                        {
                            uint data = (
                                ((uint)MathF.Round((translation.X - unpackData[2]) / unpackData[0] * 0xFFF) << 00) |
                                ((uint)MathF.Round((translation.Y - unpackData[3]) / unpackData[1] * 0xFFF) << 12)
                            );
                            WriteUnit64AsBytes(handler, data, 3);
                            break;
                        }
                    case Vector3Decompression.LoadVector3sXYAxis16Bit:
                        {
                            handler.Write((ushort)MathF.Round((translation.X - unpackData[2]) / unpackData[0] * 0xFFFF));
                            handler.Write((ushort)MathF.Round((translation.Y - unpackData[3]) / unpackData[1] * 0xFFFF));
                            break;
                        }
                    case Vector3Decompression.LoadVector3sXZAxis16Bit:
                        {
                            handler.Write((ushort)MathF.Round((translation.X - unpackData[2]) / unpackData[0] * 0xFFFF));
                            handler.Write((ushort)MathF.Round((translation.Z - unpackData[4]) / unpackData[1] * 0xFFFF));
                            break;
                        }
                    case Vector3Decompression.LoadVector3sYZAxis16Bit:
                        {
                            handler.Write((ushort)MathF.Round((translation.Y - unpackData[3]) / unpackData[0] * 0xFFFF));
                            handler.Write((ushort)MathF.Round((translation.Z - unpackData[4]) / unpackData[1] * 0xFFFF));
                            break;
                        }
                    case Vector3Decompression.LoadVector3sXYAxis20Bit:
                        {
                            ulong data = (
                                ((ulong)MathF.Round((translation.X - unpackData[2]) / unpackData[0] * 0xFFFFF) << 00) |
                                ((ulong)MathF.Round((translation.Y - unpackData[3]) / unpackData[1] * 0xFFFFF) << 20)
                            );
                            WriteUnit64AsBytes(handler, data, 5);
                            break;
                        }
                    case Vector3Decompression.LoadVector3sYZAxis20Bit:
                        {
                            ulong data = (
                                ((ulong)MathF.Round((translation.Y - unpackData[3]) / unpackData[0] * 0xFFFFF) << 00) |
                                ((ulong)MathF.Round((translation.Z - unpackData[4]) / unpackData[1] * 0xFFFFF) << 20)
                            );
                            WriteUnit64AsBytes(handler, data, 5);
                            break;
                        }
                    case Vector3Decompression.LoadVector3sZXAxis20Bit:
                        {
                            ulong data = (
                                ((ulong)MathF.Round((translation.Z - unpackData[4]) / unpackData[0] * 0xFFFFF) << 00) |
                                ((ulong)MathF.Round((translation.X - unpackData[2]) / unpackData[1] * 0xFFFFF) << 20)
                            );
                            WriteUnit64AsBytes(handler, data, 5);
                            break;
                        }
                    case Vector3Decompression.LoadVector3sYZAxis24Bit:
                        {
                            ulong data = (
                                ((ulong)MathF.Round((translation.Y - unpackData[3]) / unpackData[0] * 0xFFFFFF) << 00) |
                                ((ulong)MathF.Round((translation.Z - unpackData[4]) / unpackData[1] * 0xFFFFFF) << 24)
                            );
                            WriteUnit64AsBytes(handler, data, 6);
                            break;
                        }
                    case Vector3Decompression.LoadVector3sZXAxis24Bit:
                        {
                            ulong data = (
                                ((ulong)MathF.Round((translation.Z - unpackData[4]) / unpackData[0] * 0xFFFFFF) << 00) |
                                ((ulong)MathF.Round((translation.X - unpackData[2]) / unpackData[1] * 0xFFFFFF) << 24)
                            );
                            WriteUnit64AsBytes(handler, data, 6);
                            break;
                        }
                    case Vector3Decompression.LoadVector3sXYZAxis:
                        {
                            handler.Write(translation.X);
                            break;
                        }
                    default:
                        throw new InvalidOperationException($"Unknown Vector3 compression type {type} ({Compression.ToString("X")})");
                }
            }
        }

        private static float ComputeQuaternionW(Vector3 v) => MathF.Sqrt(1 - v.LengthSquared());
        private static float ComputeQuaternionW(Quaternion v) => MathF.Sqrt(1 - v.X * v.X - v.Y * v.Y - v.Z * v.Z);

        public void ReadFrameDataRotation(FileHandler handler)
        {
            using var defer = handler.SeekJumpBack(frameDataOffset);
            QuaternionDecompression type = RotationCompressionType;
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
                            quaternion = new(vector, ComputeQuaternionW(vector));
                            break;
                        }
                    case QuaternionDecompression.LoadQuaternions5Bit:
                        {
                            var data = handler.Read<ushort>();
                            quaternion.X = (unpackData[0] * ((data >> 00) & 0x1F) * (1f / 0x1F)) + unpackData[4];
                            quaternion.Y = (unpackData[1] * ((data >> 05) & 0x1F) * (1f / 0x1F)) + unpackData[5];
                            quaternion.Z = (unpackData[2] * ((data >> 10) & 0x1F) * (1f / 0x1F)) + unpackData[6];
                            quaternion.W = ComputeQuaternionW(quaternion);
                            break;
                        }
                    case QuaternionDecompression.LoadQuaternions8Bit:
                        {
                            quaternion.X = (unpackData[0] * (handler.ReadByte() * (1f / 0xFF))) + unpackData[4];
                            quaternion.Y = (unpackData[1] * (handler.ReadByte() * (1f / 0xFF))) + unpackData[5];
                            quaternion.Z = (unpackData[2] * (handler.ReadByte() * (1f / 0xFF))) + unpackData[6];
                            quaternion.W = ComputeQuaternionW(quaternion);
                            break;
                        }
                    case QuaternionDecompression.LoadQuaternions10Bit:
                        {
                            var data = handler.Read<uint>();
                            quaternion.X = (unpackData[0] * ((data >> 00) & 0x3FF) * (1f / 0x3FF)) + unpackData[4];
                            quaternion.Y = (unpackData[1] * ((data >> 10) & 0x3FF) * (1f / 0x3FF)) + unpackData[5];
                            quaternion.Z = (unpackData[2] * ((data >> 20) & 0x3FF) * (1f / 0x3FF)) + unpackData[6];
                            quaternion.W = ComputeQuaternionW(quaternion);
                            break;
                        }
                    case QuaternionDecompression.LoadQuaternions13Bit:
                        {
                            var val = ReadBytesAsUInt64(handler, 5);
                            quaternion.X = (unpackData[0] * ((val >> 00) & 0x1FFF) / (float)0x1FFF) + unpackData[4];
                            quaternion.Y = (unpackData[1] * ((val >> 13) & 0x1FFF) / (float)0x1FFF) + unpackData[5];
                            quaternion.Z = (unpackData[2] * ((val >> 26) & 0x1FFF) / (float)0x1FFF) + unpackData[6];
                            quaternion.W = ComputeQuaternionW(quaternion);
                            break;
                        }
                    case QuaternionDecompression.LoadQuaternions16Bit:
                        {
                            if (MotVersion >= MotVersion.Pragmata)
                            {
                                // byte order changed because why tf not (see: ch5800_attack.motlist.1057 1020_Flare)
                                var data = ReadBytesAsUInt64(handler, 6);
                                quaternion.X = (unpackData[0] * ((data >> 00) & 0xFFFF) / (float)0xFFFF) + unpackData[4];
                                quaternion.Y = (unpackData[1] * ((data >> 16) & 0xFFFF) / (float)0xFFFF) + unpackData[5];
                                quaternion.Z = (unpackData[2] * ((data >> 32) & 0xFFFF) / (float)0xFFFF) + unpackData[6];
                            }
                            else
                            {
                                var data = handler.Read<PackedVector3>();
                                quaternion.X = (unpackData[0] * (data.X / (float)0xFFFF)) + unpackData[4];
                                quaternion.Y = (unpackData[1] * (data.Y / (float)0xFFFF)) + unpackData[5];
                                quaternion.Z = (unpackData[2] * (data.Z / (float)0xFFFF)) + unpackData[6];
                            }
                            quaternion.W = ComputeQuaternionW(quaternion);
                            break;
                        }
                    case QuaternionDecompression.LoadQuaternions18Bit:
                        {
                            var val = ReadBytesAsUInt64(handler, 7);
                            quaternion.X = (unpackData[0] * ((val >> 00) & 0x3FFFF) / (float)0x3FFFF) + unpackData[4];
                            quaternion.Y = (unpackData[1] * ((val >> 18) & 0x3FFFF) / (float)0x3FFFF) + unpackData[5];
                            quaternion.Z = (unpackData[2] * ((val >> 36) & 0x3FFFF) / (float)0x3FFFF) + unpackData[6];
                            quaternion.W = ComputeQuaternionW(quaternion);
                            break;
                        }
                    case QuaternionDecompression.LoadQuaternions21Bit:
                        {
                            var data = handler.Read<ulong>();
                            quaternion.X = (unpackData[0] * ((data >> 00) & 0x1F_FFFF) / (float)0x1F_FFFF) + unpackData[4];
                            quaternion.Y = (unpackData[1] * ((data >> 21) & 0x1F_FFFF) / (float)0x1F_FFFF) + unpackData[5];
                            quaternion.Z = (unpackData[2] * ((data >> 42) & 0x1F_FFFF) / (float)0x1F_FFFF) + unpackData[6];
                            quaternion.W = ComputeQuaternionW(quaternion);
                            break;
                        }
                    case QuaternionDecompression.LoadQuaternionsXAxis16Bit:
                        quaternion.X = unpackData[0] * ((float)handler.Read<ushort>() * (1f / 0xFFFF)) + unpackData[1];
                        quaternion.Y = 0.0f;
                        quaternion.Z = 0.0f;
                        quaternion.W = ComputeQuaternionW(quaternion);
                        break;
                    case QuaternionDecompression.LoadQuaternionsYAxis16Bit:
                        quaternion.X = 0.0f;
                        quaternion.Y = unpackData[0] * ((float)handler.Read<ushort>() * (1f / 0xFFFF)) + unpackData[1];
                        quaternion.Z = 0.0f;
                        quaternion.W = ComputeQuaternionW(quaternion);
                        break;
                    case QuaternionDecompression.LoadQuaternionsZAxis16Bit:
                        quaternion.X = 0.0f;
                        quaternion.Y = 0.0f;
                        quaternion.Z = unpackData[0] * ((float)handler.Read<ushort>() * (1f / 0xFFFF)) + unpackData[1];
                        quaternion.W = ComputeQuaternionW(quaternion);
                        break;
                    case QuaternionDecompression.LoadQuaternionsXAxis24Bit:
                        {
                            var val = ReadBytesAsUInt32(handler, 3);
                            quaternion.X = unpackData[0] * ((float)val * (1f / 0xFFFFFF)) + unpackData[1];
                            quaternion.Y = 0.0f;
                            quaternion.Z = 0.0f;
                            quaternion.W = ComputeQuaternionW(quaternion);
                            break;
                        }
                    case QuaternionDecompression.LoadQuaternionsYAxis24Bit:
                        {
                            var val = ReadBytesAsUInt32(handler, 3);
                            quaternion.X = 0.0f;
                            quaternion.Y = unpackData[0] * ((float)val * (1f / 0xFFFFFF)) + unpackData[1];
                            quaternion.Z = 0.0f;
                            quaternion.W = ComputeQuaternionW(quaternion);
                            break;
                        }
                    case QuaternionDecompression.LoadQuaternionsZAxis24Bit:
                        {
                            var val = ReadBytesAsUInt32(handler, 3);
                            quaternion.X = 0.0f;
                            quaternion.Y = 0.0f;
                            quaternion.Z = unpackData[0] * ((float)val * (1f / 0xFFFFFF)) + unpackData[1];
                            quaternion.W = ComputeQuaternionW(quaternion);
                            break;
                        }
                    case QuaternionDecompression.LoadQuaternionsXAxis:
                        quaternion.X = handler.Read<float>();
                        quaternion.Y = 0.0f;
                        quaternion.Z = 0.0f;
                        quaternion.W = ComputeQuaternionW(quaternion);
                        break;
                    case QuaternionDecompression.LoadQuaternionsYAxis:
                        quaternion.X = 0.0f;
                        quaternion.Y = handler.Read<float>();
                        quaternion.Z = 0.0f;
                        quaternion.W = ComputeQuaternionW(quaternion);
                        break;
                    case QuaternionDecompression.LoadQuaternionsZAxis:
                        quaternion.X = 0.0f;
                        quaternion.Y = 0.0f;
                        quaternion.Z = handler.Read<float>();
                        quaternion.W = ComputeQuaternionW(quaternion);
                        break;
                    default:
                        throw new InvalidOperationException($"Unknown Quaternion compression type {type} ({Compression.ToString("X")})");
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
            RecomputeDenormalizationParams();
            QuaternionDecompression type = RotationCompressionType;
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
                                (ushort)MathF.Round((quaternion.X - unpackData[4]) / unpackData[0] * 0b11111) << 00 |
                                (ushort)MathF.Round((quaternion.Y - unpackData[5]) / unpackData[1] * 0b11111) << 05 |
                                (ushort)MathF.Round((quaternion.Z - unpackData[6]) / unpackData[2] * 0b11111) << 10);
                            handler.Write(data);
                            break;
                        }
                    case QuaternionDecompression.LoadQuaternions8Bit:
                        {
                            byte x = (byte)MathF.Round((quaternion.X - unpackData[4]) / unpackData[0] * 0xFF);
                            byte y = (byte)MathF.Round((quaternion.Y - unpackData[5]) / unpackData[1] * 0xFF);
                            byte z = (byte)MathF.Round((quaternion.Z - unpackData[6]) / unpackData[2] * 0xFF);
                            handler.Write(x);
                            handler.Write(y);
                            handler.Write(z);
                            break;
                        }
                    case QuaternionDecompression.LoadQuaternions10Bit:
                        {
                            uint data =
                                (((uint)MathF.Round((quaternion.X - unpackData[4]) / unpackData[0] * 0x3FF) & 0x3FF) << 00) |
                                (((uint)MathF.Round((quaternion.Y - unpackData[5]) / unpackData[1] * 0x3FF) & 0x3FF) << 10) |
                                (((uint)MathF.Round((quaternion.Z - unpackData[6]) / unpackData[2] * 0x3FF) & 0x3FF) << 20);
                            handler.Write(data);
                            break;
                        }
                    case QuaternionDecompression.LoadQuaternions13Bit:
                        {
                            ulong val =
                                (((ulong)MathF.Round((quaternion.X - unpackData[4]) / unpackData[0] * 0x1FFF) & 0x1FFF) << 00) |
                                (((ulong)MathF.Round((quaternion.Y - unpackData[5]) / unpackData[1] * 0x1FFF) & 0x1FFF) << 13) |
                                (((ulong)MathF.Round((quaternion.Z - unpackData[6]) / unpackData[2] * 0x1FFF) & 0x1FFF) << 26);
                            Span<byte> data = stackalloc byte[5];
                            for (int j = 0; j < 5; j++)
                            {
                                data[4 - j] = (byte)(val & 0xFF);
                                val >>= 8;
                            }
                            handler.WriteSpan<byte>(data);
                            break;
                        }
                    case QuaternionDecompression.LoadQuaternions16Bit:
                        {
                            var data = new PackedVector3(
                                (ushort)MathF.Round((quaternion.X - unpackData[4]) / unpackData[0] * 0xFFFF),
                                (ushort)MathF.Round((quaternion.Y - unpackData[5]) / unpackData[1] * 0xFFFF),
                                (ushort)MathF.Round((quaternion.Z - unpackData[6]) / unpackData[2] * 0xFFFF)
                            );
                            if (MotVersion >= MotVersion.Pragmata)
                            {
                                // need to write in the right byte order
                                handler.Write((byte)((data.Z >> 8) & 0xFF)); handler.Write((byte)data.Z);
                                handler.Write((byte)((data.Y >> 8) & 0xFF)); handler.Write((byte)data.Y);
                                handler.Write((byte)((data.X >> 8) & 0xFF)); handler.Write((byte)data.X);
                            }
                            else
                            {
                                handler.Write(data);
                            }
                            break;
                        }
                    case QuaternionDecompression.LoadQuaternions18Bit:
                        {
                            ulong val =
                                (((ulong)MathF.Round((quaternion.X - unpackData[4]) / unpackData[0] * 0x3FFFF) & 0x3FFFF) << 00) |
                                (((ulong)MathF.Round((quaternion.Y - unpackData[5]) / unpackData[1] * 0x3FFFF) & 0x3FFFF) << 18) |
                                (((ulong)MathF.Round((quaternion.Z - unpackData[6]) / unpackData[2] * 0x3FFFF) & 0x3FFFF) << 36);

                            Span<byte> data = stackalloc byte[7];
                            for (int j = 0; j < 7; j++)
                            {
                                data[6 - j] = (byte)(val & 0xFF);
                                val >>= 8;
                            }
                            handler.WriteSpan<byte>(data);
                            break;
                        }
                    case QuaternionDecompression.LoadQuaternions21Bit:
                        {
                            ulong val = (((ulong)MathF.Round((quaternion.X - unpackData[4]) / unpackData[0] * 0x1F_FFFF) & 0x1F_FFFF) << 00) |
                                        (((ulong)MathF.Round((quaternion.Y - unpackData[5]) / unpackData[1] * 0x1F_FFFF) & 0x1F_FFFF) << 21) |
                                        (((ulong)MathF.Round((quaternion.Z - unpackData[6]) / unpackData[2] * 0x1F_FFFF) & 0x1F_FFFF) << 42);
                            handler.Write(val);
                            break;
                        }
                    case QuaternionDecompression.LoadQuaternionsXAxis16Bit:
                        {
                            ushort data = (ushort)MathF.Round((quaternion.X - unpackData[1]) / unpackData[0] * 0xFFFF);
                            handler.Write(data);
                            break;
                        }
                    case QuaternionDecompression.LoadQuaternionsYAxis16Bit:
                        {
                            ushort data = (ushort)MathF.Round((quaternion.Y - unpackData[1]) / unpackData[0] * 0xFFFF);
                            handler.Write(data);
                            break;
                        }
                    case QuaternionDecompression.LoadQuaternionsZAxis16Bit:
                        {
                            ushort data = (ushort)MathF.Round((quaternion.Z - unpackData[1]) / unpackData[0] * 0xFFFF);
                            handler.Write(data);
                            break;
                        }
                    case QuaternionDecompression.LoadQuaternionsXAxis24Bit:
                        {
                            WriteUnit64AsBytes(handler, (uint)MathF.Round((quaternion.X - unpackData[1]) / unpackData[0] * 0xFFFFFF), 3);
                            break;
                        }
                    case QuaternionDecompression.LoadQuaternionsYAxis24Bit:
                        {
                            WriteUnit64AsBytes(handler, (uint)MathF.Round((quaternion.Y - unpackData[1]) / unpackData[0] * 0xFFFFFF), 3);
                            break;
                        }
                    case QuaternionDecompression.LoadQuaternionsZAxis24Bit:
                        {
                            WriteUnit64AsBytes(handler, (uint)MathF.Round((quaternion.Z - unpackData[1]) / unpackData[0] * 0xFFFFFF), 3);
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
                        throw new InvalidOperationException($"Unknown Quaternion compression type {type} ({Compression.ToString("X")})");
                }
            }
        }

        public void ReadFrameDataFloats(FileHandler handler)
        {
            using var defer = handler.SeekJumpBack(frameDataOffset);
            FloatDecompression type = FloatCompressionType;
            floats = new float[keyCount];

            for (int i = 0; i < keyCount; i++)
            {
                float value = 0;
                switch (type)
                {
                    case FloatDecompression.LoadFloatsFull:
                        handler.Read(ref value);
                        break;
                    case FloatDecompression.LoadFloats8Bit:
                        value = unpackData[0] * ((float)handler.Read<byte>() * (1f / 0xFF)) + unpackData[1];
                        break;

                    default:
                        throw new NotSupportedException("Unsupported float compression type " + type);
                }
                floats[i] = value;
            }
        }

        public void WriteFrameDataFloats(FileHandler handler)
        {
            if (floats == null) throw new NullReferenceException($"{nameof(floats)} is null");
            var type = FloatCompressionType;
            RecomputeDenormalizationParams();
            for (int i = 0; i < keyCount; i++)
            {
                var value = floats[i];
                switch (type)
                {
                    case FloatDecompression.LoadFloatsFull:
                        handler.Write(value);
                        break;
                    case FloatDecompression.LoadFloats8Bit:
                        handler.Write((byte)MathF.Round((value - unpackData[1]) / unpackData[0] * 0xFF));
                        break;
                    default:
                        throw new InvalidOperationException($"Unknown float compression type {type} ({Compression.ToString("X")})");
                }
            }
        }

        internal void RecomputeDenormalizationParams()
        {
            if (!RequiresUnpackData) return;

            // NOTE: basegame includes data in all 8 fields, not just the ones used for decompression
            // probably just a side effect of their serializer and not actually meaningful?
            Vector3 max = new Vector3(float.MinValue);
            Vector3 min = new Vector3(float.MaxValue);
            if (TrackType == TrackValueType.Quaternion)
            {
                foreach (Quaternion q in rotations!) {
                    max = Vector3.Max(max, new Vector3(q.X, q.Y, q.Z));
                    min = Vector3.Min(min, new Vector3(q.X, q.Y, q.Z));
                }
                var scale = max - min;
                switch (RotationCompressionType)
                {
                    case QuaternionDecompression.LoadQuaternions5Bit:
                    case QuaternionDecompression.LoadQuaternions8Bit:
                    case QuaternionDecompression.LoadQuaternions10Bit:
                    case QuaternionDecompression.LoadQuaternions13Bit:
                    case QuaternionDecompression.LoadQuaternions16Bit:
                    case QuaternionDecompression.LoadQuaternions18Bit:
                    case QuaternionDecompression.LoadQuaternions21Bit:
                        unpackData[0] = scale.X;
                        unpackData[1] = scale.Y;
                        unpackData[2] = scale.Z;
                        unpackData[4] = min.X;
                        unpackData[5] = min.Y;
                        unpackData[6] = min.Z;
                        break;
                    case QuaternionDecompression.LoadQuaternionsXAxis16Bit:
                    case QuaternionDecompression.LoadQuaternionsYAxis16Bit:
                    case QuaternionDecompression.LoadQuaternionsZAxis16Bit:
                    case QuaternionDecompression.LoadQuaternionsXAxis24Bit:
                    case QuaternionDecompression.LoadQuaternionsYAxis24Bit:
                    case QuaternionDecompression.LoadQuaternionsZAxis24Bit:
                        unpackData[0] = RotationCompressionType switch {
                            QuaternionDecompression.LoadQuaternionsYAxis16Bit => scale.Y,
                            QuaternionDecompression.LoadQuaternionsZAxis16Bit => scale.Z,
                            QuaternionDecompression.LoadQuaternionsYAxis24Bit => scale.Y,
                            QuaternionDecompression.LoadQuaternionsZAxis24Bit => scale.Z,
                            _ => scale.X,
                        };
                        unpackData[1] = RotationCompressionType switch {
                            QuaternionDecompression.LoadQuaternionsYAxis16Bit => min.Y,
                            QuaternionDecompression.LoadQuaternionsZAxis16Bit => min.Z,
                            QuaternionDecompression.LoadQuaternionsYAxis24Bit => min.Y,
                            QuaternionDecompression.LoadQuaternionsZAxis24Bit => min.Z,
                            _ => min.X,
                        };
                        break;
                }
            }
            else if (TrackType == TrackValueType.Vector3)
            {
                foreach (Vector3 vec in translations!) {
                    max = Vector3.Max(max, vec);
                    min = Vector3.Min(min, vec);
                }
                var scale = max - min;
                switch (TranslationCompressionType)
                {
                    case Vector3Decompression.LoadVector3s5BitA:
                    case Vector3Decompression.LoadVector3s10BitA:
                    case Vector3Decompression.LoadVector3s21BitA:
                        unpackData[0] = scale.X;
                        unpackData[1] = scale.Y;
                        unpackData[2] = scale.Z;
                        unpackData[4] = min.X;
                        unpackData[5] = min.Y;
                        unpackData[6] = min.Z;
                        break;
                    case Vector3Decompression.LoadVector3s5BitB:
                    case Vector3Decompression.LoadVector3s8BitB:
                    case Vector3Decompression.LoadVector3s10BitB:
                    case Vector3Decompression.LoadVector3s13Bit:
                    case Vector3Decompression.LoadVector3s16Bit:
                    case Vector3Decompression.LoadVector3s18Bit:
                    case Vector3Decompression.LoadVector3s21BitB:
                        unpackData[0] = scale.X;
                        unpackData[1] = scale.Y;
                        unpackData[2] = scale.Z;
                        unpackData[3] = min.X;
                        unpackData[4] = min.Y;
                        unpackData[5] = min.Z;
                        break;
                    case Vector3Decompression.LoadVector3sXAxis16Bit:
                    case Vector3Decompression.LoadVector3sYAxis16Bit:
                    case Vector3Decompression.LoadVector3sZAxis16Bit:
                    case Vector3Decompression.LoadVector3sXAxis24Bit:
                    case Vector3Decompression.LoadVector3sYAxis24Bit:
                    case Vector3Decompression.LoadVector3sZAxis24Bit:
                        unpackData[0] = TranslationCompressionType switch {
                            Vector3Decompression.LoadVector3sYAxis16Bit => scale.Y,
                            Vector3Decompression.LoadVector3sYAxis24Bit => scale.Y,
                            Vector3Decompression.LoadVector3sZAxis16Bit => scale.Z,
                            Vector3Decompression.LoadVector3sZAxis24Bit => scale.Z,
                            _ => scale.X,
                        };
                        SetVector(unpackData, 1, min);
                        break;
                    case Vector3Decompression.LoadVector3sXAxis:
                    case Vector3Decompression.LoadVector3sYAxis:
                    case Vector3Decompression.LoadVector3sZAxis:
                    case Vector3Decompression.LoadVector3sZXAxis:
                    case Vector3Decompression.LoadVector3sXYAxis:
                    case Vector3Decompression.LoadVector3sYZAxis:
                        SetVector(unpackData, 0, min);
                        break;
                    case Vector3Decompression.LoadVector3sXYAxis8Bit:
                    case Vector3Decompression.LoadVector3sXYAxis12Bit:
                    case Vector3Decompression.LoadVector3sXYAxis16Bit:
                    case Vector3Decompression.LoadVector3sXYAxis20Bit:
                    case Vector3Decompression.LoadVector3sXYAxis24Bit:
                        unpackData[0] = scale.X; unpackData[1] = scale.Y;
                        SetVector(unpackData, 2, min);
                        break;
                    case Vector3Decompression.LoadVector3sXZAxis8Bit:
                    case Vector3Decompression.LoadVector3sXZAxis12Bit:
                    case Vector3Decompression.LoadVector3sXZAxis16Bit:
                        unpackData[0] = scale.X; unpackData[1] = scale.Z;
                        SetVector(unpackData, 2, min);
                        break;
                    case Vector3Decompression.LoadVector3sZXAxis20Bit:
                        unpackData[0] = scale.Z; unpackData[1] = scale.X;
                        SetVector(unpackData, 2, min);
                        break;
                    case Vector3Decompression.LoadVector3sYZAxis8Bit:
                    case Vector3Decompression.LoadVector3sYZAxis12Bit:
                    case Vector3Decompression.LoadVector3sYZAxis16Bit:
                    case Vector3Decompression.LoadVector3sYZAxis20Bit:
                    case Vector3Decompression.LoadVector3sYZAxis24Bit:
                        unpackData[0] = scale.Y; unpackData[1] = scale.Z;
                        SetVector(unpackData, 2, min);
                        break;
                }
                static void SetVector(float[] bytes, int offset, Vector3 vec)
                {
                    MemoryMarshal.Cast<float, Vector3>(bytes.AsSpan(offset))[0] = vec;
                }
            }
            else if (TrackType == TrackValueType.Float)
            {
                float maxN = float.MinValue;
                float minN = float.MaxValue;
                foreach (float val in floats!) {
                    maxN = float.Max(val, maxN);
                    minN = float.Min(val, minN);
                }
                var scale = maxN - minN;
                switch (FloatCompressionType)
                {
                    case FloatDecompression.LoadFloats8Bit:
                        unpackData[0] = scale;
                        unpackData[1] = minN;
                        break;
                }
            }
        }

        public static void SetRotationW(ref Quaternion quaternion)
        {
            float w = 1.0f - (quaternion.X * quaternion.X + quaternion.Y * quaternion.Y + quaternion.Z * quaternion.Z);
            quaternion.W = w > 0.0f ? (float)Math.Sqrt(w) : 0.0f;
        }

        public override string ToString() => $"{TrackType} Track ({translations?.Length ?? rotations?.Length ?? -1})";
    }


    public class BoneMotionClip : BaseModel
    {
        public BoneClipHeader ClipHeader { get; }

        public Track? Translation { get; set; }
        public Track? Rotation { get; set; }
        public Track? Scale { get; set; }

        public bool HasTranslation => Translation != null && (ClipHeader.trackFlags & TrackFlag.Translation) != 0;
        public bool HasRotation => Rotation != null && (ClipHeader.trackFlags & TrackFlag.Rotation) != 0;
        public bool HasScale => Scale != null && (ClipHeader.trackFlags & TrackFlag.Scale) != 0;

        public BoneMotionClip(BoneClipHeader clipHeader)
        {
            ClipHeader = clipHeader;
        }

        protected override bool DoRead(FileHandler handler)
        {
            var header = ClipHeader;
            handler.Seek(header.trackHeaderOffset);
            if (header.trackFlags.HasFlag(TrackFlag.Translation))
            {
                Translation = new(header.MotVersion, TrackValueType.Vector3);
                Translation.Read(handler);
            }
            if (header.trackFlags.HasFlag(TrackFlag.Rotation))
            {
                Rotation = new(header.MotVersion, TrackValueType.Quaternion);
                Rotation.Read(handler);
            }
            if (header.trackFlags.HasFlag(TrackFlag.Scale))
            {
                Scale = new(header.MotVersion, TrackValueType.Vector3);
                Scale.Read(handler);
            }
            Translation?.ReadFrameDataTranslation(handler);
            Rotation?.ReadFrameDataRotation(handler);
            Scale?.ReadFrameDataTranslation(handler);
            // Translation?.RecomputeDenormalizationParams();
            // Rotation?.RecomputeDenormalizationParams();
            // Scale?.RecomputeDenormalizationParams();
            return true;
        }

        protected override bool DoWrite(FileHandler handler)
        {
            ClipHeader.trackHeaderOffset = handler.Tell();
            Translation?.Write(handler);
            Rotation?.Write(handler);
            Scale?.Write(handler);
            // update the clip header with our newly found offset
            ClipHeader.Write(handler, ClipHeader.Start);
            return true;
        }

        public void WriteOffsetContents(FileHandler handler)
        {
            Translation?.WriteOffsetContents(handler);
            Rotation?.WriteOffsetContents(handler);
            Scale?.WriteOffsetContents(handler);
        }

        public void ChangeVersion(MotVersion version)
        {
            ClipHeader.MotVersion = version;
            Translation?.ChangeVersion(version);
            Rotation?.ChangeVersion(version);
            Scale?.ChangeVersion(version);
        }

        public override string ToString() => $"{ClipHeader} [{(Translation == null ? "": "T")}{(Rotation == null ? "": "R")}{(Scale == null ? "": "S")}]";
    }


    public class MotClip : BaseModel
    {
        internal long clipOffset;
        internal long endClipStructsRelocation;
        public int lastTrackIndex; // 99.9% of the time equal to last track index
        public int mainTrackIndex = 1; // always equal to 1 (first non-root track)
        public byte[] uknBytes28 = new byte[28];
        public EmbeddedClip ClipEntry { get; set; } = new();
        public EndClipStruct[]? EndClipStructs { get; set; }

        public string MainTrackName
        {
            get
            {
                if (ClipEntry == null) return "";
                if (ClipEntry.Tracks.Count <= mainTrackIndex) return "";
                return ClipEntry.Tracks[mainTrackIndex].Name;
            }
        }

        protected override bool DoRead(FileHandler handler)
        {
            handler.ReadNull(8);
            handler.Read(ref clipOffset);
            handler.Read(ref endClipStructsRelocation);
            handler.ReadNull(4);
            handler.Read(ref lastTrackIndex);
            handler.Read(ref mainTrackIndex);
            handler.ReadBytes(uknBytes28);

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
            DataInterpretationException.ThrowIfDifferent(mainTrackIndex, 1);

            return true;
        }

        protected override bool DoWrite(FileHandler handler)
        {
            handler.WriteNull(8);
            handler.Write(ref clipOffset);
            handler.Write(ref endClipStructsRelocation);
            handler.WriteNull(4);
            handler.Write(ref lastTrackIndex);
            handler.Write(ref mainTrackIndex);
            handler.WriteBytes(uknBytes28);

            clipOffset = handler.Tell();
            ClipEntry.Write(handler);

            if (ClipEntry.Header.version > ClipVersion.RE7 && ClipEntry.Header.numNodes > 1)
            {
                handler.Align(16);
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

        public void ChangeVersion(ClipVersion clipVer)
        {
            ClipEntry.Version = clipVer;
            foreach (var track in ClipEntry.Tracks) track.Version = clipVer;
            foreach (var track in ClipEntry.Properties) track.Info.Version = clipVer;
            foreach (var key in ClipEntry.ClipKeys) key.Version = clipVer;
            ClipEntry.ExtraPropertyData.Version = clipVer;
        }

        public override string ToString() => $"MotClip: {MainTrackName}";
    }


    public class MotEndClipHeader(MotVersion version) : BaseModel
    {
        public byte count1;
        public byte type1;
        public byte endFlags1;
        public byte endFlags2;

        public float[]? frameValues1;
        public float[]? frameValues2;

        public MotVersion Version { get; set; } = version;

        protected override bool DoRead(FileHandler handler)
        {
            long valuesOffset = 0;
            if (Version < MotVersion.MHR_DEMO) valuesOffset = handler.Read<long>();

            handler.Read(ref count1);
            handler.Read(ref type1);
            handler.Read(ref endFlags1);
            handler.Read(ref endFlags2);

            if (valuesOffset != 0) handler.Seek(valuesOffset);
            // 4 2 0 count => 8 or 9 (ch20_000_comem.motlist.751 mot 300)
            // 1 5 0 count => 6 (ch53_000_low_com.motlist.751)
            // 1 7 0 count => 8 or 9 (ch53_000_com.motlist.751 mot 206)
            // 2 2 1 count => 5 (ch51_000_combm.motlist.751 mot 181)
            // 1 2 1 count => 3 (ch51_000_com_catch.motlist.751 mot 1)
            // 1 2 0 count => 3 (cmn_move_cnagner_stwater.motlist.85 mot 192 WTF??)
            // 2 2 0 1 => 5 (re3rt base_cmn_move.motlist.524 mot 170)
            // 2 2 1 1 => 5 (re3rt em9400_attack.motlist.524 mot 1)
            // 3 4 2 0 => 13 (re8 ch01_6000_handsrose.motlist.486 mot 2)

            // note: not sure how the counts are supposed to work, the commented out code "mostly" worked but missed a few values sometimes
            // if (type1 == 2 || type1 == 4) {
            //     frameValues1 = handler.ReadArray<float>(count1 * type1);
            // } else {
            //     frameValues1 = handler.ReadArray<float>(count1 + type1);
            // }
            // if (endFlags1 > 0 || endFlags2 > 0) {
            //     frameValues2 = handler.ReadArray<float>(1);
            // }
            // guessing based on values instead - the floats seem to be frame numbers so surely they'll be within a reasonable range
            var floats = new List<float>();
            float flt = 0;
            while (handler.Read(ref flt) != 0 && (flt == 0 || flt >= 1 && flt < 3000)) {
                floats.Add(flt);
            }
            frameValues1 = floats.ToArray();
            return true;
        }

        protected override bool DoWrite(FileHandler handler)
        {
            if (Version < MotVersion.MHR_DEMO)
            {
                handler.Skip(8);
            }
            handler.Write(ref count1);
            handler.Write(ref type1);
            handler.Write(ref endFlags1);
            handler.Write(ref endFlags2);

            if (Version < MotVersion.MHR_DEMO)
            {
                handler.Align(8);
                handler.Write(Start, handler.Tell());
            }
            if (frameValues1 != null) handler.WriteArray<float>(frameValues1);
            if (frameValues2 != null) handler.WriteArray<float>(frameValues2);

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

    public class MotPropertyTrack
    {
        public uint propertyHash;
        public Track? Track;

        public MotVersion Version;

        internal const int HeaderStructSize = 16;

        public bool Read(FileHandler handler)
        {
            var dataOffset = handler.Read<long>();
            handler.Read(ref propertyHash);
            handler.ReadNull(4);

            var pos = handler.Tell();
            var trackType = (TrackValueType)handler.Read<short>(dataOffset);
            DataInterpretationException.ThrowIf(trackType != TrackValueType.Float);

            Track = new Track(Version, trackType);
            handler.Seek(dataOffset);
            Track.Read(handler);
            Track.ReadFrameDataFloats(handler);

            handler.Seek(pos);
            return true;
        }

        public void Write(FileHandler handler, ref long dataOffset)
        {
            handler.Write(dataOffset);
            handler.Write(ref propertyHash);
            handler.WriteNull(4);

            var headerEnd = handler.Tell();

            if (Track == null) throw new NullReferenceException();
            handler.Seek(dataOffset);
            Track.Write(handler);
            Track.WriteOffsetContents(handler);

            dataOffset = handler.Tell();
            handler.Seek(headerEnd);
        }

        public override string ToString() => $"Property {propertyHash} {Track?.TrackType} ({Track?.floats?.Length})";
    }

    public class MotPropertyTree : BaseModel
    {
        public List<InnerNode> Nodes { get; } = new();
        public List<LeafNodeRemap> HashMapping { get; } = new();

        public class InnerNode
        {
            public int count1;
            public int count2;
            public uint nameHash;
            public uint nameHash2;

            public List<SingleValueNode> SimpleValues { get; } = new();
            public List<MultiValueNode> MultiValues { get; } = new();

            public override string ToString() => $"{nameHash}/{nameHash2}";
        }

        public class SingleValueNode
        {
            public uint propertyHash;
            public byte valueType;
            public byte valueCount;
            public object? value;

            public override string ToString() => $"{propertyHash} => {value}";
        }

        public class MultiValueNode
        {
            public uint propertyHash;
            public int valueCount;
            public int zero;
            public int valueType;
            public object? value;

            public override string ToString() => $"{propertyHash} ({valueCount})";
        }

        public struct LeafNodeRemap
        {
            public uint keyHash;
            public uint nodePropertyHash;

            public override string ToString() => $"{keyHash} => {nodePropertyHash}";
        }

        protected override bool DoRead(FileHandler handler)
        {
            var innerNodeOffset = handler.Read<long>();
            var remapsOffset = handler.Read<long>();
            var innerCount = handler.Read<int>();
            var leafCount = handler.Read<int>();
            handler.ReadNull(8);
            handler.Seek(innerNodeOffset);
            for (int i = 0; i < innerCount; ++i)
            {
                var node = new InnerNode();
                var leftOffset = handler.Read<long>();
                var rightOffset = handler.Read<long>();

                handler.Read(ref node.count1);
                handler.Read(ref node.count2);
                handler.Read(ref node.nameHash);
                handler.Read(ref node.nameHash2);
                var end = handler.Tell();

                handler.Seek(leftOffset);
                for (int n = 0; n < node.count1; n++)
                {
                    var child = new SingleValueNode();
                    var valueOffs = handler.Read<long>();
                    handler.Read(ref child.propertyHash);
                    handler.Read(ref child.valueType);
                    handler.Read(ref child.valueCount);
                    handler.ReadNull(2);
                    var pos = handler.Tell();
                    child.value = ReadValue(handler, valueOffs, child.valueType, child.valueCount);
                    node.SimpleValues.Add(child);
                    handler.Seek(pos);
                }

                handler.Seek(rightOffset);
                for (int n = 0; n < node.count2; n++)
                {
                    var child = new MultiValueNode();
                    var valueOffs = handler.Read<long>();
                    handler.Read(ref child.propertyHash);
                    handler.Read(ref child.valueCount);
                    handler.Read(ref child.zero);
                    handler.Read(ref child.valueType);
                    if (child.valueCount > 0)
                    {
                        var pos = handler.Tell();
                        child.value = ReadValue(handler, valueOffs, (byte)child.valueType, child.valueCount);
                        handler.Seek(pos);
                    }
                    node.MultiValues.Add(child);
                }

                Nodes.Add(node);
                handler.Seek(end);
            }

            handler.Seek(remapsOffset);
            HashMapping.ReadStructList(handler, leafCount);
            return true;
        }

        protected override bool DoWrite(FileHandler handler)
        {
            handler.Write(32L);
            handler.Skip(8);
            handler.Write(Nodes.Count);
            handler.Write(HashMapping.Count);
            handler.WriteNull(8);

            var nodesOffset = handler.Tell() + Nodes.Count * 32;
            var dataOffset = nodesOffset + Nodes.Sum(n => n.SimpleValues.Count * 16 + n.MultiValues.Count * 24 + (n.MultiValues.Count % 2 == 0 ? 0 : 8));
            foreach (var inner in Nodes)
            {
                nodesOffset = Utils.Align16((int)nodesOffset);
                var start = handler.Tell();
                handler.Write(nodesOffset);
                handler.Skip(8);
                handler.Write(ref inner.count1);
                handler.Write(ref inner.count2);
                handler.Write(ref inner.nameHash);
                handler.Write(ref inner.nameHash2);
                var nodeEnd = handler.Tell();
                handler.Seek(nodesOffset);
                dataOffset = Utils.Align16((int)dataOffset);
                foreach (var left in inner.SimpleValues)
                {
                    WriteValue(handler, ref dataOffset, left.valueType, left.valueCount, left.value);
                    handler.Write(left.propertyHash);
                    handler.Write(left.valueType);
                    handler.Write(left.valueCount);
                    handler.WriteNull(2);
                    nodesOffset += 16;
                }

                handler.Write(start + 8, nodesOffset);
                foreach (var multi in inner.MultiValues)
                {
                    dataOffset = Utils.Align16((int)dataOffset);
                    if (multi.valueCount > 0)
                    {
                        WriteValue(handler, ref dataOffset, (byte)multi.valueType, multi.valueCount, multi.value);
                    }
                    else
                    {
                        handler.Write(dataOffset);
                    }
                    handler.Write(ref multi.propertyHash);
                    handler.Write(ref multi.valueCount);
                    handler.Write(ref multi.zero);
                    handler.Write(ref multi.valueType);
                    nodesOffset += 24;
                }
                handler.Seek(nodeEnd);
            }

            handler.Seek(dataOffset);
            handler.Align(4);
            handler.StringTableFlush();
            handler.Align(16);
            handler.Write(8, handler.Tell());
            HashMapping.Write(handler);
            return true;
        }

        private static object ReadValue(FileHandler handler, long offsetOrConstant, byte valueType, int valueCount)
        {
            if (valueCount == 0)
            {
                // NOTE: types look similar to via.timeline.PropertyType but not quite identical here either
                return valueType switch {
                    1 => (object)(offsetOrConstant > 1 || offsetOrConstant < 0 ? throw new DataInterpretationException("Assumed bool was not a bool") : offsetOrConstant != 0),
                    2 => (object)(byte)offsetOrConstant,
                    7 => (object)(int)offsetOrConstant,
                    8 => (uint)offsetOrConstant,
                    11 => (float)offsetOrConstant,
                    15 => handler.ReadWString(offsetOrConstant),
                    _ => throw new NotImplementedException(),
                };
            }

            handler.Seek(offsetOrConstant);
            switch (valueType)
            {
                case 7:
                    return valueCount switch {
                        1 => handler.Read<int>(),
                        _ => handler.ReadArray<int>(valueCount),
                    };
                case 8:
                    return valueCount switch {
                        1 => handler.Read<uint>(),
                        _ => handler.ReadArray<uint>(valueCount),
                    };
                case 10:
                    return valueCount switch {
                        1 => handler.Read<FrameValue>(),
                        _ => handler.ReadArray<FrameValue>(valueCount),
                    };
                case 15:
                    {
                        DataInterpretationException.DebugThrowIf(true, "Verify correctness pls");
                        var strings = new string[valueCount];
                        for (int i = 0; i < valueCount; ++i) strings[i] = handler.ReadOffsetWString();
                        return strings;
                    }
                case 17:
                    return valueCount switch {
                        1 => handler.ReadFloat(),
                        4 => handler.Read<Quaternion>(),
                        _ => handler.ReadArray<float>(valueCount),
                    };
                default:
                    throw new NotImplementedException("Unsupported mot property tree value type " + valueType);
            }
        }

        private static void WriteValue(FileHandler handler, ref long dataOffset, byte valueType, int valueCount, object? value)
        {
            if (valueCount == 0)
            {
                if (value == null) throw new NotImplementedException();
                switch (valueType) {
                    case 1: handler.Write((bool)value); handler.WriteNull(7); break;
                    case 2: handler.Write((byte)value); handler.WriteNull(7); break;
                    case 7: handler.Write((int)value); handler.WriteNull(4); break;
                    case 8: handler.Write((uint)value); handler.WriteNull(4); break;
                    case 11: handler.Write((float)value); handler.WriteNull(4); break;
                    case 15: handler.WriteOffsetWString((string)value); break;
                    default: throw new NotImplementedException();
                }
                return;
            }

            handler.Write(dataOffset);
            using var _ = handler.SeekJumpBack(dataOffset);
            switch (valueType)
            {
                case 7:
                    switch (valueCount) {
                        case 1: handler.Write((int)value!); break;
                        default: handler.WriteArray((int[])value!); break;
                    }
                    break;
                case 8:
                    switch (valueCount) {
                        case 1: handler.Write((uint)value!); break;
                        default: handler.WriteArray((uint[])value!); break;
                    }
                    break;
                case 10:
                    // TODO not fully sure about the type here but it looks vaguely correct
                    switch (valueCount) {
                        case 1: handler.Write((FrameValue)value!); break;
                        default: handler.WriteArray((FrameValue[])value!); break;
                    }
                    break;
                case 15:
                    foreach (var str in (string[])value!) handler.WriteOffsetWString(str);
                    break;
                case 17:
                    switch (valueCount) {
                        case 1: handler.Write((float)value!); break;
                        case 4: handler.Write((Quaternion)value!); break;
                        default: handler.WriteArray((float[])value!); break;
                    }
                    break;
                default:
                    throw new NotImplementedException("Unsupported mot property tree value type " + valueType);
            }
            dataOffset = handler.Tell();
        }
    }
}


namespace ReeLib
{
    public abstract class MotFileBase(FileHandler fileHandler)
        : BaseFile(fileHandler)
    {
        public abstract KnownFileFormats MotType { get; }

        public abstract string Name { get; set; }
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
        public List<MotPropertyTrack> MotPropertyTracks { get; } = new();
        public MotPropertyTree? PropertyTree { get; set; }

        public List<MotBone> Bones { get; } = new();
        public List<MotBone> RootBones { get; } = new();

        public override string Name { get => Header.motName; set => Header.motName = value; }

        public MotBone? GetBoneByHash(uint hash) => Bones.FirstOrDefault(b => b.Header.boneHash == hash);

        private bool IsMotlist => PathUtils.GetFilenameExtensionWithoutSuffixes(FileHandler.FilePath ?? string.Empty).SequenceEqual("motlist") == true;

        public override KnownFileFormats MotType => KnownFileFormats.Motion;

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

            // if (header.boneClipCount > header.boneCount)
            //     throw new InvalidDataException($"boneClipCount {header.boneClipCount} > boneCount {header.boneCount}");

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

                    ReadMotEndClips();
                }
            }

            if (header.animatedPropertyCount > 0)
            {
                handler.Seek(header.motPropertyTracksOffset);
                for (int i = 0; i < header.animatedPropertyCount; ++i) {
                    var p = new MotPropertyTrack() { Version = Header.version };
                    p.Read(handler);
                    MotPropertyTracks.Add(p);
                }
            }

            if (header.propertyTreeOffset > 0)
            {
                handler.Seek(header.propertyTreeOffset);
                PropertyTree = new();
                PropertyTree.Read(handler.WithOffset(header.propertyTreeOffset));
            }

            if (!IsMotlist)
            {
                ReadBones(null);
            }

            return true;
        }

        protected override bool DoWrite()
        {
            FileHandler handler = FileHandler;
            var header = Header;
            header.Write(handler);
            // we'll be re-writing the header's string table at the end again, remove it for now
            handler.StringTable?.Clear();
            handler.Align(16);

            header.boneCount = (ushort)Bones.Count;
            header.boneClipCount = (ushort)BoneClips.Count;
            header.boneClipHeaderOffset = BoneClips.Count == 0 ? 0 : handler.Tell();
            foreach (var clip in BoneClips)
            {
                clip.ClipHeader.Write(handler);
            }

            handler.Align(16);
            foreach (var clip in BoneClips)
            {
                clip.ClipHeader.trackHeaderOffset = handler.Tell();
                clip.Write(handler);
            }

            foreach (var clip in BoneClips)
            {
                clip.WriteOffsetContents(handler);
            }

            handler.Align(16);

            header.clipCount = (byte)Clips.Count;
            header.clipFileOffset = 0;
            header.motEndClipDataOffset = 0;
            if (Clips.Count > 0)
            {
                handler.Align(16);
                header.clipFileOffset = handler.Tell();
                handler.Skip(header.clipCount * sizeof(long));
                for (int i = 0; i < Clips.Count; i++)
                {
                    handler.Align(16);
                    var clip = Clips[i];
                    handler.Write(header.clipFileOffset + i * sizeof(long), handler.Tell());
                    clip.Write(handler);
                }

                // end clips nested within clip count check to mirror read procedure
                if (EndClips.Count > 0)
                {
                    handler.Align(8);
                    header.motEndClipDataOffset = handler.Tell();
                    header.motEndClipCount = (byte)EndClips.Count;

                    WriteMotEndClips();
                }
            }

            if (MotPropertyTracks.Count > 0)
            {
                header.motPropertyTracksOffset = handler.Tell();
                var dataOffset = header.motPropertyTracksOffset + MotPropertyTracks.Count * MotPropertyTrack.HeaderStructSize;
                foreach (var prop in MotPropertyTracks)
                {
                    prop.Write(handler, ref dataOffset);
                }

                handler.Seek(dataOffset);
            }
            else
            {
                header.motPropertyTracksOffset = 0;
            }

            if (PropertyTree != null)
            {
                handler.Align(16);
                header.propertyTreeOffset = handler.Tell();
                PropertyTree.Write(handler.WithOffset(header.propertyTreeOffset));
            }

            header.Write(handler, 0);
            handler.StringTableFlush();
            header.motSize = (uint)handler.Tell();
            handler.Write(12, header.motSize);

            handler.Align(16);
            header.boneHeaderOffsetStart = handler.Tell();
            handler.Write(header.BoneHeaderOffsetStartOffset, header.boneHeaderOffsetStart);

            if (!IsMotlist)
            {
                if (BoneHeaders == null) throw new Exception("Missing bone headers for MOT");

                WriteBones();
            }

            return true;
        }

        public void AddBone(MotBone bone)
        {
            Bones.Add(bone);
            if (bone.Parent == null) {
                RootBones.Add(bone);
            } else {
                bone.Parent.Children.Add(bone);
            }
            BoneHeaders ??= new();
            BoneHeaders.Add(bone.Header);
        }

        public void RemoveBone(MotBone bone)
        {
            Bones.Remove(bone);
            if (bone.Parent == null) {
                RootBones.Remove(bone);
            } else {
                bone.Parent.Children.Remove(bone);
            }
            BoneHeaders?.Remove(bone.Header);
        }

        public void CopyBones(MotFile source)
        {
            if (source.Bones.Count > 0) {
                Bones.AddRange(source.Bones);
                RootBones.AddRange(source.RootBones);
            }
            if (source.BoneClips.Count > 0) {
                foreach (var src in source.BoneClips) {
                    var clip = new BoneMotionClip(src.ClipHeader.Clone());
                    clip.ClipHeader.trackFlags = TrackFlag.None;
                    BoneClips.Add(clip);
                }
            }
        }

#region Read helpers
        private void ReadMotEndClips()
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

            if (Header.version < MotVersion.MHR_DEMO)
            {
                EndClips.AddRange(endClipHeaders.Select(hh => new MotEndClip(hh)));
                return;
            }
            // new version
            handler.Seek(Header.motEndClipFrameValuesOffset);
            var frameDataOffsets = handler.ReadArray<long>(Header.motEndClipCount);
            for (int i = 0; i < Header.motEndClipCount; ++i)
            {
                handler.Seek(frameDataOffsets[i]);
                var endClip = new MotEndClip(endClipHeaders[i]);
                endClip.Read(handler);
                EndClips.Add(endClip);
            }
        }

        private void WriteMotEndClips()
        {
            var handler = FileHandler;
            var offsetsStart = handler.Tell();
            handler.Skip(EndClips.Count * sizeof(long));
            for (int i = 0; i < EndClips.Count; ++i)
            {
                handler.Write(offsetsStart, handler.Tell());
                offsetsStart += sizeof(long);
                EndClips[i].Header.Write(handler);
            }
            if (Header.version < MotVersion.MHR_DEMO)
            {
                return;
            }

            handler.Align(8);
            Header.motEndClipFrameValuesOffset = handler.Tell();

            offsetsStart = handler.Tell();
            handler.Skip(EndClips.Count * sizeof(long));
            for (int i = 0; i < EndClips.Count; ++i)
            {
                handler.Write(offsetsStart, handler.Tell());
                offsetsStart += sizeof(long);
                EndClips[i].Write(handler);
            }
        }

        public void ReadBones(MotFile? mainSourceMot)
        {
            var header = Header;
            var handler = FileHandler;
            var hasOwnBoneList = !IsMotlist || header.motSize == 0 || header.boneHeaderOffsetStart > 0 && header.boneHeaderOffsetStart < header.motSize;
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

                    int refBoneIndex;
                    if (boneHeader.parentOffs != 0) {
                        refBoneIndex = (int)((boneHeader.parentOffs - boneHeaderOffset) / BoneHeader.StructSize);
                        var parentBone = GetBoneByHash(BoneHeaders[refBoneIndex].boneHash);
                        if (parentBone == null)
                        {
                            Log.Warn("Parent bone not found");
                            continue;
                        }

                        bone.Parent = parentBone;
                        if (!parentBone.Children.Contains(bone)) parentBone.Children.Add(bone);
                    }
                    else
                    {
                        RootBones.Add(bone);
                    }

                    if (boneHeader.childOffs != 0)
                    {
                        refBoneIndex = (int)((boneHeader.childOffs - boneHeaderOffset) / BoneHeader.StructSize);
                        if (refBoneIndex != bone.Index)
                        {
                            var next = GetBoneByHash(BoneHeaders[refBoneIndex].boneHash);
                            while (next != null)
                            {
                                if (!bone.Children.Contains(next)) bone.Children.Add(next);
                                if (BoneHeaders[refBoneIndex].nextSiblingOffs == 0) break;

                                refBoneIndex = (int)((BoneHeaders[refBoneIndex].nextSiblingOffs - boneHeaderOffset) / BoneHeader.StructSize);
                                if (refBoneIndex == next.Index) break;

                                next = GetBoneByHash(BoneHeaders[refBoneIndex].boneHash);
                                // NOTE: some files have the same bone defined twice for some reason (dd2 - ch253000_00_cs_feather.motlist.751)
                                // ignore such cases and forget about the duplicates, hopefully not important
                                if (next != null && bone.Children.Contains(next)) break;
                            }
                        }
                    }
                }
            }

            foreach (var bone in BoneHeaders)
            {
                // could be sped up with a dictionary - but then we need to maintain both the list and dict, maybe later
                var clip = BoneClips.FirstOrDefault(c => c.ClipHeader.boneHash == bone.boneHash);
                if (clip != null)
                {
                    clip.ClipHeader.OriginalName = clip.ClipHeader.boneName = bone.boneName;
                }
            }
        }

        public void WriteBones()
        {
            if (BoneHeaders == null)
            {
                throw new Exception("Bone headers missing for mot file " + Name);
            }

            var header = Header;
            var handler = FileHandler;
            handler.Align(16);
            header.boneHeaderOffsetStart = handler.Tell();
            handler.Write(header.boneHeaderOffsetStart + sizeof(long) * 2);
            handler.Write((long)BoneHeaders.Count);

            // rebuild Bone headers in case anything changed and to ensure correct offsets
            var sortedBones = Bones.OrderBy(b => b.Index).ToList();
            DataInterpretationException.ThrowIf(sortedBones.Count > 0 && sortedBones.Last().Index != sortedBones.Count - 1 && !sortedBones.All(b => b.Index == 0), "Detected skips in bone indices, this probably shouldn't happen!");

            BoneHeaders.Sort(new FuncComparer<BoneHeader>((a, b) => a.Index.CompareTo(b.Index)));
            var firstBoneOffset = handler.Tell();
            foreach (var bone in Bones)
            {
                bone.Header.parentOffs = bone.Parent == null ? 0 : firstBoneOffset + bone.Parent.Index * BoneHeader.StructSize;
                var sibling = GetNextSiblingIndex(bone.Parent?.Children ?? RootBones, bone);
                bone.Header.nextSiblingOffs = sibling == -1 ? 0 : firstBoneOffset + sibling * BoneHeader.StructSize;
                bone.Header.childOffs = bone.Children.Count == 0 ? 0 : firstBoneOffset + bone.Children[0].Index * BoneHeader.StructSize;
                if (!string.IsNullOrEmpty(bone.Header.boneName)) {
                    bone.Header.boneHash = MurMur3HashUtils.GetHash(bone.Header.boneName);
                }
            }

            BoneHeaders.Write(handler);
            handler.StringTableFlush();
            handler.Align(16);
        }

        private static int GetNextSiblingIndex(List<MotBone> list, MotBone value, int defaultValue = -1)
        {
            var index = list.IndexOf(value);
            if (index == -1 || index == list.Count - 1) return defaultValue;
            return list[index + 1].Index;
        }
#endregion

        public override string ToString() => $"{Header.motName}";

        public void ChangeVersion(MotVersion version)
        {
            foreach (var clip in BoneClips)
            {
                clip.ChangeVersion(version);
            }
            foreach (var clip in EndClips)
            {
                clip.Header.Version = Header.version;
            }
            var clipVer = Header.version.GetClipVersion();
            foreach (var clip in Clips)
            {
                clip.ChangeVersion(clipVer);
            }
            foreach (var tracks in MotPropertyTracks)
            {
                tracks.Version = version;
                tracks.Track?.ChangeVersion(version);
            }
        }

        public void CopyValuesFrom(MotFile source, bool replaceBehaviorClips)
        {
            if (replaceBehaviorClips)
            {
                Clips.Clear();
                Clips.AddRange(source.Clips);
            }
            EndClips.Clear();
            EndClips.AddRange(source.EndClips);
            BoneClips.Clear();
            BoneClips.AddRange(source.BoneClips);
            if (BoneHeaders == null)
            {
                BoneHeaders = source.BoneHeaders?.ToList();
            }
            else
            {
                // keep the current bone list, otherwise we'd break object references without a big rewrite
                // and a vastly different source hierarchy probably wouldn't work anyway
                var fallbackBone = Bones.First();
                foreach (var clip in BoneClips)
                {
                    var bone = GetBoneByHash(clip.ClipHeader.boneHash);
                    if (bone == null)
                    {
                        // maintain received bone hashes in case any of them are actually important bones
                        clip.ClipHeader.OriginalName ??= clip.ClipHeader.boneName;
                        if (!string.IsNullOrEmpty(clip.ClipHeader.boneName) && MurMur3HashUtils.GetHash(clip.ClipHeader.boneName) != clip.ClipHeader.boneHash)
                        {
                            // clear the name because it's not the right name and we don't want to overwrite the hash
                            clip.ClipHeader.boneName = null;
                        }
                    }
                    else
                    {
                        clip.ClipHeader.boneIndex = (ushort)bone.Index;
                        clip.ClipHeader.boneName ??= bone.Name;
                    }

                    if (Header.version <= MotVersion.RE2_DMC5)
                    {
                        clip.ClipHeader.uknIndex = 0;
                    }
                }
            }

            // ensure all versions match
            ChangeVersion(Header.version);

            Header.CopyValuesFrom(source.Header);
        }
    }
}
