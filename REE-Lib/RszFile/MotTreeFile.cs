using System.Numerics;
using ReeLib.Common;
using ReeLib.InternalAttributes;
using ReeLib.MotTree;

namespace ReeLib.MotTree
{
    public enum MotTreeVersion
    {
        RE2 = 4,
        RE3 = 5,
        RE8 = 10,
        RE_RT = 13,
        MHR = 14,
        RE4 = 19,
        DD2 = 20,
        Pragmata = 22,
    }

    public enum MotionTreeParamType
    {
        Bool = 0x0,
        Int8 = 0x1,
        UInt8 = 0x2,
        Int16 = 0x3,
        UInt16 = 0x4,
        Int32 = 0x5,
        UInt32 = 0x6,
        Int64 = 0x7,
        Uint64 = 0x8,
        Float = 0x9,
        Double = 0xA,
        StrAscii = 0xB,
        StrUTF16 = 0xC,
        ExtraData = 0xD,
        Hermite = 0xE,
        Guid = 0xF,
        Vec2 = 0x10,
        Vec3 = 0x11,
        Vec4 = 0x12,
        Matrix = 0x13,
    }

    public enum MotionTreeLinkType
    {
        Unknown = 0,
        Motion = 1,
        Param = 2,
    }

    public class MotionTreeNodeParameter
    {
        public string? name;

        public MotionTreeParamType type;
        public uint hash;
        public object? value;

        public MotionTreeNodeParameter()
        {
        }

        public MotionTreeNodeParameter(string name)
        {
            this.name = name;
            hash = MurMur3HashUtils.GetHash(name);
        }

        internal void Read(FileHandler handler, MotTreeVersion version)
        {
            if (version <= MotTreeVersion.RE8) {
                name = handler.ReadOffsetAsciiString();
            }
            handler.Read(ref type);
            handler.Read(ref hash);
            ReadValue(handler, handler.Read<long>());
            if (version > MotTreeVersion.RE8) {
                name = MotionTreeNode.GetParameterName(hash);
            }
            // note: some re7 files have hash == 0, hence the extra check here
            if (hash != 0 && MotionTreeNode.GetParameterName(hash) == null && type != MotionTreeParamType.ExtraData) {
                Log.Warn("Unknown motree parameter name: " + hash + " " + name);
            }
        }

        internal void Write(FileHandler handler, MotTreeVersion version)
        {
            if (version <= MotTreeVersion.RE8) {
                handler.WriteOffsetAsciiString(name ?? "");
            }
            handler.Write(ref type);
            handler.Write(ref hash);
            WriteValue(handler);
            // handler.WriteOffsetContent(hh => WriteValue(hh));
        }

        internal void ReadValue(FileHandler handler, long valueOrOffset)
        {
            switch (type)
            {
                case MotionTreeParamType.Bool:
                    value = valueOrOffset != 0;
                    break;
                case MotionTreeParamType.Int32:
                    value = (int)valueOrOffset;
                    break;
                case MotionTreeParamType.UInt32:
                    value = (uint)valueOrOffset;
                    break;
                case MotionTreeParamType.Float:
                    value = BitConverter.Int32BitsToSingle((int)valueOrOffset);
                    break;
                case MotionTreeParamType.StrUTF16:
                    value = handler.ReadWString(valueOrOffset);
                    break;
                case MotionTreeParamType.ExtraData:
                    {
                        using var _ = handler.SeekJumpBack(valueOrOffset);
                        value = handler.ReadArray<byte>((int)hash);
                    }
                    break;
                default:
                    throw new NotImplementedException("Unsupported motree param type " + type);
            }
        }

        internal void WriteValue(FileHandler handler)
        {
            switch (type)
            {
                case MotionTreeParamType.Bool:
                    handler.Write((bool)value!);
                    break;
                case MotionTreeParamType.Int32:
                    handler.Write((int)value!);
                    break;
                case MotionTreeParamType.UInt32:
                    handler.Write((uint)value!);
                    break;
                case MotionTreeParamType.Float:
                    handler.Write((float)value!);
                    break;
                case MotionTreeParamType.StrUTF16:
                    handler.WriteOffsetWString((string?)value ?? "");
                    break;
                case MotionTreeParamType.ExtraData:
                    {
                        var buffer = value as byte[];
                        if (value is IModel model) {
                            var wr = new FileHandler(new MemoryStream());
                            model.Write(wr);
                            buffer = ((MemoryStream)wr.Stream).GetBuffer().AsSpan(0, (int)wr.Stream.Length).ToArray();
                        }
                        if (buffer == null) {
                            throw new NotImplementedException("Invalid motion tree value type " + (value?.GetType().ToString() ?? "null"));
                        }
                        var hashOffset = handler.Tell() - sizeof(uint);
                        hash = (uint)buffer.Length;
                        handler.Write(hashOffset, hash);
                        handler.WriteOffsetContent((hh) => {
                            hh.WriteArray(buffer);
                            hh.Align(16);
                        });
                    }
                    break;
                default:
                    throw new NotImplementedException("Unsupported motree param type " + type);
            }
            handler.Align(8);
        }

        public override string ToString() => $"[{name ?? hash.ToString()}]: {value}";
    }

    public enum MotionTreeNodeType : byte
    {
        Unknown = 0,
        GameObject = 1,
        Component = 2,
        Folder = 3,
    }

    public class MotionTreeNode : BaseModel
    {
        public MotTreeVersion version;

        public MotionTreeNodeType nodeType;
        public string typeName = "";
        public string name = "";
        public uint fullTypeHash; // ascii hash of fully qualified node type classname
        public uint typeHash; // UTF-16 hash of (sub)type name; sometimes equal to node typeName -"Node", sometimes not
        public uint nameHash;
        public uint unknownHash;
        public byte flags;

        public NodeTypeInfo? TypeInfo => fullTypeHash switch {
            0 => NodeInfosDict.GetValueOrDefault((MurMur3HashUtils.GetAsciiHash("via.motion.tree." + typeName), 0u))
                // ?? NodeInfosNameDict.GetValueOrDefault(typeName),
                ?? NodeInfosNew
                    .Where(ni => ni.classShortname == typeName)
                    .OrderBy(ni => Math.Abs(ni.parameters.Length - Parameters.Count))
                    .FirstOrDefault()
                ?? NodeInfosNameDict.GetValueOrDefault(typeName),
            _ => NodeInfosDict.GetValueOrDefault((fullTypeHash, typeHash))
                ?? NodeInfosNameDict.GetValueOrDefault(Log.ValueLog<string>.RecordUniqueAndReturn(typeName, valueKey: "Mot Tree Hash Combination")),
        };

        public static string? GetParameterName(uint nameHash) => nameHash switch {
            ParameterNameHash.RootTransPolarBlend => nameof(ParameterNameHash.RootTransPolarBlend),
            ParameterNameHash.BlendRate => nameof(ParameterNameHash.BlendRate),
            ParameterNameHash.CircledBlend => nameof(ParameterNameHash.CircledBlend),
            ParameterNameHash.BlendType => nameof(ParameterNameHash.BlendType),
            ParameterNameHash.BlendMode => nameof(ParameterNameHash.BlendMode),
            ParameterNameHash.InMin => nameof(ParameterNameHash.InMin),
            ParameterNameHash.InMax => nameof(ParameterNameHash.InMax),
            ParameterNameHash.Flag => nameof(ParameterNameHash.Flag),
            ParameterNameHash.No => nameof(ParameterNameHash.No),
            ParameterNameHash.Value => nameof(ParameterNameHash.Value),
            ParameterNameHash.Asset => nameof(ParameterNameHash.Asset),
            ParameterNameHash.Speed => nameof(ParameterNameHash.Speed),
            ParameterNameHash.BankID => nameof(ParameterNameHash.BankID),
            ParameterNameHash.MotionID => nameof(ParameterNameHash.MotionID),
            ParameterNameHash.Operation => nameof(ParameterNameHash.Operation),
            ParameterNameHash.DirectID => nameof(ParameterNameHash.DirectID),
            ParameterNameHash.JointMaskID => nameof(ParameterNameHash.JointMaskID),
            ParameterNameHash.ClampMode => nameof(ParameterNameHash.ClampMode),
            ParameterNameHash.No00_BankID => nameof(ParameterNameHash.No00_BankID),
            ParameterNameHash.No00_MotionID => nameof(ParameterNameHash.No00_MotionID),
            ParameterNameHash.No01_BankID => nameof(ParameterNameHash.No01_BankID),
            ParameterNameHash.No01_MotionID => nameof(ParameterNameHash.No01_MotionID),
            ParameterNameHash.No02_BankID => nameof(ParameterNameHash.No02_BankID),
            ParameterNameHash.No02_MotionID => nameof(ParameterNameHash.No02_MotionID),
            ParameterNameHash.No03_BankID => nameof(ParameterNameHash.No03_BankID),
            ParameterNameHash.No03_MotionID => nameof(ParameterNameHash.No03_MotionID),
            ParameterNameHash.No04_BankID => nameof(ParameterNameHash.No04_BankID),
            ParameterNameHash.No04_MotionID => nameof(ParameterNameHash.No04_MotionID),
            ParameterNameHash.No05_BankID => nameof(ParameterNameHash.No05_BankID),
            ParameterNameHash.No05_MotionID => nameof(ParameterNameHash.No05_MotionID),
            ParameterNameHash.No06_BankID => nameof(ParameterNameHash.No06_BankID),
            ParameterNameHash.No06_MotionID => nameof(ParameterNameHash.No06_MotionID),
            ParameterNameHash.No07_BankID => nameof(ParameterNameHash.No07_BankID),
            ParameterNameHash.No07_MotionID => nameof(ParameterNameHash.No07_MotionID),
            ParameterNameHash.No08_BankID => nameof(ParameterNameHash.No08_BankID),
            ParameterNameHash.No08_MotionID => nameof(ParameterNameHash.No08_MotionID),
            ParameterNameHash.No09_BankID => nameof(ParameterNameHash.No09_BankID),
            ParameterNameHash.No09_MotionID => nameof(ParameterNameHash.No09_MotionID),
            ParameterNameHash.No10_BankID => nameof(ParameterNameHash.No10_BankID),
            ParameterNameHash.No10_MotionID => nameof(ParameterNameHash.No10_MotionID),
            ParameterNameHash.No11_BankID => nameof(ParameterNameHash.No11_BankID),
            ParameterNameHash.No11_MotionID => nameof(ParameterNameHash.No11_MotionID),
            ParameterNameHash.No12_BankID => nameof(ParameterNameHash.No12_BankID),
            ParameterNameHash.No12_MotionID => nameof(ParameterNameHash.No12_MotionID),
            ParameterNameHash.No13_BankID => nameof(ParameterNameHash.No13_BankID),
            ParameterNameHash.No13_MotionID => nameof(ParameterNameHash.No13_MotionID),
            ParameterNameHash.No14_BankID => nameof(ParameterNameHash.No14_BankID),
            ParameterNameHash.No14_MotionID => nameof(ParameterNameHash.No14_MotionID),
            ParameterNameHash.No15_BankID => nameof(ParameterNameHash.No15_BankID),
            ParameterNameHash.No15_MotionID => nameof(ParameterNameHash.No15_MotionID),
            ParameterNameHash.No00_Parameter => nameof(ParameterNameHash.No00_Parameter),
            ParameterNameHash.No01_Parameter => nameof(ParameterNameHash.No01_Parameter),
            ParameterNameHash.No02_Parameter => nameof(ParameterNameHash.No02_Parameter),
            ParameterNameHash.No03_Parameter => nameof(ParameterNameHash.No03_Parameter),
            ParameterNameHash.No04_Parameter => nameof(ParameterNameHash.No04_Parameter),
            ParameterNameHash.No05_Parameter => nameof(ParameterNameHash.No05_Parameter),
            ParameterNameHash.No06_Parameter => nameof(ParameterNameHash.No06_Parameter),
            ParameterNameHash.No07_Parameter => nameof(ParameterNameHash.No07_Parameter),
            ParameterNameHash.No08_Parameter => nameof(ParameterNameHash.No08_Parameter),
            ParameterNameHash.No09_Parameter => nameof(ParameterNameHash.No09_Parameter),
            ParameterNameHash.No10_Parameter => nameof(ParameterNameHash.No10_Parameter),
            ParameterNameHash.No11_Parameter => nameof(ParameterNameHash.No11_Parameter),
            ParameterNameHash.No12_Parameter => nameof(ParameterNameHash.No12_Parameter),
            ParameterNameHash.No13_Parameter => nameof(ParameterNameHash.No13_Parameter),
            ParameterNameHash.No14_Parameter => nameof(ParameterNameHash.No14_Parameter),
            ParameterNameHash.No15_Parameter => nameof(ParameterNameHash.No15_Parameter),
            ParameterNameHash.BankSelectMode => nameof(ParameterNameHash.BankSelectMode),
            ParameterNameHash.WorldRotation => nameof(ParameterNameHash.WorldRotation),
            ParameterNameHash.BaseNo => nameof(ParameterNameHash.BaseNo),
            ParameterNameHash.CenterBlendRate => nameof(ParameterNameHash.CenterBlendRate),
            ParameterNameHash.ChestBlendRate => nameof(ParameterNameHash.ChestBlendRate),
            ParameterNameHash.CenterName => nameof(ParameterNameHash.CenterName),
            ParameterNameHash.LeftFootName => nameof(ParameterNameHash.LeftFootName),
            ParameterNameHash.RightFootName => nameof(ParameterNameHash.RightFootName),
            ParameterNameHash.ChestName => nameof(ParameterNameHash.ChestName),
            ParameterNameHash.InterpolationFrame => nameof(ParameterNameHash.InterpolationFrame),
            ParameterNameHash.RoundingMode => nameof(ParameterNameHash.RoundingMode),
            ParameterNameHash.TargetFrame => nameof(ParameterNameHash.TargetFrame),
            ParameterNameHash.Always => nameof(ParameterNameHash.Always),
            ParameterNameHash.ToolName => nameof(ParameterNameHash.ToolName),

            ParameterNameHash._SyncBlendNode_357794474 => nameof(ParameterNameHash._SyncBlendNode_357794474),

            _ => null,
        };

        public static class ParameterNameHash
        {
            public const uint RootTransPolarBlend = 964257277;
            public const uint BlendRate = 2370884634;
            public const uint BlendMode = 4039545002;
            public const uint CircledBlend = 459737511;
            public const uint BlendType = 4114796164;
            public const uint _SyncBlendNode_357794474 = 357794474;
            public const uint InMin = 2609214554;
            public const uint InMax = 2157531142;
            public const uint Clamp = 3785007023;
            public const uint Flag = 3635188307;
            public const uint No = 762855704;
            public const uint Operation = 3248822590;
            public const uint Value = 4253840158;
            public const uint Asset = 3620651061;
            public const uint BankID = 1557423995;
            public const uint MotionID = 2673430856;
            public const uint JointMaskID = 2204709593;
            public const uint Always = 2166698608;
            public const uint ToolName = 1847845250;
            public const uint ClampMode = 798910796;
            public const uint No00_BankID = 119573349;
            public const uint No00_MotionID = 3714534649;
            public const uint No01_BankID = 43879613;
            public const uint No01_MotionID = 3064692809;
            public const uint No02_BankID = 2106658217;
            public const uint No02_MotionID = 997690459;
            public const uint No03_BankID = 1247431348;
            public const uint No03_MotionID = 2952910012;
            public const uint No04_BankID = 3644911457;
            public const uint No04_MotionID = 3401255985;
            public const uint No05_BankID = 2405258189;
            public const uint No05_MotionID = 452812079;
            public const uint No06_BankID = 3708765824;
            public const uint No06_MotionID = 1380573403;
            public const uint No07_BankID = 2257339756;
            public const uint No07_MotionID = 3608271588;
            public const uint No08_BankID = 1442361408;
            public const uint No08_MotionID = 631602840;
            public const uint No09_BankID = 2758339974;
            public const uint No09_MotionID = 491277133;
            public const uint No10_BankID = 1666226515;
            public const uint No10_MotionID = 1678053737;
            public const uint No11_BankID = 389971553;
            public const uint No11_MotionID = 2383168213;
            public const uint No12_BankID = 1339797869;
            public const uint No12_MotionID = 2232026136;
            public const uint No13_BankID = 1222198503;
            public const uint No13_MotionID = 1430378216;
            public const uint No14_BankID = 1570391611;
            public const uint No14_MotionID = 1286133883;
            public const uint No15_BankID = 558116323;
            public const uint No15_MotionID = 3748190535; // used by re7
            public const uint BankSelectMode = 640354462;
            public const uint BaseNo = 3772383378;
            public const uint WorldRotation = 506603;
            public const uint DirectID = 743296119;
            public const uint Speed = 4102950055;
            public const uint CenterBlendRate = 1684015762;
            public const uint ChestBlendRate = 1311223130;
            public const uint CenterName = 102621889;
            public const uint LeftFootName = 2422279243;
            public const uint RightFootName = 3667974927;
            public const uint ChestName = 2266762061;
            public const uint InterpolationFrame = 2714732966;
            public const uint RoundingMode = 188750666;
            public const uint TargetFrame = 4103375258;

            public const uint No00_Parameter = 832246504;
            public const uint No01_Parameter = 2194932303;
            public const uint No02_Parameter = 597458614;
            public const uint No03_Parameter = 215093529;
            public const uint No04_Parameter = 569416378;
            public const uint No05_Parameter = 2783989818;
            public const uint No06_Parameter = 4227443455;
            public const uint No07_Parameter = 1804482016;
            public const uint No08_Parameter = 2517091292;
            public const uint No09_Parameter = 78898067;
            public const uint No10_Parameter = 556922824;
            public const uint No11_Parameter = 3374882909;
            public const uint No12_Parameter = 2118342836;
            public const uint No13_Parameter = 2675596546;
            public const uint No14_Parameter = 1921804637;
            public const uint No15_Parameter = 4078385509;

            public const uint _Data = uint.MaxValue;
        }

        public class ScriptCallParameter : ReadWriteModel
        {
            public uint hash;
            public string name = "";

            protected override bool ReadWrite<THandler>(THandler action)
            {
                action.Handle(ref hash);
                action.HandleInlineAsciiString(ref name);
                return true;
            }

            public override string ToString() => name;
        }

        public class FloatList : BaseModel
        {
            public List<float> Values { get; } = new();

            protected override bool DoRead(FileHandler handler)
            {
                var count = handler.Read<int>();
                Values.Clear();
                Values.ReadStructList(handler, count);
                return true;
            }

            protected override bool DoWrite(FileHandler handler)
            {
                handler.Write(Values.Count);
                Values.Write(handler);
                return true;
            }
        }

        public struct Range3
        {
            public int num;
            public Vector3 values;

            public readonly override string ToString() => $"{num} => {values}";
        }

        public class MultiFloatList : BaseModel
        {
            public List<Range3> Lists { get; } = new();
            public float ukn;

            protected override bool DoRead(FileHandler handler)
            {
                var listCount = handler.Read<int>();
                // handler.ReadNull(4);
                handler.Read(ref ukn);
                Lists.Clear();
                Lists.ReadStructList(handler, listCount);
                return true;
            }

            protected override bool DoWrite(FileHandler handler)
            {
                handler.Write(Lists.Count);
                handler.WriteNull(4);
                Lists.Write(handler);
                return true;
            }
        }
        public class RotationGlobalOffsetData : BaseModel
        {
            public float angle;
            public float num;
            public uint hash;
            public List<float> Values { get; } = new();

            protected override bool DoRead(FileHandler handler)
            {
                handler.Read(ref angle);
                handler.Read(ref num);
                handler.Read(ref hash);
                var count = handler.Read<int>();
                Values.Clear();
                Values.ReadStructList(handler, count);
                return true;
            }

            protected override bool DoWrite(FileHandler handler)
            {
                handler.Write(ref angle);
                handler.Write(ref num);
                handler.Write(ref hash);
                handler.Write(Values.Count);
                Values.Write(handler);
                return true;
            }
        }

        public record class MotionTreeParameterInfo(string name, uint nameHash, MotionTreeParamType type, bool optional = false, Func<object?>? defaultValueGetter = null, params MotTreeVersion[]? disableFor)
        {
            public string? Name => GetParameterName(nameHash);
            public Type? DataBufferType { get; init; }
            public KnownFileFormats AssetFormat { get; init; }

            public MotionTreeNodeParameter Instantiate() => new MotionTreeNodeParameter(name);
        }

        public record class NodeTypeInfo(string classShortname, string? subtype, uint fullTypeHash, uint subtypeHash, string[] inputs, params MotionTreeParameterInfo[] parameters)
        {
            public override string ToString() => subtype != null ? subtype : $"{classShortname} / {subtypeHash.ToString()}";

            public MotionTreeParameterInfo? GetPropertyInfo(uint propertyHash) => parameters.FirstOrDefault(p => p.nameHash == propertyHash);
            public MotionTreeParameterInfo? GetBufferProperty() => parameters.FirstOrDefault(p => p.type == MotionTreeParamType.ExtraData);

            internal void Validate(MotionTreeNode node, MotTreeVersion game, MotTreeFile tree)
            {
                if (node.typeHash != 0) {
                    DataInterpretationException.DebugWarnIf(subtypeHash == 0, "Undeclared type hash for node type", $"{tree}: {node.typeName} => {node.typeHash}");
                    DataInterpretationException.DebugWarnIf(subtypeHash != 0 && subtypeHash != node.typeHash, "WRONG type hash for node type", $"{tree}: {node.typeName} => {node.typeHash}");
                    DataInterpretationException.DebugWarnIf(fullTypeHash == 0, "Undeclared full type hash for node type", $"{tree}: {node.typeName} => {node.fullTypeHash}");
                    DataInterpretationException.DebugWarnIf(fullTypeHash != 0 && fullTypeHash != node.fullTypeHash, "WRONG full type hash for node type", $"{tree}: {node.typeName} => {node.fullTypeHash}");
                }

                foreach (var p in node.Parameters) {
                    var knownParam = p.type == MotionTreeParamType.ExtraData
                        ? parameters.FirstOrDefault(pp => pp.type == MotionTreeParamType.ExtraData)
                        : parameters.FirstOrDefault(pp => p.hash == 0 ? p.name == pp.name : pp.nameHash == p.hash);
                    if (knownParam == null) {
                        Log.Warn($"{tree}: Undeclared mot tree parameter {{{p}}} for node {node}");
                        continue;
                    }

                    if (knownParam.disableFor?.Contains(game) == true) {
                        Log.Warn($"{tree}: Assumed unsupported mot tree parameter {{{p}}} found in node {node} for {game}");
                        continue;
                    }

                    if (p.type != knownParam.type) {
                        Log.Warn($"{tree}: Mot tree parameter type mismatch {{{p}}} for node {node} is {p.type}, expected {knownParam.type}");
                    }
                }

                foreach (var knownParam in parameters) {
                    if (knownParam.disableFor?.Contains(game) == true) continue;
                    // var p = node.Parameters.FirstOrDefault(pp => pp.hash == knownP.nameHash);
                    var p = knownParam.type == MotionTreeParamType.ExtraData
                        ? node.Parameters.FirstOrDefault(pp => pp.type == MotionTreeParamType.ExtraData)
                        : node.Parameters.FirstOrDefault(pp => pp.hash == 0 ? pp.name == knownParam.name : pp.hash == knownParam.nameHash);
                    if (p == null) {
                        if (!knownParam.optional) {
                            Log.Warn($"{tree}: Assumed required mot tree parameter {{{knownParam}}} not found in node {node} for {game}");
                        }
                        continue;
                    }
                }
            }
        }
        // private static readonly MotTreeVersion[] StartingAfterRE2 = [MotTreeVersion.RE2];
        private static readonly MotTreeVersion[] StartingAfterRE3 = [MotTreeVersion.RE2, MotTreeVersion.RE3];
        private static readonly MotTreeVersion[] StartingAfterMHR = [..StartingAfterRE3, MotTreeVersion.RE8, MotTreeVersion.RE_RT, MotTreeVersion.MHR];
        private static readonly MotTreeVersion[] StartingWithRE9 = [..StartingAfterMHR, MotTreeVersion.RE4, MotTreeVersion.DD2];

        private static object Default_MaxUnit() => (object)uint.MaxValue;

        private static MotionTreeParameterInfo Param(string paramName, MotionTreeParamType type, MotTreeVersion[]? disableFor = null)
            => new MotionTreeParameterInfo(paramName, MurMur3HashUtils.GetAsciiHash(paramName), type, disableFor: disableFor);

        private static MotionTreeParameterInfo Float(string paramName)
            => new MotionTreeParameterInfo(paramName, MurMur3HashUtils.GetAsciiHash(paramName), MotionTreeParamType.Float);

        private static MotionTreeParameterInfo Str(string paramName)
            => new MotionTreeParameterInfo(paramName, MurMur3HashUtils.GetAsciiHash(paramName), MotionTreeParamType.StrUTF16);

        private static MotionTreeParameterInfo Optional(string paramName, MotionTreeParamType type, MotTreeVersion[]? disableFor = null)
            => new MotionTreeParameterInfo(paramName, MurMur3HashUtils.GetAsciiHash(paramName), type, optional: true, disableFor: disableFor);

        private static MotionTreeParameterInfo Asset(KnownFileFormats format)
            => new MotionTreeParameterInfo(nameof(ParameterNameHash.Asset), ParameterNameHash.Asset, MotionTreeParamType.StrUTF16) { AssetFormat = format };

        private static MotionTreeParameterInfo SelectBankParam(string paramName)
            => new MotionTreeParameterInfo(paramName, MurMur3HashUtils.GetAsciiHash(paramName), MotionTreeParamType.UInt32);

        private static MotionTreeParameterInfo SelectMotionParam(string paramName)
            => new MotionTreeParameterInfo(paramName, MurMur3HashUtils.GetAsciiHash(paramName), MotionTreeParamType.UInt32, false, Default_MaxUnit);

        private static MotionTreeParameterInfo SelectParam(string paramName)
            => new MotionTreeParameterInfo(paramName, MurMur3HashUtils.GetAsciiHash(paramName), MotionTreeParamType.Float);

        private static MotionTreeParameterInfo StructParam<T>()
            => new MotionTreeParameterInfo(nameof(ParameterNameHash._Data), ParameterNameHash._Data, MotionTreeParamType.ExtraData) { DataBufferType = typeof(T) };
        private static MotionTreeParameterInfo ByteBuffer = new MotionTreeParameterInfo(nameof(ParameterNameHash._Data), ParameterNameHash._Data, MotionTreeParamType.ExtraData);

        private static readonly MotionTreeParameterInfo[] BaseBlendTreeParams_Modern = [
            Param(nameof(ParameterNameHash.RootTransPolarBlend), MotionTreeParamType.Bool),
            Float(nameof(ParameterNameHash.BlendRate)),
            Param(nameof(ParameterNameHash.CircledBlend), MotionTreeParamType.Bool),
            Param(nameof(ParameterNameHash.BlendType), MotionTreeParamType.UInt32),
            new (nameof(ParameterNameHash._SyncBlendNode_357794474), ParameterNameHash._SyncBlendNode_357794474, MotionTreeParamType.Int32, false, null, StartingAfterMHR),
            Optional(nameof(ParameterNameHash.ToolName), MotionTreeParamType.StrUTF16),
        ];

        private static readonly MotionTreeParameterInfo[] BaseBlendTreeParams_RE8 = [
            Optional(nameof(ParameterNameHash.RootTransPolarBlend), MotionTreeParamType.Bool, StartingAfterRE3),
            Float(nameof(ParameterNameHash.BlendRate)),
            Optional(nameof(ParameterNameHash.CircledBlend), MotionTreeParamType.Bool),
            Param(nameof(ParameterNameHash.BlendType), MotionTreeParamType.UInt32, StartingAfterRE3),
            Optional(nameof(ParameterNameHash.ToolName), MotionTreeParamType.StrUTF16, StartingAfterRE3),
        ];

        internal static readonly List<NodeTypeInfo> NodeInfosNew = new() {
            new NodeTypeInfo("VariablesNode", "Variables", 1318946890, 2732277690, [],
                Asset(KnownFileFormats.UserVariables)),
            new NodeTypeInfo("MotionTreeNode", null, 0, 0, [],
                Asset(KnownFileFormats.MotionTree)),
            new NodeTypeInfo("SwitchNode", "Switch", 1677865356, 3210958442, [],
                Param(nameof(ParameterNameHash.Flag), MotionTreeParamType.Bool)),
            new NodeTypeInfo("BasePoseNode", null, 1329486701, 2698905830, [],
                Param(nameof(ParameterNameHash.JointMaskID), MotionTreeParamType.UInt32)),
            new NodeTypeInfo("NumberSwitchNode", null, 4261549358, 3457278066, [],
                Param(nameof(ParameterNameHash.No), MotionTreeParamType.UInt32)),
            new NodeTypeInfo("SwitchParameterNode", null, 2092953933, 3060193183, [],
                SelectParam(nameof(ParameterNameHash.No00_Parameter)),
                SelectParam(nameof(ParameterNameHash.No01_Parameter)),
                SelectParam(nameof(ParameterNameHash.No02_Parameter)),
                SelectParam(nameof(ParameterNameHash.No03_Parameter)),
                SelectParam(nameof(ParameterNameHash.No04_Parameter)),
                SelectParam(nameof(ParameterNameHash.No05_Parameter)),
                SelectParam(nameof(ParameterNameHash.No06_Parameter)),
                SelectParam(nameof(ParameterNameHash.No07_Parameter)),
                SelectParam(nameof(ParameterNameHash.No08_Parameter)),
                SelectParam(nameof(ParameterNameHash.No09_Parameter)),
                SelectParam(nameof(ParameterNameHash.No10_Parameter)),
                SelectParam(nameof(ParameterNameHash.No11_Parameter)),
                SelectParam(nameof(ParameterNameHash.No12_Parameter)),
                SelectParam(nameof(ParameterNameHash.No13_Parameter)),
                SelectParam(nameof(ParameterNameHash.No14_Parameter)),
                SelectParam(nameof(ParameterNameHash.No15_Parameter))),
            new NodeTypeInfo("DynamicSwitchParameterNode", "DynamicSwitchParameter", 1767800217, 3419527688, []),
            new NodeTypeInfo("ParameterNode", "Parameter", 876237402, 2038626523, [],
                Float(nameof(ParameterNameHash.Value)),
                Optional(nameof(ParameterNameHash.ToolName), MotionTreeParamType.StrUTF16, StartingAfterRE3)),
            new NodeTypeInfo("CompareNode", "Compare", 504175270, 3566971628, [],
                new MotionTreeParameterInfo(nameof(ParameterNameHash.Operation), ParameterNameHash.Operation, MotionTreeParamType.UInt32)),
            new NodeTypeInfo("CalculateNode", "Calculate", 1176952898, 3049156317, [],
                new MotionTreeParameterInfo(nameof(ParameterNameHash.Operation), ParameterNameHash.Operation, MotionTreeParamType.UInt32)),
            new NodeTypeInfo("PlaySpeedNode", "PlaySpeed", 1922759541, 2345610969, [],
                Float(nameof(ParameterNameHash.Speed))),
            new NodeTypeInfo("PlaySpeedScalingNode", "PlaySpeedScaling", 1238078276, 3866385883, [],// 904519887 = re2 sectionroot/animation/player/common/list/hld_tree.motlist.85
                Float(nameof(ParameterNameHash.Speed))),
            new NodeTypeInfo("AddBlendNode", "AddBlend", 2416675456, 3762469993, [],
                Float(nameof(ParameterNameHash.BlendRate)),
                new MotionTreeParameterInfo(nameof(ParameterNameHash.BaseNo), ParameterNameHash.BaseNo, MotionTreeParamType.UInt32),
                new MotionTreeParameterInfo(nameof(ParameterNameHash.WorldRotation), ParameterNameHash.WorldRotation, MotionTreeParamType.Bool)),
            new NodeTypeInfo("SelectMotionNode", "SelectMotion", 1782532701, 57058317, [],
                Param(nameof(ParameterNameHash.BankID), MotionTreeParamType.UInt32),

                // Optional(nameof(ParameterNameHash.DirectID), MotionTreeParamType.Bool),
                SelectBankParam(nameof(ParameterNameHash.No00_BankID)),
                SelectMotionParam(nameof(ParameterNameHash.No00_MotionID)),
                SelectBankParam(nameof(ParameterNameHash.No01_BankID)),
                SelectMotionParam(nameof(ParameterNameHash.No01_MotionID)),

                Param(nameof(ParameterNameHash.BankSelectMode), MotionTreeParamType.Int32),
                new (nameof(ParameterNameHash.JointMaskID), ParameterNameHash.JointMaskID, MotionTreeParamType.UInt32, false, null, StartingAfterMHR),
                Optional(nameof(ParameterNameHash.Always), MotionTreeParamType.Bool)),
            new NodeTypeInfo("SelectMotionNode", null, 1782532701, 1341727534, [],
                Param(nameof(ParameterNameHash.BankID), MotionTreeParamType.UInt32),
                new (nameof(ParameterNameHash.JointMaskID), ParameterNameHash.JointMaskID, MotionTreeParamType.UInt32, false, null, StartingAfterMHR),
                Param(nameof(ParameterNameHash.DirectID), MotionTreeParamType.Bool),
                Optional(nameof(ParameterNameHash.Always), MotionTreeParamType.Bool)),
            new NodeTypeInfo("SelectMotionNode", null, 1782532701, 440989612, [],
                Param(nameof(ParameterNameHash.BankID), MotionTreeParamType.UInt32),
                SelectBankParam(nameof(ParameterNameHash.No00_BankID)),
                SelectMotionParam(nameof(ParameterNameHash.No00_MotionID)),
                SelectBankParam(nameof(ParameterNameHash.No01_BankID)),
                SelectMotionParam(nameof(ParameterNameHash.No01_MotionID)),
                SelectBankParam(nameof(ParameterNameHash.No02_BankID)),
                SelectMotionParam(nameof(ParameterNameHash.No02_MotionID)),
                SelectBankParam(nameof(ParameterNameHash.No03_BankID)),
                SelectMotionParam(nameof(ParameterNameHash.No03_MotionID)),
                SelectBankParam(nameof(ParameterNameHash.No04_BankID)),
                SelectMotionParam(nameof(ParameterNameHash.No04_MotionID)),
                SelectBankParam(nameof(ParameterNameHash.No05_BankID)),
                SelectMotionParam(nameof(ParameterNameHash.No05_MotionID)),
                SelectBankParam(nameof(ParameterNameHash.No06_BankID)),
                SelectMotionParam(nameof(ParameterNameHash.No06_MotionID)),
                SelectBankParam(nameof(ParameterNameHash.No07_BankID)),
                SelectMotionParam(nameof(ParameterNameHash.No07_MotionID)),
                Param(nameof(ParameterNameHash.BankSelectMode), MotionTreeParamType.Int32),
                new (nameof(ParameterNameHash.JointMaskID), ParameterNameHash.JointMaskID, MotionTreeParamType.UInt32, false, null, StartingAfterMHR),
                Optional(nameof(ParameterNameHash.Always), MotionTreeParamType.Bool)),
            new NodeTypeInfo("SelectMotionNode", null, 1782532701, 2210888714, [],
                new (nameof(ParameterNameHash.BankID), ParameterNameHash.BankID, MotionTreeParamType.UInt32),
                SelectBankParam(nameof(ParameterNameHash.No00_BankID)),
                SelectMotionParam(nameof(ParameterNameHash.No00_MotionID)),
                SelectBankParam(nameof(ParameterNameHash.No01_BankID)),
                SelectMotionParam(nameof(ParameterNameHash.No01_MotionID)),
                SelectBankParam(nameof(ParameterNameHash.No02_BankID)),
                SelectMotionParam(nameof(ParameterNameHash.No02_MotionID)),
                SelectBankParam(nameof(ParameterNameHash.No03_BankID)),
                SelectMotionParam(nameof(ParameterNameHash.No03_MotionID)),
                SelectBankParam(nameof(ParameterNameHash.No04_BankID)),
                SelectMotionParam(nameof(ParameterNameHash.No04_MotionID)),
                SelectBankParam(nameof(ParameterNameHash.No05_BankID)),
                SelectMotionParam(nameof(ParameterNameHash.No05_MotionID)),
                SelectBankParam(nameof(ParameterNameHash.No06_BankID)),
                SelectMotionParam(nameof(ParameterNameHash.No06_MotionID)),
                SelectBankParam(nameof(ParameterNameHash.No07_BankID)),
                SelectMotionParam(nameof(ParameterNameHash.No07_MotionID)),
                SelectBankParam(nameof(ParameterNameHash.No08_BankID)),
                SelectMotionParam(nameof(ParameterNameHash.No08_MotionID)),
                SelectBankParam(nameof(ParameterNameHash.No09_BankID)),
                SelectMotionParam(nameof(ParameterNameHash.No09_MotionID)),
                SelectBankParam(nameof(ParameterNameHash.No10_BankID)),
                SelectMotionParam(nameof(ParameterNameHash.No10_MotionID)),
                SelectBankParam(nameof(ParameterNameHash.No11_BankID)),
                SelectMotionParam(nameof(ParameterNameHash.No11_MotionID)),
                SelectBankParam(nameof(ParameterNameHash.No12_BankID)),
                SelectMotionParam(nameof(ParameterNameHash.No12_MotionID)),
                SelectBankParam(nameof(ParameterNameHash.No13_BankID)),
                SelectMotionParam(nameof(ParameterNameHash.No13_MotionID)),
                SelectBankParam(nameof(ParameterNameHash.No14_BankID)),
                SelectMotionParam(nameof(ParameterNameHash.No14_MotionID)),
                SelectBankParam(nameof(ParameterNameHash.No15_BankID)),
                SelectMotionParam(nameof(ParameterNameHash.No15_MotionID)),

                Param(nameof(ParameterNameHash.BankSelectMode), MotionTreeParamType.Int32),
                new (nameof(ParameterNameHash.JointMaskID), ParameterNameHash.JointMaskID, MotionTreeParamType.UInt32, false, null, StartingAfterMHR),
                Optional(nameof(ParameterNameHash.Always), MotionTreeParamType.Bool)),
            new NodeTypeInfo("MotionNode", "Motion", 2463125501, 3868588456, [],
                Asset(KnownFileFormats.Motion), // TODO verify - should we do motionBase? allow motionList, tree?
                Param(nameof(ParameterNameHash.BankID), MotionTreeParamType.UInt32),
                Param(nameof(ParameterNameHash.MotionID), MotionTreeParamType.UInt32),
                Param(nameof(ParameterNameHash.JointMaskID), MotionTreeParamType.UInt32, StartingAfterMHR),
                Optional(nameof(ParameterNameHash.Always), MotionTreeParamType.Bool),
                Optional(nameof(ParameterNameHash.ToolName), MotionTreeParamType.StrUTF16, StartingAfterRE3)), // at least in RE8
            new NodeTypeInfo("RangeNormalizeNode", "Clamp", 2809927802, 3318849321, [],
                Float(nameof(ParameterNameHash.InMin)),
                Float(nameof(ParameterNameHash.InMax)),
                Param(nameof(ParameterNameHash.ClampMode), MotionTreeParamType.Bool)),
            new NodeTypeInfo("RangeNormalizeNode", "Normalize", 2809927802, 2814588301, [],
                Float(nameof(ParameterNameHash.InMin)),
                Float(nameof(ParameterNameHash.InMax))),
            new NodeTypeInfo("RangeNormalizeNode", null, 2809927802, 0, [],
                Float(nameof(ParameterNameHash.InMin)),
                Float(nameof(ParameterNameHash.InMax)),
                Optional(nameof(ParameterNameHash.ClampMode), MotionTreeParamType.Bool)),
            new NodeTypeInfo("RandomParameterNode", "RandomParameter", 2101790633, 3679509593, [],
                Float(nameof(ParameterNameHash.InMin)),
                Float(nameof(ParameterNameHash.InMax))),
            new NodeTypeInfo("StepRangeNormalizeNode", "StepNormalize", 1264711654, 3253589354, [], StructParam<FloatList>()),
            new NodeTypeInfo("BlendNode", "Blend", 3065294479, 108828207, [],
                Float(nameof(ParameterNameHash.BlendRate))),
            new NodeTypeInfo("BlendRateNode", null, 3232032544, 1179641321, [],
                Float(nameof(ParameterNameHash.BlendRate))),
            new NodeTypeInfo("LayerBlendNode", "Overwrite", 3186742464, 2995066944, [],
                new (nameof(ParameterNameHash.RootTransPolarBlend), ParameterNameHash.RootTransPolarBlend, MotionTreeParamType.Bool, false, null, StartingAfterRE3),
                Float(nameof(ParameterNameHash.BlendRate))),
            new NodeTypeInfo("MultiLayerBlendNode", null, 1286598350, 431169198, [],
                new MotionTreeParameterInfo(nameof(ParameterNameHash.BlendMode), ParameterNameHash.BlendMode, MotionTreeParamType.Int32)),
            new NodeTypeInfo("SyncAddBlendNode", "SyncAddBlend", 1823014197, 95249988, [],
                Float(nameof(ParameterNameHash.BlendRate)),
                new (nameof(ParameterNameHash.BaseNo), ParameterNameHash.BaseNo, MotionTreeParamType.UInt32),
                new (nameof(ParameterNameHash.WorldRotation), ParameterNameHash.WorldRotation, MotionTreeParamType.Bool)),

            new NodeTypeInfo("SyncBlendNode", "SyncBlend", 720321070, 1856389764, [], BaseBlendTreeParams_Modern),
            new NodeTypeInfo("SyncBlendNode", "SyncBlend", 720321070, 0, [], BaseBlendTreeParams_RE8),

            new NodeTypeInfo("OctSyncBlendNode", "OctSyncBlend", 199633321, 62627309, [], BaseBlendTreeParams_Modern),
            new NodeTypeInfo("OctSyncBlendNode", "OctSyncBlend", 199633321, 0, [], BaseBlendTreeParams_RE8),

            new NodeTypeInfo("HexSyncBlendNode", "HexSyncBlend", 2864450539, 585746337, [], BaseBlendTreeParams_Modern),
            new NodeTypeInfo("HexSyncBlendNode", "HexSyncBlend", 2864450539, 0, [], BaseBlendTreeParams_RE8),

            new NodeTypeInfo("UpperBodyBlendNode", null, 0, 0, [],
                Float(nameof(ParameterNameHash.BlendRate)),
                Float(nameof(ParameterNameHash.CenterBlendRate)),
                Float(nameof(ParameterNameHash.ChestBlendRate)),
                Str(nameof(ParameterNameHash.CenterName)),
                Str(nameof(ParameterNameHash.LeftFootName)),
                Str(nameof(ParameterNameHash.RightFootName)),
                Str(nameof(ParameterNameHash.ChestName)),
                Param(nameof(ParameterNameHash.BaseNo), MotionTreeParamType.UInt32)),

            new NodeTypeInfo("SyncFadeSwitchNode", "SyncFadeSwitch", 2129571270, 1386812972, [],
                Float(nameof(ParameterNameHash.InterpolationFrame))),

            new NodeTypeInfo("OctSyncFadeSwitchNode", "OctSyncFadeSwitch", 1792897910, 2819506855, [],
                Float(nameof(ParameterNameHash.InterpolationFrame))),

            new NodeTypeInfo("SyncNormalizeTimeNode", "SyncNormalizeTime", 2408685917, 2973802780, []),

            new NodeTypeInfo("RoundingNode", "Rounding", 170056945, 4293504823, [],
                Param(nameof(ParameterNameHash.RoundingMode), MotionTreeParamType.Int32)),

            new NodeTypeInfo("ParentLocalConstraintsNode", null, 0, 0, []),
            new NodeTypeInfo("InitialCacheNode", null, 2687167913, 1406782740, []),
            new NodeTypeInfo("AbsNode", "Abs", 3606840168, 1622263416, []),
            new NodeTypeInfo("MirrorNode", "Mirror", 919869368, 4090004415, []),
            new NodeTypeInfo("MirrorNode", null, 919869368, 2922565840, []),
            new NodeTypeInfo("ScriptCallNode", null, 2619151604, 3795636541, [], StructParam<ScriptCallParameter>()),
            new NodeTypeInfo("InnerFacialBlendNode", "InnerFacialBlend", 2967467778, 2130557010, [], ByteBuffer),
            new NodeTypeInfo("RangeSwitchCaseNode", "RangeSwitchCase", 3262964866, 1321968728, [], StructParam<MultiFloatList>()),
            new NodeTypeInfo("RotationGlobalOffsetNode", null, 1760913074, 1077756185, [], StructParam<RotationGlobalOffsetData>()),
            new NodeTypeInfo("FrameNode", "Frame", 1818166333, 4503416, [],
                Float(nameof(ParameterNameHash.TargetFrame))),
            new NodeTypeInfo("SequentialNode", "Sequential", 1010992662, 1869103989, []),
            new NodeTypeInfo("NotNode", "Not", 3482315861, 3356171420, []),
        };

        internal static readonly Dictionary<(uint fqn, uint sub), NodeTypeInfo> NodeInfosDict = NodeInfosNew.Where(x => x.fullTypeHash != 0).ToDictionary(k => (k.fullTypeHash, k.subtypeHash));

        public static readonly string[] NodeTypes = NodeInfosNew.Select(ni => ni.classShortname).Distinct().ToArray();

        internal static readonly Dictionary<string, NodeTypeInfo> NodeInfosNameDict = NodeTypes
            .Select(type => NodeInfosNew.First(n => n.classShortname == type))
            .ToDictionary(k => k.classShortname);

        public List<string> Tags { get; } = new(0);

        public List<MotionTreeNodeParameter> Parameters { get; } = new();

        protected override bool DoRead(FileHandler handler)
        {
            if (version <= MotTreeVersion.RE8)
            {
                handler.ReadOffsetAsciiString(out typeName);
                if (!MotionTreeNode.NodeTypes.Contains(typeName)) {
                    Log.UniqueValueKeyed(typeName, "Unknown mottree node type");
                }
            }

            handler.ReadOffsetWString(out name);

            var tagOffsetsOffset = handler.Read<long>();
            var tagHashTableOffsetMaybe = handler.Read<long>();
            var dataOffset = handler.Read<long>();
            if (version <= MotTreeVersion.RE8) {
                handler.Read(ref unknownHash);
                handler.ReadNull(4);
                handler.Read(ref nameHash); // dmc5 pl0200_strafe.motlist.85 pl0200_Move_Start_180
            } else if (version <= MotTreeVersion.MHR) {
                handler.Read(ref unknownHash);
                handler.Read(ref fullTypeHash);
                handler.Read(ref typeHash);
                handler.Read(ref nameHash); // TODO verify RE_RT + no clue about MHR
            } else {
                handler.Read(ref fullTypeHash);
                handler.Read(ref typeHash);
                handler.Read(ref nameHash);
            }
            DataInterpretationException.DebugWarnIf(!string.IsNullOrEmpty(name) && MurMur3HashUtils.GetHash(name) != nameHash);

            var tagCount = handler.Read<byte>();
            var paramCount = handler.Read<byte>();
            handler.Read(ref nodeType);
            handler.Read(ref flags);
            if (version is MotTreeVersion.RE_RT or MotTreeVersion.MHR) handler.ReadNull(4);

            if (tagCount > 0)
            {
                using var __ = handler.SeekJumpBack(tagOffsetsOffset);
                handler.Seek(tagOffsetsOffset);
                for (int i = 0; i < tagCount; ++i) Tags.Add(handler.ReadOffsetWString());
                // alpha's template has these marked as hashes, but they seem to just be 0s and always just 1 of them
                // maybe in mh games
                handler.Seek(tagHashTableOffsetMaybe);
                handler.ReadNull(4 * tagCount);
            }

            using var _ = handler.SeekJumpBack(dataOffset);
            for (int i = 0; i < paramCount; ++i) {
                var param = new MotionTreeNodeParameter();
                param.Read(handler, version);
                Parameters.Add(param);
            }

            return true;
        }

        protected override bool DoWrite(FileHandler handler)
        {
            if (version <= MotTreeVersion.RE8)
            {
                handler.Skip(8); // typeName
                // handler.ReadOffsetAsciiString(out typeName);
            }

            handler.Skip(8); // name
            if (Tags.Count > 0)
            {
                // note: untested correctness for count > 0
                handler.WriteOffsetContent((hh) => {
                    var start = hh.Tell();

                    handler.Skip(Tags.Count * sizeof(long)); // skip string offsets
                    handler.Skip(Tags.Count * sizeof(uint)); // skip string hashes
                    handler.Align(8);
                    foreach (var tag in Tags) {
                        hh.Write(start, handler.Tell());
                        hh.WriteWString(tag);
                        start += sizeof(long);
                    }
                });
                handler.WriteOffsetContent((hh) => {
                    foreach (var tag in Tags) handler.Write(0);
                    handler.Align(8);
                });
            }
            else
            {
                handler.WriteNull(16);
            }
            handler.Skip(8); // var dataOffset = handler.Write<long>();
            if (!string.IsNullOrEmpty(name)) {
                nameHash = MurMur3HashUtils.GetHash(name);
            }
            if (version <= MotTreeVersion.RE8) {
                handler.Write(ref unknownHash);
                handler.WriteNull(4);
                handler.Write(ref nameHash); // dmc5 pl0200_strafe.motlist.85 pl0200_Move_Start_180
            } else if (version <= MotTreeVersion.MHR) {
                handler.Write(ref unknownHash);
                handler.Write(ref fullTypeHash);
                handler.Write(ref typeHash);
                handler.Write(ref nameHash); // TODO verify RE_RT + no clue about MHR
            } else {
                handler.Write(ref fullTypeHash);
                handler.Write(ref typeHash);
                handler.Write(ref nameHash);
            }

            handler.Write((byte)Tags.Count);
            handler.Write((byte)Parameters.Count);
            handler.Write(ref nodeType);
            handler.Write(ref flags);
            if (version is MotTreeVersion.RE_RT or MotTreeVersion.MHR) handler.WriteNull(4);

            return true;
        }

        public void WriteTypes(FileHandler handler)
        {
            if (version <= MotTreeVersion.RE8)
            {
                handler.Write(Start, handler.Tell());
            }

            handler.WriteAsciiString(typeName);
        }

        public void WriteNames(FileHandler handler)
        {
            handler.Write(Start + (version <= MotTreeVersion.RE8 ? 8 : 0), handler.Tell());
            handler.WriteWString(name);
        }

        public void WriteParameters(FileHandler handler)
        {
            handler.Write(Start + 24 + (version <= MotTreeVersion.RE8 ? 8 : 0), handler.Tell());
            foreach (var param in Parameters)
            {
                param.Write(handler, version);
            }
        }

        public override string ToString() => TypeInfo == null
            ? $"[{typeName} / {typeHash}] {name} ({nodeType})"
            : $"[{TypeInfo.ToString()}] {name} ({nodeType})";
    }

    [RszGenerate]
    public partial class MotionTreeLink : BaseModel
    {
        public int inputNodeIndex;
        public int inputPinNo;
        public int outputNodeIndex;
        public int outputPinNo;
        [RszPaddingAfter(4)]
        public MotionTreeLinkType type;

        [RszIgnore] public Guid inputGuid;
        [RszIgnore] public Guid outputGuid;

        protected override bool DoRead(FileHandler handler)
        {
            DefaultRead(handler);
            var inputGuidOffset = handler.Read<long>();
            var outputGuidOffset = handler.Read<long>();
            if (inputGuidOffset > 0) inputGuid = handler.Read<Guid>(inputGuidOffset);
            if (outputGuidOffset > 0) outputGuid = handler.Read<Guid>(outputGuidOffset);
            return true;
        }

        protected override bool DoWrite(FileHandler handler)
        {
            DefaultWrite(handler);
            if (inputGuid == Guid.Empty) handler.Write(0L); else handler.WriteOffsetContent((hh) => hh.Write(inputGuid));
            if (outputGuid == Guid.Empty) handler.Write(0L); else handler.WriteOffsetContent((hh) => hh.Write(outputGuid));
            return true;
        }

        public override string ToString() => $"{type} {inputNodeIndex} => {outputNodeIndex}";
    }

    public struct TreeIndexPair
    {
        public int source;
        public int target;

        public override string ToString() => $"{source} => {target}";
    }
}


namespace ReeLib
{
    public class MotTreeFile(FileHandler fileHandler)
        : MotFileBase(fileHandler)
    {
        public const uint Magic = 0x6572746D;
        public const string Extension = ".mottree";

        public override KnownFileFormats MotType => KnownFileFormats.MotionTree;
        public override string Name { get; set; } = string.Empty;

        public MotTreeVersion version;

        public string UvarPath { get; set; } = string.Empty;
        public string ResourcePath { get; set; } = string.Empty;
        public List<TreeIndexPair> Indices { get; } = new();
        public List<TreeIndexPair> MotionIDRemaps { get; } = new();
        public List<MotionTreeNode> Nodes { get; } = new();
        public List<MotionTreeLink> Links { get; } = new();

        public short uknCount1;
        public short uknCount2;
        public short expandedMotionsCount;

        protected override bool DoRead()
        {
            Indices.Clear();
            MotionIDRemaps.Clear();
            var handler = FileHandler;
            handler.Read(ref version);
            if (handler.Read<int>() != Magic)
            {
                throw new InvalidDataException($"{handler.FilePath} Not a motion tree file");
            }
            handler.ReadNull(8);
            UvarPath = handler.ReadOffsetWStringNullable() ?? "";
            ResourcePath = handler.ReadOffsetWStringNullable() ?? "";

            var nodeOffset = handler.Read<long>();
            var linkDataOffset = handler.Read<long>();
            var nameOffset = handler.Read<long>();
            Name = handler.ReadWString(nameOffset);

            long indicesOffset = 0;
            if (version > MotTreeVersion.MHR)
            {
                indicesOffset = handler.Read<long>();
            }
            var nodeIdTableOffset = handler.Read<long>();
            var nodeCount = handler.Read<short>();
            var linkCount = handler.Read<short>();
            handler.Read(ref uknCount1);
            handler.Read(ref uknCount2);
            var basicMotionsCount = handler.Read<short>();

            // "expandedMotionsCount" includes sub motion counts (e.g. SelectMotionNode can have 2~any amount of motions itself), always (?) zero in earlier games
            // always 0 for <= RE_RT
            handler.Read(ref expandedMotionsCount);

            handler.ReadNull(4);

            // TODO figure out exactly why or how the counts here work
            // reference: dd2 ch00_000_com_tree StartMoveRight (18 + 14 + 22 nodes + 27 links)
            // one other case has real count 9 while SelectMotionNode has itself 9 + another normal MotionNode for a grand total of 10 actual motions...
            if (indicesOffset > 0)
            {
                handler.Seek(indicesOffset);
                var count = (int)((nodeIdTableOffset - indicesOffset) / 4);
                for (int i = 0; i < count; ++i)
                {
                    Indices.Add(new TreeIndexPair() {
                        source = handler.Read<short>(),
                        target = handler.Read<short>(),
                    });
                }
            }

            if (basicMotionsCount > 0 && nodeIdTableOffset > 0)
            {
                handler.Seek(nodeIdTableOffset);
                // Motion = +1 node
                // SelectMotion = +2~any nodes
                var nodeSize = version switch {
                    < MotTreeVersion.RE_RT => 4,
                    < MotTreeVersion.RE4 => 6,
                    _ => 8,
                };
                var realCount = (nameOffset - nodeIdTableOffset) / nodeSize;
                for (int i = 0; i < realCount; ++i)
                {
                    var pair = new TreeIndexPair();
                    pair.target = version < MotTreeVersion.RE_RT ? handler.Read<short>() : handler.Read<int>();
                    pair.source = version < MotTreeVersion.RE4 ? handler.Read<short>() : handler.Read<int>();
                    MotionIDRemaps.Add(pair);
                }
                handler.Align(8);
                DataInterpretationException.ThrowIfDifferent(handler.Tell(), nameOffset);
            }

            Nodes.Clear();
            Links.Clear();
            handler.Seek(nodeOffset);
            for (int i = 0; i < nodeCount; ++i)
            {
                var node = new MotionTreeNode() { version = version };
                node.Read(handler);
                Nodes.Add(node);
            }

            if (version > MotTreeVersion.RE8)
            {
                foreach (var node in Nodes)
                {
                    node.typeName = handler.ReadAsciiString(jumpBack: false);
                }
            }

            foreach (var node in Nodes)
            {
                var typeinfo = node.TypeInfo;
                if (typeinfo == null) continue;

                foreach (var p in node.Parameters) {
                    if (p.type != MotionTreeParamType.ExtraData) continue;

                    var propInfo = typeinfo.GetBufferProperty();
                    if (propInfo == null) continue;

                    if (propInfo.DataBufferType != null) {
                        var obj = (IModel)Activator.CreateInstance(propInfo.DataBufferType)!;
                        var bufHandler = new FileHandler(new MemoryStream((byte[])p.value!));
                        if (!obj.Read(bufHandler)) {
                            Log.Error($"{this}: Failed to read motion tree parameter data buffer for node {node}");
                            continue;
                        }
                        p.value = obj;
                        if (!bufHandler.IsEnd) {
                            Log.Warn("Motion tree parameter data was not fully read");
                        }
                    } else {
                        Log.Info($"Motion tree extra data parameter structure for node type {node.typeName} is still unresolved, will be displayed as raw data.");
                    }
                }
            }
#if DEBUG
            foreach (var node in Nodes)
            {
                DataInterpretationException.DebugWarnIf(node.fullTypeHash != 0 && node.fullTypeHash != MurMur3HashUtils.GetAsciiHash("via.motion.tree." + node.typeName));
                var typeinfo = node.TypeInfo;
                if (typeinfo == null) {
                    Log.UniqueValueKeyed(node.typeName, "Unknown mottree node");
                } else {
                    typeinfo.Validate(node, version, this);
                }
            }
#endif

            handler.Seek(linkDataOffset);
            Links.Read(handler, linkCount);

            handler.Align(16);
            handler.OffsetContentTableFlush();

            return true;
        }

        protected override bool DoWrite()
        {
            FileHandler handler = FileHandler;

            handler.Write(ref version);
            handler.Write(Magic);
            handler.WriteNull(8);
            if (string.IsNullOrEmpty(UvarPath)) handler.WriteNull(8); else handler.WriteOffsetWString(UvarPath);
            if (string.IsNullOrEmpty(ResourcePath)) handler.WriteNull(8); else handler.WriteOffsetWString(ResourcePath);
            var nodeOffsetOffset = handler.Tell();
            handler.Skip(24); // nodeOffset, linksOffset, nameOffset
            if (version > MotTreeVersion.MHR)
            {
                handler.Skip(8); // indicesOffset
            }
            var idTableOffset = handler.Tell();
            handler.Skip(8);

            handler.Write((short)Nodes.Count);
            handler.Write((short)Links.Count);
            handler.Write(ref uknCount1);
            handler.Write(ref uknCount2);
            handler.Write((short)Nodes.Count(n => n.typeName == "MotionNode")); // basicMotionsCount
            handler.Write(ref expandedMotionsCount);
            handler.WriteNull(4);

            if (version > MotTreeVersion.MHR)
            {
                handler.Align(8);
                handler.Write(nodeOffsetOffset + 24, handler.Tell());
                Indices.Write(handler);
            }

            handler.Align(8);
            handler.Write(idTableOffset, handler.Tell());
            foreach (var ind in MotionIDRemaps)
            {
                if (version < MotTreeVersion.RE_RT) handler.Write((short)ind.target); else handler.Write(ind.target);
                if (version < MotTreeVersion.RE4) handler.Write((short)ind.source); else handler.Write(ind.source);
            }
            handler.Align(8);
            handler.Write(nodeOffsetOffset + 16, handler.Tell());
            handler.WriteWString(Name);

            handler.Align(8);
            handler.Write(nodeOffsetOffset, handler.Tell());
            foreach (var node in Nodes) node.Write(handler);
            foreach (var node in Nodes) node.WriteTypes(handler);
            foreach (var node in Nodes) node.WriteNames(handler);

            handler.Align(8);
            foreach (var node in Nodes) node.WriteParameters(handler);
            handler.AsciiStringTableFlush();
            handler.StringTableFlush();
            handler.Align(16);
            handler.OffsetContentTableFlush();

            handler.Align(16);
            handler.Write(nodeOffsetOffset + 8, handler.Tell());
            foreach (var link in Links) link.Write(handler);
            handler.Align(16);
            handler.OffsetContentTableFlush();

            return false;
        }

        public override string ToString() => Name;
    }
}
