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
    public float MaxSpeed;
    public uint TransitionWaitFrame;
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
    public Vector2 Pressure;
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
