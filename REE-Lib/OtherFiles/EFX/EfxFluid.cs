using ReeLib.Efx.Structs.Common;
using ReeLib.InternalAttributes;

namespace ReeLib.Efx.Structs.Fluid;

public struct FloatWithColor
{
    public float value;
    public via.Color color;
}

[RszGenerate, RszAutoReadWrite, RszVersionedObject(typeof(EfxVersion)), EfxStruct(EfxAttributeType.FluidEmitter2D, EfxVersion.RE7, EfxVersion.RE2, EfxVersion.DMC5, EfxVersion.RE8)]
public partial class EFXAttributeFluidEmitter2D : ReeLib.Efx.EFXAttribute
{
    public EFXAttributeFluidEmitter2D() : base(EfxAttributeType.FluidEmitter2D) { }

    public uint unkn0;
    [RszVersion(EfxVersion.DD2)]
    public uint dd2_unkn0;
    public float unkn1;
    public float unkn2;
    public float unkn3;
    public float unkn4;
    public float unkn5;
    public float unkn6;
    public float unkn7;
    [RszVersion(EfxVersion.RE8, EndAt = nameof(unkn9))]
    public float unkn8;
    public float unkn9;
    [RszVersion(EfxVersion.DD2, EndAt = nameof(unkn11))]
	public float unkn10;
	public float unkn11;
}

[RszGenerate, RszAutoReadWrite, RszVersionedObject(typeof(EfxVersion)), EfxStruct(EfxAttributeType.FluidEmitter2DExpression, EfxVersion.DD2)]
public partial class EFXAttributeFluidEmitter2DExpression : EFXAttribute, IExpressionAttribute
{
    public EFXExpressionList? Expression { get => expressions; set => expressions = value; }
	public BitSet ExpressionBits => expressionBits;

	public EFXAttributeFluidEmitter2DExpression() : base(EfxAttributeType.FluidEmitter2DExpression) { }

	[RszClassInstance] public readonly BitSet expressionBits = new BitSet(2);
	public ExpressionAssignType unkn2;
	public ExpressionAssignType unkn3;
	[RszClassInstance, RszConstructorParams(nameof(Version))] public EFXExpressionList? expressions;
}

[RszGenerate, RszAutoReadWrite, RszVersionedObject(typeof(EfxVersion)), EfxStruct(EfxAttributeType.FluidSimulator2D, EfxVersion.RE2, EfxVersion.DMC5, EfxVersion.RE8, EfxVersion.RERT)]
public partial class EFXAttributeFluidSimulator2D : ReeLib.Efx.EFXAttribute
{
    public EFXAttributeFluidSimulator2D() : base(EfxAttributeType.FluidSimulator2D) { }

    public byte unkn0_1;
    public byte unkn0_2;
    public byte unkn0_3;
    public byte unkn0_4;
    public uint unkn1;
    public uint unkn2;
    public uint unkn3;
    public uint unkn4;

    public float unkn5;
    public float unkn6;
    public float unkn7;
    public float unkn8;

    public uint unkn9;
    public uint unkn10;
    public uint unkn11;
    public uint unkn12;

    public float unkn13;
    public float unkn14;
    public float unkn15;
    public float unkn16;
    public float unkn17;
    public float unkn18;

    public uint unkn19;
    public uint unkn20;
    public uint unkn21;
    public uint unkn22;
    [RszVersion(EfxVersion.RE4, EndAt = nameof(re4_unkn2))]
    public float re4_unkn1;
    public float re4_unkn2;

    public float unkn1_23;
    public float unkn1_24;
    public float unkn1_25;
    public float unkn1_26;
    public float unkn1_27;
    // NOTE: the fields here seem to have gotten order swapped around past this point
    // TODO figure out correct re-ordering, match up names and read/write procedures
    [RszVersion(EfxVersion.RE8, EndAt = nameof(unkn1_29_re8))]
    public float unkn1_28_re8;
    public float unkn1_29_re8;
    [RszVersion('<', EfxVersion.RE8, EndAt = nameof(unkn1_29_re3))] // else
    public int unkn1_28_re3;
    public int unkn1_29_re3;
    public float unkn1_30;
    public float unkn1_31;
    public float unkn1_32;
    public float unkn1_33;

    [RszVersion("<", EfxVersion.DD2)]
    public float unkn1_34_v1;
    [RszVersion(EfxVersion.DD2)] // else
    public uint unkn1_34_v2;
    public float unkn1_35;

    [RszVersion(EfxVersion.RE4, EndAt = nameof(re4_unk2_10))]
    public float re4_unk2_0;
    public float re4_unk2_1;
    public float re4_unk2_2;
    public float re4_unk2_3;
    public float re4_unk2_4;
    public float re4_unk2_5;
    public float re4_unk2_6;
    public float re4_unk2_7;
    public float re4_unk2_8;
    public float re4_unk2_9;
    public float re4_unk2_10;

    [RszVersion("<", EfxVersion.DD2)]
    public uint unkn1_36_v1;
    [RszVersion(EfxVersion.DD2)] // else
    public float unkn1_36_v2;

    public float unkn1_37;
    public float unkn1_38;
    [RszVersion(EfxVersion.RE8)]
    public float unkn1_39_re8;
    [RszVersion("<", EfxVersion.RE8)] // else
    public int unkn1_39_re3;
    public float unkn1_40;

    [RszVersion("<", EfxVersion.RE4)]
    public float unkn1_41;
    [RszVersion(EfxVersion.RE8)]
    public int unkn1_42_re8;
    [RszVersion("<", EfxVersion.RE8)] // else
    public float unkn1_42_re3;

    [RszVersion(EfxVersion.DD2, EndAt = nameof(dd2_unk2))]
    public float dd2_unk1;
    public uint dd2_unk2;

    [RszVersion("<", EfxVersion.RERT)]
    public UndeterminedFieldType unkn2_0;
    [RszVersion("!=", EfxVersion.RE8, EndAt = nameof(unkn2_2))]
    public uint unkn2_1;
    public uint unkn2_2;
    [RszVersion(EfxVersion.RE4, EndAt = nameof(re4_unk3_3))]
    public float re4_unk3_1;
    public float re4_unk3_2;
    public uint re4_unk3_3;

    public uint extraByteCount;
    public uint path1Size;
    public uint path2Size;
    public uint path3Size;
    public uint path4Size;
    [RszVersion(nameof(Version), "<=", EfxVersion.RE3, "||", nameof(Version), ">=", EfxVersion.RE4)]
    public uint path5Size;
    [RszInlineWString(nameof(path1Size))] public string? uvsPath1; // turbulence
    [RszInlineWString(nameof(path2Size))] public string? uvsPath2; // source density map
    [RszInlineWString(nameof(path3Size))] public string? uvsPath3;
    [RszInlineWString(nameof(path4Size))] public string? uvsPath4; // density colormap
    [RszVersion(nameof(Version), "<=", EfxVersion.RE3, "||", nameof(Version), ">=", EfxVersion.RE4), RszInlineWString(nameof(path5Size))]
    public string? uvsPath5; // projection diffuse map
    [RszFixedSizeArray(nameof(extraByteCount))] public byte[]? extraBytes;

    // may be an array - count in re4_unk3_3?
    [RszVersion(EfxVersion.DD2), RszFixedSizeArray(nameof(re4_unk3_3))]
    public FloatWithColor[]? gradient;
}
