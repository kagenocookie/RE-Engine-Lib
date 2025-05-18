using RszTool.Common;
using RszTool.Efx;
using RszTool.Efx.Structs.RE4;

namespace RszTool;

public class EfxStructCache
{
    /// <summary>
    /// All the base attribute object types.
    /// </summary>
    public Dictionary<string, EfxStructInfo> AttributeTypes { get; set; } = new();
    /// <summary>
    /// Nested object / struct types.
    /// </summary>
    public Dictionary<string, EfxStructInfo> Structs { get; set; } = new();
    /// <summary>
    /// Known enums used by any EFX objects.
    /// </summary>
    public Dictionary<string, SignedEnumData> Enums { get; set; } = new();
}

public record SignedEnumData(SignedEnumItem[] Items, bool Flags, string BackingType);
public record SignedEnumItem(string Name, long Value);

public class EfxStructInfo
{
    /// <summary>
    /// The ID this struct is serialized to for the game.
    /// </summary>
    public int TypeID { get; set; }
    /// <summary>
    /// The name of the EFX struct type.
    /// </summary>
    public string Name { get; set; } = string.Empty;
    /// <summary>
    /// Full classname of the RszTool struct for this object.
    /// </summary>
    public string Classname { get; set; } = string.Empty;
    /// <summary>
    /// A hash value of the containing fields.
    /// </summary>
    public uint Hash { get; set; }
    /// <summary>
    /// List of fields that this struct contains.
    /// </summary>
    public List<EfxFieldInfo> Fields { get; set; } = new();
}

public class EfxFieldInfo
{
    public string Name { get; set; } = string.Empty;
    public RszFieldType FieldType { get; set; }
    public string? Classname { get; set; }
    public bool IsArray { get; set; }

    public EfxFieldFlags Flag { get; set; }
    public string? FlagTarget { get; set; }
}

public enum EfxFieldFlags
{
    None = 0,
    /// <summary>
    /// Field contains the UTF16 murmur3 hash of a field.
    /// </summary>
    UTF16StringHash = 1,
    /// <summary>
    /// Field contains the UTF8 murmur3 hash of a field.
    /// </summary>
    UTF8StringHash = 2,
    /// <summary>
    /// Field contains the ASCII murmur3 hash of a field.
    /// </summary>
    AsciiStringHash = 3,
    /// <summary>
    /// Field contains the UTF8 murmur3 hash of a property name.
    /// </summary>
    PropertyNameUTF8Hash = 4,
    /// <summary>
    /// Field contains the length of a string.
    /// </summary>
    StringLength = 5,
    /// <summary>
    /// Field contains the size or length of another field / struct.
    /// </summary>
    StructSize = 6,
    /// <summary>
    /// Field is an embedded EFX file.
    /// </summary>
    EmbeddedEFX = 7,
}

public struct UndeterminedFieldType
{
	public int value;

	public UndeterminedFieldType() { }
	public UndeterminedFieldType(int value)
	{
		this.value = value;
	}
	public UndeterminedFieldType(uint value)
	{
		this.value = (int)value;
	}
	public UndeterminedFieldType(float value)
	{
		this.value = MemoryUtils.SingleToInt32(value);
	}

	public string GetMostLikelyValueTypeString()
	{
		static bool LooksLikeFloat(int n) => BitConverter.Int32BitsToSingle(n) is float f && Math.Abs(f) > 0.00001f && Math.Abs(f) < 10000f;

		if (value == 0) return "0";
		if (value > 0 && value < 10000) {
			return value.ToString();
		}
		if (LooksLikeFloat(value)) {
			return BitConverter.Int32BitsToSingle(value).ToString("0.0#");
		}
		return ToString();
	}

	public override string ToString() => $"{value} {value.ToString("X")} {MemoryUtils.Int32ToSingle(value).ToString("0.0#", System.Globalization.CultureInfo.InvariantCulture)}";
}
