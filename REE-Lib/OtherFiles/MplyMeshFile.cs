using System.Buffers;
using System.Numerics;
using System.Runtime.CompilerServices;
using System.Text.Json;
using ReeLib.Common;
using ReeLib.via;

namespace ReeLib.MplyMesh
{
    public class MplyHeader : ReadWriteModel
    {
        public uint magic = MplyMeshFile.Magic;
        public uint version;
        public uint fileSize;
        public uint hash;

        public Mesh.ContentFlags flags;
        public ushort stringCount;

        public long uknOffset0;
        public long uknOffset1;
        public long gpuMeshletOffset;
        public long uknOffset2;

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

        protected override sealed bool ReadWrite<THandler>(THandler action)
        {
            action.Do(ref magic);
            action.Do(ref version);
            action.Do(ref fileSize);
            action.Do(ref hash);

            throw new NotSupportedException("MPLY meshes not yet supported");
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

        protected override bool DoRead()
        {
            var handler = FileHandler;
            var header = Header;
            header.Read(handler);
            if (header.magic != Magic)
            {
                throw new Exception("Not a valid MPLY mesh file!");
            }

            throw new NotImplementedException();
        }

        protected override bool DoWrite()
        {
            throw new NotImplementedException();
        }
    }
}