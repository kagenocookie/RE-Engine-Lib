using ReeLib.Common;

namespace ReeLib.Sfur
{
    public class ShellFurMaterial : ReadWriteModel
    {
        public int shellCount = 3;
        public ShellThinType shellThinType;
        public GroomingTexCoordType groomingTexCoordType;
        public float shellHeight = 0.05f;
        public float bendRate = 1;
        public float bendRootRate;
        public float normalTransformRate;
        public float stiffness = 1;
        public float stiffnessDistribution;
        public float springCoefficient;
        public float damping;
        public float gravityForceScale = 1;
        public float directWindForceScale = 1;
        public bool isForceTwoSide;
        public bool isForceAlphaTest;
        public bool isShellBottomSave;
        public bool isDisable;
        public string materialName = "";
        public string? furmaskTexture;

        protected override bool ReadWrite<THandler>(THandler action)
        {
            action.Do(ref shellCount);
            action.Do(ref shellThinType);
            action.Do(ref groomingTexCoordType);
            action.Do(ref shellHeight);
            action.Do(ref bendRate);
            action.Do(ref bendRootRate);
            action.Do(ref normalTransformRate);
            action.Do(ref stiffness);
            action.Do(ref stiffnessDistribution);
            action.Do(ref springCoefficient);
            action.Do(ref damping);
            action.Do(ref gravityForceScale);
            action.Do(ref directWindForceScale);
            action.Do(ref isForceTwoSide);
            action.Do(ref isForceAlphaTest);
            action.Do(ref isShellBottomSave);
            action.Do(ref isDisable);
            if (action.Version == 4) action.Null(8);
            action.HandleOffsetWString(ref materialName);
            action.HandleOffsetWString(ref furmaskTexture, true);
            return true;
        }

        public override string ToString() => materialName;
    }

    public enum ShellThinType
    {
        Ascending = 0,
        Descending = 1,
        Intermediate = 2,
    }

    public enum GroomingTexCoordType
    {
        UvPrimary = 0,
        UvSecondary = 1,
    }
}

namespace ReeLib
{
    using ReeLib.Sfur;

    public class SfurFile(FileHandler fileHandler) : BaseFile(fileHandler)
    {
        public List<ShellFurMaterial> Materials { get; } = new();

        public const uint Magic = 0x52554653;

        protected override bool DoRead()
        {
            Materials.Clear();

            var handler = FileHandler;
            var magic = handler.Read<int>();
            if (magic != Magic)
            {
                throw new InvalidDataException($"{handler.FilePath} Not a SFUR file");
            }

            int version = handler.Read<int>();
            var count = handler.Read<int>();
            handler.ReadNull(4);
            var offsetsOffset = handler.Read<long>();
            handler.Seek(offsetsOffset);
            var offsets = handler.ReadArray<long>(count);
            for (int i = 0; i < count; i++) {
                handler.Seek(offsets[i]);
                var mat = new ShellFurMaterial();
                mat.Read(handler);
                Materials.Add(mat);
            }

            return true;
        }

        protected override bool DoWrite()
        {
            var handler = FileHandler;
            var version = handler.FileVersion;
            handler.Write(Magic);
            handler.Write(version);
            handler.Write(Materials.Count);
            handler.WriteNull(4);
            var offsetsOffset = handler.Tell() + 8;
            handler.Write(offsetsOffset);
            handler.Skip(8 * Materials.Count);
            for (int i = 0; i < Materials.Count; i++) {
                var mat = Materials[i];
                handler.Write(offsetsOffset + i * 8, handler.Tell());
                mat.Write(handler);
            }
            handler.Align(8);
            handler.StringTableFlush();
            return true;
        }
    }
}