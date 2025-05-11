using System.Reflection;
using RszTool.Common;
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
        [RszVersion(">", EfxVersion.RE7, EndAt = nameof(collisionEffectLength))]
        public int collisionEffectCount;
        public int collisionEffectLength;
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

    public class Strings : BaseModel
    {
        public string[] ExpressionParameterNames = Array.Empty<string>();
        public string[]? BoneNames;
        public string[] ActionNames = Array.Empty<string>();
        public string[] FieldParameterNames = Array.Empty<string>();
        public string[] EfxNames = Array.Empty<string>();
        public string[] CollisionEffectNames = Array.Empty<string>();

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
            CollisionEffectNames = ReadStrings(Header.collisionEffectCount, handler, false);
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
            WriteStrings(CollisionEffectNames, handler, false);
            if (Header.Version <= EfxVersion.RE7) WriteStrings(ActionNames, handler, false);
            return true;
        }
    }

    public enum EfxEntryEnum {
        AssignToCollisionEffect = 0,
        Unknown1 = 1,
        NoAssignment = 2,
    }


    public abstract class EFXAttribute : BaseModel
    {
        public EfxAttributeType type;
        public int unknSeqNum;

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
    }

    public class EFXEntry : BaseModel
    {
        public EfxVersion Version;

        public int index;
        public uint effectNameHash;
        public EfxEntryEnum entryAssignment;
        public string? name;
        public List<EFXAttribute> Attributes { get; } = new();

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
        public uint unkn1;
        public int unkn2;
        public uint unkn3;
        public uint unkn4;
        [RszIgnore] public string? name;
    }

    internal struct EFXBoneNameValuePair
    {
        public uint nameHash;
        public uint value;
    }

    public class EFXAction : BaseModel
    {
        public uint actionUnkn0;
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
        public uint unkn5;
        [RszConditional(nameof(Version), ">", EfxVersion.RE7, EndAt = nameof(unkn10))]
        public uint unkn6;
        public uint unkn7;
        public float unkn8;
        public float unkn9;
        public float unkn10;
        [RszIgnore] public string? name;
        [RszIgnore] public string? filePath;

        protected override bool DoRead(FileHandler handler)
        {
            DefaultRead(handler);
            if (type is 110 or 183 or 184 or 202 or 194 or 215) {
                filePath = handler.ReadWString(-1, handler.Read<int>(), false);
            }
            return true;
        }

        protected override bool DoWrite(FileHandler handler)
        {
            DefaultWrite(handler);
            if (type is 110 or 184 or 202 or 194) {
                filePath ??= "";
                handler.Write(filePath.Length);
                handler.WriteWString(filePath);
            }
            return true;
        }
    }

    [RszGenerate, RszAutoReadWrite]
    public partial class CollisionEffect : BaseModel
    {
        [RszStringHash(nameof(conditionalEffectGroupName))] public uint conditionalEffectGroupNameHashUTF16;
        [RszStringUTF8Hash(nameof(conditionalEffectGroupName))] public uint conditionalEffectGroupNameHashUTF8;
        [RszArraySizeField(nameof(efxEntryIndexes))] public int valueCount;
        [RszFixedSizeArray(nameof(valueCount))] public uint[]? efxEntryIndexes;
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
}

namespace RszTool
{
    using RszTool.Efx;

    public partial class EfxFile : BaseFile
    {
        public List<EFXEntry> Entries { get; } = new();
        public List<EFXBone> Bones { get; } = new();

        public EfxHeader? Header;
        public Strings? Strings;

        public List<EFXExpressionParameter> ExpressionParameters = new();
        public List<EFXAction> Actions = new();
        public List<EFXFieldParameterValue> FieldParameterValues = new();
        public List<CollisionEffect> CollisionEffects = new();
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
            var boneRelations = new List<short>();
            for (int i = 0; i < Header.boneAttributeEntryCount; ++i) boneRelations.Add(handler.Read<short>());

            if (Header.Version > EfxVersion.RE7) {
                for (int i = 0; i < Header.actionCount; ++i) {
                    var action = new EFXAction() { Version = Header.Version };
                    action.name = Strings.ActionNames[i];
                    action.Read(handler);
                    Actions.Add(action);
                }
            }
            for (int i = 0; i < Header.fieldParameterCount; ++i) {
                var entry = new EFXFieldParameterValue();
                entry.Version = Header.Version;
                entry.name = Strings.FieldParameterNames[i];
                entry.Read(handler);
                FieldParameterValues.Add(entry);
            }
            if (Header.Version <= EfxVersion.RE7) {
                for (int i = 0; i < Header.actionCount; ++i) {
                    var action = new EFXAction() { Version = Header.Version };
                    action.name = Strings.ActionNames[i];
                    action.Read(handler);
                    Actions.Add(action);
                }
            }
            Entries.Clear();
            for (int i = 0; i < Header.entryCount; ++i) {
                var entry = new EFXEntry() { Version = Header.Version };
                entry.name = Strings.EfxNames[i];
                entry.Read(handler);
                Entries.Add(entry);
            }

            for (int i = 0; i < Header.collisionEffectCount; ++i) {
                var effect = new CollisionEffect();
                effect.conditionalEffectGroupName = Strings.CollisionEffectNames[i];
                effect.Read(handler);
                CollisionEffects.Add(effect);
            }

            if (Header.Version > EfxVersion.DMC5) {
                var uvarCount = handler.Read<int>();
                // note: found these as either 0 or 1 in DD2, sometimes other numbers too
                // always 0 for RE4,DMC5,RERT

                // if (uvarCount != 0) {
                //     throw new Exception("Unexpected non-0 uvar count at EFX EOF");
                // }
                var end = handler.Read<int>();
                // if (end != 0) {
                //     throw new Exception("Unexpected non-0 at EFX EOF");
                // }
            } else if (Header.expressionParameterSize > 0) {
                expressionData = handler.ReadArray<int>(Header.expressionParameterSize);
            }

            SetupReferences(boneRelations);
            return true;
        }

        private void SetupReferences(List<short> boneRelations)
        {
            int index = 0;
            foreach (var entry in Entries) {
                foreach (var attr in entry.Attributes) {
                    if (attr is IBoneRelationAttribute parented) {
                        var parentBoneIndex = boneRelations[index++];
                        parented.ParentBone = parentBoneIndex == -1 ? null : Bones[parentBoneIndex].name;
                    }
                }
            }
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
            Header.collisionEffectCount = CollisionEffects.Count;
            Header.actionCount = Actions.Count;
            Header.fieldParameterCount = FieldParameterValues.Count;
            Header.boneAttributeEntryCount = Bones.Count;

            Strings ??= new(Header);

            Strings.EfxNames = Entries.Select(e => e.name ?? string.Empty).ToArray();
            Strings.CollisionEffectNames = CollisionEffects.Select(e => e.conditionalEffectGroupName ?? string.Empty).ToArray();
            Strings.BoneNames = Bones.Select(b => b.name ?? string.Empty).ToArray();
            Strings.ActionNames = Actions.Select(a => a.name ?? string.Empty).ToArray();
            Strings.FieldParameterNames = FieldParameterValues.Select(a => a.name ?? string.Empty).ToArray();
            Strings.ExpressionParameterNames = ExpressionParameters.Select(a => a.name ?? string.Empty).ToArray();
        }

        protected override bool DoWrite()
        {
            if (Header == null) return false;
            var handler = FileHandler;

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
            CollisionEffects.Write(handler);
            Header.collisionEffectLength = (int)(handler.Tell() - writeStart);

            if (Header.Version > EfxVersion.DMC5) {
                handler.Write(0); // Uvar count
                handler.Write(0); // 0x00 EOF bytes x4
            }
            handler.Seek(0);

            // write header again to update the length params
            Header.Write(handler);

            return true;
        }
    }
}