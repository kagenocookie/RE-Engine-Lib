using System.Numerics;
using ReeLib.InternalAttributes;

namespace ReeLib.FbxSkel
{
    [RszGenerate]
    public partial class FbxSkelBone : BaseModel
    {
        [RszOffsetWString] public string name = "";
        [RszStringHash(nameof(name))] public uint nameHash;
        public short parentIndex;
        public short symmetryIndex;
        [RszIgnore] public Quaternion rotation;
        [RszIgnore] public Vector3 position;
        [RszIgnore] public Vector3 scale;
        [RszIgnore] public int segmentScaling;

        protected override bool DoRead(FileHandler handler)
        {
            DefaultRead(handler);
            if (handler.FileVersion >= 5)
            {
                handler.Read(ref rotation);
                handler.Read(ref position);
                handler.Read(ref scale);
                handler.Read(ref segmentScaling);
                handler.ReadNull(4);
            }
            else
            {
                handler.Read(ref position);
                handler.ReadNull(4);
                handler.Read(ref rotation);
                handler.Read(ref scale);
                handler.ReadNull(4);
            }
            return true;
        }

        protected override bool DoWrite(FileHandler handler)
        {
            DefaultWrite(handler);
            if (handler.FileVersion >= 5)
            {
                handler.Write(ref rotation);
                handler.Write(ref position);
                handler.Write(ref scale);
                handler.Write(ref scale);
                handler.WriteNull(4);
            }
            else
            {
                handler.Write(ref position);
                handler.WriteNull(4);
                handler.Write(ref rotation);
                handler.Write(ref scale);
                handler.WriteNull(4);
            }
            return true;
        }
    }
}

namespace ReeLib
{
    using ReeLib.FbxSkel;
    using ReeLib.via;

    public class FbxSkelFile(FileHandler fileHandler) : BaseFile(fileHandler)
    {
        public int version = 8;

        public List<FbxSkelBone> Bones { get; } = new();
        public List<Uint2> BonesLookup { get; } = new();

        public const int Magic = 0x6E6C6B73;

        protected override bool DoRead()
        {
            var handler = FileHandler;
            handler.Read(ref version);
            var magic = handler.Read<int>();
            if (magic != Magic)
            {
                throw new InvalidDataException($"{handler.FilePath} Not a FbxSkel file");
            }
            handler.ReadNull(8);

            var dataOffset = handler.Read<long>();
            var lookupOffset = handler.Read<long>();
            var boneCount = handler.Read<int>();

            handler.Seek(dataOffset);
            Bones.Clear();
            Bones.Read(handler, boneCount);
            handler.Seek(lookupOffset);
            BonesLookup.ReadStructList(handler, boneCount);
            return true;
        }

        protected override bool DoWrite()
        {
            var handler = FileHandler;
            handler.Write(version);
            handler.Write(Magic);
            handler.Skip(8);

            var offsetStart = handler.Tell();
            handler.Skip(16);

            handler.Write(Bones.Count);
            handler.Skip(12);

            handler.Write(offsetStart, handler.Tell());
            Bones.Write(handler);

            BonesLookup.Clear();
            foreach (var b in Bones.OrderBy(b => b.nameHash)) {
                BonesLookup.Add(new Uint2() { x = b.nameHash, y = (uint)Bones.IndexOf(b) });
            }

            handler.Align(16);
            handler.Write(offsetStart + sizeof(long), handler.Tell());
            BonesLookup.Write(handler);

            handler.Align(16);
            handler.StringTableFlush();
            return true;
        }
    }
}