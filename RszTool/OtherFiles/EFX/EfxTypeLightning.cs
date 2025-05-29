using RszTool.Efx.Structs.Common;
using RszTool.InternalAttributes;

namespace RszTool.Efx.Structs.Main;

[RszGenerate, RszAutoReadWrite, RszVersionedObject(typeof(EfxVersion)), EfxStruct(EfxAttributeType.TypeLightning3D, EfxVersion.RE7, EfxVersion.RE2, EfxVersion.DMC5)]
public partial class EFXAttributeTypeLightning3DV1 : RszTool.Efx.EFXAttribute
{
    public EFXAttributeTypeLightning3DV1() : base(EfxAttributeType.TypeLightning3D) { }

	public uint unknBitFlag;
	public via.Color color0;
	public via.Color color1;
	public float unkn2_0;
	public UndeterminedFieldType unkn2_1;
	public UndeterminedFieldType unkn2_2;
	public UndeterminedFieldType unkn2_3;

    public float unkn2_4;
	public UndeterminedFieldType unkn2_5;
	public float unkn2_6;
	public UndeterminedFieldType unkn2_7;
	public uint unkn2_8;
	public UndeterminedFieldType unkn2_9; // either int or float (DMC5) [3, 0.1]
	public UndeterminedFieldType unkn2_10; // either int or float (DMC5) [5, 0.12]
	public UndeterminedFieldType unkn2_11; // either int or float (DMC5) [1, 0.3]
	public UndeterminedFieldType unkn2_12; // either int or float (DMC5) [1, 0.4]
	public float unkn2_13;
	public UndeterminedFieldType unkn2_14;
	public float unkn2_15;
	public float unkn2_16;
	public float unkn2_17;
	public UndeterminedFieldType unkn2_18;
	public float unkn2_19;
	public UndeterminedFieldType unkn2_20;
	public float unkn2_21;
	public UndeterminedFieldType unkn2_22;
	public float unkn2_23;
	public float unkn2_24;
	public float unkn2_25;
	public UndeterminedFieldType unkn2_26;
	public float unkn2_27;
	public UndeterminedFieldType unkn2_28;
	public float unkn2_29;
	public UndeterminedFieldType unkn2_30;
	public float unkn2_31;
	public UndeterminedFieldType unkn2_32;
	public UndeterminedFieldType unkn2_33;
	public float unkn2_34;
	public UndeterminedFieldType unkn2_35;
	public UndeterminedFieldType unkn2_36;
	public UndeterminedFieldType unkn2_37;
	public float unkn2_38;
	public UndeterminedFieldType unkn2_39;
	public float unkn2_40;
	public float unkn2_41;
	public float unkn2_42;
	public float unkn2_43;
	public float unkn2_44;
	public float unkn2_45;
	public float unkn2_46;
	public float unkn2_47;
	public float unkn2_48;
	public uint unkn2_49;
	public UndeterminedFieldType unkn2_50;
	public UndeterminedFieldType unkn2_51;
	public UndeterminedFieldType unkn2_52;
	public UndeterminedFieldType unkn2_53;
	public UndeterminedFieldType unkn2_54;
	public uint unkn2_55;
    [RszVersion(EfxVersion.DMC5)]
	public uint unkn2_56;
    short boneNameEmpty;
}
[RszGenerate, RszAutoReadWrite, RszVersionedObject(typeof(EfxVersion)), EfxStruct(EfxAttributeType.TypeLightning3D, EfxVersion.MHRiseSB, EfxVersion.DD2)]
public partial class EFXAttributeTypeLightning3D : EFXAttribute, IBoneRelationAttribute
{
	public EFXAttributeTypeLightning3D() : base(EfxAttributeType.TypeLightning3D) { }

	// MHR: The unknBitFlag values change between base game and sunbreak, even on files that have no changes. Game will crash when value is set wrong.
	// DMC5: likely determines some nested struct type (shape?), as the value types are sometimes int and sometimes float: vfx\effectediter\efd_cutscene\efd_cs_mission01\efd_m01_100\efd_03_cs_m01_100_00_021.efx.1769672
	public uint unknBitFlag;
	public via.Color color0;
	public via.Color color1;
	public float unkn2_0;
	public float unkn2_1;
	public float unkn2_2;
	public float unkn2_3;
	[RszVersion(EfxVersion.RERT)]
	public float sb_unkn0;

    public float unkn2_4;
	public float unkn2_5;
	public float unkn2_6;
	public float unkn2_7;
	public float unkn2_8;

	// this section types seem to change - likely some dynamic struct based on the bit flag
	public UndeterminedFieldType unkn2_9;
	public UndeterminedFieldType unkn2_10;
	public UndeterminedFieldType unkn2_11;
	public UndeterminedFieldType unkn2_12;
	public UndeterminedFieldType unkn2_13;
	public UndeterminedFieldType unkn2_14;
	public UndeterminedFieldType unkn2_15;

	public float unkn2_16;
	public float unkn2_17;
	public float unkn2_18;
	public float unkn2_19;
	public float unkn2_20;
	public float unkn2_21;
	public float unkn2_22;
	public float unkn2_23;
	public float unkn2_24;
	public float unkn2_25;
	public float unkn2_26;
	public float unkn2_27;
	public float unkn2_28;
	public float unkn2_29;
	public float unkn2_30;
	public float unkn2_31;
	public float unkn2_32;
	public float unkn2_33;
	public float unkn2_34;
	public float unkn2_35;
	public float unkn2_36;
	public float unkn2_37;
	public float unkn2_38;
	public float unkn2_39;
	public float unkn2_40;
	public float unkn2_41;
	public float unkn2_42;
	public float unkn2_43;
	public float unkn2_44;
	public float unkn2_45;
	public float unkn2_46;
	public float unkn2_47;
	public uint unkn2_48;
	public uint unkn2_49;
	public float unkn2_50;
	public float unkn2_51;
	public float unkn2_52;
	public float unkn2_53;
	public float unkn2_54;
	public float unkn2_55;
	[RszVersion(EfxVersion.RE3, EndAt = nameof(unkn2_72))]
	public float unkn2_56;
	public float unkn2_57;
	public float unkn2_58;
	public float unkn2_59;
	public float unkn2_60;
	public float unkn2_61;
	public float unkn2_62;
	public float unkn2_63;
	[RszVersion(EfxVersion.DD2)]
	public float dd2_unkn;
	public uint unkn2_64;
	public uint unkn2_65;
	public uint unkn2_66;
	public float unkn2_67;
	public float unkn2_68;
	public float unkn2_69;
	public uint unkn2_70;

	public float unkn2_71;
	public float unkn2_72;
	[RszInlineWString] public string? boneName;

    public string? ParentBone { get; set; }
}
[RszGenerate, RszAutoReadWrite, RszVersionedObject(typeof(EfxVersion)), EfxStruct(EfxAttributeType.TypeLightning3DExpression, EfxVersion.DD2)]
public partial class EFXAttributeTypeLightning3DExpression : EFXAttribute, IExpressionAttribute
{
	public EFXExpressionList? Expression => expressions;
	public BitSet ExpressionBits => expressionBits;

	public EFXAttributeTypeLightning3DExpression() : base(EfxAttributeType.TypeLightning3DExpression) { }

	// NOTE: may need to add a dynamic size constructor call for pre-DD2 if it was ever present there
	[RszClassInstance] public readonly BitSet expressionBits = new BitSet(48) { BitNameDict = new () {
		[1] = nameof(color),
		[7] = nameof(terminalPosX),
		[8] = nameof(terminalPosY),
		[9] = nameof(terminalPosZ),
	} };
	public ExpressionAssignType color;
	public ExpressionAssignType unkn2;
	public ExpressionAssignType unkn3;
	public ExpressionAssignType unkn4;
	public ExpressionAssignType unkn5;
	public ExpressionAssignType unkn6;
	public ExpressionAssignType terminalPosX;
	public ExpressionAssignType terminalPosY;
	public ExpressionAssignType terminalPosZ;
	public ExpressionAssignType unkn10;
	public ExpressionAssignType unkn11;
	public ExpressionAssignType unkn12;
	public ExpressionAssignType unkn13;
	public ExpressionAssignType unkn14;
	public ExpressionAssignType unkn15;
	public ExpressionAssignType unkn16;
	public ExpressionAssignType unkn17;
	public ExpressionAssignType unkn18;
	public ExpressionAssignType unkn19;
	public ExpressionAssignType unkn20;
	public ExpressionAssignType unkn21;
	public ExpressionAssignType unkn22;
	public ExpressionAssignType unkn23;
	public ExpressionAssignType unkn24;
	public ExpressionAssignType unkn25;
	public ExpressionAssignType unkn26;
	public ExpressionAssignType unkn27;
	public ExpressionAssignType unkn28;

	[RszVersion(EfxVersion.DD2, EndAt = nameof(unkn48))]
	public ExpressionAssignType unkn29;
    public ExpressionAssignType unkn30;
    public ExpressionAssignType unkn31;
    public ExpressionAssignType unkn32;
    public ExpressionAssignType unkn33;
    public ExpressionAssignType unkn34;
    public ExpressionAssignType unkn35;
    public ExpressionAssignType unkn36;
    public ExpressionAssignType unkn37;
    public ExpressionAssignType unkn38;
    public ExpressionAssignType unkn39;
    public ExpressionAssignType unkn40;
    public ExpressionAssignType unkn41;
    public ExpressionAssignType unkn42;
    public ExpressionAssignType unkn43;
    public ExpressionAssignType unkn44;
    public ExpressionAssignType unkn45;
    public ExpressionAssignType unkn46;
    public ExpressionAssignType unkn47;
    public ExpressionAssignType unkn48;

	[RszClassInstance, RszConstructorParams(nameof(Version))] public EFXExpressionList? expressions;

}

[RszGenerate, RszAutoReadWrite, RszVersionedObject(typeof(EfxVersion)), EfxStruct(EfxAttributeType.TypeLightning3DMaterial, EfxVersion.DD2)]
public partial class EFXAttributeTypeLightning3DMaterial : EFXAttribute
{
	public EFXAttributeTypeLightning3DMaterial() : base(EfxAttributeType.TypeLightning3DMaterial) { }

	public uint unknBitFlag;
	public via.Color color0;
	public via.Color color1;
	public float unkn2_0;
	public float unkn2_1;
	public float unkn2_2;
	public UndeterminedFieldType unkn2_3;
	[RszVersion(EfxVersion.RERT)]
	public float sb_unkn0;

    public float unkn2_4;
	public uint unkn2_5;
	public uint unkn2_6;
	public uint unkn2_7;
	public uint unkn2_8;
	public uint unkn2_9;
	public float unkn2_10;
	public float unkn2_11;
	public float unkn2_12;
	public float unkn2_13;
	public float unkn2_14;
	public UndeterminedFieldType unkn2_15;
	public float unkn2_16;
	public float unkn2_17;
	public float unkn2_18;
	public float unkn2_19;
	public float unkn2_20;
	public UndeterminedFieldType unkn2_21;
	public float unkn2_22;
	public UndeterminedFieldType unkn2_23;
	public float unkn2_24;
	public UndeterminedFieldType unkn2_25;
	public float unkn2_26;
	public UndeterminedFieldType unkn2_27;
	public float unkn2_28;
	public UndeterminedFieldType unkn2_29;
	public float unkn2_30;
	public UndeterminedFieldType unkn2_31;
	public float unkn2_32;
	public UndeterminedFieldType unkn2_33;
	public float unkn2_34;
	public float unkn2_35;
	public UndeterminedFieldType unkn2_36;
	public float unkn2_37;
	public UndeterminedFieldType unkn2_38;
	public float unkn2_39;
	public UndeterminedFieldType unkn2_40;
	public float unkn2_41;
	public UndeterminedFieldType unkn2_42;
	public uint dd2_extra1;
	public UndeterminedFieldType unkn2_43;
	public float unkn2_44;
	public UndeterminedFieldType unkn2_45;
	public float unkn2_46;
	public UndeterminedFieldType unkn2_47;
	public float unkn2_48;
	public float unkn2_49;
	public float unkn2_50;
	public float unkn2_51;
	public float unkn2_52;
	public float unkn2_53;
	public float unkn2_54;
	public float unkn2_55;
	public float unkn2_56;
	public float unkn2_57;
	public uint unkn2_58;
	public UndeterminedFieldType unkn2_59;
	public UndeterminedFieldType unkn2_60;
	public float unkn2_61;
	public float unkn2_62;
	public UndeterminedFieldType unkn2_63;
	public uint unkn2_64;
	public float unkn2_65;
	public float unkn2_66;

	[RszConstructorParams(nameof(Version)), RszSwitch(
		nameof(Version), ">=", EfxVersion.DD2, typeof(EfxMaterialStructV2),
		typeof(EfxMaterialStructV1)
	)]
	public EfxMaterialStructBase? material;
	[RszInlineWString] public string? uknString;
}

[RszGenerate, RszAutoReadWrite, RszVersionedObject(typeof(EfxVersion)), EfxStruct(EfxAttributeType.TypeLightning3DMaterialClip, EfxVersion.DD2)]
public partial class EFXAttributeTypeLightning3DMaterialClip : EFXAttribute
{
	public EFXAttributeTypeLightning3DMaterialClip() : base(EfxAttributeType.TypeLightning3DMaterialClip) { }

	public uint ukn1;
	public uint ukn2;
	public uint ukn3;
	public float ukn4;
	public uint ukn5;
	public uint ukn6;
	public UndeterminedFieldType ukn7;
	public uint ukn8;
	public uint subSize1;
	public UndeterminedFieldType ukn9;
	public uint ukn10;
	public uint subSize2;

	[RszFixedSizeArray(nameof(subSize2), "* 2")] public int[]? sub1;
	[RszFixedSizeArray(nameof(subSize1), "/ 4")] public int[]? sub2;
	[RszFixedSizeArray(nameof(subSize2), "* 4")] public int[]? sub3;
	[RszFixedSizeArray(nameof(subSize2))] public int[]? sub4;
}

[RszGenerate, RszAutoReadWrite, RszVersionedObject(typeof(EfxVersion)), EfxStruct(EfxAttributeType.TypeLightning3DMaterialExpression, EfxVersion.DD2)]
public partial class EFXAttributeTypeLightning3DMaterialExpression : EFXAttribute, IExpressionAttribute, IMaterialExpressionAttribute
{
    public EFXExpressionList? Expression => expressions;
    public EFXMaterialExpressionList? MaterialExpressions => materialExpressions;

	public EFXAttributeTypeLightning3DMaterialExpression() : base(EfxAttributeType.TypeLightning3DMaterialExpression) { }

	public BitSet ExpressionBits => expressionBits;
	[RszClassInstance] public readonly BitSet expressionBits = new BitSet(44) { BitNameDict = new () {
		[1] = nameof(terminalPosX),
		[2] = nameof(terminalPosY),
		[3] = nameof(terminalPosZ),
	} };
	public ExpressionAssignType terminalPosX;
	public ExpressionAssignType terminalPosY;
	public ExpressionAssignType terminalPosZ;
	public ExpressionAssignType unkn4;
	public ExpressionAssignType unkn5;
	public ExpressionAssignType unkn6;
	public ExpressionAssignType unkn7;
	public ExpressionAssignType unkn8;
	public ExpressionAssignType unkn9;
    public ExpressionAssignType unkn10;
    public ExpressionAssignType unkn12;
    public ExpressionAssignType unkn13;
    public ExpressionAssignType unkn14;
    public ExpressionAssignType unkn15;
    public ExpressionAssignType unkn16;
    public ExpressionAssignType unkn17;
    public ExpressionAssignType unkn18;
    public ExpressionAssignType unkn19;
    public ExpressionAssignType unkn20;
    public ExpressionAssignType unkn21;
    public ExpressionAssignType unkn22;
    public ExpressionAssignType unkn23;
    public ExpressionAssignType unkn24;
    public ExpressionAssignType unkn25;
    public ExpressionAssignType unkn26;
    public ExpressionAssignType unkn27;
    public ExpressionAssignType unkn28;
    public ExpressionAssignType unkn29;
    public ExpressionAssignType unkn30;
    public ExpressionAssignType unkn31;
    public ExpressionAssignType unkn32;
    public ExpressionAssignType unkn33;
    public ExpressionAssignType unkn34;
    public ExpressionAssignType unkn35;
    public ExpressionAssignType unkn36;
    public ExpressionAssignType unkn37;
    public ExpressionAssignType unkn38;
    public ExpressionAssignType unkn39;
    public ExpressionAssignType unkn40;
    public ExpressionAssignType unkn41;
    public ExpressionAssignType unkn42;
    public ExpressionAssignType unkn43;
	[RszClassInstance, RszConstructorParams(nameof(Version))] public EFXExpressionList? expressions;

	public ExpressionAssignType unkn44;
	public uint materialExpressionCount;
	[RszClassInstance, RszConstructorParams(nameof(Version))] public EFXMaterialExpressionList? materialExpressions;
}

[RszGenerate, RszAutoReadWrite, RszVersionedObject(typeof(EfxVersion)), EfxStruct(EfxAttributeType.TypeGpuLightning3D, EfxVersion.RE4)]
public partial class EFXAttributeTypeGpuLightning3D : EFXAttribute
{
	public EFXAttributeTypeGpuLightning3D() : base(EfxAttributeType.TypeGpuLightning3D) { }

	public uint unkn1;
	public via.Color color0;
	public via.Color color1;
	public int unkn2_0;
	public float unkn2_1;
	public UndeterminedFieldType unkn2_2;
	public UndeterminedFieldType unkn2_3;
	public float unkn2_4;
	public uint unkn2_5;
	public uint unkn2_6;
	public UndeterminedFieldType unkn2_7;
	public float unkn2_8;
	public UndeterminedFieldType unkn2_9;
	public float unkn2_10;
	public UndeterminedFieldType unkn2_11;
	public float unkn2_12;
	public float unkn2_13;
	public UndeterminedFieldType unkn2_14;
	public float unkn2_15;
	public UndeterminedFieldType unkn2_16;
	public float unkn2_17;
	public UndeterminedFieldType unkn2_18;
	public float unkn2_19;
	public UndeterminedFieldType unkn2_20;
	public UndeterminedFieldType unkn2_21;
	public UndeterminedFieldType unkn2_22;
	public UndeterminedFieldType unkn2_23;
	public UndeterminedFieldType unkn2_24;
	public UndeterminedFieldType unkn2_25;
	public UndeterminedFieldType unkn2_26;
	public UndeterminedFieldType unkn2_27;
	public float unkn2_28;
	public UndeterminedFieldType unkn2_29;
	public UndeterminedFieldType unkn2_30;
	public float unkn2_31;
	public float unkn2_32;
	public UndeterminedFieldType unkn2_33;
	public float unkn2_34;
	public float unkn2_35;
	public float unkn2_36;
	public uint unkn2_37;
	public uint unkn2_38;
	public float unkn2_39;
	public float unkn2_40;
	public uint unkn2_41;
	public float unkn2_42;
	public float unkn2_43;
	public float unkn2_44;
	public float unkn2_45;
	public float unkn2_46;
	public float unkn2_47;
	public uint unkn2_48;
	public uint unkn2_49;
	public uint unkn2_50;
	public uint unkn2_51;
	public uint unkn2_52;
	public uint unkn2_53;
	public uint unkn2_54;
	public float unkn2_55;
	public float unkn2_56;
	public float unkn2_57;
	public float unkn2_58;
	public float unkn2_59;
	public float unkn2_60;
	public uint unkn2_61;
	public uint unkn2_62;
	public uint unkn2_63;
	public uint unkn2_64;
	public uint unkn2_65;
	public uint unkn2_66;
	public uint unkn2_67;
	public float unkn2_68;
	public float unkn2_69;
	public float unkn2_70;
	public float unkn2_71;
	public float unkn2_72;
	public float unkn2_73;
	public uint unkn2_74;
	public uint unkn2_75;
	public uint unkn2_76;
	public uint unkn2_77;
	public uint unkn2_78;
	public uint unkn2_79;
	public uint unkn2_80;

	public uint str1Len;
	public uint str2Len;
	public uint str3Len;
	[RszInlineWString(-1)] public string? str1;
	[RszInlineWString(-1)] public string? str2;
	[RszInlineWString(-1)] public string? str3;
}