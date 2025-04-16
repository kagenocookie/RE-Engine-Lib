using RszTool.Common;
using System.Collections.ObjectModel;

namespace RszTool.Cdef
{
    public struct CdefHeader
    {
        public uint magic;
        public uint layerCount;
        public int tagCount;
        public int attributeCount;
        public int materialCount;
        public int presetCount;
        public ulong padding;
    }

    public class LayerDefinition : BaseModel
    {
        public Guid guid;
        public uint nameHash;
        public uint ukn1;
        public uint ukn2;
        public uint colorRgba;
        public uint ukn3;
        public uint ukn4;
        public string? name;

        protected override bool DoRead(FileHandler handler)
        {
            handler.Read(ref guid);
            name = handler.ReadOffsetWString();
            handler.ReadRange(ref nameHash, ref ukn4);
            return true;
        }

        protected override bool DoWrite(FileHandler handler)
        {
            handler.Write(ref guid);
            handler.WriteOffsetWString(name ?? string.Empty);
            handler.WriteRange(ref nameHash, ref ukn4);
            return true;
        }
    }

    public class LayerMask : BaseModel
    {
        public Guid guid;
        public uint nameHash;
        public int ukn1;
        public int layerId;
        public int maskId;
        public uint padding1;
        public uint padding2;
        public string? name;

        protected override bool DoRead(FileHandler handler)
        {
            handler.Read(ref guid);
            name = handler.ReadOffsetWString();
            handler.ReadRange(ref nameHash, ref padding2);
            return true;
        }

        protected override bool DoWrite(FileHandler handler)
        {
            handler.Write(ref guid);
            handler.WriteOffsetWString(name ?? string.Empty);
            handler.WriteRange(ref nameHash, ref padding2);
            return true;
        }
    }

    public struct LayerBits
    {
        public uint a;
        public uint b;
    }

    public class PhysicsMaterial : BaseModel
    {
        public Guid guid;
        public uint nameHash;
        public uint padding1;
        public uint colorRgba;
        public uint padding2;
        public string? name;

        protected override bool DoRead(FileHandler handler)
        {
            handler.Read(ref guid);
            name = handler.ReadOffsetWString();
            if (handler.FileVersion >= 7) {
                handler.ReadRange(ref nameHash, ref padding2);
            } else {
                handler.Read(ref nameHash);
                handler.Read(ref padding1);
            }
            return true;
        }

        protected override bool DoWrite(FileHandler handler)
        {
            handler.Write(ref guid);
            handler.WriteOffsetWString(name ?? string.Empty);
            if (handler.FileVersion >= 7) {
                handler.WriteRange(ref nameHash, ref padding2);
            } else {
                handler.Write(ref nameHash);
                handler.Write(ref padding1);
            }
            return true;
        }
    }

    public class PhysicsAttribute : BaseModel
    {
        public Guid guid;
        public uint nameHah;
        public uint padding;
        public string? name;

        protected override bool DoRead(FileHandler handler)
        {
            handler.Read(ref guid);
            name = handler.ReadOffsetWString();
            handler.Read(ref nameHah);
            handler.Read(ref padding);
            return true;
        }

        protected override bool DoWrite(FileHandler handler)
        {
            handler.Write(ref guid);
            handler.WriteOffsetWString(name ?? string.Empty);
            handler.Write(ref nameHah);
            handler.Write(ref padding);
            return true;
        }
    }

    public class ColliderPreset : BaseModel
    {
        public Guid guid;
        public uint nameHash;
        public uint maskBits;
        public uint colorRgba;
        public int ukn1;
        public int ukn2;
        public uint ukn3;
        public uint padding1;
        public uint padding2;
        public string? name;
        public string? description;

        protected override bool DoRead(FileHandler handler)
        {
            handler.Read(ref guid);
            name = handler.ReadOffsetWString();
            if (handler.FileVersion >= 5) {
                description = handler.ReadOffsetWString();
            }
            if (handler.FileVersion >= 4) {
                handler.ReadRange(ref nameHash, ref padding2);
            } else {
                handler.ReadRange(ref nameHash, ref ukn3);
            }
            return true;
        }

        protected override bool DoWrite(FileHandler handler)
        {
            handler.Write(ref guid);
            handler.WriteOffsetWString(name ?? string.Empty);
            if (handler.FileVersion >= 5) {
                handler.WriteOffsetWString(description ?? string.Empty);
            }
            if (handler.FileVersion >= 4) {
                handler.WriteRange(ref nameHash, ref padding2);
            } else {
                handler.WriteRange(ref nameHash, ref ukn3);
            }
            return true;
        }
    }
}

namespace RszTool
{
    using RszTool.Cdef;

    public class CdefFile : BaseFile
    {
        public CdefHeader Header = new();
        public LayerDefinition[] Layers = new LayerDefinition[64];
        public LayerBits[] Bits = new LayerBits[64];
        public List<LayerMask> Masks { get; } = new();
        public List<PhysicsMaterial> Materials { get; } = new();
        public List<PhysicsAttribute> Attributes { get; } = new();
        public List<ColliderPreset> Presets { get; } = new();

        private const int Magic = 0x46454443;

        public CdefFile(FileHandler fileHandler) : base(fileHandler)
        {
        }

        protected override bool DoRead()
        {
            var handler = FileHandler;
            handler.Read(ref Header);
            for (int i = 0; i < 64; ++i) {
                Layers[i] ??= new();
                Layers[i].Read(handler);
            }
            handler.ReadArray(Bits);

            var tagsOffset = handler.ReadInt64();
            var materialsOffset = handler.ReadInt64();
            var attrsOffset = handler.ReadInt64();
            var presetsOffset = handler.ReadInt64();

            handler.Seek(tagsOffset);
            Masks.Read(handler, Header.tagCount);

            handler.Seek(materialsOffset);
            Materials.Read(handler, Header.materialCount);

            handler.Seek(attrsOffset);
            Attributes.Read(handler, Header.attributeCount);

            handler.Seek(presetsOffset);
            Presets.Read(handler, Header.presetCount);

            return true;
        }

        protected override bool DoWrite()
        {
            var handler = FileHandler;
            Header.magic = Magic;
            Header.tagCount = Masks.Count;
            Header.materialCount = Materials.Count;
            Header.attributeCount = Attributes.Count;
            Header.presetCount = Presets.Count;

            handler.Write(Header);
            for (int i = 0; i < 64; ++i) {
                Layers[i] ??= new();
                Layers[i].Write(handler);
            }
            handler.WriteArray(Bits);

            var offsetsStart = handler.Tell();

            handler.Align(16);
            handler.Write(offsetsStart, handler.Tell());
            Masks.Write(handler);

            handler.Write(offsetsStart + sizeof(long), handler.Tell());
            Materials.Write(handler);

            handler.Write(offsetsStart + sizeof(long) * 2, handler.Tell());
            Attributes.Write(handler);

            handler.Write(offsetsStart + sizeof(long) * 3, handler.Tell());
            Presets.Write(handler);

            return true;
        }
    }
}