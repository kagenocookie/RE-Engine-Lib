using System.Numerics;
using RszTool.Efx.Structs.Common;
using RszTool.InternalAttributes;

namespace RszTool.Efx.Structs.Transforms;

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
	public float re4_unkn0;
	public float re4_unkn1;
	public float re4_unkn2;

	public float unkn2_0;
	public float unkn2_1;
	public uint unkn2_2;
	public float unkn2_3;

	public float unkn2_4;
    [RszVersion(EfxVersion.RE4, EndAt = nameof(re4_unkn2_1))]
	public UndeterminedFieldType re4_unkn2_0;
	public UndeterminedFieldType re4_unkn2_1;
}
[RszGenerate, RszAutoReadWrite, RszVersionedObject(typeof(EfxVersion)), EfxStruct(EfxAttributeType.Velocity2DExpression, EfxVersion.RERT, EfxVersion.RE4, EfxVersion.DD2)]
public partial class EFXAttributeVelocity2DExpression : EFXAttribute, IExpressionAttribute
{
	public EFXExpressionList? Expression => expressions;
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
[RszGenerate, RszAutoReadWrite, RszVersionedObject(typeof(EfxVersion)), EfxStruct(EfxAttributeType.Velocity3D, EfxVersion.RE2, EfxVersion.DMC5, EfxVersion.RE3, EfxVersion.RE8, EfxVersion.RERT, EfxVersion.RE4)]
public partial class EFXAttributeVelocity3D : EFXAttribute
{
	public EFXAttributeVelocity3D() : base(EfxAttributeType.Velocity3D) { }

	[RszVersion(EfxVersion.RE2)]
	public uint unkn1;
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
	[RszVersion(EfxVersion.RE4, EndAt = nameof(unkn2_11))]
	public uint unkn2_10;
	public uint unkn2_11;
	public float unkn2_12;
	public float unkn2_13;
	public float unkn2_14;
	public float unkn2_15;
	public float re4_unkn0;
	public float re4_unkn1;

    public uint unkn3;
	public float unkn4_0;
	[RszVersion(EfxVersion.RE2)]
	public float unkn4_1;

    [RszVersion(EfxVersion.RE4, EndAt = nameof(re4_unkn3))]
	public uint re4_unkn2;
	public uint re4_unkn3;

	public float unkn4_2;
	[RszVersion(EfxVersion.RE2, EndAt = nameof(unkn4_5))]
	public float unkn4_3;
	public float unkn4_4;
	public float unkn4_5;

	[RszVersion(EfxVersion.RE8, EndAt = nameof(sb_unkn1))]
	public float sb_unkn0;
	public float sb_unkn1;
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
	public EFXExpressionList? Expression => expressions;
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
	public uint unkn4;
	public float unkn5;
	public uint unkn6;
	public float unkn7;
	public float unkn8;
	public float unkn9;
	public float unkn10;
	public float unkn11;
	public float radians12;
	public float radians13;
	public float radians14;
}

[RszGenerate, RszAutoReadWrite, RszVersionedObject(typeof(EfxVersion)), EfxStruct(EfxAttributeType.AngularVelocity3D, EfxVersion.DMC5, EfxVersion.RE4)]
public partial class EFXAttributeAngularVelocity3D : EFXAttribute
{
	public EFXAttributeAngularVelocity3D() : base(EfxAttributeType.AngularVelocity3D) { }

	public uint unkn1_0;
	public uint unkn1_1;
	public float unkn1_2;
	public float unkn1_3;
	public float unkn1_4;
	public float unkn1_5;
	public float unkn1_6;
	public float unkn1_7;
	public float unkn1_8;
	public float unkn1_9;
	public float unkn1_10;
	public float unkn1_11;
	public float unkn1_12;
	public float unkn1_13;
	public float unkn1_14;
	public float unkn1_15;
	public float unkn1_16;
	public float unkn1_17;
	public float unkn1_18;
	public float unkn1_19;
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
