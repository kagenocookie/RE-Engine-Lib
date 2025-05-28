using RszTool.Efx.Structs.Common;
using RszTool.InternalAttributes;

namespace RszTool.Efx.Structs.Main;

[RszGenerate, RszAutoReadWrite, RszVersionedObject(typeof(EfxVersion)), EfxStruct(EfxAttributeType.TypePolygon, EfxVersion.RE7, EfxVersion.RE2, EfxVersion.DMC5, EfxVersion.RE4)]
public partial class EFXAttributeTypePolygon : EFXAttribute
{
	public EFXAttributeTypePolygon() : base(EfxAttributeType.TypePolygon) { }

	public uint mFlags;
	public via.Color color0;
	public via.Color color1;
    public float unkn2;

	public float unkn3;
	public float unkn4_0;
	[RszVersion(EfxVersion.RE4, EndAt = nameof(re4_unkn1))]
	public float re4_unkn0;
	public uint re4_unkn1;
	public uint unkn4_1;
    public float unkn4_3;
	public float unkn4_4;
	public float unkn4_5;
	public float unkn4_6;
	public float unkn4_7;
	public float unkn4_8;
	public float unkn4_9;
	public float unkn4_10;
	public float unkn4_11;
	public float unkn4_12;
	public float unkn4_13;
	public float unkn4_14;
	public float unkn4_15;
	public float unkn4_16;
    [RszVersion('<', EfxVersion.RE4)]
	public float unkn4_17;
    [RszVersion(EfxVersion.RERT)]
	public float sb_unkn0;
    [RszVersion(EfxVersion.RE4)]
	public uint re4_unkn2;
}

[RszGenerate, RszAutoReadWrite, RszVersionedObject(typeof(EfxVersion)), EfxStruct(EfxAttributeType.TypePolygonClip, EfxVersion.DMC5)]
public partial class EFXAttributeTypePolygonClip : EFXAttribute, IClipAttribute
{
    public EfxClipData Clip => clipData;
    public BitSet ClipBits => clipBits;

    public EFXAttributeTypePolygonClip() : base(EfxAttributeType.TypePolygonClip) { }

	[RszClassInstance] public readonly BitSet clipBits = new BitSet(8);
    public uint unkn1;
	[RszClassInstance] public EfxClipData clipData = new();
}

[RszGenerate, RszAutoReadWrite, RszVersionedObject(typeof(EfxVersion)), EfxStruct(EfxAttributeType.TypePolygonExpression, EfxVersion.RE7, EfxVersion.RE2, EfxVersion.DMC5, EfxVersion.RE3, EfxVersion.RE8, EfxVersion.RERT, EfxVersion.RE4, EfxVersion.DD2)]
public partial class EFXAttributeTypePolygonExpression : RszTool.Efx.EFXAttribute, IExpressionAttribute
{
    public EFXExpressionList? Expression => expressions;
	public BitSet ExpressionBits => expressionBits;

    public EFXAttributeTypePolygonExpression() : base(EfxAttributeType.TypePolygonExpression) { }

	[RszClassInstance] public readonly BitSet expressionBits = new BitSet(19);
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
	[RszVersion(EfxVersion.RE4)]
	public ExpressionAssignType unkn19;
	[RszClassInstance, RszConstructorParams(nameof(Version))] public EFXExpressionList? expressions;
}

[RszGenerate, RszAutoReadWrite, RszVersionedObject(typeof(EfxVersion)), EfxStruct(EfxAttributeType.TypePolygonMaterial, EfxVersion.RE8, EfxVersion.DD2)]
public partial class EFXAttributeTypePolygonMaterial : EFXAttribute
{
	public EFXAttributeTypePolygonMaterial() : base(EfxAttributeType.TypePolygonMaterial) { }

	public uint unkn1;
	public via.Color unkn2;
	public via.Color unkn3;
	public int unkn4;
	public float unkn5;
	public UndeterminedFieldType unkn6;
	public float unkn7;
	public float unkn8;
	public float unkn9;
	public UndeterminedFieldType unkn10;
	public float unkn11;
	public UndeterminedFieldType unkn12;
	public float unkn13;
	public UndeterminedFieldType unkn14;
	public float unkn15;
	public UndeterminedFieldType unkn16;
	public UndeterminedFieldType unkn17;
	public float unkn18;

	[RszConstructorParams(nameof(Version)), RszSwitch(
		nameof(Version), ">=", EfxVersion.DD2, typeof(EfxMaterialStructV2),
		typeof(EfxMaterialStructV1)
	)]
	public EfxMaterialStructBase? material;
}

[RszGenerate, RszAutoReadWrite, RszVersionedObject(typeof(EfxVersion)), EfxStruct(EfxAttributeType.TypePolygonTrail, EfxVersion.RE2, EfxVersion.RERT)]
public partial class EFXAttributeTypePolygonTrail : EFXAttribute
{
	public EFXAttributeTypePolygonTrail() : base(EfxAttributeType.TypePolygonTrail) { }

	public uint unkn1;
	public via.Color color1;
	public via.Color color2;
	public float unkn4;

	public float unkn5;
	public float unkn6;
	public float unkn7;
    [RszVersionExact(EfxVersion.RE4)]
	public uint re4_unkn;
    [RszVersion(EfxVersion.RERT)]
	public float sb_unkn0;
	[RszVersion(EfxVersion.DD2)]
	public float dd2_unkn;

	public uint unkn8;
	public float unkn9;
	public float unkn10;
	public float unkn11;
	public uint unkn12;
	public uint unkn13;
	public uint unkn14;
	public uint unkn15;
	public via.Color color3;
	public via.Color color4;
    public via.Color color5;
	public float unkn19;
	public float unkn20;
}

[RszGenerate, RszAutoReadWrite, RszVersionedObject(typeof(EfxVersion)), EfxStruct(EfxAttributeType.TypePolygonTrailMaterial, EfxVersion.DD2)]
public partial class EFXAttributeTypePolygonTrailMaterial : EFXAttribute
{
	public EFXAttributeTypePolygonTrailMaterial() : base(EfxAttributeType.TypePolygonTrailMaterial) { }

    public uint unkn0;
    public via.Color unkn1;
    public via.Color unkn2;
    public uint unkn3;
    public float unkn4;
    public UndeterminedFieldType unkn5;
    public float unkn6;
    public uint unkn7;
    public uint unkn8;
    public uint unkn9;
    public uint unkn10;
    public via.Color unkn12;
    public via.Color unkn13;
    public via.Color unkn14;
    public float unkn15;
    public float unkn16;

	[RszConstructorParams(nameof(Version)), RszSwitch(
		nameof(Version), ">=", EfxVersion.DD2, typeof(EfxMaterialStructV2),
		typeof(EfxMaterialStructV1)
	)]
	public EfxMaterialStructBase? material;
}

[RszGenerate, RszAutoReadWrite, RszVersionedObject(typeof(EfxVersion)), EfxStruct(EfxAttributeType.TypePolygonTrailMaterialExpression, EfxVersion.DD2)]
public partial class EFXAttributeTypePolygonTrailMaterialExpression : EFXAttribute, IExpressionAttribute, IMaterialExpressionAttribute
{
    public EFXExpressionList? Expression => expressions;
    public EFXMaterialExpressionList? MaterialExpressions => materialExpressions;
	public BitSet ExpressionBits => expressionBits;

	public EFXAttributeTypePolygonTrailMaterialExpression() : base(EfxAttributeType.TypePolygonTrailMaterialExpression) { }

	[RszClassInstance] public readonly BitSet expressionBits = new BitSet(7);
    public ExpressionAssignType unkn1;
    public ExpressionAssignType unkn2;
    public ExpressionAssignType unkn3;
    public ExpressionAssignType unkn4;
    public ExpressionAssignType unkn5;
    public ExpressionAssignType unkn6;
	[RszClassInstance, RszConstructorParams(nameof(Version))] public EFXExpressionList? expressions;

	public ExpressionAssignType unkn7;
	public int materialExpressionCount;
	[RszClassInstance, RszConstructorParams(nameof(Version))] public EFXMaterialExpressionList? materialExpressions;
}

[RszGenerate, RszAutoReadWrite, RszVersionedObject(typeof(EfxVersion)), EfxStruct(EfxAttributeType.TypeGpuPolygon, EfxVersion.RE3, EfxVersion.RE8, EfxVersion.RERT)]
public partial class EFXAttributeTypeGpuPolygon : EFXAttribute
{
	public EFXAttributeTypeGpuPolygon() : base(EfxAttributeType.TypeGpuPolygon) { }

    public uint unkn0;
	[RszVersionExact(EfxVersion.RE3, EfxVersion.RE8, EfxVersion.RERT)]
	public uint unkn1;
	[RszVersionExact(EfxVersion.RE8, EfxVersion.RERT)]
	public uint unkn2;
    public via.Color color0;
    public via.Color color1;
    public float unkn3;

    public float unkn4;
    public float unkn5;
    public float unkn6;
	[RszVersion(EfxVersion.RE4)] public uint re_unkn1; // looks moved from unkn2
    public float unkn8;
	[RszVersionExact(EfxVersion.RE4)] public uint re_unkn2;
    public float unkn10;
    public float unkn11;
    public float unkn12;
    public float unkn13;
    public float unkn14;
    public float unkn15;
    public float unkn16;
    public float unkn17;
    public float unkn18;
    public float unkn19;
    public float unkn20;
	[RszVersion(EfxVersion.DD2)] public float dd2_unkn1;
	[RszVersion(EfxVersion.RE4)] public uint re_unkn3;
}

[RszGenerate, RszAutoReadWrite, RszVersionedObject(typeof(EfxVersion)), EfxStruct(EfxAttributeType.TypeGpuPolygonExpression, EfxVersion.DD2)]
public partial class EFXAttributeTypeGpuPolygonExpression : EFXAttribute, IExpressionAttribute
{
    public EFXExpressionList? Expression => expressions;
	public BitSet ExpressionBits => expressionBits;

	public EFXAttributeTypeGpuPolygonExpression() : base(EfxAttributeType.TypeGpuPolygonExpression) { }

	[RszClassInstance] public readonly BitSet expressionBits = new BitSet(19);
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

	[RszClassInstance, RszConstructorParams(nameof(Version))] public EFXExpressionList? expressions;
}
