using System.Net;
using RszTool.Efx.Structs.Common;
using RszTool.InternalAttributes;

namespace RszTool.Efx.Structs.DD2;

[RszGenerate, RszAutoReadWrite, RszVersionedObject(typeof(EfxVersion)), EfxStruct(EfxAttributeType.UnknownDD2_117TypeStrainRibbonExpression, EfxVersion.DD2)]
public partial class EFXAttributeUnknownDD2_117Expression : EFXAttribute, IExpressionAttribute
{
    public EFXExpressionList? Expression => expressions;
	public BitSet ExpressionBits => expressionBits;

	public EFXAttributeUnknownDD2_117Expression() : base(EfxAttributeType.UnknownDD2_117TypeStrainRibbonExpression) { }

	[RszClassInstance] public readonly BitSet expressionBits = new BitSet(24) { BitNameDict = new () {
		[13] = nameof(posX),
		[14] = nameof(posY),
		[15] = nameof(posZ),
		[16] = nameof(terminalPosX),
		[17] = nameof(terminalPosY),
		[18] = nameof(terminalPosZ),
	} };
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
    public ExpressionAssignType unkn12;
    public ExpressionAssignType posX;
    public ExpressionAssignType posY;
    public ExpressionAssignType posZ;
    public ExpressionAssignType terminalPosX;
    public ExpressionAssignType terminalPosY;
    public ExpressionAssignType terminalPosZ;
    public ExpressionAssignType unkn19;
    public ExpressionAssignType unkn20;
    public ExpressionAssignType unkn21;
    public ExpressionAssignType unkn22;
    public ExpressionAssignType unkn23;
    public ExpressionAssignType unkn24;
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
	public BitSet ExpressionBits => expressionBits;

	public EFXAttributeUnknownDD2_221Expression() : base(EfxAttributeType.UnknownDD2_221Expression) { }

	[RszClassInstance] public readonly BitSet expressionBits = new BitSet(7);
	public ExpressionAssignType unkn1;
	public ExpressionAssignType unkn2;
	public ExpressionAssignType unkn3;
	public ExpressionAssignType unkn4;
	public ExpressionAssignType unkn5;
	public ExpressionAssignType unkn6;
	public ExpressionAssignType unkn7;
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

[RszGenerate, RszAutoReadWrite, RszVersionedObject(typeof(EfxVersion)), EfxStruct(EfxAttributeType.UnknownDD2_231Clip, EfxVersion.DD2)]
public partial class EFXAttributeUnknownDD2_231Clip : EFXAttribute, IClipAttribute
{
    public EfxClipData Clip => clipData;
    public BitSet ClipBits => clipBits;

    public EFXAttributeUnknownDD2_231Clip() : base(EfxAttributeType.UnknownDD2_231Clip) { }

	[RszClassInstance] public readonly BitSet clipBits = new BitSet(7);
    public UndeterminedFieldType unkn1;
	[RszClassInstance] public EfxClipData clipData = new();
}

[RszGenerate, RszAutoReadWrite, RszVersionedObject(typeof(EfxVersion)), EfxStruct(EfxAttributeType.UnknownDD2_232Expression, EfxVersion.DD2)]
public partial class EFXAttributeUnknownDD2_232Expression : EFXAttribute, IExpressionAttribute
{
	public EFXExpressionList? Expression => expressions;
	public BitSet ExpressionBits => expressionBits;

	public EFXAttributeUnknownDD2_232Expression() : base(EfxAttributeType.UnknownDD2_232Expression) { }

	[RszClassInstance] public readonly BitSet expressionBits = new BitSet(18);
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

	[RszClassInstance, RszConstructorParams(nameof(Version))] public EFXExpressionList? expressions;
}

[RszGenerate, RszAutoReadWrite, RszVersionedObject(typeof(EfxVersion)), EfxStruct(EfxAttributeType.UnknownDD2_239RgbColor, EfxVersion.DD2)]
public partial class EFXAttributeUnknownDD2_239RgbColor : EFXAttribute
{
	public EFXAttributeUnknownDD2_239RgbColor() : base(EfxAttributeType.UnknownDD2_239RgbColor) { }

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

[RszGenerate, RszAutoReadWrite, RszVersionedObject(typeof(EfxVersion)), EfxStruct(EfxAttributeType.UnknownDD2_239RgbColorExpression, EfxVersion.DD2)]
public partial class EFXAttributeUnknownDD2_239RgbColorExpression : EFXAttribute, IExpressionAttribute
{
    public EFXExpressionList? Expression => expressions;
	public BitSet ExpressionBits => expressionBits;

	public EFXAttributeUnknownDD2_239RgbColorExpression() : base(EfxAttributeType.UnknownDD2_239RgbColorExpression) { }

	[RszClassInstance] public readonly BitSet expressionBits = new BitSet(13) { BitNameDict = new() {
		[1] = "GreenChColor",
		[3] = "GreenChIntensity",
		[4] = "GreenChSaturate",
		[6] = "RedChColor",
		[8] = "RedChIntensity",
		[9] = "Alpha1",
		[10] = "Alpha2",
	} };
	public ExpressionAssignType particleColor;
	public ExpressionAssignType unkn2;
	public ExpressionAssignType colorIntensityGreen;
	public ExpressionAssignType colorSaturate;
	public ExpressionAssignType unkn5;
	public ExpressionAssignType particleColor2;
	public ExpressionAssignType unkn7;
	public ExpressionAssignType colorIntensityRed;
	public ExpressionAssignType alpha1;
	public ExpressionAssignType alpha2;
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

	[RszClassInstance, RszConstructorParams(nameof(Version))] public EFXExpressionList? expressions;
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
