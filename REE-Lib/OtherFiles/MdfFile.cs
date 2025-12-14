using System.Numerics;
using ReeLib.Common;
using ReeLib.Mdf;


namespace ReeLib.Mdf
{
    public class MaterialHeader : BaseModel
    {
        public long matNameOffset; // 1
        public string matName = string.Empty;
        public uint matNameHash; // 2
        // tdbVersion == 49, RE7
        public ulong uknRE7;
        public ulong pragmataUkn;

        public int paramsSize; // 3
        public int paramCount; // 4
        public int texCount; // 5
        // tdbVersion >= 69, RE8+
        public int gpbfNameCount;
        public int gpbfDataCount;
        public ShadingType shaderType; // 6
        // tdbVersion >= 71, SF6+
        public uint ukn;
        public uint alphaFlags; // 7
        // tdbVersion >= 71, SF6+ {
        public ulong ukn1;
        // }
        public long paramHeaderOffset; // 8
        public long texHeaderOffset; // 9
        // tdbVersion >= 69, RE8+
        public long gpbfOffset;
        public long paramsOffset; // 10
        public long mmtrPathOffset; // 11
        public string? mmtrPath;
        // tdbVersion >= 71, SF6+
        public long texIDsOffset;


        public MaterialFlags1 Flags1
        {
            get => (MaterialFlags1)(alphaFlags & 0x03ff);
            set => alphaFlags = (uint)(((int)value & 0x03ff) + (alphaFlags & ~0x03ff));
        }

        public int Tesselation
        {
            get => (int)(((alphaFlags & 0xfc00) >> 10) & 0x3f);
            set => alphaFlags = (uint)(((value & 0x3f) << 10) + (alphaFlags & ~0xfc00));
        }

        public int Phong
        {
            get => (int)(((alphaFlags & 0xff0000) >> 16) & 0xff);
            set => alphaFlags = (uint)(((value & 0xff) << 16) + (alphaFlags & ~0xff0000));
        }

        public MaterialFlags2 Flags2
        {
            get => (MaterialFlags2)(((alphaFlags & 0xff000000) >> 24) & 0xff);
            set => alphaFlags = (uint)((((int)value & 0xff) << 24) + (alphaFlags & ~0xff000000));
        }


        protected override bool DoRead(FileHandler handler)
        {
            var Version = handler.FileVersion;
            long pos = handler.Tell();
            handler.Read(ref matNameOffset);
            matName = handler.ReadWString(matNameOffset);
            handler.Read(ref matNameHash);
            if (Version == 6) handler.Read(ref uknRE7);
            handler.Read(ref paramsSize);
            handler.Read(ref paramCount);
            handler.Read(ref texCount);
            if (Version >= 19)
            {
                handler.Read(ref gpbfNameCount);
                handler.Read(ref gpbfDataCount);
                if (gpbfNameCount != gpbfDataCount)
                {
                    throw new Exception("GPBF Count mismatch!");
                }
            }
            handler.Read(ref shaderType);
            if (Version >= 32) handler.Read(ref ukn);
            handler.Read(ref alphaFlags);
            if (Version >= 32) handler.Read(ref ukn1);
            if (Version >= 51) handler.Read(ref pragmataUkn);
            handler.Read(ref paramHeaderOffset);
            handler.Read(ref texHeaderOffset);
            if (Version >= 19)
            {
                handler.Read(ref gpbfOffset);
            }
            handler.Read(ref paramsOffset);
            handler.Read(ref mmtrPathOffset);
            mmtrPath = handler.ReadWString(mmtrPathOffset);
            if (Version >= 32) handler.Read(ref texIDsOffset);
            return true;
        }

        protected override bool DoWrite(FileHandler handler)
        {
            var Version = handler.FileVersion;
            if (matName != null)
            {
                handler.StringTableAdd(matName);
                matNameHash = MurMur3HashUtils.GetHash(matName);
            }
            handler.Write(ref matNameOffset);
            handler.Write(ref matNameHash);
            if (Version == 6) handler.Write(ref uknRE7);
            handler.Write(ref paramsSize);
            handler.Write(ref paramCount);
            handler.Write(ref texCount);
            if (Version >= 19) {
                handler.Write(ref gpbfNameCount);
                handler.Write(ref gpbfDataCount);
            }
            handler.Write(ref shaderType);
            if (Version >= 32) handler.Write(ref ukn);
            handler.Write(ref alphaFlags);
            if (Version >= 32) handler.Write(ref ukn1);
            if (Version >= 51) handler.Write(ref pragmataUkn);
            handler.Write(ref paramHeaderOffset);
            handler.Write(ref texHeaderOffset);
            if (Version >= 19)
            {
                handler.Write(ref gpbfOffset);
            }
            handler.Write(ref paramsOffset);
            handler.StringTableAdd(mmtrPath);
            handler.Write(ref mmtrPathOffset);
            if (Version >= 32) handler.Write(ref texIDsOffset);
            return true;
        }

        public override MaterialHeader Clone() => (MaterialHeader)base.Clone();
    }

    [Flags]
    public enum MaterialFlags1 {
        BaseTwoSideEnable = (1 << 0),
        BaseAlphaTestEnable = (1 << 1),
        ShadowCastDisable = (1 << 2),
        VertexShaderUsed = (1 << 3),
        EmissiveUsed = (1 << 4),
        TessellationEnable = (1 << 5),
        EnableIgnoreDepth = (1 << 6),
        AlphaMaskUsed = (1 << 7),
        ForcedTwoSideEnable = (1 << 8),
        TwoSideEnable = (1 << 9),
    }

    [Flags]
    public enum MaterialFlags2 {
        RoughTransparentEnable = (1 << 0),
        ForcedAlphaTestEnable = (1 << 1),
        AlphaTestEnable = (1 << 2),
        SSSProfileUsed = (1 << 3),
        EnableStencilPriority = (1 << 4),
        RequireDualQuaternion = (1 << 5),
        PixelDepthOffsetUsed = (1 << 6),
        NoRayTracing = (1 << 7),
    }

    public enum ShadingType
    {
        Standard = 0,
        Decal = 1,
        DecalWithMetallic = 2,
        DecalNRMR = 3,
        Transparent = 4,
        Distortion = 5,
        PrimitiveMesh = 6,
        PrimitiveSolidMesh = 7,
        Water = 8,
        SpeedTree = 9,
        GUI = 10,
        GUIMesh = 11,
        GUIMeshTransparent = 12,
        ExpensiveTransparent = 13,
        Forward = 14,
        RenderTarget = 15,
        PostProcess = 16,
        PrimitiveMaterial = 17,
        PrimitiveSolidMaterial = 18,
        SpineMaterial = 19,
        ReflectiveTransparent = 20,
    }

    public class TexHeader : BaseModel
    {
        public long texTypeOffset;
        public string texType = string.Empty;
        public uint hash;
        public uint asciiHash;
        public long texPathOffset;
        public string? texPath;

        public override TexHeader Clone() => (TexHeader)base.Clone();

        protected override bool DoRead(FileHandler handler)
        {
            handler.Read(ref texTypeOffset);
            handler.Read(ref hash);
            handler.Read(ref asciiHash);
            handler.Read(ref texPathOffset);
            texType = handler.ReadWString(texTypeOffset);
            texPath = handler.ReadWString(texPathOffset);
            // RE3R+
            if (handler.FileVersion >= 13) handler.Skip(8);
            return true;
        }

        protected override bool DoWrite(FileHandler handler)
        {
            if (texType != null)
            {
                handler.StringTableAdd(texType);
                hash = MurMur3HashUtils.GetHash(texType);
                asciiHash = MurMur3HashUtils.GetAsciiHash(texType);
            }
            handler.Write(ref texTypeOffset);
            handler.Write(ref hash);
            handler.Write(ref asciiHash);
            handler.StringTableAdd(texPath);
            handler.Write(ref texPathOffset);
            if (handler.FileVersion >= 13) handler.Skip(8);
            return true;
        }

        public override string ToString() => $"{texType}: {texPath}";
    }

    public class ParamHeader : BaseModel
    {
        public long paramNameOffset;
        public string paramName = string.Empty;
        public uint hash;
        public uint asciiHash;
        public int componentCount;

        public Vector4 parameter;

        internal int paramRelOffset;
        internal long paramAbsOffset;
        // for padding
        internal int gapSize;

        protected override bool DoRead(FileHandler handler)
        {
            handler.Read(ref paramNameOffset);
            handler.Read(ref hash);
            handler.Read(ref asciiHash);
            // RE3R+
            if (handler.FileVersion >= 13)
            {
                handler.Read(ref paramRelOffset);
                handler.Read(ref componentCount);
            }
            else
            {
                handler.Read(ref componentCount);
                handler.Read(ref paramRelOffset);
            }
            paramName = handler.ReadWString(paramNameOffset);
            return true;
        }

        protected override bool DoWrite(FileHandler handler)
        {
            if (paramName != null)
            {
                handler.StringTableAdd(paramName);
                hash = MurMur3HashUtils.GetHash(paramName);
                asciiHash = MurMur3HashUtils.GetAsciiHash(paramName);
            }
            handler.Write(ref paramNameOffset);
            handler.Write(ref hash);
            handler.Write(ref asciiHash);
            if (handler.FileVersion >= 13)
            {
                handler.Write(ref paramRelOffset);
                handler.Write(ref componentCount);
            }
            else
            {
                handler.Write(ref componentCount);
                handler.Write(ref paramRelOffset);
            }
            return true;
        }

        public override ParamHeader Clone() => (ParamHeader)base.Clone();

        public override string ToString() => $"{paramName}: {parameter}";
    }

    public class GpuBufferEntry : BaseModel
    {
        public string name = string.Empty;
        public string path = string.Empty;

        public GpuBufferEntry()
        {
        }

        public GpuBufferEntry(string name, string path)
        {
            this.name = name;
            this.path = path;
        }

        protected override bool DoRead(FileHandler handler)
        {
            name = handler.ReadOffsetWString();
            handler.Read<int>(); // utf16Hash
            handler.Read<int>(); // asciiHash
            path = handler.ReadOffsetWString();
            handler.ReadNull(4);
            var one = handler.Read<int>();
            DataInterpretationException.DebugThrowIf(one != 1);
            return true;
        }

        protected override bool DoWrite(FileHandler handler)
        {
            handler.WriteOffsetWString(name);
            handler.Write(MurMur3HashUtils.GetHash(name));
            handler.Write(MurMur3HashUtils.GetAsciiHash(name));
            handler.WriteOffsetWString(path);
            handler.Write(0);
            handler.Write(1);
            return true;
        }

        public override string ToString() => $"{name}";

        public int CompareTo(GpuBufferEntry? other) => name.CompareTo(other?.name);
    }

    public class MaterialData
    {
        public MaterialData() { }
        public MaterialData(MaterialHeader matHeader)
        {
            Header = matHeader;
        }

        public MaterialHeader Header = new();
        public List<TexHeader> Textures = new();
        public List<ParamHeader> Parameters = new();
        public List<GpuBufferEntry> GpuBuffers = new();

        public override string ToString() => Header.matName + " :  " + Path.GetFileName(Header.mmtrPath);

        public MaterialData Clone()
        {
            return new MaterialData(Header.Clone()) {
                Textures = Textures.Select(tex => tex.Clone()).ToList(),
                GpuBuffers = GpuBuffers.Select(gpbf => new GpuBufferEntry(gpbf.name, gpbf.path)).ToList(),
                Parameters = Parameters.Select(tex => tex.Clone()).ToList(),
            };
        }
    }
}


namespace ReeLib
{
    public class MdfFile : BaseFile
    {

        public MdfFile(FileHandler fileHandler) : base(fileHandler)
        {
        }

        public short version;
        public long uknValue;
        public List<MaterialData> Materials = new();

        public const uint Magic = 0x46444d;
        public const string Extension = ".mdf2";

        protected override bool DoRead()
        {
            Materials.Clear();

            var handler = FileHandler;
            if (handler.Read<int>() != Magic)
            {
                throw new InvalidDataException($"{handler.FilePath} Not a MDF file");
            }
            handler.Read(ref version);
            var matCount = handler.Read<short>();
            handler.Read(ref uknValue);
            DataInterpretationException.DebugWarnIf(uknValue > 1);

            for (int i = 0; i < matCount; i++)
            {
                MaterialData matData = new();
                matData.Header.Read(handler);
                Materials.Add(matData);
            }

            foreach (var matData in Materials)
            {
                handler.Seek(matData.Header.texHeaderOffset);
                for (int i = 0; i < matData.Header.texCount; i++)
                {
                    TexHeader texHeader = new();
                    texHeader.Read(handler);
                    matData.Textures.Add(texHeader);
                }
            }

            foreach (var matData in Materials)
            {
                handler.Seek(matData.Header.paramHeaderOffset);
                for (int i = 0; i < matData.Header.paramCount; i++)
                {
                    ParamHeader paramHeader = new();
                    paramHeader.Read(handler);
                    paramHeader.paramAbsOffset = matData.Header.paramsOffset + paramHeader.paramRelOffset;
                    if (i == 0)
                    {
                        paramHeader.gapSize = paramHeader.paramRelOffset;
                    }
                    else
                    {
                        var prevHeader = matData.Parameters[i - 1];
                        paramHeader.gapSize = (int)(
                            paramHeader.paramAbsOffset - prevHeader.paramAbsOffset +
                            prevHeader.componentCount * 4);
                    }
                    if (paramHeader.componentCount == 4)
                    {
                        handler.Read(paramHeader.paramAbsOffset, ref paramHeader.parameter);
                    }
                    else
                    {
                        handler.Read(paramHeader.paramAbsOffset, ref paramHeader.parameter.X);
                    }
                    matData.Parameters.Add(paramHeader);
                }

                matData.GpuBuffers = new();
                var tell = handler.Tell();
                handler.Seek(matData.Header.gpbfOffset);
                for (int i = 0; i < matData.Header.gpbfNameCount; ++i) {
                    var gpbf = new GpuBufferEntry();
                    gpbf.Read(handler);
                    matData.GpuBuffers.Add(gpbf);
                }
                DataInterpretationException.DebugThrowIf(matData.Header.texIDsOffset > 0, "TODO Handle tex IDs");
                handler.Seek(tell);
            }

            return true;
        }

        protected override bool DoWrite()
        {
            FileHandler handler = FileHandler;
            handler.Clear();
            handler.Seek(0);
            handler.Write(Magic);
            handler.Write((short)version);
            handler.Write((short)Materials.Count);
            handler.Write(uknValue);

            foreach (var matData in Materials)
            {
                matData.Header.Write(handler);
            }

            foreach (var matData in Materials)
            {
                matData.Header.texHeaderOffset = handler.Tell();
                matData.Textures.Write(handler);
            }

            foreach (var matData in Materials)
            {
                matData.Header.paramHeaderOffset = handler.Tell();
                matData.Parameters.Write(handler);
            }

            handler.Align(16);

            foreach (var matData in Materials)
            {
                matData.Header.gpbfOffset = handler.Tell();
                matData.Header.gpbfNameCount = matData.Header.gpbfDataCount = matData.GpuBuffers.Count;
                matData.GpuBuffers.Write(handler);
            }

            handler.StringTableWriteStrings();

            handler.Align(16);
            foreach (var matData in Materials)
            {
                int size = 0;
                matData.Header.paramsOffset = handler.Tell();
                foreach (var paramHeader in matData.Parameters)
                {
                    size += paramHeader.componentCount * 4;
                    if (paramHeader.gapSize > 0)
                    {
                        handler.FillBytes(0, paramHeader.gapSize);
                    }
                    paramHeader.paramRelOffset = (int)(handler.Tell() - matData.Header.paramsOffset);
                    if (paramHeader.componentCount == 4)
                    {
                        handler.Write(ref paramHeader.parameter);
                    }
                    else
                    {
                        handler.Write(ref paramHeader.parameter.X);
                    }
                    paramHeader.Rewrite(handler);
                }
                matData.Header.paramsSize = size;
                handler.FillBytes(0, (int)(matData.Header.paramsOffset + matData.Header.paramsSize - handler.Tell()));
                matData.Header.Rewrite(handler);
            }

            handler.StringTableFlushOffsets();

            return true;
        }
    }
}