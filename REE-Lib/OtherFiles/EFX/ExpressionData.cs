using ReeLib.Common;
using ReeLib.InternalAttributes;

namespace ReeLib.Efx.Structs.Common;

public abstract partial class EFXExpressionDataBase : BaseModel
{
}

public enum ExpressionComponentStorageType
{
	Float = 0,
	BinaryOperator = 1,
	UnaryOperator = 2,
	Function = 3,
	ParameterHash = 4,
}

[RszGenerate, RszAutoReadWrite]
public partial class EFXExpressionDataFloat : EFXExpressionDataBase
{
    public float value;
    public override string ToString() => value.ToString();
}

[RszGenerate, RszAutoReadWrite]
public partial class EFXExpressionDataBinaryOperator : EFXExpressionDataBase
{
    public BinaryExpressionOperator value;
    public override string ToString() => value.ToString();
}
[RszGenerate, RszAutoReadWrite]
public partial class EFXExpressionDataUnaryOperator : EFXExpressionDataBase
{
    public UnaryExpressionOperator value;
    public override string ToString() => value.ToString();
}
[RszGenerate, RszAutoReadWrite]
public partial class EFXExpressionDataFunction : EFXExpressionDataBase
{
    public EfxExpressionFunction value;
    public override string ToString() => value.ToString();
}

[RszGenerate, RszAutoReadWrite]
public partial class EFXExpressionDataParameterHash : EFXExpressionDataBase
{
    public uint parameterHash;
    public override string ToString() => parameterHash.ToString();
}

[RszGenerate, RszAutoReadWrite]
public partial class EFXExpressionData : BaseModel
{
	public ExpressionComponentStorageType type;
	[RszSwitch(
		nameof(type), "==", ExpressionComponentStorageType.Float, typeof(EFXExpressionDataFloat),
		nameof(type), "==", ExpressionComponentStorageType.BinaryOperator, typeof(EFXExpressionDataBinaryOperator),
		nameof(type), "==", ExpressionComponentStorageType.UnaryOperator, typeof(EFXExpressionDataUnaryOperator),
		nameof(type), "==", ExpressionComponentStorageType.Function, typeof(EFXExpressionDataFunction),
		nameof(type), "==", ExpressionComponentStorageType.ParameterHash, typeof(EFXExpressionDataParameterHash)
	)]
	public EFXExpressionDataBase data = null!;

    public EFXExpressionData(float value) {
		data = new EFXExpressionDataFloat() { value = value };
		type = ExpressionComponentStorageType.Float;
	}

    public EFXExpressionData(EFXExpressionDataBase data)
    {
        this.data = data;
		type = data switch {
			EFXExpressionDataFloat => ExpressionComponentStorageType.Float,
			EFXExpressionDataBinaryOperator => ExpressionComponentStorageType.BinaryOperator,
			EFXExpressionDataUnaryOperator => ExpressionComponentStorageType.UnaryOperator,
			EFXExpressionDataFunction => ExpressionComponentStorageType.Function,
			EFXExpressionDataParameterHash => ExpressionComponentStorageType.ParameterHash,
			_ => ExpressionComponentStorageType.Float,
		};
    }

    public EFXExpressionData()
    {
    }

    public override string ToString() => data.ToString() ?? nameof(EFXExpressionData);
}

public struct EFXExpressionParameterName
{
	public uint parameterNameHash;
	public float constantValue; // default value for External, constant value for Constant
	public ExpressionParameterSource source;

    public readonly string? GetName(IExpressionParameterSource paramSource)
    {
		switch (source) {
			case ExpressionParameterSource.Parameter:
				return paramSource.FindParameterByHash(parameterNameHash)?.name;
			case ExpressionParameterSource.External:
			case ExpressionParameterSource.Constant:
				return EfxExpressionTreeUtils.KnownExternalHashes.GetValueOrDefault(parameterNameHash);
			default:
				return null;
		}
    }

    public override string ToString() => source is ExpressionParameterSource.Constant or ExpressionParameterSource.External
		? $"{source}:{parameterNameHash}={constantValue}"
		: $"{source}:{parameterNameHash}";
}

[RszGenerate, RszVersionedObject(typeof(EfxVersion))]
public partial class EFXExpressionObject : BaseModel
{
	[RszIgnore] public EfxVersion Version;

	public EFXExpressionObject() { }
	public EFXExpressionObject(EfxVersion version) { Version = version; }

	[RszArraySizeField(nameof(parameters))] public int parameterCount;
	[RszArraySizeField(nameof(components))] public int componentsCount;

	// TODO: this seems to affect the struct somehow if != 0 (see MHWs 11_em_fallivy_03.efx); maybe it reads type info from the header parameter list directly?
	[RszVersion(EfxVersion.DD2)] public int struct3Count;

	[RszList(nameof(parameterCount))]
	public List<EFXExpressionParameterName>? parameters;

	[RszClassInstance, RszList(nameof(componentsCount))]
	public List<EFXExpressionData> components = new();

	[RszIgnore] public EFXExpressionTree? expressionTree;

	protected override bool DoRead(FileHandler handler)
	{
		return DefaultRead(handler);
	}

	protected override bool DoWrite(FileHandler handler)
	{
		return DefaultWrite(handler);
	}

	public List<EFXExpressionData> Components => components;
	public List<EFXExpressionParameterName> Parameters {
		get => parameters ??= new();
		set => parameters = value is List<EFXExpressionParameterName> arr ? arr : value?.ToList() ?? new();
	}

	public override EFXExpressionObject Clone()
	{
		var clone = new EFXExpressionObject(Version);
		clone.components.AddRange(components.Select(c => new EFXExpressionData() { data = (EFXExpressionDataBase)c.data.Clone(), type = c.type }));
		clone.parameters = parameters?.ToList();
		return clone;
    }
}

[RszGenerate, RszVersionedObject(typeof(EfxVersion))]
public partial class EFXMaterialExpression : EFXExpressionObject
{
    public EFXMaterialExpression() { }
    public EFXMaterialExpression(EfxVersion version) : base (version) { }

    public uint unkn1;
	public uint unkn2;
	public uint mdfPropertyHash;
	/// <summary>
	/// Represents which component of the value this expression targets (e.g. 0/1/2 for the X/Y/Z of a Vector3 property)
	/// </summary>
	public uint propertyComponentIndex;
	[RszVersion(EfxVersion.RE4)]
	public UndeterminedFieldType unkn5;

    protected override bool DoRead(FileHandler handler) => DefaultRead(handler) && base.DoRead(handler);
    protected override bool DoWrite(FileHandler handler) => DefaultWrite(handler) && base.DoWrite(handler);

	public static new IEnumerable<(string name, Type type)> GetFieldList(EfxVersion Version)
	{
		foreach (var field in GetFieldListDefault(Version)) {
			yield return field;
		}

		foreach (var basefield in EFXExpressionObject.GetFieldList(Version)) {
			yield return basefield;
		}
	}

	public override EFXMaterialExpression Clone()
	{
		var clone = new EFXMaterialExpression(Version);
		clone.unkn1 = unkn1;
		clone.unkn2 = unkn2;
		clone.mdfPropertyHash = mdfPropertyHash;
		clone.propertyComponentIndex = propertyComponentIndex;
		clone.unkn5 = unkn5;
		clone.components.AddRange(components.Select(c => new EFXExpressionData() { data = (EFXExpressionDataBase)c.data.Clone(), type = c.type }));
		clone.parameters = parameters?.ToList();
		return clone;
    }
}

[RszGenerate, RszAutoReadWrite]
public abstract partial class EFXExpressionContainer : BaseModel
{
	[RszByteSizeField("")] public uint solverSize;

	public List<EFXExpressionTree>? ParsedExpressions { get; set; }

	public abstract IEnumerable<EFXExpressionObject> Expressions { get; }
	public virtual int ExpressionCount => Expressions.Count();
	public abstract void AddExpression(EFXExpressionObject obj);

	public override EFXExpressionContainer Clone()
	{
		var clone = (EFXExpressionContainer)Activator.CreateInstance(GetType())!;
		clone.solverSize = solverSize;
		clone.ParsedExpressions = ParsedExpressions == null ? null : new List<EFXExpressionTree>(ParsedExpressions.Select(ex => new EFXExpressionTree() {
			parameters = ex.parameters.ToList(),
			root = ex.root.DeepClone<ExpressionAtom>()
		}));
		return clone;
    }
}

[RszGenerate]
public partial class EFXExpressionList : EFXExpressionContainer
{
	[RszIgnore] public EfxVersion Version;

	public EFXExpressionList() { }
	public EFXExpressionList(EfxVersion version) { Version = version; }

	[RszClassInstance, RszList] public List<EFXExpressionObject> expressions = new();

	public override int ExpressionCount => expressions.Count;
	public override IEnumerable<EFXExpressionObject> Expressions => expressions;
	public override void AddExpression(EFXExpressionObject obj) => expressions.Add(obj);

	protected override bool DoRead(FileHandler handler)
	{
		handler.Read(ref solverSize);
		var end = handler.Tell() + solverSize;
		while (handler.Tell() < end) {
			var expr = new EFXExpressionObject(Version);
			expr.Read(handler);
			expressions.Add(expr);
		}
		return true;
	}

	protected override bool DoWrite(FileHandler handler)
	{
		handler.Write(ref solverSize);
		expressions.Write(handler);
		handler.Write(Start, (uint)(handler.Tell() - Start) - 4);
		return true;
	}

	public override EFXExpressionList Clone()
	{
		var clone = (EFXExpressionList)base.Clone();
		clone.Version = Version;
		clone.expressions.AddRange(expressions.Select(e => e.Clone()));
		return clone;
    }
}

[RszGenerate]
public partial class EFXMaterialExpressionList : EFXExpressionContainer
{
	[RszArraySizeField(nameof(indices))] public int indexCount;
	[RszClassInstance, RszList(nameof(solverSize))]
	public List<EFXMaterialExpression> expressions = new();
	[RszFixedSizeArray(nameof(indexCount))] public uint[]? indices;

    public override int ExpressionCount => expressions.Count;
    public override IEnumerable<EFXExpressionObject> Expressions => expressions;
	public override void AddExpression(EFXExpressionObject obj) => expressions.Add((EFXMaterialExpression)obj);
	// convenience method to simplify codegen
	public int Length => expressions.Count;

	[RszIgnore] public EfxVersion Version;

    public EFXMaterialExpressionList() { }
    public EFXMaterialExpressionList(EfxVersion version) { Version = version; }

    protected override bool DoRead(FileHandler handler)
    {
		handler.Read(ref solverSize);
		handler.Read(ref indexCount);
		var end = handler.Tell() + solverSize;
		while (handler.Tell() < end) {
			var expr = new EFXMaterialExpression(Version);
			expr.Read(handler);
			expressions.Add(expr);
		}
		if (indices == null || indices.Length != indexCount) indices = new uint[indexCount];
		handler.ReadArray(indices);
		return true;
    }

    protected override bool DoWrite(FileHandler handler)
    {
		indices ??= Array.Empty<uint>();
		indexCount = indices.Length;

		handler.Write(ref solverSize);
		handler.Write(indexCount);
		var expressionStart = handler.Tell();
		expressions.Write(handler);
		handler.Write(Start, (uint)(handler.Tell() - expressionStart));
		handler.WriteArray(indices);
		return true;
    }

	public override EFXMaterialExpressionList Clone()
	{
		var clone = (EFXMaterialExpressionList)base.Clone();
		clone.Version = Version;
		clone.expressions.AddRange(expressions.Select(e => e.Clone()));
		return clone;
    }
}
