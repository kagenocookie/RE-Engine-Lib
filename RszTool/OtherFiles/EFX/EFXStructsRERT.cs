using System.Numerics;
using RszTool;
using RszTool.Efx.Structs.RE4;

namespace RszTool.Efx.Structs.RERT;

[RszGenerate, RszAutoReadWrite, EfxStruct(EfxAttributeType.unknRERTStruct168, EfxVersion.RERT)]
public partial class EFXAttributeunknRERTStruct168 : EFXAttribute
{
	public EFXAttributeunknRERTStruct168() : base(EfxAttributeType.unknRERTStruct168) { }

    public uint ukn0;
    public uint ukn1;
    public float ukn2;
    public uint ukn3;
}

[RszGenerate, RszAutoReadWrite, EfxStruct(EfxAttributeType.unknRERTStruct219, EfxVersion.RERT)]
public partial class EFXAttributeunknRERTStruct219 : EFXAttribute
{
	public EFXAttributeunknRERTStruct219() : base(EfxAttributeType.unknRERTStruct219) { }

    public uint ukn0;
    public uint ukn1;
    public uint ukn2;
    public uint ukn3;
    public uint ukn4;
    public float ukn5;
    public uint ukn6;
    public uint ukn7;
    public uint ukn8;
    public uint ukn9;
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
    // [RszFixedSizeArray(64)] public uint[]? ukn9;
    // [RszInlineWString] public string? texPath;
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
