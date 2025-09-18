using ReeLib.Common;
using ReeLib.InternalAttributes;
using ReeLib.Jmap;

namespace ReeLib.Jmap
{
    public class JmapHeader : BaseModel
    {
        public uint version;
        public uint magic = JmapFile.Magic;

        public long jointMaskGroupOffset;
        public long jointExtraGroupDataOffset;
        public long extraJointsOffset;
        public long ikMotionDataOffset;
        public long symmetryDataOffset;
        public long extraHashesOffset;
        public uint attributeFlags;
        public int jointMaskGroupCount;

        protected override bool DoRead(FileHandler handler)
        {
            var version = handler.FileVersion;
            if (version >= 17)
            {
                handler.Read(ref version);
                handler.Read(ref magic);
                handler.Read(ref jointMaskGroupOffset);
                handler.Read(ref jointExtraGroupDataOffset);
                handler.Read(ref extraJointsOffset);
                handler.Read(ref ikMotionDataOffset);
                handler.Read(ref symmetryDataOffset);
                if (version >= 19) handler.Read(ref extraHashesOffset);
                handler.Read(ref attributeFlags);
                handler.Read(ref jointMaskGroupCount);
            }
            else
            {
                throw new NotImplementedException();
            }
            return true;
        }

        protected override bool DoWrite(FileHandler handler)
        {
            var version = handler.FileVersion;
            if (version >= 17)
            {
                handler.Write(ref version);
                handler.Write(ref magic);
                handler.Write(ref jointMaskGroupOffset);
                handler.Write(ref jointExtraGroupDataOffset);
                handler.Write(ref extraJointsOffset);
                handler.Write(ref ikMotionDataOffset);
                handler.Write(ref symmetryDataOffset);
                if (version >= 19) handler.Write(ref extraHashesOffset);
                handler.Write(ref attributeFlags);
                handler.Write(ref jointMaskGroupCount);
            }
            else
            {
                throw new NotImplementedException();
            }
            return true;
        }
    }

    [RszGenerate]
    public partial class JointMaskGroup : BaseModel
    {
        public long jointNameHashOffset;
        public long masksOffset;
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
                handler.Write(Start + 0, jointNameHashOffset = handler.Tell());
                handler.WriteArray(jointHashes);
            }
            if (masks != null)
            {
                handler.Write(Start + 8, masksOffset = handler.Tell());
                handler.WriteArray(masks);
            }
            if (weights != null)
            {
                handler.Write(Start + 16, weightsOffset = handler.Tell());
                handler.WriteArray(weights);
            }
        }
    }

    public class ExtraJointGroupData : BaseModel
    {
        /// <summary>
        /// Most likely via.motion.JointExType
        /// </summary>
        public byte[] types = [];
        public byte[] lods = [];
        public long[] data = [];

        protected override bool DoRead(FileHandler handler)
        {
            var typeOffset = handler.Read<long>();
            var lodOffset = handler.Read<long>();
            var dataOffset = handler.Read<long>();
            var count = handler.Read<int>();
            handler.Seek(typeOffset);
            types = handler.ReadArray<byte>(count);
            handler.Seek(lodOffset);
            lods = handler.ReadArray<byte>(count);
            handler.Seek(dataOffset);
            data = handler.ReadArray<long>(count);
            return true;
        }

        protected override bool DoWrite(FileHandler handler)
        {
            handler.Skip(sizeof(long) * 3);
            handler.Write(types.Length);
            handler.Align(16);
            handler.Write(Start, handler.Tell());
            handler.WriteArray(types);
            handler.Write(Start + sizeof(long), handler.Tell());
            handler.WriteArray(lods);
            handler.Write(Start + sizeof(long) * 2, handler.Tell());
            handler.WriteArray(data);
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
            var parentHash = handler.Read<int>();
            var jointHash = handler.Read<int>();
            handler.Read(ref symmetryHash);
            handler.Read(ref flags);
            handler.Read(ref ukn);
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
            handler.Write(ref ukn);
            return true;
        }
    }

    public class ExtraJoints : BaseModel
    {
        public List<ExtraJointInfo> Joints { get; } = new();

        protected override bool DoRead(FileHandler handler)
        {
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
        public List<JointMaskGroup> MaskGroups { get; } = new();
        public ExtraJointGroupData ExtraJointGroups { get; } = new();
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

            if (version < 17)
            {
                throw new NotSupportedException();
            }

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

            if (Header.symmetryDataOffset > 0)
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
            if (version < 17)
            {
                throw new NotSupportedException();
            }
            Header.Write(handler);

            Header.jointMaskGroupOffset = handler.Tell();
            MaskGroups.Write(handler);

            Header.jointExtraGroupDataOffset = handler.Tell();
            ExtraJointGroups.Write(handler);

            Header.extraJointsOffset = handler.Tell();
            ExtraJoints.Write(handler);

            Header.ikMotionDataOffset = handler.Tell();
            IkMotionData.Write(handler);

            Header.symmetryDataOffset = handler.Tell();
            SymmetryData.Write(handler);

            if (version >= 19)
            {
                Header.extraHashesOffset = handler.Tell();
                ExtraHashes.Write(handler);
            }

            Header.Write(handler);
            return true;
        }
    }

}