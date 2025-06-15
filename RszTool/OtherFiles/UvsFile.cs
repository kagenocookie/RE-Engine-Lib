using System.Numerics;
using RszTool.InternalAttributes;

namespace RszTool.Uvs
{
    [RszGenerate, RszAutoReadWrite]
    public partial class Header : BaseModel
    {
        public uint magic = UvsFile.Magic;
        public int textureCount;
        public int sequenceCount;
        public int patternCount;
        public int attributes;
        public int unused;
        public long texOffset;
        public long sequenceOffset;
        public long patternOffset;
        public long stringOffset;
    }

    [RszGenerate, RszAutoReadWrite]
    public partial class TextureBlock : BaseModel
    {
        public long stateHolder;
        public long pathStringOffset;
        public long texHandle1;
        public long texHandle2;
        public long texHandle3;

        [RszIgnore] public string path = string.Empty;

        public override string ToString() => path;
    }

    [RszGenerate, RszAutoReadWrite]
    public partial class SequenceBlock : BaseModel
    {
        public int patternCount;
        public int patternTableOffset;

        [RszIgnore] public List<UvsPattern> patterns = new();
        public override string ToString() => $"{patternCount} {patternTableOffset}";
    }

    [RszGenerate, RszAutoReadWrite]
    public partial class UvsPattern : BaseModel
    {
        public long flags;
        public float left;
        public float top;
        public float right;
        public float bottom;
        public int textureIndex;
        public int cutoutUVCount;

        [RszList(nameof(cutoutUVCount))] public readonly List<Vector2> cutoutUVs = new(0);

        public override string ToString() => $"L:{left} T:{top} R:{right} B:{bottom} tex:{textureIndex}";
    }
}

namespace RszTool
{
    using System.Text;
    using RszTool.Uvs;

    public class UvsFile : BaseFile
    {
        public const uint Magic = 0x5556532e;

        public readonly Header Header = new();
        public List<TextureBlock> Textures = new();
        public List<SequenceBlock> Sequences = new();

        public UvsFile(FileHandler fileHandler) : base(fileHandler)
        {
        }

        protected override bool DoRead()
        {
            var handler = FileHandler;
            Header.Read(handler);

            handler.Seek(Header.texOffset);
            Textures.Read(handler, Header.textureCount);

            foreach (var tex in Textures) {
                handler.Seek(Header.stringOffset + tex.pathStringOffset * 2);
                tex.path = handler.ReadWString(-1, -1, false);
            }

            handler.Seek(Header.sequenceOffset);
            Sequences.Read(handler, Header.sequenceCount);

            handler.Seek(Header.patternOffset);
            for (int i = 0; i < Header.sequenceCount; ++i) {
                var seq = Sequences[i];
                seq.patterns = new List<UvsPattern>(seq.patternCount);
                seq.patterns.Read(handler, seq.patternCount);
            }

            return true;
        }

        protected override bool DoWrite()
        {
            var handler = FileHandler;
            Header.sequenceCount = Sequences.Count;
            Header.textureCount = Textures.Count;
            Header.Write(handler);

            Header.texOffset = handler.Tell();
            var strings = new StringBuilder();
            foreach (var tex in Textures) {
                tex.pathStringOffset = strings.Length;
                strings.Append(tex.path).Append("\0\0");
                tex.Write(handler);
            }

            Header.sequenceOffset = handler.Tell();
            Header.patternCount = 0;
            foreach (var seq in Sequences) {
                seq.patternTableOffset = Header.patternCount;
                seq.patternCount = seq.patterns.Count;
                Header.patternCount += seq.patternCount;
                seq.Write(handler);
            }

            Header.patternOffset = handler.Tell();
            foreach (var seq in Sequences) {
                foreach (var pattern in seq.patterns) {
                    pattern.cutoutUVCount = pattern.cutoutUVs.Count == 0 ? -1 : pattern.cutoutUVs.Count;
                    pattern.Write(handler);
                }
            }

            Header.stringOffset = handler.Tell();
            handler.WriteWString(strings.ToString());

            Header.Write(handler, 0);
            return true;
        }
    }
}