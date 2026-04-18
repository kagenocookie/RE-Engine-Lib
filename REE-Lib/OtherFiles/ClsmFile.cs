using System.Numerics;
using System.Runtime.InteropServices;
using ReeLib.InternalAttributes;

namespace ReeLib.Clsm
{
    public class Header : ReadWriteModel
    {
        public uint magic = ClsmFile.Magic;
        public int groupCount;
        public int vertexCount;
        public int faceCount;
        public int materialsCount;
        public int bonesCount;
        public WeightType weightType;

        internal long groupsOffset;
        internal long verticesOffset;
        internal long facesOffset;
        internal long dataOffset;
        internal long bonesOffset;
        internal long materialsOffset;

        protected override bool ReadWrite<THandler>(THandler action)
        {
            var version = action.Handler.FileVersion;
            action.Do(ref magic);
            action.Do(version >= 17, ref groupCount);
            action.Do(ref vertexCount);
            action.Do(ref faceCount);
            action.Do(ref materialsCount);
            action.Do(ref bonesCount);
            action.Do(ref weightType);

            if (version >= 17) {
                action.Null(4);
                action.Do(ref groupsOffset);
            }
            action.Do(ref verticesOffset);
            action.Do(ref facesOffset);
            action.Do(ref dataOffset);
            action.Do(ref bonesOffset);
            action.Do(ref materialsOffset);
            return true;
        }
    }

    public class ClsmCollisionMaterial : BaseModel
    {
        public string? name;

        public string? name2;

        protected override bool DoRead(FileHandler handler)
        {
            name = handler.ReadOffsetWStringNullable();
            name2 = handler.ReadOffsetWStringNullable();
            return true;
        }

        protected override bool DoWrite(FileHandler handler)
        {
            if (name == null) {
                handler.Write(0L);
            } else {
                handler.WriteOffsetWString(name);
            }
            if (name2 == null) {
                handler.Write(0L);
            } else {
                handler.WriteOffsetWString(name2);
            }
            return true;
        }

        public override string ToString() => name ?? name2 ?? "---";
    }

    public enum CollisionShapeType : uint
    {
        Sphere = 0,
        Capsule = 1,
        Box = 2,
        TaperedCapsule = 3,
        Plane = 4,
        Max = 5,
        None = 6,
    }

    [RszGenerate, RszAutoReadWrite]
    public partial class ClsmGroup : BaseModel
    {
        public int id;
        public int vertCount;
        public int faceCount;
        public int vertsOffset;
        public int facesOffset;

        [field: RszIgnore]
        internal ClsmFile? File { get; set; }

        public Span<ClsmVertex> Vertices => File!.Vertices.AsSpan(vertsOffset, vertCount);
        public Span<ClsmFace> Faces => File!.Faces.AsSpan(facesOffset, faceCount);
    }

    public class ClsmVertex : ReadWriteModel
    {
        public Vector3 position;

        public ushort[] Indices;
        public float[] Weights;

        private WeightType weightType;

        public ClsmVertex(WeightType weightType)
        {
            this.weightType = weightType;
            if (weightType == WeightType.Weights4) {
                Indices = new ushort[4];
                Weights = new float[4];
            } else {
                Indices = new ushort[8];
                Weights = new float[8];
            }
        }

        protected override bool ReadWrite<THandler>(THandler action)
        {
            action.Do(ref position);
            action.Null(4);
            if (weightType == WeightType.Weights8) {
                action.Do(ref Indices);
                action.Do(ref Weights);
            } else {
                if (action is FileHandlerRead) {
                    var nums = new int[4];
                    action.Do(ref nums);
                    for (int i = 0; i < 4; i++) Indices[i] = (ushort)nums[i];
                } else {
                    for (int i = 0; i < 4; i++) action.Handler.Write((int)Indices[i]);
                }
                action.Do(ref Weights);
            }

            return true;
        }
    }

    [StructLayout(LayoutKind.Explicit, Size = 16)]
    public struct ClsmFace
    {
        [FieldOffset(0)]
        public ushort index1;
        [FieldOffset(2)]
        public ushort index2;
        [FieldOffset(4)]
        public ushort index3;

        [FieldOffset(12)]
        public uint flags;
    }

    [RszGenerate, RszAutoReadWrite]
    public partial class ClsmMaterialInfo : BaseModel
    {
        public int id;
        public uint attributes1;
        public uint attributes2;
        public uint attributes3;

        public override string ToString() => id.ToString();
    }

    public partial class ClsmBone : BaseModel
    {
        public string name = "";
        public uint nameHash;

        public float[] values = new float[16];

        protected override bool DoRead(FileHandler handler)
        {
            name = handler.ReadOffsetWString();
            handler.Read(ref nameHash);
            handler.ReadNull(4);
            handler.ReadArray(values);
            return true;
        }

        protected override bool DoWrite(FileHandler handler)
        {
            handler.WriteOffsetWString(name);
            handler.Write(ref nameHash);
            handler.WriteNull(4);
            handler.WriteArray(values);
            return true;
        }

        public override string ToString() => name;
    }

    public enum WeightType
    {
        Weights4 = 1,
        Weights8 = 2,
    }
}

namespace ReeLib
{
    using ReeLib.Clsm;

    public class ClsmFile(FileHandler fileHandler) : BaseFile(fileHandler)
    {
        public Header Header { get; } = new Header();
        public List<ClsmGroup> Groups { get; set; } = [];
        public ClsmVertex[] Vertices { get; set; } = [];
        public ClsmFace[] Faces { get; set; } = [];
        public List<ClsmMaterialInfo> Materials { get; set; } = [];
        public List<ClsmBone> Bones { get; set; } = [];
        public List<ClsmCollisionMaterial> MaterialNames { get; set; } = [];

        public const int Magic = 0x4D534C43;

        protected override bool DoRead()
        {
            Groups.Clear();
            Materials.Clear();
            Bones.Clear();
            MaterialNames.Clear();

            var handler = FileHandler;
            var header = Header;

            header.Read(handler);
            if (header.magic != Magic)
            {
                throw new Exception("Invalid CLSM file");
            }

            if (handler.FileVersion >= 17) {
                handler.Seek(header.groupsOffset);
                Groups.Read(handler, header.groupCount);
                foreach (var group in Groups) group.File = this;
            }

            handler.Seek(header.verticesOffset);
            Vertices = new ClsmVertex[header.vertexCount];
            for (int i = 0; i < header.vertexCount; i++) {
                var vert = Vertices[i] = new ClsmVertex(header.weightType);
                vert.Read(handler);
            }

            handler.Seek(header.facesOffset);
            handler.ReadArray(Faces);

            handler.Seek(header.dataOffset);
            Materials.Read(handler, header.materialsCount);

            handler.Seek(header.bonesOffset);
            Bones.Read(handler, header.bonesCount);

            handler.Seek(header.materialsOffset);
            MaterialNames.Read(handler, header.materialsCount);

            return true;
        }

        protected override bool DoWrite()
        {
            var handler = FileHandler;
            var header = Header;

            header.Write(handler);
            if (header.magic != Magic)
            {
                throw new Exception("Invalid CLSM file");
            }

            if (handler.FileVersion >= 17) {
                header.groupsOffset = handler.Tell();
                Groups.Write(handler);
                handler.Align(16);
            }

            header.verticesOffset = handler.Tell();
            Vertices.Write(handler);

            header.facesOffset = handler.Tell();
            handler.WriteArray(Faces);

            header.dataOffset = handler.Tell();
            Materials.Write(handler);

            header.bonesOffset = handler.Tell();
            Bones.Write(handler);

            header.materialsOffset = handler.Tell();
            MaterialNames.Write(handler);

            handler.StringTableFlush();

            header.Rewrite(handler);
            return true;
        }
    }
}