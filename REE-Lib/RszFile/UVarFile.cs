using System.Numerics;
using System.Runtime.InteropServices;
using ReeLib.Common;
using ReeLib.UVar;
using ReeLib.via;

namespace ReeLib.UVar
{
    public class NodeParameter : BaseModel
    {
        /// <summary>
        /// ASCII hash of the parameter name.
        /// </summary>
        public uint nameHash;
        public NodeValueType type;
        public object? value;

        public NodeParameter()
        {
        }

        public NodeParameter(uint nameHash, NodeValueType type)
        {
            this.nameHash = nameHash;
            this.type = type;
            ResetValue();
        }

        /// <summary>
        /// A nullable reference to a runtime object instance. Used for cases where parameters refer to objects outside of the UVar file (e.g. motfsm2 expression trees)
        /// </summary>
        public object? ReferenceObject { get; set; }

        public static string GetParameterName(uint nameHash) => nameHash switch {
            ParameterNameHash.Operation => nameof(ParameterNameHash.Operation),
            ParameterNameHash.Value => nameof(ParameterNameHash.Value),
            ParameterNameHash.CallbackGuid => nameof(ParameterNameHash.CallbackGuid),
            ParameterNameHash.CallbackID => nameof(ParameterNameHash.CallbackID),
            ParameterNameHash.CallbackFunctionID => nameof(ParameterNameHash.CallbackFunctionID),
            ParameterNameHash.VariableID => nameof(ParameterNameHash.VariableID),
            ParameterNameHash.Count => nameof(ParameterNameHash.Count),
            ParameterNameHash.Min => nameof(ParameterNameHash.Min),
            ParameterNameHash.Max => nameof(ParameterNameHash.Max),
            _ => nameHash.ToString(),
        };

        public static class ParameterNameHash
        {
            public const uint Operation = 3248822590;
            public const uint Value = 4253840158;
            public const uint Count = 2243404683;
            public const uint VariableID = 3595450026;
            public const uint CallbackGuid = 2711667410;
            public const uint CallbackID = 1916175859;
            public const uint CallbackFunctionID = 2855057449;
            public const uint Min = 3730297036;
            public const uint Max = 3365636864;
        }

        public enum NodeValueType : int
        {
            Unknown = 0,
            UInt32 = 6,
            Int32 = 7,
            Single = 10,
            Guid = 20,
        }

        public void ResetValue()
        {
            value = type switch {
                NodeValueType.Int32 => 0,
                NodeValueType.UInt32 => 0u,
                NodeValueType.Single => 0f,
                NodeValueType.Guid => new Guid(),
                _ => 0,
            };
        }

        public override string ToString() => $"[{GetParameterName(nameHash)}] = {value ?? "NULL"}";

        protected override bool DoRead(FileHandler handler)
        {
            handler.Read(ref nameHash);
            DataInterpretationException.DebugWarnIf(char.IsDigit(GetParameterName(nameHash)[0]), "Unknown Uvar node hash " + nameHash);
            handler.Read(ref type);
            switch (type)
            {
                case NodeValueType.UInt32:
                    value = handler.Read<uint>();
                    break;
                case NodeValueType.Int32:
                    value = handler.Read<int>();
                    break;
                case NodeValueType.Single:
                    value = handler.Read<float>();
                    break;
                case NodeValueType.Guid:
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
                case NodeValueType.UInt32: handler.Write((uint?)value ?? default); break;
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
        public short nodeId;

        public string Name { get; set; } = string.Empty;
        public List<NodeParameter> Parameters = new(1);

        public NodeParameterInfo[] AllowedParameters => GetParametersForNode(Name);

        public NodeParameter? GetParameter(uint paramHash)
        {
            foreach (var p in Parameters) {
                if (p.nameHash == paramHash) return p;
            }
            return null;
        }

        public NodeParameterInfo? GetParameterInfo(uint paramHash)
        {
            foreach (var p in AllowedParameters) {
                if (p.nameHash == paramHash) return p;
            }
            return null;
        }

        public string[] GetNodeInputs()
        {
            if (Name == "MultiLogicNode") {
                var count = GetParameter(NodeParameter.ParameterNameHash.Count)?.value;
                if (count is not uint n || n < 0) n = 0u;
                if (!NumberedInputLists.TryGetValue(n, out var list)) {
                    NumberedInputLists[n] = list = Enumerable.Range(0, (int)n).Select(num => "Arg" + num).ToArray();
                }
                return list;
            }
            return NodeInfos.GetValueOrDefault(Name)?.inputs ?? [];
        }
        private static readonly Dictionary<uint, string[]> NumberedInputLists = new();

        public record class NodeParameterInfo(uint nameHash, NodeParameter.NodeValueType valueType)
        {
            public string Name => NodeParameter.GetParameterName(nameHash);
            public NodeParameter Instantiate() => new NodeParameter(nameHash, valueType);
        }

        private record class NodeTypeInfo(string[] inputs, params NodeParameterInfo[] parameters);
        private static readonly Dictionary<string, NodeTypeInfo> NodeInfos = new()
        {
            { "VariableReferenceNode", new NodeTypeInfo([],
                new NodeParameterInfo(NodeParameter.ParameterNameHash.VariableID, NodeParameter.NodeValueType.Guid)) },
            { "ValueNode", new NodeTypeInfo([],
                new NodeParameterInfo(NodeParameter.ParameterNameHash.Value, NodeParameter.NodeValueType.Single)) },
            { "NotNode", new NodeTypeInfo(["Input"]) },
            { "AbsNode", new NodeTypeInfo(["Input"]) },
            { "CompareNode", new NodeTypeInfo(["Arg1", "Arg2"],
                new NodeParameterInfo(NodeParameter.ParameterNameHash.Operation, NodeParameter.NodeValueType.Int32)) },
            { "LogicNode", new NodeTypeInfo(["Arg1", "Arg2"],
                new NodeParameterInfo(NodeParameter.ParameterNameHash.Operation, NodeParameter.NodeValueType.Int32)) },
            { "MultiLogicNode", new NodeTypeInfo([],
                new NodeParameterInfo(NodeParameter.ParameterNameHash.Count, NodeParameter.NodeValueType.UInt32),
                new NodeParameterInfo(NodeParameter.ParameterNameHash.Operation, NodeParameter.NodeValueType.Int32)
            ) },
            { "CalculateNode", new NodeTypeInfo(["Arg1", "Arg2"],
                new NodeParameterInfo(NodeParameter.ParameterNameHash.Operation, NodeParameter.NodeValueType.Int32)) },
            { "ClampNode", new NodeTypeInfo(["Value"],
                new NodeParameterInfo(NodeParameter.ParameterNameHash.Min, NodeParameter.NodeValueType.Single),
                new NodeParameterInfo(NodeParameter.ParameterNameHash.Max, NodeParameter.NodeValueType.Single)
            ) },
            { "LerpNode", new NodeTypeInfo(["Value", "Start", "End"]) },
            { "CallbackNode", new NodeTypeInfo([],
                new NodeParameterInfo(NodeParameter.ParameterNameHash.CallbackGuid, NodeParameter.NodeValueType.Guid),
                new NodeParameterInfo(NodeParameter.ParameterNameHash.CallbackID, NodeParameter.NodeValueType.UInt32),
                new NodeParameterInfo(NodeParameter.ParameterNameHash.CallbackFunctionID, NodeParameter.NodeValueType.Int32)
            ) },
        };
        public static readonly string[] NodeTypes = NodeInfos.Keys.ToArray();

        public static NodeParameterInfo[] GetParametersForNode(string nodeType) => NodeInfos.GetValueOrDefault(nodeType)?.parameters ?? [];
        public static UvarNode Create(string type)
        {
            if (!NodeInfos.TryGetValue(type, out var info)) {
                throw new Exception("Unknown UVAR node type " + type);
            }

            var node = new UvarNode() {
                Name = type,
                Parameters = info.parameters.Select(p => p.Instantiate()).ToList()
            };
            return node;
        }

        protected override bool DoRead(FileHandler handler)
        {
            var nameOffset = handler.Read<long>();
            var dataOffset = handler.Read<long>();
            handler.ReadNull(8);
            handler.Read(ref nodeId);
            var paramCount = handler.Read<short>();
            handler.ReadNull(4);
            Name = handler.ReadAsciiString(nameOffset);
            if (paramCount > 0)
            {
                using var jumpBack = handler.SeekJumpBack(dataOffset);
                Parameters.Read(handler, paramCount);
            }
            DataInterpretationException.DebugWarnIf(!NodeInfos.ContainsKey(Name), Name);
            DataInterpretationException.DebugWarnIf(Parameters.Any(p => GetParameterInfo(p.nameHash)?.valueType != p.type) == true, Name);
            return true;
        }

        protected override bool DoWrite(FileHandler handler)
        {
            handler.WriteNull(24); // nameOffset, dataOffset, 8padding
            handler.Write(ref nodeId);
            handler.Write((short)Parameters.Count);
            handler.WriteNull(4);
            return true;
        }

        public void FlushData(FileHandler handler)
        {
            handler.Write(Start, handler.Tell());
            handler.WriteAsciiString(Name ?? string.Empty);
            handler.Align(16);
            if (Parameters.Count > 0)
            {
                handler.Write(Start + 8, handler.Tell());
                Parameters.Write(handler);
            }
            handler.Align(16);
        }

        public override string ToString() => $"[{nodeId}] {Name}";
    }


    public class UvarExpression : BaseModel
    {
        public short outputNodeId;
        public List<UvarNode> Nodes { get; set; } = new();
        public List<NodeConnection> Connections { get; set; } = new();

        public struct NodeConnection
        {
            public short targetId;
            public short inputSlot;
            public short sourceId;
            public short outputSlot;

            public override string ToString() => $"{sourceId} => {targetId}";
        }

        protected override bool DoRead(FileHandler handler)
        {
            Connections.Clear();
            var nodesOffset = handler.Read<long>();
            var relationsOffset = handler.Read<long>();
            var nodeCount = handler.Read<short>();
            var connectionCount = handler.Read<short>();
            handler.Read(ref outputNodeId);
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
                    using var _ = handler.SeekJumpBack(relationsOffset);
                    Connections.ReadStructList(handler, connectionCount);
                }
            }
            return true;
        }

        protected override bool DoWrite(FileHandler handler)
        {
            handler.Skip(16); // nodesOffset, relationsOffset
            handler.Write((short)Nodes.Count);
            handler.Write((short)Connections.Count);
            handler.Write(ref outputNodeId);

            handler.Align(16);
            handler.Write(Start, handler.Tell());
            Nodes.Write(handler);

            foreach (var node in Nodes)
            {
                node.FlushData(handler);
            }

            handler.Write(Start + 8, handler.Tell());
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

        public bool IsVec3 => (flags & UvarFlags.IsVec3) != 0;

        public object? Value;
        public UvarExpression? Expression { get; set; }
        public Type? ValueType => GetValueType(type, flags);
        public Type? RszValueType => GetValueType(type, flags);

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

        public static Type? GetValueType(TypeKind type, UvarFlags flags)
        {
            switch (type) {
                case TypeKind.C8: return typeof(string);
                case TypeKind.C16: return typeof(string);
                case TypeKind.String: return typeof(string);
                case TypeKind.Trigger: return (flags & UvarFlags.IsVec3) == 0 ? null : throw new Exception($"Unhandled uvar vec3 variable type " + type);
                default:
                    var vt = GetValueRszType(type, flags);
                    var baseType = RszInstance.RszFieldTypeToCSharpType(vt.type);
                    if (vt.array) {
                        return baseType.MakeArrayType();
                    }
                    return baseType;
            }
        }

        public static (RszFieldType type, bool array) GetValueRszType(TypeKind type, UvarFlags flags)
        {
            if ((flags & UvarFlags.IsVec3) != 0) {
                switch (type) {
                    case TypeKind.Int8: return (RszFieldType.S8, true);
                    case TypeKind.Uint8: return (RszFieldType.U8, true);
                    case TypeKind.Int16: return (RszFieldType.S16, true);
                    case TypeKind.Uint16: return (RszFieldType.U16, true);
                    case TypeKind.Int32: return (RszFieldType.Int3, false);
                    case TypeKind.Uint32: return (RszFieldType.Uint3, false);
                    case TypeKind.Int64: return (RszFieldType.S64, true);
                    case TypeKind.Single: return (RszFieldType.Vec3, false);
                    case TypeKind.Double: return (RszFieldType.Position, false);
                    default:
                        throw new Exception($"Unhandled uvar vec3 variable type " + type);
                }
            }
            switch (type) {
                case TypeKind.Enum: return (RszFieldType.S32, false);
                case TypeKind.Boolean: return (RszFieldType.Bool, false);
                case TypeKind.Int8: return (RszFieldType.S8, false);
                case TypeKind.Uint8: return (RszFieldType.U8, false);
                case TypeKind.Int16: return (RszFieldType.S16, false);
                case TypeKind.Uint16: return (RszFieldType.U16, false);
                case TypeKind.Int32: return (RszFieldType.S32, false);
                case TypeKind.Uint32: return (RszFieldType.U32, false);
                case TypeKind.Single: return (RszFieldType.F32, false);
                case TypeKind.Double: return (RszFieldType.F64, false);
                case TypeKind.C8: return (RszFieldType.String, false);
                case TypeKind.C16: return (RszFieldType.String, false);
                case TypeKind.String: return (RszFieldType.String, false);
                case TypeKind.Trigger: return (RszFieldType.Undefined, false);
                case TypeKind.Vec2: return (RszFieldType.Vec2, false);
                case TypeKind.Vec3: return (RszFieldType.Vec3, false);
                case TypeKind.Vec4: return (RszFieldType.Vec4, false);
                case TypeKind.Matrix: return (RszFieldType.Mat4, false);
                case TypeKind.GUID: return (RszFieldType.Guid, false);
                default:
                    throw new Exception($"Unhandled uvar variable type " + type);
            }
        }

        public void ResetValue()
        {
            var varType = GetValueType(type, flags);
            if (varType == null) {
                Value = null;
                return;
            }

            if (varType.IsArray) {
                Value = Array.CreateInstance(varType.GetElementType()!, 3);
                return;
            }

            if (varType == typeof(string)) {
                Value = string.Empty;
                return;
            }

            Value = Activator.CreateInstance(varType);
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
                if (IsVec3)
                {
                    switch (type) {
                        case TypeKind.Int8: Value = handler.ReadArray<sbyte>(3); break;
                        case TypeKind.Uint8: Value = handler.ReadArray<byte>(3); break;
                        case TypeKind.Int16: Value = handler.ReadArray<short>(3); break;
                        case TypeKind.Uint16: Value = handler.ReadArray<ushort>(3); break;
                        case TypeKind.Int32: Value = handler.Read<Int3>(); break;
                        case TypeKind.Uint32: Value = handler.Read<Uint3>(); break;
                        case TypeKind.Single: Value = handler.Read<Vector3>(); break;
                        case TypeKind.Double: Value = handler.Read<Position>(); break;
                        default:
                            throw new Exception($"Unhandled vec3 variable type " + Name + " = " + type);
                    }
                }
                else
                {
                    switch (type) {
                        case TypeKind.C8: Value = handler.ReadOffsetAsciiString(); break;
                        case TypeKind.C16: Value = handler.ReadOffsetWString(); break;
                        case TypeKind.String: Value = handler.ReadWString(); break;
                        case TypeKind.Trigger: break;
                        default:
                            Value = handler.ReadObject(ValueType!);
                            break;
                    }
                }
            }
            if (expressionOffset > 0) {
                Expression ??= new();
                Expression.Read(handler, expressionOffset);
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
                if (IsVec3) {
                    switch (type) {
                        case TypeKind.Int8: handler.WriteArray((sbyte[])Value); break;
                        case TypeKind.Uint8: handler.WriteArray((byte[])Value); break;
                        case TypeKind.Int16: handler.WriteArray((short[])Value); break;
                        case TypeKind.Uint16: handler.WriteArray((ushort[])Value); break;
                        case TypeKind.Int32: handler.Write((Int3)Value); break;
                        case TypeKind.Uint32: handler.Write((Uint3)Value); break;
                        case TypeKind.Single: handler.Write((Vector3)Value); break;
                        default:
                            throw new Exception("Unhandled vec3 variable type " + Name + " = " + type);
                    }
                    return;
                }
                switch (type) {
                    case TypeKind.C8: handler.Write(handler.Tell() + 8); handler.WriteAsciiString((string)Value); break;
                    case TypeKind.C16: handler.Write(handler.Tell() + 8); handler.WriteWString((string)Value); break;
                    case TypeKind.String: handler.WriteWString((string)Value); break;
                    case TypeKind.Trigger: break;
                    default:
                        handler.WriteObject(Value);
                        break;
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

        public override string ToString() => $"{Name} = {(Value?.GetType().IsArray == true ? string.Join(", ", ((Array)Value).Cast<object>()) : Value?.ToString() ?? "No Value")}";
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
        public Guid[] Guids { get; set; } = [];
        public uint[] GuidMap { get; set; } = [];
        public uint[] NameHashes { get; set; } = [];
        public uint[] NameHashMap { get; set; } = [];

        public int Count
        {
            get => Guids.Length;
            set {
                if (value != Guids.Length) {
                    var arr = Guids; Array.Resize(ref arr, value); Guids = arr;
                    var arr2 = GuidMap; Array.Resize(ref arr2, value); GuidMap = arr2;
                    arr2 = NameHashes; Array.Resize(ref arr2, value); NameHashes = arr2;
                    arr2 = NameHashMap; Array.Resize(ref arr2, value); NameHashMap = arr2;
                }
            }
        }

        protected override bool DoRead(FileHandler handler)
        {
            var guidsOffset = handler.Read<long>();
            var mapsOffset = handler.Read<long>();
            var hashOffset = handler.Read<long>();
            var hashMapOffset = handler.Read<long>();

            handler.Seek(guidsOffset);
            handler.ReadArray<Guid>(Guids);

            handler.Seek(mapsOffset);
            handler.ReadArray<uint>(GuidMap);

            handler.Seek(hashOffset);
            handler.ReadArray<uint>(NameHashes);

            handler.Seek(hashMapOffset);
            handler.ReadArray<uint>(NameHashMap);
            return true;
        }

        protected override bool DoWrite(FileHandler handler)
        {
            var count = Guids.Length;
            var guidsOffset = handler.Tell() + 32;
            var mapsOffset = guidsOffset + count * 16;
            var hashOffset = mapsOffset + count * 4;
            var hashMapOffset = hashOffset + count * 4;

            handler.Write(ref guidsOffset);
            handler.Write(ref mapsOffset);
            handler.Write(ref hashOffset);
            handler.Write(ref hashMapOffset);

            handler.Seek(guidsOffset);
            handler.WriteArray(Guids);

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
            header.name = header.stringsOffset > 0 ? handler.ReadWString(header.stringsOffset) : "";
            Variables.Clear();
            EmbeddedUVARs.Clear();

            if (header.variableCount > 0 && header.dataOffset > 0)
            {
                handler.Seek(header.dataOffset);
                Variables.Read(handler, header.variableCount);
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
            header.UVARhash = string.IsNullOrEmpty(header.name) ? 0 : MurMur3HashUtils.GetHash(header.name ?? string.Empty);
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

        public override string ToString() => Header.name ?? "UVarFile";
    }
}
