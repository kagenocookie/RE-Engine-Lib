using System.Numerics;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
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
        public long unknOffset8;
        public long materialNameRemapOffset;
        public long unknOffset9;
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

                action.Do(ref materialNameRemapOffset);
                action.Do(ref unknOffset8);
                action.Do(ref unknOffset9);
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
                action.Do(ref unknOffset8);
                action.Do(ref materialNameRemapOffset);
                action.Do(ref unknOffset9);
                action.Do(ref unknOffset10);
                action.Do(ref stringTableOffset);
                action.Do(ref streamingChunkOffset);
            }

            action.HandleOffsetWString(ref sdfPath);
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
                handler.Seek(headersOffset);
                for (int i = 0; i < ClusterEntries.Length; ++i)
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
    }

    public struct Pos10bit3
    {
        public int data;

        public readonly ushort X => (ushort)((data) & 0b1111111111);
        public readonly ushort Y => (ushort)((data >> 10) & 0b1111111111);
        public readonly ushort Z => (ushort)((data >> 20) & 0b1111111111);

        public readonly Vector3 AsVector3 => new Vector3(X / (float)0xb1111111111, Y / (float)0xb1111111111, Z / (float)0xb1111111111);
    }

    public record struct Ushort3(ushort X, ushort Y, ushort Z)
    {
        public readonly Vector3 AsVector3 => new Vector3(X / (float)ushort.MaxValue, Y / (float)ushort.MaxValue, Z / (float)ushort.MaxValue);

        public override string ToString() => $"{X}, {Y}, {Z}";
    }

    public record struct HFloat2(Half x, Half y)
    {
        public readonly Vector2 AsVector2 => new Vector2((float)x, (float)y);

        public override string ToString() => $"{x}, {y}";
    }

    public enum MplyChunkFlags : uint
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
        HasUnknStruct = 1 << 15,

        PositionScaling2 = 1 << 16,
        PositionScaling4 = 1 << 17,
        PositionScaling16 = 1 << 18,
        PositionScaling256 = 1 << 19,

        PositionDescaling4 = 1 << 20,
        PositionDescaling64 = 1 << 21,
        PositionDescaling512 = 1 << 22,
        PositionDescaling2 = 1 << 23,

        // note: these don't seem to be doing anything (dd2)
        Scale2 = 1 << 24,
        Scale4 = 1 << 25,
        Scale8 = 1 << 26,
        Scale256 = 1 << 27,

        KillPerf = 1 << 28,
        Unkn21 = 1 << 29,
        Unkn22 = 1 << 30,
        UseScalingMaybe = 1u << 31,
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

        public MplyChunkFlags flags;

        public Byte3[] faces = [];
        public byte[] PositionsBuffer = [];
        public byte[] NormalsBuffer = [];
        public HFloat2[] UV0Buffer = [];
        public HFloat2[] UV1Buffer = [];
        public HFloat2[] UV2Buffer = [];
        public Color[] ColorBuffer = [];
        public VertexBoneWeights[] WeightsBuffer = [];
        public short[]? uknFixedSizeData;

        public MeshletChunk(MeshletBVH bvh)
        {
            Bvh = bvh;
        }

        private int PositionSize => (flags & MplyChunkFlags.Use32BitPos) != 0 ? 4 : 6;
        private int NormalSize => (flags & MplyChunkFlags.NoTangents) != 0 ? 4 : 8;

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
        }

        private float PositionScaling {
            get {
                var scale = 4f;
                if ((flags & MplyChunkFlags.PositionScaling2) != 0) scale *= 2;
                if ((flags & MplyChunkFlags.PositionScaling4) != 0) scale *= 4;
                if ((flags & MplyChunkFlags.PositionScaling16) != 0) scale *= 16;
                if ((flags & MplyChunkFlags.PositionScaling256) != 0) scale *= 256;

                if ((flags & MplyChunkFlags.PositionDescaling2) != 0) scale *= (1f / 2);
                if ((flags & MplyChunkFlags.PositionDescaling4) != 0) scale *= (1f / 4);
                if ((flags & MplyChunkFlags.PositionDescaling64) != 0) scale *= (1f / 64);
                if ((flags & MplyChunkFlags.PositionDescaling512) != 0) scale *= (1f / 512);
                return scale;
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private Vector3 DecodePosition(Vector3 vertPos)
            => (vertPos - new Vector3(0.5f)) * PositionScaling + center
        ;

        public Vector3 GetPosition(int index)
        {
            if (PositionSize == 4)
            {
                var data = MemoryMarshal.Cast<byte, Pos10bit3>(PositionsBuffer);
                var item = data[index];
                return DecodePosition(item.AsVector3);
            }
            else
            {
                var data = MemoryMarshal.Cast<byte, Ushort3>(PositionsBuffer);
                var item = data[index];
                return DecodePosition(item.AsVector3);
            }
        }

        public Vector2 GetUV(int index)
        {
            if ((flags & MplyChunkFlags.UniformUV0) != 0) return UV0Buffer[0].AsVector2;

            return UV0Buffer[index].AsVector2;
        }

        public Vector2 GetUV1(int index)
        {
            if ((flags & MplyChunkFlags.UniformUV1) != 0) return UV1Buffer[0].AsVector2;

            return UV1Buffer[index].AsVector2;
        }

        public Vector2 GetUV2(int index)
        {
            if ((flags & MplyChunkFlags.UniformUV2) != 0) return UV2Buffer[0].AsVector2;

            return UV2Buffer[index].AsVector2;
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
                var data = MemoryMarshal.Cast<byte, NorTanVertexBuffer>(NormalsBuffer);
                return data[index].Normal;
            }
        }

        public (Vector3 nor, Vector3 tan, Vector3 bitan) GetNormalBiTangents(int index)
        {
            if (NormalSize == 4) throw new NotSupportedException("Mesh does not contain tangents");

            if ((flags & MplyChunkFlags.UniformNormal) != 0) index = 0;
            var data = MemoryMarshal.Cast<byte, NorTanVertexBuffer>(NormalsBuffer);
            var item = data[index];
            return item.GetAll();
        }

        public (Vector3 nor, Vector3 tan, sbyte bitanSign) GetNormalBiTangentData(int index)
        {
            if (NormalSize == 4) throw new NotSupportedException("Mesh does not contain tangents");

            if ((flags & MplyChunkFlags.UniformNormal) != 0) index = 0;
            var data = MemoryMarshal.Cast<byte, NorTanVertexBuffer>(NormalsBuffer);
            var item = data[index];
            return (item.Normal, item.Tangent, item.BiTangentSign);
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

            if (flags.HasFlag(MplyChunkFlags.HasUnknStruct))
            {
                uknFixedSizeData = handler.ReadArray<short>(6);
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

            if (flags.HasFlag(MplyChunkFlags.HasVertexWeights)) {
                var count = flags.HasFlag(MplyChunkFlags.UniformVertexWeights) ? 1 : vertCount;
                WeightsBuffer = new VertexBoneWeights[count];
                for (int i = 0; i < count; ++i)
                {
                    var w = new VertexBoneWeights();
                    w.Read(handler, Bvh.Version);
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
                // DataInterpretationException.DebugThrowIf(i > 0 && handler.Position != offsets[i] + handler.Offset);
                handler.Seek(offsets[i]);
                var sub = Chunks[i];
                sub.Read(handler);
                Chunks.Add(sub);
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
            for (int i = 0; i < header.stringCount; ++i)
            {
                MaterialNames.Add(handler.ReadOffsetAsciiString());
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

            handler.Seek(header.materialNameRemapOffset);
            for (int i = 0; i < header.stringCount; ++i)
            {
                var index = handler.Read<short>();
                MaterialRemaps.Add(index);
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

            var buffer = new MeshBuffer();
            mesh.MeshBuffer = buffer;
            var verts = new List<Vector3>(LODs[minLod].Chunks.Sum(ch => ch.vertCount));
            var normals = new List<Vector3>(verts.Capacity);
            var tangents = new List<Vector3>(verts.Capacity);
            var tanSigns = new List<sbyte>(verts.Capacity);
            var uv0 = new List<Vector2>(verts.Capacity);
            var uv1 = new List<Vector2>(verts.Capacity);
            var colors = new List<Color>(verts.Capacity);
            var weights = new List<VertexBoneWeights>(verts.Capacity);
            // add a bit of extra capacity for indices to pre-emptively account for padding
            var indices = new List<ushort>(LODs[minLod].Chunks.Sum(ch => ch.faceCount) + 32);

            var data = mesh.MeshData = new MeshData(buffer) {
                lodCount = (maxLod - minLod) + 1,
                materialCount = MaterialNames.Count,
            };
            mesh.MaterialNames.AddRange(MaterialNames);
            mesh.Header.version = Header.version;
            mesh.Header.FormatVersion = Header.FormatVersion;
            mesh.Header.flags = Header.flags;

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

                    if (chunk.HasTangents)
                    {
                        for (int i = 0; i < chunk.vertCount; ++i)
                        {
                            var (nor, tan, bi) = chunk.GetNormalBiTangentData(i);
                            normals.Add(nor);
                            tangents.Add(tan);
                            tanSigns.Add(bi);
                        }
                    }
                    else
                    {
                        for (int i = 0; i < chunk.vertCount; ++i)
                        {
                            normals.Add(chunk.GetNormal(i));
                        }
                    }

                    if (chunk.UV1Buffer.Length > 0)
                    {
                        for (int i = 0; i < chunk.vertCount; ++i) uv1.Add(chunk.GetUV1(i));
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
            buffer.Normals = normals.ToArray();
            buffer.Tangents = tangents.ToArray();
            buffer.BiTangentSigns = tanSigns.ToArray();
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