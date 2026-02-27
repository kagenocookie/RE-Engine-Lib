using ReeLib.Common;
using ReeLib.Mot;

namespace ReeLib.Motpack
{
    public enum MotpackVersion
    {
        RE9 = 1012,
    }

    public class Header : BaseModel
    {
        public MotpackVersion version;
        public uint magic = MotlistFile.Magic;
        public long pointersOffset;
        public long motionIndicesOffset;
        public int numMots;

        protected override bool DoRead(FileHandler handler)
        {
            handler.Read(ref version);
            handler.Read(ref magic);
            handler.ReadNull(8);
            handler.Read(ref pointersOffset);
            handler.Read(ref motionIndicesOffset);
            handler.Read(ref numMots);
            return true;
        }

        protected override bool DoWrite(FileHandler handler)
        {
            handler.Write(ref version);
            handler.Write(ref magic);
            handler.WriteNull(8);
            handler.Write(ref pointersOffset);
            handler.Write(ref motionIndicesOffset);
            handler.Write(ref numMots);
            return true;
        }
    }

    public class MotpackItem
    {
        public int index;
        public MotFile? motion;

        public MotpackItem()
        {
        }

        public MotpackItem(MotFile motion)
        {
            this.motion = motion;
        }

        public override string ToString() => $"[{index}] {motion}";
    }
}


namespace ReeLib
{
    using System.Diagnostics;
    using ReeLib.Motpack;

    public class MotpackFile(FileHandler fileHandler) : BaseFile(fileHandler)
    {
        public const uint Magic = 0x6B63706D;

        public Header Header { get; } = new();
        public List<MotpackItem> Motions { get; } = new();

        public MotpackItem? Find(string motName)
        {
            foreach (var mot in Motions)
            {
                if (mot.motion?.Name == motName) return mot;
            }
            return null;
        }

        protected override bool DoRead()
        {
            Motions.Clear();
            var handler = FileHandler;
            var header = Header;
            if (!header.Read(handler)) return false;
            if (header.magic != Magic)
            {
                throw new InvalidDataException($"{handler.FilePath} Not a motpack file");
            }
            handler.Seek(header.pointersOffset);
            long[] motOffsets = handler.ReadArray<long>(header.numMots);

            MotFile? headerMot = null;
            for (int i = 0; i < motOffsets.Length; i++)
            {
                DataInterpretationException.DebugThrowIf(motOffsets[0] == 0);

                var fileHandler = handler.WithOffset(motOffsets[i]);
                var magic = fileHandler.ReadInt(4);
                DataInterpretationException.DebugThrowIf(magic != MotFile.Magic);
                if (magic == MotFile.Magic) {
                    MotFile motFile = new(fileHandler);
                    motFile.Embedded = true;
                    motFile.Read();
                    motFile.ReadBones(headerMot);
                    headerMot ??= motFile;
                    Motions.Add(new MotpackItem(motFile));
                }
            }

            handler.Seek(header.motionIndicesOffset);
            for (int i = 0;i < header.numMots; ++i)
            {
                Motions[i].index = handler.Read<int>();
            }

            return true;
        }

        protected override bool DoWrite()
        {
            var handler = FileHandler;
            var header = Header;
            header.Write(handler);
            handler.Align(16);

            header.numMots = Motions.Count;
            header.pointersOffset = handler.Tell();
            handler.Skip(Motions.Count * sizeof(long));

            header.motionIndicesOffset = handler.Tell();
            foreach (var mot in Motions) handler.Write(mot.index);

            var didWriteMot = false;
            for (int i = 0; i < Motions.Count; ++i)
            {
                var mot = Motions[i];
                handler.Align(16);
                var motOffset = handler.Tell();
                handler.Write(header.pointersOffset + i * 8, motOffset);
                mot.motion!.FileHandler = handler.WithOffset(motOffset);
                mot.motion.Write();

                if (!didWriteMot && mot.motion is MotFile)
                {
                    mot.motion.WriteBones();
                    mot.motion.Header.motSize = 0;
                    mot.motion.FileHandler.Write(12, 0);
                    didWriteMot = true;
                }
            }

            return true;
        }
    }
}
