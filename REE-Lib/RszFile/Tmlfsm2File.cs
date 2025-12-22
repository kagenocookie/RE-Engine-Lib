using ReeLib.Common;
using ReeLib.InternalAttributes;
using ReeLib.Tmlfsm2;

namespace ReeLib.Tmlfsm2
{
    [RszGenerate, RszAutoReadWrite]
    public partial class Header : BaseModel
    {
        public uint magic = Tmlfsm2File.Magic;
        public uint version;
        public int bhvtCount;
        [RszPaddingAfter(8)]
        public int clipCount;

        internal long bhvtOffsetsOffset;
        internal long bhvtInfosOffset;
        internal long clipOffsetsOffset;
        internal long clipInfosOffset;
        internal long uknOffset1;
        internal long uknOffset2;
        internal long asciiStringOffset;
        internal long unicodeStringOffset;
    }

    public class TimelineBhvt : BaseModel
    {
        public Guid guid;
        public string Name { get; set; } = "";

        public BhvtFile? Bhvt { get; set; }

        private int asciiNameOffset;
        private int unicodeNameOffset;

        protected override bool DoRead(FileHandler handler)
        {
            var size = handler.Read<long>();
            handler.Read(ref guid);
            handler.Read(ref asciiNameOffset);
            handler.ReadNull(4);
            handler.Read(ref unicodeNameOffset);
            handler.ReadNull(4);
            return true;
        }

        protected override bool DoWrite(FileHandler handler)
        {
            if (Bhvt == null) throw new NullReferenceException("Missing BHVT");
            handler.Write<long>(Bhvt.Size);
            handler.Write(ref guid);
            asciiNameOffset = handler.AsciiStringTableAdd(Name, false).TableOffset;
            unicodeNameOffset = handler.StringTableAdd(Name, false).TableOffset;
            handler.Write(ref asciiNameOffset);
            handler.WriteNull(4);
            handler.Write(ref unicodeNameOffset);
            handler.WriteNull(4);
            return true;
        }

        public void ReadName(FileHandler handler, long asciiOffset, long unicodeOffset)
        {
            Name = handler.ReadWString(unicodeOffset + unicodeNameOffset * 2);
            var asciiName = handler.ReadAsciiString(asciiOffset + asciiNameOffset);
            DataInterpretationException.DebugThrowIf(Name != asciiName);
        }

        public override string ToString() => $"[{guid}] {Name}";
    }

    public class TimelineClip : BaseModel
    {
        public string Name { get; set; } = "";

        public Guid guid;
        public uint nameHash;

        public ClipFile? Clip { get; set; }

        private int nameOffset;

        protected override bool DoRead(FileHandler handler)
        {
            handler.Read(ref nameHash);
            handler.ReadNull(4);
            handler.Read(ref guid);
            handler.Read(ref nameOffset);
            handler.ReadNull(4);
            return true;
        }

        protected override bool DoWrite(FileHandler handler)
        {
            handler.Write(nameHash = MurMur3HashUtils.GetHash(Name));
            handler.WriteNull(4);
            handler.Write(ref guid);
            handler.Write(nameOffset = handler.StringTableAdd(Name, false).TableOffset);
            handler.WriteNull(4);
            return true;
        }

        public void ReadName(FileHandler handler, long unicodeOffset)
        {
            Name = handler.ReadWString(unicodeOffset + nameOffset * 2);
        }

        public override string ToString() => $"[{guid}] {Name}";
    }
}


namespace ReeLib
{
    public class Tmlfsm2File(RszFileOption option, FileHandler fileHandler) : BaseRszFile(option, fileHandler)
    {
        public const uint Magic = 0x32736674;

        public Header Header { get; } = new();
        public List<TimelineBhvt> BehaviorTrees { get; private set; } = new();
        public List<TimelineClip> Clips { get; private set; } = new();

        protected override bool DoRead()
        {
            var handler = FileHandler;
            if (!Header.Read(handler)) return false;
            var header = Header;
            if (header.magic != Magic)
            {
                throw new InvalidDataException($"{handler.FilePath} Not a Tmlfsm2 file");
            }

            BehaviorTrees.Clear();
            Clips.Clear();

            handler.Seek(header.bhvtOffsetsOffset);
            var bhvtOffets = handler.ReadArray<long>(header.bhvtCount);
            for (int i = 0; i < header.bhvtCount; ++i)
            {
                handler.Seek(bhvtOffets[i]);
                var bhvt = new BhvtFile(Option, handler.WithOffset(handler.Tell()));
                bhvt.Read();
                BehaviorTrees.Add(new TimelineBhvt() { Bhvt = bhvt });
            }

            handler.Seek(Header.bhvtInfosOffset);
            for (int i = 0; i < header.bhvtCount; ++i)
            {
                var tmlbhvt = BehaviorTrees[i];
                tmlbhvt.Read(handler);
                tmlbhvt.ReadName(handler, header.asciiStringOffset, header.unicodeStringOffset);
            }

            handler.Seek(header.clipOffsetsOffset);
            var clipOffets = handler.ReadArray<long>(header.clipCount);
            for (int i = 0; i < header.clipCount; ++i)
            {
                handler.Seek(clipOffets[i]);
                var clip = new ClipFile(handler);
                clip.ReadNoSeek();
                Clips.Add(new TimelineClip() { Clip = clip });
            }

            handler.Seek(Header.clipInfosOffset);
            for (int i = 0; i < header.clipCount; ++i)
            {
                var tmlclip = Clips[i];
                tmlclip.Read(handler);
                tmlclip.ReadName(handler, header.unicodeStringOffset);
            }

            DataInterpretationException.DebugThrowIf(header.uknOffset1 != header.uknOffset2);
            DataInterpretationException.DebugThrowIf(header.uknOffset2 != header.asciiStringOffset);

            return true;
        }

        protected override bool DoWrite()
        {
            var handler = FileHandler;
            var header = Header;

            header.Write(handler);
            header.bhvtOffsetsOffset = handler.Tell();
            handler.Skip(BehaviorTrees.Count * sizeof(long));
            for (int i = 0; i < BehaviorTrees.Count; ++i)
            {
                handler.Align(8);
                handler.Write(header.bhvtOffsetsOffset + i * sizeof(long), handler.Tell());
                var bhvt = BehaviorTrees[i].Bhvt!;
                bhvt.FileHandler = handler.WithOffset(handler.Tell());
                bhvt.Write();
            }

            handler.Align(8);
            header.bhvtInfosOffset = handler.Tell();
            for (int i = 0; i < BehaviorTrees.Count; ++i)
            {
                var bhvt = BehaviorTrees[i];
                bhvt.Write(handler);
            }

            header.clipOffsetsOffset = handler.Tell();
            handler.Skip(Clips.Count * sizeof(long));
            for (int i = 0; i < Clips.Count; ++i)
            {
                handler.Align(8);
                handler.Write(header.clipOffsetsOffset + i * sizeof(long), handler.Tell());
                var clip = Clips[i].Clip!;
                clip.FileHandler = handler.WithOffset(0); // create handler copy because the clip has its own string table but inherited base offset
                clip.WriteNoSeek();
            }

            handler.Align(8);
            header.clipInfosOffset = handler.Tell();
            for (int i = 0; i < Clips.Count; ++i)
            {
                var clipInfo = Clips[i];
                clipInfo.Write(handler);
            }

            header.uknOffset1 = handler.Tell();
            header.uknOffset2 = handler.Tell();

            header.asciiStringOffset = handler.Tell();
            handler.AsciiStringTableFlush();

            handler.Align(16);
            header.unicodeStringOffset = handler.Tell();
            handler.StringTableFlush();

            header.Write(handler, 0);
            return true;
        }
    }
}
