using System.Numerics;

namespace ReeLib.SSS
{
    public class SSSProfile : ReadWriteModel
    {
        public Vector3 wideBlur;
        public Vector3 nearStrength;
        public float globalRadius;
        public float nearRadiusRate;
        public float phaseControl;
        public float phaseScatter;

        public float roughness0;
        public float roughness1;
        public float roughnessBlend;
        public Vector3 scatterColor;
        public float scatterDistance;
        public Vector3 transmitColor;
        public float transmitDistance;
        public float transmitIntensity;

        public int toggle;
        public float uknNum;

        protected override bool ReadWrite<THandler>(THandler action)
        {
            var version = action.Version;
            action.Do(ref wideBlur);
            action.Do(ref nearStrength);
            action.Do(ref globalRadius);
            action.Do(ref nearRadiusRate);
            if (version >= 2) {
                action.Do(ref phaseControl);
                action.Do(ref phaseScatter);
            }
            if (version >= 3) {
                action.Do(ref roughness0);
                action.Do(ref roughness1);
                action.Do(ref roughnessBlend);
            }
            if (version >= 4) {
                action.Do(ref scatterColor);
                action.Do(ref scatterDistance);
                action.Do(ref transmitColor);
                action.Do(ref transmitDistance);
                action.Do(ref transmitIntensity);
            }
            if (version >= 6) {
                action.Do(ref toggle);
                action.Do(ref uknNum);
            }
            return true;
        }
    }
}

namespace ReeLib
{
    using ReeLib.Common;
    using ReeLib.SSS;

    public partial class SSSFile(FileHandler handler) : BaseFile(handler)
    {
        public uint version; // seems to be 4 separate u8 version bytes, latest 00 17 16 20

        public List<SSSProfile> Profiles { get; } = new();

        public const uint Magic = 0x00535353;

        protected override bool DoRead()
        {
            var handler = FileHandler;
            if (handler.FileVersion == 5) {
                Log.Warn("SSS.5 isn't fully supported yet. Data might be wrong.");
            }

            var magic = handler.Read<uint>();
            if (magic != Magic) {
                throw new InvalidDataException($"{handler.FilePath}: invalid SSS file");
            }
            handler.Read(ref version);
            var count = handler.Read<int>();
            handler.ReadNull(4);
            Profiles.Clear();
            Profiles.Read(handler, count);
            return true;
        }

        protected override bool DoWrite()
        {
            var handler = FileHandler;
            handler.Write(Magic);
            handler.Write(ref version);
            handler.Write(Profiles.Count);
            handler.WriteNull(4);
            Profiles.Write(handler);
            return true;
        }
    }
}