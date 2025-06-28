using System.Numerics;
using ReeLib.Efx.Structs.Common;
using ReeLib.InternalAttributes;

namespace ReeLib.Efx.Structs.Unknowns;

[RszGenerate, RszAutoReadWrite, RszVersionedObject(typeof(EfxVersion)), EfxStruct(EfxAttributeType.UnknownRERT_233, EfxVersion.RERT)]
public partial class EFXAttributeUnknownRERT_233 : EFXAttribute
{
	public EFXAttributeUnknownRERT_233() : base(EfxAttributeType.UnknownRERT_233) { }

	public uint ukn0;
	public uint hash1;
	public uint hash2;
	public UndeterminedFieldType ukn3;
	public int ukn4;
}

[RszGenerate, RszAutoReadWrite, RszVersionedObject(typeof(EfxVersion)), EfxStruct(EfxAttributeType.UnknownRE4_197, EfxVersion.RE4)]
public partial class EFXAttributeUnknownRE4_197 : EFXAttribute
{
	public EFXAttributeUnknownRE4_197() : base(EfxAttributeType.UnknownRE4_197) { }

    public uint unkn0;
    public uint unkn1;
    public uint unkn2;
    public uint unkn3;
    public float unkn4;
    public float unkn5;
    public float unkn6;
    public float unkn7;
    public byte unkn8;
    public byte unkn9;
    public byte unkn10;
    public float unkn11;
    public float unkn12;
    public float unkn13;
    public float unkn14;
    public float unkn15;
    public float unkn16;
    public float unkn17;
    public float unkn18;
    public float unkn19;
    public byte unkn20;
}

[RszGenerate, RszAutoReadWrite, RszVersionedObject(typeof(EfxVersion)), EfxStruct(EfxAttributeType.UnknownDD2_250, EfxVersion.DD2)]
public partial class EFXAttributeUnknownDD2_250 : EFXAttribute
{
	public EFXAttributeUnknownDD2_250() : base(EfxAttributeType.UnknownDD2_250) { }
}
