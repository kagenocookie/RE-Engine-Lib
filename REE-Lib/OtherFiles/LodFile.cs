namespace ReeLib.Lod
{
    public class LodParameter
    {
        public float occupancyRateMin;
        public float occupancyRateMax;
        public float[] ProjectedAreaRates = [];

        public override string ToString() => $"{occupancyRateMin} - {occupancyRateMax}";
    }

    public class LodSetting : ReadWriteModel
    {
        public LodBits lodBits;
        public uint lodCount;
        public uint hash;
        public LodParameter[] Parameters = new LodParameter[MaxLodLevel];
        public override string ToString() => $"LOD Group {hash}";

        private const int MaxLodLevel = 7;

        protected override bool ReadWrite<THandler>(THandler action)
        {
            var version = action.Version;
            action.Do(ref lodBits);
            action.Do(ref lodCount);
            if (version >= 3) {
                action.Do(ref hash);
            }
            for (int i = 0; i < MaxLodLevel; i++) {
                if (((int)lodBits & (1 << i)) != 0) {
                    var param = Parameters[i] ??= new LodParameter();
                    var valueCount = Math.Max(0, version == 2 ? i + 1 : i - 1);
                    action.Do(ref param.occupancyRateMin);
                    action.Do(ref param.occupancyRateMax);
                    if (param.ProjectedAreaRates.Length != valueCount) {
                        Array.Resize(ref param.ProjectedAreaRates, valueCount);
                    }
                    action.Do(ref param.ProjectedAreaRates);
                }
            }
            return true;
        }
    }

    public enum LodResourceType : int
    {
        Global = 0,
        Local = 1,
        Unknown = 2
    }

    [Flags]
    public enum LodBits : uint
    {
        Lod0 = 1,
        Lod1 = 2,
        Lod2 = 4,
        Lod3 = 8,
        Lod4 = 16,
        Lod5 = 32,
        Lod6 = 64,
    }
}

namespace ReeLib
{
    using ReeLib.Lod;

    public partial class LodFile(FileHandler handler) : BaseFile(handler)
    {
        public LodResourceType Type { get; set; }
        public LodSetting DefaultSettings { get; } = new();
        public List<LodSetting> Settings { get; } = new();

        public const uint Magic = 0x00444F4C;

        protected override bool DoRead()
        {
            var handler = FileHandler;
            var magic = handler.Read<uint>();
            if (magic != Magic) {
                throw new InvalidDataException($"{handler.FilePath}: invalid LOD file");
            }
            Settings.Clear();
            Type = handler.Read<LodResourceType>();
            if (handler.FileVersion >= 3) {
                var settingCount = handler.Read<int>();
                DefaultSettings.Read(handler);
                Settings.Read(handler, settingCount);
            } else {
                DefaultSettings.Read(handler);
            }
            return true;
        }

        protected override bool DoWrite()
        {
            var handler = FileHandler;
            handler.Write(Magic);
            handler.Write(Type);
            if (handler.FileVersion >= 3) {
                handler.Write(Settings.Count);
                DefaultSettings.Write(handler);
                Settings.Write(handler);
            } else {
                DefaultSettings.Write(handler);
            }
            return true;
        }
    }
}