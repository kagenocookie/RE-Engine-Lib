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
        internal long jointExtraGroupDataOffset;
        internal long extraJointsOffset;
        internal long ikMotionDataOffset;
        internal long symmetryDataOffset;
        internal long extraHashesOffset;

        public uint attributeFlags;
        internal ushort boneCount;
        internal ushort jointMaskGroupCount;

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
            action.Do(ref jointExtraGroupDataOffset);
            action.Do(ref extraJointsOffset);

            if (version >= 17)
            {
                action.Do(ref ikMotionDataOffset);
                action.Do(ref symmetryDataOffset);
                if (version >= 19) action.Do(ref extraHashesOffset);
                if (version >= 28) action.Null(6 * 8);
                action.Do(ref attributeFlags);
                action.Do(ref jointMaskGroupCount);
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
        public long jointNameHashOffset;
        public long masksOffset;
        [RszConditional("handler.FileVersion >= 17")]
        public long weightsOffset;
        public uint groupId;
        public ushort jointCount;
        public ushort empty;
        [RszIgnore]
        public uint[]? jointHashes;
        [RszIgnore]
        public byte[]? masks;
        [RszIgnore]
        public float[]? weights;

        protected override bool DoRead(FileHandler handler)
        {
            DefaultRead(handler);
            var pos = handler.Tell();
            if (jointNameHashOffset > 0)
            {
                handler.Seek(jointNameHashOffset);
                jointHashes = handler.ReadArray<uint>(jointCount);
            }
            if (masksOffset > 0)
            {
                handler.Seek(masksOffset);
                masks = handler.ReadArray<byte>(jointCount);
            }
            if (weightsOffset > 0)
            {
                handler.Seek(weightsOffset);
                weights = handler.ReadArray<float>(jointCount);
            }
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

        public void WriteData(FileHandler handler)
        {
            if (jointHashes != null)
            {
                handler.Align(16);
                handler.Write(Start + 0, jointNameHashOffset = handler.Tell());
                handler.WriteArray(jointHashes);
            }
            if (masks != null)
            {
                handler.Align(16);
                handler.Write(Start + 8, masksOffset = handler.Tell());
                handler.WriteArray(masks);
            }
            if (weights != null)
            {
                handler.Align(16);
                handler.Write(Start + 16, weightsOffset = handler.Tell());
                handler.WriteArray(weights);
            }
        }

        public override string ToString() => $"MaskGroup {groupId}";
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

    [RszGenerate, RszAutoReadWrite]
    public partial class ExtraJointGroupRotation : ExtraJointGroup
    {
        public override JointExType JointType => JointExType.Rotation;

        public uint hash;
        public uint hash2;
        public uint num;
        public Vector3 localRotation;
    }

    [RszGenerate, RszAutoReadWrite]
    public partial class ExtraJointGroupRotToTrans : ExtraJointGroup
    {
        public override JointExType JointType => JointExType.RotToTrans;

        public uint hash;
        public uint hash2;
        public uint num;
        public uint flags;
        [RszFixedSizeArray(24)] public uint[] data = new uint[24];
    }
    [RszGenerate, RszAutoReadWrite]
    public partial class ExtraJointGroupRotToTransEx : ExtraJointGroup
    {
        public override JointExType JointType => JointExType.RotToTransEx;

        public uint hash;
        public uint hash2;
        public uint num;
        public uint flags;
        [RszFixedSizeArray(36)] public uint[] data = new uint[36];
    }
    [RszGenerate, RszAutoReadWrite]
    public partial class ExtraJointGroupRotToRotEx : ExtraJointGroup
    {
        public override JointExType JointType => JointExType.RotToRotEx;

        public uint hash;
        public uint hash2;
        public uint num;
        public uint flags;
        [RszFixedSizeArray(36)] public uint[] data = new uint[36];
    }

    [RszGenerate, RszAutoReadWrite]
    public partial class ExtraJointGroupFinger : ExtraJointGroup
    {
        public override JointExType JointType => JointExType.Finger;

        public uint hash;
        public uint num;
        public uint hash2;
        public uint flags;
        [RszFixedSizeArray(88)] public uint[] data = new uint[88];
    }

    [RszGenerate, RszAutoReadWrite]
    public partial class ExtraJointGroupThumb : ExtraJointGroup
    {
        public override JointExType JointType => JointExType.Thumb;

        public uint hash;
        public uint num;
        public uint hash2;
        public uint flags;
        [RszFixedSizeArray(76)] public uint[] data = new uint[76];
    }

    [RszGenerate, RszAutoReadWrite]
    public partial class ExtraJointGroupRemapValue : ExtraJointGroup
    {
        public override JointExType JointType => JointExType.RemapValue;

        public uint hash;
        [RszFixedSizeArray(51)] public uint[] data = new uint[51];
    }

    [RszGenerate, RszAutoReadWrite]
    public partial class ExtraJointGroupRotation2 : ExtraJointGroup
    {
        public override JointExType JointType => JointExType.Rotation2;

        public Quaternion quat1;
        public Quaternion quat2;
        public uint hash1;
        public uint hash2;
        public uint ukn;
        public Vector3 vec;
    }

    public abstract class ExtraJointGroup : BaseModel
    {
        [RszIgnore] public byte lod;

        public static readonly Dictionary<JointExType, Type> ExtraJointTypeMap = new () {
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

    public class ExtraJointGroupContainer : BaseModel
    {
        public List<ExtraJointGroup> Groups { get; } = new();

        protected override bool DoRead(FileHandler handler)
        {
            Groups.Clear();
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

            Groups.Clear();
            for (int i = 0; i < count; ++i)
            {
                DataInterpretationException.DebugThrowIf(i > 0 && Utils.Align16((int)handler.Tell()) != dataOffsets[i]);
                handler.Seek(dataOffsets[i]);
                if (!ExtraJointGroup.ExtraJointTypeMap.TryGetValue(types[i], out var type)) {
                    throw new NotSupportedException("Unsupported extra joint group type " + types[i]);
                }

                var data = (ExtraJointGroup)Activator.CreateInstance(type)!;
                data.lod = lods[i];
                data.Read(handler);
                Groups.Add(data);

            }
            return true;
        }

        protected override bool DoWrite(FileHandler handler)
        {
            handler.Skip(sizeof(long) * 3);
            handler.Write((long)Groups.Count);

            handler.Align(16);
            handler.Write(Start, handler.Tell());
            foreach (var grp in Groups) handler.Write(grp.JointType);

            handler.Align(16);
            handler.Write(Start + sizeof(long), handler.Tell());
            foreach (var grp in Groups) handler.Write(grp.lod);

            handler.Align(16);
            var offsetsOffset = handler.Tell();
            handler.Write(Start + sizeof(long) * 2, offsetsOffset);
            handler.Skip(Groups.Count * sizeof(long));

            for (int i = 0; i < Groups.Count; ++i)
            {
                handler.Align(16);
                handler.Write(offsetsOffset + i * sizeof(long), handler.Tell());
                Groups[i].Write(handler);
            }
            return true;
        }
    }

    public class ExtraJointInfo : BaseModel
    {
        public string? parentName;
        public string? jointName;
        public int symmetryHash;
        public int flags;
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
        public byte ikType;
        public char descendantLevelFromIkHandle;
        public byte ikDirection;
        public byte ikUpvector;
    }

    public partial class SymmetryMirrorData : BaseModel
    {
        public uint[] hashes = [];
        public (byte, byte)[] pairs = [];
        public (byte, byte) primaryPair;

        protected override bool DoRead(FileHandler handler)
        {
            var jointsOffset = handler.Read<long>();
            var mapsOffset = handler.Read<long>();
            var count = handler.Read<int>();
            handler.Read(ref primaryPair);
            handler.Seek(jointsOffset);
            hashes = handler.ReadArray<uint>(count);
            handler.Seek(mapsOffset);
            pairs = handler.ReadArray<(byte, byte)>(count);
            return true;
        }

        protected override bool DoWrite(FileHandler handler)
        {
            handler.Skip(sizeof(long) * 2);
            handler.Write(hashes.Length);
            handler.Write(ref primaryPair);
            handler.Write(Start, handler.Tell());
            handler.WriteArray(hashes);
            handler.Write(Start + sizeof(long), handler.Tell());
            handler.WriteArray(pairs);
            return true;
        }
    }

    public partial class ExtraHashData : BaseModel
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
        public ExtraJointGroupContainer ExtraJointGroups { get; } = new();
        public ExtraJoints ExtraJoints { get; } = new();
        public IkMotionData IkMotionData { get; } = new();
        public SymmetryMirrorData SymmetryData { get; } = new();
        public ExtraHashData ExtraHashes { get; } = new();

        public const int Magic = 0x70616D6A;

        protected override bool DoRead()
        {
            var handler = FileHandler;
            Header.Read(handler);
            if (Header.magic != Magic) {
                throw new Exception("Invalid JMAP file!");
            }
            var version = handler.FileVersion;

            Joints.Clear();
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

            if (Header.jointExtraGroupDataOffset > 0)
            {
                handler.Seek(Header.jointExtraGroupDataOffset);
                ExtraJointGroups.Read(handler);
            }

            if (Header.extraJointsOffset > 0)
            {
                handler.Seek(Header.extraJointsOffset);
                ExtraJoints.Read(handler);
            }

            if (Header.ikMotionDataOffset > 0)
            {
                handler.Seek(Header.ikMotionDataOffset);
                IkMotionData.Read(handler);
            }

            if (Header.symmetryDataOffset > 0 && Header.symmetryDataOffset < handler.FileSize())
            {
                handler.Seek(Header.symmetryDataOffset);
                SymmetryData.Read(handler);
            }

            if (Header.extraHashesOffset > 0)
            {
                handler.Seek(Header.extraHashesOffset);
                ExtraHashes.Read(handler);
            }

            return true;
        }

        protected override bool DoWrite()
        {
            var handler = FileHandler;
            var version = FileHandler.FileVersion;
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
            Header.jointExtraGroupDataOffset = handler.Tell();
            ExtraJointGroups.Write(handler);

            if (ExtraJoints.Joints.Count > 0) {
                handler.Align(16);
                Header.extraJointsOffset = handler.Tell();
                ExtraJoints.Write(handler);
            } else {
                Header.extraJointsOffset = 0;
            }

            if (version >= 19)
            {
                handler.Align(16);
                Header.ikMotionDataOffset = handler.Tell();
                IkMotionData.Write(handler);

                handler.Align(16);
                Header.symmetryDataOffset = handler.Tell();
                SymmetryData.Write(handler);

                handler.Align(16);
                Header.extraHashesOffset = handler.Tell();
                ExtraHashes.Write(handler);
            }

            handler.StringTableFlush();

            Header.Write(handler, 0);
            return true;
        }
    }

}