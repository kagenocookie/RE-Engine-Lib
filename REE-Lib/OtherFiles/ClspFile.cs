
namespace ReeLib.Clsp
{
    public class CollisionPreset : BaseModel
    {
        public CollisionShapeType shapeType;
        public uint hash1;
        public uint hash2;
        public int flags;
        public int flags2;
        public object? shape;

        protected override bool DoRead(FileHandler handler)
        {
            handler.Read(ref shapeType);
            handler.Read(ref hash1);
            handler.Read(ref hash2);
            handler.ReadNull(4);
            handler.Read(ref flags);
            handler.Read(ref flags2);
            handler.ReadNull(8);
            var pos = handler.Tell();
            shape = shapeType switch
            {
                CollisionShapeType.Sphere => handler.Read<via.Sphere>(),
                CollisionShapeType.Capsule => handler.Read<via.Capsule>(),
                CollisionShapeType.Box => handler.Read<via.OBB>(),
                CollisionShapeType.TaperedCapsule => handler.Read<via.TaperedCapsule>(),
                CollisionShapeType.Plane => handler.Read<via.Plane>(),
                _ => throw new Exception("Unsupported CLSP shape type " + shapeType),
            };
            handler.ReadNull((int)(pos - handler.Tell()) + 80);
            return true;
        }

        protected override bool DoWrite(FileHandler handler)
        {
            handler.Write(ref shapeType);
            handler.Write(ref hash1);
            handler.Write(ref hash2);
            handler.WriteNull(4);
            handler.Write(ref flags);
            handler.Write(ref flags2);
            handler.WriteNull(8);
            var pos = handler.Tell();
            switch (shapeType)
            {
                case CollisionShapeType.Sphere: handler.Write((via.Sphere)shape!); break;
                case CollisionShapeType.Capsule: handler.Write((via.Capsule)shape!); break;
                case CollisionShapeType.Box: handler.Write((via.OBB)shape!); break;
                case CollisionShapeType.TaperedCapsule: handler.Write((via.TaperedCapsule)shape!); break;
                case CollisionShapeType.Plane: handler.Write((via.Plane)shape!); break;
                default: throw new Exception("Unsupported CLSP shape type " + shapeType);
            };
            handler.WritePaddingUntil(pos + 80);
            return true;
        }

        public override string ToString() => $"[{hash1}]  {shape}";
    }

    public enum CollisionShapeType : uint
    {
        Sphere = 0,
        Capsule = 1,
        Box = 2,
        TaperedCapsule = 3,
        Plane = 4,
        Max = 5,
        None = 6,
    }
}


namespace ReeLib
{
    using ReeLib.Clsp;

    public class ClspFile(FileHandler fileHandler) : BaseFile(fileHandler)
    {
        public List<CollisionPreset> Presets = [];

        public const int Magic = 0x50534C43;

        protected override bool DoRead()
        {
            var handler = FileHandler;
            var magic = handler.Read<int>();
            if (magic != Magic)
            {
                throw new Exception("Invalid CLSP file");
            }
            var count = handler.Read<int>();
            var dataOffset = handler.Read<long>();
            handler.Seek(dataOffset);
            Presets.Read(handler, count);
            return true;
        }

        protected override bool DoWrite()
        {
            var handler = FileHandler;
            handler.Write(Magic);
            handler.Write(Presets.Count);
            handler.Write(handler.Tell() + 8);
            Presets.Write(handler);
            return true;
        }
    }
}