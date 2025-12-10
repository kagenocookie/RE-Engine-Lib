using ReeLib.Efx.Enums;
using ReeLib.Efx.Structs.Common;
using ReeLib.InternalAttributes;

namespace ReeLib.Efx.Structs.Main;

[RszGenerate, RszAutoReadWrite, RszVersionedObject(typeof(EfxVersion)), EfxStruct(EfxAttributeType.TypePolygon, EfxVersion.RE7, EfxVersion.RE2, EfxVersion.DMC5, EfxVersion.RE4)]
public partial class EFXAttributeTypePolygon : EFXAttribute
{
	public EFXAttributeTypePolygon() : base(EfxAttributeType.TypePolygon) { }

	public uint Flags;
	public via.Color Color;
	public via.Color ColorRange;
    public float ColorRate;
    public float Intensity;

	public float EdgeBlendRange;
    
    [RszVersion(EfxVersion.RE4, EndAt = nameof(Flags2))]
    public float AlphaRate;
	public uint Flags2;
	public RotationOrder RotationOrder;
    public via.Range RotationX;
    public via.Range RotationY;
    public via.Range RotationZ;
    public via.Range SizeScalar;
    public via.Range Width;
    public via.Range Height;
    public via.Range Offset;
    [RszVersion('<', EfxVersion.RE4)]
	public float AlphaRateLegacy;
    [RszVersion(EfxVersion.RERT)]
	public float ShadowMultiplier;
    [RszVersion(EfxVersion.RE4)]
	public AxisXYZ OrientDirectionUpVector;
}

[RszGenerate, RszAutoReadWrite, RszVersionedObject(typeof(EfxVersion)), EfxStruct(EfxAttributeType.TypePolygonClip, EfxVersion.DMC5)]
public partial class EFXAttributeTypePolygonClip : EFXAttribute, IClipAttribute
{
    public EfxClipData Clip => clipData;
    public BitSet ClipBits => clipBits;

    public EFXAttributeTypePolygonClip() : base(EfxAttributeType.TypePolygonClip) { }

	[RszClassInstance] public readonly BitSet clipBits = new BitSet(9);
    public uint unkn1;
	[RszClassInstance] public EfxClipData clipData = new();
}

[RszGenerate, RszAutoReadWrite, RszVersionedObject(typeof(EfxVersion)), EfxStruct(EfxAttributeType.TypePolygonExpression, EfxVersion.RE7, EfxVersion.RE2, EfxVersion.DMC5, EfxVersion.RE3, EfxVersion.RE8, EfxVersion.RERT, EfxVersion.RE4, EfxVersion.DD2)]
public partial class EFXAttributeTypePolygonExpression : ReeLib.Efx.EFXAttribute, IExpressionAttribute
{
    public EFXExpressionList? Expression { get => expressions; set => expressions = value; }
	public BitSet ExpressionBits => expressionBits;

    public EFXAttributeTypePolygonExpression() : base(EfxAttributeType.TypePolygonExpression) { }

	[RszClassInstance] public readonly BitSet expressionBits = new BitSet(19) { BitNameDict = new () {
		[1] = nameof(color),
		[2] = nameof(alpha),
		[3] = nameof(emissive),
		[4] = nameof(colorStrength),
		[12] = nameof(scale),
		[14] = nameof(size),
	} };
    public ExpressionAssignType color;
    public ExpressionAssignType alpha;
    public ExpressionAssignType emissive;
    public ExpressionAssignType colorStrength;
    public ExpressionAssignType unkn5; // rert DistortionRate
    public ExpressionAssignType unkn6;
    public ExpressionAssignType unkn7;
    public ExpressionAssignType unkn8;
    public ExpressionAssignType unkn9;
    public ExpressionAssignType unkn10;
    public ExpressionAssignType unkn11;
    public ExpressionAssignType scale;
    public ExpressionAssignType unkn13; // dd2: emit size / range
    public ExpressionAssignType size; // dmc5: size / width
    public ExpressionAssignType unkn15;
    public ExpressionAssignType unkn16; // dmc5: (spect + 2) / length
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

	public uint Flags;
	public via.Color Color;
	public via.Color ColorRange;
	public RotationOrder RotationOrder;
    public via.Range RotationX;
    public via.Range RotationY;
    public via.Range RotationZ;
    public via.Range SizeScalar;
    public via.Range Width;
    public via.Range Height;
    public via.Range Offset;

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

	public uint Flags;
	public via.Color Color;
	public via.Color ColorRange;
	public float ColorRate;

	public float Intensity;
	public float EdgeBlendRate;
	public float AlphaRate;
    [RszVersion(EfxVersion.RE4)]
	public uint re4_unkn;
    [RszVersion(EfxVersion.RERT)]
	public float sb_unkn0;

	public AxisXYZ Axis;
	public via.Range Length;
	public float StretchDistance;
	public uint NumTrailDivision;
	public uint NumVerticalDivision;
	public uint NumSplineDivision;
	public uint IntervalFrame;
	public via.Color HeadColor;
	public via.Color Place1;
    public via.Color Place2;
	public float Place1Ratio;
	public float Place2Ratio;
}

[RszGenerate, RszAutoReadWrite, RszVersionedObject(typeof(EfxVersion)), EfxStruct(EfxAttributeType.TypePolygonTrailMaterial, EfxVersion.DD2)]
public partial class EFXAttributeTypePolygonTrailMaterial : EFXAttribute
{
	public EFXAttributeTypePolygonTrailMaterial() : base(EfxAttributeType.TypePolygonTrailMaterial) { }

    public uint Flags;
    public via.Color Color;
    public via.Color ColorRange;
    public AxisXYZ Axis;
    public via.Range Length;
    public float StretchDistance;
    public uint NumTrailDivision;
    public uint NumVerticalDivision;
    public uint NumSplineDivision;
    public uint IntervalFrame;
    public via.Color HeadColor;
    public via.Color Place1;
    public via.Color Place2;
    public float Place1Ratio;
    public float Place2Ratio;

    [RszConstructorParams(nameof(Version)), RszSwitch(
		nameof(Version), ">=", EfxVersion.DD2, typeof(EfxMaterialStructV2),
		typeof(EfxMaterialStructV1)
	)]
	public EfxMaterialStructBase? material;
}

[RszGenerate, RszAutoReadWrite, RszVersionedObject(typeof(EfxVersion)), EfxStruct(EfxAttributeType.TypePolygonTrailMaterialExpression, EfxVersion.DD2)]
public partial class EFXAttributeTypePolygonTrailMaterialExpression : EFXAttribute, IExpressionAttribute, IMaterialExpressionAttribute
{
    public EFXExpressionList? Expression { get => expressions; set => expressions = value; }
    public EFXMaterialExpressionList? MaterialExpressions => materialExpressions;
	public BitSet ExpressionBits => expressionBits;

	public EFXAttributeTypePolygonTrailMaterialExpression() : base(EfxAttributeType.TypePolygonTrailMaterialExpression) { }

	[RszClassInstance] public readonly BitSet expressionBits = new BitSet(7) { BitNameDict = new () {
		[1] = nameof(color),
	} };
    public ExpressionAssignType color;
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


    //TODO Fix this for older games
    public uint BlendFlags;
    public via.Color Color;
    public via.Color ColorRange;
    public float ColorRate;
    public float Intensity;
    public float EdgeBlendRate;
    public float AlphaRate;
    public uint Flags;
    public uint ParticleNum;
    
    public via.Range RotationX;
    public via.Range RotationY;
    public via.Range RotationZ;
    public via.Range SizeScalar;
    public via.Range Width;
    public via.Range Height;
    public AxisXYZ OrientDirectionUpVector;

    /*
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
    */
}

[RszGenerate, RszAutoReadWrite, RszVersionedObject(typeof(EfxVersion)), EfxStruct(EfxAttributeType.TypeGpuPolygonExpression, EfxVersion.DD2)]
public partial class EFXAttributeTypeGpuPolygonExpression : EFXAttribute, IExpressionAttribute
{
    public EFXExpressionList? Expression { get => expressions; set => expressions = value; }
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
