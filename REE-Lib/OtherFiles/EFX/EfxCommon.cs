using System.Numerics;
using System.Runtime.InteropServices;
using ReeLib.Common;
using ReeLib.InternalAttributes;

namespace ReeLib.Efx.Structs.Common;

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
	Float = 1, // data structure: float min, float max OR color float4
	Range = 2, // data structure: float min_a, float min_b, float max_a, float max_b
	Texture = 3,
}

[RszGenerate, RszVersionedObject(typeof(EfxVersion))]
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
    public ushort mdfParameterValueCount;
	public MaterialParameterType parameterType;
	public int flags;
	[RszFixedSizeArray(16)] private byte[] rawValue = new byte[16];

    [RszIgnore] public string? texturePath;

    public object Value {
        get => parameterType switch {
            MaterialParameterType.Texture => TextureValue,
            _ => VectorValue,
        };
        set {
            switch (parameterType) {
                case MaterialParameterType.Texture:
                    TextureValue = (MdfPropertyTextureValue)value;
                    break;
                default:
                    VectorValue = (Vector4)value;
                    break;
            }
        }
    }

	public Vector4 VectorValue {
        get => MemoryMarshal.Read<Vector4>(rawValue);
        set => MemoryMarshal.Write<Vector4>(rawValue, value);
    }

	public ReeLib.via.Range RangeValue {
        get => MemoryMarshal.Read<ReeLib.via.Range>(rawValue);
        set => MemoryMarshal.Write<ReeLib.via.Range>(rawValue, value);
    }

	public MdfPropertyTextureValue TextureValue {
        get => MemoryMarshal.Read<MdfPropertyTextureValue>(rawValue);
        set => MemoryMarshal.Write<MdfPropertyTextureValue>(rawValue, value);
    }

    public string? Name => HashToName(PropertyNameUTF8Hash);

    public static int GetSize(EfxVersion version) => version >= EfxVersion.RE3 ? 32 : 28;

    public override string ToString() => $"[{HashToName(PropertyNameUTF8Hash)}] {parameterType}";

    public static Dictionary<uint, string> ParameterNameHashes = new() {
        { 3072153074, "BaseMap" },
    };

    public static string HashToName(uint hash) => ParameterNameHashes.TryGetValue(hash, out var str) ? str : hash.ToString();

    protected override bool DoRead(FileHandler handler)
    {
        return DefaultRead(handler);
    }

    protected override bool DoWrite(FileHandler handler)
    {
        if (parameterType == MaterialParameterType.Texture) {
            mdfPropertyIndex = -1;
        }
        return DefaultWrite(handler);
    }
}

public struct MdfPropertyTextureValue
{
    public int pathLength;
    public int textureIndex;
    public int uknInt;
    public int _padding;

    public override string ToString() => $"Texture {textureIndex} (len={pathLength})";
}

[RszGenerate, RszVersionedObject(typeof(EfxVersion))]
public abstract partial class EfxMaterialStructBase : BaseModel
{
    [RszIgnore] public EfxVersion Version;

    public EfxMaterialStructBase() { }
    public EfxMaterialStructBase(EfxVersion version)
    {
        Version = version;
    }
}

[RszGenerate, RszVersionedObject(typeof(EfxVersion))]
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

    public override string ToString() => $"Material: {mdfPath}";

    protected override bool DoRead(FileHandler handler)
    {
        DefaultRead(handler);
        foreach (var p in properties) {
            if (p.parameterType == MaterialParameterType.Texture && p.TextureValue.textureIndex < texPaths?.Length) {
                p.texturePath = texPaths![p.TextureValue.textureIndex];
            }
        }
        return true;
    }

    protected override bool DoWrite(FileHandler handler)
    {
        texPaths = properties.Where(p => p.parameterType == MaterialParameterType.Texture).Select(p => p.texturePath ??= "").ToArray();
        texBlockSize = texPaths.Length == 0 ? 0 : texPaths.Sum(p => p.Length + 1) * 2;
        foreach (var p in properties) {
            if (p.parameterType == MaterialParameterType.Texture) {
                p.TextureValue = p.TextureValue with { pathLength = (p.texturePath!).Length + 1, textureIndex = texPaths.IndexOf(p.texturePath) };
            }
        }
        return DefaultWrite(handler);
    }
}

[RszGenerate, RszVersionedObject(typeof(EfxVersion))]
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
	[RszInlineWString(ByteSize = true)] public string? mdfPath;
	[RszInlineWString(ByteSize = true)] public string? mmtrPath;
	[RszByteSizeField(nameof(properties))] public uint propDataSize;

    [RszClassInstance, RszList(nameof(propertyCount)), RszConstructorParams(nameof(Version))]
    public List<MdfProperty> properties = new();
	public int texBlockSize;
	[RszInlineWString, RszList(nameof(texCount))] public string[]? texPaths;

    public override string ToString() => $"Material: {mdfPath}";

    protected override bool DoRead(FileHandler handler)
    {
        DefaultRead(handler);
        foreach (var p in properties) {
            if (p.parameterType == MaterialParameterType.Texture && p.TextureValue.textureIndex < texPaths?.Length) {
                p.texturePath = texPaths![p.TextureValue.textureIndex];
            }
        }
        return true;
    }

    protected override bool DoWrite(FileHandler handler)
    {
        texPaths = properties.Where(p => p.parameterType == MaterialParameterType.Texture).Select(p => p.texturePath ??= "").ToArray();
        texBlockSize = texPaths.Length == 0 ? 0 : texPaths.Sum(p => p.Length + 1) * 2;
        foreach (var p in properties) {
            if (p.parameterType == MaterialParameterType.Texture) {
                p.TextureValue = p.TextureValue with { pathLength = (p.texturePath!).Length + 1, textureIndex = texPaths.IndexOf(p.texturePath) };
            }
        }
        return DefaultWrite(handler);
    }
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
