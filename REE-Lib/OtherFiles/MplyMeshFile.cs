using System.Diagnostics;
using System.Numerics;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using ReeLib.Common;
using ReeLib.Mesh;
using ReeLib.via;

namespace ReeLib.MplyMesh
{
    public class MplyHeader : ReadWriteModel
    {
        public uint magic = MplyMeshFile.Magic;
        public uint version;
        public uint fileSize;
        public uint hash;

        public ContentFlags flags;
        public ushort stringCount;

        public long unknOffset0;
        public long unknOffset1;
        public long gpuMeshletOffset;
        public long unknOffset2;

        public long meshletOffset;
        public long meshletBVHOffset;
        public long meshletPartsOffset;

        public long unknOffset3;
        public long unknOffset4;
        public long unknOffset5;
        public long unknOffset6;

        public long unknOffset7;
        public long bonesOffset;
        public long materialIndicesOffset;
        public long boneIndicesOffset;
        public long unknOffset10;
        public long stringTableOffset;
        public long streamingChunkOffset;

        public long sdfPathOffset;
        public string sdfPath = "";

        public uint wilds_unkn0;

		internal MeshSerializerVersion FormatVersion = MeshSerializerVersion.Unknown;

        protected override sealed bool ReadWrite<THandler>(THandler action)
        {
            action.Do(ref magic);
            action.Do(ref version);
            action.Do(ref fileSize);
            action.Do(ref hash);
			if (action.Handler.FileVersion != 0)
			{
				FormatVersion = MeshFile.GetSerializerVersion(version, (uint)action.Handler.FileVersion);
			}
			else if (FormatVersion == MeshSerializerVersion.Unknown)
			{
				throw new Exception("Unknown mesh file format! Unable to load file");
			}

            if (FormatVersion == MeshSerializerVersion.DD2)
            {
                action.Do(ref flags);
                action.Null(2);
                action.Do(ref stringCount);
                action.Null(2);
                action.Do(ref unknOffset0);
                action.Do(ref unknOffset1);
                action.Do(ref meshletOffset);
                action.Do(ref meshletBVHOffset);
                action.Do(ref meshletPartsOffset);

                action.Do(ref unknOffset2);
                action.Do(ref unknOffset3);
                action.Do(ref unknOffset4);
                action.Do(ref unknOffset5);
                action.Do(ref unknOffset6);
                action.Do(ref unknOffset7);

                action.Do(ref materialIndicesOffset);
                action.Do(ref boneIndicesOffset);
                action.Do(ref bonesOffset);
                action.Do(ref stringTableOffset);
                action.Do(ref unknOffset10);
                action.Do(ref streamingChunkOffset);
                action.Do(ref gpuMeshletOffset);
            }
            else
            {
                action.Do(ref wilds_unkn0);
                action.Do(ref stringCount);
                action.Do(ref flags);
                action.Do(ref unknOffset0);
                action.Do(ref unknOffset1);
                action.Do(ref gpuMeshletOffset);
                action.Do(ref unknOffset2);
                action.Do(ref meshletOffset);
                action.Do(ref meshletBVHOffset);
                action.Do(ref meshletPartsOffset);

                action.Do(ref unknOffset3);
                action.Do(ref unknOffset4);
                action.Do(ref unknOffset5);
                action.Do(ref unknOffset6);
                action.Do(ref unknOffset7);
                action.Do(ref bonesOffset);
                action.Do(ref materialIndicesOffset);
                action.Do(ref boneIndicesOffset);
                action.Do(ref unknOffset10);
                action.Do(ref stringTableOffset);
                action.Do(ref streamingChunkOffset);
            }

            action.HandleOffsetWString(ref sdfPath, true);
            return true;
        }
    }

    public class MeshletHeader : BaseModel
    {
        public AABB bounds;
        public byte lodCount;
        public byte loadedLod;
        public byte residentLod;
        public uint lodBits;
        internal readonly uint[] clusterOffsets = new uint[8];
        public readonly float[] lodFactors = new float[8];
        internal readonly uint[] bindlessGeometryOffsets = new uint[8];

        protected override bool DoRead(FileHandler handler)
        {
            handler.Read(ref bounds.minpos);
            handler.Read(ref lodCount);
            handler.Read(ref loadedLod);
            handler.Read(ref residentLod);
            handler.ReadNull(1);
            handler.Read(ref bounds.maxpos);
            handler.Read(ref lodBits);
            handler.ReadArray(clusterOffsets);
            handler.ReadArray(lodFactors);
            handler.ReadArray(bindlessGeometryOffsets);
            return true;
        }

        protected override bool DoWrite(FileHandler handler)
        {
            throw new NotImplementedException();
        }
    }


    public class MeshletLayout : BaseModel
    {
        public MeshletHeader Header { get; } = new();
        public MeshletHeader GpuHeader { get; } = new();

        internal long gpuMeshletOffset;
        internal long wilds_unkn0;
        internal long wilds_unkn1;
        internal uint gpuDataSize;

        public MeshSerializerVersion Version;

        protected override bool DoRead(FileHandler handler)
        {
            handler.Read(ref gpuMeshletOffset);
            Header.Read(handler);

            if (Version >= MeshSerializerVersion.MHWILDS)
            {
                handler.Read(ref wilds_unkn0);
                handler.Read(ref wilds_unkn1);
            }
            handler.Read(ref gpuDataSize);
            handler.ReadNull(4);

            GpuHeader.Read(handler, gpuMeshletOffset);
            return true;
        }

        protected override bool DoWrite(FileHandler handler)
        {
            throw new NotImplementedException();
        }
    }

    public class MeshletBVH : BaseModel
    {
        public Vector3 baseOffset;
        public float scale;
        public ClusterInfo[] ClusterEntries = new ClusterInfo[8];
        public uint[] ClusterOffsets = new uint[8];
        public uint[] BvhOffsets = new uint[8];

        public List<MplyLodGroup> LODs = new();

        public ContentFlags ContentFlags { get; set; }

        public MeshSerializerVersion Version;

        public struct ClusterInfo { public ushort count; public byte ukn1; public byte ukn2; }

        protected override bool DoRead(FileHandler handler)
        {
            var headersOffset = handler.Read<long>();
            var quantizeOffset = handler.Read<long>();
            handler.Read(ref baseOffset);
            handler.Read(ref scale);
            handler.ReadArray(ClusterEntries);
            handler.ReadArray(ClusterOffsets);
            handler.ReadArray(BvhOffsets);

            using var _ = handler.SeekJumpBack();

            for (int i = 0; i < ClusterEntries.Length; ++i)
            {
                if (ClusterEntries[i].count == 0) continue;

                handler.Seek(headersOffset + ClusterOffsets[i]);
                var lod = new MplyLodGroup(this);
                lod.ReadClusterHeaders(handler, ClusterEntries[i].count);
                LODs.Add(lod);
            }
            if (quantizeOffset > 0)
            {
                handler.Seek(quantizeOffset);
                for (int i = 0; i < LODs.Count; ++i)
                {
                    foreach (var chunk in LODs[i].Chunks) handler.Read(ref chunk.Header.quantize);
                }
            }
            return true;
        }

        protected override bool DoWrite(FileHandler handler)
        {
            throw new NotImplementedException();
        }
    }

    public class MeshletCluster : BaseModel
    {
        public MeshletClusterHeader Header;
        public Ushort3 quantize;

        protected override bool DoRead(FileHandler handler)
        {
            handler.Read(ref Header);
            return true;
        }

        protected override bool DoWrite(FileHandler handler)
        {
            throw new NotImplementedException();
        }
    }

    [Flags]
    public enum MeshletCompactedClusterHeader : uint
    {
        VertexCount = 0xff,
        IndexCount = 0x1ff << 8,
        NoRT = 1 << 17,
        AlphaTest = 1 << 18,
        Transparent = 1 << 19,
        PositionCompressLevel = 0b11 << 20,
        MaterialID = 0xffu << 23,
    }

    public struct MeshletClusterHeader
    {
        public MeshletCompactedClusterHeader flags;
        public byte partId;
        public Byte3 centerOffset;

        public uint vertexOffset;
        public uint indicesOffset;

        public readonly int VertexCount => (int)(flags & MeshletCompactedClusterHeader.VertexCount);
        public readonly int IndexCount => (int)(flags & MeshletCompactedClusterHeader.IndexCount) >> 8;
        public readonly int MaterialID => (int)(flags & MeshletCompactedClusterHeader.MaterialID) >> 23;
    }

    public struct Byte3
    {
        public byte X;
        public byte Y;
        public byte Z;

        public readonly Vector3 MplyPosition => new Vector3((X * (1f / 0xff) - 0.5f) / 256f, (Y * (1f / 0xff) - 0.5f) / 256f, (Z * (1f / 0xff) - 0.5f) / 256f);
    }

    public struct Pos10bit3
    {
        public int data;

        public readonly float X => (ushort)((data >>  0) & 0b1111111111) / (float)0b1111111111 - 0.5f;
        public readonly float Y => (ushort)((data >> 10) & 0b1111111111) / (float)0b1111111111 - 0.5f;
        public readonly float Z => (ushort)((data >> 20) & 0b1111111111) / (float)0b1111111111 - 0.5f;

        public readonly Vector3 MplyPosition => new Vector3(X / 64f, Y / 64f, Z / 64f);
    }

    public record struct Ushort3(ushort X, ushort Y, ushort Z)
    {
        public readonly Vector3 MplyPosition => new Vector3(
            X / (float)ushort.MaxValue - 0.5f,
            Y / (float)ushort.MaxValue - 0.5f,
            Z / (float)ushort.MaxValue - 0.5f);

        public override string ToString() => $"{X}, {Y}, {Z}";
    }

    public record struct HFloat2(Half x, Half y)
    {
        public HFloat2(float x, float y) : this((Half)x, (Half)y) { }

        public readonly Vector2 AsVector2 => new Vector2((float)x, (float)y);

        public override string ToString() => $"{x}, {y}";
    }

    [Flags]
    public enum MplyChunkFlags : uint
    {
        UniformNormal = 1 << 0,
        UniformUV0 = 1 << 1,
        UniformUV1 = 1 << 2,
        UniformUV2 = 1 << 3,
        UniformColor = 1 << 4,
        SmallNormals = 1 << 5,
        HasVertexWeights = 1 << 6,
        UniformVertexWeights = 1 << 7,

        HasVertexColor = 1 << 8,
        HasUV1 = 1 << 9,
        HasUV2 = 1 << 10,
        Unknown11 = 1 << 11,
        Use32BitPos = 1 << 12,
        Use24BitPos = 1 << 13,
        Unknown14 = 1 << 14,
        HasTangentBitsBlock = 1 << 15,

        ScaleBit1 = 1 << 16,
        ScaleBit2 = 1 << 17,
        ScaleBit3 = 1 << 18,
        ScaleBit4 = 1 << 19,

        ScaleBit5 = 1 << 20,
        ScaleBit6 = 1 << 21,
        ScaleBit7 = 1 << 22,
        ScaleBit8 = 1 << 23,

        ScaleBit9 = 1 << 24,
        ScaleBit10 = 1 << 25,
        ScaleBit11 = 1 << 26,
        ScaleBit12 = 1 << 27,

        ScaleBit13 = 1 << 28,
        ScaleBit14 = 1 << 29,
        ScaleBit15 = 1 << 30,
        ScaleBit16 = 1u << 31,
    }

    [Flags]
    public enum MplyChunkFlagsDD2 : uint
    {
        UniformNormal = 1 << 0,
        UniformUV0 = 1 << 1,
        UniformUV1 = 1 << 2,
        UniformUV2 = 1 << 3,
        UniformColor = 1 << 4,
        UniformVertexWeights = 1 << 5,
        NoTangents = 1 << 6,
        Unknown7 = 1 << 7,

        HasVertexColor = 1 << 8,
        HasUV1 = 1 << 9,
        HasUV2 = 1 << 10,
        HasVertexWeights = 1 << 11,
        Use32BitPos = 1 << 12,
        unkn5 = 1 << 13,
        unkn6 = 1 << 14,
        Unused = 1 << 15,

        PositionScaling2 = 1 << 16,
        PositionScaling4 = 1 << 17,
        PositionScaling16 = 1 << 18,
        PositionScaling256 = 1 << 19,

        PositionDescaling4 = 1 << 20,
        PositionDescaling64 = 1 << 21,
        PositionDescaling512 = 1 << 22,
        PositionDescaling2 = 1 << 23,

        Unknown24 = 1 << 24,
        Unknown25 = 1 << 25,
        Unknown26 = 1 << 26,
        Unknown27 = 1 << 27,

        KillPerf = 1 << 28,
        Unknown29 = 1 << 29,
        Unknown30 = 1 << 30,
        Unknown31 = 1u << 31,
    }

    public class MeshletChunk : BaseModel
    {
        public MeshletCluster Header { get; } = new();

        public Vector3 center;
        public byte vertCount;
        public byte faceCount;
        public byte materialId;
        public byte partId;

        public CompressedAABB relativeAABB;

        private MplyChunkFlags flags;
        private MplyChunkFlagsDD2 FlagsDD2 => (MplyChunkFlagsDD2)flags;

        public Byte3[] faces = [];
        public byte[] PositionsBuffer = [];
        public byte[] NormalsBuffer = [];
        public HFloat2[] UV0Buffer = [];
        public HFloat2[] UV1Buffer = [];
        public HFloat2[] UV2Buffer = [];
        public Color[] ColorBuffer = [];
        public VertexBoneWeights[] WeightsBuffer = [];
        public byte[]? TangentSignBits;

        public MeshletChunk(MeshletBVH bvh)
        {
            Bvh = bvh;
        }

        private int PositionSize
        {
            get {
                if ((flags & MplyChunkFlags.Use24BitPos) != 0) return 3;
                if ((flags & MplyChunkFlags.Use32BitPos) != 0) return 4;
                return 6;
            }
        }
        private int NormalSize => Bvh.Version switch {
            MeshSerializerVersion.DD2 => (FlagsDD2 & MplyChunkFlagsDD2.NoTangents) != 0 ? 4 : 8,
            _ => (flags & MplyChunkFlags.SmallNormals) != 0 ? 4 : 8
        };

        public bool HasTangents => NormalSize == 8;

        public MeshletBVH Bvh { get; }

        public struct CompressedAABB
        {
            public ushort offsetX;
            public ushort scaleX;
            public ushort offsetY;
            public ushort scaleY;
            public ushort offsetZ;
            public ushort scaleZ;

            public readonly Vector3 Offset => new Vector3(offsetX, offsetY, offsetZ) * (1f / ushort.MaxValue) - new Vector3(0.5f);
            public readonly Vector3 Size => new Vector3(scaleX, scaleY, scaleZ) * (1f / ushort.MaxValue);
        }

        private Vector3 DecodePosition(Vector3 localpos)
        {
            float scale;
            if (Bvh.Version == MeshSerializerVersion.DD2)
            {
                var exp1 = unchecked(((int)flags >> 16) & 0b1111);
                scale = 4f * (1 << exp1);

                if ((flags & MplyChunkFlags.ScaleBit8) != 0) scale *= (1f / 2);
                if ((flags & MplyChunkFlags.ScaleBit5) != 0) scale *= (1f / 4);
                if ((flags & MplyChunkFlags.ScaleBit6) != 0) scale *= (1f / 64);
                if ((flags & MplyChunkFlags.ScaleBit7) != 0) scale *= (1f / 512);
                return localpos * scale + center;
            }

            var num = (uint)flags;
            var divByte = (int)((num >> 24) & 0b11111111);
            var multByte = (int)((num >> 16) & 0b11111111);
            var divShift = (divByte - 127);
            scale = divShift >= 0 ? (1 << divShift) : (1f / (1 << -divShift));
            var offset = 1 << (multByte - divByte);

            return (localpos + relativeAABB.Offset * offset) * scale + center;
        }

        public static void TestMplyValues()
        {
            static void AssertValue(uint flags, float expectedScale, float expectedOffset)
            {
                var flagValue = flags << 16;
                var chunk = new MeshletChunk(new MeshletBVH());
                chunk.relativeAABB = new CompressedAABB();
                chunk.relativeAABB.offsetX = ushort.MaxValue;
                chunk.flags = (MplyChunkFlags)flagValue;
                var basePos = new Vector3(0.5f, 0.5f, 0.5f);
                var decodedX = chunk.DecodePosition(basePos).X;
                var expectedX = (0.5f + 0.5f * expectedOffset) * expectedScale;
                Debug.Assert(decodedX == expectedX, $"MPLY decode {flagValue:X} got {decodedX}, expected {expectedX} (scale {expectedScale}, offset {expectedOffset})");
            }
            AssertValue(0b10000011_10000011, 16,    1); // prag sm30_050_00     | 131 131
            AssertValue(0b10000100_10000100, 32,    1); // prag sm35_043_00     | 132 132
            AssertValue(0b10000000_10000000, 2,     1); // prag sm33_047_00     | 128 128
            AssertValue(0b10000001_10000001, 4,     1); // prag sm31_017_00_02  | 129 129
            AssertValue(0b10000010_10000010, 8,     1); // prag sm39_066_0001   | 130 130
            AssertValue(0b10000010_10000011, 8,     2); // prag sm39_066_0003   | 130 131 | offset <<= 1
            AssertValue(0b10000011_10000100, 16,    2); // prag sm26_011_01     | 131 132 | offset <<= 1
            AssertValue(0b10000110_10000110, 128,   1); // prag sm39_090_05_Env | 134 134
            AssertValue(0b10000000_10000010, 2,     4); // prag sm30_097_00     | 128 130 | offset <<= 2

            AssertValue(0b01111111_10000000, 1,     2); // prag sm34_011_00     | 127 128 | offset <<= 1
            AssertValue(0b01111111_01111111, 1,     1); // prag sm33_031_00     | 127 127 |
            AssertValue(0b01111100_01111101, .125f, 2); // prag sm63_080_05     | 124 125 | offset <<= 1
            AssertValue(0b01111101_01111101, .25f,  1); // prag sm63_080_04     | 125 125
            AssertValue(0b01111101_01111110, .25f,  2); // prag sm63_080_03     | 125 126 | offset <<= 1
            AssertValue(0b01111110_01111110, .5f,   1); //                      | 126 126

            AssertValue(0b01111110_01111111, .5f,   2); // prag sm76_003_02     | 126 127 | offset <<= 1
            AssertValue(0b01111111_10000010, 1,     8); // re9 sm11_006_00      | 127 130 | offset <<= 3
            AssertValue(0b01111111_10000001, 1,     4); // re9 sm16_057_02      | 127 129 | offset <<= 2
        }

        public Vector3 GetPosition(int index)
        {
            // good sample for compressed verts re9: sm12_129_01.mesh.250925211   --- submesh 17 = 24bit, submesh 53 = 32bit
            if (PositionSize == 3)
            {
                var data = MemoryMarshal.Cast<byte, Byte3>(PositionsBuffer);
                var item = data[index];
                return DecodePosition(item.MplyPosition);
            }
            else if (PositionSize == 4)
            {
                var data = MemoryMarshal.Cast<byte, Pos10bit3>(PositionsBuffer);
                var item = data[index];
                return DecodePosition(item.MplyPosition);
            }
            else
            {
                var data = MemoryMarshal.Cast<byte, Ushort3>(PositionsBuffer);
                var item = data[index];
                return DecodePosition(item.MplyPosition);
            }
        }

        public HFloat2 GetUV(int index)
        {
            if ((flags & MplyChunkFlags.UniformUV0) != 0) return UV0Buffer[0];

            return UV0Buffer[index];
        }

        public HFloat2 GetUV1(int index)
        {
            if ((flags & MplyChunkFlags.UniformUV1) != 0) return UV1Buffer[0];

            return UV1Buffer[index];
        }

        public HFloat2 GetUV2(int index)
        {
            if ((flags & MplyChunkFlags.UniformUV2) != 0) return UV2Buffer[0];

            return UV2Buffer[index];
        }

        public Vector3 GetNormal(int index)
        {
            if ((flags & MplyChunkFlags.UniformNormal) != 0) index = 0;
            if (NormalSize == 4)
            {
                var data = MemoryMarshal.Cast<byte, SByte4>(NormalsBuffer);
                return data[index].DequantizeNormal();
            }
            else
            {
                var data = MemoryMarshal.Cast<byte, QuantizedNorTan>(NormalsBuffer);
                return data[index].Normal;
            }
        }

        public QuantizedNorTan GetQuantizedNormal(int index)
        {
            if ((flags & MplyChunkFlags.UniformNormal) != 0) index = 0;
            if (NormalSize == 4)
            {
                var data = MemoryMarshal.Cast<byte, SByte4>(NormalsBuffer);
                var norm = data[index].DequantizeNormal();
                // the tangent here is probably not correct but we mostly don't need it and these types of normals are rare
                var tan = Vector3.Cross(norm, Vector3.Dot(norm, Vector3.UnitY) > 0.9999f ? Vector3.UnitX : Vector3.UnitY);
                var biTanSign = TangentSignBits?.Length > 0
                    ? (Math.Sign(TangentSignBits[index / 8] & (1 << (index % 8))) != 0 ? sbyte.MaxValue : sbyte.MinValue)
                    : sbyte.MaxValue;
                return new QuantizedNorTan(data[index], SByte4.QuantizeNormal(tan, biTanSign));
            }
            else
            {
                var data = MemoryMarshal.Cast<byte, QuantizedNorTan>(NormalsBuffer);
                return data[index];
            }
        }

        protected override bool DoRead(FileHandler handler)
        {
            handler.Read(ref center);
            handler.Read(ref vertCount);
            handler.Read(ref faceCount);
            handler.Read(ref materialId);
            handler.Read(ref partId);

            DataInterpretationException.DebugThrowIf(partId != Header.Header.partId);
            DataInterpretationException.DebugThrowIf(materialId != Header.Header.MaterialID);

            handler.Read(ref relativeAABB);
            handler.Read(ref flags);
            // Log.Info("flags " + flags);
            var hasTangentBits = Bvh.Version == MeshSerializerVersion.DD2 ? false : flags.HasFlag(MplyChunkFlags.SmallNormals) && flags.HasFlag(MplyChunkFlags.HasTangentBitsBlock);
            var hasWeights = Bvh.Version == MeshSerializerVersion.DD2 ? FlagsDD2.HasFlag(MplyChunkFlagsDD2.HasVertexWeights) : flags.HasFlag(MplyChunkFlags.HasVertexWeights);
            var hasUniformWeights = Bvh.Version == MeshSerializerVersion.DD2 ? FlagsDD2.HasFlag(MplyChunkFlagsDD2.UniformVertexWeights) : flags.HasFlag(MplyChunkFlags.UniformVertexWeights);

            faces = handler.ReadArray<Byte3>(faceCount);
            handler.Align(4);

            PositionsBuffer = handler.ReadArray<byte>(PositionSize * vertCount);
            handler.Align(4);

            if (flags.HasFlag(MplyChunkFlags.UniformNormal))
            {
                NormalsBuffer = handler.ReadArray<byte>(NormalSize * 1);
            }
            else
            {
                NormalsBuffer = handler.ReadArray<byte>(NormalSize * vertCount);
            }

            if (hasTangentBits)
            {
                TangentSignBits = handler.ReadArray<byte>(((vertCount + 7) / 8));
                handler.Align(4);
            }

            UV0Buffer = handler.ReadArray<HFloat2>(flags.HasFlag(MplyChunkFlags.UniformUV0) ? 1 : vertCount);

            if (flags.HasFlag(MplyChunkFlags.HasUV1))
            {
                var count = flags.HasFlag(MplyChunkFlags.UniformUV1) ? 1 : vertCount;
                UV1Buffer = handler.ReadArray<HFloat2>(count);
            }

            if (flags.HasFlag(MplyChunkFlags.HasUV2))
            {
                var count = flags.HasFlag(MplyChunkFlags.UniformUV2) ? 1 : vertCount;
                UV2Buffer = handler.ReadArray<HFloat2>(count);
            }

            if (hasWeights) {
                var count = hasUniformWeights ? 1 : vertCount;
                WeightsBuffer = new VertexBoneWeights[count];
                for (int i = 0; i < count; ++i)
                {
                    var w = new VertexBoneWeights(Bvh.Version);
                    w.Read(handler);
                    WeightsBuffer[i] = w;
                }
                handler.Align(4);
            }

            if (flags.HasFlag(MplyChunkFlags.HasVertexColor))
            {
                var count = (flags & MplyChunkFlags.UniformColor) != 0 ? 1 : vertCount;
                ColorBuffer = handler.ReadArray<Color>(count);
            }

            return true;
        }

        protected override bool DoWrite(FileHandler handler)
        {
            throw new NotImplementedException();
        }

        public override string ToString() => center.ToString();
    }

    public class MplyLodGroup : BaseModel
    {
        public List<MeshletChunk> Chunks { get; } = new();
        public MeshletBVH Bvh { get; }

        public MplyLodGroup(MeshletBVH bvh)
        {
            Bvh = bvh;
        }

        internal void ReadClusterHeaders(FileHandler handler, int count)
        {
            for (int i = 0; i < count; ++i)
            {
                var chunk = new MeshletChunk(Bvh);
                chunk.Header.Read(handler);
                Chunks.Add(chunk);
            }
        }

        protected override bool DoRead(FileHandler handler)
        {
            var count = handler.Read<int>();
            var offsets = handler.ReadArray<uint>(count);
            DataInterpretationException.DebugThrowIf(count != Chunks.Count);

            for (int i = 0; i < count; ++i)
            {
                handler.Seek(offsets[i]);
                var sub = Chunks[i];
                sub.Read(handler);
                DataInterpretationException.DebugThrowIf(i < count - 1 && handler.Position != offsets[i + 1] + handler.Offset);
            }

            return true;
        }

        protected override bool DoWrite(FileHandler handler)
        {
            throw new NotImplementedException();
        }
    }
}

namespace ReeLib
{
    using ReeLib.MplyMesh;

    public class MplyMeshFile(FileHandler handler) : BaseFile(handler)
    {
        public const uint Magic = 0x594C504D;

        public MplyHeader Header { get; } = new();
        public MeshletLayout Layout { get; } = new();
        public MeshletBVH BVH { get; } = new();
        public List<int> MeshletParts { get; } = new();
        public MeshStreamingInfo StreamingInfo { get; } = new();
        public List<MplyLodGroup> LODs { get; } = new();
        public List<string> MaterialNames { get; } = new();
        public List<int> MaterialRemaps { get; } = new();
		public MeshBoneHierarchy? BoneData { get; set; }

        protected override bool DoRead()
        {
            var handler = FileHandler;
            var header = Header;
            header.Read(handler);
            if (header.magic != Magic)
            {
                throw new Exception("Not a valid MPLY mesh file!");
            }

            BVH.Version = header.FormatVersion;
            handler.Seek(header.meshletOffset);
            Layout.Read(handler);

            handler.Seek(header.stringTableOffset);
            var strings = new string[header.stringCount];
            for (int i = 0; i < header.stringCount; ++i)
            {
                strings[i] = handler.ReadOffsetAsciiString();
            }

            if (header.bonesOffset > 0)
            {
                handler.Seek(header.bonesOffset);
                BoneData ??= new();
                BoneData.Read(handler);
                handler.Seek(header.boneIndicesOffset);
                BoneData.ReadBoneNames(handler, strings);
            }

            handler.Seek(header.materialIndicesOffset);
            var matCount = header.stringCount - (BoneData?.Bones.Count ?? 0);
            for (int i = 0; i < matCount; ++i)
            {
                var index = handler.Read<short>();
                MaterialNames.Add(strings[i]);
            }

            handler.Seek(header.meshletPartsOffset);
            var partIndicesOffset = handler.Read<long>();
            var partCount = handler.Read<int>();
            handler.Seek(partIndicesOffset);
            MeshletParts.ReadStructList(handler, partCount);

            handler.Seek(header.meshletBVHOffset);
            BVH.Read(handler);

            if (header.streamingChunkOffset > 0)
            {
                handler.Seek(header.streamingChunkOffset);
                StreamingInfo.Read(handler);
            }

            var lodsHandler = handler.WithOffset(header.gpuMeshletOffset);
            for (int i = 0; i < Layout.Header.lodCount; ++i)
            {
                lodsHandler.Seek(Layout.Header.clusterOffsets[i]);
                var lod = BVH.LODs[i];
                lod.Read(lodsHandler);
                LODs.Add(lod);
            }

            return true;
        }

        public void LoadStreamingData(FileHandler streamingHandler)
        {
            // TODO
        }

        /// <summary>
        /// Converts this MPLY formatted mesh into a normal one. Chunks are merged into the minimum required number of submeshes in order to minimize rendering overhead.
        /// </summary>
        public MeshFile ConvertToMergedClassicMesh(int minLod = 0, int maxLod = 0)
        {
            var mesh = new MeshFile(new FileHandler(new MemoryStream(), FileHandler.FilePath));
            maxLod = Math.Max(minLod, maxLod);

            var buffer = new MeshBuffer() { Version = Header.FormatVersion };
            mesh.MeshBuffer = buffer;
            var verts = new List<Vector3>(LODs[minLod].Chunks.Sum(ch => ch.vertCount));
            var normals = new List<QuantizedNorTan>(verts.Capacity);
            var uv0 = new List<HFloat2>(verts.Capacity);
            var uv1 = new List<HFloat2>(verts.Capacity);
            var uv2 = new List<HFloat2>(verts.Capacity);
            var colors = new List<Color>(verts.Capacity);
            var weights = new List<VertexBoneWeights>(verts.Capacity);
            // add a bit of extra capacity for indices to pre-emptively account for padding
            var indices = new List<ushort>(LODs[minLod].Chunks.Sum(ch => ch.faceCount) + 32);

            var data = mesh.MeshData = new MeshData(buffer) {
                lodCount = (maxLod - minLod) + 1,
                materialCount = MaterialNames.Count,
                Version = Header.FormatVersion,
            };
            mesh.MaterialNames.AddRange(MaterialNames);
            mesh.Header.version = Header.version;
            mesh.Header.FormatVersion = Header.FormatVersion;
            mesh.Header.flags = Header.flags;
            mesh.BoneData = BoneData?.Clone();

            for (int lodIndex = minLod; lodIndex <= maxLod; ++lodIndex)
            {
                var mplyLod = LODs[lodIndex];
                var outLod = new MeshLOD(buffer) { Version = mesh.Header.FormatVersion };
                outLod.lodFactor = Layout.Header.lodFactors[lodIndex];
                data.LODs.Add(outLod);

                // sort it because we need submeshes to be contiguous if we want to minimize their number
                var orderedChunks = mplyLod.Chunks.OrderBy(ch => ch.partId).ThenBy(ch => ch.materialId).ToList();

                MeshGroup? group = null;
                foreach (var chunk in orderedChunks)
                {
                    if (group == null || group.groupId != chunk.partId) {
                        outLod.MeshGroups.Add(group = new MeshGroup(buffer) {
                            groupId = chunk.partId,
                            Version = mesh.Header.FormatVersion,
                        });
                    }

                    var sub = group.Submeshes.Count == 0 ? null : group.Submeshes[^1];
                    // if (sub == null || sub.vertCount > 0) // this one will force separate submeshes per chunk, for debugging
                    if (sub == null || sub.vertCount + group.vertexCount > ushort.MaxValue || sub.materialIndex != chunk.materialId)
                    {
                        // ensure 4-byte alignment padding is accounted for
                        if (sub != null && (sub.vertCount % 2) != 0) indices.Add(0);

                        group.Submeshes.Add(sub = new Submesh(buffer) {
                            materialIndex = (ushort)chunk.materialId,
                            facesIndexOffset = indices.Count,
                            vertsIndexOffset = verts.Count,
                            Version = mesh.Header.FormatVersion,
                        });
                        group.submeshCount++;
                    }
                    var vertOffset = sub.vertCount;

                    sub.vertCount += chunk.vertCount;
                    sub.indicesCount += chunk.faceCount * 3;
                    group.indicesCount += chunk.faceCount * 3;
                    group.vertexCount += chunk.vertCount;

                    for (int i = 0; i < chunk.vertCount; ++i)
                    {
                        verts.Add(chunk.GetPosition(i));
                        uv0.Add(chunk.GetUV(i));
                    }

                    for (int i = 0; i < chunk.vertCount; ++i)
                    {
                        normals.Add(chunk.GetQuantizedNormal(i));
                    }

                    if (chunk.UV1Buffer.Length > 0)
                    {
                        for (int i = 0; i < chunk.vertCount; ++i) uv1.Add(chunk.GetUV1(i));
                    }

                    if (chunk.UV2Buffer.Length > 0)
                    {
                        for (int i = 0; i < chunk.vertCount; ++i) uv2.Add(chunk.GetUV2(i));
                    }

                    if (chunk.ColorBuffer.Length == 1)
                    {
                        for (int i = 0; i < chunk.vertCount; ++i) colors.Add(chunk.ColorBuffer[0]);
                    }
                    else if (chunk.ColorBuffer.Length > 0)
                    {
                        colors.AddRange(chunk.ColorBuffer);
                    }

                    if (chunk.WeightsBuffer.Length == 1)
                    {
                        for (int i = 0; i < chunk.vertCount; ++i) weights.Add(chunk.WeightsBuffer[0]);
                    }
                    else if (chunk.WeightsBuffer.Length > 0)
                    {
                        weights.AddRange(chunk.WeightsBuffer);
                    }

                    foreach (var index in chunk.faces)
                    {
                        indices.Add((ushort)(vertOffset + index.X));
                        indices.Add((ushort)(vertOffset + index.Y));
                        indices.Add((ushort)(vertOffset + index.Z));
                    }
                }
            }

            buffer.Positions = verts.ToArray();
            buffer.UV0 = uv0.ToArray();
            buffer.UV1 = uv1.ToArray();
            buffer.UV2 = uv2.ToArray();
            buffer.NormalsTangents = normals.ToArray();
            buffer.Colors = colors.ToArray();
            buffer.Weights = weights.ToArray();
            buffer.Faces = indices.ToArray();
            data.RecalculateBoundingBox();
            return mesh;
        }

        protected override bool DoWrite()
        {
            throw new NotImplementedException();
        }
    }
}