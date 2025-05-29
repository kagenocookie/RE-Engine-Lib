using System.Numerics;
using RszTool.Efx.Structs.Common;
using RszTool.InternalAttributes;

namespace RszTool.Efx.Structs.Pt;

public enum PtBehaviorPropType
{
    PropFloat = 9,
    PropFloat2 = 19,
    PropFloat3 = 11,
    PropRange = 10,
    PropUint = 4,
    PropInt = 14,
    PropPrefabpath = 17,
    PropWstring = 21,
    PropEnum = 18,
    PropColor = 15,
}

[RszGenerate, RszAutoReadWrite, RszVersionedObject(typeof(EfxVersion))]
public partial class PtBehaviorVariableDataBase : BaseModel
{
	[RszIgnore] public EfxVersion Version;

	public PtBehaviorVariableDataBase(EfxVersion version) { Version = version; }

	public int unkn;
	public int size;
	[RszVersion(EfxVersion.RE4, EndAt = nameof(re4_unkn1))]
	public short re4_unkn0;
	public short re4_unkn1;

	protected bool EnsureMinimumSize(int minSize)
	{
		if (size < minSize) size = minSize;
		return true;
	}
}

[RszGenerate, RszVersionedObject(typeof(EfxVersion))]
public partial class PtBehaviorVariableDataColor : PtBehaviorVariableDataBase
{
	public PtBehaviorVariableDataColor(EfxVersion version) : base(version) { }

	public via.Color color;
	[RszFixedSizeArray(nameof(size), - 4)] public byte[]? restData;

    protected override bool DoRead(FileHandler handler) => base.DoRead(handler) && DefaultRead(handler);
    protected override bool DoWrite(FileHandler handler) => EnsureMinimumSize(4) && base.DoWrite(handler) && DefaultWrite(handler);
}

[RszGenerate, RszVersionedObject(typeof(EfxVersion))]
public partial class PtBehaviorVariableInteger : PtBehaviorVariableDataBase
{
	public PtBehaviorVariableInteger(EfxVersion version) : base(version) { }

	public int value;
	[RszFixedSizeArray(nameof(size), - 4)] public byte[]? restData;

    protected override bool DoRead(FileHandler handler) => base.DoRead(handler) && DefaultRead(handler);
    protected override bool DoWrite(FileHandler handler) => EnsureMinimumSize(4) && base.DoWrite(handler) && DefaultWrite(handler);
}
[RszGenerate, RszVersionedObject(typeof(EfxVersion))]
public partial class PtBehaviorVariableFloat : PtBehaviorVariableDataBase
{
	public PtBehaviorVariableFloat(EfxVersion version) : base(version) { }

	public float value;
	[RszFixedSizeArray(nameof(size), - 4)] public byte[]? restData;

    protected override bool DoRead(FileHandler handler) => base.DoRead(handler) && DefaultRead(handler);
    protected override bool DoWrite(FileHandler handler) => EnsureMinimumSize(4) && base.DoWrite(handler) && DefaultWrite(handler);
}
[RszGenerate, RszVersionedObject(typeof(EfxVersion))]
public partial class PtBehaviorVariableFloat2 : PtBehaviorVariableDataBase
{
	public PtBehaviorVariableFloat2(EfxVersion version) : base(version) { }

	public Vector2 Vec;
	[RszFixedSizeArray(nameof(size), - 8)] public byte[]? restData;

    protected override bool DoRead(FileHandler handler) => base.DoRead(handler) && DefaultRead(handler);
    protected override bool DoWrite(FileHandler handler) => EnsureMinimumSize(8) && base.DoWrite(handler) && DefaultWrite(handler);
}
[RszGenerate, RszVersionedObject(typeof(EfxVersion))]
public partial class PtBehaviorVariableFloat3 : PtBehaviorVariableDataBase
{
	public PtBehaviorVariableFloat3(EfxVersion version) : base(version) { }

	public Vector3 vec;
	[RszFixedSizeArray(nameof(size), - 12)] public byte[]? restData;

    protected override bool DoRead(FileHandler handler) => base.DoRead(handler) && DefaultRead(handler);
    protected override bool DoWrite(FileHandler handler) => EnsureMinimumSize(12) && base.DoWrite(handler) && DefaultWrite(handler);
}

[RszGenerate, RszVersionedObject(typeof(EfxVersion))]
public partial class PtBehaviorVariableDataPrefabPath : PtBehaviorVariableDataBase
{
	public PtBehaviorVariableDataPrefabPath(EfxVersion version) : base(version) { }

	[RszInlineWString(nameof(size))] public string? prefabPath;

    protected override bool DoRead(FileHandler handler) => base.DoRead(handler) && DefaultRead(handler);
    protected override bool DoWrite(FileHandler handler) => EnsureMinimumSize(prefabPath?.Length ?? 2) && base.DoWrite(handler) && DefaultWrite(handler);
}

[RszGenerate, RszVersionedObject(typeof(EfxVersion))]
public partial class PtBehaviorVariableDataPrefabUnknown : PtBehaviorVariableDataBase
{
	public PtBehaviorVariableDataPrefabUnknown(EfxVersion version) : base(version) { }

	[RszFixedSizeArray(nameof(size))] public byte[]? data;

    protected override bool DoRead(FileHandler handler) => base.DoRead(handler) && DefaultRead(handler);
    protected override bool DoWrite(FileHandler handler) => EnsureMinimumSize(data?.Length ?? 0) && base.DoWrite(handler) && DefaultWrite(handler);
}

[RszGenerate, RszVersionedObject(typeof(EfxVersion))]
public partial class PtBehaviorVariable : BaseModel
{
	[RszIgnore] public EfxVersion Version;

	public PtBehaviorVariable(EfxVersion version) { Version = version; }

	public int varSize;
	public PtBehaviorPropType dataType;
	// public uint unkn2;
	// public int dataSize;
	// [RszVersion(EfxVersion.RE4, EndAt = nameof(re4_unkn1))]
	// public short re4_unkn0;
	// public short re4_unkn1;

	// TODO - possibly replaces re4_unkn0, re4_unkn1
	// if(varID == 0x0F){
	//     spos[i] = FTell();
	//     i++;
	//     via.Color small_color;
	//     char var[varSize-16];// <name="Variable", bgcolor=0xA3BECC, open=suppress>;
	// }else{
	//     char  variable[varSize-12];// <name="Vairable", bgcolor=0xD6D6D6, open=suppress>;
	// }
	[RszClassInstance, RszConstructorParams(nameof(Version)), RszSwitch(
		nameof(dataType), "==", PtBehaviorPropType.PropColor, typeof(PtBehaviorVariableDataColor),
		nameof(dataType), "==", PtBehaviorPropType.PropPrefabpath, typeof(PtBehaviorVariableDataPrefabPath),
		nameof(dataType), "==", PtBehaviorPropType.PropInt, typeof(PtBehaviorVariableInteger),
		nameof(dataType), "==", PtBehaviorPropType.PropEnum, typeof(PtBehaviorVariableInteger),
		nameof(dataType), "==", PtBehaviorPropType.PropFloat, typeof(PtBehaviorVariableFloat),
		nameof(dataType), "==", PtBehaviorPropType.PropFloat2, typeof(PtBehaviorVariableFloat2),
		nameof(dataType), "==", PtBehaviorPropType.PropFloat3, typeof(PtBehaviorVariableFloat3),
		typeof(PtBehaviorVariableDataPrefabUnknown)
	)]
	public PtBehaviorVariableDataBase? variable;

	[RszInlineString(-1)] public string? behaviorProperty;

	protected override bool DoRead(FileHandler handler)
	{
		DefaultRead(handler);
		return true;
	}

	protected override bool DoWrite(FileHandler handler)
	{
		DefaultWrite(handler);
		return true;
	}
}

[RszGenerate, RszAutoReadWrite, RszVersionedObject(typeof(EfxVersion)), EfxStruct(EfxAttributeType.PtBehavior, EfxVersion.RE4)]
public partial class EFXAttributePtBehavior : EFXAttribute
{
    public EFXAttributePtBehavior() : base(EfxAttributeType.PtBehavior) { }

    public uint unkn1;
    [RszStringLengthField(nameof(behaviorString))] public int behaviorStringLength;
    [RszArraySizeField(nameof(properties))] public int varCount;
    [RszInlineString(nameof(behaviorStringLength))]
	public string? behaviorString;

	[RszClassInstance, RszList(nameof(varCount)), RszConstructorParams(nameof(Version))]
	public List<PtBehaviorVariable> properties = new();
}


[RszGenerate, RszAutoReadWrite, RszVersionedObject(typeof(EfxVersion)), EfxStruct(EfxAttributeType.PtColliderAction, EfxVersion.DMC5, EfxVersion.RE4)]
public partial class EFXAttributePtColliderAction : EFXAttribute
{
    public EFXAttributePtColliderAction() : base(EfxAttributeType.PtColliderAction) { }

    public uint dataFlags;

    public float unkn1;
    public float unkn2;
    public uint linkedAction;

	// found in RE2RT
	[RszConditional("(dataFlags & (1 << 10)) != 0", EndAt = nameof(ukn_flag2_11))]
	public uint ukn_flag2_0; // likely some sort of shape type + data
    public float ukn_flag2_2;
    public float ukn_flag2_3;
    public float ukn_flag2_4;
    public float ukn_flag2_5;
	public uint ukn_flag2_6;
    public float ukn_flag2_7;
    public float ukn_flag2_8;
    public float ukn_flag2_9;
    public float ukn_flag2_10;
    public float ukn_flag2_11;

	[RszVersion(EfxVersion.DD2, EndAt = nameof(dd2_unkn2))]
	public uint dd2_unkn0;
	public uint dd2_unkn1;
	public uint dd2_unkn2;

	[RszConditional("(dataFlags & (1 << 1)) != 0"), RszInlineWString]
	public string? unknString_flag2;

	[RszConditional("(dataFlags & (1 << 0)) != 0"), RszInlineWString]
	public string? unknString_flag1;
}

[RszGenerate, RszAutoReadWrite, RszVersionedObject(typeof(EfxVersion)), EfxStruct(EfxAttributeType.PtCollision, EfxVersion.RE7, EfxVersion.DMC5, EfxVersion.RE3, EfxVersion.RE4, EfxVersion.DD2)]
public partial class EFXAttributePtCollision : EFXAttribute
{
    public EFXAttributePtCollision() : base(EfxAttributeType.PtCollision) { }

    public byte stringBitFlag;//<format=binary,comment = "bitflag, determines whether there will be strings read">;
    public byte unkn0_1;
    public byte unkn0_2;
    public byte unkn0_3;
    public float unkn1;
    public float unkn2;
    public uint unkn3;
    public uint unkn4;
    public float unkn5;
    public float unkn6;
    public float unkn7;
    public float unkn8;
    public uint unkn9;
	[RszVersion('>', EfxVersion.RE7, EndAt = nameof(unkn16))]
    public float unkn10;
    public float unkn11;
    public float unkn12;
    public float unkn13;
	[RszVersion('>', EfxVersion.RE3, EndAt = nameof(unkn16))]
    public float unkn14;
	[RszVersion('>', EfxVersion.RERT, EndAt = nameof(unkn16))]
    public uint unkn15;
    public float unkn16;
	[RszConditional("(stringBitFlag & (1 << 0)) != 0"), RszInlineWString]
	public string? unknString0;

	[RszConditional("(stringBitFlag & (1 << 1)) != 0"), RszInlineWString]
	public string? unknString1;
}


[RszGenerate, RszAutoReadWrite, RszVersionedObject(typeof(EfxVersion)), EfxStruct(EfxAttributeType.PtColor, EfxVersion.RE7, EfxVersion.RE2, EfxVersion.DMC5, EfxVersion.RE3, EfxVersion.RE8, EfxVersion.RERT, EfxVersion.RE4)]
public partial class EFXAttributePtColor : EFXAttribute
{
	public EFXAttributePtColor() : base(EfxAttributeType.PtColor) { }

	public uint unkn1;
	public uint unkn2;
	public via.Color color0;
}

[RszGenerate, RszAutoReadWrite, RszVersionedObject(typeof(EfxVersion)), EfxStruct(EfxAttributeType.PtColorClip, EfxVersion.DMC5)]
public partial class EFXAttributePtColorClip : RszTool.Efx.EFXAttribute, IColorClipAttribute
{
    public EfxColorClipData Clip => clipData;
    public BitSet ClipBits => clipBits;

    public EFXAttributePtColorClip() : base(EfxAttributeType.PtColorClip) { }

    /// <summary>
    /// This flag tells us which color channels have a clip:
    /// 1 = R, 2 = G, 4 = B, 8 = A
    /// </summary>
    // public uint colorClipBits;
	[RszClassInstance] public readonly BitSet clipBits = new BitSet(4) { BitNames = ["R", "G", "B", "A"] };
    public uint unkn1;
	[RszClassInstance] public EfxColorClipData clipData = new();

    public override string ToString() => $"PtColorClip: {clipBits}";
}

[RszGenerate, RszAutoReadWrite, RszVersionedObject(typeof(EfxVersion)), EfxStruct(EfxAttributeType.PtLife, EfxVersion.RE7, EfxVersion.RE2, EfxVersion.DMC5, EfxVersion.RE3, EfxVersion.RE8, EfxVersion.RERT, EfxVersion.RE4)]
public partial class EFXAttributePtLife : EFXAttribute
{
	public EFXAttributePtLife() : base(EfxAttributeType.PtLife) { }

	public uint unkn1_0;
	public uint unkn1_1;
	public int actionIndex;
}

[RszGenerate, RszAutoReadWrite, RszVersionedObject(typeof(EfxVersion)), EfxStruct(EfxAttributeType.PtSort, EfxVersion.RE8, EfxVersion.RE4, EfxVersion.DD2)]
public partial class EFXAttributePtSort : EFXAttribute
{
	public EFXAttributePtSort() : base(EfxAttributeType.PtSort) { }

	public uint unkn;
}

[RszGenerate, RszAutoReadWrite, RszVersionedObject(typeof(EfxVersion)), EfxStruct(EfxAttributeType.PtUvSequence, EfxVersion.DMC5, EfxVersion.RE3, EfxVersion.RE4)]
public partial class EFXAttributePtUvSequence : EFXAttribute
{
	public EFXAttributePtUvSequence() : base(EfxAttributeType.PtUvSequence) { }

	public uint unkn1_0;
	public uint unkn1_1;
	[RszVersion("<=", EfxVersion.RE3)]
	public uint unkn1_2;
	public float unkn1_3;
}

[RszGenerate, RszAutoReadWrite, RszVersionedObject(typeof(EfxVersion)), EfxStruct(EfxAttributeType.PtUvSequenceClip, EfxVersion.DMC5, EfxVersion.RE4)]
public partial class EFXAttributePtUvSequenceClip : EFXAttribute, IClipAttribute
{
    public EfxClipData Clip => clipData;
    public BitSet ClipBits => clipBits;

    public EFXAttributePtUvSequenceClip() : base(EfxAttributeType.PtUvSequenceClip) { }

	[RszClassInstance] public readonly BitSet clipBits = new BitSet(3);
    uint unkn1;
	[RszClassInstance] public EfxClipData clipData = new();
}
