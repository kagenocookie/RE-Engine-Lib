using System.Numerics;
using ReeLib.Efx.Structs.Common;
using ReeLib.Efx.Enums;
using ReeLib.InternalAttributes;
using ReeLib.via;

namespace ReeLib.Efx.Structs.Transforms;

[RszGenerate, RszAutoReadWrite, RszVersionedObject(typeof(EfxVersion)), EfxStruct(EfxAttributeType.Transform2D, EfxVersion.DMC5, EfxVersion.RE4)]
public partial class EFXAttributeTransform2D : EFXAttribute
{
	public Vector2 LocalPosition;
	public float LocalRotation;
	public Vector2 LocalScale;

	public EFXAttributeTransform2D() : base(EfxAttributeType.Transform2D) { }

    public override string ToString() => $"Translate {LocalPosition} rotate {LocalRotation} scale {LocalScale}";
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

[RszGenerate, RszAutoReadWrite, RszVersionedObject(typeof(EfxVersion)), EfxStruct(EfxAttributeType.Transform2DExpression, EfxVersion.RE2, EfxVersion.RERT, EfxVersion.RE4)]
public partial class EFXAttributeTransform2DExpression : EFXAttribute, IExpressionAttribute
{
    public EFXExpressionList? Expression { get => expressions; set => expressions = value; }
	public BitSet ExpressionBits => expressionBits;

	public EFXAttributeTransform2DExpression() : base(EfxAttributeType.Transform2DExpression) { }

	[RszClassInstance] public readonly BitSet expressionBits = new BitSet(5) { BitNames = [nameof(posX), nameof(posY), nameof(rot), nameof(scaleX), nameof(scaleY)] };
	public ExpressionAssignType posX;
	public ExpressionAssignType posY;
	public ExpressionAssignType rot;
	public ExpressionAssignType scaleX;
	public ExpressionAssignType scaleY;
	[RszClassInstance, RszConstructorParams(nameof(Version))] public EFXExpressionList? expressions;
}

[RszGenerate, RszAutoReadWrite, RszVersionedObject(typeof(EfxVersion)), EfxStruct(EfxAttributeType.Transform2DModifierDelayFrame, EfxVersion.RE4)]
public partial class EFXAttributeTransform2DModifierDelayFrame : EFXAttribute
{
	public EFXAttributeTransform2DModifierDelayFrame() : base(EfxAttributeType.Transform2DModifierDelayFrame) { }

	public uint frameDelay;
	public uint unkn1;
}

[RszGenerate, RszAutoReadWrite, RszVersionedObject(typeof(EfxVersion)), EfxStruct(EfxAttributeType.Transform3D, EfxVersion.DMC5, EfxVersion.RE4)]
public partial class EFXAttributeTransform3D : EFXAttribute
{
	public EFXAttributeTransform3D() : base(EfxAttributeType.Transform3D) { }

	// [RszVersion(EfxVersion.MHWilds)] public uint mhws_unkn;
	public Vector3 LocalPosition;
	public Vector3 LocalRotation;
	public Vector3 LocalScale;
	public RotationOrder RotationOrder;

    public override string ToString() => $"Translate {LocalPosition} rotate {LocalRotation} scale {LocalScale}";
}

[RszGenerate, RszAutoReadWrite, RszVersionedObject(typeof(EfxVersion)), EfxStruct(EfxAttributeType.Transform3DExpression, EfxVersion.DMC5, EfxVersion.RE4, EfxVersion.DD2)]
public partial class EFXAttributeTransform3DExpression : EFXAttribute, IExpressionAttribute
{
    public EFXExpressionList? Expression { get => expressions; set => expressions = value; }
	public BitSet ExpressionBits => expressionBits;

	public EFXAttributeTransform3DExpression() : base(EfxAttributeType.Transform3DExpression) { }

	/// <summary>
	/// Up to 9 bits, in order: pos X/Y/Z, rot X/Y/Z, scale X/Y/Z
	/// </summary>
	[RszClassInstance] public readonly BitSet expressionBits = new BitSet(9) { BitNameDict = new () {
		[1] = nameof(translationX),
		[2] = nameof(translationY),
		[3] = nameof(translationZ),
		[4] = nameof(rotationX),
		[5] = nameof(rotationY),
		[6] = nameof(rotationZ),
		[7] = nameof(scaleX),
		[8] = nameof(scaleY),
		[9] = nameof(scaleZ),
	} };

	public ExpressionAssignType translationX;
	public ExpressionAssignType translationY;
	public ExpressionAssignType translationZ;
	public ExpressionAssignType rotationX;
	public ExpressionAssignType rotationY;
	public ExpressionAssignType rotationZ;
	public ExpressionAssignType scaleX;
	public ExpressionAssignType scaleY;
	public ExpressionAssignType scaleZ;
	[RszClassInstance, RszConstructorParams(nameof(Version))] public EFXExpressionList? expressions;
}

[RszGenerate, RszAutoReadWrite, RszVersionedObject(typeof(EfxVersion)), EfxStruct(EfxAttributeType.Transform3DClip, EfxVersion.DMC5, EfxVersion.RE4)]
public partial class EFXAttributeTransform3DClip : EFXAttribute, IClipAttribute
{
    public EfxClipData Clip => clipData;
    public BitSet ClipBits => clipBits;

    public EFXAttributeTransform3DClip() : base(EfxAttributeType.Transform3DClip) { }

	[RszClassInstance] public readonly BitSet clipBits = new BitSet(9);
    uint unkn1;
	[RszClassInstance] public EfxClipData clipData = new();
}

[RszGenerate, RszAutoReadWrite, RszVersionedObject(typeof(EfxVersion)), EfxStruct(EfxAttributeType.Transform3DModifierDelayFrame, EfxVersion.RE4)]
public partial class EFXAttributeTransform3DModifierDelayFrame : EFXAttribute
{
	public EFXAttributeTransform3DModifierDelayFrame() : base(EfxAttributeType.Transform3DModifierDelayFrame) { }

	public uint frameDelay;
	public uint unkn1;

}

[RszGenerate, RszAutoReadWrite, RszVersionedObject(typeof(EfxVersion)), EfxStruct(EfxAttributeType.Transform3DModifier, EfxVersion.RE2, EfxVersion.DMC5, EfxVersion.RE8, EfxVersion.RERT, EfxVersion.RE4, EfxVersion.DD2)]
public partial class EFXAttributeTransform3DModifier : ReeLib.Efx.EFXAttribute
{
    public EFXAttributeTransform3DModifier() : base(EfxAttributeType.Transform3DModifier) { }

    public uint unkn0;
    public float unkn1;
    public float unkn2;
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
    public float unkn16;
    public float unkn17;
    public float unkn18;
    public float unkn19;
    public float unkn20;
    public float unkn21;
    public UndeterminedFieldType unkn22;
    public float unkn23;
    public float unkn24;
    public float unkn25;
    public UndeterminedFieldType unkn26;
    public float unkn27;
    public float unkn28;
    public float unkn29;
    public float unkn30;
    public float unkn31;
    public float unkn32;
    public float unkn33;
    public UndeterminedFieldType unkn34;
    public float unkn35;
    public float unkn36;
    public float unkn37;
    public float unkn38;
    public float unkn39;
    public float unkn40;
    public float unkn41;
    public float unkn42;
    public float unkn43;
    public float unkn44;
    public float unkn45;
    public UndeterminedFieldType unkn46;
    public float unkn47;
    public UndeterminedFieldType unkn48;
    public float unkn49;
    public UndeterminedFieldType unkn50;
    public float unkn51;
    public float unkn52;
    public float unkn53;
    public UndeterminedFieldType unkn54;
}

[RszGenerate, RszAutoReadWrite, RszVersionedObject(typeof(EfxVersion)), EfxStruct(EfxAttributeType.PtTransform2D, EfxVersion.DMC5, EfxVersion.RE8)]
public partial class EFXAttributePtTransform2D : ReeLib.Efx.EFXAttribute
{
    public EFXAttributePtTransform2D() : base(EfxAttributeType.PtTransform2D) { }

    public float unkn0;
    public float unkn1;
    public UndeterminedFieldType unkn2;
    public float unkn3;
    public float unkn4;
}

[RszGenerate, RszAutoReadWrite, RszVersionedObject(typeof(EfxVersion)), EfxStruct(EfxAttributeType.PtTransform2DClip, EfxVersion.RE8)]
public partial class EFXAttributePtTransform2DClip : EFXAttribute, IClipAttribute
{
    public EfxClipData Clip => clipData;
    public BitSet ClipBits => clipBits;

	public EFXAttributePtTransform2DClip() : base(EfxAttributeType.PtTransform2DClip) { }

	[RszClassInstance] public readonly BitSet clipBits = new BitSet(5);
    public UndeterminedFieldType unkn1;
	[RszClassInstance] public EfxClipData clipData = new();
}

[RszGenerate, RszAutoReadWrite, RszVersionedObject(typeof(EfxVersion)), EfxStruct(EfxAttributeType.PtTransform3D, EfxVersion.RE7, EfxVersion.RE2, EfxVersion.DMC5, EfxVersion.RE3, EfxVersion.RE8, EfxVersion.RERT, EfxVersion.RE4, EfxVersion.DD2)]
public partial class EFXAttributePtTransform3D : EFXAttribute
{
	public EFXAttributePtTransform3D() : base(EfxAttributeType.PtTransform3D) { }

	public Vector3 position;
	public Vector3 rotation;
	public Vector3 scale;
}

[RszGenerate, RszAutoReadWrite, RszVersionedObject(typeof(EfxVersion)), EfxStruct(EfxAttributeType.PtTransform3DClip, EfxVersion.RE7, EfxVersion.RE2, EfxVersion.DMC5, EfxVersion.RE3, EfxVersion.RE8, EfxVersion.RERT, EfxVersion.RE4)]
public partial class EFXAttributePtTransform3DClip : EFXAttribute, IClipAttribute
{
    public EfxClipData Clip => clipData;
    public BitSet ClipBits => clipBits;

    public EFXAttributePtTransform3DClip() : base(EfxAttributeType.PtTransform3DClip) { }

	[RszClassInstance] public readonly BitSet clipBits = new BitSet(9);
    public uint unkn1;
	[RszClassInstance] public EfxClipData clipData = new();
}

[RszGenerate, RszAutoReadWrite, RszVersionedObject(typeof(EfxVersion)), EfxStruct(EfxAttributeType.PtTransform3DExpression, EfxVersion.DD2)]
public partial class EFXAttributePtTransform3DExpression : EFXAttribute, IExpressionAttribute
{
    public EFXExpressionList? Expression { get => expressions; set => expressions = value; }
	public BitSet ExpressionBits => expressionBits;

	public EFXAttributePtTransform3DExpression() : base(EfxAttributeType.PtTransform3DExpression) { }

	[RszClassInstance] public readonly BitSet expressionBits = new BitSet(9) { BitNameDict = new () {
		[1] = nameof(posX),
		[2] = nameof(posY),
		[3] = nameof(posZ),
		[4] = nameof(rotationX),
		[5] = nameof(rotationY),
		[6] = nameof(rotationZ),
		[7] = nameof(scaleX),
		[8] = nameof(scaleY),
		[9] = nameof(scaleZ),
	} };
	public ExpressionAssignType posX;
	public ExpressionAssignType posY;
	public ExpressionAssignType posZ;
	public ExpressionAssignType rotationX;
	public ExpressionAssignType rotationY;
	public ExpressionAssignType rotationZ;
	public ExpressionAssignType scaleX;
	public ExpressionAssignType scaleY;
	public ExpressionAssignType scaleZ;
	[RszClassInstance, RszConstructorParams(nameof(Version))] public EFXExpressionList? expressions;
}

[RszGenerate, RszAutoReadWrite, RszVersionedObject(typeof(EfxVersion)), EfxStruct(EfxAttributeType.RotateAnim, EfxVersion.DMC5, EfxVersion.RE4)]
public partial class EFXAttributeRotateAnim : EFXAttribute
{
	public EFXAttributeRotateAnim() : base(EfxAttributeType.RotateAnim) { }

	public uint Flags;
    public via.Range RotationAddX;
    public via.Range RotationAddY;
    public via.Range RotationAddZ;
    public via.Range RotationCoefX;
    public via.Range RotationCoefY;
    public via.Range RotationCoefZ;
    [RszVersion(EfxVersion.DD2, EndAt = nameof(RotationDelayFrame))]
	public via.RangeI RotationDelayFrame;
}

[RszGenerate, RszAutoReadWrite, RszVersionedObject(typeof(EfxVersion)), EfxStruct(EfxAttributeType.RotateAnimDelayFrame, EfxVersion.RE4)]
public partial class EFXAttributeRotateAnimDelayFrame : EFXAttribute
{
	public EFXAttributeRotateAnimDelayFrame() : base(EfxAttributeType.RotateAnimDelayFrame) { }

	public uint frameDelay;
	public uint unkn2;
}

[RszGenerate, RszAutoReadWrite, RszVersionedObject(typeof(EfxVersion)), EfxStruct(EfxAttributeType.RotateAnimExpression, EfxVersion.RE7, EfxVersion.DMC5, EfxVersion.RE4)]
public partial class EFXAttributeRotateAnimExpression : EFXAttribute, IExpressionAttribute
{
	public EFXExpressionList? Expression { get => expressions; set => expressions = value; }
	public BitSet ExpressionBits => expressionBits;

	public EFXAttributeRotateAnimExpression() : base(EfxAttributeType.RotateAnimExpression) { }

	[RszClassInstance] public readonly BitSet expressionBits = new BitSet(12){ BitNameDict = new () {
		[1] = nameof(rotateSpeedX),
		[2] = nameof(rotateSpeedXRand),
		[3] = nameof(rotateSpeedY),
		[4] = nameof(rotateSpeedYRand),
		[5] = nameof(rotateSpeedZ),
		[6] = nameof(rotateSpeedZRand),
	} };
	public ExpressionAssignType rotateSpeedX;
	public ExpressionAssignType rotateSpeedXRand;
	public ExpressionAssignType rotateSpeedY;
	public ExpressionAssignType rotateSpeedYRand;
	public ExpressionAssignType rotateSpeedZ;
	public ExpressionAssignType rotateSpeedZRand;
	public ExpressionAssignType ukn1_7;
	public ExpressionAssignType ukn1_8;
	public ExpressionAssignType ukn1_9;
	public ExpressionAssignType ukn1_10;
	public ExpressionAssignType ukn1_11;
	public ExpressionAssignType ukn1_12;
	[RszClassInstance, RszConstructorParams(nameof(Version))] public EFXExpressionList? expressions;
}

[RszGenerate, RszAutoReadWrite, RszVersionedObject(typeof(EfxVersion)), EfxStruct(EfxAttributeType.ScaleAnim, EfxVersion.DMC5, EfxVersion.RE4, EfxVersion.DD2)]
public partial class EFXAttributeScaleAnim : EFXAttribute
{
	public EFXAttributeScaleAnim() : base(EfxAttributeType.ScaleAnim) { }

    public via.Range SizeScalarAdd;
    public via.Range SizeScalarCoef;
    public via.Range SizeXAdd;
    public via.Range SizeXAddCoef;
    public via.Range SizeYAdd;
    public via.Range SizeYAddCoef;
    public via.Range SizeZAdd;
    public via.Range SizeZAddCoef;
    [RszVersion(EfxVersion.DD2, EndAt = nameof(SizeDelayFrame))]
    public via.RangeI SizeDelayFrame;
}
[RszGenerate, RszAutoReadWrite, RszVersionedObject(typeof(EfxVersion)), EfxStruct(EfxAttributeType.ScaleAnimDelayFrame, EfxVersion.RE4)]
public partial class EFXAttributeScaleAnimDelayFrame : EFXAttribute
{
	public EFXAttributeScaleAnimDelayFrame() : base(EfxAttributeType.ScaleAnimDelayFrame) { }

	public uint frameDelay;
	public uint unkn2;

}
[RszGenerate, RszAutoReadWrite, RszVersionedObject(typeof(EfxVersion)), EfxStruct(EfxAttributeType.ScaleAnimExpression, EfxVersion.DMC5, EfxVersion.RE4)]
public partial class EFXAttributeScaleAnimExpression : EFXAttribute, IExpressionAttribute
{
	public EFXExpressionList? Expression { get => expressions; set => expressions = value; }
	public BitSet ExpressionBits => expressionBits;

	public EFXAttributeScaleAnimExpression() : base(EfxAttributeType.ScaleAnimExpression) { }

	[RszClassInstance] public readonly BitSet expressionBits = new BitSet(8) { BitNameDict = new () {
		[1] = nameof(scale),
		[2] = nameof(scaleRand),
	} };
	public ExpressionAssignType scale;
	public ExpressionAssignType scaleRand;
    [RszVersion(EfxVersion.RERT, EndAt = nameof(unkn8))]
	public ExpressionAssignType unkn3;
	public ExpressionAssignType unkn4;
	public ExpressionAssignType unkn5;
	public ExpressionAssignType unkn6;
	public ExpressionAssignType unkn7;
	public ExpressionAssignType unkn8;
	[RszClassInstance, RszConstructorParams(nameof(Version))] public EFXExpressionList? expressions;
}

[RszGenerate, RszAutoReadWrite, RszVersionedObject(typeof(EfxVersion)), EfxStruct(EfxAttributeType.ScaleByDepth, EfxVersion.RE4)]
public partial class EFXAttributeScaleByDepth : EFXAttribute
{
	public EFXAttributeScaleByDepth() : base(EfxAttributeType.ScaleByDepth) { }

	public uint Flags;
	public float NearStart;
	public float NearEnd;
	public float NearScale;
	public float FarStart;
	public float FarEnd;
	public float FarScale;

}
