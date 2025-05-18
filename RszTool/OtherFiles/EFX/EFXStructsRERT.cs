using System.Numerics;
using RszTool;
using RszTool.Efx.Structs.Common;
using RszTool.Efx.Structs.RE4;
using RszTool.InternalAttributes;

namespace RszTool.Efx.Structs.RERT;

[RszGenerate, RszAutoReadWrite, RszVersionedObject(typeof(EfxVersion)), EfxStruct(EfxAttributeType.UnknownRERT_215, EfxVersion.RERT)]
public partial class EFXAttributeUnknownRERT_215 : EFXAttribute
{
	public EFXAttributeUnknownRERT_215() : base(EfxAttributeType.UnknownRERT_215) { }

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

[RszGenerate, RszAutoReadWrite, RszVersionedObject(typeof(EfxVersion)), EfxStruct(EfxAttributeType.UnknownRERT_219, EfxVersion.RERT)]
public partial class EFXAttributeUnknownRERT_219 : EFXAttribute
{
    // similar to TypeGpuPolygon
	public EFXAttributeUnknownRERT_219() : base(EfxAttributeType.UnknownRERT_219) { }

    public uint ukn0;
    public uint ukn1;
    public uint ukn2;
    public via.Color ukn3;
    public via.Color ukn4;
    public float ukn5;
    public uint ukn6;
    public uint ukn7;
    public float ukn8;
    public float ukn9;
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

[RszGenerate, RszAutoReadWrite, RszVersionedObject(typeof(EfxVersion)), EfxStruct(EfxAttributeType.TypeGpuMeshTrailExpression, EfxVersion.RERT)]
public partial class EFXAttributeTypeGpuMeshTrailExpression : EFXAttribute
{
	public EFXAttributeTypeGpuMeshTrailExpression() : base(EfxAttributeType.TypeGpuMeshTrailExpression) { }

    public uint ukn0;
    public float ukn1;
    public float ukn2;
    public uint ukn3;
    public uint ukn4;
}

[RszGenerate, RszAutoReadWrite, RszVersionedObject(typeof(EfxVersion)), EfxStruct(EfxAttributeType.ScreenSpaceEmitter, EfxVersion.RE8, EfxVersion.RE4)]
public partial class EFXAttributeScreenSpaceEmitter : EFXAttribute
{
	public EFXAttributeScreenSpaceEmitter() : base(EfxAttributeType.ScreenSpaceEmitter) { }

    public uint unkn0;
    public uint unkn1;
}

[RszGenerate, RszAutoReadWrite, RszVersionedObject(typeof(EfxVersion)), EfxStruct(EfxAttributeType.UnknownRERT_159, EfxVersion.RERT)]
public partial class EFXAttributeUnknownRERT_159 : EFXAttribute
{
	public EFXAttributeUnknownRERT_159() : base(EfxAttributeType.UnknownRERT_159) { }

    public float unkn0;
    public float unkn1;
    public UndeterminedFieldType unkn2;
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
    public float unkn15;
    public UndeterminedFieldType unkn16;
    public UndeterminedFieldType unkn17;
    public uint ukn6;
    public uint ukn7;
}

[RszGenerate, RszAutoReadWrite, RszVersionedObject(typeof(EfxVersion)), EfxStruct(EfxAttributeType.Transform2DClip, EfxVersion.RERT)]
public partial class EFXAttributeTransform2DClip : EFXAttribute
{
	public EFXAttributeTransform2DClip() : base(EfxAttributeType.Transform2DClip) { }

    public uint ukn0;
    public uint ukn1;
    public uint ukn2;
    public uint ukn3;
    public uint ukn4;
    public uint ukn5_0;
    public uint ukn5_1;
    public uint ukn5_2;
    public uint ukn5_3;
    public uint ukn5_4;
    public uint ukn5_5;
    public uint ukn5_6;
    public uint ukn5_7;
    public uint ukn5_8;
    public uint ukn5_9;
    public uint ukn5_10;
    public uint ukn5_11;
    public uint ukn5_12;
    public uint ukn5_13;
    public uint ukn5_14;
    public uint ukn5_15;
    public uint ukn5_16;
    public uint ukn5_17;
    public uint ukn5_18;
    public uint ukn5_19;
    public uint ukn5_20;
    public uint ukn5_21;
    public uint ukn5_22;
    public uint ukn5_23;
    public uint ukn5_24;
    public uint ukn5_25;
    public uint ukn5_26;
    public uint ukn5_27;
    public uint ukn5_28;
    public uint ukn5_29;
    public uint ukn5_30;
    public uint ukn5_31;
    public uint ukn5_32;
    public uint ukn5_33;
    public byte ukn6;
    public uint ukn7_0;
    public uint ukn7_1;
    public uint ukn7_2;
    public uint ukn7_3;
    public uint ukn7_4;
    public uint ukn7_5;
    public uint ukn7_6;
    public byte ukn8_0;
    public byte ukn8_1;
    public uint ukn9;
}

[RszGenerate, RszAutoReadWrite, RszVersionedObject(typeof(EfxVersion)), EfxStruct(EfxAttributeType.PtVelocity2D, EfxVersion.RERT)]
public partial class EFXAttributePtVelocity2D : EFXAttribute
{
	public EFXAttributePtVelocity2D() : base(EfxAttributeType.PtVelocity2D) { }

    public float ukn0;
    public float ukn1;
    public float ukn2;
}

[RszGenerate, RszAutoReadWrite, RszVersionedObject(typeof(EfxVersion)), EfxStruct(EfxAttributeType.PtVelocity2DClip, EfxVersion.RERT)]
public partial class EFXAttributePtVelocity2DClip : EFXAttribute
{
	public EFXAttributePtVelocity2DClip() : base(EfxAttributeType.PtVelocity2DClip) { }

    public uint ukn0;
    public float ukn1;
    public float ukn2;
    public uint ukn3;
}

[RszGenerate, RszAutoReadWrite, RszVersionedObject(typeof(EfxVersion)), EfxStruct(EfxAttributeType.UnknownRERT_153, EfxVersion.RERT)]
public partial class EFXAttributeUnknownRERT_153 : EFXAttribute
{
	public EFXAttributeUnknownRERT_153() : base(EfxAttributeType.UnknownRERT_153) { }

	public uint unkn1;
	public uint unkn2;
	public via.Color color0;
}

[RszGenerate, RszAutoReadWrite, RszVersionedObject(typeof(EfxVersion)), EfxStruct(EfxAttributeType.UnknownRERT_154Clip, EfxVersion.RERT)]
public partial class EFXAttributeUnknownRERT_154Clip : EFXAttribute
{
    public EFXAttributeUnknownRERT_154Clip() : base(EfxAttributeType.UnknownRERT_154Clip) { }

    uint unkn0;
    uint unkn1;
    int unkn2;
    float unkn3;
    uint unkn4;
    uint unkn5;
    uint unkn6;
    [RszArraySizeField(nameof(substruct1))] int substruct1Length;
    [RszArraySizeField(nameof(substruct2))] int substruct2Length;
    [RszArraySizeField(nameof(substruct3))] int substruct3Length;
    [RszFixedSizeArray(nameof(substruct1Length), '/', 4)] public int[]? substruct1;
    [RszFixedSizeArray(nameof(substruct2Length), '/', 4)] public int[]? substruct2;
    [RszFixedSizeArray(nameof(substruct3Length), '/', 4)] public float[]? substruct3;
}

[RszGenerate, RszAutoReadWrite, RszVersionedObject(typeof(EfxVersion)), EfxStruct(EfxAttributeType.UnknownRERT_220Expression, EfxVersion.RERT)]
public partial class EFXAttributeUnknownRERT_220Expression : EFXAttribute
{
	public EFXAttributeUnknownRERT_220Expression() : base(EfxAttributeType.UnknownRERT_220Expression) { }

	public uint unkn1;
	public uint unkn2;
    public uint unkn3_0;
    public uint unkn3_1;
    public uint unkn3_2;
    public uint unkn3_3;
    public uint unkn3_4;
    public uint unkn3_5;
    public uint unkn3_6;
    public uint unkn3_7;
    public uint unkn3_8;
    public uint unkn3_9;
    public uint unkn3_10;
    public uint unkn3_11;
	[RszClassInstance] public EFXExpressionListWrapper expressions = new();
}

[RszGenerate, RszAutoReadWrite, RszVersionedObject(typeof(EfxVersion)), EfxStruct(EfxAttributeType.TypeRibbonChainExpression, EfxVersion.RE2, EfxVersion.RERT, EfxVersion.DD2)]
public partial class EFXAttributeTypeRibbonChainExpression : EFXAttribute
{
	public EFXAttributeTypeRibbonChainExpression() : base(EfxAttributeType.TypeRibbonChainExpression) { }

    public uint unkn0;
    public uint unkn1;
    public uint unkn2;
    public uint unkn3;
    public uint unkn4;
    public uint unkn5;
    public uint unkn6;
    public uint unkn7;
    [RszVersion(EfxVersion.RE8, EndAt = nameof(unkn9))]
    public uint unkn8;
    public uint unkn9;
    [RszVersion(EfxVersion.DD2, EndAt = nameof(unkn17))]
    public uint unkn10;
    public uint unkn11;
    public uint unkn12;
    public uint unkn13;
    public uint unkn14;
    public uint unkn15;
    public uint unkn16;
    public uint unkn17;
	[RszSwitch(
		nameof(Version), ">=", EfxVersion.DD2, typeof(EFXExpressionListWrapper3),
		typeof(EFXExpressionListWrapper)
	)]
	public EFXExpressionListWrapperBase expressions = null!;
}
