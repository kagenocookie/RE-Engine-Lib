using System.Numerics;
using System.Runtime.InteropServices;
using ReeLib.Aimp;
using ReeLib.Common;
using ReeLib.InternalAttributes;
using ReeLib.via;

namespace ReeLib.Aimp
{
    public enum AimpFormat
    {
        Invalid,

        /// <summary>
        /// RE7
        /// </summary>
        Format8 = 8,

        /// <summary>
        /// DMC5, RE2(27), RE3
        /// </summary>
        Format28 = 28,

        /// <summary>
        /// RE3 RT, RE2 RT, RE7 RT, RE8, MHRise
        /// </summary>
        Format41 = 41,

        /// <summary>
        /// RE4, SF6
        /// </summary>
        Format43 = 43,

        /// <summary>
        /// DD2, MHWilds
        /// </summary>
        Format46 = 46,
    }

    public enum ContentGroupType
    {
        ContentGroupMapPoint,
        ContentGroupTriangle,
        ContentGroupPolygon,
        ContentGroupMapBoundary,
        ContentGroupMapAABB,
    }

    public enum SectionType
    {
        NoSection = 0,
        Owner = 1,
        Section = 2,
        ConnectManager = 3,
        IndividualSection = 4,
        Invalid = -1,
    }

    public enum MapType
    {
        Navmesh = 0,
	    WayPoint = 1,
	    VolumeSpace = 2,
	    NoMap = 3,
    }

    public class AimpHeader : BaseModel
    {
        public uint magic = AimpFile.Magic;
        public string? name;
        public string? hash;
        public MapType mapType;
        public SectionType sectionType;
        public Guid guid;
        public float agentRadWhenBuild;
        public ulong uriHash;
        public int uknId; // usually 1, sometimes 0 or 2; not related to embedded data nor is it the sectionID

        public long layersOffset;
        public long rszOffset;
        public long embeddedContentOffset;

        public long contentGroup1Offset;
        public long group1DataOffset;
        public long contentGroup2Offset;
        public long group2DataOffset;

        public AimpFormat Version { get; }

        public AimpHeader(AimpFormat version)
        {
            Version = version;
            if (version == AimpFormat.Invalid) {
                throw new Exception("Invalid AIMP file format");
            }
        }

        protected override bool DoRead(FileHandler handler)
        {
            handler.Read(ref magic);
            name = handler.ReadInlineWString();
            if (Version > AimpFormat.Format41) {
                hash = handler.ReadInlineWString();
            }

            mapType = (MapType)handler.ReadByte();
            sectionType = (SectionType)handler.ReadByte();
            handler.ReadNull(2);

            if (Version >= AimpFormat.Format46) {
                handler.Read(ref agentRadWhenBuild);
                handler.Read(ref uriHash);
                handler.Read(ref uknId);
            } else if (Version >= AimpFormat.Format28) {
                handler.Read(ref guid);
                handler.Read(ref uknId);
            }

            handler.Read(ref layersOffset);
            if (Version >= AimpFormat.Format28) {
                handler.Read(ref rszOffset);
                handler.Read(ref embeddedContentOffset);
            }

            handler.Read(ref contentGroup1Offset);
            handler.Read(ref group1DataOffset);
            handler.Read(ref contentGroup2Offset);
            handler.Read(ref group2DataOffset);
            return true;
        }

        protected override bool DoWrite(FileHandler handler)
        {
            handler.Write(ref magic);
            handler.WriteInlineWString(name ??= "");
            if (Version > AimpFormat.Format41) {
                handler.WriteInlineWString(hash ??= Guid.NewGuid().ToString().Replace("-", ""));
            }

            handler.WriteByte((byte)mapType);
            handler.WriteByte((byte)sectionType);
            handler.WriteNull(2);

            if (Version >= AimpFormat.Format46) {
                handler.Write(ref agentRadWhenBuild);
                handler.Write(ref uriHash);
                handler.Write(ref uknId);
            } else if (Version >= AimpFormat.Format28) {
                handler.Write(ref guid);
                handler.Write(ref uknId);
            }

            handler.Write(ref layersOffset);
            if (Version >= AimpFormat.Format28) {
                handler.Write(ref rszOffset);
                handler.Write(ref embeddedContentOffset);
            }

            handler.Write(ref contentGroup1Offset);
            handler.Write(ref group1DataOffset);
            handler.Write(ref contentGroup2Offset);
            handler.Write(ref group2DataOffset);
            return true;
        }
    }

    public class NodeInfoAttributes
    {
        public byte[] attributeIds = [];
        public float[] values = [];

        public void Init(int count)
        {
            attributeIds = new byte[count];
            values = new float[count];
        }

        public void Read(int count, FileHandler handler, AimpFormat version)
        {
            if (version < AimpFormat.Format28)
            {
                ReadRE7(count, handler);
            }
            else
            {
                Read(count, handler);
            }
        }
        public void Write(FileHandler handler, AimpFormat version)
        {
            if (version < AimpFormat.Format28)
            {
                WriteRE7(handler);
            }
            else
            {
                Write(handler);
            }
        }
        public void Read(int count, FileHandler handler)
        {
            attributeIds = handler.ReadArray<byte>(count);
            handler.Align(4);
            values = handler.ReadArray<float>(count);
        }

        public void ReadRE7(int count, FileHandler handler)
        {
            // note: values are actually ints here but it's fine
            values = handler.ReadArray<float>(count);
        }

        public void Write(FileHandler handler)
        {
            handler.WriteArray(attributeIds!);
            handler.Align(4);
            handler.WriteArray(values!);
        }

        public void WriteRE7(FileHandler handler)
        {
            foreach (var f in values!)
            {
                var num = BitConverter.SingleToInt32Bits(f);
                handler.Write(num);
            }
        }
    }

    public class TriangleNode
    {
        public int index1;
        public int index2;
        public int index3;
        public NodeInfoAttributes attributes = new();

        public void Read(FileHandler handler, AimpFormat format)
        {
            handler.Read(ref index1);
            handler.Read(ref index2);
            handler.Read(ref index3);
            attributes.Read(3, handler, format);
        }

        public void Write(FileHandler handler, AimpFormat format)
        {
            handler.Write(ref index1);
            handler.Write(ref index2);
            handler.Write(ref index3);
            attributes.Write(handler, format);
        }
    }

    public class PolygonNode
    {
        public int pointCount;
        public int[] indices = [];
        public NodeInfoAttributes attributes = new();
        public Vector3 min;
        public Vector3 max;

        public AimpFormat version;

        public void Read(FileHandler handler)
        {
            handler.Read(ref pointCount);
            indices = handler.ReadArray<int>(pointCount);
            attributes.Read(pointCount, handler, version);
            handler.Read(ref min);
            handler.Read(ref max);
        }

        public void Write(FileHandler handler)
        {
            handler.Write(ref pointCount);
            handler.WriteArray<int>(indices);
            if (version >= AimpFormat.Format28) {
                attributes.Write(handler);
            } else {
                attributes.WriteRE7(handler);
            }
            handler.Write(ref min);
            handler.Write(ref max);
        }
    }

    public class AABBNode
    {
        // there are a few rare cases where it's not not just 2 indices duplicated twice (aivspc)
        public short[] indices = new short[4];
        // these values also seem to only ever be not 0 in volume space maps (see DMC5 m02_340c_start.aimap.28, RE4 loc5500_ao.aivspc.6)
        public float[]? values;
        public int value;
    }

    public struct MapBoundaryNode
    {
        public int[] indices;
        public Vector3 min;
        public Vector3 max;
    }

    [RszGenerate]
    public partial class WallNode
    {
        public via.mat4 matrix;
        public PaddedVec3 scale;
        public Quaternion rotation;
        public PaddedVec3 position;
        [RszFixedSizeArray(8)] public int[] indices = new int[8];

        public void Read(FileHandler handler) => DefaultRead(handler);
        public void Write(FileHandler handler) => DefaultWrite(handler);
    }

    [StructLayout(LayoutKind.Explicit, Size = 16)]
    public struct PaddedVec3
    {
        [FieldOffset(0)] public float x;
        [FieldOffset(4)] public float y;
        [FieldOffset(8)] public float z;

        public PaddedVec3(float x, float y, float z)
        {
            this.x = x;
            this.y = y;
            this.z = z;
        }

        public Vector3 Vector3 => new Vector3(x, y, z);
        public override string ToString() => $"{x}, {y}, {z}";
    }

    public class NodeTypeData
    {
        public int[] type1 = [];
        public int[] type2 = [];
        public int[] type3 = [];
        public int[] type4 = [];
        public int TotalCount => (type1?.Length ?? 0) + (type2?.Length ?? 0) + (type3?.Length ?? 0) + (type4?.Length ?? 0);

        public void Read(FileHandler handler)
        {
            var c1 = handler.Read<int>();
            var c2 = handler.Read<int>();
            var c3 = handler.Read<int>();
            var c4 = handler.Read<int>();
            type1 = handler.ReadArray<int>(c1);
            type2 = handler.ReadArray<int>(c2);
            type3 = handler.ReadArray<int>(c3);
            type4 = handler.ReadArray<int>(c4);
        }

        public void Write(FileHandler handler)
        {
            handler.Write(type1.Length);
            handler.Write(type2.Length);
            handler.Write(type3.Length);
            handler.Write(type4.Length);
            handler.WriteArray(type1);
            handler.WriteArray(type2);
            handler.WriteArray(type3);
            handler.WriteArray(type4);
        }
    }

    public abstract class ContentGroup
    {
        public AimpFormat format;

        public abstract int NodeCount { get; }
        public string Classname => "via.navigation.map." + GetType().Name;

        public abstract bool ReadNodes(FileHandler handler, int count);
        public abstract bool ReadData(FileHandler handler);
        public abstract void WriteData(FileHandler handler);
        public abstract void WriteNodes(FileHandler handler);

        public abstract Vector3 GetNodeCenter(ContentGroupContainer container, int i);

        public static ContentGroup Create(string classname, AimpFormat format)
        {
            ContentGroup content = classname switch {
                "via.navigation.map.ContentGroupMapPoint" => new ContentGroupMapPoint(),
                "via.navigation.map.ContentGroupTriangle" => new ContentGroupTriangle(),
                "via.navigation.map.ContentGroupPolygon" => new ContentGroupPolygon(),
                "via.navigation.map.ContentGroupMapBoundary" => new ContentGroupMapBoundary(),
                "via.navigation.map.ContentGroupMapAABB" => new ContentGroupMapAABB(),
                "via.navigation.map.ContentGroupWall" => new ContentGroupWall(),
                _ => throw new NotImplementedException("Unknown content group type " + classname),
            };
            content.format = format;
            return content;
        }
    }

    public abstract class ContentGroup<TNode> : ContentGroup
    {
        public List<TNode> Nodes { get; } = new();

        public override int NodeCount => Nodes.Count;
    }

    [RszGenerate]
    public partial class NodeInfo
    {
        public int groupIndex;
        public int index;
        public int flags; // found bits: 1, 2, 16, 32; combinations: 0, 1, 2, 16, 18, 19, 34; LinkBoundary? Wall?
        public ulong attributes;
        public int userdataIndex;
        public int linkCount;
        public int nextIndex;

        [field: RszIgnore] public List<LinkInfo> Links { get; } = new();
        [field: RszIgnore] public RszInstance? UserData { get; set; }

        public Color GetColor(AimpFile file) => attributes == 0 ? new Color(0xffffffff) : file.layers![BitOperations.TrailingZeroCount(attributes)].color;

        public void Read(FileHandler handler) => DefaultRead(handler);
        public void Write(FileHandler handler) => DefaultWrite(handler);

        public NodeInfoRE7 AsRE7() => new NodeInfoRE7() { flags = flags, index = index, index2 = index, attributes = attributes };
    }

    public struct NodeInfoRE7
    {
        public int index;
        public int zero;
        public int index2; // equal to index
        public int flags;
        public ulong attributes;

        public readonly NodeInfo Upgrade() => new NodeInfo() { flags = flags, index = index, attributes = attributes, nextIndex = index + 1 };
    }

    [StructLayout(LayoutKind.Sequential, Pack = 4)]
    public struct LinkInfo
    {
        public int index;
        public int sourceNodeIndex;
        public int targetNodeIndex;
        public int ukn1; // if 2 => 2-way
        public ulong attributes;
        public int ukn2; // this is sometimes int, sometimes float...
    }

    public struct IndexSet
    {
        public int[] indices;

        public override string ToString() => string.Join(", ", indices ?? Array.Empty<int>());
    };

    public class NodeData
    {
        public NodeInfo[] Nodes = [];
        public int maxIndex;
        public int minIndex;

        private int[]? _effectiveNodeIndices;
        public int[] EffectiveNodeIndices
        {
            get
            {
                if (_effectiveNodeIndices == null)
                {
                    _effectiveNodeIndices = new int[maxIndex + 1];
                    int offsetIndex = 0;
                    for (int i = 0; i < Nodes.Length; i++) {
                        var node = Nodes[i];
                        // I'm not quite sure what the point of these extra indices is... maybe they generate lerped gap points for wayp files?
                        while (offsetIndex < node.nextIndex && offsetIndex < _effectiveNodeIndices.Length) {
                            _effectiveNodeIndices[offsetIndex++] = i;
                        }
                    }
                }
                return _effectiveNodeIndices;
            }
        }

        public void Read(FileHandler handler, AimpFormat format)
        {
            var nodeCount = handler.Read<int>();
            if (format >= AimpFormat.Format28) {
                handler.Read(ref maxIndex);
            }
            if (format >= AimpFormat.Format41) {
                handler.Read(ref minIndex);
            }

            Nodes = new NodeInfo[nodeCount];

            if (format >= AimpFormat.Format28)
            {
                for (int i = 0; i < nodeCount; ++i)
                {
                    var node = new NodeInfo();
                    node.Read(handler);
                    Nodes[i] = node;
                }

                foreach (var node in Nodes)
                {
                    for (int i = 0; i < node.linkCount; ++i)
                    {
                        node.Links.Add(handler.Read<LinkInfo>());
                    }
                }
            }
            else
            {
                for (int i = 0; i < nodeCount; ++i)
                {
                    var node = handler.Read<NodeInfoRE7>().Upgrade();
                    Nodes[i] = node;
                }
                int linkCount = handler.Read<int>();
                for (int i = 0; i < linkCount; ++i)
                {
                    var link = handler.Read<LinkInfo>();
                    Nodes[link.sourceNodeIndex].Links.Add(link);
                }
                // convenience to simplify usage - integration can also assume it's there and valid
                maxIndex = linkCount;
            }
        }

        public void Write(FileHandler handler, AimpFormat format)
        {
            handler.Write(Nodes.Length);
            if (format >= AimpFormat.Format28) handler.Write(ref maxIndex);
            if (format >= AimpFormat.Format41) handler.Write(ref minIndex);

            if (format >= AimpFormat.Format28) {
                foreach (var node in Nodes) node.Write(handler);
            } else {
                var linkCount = 0;
                foreach (var node in Nodes)
                {
                    handler.Write(node.AsRE7());
                    linkCount += node.Links.Count;
                }
                handler.Write(linkCount);
            }

            foreach (var node in Nodes)
            {
                foreach (var link in node.Links)
                {
                    handler.Write(link);
                }
            }
        }
    }

    public class ContentGroupMapPoint : ContentGroup<ContentGroupMapPoint.Point>
    {
        public int[] indexData = [];

        public override Vector3 GetNodeCenter(ContentGroupContainer container, int i) => Nodes[i].pos;

        public struct Point
        {
            public Vector3 pos;
            public Vector3 normal;

            public override string ToString() => pos.ToString();
        }

        public override bool ReadNodes(FileHandler handler, int count)
        {
            if (format >= AimpFormat.Format28) {
                Nodes.ReadStructList(handler, count);
            } else {
                Nodes.Clear();
                for (int i = 0; i < count; ++i) Nodes.Add(new Point() { pos = handler.Read<Vector3>() });
            }
            return true;
        }

        public override void WriteNodes(FileHandler handler)
        {
            if (format >= AimpFormat.Format28) {
                Nodes.Write(handler);
            } else {
                foreach (var pt in Nodes) handler.Write(pt.pos);
            }
        }

        public override bool ReadData(FileHandler handler)
        {
            indexData = handler.ReadArray<int>(Nodes.Count);
            return true;
        }

        public override void WriteData(FileHandler handler)
        {
            handler.WriteArray(indexData);
        }
    }

    public class ContentGroupTriangle : ContentGroup<TriangleNode>
    {
        public int[] polygonIndices = [];

        public override Vector3 GetNodeCenter(ContentGroupContainer container, int i) => (
            container.Vertices[Nodes[i].index1].Vector3 +
            container.Vertices[Nodes[i].index2].Vector3 +
            container.Vertices[Nodes[i].index3].Vector3
        ) * 0.333f;

        public override bool ReadNodes(FileHandler handler, int count)
        {
            Nodes.Clear();
            for (int i = 0; i < count; ++i)
            {
                var node = new TriangleNode();
                node.Read(handler, format);
                Nodes.Add(node);
            }
            return true;
        }

        public override void WriteNodes(FileHandler handler)
        {
            foreach (var node in Nodes) node.Write(handler, format);
        }

        public override bool ReadData(FileHandler handler)
        {
            polygonIndices = handler.ReadArray<int>(Nodes.Count);
            return true;
        }

        public override void WriteData(FileHandler handler)
        {
            handler.WriteArray(polygonIndices);
        }
    }

    public class ContentGroupPolygon : ContentGroup<PolygonNode>
    {
        public IndexSet[] triangleIndices = [];

        public override Vector3 GetNodeCenter(ContentGroupContainer container, int i) => container.Vertices[Nodes[i].indices[0]].Vector3;

        public override bool ReadNodes(FileHandler handler, int count)
        {
            Nodes.Clear();
            for (int i = 0; i < count; ++i) {
                var n = new PolygonNode() { version = format };
                n.Read(handler);
                Nodes.Add(n);
            }
            return true;
        }

        public override void WriteNodes(FileHandler handler)
        {
            foreach (var node in Nodes) node.Write(handler);
        }

        public override bool ReadData(FileHandler handler)
        {
            triangleIndices = new IndexSet[Nodes.Count];
            for (int i = 0; i < triangleIndices.Length; ++i) {
                triangleIndices[i] = new IndexSet() {
                    indices = handler.ReadArray<int>(handler.Read<int>()),
                };
            }
            return true;
        }

        public override void WriteData(FileHandler handler)
        {
            foreach (var item in triangleIndices)
            {
                handler.Write(item.indices.Length);
                handler.WriteArray(item.indices);
            }
        }
    }

    public class ContentGroupMapBoundary : ContentGroup<ContentGroupMapBoundary.MapBoundaryNodeInfo>
    {
        public int[] integers = []; // always all -1

        public class MapBoundaryNodeInfo
        {
            public int[] indices = new int[8];
            public Vector3 min, max;
        }

        public override Vector3 GetNodeCenter(ContentGroupContainer container, int i) => (Nodes[i].min + Nodes[i].max) * 0.5f;

        public override bool ReadNodes(FileHandler handler, int count)
        {
            Nodes.Clear();
            for (int i = 0; i < count; ++i) {
                var bi = new MapBoundaryNodeInfo() {
                    indices = handler.ReadArray<int>(8),
                    min = handler.Read<Vector3>(),
                    max = handler.Read<Vector3>(),
                };
                Nodes.Add(bi);
            }
            return true;
        }

        public override void WriteNodes(FileHandler handler)
        {
            foreach (var node in Nodes)
            {
                handler.WriteArray(node.indices);
                handler.Write(node.min);
                handler.Write(node.max);
            }
        }

        public override bool ReadData(FileHandler handler)
        {
            integers = handler.ReadArray<int>(Nodes.Count);
            return true;
        }

        public override void WriteData(FileHandler handler)
        {
            handler.WriteArray(integers);
        }
    }

    public class ContentGroupMapAABB : ContentGroup<AABBNode>
    {
        public IndexSet[] data = [];

        public override Vector3 GetNodeCenter(ContentGroupContainer container, int i) => container.Vertices[Nodes[i].indices[0]].Vector3;

        public override bool ReadNodes(FileHandler handler, int count)
        {
            Nodes.Clear();
            for (int i = 0; i < count; ++i) {
                var item = new AABBNode();
                handler.ReadArray(item.indices);
                if (format > AimpFormat.Format28) {
                    item.values = handler.ReadArray<float>(12);
                    item.value = handler.ReadInt();
                }
                Nodes.Add(item);
            }
            return true;
        }

        public override void WriteNodes(FileHandler handler)
        {
            foreach (var item in Nodes)
            {
                handler.WriteArray(item.indices);
                if (format > AimpFormat.Format28) {
                    handler.WriteArray(item.values ??= new float[12]);
                    handler.Write(item.value);
                }
            }
        }


        public override bool ReadData(FileHandler handler)
        {
            data = new IndexSet[Nodes.Count];
            for (int i = 0; i < data.Length; ++i) {
                data[i] = new IndexSet() {
                    indices = handler.ReadArray<int>(handler.Read<int>()),
                };
            }
            return true;
        }

        public override void WriteData(FileHandler handler)
        {
            foreach (var item in data)
            {
                handler.Write(item.indices.Length);
                handler.WriteArray(item.indices);
            }
        }
    }
    public class ContentGroupWall : ContentGroup<WallNode>
    {
        public OffsetData[] data = [];

        public override Vector3 GetNodeCenter(ContentGroupContainer container, int i) => container.Vertices[Nodes[i].indices[0]].Vector3;

        public struct OffsetData { public uint mask; }; // re4: 1x uint; before was saved as 2x uint -- why? dd2+ change?

        public override bool ReadNodes(FileHandler handler, int count)
        {
            Nodes.Clear();
            for (int i = 0; i < count; ++i) {
                var node = new WallNode();
                node.Read(handler);
                Nodes.Add(node);
            }
            return true;
        }

        public override void WriteNodes(FileHandler handler)
        {
            foreach (var node in Nodes) node.Write(handler);
        }

        public override bool ReadData(FileHandler handler)
        {
            data = handler.ReadArray<OffsetData>(Nodes.Count);
            return true;
        }

        public override void WriteData(FileHandler handler)
        {
            handler.WriteArray(data);
        }
    }

    public class ContentGroupContainer : BaseModel
    {
        public ContentGroup[] contents = [];

        public PaddedVec3[] Vertices = [];
        public NodeData Nodes = new();

        // aiwayp, aivspc: float1,2 = 1f, 0f
        // ainvm: float2 always 1.396263f
        // aimap:
        // *, 1.396263f
        // 5.936347, 1.4835299
        // 9.68779, 1.4835299
        // 0, 0
        // 3.323435, 0.7853982
        // float2 is always same between both content groups
        // one of them may be a cell size? margin? cell height? max region area? some sort of distance?

        public float float1;
        public float float2;
        public AABB bounds;

        private Vector3[]? _nodeOrigins;
        public Vector3[] NodeOrigins
        {
            get {
                if (_nodeOrigins == null)
                {
                    _nodeOrigins = new Vector3[Nodes.Nodes.Length];
                    int offset = 0;
                    foreach (var content in contents)
                    {
                        for (int i = 0; i < content.NodeCount; ++i)
                        {
                            _nodeOrigins[offset + i] = content.GetNodeCenter(this, i);
                        }
                    }
                }
                return _nodeOrigins;
            }
        }

        // for paired content group types, this is only ever present on group 2 (always the last group)
        // each sub array contains indices of the current group nodes, one node can be in multiple
        public NodeTypeData? QuadData;
        public AimpFormat version;

        internal AimpHeader header = null!;

        public ContentGroupContainer(AimpFormat version)
        {
            this.version = version;
        }

        private ContentGroup ReadContentHeader(FileHandler handler)
        {
            var classname = handler.ReadInlineWString();
            int count = handler.Read<int>();
            var content = ContentGroup.Create(classname, version);
            content.ReadNodes(handler, count);
            return content;
        }

        protected override bool DoRead(FileHandler handler)
        {
            var contentCount = handler.Read<int>();
            var firstHeaderOffset = handler.Tell();
            if (contentCount == 0) {
                contents = Array.Empty<ContentGroup>();
            } else {
                contents = new ContentGroup[contentCount];

                for (int i = 0; i < contentCount; ++i) {
                    contents[i] = ReadContentHeader(handler);
                }
            }

            handler.ReadNull(4);

            Vertices = handler.ReadArray<PaddedVec3>(handler.Read<int>());
            Nodes.Read(handler, version);

            // why the hell does this exist
            // maybe it's supposed to be a section type override, so a file could contain multiple section types?
            // leaving my original "hasMysteryPadding" hack condition here in case we need it again and the section type isn't the true indicator of its presence
            // var hasMysteryPadding = version > AimpFormat.Format8 && (handler.Read<int>(handler.Tell() + 36) != 0 || handler.Read<int>(handler.Tell() + 20) != 0);
            if (header.Version > AimpFormat.Format8 && header.sectionType == SectionType.NoSection) {
                handler.ReadNull(4);
            }

            handler.Read(ref float1);
            handler.Read(ref float2);

            if (version > AimpFormat.Format8) {
                handler.Read(ref bounds);
                QuadData ??= new();
                QuadData.Read(handler);
            }

            return true;
        }

        protected override bool DoWrite(FileHandler handler)
        {
            handler.Write(contents.Length);
            foreach (var content in contents) {
                handler.WriteInlineWString(content.Classname);
                handler.Write(content.NodeCount);
                content.WriteNodes(handler);
            }

            handler.WriteNull(4);
            handler.Write(Vertices.Length);
            handler.WriteArray(Vertices);
            Nodes.Write(handler, version);
            if (header.Version > AimpFormat.Format8 && header.sectionType == SectionType.NoSection) {
                handler.WriteNull(4);
            }

            handler.Write(ref float1);
            handler.Write(ref float2);
            if (version > AimpFormat.Format8) {
                handler.Write(ref bounds);
                QuadData ??= new();
                QuadData.Write(handler);
            }

            return true;
        }

        public void ReadData(FileHandler handler)
        {
            foreach (var group in contents) {
                group.ReadData(handler);
            }
        }

        public void WriteData(FileHandler handler)
        {
            foreach (var group in contents) {
                group.WriteData(handler);
            }
        }
    }

    public class MapLayers
    {
        public uint nameHash;
        public string? name;
        public Color color;

        public void Read(FileHandler handler, AimpFormat format)
        {
            if (format <= AimpFormat.Format28) {
                handler.Read(ref nameHash);
            } else {
                name = handler.ReadInlineWString();
            }
            handler.Read(ref color);
        }

        public void Write(FileHandler handler, AimpFormat format)
        {
            if (format <= AimpFormat.Format28) {
                handler.Write(ref nameHash);
            } else {
                handler.WriteInlineWString(name ??= "");
            }
            handler.Write(ref color);
        }

        public override string ToString() => $"{name ?? nameHash.ToString()}: {color}";
    }

    public class EmbeddedGroupData
    {
        public struct FlaggedIndex
        {
            public short index;
            public byte indexUpperByte;
            public byte flag;
        }

        [StructLayout(LayoutKind.Sequential, Pack = 1)]
        public struct EmbedIndexedPosition
        {
            // one of these unknowns could be a userdata index; not enough sample files to make a good conclusion
            // index is equal to index2 _most_ of the time but not always
            public FlaggedIndex index;
            public int ukn1;
            public int index2;
            public int zero2;
            public int zero3;
            public int zero4;
            public int ukn2;
            public int ukn3;
        }

        public struct EmbedIndexedTriangle
        {
            public int index;

            public FlaggedIndex flagIndex1;
            public FlaggedIndex flagIndex2;

            public int ukn1;
            public int ukn2_zero;
            public int ukn3_zero;
            public int ukn4;
        }

        public struct Data2
        {
            public int num1, num2;
        }

        public ContentGroup? contentGroup;

        public EmbedIndexedPosition[] positions1 = [];
        public PaddedVec3[] vertices = [];
        public AABB bounds;

        // the main index between the 3 data groups seems to increment across all of them
        public EmbedIndexedTriangle[] data1 = [];
        public EmbedIndexedTriangle[] data2 = [];
        public EmbedIndexedTriangle[] data3 = [];
        // these might be wrong
        public Data2[]? data4;
        public int[]? data5; // unknown

        public void Read(FileHandler handler, AimpFormat format)
        {
            var nodeCount = handler.Read<int>();
            if (nodeCount > 0) {
                var classname = handler.ReadInlineWString();
                contentGroup = ContentGroup.Create(classname, format);
                contentGroup.ReadNodes(handler, nodeCount);
            }

            positions1 = handler.ReadArray<EmbedIndexedPosition>(handler.ReadInt());
            // TODO: this doesn't seem quite correct in all cases (specifically when nodeCount == 0, they aren't always floats - see dd2 aimapwaypoint_event_11)
            // could also be a data4 interpretation issue

            vertices = handler.ReadArray<PaddedVec3>(handler.ReadInt());
            handler.Read(ref bounds);
            data1 = handler.ReadArray<EmbedIndexedTriangle>(handler.ReadInt());
            data2 = handler.ReadArray<EmbedIndexedTriangle>(handler.ReadInt());
            data3 = handler.ReadArray<EmbedIndexedTriangle>(handler.ReadInt());
            if (format >= AimpFormat.Format41) {
                data4 = handler.ReadArray<Data2>(handler.ReadInt());
                data5 = handler.ReadArray<int>(handler.ReadInt());
                if (data4.Length > 0) Log.Info("Found data4 " + handler.FilePath);
                if (data5.Length > 0) throw new Exception("Found unhandled data5 embed data");
            }

#if DEBUG
            foreach (var p in positions1) {
                DataInterpretationException.DebugThrowIf(p.index.indexUpperByte != 0);
                DataInterpretationException.DebugThrowIf(p.zero2 != 0);
                DataInterpretationException.DebugThrowIf(p.zero3 != 0);
                DataInterpretationException.DebugThrowIf(p.zero4 != 0);
            }

            foreach (var tri in data1) {
                DataInterpretationException.DebugThrowIf(tri.ukn2_zero != 0 || tri.ukn3_zero != 0 || tri.flagIndex1.indexUpperByte != 0 || tri.flagIndex2.indexUpperByte != 0);
            }
            foreach (var tri in data2) {
                DataInterpretationException.DebugThrowIf(tri.ukn2_zero != 0 || tri.ukn3_zero != 0 || tri.flagIndex1.indexUpperByte != 0 || tri.flagIndex2.indexUpperByte != 0);
            }
            foreach (var tri in data3) {
                DataInterpretationException.DebugThrowIf(tri.ukn2_zero != 0 || tri.ukn3_zero != 0 || tri.flagIndex1.indexUpperByte != 0 || tri.flagIndex2.indexUpperByte != 0);
            }
#endif
        }

        public void Write(FileHandler handler, AimpFormat format)
        {
            handler.Write(contentGroup?.NodeCount ?? 0);
            if (contentGroup != null)
            {
                handler.WriteInlineWString(contentGroup.Classname);
                contentGroup.WriteNodes(handler);
            }

            handler.Write(positions1.Length);
            handler.WriteArray(positions1);
            handler.Write(vertices.Length);
            handler.WriteArray(vertices);
            handler.Write(ref bounds);
            handler.Write(data1.Length);
            handler.WriteArray(data1);
            handler.Write(data2.Length);
            handler.WriteArray(data2);
            handler.Write(data3.Length);
            handler.WriteArray(data3);
            if (format > AimpFormat.Format28) {
                data4 ??= [];
                data5 ??= [];
                handler.Write(data4.Length);
                handler.WriteArray(data4);
                handler.Write(data5.Length);
                handler.WriteArray(data5);
            }
        }
    }

    public class EmbeddedMap
    {
        public ulong hash1;
        public ulong hash2;

        public string? key;

        public int[] preData1 = [];
        public int[] preData2 = [];

        public EmbeddedGroupData data1 = new();
        public EmbeddedGroupData? data2;

        public void Read(FileHandler handler, AimpFormat format)
        {
            handler.Read(ref hash1);
            if (format <= AimpFormat.Format41) {
                handler.Read(ref hash2);
            }
            preData1 = handler.ReadArray<int>(handler.ReadInt());
            preData2 = handler.ReadArray<int>(handler.ReadInt());
            data1.Read(handler, format);
            if (format >= AimpFormat.Format41) {
                data2 ??= new();
                data2.Read(handler, format);
                if (format > AimpFormat.Format41) {
                    key = handler.ReadInlineWString();
                }
            }
        }

        public void Write(FileHandler handler, AimpFormat format)
        {
            handler.Write(ref hash1);
            if (format <= AimpFormat.Format41)
            {
                handler.Write(ref hash2);
            }
            handler.Write(preData1.Length);
            handler.WriteArray(preData1);
            handler.Write(preData2.Length);
            handler.WriteArray(preData2);

            data1.Write(handler, format);
            if (format >= AimpFormat.Format41) {
                data2 ??= new();
                data2.Write(handler, format);
                if (format > AimpFormat.Format41) {
                    handler.WriteInlineWString(key ??= "");
                }
            }
        }

        public override string ToString() => key ?? $"{hash1} {hash2}";
    }
}


namespace ReeLib
{
    public class AimpFile : BaseRszFile
    {
        public enum AimpType
        {
            Invalid,
            Map,
            Navmesh,
            Waypoint,
            VolumeSpace,
            WaypointManager,
            NavmeshManager,
        }

        public AimpHeader Header { get; }
        public RSZFile RSZ { get; }

        public ContentGroupContainer? mainContent;
        public ContentGroupContainer? secondaryContent;
        public MapLayers[]? layers;

        public ulong embedHash1;
        public ulong embedHash2;

        public EmbeddedMap[] embeds = Array.Empty<EmbeddedMap>();

        public AimpFile(RszFileOption option, FileHandler fileHandler, AimpType type) : base(option, fileHandler)
        {
            Header = new(DetermineFileFormatFromFileType(type, fileHandler.FileVersion));
            RSZ = new RSZFile(option, fileHandler);
        }

        public AimpFile(RszFileOption option, FileHandler fileHandler)
            : this(option, fileHandler, DetermineMapTypeFromFilename(fileHandler.FilePath))
        {
        }

        private static AimpFormat DetermineFileFormatFromFileType(AimpType type, int fileVersion)
        {
            // the file format itself is identical between several files, but they have different versioning
            // map file versions into a common enum to simplify later versioning logic
            return type switch {
                AimpType.Map => fileVersion switch {
                    8 => AimpFormat.Format8,
                    27 => AimpFormat.Format28,
                    28 => AimpFormat.Format28,
                    34 => AimpFormat.Format28,
                    41 => AimpFormat.Format41,
                    43 => AimpFormat.Format43,
                    > 43 => AimpFormat.Format46,
                    _ => throw new Exception($"Unsupported AIMP file format {type} {fileVersion}"),
                },
                AimpType.Navmesh => fileVersion switch {
                    8 => AimpFormat.Format41,
                    14 => AimpFormat.Format41,
                    17 => AimpFormat.Format43,
                    18 => AimpFormat.Format43,
                    >= 30 => AimpFormat.Format46,
                    _ => throw new Exception($"Unsupported AIMP file format {type} {fileVersion}"),
                },
                AimpType.Waypoint => fileVersion switch {
                    3 => AimpFormat.Format41,
                    5 => AimpFormat.Format43,
                    >= 6 => AimpFormat.Format46,
                    _ => throw new Exception($"Unsupported AIMP file format {type} {fileVersion}"),
                },
                AimpType.VolumeSpace => fileVersion switch {
                    4 => AimpFormat.Format41,
                    6 => AimpFormat.Format43,
                    > 6 => AimpFormat.Format46,
                    _ => throw new Exception($"Unsupported AIMP file format {type} {fileVersion}"),
                },
                AimpType.WaypointManager => fileVersion switch {
                    >= 8 => AimpFormat.Format46,
                    _ => throw new Exception($"Unsupported AIMP file format {type} {fileVersion}"),
                },
                AimpType.NavmeshManager => fileVersion switch {
                    >= 8 => AimpFormat.Format46,
                    _ => throw new Exception($"Unsupported AIMP file format {type} {fileVersion}"),
                },
                _ => throw new Exception($"Invalid AIMP file format {type} {fileVersion}"),
            };
        }

        private static AimpType DetermineMapTypeFromFilename(string? filename)
        {
            var ext = PathUtils.GetFilenameExtensionWithoutSuffixes(filename);
            return ext switch {
                "aimap" => AimpType.Map,
                "aiwayp" => AimpType.Waypoint,
                "aiwaypmgr" => AimpType.WaypointManager,
                "aivspc" => AimpType.VolumeSpace,
                "ainvm" => AimpType.Navmesh,
                "ainvmmgr" => AimpType.NavmeshManager,
                _ => AimpType.Map,
            };
        }

        public const uint Magic = 0x504D4941;

        public override RSZFile? GetRSZ() => RSZ;

        protected override bool DoRead()
        {
            var handler = FileHandler;
            var header = Header;
            if (!header.Read(handler)) return false;
            if (header.magic != Magic) {
                throw new InvalidDataException($"{handler.FilePath} Not a AIMP file");
            }
            var format = header.Version;

            if (header.contentGroup1Offset > 0) {
                handler.Seek(header.contentGroup1Offset);
                // note: there are cases with no content groups (DMC5)
                mainContent ??= new(format) { header = header };
                mainContent.Read(handler);
            }

            if (header.contentGroup2Offset > 0 && header.group2DataOffset > 0) {
                secondaryContent ??= new(format) { header = header };
                handler.Seek(header.contentGroup2Offset);
                secondaryContent.Read(handler);
            }

            if (header.group1DataOffset > 0) {
                handler.Seek(header.group1DataOffset);
                if (mainContent != null) {
                    handler.Align(16);
                    mainContent.ReadData(handler);
                }
                if (secondaryContent != null) {
                    handler.Align(16);
                    secondaryContent.ReadData(handler);
                }
            }

            if (header.layersOffset > 0) {
                handler.Seek(header.layersOffset);
                layers = new MapLayers[64];
                for (int i = 0; i < 64; ++i) {
                    layers[i] = new MapLayers();
                    layers[i].Read(handler, format);
                }
            }

            if (header.rszOffset > 0)
            {
                ReadRsz(RSZ, header.rszOffset);
                var nodes = (mainContent?.Nodes.Nodes ?? []).Concat(secondaryContent?.Nodes.Nodes ?? []);
                foreach (var node in nodes)
                {
                    if (node.userdataIndex == 0) continue;
                    node.UserData = RSZ.ObjectList[node.userdataIndex - 1];
                }
            }

            if (header.embeddedContentOffset > 0) {
                handler.Seek(header.embeddedContentOffset);

                handler.Read(ref embedHash1);
                if (format <= AimpFormat.Format41) {
                    handler.Read(ref embedHash2);
                }

                var embedCount = handler.Read<int>();
                embeds = new EmbeddedMap[embedCount];
                for (int i = 0; i < embedCount; ++i) {
                    var embed = new EmbeddedMap();
                    embed.Read(handler, format);
                    embeds[i] = embed;
                }
            }
            else
            {
                DataInterpretationException.DebugThrowIf(header.Version >= AimpFormat.Format28, "Missing embed offset");
            }

            DataInterpretationException.ThrowIfNotZeroEOF(handler);
            return true;
        }

        protected override bool DoWrite()
        {
            var handler = FileHandler;
            var header = Header;
            header.Write(handler);

            if (mainContent != null)
            {
                header.contentGroup1Offset = handler.Tell();
                mainContent.header = header;
                mainContent.Write(handler);
            }

            if (secondaryContent != null)
            {
                handler.Align(16);
                header.contentGroup2Offset = handler.Tell();
                secondaryContent.header = header;
                secondaryContent.Write(handler);
            }

            if (mainContent != null)
            {
                handler.Align(16);
                header.group1DataOffset = handler.Tell();
                mainContent.WriteData(handler);
            }

            if (secondaryContent != null)
            {
                handler.Align(16);
                header.group2DataOffset = handler.Tell();
                secondaryContent.WriteData(handler);
            }

            if (layers?.Length > 0)
            {
                handler.Align(16);
                header.layersOffset = handler.Tell();
                foreach (var layer in layers) layer.Write(handler, header.Version);
            }

            if (header.Version >= AimpFormat.Format28) {
                // rebuild the RSZ data table just in case anything relevant changed
                var userdataInstances = (mainContent?.Nodes.Nodes ?? [])
                    .Concat(secondaryContent?.Nodes.Nodes ?? [])
                    .Where(x => x.UserData != null);

                RSZ.ClearObjects();
                foreach (var ud in userdataInstances)
                {
                    RSZ.AddToObjectTable(ud.UserData!);
                    ud.userdataIndex = ud.UserData!.ObjectTableIndex + 1;
                }
                RSZ.RebuildInstanceList(userdataInstances.Select(x => x.UserData!));

                handler.Align(16);
                WriteRsz(RSZ, header.rszOffset = handler.Tell());

                handler.Align(16);
                header.embeddedContentOffset = handler.Tell();
                handler.Write(ref embedHash1);
                if (header.Version <= AimpFormat.Format41) {
                    handler.Write(ref embedHash2);
                }
                handler.Write(embeds.Length);

                if (embeds.Length > 0)
                {
                    foreach (var embed in embeds)
                    {
                        embed.Write(handler, header.Version);
                    }
                }

                // some DD2 base game files _sometimes_ have extra 0s, seemingly when no embeds and embedHash1 == 0; some games don't
                // I don't think it's worth trying to figure out what exactly this is so just in case, write an extra 8 bytes here
                if (embeds.Length == 0) handler.WriteNull(8);
            }

            header.Write(handler, 0);
            return true;
        }
    }
}
