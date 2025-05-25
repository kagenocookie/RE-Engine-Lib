using RszTool.Efx.Structs.Common;
using RszTool.InternalAttributes;

namespace RszTool.Efx.Structs.DD2;

[RszGenerate, RszAutoReadWrite, RszVersionedObject(typeof(EfxVersion)), EfxStruct(EfxAttributeType.ParentOptionsExpression, EfxVersion.DD2)]
public partial class EFXAttributeParentOptionsExpression : EFXAttribute, IExpressionAttribute
{
    public EFXExpressionList? Expression => expressions;

	public EFXAttributeParentOptionsExpression() : base(EfxAttributeType.ParentOptionsExpression) { }

    public uint unkn1_0;
    public uint unkn1_1;
    public uint unkn1_2;
    public uint unkn1_3;
    public uint unkn1_4;
    public uint unkn1_5;
    public uint unkn1_6;
    public uint unkn1_7;
	[RszClassInstance, RszConstructorParams(nameof(Version))] public EFXExpressionList? expressions;
}

[RszGenerate, RszAutoReadWrite, RszVersionedObject(typeof(EfxVersion)), EfxStruct(EfxAttributeType.TextureUnitExpression, EfxVersion.DD2)]
public partial class EFXAttributeTextureUnitExpression : EFXAttribute, IExpressionAttribute
{
    public EFXExpressionList? Expression => expressions;

	public EFXAttributeTextureUnitExpression() : base(EfxAttributeType.TextureUnitExpression) { }

	public UndeterminedFieldType ukn0;
	public uint ukn1;
	public UndeterminedFieldType ukn2;
	public UndeterminedFieldType ukn3;
	[RszFixedSizeArray(114)] public uint[]? expressionArgs;
	[RszClassInstance, RszConstructorParams(nameof(Version))] public EFXExpressionList? expressions;}

[RszGenerate, RszAutoReadWrite, RszVersionedObject(typeof(EfxVersion)), EfxStruct(EfxAttributeType.WindInfluence3DDelayFrame, EfxVersion.DD2)]
public partial class EFXAttributeWindInfluence3DDelayFrame : EFXAttribute
{
	public EFXAttributeWindInfluence3DDelayFrame() : base(EfxAttributeType.WindInfluence3DDelayFrame) { }

	public uint frameDelay;
	public uint unkn2;
}

[RszGenerate, RszAutoReadWrite, RszVersionedObject(typeof(EfxVersion)), EfxStruct(EfxAttributeType.TypeStrainRibbonMaterial, EfxVersion.DD2)]
public partial class EFXAttributeTypeStrainRibbonMaterial : EFXAttribute
{
	public EFXAttributeTypeStrainRibbonMaterial() : base(EfxAttributeType.TypeStrainRibbonMaterial) { }

	public uint unkn0;
	public UndeterminedFieldType unkn1;
	public uint unkn2;
	public UndeterminedFieldType unkn3;
	public UndeterminedFieldType unkn4;
	public UndeterminedFieldType unkn5;
	public float unkn6;
	public float unkn7;
	public float unkn8;
	public uint unkn9;
	public float unkn10;
	public float unkn11;
	public float unkn12;
	public float unkn13;
	public float unkn14;
	public uint unkn15;
	public uint	 unkn16;
	public float unkn17;
	public float unkn18;

	public via.Color color3;
	public via.Color color4;
	public via.Color color5;

	public float unkn22;
	public float unkn23;
	public float unkn24;
	public float unkn25;
	public float unkn26;
	public UndeterminedFieldType unkn27;
	public UndeterminedFieldType unkn28;
	public UndeterminedFieldType unkn29;
	public UndeterminedFieldType unkn30;
	public UndeterminedFieldType unkn31;

	public float unkn32;
	public UndeterminedFieldType unkn33;
	public float unkn34;
	public UndeterminedFieldType unkn35;
	public float unkn36;
	public UndeterminedFieldType unkn37;
	public float unkn38;
	public UndeterminedFieldType unkn39;
	public float unkn40;
	public float unkn41;
	public float unkn42;
	public float unkn43;
	public float unkn44;
	public float unkn45;

	public uint unkn46;
	public UndeterminedFieldType unkn47;
	public uint unkn48;
	public UndeterminedFieldType unkn49;
	public UndeterminedFieldType unkn50;
	public UndeterminedFieldType unkn51;
	public UndeterminedFieldType unkn52;
	public UndeterminedFieldType unkn53;
	public uint unkn54;
	public UndeterminedFieldType unkn55;
	public UndeterminedFieldType unkn56;
	public float unkn57;
	public float unkn58;
	public float unkn59;
	public UndeterminedFieldType unkn60;
	[RszInlineWString] public string? propName;
	public UndeterminedFieldType unkn61;
	public via.Color unkn62;
	public via.Color unkn63;
	public float unkn64;
	public UndeterminedFieldType unkn65;
	public UndeterminedFieldType unkn66;
	public UndeterminedFieldType unkn67;

	[RszConstructorParams(nameof(Version)), RszSwitch(
		nameof(Version), ">=", EfxVersion.DD2, typeof(EfxMaterialStructV2),
		typeof(EfxMaterialStructV1)
	)]
	public EfxMaterialStructBase? material;
}

[RszGenerate, RszAutoReadWrite, RszVersionedObject(typeof(EfxVersion)), EfxStruct(EfxAttributeType.TypeStrainRibbonMaterialClip, EfxVersion.DD2)]
public partial class EFXAttributeTypeStrainRibbonMaterialClip : EFXAttribute
{
    public EFXAttributeTypeStrainRibbonMaterialClip() : base(EfxAttributeType.TypeStrainRibbonMaterialClip) { }

    public uint unkn0;
    public uint unkn1;
    public int unkn2;
    public float unkn3;
    public uint unkn4;
    public uint unkn5;
    public UndeterminedFieldType unkn6;
    public uint substruct1Count;
    public uint substruct2Count;
    public uint substruct3Count;
    public uint substruct4Count;
    public uint substruct5Count;
    [RszFixedSizeArray(nameof(substruct1Count), '/', 4)] public int[]? substruct1;
    [RszFixedSizeArray(nameof(substruct2Count), '/', 4)] public int[]? substruct2;
    [RszFixedSizeArray(nameof(substruct3Count), '/', 4)] public float[]? substruct3;
    [RszFixedSizeArray(nameof(substruct4Count), '*', 4)] public uint[]? substruct4;
    [RszFixedSizeArray(nameof(substruct5Count))] public uint[]? substruct5;
}

[RszGenerate, RszAutoReadWrite, RszVersionedObject(typeof(EfxVersion)), EfxStruct(EfxAttributeType.TypeStrainRibbonMaterialExpression, EfxVersion.DD2)]
public partial class EFXAttributeTypeStrainRibbonMaterialExpression : EFXAttribute, IExpressionAttribute, IMaterialExpressionAttribute
{
	public EFXExpressionList? Expression => expressions;
    public EFXMaterialExpressionList? MaterialExpressions => materialExpressions;

    public EFXAttributeTypeStrainRibbonMaterialExpression() : base(EfxAttributeType.TypeStrainRibbonMaterialExpression) { }

	public uint unkn0;
	public uint unkn1;
	public uint unkn2;
	public uint unkn3;
	public uint unkn4;
	public uint unkn5;
	public uint unkn6;
	public uint unkn7;
	public uint unkn9;
	public uint unkn10;
	public uint unkn11;
	public uint unkn12;
	public uint unkn13;
	public uint unkn14;
	public uint unkn15;
	public uint unkn16;
	public uint unkn17;
	public uint unkn18;
	public uint unkn19;
	public uint unkn20;
	public uint unkn21;
	[RszClassInstance, RszConstructorParams(nameof(Version))] public EFXExpressionList? expressions;
	public uint ukn14;
	public uint ukn15;
	[RszVersion(EfxVersion.DD2), RszClassInstance, RszConstructorParams(nameof(Version))]
	public EFXMaterialExpressionList? materialExpressions;
}

[RszGenerate, RszAutoReadWrite, RszVersionedObject(typeof(EfxVersion)), EfxStruct(EfxAttributeType.UnknownDD2_117TypeStrainRibbonExpression, EfxVersion.DD2)]
public partial class EFXAttributeUnknownDD2_117Expression : EFXAttribute, IExpressionAttribute
{
    public EFXExpressionList? Expression => expressions;

	public EFXAttributeUnknownDD2_117Expression() : base(EfxAttributeType.UnknownDD2_117TypeStrainRibbonExpression) { }

    public uint unkn0;
    public uint unkn1;
    public uint unkn2;
    public uint unkn3;
    public uint unkn4;
    public uint unkn5;
    public uint unkn6;
    public uint unkn7;
    public uint unkn8;
    public uint unkn9;
    public uint unkn10;
    public uint unkn12;
    public uint unkn13;
    public uint unkn14;
    public uint unkn15;
    public uint unkn16;
    public uint unkn17;
    public uint unkn18;
    public uint unkn19;
    public uint unkn20;
    public uint unkn21;
    public uint unkn22;
    public uint unkn23;
    public uint unkn24;
	[RszClassInstance, RszConstructorParams(nameof(Version))] public EFXExpressionList? expressions;
}

[RszGenerate, RszAutoReadWrite, RszVersionedObject(typeof(EfxVersion)), EfxStruct(EfxAttributeType.PtTransform3DExpression, EfxVersion.DD2)]
public partial class EFXAttributePtTransform3DExpression : EFXAttribute, IExpressionAttribute
{
    public EFXExpressionList? Expression => expressions;

	public EFXAttributePtTransform3DExpression() : base(EfxAttributeType.PtTransform3DExpression) { }

	public uint unkn0;
	public uint unkn1;
	public uint unkn2;
	public uint unkn3;
	public uint unkn4;
	public uint unkn5;
	public uint unkn6;
	public uint unkn7;
	public uint unkn9;
	public uint unkn10;
	[RszClassInstance, RszConstructorParams(nameof(Version))] public EFXExpressionList? expressions;
}

[RszGenerate, RszAutoReadWrite, RszVersionedObject(typeof(EfxVersion)), EfxStruct(EfxAttributeType.FluidEmitter2DExpression, EfxVersion.DD2)]
public partial class EFXAttributeFluidEmitter2DExpression : EFXAttribute, IExpressionAttribute
{
    public EFXExpressionList? Expression => expressions;

	public EFXAttributeFluidEmitter2DExpression() : base(EfxAttributeType.FluidEmitter2DExpression) { }

	public uint unkn1;
	public uint unkn2;
	public uint unkn3;
	[RszClassInstance, RszConstructorParams(nameof(Version))] public EFXExpressionList? expressions;
}

[RszGenerate, RszAutoReadWrite, RszVersionedObject(typeof(EfxVersion)), EfxStruct(EfxAttributeType.TypeRibbonLength, EfxVersion.DD2)]
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
	public uint unkn1_7;
	public float unkn1_8;
	public float unkn1_9;
	public float unkn1_10;
	public float unkn1_11;
	[RszVersion(EfxVersion.RERT)]
	public float sb_unkn0;
	public float unkn1_12;
	public float unkn1_13;
	public uint unkn1_14;
	public float unkn1_15;
	public float unkn1_16;
	public uint unkn1_17;
	public float unkn1_18;
	public float unkn1_19;
	public float unkn1_20;
	public float unkn1_21;
	public float unkn1_22;
	public float unkn1_23;
	public float unkn1_24;
	public float unkn1_25;

	public float unkn1_26;
	public float unkn1_27;
	public int unkn1_28;
	public float unkn1_29;

	public uint unkn1_30;
	public float unkn1_31;
	public float unkn1_32;
	public float unkn1_33;
	public float unkn1_34;
	public float unkn1_35;
	[RszVersion(EfxVersion.RERT, EndAt = nameof(sb_unkn2))]
	public float sb_unkn1;
	public float sb_unkn2;
	[RszVersion(EfxVersion.RE4)]
	public float re4_unkn0;
	[RszVersion(EfxVersion.DD2)]
	public uint dd2_null;
}

[RszGenerate, RszAutoReadWrite, RszVersionedObject(typeof(EfxVersion)), EfxStruct(EfxAttributeType.TypeRibbonLengthMaterialExpression, EfxVersion.DD2)]
public partial class EFXAttributeTypeRibbonLengthMaterialExpression : EFXAttribute, IExpressionAttribute, IMaterialExpressionAttribute
{
    public EFXExpressionList? Expression => expressions;
    public EFXMaterialExpressionList? MaterialExpressions => materialExpressions;

	public EFXAttributeTypeRibbonLengthMaterialExpression() : base(EfxAttributeType.TypeRibbonLengthMaterialExpression) { }

    public uint unkn0;
    public uint unkn1;
    public uint unkn2;
    public uint unkn3;
    public uint unkn4;
    public uint unkn5;
    public uint unkn6;
    public uint unkn7;
    public uint unkn8;
    public uint unkn9;
    public uint unkn10;
    public uint unkn11;
    public uint unkn12;
    public uint unkn13;
    public uint unkn14;

	[RszClassInstance, RszConstructorParams(nameof(Version))] public EFXExpressionList? expressions;

	public uint extra1;
	public uint extra2;

	[RszClassInstance, RszConstructorParams(nameof(Version))] public EFXMaterialExpressionList? materialExpressions;
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
public partial class EFXAttributeTypeRibbonChainMaterialClip : EFXAttribute
{
    public EFXAttributeTypeRibbonChainMaterialClip() : base(EfxAttributeType.TypeRibbonChainMaterialClip) { }

    public uint unkn0;
    public uint unkn1;
    public int unkn2;
    public float unkn3;
    public uint unkn4;
    public uint unkn5;
    public UndeterminedFieldType unkn6;
    public uint substruct1Count;
    public uint substruct2Count;
    public uint substruct3Count;
    public uint substruct4Count;
    public uint substruct5Count;
    [RszFixedSizeArray(nameof(substruct1Count), '/', 4)] public int[]? substruct1;
    [RszFixedSizeArray(nameof(substruct2Count), '/', 4)] public int[]? substruct2;
    [RszFixedSizeArray(nameof(substruct3Count), '/', 4)] public float[]? substruct3;
    [RszFixedSizeArray(nameof(substruct4Count), '*', 4)] public uint[]? substruct4;
    [RszFixedSizeArray(nameof(substruct5Count))] public uint[]? substruct5;
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
public partial class EFXAttributeTypeRibbonFixEndMaterialClip : EFXAttribute
{
    public EFXAttributeTypeRibbonFixEndMaterialClip() : base(EfxAttributeType.TypeRibbonFixEndMaterialClip) { }

    public uint unkn0;
    public uint unkn1;
    public int unkn2;
    public float unkn3;
    public uint unkn4;
    public uint unkn5;
    public UndeterminedFieldType unkn6;
    public uint substruct1Count;
    public uint substruct2Count;
    public uint substruct3Count;
    public uint substruct4Count;
    public uint substruct5Count;
    [RszFixedSizeArray(nameof(substruct1Count), '/', 4)] public int[]? substruct1;
    [RszFixedSizeArray(nameof(substruct2Count), '/', 4)] public int[]? substruct2;
    [RszFixedSizeArray(nameof(substruct3Count), '/', 4)] public float[]? substruct3;
    [RszFixedSizeArray(nameof(substruct4Count), '*', 4)] public uint[]? substruct4;
    [RszFixedSizeArray(nameof(substruct5Count))] public uint[]? substruct5;
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

[RszGenerate, RszAutoReadWrite, RszVersionedObject(typeof(EfxVersion)), EfxStruct(EfxAttributeType.TypeGpuRibbonLengthExpression, EfxVersion.DD2)]
public partial class EFXAttributeTypeGpuRibbonLengthExpression : EFXAttribute, IExpressionAttribute
{
    public EFXExpressionList? Expression => expressions;

	public EFXAttributeTypeGpuRibbonLengthExpression() : base(EfxAttributeType.TypeGpuRibbonLengthExpression) { }

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
    public uint unkn1_19;
    public uint unkn1_20;
    public uint unkn1_21;
    public uint unkn1_22;
    public uint unkn1_23;
	[RszClassInstance, RszConstructorParams(nameof(Version))] public EFXExpressionList? expressions;
}

[RszGenerate, RszAutoReadWrite, RszVersionedObject(typeof(EfxVersion)), EfxStruct(EfxAttributeType.TypeGpuBillboardClip, EfxVersion.RE4)]
public partial class EFXAttributeTypeGpuBillboardClip : EFXAttribute
{
    public EFXAttributeTypeGpuBillboardClip() : base(EfxAttributeType.TypeGpuBillboardClip) { }

    public uint unkn0;
    public UndeterminedFieldType unkn1;
    public via.Color unkn2;
    public float unkn3;
    public uint unkn4;
    public uint unkn5;
    public UndeterminedFieldType unkn6;
	[RszArraySizeField(nameof(substruct1))] public int substruct1Length;
	[RszArraySizeField(nameof(substruct2))] public int substruct2Length;
	[RszArraySizeField(nameof(substruct3))] public int substruct3Length;
    [RszFixedSizeArray(nameof(substruct1Length), '/', 4)] public int[]? substruct1;
    [RszFixedSizeArray(nameof(substruct2Length), '/', 4)] public int[]? substruct2;
    [RszFixedSizeArray(nameof(substruct3Length), '/', 4)] public float[]? substruct3;
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
public partial class EFXAttributeTypeRibbonFollowMaterialClip : EFXAttribute
{
    public EFXAttributeTypeRibbonFollowMaterialClip() : base(EfxAttributeType.TypeRibbonFollowMaterialClip) { }

    public uint unkn0;
    public uint unkn1;
    public int unkn2;
    public float unkn3;
    public uint unkn4;
    public uint unkn5;
    public UndeterminedFieldType unkn6;
    public uint substruct1Count;
    public uint substruct2Count;
    public uint substruct3Count;
    public uint substruct4Count;
    public uint substruct5Count;
    [RszFixedSizeArray(nameof(substruct1Count), '/', 4)] public int[]? substruct1;
    [RszFixedSizeArray(nameof(substruct2Count), '/', 4)] public int[]? substruct2;
    [RszFixedSizeArray(nameof(substruct3Count), '/', 4)] public float[]? substruct3;
    [RszFixedSizeArray(nameof(substruct4Count), '*', 4)] public uint[]? substruct4;
    [RszFixedSizeArray(nameof(substruct5Count))] public uint[]? substruct5;
}

[RszGenerate, RszAutoReadWrite, RszVersionedObject(typeof(EfxVersion)), EfxStruct(EfxAttributeType.TypeRibbonFollowMaterialExpression, EfxVersion.DD2)]
public partial class EFXAttributeTypeRibbonFollowMaterialExpression : EFXAttribute, IExpressionAttribute, IMaterialExpressionAttribute
{
    public EFXExpressionList? Expression => expressions;
    public EFXMaterialExpressionList? MaterialExpressions => materialExpressions;

	public EFXAttributeTypeRibbonFollowMaterialExpression() : base(EfxAttributeType.TypeRibbonFollowMaterialExpression) { }

    public uint unkn0;
    public uint unkn1;
    public uint unkn2;
    public uint unkn3;
    public uint unkn4;
    public uint unkn5;
    public uint unkn6;
    public uint unkn7;
    public uint unkn8;
    public uint unkn9;
    public uint unkn10;
    public uint unkn11;
    public uint unkn12;
    public uint unkn13;
    public uint unkn14;

	[RszClassInstance, RszConstructorParams(nameof(Version))] public EFXExpressionList? expressions;

	public uint extra1;
	public uint extra2;

	[RszClassInstance, RszConstructorParams(nameof(Version))] public EFXMaterialExpressionList? materialExpressions;
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
public partial class EFXAttributeTypeBillboard3DMaterialClip : EFXAttribute
{
    public EFXAttributeTypeBillboard3DMaterialClip() : base(EfxAttributeType.TypeBillboard3DMaterialClip) { }

    public uint unkn0;
    public uint unkn1;
    public int unkn2;
    public float unkn3;
    public uint unkn4;
    public uint unkn5;
    public UndeterminedFieldType unkn6;
    public uint substruct1Count;
    public uint substruct2Count;
    public uint substruct3Count;
    public uint substruct4Count;
    public uint substruct5Count;
    [RszFixedSizeArray(nameof(substruct1Count), '/', 4)] public int[]? substruct1;
    [RszFixedSizeArray(nameof(substruct2Count), '/', 4)] public int[]? substruct2;
    [RszFixedSizeArray(nameof(substruct3Count), '/', 4)] public float[]? substruct3;
    [RszFixedSizeArray(nameof(substruct4Count), '*', 4)] public uint[]? substruct4;
    [RszFixedSizeArray(nameof(substruct5Count))] public uint[]? substruct5;
}

[RszGenerate, RszAutoReadWrite, RszVersionedObject(typeof(EfxVersion)), EfxStruct(EfxAttributeType.TypeBillboard3DMaterialExpression, EfxVersion.DD2)]
public partial class EFXAttributeTypeBillboard3DMaterialExpression : EFXAttribute, IExpressionAttribute, IMaterialExpressionAttribute
{
    public EFXExpressionList? Expression => expressions;
    public EFXMaterialExpressionList? MaterialExpressions => materialExpressions;

	public EFXAttributeTypeBillboard3DMaterialExpression() : base(EfxAttributeType.TypeBillboard3DMaterialExpression) { }

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
	[RszClassInstance, RszConstructorParams(nameof(Version))] public EFXExpressionList? expressions;
	public uint ukn14;
	public uint ukn15;
	[RszClassInstance, RszConstructorParams(nameof(Version))] public EFXMaterialExpressionList? materialExpressions;
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

[RszGenerate, RszAutoReadWrite, RszVersionedObject(typeof(EfxVersion)), EfxStruct(EfxAttributeType.TypeRibbonParticleExpression, EfxVersion.DD2)]
public partial class EFXAttributeTypeRibbonParticleExpression : EFXAttribute, IExpressionAttribute
{
    public EFXExpressionList? Expression => expressions;

	public EFXAttributeTypeRibbonParticleExpression() : base(EfxAttributeType.TypeRibbonParticleExpression) { }

    public uint unkn0;
    public uint unkn1;
    public uint unkn2;
    public uint unkn3;
    public uint unkn4;
    public uint unkn5;
    public uint unkn6;
    public uint unkn7;
    public uint unkn8;
    public uint unkn9;
    public uint unkn10;
    public uint unkn11;
    public uint unkn12;
    public uint unkn13;
    public uint unkn14;
    public uint unkn15;
    public uint unkn16;
    public uint unkn17;
	[RszClassInstance, RszConstructorParams(nameof(Version))] public EFXExpressionList? expressions;
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

	public EFXAttributeTypePolygonTrailMaterialExpression() : base(EfxAttributeType.TypePolygonTrailMaterialExpression) { }

    public uint unkn1_0;
    public uint unkn1_1;
    public uint unkn1_2;
    public uint unkn1_3;
    public uint unkn1_4;
    public uint unkn1_5;
    public uint unkn1_6;
	[RszClassInstance, RszConstructorParams(nameof(Version))] public EFXExpressionList? expressions;

	public uint extra1;
	public uint extra2;
	[RszClassInstance, RszConstructorParams(nameof(Version))] public EFXMaterialExpressionList? materialExpressions;
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

	public uint unkn1;
	public uint unkn2;
	public uint unkn3;
	public uint unkn4;
	public uint unkn5;
	public uint unkn6;
	public uint unkn7;
	public uint unkn8;
	public uint unkn9;
    public uint unkn10;
    public uint unkn12;
    public uint unkn13;
    public uint unkn14;
    public uint unkn15;
    public uint unkn16;
    public uint unkn17;
    public uint unkn18;
    public uint unkn19;
    public uint unkn20;
    public uint unkn21;
    public uint unkn22;
    public uint unkn23;
    public uint unkn24;
    public uint unkn25;
    public uint unkn26;
    public uint unkn27;
    public uint unkn28;
    public uint unkn29;
    public uint unkn30;
    public uint unkn31;
    public uint unkn32;
    public uint unkn33;
    public uint unkn34;
    public uint unkn35;
    public uint unkn36;
    public uint unkn37;
    public uint unkn38;
    public uint unkn39;
    public uint unkn40;
    public uint unkn41;
    public uint unkn42;
    public uint unkn43;
    public uint unkn44;
    public uint unkn45;
	[RszClassInstance, RszConstructorParams(nameof(Version))] public EFXExpressionList? expressions;

	public uint unkn2_1;
	public uint unkn2_2;
	[RszClassInstance, RszConstructorParams(nameof(Version))] public EFXMaterialExpressionList? materialExpressions;
}

[RszGenerate, RszAutoReadWrite, RszVersionedObject(typeof(EfxVersion)), EfxStruct(EfxAttributeType.UnknownDD2_133, EfxVersion.DD2)]
public partial class EFXAttributeUnknownDD2_133 : EFXAttribute
{
	public EFXAttributeUnknownDD2_133() : base(EfxAttributeType.UnknownDD2_133) { }

    public uint unkn0;
    public uint unkn1;
    public uint unkn2;
    public uint unkn3;
    public uint unkn4;
    public uint unkn5;
    public uint unkn6;
    public uint unkn7;
    public uint unkn8;
    public uint unkn9;
    public uint unkn10;
}

[RszGenerate, RszAutoReadWrite, RszVersionedObject(typeof(EfxVersion)), EfxStruct(EfxAttributeType.UnknownDD2_134Expression, EfxVersion.DD2)]
public partial class EFXAttributeUnknownDD2_134Expression : EFXAttribute, IExpressionAttribute
{
    public EFXExpressionList? Expression => expressions;

	public EFXAttributeUnknownDD2_134Expression() : base(EfxAttributeType.UnknownDD2_134Expression) { }

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
	[RszClassInstance, RszConstructorParams(nameof(Version))] public EFXExpressionList? expressions;
}

[RszGenerate, RszAutoReadWrite, RszVersionedObject(typeof(EfxVersion)), EfxStruct(EfxAttributeType.UnknownDD2_146New, EfxVersion.DD2)]
public partial class EFXAttributeUnknownDD2_146 : EFXAttribute
{
	public EFXAttributeUnknownDD2_146() : base(EfxAttributeType.UnknownDD2_146New) { }

	public uint null1;
	public float unkn2;
	public float unkn3;
	public float unkn4;
	public float null5;

	public float unkn6;
	public float unkn7;
	public float unkn8;
	public float unkn9;
	public float unkn10;
	public float unkn11;
	public float unkn12;
	public float null13;
	public float null14;
	public float null15;

	public uint unkn16;
}

[RszGenerate, RszAutoReadWrite, RszVersionedObject(typeof(EfxVersion)), EfxStruct(EfxAttributeType.UnknownDD2_214, EfxVersion.DD2)]
public partial class EFXAttributeUnknownDD2_214 : EFXAttribute
{
	public EFXAttributeUnknownDD2_214() : base(EfxAttributeType.UnknownDD2_214) { }

	public UndeterminedFieldType unkn1;
	public UndeterminedFieldType unkn2;
	public UndeterminedFieldType unkn3;
}

[RszGenerate, RszAutoReadWrite, RszVersionedObject(typeof(EfxVersion)), EfxStruct(EfxAttributeType.UnknownDD2_218, EfxVersion.DD2)]
public partial class EFXAttributeUnknownDD2_218 : EFXAttribute
{
	public EFXAttributeUnknownDD2_218() : base(EfxAttributeType.UnknownDD2_218) { }

	public uint unkn1;
	public UndeterminedFieldType unkn2;
}

[RszGenerate, RszAutoReadWrite, RszVersionedObject(typeof(EfxVersion)), EfxStruct(EfxAttributeType.UnknownDD2_219, EfxVersion.DD2)]
public partial class EFXAttributeUnknownDD2_219 : EFXAttribute
{
	public EFXAttributeUnknownDD2_219() : base(EfxAttributeType.UnknownDD2_219) { }

	public uint unkn0;
	public uint unkn1;

    public float unkn2;
    public float unkn3;
    public float unkn4;
    public float unkn5;
    public float unkn6;
    public float unkn7;
    public float unkn8;
    public float unkn9;
    public float unkn10;
    public float unkn12;
    public float unkn13;
    public float unkn14;
    public float unkn15;
    public float unkn16;
    public float unkn17;
    public float unkn18;
    public float unkn19;
    public float unkn20;
}

[RszGenerate, RszAutoReadWrite, RszVersionedObject(typeof(EfxVersion)), EfxStruct(EfxAttributeType.UnknownDD2_220, EfxVersion.DD2)]
public partial class EFXAttributeUnknownDD2_220 : EFXAttribute
{
	public EFXAttributeUnknownDD2_220() : base(EfxAttributeType.UnknownDD2_220) { }

	public UndeterminedFieldType unkn0;
	public UndeterminedFieldType unkn1;

    public float unkn2;
    public UndeterminedFieldType unkn3;
    public UndeterminedFieldType unkn4;
    public UndeterminedFieldType unkn5;
    public float unkn6;
    public float unkn7;
}

[RszGenerate, RszAutoReadWrite, RszVersionedObject(typeof(EfxVersion)), EfxStruct(EfxAttributeType.UnknownDD2_221Expression, EfxVersion.DD2)]
public partial class EFXAttributeUnknownDD2_221Expression : EFXAttribute, IExpressionAttribute
{
    public EFXExpressionList? Expression => expressions;

	public EFXAttributeUnknownDD2_221Expression() : base(EfxAttributeType.UnknownDD2_221Expression) { }

	public uint unkn1;
	public uint unkn2;
	public uint unkn3;
	public uint unkn4;
	public uint unkn5;
	public uint unkn6;
	public uint unkn7;
	public uint unkn8;
	[RszClassInstance, RszConstructorParams(nameof(Version))] public EFXExpressionList? expressions;
}

[RszGenerate, RszAutoReadWrite, RszVersionedObject(typeof(EfxVersion)), EfxStruct(EfxAttributeType.UnknownDD2_223, EfxVersion.DD2)]
public partial class EFXAttributeUnknownDD2_223 : EFXAttribute
{
	public EFXAttributeUnknownDD2_223() : base(EfxAttributeType.UnknownDD2_223) { }

	public uint null1;
	public float unkn2;
	public float null3;
	public float null4;
	public float null5;
	public float unkn6;
	public float null7;
	public float null8;
	public float unkn9;
	public float unkn10;
	public float unkn11;
	public float unkn12;
	public float null13;
	public float null14;
	public float unkn15;
}

[RszGenerate, RszAutoReadWrite, RszVersionedObject(typeof(EfxVersion)), EfxStruct(EfxAttributeType.UnknownDD2_224, EfxVersion.DD2)]
public partial class EFXAttributeUnknownDD2_224 : EFXAttribute
{
	public EFXAttributeUnknownDD2_224() : base(EfxAttributeType.UnknownDD2_224) { }
	// note: compatible with FadeByEmitterAngle, LuminanceBleed, EmitterShape3D, ContrastHighlighter

	public uint null1;
	public float unkn2;
	public float unkn3;
	public float unkn4;
	public float null5;
	public float unkn6;
}

[RszGenerate, RszAutoReadWrite, RszVersionedObject(typeof(EfxVersion)), EfxStruct(EfxAttributeType.UnknownDD2_225Expression, EfxVersion.DD2)]
public partial class EFXAttributeUnknownDD2_225 : EFXAttribute
{
	public EFXAttributeUnknownDD2_225() : base(EfxAttributeType.UnknownDD2_225Expression) { }

	public uint unkn1;
	public uint unkn2;
	public uint unkn3;
	public uint unkn4;
	public uint unkn5;
	public uint unkn6;
	public uint unkn7;
	[RszClassInstance, RszConstructorParams(nameof(Version))] public EFXExpressionList? expressions;
}

[RszGenerate, RszAutoReadWrite, RszVersionedObject(typeof(EfxVersion)), EfxStruct(EfxAttributeType.UnknownDD2_226, EfxVersion.DD2)]
public partial class EFXAttributeUnknownDD2_226 : EFXAttribute
{
	public EFXAttributeUnknownDD2_226() : base(EfxAttributeType.UnknownDD2_226) { }

	public uint unkn1;
	public uint unkn2;
	public float unkn3;
	public uint dataSize;
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
	public float unkn16;
	public float unkn17;
	public float unkn18;
	[RszFixedSizeArray(nameof(dataSize), "* 7")] public float[]? data;
}

[RszGenerate, RszAutoReadWrite, RszVersionedObject(typeof(EfxVersion)), EfxStruct(EfxAttributeType.UnknownDD2_239, EfxVersion.DD2)]
public partial class EFXAttributeUnknownDD2_239 : EFXAttribute
{
	public EFXAttributeUnknownDD2_239() : base(EfxAttributeType.UnknownDD2_239) { }

	public uint unkn1;
	public via.Color color1;
	public via.Color color2;
	public float unkn3;
	public float unkn4;
	public float unkn5;
	public via.Color color3;
	public via.Color color4;

	public float unkn8;
	public float unkn9;
	public float unkn10;

	public byte ukn2_0;
	public uint ukn2_1;
	public uint ukn2_2;
	public uint ukn2_3;
	public uint ukn2_4;
	public uint ukn2_5;
	public uint ukn2_6;

	public byte ukn3_0;
	public uint ukn3_1;

	public byte ukn4_0;
	public uint ukn4_1;
	public uint ukn4_2;
	public uint ukn4_3;
	public uint ukn4_4;
	public uint ukn4_5;
	public uint ukn4_6;

	public byte ukn5_0;
	public uint ukn5_1;
}

[RszGenerate, RszAutoReadWrite, RszVersionedObject(typeof(EfxVersion)), EfxStruct(EfxAttributeType.UnknownDD2_243, EfxVersion.DD2)]
public partial class EFXAttributeUnknownDD2_243 : EFXAttribute
{
	public EFXAttributeUnknownDD2_243() : base(EfxAttributeType.UnknownDD2_243) { }

	public UndeterminedFieldType unkn0;
	public UndeterminedFieldType unkn1;
	public UndeterminedFieldType unkn2;
	public UndeterminedFieldType unkn3;
	public UndeterminedFieldType unkn4;
	public uint unkn5;
	public UndeterminedFieldType unkn6;
	public UndeterminedFieldType unkn7;
	public uint unkn8;
	public UndeterminedFieldType unkn9;
}

[RszGenerate, RszAutoReadWrite, RszVersionedObject(typeof(EfxVersion)), EfxStruct(EfxAttributeType.UnknownDD2_245Efcsv, EfxVersion.DD2)]
public partial class EFXAttributeUnknownDD2_245Efcsv : EFXAttribute
{
	public EFXAttributeUnknownDD2_245Efcsv() : base(EfxAttributeType.UnknownDD2_245Efcsv) { }

	public uint unkn0;
	public UndeterminedFieldType unkn1;
	public UndeterminedFieldType unkn2;
	public uint unkn3;
	public UndeterminedFieldType unkn4;
	public UndeterminedFieldType unkn5;
	public UndeterminedFieldType unkn6;
	[RszInlineWString] public string? efcsvPath;
	[RszInlineWString] public string? unknPath2;
	[RszInlineWString] public string? unknPath3;
	[RszInlineWString] public string? unknPath4;
}

[RszGenerate, RszAutoReadWrite, RszVersionedObject(typeof(EfxVersion)), EfxStruct(EfxAttributeType.UnknownDD2_247, EfxVersion.DD2)]
public partial class EFXAttributeUnknownDD2_247 : EFXAttribute
{
	public EFXAttributeUnknownDD2_247() : base(EfxAttributeType.UnknownDD2_247) { }

	public uint unkn0;
	public bool unkn1;
	public float unkn2;
	public UndeterminedFieldType unkn3;
	public float unkn4;
	public float unkn5;
	public float unkn6;
	public float unkn7;
	public short unkn8;
	public float unkn9;
	public UndeterminedFieldType unkn10;
	public float unkn11;
	public UndeterminedFieldType unkn12;
	public float unkn13;
}

[RszGenerate, RszAutoReadWrite, RszVersionedObject(typeof(EfxVersion)), EfxStruct(EfxAttributeType.UnknownDD2_249, EfxVersion.DD2)]
public partial class EFXAttributeUnknownDD2_249 : EFXAttribute
{
	public EFXAttributeUnknownDD2_249() : base(EfxAttributeType.UnknownDD2_249) { }

	public uint unkn_bitmask;
}

[RszGenerate, RszAutoReadWrite, RszVersionedObject(typeof(EfxVersion)), EfxStruct(EfxAttributeType.UnknownDD2_250, EfxVersion.DD2)]
public partial class EFXAttributeUnknownDD2_250 : EFXAttribute
{
	public EFXAttributeUnknownDD2_250() : base(EfxAttributeType.UnknownDD2_250) { }
}

[RszGenerate, RszAutoReadWrite, RszVersionedObject(typeof(EfxVersion)), EfxStruct(EfxAttributeType.UnknownDD2_255Expression, EfxVersion.DD2)]
public partial class EFXAttributeUnknownDD2_255Expression : EFXAttribute, IExpressionAttribute
{
    public EFXExpressionList? Expression => expressions;

	public EFXAttributeUnknownDD2_255Expression() : base(EfxAttributeType.UnknownDD2_255Expression) { }

    public uint unkn0;
    public uint unkn1;
    public uint unkn2;
    public uint unkn3;
    public uint unkn4;
    public uint unkn5;
    public uint unkn6;
    public uint unkn7;
    public uint unkn8;
    public uint unkn9;
    public uint unkn10;
    public uint unkn11;
    public uint unkn12;
    public uint unkn13;
    public uint unkn14;
    public uint unkn15;
    public uint unkn16;
    public uint unkn17;
    public uint unkn18;
    public uint unkn19;

	[RszClassInstance, RszConstructorParams(nameof(Version))] public EFXExpressionList? expressions;
}

[RszGenerate, RszAutoReadWrite, RszVersionedObject(typeof(EfxVersion)), EfxStruct(EfxAttributeType.UnknownDD2_259Expression, EfxVersion.DD2)]
public partial class EFXAttributeUnknownDD2_259Expression : EFXAttribute, IExpressionAttribute
{
    public EFXExpressionList? Expression => expressions;

	public EFXAttributeUnknownDD2_259Expression() : base(EfxAttributeType.UnknownDD2_259Expression) { }

	public uint unkn0;
	public uint unkn1;
	public uint unkn2;
	public uint unkn3;
	public uint unkn4;
	public uint unkn5;
	public uint unkn6;
	public uint unkn7;
	public uint unkn8;
	public uint unkn9;
	public uint unkn10;
	public uint unkn11;
	public uint unkn12;
	public uint unkn13;

	[RszClassInstance, RszConstructorParams(nameof(Version))] public EFXExpressionList? expressions;
}

[RszGenerate, RszAutoReadWrite, RszVersionedObject(typeof(EfxVersion)), EfxStruct(EfxAttributeType.UnknownDD2_260_TypeMeshUkn, EfxVersion.DD2)]
public partial class EFXAttributeUnknownDD2_260 : EFXAttribute
{
	public EFXAttributeUnknownDD2_260() : base(EfxAttributeType.UnknownDD2_260_TypeMeshUkn) { }

	public uint unkn0_flag;
	public uint unkn1;
	public uint unkn2;
	public via.Color unkn3;
    public via.Color unkn4;
    public float unkn5;
    public uint unkn6;
    public uint unkn7;
    public uint unkn8;
    public float unkn9;
    public float unkn10;
    public float unkn12;

    public float unkn13;
    public uint unkn14;
    public UndeterminedFieldType unkn15;
    public uint unkn16;
    public float unkn17;
    public float unkn18;
    public float unkn19;
    public float unkn20;
    public float unkn21;
    public float unkn22;
    public float unkn23;
    public float unkn24;
    public float unkn25;
    public float unkn26;
    public float unkn27;
    public float unkn28;
    public float unkn29;
    public float unkn30;
    public uint unkn31;
    public float unkn32;
	public uint texCount;
	public UndeterminedFieldType unkn33;
	[RszInlineWString] public string? meshPath;
	[RszInlineWString] public string? uknPath;
	[RszInlineWString] public string? mdfPath;
	[RszArraySizeField(nameof(properties))] public int propertiesDataSize;
	[RszClassInstance, RszList(nameof(propertiesDataSize), '/', 32), RszConstructorParams(nameof(Version))]
	public List<MdfProperty> properties = new();
	public uint texBlockSize;
	[RszInlineWString, RszList(nameof(texCount))] public string[]? texPaths;
}

[RszGenerate, RszAutoReadWrite, RszVersionedObject(typeof(EfxVersion)), EfxStruct(EfxAttributeType.UnknownDD2_261, EfxVersion.DD2)]
public partial class EFXAttributeUnknownDD2_261 : EFXAttribute
{
	public EFXAttributeUnknownDD2_261() : base(EfxAttributeType.UnknownDD2_261) { }

    public uint unkn0;
    public UndeterminedFieldType unkn1;
    public uint unkn2;
    public float unkn3;
    public uint unkn4;
    public uint unkn5;
    public UndeterminedFieldType unkn6;
    public uint unkn7;
    public uint unkn8;
    public UndeterminedFieldType unkn9;
    public uint unkn10;
    public uint unkn11;
    public uint unkn12;
    public uint unkn13;
    public UndeterminedFieldType unkn14;
    public uint unkn15;
    public float unkn16;
    public float unkn17;
    public uint unkn18;
    public float unkn19;
    public float unkn20;
    public uint unkn21;
    public float unkn22;
    public uint unkn23_mdfPropertyHash;
    public UndeterminedFieldType unkn24;
    public UndeterminedFieldType unkn25;
    public uint unkn26;
    public uint unkn27;
}

[RszGenerate, RszAutoReadWrite, RszVersionedObject(typeof(EfxVersion)), EfxStruct(EfxAttributeType.UnknownDD2_262Expression, EfxVersion.DD2)]
public partial class EFXAttributeUnknownDD2_262 : EFXAttribute
{
	public EFXAttributeUnknownDD2_262() : base(EfxAttributeType.UnknownDD2_262Expression) { }

	public uint substructCount;
	public uint unkn1;
	public uint unkn2;
	public uint unkn3;

	public uint unkn4_0;
	public uint unkn4_1;
	public uint unkn4_2;
	public uint unkn4_3;
	public uint unkn4_4;
	public uint unkn4_5;
	public uint unkn4_6;
	public uint unkn4_7;
	public uint unkn4_8;
	public uint unkn4_9;
	public uint unkn4_10;
	public uint unkn4_11;
	public uint unkn4_12;
	public uint unkn4_13;
	public uint unkn4_14;
	public uint substruct2Count; // [2]   maybe?

	[RszClassInstance, RszConstructorParams(nameof(Version))] public EFXExpressionList? expressions;
	// [RszVersion(EfxVersion.RE4)]
	// public uint unkn5_24;
	// [RszFixedSizeArray(nameof(substructCount))] public uint[]? extra;
	// [RszFixedSizeArray(nameof(substruct2Count))] public uint[]? extra2;
	public uint extra3;
	public uint extra4;
	public uint extra5;

	// [RszClassInstance] public EFXExpressionListWrapper2? expression2;
}

[RszGenerate, RszAutoReadWrite, RszVersionedObject(typeof(EfxVersion)), EfxStruct(EfxAttributeType.UnknownDD2_263, EfxVersion.DD2)]
public partial class EFXAttributeUnknownDD2_263 : EFXAttribute
{
	public EFXAttributeUnknownDD2_263() : base(EfxAttributeType.UnknownDD2_263) { }

	public uint unknFlags;
	public uint unkn1;
	public uint unkn2;
	public uint unkn3;
	public uint unkn4;
	public float unkn5;
	public uint unkn6;
	public uint unkn7;
	public uint unkn8;
	public float unkn9;
	public float unkn10;
	public float unkn11;
	public UndeterminedFieldType unkn12;
	public uint unkn13;
	public UndeterminedFieldType unkn14;
	public uint unkn15;
	public UndeterminedFieldType unkn16;
	public UndeterminedFieldType unkn17;
	public UndeterminedFieldType unkn18;
	public UndeterminedFieldType unkn19;
	public UndeterminedFieldType unkn20;
	public UndeterminedFieldType unkn21;
	public float unkn22;
	public UndeterminedFieldType unkn23;
	public float unkn24;
	public UndeterminedFieldType unkn25;
	public float unkn26;
	public UndeterminedFieldType unkn27;
	public float unkn28;
	public UndeterminedFieldType unkn29;
	public uint unkn30;
	public float unkn31;
	public uint texCount;
	public UndeterminedFieldType unkn32;
	public uint unkn33;
	public uint unkn34;
	public uint unkn35;
	public float unkn36;
	public uint unkn37;
	public uint unkn38;
	public uint unkn39;
	public float unkn40;
	public float unkn41;
	public float unkn42;
	public float unkn43;
	public float unkn44;
	public UndeterminedFieldType unkn45;
	public UndeterminedFieldType unkn46;
	public float unkn47;
	public float unkn48;
	public float unkn49;
	public float unkn50;
	public float unkn51;
	public float unkn52;
	public float unkn53;
	public UndeterminedFieldType unkn54;
	public float unkn55;
	public UndeterminedFieldType unkn56;
	[RszInlineWString] public string? meshPath;
	[RszInlineWString] public string? unkPath;
	[RszInlineWString] public string? mdfPath;
	public uint dataSize;
	[RszFixedSizeArray(nameof(dataSize), '/', 4)] public uint[]? data;
	public uint texBlockLength;
	[RszInlineWString, RszList(nameof(texCount))] public string[]? texPaths;
}

[RszGenerate, RszAutoReadWrite, RszVersionedObject(typeof(EfxVersion)), EfxStruct(EfxAttributeType.UnknownDD2_264MaterialClip, EfxVersion.DD2)]
public partial class EFXAttributeUnknownDD2_264MaterialClip : EFXAttribute
{
    public EFXAttributeUnknownDD2_264MaterialClip() : base(EfxAttributeType.UnknownDD2_264MaterialClip) { }

    public uint unkn0;
    public uint unkn1;
    public int unkn2;
    public float unkn3;
    public uint unkn4;
    public uint unkn5;
    public float unkn6;
    public uint substruct1Count;
    public uint substruct2Count;
    public uint substruct3Count;
    public uint substruct4Count;
    public uint substruct5Count;
    [RszFixedSizeArray(nameof(substruct1Count), '/', 4)] public int[]? substruct1;
    [RszFixedSizeArray(nameof(substruct2Count), '/', 4)] public int[]? substruct2;
    [RszFixedSizeArray(nameof(substruct3Count), '/', 4)] public float[]? substruct3;
    [RszFixedSizeArray(nameof(substruct4Count), '*', 4)] public uint[]? substruct4;
    [RszFixedSizeArray(nameof(substruct5Count))] public uint[]? substruct5;
}

[RszGenerate, RszAutoReadWrite, RszVersionedObject(typeof(EfxVersion)), EfxStruct(EfxAttributeType.UnknownDD2_265MaterialExpression, EfxVersion.DD2)]
public partial class EFXAttributeUnknownDD2_265MaterialExpression : EFXAttribute, IExpressionAttribute, IMaterialExpressionAttribute
{
    public EFXExpressionList? Expression => expressions;
    public EFXMaterialExpressionList? MaterialExpressions => materialExpressions;

	public EFXAttributeUnknownDD2_265MaterialExpression() : base(EfxAttributeType.UnknownDD2_265MaterialExpression) { }

    public uint unkn0;
    public uint unkn1;
    public uint unkn2;
    public uint unkn3;
    public uint unkn4;
    public uint unkn5;
    public uint unkn6;
    public uint unkn7;
    public uint unkn8;
    public uint unkn9;
    public uint unkn10;
    public uint unkn12;
    public uint unkn13;
    public uint unkn14;
    public uint unkn15;
    public uint unkn16;
    public uint unkn17;
    public uint unkn18;
    public uint unkn19;
    public uint unkn20;
    public uint unkn21;
	[RszClassInstance, RszConstructorParams(nameof(Version))] public EFXExpressionList? expressions;
	public uint unkn22;
	[RszClassInstance, RszConstructorParams(nameof(Version))] public EFXMaterialExpressionList? materialExpressions;
}

[RszGenerate, RszAutoReadWrite, RszVersionedObject(typeof(EfxVersion)), EfxStruct(EfxAttributeType.UnknownDD2_266, EfxVersion.DD2)]
public partial class EFXAttributeUnknownDD2_266 : EFXAttribute
{
    public EFXAttributeUnknownDD2_266() : base(EfxAttributeType.UnknownDD2_266) { }

	public uint unknFlags;
	public uint unkn1;
	public uint unkn2;
	public float unkn3;
	public UndeterminedFieldType unkn4;
	public UndeterminedFieldType unkn5;
	public float unkn6;
	public uint unkn7;

    public uint unkn8; // maybe 2 bytes of length? value count = 2*A + B or so, covers most of the next values

    public uint unkn9;
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
    public float unkn21;
    public float unkn22;
    public float unkn23;
    public UndeterminedFieldType unkn24;
    public float unkn25;
    public UndeterminedFieldType unkn26;
    public UndeterminedFieldType unkn27;
    public float unkn28;
    public UndeterminedFieldType unkn29;
    public UndeterminedFieldType unkn30;
    public float unkn31;
    public UndeterminedFieldType unkn32;
    public float unkn33;
    public float unkn34;
    public float unkn35;
    public UndeterminedFieldType unkn36;
    public float unkn37;
    public float unkn38;
    public float unkn39;
    public uint unkn40;
    public uint unkn41;
    public float unkn42;
    public float unkn43;
    public uint unkn44;
    public float unkn45;
    public float unkn46;
    public float unkn47;
    public float unkn48;
    public float unkn49;
    public float unkn50;
    public uint unkn51;
    public uint unkn52;
    public UndeterminedFieldType unkn53;
    public UndeterminedFieldType unkn54;
    public UndeterminedFieldType unkn55;
    public UndeterminedFieldType unkn56;
    public uint unkn57;
    public float unkn58;
    public float unkn59;
    public float unkn60;
    public float unkn61;
    public float unkn62;
    public float unkn63;
    public UndeterminedFieldType unkn64;
    public UndeterminedFieldType unkn65;
    public UndeterminedFieldType unkn66;
    public UndeterminedFieldType unkn67;
    public UndeterminedFieldType unkn68;
    public UndeterminedFieldType unkn69;
    public uint unkn70;
    public float unkn71;
    public float unkn72;
    public float unkn73;
    public float unkn74;
    public float unkn75;
    public float unkn76;
    public UndeterminedFieldType unkn77;
    public UndeterminedFieldType unkn78;
    public UndeterminedFieldType unkn79;
    public UndeterminedFieldType unkn80;
    public UndeterminedFieldType unkn81;
    public UndeterminedFieldType unkn82;
    public uint unkn83;

	public uint str1Len;
	public uint str2Len;
	public uint str3Len;
	[RszInlineWString(-1)] public string? str1;
	[RszInlineWString(-1)] public string? str2;
	[RszInlineWString(-1)] public string? str3;
}
