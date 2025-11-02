using ReeLib.via;
using System.Numerics;

namespace ReeLib
{
    public class FolFile : BaseFile
    {
        public int version = 3;
        public AABB aabb;
        public int ukn;
        public List<FoliageInstanceGroup> InstanceGroups { get; } = new();

        private const int Magic = 0x004c4f46;

        public FolFile(FileHandler fileHandler) : base(fileHandler)
        {
        }

        public class FoliageInstanceGroup : BaseModel
        {
            public AABB aabb;
            public int ukn;
            public Transform[]? transforms;
            public string? meshPath;
            public string? materialPath;

            protected override bool DoRead(FileHandler handler)
            {
                var instanceCount = handler.Read<int>();
                handler.Read<Vector3>(ref aabb.minpos);
                handler.Read<Vector3>(ref aabb.maxpos);
                handler.Read(ref ukn);
                transforms = new Transform[instanceCount];
                using (handler.SeekJumpBack(handler.Read<long>())) {
                    handler.ReadArray(transforms);
                }

                meshPath = handler.ReadOffsetWString();
                materialPath = handler.ReadOffsetWString();
                return true;
            }

            protected override bool DoWrite(FileHandler handler)
            {
                handler.Write(transforms?.Length ?? 0);
                handler.Write(ref aabb.minpos);
                handler.Write(ref aabb.maxpos);
                handler.Write(ref ukn);
                handler.WriteOffsetContent(h => h.WriteArray(transforms ?? Array.Empty<Transform>()));
                handler.WriteOffsetContent(h => h.WriteWString(meshPath ?? string.Empty));
                handler.WriteOffsetContent(h => h.WriteWString(materialPath ?? string.Empty));
                handler.OffsetContentTableAddAlign(16);

                return true;
            }

            public override string ToString() => $"mesh: {meshPath} + mat: {materialPath}";
        }

        protected override bool DoRead()
        {
            var handler = FileHandler;
            InstanceGroups.Clear();

            handler.Read<int>();
            handler.Read(ref version);
            var groupCount = handler.Read<int>();
            if (version == 0)
            {
                handler.ReadNull(4);
                handler.Read(ref ukn);
                handler.ReadNull(12);
                for (int i = 0; i < groupCount; ++i)
                {
                    var meshOffset = handler.Read<long>();
                    var meshOffset2 = handler.Read<long>();
                    DataInterpretationException.DebugThrowIf(meshOffset != meshOffset2);
                    var matOffset = handler.Read<long>();
                    handler.ReadNull(8);
                    var group = new FoliageInstanceGroup();
                    group.meshPath = handler.ReadWString(meshOffset);
                    group.materialPath = handler.ReadWString(matOffset);
                    InstanceGroups.Add(group);
                }
            }
            else
            {
                handler.Read<Vector3>(ref aabb.minpos);
                handler.Read<Vector3>(ref aabb.maxpos);
                handler.ReadNull(4);
                var dataOffset = handler.Read<long>();
                handler.Seek(dataOffset);
                InstanceGroups.Read(handler, groupCount);
            }
            return true;
        }

        protected override bool DoWrite()
        {
            var handler = FileHandler;
            handler.Write(Magic);
            handler.Write(ref version);
            handler.Write(InstanceGroups.Count);

            if (version == 0)
            {
                handler.Read(ref ukn);
                handler.ReadNull(12);
                foreach(var group in InstanceGroups)
                {
                    handler.WriteOffsetWString(group.meshPath ??= "");
                    handler.WriteOffsetWString(group.meshPath);
                    handler.WriteOffsetWString(group.materialPath ??= "");
                    handler.WriteNull(8);
                }
            }
            else
            {
                handler.Write(ref aabb.minpos);
                handler.Write(ref aabb.maxpos);
                handler.WriteNull(4);

                var dataOffset = handler.Tell();
                handler.Skip(sizeof(long));
                handler.Align(16);
                handler.Write(dataOffset, handler.Tell());

                InstanceGroups?.Write(handler);
                handler.Align(16);
            }
            handler.OffsetContentTableFlush();
            handler.StringTableFlush();
            return true;
        }
    }
}