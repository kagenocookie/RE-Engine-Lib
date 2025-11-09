using ReeLib.Bvh;
using ReeLib.InternalAttributes;
using ReeLib.via;

namespace ReeLib.Coco
{
    public abstract class CompositeColliderNode : BaseModel
    {
        public int nodeType;
        public int treeIndex;
    }

    public partial class McolCompositeCollider : CompositeColliderNode
    {
        public mat4 matrix;
        internal int mcolIndex = -1;

        [RszIgnore] public string mcolFile = "";

        private bool ReadWrite<THandler>(THandler handler) where THandler : IFileHandlerAction
        {
            handler.Do(ref matrix.m00);
            handler.Do(ref matrix.m01);
            handler.Do(ref matrix.m02);
            handler.Do(ref matrix.m10);
            handler.Do(ref matrix.m11);
            handler.Do(ref matrix.m12);
            handler.Do(ref matrix.m20);
            handler.Do(ref matrix.m21);
            handler.Do(ref matrix.m22);
            handler.Do(ref matrix.m30);
            handler.Do(ref matrix.m31);
            handler.Do(ref matrix.m32);
            handler.Do(ref mcolIndex);
            matrix.m33 = 1;
            return true;
        }

        protected override bool DoRead(FileHandler handler) => ReadWrite(new FileHandlerRead(handler));
        protected override bool DoWrite(FileHandler handler) => ReadWrite(new FileHandlerWrite(handler));

        public override string ToString() => mcolFile + " " + matrix.Row3;
    }

    [RszGenerate, RszAutoReadWrite]
    public partial class OBBCompositeCollider : CompositeColliderNode
    {
        public OBB box;

        public override string ToString() => $"OBB {box.Coord.Row3}";
    }

    public class CocoTree : BaseModel
    {
        public List<CompositeColliderNode> Nodes { get; } = new();
        public BvhTree Tree { get; } = new();
        public long ukn;

        private long nodesStartOffset;

        protected override bool DoRead(FileHandler handler)
        {
            var count = handler.Read<int>();
            var dataSize = handler.Read<int>();

            handler.Read(ref ukn);

            var nodesOffset = handler.Read<long>();
            var entriesOffset = handler.Read<long>();

            var headerEnd = handler.Tell();

            handler.Seek(nodesOffset);
            Nodes.EnsureCapacity(count);
            for (int i = 0; i < count; ++i)
            {
                var treeIndex = handler.Read<int>();
                var nodeType = handler.Read<int>();
                CompositeColliderNode node = nodeType switch {
                    5 => new OBBCompositeCollider(),
                    6 => new McolCompositeCollider(),
                    _ => throw new NotSupportedException("Unsupported coco collider type " + nodeType),
                };
                node.treeIndex = treeIndex;
                node.nodeType = nodeType;
                handler.ReadNull(24);
                node.Read(handler, handler.Read<long>());
                Nodes.Add(node);
            }

            handler.Seek(entriesOffset);
            int nodeCount = handler.Read<int>();
            handler.ReadNull(12);
            Tree.entries.Read(handler, nodeCount);

            handler.Seek(headerEnd);
            return true;
        }

        protected override bool DoWrite(FileHandler handler)
        {
            handler.Write(Nodes.Count);
            handler.Write(Nodes.Count * 64 - 16);
            handler.Write(ref ukn);
            handler.WriteNull(16);

            return true;
        }

        public void WriteTree(FileHandler handler)
        {
            var offsetsStart = Start + 16;
            nodesStartOffset = handler.Tell();

            handler.Write(offsetsStart, handler.Tell()); // nodesOffset
            foreach (var node in Nodes)
            {
                handler.Write(node.treeIndex);
                handler.Write(node.nodeType);
                handler.WriteNull(32);
            }

            handler.Align(16);
            handler.Write(offsetsStart + 8, handler.Tell()); // entriesOffset
            handler.Write(Tree.entries.Count);
            handler.WriteNull(12);
            Tree.entries.Write(handler);
        }

        public void WriteColliders(FileHandler handler)
        {
            for (int i = 0; i < Nodes.Count; ++i)
            {
                var node = Nodes[i];
                handler.Write(nodesStartOffset + 32 + i * 40, handler.Tell());
                node.Write(handler);
            }
        }
    }
}

namespace ReeLib
{
    using System.Reflection.Metadata;
    using ReeLib.Coco;

    public class CocoFile : BaseFile
    {
        public List<string> CollisionMeshPaths { get; } = new();
        public List<CocoTree> Trees { get; } = new();

        public const int Magic = 0x4F434F43;

        public record class ResourceID
        {
            public int ID;
            public string path = "";
        }

        public CocoFile(FileHandler fileHandler) : base(fileHandler)
        {
        }

        protected override bool DoRead()
        {
            var handler = FileHandler;
            if (handler.Read<int>() != Magic) throw new InvalidDataException("Invalid coco file");

            Trees.Clear();
            CollisionMeshPaths.Clear();

            var pathsOffset = handler.Read<int>();
            var pathCount = (int)handler.Read<long>();

            var treeHandler = handler.WithOffset(16);

            var treeCount = (int)handler.Read<long>();
            var treeHeadersOffset = handler.Read<long>();

            treeHandler.Seek(treeHeadersOffset);
            for (int i = 0; i < treeCount; ++i)
            {
                var tree = new CocoTree();
                tree.Read(treeHandler);
                Trees.Add(tree);
            }

            handler.Seek(pathsOffset);
            for (int i = 0; i < pathCount; ++i) CollisionMeshPaths.Add(handler.ReadWString(handler.Read<long>() + pathsOffset));

            foreach (var tree in Trees)
            {
                foreach (var node in tree.Nodes)
                {
                    if (node is McolCompositeCollider mcol)
                    {
                        mcol.mcolFile = CollisionMeshPaths[mcol.mcolIndex];
                    }
                }
            }

            return true;
        }

        protected override bool DoWrite()
        {
            var handler = FileHandler;
            handler.Write(Magic);
            handler.Skip(4); // pathsOffset
            handler.Write((long)CollisionMeshPaths.Count);

            var treeHandler = handler.WithOffset(handler.Tell());
            handler.Write((long)Trees.Count);
            handler.Write(16L);

            var indicesDict = new Dictionary<string, int>();
            foreach (var path in CollisionMeshPaths)
            {
                if (!indicesDict.TryGetValue(path, out _))
                    indicesDict[path] = indicesDict.Count;
            }

            foreach (var tree in Trees)
            {
                foreach (var item in tree.Nodes)
                {
                    if (item is McolCompositeCollider mcol)
                    {
                        mcol.mcolIndex = indicesDict[mcol.mcolFile];
                    }
                }

                tree.Write(treeHandler);
            }

            foreach (var tree in Trees)
            {
                tree.WriteTree(treeHandler);
            }

            foreach (var tree in Trees)
            {
                tree.WriteColliders(treeHandler);
            }

            handler.Align(8);
            handler.Write(4, (int)handler.Tell());
            var stringHandler = handler.WithOffset(handler.Tell());
            foreach (var path in CollisionMeshPaths) stringHandler.WriteOffsetWString(path);
            stringHandler.StringTableFlush();

            return true;
        }
    }
}