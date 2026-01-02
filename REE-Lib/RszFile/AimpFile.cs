using System.Diagnostics;
using System.Numerics;
using System.Runtime.InteropServices;
using ReeLib.Aimp;
using ReeLib.Common;
using ReeLib.Gui;
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

    /// <summary>
    /// via.navigation.map.NodeContent.EdgeAttribute.
    ///
    /// Note that an InnerSide=0x10000 enum entry seems to exist too, but that doesn't fit in the byte sized attribute on navmesh edge infos so we're ignoring it.
    /// </summary>
    [Flags]
    public enum EdgeAttribute : byte
    {
        /// <summary>
        /// Edge is a contour line segment, meaning it's an outer edge with no neighboring triangle.
        /// </summary>
        Contour = 1,
        Divided = 2,
        Vertical = 4,
        ContourP0 = 8,
        ContourP1 = 16,
        WallP0 = 32,
        WallP1 = 64,
    }

    public class AimpHeader : BaseModel
    {
        public uint magic = AimpFile.Magic;
        public string? name;
        public string? hash;
        public MapType mapType;
        public SectionType sectionType;
        public GuiObjectID mapId;
        public float agentRadWhenBuild;
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
                mapId = new GuiObjectID(handler.Read<long>());
                handler.Read(ref uknId);
            } else if (Version >= AimpFormat.Format28) {
                handler.Read(ref mapId.guid);
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
                handler.Write(mapId.AsID);
                handler.Write(ref uknId);
            } else if (Version >= AimpFormat.Format28) {
                handler.Write(mapId.guid);
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

    public class EdgeInfo
    {
        public EdgeAttribute[] edgeAttributes = [];
        public float[] traverseCosts = [];

        public void Init(int count)
        {
            edgeAttributes = new EdgeAttribute[count];
            traverseCosts = new float[count];
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
            edgeAttributes = handler.ReadArray<EdgeAttribute>(count);
            handler.Align(4);
            traverseCosts = handler.ReadArray<float>(count);
        }

        public void ReadRE7(int count, FileHandler handler)
        {
            edgeAttributes = handler.ReadArray<int>(count).Select(n => (EdgeAttribute)n).ToArray();
        }

        public void Write(FileHandler handler)
        {
            handler.WriteArray(edgeAttributes!);
            handler.Align(4);
            handler.WriteArray(traverseCosts!);
        }

        public void WriteRE7(FileHandler handler)
        {
            foreach (var attr in edgeAttributes!)
            {
                handler.Write((int)attr);
            }
        }
    }

    public class TriangleNode
    {
        public int index1;
        public int index2;
        public int index3;
        public EdgeInfo edges = new();

        public void Read(FileHandler handler, AimpFormat format)
        {
            handler.Read(ref index1);
            handler.Read(ref index2);
            handler.Read(ref index3);
            edges.Read(3, handler, format);
        }

        public void Write(FileHandler handler, AimpFormat format)
        {
            handler.Write(ref index1);
            handler.Write(ref index2);
            handler.Write(ref index3);
            edges.Write(handler, format);
        }
    }

    public interface IMultiIndexNode
    {
        public int[] Indices { get; set; }
    }

    public class PolygonNode : IMultiIndexNode
    {
        public EdgeInfo edges = new();
        public int[] indices = [];
        public Vector3 min;
        public Vector3 max;

        public AimpFormat version;

        public int[] Indices { get => indices; set => indices = value; }

        public void Read(FileHandler handler)
        {
            var pointCount = handler.Read<int>();
            indices = handler.ReadArray<int>(pointCount);
            edges.Read(pointCount, handler, version);
            handler.Read(ref min);
            handler.Read(ref max);
        }

        public void Write(FileHandler handler)
        {
            handler.Write(indices.Length);
            handler.WriteArray<int>(indices);
            if (version >= AimpFormat.Format28) {
                edges.Write(handler);
            } else {
                edges.WriteRE7(handler);
            }
            handler.Write(ref min);
            handler.Write(ref max);
        }
    }

    public class AABBNode : IMultiIndexNode
    {
        // there are a few rare cases where it's not not just 2 indices duplicated twice (aivspc)
        // maybe the second pair is a "ground" bounds? (see RE4 loc5510.aivspc)
        public ushort[] indices = new ushort[4];
        // these values also seem to only ever be not 0 in volume space maps (see DMC5 m02_340c_start.aimap.28, RE4 loc5500_ao.aivspc.6)
        public float[]? values;
        public int value;

        public int[] Indices { get => indices.Select(i => (int)i).ToArray(); set => indices = value.Select(i => (ushort)i).ToArray(); }
    }

    public struct MapBoundaryNode
    {
        public int[] indices;
        public Vector3 min;
        public Vector3 max;
    }

    [RszGenerate]
    public partial class WallNode : IMultiIndexNode
    {
        public via.mat4 matrix;
        public PaddedVec3 scale;
        public Quaternion rotation;
        public PaddedVec3 position;
        [RszFixedSizeArray(8)] public int[] indices = new int[8];

        public int[] Indices { get => indices; set => indices = value; }

        public void Read(FileHandler handler) => DefaultRead(handler);
        public void Write(FileHandler handler) => DefaultWrite(handler);
    }

    [StructLayout(LayoutKind.Explicit, Size = 16)]
    public struct PaddedVec3
    {
        [FieldOffset(0)] public float x;
        [FieldOffset(4)] public float y;
        [FieldOffset(8)] public float z;

        public PaddedVec3(Vector3 vec)
        {
            x = vec.X;
            y = vec.Y;
            z = vec.Z;
        }

        public PaddedVec3(float x, float y, float z)
        {
            this.x = x;
            this.y = y;
            this.z = z;
        }

        public Vector3 Vector3 => new Vector3(x, y, z);
        public override string ToString() => $"{x}, {y}, {z}";
    }

    public class NodeHeightPartitioning
    {
        public int[] height0 = [];
        public int[] height1 = [];
        public int[] height2 = [];
        public int[] height3 = [];

        public void Clear()
        {
            height0 = [];
            height1 = [];
            height2 = [];
            height3 = [];
        }

        public void Read(FileHandler handler)
        {
            var c1 = handler.Read<int>();
            var c2 = handler.Read<int>();
            var c3 = handler.Read<int>();
            var c4 = handler.Read<int>();
            height0 = handler.ReadArray<int>(c1);
            height1 = handler.ReadArray<int>(c2);
            height2 = handler.ReadArray<int>(c3);
            height3 = handler.ReadArray<int>(c4);
        }

        public void Write(FileHandler handler)
        {
            handler.Write(height0.Length);
            handler.Write(height1.Length);
            handler.Write(height2.Length);
            handler.Write(height3.Length);
            handler.WriteArray(height0);
            handler.WriteArray(height1);
            handler.WriteArray(height2);
            handler.WriteArray(height3);
        }
    }

    public abstract class ContentGroup
    {
        public AimpFormat format;

        public Vector3[]? Vertices { get; internal set; }
        public List<NodeInfo> NodeInfos { get; internal set; } = [];
        public int VertexStartOffset { get; set; }

        public abstract int NodeCount { get; }
        public string Classname => "via.navigation.map." + GetType().Name;

        public abstract bool ReadNodes(FileHandler handler, int count);
        public abstract bool ReadData(FileHandler handler);
        public abstract void WriteData(FileHandler handler);
        public abstract void WriteNodes(FileHandler handler);

        public abstract Vector3 GetNodeCenter(ContentGroupContainer container, int i);
        protected abstract RangeI GetVertexRange(ContentGroupContainer container);

        protected void UnpackVertices(ContentGroupContainer container)
        {
            var range = GetVertexRange(container);
            // DataInterpretationException.DebugThrowIf(range.r != VertexStartOffset);
            Vertices = new Vector3[range.s - range.r];
            for (int i = 0; i < Vertices.Length; i++) Vertices[i] = container.Vertices[range.r + i].Vector3;
        }

        protected void PackVertices(ContentGroupContainer container, int vertStartIndex)
        {
            Debug.Assert(Vertices != null);
            for (int i = 0; i < Vertices.Length; i++) container.Vertices[vertStartIndex + i] = new PaddedVec3(Vertices[i]);
        }

        internal abstract void UnpackData(ContentGroupContainer container, ContentGroupContainer? otherContainer);

        internal abstract void PackData(ContentGroupContainer container, int startIndex);

        protected int CalculateVertexPackOffset(ContentGroupContainer container, int vertStartIndex)
        {
            var range = GetVertexRange(container);
            return vertStartIndex - range.r;
        }

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

        protected void ShiftVertexIndices<T>(List<T> Nodes, ContentGroupContainer container, int vertStartIndex) where T : IMultiIndexNode
        {
            var vertOffset = CalculateVertexPackOffset(container, vertStartIndex);
            if (vertOffset != 0)
            {
                foreach (var n in Nodes)
                {
                    var indices = n.Indices;
                    for (int i = 0; i < indices.Length; ++i) indices[i] += vertOffset;
                    n.Indices = indices;
                }
            }
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
        [field: RszIgnore] public List<NodeInfo> PairNodes { get; } = new();

        public Color GetColor(AimpFile file) => attributes == 0 ? new Color(0xffffffff) : file.layers![BitOperations.TrailingZeroCount(attributes)].color;

        public void Read(FileHandler handler) => DefaultRead(handler);
        public void Write(FileHandler handler) => DefaultWrite(handler);

        public NodeInfoRE7 AsRE7() => new NodeInfoRE7() { flags = flags, index = index, index2 = index, attributes = attributes };

        public override string ToString() => $"{groupIndex}:{index} {flags} [{nextIndex}]";
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

    [RszGenerate]
    public partial class LinkInfo
    {
        public int index;
        public int sourceNodeIndex;
        public int targetNodeIndex;
        /// <summary>
        /// Seems to represent the index of the target triangle / polygons's matching edge index (index 1+2 = 0, 2+3 = 1, 3+1 = 2 for triangles).
        /// </summary>
        public int edgeIndex;
        public ulong attributes;
        public int ukn; // this is sometimes int, sometimes float...

        [field: RszIgnore] public NodeInfo? SourceNode { get; set; }
        [field: RszIgnore] public NodeInfo? TargetNode { get; set; }

        public void Read(FileHandler handler) => DefaultRead(handler);
        public void Write(FileHandler handler) => DefaultWrite(handler);
    }

    public struct IndexSet
    {
        public int[] indices;

        public override string ToString() => string.Join(", ", indices ?? Array.Empty<int>());
    };

    public class NodeData
    {
        public List<NodeInfo> Nodes = [];
        public int maxIndex;
        public int minIndex;

        private int[]? _effectiveNodeIndices;
        public int[] EffectiveNodeIndices
        {
            get
            {
                if (_effectiveNodeIndices == null)
                {
                    if (Nodes.Count == 0) return _effectiveNodeIndices = [];

                    _effectiveNodeIndices = new int[maxIndex + 1];
                    int offsetIndex = 0;
                    for (int i = 0; i < Nodes.Count; i++) {
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

        public void Clear()
        {
            _effectiveNodeIndices = null;
            Nodes.Clear();
            minIndex = 0;
            maxIndex = 0;
        }

        public void ResetNodeIndexCache()
        {
            _effectiveNodeIndices = null;
        }

        public void Read(FileHandler handler, AimpFormat format)
        {
            var nodeCount = handler.Read<int>();
            if (format == AimpFormat.Format28) {
                handler.ReadNull(4);
            }
            if (format >= AimpFormat.Format41) {
                handler.Read(ref maxIndex);
                handler.Read(ref minIndex);
            }

            Nodes = new List<NodeInfo>(nodeCount);

            if (format >= AimpFormat.Format28)
            {
                for (int i = 0; i < nodeCount; ++i)
                {
                    var node = new NodeInfo();
                    node.Read(handler);
                    Nodes.Add(node);
                }

                foreach (var node in Nodes)
                {
                    for (int i = 0; i < node.linkCount; ++i)
                    {
                        var link = new LinkInfo();
                        link.Read(handler);
                        node.Links.Add(link);
                    }
                }
            }
            else
            {
                for (int i = 0; i < nodeCount; ++i)
                {
                    var node = handler.Read<NodeInfoRE7>().Upgrade();
                    Nodes.Add(node);
                }
                int linkCount = handler.Read<int>();
                for (int i = 0; i < linkCount; ++i)
                {
                    var link = new LinkInfo();
                    link.Read(handler);
                    Nodes[link.sourceNodeIndex].Links.Add(link);
                }
            }

            // convenience to simplify usage - integrations can always assume it's there and valid even for older games
            if (maxIndex == 0)
            {
                foreach (var node in Nodes)
                {
                    foreach (var link in node.Links)
                    {
                        maxIndex = Math.Max(maxIndex, Math.Max(link.sourceNodeIndex, link.targetNodeIndex));
                    }
                }
            }
        }

        public void Write(FileHandler handler, AimpFormat format)
        {
            handler.Write(Nodes.Count);
            if (format == AimpFormat.Format28) handler.WriteNull(4);
            else if (format >= AimpFormat.Format41)
            {
                handler.Write(ref maxIndex);
                handler.Write(ref minIndex);
            }

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
                    link.Write(handler);
                }
            }
        }
    }

    public class ContentGroupMapPoint : ContentGroup<ContentGroupMapPoint.Point>
    {
        public int[] indexData = [];

        public override Vector3 GetNodeCenter(ContentGroupContainer container, int i) => Nodes[i].pos;
        protected override RangeI GetVertexRange(ContentGroupContainer container)
        {
            return new RangeI(); // no verts here
        }

        internal override void UnpackData(ContentGroupContainer container, ContentGroupContainer? otherContainer)
        {
            Vertices = [];
        }
        internal override void PackData(ContentGroupContainer container, int startIndex) { }

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

        protected override RangeI GetVertexRange(ContentGroupContainer container)
        {
            var range = new RangeI() { r = int.MaxValue, s = int.MinValue };
            foreach (var n in Nodes) {
                range.r = Math.Min(Math.Min(n.index1, Math.Min(n.index2, n.index3)), range.r);
                range.s = Math.Max(Math.Max(n.index1, Math.Max(n.index2, n.index3)), range.s);
            }
            range.s++;
            return range;
        }

        internal override void UnpackData(ContentGroupContainer container, ContentGroupContainer? otherContainer)
        {
            UnpackVertices(container);

            if (otherContainer == null) return;
            for (int i = 0; i < Nodes.Count; ++i)
            {
                var polygonNodeIndex = polygonIndices[i];
                NodeInfos[i].PairNodes.Add(otherContainer.NodeInfo.Nodes[polygonNodeIndex]);
            }
        }

        internal override void PackData(ContentGroupContainer container, int vertStartIndex)
        {
            Debug.Assert(Vertices != null);
            var vertOffset = CalculateVertexPackOffset(container, vertStartIndex);
            if (vertOffset != 0)
            {
                foreach (var n in Nodes)
                {
                    n.index1 += vertOffset;
                    n.index2 += vertOffset;
                    n.index3 += vertOffset;
                }
            }
            PackVertices(container, vertStartIndex);
            for (int i = 0; i < Nodes.Count; ++i)
            {
                var nodeinfo = NodeInfos[i];
                polygonIndices[i] = nodeinfo.PairNodes[0].index;
            }
        }

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

        protected override RangeI GetVertexRange(ContentGroupContainer container)
        {
            var range = new RangeI() { r = int.MaxValue, s = int.MinValue };
            foreach (var n in Nodes) {
                foreach (var ind in n.indices) {
                    range.r = Math.Min(ind, range.r);
                    range.s = Math.Max(ind, range.s);
                }
            }
            range.s++;
            return range;
        }

        internal override void UnpackData(ContentGroupContainer container, ContentGroupContainer? otherContainer)
        {
            UnpackVertices(container);

            if (otherContainer == null) return;
            for (int i = 0; i < Nodes.Count; ++i)
            {
                var otherNodeIndices = triangleIndices[i];
                var nodeinfo = NodeInfos[i];
                foreach (var index in otherNodeIndices.indices)
                {
                    nodeinfo.PairNodes.Add(otherContainer.NodeInfo.Nodes[index]);
                }
            }
        }

        internal override void PackData(ContentGroupContainer container, int vertStartIndex)
        {
            ShiftVertexIndices(Nodes, container, vertStartIndex);
            PackVertices(container, vertStartIndex);
            for (int i = 0; i < Nodes.Count; ++i)
            {
                var nodeinfo = NodeInfos[i];
                triangleIndices[i].indices = nodeinfo.PairNodes.Select(p => p.index).ToArray();
            }
        }

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
        public class MapBoundaryNodeInfo : IMultiIndexNode
        {
            public int[] indices = new int[8];
            public Vector3 min, max;

            public int[] Indices { get => indices; set => indices = value; }
        }

        public override Vector3 GetNodeCenter(ContentGroupContainer container, int i) => (Nodes[i].min + Nodes[i].max) * 0.5f;

        protected override RangeI GetVertexRange(ContentGroupContainer container)
        {
            var range = new RangeI() { r = int.MaxValue, s = int.MinValue };
            foreach (var n in Nodes) {
                foreach (var ind in n.indices) {
                    range.r = Math.Min(ind, range.r);
                    range.s = Math.Max(ind, range.s);
                }
            }
            range.s++;
            return range;
        }

        internal override void UnpackData(ContentGroupContainer container, ContentGroupContainer? otherContainer)
        {
            // we don't necessarily need to unpack anything - we just need the min/max on the nodes to regenerate the vert list later
            UnpackVertices(container);
        }

        internal override void PackData(ContentGroupContainer container, int vertStartIndex)
        {
            Debug.Assert(Vertices != null);
            var arr = Vertices;
            Array.Resize(ref arr, NodeCount * 8);
            Vertices = arr;
            for (int i = 0; i < Nodes.Count; ++i)
            {
                var node = Nodes[i];
                // I have no idea what the significance of these hardcoded offsets is, but that's how they look like in the files
                var mid = (node.min + node.max) * 0.5f;
                Vertices[i * 8 + 0] = new Vector3(node.min.X, node.min.Y, node.min.Z + 0.5f);
                Vertices[i * 8 + 1] = new Vector3(node.min.X + 0.25f, node.min.Y, node.max.Z);
                Vertices[i * 8 + 2] = new Vector3(node.max.X, node.max.Y, node.min.Z + 0.5f);
                Vertices[i * 8 + 3] = new Vector3(node.max.X - 0.25f, node.max.Y, node.min.Z);

                // TODO verify if the mid Y has any effect whatsoever (can't find a correlation in the numbers, might be meaningless, might not)
                Vertices[i * 8 + 4] = new Vector3(node.min.X, mid.Y, node.min.Z + 0.5f);
                Vertices[i * 8 + 5] = new Vector3(node.min.X + 0.25f, mid.Y, node.max.Z);
                Vertices[i * 8 + 6] = new Vector3(node.max.X, mid.Y, node.min.Z + 0.5f);
                Vertices[i * 8 + 7] = new Vector3(node.max.X - 0.25f, mid.Y, node.min.Z);
            }
            ShiftVertexIndices(Nodes, container, vertStartIndex);
            PackVertices(container, vertStartIndex);
        }

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
            // don't even store the indices here, since they're always -1
            for (int i = 0; i < Nodes.Count; ++i)
            {
                DataInterpretationException.ThrowIf(handler.Read<int>() != -1, "Boundary aimp content type isn't supposed to have indices");
            }
            return true;
        }

        public override void WriteData(FileHandler handler)
        {
            for (int i = 0; i < Nodes.Count; ++i) handler.Write(-1);
        }
    }

    public class ContentGroupMapAABB : ContentGroup<AABBNode>
    {
        // depending on usecase, the index sets can have either 0, 1 or 4 indices
        // 0 => if main content AABB
        // 1 => if secondary content AABB for a Boundary type main content (and sometimes for Walls too)
        // 4 => if secondary content AABB for a Wall type main content
        public IndexSet[] data = [];


        public override Vector3 GetNodeCenter(ContentGroupContainer container, int i) => container.Vertices[Nodes[i].indices[0]].Vector3;

        protected override RangeI GetVertexRange(ContentGroupContainer container)
        {
            var range = new RangeI() { r = int.MaxValue, s = int.MinValue };
            foreach (var n in Nodes)
            {
                foreach (var ind in n.indices)
                {
                    range.r = Math.Min(ind, range.r);
                    range.s = Math.Max(ind, range.s);
                }
            }
            range.s++;
            return range;
        }


        internal override void UnpackData(ContentGroupContainer container, ContentGroupContainer? otherContainer)
        {
            UnpackVertices(container);
            // note: we can't just reconstruct this from the paired up Boundary content group because not all AABB groups have a pair

            if (otherContainer == null) return;
            for (int i = 0; i < Nodes.Count; ++i)
            {
                var otherNodeIndices = data[i];
                var nodeinfo = NodeInfos[i];
                foreach (var index in otherNodeIndices.indices)
                {
                    nodeinfo.PairNodes.Add(otherContainer.NodeInfo.Nodes[index]);
                }
            }
        }

        internal override void PackData(ContentGroupContainer container, int vertStartIndex)
        {
            ShiftVertexIndices(Nodes, container, vertStartIndex);
            PackVertices(container, vertStartIndex);
            for (int i = 0; i < Nodes.Count; ++i)
            {
                var nodeinfo = NodeInfos[i];
                data[i].indices = nodeinfo.PairNodes.Select(p => p.index).ToArray();
            }
        }

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
        public override Vector3 GetNodeCenter(ContentGroupContainer container, int i) => container.Vertices[Nodes[i].indices[0]].Vector3;

        protected override RangeI GetVertexRange(ContentGroupContainer container)
        {
            var range = new RangeI() { r = int.MaxValue, s = int.MinValue };
            foreach (var n in Nodes) {
                foreach (var ind in n.indices) {
                    range.r = Math.Min(ind, range.r);
                    range.s = Math.Max(ind, range.s);
                }
            }
            range.s++;
            return range;
        }

        internal override void UnpackData(ContentGroupContainer container, ContentGroupContainer? otherContainer)
        {
            UnpackVertices(container);
        }

        internal override void PackData(ContentGroupContainer container, int vertStartIndex)
        {
            ShiftVertexIndices(Nodes, container, vertStartIndex);
            PackVertices(container, vertStartIndex);
        }

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
            // don't even store the indices here, since they're always -1
            for (int i = 0; i < Nodes.Count; ++i)
            {
                DataInterpretationException.ThrowIf(handler.Read<int>() != -1, "Wall aimp content type isn't supposed to have indices");
            }
            return true;
        }

        public override void WriteData(FileHandler handler)
        {
            for (int i = 0; i < Nodes.Count; ++i) handler.Write(-1);
        }
    }

    public class ContentGroupContainer : BaseModel
    {
        public ContentGroup[] contents = [];

        public PaddedVec3[] Vertices { get; internal set; } = [];
        public NodeData NodeInfo = new();

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
                    _nodeOrigins = new Vector3[NodeInfo.Nodes.Count];
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

        public NodeHeightPartitioning? NodeHeights;
        public AimpFormat version;

        internal AimpHeader header = null!;

        internal ContentGroupContainer(AimpFormat version)
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
            _nodeOrigins = null;
            NodeInfo.ResetNodeIndexCache();
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
            NodeInfo.Clear();
            NodeInfo.Read(handler, version);

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
                NodeHeights ??= new();
                NodeHeights.Read(handler);
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
            NodeInfo.Write(handler, version);
            if (header.Version > AimpFormat.Format8 && header.sectionType == SectionType.NoSection) {
                handler.WriteNull(4);
            }

            handler.Write(ref float1);
            handler.Write(ref float2);
            if (version > AimpFormat.Format8) {
                handler.Write(ref bounds);
                NodeHeights ??= new();
                NodeHeights.Write(handler);
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

        internal void UnpackData(ContentGroupContainer? other)
        {
            var effectiveNodeIndices = NodeInfo.EffectiveNodeIndices;
            foreach (var nodeinfo in NodeInfo.Nodes)
            {
                contents[nodeinfo.groupIndex].NodeInfos.Add(nodeinfo);
                foreach (var link in nodeinfo.Links)
                {
                    var n1 = NodeInfo.Nodes[effectiveNodeIndices[link.sourceNodeIndex]];
                    var n2 = NodeInfo.Nodes[effectiveNodeIndices[link.targetNodeIndex]];

                    // Debug.Assert(n1 == nodeinfo || n2 == nodeinfo);
                    // TODO figure out why sometimes neither n1 nor n2 is equal to our nodeInfo

                    link.SourceNode = n1;
                    link.TargetNode = n2;
                }
            }

            int vertOffset = 0;
            foreach (var c in contents)
            {
                c.VertexStartOffset = vertOffset;
                if (c.NodeCount == 0) {
                    c.Vertices = [];
                    continue;
                }
                c.UnpackData(this, other);
                vertOffset += c.Vertices?.Length ?? 0;
            }
        }

        internal void PackData()
        {
            NodeInfo.Nodes.Clear();
            for (int g = 0; g < contents.Length; ++g)
            {
                int i = 0;
                foreach (var nodeinfo in contents[g].NodeInfos)
                {
                    NodeInfo.Nodes.Add(nodeinfo);
                    nodeinfo.index = i++;
                    nodeinfo.groupIndex = g;
                    nodeinfo.linkCount = nodeinfo.Links.Count;
                }
            }

            // note: we can't blindly automate the nextIndex field because wayp files can have varying and not strictly-ascending values

            int verts = 0;
            foreach (var c in contents)
            {
                c.PackData(this, verts);
                verts += c.Vertices!.Length;
            }

            _nodeOrigins = null;
            NodeInfo.ResetNodeIndexCache();
            var nodeIndices = NodeInfo.EffectiveNodeIndices;
            var linkNodeIndexDict = new Dictionary<NodeInfo, int>();
            for (int i = 0; i < nodeIndices.Length; ++i)
            {
                linkNodeIndexDict.TryAdd(NodeInfo.Nodes[nodeIndices[i]], i);
            }

            foreach (var nodeInfo in NodeInfo.Nodes)
            {
                foreach (var link in nodeInfo.Links)
                {
                    // note: this only theoretically (untested) works for wayp files
                    // for non-wayp files, the node indices should mostly just be equal to the index so we may be able to get away with just taking the node index directly there otherwise
                    link.sourceNodeIndex = linkNodeIndexDict[link.SourceNode!];
                    link.targetNodeIndex = linkNodeIndexDict[link.TargetNode!];
                    DataInterpretationException.DebugThrowIf(link.sourceNodeIndex == -1 || link.targetNodeIndex == -1);

                    // backup non-dict code just in case
                    // var srcIndex = NodeInfo.Nodes.IndexOf(link.SourceNode!);
                    // var targetIndex = NodeInfo.Nodes.IndexOf(link.TargetNode!);
                    // link.sourceNodeIndex = nodeIndices.IndexOf(srcIndex);
                    // link.targetNodeIndex = nodeIndices.IndexOf(targetIndex);
                }
            }
        }

        public TGroup InitGroup<TGroup>(int vertCount, bool isMainGroup) where TGroup : ContentGroup, new()
        {
            var group = contents.OfType<TGroup>().FirstOrDefault();
            int previousGroupVertCount = 0;
            if (group == null) {
                group = new TGroup() { format = header.Version };
                if (isMainGroup) {
                    contents = new ContentGroup[] { group }.Concat(contents).ToArray();
                } else {
                    contents = contents.Append(group).ToArray();
                }
                group.Vertices = new Vector3[vertCount];
            }
            else
            {
                previousGroupVertCount = group.Vertices!.Length;
                var diff = vertCount - previousGroupVertCount;
                if (diff != 0) {
                    group.Vertices = new Vector3[vertCount];
                }
            }

            if (contents.Length > 1)
            {
                var diff = vertCount - previousGroupVertCount;
                var newArray = new PaddedVec3[Vertices.Length + diff];
                Vertices = newArray;
                // we don't need to copy values over, they'll get filled during the PackData() step from the group-local vertex arrays
            }
            else
            {
                Vertices = new PaddedVec3[vertCount];
            }

            // shift vertex indices accordingly for groups after the one we just resized
            var groupIndex = Array.IndexOf(contents, group);
            if (contents.Length > groupIndex + 1)
            {
                var delta = vertCount - contents[groupIndex + 1].VertexStartOffset;
                for (int i = groupIndex + 1; i < contents.Length; ++i) contents[i].VertexStartOffset += delta;
            }

            return group;
        }

        public void RemoveGroup(ContentGroup group)
        {
            var groupIndex = Array.IndexOf(contents, group);
            if (groupIndex == -1) return;

            contents = contents.Except([group]).ToArray();
            var vertsStart = group.VertexStartOffset;
            var vertsCount = group.Vertices?.Length ?? 0;
            foreach (var node in group.NodeInfos) {
                NodeInfo.Nodes.Remove(node);
            }
            foreach (var node in NodeInfo.Nodes)
            {
                for (int l = 0; l < node.Links.Count; l++)
                {
                    if (node.Links[l].TargetNode?.groupIndex == groupIndex || node.Links[l].SourceNode?.groupIndex == groupIndex)
                    {
                        node.Links.RemoveAt(l--);
                    }
                }
            }
            if (vertsCount != 0)
            {
                var verts = Vertices;
                var nextVertsStart = vertsStart + vertsCount;
                Array.Resize(ref verts, verts.Length - vertsCount);
                Array.Copy(Vertices, nextVertsStart, verts, vertsStart, Vertices.Length - nextVertsStart);
                Vertices = verts;
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

        public GuiObjectID parentMapId;

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

        /// <summary>
        /// Resets and initializes both content groups with empty data.
        /// </summary>
        public void ResetContentGroups(MapType mapType)
        {
            mainContent = new ContentGroupContainer(Header.Version) { header = Header };
            secondaryContent = null;
            Header.mapType = mapType;
            embeds = [];
        }

        /// <summary>
        /// Initializes both content groups with empty data if they are not yet initialized and resizes the data structures according to the given vertex counts.
        /// </summary>
        public void InitContentGroups(int mainContentVertCount, int secondaryContentVertCount)
        {
            mainContent ??= new ContentGroupContainer(Header.Version) { header = Header };
            switch (Header.mapType) {
                case MapType.Navmesh:
                    mainContent.InitGroup<ContentGroupTriangle>(mainContentVertCount, true);
                    secondaryContent ??= new ContentGroupContainer(Header.Version) { header = Header };
                    secondaryContent.InitGroup<ContentGroupPolygon>(secondaryContentVertCount, true);
                    break;
                case MapType.WayPoint:
                    mainContent.InitGroup<ContentGroupMapPoint>(mainContentVertCount, true);
                    break;
                case MapType.VolumeSpace:
                    mainContent.InitGroup<ContentGroupMapAABB>(mainContentVertCount, true);
                    break;
                default:
                    throw new NotImplementedException();
            }
        }

        public void UnpackData()
        {
            mainContent?.UnpackData(secondaryContent);
            secondaryContent?.UnpackData(mainContent);
        }

        public void PackData()
        {
            mainContent?.PackData();
            secondaryContent?.PackData();
            var boundContent = secondaryContent ?? mainContent;
            if (boundContent != null) {
                RebuildHeightPartition(boundContent);
            }
        }

        private static void RebuildHeightPartition(ContentGroupContainer container)
        {
            container.NodeHeights ??= new();
            var h0 = new List<int>();
            var h1 = new List<int>();
            var h2 = new List<int>();
            var h3 = new List<int>();
            List<int>[] heights = [h0, h1, h2, h3];
            var minHeight = container.bounds.minpos.Y;
            var totalHeight = container.bounds.Size.Y;

            for (int i = 0; i < container.NodeInfo.Nodes.Count; i++) {
                var nodeInfo = container.NodeInfo.Nodes[i];
                var content = container.contents[nodeInfo.groupIndex];
                Vector3 min, max;
                switch (content) {
                    case ContentGroupPolygon polyContent:
                        min = polyContent.Nodes[nodeInfo.index].min;
                        max = polyContent.Nodes[nodeInfo.index].max;
                        break;
                    case ContentGroupMapPoint pointContent:
                        // note: it seems like point groups have slightly different partitioning break points, but it's close enough
                        min = max = pointContent.Nodes[nodeInfo.index].pos;
                        break;
                    case ContentGroupMapAABB aabbContent:
                        // note: there's technically 4 indices but the second pair doesn't seem to affect the height partitioning
                        min = container.Vertices[aabbContent.Nodes[nodeInfo.index].indices[0]].Vector3;
                        max = container.Vertices[aabbContent.Nodes[nodeInfo.index].indices[1]].Vector3;
                        break;
                    default:
                        continue;
                }

                var h_min = Math.Clamp((int)Math.Floor(((min.Y - minHeight) / totalHeight) * 4), 0, 3);
                var h_max = Math.Clamp((int)Math.Ceiling(((max.Y - minHeight) / totalHeight) * 4) - 1, 0, 3);
                for (int h = h_min; h <= h_max; ++h) {
                    heights[h].Add(i);
                }
            }

            container.NodeHeights.height0 = h0.ToArray();
            container.NodeHeights.height1 = h1.ToArray();
            container.NodeHeights.height2 = h2.ToArray();
            container.NodeHeights.height3 = h3.ToArray();
        }

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
                mainContent = new(format) { header = header };
                mainContent.Read(handler);
            }

            if (header.contentGroup2Offset > 0 && header.group2DataOffset > 0) {
                secondaryContent = new(format) { header = header };
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
                var nodes = (mainContent?.NodeInfo.Nodes ?? []).Concat(secondaryContent?.NodeInfo.Nodes ?? []);
                foreach (var node in nodes)
                {
                    if (node.userdataIndex == 0) continue;
                    node.UserData = RSZ.ObjectList[node.userdataIndex - 1];
                }
            }

            if (header.embeddedContentOffset > 0) {
                handler.Seek(header.embeddedContentOffset);

                if (format >= AimpFormat.Format46) parentMapId = new GuiObjectID(handler.Read<long>());
                else handler.Read(ref parentMapId.guid);

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
            UnpackData();
            return true;
        }

        protected override bool DoWrite()
        {
            PackData();
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
                var userdataInstances = (mainContent?.NodeInfo.Nodes ?? [])
                    .Concat(secondaryContent?.NodeInfo.Nodes ?? [])
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
                if (header.Version >= AimpFormat.Format46) handler.Write(parentMapId.AsID);
                else handler.Write(ref parentMapId.guid);

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
