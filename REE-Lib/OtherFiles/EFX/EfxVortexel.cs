using ReeLib.Efx.Structs.Common;
using ReeLib.InternalAttributes;
using ReeLib.Efx.Enums;
using System.Numerics;

namespace ReeLib.Efx.Structs.Vortexel;

[RszGenerate, RszAutoReadWrite, RszVersionedObject(typeof(EfxVersion)), EfxStruct(EfxAttributeType.PtVortexelWind, EfxVersion.RE4, EfxVersion.MHWilds)]
public partial class EFXAttributePtVortexelWind : EFXAttribute
{
    public EFXAttributePtVortexelWind() : base(EfxAttributeType.PtVortexelWind) { }

    public via.Range TransitionRate;
    // TODO verify which fields exactly belong to wilds and which are also in RE4
    [RszVersion(EfxVersion.MHWilds)]
    public float unkn7;
    public float unkn8;
    public float TransitionThreshold;
    public uint MaxSpeed; // TODO recheck wilds
    public uint TransitionWaitFrame;
    [RszVersion('>', EfxVersion.RE4)]
    public uint TransitionBlendFrame;
    public bool IsTransitionWaitFramePerParticle;
    public bool IsMergeGravity;
}

[RszGenerate, RszAutoReadWrite, RszVersionedObject(typeof(EfxVersion)), EfxStruct(EfxAttributeType.VortexelWindEmitter, EfxVersion.MHWilds)]
public partial class EFXAttributeVortexelWindEmitter : EFXAttribute
{
    public EFXAttributeVortexelWindEmitter() : base(EfxAttributeType.VortexelWindEmitter) { }

    public VelocityEmitType EmitType;
    public VelocityShapeType Shape;
    public VelocityShapeType Direction;
    public VelocityAttenuationType Attenuation;
    public Vector3 Axis;
    public float Speed;
    public bool UseOcclusionRate;
    public bool UsePressureRate;
    public bool IsOutdoorWind;
    public bool AttenuatePressure;
    public float unkn0;
    public uint flags;
    public float unkn1;
    public float unkn2;
    public float unkn3;
    public float unkn4;
    public float unkn5;
    public float unkn6;
    public float unkn7;
    public float unkn8;
    public float unkn9;
    public short unkn10;
    // public Vector2 Pressure; TODO where should this be?
}

[RszGenerate, RszAutoReadWrite, RszVersionedObject(typeof(EfxVersion)), EfxStruct(EfxAttributeType.VortexelWindEmitterExpression, EfxVersion.MHWilds)]
public partial class EFXAttributeVortexelWindEmitterExpression : ReeLib.Efx.EFXAttribute, IExpressionAttribute
{
    public EFXExpressionList? Expression { get => expressions; set => expressions = value; }
	public BitSet ExpressionBits => expressionBits;

    public EFXAttributeVortexelWindEmitterExpression() : base(EfxAttributeType.VortexelWindEmitterExpression) { }

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
	[RszClassInstance, RszConstructorParams(nameof(Version))] public EFXExpressionList? expressions;
}

[RszGenerate, RszAutoReadWrite, RszVersionedObject(typeof(EfxVersion)), EfxStruct(EfxAttributeType.PtVortexelPhysics, EfxVersion.MHWilds)]
public partial class EFXAttributePtVortexelPhysics : EFXAttribute
{
    public EFXAttributePtVortexelPhysics() : base(EfxAttributeType.PtVortexelPhysics) { }

    public uint Flags;
    public SolidBodyShapeType Shape;
    public Vector3 Size;
    public via.Range BounceRate;
    public via.Range Friction;
    public float MassDensity;
    public float MomentBias;
    public float PenaltyKinetic;
    public via.Range RotationX;
    public via.Range RotationY;
    public via.Range RotationZ;
    public via.Range InitAngularVelocityX;
    public via.Range InitAngularVelocityY;
    public via.Range InitAngularVelocityZ;
    public via.Range AngularVelocityCoef;
}

[RszGenerate, RszAutoReadWrite, RszVersionedObject(typeof(EfxVersion)), EfxStruct(EfxAttributeType.PtVortexelPhysicsSimple, EfxVersion.MHWilds)]
public partial class EFXAttributePtVortexelPhysicsSimple : EFXAttribute
{
    public EFXAttributePtVortexelPhysicsSimple() : base(EfxAttributeType.PtVortexelPhysicsSimple) { }

    public uint Flags;
    public via.Range HorizontalBounceRate;
    public via.Range VerticalBounceRate;
    public float Radius;
}

[RszGenerate, RszAutoReadWrite, RszVersionedObject(typeof(EfxVersion)), EfxStruct(EfxAttributeType.PtVortexelSnap, EfxVersion.MHWilds)]
public partial class EFXAttributePtVortexelSnap : EFXAttribute
{
    public EFXAttributePtVortexelSnap() : base(EfxAttributeType.PtVortexelSnap) { }

    public uint Flags;
    public float RayDistance;
    public float RayStartOffset;
    public float RayHitOffset;
    public float FinishAngleMin;
    public float FinishAngleMax;
}

[RszGenerate, RszAutoReadWrite, RszVersionedObject(typeof(EfxVersion)), EfxStruct(EfxAttributeType.VortexelCollider, EfxVersion.MHWilds)]
public partial class EFXAttributeVortexelCollider : EFXAttribute
{
    public EFXAttributeVortexelCollider() : base(EfxAttributeType.VortexelCollider) { }

    public uint Flags;
    public float Unkn0;
    public float Unkn1;
    public float Unkn2;
    public float Unkn3;
    public float Unkn4;
    public float Unkn5;
    public float Unkn6;
}

[RszGenerate, RszAutoReadWrite, RszVersionedObject(typeof(EfxVersion)), EfxStruct(EfxAttributeType.VortexelIndoorMask, EfxVersion.MHWilds)]
public partial class EFXAttributeVortexelIndoorMask : EFXAttribute
{
    public EFXAttributeVortexelIndoorMask() : base(EfxAttributeType.VortexelIndoorMask) { }

    public uint Flags;
    public float Unkn0;
    public float Unkn1;
    public float Unkn2;
    public float Unkn3;
    public float Unkn4;
}
