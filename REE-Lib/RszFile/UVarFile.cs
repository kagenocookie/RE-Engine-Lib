using System.Numerics;
using System.Runtime.InteropServices;
using ReeLib.Common;
using ReeLib.UVar;

namespace ReeLib.UVar
{
    public class NodeParameter : BaseModel
    {
        public uint nameHash;
        public NodeValueType type;
        public object? value;

        // note: enum is pure guesswork
        public enum NodeValueType : int
        {
            Unknown = 0,
            UInt32Maybe = 6,
            Int32 = 7, // can also represent enums for LogicNode (LogicOperatorType enum?)
            Single = 10,
            Guid = 20,
        }

        protected override bool DoRead(FileHandler handler)
        {
            handler.Read(ref nameHash);
            handler.Read(ref type);
            switch (type)
            {
                case NodeValueType.UInt32Maybe:
                    value = handler.Read<uint>();
                    break;
                case NodeValueType.Int32:
                    value = handler.Read<int>();
                    break;
                case NodeValueType.Single:
                    value = handler.Read<float>();
                    break;
                case NodeValueType.Guid: // VariableReferenceNode
                    handler.Seek(handler.Read<long>());
                    value = handler.Read<Guid>();
                    break;
                default:
                    throw new NotImplementedException("Unhandled UVAR node value type id " + type);
            }
            handler.Align(16);
            return true;
        }

        protected override bool DoWrite(FileHandler handler)
        {
            handler.Write(ref nameHash);
            handler.Write(ref type);
            switch (type) {
                case NodeValueType.Int32: handler.Write((int?)value ?? default); break;
                case NodeValueType.UInt32Maybe: handler.Write((uint?)value ?? default); break;
                case NodeValueType.Single: handler.Write((float?)value ?? default); break;
                case NodeValueType.Guid:
                    handler.Write(handler.Tell() + 8);
                    handler.Write((Guid?)value ?? default); break;
                default:
                    throw new NotImplementedException("Unhandled UVAR node value type id " + type);
            }
            handler.Align(16);
            return true;
        }
    }


    public class UvarNode : BaseModel
    {
        public long nameOffset;
        public long dataOffset;
        public long uknOffset;
        public short nodeId;
        public short valueCount;
        public int uknCount;

        public string Name { get; set; } = string.Empty;
        public List<NodeParameter> Parameters = new(1);

        protected override bool DoRead(FileHandler handler)
        {
            handler.Read(ref nameOffset);
            handler.Read(ref dataOffset);
            handler.Read(ref uknOffset);
            handler.Read(ref nodeId);
            handler.Read(ref valueCount);
            handler.Read(ref uknCount);
            Name = handler.ReadAsciiString(nameOffset);
            using var jumpBack = handler.SeekJumpBack(dataOffset);
            Parameters.Read(handler, valueCount);
            return true;
        }

        protected override bool DoWrite(FileHandler handler)
        {
            valueCount = (short)Parameters.Count;
            handler.Write(ref nameOffset);
            handler.Write(ref dataOffset);
            handler.Write(ref uknOffset);
            handler.Write(ref nodeId);
            handler.Write(ref valueCount);
            handler.Write(ref uknCount);
            return true;
        }

        public void FlushData(FileHandler handler)
        {
            handler.Write(Start, nameOffset = handler.Tell());
            handler.WriteAsciiString(Name ?? string.Empty);
            handler.Align(16);
            if (valueCount > 0)
            {
                handler.Write(Start + 8, dataOffset = handler.Tell());
                Parameters.Write(handler);
            }
            handler.Align(16);
        }
    }


    public class UvarExpression : BaseModel
    {
        public long nodesOffset;
        public long relationsOffset;
        public short nodeCount, outputNodeId, unknownCount;
        public List<UvarNode> Nodes { get; set; } = new();
        public List<NodeConnection> Connections { get; set; } = new();

        public struct NodeConnection
        {
            public short nodeId;
            public short inputSlot;
            public short node2;
            public short outputSlot;
        }

        protected override bool DoRead(FileHandler handler)
        {
            handler.Read(ref nodesOffset);
            handler.Read(ref relationsOffset);
            handler.Read(ref nodeCount);
            handler.Read(ref outputNodeId);
            handler.Read(ref unknownCount);
            if (nodeCount > 0)
            {
                if (nodesOffset > 0)
                {
                    handler.Seek(nodesOffset);
                    if (handler.ReadUInt(handler.Tell()) < handler.FileSize())
                    {
                        for (int i = 0; i < nodeCount; i++)
                        {
                            var node = new UvarNode();
                            node.Read(handler);
                            Nodes.Add(node);
                        }
                    }
                }
                if (relationsOffset > 0)
                {
                    // TODO not sure how to determine expected size properly - inferred based on node types maybe?
                    // the extra count field is not it, as it's sometimes lower than the actual number of connections
                    using var _ = handler.SeekJumpBack(relationsOffset);
                    while (handler.Position < handler.Stream.Length &&
                        handler.Read<ushort>(handler.Tell()) < Nodes.Count &&
                        handler.Read<long>(handler.Tell()) != 0
                    ) {
                        Connections.Add(handler.Read<NodeConnection>());
                    }
                }
            }
            return true;
        }

        protected override bool DoWrite(FileHandler handler)
        {
            handler.Skip(16); // nodesOffset, relationsOFfset
            handler.Write(nodeCount = (short)Nodes.Count);
            handler.Write(ref outputNodeId);
            handler.Write(ref unknownCount);

            handler.Align(16);
            handler.Write(Start, nodesOffset = handler.Tell());
            Nodes.Write(handler);

            foreach (var node in Nodes)
            {
                node.FlushData(handler);
            }

            handler.Write(Start + 8, relationsOffset = handler.Tell());
            foreach (var relId in Connections)
            {
                handler.Write(relId);
            }
            handler.Align(16);

            return true;
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
        public long expressionOffset;
        public TypeKind type;
        public UvarFlags flags;
        public uint nameHash;

        public object? Value;
        public UvarExpression? Expression { get; set; }

        // likely set of flags: Const, Expression, HasRange, ReadOnly, WriteProtect, + at least 2 more (Valid?)
        [Flags]
        public enum UvarFlags
        {
            Ukn1 = (1 << 0),
            Ukn2 = (1 << 1),
            Ukn3 = (1 << 2),
            Ukn4 = (1 << 3),
            Ukn5 = (1 << 4),
            // ukn 1-5: unused?

            Ukn6 = (1 << 5),
            IsVec3 = (1 << 6),
            Ukn8 = (1 << 7), // readonly, maybe?
        }

        /// <summary>
        /// via.userdata.TypeKind
        /// </summary>
        public enum TypeKind : int
        {
            Unknown = 0,
            Enum = 1,
            Boolean = 2,
            Int8 = 3,
            Uint8 = 4,
            Int16 = 5,
            Uint16 = 6,
            Int32 = 7,
            Uint32 = 8,
            Int64 = 9,
            Uint64 = 10,
            Single = 11,
            Double = 12,
            C8 = 13,
            C16 = 14,
            String = 15,
            Trigger = 16,
            Vec2 = 17,
            Vec3 = 18,
            Vec4 = 19,
            Matrix = 20,
            GUID = 21,
            Num = 22,
        }

        protected override bool DoRead(FileHandler handler)
        {
            handler.Read(ref guid);
            handler.Read(ref nameOffset);
            Name = handler.ReadWString(nameOffset);
            handler.Read(ref valueOffset);
            handler.Read(ref expressionOffset);
            var typeBits = handler.Read<uint>();
            type = (TypeKind)(typeBits & 0xffffff);
            flags = (UvarFlags)(int)((typeBits & 0xff000000) >> 24);
            handler.Read(ref nameHash);

            var pos = handler.Tell();

            if (valueOffset > 0) {
                handler.Seek(valueOffset);
                if ((flags & UvarFlags.IsVec3) != 0)
                {
                    switch (type) {
                        case TypeKind.Int8: Value = handler.ReadArray<sbyte>(3); break;
                        case TypeKind.Uint8: Value = handler.ReadArray<byte>(3); break;
                        case TypeKind.Int16: Value = handler.ReadArray<short>(3); break;
                        case TypeKind.Uint16: Value = handler.ReadArray<ushort>(3); break;
                        case TypeKind.Int32: Value = handler.ReadArray<int>(3); break;
                        case TypeKind.Uint32: Value = handler.ReadArray<uint>(3); break;
                        case TypeKind.Single: Value = handler.Read<Vector3>(); break;
                        default:
                            throw new Exception($"Unhandled vec3 variable type " + Name + " = " + type);
                    }
                }
                else
                {
                    switch (type) {
                        case TypeKind.Enum: Value = handler.Read<int>(); break;
                        case TypeKind.Boolean: Value = handler.Read<bool>(); break;
                        case TypeKind.Int8: Value = handler.Read<sbyte>(); break;
                        case TypeKind.Uint8: Value = handler.Read<byte>(); break;
                        case TypeKind.Int16: Value = handler.Read<short>(); break;
                        case TypeKind.Uint16: Value = handler.Read<ushort>(); break;
                        case TypeKind.Int32: Value = handler.Read<int>(); break;
                        case TypeKind.Uint32: Value = handler.Read<uint>(); break;
                        case TypeKind.Single: Value = handler.Read<float>(); break;
                        case TypeKind.Double: Value = handler.Read<double>(); break;
                        case TypeKind.C8: Value = handler.ReadOffsetAsciiString(); break;
                        case TypeKind.C16: Value = handler.ReadOffsetWString(); break;
                        case TypeKind.String: Value = handler.ReadWString(); break;
                        case TypeKind.Trigger: break;
                        case TypeKind.Vec2: Value = handler.Read<Vector2>(); break;
                        case TypeKind.Vec3: Value = handler.Read<Vector3>(); break;
                        case TypeKind.Vec4: Value = handler.Read<Vector4>(); break;
                        case TypeKind.Matrix: Value = handler.Read<via.mat4>(); break;
                        case TypeKind.GUID: Value = handler.Read<Guid>(); break;
                        default:
                            throw new Exception("Unhandled variable type " + Name + " = " + type);
                    }
                }
            }
            if (expressionOffset > 0) {
                Expression ??= new();
                Expression.Read(handler, expressionOffset);
            }
            if (expressionOffset > 0) {
                Expression = new UvarExpression();
                using var _ = handler.SeekJumpBack(expressionOffset);
                Expression.Read(handler);
            }
            handler.Seek(pos);
            return true;
        }

        protected override bool DoWrite(FileHandler handler)
        {
            handler.Write(ref guid);
            handler.Write(0L); //nameOffset
            handler.Write(0L); //valueOffset
            handler.Write(0L); //expressionsOffset
            var typeBits = (uint)type & 0xffffff | ((uint)flags << 24);
            handler.Write<uint>(ref typeBits);
            handler.Write<uint>(nameHash = MurMur3HashUtils.GetHash(Name ?? string.Empty));
            return true;
        }

        public void WriteValue(FileHandler handler)
        {
            if (Value != null)
            {
                handler.Write(Start + 24, valueOffset = handler.Tell());
                if ((flags & UvarFlags.IsVec3) != 0) {
                    switch (type) {
                        case TypeKind.Int8: handler.WriteArray((sbyte[])Value); break;
                        case TypeKind.Uint8: handler.WriteArray((byte[])Value); break;
                        case TypeKind.Int16: handler.WriteArray((short[])Value); break;
                        case TypeKind.Uint16: handler.WriteArray((ushort[])Value); break;
                        case TypeKind.Int32: handler.WriteArray((int[])Value); break;
                        case TypeKind.Uint32: handler.WriteArray((uint[])Value); break;
                        case TypeKind.Single: handler.Write((Vector3)Value); break;
                        default:
                            throw new Exception("Unhandled vec3 variable type " + Name + " = " + type);
                    }
                    return;
                }
                switch (type) {
                    case TypeKind.Enum: handler.Write((int)Value); break;
                    case TypeKind.Boolean: handler.Write((bool)Value); break;
                    case TypeKind.Int8: handler.Write((sbyte)Value); break;
                    case TypeKind.Uint8: handler.Write((byte)Value); break;
                    case TypeKind.Int16: handler.Write((short)Value); break;
                    case TypeKind.Uint16: handler.Write((ushort)Value); break;
                    case TypeKind.Int32: handler.Write((int)Value); break;
                    case TypeKind.Uint32: handler.Write((uint)Value); break;
                    case TypeKind.Single:
                            // TODO: sometimes it seems to be 3 float and not just one?s
                        if ((flags & UvarFlags.IsVec3) != 0) handler.Write((Vector3)Value);
                        else handler.Write((float)Value);
                        break;
                    case TypeKind.Double: handler.Write((double)Value); break;
                    case TypeKind.C8: handler.Write(handler.Tell() + 8); handler.WriteAsciiString((string)Value); break;
                    case TypeKind.C16: handler.Write(handler.Tell() + 8); handler.WriteWString((string)Value); break;
                    case TypeKind.String: handler.WriteWString((string)Value); break;
                    case TypeKind.Trigger: break;
                    case TypeKind.Vec2: handler.Write((Vector2)Value); break;
                    case TypeKind.Vec3: handler.Write((Vector3)Value); break;
                    case TypeKind.Vec4: handler.Write((Vector4)Value); break;
                    case TypeKind.Matrix: handler.Write((Matrix4x4)Value); break;
                    case TypeKind.GUID: handler.Write((Guid)Value); break;
                    default:
                        throw new Exception("Unhandled variable type " + Name + " = " + type);
                }
                handler.Align(4);
            }
        }

        public void WriteExpression(FileHandler handler)
        {
            if (Expression == null) return;

            handler.Write(Start + 32, expressionOffset = handler.Tell());
            Expression.Write(handler);
        }
    }


    public class HeaderStruct : BaseModel
    {
        public uint version = 3;
        public uint magic;
        public long stringsOffset;
        public long dataOffset;
        public long embedsInfoOffset;
        public long hashInfoOffset;

        public ulong ukn;
        public uint UVARhash;
        public short variableCount, embedCount;
        public string? name;

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


    public class HashData : BaseModel
    {
        public long guidsOffset;
        public long mapsOffset;
        public long hashOffset;
        public long hashMapOffset;

        public int count;

        public Guid[]? Guids { get; set; }
        public uint[]? GuidMap { get; set; }
        public uint[]? NameHashes { get; set; }
        public uint[]? NameHashMap { get; set; }

        protected override bool DoRead(FileHandler handler)
        {
            handler.Read(ref guidsOffset);
            handler.Read(ref mapsOffset);
            handler.Read(ref hashOffset);
            handler.Read(ref hashMapOffset);

            handler.Seek(guidsOffset);
            Guids = handler.ReadArray<Guid>(count);

            handler.Seek(mapsOffset);
            GuidMap = handler.ReadArray<uint>(count);

            handler.Seek(hashOffset);
            NameHashes = handler.ReadArray<uint>(count);

            handler.Seek(hashMapOffset);
            NameHashMap = handler.ReadArray<uint>(count);
            return true;
        }

        protected override bool DoWrite(FileHandler handler)
        {
            var count = Guids?.Length ?? 0;
            guidsOffset = handler.Tell() + 32;
            mapsOffset = guidsOffset + count * 16;
            hashOffset = mapsOffset + count * 4;
            hashMapOffset = hashOffset + count * 4;

            handler.Write(ref guidsOffset);
            handler.Write(ref mapsOffset);
            handler.Write(ref hashOffset);
            handler.Write(ref hashMapOffset);

            handler.Seek(guidsOffset);
            handler.WriteArray(Guids!);

            handler.Seek(mapsOffset);
            handler.WriteArray(GuidMap!);

            handler.Seek(hashOffset);
            handler.WriteArray(NameHashes!);

            handler.Seek(hashMapOffset);
            handler.WriteArray(NameHashMap!);
            return true;
        }

        public void Rebuild(List<Variable> vars)
        {
            Guids = new Guid[vars.Count];
            GuidMap = new uint[vars.Count];
            NameHashes = new uint[vars.Count];
            NameHashMap = new uint[vars.Count];
            int i = 0;
            foreach (var v in vars.OrderBy(x => x.nameHash)) {
                var index = (uint)vars.IndexOf(v);
                NameHashes[i] = v.nameHash;
                NameHashMap[i] = index;
                i++;
            }
            i = 0;
            foreach (var v in vars.OrderBy(x => x.guid)) {
                var index = (uint)vars.IndexOf(v);
                Guids[i] = v.guid;
                GuidMap[i] = index;
                i++;
            }
        }
    }
}


namespace ReeLib
{
    public class UVarFile(FileHandler fileHandler) : BaseFile(fileHandler)
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
            header.name = handler.ReadWString(header.stringsOffset);
            Variables.Clear();
            Strings.Clear();
            EmbeddedUVARs.Clear();

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
                for (int i = 0; i < header.embedCount; i++)
                {
                    var embedOffset = handler.Read<long>();
                    var pos = handler.Tell();

                    UVarFile embedFile = new(handler.WithOffset(embedOffset));
                    embedFile.Read();
                    EmbeddedUVARs.Add(embedFile);

                    handler.Seek(pos);
                }
            }

            if (header.variableCount > 0 && header.hashInfoOffset > 0)
            {
                handler.Seek(header.hashInfoOffset);
                HashData.Read(handler);
            }

            return true;
        }

        protected override bool DoWrite()
        {
            var handler = FileHandler;
            var header = Header;
            header.magic = Magic;
            header.UVARhash = MurMur3HashUtils.GetHash(header.name ?? string.Empty);
            header.variableCount = (short)Variables.Count;
            header.embedCount = (short)EmbeddedUVARs.Count;
            header.Write(handler);

            handler.Align(16);

            var stringOffsetPos = header.Start + 2 * sizeof(int);
            var dataOffsetPos = stringOffsetPos + sizeof(long);
            var embedOffsetPos = dataOffsetPos + sizeof(long);
            var hashOffsetPos = embedOffsetPos + sizeof(long);

            handler.Write(dataOffsetPos, header.dataOffset = handler.Tell());
            foreach (var v in Variables)
            {
                v.Write(handler);
            }

            foreach (var v in Variables)
            {
                v.WriteValue(handler);
            }

            handler.Align(16);
            foreach (var v in Variables)
            {
                v.WriteExpression(handler);
            }

            handler.Write(stringOffsetPos, header.stringsOffset = handler.Tell());
            handler.WriteWString(header.name ?? string.Empty);
            foreach (var v in Variables)
            {
                handler.Write(v.Start + 16, handler.Tell());
                handler.WriteWString(v.Name ?? string.Empty);
            }
            handler.StringTableFlush();

            if (EmbeddedUVARs.Count > 0)
            {
                handler.Align(16);
                handler.Write(embedOffsetPos, header.embedsInfoOffset = handler.Tell());
                handler.FillBytes(0, EmbeddedUVARs.Count * 8);

                for (var i = 0; i < EmbeddedUVARs.Count; i++)
                {
                    handler.Align(16);
                    var embed = EmbeddedUVARs[i];
                    handler.Write(header.embedsInfoOffset + i * 8, handler.Tell());
                    embed.FileHandler = handler.WithOffset(handler.Tell());
                    embed.Write();
                }
            }

            handler.Align(16);
            handler.Write(hashOffsetPos, header.hashInfoOffset = handler.Tell());
            HashData.Rebuild(Variables);
            HashData.Write(handler);
            return true;
        }
    }
}
