using System.Numerics;
using RszTool.Common;
using RszTool.Efx.Structs.Common;
using RszTool.InternalAttributes;

namespace RszTool.Efx
{
    public enum EfxVersion
    {
        Unknown = 0,
        RE7,
        RE2,
        DMC5,
        RE3,
        MHRise,
        RE8,
        RERT,
        MHRiseSB,
        SF6,
        RE4,
        DD2,
        MHWilds,
    }

    [RszGenerate, RszVersionedObject(typeof(EfxVersion))]
    public partial class EfxHeader : BaseModel
    {
        public int magic = EfxFile.Magic;
        public int ukn; // re7: 0,1  vfx\vfx_resource\vfx_effecteditor\efd_character_id\efd_em3600\vfx_efd_bh7_em3600_1008.efx.1179750
        public int entryCount;
        public int stringTableLength;
        public int actionCount;
        public int fieldParameterCount;
        public int expressionParameterCount;
        [RszVersion(">", EfxVersion.RE7, EndAt = nameof(effectGroupsLength))]
        public int effectGroupsCount;
        public int effectGroupsLength;
        [RszVersion("<=", EfxVersion.RE7)]
        public int expressionParameterSize;
        [RszVersion(">", EfxVersion.DMC5, EndAt = nameof(uknFlag))]
        public int boneCount;
        public int boneAttributeEntryCount;
        public int uknFlag;

        public EfxVersion Version { get; }

        public EfxHeader(EfxVersion version)
        {
            Version = version;
        }

        protected override bool DoRead(FileHandler handler)
        {
            DefaultRead(handler);
            if (magic != EfxFile.Magic)
            {
                throw new Exception("Invalid EFX file");
            }

            return true;
        }

        protected override bool DoWrite(FileHandler handler) => DefaultWrite(handler);
    }

    public enum EfxExpressionParameterType // TODO: enum confirmed "not wrong" with dmc5 and re4, dd2, what about the rest?
    {
        /// <summary>
        /// A single float value.
        /// </summary>
        Float = 0,
        /// <summary>
        /// A single color value (stored as a 32 bit value on the first field).
        /// </summary>
        Color = 1,
        /// <summary>
        /// 3 float values - X seems to always be within the Y-Z range, so it's probably {X = InitialValue, Y = MinValue, Z = MaxValue}.
        /// </summary>
        Range = 2,
        /// <summary>
        /// Seems to always be a single float value, same as <see cref="Float"/>, though I've only found 0.0 and 1.0 cases here.
        /// </summary>
        Float2 = 3,
    }

    public class Strings : BaseModel
    {
        public string[] ExpressionParameterNames = Array.Empty<string>();
        public string[]? BoneNames;
        public string[] ActionNames = Array.Empty<string>();
        public string[] FieldParameterNames = Array.Empty<string>();
        public string[] EfxNames = Array.Empty<string>();
        public string[] GroupNames = Array.Empty<string>();

        public EfxHeader Header { get; }

        public Strings(EfxHeader header)
        {
            Header = header;
        }

        private string[] ReadStrings(int count, FileHandler handler, bool hasUnicodePairs)
        {
            var list = new string[count];
            for (int i = 0; i < count; ++i)
            {
                if (hasUnicodePairs)
                {
                    var ascii = handler.ReadAsciiString(-1, -1, false);
                    list[i] = handler.ReadWString(-1, -1, false);
                }
                else
                {
                    list[i] = handler.ReadUTF8String(-1, false);
                }
            }
            return list;
        }

        private void WriteStrings(string[]? list, FileHandler handler, bool asciiUnicodePairs)
        {
            if (list == null) return;

            foreach (var str in list)
            {
                if (asciiUnicodePairs)
                {
                    handler.WriteAsciiString(str);
                    handler.WriteWString(str);
                }
                else
                {
                    handler.WriteUTF8String(str);
                }
            }
        }

        protected override bool DoRead(FileHandler handler)
        {
            ExpressionParameterNames = ReadStrings(Header.expressionParameterCount, handler, Header.Version > EfxVersion.RE7);
            BoneNames = ReadStrings(Header.boneCount, handler, true);
            if (Header.Version > EfxVersion.RE7) ActionNames = ReadStrings(Header.actionCount, handler, false);
            FieldParameterNames = ReadStrings(Header.fieldParameterCount, handler, Header.Version <= EfxVersion.RE7);
            EfxNames = ReadStrings(Header.entryCount, handler, false);
            GroupNames = ReadStrings(Header.effectGroupsCount, handler, false);
            if (Header.Version <= EfxVersion.RE7) ActionNames = ReadStrings(Header.actionCount, handler, false);
            return true;
        }

        protected override bool DoWrite(FileHandler handler)
        {
            WriteStrings(ExpressionParameterNames, handler, Header.Version > EfxVersion.RE7);
            WriteStrings(BoneNames, handler, true);
            if (Header.Version > EfxVersion.RE7) WriteStrings(ActionNames, handler, false);
            WriteStrings(FieldParameterNames, handler, Header.Version <= EfxVersion.RE7);
            WriteStrings(EfxNames, handler, false);
            WriteStrings(GroupNames, handler, false);
            if (Header.Version <= EfxVersion.RE7) WriteStrings(ActionNames, handler, false);
            return true;
        }
    }

    public enum EfxEntryEnum {
        AssignToCollisionEffect = 0,
        Root = 1,
        NoAssignment = 2,
    }


    public abstract class EFXAttribute : BaseModel
    {
        public EfxAttributeType type;
        public int unknSeqNum;
        public via.Int3 NodePosition => new via.Int3() {
            x = (unknSeqNum % 0xff),
            y = (unknSeqNum % 0xff00) >> 8,
            z = (unknSeqNum & 0xff0000) >> 16,
        };

        public EfxVersion Version;

        public static EFXAttribute Create(EfxVersion version, EfxAttributeType type, int seqNum = -1)
        {
            var item = EfxAttributeTypeRemapper.Create(type, version);
            if (item == null) throw new ArgumentException($"Unsupported EFX attribute type {type}", nameof(type));

            item.type = type;
            if (seqNum >= 0) item.unknSeqNum = seqNum;
            return item;
        }

        protected EFXAttribute(EfxAttributeType type)
        {
        }

        public override string ToString() => type.ToString();
    }

    public class EFXEntry : BaseModel
    {
        public EfxVersion Version;

        public int index;
        public uint effectNameHash;
        public EfxEntryEnum entryAssignment;
        public string? name;
        public List<EFXAttribute> Attributes { get; } = new();
        public List<string> Groups { get; } = new();

        protected override bool DoRead(FileHandler handler)
        {
            handler.Read(ref index);
            handler.Read(ref effectNameHash);
            handler.Read(ref entryAssignment);
            var attributeCount = handler.Read<int>();
            for (int i = 0; i < attributeCount; ++i) {
                var type = Version.GetAttributeType(handler.Read<int>());
                var seqNum = handler.Read<int>();
                var attr = EFXAttribute.Create(Version, type, seqNum);
                attr.Version = Version;
                attr.Read(handler);
                Attributes.Add(attr);
            }
            return true;
        }

        protected override bool DoWrite(FileHandler handler)
        {
            handler.Write(ref index);
            effectNameHash = MurMur3HashUtils.GetUTF8Hash(name ?? string.Empty);
            handler.Write(ref effectNameHash);
            handler.Write(ref entryAssignment);
            handler.Write(Attributes.Count);
            foreach (var attr in Attributes) {
                handler.Write(Version.ToAttributeTypeID(attr.type));
                handler.Write(attr.unknSeqNum);
                attr.Write(handler);
            }
            return true;
        }
    }

    [RszGenerate, RszAutoReadWrite]
    public partial class EFXExpressionParameter : BaseModel
    {
        public uint expressionParameterNameUTF16Hash;
        public uint expressionParameterNameUTF8Hash;
        public EfxExpressionParameterType type;
        public float value1;
        public float value2;
        public float value3;
        [RszIgnore] public string? name;

        public via.Color Color
        {
            get => type == EfxExpressionParameterType.Color ? new via.Color() { rgba = (uint)MemoryUtils.SingleToInt32(value1) } : throw new Exception("Expression parameter is not a color");
            set {
                type = EfxExpressionParameterType.Color;
                value1 = MemoryUtils.Int32ToSingle((int)value.rgba);
            }
        }

        public Vector3 Range
        {
            get => type == EfxExpressionParameterType.Range ? new Vector3(value1, value2, value3) : throw new Exception("Expression parameter is not a range");
            set {
                type = EfxExpressionParameterType.Range;
                value1 = value.X;
                value2 = value.Y;
                value3 = value.Z;
            }
        }
    }

    internal struct EFXBoneNameValuePair
    {
        public uint nameHash;
        public uint value;
    }

    public class EFXAction : BaseModel
    {
        public int actionUnkn0;
        public uint actionNameHash;
        public int actionAttributeCount;
        public List<EFXAttribute> Attributes { get; } = new();

        public string? name;

        public EfxVersion Version;

        protected override bool DoRead(FileHandler handler)
        {
            handler.Read(ref actionUnkn0);
            handler.Read(ref actionNameHash);
            handler.Read(ref actionAttributeCount);
            for (int i = 0; i < actionAttributeCount; ++i) {
                var type = Version.GetAttributeType(handler.Read<int>());
                var seqNum = handler.Read<int>();
                var attr = EFXAttribute.Create(Version, type, seqNum);
                attr.Version = Version;
                attr.Read(handler);
                Attributes.Add(attr);
            }
            return true;
        }

        protected override bool DoWrite(FileHandler handler)
        {
            actionAttributeCount = Attributes.Count;
            actionNameHash = MurMur3HashUtils.GetUTF8Hash(name ?? string.Empty); // TODO RE7: different hash (ascii?)

            handler.Write(ref actionUnkn0);
            handler.Write(ref actionNameHash);
            handler.Write(ref actionAttributeCount);
            foreach (var attr in Attributes) {
                handler.Write(Version.ToAttributeTypeID(attr.type));
                handler.Write(attr.unknSeqNum);
                attr.Write(handler);
            }
            return true;
        }
    }

    [RszGenerate]
    public partial class EFXFieldParameterValue : BaseModel
    {
        [RszIgnore] public EfxVersion Version;

        public uint unkn0;
        public uint fieldParameterNameHash;
        public uint unkn2;
        public uint type;
        public uint unkn4;
        public int value_ukn1;
        [RszIgnore] public uint value_ukn2;
        [RszIgnore] public uint value_ukn3;
        [RszIgnore] public float value_ukn4;
        [RszIgnore] public float value_ukn5;
        [RszIgnore] public float value_ukn6;
        [RszIgnore] public string? name;
        [RszIgnore] public string? filePath;

        protected override bool DoRead(FileHandler handler)
        {
            DefaultRead(handler);
            if (type == 196) {
                filePath = handler.ReadWString(-1, value_ukn1, false);
            } else if (Version > EfxVersion.RE7) {
                handler.Read(ref value_ukn2);
                handler.Read(ref value_ukn3);
                handler.Read(ref value_ukn4);
                handler.Read(ref value_ukn5);
                handler.Read(ref value_ukn6);
                if (type is 110 or 144 or 183 or 184 or 202 or 194 or 215) {
                    filePath = handler.ReadWString(-1, handler.Read<int>(), false);
                }
            }
            return true;
        }

        protected override bool DoWrite(FileHandler handler)
        {
            if (type == 196) {
                filePath ??= string.Empty;
                value_ukn1 = filePath.Length + 2;
                DefaultWrite(handler);
                handler.WriteWString(filePath);
            } else {
                DefaultWrite(handler);
                handler.Write(ref value_ukn2);
                handler.Write(ref value_ukn3);
                handler.Write(ref value_ukn4);
                handler.Write(ref value_ukn5);
                handler.Write(ref value_ukn6);
                if (type is 110 or 144 or 183 or 184 or 202 or 194 or 215) {
                    filePath ??= "";
                    handler.Write(filePath.Length);
                    handler.WriteWString(filePath);
                }
            }
            return true;
        }
    }

    [RszGenerate, RszAutoReadWrite]
    public partial class EffectGroup : BaseModel
    {
        [RszStringHash(nameof(conditionalEffectGroupName))] public uint conditionalEffectGroupNameHashUTF16;
        [RszStringUTF8Hash(nameof(conditionalEffectGroupName))] public uint conditionalEffectGroupNameHashUTF8;
        [RszArraySizeField(nameof(efxEntryIndexes))] public int valueCount;
        [RszFixedSizeArray(nameof(valueCount))] public int[]? efxEntryIndexes;
        [RszIgnore] public string? conditionalEffectGroupName;
    }

    public class EFXBone
    {
        public string name = string.Empty;
        public uint value;
    }

    public interface IBoneRelationAttribute
    {
        string? ParentBone { get; set; }
    }

    public interface IExpressionAttribute
    {
        EFXExpressionContainer? Expression { get; }
    }

    public interface IExpressionContainer
    {
        int ExpressionCount { get; }
        IEnumerable<IExpressionObject> Expressions { get; }
    }

    public interface IExpressionObject
    {
        IList<EFXExpressionData> Components { get; }
    }
}

namespace RszTool
{
    using RszTool.Efx;
    using RszTool.Efx.Structs.RE4;

    public partial class EfxFile : BaseFile
    {
        public List<EFXEntry> Entries { get; } = new();
        public List<EFXBone> Bones { get; } = new();

        public EfxHeader? Header;
        public Strings? Strings;

        public List<short> BoneRelations { get; } = new();
        public List<EFXExpressionParameter> ExpressionParameters = new();
        public List<EFXAction> Actions = new();
        public List<EFXFieldParameterValue> FieldParameterValues = new();
        private List<EffectGroup>? _effectGroups;
        public List<string> UvarStrings = new();
        /// <summary>
        /// RE7
        /// </summary>
        public int[]? expressionData;

        public const int Magic = 0x72786665;

        public EfxFile(FileHandler fileHandler) : base(fileHandler)
        {
        }

        private const int VERSION_RE7 = 1179750;
        private const int VERSION_RE2 = 1769669;
        private const int VERSION_DMC5 = 1769672;
        private const int VERSION_RE3 = 2228526;
        private const int VERSION_MHR = 2621987;
        private const int VERSION_RE8 = 2621998;
        private const int VERSION_RERT = 2818689;
        private const int VERSION_MHRSB = 2818730;
        private const int VERSION_SF6 = 3474371;
        private const int VERSION_RE4 = 3539837;
        private const int VERSION_DD2 = 4064419;
        private const int VERSION_WILDS = 5571972;

        public static EfxVersion GetEfxVersion(int fileVersion) => fileVersion switch {
            VERSION_RE7 => EfxVersion.RE7,
            VERSION_RE2 => EfxVersion.RE2,
            VERSION_DMC5 => EfxVersion.DMC5,
            VERSION_RE3 => EfxVersion.RE3,
            VERSION_MHR => EfxVersion.MHRise,
            VERSION_RE8 => EfxVersion.RE8,
            VERSION_RERT => EfxVersion.RERT,
            VERSION_MHRSB => EfxVersion.MHRiseSB,
            VERSION_SF6 => EfxVersion.SF6,
            VERSION_RE4 => EfxVersion.RE4,
            VERSION_DD2 => EfxVersion.DD2,
            VERSION_WILDS => EfxVersion.MHWilds,
            _ => EfxVersion.Unknown,
        };
        public static int GetFileVersion(EfxVersion version) => version switch {
            EfxVersion.RE7 => VERSION_RE7,
            EfxVersion.RE2 => VERSION_RE2,
            EfxVersion.DMC5 => VERSION_DMC5,
            EfxVersion.RE3 => VERSION_RE3,
            EfxVersion.MHRise => VERSION_MHR,
            EfxVersion.RE8 => VERSION_RE8,
            EfxVersion.RERT => VERSION_RERT,
            EfxVersion.MHRiseSB => VERSION_MHRSB,
            EfxVersion.SF6 => VERSION_SF6,
            EfxVersion.RE4 => VERSION_RE4,
            EfxVersion.DD2 => VERSION_DD2,
            EfxVersion.MHWilds => VERSION_WILDS,
            _ => -1,
        };
        public static EfxVersion[] AllVersions => (EfxVersion[])Enum.GetValues(typeof(EfxVersion));

        protected override bool DoRead()
        {
            var handler = FileHandler;
            Header = new EfxHeader(GetEfxVersion(FileHandler.FileVersion));
            Header.Read(handler);
            Strings = new Strings(Header);
            Strings.Read(handler);

            if (Header.expressionParameterSize == 0)
            {
                for (int i = 0; i < Header.expressionParameterCount; ++i) {
                    var param = new EFXExpressionParameter();
                    param.name = Strings.ExpressionParameterNames[i];
                    param.Read(handler);
                    ExpressionParameters.Add(param);
                }
            }

            for (int i = 0; i < Header.boneCount; ++i) {
                var data = handler.Read<EFXBoneNameValuePair>();
                var boneName = Strings.BoneNames![i];
                Bones.Add(new EFXBone() {
                    name = boneName,
                    value = data.value,
                });
            }
            BoneRelations.Clear();
            for (int i = 0; i < Header.boneAttributeEntryCount; ++i) BoneRelations.Add(handler.Read<short>());

            if (Header.Version > EfxVersion.RE7) {
                ReadActions(handler);
            }

            for (int i = 0; i < Header.fieldParameterCount; ++i) {
                var entry = new EFXFieldParameterValue();
                entry.Version = Header.Version;
                entry.name = Strings.FieldParameterNames[i];
                entry.Read(handler);
                FieldParameterValues.Add(entry);
            }
            if (Header.Version <= EfxVersion.RE7) {
                ReadActions(handler);
            }

            Entries.Clear();
            for (int i = 0; i < Header.entryCount; ++i) {
                var entry = new EFXEntry() { Version = Header.Version };
                entry.name = Strings.EfxNames[i];
                entry.Read(handler);
                Entries.Add(entry);
            }

            for (int i = 0; i < Header.effectGroupsCount; ++i) {
                var effect = new EffectGroup();
                var groupName = Strings.GroupNames[i];
                effect.Read(handler);
                if (effect.efxEntryIndexes == null) continue;
                foreach (var index in effect.efxEntryIndexes) {
                    Entries[index].Groups.Add(groupName);
                }
            }

            if (Header.Version > EfxVersion.DMC5) {
                var uvarCount = handler.Read<int>();
                // note: found these as either 0 or 1 in DD2, sometimes other numbers too
                // always 0 for RE4,DMC5,RERT

                if (uvarCount > 1) {
                    throw new Exception("Found more UVARs than we know how to handle");
                }
                var uvarType = handler.Read<int>();
                // uvarType == 1 => no extra data
                if (uvarType == 2) {
                    if (handler.Position != handler.Stream.Length - 12) {
                        throw new Exception("Uvar handling is not OK?");
                    }
                    handler.Read<int>();
                    handler.Read<int>();
                    handler.Read<int>();
                } else if (uvarType > 2) {
                    throw new Exception("Found unhandled uvar type? " + uvarType);
                }
            } else if (Header.expressionParameterSize > 0) {
                expressionData = handler.ReadArray<int>(Header.expressionParameterSize);
            }

            if (Header.Version > EfxVersion.DMC5)
            {
                SetupBoneReferences();
            }
            // ParseExpressions();
            return true;
        }

        public void ParseExpressions()
        {
            foreach (var entry in Entries) {
                foreach (var attr in entry.Attributes.OfType<IExpressionAttribute>()) {
                    if (attr.Expression != null) {
                        attr.Expression.ParsedExpressions = ParseExpression(attr.Expression);
                    }
                }
            }

            foreach (var action in Actions) {
                foreach (var a in action.Attributes.OfType<EFXAttributePlayEmitter>()) {
                    a.efxrData?.ParseExpressions();
                }
            }
        }

        public List<EFXExpressionTree> ParseExpression(EFXExpressionContainer container)
        {
            var list = new List<EFXExpressionTree>();
            foreach (var expr in container.Expressions) {
                var tree = BuildExpressionTree(expr);
                list.Add(tree);
                if (expr.Components.Count > 0) {
                    Console.WriteLine("Expression: " + tree);
                }
            }
            return list;
        }

        public EFXExpressionTree BuildExpressionTree(IExpressionObject expression)
        {
            var tree = new EFXExpressionTree();
            var comps = expression.Components;
            if (comps.Count == 0) return tree;

            var i = comps.Count - 1;
            tree.root = EvalExpressionAtom(comps, ref i);
            if (i >= 0) {
                // two DMC5 files fail this due to having an extra float at the start - Capcom user error?
                // throw new Exception("Incomplete expression!");
            }

            return tree;
        }

        public static readonly HashSet<uint> UnknownParameterHashes = new();
        private static readonly Dictionary<uint, string> KnownExternalHashes = new() {
            [4068760923] = "PI",
            [2715150528] = "speed",
            [4102950055] = "Speed",
            [3351123696] = "scale",
            [2589222962] = "TIMER",
            [124194757] = "alpha",
            [1124845316] = "Color",
            [2521345197] = "color",
            [1703575233] = "pitch",
            [3888230594] = "yaw",
            [2703807999] = "roll",
        };

        private ExpressionAtom EvalExpressionAtom(IList<EFXExpressionData> components, ref int index)
        {
            var comp = components[index--];
            if (comp.data is EFXExpressionDataParameterHash param) {
                // note: we could optimize this by also using the "parameters" array to see what kind of parameter it is
                // for now just bruteforcing for simplicity
                var exprParam = ExpressionParameters.FirstOrDefault(exp => exp.expressionParameterNameUTF8Hash == param.parameterHash);
                if (exprParam != null) {
                    return new ExpressionParameter() { hash = param.parameterHash, name = exprParam.name };
                }
                var fieldParam = FieldParameterValues.FirstOrDefault(exp => exp.fieldParameterNameHash == param.parameterHash);
                if (fieldParam != null) {
                    return new ExpressionParameter() { hash = param.parameterHash, name = fieldParam.name };
                }
                if (KnownExternalHashes.TryGetValue(param.parameterHash, out var name)) {
                    return new ExpressionParameter() { hash = param.parameterHash, name = name };
                }
                UnknownParameterHashes.Add(param.parameterHash);
                return new ExpressionParameter() { hash = param.parameterHash };
            }
            if (comp.data is EFXExpressionDataBinaryOperator binary) {
                var item = new ExpressionBinaryOperation();
                item.oper = binary.value;
                item.left = EvalExpressionAtom(components, ref index);
                item.right = EvalExpressionAtom(components, ref index);
                return item;
            }
            if (comp.data is EFXExpressionDataFloat floatType) {
                return new ExpressionFloat() { value = floatType.value };
            }
            if (comp.data is EFXExpressionDataFunction intType) {
                if (intType.value is EfxExpressionFunction.Ternary15 or EfxExpressionFunction.Ternary16 or EfxExpressionFunction.Ternary17) {
                    var item = new ExpressionTernaryOperation();
                    item.oper = intType.value;
                    item.left = EvalExpressionAtom(components, ref index);
                    item.arg2 = EvalExpressionAtom(components, ref index);
                    item.arg3 = EvalExpressionAtom(components, ref index);
                    return item;
                } else {
                    // other found values: 0, 1, 2, 4, 5, 6, 7, 8, 9, 10
                    var item = new ExpressionUnaryOperation();
                    item.oper = ((int)intType.value);
                    item.atom = EvalExpressionAtom(components, ref index);
                    return item;
                }
            }

            if (comp.data is EFXExpressionDataUnaryOperator unary) {
                if (unary.value != 0) {
                    throw new Exception("Found non-0 type 2 efx expression data");
                }
                var item = new ExpressionNegation();
                item.atom = EvalExpressionAtom(components, ref index);
                return item;
            }

            throw new Exception("Found unsupported expression component");
        }

        private void ReadActions(FileHandler handler)
        {
            for (int i = 0; i < Header!.actionCount; ++i) {
                var action = new EFXAction() { Version = Header.Version };
                action.name = Strings!.ActionNames[i];
                action.Read(handler);
                foreach (var a in action.Attributes.OfType<EFXAttributePlayEmitter>()) {
                    if (a.efxrData != null) {
                        a.efxrData.Bones.AddRange(Bones);
                        a.efxrData.SetupBoneReferences();
                    }
                }
                Actions.Add(action);
            }
        }

        private void SetupBoneReferences()
        {
            if (Bones.Count == 0) return;

            int index = 0;
            foreach (var entry in Entries) {
                foreach (var attr in entry.Attributes) {
                    if (attr is IBoneRelationAttribute parented) {
                        var parentBoneIndex = BoneRelations[index++];
                        parented.ParentBone = parentBoneIndex == -1 ? null : Bones[parentBoneIndex].name;
                    }
                }
            }
        }

        private void UpdateEffectGroups()
        {
            var dict = new Dictionary<string, List<int>>();
            for (var i = 0; i < Entries.Count; i++)
            {
                foreach (var grp in Entries[i].Groups)
                {
                    if (!dict.TryGetValue(grp, out var ids)) {
                        dict[grp] = ids = new List<int>();
                    }

                    ids.Add(i);
                }
            }

            _effectGroups = dict.Select(kv => new EffectGroup() {
                conditionalEffectGroupName = kv.Key,
                valueCount = kv.Value.Count,
                efxEntryIndexes = kv.Value.ToArray(),
                conditionalEffectGroupNameHashUTF16 = MurMur3HashUtils.GetHash(kv.Key),
                conditionalEffectGroupNameHashUTF8 = MurMur3HashUtils.GetAsciiHash(kv.Key)
            }).ToList();
        }

        private void UpdateHeaderData(EfxVersion version)
        {
            if (Header == null || Header.Version != version) {
                Header = new EfxHeader(version);
                Strings = new(Header);
            }

            Header.expressionParameterCount = ExpressionParameters.Count;
            Header.boneCount = Bones.Count;
            Header.entryCount = Entries.Count;
            Header.effectGroupsCount = _effectGroups?.Count ?? 0;
            Header.actionCount = Actions.Count;
            Header.fieldParameterCount = FieldParameterValues.Count;
            Header.boneAttributeEntryCount = Bones.Count;

            Strings ??= new(Header);

            Strings.EfxNames = Entries.Select(e => e.name ?? string.Empty).ToArray();
            Strings.GroupNames = _effectGroups?.Select(e => e.conditionalEffectGroupName ?? string.Empty).ToArray()
                ?? Array.Empty<string>();
            Strings.BoneNames = Bones.Select(b => b.name ?? string.Empty).ToArray();
            Strings.ActionNames = Actions.Select(a => a.name ?? string.Empty).ToArray();
            Strings.FieldParameterNames = FieldParameterValues.Select(a => a.name ?? string.Empty).ToArray();
            Strings.ExpressionParameterNames = ExpressionParameters.Select(a => a.name ?? string.Empty).ToArray();
        }

        protected override bool DoWrite()
        {
            if (Header == null) return false;
            var handler = FileHandler;

            UpdateEffectGroups();
            UpdateHeaderData(Header.Version);

            Header.Write(handler);
            long writeStart = handler.Tell();
            Strings!.Write(handler);
            Header.stringTableLength = (int)(handler.Tell() - writeStart);

            foreach (var exprParam in ExpressionParameters) {
                exprParam.expressionParameterNameUTF16Hash = MurMur3HashUtils.GetHash(exprParam.name ?? string.Empty);
                exprParam.expressionParameterNameUTF8Hash = MurMur3HashUtils.GetUTF8Hash(exprParam.name ?? string.Empty);
                exprParam.Write(handler);
            }

            if (Header.Version > EfxVersion.DMC5) {
                foreach (var bone in Bones) {
                    var pair = new EFXBoneNameValuePair() {
                        nameHash = MurMur3HashUtils.GetAsciiHash(bone.name),
                        value = bone.value,
                    };
                    handler.Write(ref pair);
                }

                Header.boneAttributeEntryCount = 0;
                foreach (var entry in Entries) {
                    foreach (var attr in entry.Attributes) {
                        if (attr is IBoneRelationAttribute parented) {
                            var index = string.IsNullOrEmpty(parented.ParentBone) ? -1 : Bones.FindIndex(b => b.name == parented.ParentBone);
                            handler.Write((short)index);
                            Header.boneAttributeEntryCount++;
                        }
                    }
                }
            }

            Actions.Write(handler);

            FieldParameterValues.Write(handler);
            Entries.Write(handler);

            writeStart = handler.Tell();
            _effectGroups?.Write(handler);
            Header.effectGroupsLength = (int)(handler.Tell() - writeStart);

            if (Header.Version > EfxVersion.DMC5) {
                handler.Write(0); // Uvar count
                handler.Write(0); // 0x00 EOF bytes x4
            }

            var endPosition = handler.Tell();
            handler.Seek(0);

            // write header again to update the length params
            Header.Write(handler);
            handler.Seek(endPosition);
            return true;
        }
    }
}