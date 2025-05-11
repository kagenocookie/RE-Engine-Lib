using RszTool.Common;
using RszTool.InternalAttributes;
using System.Collections.ObjectModel;

namespace RszTool.Cdef
{
    public class CdefHeader : BaseModel
    {
        public uint magic;
        public uint layerCount;
        public int maskCount;
        public int attributeCount;
        public int materialCount;
        public int presetCount;
        public ulong padding;

        protected override bool DoRead(FileHandler handler)
        {
            handler.Read(ref magic);
            handler.Read(ref layerCount);
            if (handler.FileVersion >= 3)
            {
                handler.Read(ref maskCount);
                handler.Read(ref attributeCount);
                handler.Read(ref materialCount);
                handler.Read(ref presetCount);
                handler.Skip(8);
            }
            else
            {
                maskCount = handler.Read<short>();
                attributeCount = handler.Read<short>();
                materialCount = handler.Read<short>();
                handler.Skip(2);
            }
            return true;
        }

        protected override bool DoWrite(FileHandler handler)
        {
            handler.Write(ref magic);
            handler.Write(ref layerCount);
            if (handler.FileVersion >= 3)
            {
                handler.Write(ref maskCount);
                handler.Write(ref attributeCount);
                handler.Write(ref materialCount);
                handler.Write(ref presetCount);
                handler.Skip(8);
            }
            else
            {
                handler.Write((short)maskCount);
                handler.Write((short)attributeCount);
                handler.Write((short)materialCount);
                handler.Skip(2);
            }
            return true;
        }
    }

    [RszGenerate, RszAutoReadWrite]
    public partial class LayerDefinition : BaseModel
    {
        public Guid guid;
        [RszOffsetWString] public string? name;
        public uint nameHash;
        public uint ukn1;
        public uint ukn2;
        public uint colorRgba;
        public uint ukn3;
        public uint ukn4;
    }

    [RszGenerate, RszAutoReadWrite]
    public partial class LayerMask : BaseModel
    {
        public Guid guid;
        [RszOffsetWString] public string? name;
        public uint nameHash;
        public int ukn1;
        public int layerId;
        public int maskId;
        public uint padding1;
        public uint padding2;
    }

    [RszGenerate, RszAutoReadWrite]
    public partial class PhysicsMaterial : BaseModel
    {
        public Guid guid;
        [RszOffsetWString] public string? name;
        [RszPaddingAfter(4, "handler.FileVersion >= 7")]
        public uint nameHash;
        [RszPaddingAfter(4, "handler.FileVersion >= 7")]
        public uint colorRgba;
    }

    [RszGenerate, RszAutoReadWrite]
    public partial class PhysicsAttribute : BaseModel
    {
        public Guid guid;
        [RszOffsetWString] public string? name;
        [RszStringHash(nameof(name))] public uint nameHash;
        public uint padding;
    }

    [RszGenerate, RszAutoReadWrite]
    public partial class ColliderPreset : BaseModel
    {
        public Guid guid;
        [RszOffsetWString] public string? name;
        [RszOffsetWString, RszConditional("handler.FileVersion >= 5")]
        public string? description;
        [RszStringHash(nameof(name))]
        public uint nameHash;
        public uint maskBits;
        public uint colorRgba;
        public int ukn1;
        public int ukn2;
        [RszPaddingAfter(8, "handler.FileVersion >= 4")]
        public uint ukn3;
    }
}

namespace RszTool
{
    using RszTool.Cdef;

    public class CdefFile : BaseFile
    {
        public CdefHeader Header = new();
        public LayerDefinition[] Layers = new LayerDefinition[64];
        public ulong[] Bits = new ulong[64];
        public List<LayerMask> Masks { get; } = new();
        public List<PhysicsMaterial> Materials { get; } = new();
        public List<PhysicsAttribute> Attributes { get; } = new();
        public List<ColliderPreset> Presets { get; } = new();

        private int GetLayerCount(int version) => version == 1 ? 32 : 64;

        private const int Magic = 0x46454443;

        public CdefFile(FileHandler fileHandler) : base(fileHandler)
        {
        }

        protected override bool DoRead()
        {
            var handler = FileHandler;
            Header.Read(handler);
            var version = handler.FileVersion;
            var layerCount = GetLayerCount(version);
            if (Layers == null || Layers.Length != layerCount) Layers = new LayerDefinition[layerCount];
            for (int i = 0; i < layerCount; ++i) {
                Layers[i] ??= new();
                Layers[i].Read(handler);
            }
            if (version >= 3)
            {
                handler.ReadArray(Bits);
            }
            else
            {
                Bits = handler.ReadArray<int>(layerCount).Select(n => (ulong)n).ToArray();
            }

            var tagsOffset = handler.ReadInt64();
            var materialsOffset = handler.ReadInt64();
            var attrsOffset = handler.ReadInt64();
            var presetsOffset = version >= 3 ? handler.ReadInt64() : 0;

            handler.Seek(tagsOffset);
            Masks.Read(handler, Header.maskCount);

            handler.Seek(materialsOffset);
            Materials.Read(handler, Header.materialCount);

            handler.Seek(attrsOffset);
            Attributes.Read(handler, Header.attributeCount);

            if (presetsOffset > 0)
            {
                handler.Seek(presetsOffset);
                Presets.Read(handler, Header.presetCount);
            }

            return true;
        }

        protected override bool DoWrite()
        {
            var handler = FileHandler;
            var version = handler.FileVersion;
            Header.magic = Magic;
            Header.maskCount = Masks.Count;
            Header.materialCount = Materials.Count;
            Header.attributeCount = Attributes.Count;
            Header.presetCount = Presets.Count;
            var layerCount = GetLayerCount(version);

            Header.Write(handler);
            for (int i = 0; i < layerCount; ++i) {
                Layers[i] ??= new();
                Layers[i].Write(handler);
            }
            if (version >= 3)
            {
                handler.WriteArray(Bits);
            }
            else
            {
                foreach (var bit in Bits) handler.Write((uint)bit);
            }

            var offsetsStart = handler.Tell();

            handler.Align(16);
            handler.Write(offsetsStart, handler.Tell());
            Masks.Write(handler);

            handler.Write(offsetsStart + sizeof(long), handler.Tell());
            Materials.Write(handler);

            handler.Write(offsetsStart + sizeof(long) * 2, handler.Tell());
            Attributes.Write(handler);

            if (version >= 3)
            {
                handler.Write(offsetsStart + sizeof(long) * 3, handler.Tell());
                Presets.Write(handler);
            }

            return true;
        }
    }
}