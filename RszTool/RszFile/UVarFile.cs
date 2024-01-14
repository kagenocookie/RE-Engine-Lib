using System.Runtime.InteropServices;
using RszTool.UVar;

namespace RszTool.UVar
{
    public class UvarValue : BaseModel
    {
        public uint nameHash;
        public uint type;
        public long hashOffset;
        public Guid guid;
        public uint uintValue;


        protected override bool DoRead(FileHandler handler)
        {
            handler.Read(ref nameHash);
            handler.Read(ref type);
            switch (type)
            {
                case 6:  // CallbackNode
                case 7:  // LogicNode
                    handler.Read(ref uintValue);
                    handler.Skip(4);
                    break;
                case 20: // VariableReferenceNode
                    handler.Read(ref hashOffset);
                    handler.Read(hashOffset, ref guid);
                    break;
                default:
                    break;
            }
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

        UvarValue? HashProperty { get; set; }

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


    public class UvarNode : BaseModel
    {
        public long nameOffset;
        public long dataOffset;
        public string Name { get; set; } = string.Empty;
        public short index;
        public short valueCount;
        public short uknCount1;
        public short uknCount2;

        List<UvarValue> Values { get; } = new();

        protected override bool DoRead(FileHandler handler)
        {
            handler.Read(ref nameOffset);
            handler.Read(ref dataOffset);
            Name = handler.ReadAsciiString(nameOffset);
            handler.Skip(8);
            handler.ReadRange(ref index, ref uknCount2);
            using var jumpBack = handler.SeekJumpBack(dataOffset);
            Values.Read(handler, valueCount);
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
        public List<UvarNode> Nodes { get; set; } = new();

        protected override bool DoRead(FileHandler handler)
        {
            handler.Read(ref countA);
            handler.Read(ref countB);
            handler.Read(ref countC);
            handler.Read(ref D);

            // 在bhvt内嵌的uvar文件中，似乎对不上
            /* for (int i = 0; i < countA; i++)
            {
                if (handler.ReadUInt64(handler.Tell()) > 0)
                {
                    var node = new UvarNode();
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
            } */
            return true;
        }

        protected override bool DoWrite(FileHandler handler)
        {
            throw new NotImplementedException();
        }
    }


    public class UvarProp : BaseModel
    {
        public long nodesOffset;
        public long offset2;
        public short propCount, ukn00, ukn01;
        public List<UvarNode> Nodes { get; set; } = new();
        public OFFSET2_DATA? Offset2Data { get; set; }

        protected override bool DoRead(FileHandler handler)
        {
            handler.Read(ref nodesOffset);
            handler.Read(ref offset2);
            handler.Read(ref propCount);
            handler.Read(ref ukn00);
            handler.Read(ref ukn01);
            if (propCount > 0)
            {
                if (nodesOffset > 0)
                {
                    handler.Seek(nodesOffset);
                    if (handler.ReadUInt(handler.Tell()) < handler.FileSize())
                    {
                        for (int i = 0; i < propCount; i++)
                        {
                            var node = new UvarNode();
                            node.Read(handler);
                            Nodes.Add(node);
                        }
                    }
                }
                if (offset2 > 0)
                {
                    using var jumpBack = handler.SeekJumpBack(offset2);
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


    [StructLayout(LayoutKind.Explicit, Size = 4)]
    public struct VariableValue
    {
        [FieldOffset(0)]
        public int IntValue;

        [FieldOffset(0)]
        public float FloatValue;
    }


    public class Variable : BaseModel
    {
        public Guid guid;
        public long nameOffset;
        public string Name { get; set; } = string.Empty;
        public long valueOffset;
        public long propOffset;
        private uint type_numBits;
        public uint nameHash;

        public uint Type => type_numBits & 0x00FFFFFF;
        public uint NumBits => (type_numBits >> 24) & 0xFF;

        public VariableValue Value;

        public UvarProp? Prop { get; private set; }

        protected override bool DoRead(FileHandler handler)
        {
            handler.Read(ref guid);
            handler.Read(ref nameOffset);
            Name = handler.ReadWString(nameOffset);
            handler.Read(ref valueOffset);
            handler.Read(ref propOffset);
            handler.Read(ref type_numBits);
            handler.Read(ref nameHash);

            if (valueOffset > 0)
            {
                handler.Read(valueOffset, ref Value);
            }
            if (propOffset > 0)
            {
                Prop ??= new();
                Prop.Read(handler, propOffset);
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


    public class HashData
    {
        public Guid[]? Guids { get; set; }
        public uint[]? GuidMap { get; set; }
        public uint[]? NameHashes { get; set; }
        public uint[]? NameHashMap { get; set; }
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
        public HashData HashData { get; } = new();

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
                long[] embedOffsets = handler.ReadArray<long>(header.embedCount);
                for (int i = 0; i < header.embedCount; i++)
                {
                    UVarFile embedFile = new(Option, handler.WithOffset(embedOffsets[i]));
                    embedFile.Read();
                    EmbeddedUVARs.Add(embedFile);
                }
            }

            if (header.variableCount > 0 && header.hashInfoOffset > 0)
            {
                handler.Seek(header.hashInfoOffset);
                long[] hashDataOffsets = new long[4];
                handler.ReadArray(hashDataOffsets);
                handler.Seek(hashDataOffsets[0]);
                HashData.Guids = handler.ReadArray<Guid>(header.variableCount);
                handler.Seek(hashDataOffsets[1]);
                HashData.GuidMap = handler.ReadArray<uint>(header.variableCount);
                handler.Seek(hashDataOffsets[2]);
                HashData.NameHashes = handler.ReadArray<uint>(header.variableCount);
                handler.Seek(hashDataOffsets[3]);
                HashData.NameHashMap = handler.ReadArray<uint>(header.variableCount);
            }

            return true;
        }

        protected override bool DoWrite()
        {
            throw new NotImplementedException();
        }
    }
}
