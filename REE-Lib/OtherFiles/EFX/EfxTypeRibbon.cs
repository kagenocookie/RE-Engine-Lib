using ReeLib.Efx.Structs.Common;
using ReeLib.InternalAttributes;

namespace ReeLib.Efx.Structs.Main;

[RszGenerate, RszAutoReadWrite, RszVersionedObject(typeof(EfxVersion)), EfxStruct(EfxAttributeType.TypeRibbonLength, EfxVersion.RE7, EfxVersion.RE2, EfxVersion.DMC5, EfxVersion.RE3, EfxVersion.RE8, EfxVersion.RERT, EfxVersion.RE4, EfxVersion.DD2)]
public partial class EFXAttributeTypeRibbonLength : EFXAttribute
{
	public EFXAttributeTypeRibbonLength() : base(EfxAttributeType.TypeRibbonLength) { }

	public uint unkn1_0;
	public via.Color color0;
	public via.Color color1;
	public float unkn1_3;

	public float unkn1_4;
	public float unkn1_5;
	public float unkn1_6;
    [RszVersion(EfxVersion.RE4)]
	public uint unkn1_7;
	public float unkn1_8;
	public float unkn1_9;
	public float unkn1_10;
	public float unkn1_11;
    [RszVersion(EfxVersion.RERT)]
	public float sb_unkn0;
	public float unkn1_12;
    [RszVersion(EfxVersion.DD2)] public float dd2_unkn1;
    [RszVersion(EfxVersion.RE2)]
	public uint unkn1_13; // TODO 4-byte thing?
	public float unkn1_14;
	public float unkn1_15;
	public uint unkn1_16;

	public float unkn1_17;
	public float unkn1_18;
	public float unkn1_19;
	public float unkn1_20;
	public float unkn1_21;
	public float unkn1_22;
	public float unkn1_23;
	public float unkn1_24;
	public float unkn1_25;
	public float unkn1_26;

	[RszVersion(EfxVersion.RE2, EndAt = nameof(sb_unkn2))]
	public via.Color color2_1;
	public via.Color color2_2;
	public via.Color color2_3;
	public float unkn1_29;
	public float unkn1_30;
	public float unkn1_31;
	public float unkn1_32;
	public float unkn1_33;
	public float unkn1_34;
	public float unkn1_35;
    [RszVersion(EfxVersion.RE8, EndAt = nameof(sb_unkn2))]
	public float sb_unkn1;
	public float sb_unkn2;
}

[RszGenerate, RszAutoReadWrite, RszVersionedObject(typeof(EfxVersion)), EfxStruct(EfxAttributeType.TypeRibbonLengthExpression, EfxVersion.RE7, EfxVersion.RE8, EfxVersion.RERT, EfxVersion.RE4)]
public partial class EFXAttributeTypeRibbonLengthExpression : EFXAttribute, IExpressionAttribute
{
	public EFXExpressionList? Expression { get => expressions; set => expressions = value; }
	public BitSet ExpressionBits => expressionBits;

	public EFXAttributeTypeRibbonLengthExpression() : base(EfxAttributeType.TypeRibbonLengthExpression) { }

	[RszClassInstance] public readonly BitSet expressionBits = new BitSet(19) { BitNameDict = new () {
		[1] = nameof(color),
		[2] = nameof(alpha),
		// 3,4 = size (RE7)
		// 5, 6 = size (RERT)
	} };
	public ExpressionAssignType color;
	public ExpressionAssignType alpha;
	public ExpressionAssignType size1;
	public ExpressionAssignType size2;
	public ExpressionAssignType emissive;
	[RszVersion(EfxVersion.RE2, EndAt = nameof(size))]
	public ExpressionAssignType unkn6;
	public ExpressionAssignType size;
    [RszVersion(EfxVersion.RE8, EndAt = nameof(unkn9))]
	public ExpressionAssignType unkn8;
	public ExpressionAssignType unkn9;
    [RszVersion(">", EfxVersion.RERT, EndAt = nameof(unkn15))]
	public ExpressionAssignType unkn10;
	public ExpressionAssignType unkn11;
	public ExpressionAssignType unkn12;
	public ExpressionAssignType unkn13;
	public ExpressionAssignType unkn14;
	public ExpressionAssignType unkn15;
    [RszVersion(EfxVersion.RE4, EndAt = nameof(unkn19))]
	public ExpressionAssignType unkn16;
	public ExpressionAssignType unkn17;
	public ExpressionAssignType unkn18;
	public ExpressionAssignType unkn19;
	[RszClassInstance, RszConstructorParams(nameof(Version))] public EFXExpressionList? expressions;
}

[RszGenerate, RszAutoReadWrite, RszVersionedObject(typeof(EfxVersion)), EfxStruct(EfxAttributeType.TypeRibbonLengthMaterial, EfxVersion.RE8)]
public partial class EFXAttributeTypeRibbonLengthMaterial : EFXAttribute
{
	public EFXAttributeTypeRibbonLengthMaterial() : base(EfxAttributeType.TypeRibbonLengthMaterial) { }

	public UndeterminedFieldType unkn1;
	public via.Color color1;
	public via.Color color2;
	public float unkn4;
	public UndeterminedFieldType unkn5;
	public UndeterminedFieldType unkn6;
	public float unkn7;
	public uint unkn8; // TODO 4-byte thing?
	public float unkn9;
	public float unkn10;
	public float unkn11;
	public float unkn12;
	[RszVersion(EfxVersion.DD2)]
	public float dd2_unkn1;
	[RszVersion(EfxVersion.DD2)]
	public float dd2_unkn2;
	[RszVersion(EfxVersion.DD2)]
	public float dd2_unkn3;
	public uint unkn13; // TODO 4-byte thing?
	public float unkn14;
	public float unkn15;
	public int unkn16;
	public float unkn17;
	public float unkn18;
	public UndeterminedFieldType unkn19;
	public UndeterminedFieldType unkn20;
	public float unkn21;
	public float unkn22;
	public float unkn23;
	public float unkn24;
	public float unkn25;
	public float unkn26;

	public via.Color color3;
	public via.Color color4;
	public via.Color color5;
	public float unkn30;
	public float unkn31;
	public float unkn32;
	public float unkn33;
	public float unkn34;
	public float unkn35;
	public float unkn36;
	public float unkn37;
	public UndeterminedFieldType unkn38;
	public uint unkn39;

	[RszConstructorParams(nameof(Version)), RszSwitch(
		nameof(Version), ">=", EfxVersion.DD2, typeof(EfxMaterialStructV2),
		typeof(EfxMaterialStructV1)
	)]
	public EfxMaterialStructBase? material;
}

[RszGenerate, RszAutoReadWrite, RszVersionedObject(typeof(EfxVersion)), EfxStruct(EfxAttributeType.TypeRibbonLengthMaterialExpression, EfxVersion.DD2)]
public partial class EFXAttributeTypeRibbonLengthMaterialExpression : EFXAttribute, IExpressionAttribute, IMaterialExpressionAttribute
{
    public EFXExpressionList? Expression { get => expressions; set => expressions = value; }
    public EFXMaterialExpressionList? MaterialExpressions => materialExpressions;
	public BitSet ExpressionBits => expressionBits;

	public EFXAttributeTypeRibbonLengthMaterialExpression() : base(EfxAttributeType.TypeRibbonLengthMaterialExpression) { }

	[RszClassInstance] public readonly BitSet expressionBits = new BitSet(15) { BitNameDict = new () {
		[1] = nameof(color),
	} };
    public ExpressionAssignType color;
    public ExpressionAssignType unkn2;
    public ExpressionAssignType unkn3;
    public ExpressionAssignType unkn4;
    public ExpressionAssignType unkn5;
    public ExpressionAssignType unkn6;
    public ExpressionAssignType unkn7;
    public ExpressionAssignType unkn8;
    public ExpressionAssignType unkn9;
    public ExpressionAssignType unkn10;
    public ExpressionAssignType unkn11;
    public ExpressionAssignType unkn12;
    public ExpressionAssignType unkn13;
    public ExpressionAssignType unkn14;

	[RszClassInstance, RszConstructorParams(nameof(Version))] public EFXExpressionList? expressions;

	public ExpressionAssignType unkn15;
	public int materialExpressionCount;

	[RszClassInstance, RszConstructorParams(nameof(Version))] public EFXMaterialExpressionList? materialExpressions;
}

[RszGenerate, RszAutoReadWrite, RszVersionedObject(typeof(EfxVersion)), EfxStruct(EfxAttributeType.TypeRibbonChain, EfxVersion.RE7)]
public partial class EFXAttributeTypeRibbonChainV1 : EFXAttribute
{
	public EFXAttributeTypeRibbonChainV1() : base(EfxAttributeType.TypeRibbonChain) { }

	public uint unkn1;
	public via.Color color0;
	public via.Color color1;
	public float unkn2_0;

	public UndeterminedFieldType null2_1;
	public UndeterminedFieldType null2_2;
	public float unkn2_3;
	public UndeterminedFieldType null2_4;
	public float unkn2_5;
	public UndeterminedFieldType null2_6;
	public float unkn2_7;
	public UndeterminedFieldType null2_8;
	public float unkn2_9;
    public float unkn2_10;

	public uint unkn2_11;
	public float unkn2_12;
	public UndeterminedFieldType null2_13;
	public float unkn2_14;
	public UndeterminedFieldType null2_15;
	public float unkn2_16;
	public UndeterminedFieldType null2_17;
	public uint unkn2_18;
	public UndeterminedFieldType null2_19;
	public UndeterminedFieldType null2_20;
	public float unkn2_21;
	[RszVersion(EfxVersion.RERT)]
	public float sb_unkn1;

    public UndeterminedFieldType null2_22;
	public UndeterminedFieldType null2_23;
	public UndeterminedFieldType null2_24;
}

[RszGenerate, RszAutoReadWrite, RszVersionedObject(typeof(EfxVersion)), EfxStruct(EfxAttributeType.TypeRibbonChain, EfxVersion.RE2, EfxVersion.DMC5, EfxVersion.RE4, EfxVersion.DD2)]
public partial class EFXAttributeTypeRibbonChain : EFXAttribute
{
	public EFXAttributeTypeRibbonChain() : base(EfxAttributeType.TypeRibbonChain) { }

	public uint unkn1;
	public via.Color color1;
	public via.Color color2;
	public float unkn2_0;

	public float unkn2_1;
	public float unkn2_2;
	public float unkn2_3;
    [RszVersion(EfxVersion.RE4, EndAt = nameof(re4_unkn1_4))]
	public byte re4_unkn1_1;
	public byte re4_unkn1_2;
	public byte re4_unkn1_3;
	public byte re4_unkn1_4;

	public float unkn2_4;
	public float unkn2_5;
	public float unkn2_6;
	public float unkn2_7;
    [RszVersion(EfxVersion.MHWilds)] public uint mhws_unkn1;
	public float unkn2_8;
    [RszVersion(EfxVersion.RERT)]
	public float rert_unkn1;
    [RszVersion(EfxVersion.DD2)]
	public float dd2_unkn0;
	public ByteSet unkn2_9_1;
    [RszVersion('<', EfxVersion.MHWilds)]
    public float unkn2_10;

	public float unkn2_11;
	public float unkn2_12;
	public float unkn2_13;
	public float unkn2_14;
	public float unkn2_15;
	public float unkn2_16;
	public float unkn2_17;
	public uint unkn2_18;
	public float unkn2_19;
	public float unkn2_20;
	public float unkn2_21;
	public float rert_unkn0;
	[RszVersionExact(EfxVersion.RERT)]
	public float sb_unkn1;
	[RszVersionExact(EfxVersion.RERT)]
	public float sb_unkn2;

    public float unkn2_22;
	public float unkn2_23;
    [RszVersion(EfxVersion.RE4)]
	public uint re4_unkn2_hash;
	public float unkn2_24;
	public float unkn2_25;
	public float unkn2_26;
	public float unkn2_27;
	public float unkn2_28;
	public float unkn2_29;
	public float unkn2_30;
	public float unkn2_31;
    [RszVersion('>', EfxVersion.RERT)]
	public float unkn2_32;
	public via.Color unkn2_33;
	public via.Color unkn2_34;
	public via.Color unkn2_35;
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
    [RszVersion(EfxVersion.RE3, EndAt = nameof(unkn2_49))]
	public float unkn2_48;
	public float unkn2_49;

    // [RszVersion(EfxVersion.RERT)]
    // [RszVersion("!=", EfxVersion.RE4)]
	// public float sb_unkn3;
	// public float re4_unkn0;

    [RszVersion(EfxVersion.DD2, EndAt = nameof(dd2_unkn7))]
	public float dd2_unkn1;
	public float dd2_unkn2;
	public float dd2_unkn3;
	public float dd2_unkn4;
	public float dd2_unkn5;
	public float dd2_unkn6;
	public float dd2_unkn7;
    [RszVersion(EfxVersion.MHWilds)]
	public float mhws_unkn2;
}

[RszGenerate, RszAutoReadWrite, RszVersionedObject(typeof(EfxVersion)), EfxStruct(EfxAttributeType.TypeRibbonChainExpression, EfxVersion.RE2, EfxVersion.RERT, EfxVersion.DD2)]
public partial class EFXAttributeTypeRibbonChainExpression : EFXAttribute, IExpressionAttribute
{
    public EFXExpressionList? Expression { get => expressions; set => expressions = value; }
	public BitSet ExpressionBits => expressionBits;

	public EFXAttributeTypeRibbonChainExpression() : base(EfxAttributeType.TypeRibbonChainExpression) { }

	[RszClassInstance] public readonly BitSet expressionBits = new BitSet(17);
    public ExpressionAssignType unkn1;
    public ExpressionAssignType unkn2;
    public ExpressionAssignType unkn3;
    public ExpressionAssignType unkn4;
    public ExpressionAssignType unkn5;
    public ExpressionAssignType unkn6;
    public ExpressionAssignType unkn7;
    [RszVersion(EfxVersion.RE8, EndAt = nameof(unkn9))]
    public ExpressionAssignType unkn8;
    public ExpressionAssignType unkn9;
    [RszVersion(EfxVersion.DD2, EndAt = nameof(unkn17))]
    public ExpressionAssignType unkn10;
    public ExpressionAssignType unkn11;
    public ExpressionAssignType unkn12;
    public ExpressionAssignType unkn13;
    public ExpressionAssignType unkn14;
    public ExpressionAssignType unkn15;
    public ExpressionAssignType unkn16;
    public ExpressionAssignType unkn17;
	[RszClassInstance, RszConstructorParams(nameof(Version))] public EFXExpressionList? expressions;
}

[RszGenerate, RszAutoReadWrite, RszVersionedObject(typeof(EfxVersion)), EfxStruct(EfxAttributeType.TypeRibbonChainMaterial, EfxVersion.DD2)]
public partial class EFXAttributeTypeRibbonChainMaterial : EFXAttribute
{
	public EFXAttributeTypeRibbonChainMaterial() : base(EfxAttributeType.TypeRibbonChainMaterial) { }

	public uint unkn0;
	public via.Color color1;
	public via.Color color2;
	public float unkn3;
	public UndeterminedFieldType unkn4;
	public UndeterminedFieldType unkn5;
	public UndeterminedFieldType unkn6;
	public uint unkn7;
	public float unkn8;
	public UndeterminedFieldType unkn9;
	public float unkn10;
	public float unkn11;
	public float unkn12;
	public float unkn13;
	public float unkn14;
	public uint unkn15;
	public float unkn16;
	public float unkn17;
	public float unkn18;

	public float unkn19;
	public float unkn20;
	public float color3;
	public float color4;
	public float color5;

	public uint unkn22;
	public float unkn23;
	public float unkn24;
	public float unkn25;
	public UndeterminedFieldType unkn26;
	public float unkn27;
	public UndeterminedFieldType unkn28;
	public float unkn29;
    public UndeterminedFieldType unkn30;
    public UndeterminedFieldType unkn31;
    public UndeterminedFieldType unkn32;
    public float unkn33;
    public UndeterminedFieldType unkn34;
    public UndeterminedFieldType unkn35;
    public UndeterminedFieldType unkn36;
    public float unkn37;
    public UndeterminedFieldType unkn38;
    public via.Color unkn39;
    public via.Color unkn40;
    public via.Color unkn41;
    public float unkn42;
    public float unkn43;
    public float unkn44;
    public float unkn45;
    public float unkn46;
    public float unkn47;
    public float unkn48;
    public float unkn49;
    public float unkn50;
    public float unkn51;
    public float unkn52;
    public float unkn53;
    public float unkn54;
    public float unkn55;
    public UndeterminedFieldType unkn56;
    public UndeterminedFieldType unkn57;
    public UndeterminedFieldType unkn58;
    public UndeterminedFieldType unkn59;
    public float unkn60;
    public UndeterminedFieldType unkn61;
    public float unkn62;
    public uint unkn63;

	[RszConstructorParams(nameof(Version)), RszSwitch(
		nameof(Version), ">=", EfxVersion.DD2, typeof(EfxMaterialStructV2),
		typeof(EfxMaterialStructV1)
	)]
	public EfxMaterialStructBase? material;
}

[RszGenerate, RszAutoReadWrite, RszVersionedObject(typeof(EfxVersion)), EfxStruct(EfxAttributeType.TypeRibbonChainMaterialClip, EfxVersion.DD2)]
public partial class EFXAttributeTypeRibbonChainMaterialClip : EFXAttribute, IMaterialClipAttribute
{
    public EfxMaterialClipData MaterialClip => clipData;
	EfxClipData IClipAttribute.Clip => clipData;
    public BitSet ClipBits => clipBits;

    public EFXAttributeTypeRibbonChainMaterialClip() : base(EfxAttributeType.TypeRibbonChainMaterialClip) { }

	[RszClassInstance] public readonly BitSet clipBits = new BitSet(6);
    public uint unkn1;
	[RszClassInstance, RszConstructorParams(nameof(Version))] public readonly EfxMaterialClipData clipData = new();
}

[RszGenerate, RszAutoReadWrite, RszVersionedObject(typeof(EfxVersion)), EfxStruct(EfxAttributeType.TypeRibbonChainMaterialExpression, EfxVersion.DD2)]
public partial class EFXAttributeTypeRibbonChainMaterialExpression : EFXAttribute, IMaterialExpressionAttribute
{
    public EFXMaterialExpressionList? MaterialExpressions => materialExpressions;

	public EFXAttributeTypeRibbonChainMaterialExpression() : base(EfxAttributeType.TypeRibbonChainMaterialExpression) { }

    public uint unkn1_0;
    public uint unkn1_1;
    public uint unkn1_2;
    public uint unkn1_3;
    public uint unkn1_4;
    public uint unkn1_5;
    public uint unkn1_6;
    public uint unkn1_7;
    public uint unkn1_8;
    public uint unkn1_9;
    public uint unkn1_10;
    public uint unkn1_12;
    public uint unkn1_13;
    public uint unkn1_14;
    public uint unkn1_15;
    public uint unkn1_16;
    public uint unkn1_17;
    public uint unkn1_18;

	[RszClassInstance, RszConstructorParams(nameof(Version))] public EFXMaterialExpressionList? materialExpressions;
}


[RszGenerate, RszAutoReadWrite, RszVersionedObject(typeof(EfxVersion)), EfxStruct(EfxAttributeType.TypeRibbonFixEnd, EfxVersion.RE4, EfxVersion.DD2)]
public partial class EFXAttributeTypeRibbonFixEnd : EFXAttribute
{
	public EFXAttributeTypeRibbonFixEnd() : base(EfxAttributeType.TypeRibbonFixEnd) { }

	public uint unkn1;
	public via.Color color0;
	public via.Color color1;
	public float unkn2_0;
	public float unkn2_1;
	public float unkn2_2;
	public float unkn2_3;
    [RszVersion(EfxVersion.RE4)]
	public uint re4_unkn0;
	public float unkn2_4;
	public float unkn2_5;
	public float unkn2_6;
	public float unkn2_7;
	public float unkn2_8;
	[RszVersion(EfxVersion.RERT)]
	public float sb_unkn0;
    [RszVersion(EfxVersion.DD2)]
	public float dd2_unkn0;
    public uint unkn2_9;
	public float unkn2_10;
	public float unkn2_11;
	public uint unkn2_12;
	public uint unkn2_13;
	public uint unkn2_14;
	public float unkn2_15;
	public int unkn2_16;
	public int unkn2_17;
	public int unkn2_18;
	public float unkn2_19;
	public float unkn2_20;
	public float unkn2_21;
	public float unkn2_22;
	public float unkn2_23;
	public float unkn2_24;
	public float unkn2_25;
    [RszVersion(EfxVersion.RE3, EndAt = nameof(unkn2_27))]
	public float unkn2_26;
	public float unkn2_27;
}

[RszGenerate, RszAutoReadWrite, RszVersionedObject(typeof(EfxVersion)), EfxStruct(EfxAttributeType.TypeRibbonFixEndExpression, EfxVersion.RE8, EfxVersion.DD2)]
public partial class EFXAttributeTypeRibbonFixEndExpression : ReeLib.Efx.EFXAttribute, IExpressionAttribute
{
    public EFXExpressionList? Expression { get => expressions; set => expressions = value; }
	public BitSet ExpressionBits => expressionBits;

    public EFXAttributeTypeRibbonFixEndExpression() : base(EfxAttributeType.TypeRibbonFixEndExpression) { }

	[RszClassInstance] public readonly BitSet expressionBits = new BitSet(17) { BitNameDict = new () {
		[1] = nameof(color),
		[2] = nameof(alpha),
		[14] = nameof(color2),
		[16] = nameof(color3),
	} };
    public ExpressionAssignType color;
    public ExpressionAssignType alpha;
    public ExpressionAssignType unkn3;
    public ExpressionAssignType unkn4;
    public ExpressionAssignType unkn5;
    public ExpressionAssignType unkn6;
    public ExpressionAssignType unkn7;
    [RszVersion(EfxVersion.RE8, EndAt = nameof(unkn9))]
    public ExpressionAssignType unkn8;
    public ExpressionAssignType unkn9;
    [RszVersion(EfxVersion.RE4, EndAt = nameof(unkn17))]
    public ExpressionAssignType unkn10;
    public ExpressionAssignType unkn11;
    public ExpressionAssignType color2; // may be emissive color
    public ExpressionAssignType unkn13;
    public ExpressionAssignType color3; // may be emissive color
    public ExpressionAssignType unkn15;
    public ExpressionAssignType unkn16;
    public ExpressionAssignType unkn17;
	[RszClassInstance, RszConstructorParams(nameof(Version))] public EFXExpressionList? expressions;
}

[RszGenerate, RszAutoReadWrite, RszVersionedObject(typeof(EfxVersion)), EfxStruct(EfxAttributeType.TypeRibbonFixEndMaterial, EfxVersion.DD2)]
public partial class EFXAttributeTypeRibbonFixEndMaterial : EFXAttribute
{
	public EFXAttributeTypeRibbonFixEndMaterial() : base(EfxAttributeType.TypeRibbonFixEndMaterial) { }

	public uint unkn0;
	public via.Color color1;
	public via.Color color2;
	public float unkn3;
	public UndeterminedFieldType unkn4;
	public UndeterminedFieldType unkn5;
	public UndeterminedFieldType unkn6;
	public uint unkn7;
	public float unkn8;
	public float unkn9;
	public float unkn10;
	public float unkn11;
	public float unkn12;
	public float unkn13;
	public float unkn14;
	public uint unkn15;
	public float unkn16;
	public float unkn17;
	public uint	unkn18;

	public uint unkn19;
	public uint unkn20;
	public float unkn21;
    public via.Color unkn39;
    public via.Color unkn40;
    public via.Color unkn41;
    public float unkn42;
    public float unkn43;
    public float unkn44;
    public float unkn45;
    public float unkn46;
    public float unkn47;
    public float unkn48;
    public float unkn49;
    public float unkn50;
    public uint unkn51;

	[RszConstructorParams(nameof(Version)), RszSwitch(
		nameof(Version), ">=", EfxVersion.DD2, typeof(EfxMaterialStructV2),
		typeof(EfxMaterialStructV1)
	)]
	public EfxMaterialStructBase? material;
}

[RszGenerate, RszAutoReadWrite, RszVersionedObject(typeof(EfxVersion)), EfxStruct(EfxAttributeType.TypeRibbonFixEndMaterialClip, EfxVersion.DD2)]
public partial class EFXAttributeTypeRibbonFixEndMaterialClip : EFXAttribute, IMaterialClipAttribute
{
    public EfxMaterialClipData MaterialClip => clipData;
	EfxClipData IClipAttribute.Clip => clipData;
    public BitSet ClipBits => clipBits;

    public EFXAttributeTypeRibbonFixEndMaterialClip() : base(EfxAttributeType.TypeRibbonFixEndMaterialClip) { }

	[RszClassInstance] public readonly BitSet clipBits = new BitSet(1);
    public uint unkn1;
	[RszClassInstance, RszConstructorParams(nameof(Version))] public readonly EfxMaterialClipData clipData = new();
}

[RszGenerate, RszAutoReadWrite, RszVersionedObject(typeof(EfxVersion)), EfxStruct(EfxAttributeType.TypeRibbonFixEndMaterialExpression, EfxVersion.DD2)]
public partial class EFXAttributeTypeRibbonFixEndMaterialExpression : EFXAttribute, IMaterialExpressionAttribute
{
    public EFXMaterialExpressionList? MaterialExpressions => materialExpressions;

	public EFXAttributeTypeRibbonFixEndMaterialExpression() : base(EfxAttributeType.TypeRibbonFixEndMaterialExpression) { }

    public uint unkn1_0;
    public uint unkn1_1;
    public uint unkn1_2;
    public uint unkn1_3;
    public uint unkn1_4;
    public uint unkn1_5;
    public uint unkn1_6;
    public uint unkn1_7;
    public uint unkn1_8;
    public uint unkn1_9;
    public uint unkn1_10;
    public uint unkn1_12;
    public uint unkn1_13;
    public uint unkn1_14;
    public uint unkn1_15;
    public uint unkn1_16;
    public uint unkn1_17;
    public uint unkn1_18;

	[RszClassInstance, RszConstructorParams(nameof(Version))] public EFXMaterialExpressionList? materialExpressions;
}

[RszGenerate, RszAutoReadWrite, RszVersionedObject(typeof(EfxVersion)), EfxStruct(EfxAttributeType.TypeRibbonFollow, EfxVersion.RE7, EfxVersion.RE2, EfxVersion.DMC5, EfxVersion.RE3, EfxVersion.RERT, EfxVersion.RE4)]
public partial class EFXAttributeTypeRibbonFollow : EFXAttribute
{
	public EFXAttributeTypeRibbonFollow() : base(EfxAttributeType.TypeRibbonFollow) { }

	public uint emission;
	public via.Color color0;
	public via.Color color1;
	public float unkn4;
	public float unkn5;
	public float unkn6;
	public float unkn7;
	[RszVersion(EfxVersion.RE4)]
	public uint unkn8;
	public float unkn9;
	public float unkn10;
	public float trailWidth;
	public float unkn12;
	public float sb_unkn0;
	[RszVersion(nameof(Version), ">=", EfxVersion.RE2, "&&", nameof(Version), "<=", EfxVersion.RE8)]
	public uint unkn14_re2;
	[RszVersion(nameof(Version), "<", EfxVersion.RE2, "||", nameof(Version), ">", EfxVersion.RE8)] // else
	public float unkn14;
	[RszVersion(EfxVersion.DD2)]
	public float dd2__unkn0;
	[RszVersion(EfxVersion.RERT)]
	public int rert_unkn0;
	public float unkn15;
	[RszVersion(EfxVersion.RE8)]
	public float rert_unkn1;
	public uint trailLength;
    [RszVersion(EfxVersion.RE3)]
	public uint something;
    [RszVersion("==", EfxVersion.RE7)]
	public float unkn17;

	public int ukn18;

    [RszVersion(EfxVersion.RE2, EndAt = nameof(unkn26))]
    public via.Color color2;
	public via.Color unkn19;
	public via.Color unkn20;
	public float unkn21;
	public float unkn22;
	public float unkn23;
	public float unkn24;
	public float unkn25;
	public float unkn26;
    [RszVersion(EfxVersion.RE2)]
	public float re2_unkn;
    [RszVersion(EfxVersion.RE3, EndAt = nameof(re3_unkn2))]
	public float re3_unkn1;
	public float re3_unkn2;
    [RszVersion(EfxVersion.RE8, EndAt = nameof(rert_unkn3))]
	public float rert_unkn2;
	public UndeterminedFieldType rert_unkn3;
}
[RszGenerate, RszAutoReadWrite, RszVersionedObject(typeof(EfxVersion)), EfxStruct(EfxAttributeType.TypeRibbonFollowExpression, EfxVersion.DMC5, EfxVersion.DD2)]
public partial class EFXAttributeTypeRibbonFollowExpression : EFXAttribute, IExpressionAttribute
{
	public EFXExpressionList? Expression { get => expressions; set => expressions = value; }
	public BitSet ExpressionBits => expressionBits;

	public EFXAttributeTypeRibbonFollowExpression() : base(EfxAttributeType.TypeRibbonFollowExpression) { }

	[RszClassInstance] public readonly BitSet expressionBits = new BitSet(17) { BitNameDict = new () {
		[1] = nameof(color),
		[3] = nameof(scale),
		[4] = nameof(scaleRand),
	} };
	public ExpressionAssignType color;
	public ExpressionAssignType unkn2;
	public ExpressionAssignType scale;
	public ExpressionAssignType scaleRand;
	public ExpressionAssignType unkn5;
	public ExpressionAssignType unkn6;
	public ExpressionAssignType unkn7;

    [RszVersion(EfxVersion.DD2, EndAt = nameof(unkn17))]
	public ExpressionAssignType unkn8;
	public ExpressionAssignType unkn9;
	public ExpressionAssignType unkn10;
	public ExpressionAssignType unkn11;
	public ExpressionAssignType unkn12;
	public ExpressionAssignType unkn13;
	public ExpressionAssignType unkn14;
	public ExpressionAssignType unkn15;
	public ExpressionAssignType unkn16;
	public ExpressionAssignType unkn17;
	[RszClassInstance, RszConstructorParams(nameof(Version))] public EFXExpressionList? expressions;
}

[RszGenerate, RszAutoReadWrite, RszVersionedObject(typeof(EfxVersion)), EfxStruct(EfxAttributeType.TypeRibbonFollowMaterial, EfxVersion.DD2)]
public partial class EFXAttributeTypeRibbonFollowMaterial : EFXAttribute
{
	public EFXAttributeTypeRibbonFollowMaterial() : base(EfxAttributeType.TypeRibbonFollowMaterial) { }

	public uint unkn0;
	public via.Color color1;
	public via.Color color2;
	public float unkn3;
	public UndeterminedFieldType unkn4;
	public UndeterminedFieldType unkn5;
	public UndeterminedFieldType unkn6;
	public uint unkn7;
	public float unkn8;
	public float unkn9;
	public float unkn10;
	public float unkn11;
	public float unkn12;
	public float unkn13;
	public float unkn14;
	public uint unkn15;
	public float unkn16;
	public float unkn17;
	public uint unkn18;

	public uint unkn19;
	public UndeterminedFieldType unkn20;
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
	public UndeterminedFieldType unkn32;
	public uint unkn33;

	[RszConstructorParams(nameof(Version)), RszSwitch(
		nameof(Version), ">=", EfxVersion.DD2, typeof(EfxMaterialStructV2),
		typeof(EfxMaterialStructV1)
	)]
	public EfxMaterialStructBase? material;
}

[RszGenerate, RszAutoReadWrite, RszVersionedObject(typeof(EfxVersion)), EfxStruct(EfxAttributeType.TypeRibbonFollowMaterialClip, EfxVersion.DD2)]
public partial class EFXAttributeTypeRibbonFollowMaterialClip : EFXAttribute, IMaterialClipAttribute
{
    public EfxMaterialClipData MaterialClip => clipData;
	EfxClipData IClipAttribute.Clip => clipData;
    public BitSet ClipBits => clipBits;

    public EFXAttributeTypeRibbonFollowMaterialClip() : base(EfxAttributeType.TypeRibbonFollowMaterialClip) { }

	[RszClassInstance] public readonly BitSet clipBits = new BitSet(10);
    public uint unkn1;
	[RszClassInstance, RszConstructorParams(nameof(Version))] public readonly EfxMaterialClipData clipData = new();
}

[RszGenerate, RszAutoReadWrite, RszVersionedObject(typeof(EfxVersion)), EfxStruct(EfxAttributeType.TypeRibbonFollowMaterialExpression, EfxVersion.DD2)]
public partial class EFXAttributeTypeRibbonFollowMaterialExpression : EFXAttribute, IExpressionAttribute, IMaterialExpressionAttribute
{
    public EFXExpressionList? Expression { get => expressions; set => expressions = value; }
    public EFXMaterialExpressionList? MaterialExpressions => materialExpressions;
	public BitSet ExpressionBits => expressionBits;

	public EFXAttributeTypeRibbonFollowMaterialExpression() : base(EfxAttributeType.TypeRibbonFollowMaterialExpression) { }

	[RszClassInstance] public readonly BitSet expressionBits = new BitSet(15);
    public ExpressionAssignType unkn1;
    public ExpressionAssignType unkn2;
    public ExpressionAssignType unkn3;
    public ExpressionAssignType unkn4;
    public ExpressionAssignType unkn5;
    public ExpressionAssignType unkn6;
    public ExpressionAssignType unkn7;
    public ExpressionAssignType unkn8;
    public ExpressionAssignType color1;
    public ExpressionAssignType unkn10;
    public ExpressionAssignType color2;
    public ExpressionAssignType unkn12;
    public ExpressionAssignType color3;
    public ExpressionAssignType unkn14;

	[RszClassInstance, RszConstructorParams(nameof(Version))] public EFXExpressionList? expressions;

	public ExpressionAssignType unkn15;
	public uint materialExpressionCount;

	[RszClassInstance, RszConstructorParams(nameof(Version))] public EFXMaterialExpressionList? materialExpressions;
}

[RszGenerate, RszAutoReadWrite, RszVersionedObject(typeof(EfxVersion)), EfxStruct(EfxAttributeType.TypeRibbonLightweight, EfxVersion.DMC5, EfxVersion.RE4)]
public partial class EFXAttributeTypeRibbonLightweight : EFXAttribute
{
	public EFXAttributeTypeRibbonLightweight() : base(EfxAttributeType.TypeRibbonLightweight) { }

	public uint unkn1;
	public via.Color color0;
	public via.Color color1;
	public float unkn2_0;
	public uint unkn2_1;
	public uint unkn2_2;
	public float unkn2_3;
	public float unkn2_4;
	public float unkn2_5;
	public float unkn2_6;
    [RszVersion(EfxVersion.RERT)]
	public float sb_unkn0;
	public float unkn2_7;
    [RszVersion(EfxVersion.DD2)]
	public float dd2_unkn;
	public uint unkn2_8;
    [RszVersion(EfxVersion.RE3, EndAt = nameof(unkn2_10))]
	public float unkn2_9;
	public float unkn2_10;
}

[RszGenerate, RszAutoReadWrite, RszVersionedObject(typeof(EfxVersion)), EfxStruct(EfxAttributeType.TypeRibbonParticle, EfxVersion.DD2)]
public partial class EFXAttributeTypeRibbonParticle : EFXAttribute
{
	public EFXAttributeTypeRibbonParticle() : base(EfxAttributeType.TypeRibbonParticle) { }

	public uint ukn1;
	public via.Color color1;
	public via.Color color2;
	public float ukn5;
	public uint ukn6;
	public uint ukn7;
	public float ukn8;
	public uint ukn9;

	public float ukn10;
	public float ukn11;
	public float ukn12;
	public float ukn14;
	public float ukn15;
	public float ukn16;
	public float ukn17;
	public uint color3;
	public uint color4;
	public uint color5;
	public uint ukn18;
	public float ukn19;
	public float ukn20;
	public float ukn21;
	public via.Color ukn22;
	public via.Color ukn23;
	public via.Color ukn24;
	public float ukn25;
	public float ukn26;
	public float ukn27;
	public float ukn28;
	public float ukn29;
	public float ukn30;
	public float ukn31;
	public float ukn32;
	public float ukn33;
}

[RszGenerate, RszAutoReadWrite, RszVersionedObject(typeof(EfxVersion)), EfxStruct(EfxAttributeType.TypeRibbonTrail, EfxVersion.RE7, EfxVersion.RE2, EfxVersion.RE3, EfxVersion.RE4)]
public partial class EFXAttributeTypeRibbonTrail : EFXAttribute
{
	public EFXAttributeTypeRibbonTrail() : base(EfxAttributeType.TypeRibbonTrail) { }

	public uint unkn0;
	[RszVersion(EfxVersion.RE4)]
    public uint unkn1;
    public via.Color color0;
    public via.Color color1;
    public float unkn4;
    public float unkn5;
    public float unkn6;
    public float unkn7;
    public float unkn8;
    public float unkn9;
	[RszVersion(EfxVersion.RERT)]
    public float unkn10;
    public uint unkn11;
    public uint unkn12;
}

[RszGenerate, RszAutoReadWrite, RszVersionedObject(typeof(EfxVersion)), EfxStruct(EfxAttributeType.TypeRibbonParticleExpression, EfxVersion.DD2)]
public partial class EFXAttributeTypeRibbonParticleExpression : EFXAttribute, IExpressionAttribute
{
    public EFXExpressionList? Expression { get => expressions; set => expressions = value; }
	public BitSet ExpressionBits => expressionBits;

	public EFXAttributeTypeRibbonParticleExpression() : base(EfxAttributeType.TypeRibbonParticleExpression) { }

	[RszClassInstance] public readonly BitSet expressionBits = new BitSet(17) { BitNameDict = new () {
		[1] = nameof(color),
	} };
    public ExpressionAssignType color;
    public ExpressionAssignType unkn2;
    public ExpressionAssignType unkn3;
    public ExpressionAssignType unkn4;
    public ExpressionAssignType unkn5;
    public ExpressionAssignType unkn6;
    public ExpressionAssignType unkn7;
    public ExpressionAssignType unkn8;
    public ExpressionAssignType unkn9;
    public ExpressionAssignType unkn10;
    public ExpressionAssignType unkn11;
    public ExpressionAssignType unkn12;
    public ExpressionAssignType unkn13;
    public ExpressionAssignType unkn14;
    public ExpressionAssignType unkn15;
    public ExpressionAssignType unkn16;
    public ExpressionAssignType unkn17;
	[RszClassInstance, RszConstructorParams(nameof(Version))] public EFXExpressionList? expressions;
}


[RszGenerate, RszAutoReadWrite, RszVersionedObject(typeof(EfxVersion)), EfxStruct(EfxAttributeType.TypeGpuRibbonFollow, EfxVersion.RE2, EfxVersion.DMC5, EfxVersion.RE3, EfxVersion.RE8)]
public partial class EFXAttributeTypeGpuRibbonFollow : EFXAttribute
{
	public EFXAttributeTypeGpuRibbonFollow() : base(EfxAttributeType.TypeGpuRibbonFollow) { }

	public uint unkn0;
	[RszVersion('<', EfxVersion.RE4, EndAt = nameof(unkn4))]
	public uint unkn1;
	public uint unkn2;
	public uint unkn3;
	[RszVersion(EfxVersion.RE8)]
	public uint unkn4;
	public via.Color color0;
	public via.Color color1;
	public float unkn7;

	public float unkn8;
	public float unkn9;
	public float unkn10;

	[RszVersion(EfxVersion.RE4, EndAt = nameof(re4_unkn5))]
    public uint re4_unkn1;
    public uint re4_unkn2;
    public uint re4_unkn3;
    public uint re4_unkn4;
	[RszVersionExact(EfxVersion.RE4)]
    public uint re4_unkn5;

	public float unkn11;
	public float unkn12;
	public float unkn13;
	[RszVersion(EfxVersion.RE3)]
	public float unkn14;
}
[RszGenerate, RszAutoReadWrite, RszVersionedObject(typeof(EfxVersion)), EfxStruct(EfxAttributeType.TypeGpuRibbonFollowExpression, EfxVersion.RE4)]
public partial class EFXAttributeTypeGpuRibbonFollowExpression : EFXAttribute, IExpressionAttribute
{
    public EFXExpressionList? Expression { get => expressions; set => expressions = value; }
	public BitSet ExpressionBits => expressionBits;

    public EFXAttributeTypeGpuRibbonFollowExpression() : base(EfxAttributeType.TypeGpuRibbonFollowExpression) { }

	[RszClassInstance] public readonly BitSet expressionBits = new BitSet(9) { BitNameDict = new () {
		[1] = nameof(color),
		[2] = nameof(alpha),
		[5] = nameof(colorRate),
		[8] = nameof(size),
		[9] = nameof(sizeRange),
	} };
	public ExpressionAssignType color;
	public ExpressionAssignType alpha;
	public ExpressionAssignType unkn3;
	public ExpressionAssignType unkn4;
	public ExpressionAssignType colorRate;
	public ExpressionAssignType unkn6;
	public ExpressionAssignType unkn7;
	public ExpressionAssignType size;
	public ExpressionAssignType sizeRange;
	[RszClassInstance, RszConstructorParams(nameof(Version))] public EFXExpressionList? expressions;
}

[RszGenerate, RszAutoReadWrite, RszVersionedObject(typeof(EfxVersion)), EfxStruct(EfxAttributeType.TypeGpuRibbonLength, EfxVersion.RE8)]
public partial class EFXAttributeTypeGpuRibbonLength : EFXAttribute
{
	public EFXAttributeTypeGpuRibbonLength() : base(EfxAttributeType.TypeGpuRibbonLength) { }

	public uint unkn0;
	[RszVersion('<', EfxVersion.RE4)]
	public uint unkn1;
	public via.Color color0;
	public via.Color color1;
	public float unkn3;
	public float unkn4;
	public UndeterminedFieldType unkn5;
	public float unkn6;
	[RszVersion(EfxVersion.RE4)]
    public uint re4_unkn1;
	[RszVersion("==", EfxVersion.RE4)]
    public uint re4_unkn2;
	public float unkn7;
	public float unkn8;
	public float unkn9;
	public float unkn10;
	public float unkn11;
	public uint unkn12;
	public float unkn13;
	public float unkn14;
	public uint unkn15;
	public float unkn16;
	public float unkn17;
	public float unkn18;
	public float unkn19;
	public float unkn20;
	public float unkn21;
	public float unkn22;
	public float unkn23;
}

[RszGenerate, RszAutoReadWrite, RszVersionedObject(typeof(EfxVersion)), EfxStruct(EfxAttributeType.TypeGpuRibbonLengthExpression, EfxVersion.DD2)]
public partial class EFXAttributeTypeGpuRibbonLengthExpression : EFXAttribute, IExpressionAttribute
{
    public EFXExpressionList? Expression { get => expressions; set => expressions = value; }
	public BitSet ExpressionBits => expressionBits;

	public EFXAttributeTypeGpuRibbonLengthExpression() : base(EfxAttributeType.TypeGpuRibbonLengthExpression) { }

	[RszClassInstance] public readonly BitSet expressionBits = new BitSet(14) { BitNameDict = new () {
		[1] = nameof(color), // rert: moved up by 2 (xyz = 567)
		[2] = nameof(alpha),
		[3] = nameof(alphaRate),
	} };
    public ExpressionAssignType color;
    public ExpressionAssignType alpha;
    public ExpressionAssignType alphaRate;
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
	[RszClassInstance, RszConstructorParams(nameof(Version))] public EFXExpressionList? expressions;
}
