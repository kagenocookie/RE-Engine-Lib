using System.Numerics;
using System.Runtime.CompilerServices;
using RszTool.InternalAttributes;
using RszTool.via;

namespace RszTool.Bvh
{
    [RszGenerate, RszAutoReadWrite]
    public partial class BvhHeader : BaseModel
    {
        public uint magic = BvhData.Magic;
        public int verticesCount;
        public int indicesCount;
        public int uknCountMaybePadding;
        public int stringCount;
        public int spheresCount;
        public int capsulesCount;
        public int boxesCount;
        public int ukn3;

        public int treeDataSize;
        public long treeDataOffset;
        public long verticesOffset;
        public long indicesOffset;
        public long uknOffset;

        public long stringTableOffset;
        public long spheresOffset;
        public long capsulesOffset;
        public long boxesOffset;
        public long uknOffsetOrPadding;
    }

    [RszGenerate, RszAutoReadWrite, RszAssignVersion, RszVersionedObject(typeof(int))]
    public partial class TriangleInfo : BaseModel
    {
        public int ukn0;
        public int layerIndex;
        [RszVersion(13020)] // re2rt
        public uint mask;
        [RszVersion(20021)] // re4, dd2
        public int partId;

        internal void ValidateUnknowns(int version)
        {
            if (ukn0 != 0) {
                throw new Exception("Unhandled non-0 ukn0");
            }
        }
    }

    [RszGenerate, RszAutoReadWrite]
    public partial class ObjectInfoUnversioned : BaseModel
    {
        public int ukn0;
        public int layerIndex;
        public uint mask;
        public int partId;

        internal void ValidateUnknowns(int version)
        {
            var expectedMask = version >= 13020 ? -1 : 0;
            if (mask != expectedMask) {
                throw new Exception("Unhandled non-empty mask or color thing");
            }
            if (version < 20021 && partId != 0) {
                throw new Exception("Group id shouldn't exist here");
            }
            if (ukn0 != 0) {
                throw new Exception("Unhandled non-0 ukn0");
            }
        }
    }

    [RszGenerate, RszAutoReadWrite, RszAssignVersion, RszVersionedObject(typeof(int))]
    public partial class BvhTriangle : BaseModel
    {
        [RszClassInstance] public readonly TriangleInfo info = new();
        public int ukn1;
        public int ukn2;
        public int ukn3;
        public int ukn4;
        public int ukn5;
        public int ukn6;
        public int ukn7;
        public int posIndex1;
        public int posIndex2;
        public int posIndex3;
        public int edgeIndex1;
        public int edgeIndex2;
        public int edgeIndex3;
        [RszVersion("<=", 10019)] // dmc5, re3, re8
        public int ukn;
    }

    [RszGenerate, RszAutoReadWrite, RszAssignVersion, RszVersionedObject(typeof(int))]
    public partial class BvhSphere : BaseModel
    {
        [RszClassInstance] public readonly ObjectInfoUnversioned info = new();
        public via.Sphere sphere;
    }

    [RszGenerate, RszAutoReadWrite, RszAssignVersion, RszVersionedObject(typeof(int))]
    public partial class BvhCapsule : BaseModel
    {
        [RszClassInstance] public readonly ObjectInfoUnversioned info = new();
        public via.Capsule capsule;
    }

    [RszGenerate, RszAutoReadWrite, RszAssignVersion, RszVersionedObject(typeof(int))]
    public partial class BvhOBB : BaseModel
    {
        [RszClassInstance] public readonly ObjectInfoUnversioned info = new();
        public via.OBB box;
    }
}

namespace RszTool
{
    using RszTool.Bvh;

    public class BvhData : BaseFile
    {
        public readonly BvhHeader Header = new();
        public readonly List<Vector3> vertices = new();
        public readonly List<BvhTriangle> triangles = new();
        public readonly List<(string main, string? sub)> stringTable = new();
        public readonly List<BvhSphere> spheres = new();
        public readonly List<BvhCapsule> capsules = new();
        public readonly List<BvhOBB> boxes = new();
        public BvhTree? tree;

        public const int Magic = 0x4D485642;

        private const float BvhCollisionMargin = 0.01f;

        public BvhData(FileHandler fileHandler) : base(fileHandler)
        {
        }

        public AABB ReadBounds()
        {
            if (Header.treeDataSize == 0) return new AABB();
            return FileHandler.Read<AABB>(Header.treeDataOffset - 32);
        }

        public void ReadTree()
        {
            if (Header.treeDataSize == 0) return;

            var handler = FileHandler;

            tree = new();
            handler.Seek(Header.treeDataOffset - 32);
            handler.Read(ref tree.bounds);

            var treeCount1 = handler.Read<int>();
            var leafCount = handler.Read<int>();
            handler.Skip(8);
            tree.entries.Clear();
            var totalCount = leafCount > 0 ? leafCount * 2 : treeCount1;
            tree.entries.EnsureCapacity(totalCount);
            tree.entries.Read(handler, totalCount);
        }

        public BvhCapsule AddCollider(Capsule capsule)
        {
            var obj = new BvhCapsule() { capsule = capsule };
            capsules.Add(obj);
            var bounds = obj.capsule.GetBounds(BvhCollisionMargin);
            (tree ??= new()).AddLeaf(bounds);
            return obj;
        }

        public BvhSphere AddCollider(Sphere sphere)
        {
            var obj = new BvhSphere() { sphere = sphere };
            spheres.Add(obj);
            var bounds = obj.sphere.GetBounds(BvhCollisionMargin);
            (tree ??= new()).AddLeaf(bounds);
            return obj;
        }

        public BvhOBB AddCollider(OBB box)
        {
            var obj = new BvhOBB() { box = box };
            boxes.Add(obj);
            var bounds = obj.box.GetBounds(BvhCollisionMargin);
            (tree ??= new()).AddLeaf(bounds);
            return obj;
        }

        public void AddTriangle(BvhTriangle triangle)
        {
            triangles.Add(triangle);
            var vec1 = vertices[triangle.posIndex1];
            var vec2 = vertices[triangle.posIndex2];
            var vec3 = vertices[triangle.posIndex3];
            var leafBounds = new AABB(vec1, vec1).Extend(vec2).Extend(vec3).Margin(BvhCollisionMargin);
            (tree ??= new()).AddLeaf(leafBounds);
        }

        public void AddTriangle(Vector3 v1, Vector3 v2, Vector3 v3)
        {
            var leafBounds = new AABB(v1, v1).Extend(v2).Extend(v3).Margin(0.01f);
            (tree ??= new()).AddLeaf(leafBounds);
        }

        protected override bool DoRead()
        {
            var handler = FileHandler;
            var version = handler.FileVersion;

            Header.Read(handler);

            handler.Seek(Header.verticesOffset);
            vertices.EnsureCapacity(Header.verticesCount);
            for (int i = 0; i < Header.verticesCount; ++i) {
                vertices.Add(handler.Read<Vector3>());
                handler.Skip(4);
            }
            if (Header.uknOffsetOrPadding != 0 || Header.ukn3 != 0 || Header.uknOffset != 0) {
                throw new Exception("IT WASN'T UNUSED!");
            }

            handler.Seek(Header.indicesOffset);
            triangles.EnsureCapacity(Header.indicesCount);
            var uniqGroups = new HashSet<int>();
            for (int i = 0; i < Header.indicesCount; ++i) {
                var item = new BvhTriangle();
                item.Read(handler);
                triangles.Add(item);
                var unknownFields = item.ukn + item.info.ukn0 + item.ukn1 + item.ukn2 + item.ukn3 + item.ukn4 + item.ukn5 + item.ukn6 + item.ukn7;
                if (unknownFields != 0) {
                    // all these fields are known to be always 0 - otherwise we're missing proper handling of them
                    throw new Exception("Unhandled mcol face fields found at index " + i);
                }
                item.info.ValidateUnknowns(version);

                uniqGroups.Add(item.info.partId);
            }

            handler.Seek(Header.spheresOffset);
            spheres.Read(handler, Header.spheresCount);

            handler.Seek(Header.capsulesOffset);
            capsules.Read(handler, Header.capsulesCount);

            handler.Seek(Header.boxesOffset);
            boxes.Read(handler, Header.boxesCount);

            handler.Seek(Header.stringTableOffset);
            for (int i = 0; i < Header.stringCount; ++i) {
                var strOffset1 = handler.ReadInt64();
                var strOffset2 = handler.ReadInt64();
                if (strOffset1 == 0) {
                    throw new Exception("Missing main mcol tag list");
                }

                var str1 = handler.ReadWString(strOffset1);
                var str2 = strOffset2 == 0 ? null : handler.ReadWString(strOffset2);
                stringTable.Add((str1, str2));
            }

            return true;
        }


        protected override bool DoWrite()
        {
            var handler = FileHandler;
            var version = handler.FileVersion;
            Header.verticesCount = vertices.Count;
            Header.indicesCount = triangles.Count;
            Header.stringCount = stringTable.Count;
            Header.spheresCount = spheres.Count;
            Header.capsulesCount = capsules.Count;
            Header.boxesCount = boxes.Count;
            Header.Write(handler);

            handler.Align(16);
            if (tree != null) {
                handler.Write(ref tree.bounds);
                Header.treeDataOffset = handler.Tell();
                if (version >= 9018) {
                    handler.Write(tree.entries.Count);
                    handler.Skip(12);
                } else {
                    handler.Write(tree.entries.Count - 1);
                    handler.Write(tree.entries.Count / 2);
                    handler.Skip(8);
                }
                tree.entries.Write(handler);
                Header.treeDataSize = (int)(handler.Tell() - Header.treeDataOffset);
            }

            Header.verticesOffset = handler.Tell();
            foreach (var v in vertices) {
                handler.Write(v);
                handler.Skip(4);
            }

            // NOTE: there seems to be some sort of varying \0 gap here before indices in vanilla game files
            // seems to be a meaningless serializer quirk

            Header.indicesOffset = handler.Tell();
            triangles.Write(handler);

            Header.spheresOffset = handler.Tell();
            spheres.Write(handler);

            Header.capsulesOffset = handler.Tell();
            capsules.Write(handler);

            Header.boxesOffset = handler.Tell();
            boxes.Write(handler);

            Header.stringTableOffset = handler.Tell();
            WriteBvhStringTable();
            handler.Align(16);

            Header.Write(handler, 0);
            return true;
        }

        public void WriteBvhStringTable()
        {
            var handler = FileHandler;
            var stringRowCount = Header.stringCount;
            for (int i = 0; i < stringRowCount; ++i) {
                handler.WriteOffsetWString(stringTable[i].main);
                if (stringTable[i].sub == null) {
                    handler.WriteInt64(0);
                } else {
                    handler.WriteOffsetWString(stringTable[i].sub!);
                }
            }
            handler.StringTableFlush();
        }

        public void BuildTree()
        {
            if (tree == null) return;

            var leaves = tree.leaves;

            var count = leaves.Count;
            if (count == 0) throw new ArgumentOutOfRangeException(nameof(leaves), "BVH leaf list must not be empty");

            var version = FileHandler.FileVersion;

            if (version >= 9018) {
                // step 1: re-calculate the leaf indices to ensure they adhere to the target engine version's indexing scheme
                for (int i = 0; i < count; ++i) leaves[i].entry.SetLeafIndex(i);

                // step 2: prepare data - sort entries (z-order curve) and generate the morton codes
                var comp = tree.CreateBoundsComparer();
                leaves.Sort(comp);
                var sortedMortonCodes = tree.GenerateMortonCodes();
                // step 3
                var treeRoot = tree.GenerateHierarchy(sortedMortonCodes, leaves, 0, count - 1);

                // step 4: generate linear flattened BVH, set correct internal node indices
                tree.entries.EnsureCapacity(count * 2);
                static void FlattenTree(List<Entry> list, BinaryTreeItem item)
                {
                    list.Add(item.entry);
                    if (!item.IsLeaf) {
                        FlattenTree(list, item.left!);
                        FlattenTree(list, item.right!);
                        item.entry.index = list.Count;
                        item.entry.boundMin = Vector3.Min(item.left!.entry.boundMin, item.right!.entry.boundMin);
                        item.entry.boundMax = Vector3.Max(item.left!.entry.boundMax, item.right!.entry.boundMax);
                    }
                }
                FlattenTree(tree.entries, treeRoot);
            } else {
                // step 1: reset indices since these games want -1 for leaves
                for (int i = 0; i < count; ++i) leaves[i].entry.index = -1;
                // step 2: sort for better spatial efficiency, PROBABLY, I don't quite know the expected layout yet for this older version
                // the leaves must be in the same order as they appear in the file, since they have no object indexes
                var comp = tree.CreateBoundsComparer();
                leaves.Sort(comp);
                var mortonCodes = tree.GenerateMortonCodes();
                var treeRoot = tree.GenerateHierarchy(mortonCodes, leaves, 0, count - 1);
                // for <= mcol.3017, still need to figure out how exactly the internal node structure works
                // [root, ...leaves, ...internalNodes]
                tree.entries.Add(treeRoot.entry);
                treeRoot.entry.index = count * 2 - 1;
                foreach (var leaf in leaves) {
                    tree.entries.Add(leaf.entry);
                }
                static void PushRecursive(List<Entry> list, BinaryTreeItem item, int maxint)
                {
                    if (item.IsLeaf) return;
                    item.entry.index = list.Count;
                    list.Add(item.entry);
                    if (item.left != null) PushRecursive(list, item.left, maxint);
                    if (item.right != null) PushRecursive(list, item.right, list.Count);
                }
                PushRecursive(tree.entries, treeRoot, count * 2 - 1);
                static void BuildAabbs(BvhData data, BinaryTreeItem item)
                {
                    if (item.left != null && item.right != null) {
                        // we could verify that we haven't built the aabb for this node already, but it doesn't really matter
                        item.entry.boundMin = Vector3.Min(item.left.entry.boundMin, item.right.entry.boundMin);
                        item.entry.boundMax = Vector3.Max(item.left.entry.boundMax, item.right.entry.boundMax);
                    }
                    if (item.parent != null) {
                        BuildAabbs(data, item.parent);
                    }
                }
                foreach (var leaf in leaves) {
                    BuildAabbs(this, leaf);
                }
                throw new NotImplementedException("mcol <= 3017 export not yet supported");
            }
        }
    }
}