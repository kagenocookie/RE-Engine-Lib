using System.Numerics;
using RszTool.Efx.Structs.Common;
using RszTool.InternalAttributes;

namespace RszTool.Efx.Structs.RE4;

public enum BlendType
{
    AlphaBlend = 0,
    Physical = 1,
    AddContrast = 2,
    EdgeBlend = 3,
    Multiply = 4,
}

[RszGenerate, RszAutoReadWrite, RszVersionedObject(typeof(EfxVersion)), EfxStruct(EfxAttributeType.UnknownSB_195, EfxVersion.MHRiseSB)]
public partial class EFXAttributeunknSBStruct195 : EFXAttribute
{
    public EFXAttributeunknSBStruct195() : base(EfxAttributeType.UnknownSB_195) { }

    public uint unkn1;
    [RszVersion(EfxVersion.RE4)]
    public uint re4_unkn0;
    public float unkn2_0;
	[RszArraySizeField(nameof(substruct))] public int substructCount;
    [RszFixedSizeArray(nameof(substructCount))] public EFXAttributeunknSBStruct195_Substruct[]? substruct;

    public float unkn13;
    public float unkn14;
    public float unkn15;

    public struct EFXAttributeunknSBStruct195_Substruct
    {
        public float unkn0;
        public float unkn1;
        public float unkn2;
        public float unkn3;
        public float unkn4;
        public float unkn5;
        public float unkn6;
        public float unkn7;
        public float unkn11;
        public float unkn12;
        public float unkn13;
    }
}

[RszGenerate, RszAutoReadWrite, RszVersionedObject(typeof(EfxVersion)), EfxStruct(EfxAttributeType.UnknownRE4_114TypeStrainRibbon, EfxVersion.RE4)]
public partial class EFXAttributeUnknownRE4_114 : EFXAttribute
{
	public EFXAttributeUnknownRE4_114() : base(EfxAttributeType.UnknownRE4_114TypeStrainRibbon) { }

    public uint unkn0_0;
    public uint unkn0_1;
    public uint unkn0_2;
    public uint unkn0_3;
    public float unkn1_0;
    public float unkn1_1;
    public float unkn1_2;
    public int unkn2;
    public float unkn3_0;
    public float unkn3_1;
    public float unkn3_2;
    public float unkn3_3;
    public float unkn3_4;
    public uint unkn3_5;
    public uint unkn4_0;
    public float unkn4_1;
    public float unkn4_2;
    public via.Color color0;
    public via.Color color1;
    public via.Color color2;

	public float unkn5_0;
    public float unkn5_1;
    public float unkn5_2;
    public float unkn5_3;
    public float unkn5_4;
    public float unkn5_5;
    public float unkn5_6;
    public float unkn5_7;
    public uint unkn5_8;
    public float unkn5_9;
    public float unkn5_10;
    public float unkn5_11;
    public float unkn5_12;
    public float unkn5_13;
    public float unkn5_14;
    public float unkn5_15;
    public float unkn5_16;
    public float unkn5_17;
    public float unkn5_18;
    public float unkn5_19;
    public float unkn5_20;
    public float unkn5_21;
    public float unkn5_22;
    public float unkn5_23;
    public uint unkn5_24;
    public float unkn5_25;
    public float unkn5_26;
    public uint unkn5_27;
    public float unkn5_28;
    public float unkn5_29;
    public float unkn5_30;
    public float unkn5_31;

    public int unkn6;
	public float unkn7_0;
    public float unkn7_1;
    public float unkn7_2;
    public float unkn7_3;
    public float unkn7_4;
    public float unkn7_5;
	[RszInlineWString] public string? boneName;
	public int unkn8_0;
	public uint unkn8_color1;
	public uint unkn8_color2;
	public float unkn8_3;
	public float unkn8_4;
	public int unkn8_5;
	public float unkn8_6;
}

[RszGenerate, RszAutoReadWrite, RszVersionedObject(typeof(EfxVersion)), EfxStruct(EfxAttributeType.UnknownRE4_195, EfxVersion.RE4, EfxVersion.DD2)]
public partial class EFXAttributeUnknownRE4_195 : EFXAttribute
{
	public EFXAttributeUnknownRE4_195() : base(EfxAttributeType.UnknownRE4_195) { }

    public float unkn0;
    public float unkn1;
    public float unkn2;
    public float unkn3;
    public uint unkn4;
    public uint unkn5;
	[RszVersion("!=", EfxVersion.DD2)]
    ushort unkn6;
}

[RszGenerate, RszAutoReadWrite, RszVersionedObject(typeof(EfxVersion)), EfxStruct(EfxAttributeType.UnknownRE4_197, EfxVersion.RE4)]
public partial class EFXAttributeUnknownRE4_197 : EFXAttribute
{
	public EFXAttributeUnknownRE4_197() : base(EfxAttributeType.UnknownRE4_197) { }

    public uint unkn0;
    public uint unkn1;
    public uint unkn2;
    public uint unkn3;
    public float unkn4;
    public float unkn5;
    public float unkn6;
    public float unkn7;
    public byte unkn8;
    public byte unkn9;
    public byte unkn10;
    public float unkn11;
    public float unkn12;
    public float unkn13;
    public float unkn14;
    public float unkn15;
    public float unkn16;
    public float unkn17;
    public float unkn18;
    public float unkn19;
    public byte unkn20;
}

[RszGenerate, RszAutoReadWrite, RszVersionedObject(typeof(EfxVersion)), EfxStruct(EfxAttributeType.UnknownRE4_213, EfxVersion.RE4)]
public partial class EFXAttributeUnknownRE4_213 : EFXAttribute
{
    public EFXAttributeUnknownRE4_213() : base(EfxAttributeType.UnknownRE4_213) { }

    public UndeterminedFieldType unkn1;
	[RszArraySizeField(nameof(substruct1))] public int substruct1Count;
    public float unkn2;
	[RszArraySizeField(nameof(substruct2))] public int substruct2Count;

    [RszFixedSizeArray(nameof(substruct1Count))] public UnknownRE4_213_Substruct1[]? substruct1;
    [RszFixedSizeArray(nameof(substruct2Count))] public UnknownRE4_213_Substruct2[]? substruct2;

    public struct UnknownRE4_213_Substruct1
    {
        public float unkn0;
        public float unkn1;
        public float unkn2;
        public float unkn3;
        public float unkn4;
    }

    public struct UnknownRE4_213_Substruct2
    {
        public float unkn0;
        public float unkn1;
        public float unkn2;
        public float unkn3;
        public float unkn4;
        public float unkn5;
        public float unkn6;
    }
}

[RszGenerate, RszAutoReadWrite, RszVersionedObject(typeof(EfxVersion)), EfxStruct(EfxAttributeType.UnknownRE4_226, EfxVersion.RE4)]
public partial class EFXAttributeUnknownRE4_226 : EFXAttribute
{
	public EFXAttributeUnknownRE4_226() : base(EfxAttributeType.UnknownRE4_226) { }

    public uint unkn0;
    public via.Color color0;
    public via.Color color1;
    public float unkn3;
    public float unkn4;
    public float unkn5;
    public via.Color color2;
    public via.Color color3;
    public float unkn8;
    public float unkn9;
    public float unkn10;
    public float unkn11;
    public float unkn12;
    public float unkn13;
    public float unkn14;
    public float unkn15;
    public float unkn16;
    public uint unkn17;
    public float unkn18;
    public float unkn19;
    public float unkn20;
    public float unkn21;
    public float unkn22;
    public float unkn23;
    public uint unkn24;
    public float unkn25;
}

[RszGenerate, RszAutoReadWrite, RszVersionedObject(typeof(EfxVersion)), EfxStruct(EfxAttributeType.UnknownRE4_228, EfxVersion.RE4)]
public partial class EFXAttributeUnknownRE4_228 : EFXAttribute
{
    public EFXAttributeUnknownRE4_228() : base(EfxAttributeType.UnknownRE4_228) { }

	public via.Color hash1;
	public float unkn0;
	public via.Color hash2;
	public float unkn1;
	public float unkn2;
	public float unkn3;

	public byte ukn_b1;
	public UndeterminedFieldType unkn4;
	public UndeterminedFieldType unkn5;
	public UndeterminedFieldType unkn6;
	public UndeterminedFieldType unkn7;
	public byte ukn_b2;
	public UndeterminedFieldType unkn8;
	public UndeterminedFieldType unkn9;
	public UndeterminedFieldType unkn10;
	public UndeterminedFieldType unkn11;

	public UndeterminedFieldType unkn12;
	public UndeterminedFieldType unkn13;
	public UndeterminedFieldType unkn14;
	public UndeterminedFieldType unkn15;
	public UndeterminedFieldType unkn16;
	public UndeterminedFieldType unkn17;

	public byte ukn_b3;
	public UndeterminedFieldType unkn18;
	public UndeterminedFieldType unkn19;
	public uint unkn20;
	public UndeterminedFieldType unkn21;
	public uint unkn22;
	public UndeterminedFieldType unkn23;
	public UndeterminedFieldType unkn24;
}

[RszGenerate, RszAutoReadWrite, RszVersionedObject(typeof(EfxVersion)), EfxStruct(EfxAttributeType.UnknownRE4_231EfCsv, EfxVersion.RE4)]
public partial class EFXAttributeUnknownRE4_231 : EFXAttribute
{
	public EFXAttributeUnknownRE4_231() : base(EfxAttributeType.UnknownRE4_231EfCsv) { }

    public ushort unkn0;
    public uint unkn1;
    public uint unkn2;
    [RszInlineWString] public string? efcsvPath;
    [RszInlineWString] public string? unknString0;
    [RszInlineWString] public string? unknString1;
    [RszInlineWString] public string? unknString2;
}
