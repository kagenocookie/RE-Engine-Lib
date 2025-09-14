using System.Numerics;
using System.Runtime.InteropServices;
using ReeLib.via;

namespace ReeLib.Mesh
{
	internal enum MeshMainVersion
	{
		Unknown,
		RE7,
		DMC5,
		RE_RT,
		RE8,
		SF6,
		DD2_Old,
		DD2,
		MHWILDS,
	}

    public partial class Header : BaseModel
    {
        public uint magic = MeshFile.Magic;
        public uint version;
        public uint fileSize;
        public uint lodHash;

		public short flags = 0;
		public short nameCount = 0;
		public int ukn = 0;
		public long meshGroupOffset = 0;
		public long lodsOffset = 0;
		public long shadowLodsOffset = 0;
		public long occluderMeshOffset = 0;
		public long bonesOffset = 0;
		public long normalRecalcOffset = 0;
		public long blendShapesOffset = 0;
		public long boundsOffset = 0;
		public long meshOffset = 0;
		public long floatsOffset = 0;
		public long materialIndicesOffset = 0;
		public long boneIndicesOffset = 0;
		public long blendShapeIndicesOffset = 0;
		public long nameOffsetsOffset = 0;

		public short uknCount = 0;
		public int sf6unkn0 = 0;
		public long sf6unkn1 = 0;
		public long streamingInfoOffset = 0;
		public short ukn1 = 0;
		public long sf6unkn4 = 0;

		public long dd2HashOffset = 0;
		public long verticesOffset = 0;

		public int wilds_unkn1 = 0;
		public int wilds_unkn2 = 0;
		public int wilds_unkn3 = 0;
		public int wilds_unkn4 = 0;
		public short wilds_unkn5 = 0;

		internal MeshMainVersion FormatVersion;

        protected override bool DoRead(FileHandler handler)
        {
            handler.Read(ref magic);
			if (magic != MeshFile.Magic) return false;

            handler.Read(ref version);
            handler.Read(ref fileSize);
            handler.Read(ref lodHash);
			var Version = FormatVersion = MeshFile.GetConsistentMeshVersion(version, handler.FileVersion);

			if (Version < MeshMainVersion.SF6)
			{
				handler.Read(ref flags);
				handler.Read(ref nameCount);
				handler.Read(ref ukn);
				handler.Read(ref lodsOffset);
				handler.Read(ref shadowLodsOffset);
				handler.Read(ref occluderMeshOffset);
				handler.Read(ref bonesOffset);
				handler.Read(ref normalRecalcOffset);
				handler.Read(ref blendShapesOffset);
				handler.Read(ref boundsOffset);
				handler.Read(ref meshOffset);
				handler.Read(ref floatsOffset);
				handler.Read(ref materialIndicesOffset);
				handler.Read(ref boneIndicesOffset);
				handler.Read(ref blendShapeIndicesOffset);
				handler.Read(ref nameOffsetsOffset);
			}
			else if (Version >= MeshMainVersion.SF6 && Version < MeshMainVersion.MHWILDS)
			{
				handler.Read(ref flags);
				handler.Read(ref uknCount);
				handler.Read(ref nameCount);
				handler.Read(ref ukn1);
				handler.Read(ref meshGroupOffset);
				handler.Read(ref lodsOffset);
				handler.Read(ref shadowLodsOffset);
				handler.Read(ref occluderMeshOffset);
				handler.Read(ref normalRecalcOffset);
				handler.Read(ref blendShapesOffset);
				handler.Read(ref meshOffset);

				handler.Read(ref sf6unkn1);
				handler.Read(ref floatsOffset);

				handler.Read(ref boundsOffset);
				handler.Read(ref bonesOffset);
				handler.Read(ref materialIndicesOffset);
				handler.Read(ref boneIndicesOffset);
				handler.Read(ref blendShapeIndicesOffset);
				if (Version < MeshMainVersion.DD2_Old)
				{
					handler.Read(ref streamingInfoOffset);
					handler.Read(ref nameOffsetsOffset);
				}
				else
				{
					handler.Read(ref nameOffsetsOffset);
					handler.Read(ref dd2HashOffset);
					handler.Read(ref streamingInfoOffset);
				}
				handler.Read(ref verticesOffset);
				handler.Read(ref sf6unkn4);
			}
			else if (Version >= MeshMainVersion.MHWILDS)
			{
				handler.Read(ref wilds_unkn1);
				handler.Read(ref nameCount);
				handler.Read(ref flags);
				handler.Read(ref uknCount);
				handler.Read(ref wilds_unkn2);
				handler.Read(ref wilds_unkn3);
				handler.Read(ref wilds_unkn4);
				handler.Read(ref wilds_unkn5);
				handler.Read(ref verticesOffset);
				handler.Read(ref meshGroupOffset);
				handler.Read(ref shadowLodsOffset);
				handler.Read(ref occluderMeshOffset);
				handler.Read(ref normalRecalcOffset);
				handler.Read(ref blendShapesOffset);
				handler.Read(ref meshOffset);
				handler.Read(ref sf6unkn1);
			}

            return true;
        }

        protected override bool DoWrite(FileHandler handler)
        {
            throw new NotImplementedException();
        }
    }

	public struct StreamingMeshEntry
	{
		public uint start;
		public uint end;
	}

    public partial class MeshStreamingInfo : BaseModel
    {
		public int unkn1;
		public StreamingMeshEntry[] Entries = [];

        protected override bool DoRead(FileHandler handler)
        {
			var entryCount = handler.Read<int>();
			handler.Read(ref unkn1);
			var entryOffset = handler.Read<long>();
			var pos = handler.Tell();
			handler.Seek(entryOffset);
			Entries = handler.ReadArray<StreamingMeshEntry>(entryCount);
			return true;
        }

        protected override bool DoWrite(FileHandler handler)
        {
            throw new NotImplementedException();
        }
    }

	public enum VertexBufferType : short
	{
		Position = 0,
		NormalsTangents = 1,
		UV0 = 2,
		UV1 = 3,
		BoneWeights = 4,
		Colors = 5,
	}

	public struct MeshBufferItemHeader
	{
		public VertexBufferType type;
		public short size;
		public int offset;
	}

    public class MeshBuffer : BaseModel
    {
		public long elementHeadersOffset;
		public long vertexBufferOffset;
		public long uknOffset;
		public int vertexBufferSize;
		public long faceBufferOffset;
		public int faceVertBufferHeaderSize;
		public short elementCount;
		public short elementCount2;
		public int ukn1;
		public int ukn2;
		public int blendShapeOffset;
		public MeshBufferItemHeader[] BufferHeaders = [];

		public Vector3[] Positions = [];
		public Vector3[] Normals = [];
		public Vector3[] Tangents = [];
		public Vector2[] UV0 = [];
		public Vector2[] UV1 = [];
		public Color[] Colors = [];
		public ushort[] Faces = [];

		private const float ByteDenorm = 1f / 127f;

		internal MeshMainVersion Version;

        protected override bool DoRead(FileHandler handler)
        {
			handler.Read(ref elementHeadersOffset);
			handler.Read(ref vertexBufferOffset);
			if (Version >= MeshMainVersion.SF6)
			{
				handler.Read(ref uknOffset);
				handler.Read(ref vertexBufferSize);
				faceBufferOffset = handler.Read<int>() + vertexBufferOffset;
			}
			else
			{
				handler.Read(ref faceBufferOffset);
				if (Version == MeshMainVersion.RE_RT)
				{
					handler.Read(ref ukn1);
					handler.Read(ref ukn2);
				}
				handler.Read(ref vertexBufferSize);
				handler.Read(ref faceVertBufferHeaderSize);
			}
			handler.Read(ref elementCount);
			handler.Read(ref elementCount2);
			handler.Seek(elementHeadersOffset);
			BufferHeaders = handler.ReadArray<MeshBufferItemHeader>(elementCount);
			handler.Seek(vertexBufferOffset);
            for (int i = 0; i < BufferHeaders.Length; i++)
			{
                var buffer = BufferHeaders[i];
				var bufferStart = vertexBufferOffset + buffer.offset;
				var bufferEnd = (i == elementCount - 1 ? bufferStart + buffer.size * Positions.Length : vertexBufferOffset + BufferHeaders[i + 1].offset);
                handler.Seek(bufferStart);
				var count = (int)(bufferEnd - bufferStart) / buffer.size;
				switch (buffer.type) {
					case VertexBufferType.Position:
						Positions = handler.ReadArray<Vector3>(count);
						break;

					case VertexBufferType.NormalsTangents:
						Normals = new Vector3[count];
						Tangents = new Vector3[count];
						for (int k = 0; k < count; ++k)
						{
							var a = handler.Read<sbyte>();
							var b = handler.Read<sbyte>();
							var c = handler.Read<sbyte>();
							handler.Skip(1);
							Normals[k] = new Vector3(a * ByteDenorm, b * ByteDenorm, c * ByteDenorm);
							a = handler.Read<sbyte>();
							b = handler.Read<sbyte>();
							c = handler.Read<sbyte>();
							handler.Skip(1);
							Tangents[k] = new Vector3(a * ByteDenorm, b * ByteDenorm, c * ByteDenorm);
						}
						break;

					case VertexBufferType.UV0:
						UV0 = new Vector2[count];
						for (int k = 0; k < count; ++k)
						{
							var u = handler.Read<Half>();
							var v = handler.Read<Half>();
							UV0[k] = new Vector2(1f - (float)u, 1f - (float)v);
						}
						break;
					case VertexBufferType.UV1:
						UV1 = new Vector2[count];
						for (int k = 0; k < count; ++k)
						{
							var u = handler.Read<Half>();
							var v = handler.Read<Half>();
							UV1[k] = new Vector2(1f - (float)u, 1f - (float)v);
						}
						break;
					case VertexBufferType.Colors:
						Colors = handler.ReadArray<Color>(count);
						break;
				}
			}

			return true;
        }

        protected override bool DoWrite(FileHandler handler)
        {
            throw new NotImplementedException();
        }
    }

    public class Submesh(MeshBuffer buffer) : BaseModel
    {
        public MeshBuffer Buffer { get; } = buffer;

		public ushort materialIndex;
		public ushort ukn;
		public int streamingOffset;
		public int streamingOffset2;
		public int ukn2;
		public int indicesCount;
		public int facesIndexOffset;
		public int vertsIndexOffset;
		public int vertCount;

		public Span<ushort> Indices => MemoryMarshal.CreateSpan(ref Buffer.Faces[0], Buffer.Faces.Length).Slice(facesIndexOffset, indicesCount);
		public Span<Vector3> Positions => MemoryMarshal.CreateSpan(ref Buffer.Positions[0], Buffer.Positions.Length).Slice(vertsIndexOffset, vertCount);
		public Span<Vector3> Normals => MemoryMarshal.CreateSpan(ref Buffer.Normals[0], Buffer.Normals.Length).Slice(vertsIndexOffset, vertCount);
		public Span<Vector3> Tangents => MemoryMarshal.CreateSpan(ref Buffer.Tangents[0], Buffer.Tangents.Length).Slice(vertsIndexOffset, vertCount);
		public Span<Vector2> UV0 => MemoryMarshal.CreateSpan(ref Buffer.UV0[0], Buffer.UV0.Length).Slice(vertsIndexOffset, vertCount);
		public Span<Vector2> UV1 => MemoryMarshal.CreateSpan(ref Buffer.UV1[0], Buffer.UV1.Length).Slice(vertsIndexOffset, vertCount);
		public Span<Color> Colors => MemoryMarshal.CreateSpan(ref Buffer.Colors[0], Buffer.Colors.Length).Slice(vertsIndexOffset, vertCount);

		internal MeshMainVersion Version;

        protected override bool DoRead(FileHandler handler)
        {
			handler.Read(ref materialIndex);
			handler.Read(ref ukn);
			handler.Read(ref indicesCount);
			handler.Read(ref facesIndexOffset);
			handler.Read(ref vertsIndexOffset);
			if (Version >= MeshMainVersion.RE_RT)
			{
				handler.Read(ref streamingOffset);
				handler.Read(ref streamingOffset2);
			}
			if (Version >= MeshMainVersion.DD2)
			{
				handler.Read(ref ukn2);
			}

			return true;
        }

        protected override bool DoWrite(FileHandler handler)
        {
            throw new NotImplementedException();
        }

        public override string ToString() => $"SubMesh [V: {vertCount} F: {indicesCount / 3}, Mat: {materialIndex}]";
    }

    public class MeshGroup(MeshBuffer buffer) : BaseModel
    {
        public MeshBuffer Buffer { get; } = buffer;
		public byte groupId;
		public byte submeshCount;
		public int vertexCount;
		public int faceCount;
		public List<Submesh> Submeshes { get; } = new();

		internal MeshMainVersion Version;
		internal int meshVertexOffset;

        protected override bool DoRead(FileHandler handler)
        {
			handler.Read(ref groupId);
			handler.Read(ref submeshCount);
			handler.Skip(6);
			handler.Read(ref vertexCount);
			handler.Read(ref faceCount);
			for (var i = 0; i < submeshCount; ++i)
			{
				var sub = new Submesh(Buffer) { Version = Version };
				sub.Read(handler);
				Submeshes.Add(sub);
			}
			for (var i = 0; i < submeshCount; ++i)
			{
				var sub = Submeshes[i];
				if (i < submeshCount - 1)
				{
					sub.vertCount = (int)(Submeshes[i + 1].vertsIndexOffset - sub.vertsIndexOffset);
				}
				else
				{
					sub.vertCount = vertexCount - (int)(sub.vertsIndexOffset - meshVertexOffset);
				}
			}
			return true;
        }

        protected override bool DoWrite(FileHandler handler)
        {
            throw new NotImplementedException();
        }
    }

    public class MeshLOD(MeshBuffer Buffer) : BaseModel
    {
		public List<MeshGroup> MeshGroups { get; } = new();

		public float lodFactor;
		public byte vertexFormat;

		internal MeshMainVersion Version;

        protected override bool DoRead(FileHandler handler)
        {
			var meshCount = handler.Read<byte>();
			handler.Read(ref vertexFormat);
			handler.Skip(2);
			handler.Read(ref lodFactor);
			var headerOffset = handler.ReadInt64();
			handler.Seek(headerOffset);
			var meshOffsets = handler.ReadArray<long>(meshCount);

			var vertOffset = 0;
			var totalIndices = 0;
			var indicesPadding = 0;
			for (int i = 0; i < meshCount; ++i)
			{
				handler.Seek(meshOffsets[i]);
				var mesh = new MeshGroup(Buffer) { Version = Version, meshVertexOffset = vertOffset };
				mesh.Read(handler);
				MeshGroups.Add(mesh);
				vertOffset += mesh.vertexCount;
				totalIndices += mesh.faceCount;
				if (mesh.faceCount % 2 != 0) {
					indicesPadding++;
				}
			}

			handler.Seek(Buffer.faceBufferOffset);
			Buffer.Faces = handler.ReadArray<ushort>(totalIndices + indicesPadding);
			return true;
        }

        protected override bool DoWrite(FileHandler handler)
        {
            throw new NotImplementedException();
        }
    }

    public class MeshData(MeshBuffer Buffer) : BaseModel
    {
		public List<MeshLOD> LODs { get; } = new();

        public int lodCount;
		public int materialCount;
		public int uvCount;
		public int skinWeightCount;
		public int totalMeshCount;
		public long ukn1;
		public Vector4 boundingSphere;
		public AABB boundingBox;

		internal MeshMainVersion Version;

        protected override bool DoRead(FileHandler handler)
        {
			lodCount = handler.Read<byte>();
			materialCount = handler.Read<byte>();
			uvCount = handler.Read<byte>();
			skinWeightCount = handler.Read<byte>();
			handler.Read(ref totalMeshCount);
			if (Version <= MeshMainVersion.DMC5)
			{
				handler.Read(ref ukn1);
			}

			handler.Read(ref boundingSphere);
			handler.Read(ref boundingBox);

			var lodOffsetsStart = handler.Read<long>();
			handler.Seek(lodOffsetsStart);
			var lodOffsets = handler.ReadArray<long>(lodCount);

			// read only lod 1 for now
			handler.Seek(lodOffsets[0]);
			var lod = new MeshLOD(Buffer) { Version = Version };
			lod.Read(handler);
			LODs.Add(lod);

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
    using ReeLib.Mesh;

    public class MeshFile : BaseFile
    {
        public const uint Magic = 0x4853454D;
        public const uint MagicMply = 0x4D504C59;

		public Header Header { get; } = new();
		public MeshStreamingInfo? StreamingInfo { get; set; }
		public MeshBuffer? MeshBuffer { get; set; }
		public List<MeshData> Meshes { get; } = new();

		public List<string> MaterialNames { get; } = new();

		internal static MeshMainVersion GetConsistentMeshVersion(uint internalVersion, int fileVersion) => internalVersion switch
		{
			// internal versions
			386270720 => MeshMainVersion.DMC5,
			21041600 => MeshMainVersion.RE_RT,
			2020091500 => MeshMainVersion.RE8,
			220705151 => MeshMainVersion.SF6,
			220822879 => MeshMainVersion.SF6,
			230517984 => fileVersion == 231011879 ? MeshMainVersion.DD2_Old : MeshMainVersion.DD2,
			240704828 => MeshMainVersion.MHWILDS,

			// file extension versions
			1808282334 => MeshMainVersion.DMC5, // dmc5
			1808312334 => MeshMainVersion.DMC5, // re2
			1902042334 => MeshMainVersion.DMC5, // re3
			2101050001 => MeshMainVersion.RE8, // re8
			2109108288 => MeshMainVersion.RE_RT, // re2rt
			220128762 => MeshMainVersion.RE_RT, // re7rt
			2109148288 => MeshMainVersion.RE_RT, // mhrise
			230110883 => MeshMainVersion.SF6, // sf6
			221108797 => MeshMainVersion.SF6, // re4
			241111606 => MeshMainVersion.MHWILDS,
			_ => MeshMainVersion.MHWILDS, // assume latest format for anything unknown - in case of newer games
		};

        public MeshFile(FileHandler fileHandler) : base(fileHandler)
        {
        }

        protected override bool DoRead()
        {
			var handler = FileHandler;
			var header = Header;
			var magic = handler.ReadInt(handler.Tell());
			if (magic == MagicMply)
			{
				throw new NotImplementedException("MPLY meshes not yet supported");
			}
			MaterialNames.Clear();
			Meshes.Clear();
			MeshBuffer = null;
			StreamingInfo = null;

			header.Read(handler);
			if (header.streamingInfoOffset > 0)
			{
				handler.Seek(header.streamingInfoOffset);
				StreamingInfo = new MeshStreamingInfo();
				StreamingInfo.Read(handler);
			}

			if (header.meshOffset > 0)
			{
				handler.Seek(header.meshOffset);
				MeshBuffer = new() { Version = header.FormatVersion };
				MeshBuffer.Read(handler);
			}

			if (header.lodsOffset > 0 && MeshBuffer != null)
			{
				handler.Seek(header.lodsOffset);
				var mesh = new MeshData(MeshBuffer) { Version = header.FormatVersion };
				mesh.Read(handler);
				Meshes.Add(mesh);
			}

			var strings = new string[header.nameCount];
			if (header.nameOffsetsOffset > 0)
			{
				handler.Seek(header.nameOffsetsOffset);
				for (int i = 0; i < header.nameCount; ++i) strings[i] = handler.ReadOffsetAsciiString();
			}

			if (header.materialIndicesOffset > 0 && Meshes.Count > 0)
			{
				handler.Seek(header.materialIndicesOffset);
				var matCount = Meshes.First().materialCount;
				for (int i = 0; i < matCount; ++i)
				{
					var idx = handler.Read<short>();
					MaterialNames.Add(strings[idx]);
				}
			}

			if (header.boundsOffset > 0)
			{
				handler.Seek(header.boundsOffset);
				// bone bounds
			}

            return true;
        }

        protected override bool DoWrite()
        {
            throw new NotImplementedException();
        }
    }
}
