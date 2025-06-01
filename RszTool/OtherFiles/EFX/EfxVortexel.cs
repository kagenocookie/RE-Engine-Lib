using System.Numerics;
using RszTool.Efx.Structs.Common;
using RszTool.InternalAttributes;

namespace RszTool.Efx.Structs.Vortexel;

[RszGenerate, RszAutoReadWrite, RszVersionedObject(typeof(EfxVersion)), EfxStruct(EfxAttributeType.PtVortexelWind, EfxVersion.RE4, EfxVersion.MHWilds)]
public partial class EFXAttributePtVortexelWind : EFXAttribute
{
    public EFXAttributePtVortexelWind() : base(EfxAttributeType.PtVortexelWind) { }

    public float unkn1;
    // TODO verify which fields exactly belong to wilds and which are also in RE4
    [RszVersion(EfxVersion.MHWilds, EndAt = nameof(unkn3))]
    public float unkn2;
    public float unkn3;
    public float unkn4;
    public float unkn5;
    public float unkn6;
    public uint unkn7;
    public uint unkn8;
    public short unkn9;
}

[RszGenerate, RszAutoReadWrite, RszVersionedObject(typeof(EfxVersion)), EfxStruct(EfxAttributeType.VortexelWindEmitter, EfxVersion.MHWilds)]
public partial class EFXAttributeVortexelWindEmitter : EFXAttribute
{
    public EFXAttributeVortexelWindEmitter() : base(EfxAttributeType.VortexelWindEmitter) { }

    public UndeterminedFieldType unkn1;
    public uint unkn2;
    public UndeterminedFieldType unkn3;
    public uint unkn4;
    public UndeterminedFieldType unkn5;
    public float unkn6;
    public UndeterminedFieldType unkn7;
    public UndeterminedFieldType unkn8;
    public UndeterminedFieldType unkn9;
    public float unkn10;
    public ByteSet unkn11;
    public float unkn12;
    public float unkn13;
    public float unkn14;
    public float unkn15;
    public float unkn16;
    public float unkn17;
    public float unkn18;
    public float unkn19;
    public float unkn20;
    public short unknShort;
}

[RszGenerate, RszAutoReadWrite, RszVersionedObject(typeof(EfxVersion)), EfxStruct(EfxAttributeType.PtVortexelPhysics, EfxVersion.MHWilds)]
public partial class EFXAttributePtVortexelPhysics : EFXAttribute
{
    public EFXAttributePtVortexelPhysics() : base(EfxAttributeType.PtVortexelPhysics) { }

    public uint unkn1;
    public uint unkn2;
    public float unkn3;
    public uint unkn4;
    public float unkn5;
    public float unkn6;
    public UndeterminedFieldType unkn7;
    public float unkn8;
    public UndeterminedFieldType unkn9;
    public float unkn10;
    public float unkn11;
    public float unkn12;
    public UndeterminedFieldType unkn13;
    public float unkn14;
    public UndeterminedFieldType unkn15;
    public float unkn16;
    public UndeterminedFieldType unkn17;
    public float unkn18;
    public UndeterminedFieldType unkn19;
    public float unkn20;
    public UndeterminedFieldType unkn21;
    public float unkn22;
    public UndeterminedFieldType unkn23;
    public UndeterminedFieldType unkn24;
    public float unkn25;
    public UndeterminedFieldType unkn26;
}

[RszGenerate, RszAutoReadWrite, RszVersionedObject(typeof(EfxVersion)), EfxStruct(EfxAttributeType.PtVortexelPhysicsSimple, EfxVersion.MHWilds)]
public partial class EFXAttributePtVortexelPhysicsSimple : EFXAttribute
{
    public EFXAttributePtVortexelPhysicsSimple() : base(EfxAttributeType.PtVortexelPhysicsSimple) { }

    public uint unkn1;
    public float unkn2;
    public UndeterminedFieldType unkn3;
    public float unkn4;
    public UndeterminedFieldType unkn5;
    public float unkn6;
}

[RszGenerate, RszAutoReadWrite, RszVersionedObject(typeof(EfxVersion)), EfxStruct(EfxAttributeType.PtVortexelSnap, EfxVersion.MHWilds)]
public partial class EFXAttributePtVortexelSnap : EFXAttribute
{
    public EFXAttributePtVortexelSnap() : base(EfxAttributeType.PtVortexelSnap) { }

    public uint unkn1;
    public float unkn2;
    public float unkn3;
    public float unkn4;
    public UndeterminedFieldType unkn5;
    public float unkn6;
}
