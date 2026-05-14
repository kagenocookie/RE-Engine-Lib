using System.Numerics;
using ReeLib.Common;
using ReeLib.InternalAttributes;
using ReeLib.Jmap;
using ReeLib.via;

namespace ReeLib.Jmap
{
    public class JmapHeader : ReadWriteModel
    {
        internal uint version;
        public uint Version => version;
        internal uint magic = JmapFile.Magic;

        internal long bonesOffset;
        internal long bonesAfterOffset;

        internal long jointMaskGroupOffset;
        internal long jointExpressionGroupDataOffset;
        internal long extraJointsOffset;
        internal long ikMotionDataOffset;
        internal long symmetryDataOffset;
        internal long skeletonMaskDataPtr;
        internal long symmetryData2Offset;

        public uint attributeFlags;
        internal ushort boneCount;
        internal ushort jointMaskGroupCount;
        internal byte ikMotionCount;

        protected override bool ReadWrite<THandler>(THandler action)
        {
            action.Do(ref version);
            action.Do(ref magic);
            action.Null(8);

            if (version <= 11)
            {
                action.Do(ref bonesOffset);
                action.Do(ref bonesAfterOffset);
            }

            action.Do(ref jointMaskGroupOffset);
            action.Do(ref jointExpressionGroupDataOffset);
            action.Do(ref extraJointsOffset);

            if (version >= 17)
            {
                if (version >= 28) action.Null(8);
                action.Do(ref ikMotionDataOffset);
                action.Do(ref symmetryDataOffset);
                if (version >= 28) {
                    action.Do(ref symmetryData2Offset);
                    action.Null(4 * 8);
                }
                if (version >= 19) action.Do(ref skeletonMaskDataPtr);
                action.Do(ref attributeFlags);
                action.Do(ref jointMaskGroupCount);
                action.Do(ref ikMotionCount);
            }
            else
            {
                action.Do(ref boneCount);
                action.Do(ref jointMaskGroupCount);
                int extraJointsCount = extraJointsOffset > 0 ? 1 : 0;
                action.Do(ref extraJointsCount);
                DataInterpretationException.DebugThrowIf(extraJointsCount > 1);
            }

            return true;
        }
    }

    [Flags]
    public enum JointMapAttributes
    {
        Deform = 1,
        JointWeight = 2,
        Attr3 = 4,
        Attr4 = 8,
    }

    [RszGenerate, RszAutoReadWrite]
    public partial class JointData : BaseModel
    {
        public PaddedVec3 offset;
        public Quaternion rotation;

        [RszPaddingAfter(2)]
        public ushort parentId;

        [RszPaddingAfter(8)]
        public uint boneHash;

        public override string ToString() => $"Joint Hash: {boneHash}";
    }

    [RszGenerate]
    public partial class JointMaskGroup : BaseModel
    {
        internal long jointNameHashOffset;
        internal long masksOffset;
        [RszConditional("handler.FileVersion >= 17")]
        internal long weightsOffset;
        public uint groupId;
        [RszPaddingAfter(2)]
        internal ushort jointCount;

        [field: RszIgnore]
        public List<JointMask> Masks { get; } = new();

        protected override bool DoRead(FileHandler handler)
        {
            DefaultRead(handler);
            var pos = handler.Tell();
            ReadData(handler);
            handler.Seek(pos);
            return true;
        }

        protected override bool DoWrite(FileHandler handler)
        {
            jointNameHashOffset = 0;
            masksOffset = 0;
            weightsOffset = 0;
            return DefaultWrite(handler);
        }

        private void ReadData(FileHandler handler)
        {
            Masks.Clear();
            for (int i = 0; i < jointCount; i++) {
                Masks.Add(new JointMask());
            }
            if (jointNameHashOffset > 0) {
                handler.Seek(jointNameHashOffset);
                for (int i = 0; i < jointCount; i++) {
                    handler.Read(ref Masks[i].jointHash);
                }
            }
            if (masksOffset > 0) {
                handler.Seek(masksOffset);
                for (int i = 0; i < jointCount; i++) {
                    handler.Read(ref Masks[i].mask);
                }
            }
            if (weightsOffset > 0) {
                handler.Seek(weightsOffset);
                for (int i = 0; i < jointCount; i++) {
                    handler.Read(ref Masks[i].weight);
                }
            }
            DataInterpretationException.DebugWarnIf(jointNameHashOffset == 0);
            DataInterpretationException.DebugWarnIf(masksOffset == 0);
        }

        public void WriteData(FileHandler handler)
        {
            var hasWeights = false;
            handler.Align(16);
            handler.Write(Start + 0, jointNameHashOffset = handler.Tell());
            foreach (var mask in Masks) {
                handler.Write(mask.jointHash);
                hasWeights |= mask.weight != 0;
            }

            handler.Align(16);
            handler.Write(Start + 8, masksOffset = handler.Tell());
            foreach (var mask in Masks) {
                handler.Write(mask.mask);
            }

            if (hasWeights)
            {
                handler.Align(16);
                handler.Write(Start + 16, weightsOffset = handler.Tell());
                foreach (var mask in Masks) {
                    handler.Write(mask.weight);
                }
            }
        }

        public override string ToString() => $"MaskGroup {groupId}";
    }

    public class JointMask
    {
        public string? jointName;
        public uint jointHash;
        public byte mask;
        public float weight;

        public override string ToString() => $"[{jointName ?? jointHash.ToString()}] {mask} (w = {weight})";
    }

    public class SymmetryJointData
    {
        public string? jointName;
        public uint jointHash;
        public AxisMirrorFlags posMirrorFlags;
        public AxisMirrorFlags rotMirrorFlags;
        public byte attr3;

        public override string ToString() => $"[{jointName ?? jointHash.ToString()}] P={posMirrorFlags}, R={rotMirrorFlags}, {attr3}";
    }

    /// <summary>
    /// via.motion.JointExType
    /// </summary>
    public enum JointExType : byte
    {
        None = 0,
        Rotation = 1,
        RotToScale = 2,
        RotToScaleEx = 3,
        RotToTrans = 4,
        RotToTransEx = 5,
        Finger = 6,
        Thumb = 7,
        RotToRot = 8,
        RotToRotEx = 9,
        Limit = 10,
        PointConstraint = 11,
        ParentConstraint = 12,
        BsplineConstraint = 13,
        RemapValue = 14,
        MultiRemapValue = 15,
        Rotation2 = 16,
    }

    [Flags]
    public enum JointExAttribute
    {
        Inverse = 1 << 0,
        Mirroring = 1 << 1,
        InX = 1 << 2,
        InY = 1 << 3,
        InZ = 1 << 4,
        OutX = 1 << 5,
        OutY = 1 << 6,
        OutZ = 1 << 7,
        Order_1 = 1 << 8,
        Order_2 = 1 << 9,
        Order_3 = 1 << 10,
        Order = 1 << 8 | 1 << 9 | 1 << 10,
        Enabled = 1 << 11,
        InBase = 1 << 12,
        OutBase = 1 << 13,
        BaseToEnd = 1 << 14,
    }

    public enum Axis : byte
    {
        X = 0,
        Y = 1,
        Z = 2,
    }

    public enum TRS : byte
    {
        Trans = 0,
        Rot = 1,
        Scale = 2,
    }

    public enum InputType : byte
    {
        Cone = 3,
    }

    public enum CalculateMode : byte
    {
        Sum = 0,
        Average = 1,
    }

    [Flags]
    public enum JointLimitFlags : int
    {
        BasePose = 1,
    }

    [RszGenerate, RszAutoReadWrite]
    public partial class ExtraJointGroupRotation : JointExpression
    {
        public override JointExType JointType => JointExType.Rotation;

        public uint targetJointNameHash;
        public uint fromJointNameHash;
        public JointExAttribute attributes;
        public Vector3 localRotation;
    }

    [RszGenerate, RszAutoReadWrite]
    public partial class ExtraJointGroupRotToTrans : JointExpression
    {
        public override JointExType JointType => JointExType.RotToTrans;

        public uint targetJointNameHash;
        public uint fromJointNameHash;
        [RszPaddingAfter(4)]
        public JointExAttribute attributes;

        [RszFixedSizeArray(3)] public RotScalingConvertData[] scaling = new RotScalingConvertData[3];
    }

    [RszGenerate, RszAutoReadWrite]
    public partial class ExtraJointGroupRotToTransEx : JointExpression
    {
        public override JointExType JointType => JointExType.RotToTransEx;

        public uint targetJointNameHash;
        public uint fromJointNameHash;
        [RszPaddingAfter(4)]
        public JointExAttribute attributes;

        [RszFixedSizeArray(3)] public RotScalingConvertDivideData[] scaling = new RotScalingConvertDivideData[3];
    }
    [RszGenerate, RszAutoReadWrite]
    public partial class ExtraJointGroupRotToRotEx : JointExpression
    {
        public override JointExType JointType => JointExType.RotToRotEx;

        public uint targetJointNameHash;
        public uint fromJointNameHash;
        [RszPaddingAfter(4)]
        public JointExAttribute attributes;

        [RszFixedSizeArray(3)] public RotScalingConvertDivideData[] scaling = new RotScalingConvertDivideData[3];
    }

    [RszGenerate, RszAutoReadWrite]
    public partial class ExtraJointGroupFinger : JointExpression
    {
        public override JointExType JointType => JointExType.Finger;

        public uint fromJointNameHash;
        public JointExAttribute attributes;

        [RszFixedSizeArray(3)] public SubExFingerData[] scaling = new SubExFingerData[3];
    }

    [RszGenerate, RszAutoReadWrite]
    public partial class ExtraJointGroupThumb : JointExpression
    {
        public override JointExType JointType => JointExType.Thumb;

        public uint fromJointNameHash;
        public JointExAttribute attributes;

        [RszFixedSizeArray(3)] public SubExThumbData[] scaling = new SubExThumbData[3];
    }

    [RszGenerate, RszAutoReadWrite]
    public partial class ExtraJointGroupRemapValue : JointExpression
    {
        public override JointExType JointType => JointExType.RemapValue;

        public SubExJointRemapOutput output;
        public SubExJointRemapInput input1;
        public SubExJointRemapInput input2;
        public SubExJointRemapInput input3;
    }

    [RszGenerate, RszAutoReadWrite]
    public partial class ExtraJointGroupRotation2 : JointExpression
    {
        public override JointExType JointType => JointExType.Rotation2;

        public Quaternion inBaseQuat;
        public Quaternion outBaseQuat;
        public uint targetJointNameHash;
        public uint fromJointNameHash;
        public JointExAttribute attributes;
        public Vector3 vector;
    }

    public struct RotScalingConvertData
    {
        public ushort attribute;
        public ushort input;
        public uint reserved;
        public float inStart;
        public float inEnd;
        public float outStart;
        public float outEnd;
        public float tangent1;
        public float tangent2;
    }

    public struct RotScalingConvertDivideData
    {
        public ushort attribute;
        public ushort input;
        public float inLowerLow;
        public float inLowerHight;
        public float inUpperLow;
        public float inUpperHigh;
        public float outLower;
        public float outMiddle;
        public float outUpper;
        public float lowerTangent1;
        public float lowerTangent2;
        public float upperTangent1;
        public float upperTangent2;
    }

    public struct SubExFingerData
    {
        public uint targetJointNameHash;
        public JointExAttribute attributes;
        public float rotRateX;
        public float rotRateY;
        [RszPaddingAfter(4)]
        public float rotRateZ;

        public RotScalingConvertData scaling1;
        public RotScalingConvertData scaling2;
        public RotScalingConvertData scaling3;
    }

    public struct SubExThumbData
    {
        public uint targetJointNameHash;
        public JointExAttribute attributes;

        public RotScalingConvertData scaling1;
        public RotScalingConvertData scaling2;
        public RotScalingConvertData scaling3;
    }

    public struct SubExJointRemapOutput
    {
        public uint jointNameHash;
        public uint flags;
        public TRS trs;
        public Axis axis;
        public byte outRotOrder;
        public CalculateMode mode;
        public uint reserved;
    }

    public struct SubExJointRemapInput
    {
        public uint jointNameHash;
        public uint flags;
        public Vector3 offset;
        public float inMin;
        public float inMid;
        public float inMax;
        public float outMin;
        public float outMid;
        public float outMax;
        public float coneHalfAngle;
        public InputType type;
        public Axis inAxis;
        public byte inRotOrder;
        public byte reserved;
        public Vector3 reservedF3;
    }

    public abstract class JointExpression : BaseModel
    {
        [RszIgnore] public byte lod;

        public static readonly Dictionary<JointExType, Type> JointExpressionTypeMap = new () {
            { JointExType.Rotation, typeof(ExtraJointGroupRotation) },
            { JointExType.RotToTrans, typeof(ExtraJointGroupRotToTrans) },
            { JointExType.RotToTransEx, typeof(ExtraJointGroupRotToTransEx) },
            { JointExType.Finger, typeof(ExtraJointGroupFinger) },
            { JointExType.Thumb, typeof(ExtraJointGroupThumb) },
            { JointExType.RotToRotEx, typeof(ExtraJointGroupRotToRotEx) },
            { JointExType.RemapValue, typeof(ExtraJointGroupRemapValue) },
            { JointExType.Rotation2, typeof(ExtraJointGroupRotation2) },
        };

        public abstract JointExType JointType { get; }

        public override string ToString() => $"{JointType}";
    }

    public class JointExpressionGroupData : BaseModel
    {
        public List<JointExpression> Expressions { get; } = new();

        protected override bool DoRead(FileHandler handler)
        {
            Expressions.Clear();
            var typeOffset = handler.Read<long>();
            var lodOffset = handler.Read<long>();
            var dataOffset = handler.Read<long>();
            var count = handler.Read<int>();

            handler.Seek(typeOffset);
            var types = handler.ReadArray<JointExType>(count);

            handler.Seek(lodOffset);
            var lods = handler.ReadArray<byte>(count);

            handler.Seek(dataOffset);
            var dataOffsets = handler.ReadArray<long>(count);

            Expressions.Clear();
            for (int i = 0; i < count; ++i)
            {
                DataInterpretationException.DebugThrowIf(i > 0 && Utils.Align16((int)handler.Tell()) != dataOffsets[i]);
                handler.Seek(dataOffsets[i]);
                if (!JointExpression.JointExpressionTypeMap.TryGetValue(types[i], out var type)) {
                    throw new NotSupportedException("Unsupported extra joint group type " + types[i]);
                }

                var data = (JointExpression)Activator.CreateInstance(type)!;
                data.lod = lods[i];
                data.Read(handler);
                Expressions.Add(data);

            }
            return true;
        }

        protected override bool DoWrite(FileHandler handler)
        {
            handler.Skip(sizeof(long) * 3);
            handler.Write((long)Expressions.Count);

            handler.Align(16);
            handler.Write(Start, handler.Tell());
            foreach (var grp in Expressions) handler.Write(grp.JointType);

            handler.Align(16);
            handler.Write(Start + sizeof(long), handler.Tell());
            foreach (var grp in Expressions) handler.Write(grp.lod);

            handler.Align(16);
            var offsetsOffset = handler.Tell();
            handler.Write(Start + sizeof(long) * 2, offsetsOffset);
            handler.Skip(Expressions.Count * sizeof(long));

            for (int i = 0; i < Expressions.Count; ++i)
            {
                handler.Align(16);
                handler.Write(offsetsOffset + i * sizeof(long), handler.Tell());
                Expressions[i].Write(handler);
            }
            return true;
        }
    }

    public class ExtraJointInfo : BaseModel
    {
        public string? parentName;
        public string? jointName;
        public uint symmetryHash;
        public uint flags;
        public long ukn;

        protected override bool DoRead(FileHandler handler)
        {
            parentName = handler.ReadOffsetWString();
            jointName = handler.ReadOffsetWString();
            var parentHash = handler.Read<uint>();
            var jointHash = handler.Read<uint>();
            DataInterpretationException.DebugThrowIf(parentHash != MurMur3HashUtils.GetHash(parentName));
            DataInterpretationException.DebugThrowIf(jointHash != MurMur3HashUtils.GetHash(jointName));
            handler.Read(ref symmetryHash);
            handler.Read(ref flags);
            if (handler.FileVersion >= 19) handler.Read(ref ukn);
            return true;
        }

        protected override bool DoWrite(FileHandler handler)
        {
            handler.WriteOffsetWString(parentName ??= "");
            handler.WriteOffsetWString(jointName ??= "");
            handler.Write(MurMur3HashUtils.GetHash(parentName));
            handler.Write(MurMur3HashUtils.GetHash(jointName));
            handler.Write(ref symmetryHash);
            handler.Write(ref flags);
            if (handler.FileVersion >= 19) handler.Write(ref ukn);
            return true;
        }

        public override string ToString() => $"{parentName} => {jointName}";
    }

    public class ExtraJoints : BaseModel
    {
        public List<ExtraJointInfo> Joints { get; } = new();

        protected override bool DoRead(FileHandler handler)
        {
            Joints.Clear();
            var offset = handler.Read<long>();
            var count = handler.Read<int>();
            handler.Seek(offset);
            Joints.Read(handler, count);
            return true;
        }


        protected override bool DoWrite(FileHandler handler)
        {
            handler.Write(handler.Tell() + 16);
            handler.Write(Joints.Count);
            handler.Align(16);
            Joints.Write(handler);
            return true;
        }
    }

    [RszGenerate, RszAutoReadWrite]
    public partial class IkMotionData : BaseModel
    {
        public uint ikJointNameHash;
        public uint fkJointNameHash;
        public IkType ikType;
        public byte descendantLevelFromIkHandle;
        public AxisDirection ikDirection;
        public AxisDirection ikUpvector;
    }

    public enum IkType : byte
    {
        TwoBone = 0,
        ThreeBoneTypeC = 1,
    }

    public enum AxisDirection : byte
    {
        Undef = 0,
        X = 1,
        Y = 2,
        Z = 3,
        NX = 5,
        NY = 6,
        NZ = 7,
    }

    [Flags]
    public enum AxisMirrorFlags : byte
    {
        X = 1,
        Y = 2,
        Z = 4,
        Enable = 8,
    }

    public partial class SymmetryMirrorData : BaseModel
    {
        public AxisMirrorFlags defaultPosMirrorFlags;
        public AxisMirrorFlags defaultRotMirrorFlags;
        public byte defaultFlags3;
        public byte defaultFlags4;

        public List<SymmetryJointData> Joints { get; } = new();

        public void UpdateJointNames()
        {
            foreach (var joint in Joints) {
                joint.jointName = Utils.HashedBoneNames.GetValueOrDefault(joint.jointHash);
            }
        }

        protected override bool DoRead(FileHandler handler)
        {
            Joints.Clear();

            var jointsOffset = handler.Read<long>();
            var mapsOffset = handler.Read<long>();
            var count = handler.Read<int>();
            handler.Read(ref defaultPosMirrorFlags);
            handler.Read(ref defaultRotMirrorFlags);
            handler.Read(ref defaultFlags3);
            handler.Read(ref defaultFlags4);

            handler.Seek(jointsOffset);
            var hashes = handler.ReadArray<uint>(count);
            handler.Seek(mapsOffset);
            for (int i = 0; i < count; i++) {
                Joints.Add(new SymmetryJointData() {
                    jointHash = hashes[i],
                    jointName = Utils.HashedBoneNames.GetValueOrDefault(hashes[i]),
                    posMirrorFlags = handler.Read<AxisMirrorFlags>(),
                    rotMirrorFlags = handler.Read<AxisMirrorFlags>(),
                    attr3 = handler.FileVersion >= 28 ? handler.Read<byte>() : (byte)0,
                });
            }
            return true;
        }

        protected override bool DoWrite(FileHandler handler)
        {
            handler.Skip(sizeof(long) * 2);
            handler.Write(Joints.Count);
            handler.Write(ref defaultPosMirrorFlags);
            handler.Write(ref defaultRotMirrorFlags);
            handler.Write(ref defaultFlags3);
            handler.Write(ref defaultFlags4);
            handler.Write(Start, handler.Tell()); // jointsOffset
            foreach (var joint in Joints) {
                handler.Write(ref joint.jointHash);
            }
            handler.Write(Start + sizeof(long), handler.Tell()); // mapsOffset
            foreach (var joint in Joints) {
                handler.Write(ref joint.posMirrorFlags);
                handler.Write(ref joint.rotMirrorFlags);
                if (handler.FileVersion >= 28) {
                    handler.Write(ref joint.attr3);
                }
            }
            return true;
        }
    }

    public partial class SkeletonMaskData : BaseModel
    {
        public uint[] hashes = [];

        protected override bool DoRead(FileHandler handler)
        {
            var offset = handler.Read<long>();
            var count = handler.Read<int>();
            handler.Seek(offset);
            hashes = handler.ReadArray<uint>(count);
            return true;
        }

        protected override bool DoWrite(FileHandler handler)
        {
            handler.Skip(sizeof(long));
            handler.Write(hashes.Length);
            handler.Write(Start, handler.Tell());
            handler.WriteArray(hashes);
            return true;
        }
    }
}

namespace ReeLib
{
    public class JmapFile(FileHandler fileHandler) : BaseFile(fileHandler)
    {
        public JmapHeader Header { get; } = new();
        public List<JointData> Joints { get; } = new();
        public uint[]? AfterBoneData { get; set; }
        public List<JointMaskGroup> MaskGroups { get; } = new();
        public JointExpressionGroupData JointExpressionGroups { get; } = new();
        public ExtraJoints ExtraJoints { get; } = new();
        public List<IkMotionData> IkMotionData { get; } = new();
        public SymmetryMirrorData SymmetryData { get; } = new();
        public SymmetryMirrorData? SymmetryData2 { get; set; }
        public SkeletonMaskData SkeletonMaskData { get; } = new();

        public const int Magic = 0x70616D6A;

        public void UpdateJointNames()
        {
            foreach (var group in MaskGroups) {
                foreach (var mask in group.Masks) {
                    mask.jointName = Utils.HashedBoneNames.GetValueOrDefault(mask.jointHash);
                }
            }
            SymmetryData.UpdateJointNames();
            SymmetryData2?.UpdateJointNames();
        }

        protected override bool DoRead()
        {
            var handler = FileHandler;
            Header.Read(handler);
            if (Header.magic != Magic) {
                throw new Exception("Invalid JMAP file!");
            }
            var version = handler.FileVersion;

            Joints.Clear();
            IkMotionData.Clear();
            if (version <= 11)
            {
                handler.Seek(Header.bonesOffset);
                Joints.Read(handler, Header.boneCount);

                handler.Seek(Header.bonesAfterOffset);
                var afterOffset = handler.Read<long>();
                handler.ReadNull(8 * 5);
                handler.Seek(afterOffset);
                AfterBoneData = handler.ReadArray<uint>(60);
            }

            MaskGroups.Clear();
            if (Header.jointMaskGroupOffset > 0)
            {
                handler.Seek(Header.jointMaskGroupOffset);
                MaskGroups.Read(handler, Header.jointMaskGroupCount);
            }

            handler.Seek(Header.jointExpressionGroupDataOffset);
            JointExpressionGroups.Read(handler);

            if (Header.extraJointsOffset > 0)
            {
                handler.Seek(Header.extraJointsOffset);
                ExtraJoints.Read(handler);
            }

            if (Header.ikMotionDataOffset > 0 && Header.ikMotionCount > 0)
            {
                handler.Seek(Header.ikMotionDataOffset);
                IkMotionData.Read(handler, Header.ikMotionCount);
            }

            if (Header.symmetryDataOffset > 0)
            {
                handler.Seek(Header.symmetryDataOffset);
                SymmetryData.Read(handler);
            }

            if (Header.skeletonMaskDataPtr > 0)
            {
                handler.Seek(Header.skeletonMaskDataPtr);
                SkeletonMaskData.Read(handler);
            }

            if (Header.symmetryData2Offset > 0)
            {
                handler.Seek(Header.symmetryData2Offset);
                SymmetryData2 ??= new();
                SymmetryData2.Read(handler);
            }

            return true;
        }

        protected override bool DoWrite()
        {
            var handler = FileHandler;
            var version = FileHandler.FileVersion;
            Header.ikMotionCount = (byte)IkMotionData.Count;
            Header.Write(handler);

            if (version <= 11)
            {
                Header.boneCount = (ushort)Joints.Count;
                handler.Align(16);
                Header.bonesOffset = handler.Tell();
                Joints.Write(handler);

                Header.bonesAfterOffset = handler.Tell();
                handler.WriteNull(8 * 6);
                handler.Write(Header.bonesAfterOffset, handler.Tell());
                handler.WriteArray(AfterBoneData ??= new uint[60]);
            }

            handler.Align(16);
            Header.jointMaskGroupCount = (ushort)MaskGroups.Count;
            Header.jointMaskGroupOffset = handler.Tell();
            MaskGroups.Write(handler);
            foreach (var grp in MaskGroups)
            {
                grp.WriteData(handler);
            }

            handler.Align(16);
            Header.jointExpressionGroupDataOffset = handler.Tell();
            JointExpressionGroups.Write(handler);

            Header.extraJointsOffset = 0;
            if (ExtraJoints.Joints.Count > 0) {
                handler.Align(16);
                Header.extraJointsOffset = handler.Tell();
                ExtraJoints.Write(handler);
            }

            if (version >= 19)
            {
                handler.Align(16);
                Header.ikMotionDataOffset = handler.Tell();
                IkMotionData.Write(handler);

                Header.symmetryDataOffset = 0;
                if (SymmetryData.Joints.Count > 0) {
                    handler.Align(16);
                    Header.symmetryDataOffset = handler.Tell();
                    SymmetryData.Write(handler);
                }

                handler.Align(16);
                Header.skeletonMaskDataPtr = handler.Tell();
                SkeletonMaskData.Write(handler);

                if (version >= 28) {
                    Header.symmetryData2Offset = 0;
                    if (SymmetryData2?.Joints.Count > 0) {
                        handler.Align(16);
                        Header.symmetryData2Offset = handler.Tell();
                        SymmetryData2.Write(handler);
                    }
                }
            }

            handler.StringTableFlush();

            Header.Write(handler, 0);
            return true;
        }
    }

}