using System.Numerics;
using ReeLib.InternalAttributes;
using ReeLib.via;

namespace ReeLib.Bvh
{
    [RszGenerate, RszAutoReadWrite, RszVersionedObject(typeof(int)), RszAssignVersion]
    public partial class BvhHeader : BaseModel
    {
        public uint magic = BvhData.Magic;
        public int verticesCount;
        [RszPaddingAfter(4)]
        public int indicesCount;
        public int stringCount;
        [RszVersion(3017, EndAt = nameof(boxesCount))]
        public int spheresCount;
        public int capsulesCount;
        [RszPaddingAfter(4)]
        public int boxesCount;

        public int treeDataSize;
        public long treeDataOffset;
        public long verticesOffset;
        [RszPaddingAfter(8)]
        public long indicesOffset;

        public long stringTableOffset;
        [RszVersion(3017, EndAt = nameof(boxesOffset))]
        public long spheresOffset;
        public long capsulesOffset;
        [RszPaddingAfter(8)]
        public long boxesOffset;
    }

    [RszGenerate, RszAutoReadWrite, RszAssignVersion, RszVersionedObject(typeof(int))]
    public partial class TriangleInfo : BaseModel
    {
        public int ukn0;
        public int layerIndex;
        [RszVersion(13008)] // re2rt
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

    [RszGenerate, RszAssignVersion, RszVersionedObject(typeof(int))]
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

        protected override bool DoRead(FileHandler handler)
        {
            if (handler.FileVersion <= 4) {
                return ReadRE7(handler);
            } else {
                return DefaultRead(handler);
            }
        }

        protected override bool DoWrite(FileHandler handler)
        {
            if (handler.FileVersion <= 4) {
                return WriteRE7(handler);
            } else {
                return DefaultWrite(handler);
            }
        }

        public bool ReadRE7(FileHandler handler)
        {
            handler.Skip(7 * 4);
            // note: I don't think this is actually a part id, but it probably doesn't matter, we can use this to store it either way
            handler.Read(ref info.partId);
            handler.Read(ref posIndex1);
            handler.Read(ref posIndex2);
            handler.Read(ref posIndex3);
            handler.Read(ref edgeIndex1);
            handler.Read(ref edgeIndex2);
            handler.Read(ref edgeIndex3);
            handler.Read(ref info.layerIndex);
            handler.Read(ref ukn);
            return true;
        }

        public bool WriteRE7(FileHandler handler)
        {
            handler.Skip(7 * 4);
            handler.Write(ref info.partId);
            handler.Write(ref posIndex1);
            handler.Write(ref posIndex2);
            handler.Write(ref posIndex3);
            handler.Write(ref edgeIndex1);
            handler.Write(ref edgeIndex2);
            handler.Write(ref edgeIndex3);
            handler.Write(ref info.layerIndex);
            handler.Write(ref ukn);
            return true;
        }
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

namespace ReeLib
{
    using System.Runtime.InteropServices;
    using ReeLib.Bvh;

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

            var entryCount = handler.Read<int>();
            var leafCount = handler.Read<int>();
            handler.Skip(8);
            tree.entries.Clear();

            if (leafCount > 0) {
                // there's an extra bounds/root node equivalent to the base tree bounds that doesn't seem to really have a meaningful function, skip it
                handler.Read<AABB>();
            }
            tree.entries.EnsureCapacity(entryCount);
            tree.entries.Read(handler, entryCount);
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

            handler.Seek(Header.indicesOffset);
            triangles.EnsureCapacity(Header.indicesCount);
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
                if (version <= 4) { // re7: mcol.2 and terr.4
                    stringTable.Add((strOffset1.ToString(), null));
                } else {
                    var str1 = strOffset1 == 0 ? string.Empty : handler.ReadWString(strOffset1);
                    var str2 = strOffset2 == 0 ? null : handler.ReadWString(strOffset2);
                    stringTable.Add((str1, str2));
                }
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
                    handler.Write(tree.entries.Count);
                    handler.Write(tree.leaves.Count);
                    handler.Skip(8);
                    handler.Write(tree.bounds);
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
            int version = handler.FileVersion;
            for (int i = 0; i < stringRowCount; ++i) {
                if (version == 2) {
                    _ = int.TryParse(stringTable[i].main, out var main);
                    handler.WriteInt(main);
                    handler.FillBytes(0, 12);
                } else {
                    if (string.IsNullOrEmpty(stringTable[i].main)) {
                        handler.WriteInt64(0);
                    } else {
                        handler.WriteOffsetWString(stringTable[i].main);
                    }
                    if (string.IsNullOrEmpty(stringTable[i].sub)) {
                        handler.WriteInt64(0);
                    } else {
                        handler.WriteOffsetWString(stringTable[i].sub!);
                    }
                }
            }
            handler.StringTableFlush();
        }

        public void RegenerateNodeBoundaries()
        {
            tree ??= new();
            tree.leaves.Clear();
            tree.entries.Clear();
            tree.bounds = AABB.MaxMin;
            foreach (var obj in triangles) {
                var bounds = new AABB(vertices[obj.posIndex1], vertices[obj.posIndex1])
                    .Extend(vertices[obj.posIndex2])
                    .Extend(vertices[obj.posIndex3])
                    .Margin(BvhCollisionMargin);
                tree.AddLeaf(bounds);
            }
            foreach (var obj in spheres) tree.AddLeaf(obj.sphere.GetBounds(BvhCollisionMargin));
            foreach (var obj in capsules) tree.AddLeaf(obj.capsule.GetBounds(BvhCollisionMargin));
            foreach (var obj in boxes) tree.AddLeaf(obj.box.GetBounds(BvhCollisionMargin));
        }

        public void BuildTree()
        {
            if (tree == null) return;

            var leaves = tree.leaves;

            var count = leaves.Count;
            if (count == 0) throw new ArgumentOutOfRangeException(nameof(leaves), "BVH leaf list must not be empty");

            var version = FileHandler.FileVersion;

            tree.entries.EnsureCapacity(count * 2);
            if (version >= 9018) {
                // step 1: reset the leaf indices to ensure they adhere to the target engine's indexing scheme
                // this needs to be done before we sort the bvh entries
                for (int i = 0; i < count; ++i) leaves[i].entry.SetLeafIndex(i);

                // step 2: prepare data - sort entries (z-order curve)
                leaves.Sort(tree.CreateBoundsComparer());
                // step 3: generate the morton codes and build up the hierarchy
                var sortedMortonCodes = tree.GenerateMortonCodes();
                var treeRoot = tree.GenerateHierarchy(sortedMortonCodes, leaves, 0, count - 1);

                // step 4: generate linear flattened BVH, set correct internal node indices
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
                // step 1: sort for better spatial efficiency
                static void SortDoubleList<T2>(List<BinaryTreeItem> entries, int start, List<T2> dataList, BvhMortonCodeComparer comparer) {
                    if (dataList.Count == 0) return;
                    var arr1 = CollectionsMarshal.AsSpan(entries).Slice(start, dataList.Count);
                    var arr2 = CollectionsMarshal.AsSpan(dataList);
                    MemoryExtensions.Sort(arr1, arr2, comparer);
                }
                var prevSectionsCount = 0;
                var comp = tree.CreateBoundsComparer();
                // we must keep the object types in the same order since there's no indexes here, sort each section separately
                SortDoubleList(leaves, 0, triangles, comp);
                SortDoubleList(leaves, prevSectionsCount += triangles.Count, spheres, comp);
                SortDoubleList(leaves, prevSectionsCount += spheres.Count, capsules, comp);
                SortDoubleList(leaves, prevSectionsCount += capsules.Count, boxes, comp);

                // step 2: generate the morton codes and build up the hierarchy
                var treeRoot = tree.GenerateHierarchy(tree.GenerateMortonCodes(), leaves, 0, count - 1);

                // step 3: flatten the hierarchy
                // non-leaf nodes contain the index of their left and right branches
                // leaf nodes have both indexes at - 1
                foreach (var leaf in leaves) tree.entries.Add(leaf.entry);
                BuildSubtree(tree.entries, treeRoot);
                static void BuildSubtree(List<Entry> list, BinaryTreeItem item)
                {
                    if (item.IsLeaf) {
                        item.entry.index = item.entry.index2 = -1;
                        return;
                    }

                    list.Add(item.entry);
                    var left = item.left!;
                    var right = item.right!;

                    BuildSubtree(list, left);
                    BuildSubtree(list, right);
                    item.entry.index = list.IndexOf(left.entry);
                    item.entry.index2 = list.IndexOf(right.entry);

                    item.entry.boundMin = Vector3.Min(item.left!.entry.boundMin, item.right!.entry.boundMin);
                    item.entry.boundMax = Vector3.Max(item.left!.entry.boundMax, item.right!.entry.boundMax);
                }
            }
        }
    }
}