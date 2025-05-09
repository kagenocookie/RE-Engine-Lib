using System.Globalization;
using System.Numerics;
using System.Runtime.CompilerServices;
using RszTool;
using RszTool.Common;
using RszTool.Efx.Structs.RE4;

namespace RszTool.Efx.Structs.RERT;

public struct UndeterminedFieldType
{
	public int value;

	public UndeterminedFieldType() { }
	public UndeterminedFieldType(int value)
	{
		this.value = value;
	}
	public UndeterminedFieldType(uint value)
	{
		this.value = (int)value;
	}
	public UndeterminedFieldType(float value)
	{
		this.value = MemoryUtils.SingleToInt32(value);
	}

	public override string ToString() => $"{value} {value.ToString("X")} {MemoryUtils.Int32ToSingle(value).ToString("0.0#", CultureInfo.InvariantCulture)}";
}

[RszGenerate, RszAutoReadWrite, EfxStruct(EfxAttributeType.TypeGpuRibbonLengthExpression, EfxVersion.DD2)]
public partial class EFXAttributeTypeGpuRibbonLengthExpression : EFXAttribute
{
	public EFXAttributeTypeGpuRibbonLengthExpression() : base(EfxAttributeType.TypeGpuRibbonLengthExpression) { }

	[RszFixedSizeArray(23)] public uint[]? ukn0;
	[RszSwitch(
		nameof(Version), ">=", EfxVersion.DD2, typeof(EFXExpressionListWrapper3),
		typeof(EFXExpressionListWrapper)
	)]
	public EFXExpressionListWrapperBase expressions = null!;
}

[RszGenerate, RszAutoReadWrite, EfxStruct(EfxAttributeType.TypeBillboard3DMaterial, EfxVersion.DD2)]
public partial class EFXAttributeTypeBillboard3DMaterial : EFXAttribute
{
	public EFXAttributeTypeBillboard3DMaterial() : base(EfxAttributeType.TypeBillboard3DMaterial) { }

	public UndeterminedFieldType ukn0;
	public via.Color ukn1;
	public via.Color ukn2;
	public UndeterminedFieldType ukn3;
	public float ukn4;
	public float ukn5;
	public UndeterminedFieldType ukn6;
	public float ukn7;
	public UndeterminedFieldType ukn8;
	public float ukn9;
	public UndeterminedFieldType ukn10;
	public UndeterminedFieldType ukn11;
	public UndeterminedFieldType ukn12;
	public uint ukn13;
	public uint ukn14;
	public uint texCount;
	public UndeterminedFieldType ukn16;
	public uint ukn17;
	public uint ukn18;
	[RszInlineWString] public string? mdfPath;
	[RszInlineWString] public string? mmtrPath;
	public uint ukn19;
	[RszFixedSizeArray(nameof(ukn19), "/4")] public UndeterminedFieldType[]? data;
	// public uint ukn20;
	public uint texBlockLength;
	[RszInlineWString, RszList(nameof(texCount))] public string[]? texPaths;
}

[RszGenerate, RszAutoReadWrite, EfxStruct(EfxAttributeType.TypeBillboard3DMaterialExpression, EfxVersion.DD2)]
public partial class EFXAttributeTypeBillboard3DMaterialExpression : EFXAttribute
{
	public EFXAttributeTypeBillboard3DMaterialExpression() : base(EfxAttributeType.TypeBillboard3DMaterialExpression) { }

	public UndeterminedFieldType ukn0;
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
	public uint ukn15;
	// TODO new expression structure
	public uint solverSize;
	[RszFixedSizeArray(nameof(solverSize), "/4")] public UndeterminedFieldType[]? expression;
	public uint ukn16;
	public uint ukn17;
	public uint ukn18;
}

[RszGenerate, RszAutoReadWrite, EfxStruct(EfxAttributeType.unknDD2Struct252, EfxVersion.DD2)]
public partial class EFXAttributeunknDD2Struct252 : EFXAttribute
{
	public EFXAttributeunknDD2Struct252() : base(EfxAttributeType.unknDD2Struct252) { }

	public uint ukn0;
	public uint ukn1Mask;
	public uint ukn2Mask;
	public float ukn3;
	public float ukn4;
	public float ukn5;
	public float ukn6;
	public uint ukn7;
	public uint ukn8;
	public uint ukn9;
	public float ukn10;
	public float ukn11;
	public float ukn12;
	public float ukn13;
	public float ukn14;
	public float ukn15;
	public float ukn16;
	public float ukn17;
	public float ukn18;
}

[RszGenerate, RszAutoReadWrite, EfxStruct(EfxAttributeType.ProceduralDistortion, EfxVersion.DD2)]
public partial class EFXAttributeProceduralDistortion : EFXAttribute
{
	public EFXAttributeProceduralDistortion() : base(EfxAttributeType.ProceduralDistortion) { }

	public float unkn1;
	public float unkn2_0;
	public float unkn2_1;
	public float unkn2_2;
	public float unkn2_3;
	public float unkn2_4;
	public float unkn2_5;
	public float unkn2_6;
	public float unkn2_7;
	public float unkn2_8;
	public float unkn2_9;
	public float unkn2_10;
	public float unkn2_11;
	public float unkn2_12;
	public float unkn2_13;
	public float unkn2_14;
	public float unkn2_15;
	public float unkn2_16;
	public uint unkn2_17; // TODO RE4 has this as float; might be different thing?
	[RszConditional(nameof(Version), ">=", EfxVersion.DD2)]
	public uint unkn2_18;
}

[RszGenerate, RszAutoReadWrite, EfxStruct(EfxAttributeType.unknDD2Struct249, EfxVersion.DD2)]
public partial class EFXAttributeunknDD2Struct249 : EFXAttribute
{
	public EFXAttributeunknDD2Struct249() : base(EfxAttributeType.unknDD2Struct249) { }

	public float unkn1;
}
[RszGenerate, RszAutoReadWrite, EfxStruct(EfxAttributeType.unknDD2Struct250, EfxVersion.DD2)]
public partial class EFXAttributeunknDD2Struct250 : EFXAttribute
{
	public EFXAttributeunknDD2Struct250() : base(EfxAttributeType.unknDD2Struct250) { }
}

[RszGenerate, RszAutoReadWrite, EfxStruct(EfxAttributeType.unknDD2Struct214, EfxVersion.DD2)]
public partial class EFXAttributeunknDD2Struct214 : EFXAttribute
{
	public EFXAttributeunknDD2Struct214() : base(EfxAttributeType.unknDD2Struct214) { }

	public uint unkn1;
	public uint unkn2;
	public uint unkn3;
}

[RszGenerate, RszAutoReadWrite, EfxStruct(EfxAttributeType.unknDD2Struct223, EfxVersion.DD2)]
public partial class EFXAttributeunknDD2Struct223 : EFXAttribute
{
	public EFXAttributeunknDD2Struct223() : base(EfxAttributeType.unknDD2Struct223) { }

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

[RszGenerate, RszAutoReadWrite, EfxStruct(EfxAttributeType.unknDD2Struct146, EfxVersion.DD2)]
public partial class EFXAttributeunknDD2Struct146 : EFXAttribute
{
	public EFXAttributeunknDD2Struct146() : base(EfxAttributeType.unknDD2Struct146) { }
	// note: compatible with FadeByEmitterAngle, LuminanceBleed, EmitterShape3D, ContrastHighlighter

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
[RszGenerate, RszAutoReadWrite, EfxStruct(EfxAttributeType.unknDD2Struct198, EfxVersion.DD2)]
public partial class EFXAttributeunknDD2Struct198 : EFXAttribute, IBoneRelationAttribute
{
	public EFXAttributeunknDD2Struct198() : base(EfxAttributeType.unknDD2Struct198) { }

	public UndeterminedFieldType unkn1_0;
	public UndeterminedFieldType unkn1_1;
	public UndeterminedFieldType unkn1_2;
	public float unkn1_3;
	public float unkn1_4;
	public float unkn1_5;
	public float unkn1_6;
	public float unkn1_7;
	public UndeterminedFieldType unkn1_8;
	public UndeterminedFieldType unkn1_9;
	public float unkn1_10;
	// public uint unkn1_11; // not there: E:\mods\dd2\REtool\re_chunk_000\natives\stm\vfx\event\effects\cutscene\cs3122\s00\13_cs3122_s00_sm81_104_00_00_main01_brokenbowl_00.efx.4064419
	[RszInlineWString] public string? boneName;

	public string? ParentBone { get; set; }
}
[RszGenerate, RszAutoReadWrite, EfxStruct(EfxAttributeType.unknDD2Struct197, EfxVersion.DD2)]
public partial class EFXAttributeunknDD2Struct197 : EFXAttribute
{
	public EFXAttributeunknDD2Struct197() : base(EfxAttributeType.unknDD2Struct197) { }

	public float unkn1_1;
	public float unkn1_2;
	public float unkn1_3;
	public UndeterminedFieldType unkn1_4;
	public uint unkn1_5;
	public uint unkn1_6;
}
[RszGenerate, RszAutoReadWrite, EfxStruct(EfxAttributeType.unknDD2Struct224, EfxVersion.DD2)]
public partial class EFXAttributeunknDD2Struct224 : EFXAttribute
{
	public EFXAttributeunknDD2Struct224() : base(EfxAttributeType.unknDD2Struct224) { }
	// note: compatible with FadeByEmitterAngle, LuminanceBleed, EmitterShape3D, ContrastHighlighter

	public uint null1;
	public float unkn2;
	public float unkn3;
	public float unkn4;
	public float null5;
	public float unkn6;
}
[RszGenerate, RszAutoReadWrite, EfxStruct(EfxAttributeType.unknDD2Struct152, EfxVersion.DD2)]
public partial class EFXAttributeunknDD2Struct152 : EFXAttribute
{
	public EFXAttributeunknDD2Struct152() : base(EfxAttributeType.unknDD2Struct152) { }

	public uint unkn0;
	public uint unkn1;
	public float unkn2;
	public float unkn3;
	public float unkn4;
	public float unkn5;
	public UndeterminedFieldType unkn6;
	public float unkn7;
	public float unkn8;
	public float unkn9;
	public float unkn10;
	public float unkn11;
	public float unkn12;
}

public struct EFX_DD2_263_substruct
{
	public uint unkn0;
	public short unkn1_0;
	public short unkn1_1;
	public UndeterminedFieldType unkn2;
	public float unkn3;
	public float unkn4;
	public float unkn5;
	public float unkn6;
}

[RszGenerate, RszAutoReadWrite, EfxStruct(EfxAttributeType.unknDD2Struct263, EfxVersion.DD2)]
public partial class EFXAttributeunknDD2Struct263 : EFXAttribute
{
	public EFXAttributeunknDD2Struct263() : base(EfxAttributeType.unknDD2Struct263) { }

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
	public UndeterminedFieldType unkn13;
	public UndeterminedFieldType unkn14;
	public uint unkn15;
	[RszFixedSizeArray(6)] public UndeterminedFieldType[]? unkn16;
	public float unkn17;
	public UndeterminedFieldType unkn18;
	public float unkn19;
	public UndeterminedFieldType unkn20;
	public float unkn21;
	public UndeterminedFieldType unkn22;
	public float unkn23;
	public UndeterminedFieldType unkn24;
	public uint unkn25;
	public float unkn26;
	public uint texCount;
	public UndeterminedFieldType unkn28;
	public uint unkn29;
	public UndeterminedFieldType unkn30;
	public uint unkn31;
	public float unkn32;
	[RszFixedSizeArray(4)] public UndeterminedFieldType[]? unkn33;
	public float unkn34;
	public float unkn35;
	[RszFixedSizeArray(5)] public UndeterminedFieldType[]? unkn36;
	[RszFixedSizeArray(5)] public float[]? unkn37;
	[RszFixedSizeArray(4)] public UndeterminedFieldType[]? unkn38;
	[RszInlineWString] public string? meshPath;
	[RszInlineWString] public string? unkPath;
	[RszInlineWString] public string? mdfPath;
	public uint dataSize;
	[RszFixedSizeArray(nameof(dataSize), '/', 4)] public uint[]? data;
	public uint texBlockLength;
	[RszInlineWString, RszList(nameof(texCount))] public string[]? texPaths;
}

[RszGenerate, RszAutoReadWrite, EfxStruct(EfxAttributeType.unknDD2Struct265Expression, EfxVersion.DD2)]
public partial class EFXAttributeunknDD2Struct265Expression : EFXAttribute
{
	public EFXAttributeunknDD2Struct265Expression() : base(EfxAttributeType.unknDD2Struct265Expression) { }

	[RszFixedSizeArray(21)] public uint[]? unkn0;
	[RszSwitch(
		nameof(Version), ">=", EfxVersion.DD2, typeof(EFXExpressionListWrapper3),
		typeof(EFXExpressionListWrapper)
	)]
	public EFXExpressionListWrapperBase expressions = null!;
	public uint unkn1; //[1]
	[RszClassInstance] public EFXExpressionListWrapper2 expressions2 = null!;
}

[RszGenerate, RszAutoReadWrite, EfxStruct(EfxAttributeType.unknDD2Struct219, EfxVersion.DD2)]
public partial class EFXAttributeunknDD2Struct219 : EFXAttribute
{
	public EFXAttributeunknDD2Struct219() : base(EfxAttributeType.unknDD2Struct219) { }

	public uint unkn0;
	public uint unkn1;
	[RszFixedSizeArray(18)] public float[]? unkn2;
}

[RszGenerate, RszAutoReadWrite, EfxStruct(EfxAttributeType.unknDD2Struct225Expression, EfxVersion.DD2)]
public partial class EFXAttributeunknDD2Struct225 : EFXAttribute
{
	public EFXAttributeunknDD2Struct225() : base(EfxAttributeType.unknDD2Struct224) { }

	public uint unkn1;
	public uint unkn2;
	public uint unkn3;
	public uint unkn4;
	public uint unkn5;
	public uint unkn6;
	public uint unkn7;
	[RszSwitch(
		nameof(Version), ">=", EfxVersion.DD2, typeof(EFXExpressionListWrapper3),
		typeof(EFXExpressionListWrapper)
	)]
	public EFXExpressionListWrapperBase expressions = null!;
}

[RszGenerate, RszAutoReadWrite, EfxStruct(EfxAttributeType.unknDD2Struct241, EfxVersion.DD2)]
public partial class EFXAttributeunknDD2Struct241 : EFXAttribute
{
	public EFXAttributeunknDD2Struct241() : base(EfxAttributeType.unknDD2Struct241) { }

	public via.Color unkn0;
	public float unkn1;
	public via.Color unkn2;
	public float unkn3;
	public float unkn4;
	public float unkn5;
	public UndeterminedFieldType unkn6;
	[RszFixedSizeArray(20)] public UndeterminedFieldType[]? unkn7;
	byte a;
	byte b;
	byte c;
}

[RszGenerate, RszAutoReadWrite, EfxStruct(EfxAttributeType.unknDD2Struct247, EfxVersion.DD2)]
public partial class EFXAttributeunknDD2Struct247 : EFXAttribute
{
	public EFXAttributeunknDD2Struct247() : base(EfxAttributeType.unknDD2Struct247) { }

	public uint unkn0;
	public bool unkn1;
	public float unkn2;
	public uint null3;
	public uint null4;
	public uint null5;
	public uint null6;
	public uint null7;
	public short null8;
	public float unkn9;
	public float null10;
	public float null11;
	public float null12;
	public float unkn13;
}

[RszGenerate, RszAutoReadWrite, EfxStruct(EfxAttributeType.unknDD2Struct256, EfxVersion.DD2)]
public partial class EFXAttributeunknDD2Struct256 : EFXAttribute
{
	public EFXAttributeunknDD2Struct256() : base(EfxAttributeType.unknDD2Struct256) { }

	public uint unkn0;
	public via.Color unkn1;
	public via.Color unkn2;
	public float unkn3;
	public uint null4;
	public uint null5;
	public uint null6;
	public uint null7;
	public uint unkn9;
	public uint unkn10;
	public uint unkn11;
	public float unkn12;
	public float unkn13;
	public float unkn14;
	public float unkn15;
}

[RszGenerate, RszAutoReadWrite, EfxStruct(EfxAttributeType.unknDD2Struct253Expression, EfxVersion.DD2)]
public partial class EFXAttributeunknDD2Struct253 : EFXAttribute
{
	public EFXAttributeunknDD2Struct253() : base(EfxAttributeType.unknDD2Struct253Expression) { }

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
	[RszSwitch(
		nameof(Version), ">=", EfxVersion.DD2, typeof(EFXExpressionListWrapper3),
		typeof(EFXExpressionListWrapper)
	)]
	public EFXExpressionListWrapperBase expressions = null!;
}

[RszGenerate, RszAutoReadWrite, EfxStruct(EfxAttributeType.TypeRibbonLength, EfxVersion.DD2)]
public partial class EFXAttributeTypeRibbonLength_DD2 : EFXAttribute
{
	public EFXAttributeTypeRibbonLength_DD2() : base(EfxAttributeType.TypeRibbonLength) { }

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
	[RszConditional(nameof(Version), ">=", EfxVersion.RERT)]
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

	public int unkn1_26;
	public int unkn1_27;
	public int unkn1_28;
	public uint unkn1_29;

	public uint unkn1_30;
	public float unkn1_31;
	public float unkn1_32;
	public float unkn1_33;
	public float unkn1_34;
	public float unkn1_35;
	[RszConditional(nameof(Version), ">=", EfxVersion.RERT, EndAt = nameof(sb_unkn2))]
	public float sb_unkn1;
	public float sb_unkn2;
	[RszConditional(nameof(Version), ">=", EfxVersion.RE4)]
	public float re4_unkn0;
	[RszConditional(nameof(Version), ">=", EfxVersion.DD2)]
	public uint dd2_null;
}

[RszGenerate, RszAutoReadWrite, EfxStruct(EfxAttributeType.unknDD2Struct260, EfxVersion.DD2)]
public partial class EFXAttributeunknDD2Struct260 : EFXAttribute
{
	public EFXAttributeunknDD2Struct260() : base(EfxAttributeType.unknDD2Struct260) { }

	public uint unkn0_flag;
	public uint unkn1;
	public uint unkn2;
	public uint unkn3;
	[RszFixedSizeArray(28)] public uint[]? unkn4;
	public uint texCount;
	public uint unkn5;
	[RszInlineWString] public string? meshPath;
	[RszInlineWString] public string? uknPath;
	[RszInlineWString] public string? mdfPath;
	public uint uknSize;
	[RszFixedSizeArray(nameof(uknSize), "/4")] public uint[]? unkn6;
	public uint texBlockSize;
	[RszInlineWString, RszList(nameof(texCount))] public string[]? texPaths;
}

[RszGenerate, RszAutoReadWrite, EfxStruct(EfxAttributeType.unknDD2Struct262Expression, EfxVersion.DD2)]
public partial class EFXAttributeunknDD2Struct262 : EFXAttribute
{
	public EFXAttributeunknDD2Struct262() : base(EfxAttributeType.unknDD2Struct262Expression) { }

	// E:\mods\dd2\REtool\re_chunk_000\natives\stm\vfx\effects\character\ch52\000\13_ch52_000_death_003_01.efx.4064419
	// E:\mods\dd2\REtool\re_chunk_000\natives\stm\vfx\event\effects\cutscene\cs3100\common\13_cs3100_ch257002_00_02_fusible_00.efx.4064419

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

	[RszSwitch(
		nameof(Version), ">=", EfxVersion.DD2, typeof(EFXExpressionListWrapper3),
		typeof(EFXExpressionListWrapper)
	)]
	public EFXExpressionListWrapperBase expressions = null!;
	// [RszConditional(nameof(Version), ">=", EfxVersion.RE4)]
	// public uint unkn5_24;
	// [RszFixedSizeArray(nameof(substructCount))] public uint[]? extra;
	// [RszFixedSizeArray(nameof(substruct2Count))] public uint[]? extra2;
	[RszFixedSizeArray(3)] public uint[]? extra3;

	// [RszClassInstance] public EFXExpressionListWrapper2? expression2;
}
