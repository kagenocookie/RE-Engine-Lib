using RszTool.Efx.Structs.Common;
using RszTool.InternalAttributes;

namespace RszTool.Efx.Structs.Unconfirmed;

[RszGenerate, RszAutoReadWrite, RszVersionedObject(typeof(EfxVersion)), EfxStruct(EfxAttributeType.RgbCommon, EfxVersion.Unknown)]
public partial class EFXAttributeRgbCommon : EFXAttribute
{
	public EFXAttributeRgbCommon() : base(EfxAttributeType.RgbCommon) { }

	[RszVersion(EfxVersion.RE4, EndAt = nameof(re4_unkn3))]
    public uint re4_unkn0;
    public uint re4_unkn1;
    public uint re4_unkn2;
    public uint re4_unkn3;

	public via.Color color0;
	public float unkn1_0;
	public float unkn1_1;
	public float unkn1_2;
	public via.Color unkn1_3;
	public float unkn1_4;
	public float unkn1_5;
	public float unkn1_6;
	public uint unkn1_7;
	public uint unkn1_8;
	public uint unkn1_9;
	public uint unkn1_10;
	public uint unkn1_11;
	public uint unkn1_12;
	public uint unkn1_13;
	public uint unkn1_14;
	public uint unkn1_15;
	public uint unkn1_16;
	public uint unkn1_17;
	public uint unkn1_18;
	public uint unkn1_19;
	public uint unkn1_20;
	public uint unkn1_21;

}
