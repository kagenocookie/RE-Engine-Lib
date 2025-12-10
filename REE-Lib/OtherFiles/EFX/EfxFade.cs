using ReeLib.Efx.Structs.Common;
using ReeLib.Efx.Enums;
using ReeLib.InternalAttributes;
using System.Numerics;

namespace ReeLib.Efx.Structs.Transforms;

[RszGenerate, RszAutoReadWrite, RszVersionedObject(typeof(EfxVersion)), EfxStruct(EfxAttributeType.FadeByAngle, EfxVersion.DMC5, EfxVersion.RE4)]
public partial class EFXAttributeFadeByAngle : EFXAttribute
{
	public EFXAttributeFadeByAngle() : base(EfxAttributeType.FadeByAngle) { }

	public uint Flags;
	public float Cone;
	public float Spread;
	public float AlphaRate;
	public Vector3 ConeDirection;
}

[RszGenerate, RszAutoReadWrite, RszVersionedObject(typeof(EfxVersion)), EfxStruct(EfxAttributeType.FadeByAngleExpression, EfxVersion.DMC5, EfxVersion.RE4)]
public partial class EFXAttributeFadeByAngleExpression : EFXAttribute, IExpressionAttribute
{
	public EFXExpressionList? Expression { get => expressions; set => expressions = value; }
	public BitSet ExpressionBits => expressionBits;

	public EFXAttributeFadeByAngleExpression() : base(EfxAttributeType.FadeByAngleExpression) { }

	[RszClassInstance] public readonly BitSet expressionBits = new BitSet(2) { BitNameDict = new () {
		[1] = nameof(minAngle),
		[2] = nameof(maxAngle),
	} };
    public ExpressionAssignType minAngle;
    public ExpressionAssignType maxAngle;
	[RszClassInstance] public EFXExpressionList? expressions = new();
}
[RszGenerate, RszAutoReadWrite, RszVersionedObject(typeof(EfxVersion)), EfxStruct(EfxAttributeType.FadeByDepth, EfxVersion.DMC5, EfxVersion.RERT, EfxVersion.RE4, EfxVersion.DD2)]
public partial class EFXAttributeFadeByDepth : EFXAttribute
{
	public EFXAttributeFadeByDepth() : base(EfxAttributeType.FadeByDepth) { }

	public float NearStart;
	public float NearEnd;
	public float FarStart;
	public float FarEnd;

    public override string ToString() => $"{NearStart} - {NearEnd} - {FarStart} - {FarEnd}";
}

[RszGenerate, RszAutoReadWrite, RszVersionedObject(typeof(EfxVersion)), EfxStruct(EfxAttributeType.FadeByDepthExpression, EfxVersion.DMC5, EfxVersion.RERT, EfxVersion.RE4, EfxVersion.DD2)]
public partial class EFXAttributeFadeByDepthExpression : EFXAttribute, IExpressionAttribute
{
	public EFXExpressionList? Expression { get => expressions; set => expressions = value; }
	public BitSet ExpressionBits => expressionBits;

	public EFXAttributeFadeByDepthExpression() : base(EfxAttributeType.FadeByDepthExpression) { }

	[RszClassInstance] public readonly BitSet expressionBits = new BitSet(4) { BitNames = [ nameof(nearStart), nameof(nearEnd), nameof(farStart), nameof(farEnd) ] };
	public ExpressionAssignType nearStart;
	public ExpressionAssignType nearEnd;
	public ExpressionAssignType farStart;
	public ExpressionAssignType farEnd;
	[RszClassInstance, RszConstructorParams(nameof(Version))] public EFXExpressionList? expressions;
}

[RszGenerate, RszAutoReadWrite, RszVersionedObject(typeof(EfxVersion)), EfxStruct(EfxAttributeType.FadeByEmitterAngle, EfxVersion.DMC5, EfxVersion.RE4)]
public partial class EFXAttributeFadeByEmitterAngle : EFXAttribute
{
	public EFXAttributeFadeByEmitterAngle() : base(EfxAttributeType.FadeByEmitterAngle) { }

	public uint Flags;
	public float Cone;
	public float Spread;
	public float AlphaRate;
	public float FadeInStart;
	public float FadeInEnd;
	[RszVersion(EfxVersion.DD2)]
	public UndeterminedFieldType FadeBlend;
}

[RszGenerate, RszAutoReadWrite, RszVersionedObject(typeof(EfxVersion)), EfxStruct(EfxAttributeType.FadeByOcclusion, EfxVersion.DMC5, EfxVersion.RE4)]
public partial class EFXAttributeFadeByOcclusion : EFXAttribute
{
	public EFXAttributeFadeByOcclusion() : base(EfxAttributeType.FadeByOcclusion) { }

	public float Radius;
	public Vector2 Offset;
	public float MinSize;
}

[RszGenerate, RszAutoReadWrite, RszVersionedObject(typeof(EfxVersion)), EfxStruct(EfxAttributeType.FadeByRootCulling, EfxVersion.DD2, EfxVersion.MHWilds)]
public partial class EFXAttributeFadeByRootCulling : EFXAttribute
{
	public EFXAttributeFadeByRootCulling() : base(EfxAttributeType.FadeByRootCulling) { }

	public EffectBoundsType ShapeType;
	public Vector3 Center;
	public float InnerRadius;
	public float OuterRadius;
    public Vector3 InnerSize;
    public Vector3 OuterSize;
    public Vector3 LocalRotation;
    public RotationOrder RotationOrder;
}
