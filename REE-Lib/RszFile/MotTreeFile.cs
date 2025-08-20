using System.Numerics;
using System.Runtime.InteropServices;
using ReeLib.Clip;
using ReeLib.Mot;
using ReeLib.MotTree;

namespace ReeLib.MotTree
{
    public enum MotTreeVersion
    {
        RE3 = 5,
        RE_RT = 13,
        DD2 = 14,
    }


    public class MotTreeHeader : ReadWriteModel
    {
        public MotTreeVersion version;
        public uint magic;
        public uint ukn00;
        public uint motSize;
        public long boneHeaderOffsetStart; // BoneBaseDataPointer
        public long boneClipHeaderOffset; // BoneDataPointer

        public long clipFileOffset;
        public long jmapOffset;
        public long Offset2; // Extra Data Offset

        // public long namesOffset;
        public float frameCount;
        public float blending; // Set to 0 to enable repeating
        public float uknFloat1;
        public float uknFloat2;
        public ushort boneCount;
        public ushort boneClipCount;
        public byte clipCount;
        public byte uknPointer3Count;
        public ushort FrameRate;
        public ushort uknPointerCount;
        public ushort uknShort;

        public string motName = string.Empty;
        public string? jmap;

        protected override bool ReadWrite(IFileHandlerAction action)
        {
            action.Then(ref version)
                 ?.Then(ref magic);
            return true;
        }
    }
}


namespace ReeLib
{
    public class MotTreeFile(FileHandler fileHandler)
        : MotFileBase(fileHandler)
    {
        public const uint Magic = 0x6572746D;
        public const string Extension = ".mottree";

        public MotTreeHeader Header { get; } = new();
        public List<Bone> Bones { get; } = new();
        public List<MotClip> MotClips { get; } = new();

        protected override bool DoRead()
        {
            Bones.Clear();
            MotClips.Clear();
            var handler = FileHandler;
            var header = Header;
            if (!header.Read(handler)) return false;
            if (header.magic != Magic) {
                throw new InvalidDataException($"{handler.FilePath} Not a MOTTREE file");
            }

            return true;
        }

        protected override bool DoWrite()
        {
            FileHandler handler = FileHandler;
            handler.Clear();
            var header = Header;
            handler.Seek(Header.Size);
            handler.Align(16);

            Header.Write(handler, 0);
            return true;
        }

        public override string ToString() => $"{Header.motName}";
    }
}
