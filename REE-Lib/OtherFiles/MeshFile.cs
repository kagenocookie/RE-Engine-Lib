using System.Numerics;
using System.Runtime.InteropServices;
using ReeLib.Common;
using ReeLib.via;

namespace ReeLib.Mesh
{
	public enum MeshSerializerVersion
	{
		Unknown,
		RE7,
		DMC5,
		RE_RT,
		RE8,
		RE4,
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

		internal MeshSerializerVersion FormatVersion;

        protected override bool DoRead(FileHandler handler)
        {
            handler.Read(ref magic);
			if (magic != MeshFile.Magic) return false;

            handler.Read(ref version);
            handler.Read(ref fileSize);
            handler.Read(ref lodHash);
			var Version = FormatVersion = MeshFile.GetSerializerVersion(version, (uint)handler.FileVersion);

			if (Version < MeshSerializerVersion.RE4)
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
			else if (Version >= MeshSerializerVersion.RE4 && Version < MeshSerializerVersion.MHWILDS)
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
				if (Version < MeshSerializerVersion.DD2_Old)
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
			else if (Version >= MeshSerializerVersion.MHWILDS)
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

	public class VertexBoneWeights
	{
		public int[] boneIndices = [];
		public float[] boneWeights = [];
		private static readonly byte[] byteArray8 = new byte[8];

        public VertexBoneWeights()
			: this(MeshSerializerVersion.MHWILDS)
        {
        }

        public VertexBoneWeights(MeshSerializerVersion version)
        {
            boneIndices = new int[GetIndexCount(version)];
			boneWeights = new float[8];
        }

		public void NormalizeWeights()
		{
			var sum = boneWeights.Sum();
			for (int i = 0; i < boneWeights.Length; ++i) boneWeights[i] /= sum;
		}

		private static int GetIndexCount(MeshSerializerVersion version) => version == MeshSerializerVersion.SF6 ? 6 : 8;

		public void ChangeVersion(MeshSerializerVersion version)
		{
			if (GetIndexCount(version) != boneIndices.Length) {
				UpdateArrays(version);
			}
		}

        private void UpdateArrays(MeshSerializerVersion version)
        {
			var previous = boneIndices;
            boneIndices = new int[GetIndexCount(version)];
			if (previous.Length > 0)
			{
				Array.Copy(previous, boneIndices, Math.Min(previous.Length, boneIndices.Length));
			}
        }

        internal void Read(FileHandler handler, MeshSerializerVersion version)
		{
			if (version == MeshSerializerVersion.SF6) {
				var b1 = handler.Read<uint>();
				var b2 = handler.Read<uint>();
				boneIndices = new int[6];
				boneIndices[0] = (int)((b1) & 0x3ff);
				boneIndices[1] = (int)((b1 >> 10) & 0x3ff);
				boneIndices[2] = (int)((b1 >> 20) & 0x3ff);
				boneIndices[3] = (int)((b2) & 0x3ff);
				boneIndices[4] = (int)((b2 >> 10) & 0x3ff);
				boneIndices[5] = (int)((b2 >> 20) & 0x3ff);
			} else {
				handler.ReadArray(byteArray8);
				boneIndices = byteArray8.Select(b => (int)b).ToArray();
			}
			handler.ReadArray(byteArray8);
			boneWeights = byteArray8.Select(b => ((float)b) / 255f).ToArray();
		}
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
		public VertexBoneWeights[] Weights = [];
		public ushort[] Faces = [];

		private const float ByteDenorm = 1f / 127f;

		internal MeshSerializerVersion Version;

        protected override bool DoRead(FileHandler handler)
        {
			handler.Read(ref elementHeadersOffset);
			handler.Read(ref vertexBufferOffset);
			if (Version >= MeshSerializerVersion.RE4)
			{
				handler.Read(ref uknOffset);
				handler.Read(ref vertexBufferSize);
				faceBufferOffset = handler.Read<int>() + vertexBufferOffset;
			}
			else
			{
				handler.Read(ref faceBufferOffset);
				if (Version == MeshSerializerVersion.RE_RT)
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
							UV0[k] = new Vector2((float)u, (float)v);
						}
						break;
					case VertexBufferType.UV1:
						UV1 = new Vector2[count];
						for (int k = 0; k < count; ++k)
						{
							var u = handler.Read<Half>();
							var v = handler.Read<Half>();
							UV1[k] = new Vector2((float)u, (float)v);
						}
						break;
					case VertexBufferType.Colors:
						Colors = handler.ReadArray<Color>(count);
						break;
					case VertexBufferType.BoneWeights:
						Weights = new VertexBoneWeights[count];
						for (int k = 0; k < count; ++k)
						{
							Weights[k] = new();
							Weights[k].Read(handler, Version);
						}
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

		public Span<ushort> Indices => Buffer.Faces.AsSpan(facesIndexOffset, indicesCount);
		public Span<Vector3> Positions => Buffer.Positions.AsSpan(vertsIndexOffset, vertCount);
		public Span<Vector3> Normals => Buffer.Normals.AsSpan(vertsIndexOffset, vertCount);
		public Span<Vector3> Tangents => Buffer.Tangents.AsSpan(vertsIndexOffset, vertCount);
		public Span<Vector2> UV0 => Buffer.UV0.AsSpan(vertsIndexOffset, vertCount);
		public Span<Vector2> UV1 => Buffer.UV1.AsSpan(vertsIndexOffset, vertCount);
		public Span<Color> Colors => Buffer.Colors.AsSpan(vertsIndexOffset, vertCount);
		public Span<VertexBoneWeights> Weights => Buffer.Weights.AsSpan(vertsIndexOffset, vertCount);
		// public Span<ushort> Indices => MemoryMarshal.CreateSpan(ref Buffer.Faces[0], Buffer.Faces.Length).Slice(facesIndexOffset, indicesCount);
		// public Span<Vector3> Positions => MemoryMarshal.CreateSpan(ref Buffer.Positions[0], Buffer.Positions.Length).Slice(vertsIndexOffset, vertCount);
		// public Span<Vector3> Normals => MemoryMarshal.CreateSpan(ref Buffer.Normals[0], Buffer.Normals.Length).Slice(vertsIndexOffset, vertCount);
		// public Span<Vector3> Tangents => MemoryMarshal.CreateSpan(ref Buffer.Tangents[0], Buffer.Tangents.Length).Slice(vertsIndexOffset, vertCount);
		// public Span<Vector2> UV0 => MemoryMarshal.CreateSpan(ref Buffer.UV0[0], Buffer.UV0.Length).Slice(vertsIndexOffset, vertCount);
		// public Span<Vector2> UV1 => MemoryMarshal.CreateSpan(ref Buffer.UV1[0], Buffer.UV1.Length).Slice(vertsIndexOffset, vertCount);
		// public Span<Color> Colors => MemoryMarshal.CreateSpan(ref Buffer.Colors[0], Buffer.Colors.Length).Slice(vertsIndexOffset, vertCount);
		// public Span<VertexBoneWeights> Weights => MemoryMarshal.CreateSpan(ref Buffer.Weights[0], Buffer.Weights.Length).Slice(vertsIndexOffset, vertCount);

		internal MeshSerializerVersion Version;

        protected override bool DoRead(FileHandler handler)
        {
			handler.Read(ref materialIndex);
			handler.Read(ref ukn);
			handler.Read(ref indicesCount);
			handler.Read(ref facesIndexOffset);
			handler.Read(ref vertsIndexOffset);
			if (Version >= MeshSerializerVersion.RE_RT)
			{
				handler.Read(ref streamingOffset);
				handler.Read(ref streamingOffset2);
			}
			if (Version >= MeshSerializerVersion.DD2)
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
		public int indicesCount;
		public List<Submesh> Submeshes { get; } = new();

		internal MeshSerializerVersion Version;
		internal int meshVertexOffset;

        protected override bool DoRead(FileHandler handler)
        {
			handler.Read(ref groupId);
			handler.Read(ref submeshCount);
			handler.Skip(6);
			handler.Read(ref vertexCount);
			handler.Read(ref indicesCount);
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
			submeshCount = (byte)Submeshes.Count;
            throw new NotImplementedException();
        }
    }

    public class MeshLOD(MeshBuffer Buffer) : BaseModel
    {
		public List<MeshGroup> MeshGroups { get; } = new();

		public float lodFactor;
		public byte vertexFormat;

		public int FaceCount => IndexCount / 3;
		public int IndexCount => MeshGroups.Sum(g => g.indicesCount);
		public int VertexCount => MeshGroups.Sum(g => g.vertexCount);

		internal MeshSerializerVersion Version;

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
				totalIndices += mesh.indicesCount;
				if (mesh.indicesCount % 2 != 0) {
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
		public Sphere boundingSphere;
		public AABB boundingBox;

		internal MeshSerializerVersion Version;

        protected override bool DoRead(FileHandler handler)
        {
			lodCount = handler.Read<byte>();
			materialCount = handler.Read<byte>();
			uvCount = handler.Read<byte>();
			skinWeightCount = handler.Read<byte>();
			handler.Read(ref totalMeshCount);
			if (Version <= MeshSerializerVersion.DMC5)
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
        	lodCount = LODs.Count;
            throw new NotImplementedException();
        }
    }

    public class MeshBone : BaseModel
    {
		public int index;
		public int parentIndex;
		public int nextSibling;
		public int childIndex;
		public int symmetryIndex;

		public int remapIndex = -1;
		public bool IsDeformBone => remapIndex != -1;

		public mat4 localTransform;
		public mat4 globalTransform;
		public mat4 inverseGlobalTransform;

		public string name = "";

		public readonly List<MeshBone> Children = new();

		public MeshBone? Parent;
		public MeshBone? Symmetry;

        protected override bool DoRead(FileHandler handler)
        {
            index = handler.Read<short>();
            parentIndex = handler.Read<short>();
            nextSibling = handler.Read<short>();
            childIndex = handler.Read<short>();
            symmetryIndex = handler.Read<short>();
			handler.Skip(6);
			return true;
        }

        protected override bool DoWrite(FileHandler handler)
        {
            handler.Write((short)index);
            handler.Write((short)parentIndex);
            handler.Write((short)nextSibling);
            handler.Write((short)childIndex);
            handler.Write((short)symmetryIndex);
			handler.Skip(6);
			return true;
        }

        public override string ToString() => name ?? $"Bone {index}";
    }

	public class MeshBoneHierarchy
	{
		public List<MeshBone> Bones { get; } = new();
		public List<MeshBone> DeformBones { get; } = new();
		public List<MeshBone> RootBones { get; } = new();

		private Dictionary<uint, MeshBone>? hashLookup;

		public MeshBone? GetByIndex(int index) => index < Bones.Count ? Bones[index] : null;
		public MeshBone? GetByName(string? name) => Bones.FirstOrDefault(bone => bone.name == name);
		public MeshBone? GetDeformByIndex(int index) => index < DeformBones.Count ? DeformBones[index] : null;
		public MeshBone? GetByHash(uint boneHash)
		{
			hashLookup ??= Bones.ToDictionary(k => MurMur3HashUtils.GetHash(k.name ?? string.Empty));
			return hashLookup.GetValueOrDefault(boneHash);
		}

		public void Clear()
		{
			Bones.Clear();
			DeformBones.Clear();
			RootBones.Clear();
		}
	}
}

namespace ReeLib
{
    using ReeLib.Mesh;

    public class MeshFile : BaseFile
    {
        public const uint Magic = 0x4853454D;
        public const uint MagicMply = 0x594C504D;

		public Header Header { get; } = new();
		public MeshStreamingInfo? StreamingInfo { get; set; }
		public MeshBuffer? MeshBuffer { get; set; }
		public List<MeshData> Meshes { get; } = new();

		public MeshBoneHierarchy? BoneData { get; set; }

		public List<string> MaterialNames { get; } = new();

		private static readonly Dictionary<string, (uint internalVersion, uint fileVersion, MeshSerializerVersion serializerVersion, GameName[] games)> Versions = new()
		{
			{ "RE7", (352921600, 32, MeshSerializerVersion.RE7, [GameName.re7]) }, // currently unsupported

			{ "DMC5", (386270720, 1808282334, MeshSerializerVersion.DMC5, [GameName.dmc5]) },
			{ "RE2", (386270720, 1808312334, MeshSerializerVersion.DMC5, [GameName.re2]) },
			{ "RE3", (386270720, 1902042334, MeshSerializerVersion.DMC5, [GameName.re3]) },

			{ "RE2/3 RT", (21041600, 2109108288, MeshSerializerVersion.RE_RT, [GameName.re2rt, GameName.re3rt]) },
			{ "RE7RT", (21041600, 220128762, MeshSerializerVersion.RE_RT, [GameName.re7rt]) },

			{ "MHRISE", (21041600, 2109148288, MeshSerializerVersion.RE_RT, [GameName.mhrise]) },
			{ "RE8", (2020091500, 2101050001, MeshSerializerVersion.RE8, [GameName.re8]) },

			{ "RE4", (220822879, 221108797, MeshSerializerVersion.RE4, [GameName.re4]) },
			{ "SF6", (220705151, 230110883, MeshSerializerVersion.SF6, [GameName.sf6]) },
			{ "DD2", (230517984, 240423143, MeshSerializerVersion.DD2, [GameName.dd2]) },
			{ "DD2 (old)", (230517984, 231011879, MeshSerializerVersion.DD2_Old, [GameName.dd2]) },

			{ "MHWILDS", (240704828, 241111606, MeshSerializerVersion.MHWILDS, [GameName.mhwilds]) }, // currently unsupported
		};

		public static readonly string[] AllExportConfigs = Versions.OrderBy(kv => kv.Value.serializerVersion).Select(kv => kv.Key).ToArray();

		private static readonly Dictionary<GameName, string[]> versionsPerGame = Enum.GetValues<GameName>().ToDictionary(
			game => game,
			game => Versions.Where(kv => kv.Value.games.Contains(game)).Select(pair => pair.Key).ToArray()
		);

		private static readonly Dictionary<ulong, MeshSerializerVersion> SerializerVersionLookups = Versions.ToDictionary(v => (((ulong)v.Value.internalVersion << 32) | (ulong)v.Value.fileVersion), kv => kv.Value.serializerVersion);

		internal static MeshSerializerVersion GetSerializerVersion(uint internalVersion, uint fileVersion)
			// on match failure, assume latest format for anything unknown - in case of newer games
			=> SerializerVersionLookups.TryGetValue((ulong)internalVersion << 32 | fileVersion, out var vvv) ? vvv : MeshSerializerVersion.MHWILDS;

		public static string[] GetGameMeshVersions(GameName game) => versionsPerGame.GetValueOrDefault(game) ?? AllExportConfigs;
		public static MeshSerializerVersion GetPrimarySerializerVersion(GameName game) => Versions[GetGameMeshVersions(game)[0]].serializerVersion;
		public static MeshSerializerVersion GetSerializerVersion(string exportConfig) => Versions.TryGetValue(exportConfig, out var cfg) ? cfg.serializerVersion : MeshSerializerVersion.Unknown;

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
			BoneData?.Clear();
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
			if (header.bonesOffset > 0) {
				BoneData ??= new();
				handler.Seek(header.bonesOffset);
				var boneCount = handler.Read<int>();
				var boneMapCount = handler.Read<int>();
				handler.Skip(sizeof(long));
				var hierarchyOff = handler.Read<long>();
				var localTransformsOff = handler.Read<long>();
				var globallTransformsOff = handler.Read<long>();
				var invGloballTransformsOff = handler.Read<long>();
				var remapIndices = handler.ReadArray<short>(boneMapCount);

				handler.Seek(hierarchyOff);
				BoneData.Bones.Read(handler, boneCount);
				handler.Seek(localTransformsOff);
				for (int i = 0; i < boneCount; ++i) handler.Read(ref BoneData.Bones[i].localTransform);
				handler.Seek(globallTransformsOff);
				for (int i = 0; i < boneCount; ++i) handler.Read(ref BoneData.Bones[i].globalTransform);
				handler.Seek(invGloballTransformsOff);
				for (int i = 0; i < boneCount; ++i) handler.Read(ref BoneData.Bones[i].inverseGlobalTransform);

				for (int i = 0; i < boneMapCount; ++i) {
					var bone = BoneData.Bones[remapIndices[i]];
					bone.remapIndex = i;
					BoneData.DeformBones.Add(bone);
				}

				SetupBoneHierarchy();
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

			if (header.boneIndicesOffset > 0 && BoneData?.Bones.Count > 0)
			{
				handler.Seek(header.boneIndicesOffset);
				for (int i = 0; i < BoneData.Bones.Count; ++i)
				{
					var idx = handler.Read<short>();
					BoneData.Bones[i].name = strings[idx];
				}
			}

			if (header.boundsOffset > 0)
			{
				handler.Seek(header.boundsOffset);
				// bone bounds
			}

            return true;
        }

		private void SetupBoneHierarchy()
		{
			BoneData!.RootBones.Clear();
			foreach (var bone in BoneData.Bones)
			{
				if (bone.symmetryIndex != bone.index && bone.symmetryIndex != -1)
				{
					bone.Symmetry = BoneData.Bones[bone.symmetryIndex];
				}
				if (bone.parentIndex == -1)
				{
					BoneData.RootBones.Add(bone);
					continue;
				}
				var parent = BoneData.Bones[bone.parentIndex];
				bone.Parent = parent;
				parent.Children.Add(bone);
			}
		}

        protected override bool DoWrite()
        {
            throw new NotImplementedException();
        }
    }
}
