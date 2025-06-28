using System.Collections.ObjectModel;

namespace ReeLib
{
    public class ResourceInfo : BaseModel
    {
        public ulong pathOffset;
        public string? Path { get; set; }
        public bool HasOffset { get; set; } = true;

        public ResourceInfo()
        {
        }

        public ResourceInfo(GameVersion version, bool isPfb)
        {
            HasOffset = !(version == GameVersion.re7 || isPfb && (version is GameVersion.re2 or GameVersion.dmc5));
        }

        protected override bool DoRead(FileHandler handler)
        {
            if (HasOffset)
            {
                handler.Read(ref pathOffset);
                Path = handler.ReadWString((long)pathOffset);
            }
            else
            {
                Path = handler.ReadWString(-1, -1, false);
            }
            return true;
        }

        protected override bool DoWrite(FileHandler handler)
        {
            if (HasOffset)
            {
                handler.StringTableAdd(Path);
                handler.Write(pathOffset);
            }
            else
            {
                handler.WriteWString(Path ?? "");
            }
            return true;
        }

        public override string ToString() => Path ?? base.ToString()!;
    }


    public class UserdataInfo : BaseModel
    {
        public uint typeId;
        public uint CRC;
        public ulong pathOffset;
        public string? Path { get; set; }

        protected override bool DoRead(FileHandler handler)
        {
            handler.Read(ref typeId);
            handler.Read(ref CRC);
            handler.Read(ref pathOffset);
            Path = handler.ReadWString((long)pathOffset);
            return true;
        }

        protected override bool DoWrite(FileHandler handler)
        {
            handler.Write(ref typeId);
            handler.Write(ref CRC);
            handler.StringTableAdd(Path);
            handler.Write(ref pathOffset);
            return true;
        }

        public override string ToString() => Path ?? CRC.ToString();
    }


    public interface IGameObject
    {
        string? Name { get; }
        RszInstance? Instance { get; }
        ObservableCollection<RszInstance> Components { get; }

        IEnumerable<IGameObject> GetChildren();
    }
}
