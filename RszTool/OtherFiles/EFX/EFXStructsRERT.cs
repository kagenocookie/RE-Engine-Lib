using RszTool.Efx.Structs.Common;
using RszTool.Efx.Structs.RE4;
using RszTool.InternalAttributes;

namespace RszTool.Efx.Structs.RERT;

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
