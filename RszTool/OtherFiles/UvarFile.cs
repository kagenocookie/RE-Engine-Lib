using System.Numerics;
using RszTool.Common;

namespace RszTool
{
    public class UvarFile : BaseFile
    {
        private const int Magic = 0x72617675;

        public int version = 3;
        public long stringsOffset;
        public long dataOffset;
        public long embedsInfoOffset;
        public long hashInfoOffset;
        public long unkOffset;
        public int hash;
        public short variableCount;
        public short embedCount;
        public string? name;

        public List<Variable> Variables { get; } = new();
        public List<UvarFile> EmbeddedData { get; } = new();
        public HashData Hashes { get; } = new();

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

        public class Variable : BaseModel
        {
            public Guid guid;
            public string? name;
            public long valueOffset;
            public long expressionsOffset;
            public TypeKind type;
            public UvarFlags flags;
            public uint nameHash;

            public object? value;
            public UvarExpression? expression;

            /// <summary>
            /// For verifying data correctness until someone figures out if there's some other rules at play here
            /// </summary>
            internal bool verifyNextVariableBytes;

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
                guid = handler.Read<Guid>();// handler.Read(ref guid);
                name = handler.ReadOffsetWString();
                handler.Read(ref valueOffset);
                handler.Read(ref expressionsOffset);
                var typeBits = handler.Read<uint>();
                type = (TypeKind)(typeBits & 0xffffff);
                flags = (UvarFlags)(int)((typeBits & 0xff000000) >> 24);
                handler.Read<uint>(ref nameHash);
                var pos = handler.Tell();
                if (expressionsOffset != 0)
                {
                    expression = new UvarExpression();
                    using var _ = handler.SeekJumpBack(expressionsOffset);
                    expression.Read(handler);
                }

                if (valueOffset != 0)
                {
                    var nextValueOffset = verifyNextVariableBytes ? (long)handler.Read<ulong>(Start + 48 + 24) : 0;
                    var expectedBytes = nextValueOffset != 0 ? nextValueOffset - valueOffset : 0;
                    handler.Seek(valueOffset);
                    if ((flags & UvarFlags.IsVec3) != 0)
                    {
                        if (expectedBytes != 0 && expectedBytes != 12)
                        {
                            throw new Exception($"Likely UVAR read issue, {type} value {name} is expecting {expectedBytes} bytes, we are expecting 12");
                        }

                        switch (type) {
                            case TypeKind.Int8: value = handler.ReadArray<sbyte>(3); break;
                            case TypeKind.Uint8: value = handler.ReadArray<byte>(3); break;
                            case TypeKind.Int16: value = handler.ReadArray<short>(3); break;
                            case TypeKind.Uint16: value = handler.ReadArray<ushort>(3); break;
                            case TypeKind.Int32: value = handler.ReadArray<int>(3); break;
                            case TypeKind.Uint32: value = handler.ReadArray<uint>(3); break;
                            case TypeKind.Single: value = handler.Read<Vector3>(); break;
                            default:
                                throw new Exception($"Unhandled vec3 variable {name} type " + name + " = " + type);
                        }
                    }
                    else
                    {
                        if (nextValueOffset != 0 && expectedBytes != 4 && type is not TypeKind.String and not TypeKind.C8 and not TypeKind.C16 and not TypeKind.Matrix and not TypeKind.GUID)
                        {
                            throw new Exception($"Likely UVAR read issue - {type} value {name} is expecting {expectedBytes} bytes");
                        }
                        switch (type) {
                            case TypeKind.Enum: value = handler.Read<int>(); break;
                            case TypeKind.Boolean: value = handler.Read<bool>(); break;
                            case TypeKind.Int8: value = handler.Read<sbyte>(); break;
                            case TypeKind.Uint8: value = handler.Read<byte>(); break;
                            case TypeKind.Int16: value = handler.Read<short>(); break;
                            case TypeKind.Uint16: value = handler.Read<ushort>(); break;
                            case TypeKind.Int32: value = handler.Read<int>(); break;
                            case TypeKind.Uint32: value = handler.Read<uint>(); break;
                            case TypeKind.Single: value = handler.Read<float>(); break;
                            case TypeKind.Double: value = handler.Read<double>(); break;
                            case TypeKind.C8: value = handler.ReadOffsetAsciiString(); break;
                            case TypeKind.C16: value = handler.ReadOffsetWString(); break;
                            case TypeKind.String: value = handler.ReadWString(); break;
                            case TypeKind.Trigger: break;
                            case TypeKind.Vec2: value = handler.Read<Vector2>(); break;
                            case TypeKind.Vec3: value = handler.Read<Vector3>(); break;
                            case TypeKind.Vec4: value = handler.Read<Vector4>(); break;
                            case TypeKind.Matrix: value = handler.Read<via.mat4>(); break;
                            case TypeKind.GUID: value = handler.Read<Guid>(); break;
                            default:
                                throw new Exception("Unhandled variable type " + name + " = " + type);
                        }
                    }
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
                handler.Write<uint>(nameHash = MurMur3HashUtils.GetHash(name ?? string.Empty));
                return true;
            }

            public void WriteValue(FileHandler handler)
            {
                if (value != null)
                {
                    handler.Write(Start + 24, valueOffset = handler.Tell());
                    if ((flags & UvarFlags.IsVec3) != 0) {
                        switch (type) {
                            case TypeKind.Int8: handler.WriteArray((sbyte[])value); break;
                            case TypeKind.Uint8: handler.WriteArray((byte[])value); break;
                            case TypeKind.Int16: handler.WriteArray((short[])value); break;
                            case TypeKind.Uint16: handler.WriteArray((ushort[])value); break;
                            case TypeKind.Int32: handler.WriteArray((int[])value); break;
                            case TypeKind.Uint32: handler.WriteArray((uint[])value); break;
                            case TypeKind.Single: handler.Write((Vector3)value); break;
                            default:
                                throw new Exception("Unhandled vec3 variable type " + name + " = " + type);
                        }
                        return;
                    }
                    switch (type) {
                        case TypeKind.Enum: handler.Write((int)value); break;
                        case TypeKind.Boolean: handler.Write((bool)value); break;
                        case TypeKind.Int8: handler.Write((sbyte)value); break;
                        case TypeKind.Uint8: handler.Write((byte)value); break;
                        case TypeKind.Int16: handler.Write((short)value); break;
                        case TypeKind.Uint16: handler.Write((ushort)value); break;
                        case TypeKind.Int32: handler.Write((int)value); break;
                        case TypeKind.Uint32: handler.Write((uint)value); break;
                        case TypeKind.Single:
                             // TODO: sometimes it seems to be 3 float and not just one?s
                            if ((flags & UvarFlags.IsVec3) != 0) handler.Write((Vector3)value);
                            else handler.Write((float)value);
                            break;
                        case TypeKind.Double: handler.Write((double)value); break;
                        case TypeKind.C8: handler.Write(handler.Tell() + 8); handler.WriteAsciiString((string)value); break;
                        case TypeKind.C16: handler.Write(handler.Tell() + 8); handler.WriteWString((string)value); break;
                        case TypeKind.String: handler.WriteWString((string)value); break;
                        case TypeKind.Trigger: break;
                        case TypeKind.Vec2: handler.Write((Vector2)value); break;
                        case TypeKind.Vec3: handler.Write((Vector3)value); break;
                        case TypeKind.Vec4: handler.Write((Vector4)value); break;
                        case TypeKind.Matrix: handler.Write((Matrix4x4)value); break;
                        case TypeKind.GUID: handler.Write((Guid)value); break;
                        default:
                            throw new Exception("Unhandled variable type " + name + " = " + type);
                    }
                    handler.Align(4);
                }
            }
            public void WriteExpression(FileHandler handler)
            {
                if (expression == null) return;

                handler.Write(Start + 32, expressionsOffset = handler.Tell());
                expression.Write(handler);
            }

            public override string ToString() => (name ?? guid.ToString()) + " = " + (value ?? "NULL");
        }

        public class UvarExpression : BaseModel
        {
            public long nodesOffset;
            public long relationsOffset;
            public short nodeCount;
            public short outputNodeId;
            public short unknownCount;

            public List<Node> Nodes { get; } = new();
            public List<NodeConnection> Connections { get; } = new();

            public struct NodeConnection
            {
                public short nodeId;
                public short inputSlot;
                public short node2;
                public short outputSlot;
            }

            // note: enum is pure guesswork
            public enum NodeValueType : int
            {
                Unknown = 0,
                UInt32Maybe = 6,
                Int32 = 7, // can also represent enums for LogicNode (LogicOperatorType enum?)
                Single = 10,
                Guid = 20,
            }

            public class Node : BaseModel
            {
                public long nameOffset;
                public long dataOffset;
                public long uknOffset;
                public short nodeId;
                public short valueCount;
                public int uknCount;

                public string? name;
                public List<NodeParameter> parameters = new(1);

                public class NodeParameter : BaseModel
                {
                    public uint nameHash;
                    public NodeValueType type;
                    public object? value;

                    protected override bool DoRead(FileHandler handler)
                    {
                        handler.Read(ref nameHash);
                        handler.Read(ref type);
                        switch (type) {
                            case NodeValueType.UInt32Maybe: value = handler.Read<uint>(); break;
                            case NodeValueType.Int32: value = handler.Read<int>(); break;
                            case NodeValueType.Single: value = handler.Read<float>(); break;
                            case NodeValueType.Guid:
                                handler.Seek(handler.Read<long>());
                                value = handler.Read<Guid>(); break;
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

                protected override bool DoRead(FileHandler handler)
                {
                    handler.Read(ref nameOffset);
                    handler.Read(ref dataOffset);
                    handler.Read(ref uknOffset);
                    handler.Read(ref nodeId);
                    handler.Read(ref valueCount);
                    handler.Read(ref uknCount);
                    var tell = handler.Tell();
                    name = handler.ReadAsciiString(nameOffset);
                    if (dataOffset != 0 && valueCount > 0)
                    {
                        handler.Seek(dataOffset);
                        parameters.Read(handler, valueCount);
                    }
                    handler.Seek(tell);
                    return true;
                }

                protected override bool DoWrite(FileHandler handler)
                {
                    valueCount = (short)parameters.Count;
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
                    handler.WriteAsciiString(name ?? string.Empty);
                    handler.Align(16);
                    if (valueCount > 0)
                    {
                        handler.Write(Start + 8, dataOffset = handler.Tell());
                        parameters.Write(handler);
                    }
                    handler.Align(16);
                }
            }

            protected override bool DoRead(FileHandler handler)
            {
                handler.Read(ref nodesOffset);
                handler.Read(ref relationsOffset);
                handler.Read(ref nodeCount);
                handler.Read(ref outputNodeId);
                handler.Read(ref unknownCount);
                if (nodesOffset != 0)
                {
                    using var _ = handler.SeekJumpBack(nodesOffset);
                    Nodes.Read(handler, nodeCount);
                }

                if (relationsOffset != 0)
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

        public class HashData : BaseModel
        {
            public long guidsOffset;
            public long mapsOffset;
            public long hashOffset;
            public long hashMapOffset;

            public int count;

            public Guid[] Guids { get; set; } = Array.Empty<Guid>();
            public int[] GuidMaps { get; set; } = Array.Empty<int>();
            public uint[] Hashes { get; set; } = Array.Empty<uint>();
            public int[] HashMaps { get; set; } = Array.Empty<int>();

            protected override bool DoRead(FileHandler handler)
            {
                handler.Read(ref guidsOffset);
                handler.Read(ref mapsOffset);
                handler.Read(ref hashOffset);
                handler.Read(ref hashMapOffset);

                handler.Seek(guidsOffset);
                handler.ReadArray(Guids);

                handler.Seek(mapsOffset);
                handler.ReadArray(GuidMaps);

                handler.Seek(hashOffset);
                handler.ReadArray(Hashes);

                handler.Seek(hashMapOffset);
                handler.ReadArray(HashMaps);
                return true;
            }

            protected override bool DoWrite(FileHandler handler)
            {
                guidsOffset = handler.Tell() + 32;
                mapsOffset = guidsOffset + count * 16;
                hashOffset = mapsOffset + count * 4;
                hashMapOffset = hashOffset + count * 4;

                handler.Write(ref guidsOffset);
                handler.Write(ref mapsOffset);
                handler.Write(ref hashOffset);
                handler.Write(ref hashMapOffset);

                handler.Seek(guidsOffset);
                handler.WriteArray(Guids);

                handler.Seek(mapsOffset);
                handler.WriteArray(GuidMaps);

                handler.Seek(hashOffset);
                handler.WriteArray(Hashes);

                handler.Seek(hashMapOffset);
                handler.WriteArray(HashMaps);
                return true;
            }

            public void Rebuild(List<Variable> vars)
            {
                count = vars.Count;
                Guids = new Guid[count];
                GuidMaps = new int[count];
                Hashes = new uint[count];
                HashMaps = new int[count];
                int i = 0;
                foreach (var v in vars.OrderBy(x => x.nameHash)) {
                    var index = vars.IndexOf(v);
                    Hashes[i] = v.nameHash;
                    HashMaps[i] = index;
                    i++;
                }
                i = 0;
                foreach (var v in vars.OrderBy(x => x.guid)) {
                    var index = vars.IndexOf(v);
                    Guids[i] = v.guid;
                    GuidMaps[i] = index;
                    i++;
                }
            }
        }

        public UvarFile(FileHandler fileHandler) : base(fileHandler)
        {
        }

        protected override bool DoRead()
        {
            var handler = FileHandler;
            handler.Read(ref version);
            handler.Read<int>();
            handler.Read(ref stringsOffset);
            handler.Read(ref dataOffset);
            handler.Read(ref embedsInfoOffset);
            handler.Read(ref hashInfoOffset);
            if (handler.FileVersion < 3)
            {
                handler.Read(ref unkOffset);
            }
            handler.Read(ref hash);
            handler.Read(ref variableCount);
            handler.Read(ref embedCount);

            name = handler.ReadWString(stringsOffset);

            handler.Seek(dataOffset);
            for (int i = 0; i < variableCount; ++i)
            {
                var v = new Variable();
                v.verifyNextVariableBytes = i + 1 < variableCount;
                v.Read(handler);
                Variables.Add(v);
            }

            handler.Seek(embedsInfoOffset);
            for (int i = 0; i < embedCount; ++i)
            {
                var embedOffset = handler.Read<long>();
                var tell = handler.Tell();

                var embed = new UvarFile(handler.WithOffset(embedOffset));
                embed.Read();
                EmbeddedData.Add(embed);

                handler.Seek(tell);
            }

            // do we care to read these at all?
            // handler.Seek(hashInfoOffset);
            // Hashes.Read(handler);

            return true;
        }

        protected override bool DoWrite()
        {
            var handler = FileHandler;
            handler.Write(ref version);
            handler.Write(Magic);
            var stringOffsetPos = handler.Tell();
            handler.Write(ref stringsOffset);
            var dataOffsetPos = handler.Tell();
            handler.Write(ref dataOffset);
            var embedOffsetPos = handler.Tell();
            handler.Write(ref embedsInfoOffset);
            var hashOffsetPos = handler.Tell();
            handler.Write(ref hashInfoOffset);
            if (handler.FileVersion < 3)
            {
                handler.Write(ref unkOffset);
            }
            handler.Write(ref hash);
            handler.Write(variableCount = (short)Variables.Count);
            handler.Write(embedCount = (short)EmbeddedData.Count);
            handler.Align(16);

            handler.Write(dataOffsetPos, dataOffset = handler.Tell());
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

            handler.Write(stringOffsetPos, stringsOffset = handler.Tell());
            handler.WriteWString(name ?? string.Empty);
            foreach (var v in Variables)
            {
                handler.Write(v.Start + 16, handler.Tell());
                handler.WriteWString(v.name ?? string.Empty);
            }
            handler.StringTableFlush();

            if (EmbeddedData.Count > 0)
            {
                handler.Align(16);
                handler.Write(embedOffsetPos, embedsInfoOffset = handler.Tell());
                handler.FillBytes(0, EmbeddedData.Count * 8);

                for (var i = 0; i < EmbeddedData.Count; i++)
                {
                    handler.Align(16);
                    var embed = EmbeddedData[i];
                    handler.Write(embedsInfoOffset + i * 8, handler.Tell());
                    embed.FileHandler = handler.WithOffset(handler.Tell());
                    embed.Write();
                }
            }

            handler.Align(16);
            handler.Write(hashOffsetPos, hashInfoOffset = handler.Tell());
            Hashes.Rebuild(Variables);
            Hashes.Write(handler);

            return true;
        }

    }
}