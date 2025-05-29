using RszTool.Efx.Structs.Common;
using RszTool.InternalAttributes;

namespace RszTool.Efx.Structs.Main;

[RszGenerate, RszAutoReadWrite, RszVersionedObject(typeof(EfxVersion)), EfxStruct(EfxAttributeType.TypeBillboard2D, EfxVersion.RE7, EfxVersion.RE2, EfxVersion.DMC5, EfxVersion.RERT, EfxVersion.RE4)]
public partial class EFXAttributeTypeBillboard2D : RszTool.Efx.EFXAttribute
{
    public EFXAttributeTypeBillboard2D() : base(EfxAttributeType.TypeBillboard2D) { }

    public uint unkn0;
    public via.Color unkn1;
    public via.Color unkn2;
    public float unkn3;
    public float unkn4;
    public float unkn5;
    public float unkn6;
    public float unkn7;
    public float unkn8;
    public float unkn9;
    public float unkn10;
    public float unkn11;
    public float unkn12;
    public float unkn13;
    public float unkn14;
    public uint unkn15;
}

[RszGenerate, RszAutoReadWrite, RszVersionedObject(typeof(EfxVersion)), EfxStruct(EfxAttributeType.TypeBillboard2DExpression, EfxVersion.RE7, EfxVersion.RERT, EfxVersion.RE4, EfxVersion.DD2)]
public partial class EFXAttributeTypeBillboard2DExpression : EFXAttribute, IExpressionAttribute
{
	public EFXExpressionList? Expression => expressions;
	public BitSet ExpressionBits => expressionBits;

	public EFXAttributeTypeBillboard2DExpression() : base(EfxAttributeType.TypeBillboard2DExpression) { }

	[RszClassInstance] public readonly BitSet expressionBits = new BitSet(13) { BitNameDict = new () {
		[1] = nameof(color),
		[2] = nameof(alpha),
		// 4 = alpha? (RE7)
		// 3 = alpha? (RERT)
		// 7, 8 = size (RE4, DD2)
		// 10-13 = potentially size adjacent
	} };
	public ExpressionAssignType color;
	public ExpressionAssignType alpha;
	public ExpressionAssignType unkn3;
	public ExpressionAssignType unkn4;
	public ExpressionAssignType unkn5;
    public ExpressionAssignType unkn6;
    public ExpressionAssignType unkn7;
	[RszVersion(EfxVersion.RERT, EndAt = nameof(unkn13))]
    public ExpressionAssignType unkn8;
    public ExpressionAssignType unkn9;
    public ExpressionAssignType unkn10;
    public ExpressionAssignType unkn11;
    public ExpressionAssignType unkn12;
	[RszVersion(">", EfxVersion.RERT)]
    public ExpressionAssignType unkn13;
	[RszClassInstance, RszConstructorParams(nameof(Version))] public EFXExpressionList? expressions;
}

[RszGenerate, RszAutoReadWrite, RszVersionedObject(typeof(EfxVersion)), EfxStruct(EfxAttributeType.TypeBillboard3D, EfxVersion.DMC5, EfxVersion.RE4)]
public partial class EFXAttributeTypeBillboard3D : EFXAttribute
{
	public EFXAttributeTypeBillboard3D() : base(EfxAttributeType.TypeBillboard3D) { }

	public uint unkn1;
	public via.Color color0;
	public via.Color color1;
	public float unknOpacityValue;

	public float ColorRate; // 1
	public float AlphaRate;
	public float Rotation;
	public float RotationVariation;
	public float SizeScaler;
	public float SizeScalerVariation;
	public float SizeX;
	public float SizeXVariation;
	public float SizeY;
	public float SizeYVariation; // 10

    public float unkn13;
	[RszVersion(EfxVersion.RERT)]
	public float sb_unkn0;

	public uint unkn14;
    [RszVersion(EfxVersion.RE2, EndAt = nameof(unkn16))]
    public float unkn15;
	public float unkn16;

}
[RszGenerate, RszAutoReadWrite, RszVersionedObject(typeof(EfxVersion)), EfxStruct(EfxAttributeType.TypeBillboard3DExpression, EfxVersion.DMC5, EfxVersion.RE4)]
public partial class EFXAttributeTypeBillboard3DExpression : EFXAttribute, IExpressionAttribute
{
	public EFXExpressionList? Expression => expressions;
	public BitSet ExpressionBits => expressionBits;

	public EFXAttributeTypeBillboard3DExpression() : base(EfxAttributeType.TypeBillboard3DExpression) { }

	// >= RE4
	[RszClassInstance] public readonly BitSet expressionBits = new BitSet(13) { BitNameDict = new () {
		[1] = nameof(color),
		[2] = nameof(colorRange),
		[3] = nameof(alpha),
		[4] = nameof(alphaRate),
		// dd2 adds emissive and something else in here
		[5] = nameof(emissive),
		[7] = nameof(size),
		[8] = nameof(sizeRand),
	} };

	// <= RE RT: TODO versioned bitsets
	// [RszClassInstance] public readonly BitSet expressionBits = new BitSet(13) { BitNameDict = new () {
	// 	[1] = nameof(color),
	// 	[2] = nameof(colorRange),
	// 	[3] = nameof(alpha),
	// 	[4] = nameof(alphaRate),
	// 	[5] = nameof(size),
	//	[6] = nameof(sizeRand),
	// 	[7] = nameof(colorRate),
	// 	[8] = nameof(alphaRate2),
	// } };

	public ExpressionAssignType color;
	public ExpressionAssignType colorRange;
	public ExpressionAssignType alpha;
	public ExpressionAssignType alphaRate;
	public ExpressionAssignType emissive;
    [RszVersion(EfxVersion.RE2)]
	public ExpressionAssignType unkn6;
	public ExpressionAssignType size;
	public ExpressionAssignType sizeRand;
    [RszVersion(EfxVersion.RE3, EndAt = nameof(unkn12))]
	public ExpressionAssignType unkn9;
	public ExpressionAssignType unkn10;
	public ExpressionAssignType unkn11;
	public ExpressionAssignType unkn12;
    [RszVersion(EfxVersion.RE4)]
	public ExpressionAssignType unkn13;

	[RszClassInstance, RszConstructorParams(nameof(Version))] public EFXExpressionList? expressions;

}

[RszGenerate, RszAutoReadWrite, RszVersionedObject(typeof(EfxVersion)), EfxStruct(EfxAttributeType.TypeBillboard3DMaterial, EfxVersion.DD2)]
public partial class EFXAttributeTypeBillboard3DMaterial : EFXAttribute
{
	public EFXAttributeTypeBillboard3DMaterial() : base(EfxAttributeType.TypeBillboard3DMaterial) { }

	public uint ukn0;
	public via.Color ukn1;
	public via.Color ukn2;
	public float ukn3;
	public float ukn4;
	public float ukn5;
	public float ukn6;
	public float ukn7;
	public float ukn8;
	public float ukn9;
	public float ukn10;
	public float ukn11;
	public float ukn12;

	[RszConstructorParams(nameof(Version)), RszSwitch(
		nameof(Version), ">=", EfxVersion.DD2, typeof(EfxMaterialStructV2),
		typeof(EfxMaterialStructV1)
	)]
	public EfxMaterialStructBase? material;
}

[RszGenerate, RszAutoReadWrite, RszVersionedObject(typeof(EfxVersion)), EfxStruct(EfxAttributeType.TypeBillboard3DMaterialClip, EfxVersion.DD2)]
public partial class EFXAttributeTypeBillboard3DMaterialClip : EFXAttribute, IMaterialClipAttribute
{
    public EfxMaterialClipData MaterialClip => clipData;
    public BitSet ClipBits => clipBits;

    public EFXAttributeTypeBillboard3DMaterialClip() : base(EfxAttributeType.TypeBillboard3DMaterialClip) { }

	[RszClassInstance] public readonly BitSet clipBits = new BitSet(5);
    public uint unkn1;
	[RszClassInstance, RszConstructorParams(nameof(Version))] public readonly EfxMaterialClipData clipData = new();
}

[RszGenerate, RszAutoReadWrite, RszVersionedObject(typeof(EfxVersion)), EfxStruct(EfxAttributeType.TypeBillboard3DMaterialExpression, EfxVersion.DD2)]
public partial class EFXAttributeTypeBillboard3DMaterialExpression : EFXAttribute, IExpressionAttribute, IMaterialExpressionAttribute
{
    public EFXExpressionList? Expression => expressions;
    public EFXMaterialExpressionList? MaterialExpressions => materialExpressions;
	public BitSet ExpressionBits => expressionBits;

	public EFXAttributeTypeBillboard3DMaterialExpression() : base(EfxAttributeType.TypeBillboard3DMaterialExpression) { }

	[RszClassInstance] public readonly BitSet expressionBits = new BitSet(13) { BitNameDict = new () {
		[1] = nameof(color),
	} };
	public ExpressionAssignType color;
	public ExpressionAssignType ukn2;
	public ExpressionAssignType ukn3;
	public ExpressionAssignType ukn4;
	public ExpressionAssignType ukn5;
	public ExpressionAssignType ukn6;
	public ExpressionAssignType ukn7;
	public ExpressionAssignType ukn8;
	public ExpressionAssignType ukn9;
	public ExpressionAssignType ukn10;
	public ExpressionAssignType ukn11;
	public ExpressionAssignType ukn12;
	[RszClassInstance, RszConstructorParams(nameof(Version))] public EFXExpressionList? expressions;
	public ExpressionAssignType ukn13;
	public uint materialExpressionCount;
	[RszClassInstance, RszConstructorParams(nameof(Version))] public EFXMaterialExpressionList? materialExpressions;
}

[RszGenerate, RszAutoReadWrite, RszVersionedObject(typeof(EfxVersion)), EfxStruct(EfxAttributeType.TypeNodeBillboard, EfxVersion.RE7, EfxVersion.RE2, EfxVersion.DMC5, EfxVersion.RE3, EfxVersion.RERT, EfxVersion.DD2)]
public partial class EFXAttributeTypeNodeBillboard : EFXAttribute
{
	public EFXAttributeTypeNodeBillboard() : base(EfxAttributeType.TypeNodeBillboard) { }

	public uint unkn1_0;
	public uint unkn1_1;
	public uint unkn1_2;
	[RszVersion(EfxVersion.RE2)]
	public uint unkn1_3;
	public float unkn1_4;
	public float unkn1_5;
	public float unkn1_6;
	public float unkn1_7;
	public float unkn1_8;
	public float unkn1_9;

	public via.Color color1;
	[RszVersion(EfxVersion.RE2)]
	public via.Color color2;
	public float unkn2_1;
	[RszVersion(EfxVersion.RE2, EndAt = nameof(re2_unkn1_2))]
	public float re2_unkn1_1;
	public float re2_unkn1_2;
	public float unkn2_2;
	public float unkn2_3;
	public float unkn2_4;
	public float unkn2_5;
	public float unkn2_6;
	public float unkn2_7;

	public via.Color color3;
	[RszVersion(EfxVersion.RE2)]
	public via.Color color4;
	public float unkn3_1;
	[RszVersion(EfxVersion.RE2, EndAt = nameof(re2_unkn2_2))]
	public float re2_unkn2_1;
	public float re2_unkn2_2;
	public float unkn3_2;
	public float unkn3_3;
	public float unkn3_4;
	public float unkn3_5;
	public float unkn3_6;
	public float unkn3_7;

	public via.Color color5;
	[RszVersion(EfxVersion.RE2)]
	public via.Color color6;
	public float unkn4_1;
	[RszVersion(EfxVersion.RE2, EndAt = nameof(re2_unkn3_2))]
	public float re2_unkn3_1;
	public float re2_unkn3_2;
	public float unkn4_2;
	public float unkn4_3;
	public float unkn4_4;
	public float unkn4_5;
	public float unkn4_6;
	public float unkn4_7;

	public via.Color color7;
	[RszVersion(EfxVersion.RE2)]
	public via.Color color8;
	public float unkn5_1;
	[RszVersion(EfxVersion.RE2, EndAt = nameof(re2_unkn4_2))]
	public float re2_unkn4_1;
	public float re2_unkn4_2;
	public float unkn5_2;
	public float unkn5_3;
	public float unkn5_4;
	public float unkn5_5;

	public uint unkn1_52;
	public float unkn1_53;
	public float unkn1_54;
	public float unkn1_55;
	public float unkn1_56;
	public uint unkn1_57;
	public uint unkn1_58;
	[RszVersion(EfxVersion.RERT, EndAt = nameof(unkn1_60))]
	public uint unkn1_59;
	public uint unkn1_60;
	public float unkn1_61;
}

[RszGenerate, RszAutoReadWrite, RszVersionedObject(typeof(EfxVersion)), EfxStruct(EfxAttributeType.TypeNodeBillboardExpression, EfxVersion.DMC5, EfxVersion.RERT, EfxVersion.RE4)]
public partial class EFXAttributeTypeNodeBillboardExpression : EFXAttribute, IExpressionAttribute
{
	public EFXExpressionList? Expression => expressions;
	public BitSet ExpressionBits => expressionBits;

	public EFXAttributeTypeNodeBillboardExpression() : base(EfxAttributeType.TypeNodeBillboardExpression) { }

	[RszClassInstance] public readonly BitSet expressionBits = new BitSet(42){ BitNameDict = new () {
		[1] = nameof(posX_1),
		[2] = nameof(posY_1),
		[3] = nameof(posZ_1),
		[4] = nameof(rangeX_1),
		[5] = nameof(rangeY_1),
		[6] = nameof(rangeZ_1),
		[7] = nameof(color_1),
		[8] = nameof(alpha_1),

		[9] = nameof(posX_2),
		[10] = nameof(posY_2),
		[11] = nameof(posZ_2),
		[12] = nameof(rangeX_2),
		[13] = nameof(rangeY_2),
		[14] = nameof(rangeZ_2),
		[15] = nameof(color_2),
		[16] = nameof(alpha_2),

		[17] = nameof(posX_3),
		[18] = nameof(posY_3),
		[19] = nameof(posZ_3),
		[20] = nameof(rangeX_3),
		[21] = nameof(rangeY_3),
		[22] = nameof(rangeZ_3),
		[23] = nameof(color_3),
		[24] = nameof(alpha_3),

		[25] = nameof(posX_4),
		[26] = nameof(posY_4),
		[27] = nameof(posZ_4),
		[28] = nameof(rangeX_4),
		[29] = nameof(rangeY_4),
		[30] = nameof(rangeZ_4),
		[31] = nameof(color_4),
		[32] = nameof(alpha_4),

		[35] = nameof(sizeUnkn),
	} };

	public ExpressionAssignType posX_1;
	public ExpressionAssignType posY_1;
	public ExpressionAssignType posZ_1;
	public ExpressionAssignType rangeX_1;
	public ExpressionAssignType rangeY_1;
	public ExpressionAssignType rangeZ_1;
	public ExpressionAssignType color_1;
	public ExpressionAssignType alpha_1;
	public ExpressionAssignType posX_2;
	public ExpressionAssignType posY_2;
	public ExpressionAssignType posZ_2;
	public ExpressionAssignType rangeX_2;
	public ExpressionAssignType rangeY_2;
	public ExpressionAssignType rangeZ_2;
	public ExpressionAssignType color_2;
	public ExpressionAssignType alpha_2;
	public ExpressionAssignType posX_3;
	public ExpressionAssignType posY_3;
	public ExpressionAssignType posZ_3;
	public ExpressionAssignType rangeX_3;
	public ExpressionAssignType rangeY_3;
	public ExpressionAssignType rangeZ_3;
	public ExpressionAssignType color_3;
	public ExpressionAssignType alpha_3;
	public ExpressionAssignType posX_4;
	public ExpressionAssignType posY_4;
	public ExpressionAssignType posZ_4;
	public ExpressionAssignType rangeX_4;
	public ExpressionAssignType rangeY_4;
	public ExpressionAssignType rangeZ_4;
	public ExpressionAssignType color_4;
	public ExpressionAssignType alpha_4;
	public ExpressionAssignType unkn33;
	public ExpressionAssignType unkn34;
	public ExpressionAssignType sizeUnkn;
	public ExpressionAssignType unkn36;
	public ExpressionAssignType unkn37;
	public ExpressionAssignType unkn38;
	public ExpressionAssignType unkn39;
	public ExpressionAssignType unkn40;
	public ExpressionAssignType unkn41;
	public ExpressionAssignType unkn42;
	[RszClassInstance] public EFXExpressionList expressions = new();
}

[RszGenerate, RszAutoReadWrite, RszVersionedObject(typeof(EfxVersion)), EfxStruct(EfxAttributeType.TypeGpuBillboard, EfxVersion.RE2, EfxVersion.DMC5, EfxVersion.RE3, EfxVersion.RE8, EfxVersion.RERT)]
public partial class EFXAttributeTypeGpuBillboard : EFXAttribute
{
	public EFXAttributeTypeGpuBillboard() : base(EfxAttributeType.TypeGpuBillboard) { }

    [RszVersion('<', EfxVersion.RE4)]
	public uint instanceCount;
	public uint unkn2;
    [RszVersion(nameof(Version), ">=", EfxVersion.RE8, "&&", nameof(Version), '<', EfxVersion.RE4)]
	public uint unkn3;
	public via.Color color1;
	public via.Color color2;
	public float emission;

	public float unkn5;
	public float unkn6;
	public float unkn7;
	[RszVersion(EfxVersion.RE4, EndAt = nameof(re4_unkn3))]
    public uint instanceCount_re4;
    public uint re4_unkn2;
	[RszVersion("==", EfxVersion.RE4)]
    public uint re4_unkn3;

	public float unkn8;
	public float effectScale;
	public float unkn10;
	public float unkn11;

    [RszVersion(EfxVersion.RERT, EndAt = nameof(sb_unkn5))]
	public UndeterminedFieldType sb_unkn0;
	public float sb_unkn1;
	public UndeterminedFieldType sb_unkn2;
	public float sb_unkn3;
	public UndeterminedFieldType sb_unkn4;
	public float sb_unkn5;
}

[RszGenerate, RszAutoReadWrite, RszVersionedObject(typeof(EfxVersion)), EfxStruct(EfxAttributeType.TypeGpuBillboardExpression, EfxVersion.DMC5, EfxVersion.RE4)]
public partial class EFXAttributeTypeGpuBillboardExpression : EFXAttribute, IExpressionAttribute
{
	public EFXExpressionList? Expression => expressions;
	public BitSet ExpressionBits => expressionBits;

	public EFXAttributeTypeGpuBillboardExpression() : base(EfxAttributeType.TypeGpuBillboardExpression) { }

	[RszClassInstance] public readonly BitSet expressionBits = new BitSet(14) { BitNameDict = new () {
		[1] = nameof(colorR),
		[2] = nameof(colorG),
		[3] = nameof(colorB),
		[4] = nameof(colorA),
		[5] = nameof(colorRate),
		[6] = nameof(alpha2),
		[7] = nameof(particleSize),
	}};
	public ExpressionAssignType colorR;
	public ExpressionAssignType colorG;
	public ExpressionAssignType colorB;
	public ExpressionAssignType colorA;
	public ExpressionAssignType colorRate;
	public ExpressionAssignType alpha2;
	public ExpressionAssignType particleSize;
    [RszVersion(EfxVersion.RERT, EndAt = nameof(unkn13))]
	public ExpressionAssignType unkn8;
	public ExpressionAssignType unkn9;
	public ExpressionAssignType unkn10;
	public ExpressionAssignType unkn11;
	public ExpressionAssignType unkn12;
	public ExpressionAssignType unkn13;
    [RszVersion(EfxVersion.RE4)]
	public ExpressionAssignType unkn14;

	[RszClassInstance, RszConstructorParams(nameof(Version))] public EFXExpressionList? expressions;
}

[RszGenerate, RszAutoReadWrite, RszVersionedObject(typeof(EfxVersion)), EfxStruct(EfxAttributeType.TypeGpuBillboardClip, EfxVersion.RE4)]
public partial class EFXAttributeTypeGpuBillboardClip : EFXAttribute, IClipAttribute
{
    public EfxClipData Clip => clipData;
    public BitSet ClipBits => clipBits;

    public EFXAttributeTypeGpuBillboardClip() : base(EfxAttributeType.TypeGpuBillboardClip) { }

	[RszClassInstance] public readonly BitSet clipBits = new BitSet(7);
    public UndeterminedFieldType unkn1;
	[RszClassInstance] public EfxClipData clipData = new();
}
