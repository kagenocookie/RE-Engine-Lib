using RszTool.InternalAttributes;

namespace RszTool.Efx.Structs.Common;

public enum ExpressionAssignType
{
	Add = 0,
	Subtract = 1,
	Multiply = 2,
	Divide = 3,
	Assign = 4,
	ForceWord = -1
}

public enum MaterialParameterType : short
{
	None = 0,
	Float = 1, // data structure: float min, float max
	Range = 2, // data structure: float min_a, float min_b, float max_a, float max_b
	Texture = 3, // data structure: 4B (path len) 4B (texture index) 4B (unknown int) 4B (padding); mdfPropertyIndex = -1
}

[RszGenerate, RszAutoReadWrite, RszVersionedObject(typeof(EfxVersion))]
public partial class MdfProperty : BaseModel
{
    [RszIgnore] public EfxVersion Version;

    public MdfProperty() { }
    public MdfProperty(EfxVersion version)
    {
        Version = version;
    }

    public uint PropertyNameUTF8Hash;
    [RszVersion(EfxVersion.RE3)]
	public int mdfPropertyIndex;
    public ushort parameterCount;
	public MaterialParameterType parameterType;
    [RszVersion("<", EfxVersion.RE3)]
	public int ukn1_2;

    [RszVersion(EfxVersion.RE3)]
	public int unkn2;

	[RszFixedSizeArray(16)] public byte[]? propertyValue;

    public static int Size(EfxVersion version) => version >= EfxVersion.RE3 ? 32 : 28;
}

[RszGenerate, RszAutoReadWrite, RszVersionedObject(typeof(EfxVersion))]
public partial class EfxMaterialStructBase : BaseModel
{
    [RszIgnore] public EfxVersion Version;

    public EfxMaterialStructBase() { }
    public EfxMaterialStructBase(EfxVersion version)
    {
        Version = version;
    }
}

[RszGenerate, RszAutoReadWrite, RszVersionedObject(typeof(EfxVersion))]
public partial class EfxMaterialStructV1 : EfxMaterialStructBase
{
    public EfxMaterialStructV1() { }
    public EfxMaterialStructV1(EfxVersion version) : base(version) {}

	public int maxPropertyIndex;
	public int ukn1;
    [RszArraySizeField(nameof(properties))] public int propertyCount;
	[RszArraySizeField(nameof(texPaths))] public int texCount;
    public int ukn2;
	[RszStringLengthField(nameof(mdfPath))] public int mdfPathLength;
    [RszStringLengthField(nameof(mmtrPath))] public int mmtrPathLength;
	public int texBlockSize;

    [RszClassInstance, RszList(nameof(propertyCount)), RszConstructorParams(nameof(Version))]
    public List<MdfProperty> properties = new();
    [RszInlineWString(nameof(mdfPathLength))] public string? mdfPath;
    [RszInlineWString(nameof(mmtrPathLength))] public string? mmtrPath;
	[RszInlineWString, RszList(nameof(texCount))] public string[]? texPaths;
}

[RszGenerate, RszAutoReadWrite, RszVersionedObject(typeof(EfxVersion))]
public partial class EfxMaterialStructV2 : EfxMaterialStructBase
{
    public EfxMaterialStructV2() { }
    public EfxMaterialStructV2(EfxVersion version) : base(version) { }

	public uint ukn1;
	[RszVersion(EfxVersion.MHWilds)] public float mhws_unkn;
    [RszArraySizeField(nameof(properties))] public int propertyCount;
	[RszArraySizeField(nameof(texPaths))] public int texCount;
	public uint ukn2;
    public uint ukn3;
	public uint ukn4;
	[RszInlineWString] public string? mdfPath;
	[RszInlineWString] public string? mmtrPath;
	[RszByteSizeField(nameof(properties))] public uint propDataSize;

    [RszClassInstance, RszList(nameof(propertyCount)), RszConstructorParams(nameof(Version))]
    public List<MdfProperty> properties = new();
	public uint texBlockSize;
	[RszInlineWString, RszList(nameof(texCount))] public string[]? texPaths;
}

public struct ByteSet
{
    public byte b1;
    public byte b2;
    public byte b3;
    public byte b4;

    public override string ToString() => $"{b1} {b2} {b3} {b4}";
}

public struct SByteSet
{
    public sbyte b1;
    public sbyte b2;
    public sbyte b3;
    public sbyte b4;

    public override string ToString() => $"{b1} {b2} {b3} {b4}";
}
