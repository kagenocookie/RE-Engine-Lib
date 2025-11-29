using System.Buffers;
using System.Numerics;
using System.Runtime.CompilerServices;
using ReeLib.Common;
using ReeLib.InternalAttributes;
using ReeLib.MplyMesh;
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

	[Flags]
	public enum ContentFlags : ushort
	{
		None = 0,
		IsSkinning = (1 << 0),
		HasJoint = (1 << 1),
		HasBlendShape = (1 << 2),
		HasVertexGroup = (1 << 3),
		QuadEnable = (1 << 4),
		StreamingBvh = (1 << 5),
		HasTertiaryUV = (1 << 6),
		HasVertexColor = (1 << 7),
		SolvedOffset = (1 << 8),
		BufferCount = (1 << 9) | (1 << 10) | (1 << 11),
		UseRTVertexAnimation = (1 << 12),
		UseSDF = (1 << 13),
		EnableRebraiding = (1 << 14),
		EnableRebraiding2 = (1 << 15),
	}

    public partial class Header : ReadWriteModel
    {
        public uint magic = MeshFile.Magic;
        public uint version;
        public uint fileSize;
        public uint lodHash;

		public ContentFlags flags = 0;
		public short uknCount = 0;
		public short nameCount = 0;
		public short ukn1 = 0;

		public int ukn = 0;
		public long uknOffset = 0;
		public long lodsOffset = 0;
		public long shadowLodsOffset = 0;
		public long occluderMeshOffset = 0;
		public long bonesOffset = 0;
		public long normalRecalcOffset = 0;
		public long blendShapeHeadersOffset = 0;
		public long boundsOffset = 0;
		public long meshOffset = 0;
		public long floatsOffset = 0;
		public long materialIndicesOffset = 0;
		public long boneIndicesOffset = 0;
		public long blendShapeIndicesOffset = 0;
		public long nameOffsetsOffset = 0;

		public int sf6unkn0 = 0;
		public long sf6unkn1 = 0;
		public long streamingInfoOffset = 0;
		public long sf6unkn4 = 0;

		public long dd2HashOffset = 0;
		public long verticesOffset = 0;

		public int wilds_unkn1 = 0;
		public int wilds_unkn2 = 0;
		public int wilds_unkn3 = 0;
		public int wilds_unkn4 = 0;
		public short wilds_unkn5 = 0;

		internal MeshSerializerVersion FormatVersion = MeshSerializerVersion.Unknown;

		public int BufferCount
		{
			get => ((int)flags >> 9) & 0x7;
			set => flags = (flags & ~ContentFlags.BufferCount) | (ContentFlags)((value & 0x7) << 9);
		}

        protected override sealed bool ReadWrite<THandler>(THandler action)
        {
            action.Do(ref magic);
			if (magic != MeshFile.Magic) return false;

            action.Do(ref version);
            action.Do(ref fileSize);
            action.Do(ref lodHash);
			if (action.Handler.FileVersion != 0)
			{
				FormatVersion = MeshFile.GetSerializerVersion(version, (uint)action.Handler.FileVersion);
			}
			else if (FormatVersion == MeshSerializerVersion.Unknown)
			{
				throw new Exception("Unknown mesh file format! Unable to load file");
			}

			if (FormatVersion < MeshSerializerVersion.RE4)
			{
				action.Do(ref flags);
				action.Do(ref nameCount);
				action.Do(ref ukn);
				action.Do(ref lodsOffset);
				action.Do(ref shadowLodsOffset);
				action.Do(ref occluderMeshOffset);
				action.Do(ref bonesOffset);
				action.Do(ref normalRecalcOffset);
				action.Do(ref blendShapeHeadersOffset);
				action.Do(ref boundsOffset);
				action.Do(ref meshOffset);
				action.Do(ref floatsOffset);
				action.Do(ref materialIndicesOffset);
				action.Do(ref boneIndicesOffset);
				action.Do(ref blendShapeIndicesOffset);
				action.Do(ref nameOffsetsOffset);
			}
			else if (FormatVersion >= MeshSerializerVersion.RE4 && FormatVersion < MeshSerializerVersion.MHWILDS)
			{
				action.Do(ref flags);
				action.Do(ref uknCount);
				action.Do(ref nameCount);
				action.Do(ref ukn1);
				action.Do(ref uknOffset);
				action.Do(ref lodsOffset);
				action.Do(ref shadowLodsOffset);
				action.Do(ref occluderMeshOffset);
				action.Do(ref normalRecalcOffset);
				action.Do(ref blendShapeHeadersOffset);
				action.Do(ref meshOffset);

				action.Do(ref sf6unkn1);
				action.Do(ref floatsOffset);

				action.Do(ref boundsOffset);
				action.Do(ref bonesOffset);
				action.Do(ref materialIndicesOffset);
				action.Do(ref boneIndicesOffset);
				action.Do(ref blendShapeIndicesOffset);
				if (FormatVersion < MeshSerializerVersion.DD2_Old)
				{
					action.Do(ref streamingInfoOffset);
					action.Do(ref nameOffsetsOffset);
				}
				else
				{
					action.Do(ref nameOffsetsOffset);
					action.Do(ref dd2HashOffset);
					action.Do(ref streamingInfoOffset);
				}
				action.Do(ref verticesOffset);
				action.Do(ref sf6unkn4);
			}
			else if (FormatVersion >= MeshSerializerVersion.MHWILDS)
			{
				action.Do(ref wilds_unkn1);
				action.Do(ref nameCount);
				action.Do(ref flags);
				action.Do(ref uknCount);
				action.Do(ref wilds_unkn2);
				action.Do(ref wilds_unkn3);
				action.Do(ref wilds_unkn4);
				action.Do(ref wilds_unkn5);
				action.Do(ref verticesOffset);
				action.Do(ref lodsOffset);
				action.Do(ref shadowLodsOffset);
				action.Do(ref occluderMeshOffset);
				action.Do(ref normalRecalcOffset);
				action.Do(ref blendShapeHeadersOffset);
				action.Do(ref meshOffset);
				action.Do(ref sf6unkn1);
				action.Do(ref floatsOffset);
				action.Do(ref boundsOffset);
				action.Do(ref bonesOffset);
				action.Do(ref materialIndicesOffset);
				action.Do(ref boneIndicesOffset);
				action.Do(ref blendShapeIndicesOffset);
				action.Do(ref nameOffsetsOffset);
				action.Do(ref streamingInfoOffset);
				action.Do(ref sf6unkn4);
			}

            return true;
        }
    }

	public struct StreamingMeshEntry
	{
		public uint start;
		public uint end;
	}

    public partial class MeshStreamingInfo : BaseModel
    {
		public StreamingMeshEntry[] Entries = [];

		internal long HeaderOffset => Start + Utils.Align16(Entries.Length * 8);

        protected override bool DoRead(FileHandler handler)
        {
			var entryCount = handler.Read<int>();
			handler.ReadNull(4);
			var entryOffset = handler.Read<long>();
			if (entryCount > 0 && entryOffset > 0)
			{
				var pos = handler.Tell();
				handler.Seek(entryOffset);
				Entries = handler.ReadArray<StreamingMeshEntry>(entryCount);
			}
			return true;
        }

        protected override bool DoWrite(FileHandler handler)
        {
			handler.WriteArray(Entries);
			handler.Align(16);
			handler.Write<int>(Entries.Length);
			handler.WriteNull(4);
			handler.Write(Entries.Length == 0 ? 0L : Start);
            return true;
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

		public int CalculateNextOffset(int count) => offset + size * count;
	}

	public class VertexBoneWeights
	{
		public int[] boneIndices = [];
		public float[] boneWeights = [];
		private static readonly byte[] byteArray8 = new byte[8];

        public VertexBoneWeights()
			: this(MeshSerializerVersion.Unknown)
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

        internal void Write(FileHandler handler, MeshSerializerVersion version)
		{
			if (version == MeshSerializerVersion.SF6)
			{
				var n1 = boneIndices[0] & (boneIndices[1] << 10) & (boneIndices[2] << 20);
				var n2 = boneIndices[3] & (boneIndices[4] << 10) & (boneIndices[5] << 20);
				handler.Write(n1);
				handler.Write(n2);
			}
			else
			{
				for (int i = 0; i < 8; i++) handler.Write((byte)boneIndices[i]);
			}
			for (int i = 0; i < 8; i++) handler.Write((byte)MathF.Round(boneWeights[i] * 255f));
		}
	}

    public class MeshBufferHeaderList
    {
		public MeshBufferItemHeader[] BufferHeaders = [];

        internal void Read(FileHandler handler, int count)
        {
			BufferHeaders = handler.ReadArray<MeshBufferItemHeader>(count);
        }

        internal void Write(FileHandler handler, MeshBuffer buffer)
        {
			handler.WriteArray(BufferHeaders);
        }

		internal void RegenerateHeaders(MeshBuffer buffer)
		{
			// if there's no positions, we're likely not fully loaded (streaming mesh), in which case assume the existing data is valid
			if (buffer.Positions.Length == 0) return;

			var offset = 0;
			var headers = new List<MeshBufferItemHeader>();
			if (buffer.Positions.Length > 0) {
				headers.Add(new MeshBufferItemHeader() { type = VertexBufferType.Position, offset = offset, size = 12 });
				offset = headers.Last().CalculateNextOffset(buffer.Positions.Length);
			}
			if (buffer.Normals.Length > 0) {
				headers.Add(new MeshBufferItemHeader() { type = VertexBufferType.NormalsTangents, offset = offset, size = 8 });
				offset = headers.Last().CalculateNextOffset(buffer.Normals.Length);
			}
			if (buffer.UV0.Length > 0) {
				headers.Add(new MeshBufferItemHeader() { type = VertexBufferType.UV0, offset = offset, size = 4 });
				offset = headers.Last().CalculateNextOffset(buffer.UV0.Length);
			}
			if (buffer.UV1.Length > 0) {
				headers.Add(new MeshBufferItemHeader() { type = VertexBufferType.UV1, offset = offset, size = 4 });
				offset = headers.Last().CalculateNextOffset(buffer.UV1.Length);
			}
			if (buffer.Weights.Length > 0) {
				headers.Add(new MeshBufferItemHeader() { type = VertexBufferType.BoneWeights, offset = offset, size = 16 });
				offset = headers.Last().CalculateNextOffset(buffer.Weights.Length);
			}
			if (buffer.Colors.Length > 0) {
				headers.Add(new MeshBufferItemHeader() { type = VertexBufferType.Colors, offset = offset, size = 4 });
				offset = headers.Last().CalculateNextOffset(buffer.Colors.Length);
			}
			BufferHeaders = headers.ToArray();
		}
    }

    public struct SByte4
    {
        public sbyte X;
        public sbyte Y;
        public sbyte Z;
        public sbyte W;

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public readonly Vector3 DequantizeNormal() => new Vector3(
			((X << 1) + 1) / 255f,
			((Y << 1) + 1) / 255f,
			((Z << 1) + 1) / 255f
		);

        public static SByte4 QuantizeNormal(Vector3 value, sbyte w = 0) => new SByte4() {
            X = QuantizeNormalSingle(value.X),
            Y = QuantizeNormalSingle(value.Y),
            Z = QuantizeNormalSingle(value.Z),
			W = w,
        };

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static sbyte QuantizeNormalSingle(float value) => (sbyte)Math.Round((value * 255 - 1) * 0.5f);

        public override string ToString() => $"{X}, {Y}, {Z}, {W}";
    }

	internal struct NorTanVertexBuffer
	{
#pragma warning disable CS0649
		public SByte4 norm;
		public SByte4 tan;
#pragma warning restore CS0649

		public Vector3 Normal => norm.DequantizeNormal();
		public Vector3 Tangent => tan.DequantizeNormal();
		public sbyte BiTangentSign => tan.W;

		public (Vector3 nor, Vector3 tan, Vector3 bitan) GetAll()
		{
			var norm = Normal;
			var tan = Tangent;
			return (norm, tan, Vector3.Cross(norm, tan) * MathF.Sin(BiTangentSign));
		}
	}

    public class MeshBuffer : BaseModel
    {
		public long elementHeadersOffset;
		public long vertexBufferOffset;
		public long shapekeyWeightBufferOffset;
		public int totalBufferSize;
		public long faceBufferOffset;
		public int faceVertBufferHeaderSize;
		public short elementCount;
		public short totalElementCount;
		public int uknSize1;
		public int uknSize2;
		public int blendShapeOffset;

		public int shapekeyWeightBufferSize;
		public int bufferIndex;
		public int bufferUkn1;
		public int bufferUkn2;

		public MeshBufferHeaderList Headers = new();

		public Vector3[] Positions = [];
		public Vector3[] Normals = [];
		public Vector3[] Tangents = [];
		public sbyte[] BiTangentSigns = [];
		public Vector2[] UV0 = [];
		public Vector2[] UV1 = [];
		public Color[] Colors = [];
		public VertexBoneWeights[] Weights = [];
		public ushort[]? Faces;
		public int[]? IntegerFaces;

		public Vector3[] BlendShapeData = [];
		public VertexBoneWeights[] ShapeKeyWeights = [];

		private const float ByteDenorm = 1f / 127f;
		private const float ByteDenormNegative = 1f / 128f;
		private const float ByteNorm = 127f;
		private const float ByteNormNegative = 128f;

		internal MeshSerializerVersion Version;

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public Vector3 GetBiTangent(int index) => MathF.Sign(BiTangentSigns[index]) * Vector3.Cross(Normals[index], Tangents[index]);

        protected override bool DoRead(FileHandler handler)
        {
			handler.Read(ref elementHeadersOffset);
			handler.Read(ref vertexBufferOffset);
			if (Version >= MeshSerializerVersion.RE4)
			{
				handler.Read(ref shapekeyWeightBufferOffset);
				handler.Read(ref totalBufferSize);
				var vertexBufferSize = handler.Read<int>();
				faceBufferOffset = vertexBufferSize + vertexBufferOffset;
				faceVertBufferHeaderSize = (int)(totalBufferSize - (faceBufferOffset - vertexBufferOffset));
			}
			else
			{
				handler.Read(ref faceBufferOffset);
				if (Version == MeshSerializerVersion.RE_RT) handler.ReadNull(8);
				handler.Read(ref totalBufferSize);
				handler.Read(ref faceVertBufferHeaderSize);
			}
			handler.Read(ref elementCount);
			handler.Read(ref totalElementCount);
			handler.Read(ref uknSize1);
			handler.Read(ref uknSize2);
			handler.Read(ref blendShapeOffset);
			if (Version >= MeshSerializerVersion.RE4)
			{
				handler.Read(ref shapekeyWeightBufferSize);
				handler.Read(ref bufferIndex);
				handler.Read(ref bufferUkn1);
				handler.Read(ref bufferUkn2);
			}

			using var _ = handler.SeekJumpBack(elementHeadersOffset);
			handler.Seek(elementHeadersOffset);
			Headers.Read(handler, elementCount);
			// TODO earlier games can have totalElementCount > elementCount as an alternative to streaming buffers
			return true;
        }

        protected override bool DoWrite(FileHandler handler)
        {
			handler.Write(ref elementHeadersOffset);
			handler.Write(ref vertexBufferOffset);
			if (Version >= MeshSerializerVersion.RE4)
			{
				handler.Write(ref shapekeyWeightBufferOffset);
				handler.Write(ref totalBufferSize);
				handler.Write((int)(faceBufferOffset - vertexBufferOffset));
			}
			else
			{
				handler.Write(ref faceBufferOffset);
				if (Version == MeshSerializerVersion.RE_RT) handler.WriteNull(8);
				handler.Write(ref totalBufferSize);
				faceVertBufferHeaderSize = IntegerFaces != null ? IntegerFaces.Length * 4 : Faces!.Length * 2;
				handler.Write(ref faceVertBufferHeaderSize);
			}
			handler.Write(ref elementCount);
			handler.Write(ref totalElementCount);
			handler.Write(ref uknSize1);
			handler.Write(ref uknSize2);
			handler.Write(ref blendShapeOffset);
			if (Version >= MeshSerializerVersion.RE_RT)
			{
				handler.Write(ref shapekeyWeightBufferSize);
				handler.Write(ref bufferIndex);
				handler.Write(ref bufferUkn1);
				handler.Write(ref bufferUkn2);
			}
            return true;
        }

		public void WriteElementHeaders(FileHandler handler)
		{
			elementHeadersOffset = handler.Tell();
			Headers.RegenerateHeaders(this);
			handler.WriteArray(Headers.BufferHeaders);
		}

		public void ReadBufferData(FileHandler handler, BlendShapeData? blendShapes, bool integerFaces)
		{
			handler.Seek(vertexBufferOffset);
			var BufferHeaders = Headers.BufferHeaders;
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
						BiTangentSigns = new sbyte[count];
						for (int k = 0; k < count; ++k)
						{
							var nortan = handler.Read<NorTanVertexBuffer>();
							Normals[k] = nortan.Normal;
							Tangents[k] = nortan.Tangent;
							BiTangentSigns[k] = nortan.BiTangentSign;
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

			handler.Seek(faceBufferOffset);
			if (integerFaces) {
				IntegerFaces = handler.ReadArray<int>((int)faceVertBufferHeaderSize / 4);
            } else {
				Faces = handler.ReadArray<ushort>((int)faceVertBufferHeaderSize / 2);
            }

			if (shapekeyWeightBufferOffset > 0)
			{
				handler.Seek(shapekeyWeightBufferOffset);
				var count = Weights.Length;
				ShapeKeyWeights = new VertexBoneWeights[count];
				for (int k = 0; k < count; ++k)
				{
					ShapeKeyWeights[k] = new();
					ShapeKeyWeights[k].Read(handler, Version);
				}
			}

			if (blendShapeOffset > 0 && blendShapes != null)
			{
				handler.Seek(vertexBufferOffset + blendShapeOffset);
				BlendShapeData = new Vector3[blendShapes.totalShapeVertexCount];
				var shorts = new short[4];
				var bsPosCount = blendShapes.Shapes.Max(sh => sh.Targets.Max(tt => tt.Submeshes.Max(sss => sss.vertOffset + sss.vertCount)));
				for (int i = 0; i < blendShapes.totalShapeVertexCount; ++i)
				{
					handler.ReadArray(shorts);

					BlendShapeData[i] = new Vector3(
						shorts[0] / ((float)short.MaxValue + Math.Sign(shorts[0] & 0x8000)),
						shorts[1] / ((float)short.MaxValue + Math.Sign(shorts[1] & 0x8000)),
						shorts[2] / ((float)short.MaxValue + Math.Sign(shorts[2] & 0x8000))
					);
				}
			}
		}

		public void WriteBufferData(FileHandler handler)
		{
			var elementHeaders = Headers.BufferHeaders;

			vertexBufferOffset = handler.Tell();
			blendShapeOffset = -(int)vertexBufferOffset;
			totalElementCount = elementCount = (short)elementHeaders.Length;

			for (int i = 0; i < elementHeaders.Length; i++)
			{
                var buffer = elementHeaders[i];
				var bufferStart = vertexBufferOffset + buffer.offset;
				var bufferEnd = (i == elementCount - 1 ? bufferStart + buffer.size * Positions.Length : vertexBufferOffset + elementHeaders[i + 1].offset);
                handler.Seek(bufferStart);
				var count = (int)(bufferEnd - bufferStart) / buffer.size;
				switch (buffer.type) {
					case VertexBufferType.Position:
						handler.WriteArray(Positions);
						break;

					case VertexBufferType.NormalsTangents:
						for (int k = 0; k < count; ++k)
						{
							handler.Write(SByte4.QuantizeNormal(Normals[k]));
							handler.Write(SByte4.QuantizeNormal(Tangents[k], BiTangentSigns[k]));
						}
						break;

					case VertexBufferType.UV0:
						for (int k = 0; k < count; ++k)
						{
							handler.Write((Half)UV0[k].X);
							handler.Write((Half)UV0[k].Y);
						}
						break;
					case VertexBufferType.UV1:
						for (int k = 0; k < count; ++k)
						{
							handler.Write((Half)UV1[k].X);
							handler.Write((Half)UV1[k].Y);
						}
						break;
					case VertexBufferType.Colors:
						handler.WriteArray<Color>(Colors);
						break;
					case VertexBufferType.BoneWeights:
						for (int k = 0; k < count; ++k)
						{
							Weights[k].Write(handler, Version);
						}
						break;
				}
			}

			var vertBufferSize = (int)(handler.Tell() - vertexBufferOffset);

			if (BlendShapeData.Length > 0)
			{
				blendShapeOffset = (int)(handler.Tell() - vertexBufferOffset);
				foreach (var vert in BlendShapeData)
				{
					handler.Write((short)MathF.Round(vert.X * (short.MaxValue + (vert.X < 0 ? 1 : 0))));
					handler.Write((short)MathF.Round(vert.Y * (short.MaxValue + (vert.Y < 0 ? 1 : 0))));
					handler.Write((short)MathF.Round(vert.Z * (short.MaxValue + (vert.Z < 0 ? 1 : 0))));
					handler.WriteNull(2);
				}
				vertBufferSize = (int)(handler.Tell() - vertexBufferOffset);
			}

			handler.Align(16);
			faceBufferOffset = handler.Tell();
			if (IntegerFaces != null) {
				handler.WriteArray(IntegerFaces);
            } else {
				handler.WriteArray(Faces!);
            }

			if (Version >= MeshSerializerVersion.RE4)
			{
				uknSize1 = uknSize2 = vertBufferSize + (int)(handler.Tell() - faceBufferOffset);
			}

			handler.WriteNull(Utils.Align16((int)handler.Tell()) - (int)handler.Tell());

			totalBufferSize = (int)(handler.Tell() - vertexBufferOffset);

			if (ShapeKeyWeights.Length > 0)
			{
				handler.Align(16);
				shapekeyWeightBufferOffset = handler.Tell();
				var count = ShapeKeyWeights.Length;
				for (int k = 0; k < count; ++k)
				{
					ShapeKeyWeights[k].Write(handler, Version);
				}
			}
			// update header with offsets
			this.Write(handler, Start);
		}
    }

    public class Submesh(MeshBuffer buffer) : ReadWriteModel
    {
        public MeshBuffer Buffer { get; set; } = buffer;

		public ushort materialIndex;
		public ushort bufferIndex;
		public int streamingOffset;
		public int streamingOffset2;
		public int ukn1;
		public int ukn2;
		public int indicesCount;
		public int facesIndexOffset;
		public int vertsIndexOffset;
		public int vertCount;

		internal MeshSerializerVersion Version;

		public Span<ushort> Indices => Buffer.Faces.AsSpan(facesIndexOffset, indicesCount);
		public Span<int> IntegerIndices => Buffer.IntegerFaces.AsSpan(facesIndexOffset, indicesCount);
		public Span<Vector3> Positions => Buffer.Positions.AsSpan(vertsIndexOffset, vertCount);
		public Span<Vector3> Normals => Buffer.Normals.AsSpan(vertsIndexOffset, vertCount);
		public Span<Vector3> Tangents => Buffer.Tangents.AsSpan(vertsIndexOffset, vertCount);
		public Span<sbyte> BiTangents => Buffer.BiTangentSigns.AsSpan(vertsIndexOffset, vertCount);
		public Span<Vector2> UV0 => Buffer.UV0.AsSpan(vertsIndexOffset, vertCount);
		public Span<Vector2> UV1 => Buffer.UV1.AsSpan(vertsIndexOffset, vertCount);
		public Span<Color> Colors => Buffer.Colors.AsSpan(vertsIndexOffset, vertCount);
		public Span<VertexBoneWeights> Weights => Buffer.Weights.AsSpan(vertsIndexOffset, vertCount);
		public Span<VertexBoneWeights> ShapeKeyWeights => Buffer.ShapeKeyWeights.AsSpan(vertsIndexOffset, vertCount);

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public Vector3 GetBiTangent(int index) => Buffer.GetBiTangent(vertsIndexOffset + index);

        public readonly ref struct ResolvedBlendShapeSegment(Span<Vector3> Span, int start)
        {
            public Span<Vector3> Span { get; } = Span;
            public int StartIndex { get; } = start;
        }

        public ResolvedBlendShapeSegment GetBlendShapeRange(BlendShapeSubmesh blend)
		{
			// note: unsure if this is the intended way to handle these
			var start = Math.Clamp(blend.vertOffset, vertsIndexOffset, vertsIndexOffset + vertCount) - vertsIndexOffset;
			var end = Math.Clamp(vertsIndexOffset + vertCount, vertsIndexOffset, vertsIndexOffset + vertCount) - vertsIndexOffset;
			if (end == start) return default;

			return new ResolvedBlendShapeSegment(Buffer.BlendShapeData.AsSpan(start, end - start), start);
		}

        protected override bool ReadWrite<THandler>(THandler action)
        {
			action.Do(ref materialIndex);
			action.Do(ref bufferIndex);
			action.Do(Version >= MeshSerializerVersion.MHWILDS, ref ukn1);
			action.Do(ref indicesCount);
			action.Do(ref facesIndexOffset);
			action.Do(ref vertsIndexOffset);
			if (Version >= MeshSerializerVersion.RE_RT)
			{
				action.Do(ref streamingOffset);
				action.Do(ref streamingOffset2);
			}
			action.Do(Version >= MeshSerializerVersion.DD2, ref ukn2);
			return true;
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

		public void ChangeVersion(MeshSerializerVersion version)
		{
			Version = version;
			foreach (var sub in Submeshes) sub.Version = version;
		}

        protected override bool DoRead(FileHandler handler)
        {
			handler.Read(ref groupId);
			handler.Read(ref submeshCount);
			handler.ReadNull(6);
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
					sub.vertCount = vertexCount - (sub.vertsIndexOffset - Submeshes[0].vertsIndexOffset);
				}
				DataInterpretationException.DebugThrowIf(sub.vertCount <= 0);
			}
			return true;
        }

        protected override bool DoWrite(FileHandler handler)
        {
			submeshCount = (byte)Submeshes.Count;
			handler.Write(ref groupId);
			handler.Write(submeshCount);
			handler.WriteNull(6);
			handler.Write(ref vertexCount);
			handler.Write(ref indicesCount);
			Submeshes.Write(handler);

            return true;
        }

        public override string ToString() => $"Mesh Group {groupId} [Submeshes: {Submeshes.Count}]";
    }

    public class MeshLOD(MeshBuffer Buffer) : BaseModel
    {
		public List<MeshGroup> MeshGroups { get; } = new();

		public float lodFactor;
		public byte vertexFormat;
		public byte ukn1;
		// public byte ukn2;

		public int FaceCount => IndexCount / 3;
		public int IndexCount => MeshGroups.Sum(g => g.indicesCount);
		public int VertexCount => MeshGroups.Sum(g => g.vertexCount);

		public int PaddedIndexCount => MeshGroups.Sum(g => g.indicesCount + (g.indicesCount % 2 != 0 ? 1 : 0));

		internal MeshSerializerVersion Version;

        protected override bool DoRead(FileHandler handler)
        {
			var meshCount = handler.Read<byte>();
			handler.Read(ref vertexFormat);
			handler.Read(ref ukn1);
			handler.ReadNull(1);
			handler.Read(ref lodFactor);
			var headerOffset = handler.ReadInt64();
			handler.Seek(headerOffset);
			var meshOffsets = handler.ReadArray<long>(meshCount);

			var vertOffset = 0;
			for (int i = 0; i < meshCount; ++i)
			{
				handler.Seek(meshOffsets[i]);
				var mesh = new MeshGroup(Buffer) { Version = Version, meshVertexOffset = vertOffset };
				mesh.Read(handler);
				MeshGroups.Add(mesh);
				vertOffset += mesh.vertexCount;
			}

			return true;
        }

        protected override bool DoWrite(FileHandler handler)
        {
			handler.Write((byte)MeshGroups.Count);
			handler.Write(ref vertexFormat);
			handler.Write(ref ukn1);
			handler.WriteNull(1);
			handler.Write(ref lodFactor);
			var offsetOffset = handler.Tell();
			handler.Skip(8);
			handler.Align(16);

			var meshOffsetsOffset = handler.Tell();
			handler.Write(offsetOffset, meshOffsetsOffset);
			handler.Skip(8 * MeshGroups.Count);

			handler.Align(16);
			for (int i = 0; i < MeshGroups.Count; ++i)
			{
				handler.Write(meshOffsetsOffset + i * 8, handler.Tell());
				MeshGroups[i].Write(handler);
			}

			return true;
        }
    }

    public class OccluderMesh(MeshBuffer Buffer) : BaseModel
    {
        public int count;
        public float uknFloat; // maybe a max draw distance?

		public List<MeshGroup> Meshes { get; } = new();

		internal MeshSerializerVersion Version;
		internal MeshData? mainMesh;

		public void ChangeVersion(MeshSerializerVersion version)
		{
			Version = version;
			foreach (var mg in Meshes) mg.ChangeVersion(version);
		}

        protected override bool DoRead(FileHandler handler)
        {
			handler.Read(ref count);
			handler.Read(ref uknFloat);

			var offsetStart = handler.Read<long>();
			handler.Seek(offsetStart);
			var offsets = handler.ReadArray<long>(count);

			for (int i = 0; i < count; ++i)
			{
				handler.Seek(offsets[i]);
				var group = new MeshGroup(Buffer) { Version = Version };
				group.Read(handler);
				Meshes.Add(group);
			}
			return true;
        }

        protected override bool DoWrite(FileHandler handler)
        {
        	count = Meshes.Count;

			handler.Write(ref count);
			handler.Write(ref uknFloat);

			var offsetOffset = handler.Tell();
			handler.Skip(8);
			handler.WriteNull(6 * 8);
			var offsetsStart = handler.Tell();
			handler.Write(offsetOffset, offsetsStart);
			handler.Skip(8 * count);

			handler.Align(16);
            for (int i = 0; i < Meshes.Count; i++)
			{
                var group = Meshes[i];
				handler.Write(offsetsStart + i * 8, handler.Tell());
				group.Write(handler);
			}
			return true;
        }
    }

    public class ShadowMesh(MeshBuffer Buffer) : BaseModel
    {
        public int lodCount;
		public int materialCount;
		public int uvCount;
		public int skinWeightCount = 18;
		public int totalMeshCount;

		public List<MeshLOD> LODs { get; } = new();

		internal MeshSerializerVersion Version;
		internal MeshData? mainMesh;

		public void ChangeVersion(MeshSerializerVersion version)
		{
			Version = version;
			if (version == MeshSerializerVersion.SF6) skinWeightCount = 9;
			else if (version <= MeshSerializerVersion.DMC5) skinWeightCount = 1;
			else skinWeightCount = 18;

			foreach (var lod in LODs)
			{
				lod.Version = version;
				foreach (var mg in lod.MeshGroups) mg.ChangeVersion(version);
			}
		}

        protected override bool DoRead(FileHandler handler)
        {
			var shadowLodsOffset = handler.Tell();
			lodCount = handler.Read<byte>();
			materialCount = handler.Read<byte>();
			uvCount = handler.Read<byte>();
			skinWeightCount = handler.Read<byte>();
			totalMeshCount = handler.Read<int>();

			if (Version <= MeshSerializerVersion.DMC5)
			{
				handler.ReadNull(8);
			}

			var offsetStart = handler.Read<long>();
			handler.ReadNull(6 * 8);
			handler.Seek(offsetStart);
			var shadowOffsets = handler.ReadArray<long>(lodCount);
			for (int i = 0; i < lodCount; ++i)
			{
				var offset = shadowOffsets[i];
				// sometimes (usually) the shadow mesh reuses the main mesh lod data
				// reuse the same instance so we can also reserialize with the reused offsets
				if (offset < shadowLodsOffset && mainMesh != null)
				{
					var lodIndex = mainMesh.LODs.FindIndex(lod => lod.Start == offset);
					if (lodIndex != -1)
					{
						LODs.Add(mainMesh.LODs[lodIndex]);
						continue;
					}
					throw new DataInterpretationException("Found backtracked shadow LOD that wasn't a main LOD");
				}

				handler.Seek(offset);
				var lod = new MeshLOD(Buffer) { Version = Version };
				lod.Read(handler);
				LODs.Add(lod);
			}
			return true;
        }

        protected override bool DoWrite(FileHandler handler)
        {
        	lodCount = LODs.Count;
			uvCount = Math.Sign(Buffer.UV0.Length) + Math.Sign(Buffer.UV1.Length);
			totalMeshCount = LODs[0].MeshGroups.Sum(mg => mg.Submeshes.Count);
			// materialCount also includes other (shadow) mesh materials as well, don't recalculate it here

			handler.Write((byte)lodCount);
			handler.Write((byte)materialCount);
			handler.Write((byte)uvCount);
			handler.Write((byte)skinWeightCount);
			handler.Write(ref totalMeshCount);
			if (Version <= MeshSerializerVersion.DMC5)
			{
				handler.WriteNull(8);
			}

			var offsetOffset = handler.Tell();
			handler.Skip(8);
			handler.WriteNull(6 * 8);
			var offsetsStart = handler.Tell();
			handler.Write(offsetOffset, offsetsStart);
			handler.Skip(8 * lodCount);

			handler.Align(16);
            for (int i = 0; i < LODs.Count; i++)
			{
                var lod = LODs[i];
				if (lod.Start != 0)
				{
					handler.Write(offsetsStart + i * 8, lod.Start);
					continue;
				}

				handler.Write(offsetsStart + i * 8, handler.Tell());
				lod.Write(handler);
			}
			return true;
        }
    }

    public class MeshData(MeshBuffer Buffer) : BaseModel
    {
		public List<MeshLOD> LODs { get; } = new();

        public int lodCount;
		public int materialCount;
		public int uvCount;
		public int skinWeightCount = 18;
		public short totalMeshCount;
		public bool integerFaces;
		public Sphere boundingSphere;
		public AABB boundingBox;

		internal MeshSerializerVersion Version;
		private int ExpectedSkinWeightCount => Version switch {
			MeshSerializerVersion.SF6 => 9,
			MeshSerializerVersion.MHWILDS => 25,
			<= MeshSerializerVersion.DMC5 => 1,
			_ => 18,
        };

		public void ChangeVersion(MeshSerializerVersion version)
		{
			Version = version;
			skinWeightCount = ExpectedSkinWeightCount;

			foreach (var lod in LODs)
			{
				lod.Version = version;
				foreach (var mg in lod.MeshGroups) mg.ChangeVersion(version);
			}
		}

		public void RecalculateBoundingBox()
        {
            var bounds = AABB.MaxMin;
			foreach (var pos in Buffer.Positions)
			{
				bounds = bounds.Extend(pos);
			}
			boundingBox = bounds;
			// TODO not exactly correct
        	boundingSphere = new Sphere(bounds.Center, Math.Max(bounds.Size.X, Math.Max(bounds.Size.Y, bounds.Size.Z)) / 2);
        }

        protected override bool DoRead(FileHandler handler)
        {
			lodCount = handler.Read<byte>();
			materialCount = handler.Read<byte>();
			uvCount = handler.Read<byte>();
			skinWeightCount = handler.Read<byte>();
			DataInterpretationException.DebugThrowIf(skinWeightCount != ExpectedSkinWeightCount);
			handler.Read(ref totalMeshCount);
			handler.Read(ref integerFaces);
			handler.ReadNull(1);
			if (Version <= MeshSerializerVersion.DMC5)
			{
				handler.ReadNull(8);
			}

			handler.Read(ref boundingSphere);
			handler.Read(ref boundingBox);

			var lodOffsetsStart = handler.Read<long>();
			handler.Seek(lodOffsetsStart);
			var lodOffsets = handler.ReadArray<long>(lodCount);

			for (int i = 0; i < lodCount; ++i)
			{
				handler.Seek(lodOffsets[i]);
				var lod = new MeshLOD(Buffer) { Version = Version };
				lod.Read(handler);
				LODs.Add(lod);
			}

            return true;
        }

        protected override bool DoWrite(FileHandler handler)
        {
        	lodCount = LODs.Count;
			uvCount = Math.Sign(Buffer.UV0.Length) + Math.Sign(Buffer.UV1.Length);
			totalMeshCount = (short)LODs[0].MeshGroups.Sum(mg => mg.Submeshes.Count);

			handler.Write((byte)lodCount);
			handler.Write((byte)materialCount);
			handler.Write((byte)uvCount);
			handler.Write((byte)skinWeightCount);
			handler.Write(ref totalMeshCount);
			handler.Write(ref integerFaces);
			handler.WriteNull(1);
			if (Version <= MeshSerializerVersion.DMC5)
			{
				handler.WriteNull(8);
			}

			handler.Write(ref boundingSphere);
			handler.Write(ref boundingBox);

			var lodOffsetOffset = handler.Tell();
			handler.Skip(8);
			handler.Align(8);
			var lodOffsetsStart = handler.Tell();
			handler.Write(lodOffsetOffset, lodOffsetsStart);
			var lodOffsets = new long[LODs.Count];
			handler.Skip(LODs.Count * 8);

			handler.Align(16);
            for (int i = 0; i < LODs.Count; i++)
			{
				handler.Write(lodOffsetsStart + i * 8, lodOffsets[i] = handler.Tell());
                LODs[i].Write(handler);
			}

			return true;
        }
    }

	[RszGenerate, RszAutoReadWrite]
	public partial class BlendShapeSubmesh : BaseModel
	{
		public int vertIndex;
		public int vertOffset;
		[RszPaddingAfter(4)]
		public int vertCount;
	}

    public class BlendShapeTarget : BaseModel
    {
		public string name = string.Empty;

		public short blendSSIndex;
		public short blendShapeNum;
		public short ukn;
		public byte ukn2;

        public AABB bounds;

		public List<BlendShapeSubmesh> Submeshes = new();

        protected override bool DoRead(FileHandler handler)
        {
			handler.Read(ref blendSSIndex);
			handler.Read(ref blendShapeNum);
			handler.Read(ref ukn);
			var submeshEntryCount = handler.Read<byte>();
			handler.Read(ref ukn2);
			var offset = handler.Read<long>();
			using var _ = handler.SeekJumpBack(offset);
			Submeshes.Read(handler, submeshEntryCount);
			return true;
        }

        protected override bool DoWrite(FileHandler handler)
        {
			handler.Write(ref blendSSIndex);
			handler.Write(ref blendShapeNum);
			handler.Write(ref ukn);
			handler.Write<byte>((byte)Submeshes.Count);
			handler.WriteNull(1);
			handler.Write(Submeshes[0].Start);
			return true;
        }

        public override string ToString() => name;
    }

    public class BlendShapeInfo : BaseModel
    {
		public ushort flag;
		public ushort ukn;
		public uint ukn2;

		public List<BlendShapeTarget> Targets { get; } = new();
		public List<BlendShapeTarget> BlankTargets { get; } = new();

		public int[] indicesS = [];
		public int[] indicesSS = [];

		public int totalShapeVertexCount;

		public MeshSerializerVersion Version;

        protected override bool DoRead(FileHandler handler)
        {
			var targetCount = handler.Read<ushort>();
			var typing = handler.Read<short>();
			handler.Read(ref flag);
			handler.Read(ref ukn);
			handler.Read(ref ukn2);
			handler.ReadNull(4);

			var dataOffset = handler.Read<long>();
			var boundsOffset = handler.Read<long>();
			var blendSOffset = handler.Read<long>();
			var blendSSOffset = handler.Read<long>();

			handler.Seek(dataOffset);
			Targets.Read(handler, targetCount);
			BlankTargets.Read(handler, typing);

			handler.Seek(boundsOffset);
			for (int i = 0; i < targetCount; ++i) Targets[i].bounds = handler.Read<AABB>();

			handler.Seek(blendSOffset);
			indicesS = handler.ReadArray<int>(targetCount);

			handler.Seek(blendSSOffset);
			indicesSS = handler.ReadArray<int>(targetCount);

			totalShapeVertexCount = 0;
			foreach (var target in Targets) totalShapeVertexCount += target.Submeshes.Sum(ss => ss.vertCount);

			return true;
        }

        protected override bool DoWrite(FileHandler handler)
        {
			var targetCount = Targets.Count;
			var typing = BlankTargets.Count;

			handler.Write((short)targetCount);
			handler.Write((short)typing);
			handler.Write(ref flag);
			handler.Write(ref ukn);
			handler.Write(ref ukn2);
			handler.WriteNull(4);

			var offsets = handler.Tell();
			handler.Skip(32);

			foreach (var t in Targets) t.Submeshes.Write(handler);
			foreach (var t in BlankTargets) t.Submeshes.Write(handler);

			handler.Write(offsets, handler.Tell());
			Targets.Write(handler);
			BlankTargets.Write(handler);

			handler.Write(offsets + 8, handler.Tell());
			for (int i = 0; i < targetCount; ++i) handler.Write<AABB>(Targets[i].bounds);

			handler.Align(16);
			handler.Write(offsets + 16, handler.Tell());
			handler.WriteArray(indicesS);

			handler.Align(16);
			handler.Write(offsets + 24, handler.Tell());
			handler.WriteArray(indicesSS);

			return true;
        }
    }

    public class NormalRecalcData : BaseModel
    {
		public struct NormalIndexRecalc
		{
			public ushort index;
			public byte left;
			public byte right;
		}

		public class RecalcGroup
		{
			public List<NormalIndexRecalc> vertices = new();
			public List<NormalIndexRecalc> faces = new();
		}

		public List<RecalcGroup> Groups { get; } = new();

        internal MeshSerializerVersion Version;

        internal List<MeshLOD> lods = null!;

        protected override bool DoRead(FileHandler handler)
        {
			int count = 1;
			long[] offsets;
			if (Version >= MeshSerializerVersion.DD2_Old)
			{
				// TODO verify correct version
				count = handler.Read<int>();
				handler.ReadNull(4);
				var offset = handler.Read<long>(); // note: might be only 1 group for earlier games
				handler.Seek(offset);
				offsets = handler.ReadArray<long>(count);
			}
			else
			{
				offsets = [handler.Tell()];
			}

			DataInterpretationException.DebugThrowIf(lods.Count < count);
			for (int i = 0; i < count; ++i)
			{
				var group = new RecalcGroup();
				handler.Seek(offsets[i]);
				var vertsOffset = handler.Read<long>();
				var facesOffset = handler.Read<long>();

				handler.Seek(vertsOffset);
				group.vertices.ReadStructList(handler, lods[i].VertexCount);

				handler.Seek(facesOffset);
				group.faces.ReadStructList(handler, lods[i].IndexCount);
				Groups.Add(group);
			}

			return true;
        }

        protected override bool DoWrite(FileHandler handler)
        {
			int count = Groups.Count;
			long offsetStart = handler.Tell();
			if (Version >= MeshSerializerVersion.DD2_Old)
			{
				handler.Write(count);
				handler.WriteNull(4);
				handler.Write(handler.Tell() + 8);
				offsetStart = handler.Tell();
				handler.Skip(count * 8);
			}
			else
			{
				count = 1;
			}

			for (int i = 0; i < count; ++i)
			{
				var group = Groups[i];
				handler.Align(16);

				handler.Write(offsetStart + i * 8, handler.Tell());
				var itemOffsetsStart = handler.Tell();
				handler.Skip(16);

				handler.Write(itemOffsetsStart + 0, handler.Tell());
				group.vertices.Write(handler);

				handler.Align(16);
				handler.Write(itemOffsetsStart + 8, handler.Tell());
				group.faces.Write(handler);
			}

            return true;
        }
    }
    public class BlendShapeData : BaseModel
    {
		public List<BlendShapeInfo> Shapes { get; } = new();

		public int totalShapeVertexCount;

		public MeshSerializerVersion Version;

        protected override bool DoRead(FileHandler handler)
        {
			var count = handler.Read<int>();
			handler.ReadNull(4);
			long offset;
			if (Version < MeshSerializerVersion.DD2_Old)
			{
				offset = handler.Read<long>();
				var offset2 = handler.Read<long>();
				// throw new NotSupportedException("Older blend shapes are currently not supported");
				return false;
			}
			else
			{
				handler.ReadNull(8);
				offset = handler.Read<long>();
			}
			handler.Seek(offset);
			var offsets = handler.ReadArray<long>(count);
			for (int i = 0; i < count; ++i)
			{
				handler.Seek(offsets[i]);
				var shape = new BlendShapeInfo() { Version = Version };
				shape.Read(handler);
				Shapes.Add(shape);
				totalShapeVertexCount += shape.totalShapeVertexCount;
			}

            return true;
        }

        protected override bool DoWrite(FileHandler handler)
        {
			handler.Write(Shapes.Count);
			handler.WriteNull(4);

			if (Version < MeshSerializerVersion.DD2_Old)
			{
				handler.Write<long>(Utils.Align16((int)handler.Tell() + 16));
			}
			else
			{
				handler.WriteNull(8);
				handler.Write<long>(Utils.Align16((int)handler.Tell() + 8));
			}

			handler.Align(16);
			var offsets = handler.Tell();
			handler.Skip(Shapes.Count * 8);
			// handler.Align(16);
			for (int i = 0; i < Shapes.Count; ++i)
			{
				handler.Align(16);
				handler.Write(offsets + i * 8, handler.Tell());
				Shapes[i].Write(handler);
			}

            return true;
        }
    }

    public class MeshBone : BaseModel
    {
		public string name = "";

		public int index;
		public int parentIndex;
		public int nextSibling;
		public int childIndex;
		public int symmetryIndex;
		public bool useSecondaryWeight;

		public int remapIndex = -1;
		public bool IsDeformBone => remapIndex != -1;

		public mat4 localTransform;
		public mat4 globalTransform;
		public mat4 inverseGlobalTransform;
		public AABBVec4 boundingBox;

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
            handler.Read(ref useSecondaryWeight);
			handler.ReadNull(5);
			return true;
        }

        protected override bool DoWrite(FileHandler handler)
        {
            handler.Write((short)index);
            handler.Write((short)parentIndex);
            handler.Write((short)nextSibling);
            handler.Write((short)childIndex);
            handler.Write((short)symmetryIndex);
            handler.Write(ref useSecondaryWeight);
			handler.WriteNull(5);
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
		public List<MeshBuffer>? StreamingBuffers { get; set; }
		public MeshData? MeshData { get; set; }
		public ShadowMesh? ShadowMesh { get; set; }
		public OccluderMesh? OccluderMesh { get; set; }
		public BlendShapeData? BlendShapes { get; set; }
		public NormalRecalcData? NormalRecalcData { get; set; }
		public List<uint>? Hashes { get; set; }
		public List<Vector3>? FloatData { get; set; }

		public bool RequiresStreamingData => Header.BufferCount > 1;

		public MeshBoneHierarchy? BoneData { get; set; }

		public List<string> MaterialNames { get; } = new();

        public string? CurrentVersionConfig => VersionHashLookups.TryGetValue((ulong)Header.version << 32 | (uint)FileHandler.FileVersion, out var config)
			? Versions.FirstOrDefault(kv => kv.Value == config).Key
			: null;

		private readonly record struct MeshVersionConfig(uint internalVersion, uint fileVersion, MeshSerializerVersion serializerVersion, GameName[] games);

        private static readonly Dictionary<string, MeshVersionConfig> Versions = new()
		{
			{ "RE7", new (352921600, 32, MeshSerializerVersion.RE7, [GameName.re7]) }, // currently unsupported

			{ "DMC5", new (386270720, 1808282334, MeshSerializerVersion.DMC5, [GameName.dmc5]) },
			{ "RE2", new (386270720, 1808312334, MeshSerializerVersion.DMC5, [GameName.re2]) },
			{ "RE3", new (386270720, 1902042334, MeshSerializerVersion.DMC5, [GameName.re3]) },

			{ "RE2/3 RT", new (21041600, 2109108288, MeshSerializerVersion.RE_RT, [GameName.re2rt, GameName.re3rt]) },
			{ "RE7RT", new (21041600, 220128762, MeshSerializerVersion.RE_RT, [GameName.re7rt]) },

			{ "MHRISE", new (21041600, 2109148288, MeshSerializerVersion.RE_RT, [GameName.mhrise]) },
			{ "RE8", new (2020091500, 2101050001, MeshSerializerVersion.RE8, [GameName.re8]) },

			{ "RE4", new (220822879, 221108797, MeshSerializerVersion.RE4, [GameName.re4]) },
			{ "SF6", new (220705151, 230110883, MeshSerializerVersion.SF6, [GameName.sf6]) },
			{ "DD2", new (230517984, 240423143, MeshSerializerVersion.DD2, [GameName.dd2]) },
			{ "DD2 old", new (230517984, 231011879, MeshSerializerVersion.DD2_Old, [GameName.dd2]) },

			{ "MHWILDS", new (240704828, 241111606, MeshSerializerVersion.MHWILDS, [GameName.mhwilds]) }, // currently unsupported
		};

		public static readonly string[] AllVersionConfigs = Versions.OrderBy(kv => kv.Value.serializerVersion).Select(kv => kv.Key).ToArray();
		public static readonly string[] AllVersionConfigsWithExtension = Versions.OrderBy(kv => kv.Value.serializerVersion).Select(kv => $"{kv.Key} (.mesh.{kv.Value.fileVersion} / {kv.Value.internalVersion})").ToArray();

		private static readonly Dictionary<GameName, string[]> versionsPerGame = Enum.GetValues<GameName>().ToDictionary(
			game => game,
			game => Versions.Where(kv => kv.Value.games.Contains(game)).Select(pair => pair.Key).ToArray()
		);

		private static readonly Dictionary<ulong, MeshVersionConfig> VersionHashLookups = Versions.ToDictionary(v => (((ulong)v.Value.internalVersion << 32) | (ulong)v.Value.fileVersion), kv => kv.Value);

		internal static MeshSerializerVersion GetSerializerVersion(uint internalVersion, uint fileVersion)
			// on match failure, assume latest format for anything unknown - in case of newer games
			=> VersionHashLookups.TryGetValue((ulong)internalVersion << 32 | fileVersion, out var vvv) ? vvv.serializerVersion : MeshSerializerVersion.MHWILDS;

		public static string[] GetGameVersionConfigs(GameName game) => versionsPerGame.GetValueOrDefault(game) ?? AllVersionConfigs;
		public static MeshSerializerVersion GetPrimarySerializerVersion(GameName game) => Versions[GetGameVersionConfigs(game)[0]].serializerVersion;
		public static MeshSerializerVersion GetSerializerVersion(string exportConfig) => Versions.TryGetValue(exportConfig, out var cfg) ? cfg.serializerVersion : MeshSerializerVersion.Unknown;

        public static uint GetFileExtension(string exportConfig) => Versions.TryGetValue(exportConfig, out var cfg) ? cfg.fileVersion : 0;

        public MeshFile(FileHandler fileHandler) : base(fileHandler)
        {
        }

		public void ChangeVersion(string versionConfig)
		{
			if (!Versions.TryGetValue(versionConfig, out var config)) {
				Log.Error("Unknown mesh version config " + versionConfig);
				return;
			}

			// TODO migrate field differences if applicable
			// - streaming mesh buffer <-> multiple mesh buffer headers
			// - blend shapes
			Header.version = config.internalVersion;
			Header.FormatVersion = config.serializerVersion;
			MeshData?.ChangeVersion(config.serializerVersion);
			ShadowMesh?.ChangeVersion(config.serializerVersion);
			OccluderMesh?.ChangeVersion(config.serializerVersion);
			if (MeshBuffer != null) MeshBuffer.Version = config.serializerVersion;
			if (StreamingBuffers != null)
			{
				foreach (var buf in StreamingBuffers) buf.Version = config.serializerVersion;
			}
			if (BlendShapes != null) BlendShapes.Version = config.serializerVersion;
			FileHandler.FileVersion = (int)config.fileVersion;
		}

		public void LoadStreamingData(FileHandler handler)
		{
			if (!(StreamingBuffers?.Count > 0) || StreamingInfo == null) return;

            for (int i = 0; i < StreamingBuffers.Count; i++)
			{
                var buffer = StreamingBuffers[i];
                buffer.ReadBufferData(handler, BlendShapes, MeshData?.integerFaces ?? false);
            }

			if (MeshData == null) return;
			foreach (var lod in MeshData.LODs)
			{
				foreach (var group in lod.MeshGroups)
				{
					foreach (var sub in group.Submeshes)
					{
						if (sub.bufferIndex > 0)
						{
							sub.Buffer = StreamingBuffers[sub.bufferIndex - 1];
						}
					}
				}
			}
		}

        protected override bool DoRead()
        {
			var handler = FileHandler;
			var header = Header;
			var magic = handler.ReadInt(handler.Tell());
			if (magic == MagicMply)
			{
				throw new NotSupportedException("MPLY meshes not yet supported");
			}
			MaterialNames.Clear();
			BoneData?.Clear();
			MeshBuffer = null;
			StreamingInfo = null;

			header.Read(handler);
			if (header.streamingInfoOffset > 0)
			{
				handler.Seek(header.streamingInfoOffset);
				StreamingInfo = new MeshStreamingInfo();
				StreamingInfo.Read(handler);
				DataInterpretationException.ThrowIf(StreamingInfo.Entries.Length + 1 != header.BufferCount);
			}

			if (header.meshOffset > 0)
			{
				handler.Seek(header.meshOffset);
				MeshBuffer = new() { Version = header.FormatVersion };
				MeshBuffer.Read(handler);

				if (StreamingInfo != null)
				{
					StreamingBuffers = new();
					foreach (var entry in StreamingInfo.Entries)
					{
						var buffer = new MeshBuffer() { Version = header.FormatVersion };
						buffer.Read(handler);
						StreamingBuffers.Add(buffer);
					}
				}

				if (header.lodsOffset > 0)
				{
					handler.Seek(header.lodsOffset);
					MeshData = new MeshData(MeshBuffer) { Version = header.FormatVersion };
					MeshData.Read(handler);
				}

				if (header.shadowLodsOffset > 0)
				{
					handler.Seek(header.shadowLodsOffset);
					ShadowMesh = new ShadowMesh(MeshBuffer) { Version = header.FormatVersion, mainMesh = MeshData };
					ShadowMesh.Read(handler);
				}

				if (header.occluderMeshOffset > 0)
				{
					handler.Seek(header.occluderMeshOffset);
					OccluderMesh = new OccluderMesh(MeshBuffer) { Version = header.FormatVersion, mainMesh = MeshData };
					OccluderMesh.Read(handler);
				}

				if (header.blendShapeHeadersOffset > 0)
				{
					handler.Seek(header.blendShapeHeadersOffset);
					BlendShapes = new BlendShapeData() { Version = header.FormatVersion };
					if (!BlendShapes.Read(handler)) {
						BlendShapes = null;
						Log.Warn("Discarding unsupported mesh blend shapes");
					}
				}

				handler.Seek(header.verticesOffset);
				MeshBuffer.ReadBufferData(handler, BlendShapes, MeshData?.integerFaces ?? false);

				if (MeshBuffer.totalElementCount > MeshBuffer.elementCount)
				{
					// additional buffer seems to be used for the extra meshes in older games
					Log.Warn("Separate shadow/occluder mesh buffer detected, this is currently not supported and will be discarded.");
					ShadowMesh = null;
					OccluderMesh = null;
				}
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
				handler.ReadNull(8);
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

			if (header.materialIndicesOffset > 0)
			{
				handler.Seek(header.materialIndicesOffset);
				var matCount = MeshData?.materialCount ?? ShadowMesh?.materialCount ?? 0;
				for (int i = 0; i < matCount; ++i)
				{
					var idx = handler.Read<short>();
					MaterialNames.Add(strings[idx]);
				}
			}

			if (header.dd2HashOffset > 0)
			{
				Hashes = new();
				handler.Seek(header.dd2HashOffset);
				var hashCount = MeshData?.LODs[0].MeshGroups.Count;
				for (int i = 0; i < hashCount; ++i) Hashes.Add(handler.Read<uint>());
			}

			if (header.floatsOffset > 0)
			{
				FloatData = new();
				handler.Seek(header.floatsOffset);
				var size = handler.Read<int>();
				handler.ReadNull(4);
				var offset = handler.Read<long>();
				handler.Seek(offset);
				FloatData.ReadStructList(handler, size / 12);
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

			if (header.normalRecalcOffset > 0 && MeshData != null)
			{
				handler.Seek(header.normalRecalcOffset);
				NormalRecalcData = new NormalRecalcData() { lods = MeshData.LODs, Version = header.FormatVersion };
				NormalRecalcData.Read(handler);
			}

			if (header.blendShapeIndicesOffset > 0 && BlendShapes != null)
			{
				handler.Seek(header.blendShapeIndicesOffset);
				foreach (var shape in BlendShapes.Shapes)
				{
					foreach (var target in shape.Targets)
					{
						var idx = handler.Read<short>();
						target.name = strings[idx];
					}
				}
			}

			if (header.boundsOffset > 0)
			{
				DataInterpretationException.ThrowIf(BoneData == null);
				handler.Seek(header.boundsOffset);
				var bbCount = handler.Read<int>();
				handler.ReadNull(4);
				handler.Seek(handler.Read<long>());
				if (bbCount > BoneData.DeformBones.Count)
				{
					DataInterpretationException.ThrowIf(bbCount != BoneData.DeformBones.Count + 1);
					BoneData.RootBones.First().boundingBox = handler.Read<AABBVec4>();
					bbCount--;
				}
				for (int i = 0; i < bbCount; ++i)
				{
					BoneData.DeformBones[i].boundingBox = handler.Read<AABBVec4>();
				}
			}

            return true;
        }

        protected override bool DoWrite()
        {
			var handler = FileHandler;
			var header = Header;
			header.Write(handler);

			// force reset lod object start offsets - this will allow us to only write them once and reuse the LODs if multiple meshes reference the same one
			foreach (var mesh in MeshData?.LODs ?? []) mesh.Start = 0;
			foreach (var mesh in ShadowMesh?.LODs ?? []) mesh.Start = 0;

			if (MeshData != null)
			{
				handler.Align(8);
				header.lodsOffset = handler.Tell();
				MeshData.materialCount = MaterialNames.Count;
				MeshData.Write(handler);
			}

			header.shadowLodsOffset = 0;
			if (ShadowMesh != null)
			{
				handler.Align(8);
				header.shadowLodsOffset = handler.Tell();
				ShadowMesh.materialCount = MaterialNames.Count;
				if (MeshData != null) MeshData.skinWeightCount = MeshData.skinWeightCount;
				ShadowMesh.Write(handler);
			}

			header.occluderMeshOffset = 0;
			if (OccluderMesh != null)
			{
				handler.Align(8);
				header.occluderMeshOffset = handler.Tell();
				OccluderMesh.Write(handler);
			}

			if (BoneData != null)
			{
				// handler.Align(8);
				header.bonesOffset = handler.Tell();
				handler.Write(BoneData.Bones.Count);
				handler.Write(BoneData.DeformBones.Count);
				handler.WriteNull(8);
				var offsets = handler.Tell();
				handler.Skip(32);
				foreach (var bone in BoneData.DeformBones) handler.Write((short)bone.index);

				handler.Align(16);
				handler.Write(offsets, handler.Tell()); // hierarchyOff
				BoneData.Bones.Write(handler);

				handler.Write(offsets + 8, handler.Tell()); // localTransformsOff
				foreach (var bone in BoneData.Bones) handler.Write(bone.localTransform);
				handler.Write(offsets + 16, handler.Tell()); // globallTransformsOff
				foreach (var bone in BoneData.Bones) handler.Write(bone.globalTransform);
				handler.Write(offsets + 24, handler.Tell()); // invGloballTransformsOff
				foreach (var bone in BoneData.Bones) handler.Write(bone.inverseGlobalTransform);
			}

			header.normalRecalcOffset = 0;
			if (NormalRecalcData != null)
			{
				handler.Align(16);
				header.normalRecalcOffset = handler.Tell();
				NormalRecalcData.Write(handler);
			}

			header.blendShapeHeadersOffset = 0;
			if (BlendShapes != null)
			{
				handler.Align(8);
				header.blendShapeHeadersOffset = handler.Tell();
				BlendShapes.Write(handler);
			}

			header.floatsOffset = 0;
			if (FloatData != null)
			{
				handler.Align(16);
				header.floatsOffset = handler.Tell();
				handler.Write(FloatData.Count * 12);
				handler.WriteNull(4);
				handler.Write(handler.Tell() + 8);
				FloatData.Write(handler);
			}

			short stringOffset = 0;
			handler.Align(16);
			header.materialIndicesOffset = handler.Tell();
			foreach (var name in MaterialNames) handler.Write(stringOffset++);

			header.boneIndicesOffset = 0;
			if (BoneData != null)
			{
				handler.Align(16);
				header.boneIndicesOffset = handler.Tell();
				foreach (var bone in BoneData.Bones) handler.Write(stringOffset++);
			}

			header.blendShapeIndicesOffset = 0;
			if (BlendShapes != null)
			{
				handler.Align(16);
				header.blendShapeIndicesOffset = handler.Tell();
				foreach (var shape in BlendShapes.Shapes) foreach (var target in shape.Targets) handler.Write(stringOffset++);
			}
			header.nameCount = stringOffset;

			handler.Align(16);
			header.nameOffsetsOffset = handler.Tell();
			foreach (var name in MaterialNames) handler.WriteOffsetAsciiString(name);
			if (BoneData != null)
			{
				foreach (var bone in BoneData.Bones) handler.WriteOffsetAsciiString(bone.name);
			}
			if (BlendShapes != null)
			{
				foreach (var shape in BlendShapes.Shapes)
				{
					foreach (var target in shape.Targets)
					{
						handler.WriteOffsetAsciiString(target.name);
					}
				}
			}
			handler.Align(16);
			handler.AsciiStringTableFlush();

			if (Hashes != null)
			{
				handler.Align(16);
				header.dd2HashOffset = handler.Tell();
				Hashes.Write(handler);
			}
			else
			{
				header.dd2HashOffset = 0;
			}

			if (BoneData?.Bones.Count > 0)
			{
				handler.Align(16);
				header.boundsOffset = handler.Tell();

				var boundCount = BoneData.Bones.Count(b => !b.boundingBox.IsEmpty);
				if (boundCount == 0)
				{
					// TODO recalculate bounds?
					// foreach (var bone in BoneData.DeformBones)
					// {
					// }
				}

				handler.Write(boundCount);
				handler.WriteNull(4);
				var off = handler.Tell();
				handler.Align(16);
				handler.Write(off, handler.Tell());
				// instead of writing every deform bone, write any bone that has non-0 bounds
				// some meshes contain an extra bound (root node?), and this way we retain all existing data
				foreach (var bone in BoneData.Bones)
				{
					if (!bone.boundingBox.IsEmpty)
						handler.Write(bone.boundingBox);
				}
			}

			if (MeshBuffer != null)
			{
				handler.Align(16);
				header.meshOffset = handler.Tell();
				MeshBuffer.Write(handler);
				if (StreamingBuffers != null)
				{
					foreach (var buf in StreamingBuffers) buf.Write(handler);
				}

				if (header.FormatVersion >= MeshSerializerVersion.RE4) StreamingInfo ??= new();

				if (StreamingInfo != null)
				{
					handler.Align(16);
					StreamingInfo.Write(handler);
					header.streamingInfoOffset = StreamingInfo.HeaderOffset;
				}

				MeshBuffer.WriteElementHeaders(handler);
				if (StreamingBuffers != null)
				{
					foreach (var buf in StreamingBuffers) buf.WriteElementHeaders(handler);
				}

				MeshBuffer.WriteBufferData(handler);
				header.verticesOffset = MeshBuffer.vertexBufferOffset;
			}

			header.fileSize = (uint)handler.Tell();
			header.Write(handler, 0);

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
    }
}
