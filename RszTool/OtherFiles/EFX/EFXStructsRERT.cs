using System.Numerics;
using RszTool;
using RszTool.Efx.Structs.RE4;

namespace RszTool.Efx.Structs.RERT;

[RszGenerate, RszAutoReadWrite, EfxStruct(EfxAttributeType.unknRERTStruct168, EfxVersion.RERT)]
public partial class EFXAttributeunknRERTStruct168 : EFXAttribute
{
	public EFXAttributeunknRERTStruct168() : base(EfxAttributeType.unknRERTStruct168) { }

    public uint ukn0;
    public float ukn1;
    public float ukn2;
    public float ukn3;
}

[RszGenerate, RszAutoReadWrite, EfxStruct(EfxAttributeType.unknRERTStruct215, EfxVersion.RERT)]
public partial class EFXAttributeunknRERTStruct215 : EFXAttribute
{
	public EFXAttributeunknRERTStruct215() : base(EfxAttributeType.unknRERTStruct215) { }

    public uint ukn0;
    public uint ukn1;
    public uint ukn2;
    public uint ukn3;
    public uint ukn4;
    public uint ukn5;
    public uint ukn6;
    public uint ukn7;
    public uint ukn8;
    public uint ukn9;
    public uint ukn10;
    public uint ukn11;
    public uint ukn12;
    public uint ukn13;
    public uint ukn14;
}

[RszGenerate, RszAutoReadWrite, EfxStruct(EfxAttributeType.unknRERTStruct219, EfxVersion.RERT)]
public partial class EFXAttributeunknRERTStruct219 : EFXAttribute
{
    // similar to TypeGpuPolygon
	public EFXAttributeunknRERTStruct219() : base(EfxAttributeType.unknRERTStruct219) { }

    public uint ukn0;
    public uint ukn1;
    public uint ukn2;
    public via.Color ukn3; // rert 3207412549, 3213004694, 3221225471, 3212865134, 3207476021, 3103784959, 3210191284, 3099517439
    public via.Color ukn4; // rert 3207476021, 3103784959
    public float ukn5;
    public uint ukn6;
    public uint ukn7;
    public float ukn8; // rert 1065749138, 1070141403, 3217625051, 1076333878, 1078530011
    public float ukn9; // rert: 1078530011, 1086918619, 1043511491, 1057360530, 1051900099, 1060288707, 1061752795
    public float ukn10;
    public float ukn11;
    public float ukn12;
    public float ukn13;
    public float ukn14;
    public float ukn15;
    public uint ukn16;
    public uint ukn17;
    public uint ukn18;
}

[RszGenerate, RszAutoReadWrite, EfxStruct(EfxAttributeType.TypeGpuMeshTrailExpression, EfxVersion.RERT)]
public partial class EFXAttributeTypeGpuMeshTrailExpression : EFXAttribute
{
	public EFXAttributeTypeGpuMeshTrailExpression() : base(EfxAttributeType.TypeGpuMeshTrailExpression) { }

    public uint ukn0;
    public float ukn1;
    public float ukn2;
    public uint ukn3;
    public uint ukn4;
}

[RszGenerate, RszAutoReadWrite, EfxStruct(EfxAttributeType.ScreenSpaceEmitter, EfxVersion.RERT)]
public partial class EFXAttributeScreenSpaceEmitter : EFXAttribute
{
	public EFXAttributeScreenSpaceEmitter() : base(EfxAttributeType.ScreenSpaceEmitter) { }

    public float ukn0;
    public float ukn1;
    public float ukn2;
    public float ukn3;
    public float ukn4;
    [RszFixedSizeArray(13)] public float[]? ukn5;
    public uint ukn6;
    public uint ukn7;
}

[RszGenerate, RszAutoReadWrite, EfxStruct(EfxAttributeType.Transform2DClip, EfxVersion.RERT)]
public partial class EFXAttributeTransform2DClip : EFXAttribute
{
	public EFXAttributeTransform2DClip() : base(EfxAttributeType.Transform2DClip) { }

    public uint ukn0;
    public uint ukn1;
    public uint ukn2;
    public uint ukn3;
    public uint ukn4;
    [RszFixedSizeArray(34)] public uint[]? ukn5;
    public byte ukn6;
    [RszFixedSizeArray(7)] public uint[]? ukn7;
    [RszFixedSizeArray(2)] public byte[]? ukn8;
    [RszFixedSizeArray(1)] public uint[]? ukn9;
}

[RszGenerate, RszAutoReadWrite, EfxStruct(EfxAttributeType.PtVelocity2D, EfxVersion.RERT)]
public partial class EFXAttributePtVelocity2D : EFXAttribute
{
	public EFXAttributePtVelocity2D() : base(EfxAttributeType.PtVelocity2D) { }

    public float ukn0;
    public float ukn1;
    public float ukn2;
}

[RszGenerate, RszAutoReadWrite, EfxStruct(EfxAttributeType.PtVelocity2DClip, EfxVersion.RERT)]
public partial class EFXAttributePtVelocity2DClip : EFXAttribute
{
	public EFXAttributePtVelocity2DClip() : base(EfxAttributeType.PtVelocity2DClip) { }

    public uint ukn0;
    public float ukn1;
    public float ukn2;
    public uint ukn3;
}

[RszGenerate, RszAutoReadWrite, EfxStruct(EfxAttributeType.unknRERTStruct136, EfxVersion.RERT)]
public partial class EFXAttributeunknRERTStruct136 : EFXAttribute
{
    // note: looks eerily similar to UnitCulling, except for the extra last field
	public EFXAttributeunknRERTStruct136() : base(EfxAttributeType.unknRERTStruct136) { }

	public uint unkn1_0;
	public float unkn1_1;
	public float unkn1_2;
	public float unkn1_3;
	public float unkn1_4;
	public float unkn1_5;
	public float unkn1_6;
	public float unkn1_7;
	public float unkn1_8;
	public float unkn1_9;
}

[RszGenerate, RszAutoReadWrite, EfxStruct(EfxAttributeType.unknRERTStruct151_PtColor, EfxVersion.RERT)]
public partial class EFXAttributeunknRERTStruct151_PtColor : EFXAttribute
{
	public EFXAttributeunknRERTStruct151_PtColor() : base(EfxAttributeType.unknRERTStruct151_PtColor) { }

	public uint unkn1;
	public uint unkn2;
	public via.Color color0;
}

[RszGenerate, RszAutoReadWrite, EfxStruct(EfxAttributeType.unknRERTStruct152_PtColorClip, EfxVersion.RERT)]
public partial class EFXAttributeunknRERTStruct152_PtColorClip : EFXAttribute
{
    public EFXAttributeunknRERTStruct152_PtColorClip() : base(EfxAttributeType.unknRERTStruct152_PtColorClip) { }

    uint unkn0;
    uint unkn1;
    int unkn2;
    float unkn3;
    uint unkn4;
    uint unkn5;
    uint unkn6;
    uint substruct1Length;
    uint substruct2Length;
    uint substruct3Length;
    [RszFixedSizeArray(nameof(substruct1Length), '/', 4)] public int[]? substruct1;
    [RszFixedSizeArray(nameof(substruct2Length), '/', 4)] public int[]? substruct2;
    [RszFixedSizeArray(nameof(substruct3Length), '/', 4)] public float[]? substruct3;
}

[RszGenerate, RszAutoReadWrite, EfxStruct(EfxAttributeType.unknRERTStruct220Expression, EfxVersion.RERT)]
public partial class EFXAttributeunknRERTStruct220Expression : EFXAttribute
{
	public EFXAttributeunknRERTStruct220Expression() : base(EfxAttributeType.unknRERTStruct220Expression) { }

	public uint unkn1;
	public uint unkn2;
    [RszFixedSizeArray(12)] public uint[]? ukn;
	[RszClassInstance] public EFXExpressionListWrapper expressions = new();
}

[RszGenerate, RszAutoReadWrite, EfxStruct(EfxAttributeType.TypeRibbonChainExpression, EfxVersion.RE2, EfxVersion.RERT)]
public partial class EFXAttributeTypeRibbonChainExpression : EFXAttribute
{
	public EFXAttributeTypeRibbonChainExpression() : base(EfxAttributeType.TypeRibbonChainExpression) { }

	public uint unkn1;
	public uint unkn2;
    [RszFixedSizeArray(6)] public uint[]? ukn;
    [RszConditional(nameof(Version), '>', EfxVersion.RE2, EndAt = nameof(ukn5))]
    public uint ukn4;
    public uint ukn5;
	[RszClassInstance] public EFXExpressionListWrapper expressions = new();
}
