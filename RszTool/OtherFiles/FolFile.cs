using RszTool.Common;
using RszTool.via;
using System.Collections.ObjectModel;
using System.Numerics;

namespace RszTool
{
    public class FolFile : BaseFile
    {
        public int version = 3;
        public int groupCount;
        public AABB aabb;
        public int ukn;
        public long dataOffset;
        public List<FoliageInstanceGroup>? InstanceGroups;

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
        }

        protected override bool DoRead()
        {
            var handler = FileHandler;
            handler.Read<int>();
            handler.Read(ref version);
            handler.Read(ref groupCount);
            handler.Read<Vector3>(ref aabb.minpos);
            handler.Read<Vector3>(ref aabb.maxpos);
            handler.Read(ref ukn);
            var dataOffset = handler.Read<long>();
            handler.Seek(dataOffset);
            InstanceGroups = new (groupCount);
            InstanceGroups.Read(handler, groupCount);
            return true;
        }

        protected override bool DoWrite()
        {
            var handler = FileHandler;
            handler.Write(Magic);
            handler.Write(ref version);
            groupCount = InstanceGroups?.Count ?? 0;
            handler.Write(ref groupCount);
            handler.Write(ref aabb.minpos);
            handler.Write(ref aabb.maxpos);
            handler.Write(ref ukn);

            var dataOffset = handler.Tell();
            handler.Skip(sizeof(long));
            handler.Align(16);
            handler.Write(dataOffset, handler.Tell());

            InstanceGroups?.Write(handler);
            handler.Align(16);
            handler.OffsetContentTableFlush();
            handler.StringTableFlush();
            return true;
        }
    }
}