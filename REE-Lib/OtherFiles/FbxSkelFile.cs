using System.Numerics;

namespace ReeLib.FbxSkel
{
    public class RefBone : ReadWriteModel
    {
        public string name = "";
        public uint nameHash;
        public short parentIndex;
        public short symmetryIndex;
        public Quaternion rotation;
        public Vector3 position;
        public Vector3 scale;
        public bool segmentScaling;

        public override string ToString() => name;

        protected override bool ReadWrite<THandler>(THandler action)
        {
            action.HandleOffsetWString(ref name);
            action.Do(ref nameHash);
            action.Do(ref parentIndex);
            action.Do(ref symmetryIndex);
            if (action.Handler.FileVersion >= 5)
            {
                action.Do(ref rotation);
                action.Do(ref position);
                action.Do(ref scale);
                action.Do(ref segmentScaling);
                action.Null(7);
            }
            else
            {
                action.Do(ref position);
                action.Null(4);
                action.Do(ref rotation);
                action.Do(ref scale);
                action.Null(4);
            }
            return true;
        }
    }
}

namespace ReeLib
{
    using ReeLib.Common;
    using ReeLib.FbxSkel;
    using ReeLib.via;

    public class FbxSkelFile(FileHandler fileHandler) : BaseFile(fileHandler)
    {
        public int version = 8;

        public List<RefBone> Bones { get; } = new();
        public List<Uint2> BonesLookup { get; } = new();

        public const int Magic = 0x6E6C6B73;

        public RefBone? FindBoneByHash(uint boneHash)
        {
            int a = 0;
            int b = BonesLookup.Count - 1;
            while (a <= b) {
                var m = a + ((b - a) / 2);
                if (BonesLookup[m].x < boneHash) {
                    a = m + 1;
                } else if (BonesLookup[m].x > boneHash) {
                    b = m - 1;
                } else {
                    return Bones[(int)BonesLookup[m].y];
                }
            }
            return null;
        }

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
            foreach (var bone in Bones) bone.nameHash = MurMur3HashUtils.GetHash(bone.name);

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

            // ensure we update the lookup in case any changes were made
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