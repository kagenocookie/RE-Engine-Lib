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

	[RszClassInstance] public readonly BitSet expressionBits = new BitSet(13);
	public ExpressionAssignType unkn1;
	public ExpressionAssignType unkn2;
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

	[RszClassInstance] public readonly BitSet expressionBits = new BitSet(13);
	public ExpressionAssignType unkn1;
	public ExpressionAssignType unkn2;
	public ExpressionAssignType unkn3;
	public ExpressionAssignType unkn4;
	public ExpressionAssignType unkn5;
	public ExpressionAssignType unkn6;
	public ExpressionAssignType unkn7;
    [RszVersion(EfxVersion.RE2)]
	public ExpressionAssignType unkn8;
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

	[RszClassInstance] public readonly BitSet expressionBits = new BitSet(13);
	public ExpressionAssignType ukn1;
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

	[RszClassInstance] public readonly BitSet expressionBits = new BitSet(42);
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
	public via.Color color0;
	public via.Color color1;
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

	[RszClassInstance] public readonly BitSet expressionBits = new BitSet(14);
	public ExpressionAssignType unkn1;
	public ExpressionAssignType unkn2;
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
