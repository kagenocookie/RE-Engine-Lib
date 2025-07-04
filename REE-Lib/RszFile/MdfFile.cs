using System.Numerics;
using ReeLib.Common;
using ReeLib.Mdf;


namespace ReeLib.Mdf
{
    public struct MdfHeaderStruct {
        public uint magic;
        public short mdfVersion;
        public short matCount;
    }

    public class MatHeader : BaseModel
    {
        public long matNameOffset; // 1
        public string? matName;
        public uint matNameHash; // 2
        // tdbVersion == 49, RE7
        public ulong uknRE7;
        public int paramsSize; // 3
        public int paramCount; // 4
        public int texCount; // 5
        // tdbVersion >= 69, RE8+
        public int gpbfNameCount;
        public int gpbfDataCount;
        public uint shaderType; // 6
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

        private GameVersion Version { get; }

        public MatHeader(GameVersion version)
        {
            Version = version;
        }

        protected override bool DoRead(FileHandler handler)
        {
            long pos = handler.Tell();
            handler.Read(ref matNameOffset);
            matName = handler.ReadWString(matNameOffset);
            handler.Read(ref matNameHash);
            if (Version == GameVersion.re7) handler.Read(ref uknRE7);
            handler.Read(ref paramsSize);
            handler.Read(ref paramCount);
            handler.Read(ref texCount);
            if (Version >= GameVersion.re8)
            {
                handler.Read(ref gpbfNameCount);
                handler.Read(ref gpbfDataCount);
                if (gpbfNameCount != gpbfDataCount)
                {
                    throw new Exception("GPBF Count mismatch!");
                }
            }
            handler.Read(ref shaderType);
            if (Version >= GameVersion.re4) handler.Read(ref ukn);
            handler.Read(ref alphaFlags);
            if (Version >= GameVersion.re4) handler.Read(ref ukn1);
            handler.Read(ref paramHeaderOffset);
            handler.Read(ref texHeaderOffset);
            if (Version >= GameVersion.re8)
            {
                handler.Read(ref gpbfOffset);
            }
            handler.Read(ref paramsOffset);
            handler.Read(ref mmtrPathOffset);
            mmtrPath = handler.ReadWString(mmtrPathOffset);
            if (Version >= GameVersion.re4) handler.Read(ref texIDsOffset);
            return true;
        }

        protected override bool DoWrite(FileHandler handler)
        {
            if (matName != null)
            {
                handler.StringTableAdd(matName);
                matNameHash = MurMur3HashUtils.GetHash(matName);
            }
            handler.Write(ref matNameOffset);
            handler.Write(ref matNameHash);
            if (Version == GameVersion.re7) handler.Write(ref uknRE7);
            handler.Write(ref paramsSize);
            handler.Write(ref paramCount);
            handler.Write(ref texCount);
            if (Version >= GameVersion.re8) {
                handler.Write(ref gpbfNameCount);
                handler.Write(ref gpbfDataCount);
            }
            handler.Write(ref shaderType);
            if (Version >= GameVersion.re4) handler.Write(ref ukn);
            handler.Write(ref alphaFlags);
            if (Version >= GameVersion.re4) handler.Write(ref ukn1);
            handler.Write(ref paramHeaderOffset);
            handler.Write(ref texHeaderOffset);
            if (Version >= GameVersion.re8)
            {
                handler.Write(ref gpbfOffset);
            }
            handler.Write(ref paramsOffset);
            handler.StringTableAdd(mmtrPath);
            handler.Write(ref mmtrPathOffset);
            if (Version >= GameVersion.re4) handler.Write(ref texIDsOffset);
            return true;
        }
    }

    public class TexHeader : BaseModel
    {
        public long texTypeOffset;
        public string? texType;
        public uint hash;
        public uint asciiHash;
        public long texPathOffset;
        public string? texPath;

        private GameVersion Version { get; }

        public TexHeader(GameVersion version)
        {
            Version = version;
        }

        protected override bool DoRead(FileHandler handler)
        {
            handler.Read(ref texTypeOffset);
            handler.Read(ref hash);
            handler.Read(ref asciiHash);
            handler.Read(ref texPathOffset);
            texType = handler.ReadWString(texTypeOffset);
            texPath = handler.ReadWString(texPathOffset);
            // RE3R+
            if (Version >= GameVersion.re3) handler.Skip(8);
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
            if (Version >= GameVersion.re3) handler.Skip(8);
            return true;
        }
    }

    public class ParamHeader : BaseModel
    {
        public long paramNameOffset;
        public string? paramName;
        public uint hash;
        public uint asciiHash;
        public int componentCount;
        public int paramRelOffset;
        // fileData end
        // MatHeader.paramsOffset + paramRelOffset
        public long paramAbsOffset;
        // for padding
        public int gapSize;
        public Vector4 parameter;

        private GameVersion Version { get; }

        public ParamHeader(GameVersion version)
        {
            Version = version;
        }

        protected override bool DoRead(FileHandler handler)
        {
            handler.Read(ref paramNameOffset);
            handler.Read(ref hash);
            handler.Read(ref asciiHash);
            // RE3R+
            if (Version >= GameVersion.re3)
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
            if (Version >= GameVersion.re3)
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
    }

    public class GpbfHeader : BaseModel
    {
        public long nameOffset;
        public string? name;
        public uint utf16Hash;
        public uint asciiHash;

        public GpbfHeader()
        {
        }

        public GpbfHeader(string name)
        {
            this.name = name;
            asciiHash = MurMur3HashUtils.GetAsciiHash(name);
            utf16Hash = MurMur3HashUtils.GetHash(name);
        }

        protected override bool DoRead(FileHandler handler)
        {
            handler.Read(ref nameOffset);
            name = handler.ReadWString(nameOffset);
            handler.Read(ref utf16Hash);
            handler.Read(ref asciiHash);
            return true;
        }

        protected override bool DoWrite(FileHandler handler)
        {
            nameOffset = handler.Tell();
            handler.WriteOffsetWString(name ?? string.Empty);
            handler.Write(ref utf16Hash);
            handler.Write(ref asciiHash);
            return true;
        }
    }

    public class MatData
    {
        public MatData(MatHeader matHeader)
        {
            Header = matHeader;
        }

        public MatHeader Header;
        public List<TexHeader> TexHeaders = new();
        public List<ParamHeader> ParamHeaders = new();
        public List<(GpbfHeader name, GpbfHeader data)> GpbfHeaders = new();
    }
}


namespace ReeLib
{
    public class MdfFile : BaseRszFile
    {

        public MdfFile(RszFileOption option, FileHandler fileHandler) : base(option, fileHandler)
        {
        }

        public StructModel<MdfHeaderStruct> Header = new();
        public List<MatData> MatDatas = new();

        public const uint Magic = 0x46444d;
        public const string Extension2 = ".mdf2";

        public string? GetExtension()
        {
            return Option.GameName switch
            {
                GameName.re2 => ".10",
                GameName.re2rt => ".21",
                GameName.re3 => ".13",
                GameName.re3rt => ".21",
                GameName.re4 => ".32",
                GameName.re8 => ".19",
                GameName.re7 => ".6",
                GameName.re7rt => ".21",
                GameName.dmc5 =>".10",
                GameName.mhrise => ".23",
                GameName.sf6 => ".31",
                GameName.dd2 => ".31",
                GameName.mhwilds => ".45",
                _ => null
            };
        }

        protected override bool DoRead()
        {
            MatDatas.Clear();

            var handler = FileHandler;
            if (!Header.Read(handler)) return false;
            if (Header.Data.magic != Magic)
            {
                throw new InvalidDataException($"{handler.FilePath} Not a MDF file");
            }

            handler.Align(16);
            for (int i = 0; i < Header.Data.matCount; i++)
            {
                MatData matData = new(new(Option.Version));
                matData.Header.Read(handler);
                MatDatas.Add(matData);
            }

            foreach (var matData in MatDatas)
            {
                handler.Seek(matData.Header.texHeaderOffset);
                for (int i = 0; i < matData.Header.texCount; i++)
                {
                    TexHeader texHeader = new(Option.Version);
                    texHeader.Read(handler);
                    matData.TexHeaders.Add(texHeader);
                }
            }

            foreach (var matData in MatDatas)
            {
                handler.Seek(matData.Header.paramHeaderOffset);
                for (int i = 0; i < matData.Header.paramCount; i++)
                {
                    ParamHeader paramHeader = new(Option.Version);
                    paramHeader.Read(handler);
                    paramHeader.paramAbsOffset = matData.Header.paramsOffset + paramHeader.paramRelOffset;
                    if (i == 0)
                    {
                        paramHeader.gapSize = paramHeader.paramRelOffset;
                    }
                    else
                    {
                        var prevHeader = matData.ParamHeaders[i - 1];
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
                    matData.ParamHeaders.Add(paramHeader);
                }

                matData.GpbfHeaders = new();
                var tell = handler.Tell();
                handler.Seek(matData.Header.gpbfOffset);
                for (int i = 0; i < matData.Header.gpbfNameCount; ++i) {
                    var name = new GpbfHeader();
                    var data = new GpbfHeader();
                    name.Read(handler);
                    data.Read(handler);
                    matData.GpbfHeaders.Add((name, data));
                }
                handler.Seek(tell);
            }

            return true;
        }

        protected override bool DoWrite()
        {
            FileHandler handler = FileHandler;
            handler.Clear();
            handler.Seek(0);
            Header.Write(handler);

            handler.Align(16);
            foreach (var matData in MatDatas)
            {
                matData.Header.Write(handler);
            }

            foreach (var matData in MatDatas)
            {
                matData.Header.texHeaderOffset = handler.Tell();
                matData.TexHeaders.Write(handler);
            }

            foreach (var matData in MatDatas)
            {
                matData.Header.paramHeaderOffset = handler.Tell();
                matData.ParamHeaders.Write(handler);
            }

            handler.Align(16);

            foreach (var matData in MatDatas)
            {
                matData.Header.gpbfOffset = handler.Tell();
                matData.Header.gpbfNameCount = matData.Header.gpbfDataCount = matData.GpbfHeaders.Count;
                foreach (var gpbf in matData.GpbfHeaders) {
                    gpbf.name.Write(handler);
                    gpbf.data.Write(handler);
                }
            }

            handler.StringTableWriteStrings();

            handler.Align(16);
            foreach (var matData in MatDatas)
            {
                int size = 0;
                matData.Header.paramsOffset = handler.Tell();
                foreach (var paramHeader in matData.ParamHeaders)
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