using ReeLib.Efx.Structs.Common;
using ReeLib.InternalAttributes;

namespace ReeLib.Efx.Structs.Main;

[RszGenerate, RszAutoReadWrite, RszVersionedObject(typeof(EfxVersion)), EfxStruct(EfxAttributeType.TypeStrainRibbon, EfxVersion.RE7, EfxVersion.RE2, EfxVersion.DMC5, EfxVersion.RE3)]
public partial class EFXAttributeTypeStrainRibbonV1 : ReeLib.Efx.EFXAttribute
{
    public EFXAttributeTypeStrainRibbonV1() : base(EfxAttributeType.TypeStrainRibbon) { }

    uint ukn1;
	public via.Color color1;
	public via.Color color2;
	public float unkn1_3;
    public float ukn2_0;
    public float ukn2_1;
    public float ukn2_2;
    public float ukn2_3;
    public float ukn2_4;
    public float ukn2_5;
    public float ukn2_6;
    public float ukn2_7;
    public uint ukn2_8;
    public float ukn2_9;
    public float ukn2_10;
    public float ukn2_11;
    public float ukn2_12;
    public float ukn2_13;
    public float ukn2_14;
    public float ukn2_15;
    public float ukn2_16;
    public float ukn2_17;
    public float ukn2_18;
    public uint ukn2_19;
    public uint ukn2_20;
    public uint ukn2_21;
    public float ukn2_22;
    public float ukn2_23;
    public float ukn2_24;
    public uint ukn2_25;
    public float ukn2_26;
    public uint ukn2_27;
    [RszVersion(EfxVersion.RE2, EndAt = nameof(ukn3_7))]
    public uint ukn3_0;
    public uint ukn3_1;
    public float ukn3_2;
    public via.Color color3;
    public via.Color color4;
    public via.Color color5;
    public float ukn3_6;
    public float ukn3_7;
    [RszVersion(EfxVersion.RE3, EndAt = nameof(ukn4_1))]
    public float ukn4_0;
    public float ukn4_1;
    [RszInlineWString] public string? boneName;

    public override string ToString() => !string.IsNullOrEmpty(boneName) ? boneName : type.ToString();
}

[RszGenerate, RszAutoReadWrite, RszVersionedObject(typeof(EfxVersion)), EfxStruct(EfxAttributeType.TypeStrainRibbon, EfxVersion.RE8, EfxVersion.RERT)]
public partial class EFXAttributeTypeStrainRibbonV2 : EFXAttribute, IBoneRelationAttribute
{
	public EFXAttributeTypeStrainRibbonV2() : base(EfxAttributeType.TypeStrainRibbon) { }

	public uint unkn1_0;
	public via.Color color0;
	public via.Color color1;
	public float unkn1_3;

	public float unkn1_4;
	public uint unkn1_5;
	public float unkn1_6;
	public float unkn1_7;
	public float unkn1_8;
	public float unkn1_9;
	public float unkn1_10;
	public float unkn1_11;
	public uint unkn1_12;
	public float unkn1_13;
	public float unkn1_14;
	public float unkn1_15;
	public float unkn1_16;
	public float unkn1_17;
    [RszVersion(EfxVersion.RERT)]
	public float sb_unkn0;

	public uint unkn1_18;
	public uint unkn1_19;
    [RszVersion(EfxVersion.RERT)]
	public uint unkn1_20;
    [RszVersion('<', EfxVersion.RERT)]
	public float unkn1_20_re8;
	public float unkn1_21;

	public via.Color color2;
	public via.Color color3;
	public via.Color color4;
	public float unkn1_25;
	public float unkn1_26;
	public float unkn1_27;
	public float unkn1_28;
	public float unkn1_29;
	public float unkn1_30;
	public float unkn1_31;
	public float unkn1_32;
	public uint unkn1_33;
	public uint unkn1_34;
	public float unkn1_35;
	public float unkn1_36;
	public float unkn1_37;
	public float unkn1_38;
	public float unkn1_39;
	public float unkn1_40;
	public uint unkn1_41;
	public float unkn1_42;
	public float unkn1_43;
	public float unkn1_44;
	public float unkn1_45;
	public float unkn1_46;
	public float unkn1_47;
	public uint unkn1_48;
	public uint unkn1_49;
	public uint unkn1_50;
	public uint unkn1_51;
	public uint unkn1_52;
	public float unkn1_53;
	public float unkn1_54;
	public float unkn1_55;
	public uint unkn1_56;
	public byte unkn2;
	public float unkn3_0;
	public float sb_unkn1;

	public float unkn3_1;

    public float unkn3_2;
    public float unkn3_3;
	[RszInlineWString] public string? boneName;

    public string? ParentBone { get; set; }

    public override string ToString() => !string.IsNullOrEmpty(boneName) ? boneName : type.ToString();
}

[RszGenerate, RszAutoReadWrite, RszVersionedObject(typeof(EfxVersion)), EfxStruct(EfxAttributeType.TypeStrainRibbon, EfxVersion.RE4)]
public partial class EFXAttributeTypeStrainRibbonV3 : EFXAttribute
{
	// structure looks vaguely similar to TypeStrainRibbon, but big reodering
	// TODO verify for MHWilds
	public EFXAttributeTypeStrainRibbonV3() : base(EfxAttributeType.TypeStrainRibbon) { }

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

    public override string ToString() => !string.IsNullOrEmpty(boneName) ? boneName : type.ToString();
}


[RszGenerate, RszAutoReadWrite, RszVersionedObject(typeof(EfxVersion)), EfxStruct(EfxAttributeType.TypeStrainRibbonExpression, EfxVersion.RE2, EfxVersion.DMC5, EfxVersion.RE8, EfxVersion.RERT)]
public partial class EFXAttributeTypeStrainRibbonExpression : EFXAttribute, IExpressionAttribute
{
	public EFXExpressionList? Expression { get => expressions; set => expressions = value!; }
	public BitSet ExpressionBits => expressionBits;

	public EFXAttributeTypeStrainRibbonExpression() : base(EfxAttributeType.TypeStrainRibbonExpression) { }

	// TODO bitset versioning
	[RszClassInstance] public readonly BitSet expressionBits = new BitSet(15) { BitNameDict = new () {
		[3] = nameof(posX), // rert: moved up by 2 (xyz = 567)
		[4] = nameof(posY),
		[5] = nameof(posZ),
	} };
	public ExpressionAssignType unkn1;
	public ExpressionAssignType unkn2;
	public ExpressionAssignType posX;
	public ExpressionAssignType posY;
	public ExpressionAssignType posZ;
	public ExpressionAssignType unkn6;
	public ExpressionAssignType unkn7;
	public ExpressionAssignType unkn8;
	public ExpressionAssignType unkn9;
	public ExpressionAssignType unkn10;
	public ExpressionAssignType unkn11;
	public ExpressionAssignType unkn12;
    [RszVersion(EfxVersion.RE8, EndAt = nameof(unkn14))]
	public ExpressionAssignType unkn13;
	public ExpressionAssignType unkn14;
	public ExpressionAssignType unkn15;

	[RszClassInstance, RszConstructorParams(nameof(Version))] public EFXExpressionList? expressions;
}


[RszGenerate, RszAutoReadWrite, RszVersionedObject(typeof(EfxVersion)), EfxStruct(EfxAttributeType.TypeStrainRibbonExpression, EfxVersion.DD2)]
public partial class EFXAttributeTypeStrainRibbonExpressionV2 : EFXAttribute, IExpressionAttribute
{
    public EFXExpressionList? Expression { get => expressions; set => expressions = value; }
	public BitSet ExpressionBits => expressionBits;

	public EFXAttributeTypeStrainRibbonExpressionV2() : base(EfxAttributeType.TypeStrainRibbonExpression) { }

	[RszClassInstance] public readonly BitSet expressionBits = new BitSet(24) { BitNameDict = new () {
		[13] = nameof(posX),
		[14] = nameof(posY),
		[15] = nameof(posZ),
		[16] = nameof(terminalPosX),
		[17] = nameof(terminalPosY),
		[18] = nameof(terminalPosZ),
	} };
    public ExpressionAssignType unkn1;
    public ExpressionAssignType unkn2;
    public ExpressionAssignType unkn3;
    public ExpressionAssignType unkn4;
    public ExpressionAssignType unkn5;
    public ExpressionAssignType unkn6;
    public ExpressionAssignType unkn7;
    public ExpressionAssignType unkn8;
    public ExpressionAssignType unkn9;
    public ExpressionAssignType unkn10;
    public ExpressionAssignType unkn12;
    public ExpressionAssignType posX;
    public ExpressionAssignType posY;
    public ExpressionAssignType posZ;
    public ExpressionAssignType terminalPosX;
    public ExpressionAssignType terminalPosY;
    public ExpressionAssignType terminalPosZ;
    public ExpressionAssignType unkn19;
    public ExpressionAssignType unkn20;
    public ExpressionAssignType unkn21;
    public ExpressionAssignType unkn22;
    public ExpressionAssignType unkn23;
    public ExpressionAssignType unkn24;
	[RszClassInstance, RszConstructorParams(nameof(Version))] public EFXExpressionList? expressions;
}

[RszGenerate, RszAutoReadWrite, RszVersionedObject(typeof(EfxVersion)), EfxStruct(EfxAttributeType.TypeStrainRibbonMaterial, EfxVersion.DD2, EfxVersion.MHWilds)]
public partial class EFXAttributeTypeStrainRibbonMaterial : EFXAttribute
{
	public EFXAttributeTypeStrainRibbonMaterial() : base(EfxAttributeType.TypeStrainRibbonMaterial) { }

	public uint unkn0;
	public UndeterminedFieldType unkn1;
	public uint unkn2;
	public UndeterminedFieldType unkn3;
	public UndeterminedFieldType unkn4;
	public UndeterminedFieldType unkn5;
	public float unkn6;
	public float unkn7;
	public float unkn8;
	public uint unkn9;
	public float unkn10;
	public float unkn11;
	public float unkn12;
	public float unkn13;
	public float unkn14;
	public uint unkn15;
	public uint	 unkn16;
	public float unkn17;
	public float unkn18;

	public via.Color color3;
	public via.Color color4;
	public via.Color color5;

	public float unkn22;
	public float unkn23;
	public float unkn24;
	public float unkn25;
	public float unkn26;
	public float unkn27;
	public float unkn28;
	public float unkn29;
	public float unkn30;
	public float unkn31;

	public float unkn32;
	public float unkn33;
	public float unkn34;
	[RszVersion(EfxVersion.MHWilds)]
	public uint wildsUnkn0;
	public uint unkn35;
	public float unkn36;
	public UndeterminedFieldType unkn37;
	public float unkn38;
	public UndeterminedFieldType unkn39;
	public float unkn40;
	public float unkn41;
	public float unkn42;
	[RszVersion(EfxVersion.MHWilds)]
	public uint wildsUnkn1;
	[RszVersion(EfxVersion.MHWilds, EndAt = nameof(wildsColor2))]
	public via.Color wildsColor1;
	public via.Color wildsColor2;
	public float unkn43;
	[RszVersion(EfxVersion.MHWilds)]
	public uint wildsUnkn4;

	public float unkn44;
	public float unkn45;

	[RszVersionExact(EfxVersion.DD2)]
	public uint unkn46;
	public float unkn47;
	public uint unkn48;
	[RszVersion(EfxVersion.MHWilds)]
	public UndeterminedFieldType unkn49;
	public UndeterminedFieldType unkn50;
	public float unkn51;
	public float unkn52;
	public uint unkn53;
	public uint unkn54;
	[RszVersionExact(EfxVersion.DD2)]
	public uint dd2Unkn;
	public UndeterminedFieldType unkn55;
	public UndeterminedFieldType unkn56;
	public float unkn57;
	public float unkn58;
	public float unkn59;
	public UndeterminedFieldType unkn60;
	[RszInlineWString] public string? propName;
	public UndeterminedFieldType unkn61;
	public via.Color unkn62;
	public via.Color unkn63;
	public float unkn64;
	public UndeterminedFieldType unkn65;
	public UndeterminedFieldType unkn66;
	public UndeterminedFieldType unkn67;

	[RszConstructorParams(nameof(Version)), RszSwitch(
		nameof(Version), ">=", EfxVersion.DD2, typeof(EfxMaterialStructV2),
		typeof(EfxMaterialStructV1)
	)]
	public EfxMaterialStructBase? material;
}

[RszGenerate, RszAutoReadWrite, RszVersionedObject(typeof(EfxVersion)), EfxStruct(EfxAttributeType.TypeStrainRibbonMaterialClip, EfxVersion.DD2)]
public partial class EFXAttributeTypeStrainRibbonMaterialClip : EFXAttribute, IMaterialClipAttribute
{
    public EfxMaterialClipData MaterialClip => clipData;
    public BitSet ClipBits => clipBits;

    public EFXAttributeTypeStrainRibbonMaterialClip() : base(EfxAttributeType.TypeStrainRibbonMaterialClip) { }

	[RszClassInstance] public readonly BitSet clipBits = new BitSet(1);
    public uint unkn1;
	[RszClassInstance, RszConstructorParams(nameof(Version))] public readonly EfxMaterialClipData clipData = new();
}

[RszGenerate, RszAutoReadWrite, RszVersionedObject(typeof(EfxVersion)), EfxStruct(EfxAttributeType.TypeStrainRibbonMaterialExpression, EfxVersion.DD2, EfxVersion.MHWilds)]
public partial class EFXAttributeTypeStrainRibbonMaterialExpression : EFXAttribute, IExpressionAttribute, IMaterialExpressionAttribute
{
	public EFXExpressionList? Expression { get => expressions; set => expressions = value; }
    public EFXMaterialExpressionList? MaterialExpressions => materialExpressions;

	public BitSet ExpressionBits => expressionBits;

    public EFXAttributeTypeStrainRibbonMaterialExpression() : base(EfxAttributeType.TypeStrainRibbonMaterialExpression) { }

	[RszClassInstance] public readonly BitSet expressionBits = new BitSet(22) { BitNameDict = new () {
		[1] = nameof(color),
		[5] = nameof(posX),
		[6] = nameof(posY),
		[7] = nameof(posZ),
		[8] = nameof(terminalPosX),
		[9] = nameof(terminalPosY),
		[10] = nameof(terminalPosZ),
		[15] = nameof(color2),
		[17] = nameof(color3),
		[19] = nameof(color4),
	} };
	public ExpressionAssignType color;
	public ExpressionAssignType unkn2;
	public ExpressionAssignType unkn3;
	public ExpressionAssignType unkn4;
	public ExpressionAssignType posX;
	public ExpressionAssignType posY;
	public ExpressionAssignType posZ;
	public ExpressionAssignType terminalPosX;
	public ExpressionAssignType terminalPosY;
	public ExpressionAssignType terminalPosZ;
	public ExpressionAssignType unkn11;
	public ExpressionAssignType unkn12;
	public ExpressionAssignType unkn13;
	public ExpressionAssignType unkn14;
	public ExpressionAssignType color2;
	public ExpressionAssignType unkn16;
	public ExpressionAssignType color3;
	public ExpressionAssignType unkn18;
	public ExpressionAssignType color4;
	public ExpressionAssignType unkn20;
	[RszVersion(EfxVersion.MHWilds)]
	public ExpressionAssignType unkn21;
	[RszClassInstance, RszConstructorParams(nameof(Version))] public EFXExpressionList? expressions;
	[RszVersionExact(EfxVersion.DD2)] // TODO probably shouldn't be here, need to fully resolve MHWilds struct
	public ExpressionAssignType unkn22;
	[RszArraySizeField(nameof(materialExpressions))] public int materialExpressionCount;
	[RszVersion(EfxVersion.DD2), RszClassInstance, RszConstructorParams(nameof(Version))]
	public EFXMaterialExpressionList? materialExpressions;
}
