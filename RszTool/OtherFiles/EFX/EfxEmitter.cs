using RszTool.Efx.Structs.Common;
using RszTool.InternalAttributes;

namespace RszTool.Efx.Structs.Transforms;

[RszGenerate, RszAutoReadWrite, RszVersionedObject(typeof(EfxVersion)), EfxStruct(EfxAttributeType.EmitterColor, EfxVersion.RE4)]
public partial class EFXAttributeEmitterColor : EFXAttribute
{
	public EFXAttributeEmitterColor() : base(EfxAttributeType.EmitterColor) { }

	public uint unkn1;
	public uint unkn2;
	public via.Color color;

}

[RszGenerate, RszAutoReadWrite, RszVersionedObject(typeof(EfxVersion)), EfxStruct(EfxAttributeType.EmitterColorClip, EfxVersion.RE4)]
public partial class EFXAttributeEmitterColorClip : EFXAttribute, IClipAttribute
{
    public EfxClipData Clip => clipData;
    public BitSet ClipBits => clipBits;

	public EFXAttributeEmitterColorClip() : base(EfxAttributeType.EmitterColorClip) { }

    /// <summary>
    /// This flag tells us which color channels have a clip:
    /// 1 = R, 2 = G, 4 = B, 8 = A
    /// </summary>
	// public uint colorClipBits;
	[RszClassInstance] public readonly BitSet clipBits = new BitSet(4) { BitNames = ["R", "G", "B", "A"] };
	public uint unkn1;
	[RszClassInstance] public EfxClipData clipData = new();

    public override string ToString() => $"PtColorClip: {clipBits}";
}

[RszGenerate, RszAutoReadWrite, RszVersionedObject(typeof(EfxVersion)), EfxStruct(EfxAttributeType.EmitterPriority, EfxVersion.DMC5, EfxVersion.RE4)]
public partial class EFXAttributeEmitterPriority : EFXAttribute
{
	public EFXAttributeEmitterPriority() : base(EfxAttributeType.EmitterPriority) { }

}

[RszGenerate, RszAutoReadWrite, RszVersionedObject(typeof(EfxVersion)), EfxStruct(EfxAttributeType.EmitterShape2D, EfxVersion.DMC5, EfxVersion.RE4)]
public partial class EFXAttributeEmitterShape2D : EFXAttribute
{
	public EFXAttributeEmitterShape2D() : base(EfxAttributeType.EmitterShape2D) { }

	public float unkn1_0;
	public float unkn1_1;
	public float unkn1_2;
	public float unkn1_3;
	public uint unkn1_4;
	public uint unkn1_5;
	public uint unkn1_6;
	public float unkn1_7;

}

[RszGenerate, RszAutoReadWrite, RszVersionedObject(typeof(EfxVersion)), EfxStruct(EfxAttributeType.EmitterShape2DExpression, EfxVersion.RE4)]
public partial class EFXAttributeEmitterShape2DExpression : EFXAttribute, IExpressionAttribute
{
	public EFXExpressionList Expression { get => expressions; set => expressions = value; }
	public BitSet ExpressionBits => expressionBits;

	public EFXAttributeEmitterShape2DExpression() : base(EfxAttributeType.EmitterShape2DExpression) { }

	[RszClassInstance] public readonly BitSet expressionBits = new BitSet(5) { BitNameDict = new () {
		// [1] = nameof(unkn1),
		// [2] = nameof(unkn2),
		[3] = nameof(size),
		// [4] = nameof(unkn4),
		// [5] = nameof(unkn5),
	} };
	public ExpressionAssignType unkn1;
	public ExpressionAssignType unkn2;
	public ExpressionAssignType size;
	public ExpressionAssignType unkn4;
	public ExpressionAssignType unkn5;
	[RszClassInstance] public EFXExpressionList expressions = new();
}

[RszGenerate, RszAutoReadWrite, RszVersionedObject(typeof(EfxVersion)), EfxStruct(EfxAttributeType.EmitterShape3D, EfxVersion.RERT, EfxVersion.RE4, EfxVersion.DD2)]
public partial class EFXAttributeEmitterShape3D : EFXAttribute
{
	public EFXAttributeEmitterShape3D() : base(EfxAttributeType.EmitterShape3D) { }

	public float RangeXMin;
	public float RangeXMax;
	public float RangeYMin;
	public float RangeYMax;
	public float RangeZMin;
	public float RangeZMax;
	public uint ShapeType;
	public uint RangeDivideNum;
	public uint RangeDivideAxis;
	[RszVersion(">", EfxVersion.RE3, EndAt = nameof(unkn1_10))]
	public uint emitCount;
	public uint unkn1_10;
	public float LocalRotationX;
	public float LocalRotationY;
	public float LocalRotationZ;
	public uint RotationOrder;
	[RszVersion(EfxVersion.RE2, EndAt = nameof(unkn4_3))]
	[RszVersion(nameof(Version), "==", EfxVersion.MHRiseSB, "||", nameof(Version), "==", EfxVersion.RERT, "||", nameof(Version), "==", EfxVersion.RE8)]
	public byte unkn2;
	public uint unkn3;
	[RszVersion(EfxVersion.RE4)]
	public uint re4_unkn0;
	public float unkn4_0;
	public float unkn4_1;
	public float unkn4_2;
	public float unkn4_3;
}

[RszGenerate, RszAutoReadWrite, RszVersionedObject(typeof(EfxVersion)), EfxStruct(EfxAttributeType.EmitterShape3DExpression, EfxVersion.RE7, EfxVersion.DMC5, EfxVersion.RE3, EfxVersion.RE4)]
public partial class EFXAttributeEmitterShape3DExpression : EFXAttribute, IExpressionAttribute
{
	public EFXExpressionList? Expression { get => expressions; set => expressions = value; }
	public BitSet ExpressionBits => expressionBits;

	public EFXAttributeEmitterShape3DExpression() : base(EfxAttributeType.EmitterShape3DExpression) { }

	[RszClassInstance] public readonly BitSet expressionBits = new BitSet(9) { BitNameDict = new () {
		[1] = nameof(rangeXMin),
		[2] = nameof(rangeXMax),
		[3] = nameof(rangeYMin),
		[4] = nameof(rangeYMax),
		[5] = nameof(rangeZMin),
		[6] = nameof(rangeZMax),
		[7] = nameof(spawnNum),
	} };
	public ExpressionAssignType rangeXMin;
	public ExpressionAssignType rangeXMax;
	public ExpressionAssignType rangeYMin;
	public ExpressionAssignType rangeYMax;
	public ExpressionAssignType rangeZMin;
	public ExpressionAssignType rangeZMax;
	[RszVersion(EfxVersion.RE2)]
	public ExpressionAssignType spawnNum;
	[RszVersion('>', EfxVersion.RE3, EndAt = nameof(unkn9))]
	public ExpressionAssignType unkn8; // seems like an additional range parameter
	public ExpressionAssignType unkn9;
	[RszClassInstance, RszConstructorParams(nameof(Version))] public EFXExpressionList? expressions;
}

[RszGenerate, RszAutoReadWrite, RszVersionedObject(typeof(EfxVersion)), EfxStruct(EfxAttributeType.ScreenSpaceEmitter, EfxVersion.RE8, EfxVersion.RE4)]
public partial class EFXAttributeScreenSpaceEmitter : EFXAttribute
{
	public EFXAttributeScreenSpaceEmitter() : base(EfxAttributeType.ScreenSpaceEmitter) { }

    public uint unkn0;
    public uint unkn1;
}


[RszGenerate, RszAutoReadWrite, RszVersionedObject(typeof(EfxVersion)), EfxStruct(EfxAttributeType.MeshEmitter, EfxVersion.DMC5, EfxVersion.RE4)]
public partial class EFXAttributeMeshEmitter : EFXAttribute
{
    public EFXAttributeMeshEmitter() : base(EfxAttributeType.MeshEmitter) { }

	public byte unkn0_1; // TODO 4-byte thing
	public byte unkn0_2;
	public byte unkn0_3;
	public byte unkn0_4;
	public float unkn1;
	public int unkn2; // TODO 4-byte sbyte thing?
	public int unkn3;

	public uint unkn4;
	public float unkn5;
	public float unkn6;
	public float unkn7;
    public float unkn8;
    public float unkn9;
    public float unkn10;
    public uint unkn11;
    public uint unkn12;
    public UndeterminedFieldType unkn13;
    public UndeterminedFieldType unkn14;
    public uint unkn15;
    public uint unkn16;
    public UndeterminedFieldType unkn17;

	public float unkn18;
	public float unkn19;
	[RszVersion('>', EfxVersion.RERT, EndAt = nameof(unkn25))]
	public float unkn20;
	public uint unkn21;
	public UndeterminedFieldType unkn22;
	public uint unkn23;
	public uint unkn24;
	public float unkn25;

    [RszStringLengthField(nameof(mapPath))] public int mapPathLength;
    [RszStringLengthField(nameof(meshPath))] public int meshPathLength;
    [RszStringLengthField(nameof(mdfPath))] public int mdfPathLength;
	[RszVersion('>', EfxVersion.DMC5), RszStringLengthField(nameof(texPath))] public int texPathLength;
    [RszStringLengthField(nameof(maskPath))] public int maskPathLength;
	[RszVersion(EfxVersion.DD2), RszStringLengthField(nameof(name))] public int nameLength;
	[RszInlineWString(nameof(mapPathLength))] public string? mapPath; // mapName: NormalRoughnessMap
	[RszInlineWString(nameof(meshPathLength))] public string? meshPath;
	[RszInlineWString(nameof(mdfPathLength))] public string? mdfPath;
	[RszVersion('>', EfxVersion.DMC5), RszInlineWString(nameof(texPathLength))]
	public string? texPath;
	[RszInlineWString(nameof(maskPathLength))] public string? maskPath; // note: might be a path to something else; RE4 doesn't use this one at all

	[RszVersion(EfxVersion.DD2), RszInlineWString(nameof(nameLength))]
	public string? name;
}

[RszGenerate, RszAutoReadWrite, RszVersionedObject(typeof(EfxVersion)), EfxStruct(EfxAttributeType.MeshEmitterClip, EfxVersion.DMC5)]
public partial class EFXAttributeMeshEmitterClip : RszTool.Efx.EFXAttribute, IClipAttribute
{
    public EfxClipData Clip => clipData;
    public BitSet ClipBits => clipBits;

    public EFXAttributeMeshEmitterClip() : base(EfxAttributeType.MeshEmitterClip) { }

	[RszClassInstance] public readonly BitSet clipBits = new BitSet(9);
    public uint unkn1;
	[RszClassInstance] public EfxClipData clipData = new();
}

[RszGenerate, RszAutoReadWrite, RszVersionedObject(typeof(EfxVersion)), EfxStruct(EfxAttributeType.MeshEmitterExpression, EfxVersion.DMC5)]
public partial class EFXAttributeMeshEmitterExpression : RszTool.Efx.EFXAttribute, IExpressionAttribute
{
    public EFXExpressionList Expression { get => expressions; set => expressions = value; }
	public BitSet ExpressionBits => expressionBits;

    public EFXAttributeMeshEmitterExpression() : base(EfxAttributeType.MeshEmitterExpression) { }

	[RszClassInstance] public readonly BitSet expressionBits = new BitSet(16) { BitNameDict = new () {
		[1] = nameof(emitRate),
	} };
    public ExpressionAssignType emitRate;
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
	[RszClassInstance] public EFXExpressionList expressions = new();
}
