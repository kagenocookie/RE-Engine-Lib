using System.Numerics;
using ReeLib.Efx.Enums;
using ReeLib.Efx.Structs.Common;
using ReeLib.InternalAttributes;

namespace ReeLib.Efx.Structs.Transforms;

[RszGenerate, RszAutoReadWrite, RszVersionedObject(typeof(EfxVersion)), EfxStruct(EfxAttributeType.Velocity2D, EfxVersion.DMC5, EfxVersion.RE4)]
public partial class EFXAttributeVelocity2D : EFXAttribute
{
	public EFXAttributeVelocity2D() : base(EfxAttributeType.Velocity2D) { }

	public float unkn1_0;
	public float unkn1_1;
	public float unkn1_2;
	public float unkn1_3;
	public float unkn1_4;

	public float unkn1_5;
	public float unkn1_6;
	public float unkn1_7;
    [RszVersion(EfxVersion.RE4, EndAt = nameof(re4_unkn2))]
	public uint re4_unkn0;
	public float re4_unkn1;
	public float re4_unkn2;

	public float unkn2_0;
	public float unkn2_1;
	public uint unkn2_2;
	public float unkn2_3;

	public float unkn2_4;
    [RszVersion(EfxVersion.RE4, EndAt = nameof(re4_unkn2_1))]
	public uint re4_unkn2_0;
	public uint re4_unkn2_1;
}
[RszGenerate, RszAutoReadWrite, RszVersionedObject(typeof(EfxVersion)), EfxStruct(EfxAttributeType.Velocity2DDelayFrame, EfxVersion.MHWilds)]
public partial class EFXAttributeVelocity2DDelayFrame : EFXAttribute
{
	public EFXAttributeVelocity2DDelayFrame() : base(EfxAttributeType.Velocity2DDelayFrame) { }

	public uint DelayFrames;
	public uint Unkn1;
}

[RszGenerate, RszAutoReadWrite, RszVersionedObject(typeof(EfxVersion)), EfxStruct(EfxAttributeType.Velocity2DExpression, EfxVersion.RERT, EfxVersion.RE4, EfxVersion.DD2)]
public partial class EFXAttributeVelocity2DExpression : EFXAttribute, IExpressionAttribute
{
	public EFXExpressionList? Expression { get => expressions; set => expressions = value; }
	public BitSet ExpressionBits => expressionBits;

	public EFXAttributeVelocity2DExpression() : base(EfxAttributeType.Velocity2DExpression) { }

	[RszClassInstance] public readonly BitSet expressionBits = new BitSet(13) { BitNameDict = new () {
		[1] = nameof(speed),
		[2] = nameof(speedRand),
		[4] = nameof(gravity),
		[7] = nameof(angle),
		[8] = nameof(angleRand),
	}};
    public ExpressionAssignType speed;
    public ExpressionAssignType speedRand;
    public ExpressionAssignType unkn3;
    public ExpressionAssignType gravity;
    public ExpressionAssignType unkn5;
    public ExpressionAssignType unkn6;
    public ExpressionAssignType angle;
    public ExpressionAssignType angleRand;
    [RszVersion(EfxVersion.RE4, EndAt = nameof(unkn13))]
    public ExpressionAssignType unkn9;
    public ExpressionAssignType unkn10;
    public ExpressionAssignType unkn11;
    public ExpressionAssignType unkn12;
    public ExpressionAssignType unkn13;

	[RszClassInstance, RszConstructorParams(nameof(Version))] public EFXExpressionList? expressions;
}
[RszGenerate, RszAutoReadWrite, RszVersionedObject(typeof(EfxVersion)), EfxStruct(EfxAttributeType.Velocity3D, EfxVersion.RE7, EfxVersion.RE2, EfxVersion.DMC5, EfxVersion.RE3, EfxVersion.RE8, EfxVersion.RERT, EfxVersion.RE4)]
public partial class EFXAttributeVelocity3D : EFXAttribute
{
	public EFXAttributeVelocity3D() : base(EfxAttributeType.Velocity3D) { }

	[RszVersion(EfxVersion.RE2)]
	public uint Flags;
	public via.Range DirectionVectorX;
    public via.Range DirectionVectorY;
    public via.Range DirectionVectorZ;
    public via.Range Speed;
    public via.Range SpeedCoef;
    [RszVersion(EfxVersion.RE4)]
    public via.RangeI SpeedDelayFrame;

    public Vector3 Offset;
    public Vector3 Size;
    public VelocityType VelocityType;
    public via.Range GravityRate;
    [RszVersion(EfxVersion.RE4)]
    public via.RangeI GravityDelayFrame;

    [RszVersion(EfxVersion.RE2, EndAt = nameof(Spread))]
    public via.Range InheritRate;
    public via.Range InheritDistance;
    [RszVersion(EfxVersion.RE8)]
    public via.Range Spread;
}
[RszGenerate, RszAutoReadWrite, RszVersionedObject(typeof(EfxVersion)), EfxStruct(EfxAttributeType.Velocity3DDelayFrame, EfxVersion.RE4)]
public partial class EFXAttributeVelocity3DDelayFrame : EFXAttribute
{
	public EFXAttributeVelocity3DDelayFrame() : base(EfxAttributeType.Velocity3DDelayFrame) { }

	public uint frameDelay;
	public uint unkn2;

}
[RszGenerate, RszAutoReadWrite, RszVersionedObject(typeof(EfxVersion)), EfxStruct(EfxAttributeType.Velocity3DExpression, EfxVersion.RE3, EfxVersion.RE8, EfxVersion.RERT, EfxVersion.RE4)]
public partial class EFXAttributeVelocity3DExpression : EFXAttribute, IExpressionAttribute
{
	public EFXExpressionList? Expression { get => expressions; set => expressions = value; }
    public BitSet ExpressionBits => expressionBits;

	public EFXAttributeVelocity3DExpression() : base(EfxAttributeType.Velocity3DExpression) { }

    [RszVersion(EfxVersion.RE8, EndAt = nameof(range3))]
	public uint rert_unkn0;
	public uint rert_unkn1;
	public Vector2 range1;
	public Vector2 range2;
	public Vector2 range3;

	[RszClassInstance] public readonly BitSet expressionBits = new BitSet(19){ BitNameDict = new () {
		[1] = nameof(speed),
		[2] = nameof(speedRand),
		[7] = nameof(velocityX),
		[8] = nameof(velocityXRandom),
		[9] = nameof(velocityY),
		[10] = nameof(velocityYRandom),
		[11] = nameof(velocityZ),
		[12] = nameof(velocityZRandom),
		// [13] = nameof(vectorX), // direction?
		// [14] = nameof(vectorY),
		// [15] = nameof(vectorZ),
	} };
	public ExpressionAssignType speed;
	public ExpressionAssignType speedRand;
	public ExpressionAssignType unkn3;
	public ExpressionAssignType unkn4;
	public ExpressionAssignType unkn5;
	public ExpressionAssignType unkn6;
	public ExpressionAssignType velocityX;
	public ExpressionAssignType velocityXRandom;
	public ExpressionAssignType velocityY;
	public ExpressionAssignType velocityYRandom;
	public ExpressionAssignType velocityZ;
	public ExpressionAssignType velocityZRandom;

    [RszVersion(">", EfxVersion.RERT, EndAt = nameof(unkn14))]
	public ExpressionAssignType unkn13;
	public ExpressionAssignType unkn14;

    [RszVersion(EfxVersion.RE4, EndAt = nameof(unkn19))]
	public ExpressionAssignType unkn15;
	public ExpressionAssignType unkn16;
	public ExpressionAssignType unkn17;
	public ExpressionAssignType unkn18;
	public ExpressionAssignType unkn19;

	[RszClassInstance, RszConstructorParams(nameof(Version))] public EFXExpressionList? expressions;

}

[RszGenerate, RszAutoReadWrite, RszVersionedObject(typeof(EfxVersion)), EfxStruct(EfxAttributeType.AngularVelocity2D, EfxVersion.DMC5, EfxVersion.RE4)]
public partial class EFXAttributeAngularVelocity2D : EFXAttribute
{
	public EFXAttributeAngularVelocity2D() : base(EfxAttributeType.AngularVelocity2D) { }

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
	public float radians12;
	public float radians13;
	public float radians14;
}

[RszGenerate, RszAutoReadWrite, RszVersionedObject(typeof(EfxVersion)), EfxStruct(EfxAttributeType.AngularVelocity2DDelayFrame, EfxVersion.DMC5, EfxVersion.RE4)]
public partial class EFXAttributeAngularVelocity2DDelayFrame : EFXAttribute
{
	public EFXAttributeAngularVelocity2DDelayFrame() : base(EfxAttributeType.AngularVelocity2DDelayFrame) { }

	public uint DelayFrames;
	public uint Unkn1;
}

[RszGenerate, RszAutoReadWrite, RszVersionedObject(typeof(EfxVersion)), EfxStruct(EfxAttributeType.AngularVelocity3D, EfxVersion.DMC5, EfxVersion.RE4)]
public partial class EFXAttributeAngularVelocity3D : EFXAttribute
{
	public EFXAttributeAngularVelocity3D() : base(EfxAttributeType.AngularVelocity3D) { }

	public uint Flags;
	public RotationOrder RotationOrder;
	public via.Range Radius;
	public via.Range AddRadius;
	public float AddRadiusCoef;
	public via.Range RotationAxisX;
    public via.Range RotationAxisY;
    public via.Range RotationAxisZ;
    public via.Range AddRotationAxisX;
    public via.Range AddRotationAxisY;
    public via.Range AddRotationAxisZ;
    public float AddRotationCoef;
}

[RszGenerate, RszAutoReadWrite, RszVersionedObject(typeof(EfxVersion)), EfxStruct(EfxAttributeType.AngularVelocity3DDelayFrame, EfxVersion.RERT, EfxVersion.RE4)]
public partial class EFXAttributeAngularVelocity3DDelayFrame : EFXAttribute
{
	public EFXAttributeAngularVelocity3DDelayFrame() : base(EfxAttributeType.AngularVelocity3DDelayFrame) { }

	public uint frameDelay;
	public uint unkn1;
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

[RszGenerate, RszAutoReadWrite, RszVersionedObject(typeof(EfxVersion)), EfxStruct(EfxAttributeType.PtVelocity3D, EfxVersion.RE7, EfxVersion.RE2, EfxVersion.DMC5, EfxVersion.RE3, EfxVersion.RE8, EfxVersion.RERT, EfxVersion.RE4)]
public partial class EFXAttributePtVelocity3D : EFXAttribute
{
	public EFXAttributePtVelocity3D() : base(EfxAttributeType.PtVelocity3D) { }

	public float unkn1_0;
	public float unkn1_1;
	public float unkn1_2;
	public float unkn1_3;
}

[RszGenerate, RszAutoReadWrite, RszVersionedObject(typeof(EfxVersion)), EfxStruct(EfxAttributeType.PtVelocity3DClip, EfxVersion.DMC5, EfxVersion.RE4)]
public partial class EFXAttributePtVelocity3DClip : EFXAttribute, IClipAttribute
{
    public EfxClipData Clip => clipData;
    public BitSet ClipBits => clipBits;

    public EFXAttributePtVelocity3DClip() : base(EfxAttributeType.PtVelocity3DClip) { }

	[RszClassInstance] public readonly BitSet clipBits = new BitSet(4);
    uint unkn1;
	[RszClassInstance] public EfxClipData clipData = new();
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

[RszGenerate, RszAutoReadWrite, RszVersionedObject(typeof(EfxVersion)), EfxStruct(EfxAttributeType.PtAngularVelocity3DExpression, EfxVersion.DD2)]
public partial class EFXAttributePtAngularVelocity3DExpression : EFXAttribute, IExpressionAttribute
{
    public EFXExpressionList? Expression { get => expressions; set => expressions = value; }
	public BitSet ExpressionBits => expressionBits;

	public EFXAttributePtAngularVelocity3DExpression() : base(EfxAttributeType.PtAngularVelocity3DExpression) { }

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

[RszGenerate, RszAutoReadWrite, RszVersionedObject(typeof(EfxVersion)), EfxStruct(EfxAttributeType.PtAngularVelocity2D, EfxVersion.DD2)]
public partial class EFXAttributePtAngularVelocity2D : EFXAttribute
{
	public EFXAttributePtAngularVelocity2D() : base(EfxAttributeType.PtAngularVelocity2D) { }

	public float unkn1;
	public float unkn2;
	public float unkn3;
	public float unkn4;
	public float unkn5;
	public float unkn6;
}

[RszGenerate, RszAutoReadWrite, RszVersionedObject(typeof(EfxVersion)), EfxStruct(EfxAttributeType.PtAngularVelocity2DExpression, EfxVersion.DD2)]
public partial class EFXAttributePtAngularVelocity2DExpression : EFXAttribute
{
	public EFXAttributePtAngularVelocity2DExpression() : base(EfxAttributeType.PtAngularVelocity2DExpression) { }

	public uint unkn1;
	public uint unkn2;
	public uint unkn3;
	public uint unkn4;
	public uint unkn5;
	public uint unkn6;
	public uint unkn7;
	[RszClassInstance, RszConstructorParams(nameof(Version))] public EFXExpressionList? expressions;
}
