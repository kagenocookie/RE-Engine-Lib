using RszTool.Common;
using RszTool.UVar;

namespace RszTool.UVar
{
    public class HashProperty : BaseModel
    {
        public uint nameHash;
        public uint ukn;
        private long test;
        public long hashOffset;
        public Guid guid;
        public float floatValue;
        public uint uintValue;


        protected override bool DoRead(FileHandler handler)
        {
            handler.Read(ref nameHash);
            handler.Read(ref ukn);
            handler.Read(ref test);
            if (test > 0 && test < handler.FileSize() && test > handler.Tell())
            {
                handler.Read(ref hashOffset);
                handler.Seek(hashOffset);
                handler.Read(ref guid);
                return true;
            }
            else
            {
                Span<byte> bytes = stackalloc byte[4];
                handler.ReadSpan(bytes);
                if (!Utils.DetectFloat(bytes, out floatValue))
                {
                    uintValue = MemoryUtils.AsRef<uint>(bytes);
                }
            }

            handler.Seek(Start + 16);
            return true;
        }

        protected override bool DoWrite(FileHandler handler)
        {
            throw new NotImplementedException();
        }
    }


    public class SUBPROP2 : BaseModel
    {
        public ushort propCount, B, C;
        public long offset1, offset2;
        public string? Name { get; set; }

        HashProperty? HashProperty { get; set; }

        protected override bool DoRead(FileHandler handler)
        {
            handler.Read(ref propCount);
            handler.Read(ref B);
            handler.Read(ref C);
            handler.Align(16);
            handler.Read(ref offset1);
            handler.Read(ref offset2);
            if (propCount > 0)
            {
                if (offset1 < handler.FileSize())
                {
                    handler.ReadAsciiString(offset1);
                }
                if (offset2 < handler.FileSize())
                {
                    handler.Seek(offset2);
                    if (handler.ReadUShort(handler.Tell() + 2) != 0)
                    {
                        HashProperty ??= new();
                        HashProperty.Read(handler);
                    }
                }
            }
            handler.Seek(Start + 32);
            return true;
        }

        protected override bool DoWrite(FileHandler handler)
        {
            throw new NotImplementedException();
        }
    }


    public class NODE : BaseModel
    {
        public long nameOffset;
        public long dataOffset;
        public string Name { get; set; } = string.Empty;

        HashProperty HashProperty { get; set; } = new();

        protected override bool DoRead(FileHandler handler)
        {
            handler.Read(ref nameOffset);
            handler.Read(ref dataOffset);
            Name = handler.ReadAsciiString(nameOffset);
            HashProperty.Read(handler, dataOffset);
            handler.Skip(16);
            return true;
        }

        protected override bool DoWrite(FileHandler handler)
        {
            throw new NotImplementedException();
        }
    }


    public class OFFSET2_DATA : BaseModel
    {
        public long countA;
        public short countB;
        public short countC;
        public uint D;
        public List<NODE> Nodes { get; set; } = new();

        protected override bool DoRead(FileHandler handler)
        {
            handler.Read(ref countA);
            handler.Read(ref countB);
            handler.Read(ref countC);
            handler.Read(ref D);

            for (int i = 0; i < countA; i++)
            {
                if (handler.ReadUInt64(handler.Tell()) > 0)
                {
                    var node = new NODE();
                    node.Read(handler);
                    Nodes.Add(node);
                }
            }
            if (countC > 1)
            {
                for (int i = 1; i < countC; i++)
                {
                    handler.Skip(16);
                }
            }
            return true;
        }

        protected override bool DoWrite(FileHandler handler)
        {
            throw new NotImplementedException();
        }
    }


    public class PROP : BaseModel
    {
        public long nodesOffset;
        public long offset2;
        public short propCount, B, C;
        public List<NODE> Nodes { get; set; } = new();
        public OFFSET2_DATA? Offset2Data { get; set; }

        protected override bool DoRead(FileHandler handler)
        {
            handler.Read(ref nodesOffset);
            handler.Read(ref offset2);
            handler.Read(ref propCount);
            handler.Read(ref B);
            handler.Read(ref C);
            if (propCount > 0)
            {
                if (nodesOffset > 0)
                {
                    handler.Seek(nodesOffset);
                    if (handler.ReadUInt(handler.Tell()) < handler.FileSize())
                    {
                        for (int i = 0; i < propCount; i++)
                        {
                            var node = new NODE();
                            node.Read(handler);
                            Nodes.Add(node);
                        }
                    }
                }
                if (offset2 > 0)
                {
                    handler.Seek(offset2);
                    if (handler.ReadInt64(handler.Tell()) < handler.FileSize())
                    {
                        Offset2Data ??= new();
                        Offset2Data.Read(handler);
                    }
                    else
                    {
                        handler.Skip(16);
                    }
                }
            }
            return true;
        }

        protected override bool DoWrite(FileHandler handler)
        {
            throw new NotImplementedException();
        }
    }


    public class Variable : BaseModel
    {
        public Guid guid;
        public long nameOffset;
        public string Name { get; set; } = string.Empty;
        public long floatOffset, uknOffset;
        private uint type_numBits;
        public uint nameHash;

        public uint Type => type_numBits & 0x00FFFFFF;
        public uint NumBits => (type_numBits >> 24) & 0xFF;

        public PROP? Prop { get; private set; }

        protected override bool DoRead(FileHandler handler)
        {
            handler.Read(ref guid);
            handler.Read(ref nameOffset);
            Name = handler.ReadWString(nameOffset);
            handler.Read(ref floatOffset);
            handler.Read(ref uknOffset);
            handler.Read(ref type_numBits);
            handler.Read(ref nameHash);

            if (floatOffset > 0)
            {
                handler.Seek(floatOffset);
                // TODO value
            }
            if (uknOffset > 0)
            {
                Prop ??= new();
                Prop.Read(handler, uknOffset);
            }
            return true;
        }

        protected override bool DoWrite(FileHandler handler)
        {
            throw new NotImplementedException();
        }
    }


    public class HeaderStruct : BaseModel
    {
        public uint version;
        public uint magic;
        public long stringsOffset;
        public long dataOffset;
        public long embedsInfoOffset;
        public long hashInfoOffset;

        public ulong ukn;
        public uint UVARhash;
        public short variableCount, embedCount;

        private bool ReadWrite(IFileHandlerAction action)
        {
            action.Then(ref version)?.Then(ref magic)
                 ?.Then(ref stringsOffset)?.Then(ref dataOffset)
                 ?.Then(ref embedsInfoOffset)?.Then(ref hashInfoOffset)
                 ?.Then(version < 3, ref ukn)
                 ?.Then(ref UVARhash)?.Then(ref variableCount)?.Then(ref embedCount);
            return true;
        }

        protected override bool DoRead(FileHandler handler)
        {
            return ReadWrite(new FileHandlerRead(handler));
        }

        protected override bool DoWrite(FileHandler handler)
        {
            return ReadWrite(new FileHandlerWrite(handler));
        }
    }
}


namespace RszTool
{
    public class UVarFile(RszFileOption option, FileHandler fileHandler) : BaseRszFile(option, fileHandler)
    {
        public const uint Magic = 0x72617675;
        public const string Extension2 = ".uvar";

        public HeaderStruct Header { get; set; } = new();
        public List<Variable> Variables { get; set; } = new();
        public List<string> Strings { get; set; } = new();
        public List<UVarFile> EmbeddedUVARs { get; set; } = new();

        protected override bool DoRead()
        {
            var handler = FileHandler;
            var header = Header;
            if (!header.Read(handler)) return false;
            if (header.magic != Magic)
            {
                throw new InvalidDataException($"{handler.FilePath} Not a UVAR file");
            }

            if (header.variableCount > 0 && header.dataOffset > 0)
            {
                handler.Seek(header.dataOffset);
                Variables.Read(handler, header.variableCount);
            }

            if (header.stringsOffset > 0)
            {
                handler.Seek(header.stringsOffset);
                for (int i = 0; i < header.variableCount + 1; i++)
                {
                    Strings.Add(handler.ReadWString(jumpBack: false));
                }
            }

            if (header.embedCount > 0 && header.embedsInfoOffset > 0)
            {
                handler.Seek(header.embedsInfoOffset);
                long[] embedOffsets = new long[header.embedCount];
                handler.ReadArray(embedOffsets);
                for (int i = 0; i < header.embedCount; i++)
                {
                    UVarFile embedFile = new(Option, handler.WithOffset(embedOffsets[i]));
                    embedFile.Read(0, false);
                    EmbeddedUVARs.Add(embedFile);
                }
            }

            if (header.variableCount > 0 && header.hashInfoOffset > 0)
            {
                handler.Seek(header.hashInfoOffset);
                long[] hashDataOffsets = new long[4];
                handler.ReadArray(hashDataOffsets);
                handler.Seek(hashDataOffsets[0]);
                Guid[] guids = new Guid[header.variableCount];
                handler.ReadArray(guids);
                handler.Seek(hashDataOffsets[1]);
                uint[] guidMap = new uint[header.variableCount];
                handler.ReadArray(guidMap);
                handler.Seek(hashDataOffsets[2]);
                uint[] nameHashes = new uint[header.variableCount];
                handler.ReadArray(nameHashes);
                handler.Seek(hashDataOffsets[3]);
                uint[] nameHashMap = new uint[header.variableCount];
                handler.ReadArray(nameHashMap);
            }

            return true;
        }

        protected override bool DoWrite()
        {
            throw new NotImplementedException();
        }
    }
}
