using System.Numerics;

namespace ReeLib
{
    public abstract class BaseHFField : BaseFile
    {
        public int splitCount;
        public int ukn;
        public int resourceCount;
        public float tileSizeX;
        public float tileSizeY;
        public int ukn2;
        public int ukn3;
        public Vector3 min;
        public Vector3 max;

        public float[] PointHeights = Array.Empty<float>();

        public int TileCount => splitCount * splitCount;
        public int PointCount => (splitCount + 1) * (splitCount + 1);

        public List<string> Strings = new();

        protected BaseHFField(FileHandler fileHandler) : base(fileHandler)
        {
        }

        protected void ReadHeader(int expectedMagic)
        {
            var handler = FileHandler;
            var magic = handler.Read<int>();
            if (magic != expectedMagic) {
                throw new Exception("Invalid magic " + magic);
            }

            handler.ReadRange(ref splitCount, ref ukn3);
            handler.Read(ref min);
            handler.Skip(4);
            handler.Read(ref max);
            handler.Skip(4);
        }

        protected void WriteHeader(int magic)
        {
            var handler = FileHandler;
            handler.Write(magic);
            handler.WriteRange(ref splitCount, ref ukn3);
            handler.Write(ref min);
            handler.Skip(4);
            handler.Write(ref max);
            handler.Skip(4);
        }

        protected void ReadStrings()
        {
            var handler = FileHandler;
            for (var i = 0; i < resourceCount; ++i)
            {
                Strings.Add(handler.ReadOffsetWString());
            }
        }

        protected void WriteStrings()
        {
            var handler = FileHandler;
            var offset = handler.Tell();
            handler.Seek(offset + sizeof(long) * Strings.Count);
            foreach (var coll in Strings) {
                handler.Write(offset, handler.Tell());
                handler.WriteWString(coll);
                offset += sizeof(long);
            }
        }
    }

    /// <summary>
    /// via.physics.CollisionHeightFieldResource, used by via.physics.HeightFieldShape
    /// </summary>
    public class CHFFile : BaseHFField
    {
        public int[] PointData = Array.Empty<int>();
        public uint[] MaskBits = Array.Empty<uint>();
        public int[] CollisionPresetIDs = Array.Empty<int>();

        public const int Magic = 0x20464843;

        public CHFFile(FileHandler fileHandler) : base(fileHandler)
        {
        }

        protected override bool DoRead()
        {
            var handler = FileHandler;
            ReadHeader(Magic);

            handler.ReadOffsetArray(ref PointHeights, PointCount);
            handler.ReadOffsetArray(ref PointData, PointCount);
            handler.ReadOffsetArray(ref MaskBits, PointCount);
            handler.ReadOffsetArray(ref CollisionPresetIDs, TileCount);

            handler.Seek(handler.Read<long>());
            ReadStrings();

            return true;
        }

        protected override bool DoWrite()
        {
            var handler = FileHandler;
            resourceCount = Strings.Count;
            WriteHeader(Magic);
            var offsetPosition = handler.Tell();
            handler.Skip(6 * sizeof(long));

            handler.WriteOffsetArray(PointHeights, ref offsetPosition);
            handler.WriteOffsetArray(PointData, ref offsetPosition);
            handler.WriteOffsetArray(MaskBits, ref offsetPosition);
            handler.WriteOffsetArray(CollisionPresetIDs, ref offsetPosition);

            handler.Write(offsetPosition, handler.Tell());
            WriteStrings();
            return true;
        }
    }

    /// <summary>
    /// via.dynamics.HeightFieldResource, used by via.dynamics.HeightFieldShape
    /// </summary>
    public class HFFile : BaseHFField
    {
        public int[] PointData = Array.Empty<int>();
        public uint[] MaskBits = Array.Empty<uint>();
        public int[] PointData2 = Array.Empty<int>();
        public TileInfo[] TileData = Array.Empty<TileInfo>();

        public class TileInfo
        {
            public int[] data = Array.Empty<int>();
            public int collisionPresetId;
            public int id2, id3, id4;
        }

        public const int Magic = 0x20204648;

        public HFFile(FileHandler fileHandler) : base(fileHandler)
        {
        }

        protected override bool DoRead()
        {
            var handler = FileHandler;
            ReadHeader(Magic);

            var tileCount = TileCount;

            handler.ReadOffsetArray(ref PointHeights, PointCount);
            handler.ReadOffsetArray(ref PointData, PointCount);
            handler.ReadOffsetArray(ref MaskBits, PointCount);
            handler.ReadOffsetArray(ref PointData2, PointCount);
            var tileDataOffset = handler.Read<long>();
            var stringOffset = handler.Read<long>();

            handler.Seek(tileDataOffset);
            TileData = new TileInfo[tileCount];
            for (int i = 0; i < tileCount;++i)
            {
                var data = new TileInfo() {
                    data = handler.ReadArray<int>(8),
                    collisionPresetId = handler.ReadInt(),
                    id2 = handler.ReadInt(),
                    id3 = handler.ReadInt(),
                    id4 = handler.ReadInt(),
                };

                // note: (DD2) every single file has all 0 except the collisionPresetId
                // note2: (DD2) every single TileInfo is identically duplicated twice
                // if (data.data!.Any(n => n != 0) || data.id2 != 0 || data.id3 != 0 || data.id4 != 0) {
                //     throw new Exception("Unexpected notnull in tile data");
                // }
                // if (i % 2 != 0) data.ShouldBeEquivalentTo(TileData[i / 2]);

                TileData[i / 2] = data;
            }

            handler.Seek(stringOffset);
            ReadStrings();

            return true;
        }

        protected override bool DoWrite()
        {
            var handler = FileHandler;
            resourceCount = Strings.Count;
            WriteHeader(Magic);
            var offsetPosition = handler.Tell();
            handler.Skip(6 * sizeof(long));

            handler.WriteOffsetArray(PointHeights, ref offsetPosition);
            handler.WriteOffsetArray(PointData, ref offsetPosition);
            handler.WriteOffsetArray(MaskBits, ref offsetPosition);
            handler.WriteOffsetArray(PointData2, ref offsetPosition);

            handler.Write(offsetPosition, handler.Tell());
            foreach (var td in TileData)
            {
                // write it twice - the files have duplicate data for whatever reason, seems to always be identical pairs
                handler.WriteArray(td.data ?? new int[8]);
                handler.Write(td.collisionPresetId);
                handler.Write(td.id2);
                handler.Write(td.id3);
                handler.Write(td.id4);

                handler.WriteArray(td.data ?? new int[8]);
                handler.Write(td.collisionPresetId);
                handler.Write(td.id2);
                handler.Write(td.id3);
                handler.Write(td.id4);
            }

            handler.Write(offsetPosition, handler.Tell());
            WriteStrings();

            return true;
        }
    }
}