using RszTool.Efx.Structs.Common;
using RszTool.InternalAttributes;

namespace RszTool.Efx.Structs.RE8;

[RszGenerate, RszAutoReadWrite, RszVersionedObject(typeof(EfxVersion)), EfxStruct(EfxAttributeType.WindInfluence3D, EfxVersion.RE8, EfxVersion.RE4)]
public partial class EFXAttributeWindInfluence3D : EFXAttribute
{
	public EFXAttributeWindInfluence3D() : base(EfxAttributeType.WindInfluence3D) { }

	public float unkn1;
	public float unkn2;
	public float unkn3;
	public float unkn4;
	public uint unkn5;
	public uint unkn6;
}

[RszGenerate, RszAutoReadWrite, RszVersionedObject(typeof(EfxVersion)), EfxStruct(EfxAttributeType.DepthOcclusion, EfxVersion.RE8)]
public partial class EFXAttributeDepthOcclusion : EFXAttribute
{
	public EFXAttributeDepthOcclusion() : base(EfxAttributeType.DepthOcclusion) { }

	public uint unkn;
}

[RszGenerate, RszAutoReadWrite, RszVersionedObject(typeof(EfxVersion)), EfxStruct(EfxAttributeType.PtAngularVelocity3D, EfxVersion.RE8)]
public partial class EFXAttributePtAngularVelocity3D : EFXAttribute
{
	public EFXAttributePtAngularVelocity3D() : base(EfxAttributeType.PtAngularVelocity3D) { }

	public UndeterminedFieldType unkn1;
	public float unkn2;
	public float unkn3;
	public UndeterminedFieldType unkn4;
	public UndeterminedFieldType unkn5;
	public UndeterminedFieldType unkn6;
	public float unkn7;
	public float unkn8;
}

[RszGenerate, RszAutoReadWrite, RszVersionedObject(typeof(EfxVersion)), EfxStruct(EfxAttributeType.GlobalVectorField, EfxVersion.RE8)]
public partial class EFXAttributeGlobalVectorField : EFXAttribute
{
	public EFXAttributeGlobalVectorField() : base(EfxAttributeType.GlobalVectorField) { }

	public uint unkn1;
	public float unkn2;
	public float unkn3;
	public float unkn4;
	public UndeterminedFieldType unkn5;
	public UndeterminedFieldType unkn6;
	public UndeterminedFieldType unkn7;
	public UndeterminedFieldType unkn8;
	public UndeterminedFieldType unkn9;
	public UndeterminedFieldType unkn10;
	public UndeterminedFieldType unkn11;
}

[RszGenerate, RszAutoReadWrite, RszVersionedObject(typeof(EfxVersion)), EfxStruct(EfxAttributeType.DirectionalFieldParameter, EfxVersion.RE8)]
public partial class EFXAttributeDirectionalFieldParameter : EFXAttribute
{
	public EFXAttributeDirectionalFieldParameter() : base(EfxAttributeType.DirectionalFieldParameter) { }

	public uint unkn1;
	public float unkn2;
	public float unkn3;
	public float unkn4;
	public UndeterminedFieldType unkn5;
	public float unkn6;
	public float unkn7;
	public UndeterminedFieldType unkn8;
	public float unkn9;
	public float unkn10;
	public float unkn11;
	public UndeterminedFieldType unkn12;
	public float unkn13;
	public UndeterminedFieldType unkn14;
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

[RszGenerate, RszAutoReadWrite, RszVersionedObject(typeof(EfxVersion)), EfxStruct(EfxAttributeType.TypeRibbonLengthMaterial, EfxVersion.RE8)]
public partial class EFXAttributeTypeRibbonLengthMaterial : EFXAttribute
{
	public EFXAttributeTypeRibbonLengthMaterial() : base(EfxAttributeType.TypeRibbonLengthMaterial) { }

	public UndeterminedFieldType unkn1;
	public via.Color unkn2;
	public via.Color unkn3;
	public float unkn4;
	public UndeterminedFieldType unkn5;
	public UndeterminedFieldType unkn6;
	public float unkn7;
	public uint unkn8;
	public float unkn9;
	public UndeterminedFieldType unkn10;
	public float unkn11;
	public UndeterminedFieldType unkn12;
	[RszVersion(EfxVersion.DD2)]
	public float dd2_unkn1;
	[RszVersion(EfxVersion.DD2)]
	public float dd2_unkn2;
	[RszVersion(EfxVersion.DD2)]
	public float dd2_unkn3;
	public uint unkn13;
	public float unkn14;
	public UndeterminedFieldType unkn15;
	public int unkn16;
	public float unkn17;
	public float unkn18;
	public UndeterminedFieldType unkn19;
	public UndeterminedFieldType unkn20;
	public UndeterminedFieldType unkn21;
	public UndeterminedFieldType unkn22;
	public float unkn23;
	public UndeterminedFieldType unkn24;
	public float unkn25;
	public UndeterminedFieldType unkn26;

	public via.Color unkn27;
	public via.Color unkn28;
	public via.Color unkn29;
	public float unkn30;
	public float unkn31;
	public float unkn32;
	public float unkn33;
	public float unkn34;
	public float unkn35;
	public float unkn36;
	public float unkn37;
	public UndeterminedFieldType unkn38;
	public UndeterminedFieldType unkn39;

	[RszConstructorParams(nameof(Version)), RszSwitch(
		nameof(Version), ">=", EfxVersion.DD2, typeof(EfxMaterialStructV2),
		typeof(EfxMaterialStructV1)
	)]
	public EfxMaterialStructBase? material;
	// [RszSwitch(
	// 	nameof(Version), ">=", EfxVersion.DD2, typeof(EfxMaterialStruct),
	// 	typeof(EfxMaterialStructV1)
	// )]
	// [RszClassInstance, RszConstructorParams(nameof(Version))] public EfxMaterialStructV1? material;
}

[RszGenerate, RszAutoReadWrite, RszVersionedObject(typeof(EfxVersion)), EfxStruct(EfxAttributeType.PtTransform2DClip, EfxVersion.RE8)]
public partial class EFXAttributePtTransform2DClip : EFXAttribute
{
	public EFXAttributePtTransform2DClip() : base(EfxAttributeType.PtTransform2DClip) { }

    public uint unkn0;
    public uint null1;
    public int unkn2;
    public float unkn3;
    public uint unkn4;
    public uint unkn5;
    public uint null6;
	[RszArraySizeField(nameof(substruct1))] public int substruct1Length;
	[RszArraySizeField(nameof(substruct2))] public int substruct2Length;
	[RszArraySizeField(nameof(substruct3))] public int substruct3Length;
    [RszFixedSizeArray(nameof(substruct1Length), '/', 4)] public int[]? substruct1;
    [RszFixedSizeArray(nameof(substruct2Length), '/', 4)] public int[]? substruct2;
    [RszFixedSizeArray(nameof(substruct3Length), '/', 4)] public float[]? substruct3;
}

[RszGenerate, RszAutoReadWrite, RszVersionedObject(typeof(EfxVersion)), EfxStruct(EfxAttributeType.TypeGpuMeshTrail, EfxVersion.RE8)]
public partial class EFXAttributeTypeGpuMeshTrail : EFXAttribute
{
	public EFXAttributeTypeGpuMeshTrail() : base(EfxAttributeType.TypeGpuMeshTrail) { }

	public uint unkn0;
    public uint unkn1;
    public uint unkn2;
    public uint unkn3;
    public via.Color color0;
    public via.Color color1;
    public float unkn4;
    public uint unkn5;
    public UndeterminedFieldType unkn6;
    public UndeterminedFieldType unkn7;
    public float unkn8;
    public UndeterminedFieldType unkn9;
    public UndeterminedFieldType unkn10;
    public UndeterminedFieldType unkn11;
    public float unkn12;
    public UndeterminedFieldType unkn13;
    public float unkn14;
    public UndeterminedFieldType unkn15;
    public float unkn16;
    public UndeterminedFieldType unkn17;
    public float unkn18;
    public UndeterminedFieldType unkn19;
    public UndeterminedFieldType unkn20;
    public UndeterminedFieldType unkn21;
    public uint unkn22_count;
    public UndeterminedFieldType unkn23;
    public uint unkn24;
    public UndeterminedFieldType unkn25;
    public UndeterminedFieldType unkn26;
    public UndeterminedFieldType unkn27;
    public UndeterminedFieldType unkn28;
    public UndeterminedFieldType unkn29;
    public float unkn30;
    public float unkn31;
    public UndeterminedFieldType unkn32;
    public UndeterminedFieldType unkn33;
    public UndeterminedFieldType unkn34;
    public UndeterminedFieldType unkn35;
    public uint unkn36;
    public uint unkn37;
    public UndeterminedFieldType unkn38;
    public float unkn39;
    public float unkn40;
    public uint unkn41; // "rest" array length?
    public UndeterminedFieldType unkn42;
	[RszInlineWString] public string? meshPath;
	[RszInlineWString] public string? uknPath;
	[RszInlineWString] public string? mdfPath;

    public uint dataSize;
	// [RszFixedSizeArray(nameof(dataSize), '/', 4)] public uint[]? data;
    public uint texBlockLength;
	// [RszInlineWString, RszList(nameof(texCount))] public string[]? texPaths;
}

[RszGenerate, RszAutoReadWrite, RszVersionedObject(typeof(EfxVersion)), EfxStruct(EfxAttributeType.PlaneCollider, EfxVersion.RE8, EfxVersion.RE4)]
public partial class EFXAttributePlaneCollider : EFXAttribute
{
	public EFXAttributePlaneCollider() : base(EfxAttributeType.PlaneCollider) { }

	public uint unkn1;
	public UndeterminedFieldType unkn2;
	public float unkn3;
	public UndeterminedFieldType unkn4;
	public UndeterminedFieldType unkn5;
	public UndeterminedFieldType unkn6;
	public float unkn7;
	public uint unkn8;
	public uint unkn9;
	public float unkn10;
	public float unkn11;
	public UndeterminedFieldType unkn12;
	public UndeterminedFieldType unkn13;
	public UndeterminedFieldType unkn14;
	public uint unkn15;
	public uint unkn16;
}

[RszGenerate, RszAutoReadWrite, RszVersionedObject(typeof(EfxVersion)), EfxStruct(EfxAttributeType.TypeGpuMeshExpression, EfxVersion.RE8)]
public partial class EFXAttributeTypeGpuMeshExpression : EFXAttribute, IExpressionAttribute
{
    public EFXExpressionList? Expression => expressions;

	public EFXAttributeTypeGpuMeshExpression() : base(EfxAttributeType.TypeGpuMeshExpression) { }

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
	public uint unkn1_11;
	public uint unkn1_12;
	public uint unkn1_13;
	public uint unkn1_14;
	public uint unkn1_15;
	public uint unkn1_16;
	public uint unkn1_17;
	public uint unkn1_18;
	public uint unkn1_19;

	[RszClassInstance, RszConstructorParams(nameof(Version))] public EFXExpressionList? expressions;
}