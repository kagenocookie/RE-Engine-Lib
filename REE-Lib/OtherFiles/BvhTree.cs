using System.Numerics;
using System.Runtime.CompilerServices;
using ReeLib.InternalAttributes;
using ReeLib.via;

// could be implemented without recursion and parallelized but we don't really need absolute speed here
// reference implementations:
// https://developer.nvidia.com/blog/thinking-parallel-part-iii-tree-construction-gpu/
// https://www.pbr-book.org/3ed-2018/Primitives_and_Intersection_Acceleration/Bounding_Volume_Hierarchies#CompactBVHForTraversal

namespace ReeLib.Bvh
{
    public partial class Entry : BaseModel
    {
        public Vector3 boundMin;
        public int index;
        public Vector3 boundMax;
        public int index2;

        public bool isLeaf;
        private const uint indexBits = ~0x80000000;
        private const uint leafBit = 0x80000000;
        public int RealIndex => (int)((uint)index & indexBits);

        public void SetLeafIndex(int index)
        {
            isLeaf = true;
            this.index = (int)(index | leafBit);
        }

        protected override bool DoRead(FileHandler handler)
        {
            handler.Read(ref boundMin);
            handler.Read(ref index);
            handler.Read(ref boundMax);
            handler.Read(ref index2);
            if (index != -1 && (index & leafBit) != 0) {
                isLeaf = true;
                index = (int)((uint)index & indexBits);
            }
            return true;
        }
        protected override bool DoWrite(FileHandler handler)
        {
            handler.Write(ref boundMin);
            handler.Write((isLeaf ? (uint)index | leafBit : (uint)RealIndex));
            handler.Write(ref boundMax);
            handler.Write(ref index2);
            return true;
        }
    }

    public partial class BvhTree
    {
        public readonly List<Entry> entries = new();
        public readonly List<BinaryTreeItem> leaves = new();
        public AABB bounds = ReeLib.via.AABB.MaxMin;

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static uint ExpandBits(uint v)
        {
            v = (v * 0x00010001u) & 0xFF0000FFu;
            v = (v * 0x00000101u) & 0x0F00F00Fu;
            v = (v * 0x00000011u) & 0xC30C30C3u;
            v = (v * 0x00000005u) & 0x49249249u;
            return v;
        }

        public static uint GetMortonCode(Vector3 vec)
        {
            vec.X = Math.Min(Math.Max(vec.X * 1024.0f, 0.0f), 1023.0f);
            vec.Y = Math.Min(Math.Max(vec.Y * 1024.0f, 0.0f), 1023.0f);
            vec.Z = Math.Min(Math.Max(vec.Z * 1024.0f, 0.0f), 1023.0f);
            uint xx = ExpandBits((uint)vec.X);
            uint yy = ExpandBits((uint)vec.Y);
            uint zz = ExpandBits((uint)vec.Z);
            return xx * 4 + yy * 2 + zz;
        }

        public static int FindBvhSplit(uint[] sortedMortonCodes, int first, int last)
        {
            // Identical Morton codes => split the range in the middle.

            uint firstCode = sortedMortonCodes[first];
            uint lastCode = sortedMortonCodes[last];

            if (firstCode == lastCode)
                return (first + last) >> 1;

            // Calculate the number of highest bits that are the same
            // for all objects, using the count-leading-zeros intrinsic.

            int commonPrefix = BitOperations.LeadingZeroCount(firstCode ^ lastCode);

            // Use binary search to find where the next bit differs.
            // Specifically, we are looking for the highest object that
            // shares more than commonPrefix bits with the first one.

            int split = first; // initial guess
            int step = last - first;

            do
            {
                step = (step + 1) >> 1; // exponential decrease
                int newSplit = split + step; // proposed new position

                if (newSplit < last)
                {
                    uint splitCode = sortedMortonCodes[newSplit];
                    int splitPrefix = BitOperations.LeadingZeroCount(firstCode ^ splitCode);
                    if (splitPrefix > commonPrefix)
                        split = newSplit; // accept proposal
                }
            }
            while (step > 1);

            return split;
        }

        public BvhMortonCodeComparer CreateBoundsComparer()
        {
            return new BvhMortonCodeComparer(bounds.minpos, Vector3.One / bounds.Size);
        }

        public uint[] GenerateMortonCodes()
        {
            var count = leaves.Count;
            var rangeRec = Vector3.One / bounds.Size;
            uint[] codes = new uint[count];
            for (int i = 0; i < count; ++i) {
                codes[i] = leaves[i].NormalizeGetMorton(bounds.minpos, rangeRec);
            }
            return codes;
        }
        public BinaryTreeItem GenerateHierarchy(uint[] sortedMortonCodes, List<BinaryTreeItem> sortedObjects, int first, int last)
        {
            if (first == last)
                return sortedObjects[first];

            int split = FindBvhSplit(sortedMortonCodes, first, last);

            BinaryTreeItem left = GenerateHierarchy(sortedMortonCodes, sortedObjects, first, split);
            BinaryTreeItem right = GenerateHierarchy(sortedMortonCodes, sortedObjects, split + 1, last);
            var entry = new BinaryTreeItem(new Entry()) { left = left, right = right };
            left.parent = entry;
            right.parent = entry;
            return entry;
        }

        public void AddLeaf(Vector3 minpos, Vector3 maxpos)
        {
            AddLeaf(new AABB(minpos, maxpos));
        }

        public void AddLeaf(AABB bounds)
        {
            leaves.Add(new BinaryTreeItem(new Entry() { index = leaves.Count, isLeaf = true, boundMin = bounds.minpos, boundMax = bounds.maxpos }));
            this.bounds = this.bounds.Extend(bounds);
        }
    }

    public class BinaryTreeItem
    {
        public Entry entry;

        public BinaryTreeItem? left;
        public BinaryTreeItem? right;
        public BinaryTreeItem? parent;
        public BinaryTreeItem? next;

        public bool IsLeaf => left == null;

        public BinaryTreeItem(Entry entry)
        {
            this.entry = entry;
        }

        public uint NormalizeGetMorton(Vector3 minValue, Vector3 valueRangeReciproc) => BvhTree.GetMortonCode(((entry.boundMin + entry.boundMax) * 0.5f - minValue) * valueRangeReciproc);
    }

    public class BvhMortonCodeComparer : IComparer<BinaryTreeItem>
    {
        public Vector3 min;
        public Vector3 valueRangeReciproc;

        public BvhMortonCodeComparer(Vector3 min, Vector3 valueRangeReciproc)
        {
            this.min = min;
            this.valueRangeReciproc = valueRangeReciproc;
        }

        public int Compare(BinaryTreeItem? x, BinaryTreeItem? y)
        {
            if (x == null || y == null) return 0;

            var q1 = x.NormalizeGetMorton(min, valueRangeReciproc);
            var q2 = y.NormalizeGetMorton(min, valueRangeReciproc);

            return q1.CompareTo(q2);
        }
    }

}