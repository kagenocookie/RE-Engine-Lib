using System.Numerics;
using RszTool;
using RszTool.Efx.Structs.RE4;
using RszTool.Efx.Structs.RERT;
using RszTool.InternalAttributes;

namespace RszTool.Efx.Structs.RE7;


[RszGenerate, RszAutoReadWrite, RszVersionedObject(typeof(EfxVersion)), EfxStruct(EfxAttributeType.TypeRibbonChain, EfxVersion.RE7)]
public partial class EFXAttributeTypeRibbonChain : EFXAttribute
{
	public EFXAttributeTypeRibbonChain() : base(EfxAttributeType.TypeRibbonChain) { }

	public uint unkn1;
	public via.Color color0;
	public via.Color color1;
	public float unkn2_0;

	public UndeterminedFieldType null2_1;
	public UndeterminedFieldType null2_2;
	public float unkn2_3;
	public UndeterminedFieldType null2_4;
	public float unkn2_5;
	public UndeterminedFieldType null2_6;
	public float unkn2_7;
	public UndeterminedFieldType null2_8;
	public float unkn2_9;
    public float unkn2_10;

	public uint unkn2_11;
	public float unkn2_12;
	public UndeterminedFieldType null2_13;
	public float unkn2_14;
	public UndeterminedFieldType null2_15;
	public float unkn2_16;
	public UndeterminedFieldType null2_17;
	public uint unkn2_18;
	public UndeterminedFieldType null2_19;
	public UndeterminedFieldType null2_20;
	public float unkn2_21;
	[RszVersion(EfxVersion.RERT)]
	public float sb_unkn1;

    public UndeterminedFieldType null2_22;
	public UndeterminedFieldType null2_23;
	public UndeterminedFieldType null2_24;
}
