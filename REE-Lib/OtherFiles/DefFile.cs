using ReeLib.InternalAttributes;

namespace ReeLib.Def
{
    [RszGenerate, RszAutoReadWrite]
    public partial class DefHeader : BaseModel
    {
        public uint magic = DefFile.Magic;
        public int layerCount;
        public int channelCount;
        public int materialCount;
        [RszConditional("handler.FileVersion", ">=", 6, EndAt = nameof(ukn4))]
        public int worldsCount;
        public int ukn2;
        public int ukn3;
        public int ukn4;
    }

    [RszGenerate, RszAutoReadWrite]
    public partial class LayerDefinition : BaseModel
    {
        public Guid guid;
        [RszOffsetWString] public string? name;
        public uint nameHash;
        public via.Color color;
        public uint ukn1;
        public uint ukn2;
        public uint ukn3;
        public uint ukn4;
        [RszIgnore] public uint bits;
    }

    [RszGenerate, RszAutoReadWrite]
    public partial class RigidbodyChannel : BaseModel
    {
        public Guid guid;
        [RszOffsetWString] public string? name;
        public uint nameHash;
        public short ukn1;
        public short ukn2;
    }

    [RszGenerate, RszAutoReadWrite]
    public partial class RigidbodyMaterial : BaseModel
    {
        public Guid guid;
        [RszOffsetWString] public string? name;
        public uint nameHash;
        public int ukn;
        public float uknFlt1;
        public float uknFlt2;
        public float uknFlt3;
        public UndeterminedFieldType ukn2;
    }

    [RszGenerate, RszAutoReadWrite]
    public partial class DynamicsWorld : BaseModel
    {
        public Guid guid;
        [RszOffsetWString] public string? name;
        public uint nameHash;
        public int padding;
        public float gravity;
        public UndeterminedFieldType ukn2;
        public UndeterminedFieldType ukn3;
        public UndeterminedFieldType ukn4;
    }
}

namespace ReeLib
{
    using ReeLib.Def;

    public class DefFile : BaseFile
    {
        public readonly DefHeader Header = new();
        public LayerDefinition[] Layers = new LayerDefinition[LayerCount];
        public List<RigidbodyChannel> Channels { get; } = new();
        public List<RigidbodyMaterial> Materials { get; } = new();
        public List<DynamicsWorld> Worlds { get; } = new();

        private const int LayerCount = 32;

        public const int Magic = 0x20464544;

        public DefFile(FileHandler fileHandler) : base(fileHandler)
        {
        }

        protected override bool DoRead()
        {
            var handler = FileHandler;
            Header.Read(handler);
            var version = handler.FileVersion;
            if (Layers == null || Layers.Length != LayerCount) Layers = new LayerDefinition[LayerCount];
            for (int i = 0; i < LayerCount; ++i) {
                Layers[i] ??= new();
                Layers[i].Read(handler);
            }
            for (int i = 0; i < LayerCount; ++i) {
                Layers[i].bits = handler.Read<uint>();
            }

            var channelsOffset = handler.ReadInt64();
            var materialsOffset = handler.ReadInt64();
            var worldsOffset = handler.FileVersion >= 6 ? handler.ReadInt64() : 0;
            var uknOffset2Padding = handler.FileVersion >= 6 ? handler.ReadInt64() : 0;

            handler.Seek(channelsOffset);
            Channels.Read(handler, Header.channelCount);

            handler.Seek(materialsOffset);
            Materials.Read(handler, Header.materialCount);

            if (worldsOffset != 0 && Header.worldsCount > 0) {
                handler.Seek(worldsOffset);
                Worlds.Read(handler, Header.worldsCount);
            }

            if (Header.ukn2 != 0 || Header.ukn3 != 0 || Header.ukn4 != 0) throw new Exception("Unhandled count in DEF file");

            return true;
        }

        protected override bool DoWrite()
        {
            var handler = FileHandler;
            var version = handler.FileVersion;
            Header.magic = Magic;
            Header.channelCount = Channels.Count;
            Header.materialCount = Materials.Count;

            Header.Write(handler);
            for (int i = 0; i < LayerCount; ++i) {
                Layers[i] ??= new();
                Layers[i].Write(handler);
            }
            for (int i = 0; i < LayerCount; ++i) {
                handler.Write(Layers[i].bits);
            }

            var offsetsStart = handler.Tell();

            handler.Align(16);
            handler.Write(offsetsStart, handler.Tell());
            Channels.Write(handler);

            handler.Write(offsetsStart + sizeof(long), handler.Tell());
            Materials.Write(handler);

            if (version >= 6)
            {
                handler.Write(offsetsStart + sizeof(long) * 2, handler.Tell());
                handler.Skip(8);
                handler.Skip(8);
            }

            return true;
        }
    }
}