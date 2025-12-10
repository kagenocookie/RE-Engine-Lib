using ReeLib.Efx.Structs.Common;
using ReeLib.Efx.Enums;
using ReeLib.InternalAttributes;
using System.Numerics;

namespace ReeLib.Efx.Structs.Transforms;

[RszGenerate, RszAutoReadWrite, RszVersionedObject(typeof(EfxVersion)), EfxStruct(EfxAttributeType.EmitterColor, EfxVersion.RE4)]
public partial class EFXAttributeEmitterColor : EFXAttribute
{
	public EFXAttributeEmitterColor() : base(EfxAttributeType.EmitterColor) { }

	public EmitterColorOperator ColorOperator;
	public EmitterColorOperator AlphaOperator;
	public via.Color Color;

    public override string ToString() => $"EmitterColor: {Color}";
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

	public via.Range RangeX;
    public via.Range RangeY;
	public Shape2DType ShapeType;
	public uint RangeDivideNum;
	public AxisXYZ DivideAxis;
	public float LocalRotation;

}

[RszGenerate, RszAutoReadWrite, RszVersionedObject(typeof(EfxVersion)), EfxStruct(EfxAttributeType.EmitterShape2DExpression, EfxVersion.RE4)]
public partial class EFXAttributeEmitterShape2DExpression : EFXAttribute, IExpressionAttribute
{
	public EFXExpressionList? Expression { get => expressions; set => expressions = value; }
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
	[RszClassInstance] public EFXExpressionList? expressions = new();
}

[RszGenerate, RszAutoReadWrite, RszVersionedObject(typeof(EfxVersion)), EfxStruct(EfxAttributeType.EmitterShape3D, EfxVersion.RERT, EfxVersion.RE4, EfxVersion.DD2)]
public partial class EFXAttributeEmitterShape3D : EFXAttribute
{
	public EFXAttributeEmitterShape3D() : base(EfxAttributeType.EmitterShape3D) { }

    public via.Range RangeX;
    public via.Range RangeY;
    public via.Range RangeZ;
    public Shape3DType ShapeType;
	public uint RangeDivideNum;
	public AxisXYZ RangeDivideAxis;
	[RszVersion(">", EfxVersion.RE3, EndAt = nameof(RangeDivideVerticalNum))]
	public uint RangeDivideHorizontalNum;
	public uint RangeDivideVerticalNum;
    public Vector3 LocalRotation;

    public RotationOrder RotationOrder;
	[RszVersion(EfxVersion.RE2, EndAt = nameof(ScaleVertical))]
    public RotationCorrectType RotationCorrect;

    [RszVersion(EfxVersion.RE4, EndAt = nameof(DivideEquidistantRecalcEveryFrameData))]
    public bool DivideEquidistant;
    public bool DivideEquidistantCalcOuterCurveData;
    public bool DivideEquidistantRecalcEveryFrameData;

    public bool UseExtension;

    public via.Range ScaleHorizontal;//Cylinder angle on cylinder shape
    public via.Range ScaleVertical;
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

	public uint Flags;
	public float YNormalRange;
	public int PartsNumber;
	public int ClusterNumber;

	public RotationOrder RotationOrder;
	public Vector3 LocalScale;
    public Vector3 LocalRotation;
    public uint ColorUTilingCount;
    public uint ColorVTilingCount;
    public Vector2 ColorUVOffset;
    public uint MaskUTilingCount;
    public uint MaskVTilingCount;
    public Vector2 MaskUVOffset;

	public float EmissionThreshold;
	[RszVersion('>', EfxVersion.RERT, EndAt = nameof(TriangleFilter))]
	public float EmissionVertexColorThreshold;
	public uint EmissionVertexColorChannel;
	public uint EmissionMaskMapColorChannel;
	public uint ParticleNum;
	public uint LodIndex;
	public float TriangleFilter;

    [RszStringLengthField(nameof(DynamicColorMapPath))] public int DynamicColorMapNameLength;
    [RszStringLengthField(nameof(MeshPath))] public int MeshPathLength;
    [RszStringLengthField(nameof(MeshMirrorPath))] public int MeshMirrorPathLength;
	[RszVersion('>', EfxVersion.DMC5), RszStringLengthField(nameof(ColorMapPath))] public int ColorMapPathLength;
    [RszStringLengthField(nameof(MaskMapPath))] public int MaskMapPathLength;
	[RszVersion(EfxVersion.DD2), RszStringLengthField(nameof(TargetGameObjectName))] public int TargetGameObjectNameLength;
	[RszInlineWString(nameof(DynamicColorMapNameLength))] public string? DynamicColorMapPath; // mapName: NormalRoughnessMap
	[RszInlineWString(nameof(MeshPathLength))] public string? MeshPath;
	[RszInlineWString(nameof(MeshMirrorPathLength))] public string? MeshMirrorPath;
	[RszVersion('>', EfxVersion.DMC5), RszInlineWString(nameof(ColorMapPathLength))]
	public string? ColorMapPath;
	[RszInlineWString(nameof(MaskMapPathLength))] public string? MaskMapPath; // note: might be a path to something else; RE4 doesn't use this one at all

	[RszVersion(EfxVersion.DD2), RszInlineWString(nameof(TargetGameObjectNameLength))]
	public string? TargetGameObjectName;

    public override string ToString() => !string.IsNullOrEmpty(MeshPath) ? MeshPath : type.ToString();
}

[RszGenerate, RszAutoReadWrite, RszVersionedObject(typeof(EfxVersion)), EfxStruct(EfxAttributeType.MeshEmitterClip, EfxVersion.DMC5)]
public partial class EFXAttributeMeshEmitterClip : ReeLib.Efx.EFXAttribute, IClipAttribute
{
    public EfxClipData Clip => clipData;
    public BitSet ClipBits => clipBits;

    public EFXAttributeMeshEmitterClip() : base(EfxAttributeType.MeshEmitterClip) { }

	[RszClassInstance] public readonly BitSet clipBits = new BitSet(9);
    public uint unkn1;
	[RszClassInstance] public EfxClipData clipData = new();
}

[RszGenerate, RszAutoReadWrite, RszVersionedObject(typeof(EfxVersion)), EfxStruct(EfxAttributeType.MeshEmitterExpression, EfxVersion.DMC5)]
public partial class EFXAttributeMeshEmitterExpression : ReeLib.Efx.EFXAttribute, IExpressionAttribute
{
    public EFXExpressionList? Expression { get => expressions; set => expressions = value; }
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
	[RszClassInstance] public EFXExpressionList? expressions = new();
}
