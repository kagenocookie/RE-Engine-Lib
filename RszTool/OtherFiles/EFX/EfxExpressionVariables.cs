using RszTool.InternalAttributes;

namespace RszTool.Efx.Structs.Common;

public abstract partial class EFXExpressionDataBase : BaseModel
{
}

[RszGenerate, RszAutoReadWrite]
public partial class EFXExpressionDataFloat : EFXExpressionDataBase
{
    public float value;
}

[RszGenerate, RszAutoReadWrite]
public partial class EFXExpressionDataBinaryOperator : EFXExpressionDataBase
{
    public BinaryExpressionOperator value;
}
[RszGenerate, RszAutoReadWrite]
public partial class EFXExpressionDataUnaryOperator : EFXExpressionDataBase
{
	/// <summary>
	/// Value is always 0 here - most likely a `-value` operator
	/// </summary>
    public uint value;
}
[RszGenerate, RszAutoReadWrite]
public partial class EFXExpressionDataFunction : EFXExpressionDataBase
{
    public EfxExpressionFunction value;
}

[RszGenerate, RszAutoReadWrite]
public partial class EFXExpressionDataParameterHash : EFXExpressionDataBase
{
    public uint parameterHash;
}

[RszGenerate, RszAutoReadWrite]
public partial class EFXExpressionData : BaseModel
{
	public uint type;
	[RszSwitch(
		"type == 0", typeof(EFXExpressionDataFloat),
		"type == 1", typeof(EFXExpressionDataBinaryOperator),
		"type == 2", typeof(EFXExpressionDataUnaryOperator),
		"type == 3", typeof(EFXExpressionDataFunction),
		"type == 4", typeof(EFXExpressionDataParameterHash)
	)]
	public EFXExpressionDataBase data = null!;
}

public struct EFXExpressionParameterName
{
	public uint parameterNameHash;
	public float constantValue;
	public ExpressionParameterSource source;
}

/// <summary>
/// EFX expression tested for RE4 (works for everything except TypeMeshExpression / ID: 30)
/// </summary>
[RszGenerate, RszAutoReadWrite]
public partial class EFXExpression : BaseModel, IExpressionObject
{
	[RszArraySizeField(nameof(parameters))] public int parameterCount;
	[RszArraySizeField(nameof(components))] public int componentsCount;
	[RszFixedSizeArray(nameof(parameterCount))] public EFXExpressionParameterName[]? parameters;

	[RszClassInstance, RszList(nameof(componentsCount))]
	public List<EFXExpressionData> components = new();

    public IList<EFXExpressionData> Components => components;
    public IEnumerable<EFXExpressionParameterName> Parameters {
		get => parameters ?? Array.Empty<EFXExpressionParameterName>();
		set => parameters = value is EFXExpressionParameterName[] arr ? arr : value?.ToArray() ?? Array.Empty<EFXExpressionParameterName>();
	}
}

/// <summary>
/// DD2+
/// </summary>
[RszGenerate, RszAutoReadWrite]
public partial class EFXExpression2 : BaseModel, IExpressionObject
{
    public int uknCount;
	[RszArraySizeField(nameof(parameters))] public int parameterCount;
	[RszArraySizeField(nameof(components))] public int componentsCount;
	public uint struct3Count;
	[RszFixedSizeArray(nameof(parameterCount))] public EFXExpressionParameterName[]? parameters;

	[RszClassInstance, RszList(nameof(componentsCount))]
	public List<EFXExpressionData> components = new();

    public IList<EFXExpressionData> Components => components;
    public IEnumerable<EFXExpressionParameterName> Parameters {
		get => parameters ?? Array.Empty<EFXExpressionParameterName>();
		set => parameters = value is EFXExpressionParameterName[] arr ? arr : value?.ToArray() ?? Array.Empty<EFXExpressionParameterName>();
	}
}

/// <summary>
/// EFX expression tested for DD2 (seems mostly same as EFXExpression except a 3rd integer/count that's usually 0) TODO find non-0 exp3 case
/// </summary>
[RszGenerate, RszAutoReadWrite]
public partial class EFXExpression3 : BaseModel, IExpressionObject
{
	[RszArraySizeField(nameof(parameters))] public int parameterCount;
	[RszArraySizeField(nameof(components))] public int componentsCount;
	public uint struct3Count;
	[RszFixedSizeArray(nameof(parameterCount))] public EFXExpressionParameterName[]? parameters;

	[RszClassInstance, RszList(nameof(componentsCount))]
	public List<EFXExpressionData> components = new();

    public IList<EFXExpressionData> Components => components;
    public IEnumerable<EFXExpressionParameterName> Parameters {
		get => parameters ?? Array.Empty<EFXExpressionParameterName>();
		set => parameters = value is EFXExpressionParameterName[] arr ? arr : value?.ToArray() ?? Array.Empty<EFXExpressionParameterName>();
	}
}

/// <summary>
/// pre-DD2
/// </summary>
[RszGenerate, RszAutoReadWrite, RszVersionedObject(typeof(EfxVersion))]
public partial class EFXMaterialExpression1 : BaseModel, IExpressionObject
{
	[RszIgnore] public EfxVersion Version;

    public EFXMaterialExpression1() { }
    public EFXMaterialExpression1(EfxVersion version) { Version = version; }

    public uint unkn1;
	public uint unkn2;
	public uint hash3;
	public uint unkn4;
	[RszVersion(EfxVersion.RE4)]
	public UndeterminedFieldType unkn5;
	[RszClassInstance] public EFXExpression? expression;

    public IList<EFXExpressionData> Components => expression?.Components ?? Array.Empty<EFXExpressionData>();
    public IEnumerable<EFXExpressionParameterName> Parameters {
		get => expression?.Parameters ?? Array.Empty<EFXExpressionParameterName>();
		set => (expression ??= new()).Parameters = value;
	}
}

/// <summary>
/// DD2+
/// </summary>
[RszGenerate, RszAutoReadWrite]
public partial class EFXMaterialExpression2 : BaseModel, IExpressionObject
{
	public uint unkn1;
	public uint unkn2;
	public uint hash3;
	public uint unkn4;
	[RszClassInstance] public EFXExpression2? expression;

    public IList<EFXExpressionData> Components => expression?.Components ?? Array.Empty<EFXExpressionData>();
    public IEnumerable<EFXExpressionParameterName> Parameters {
		get => expression?.Parameters ?? Array.Empty<EFXExpressionParameterName>();
		set => (expression ??= new()).Parameters = value;
	}
}

[RszGenerate, RszAutoReadWrite]
public abstract partial class EFXExpressionContainer : BaseModel
{
	[RszByteSizeField("")] public uint solverSize;

	public List<EFXExpressionTree>? ParsedExpressions { get; set; }

    public abstract IEnumerable<IExpressionObject> Expressions { get; }
    public virtual int ExpressionCount => Expressions.Count();
}

[RszGenerate]
public partial class EFXExpressionListWrapper1 : EFXExpressionContainer
{
	[RszClassInstance, RszList] public List<EFXExpression> expressions = new();

    public override int ExpressionCount => expressions.Count;
    public override IEnumerable<IExpressionObject> Expressions => expressions;

    protected override bool DoRead(FileHandler handler)
    {
		handler.Read(ref solverSize);
		var end = handler.Tell() + solverSize;
		while (handler.Tell() < end) {
			var expr = new EFXExpression();
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
}

[RszGenerate]
public partial class EFXExpressionListWrapper2 : EFXExpressionContainer
{
	[RszClassInstance, RszList(nameof(solverSize))]
	public List<EFXExpression3> expressions = new();

    public override int ExpressionCount => expressions.Count;
    public override IEnumerable<IExpressionObject> Expressions => expressions;

    protected override bool DoRead(FileHandler handler)
    {
		handler.Read(ref solverSize);
		var end = handler.Tell() + solverSize;
		while (handler.Tell() < end) {
			var expr = new EFXExpression3();
			expr.Read(handler);
			expressions.Add(expr);
		}
		return true;
    }

    protected override bool DoWrite(FileHandler handler)
    {
		handler.Write(ref solverSize);
		expressions.Write(handler);
		handler.Write(Start, (uint)(handler.Tell() - Start));
		return true;
    }
}

[RszGenerate]
public partial class EFXMaterialExpressionListWrapper1 : EFXExpressionContainer
{
	[RszArraySizeField(nameof(data2))] public int indexCount;
	[RszClassInstance, RszList(nameof(solverSize))]
	public List<EFXMaterialExpression1> expressions = new();
	[RszFixedSizeArray(nameof(indexCount))] public uint[]? data2;

    public override int ExpressionCount => expressions.Count;
    public override IEnumerable<IExpressionObject> Expressions => expressions;

	[RszIgnore] public EfxVersion Version;

    public EFXMaterialExpressionListWrapper1() { }
    public EFXMaterialExpressionListWrapper1(EfxVersion version) { Version = version; }

    protected override bool DoRead(FileHandler handler)
    {
		handler.Read(ref solverSize);
		handler.Read(ref indexCount);
		var end = handler.Tell() + solverSize;
		while (handler.Tell() < end) {
			var expr = new EFXMaterialExpression1(Version);
			expr.Read(handler);
			expressions.Add(expr);
		}
		if (data2 == null || data2.Length != indexCount) data2 = new uint[indexCount];
		handler.ReadArray(data2);
		return true;
    }

    protected override bool DoWrite(FileHandler handler)
    {
		handler.Write(ref solverSize);
		handler.Write(data2?.Length ?? 0);
		expressions.Write(handler);
		handler.Write(Start, (uint)(handler.Tell() - Start));
		return true;
    }
}

[RszGenerate]
public partial class EFXMaterialExpressionListWrapper2 : EFXExpressionContainer
{
	[RszArraySizeField(nameof(data2))] public int indexCount;
	[RszClassInstance, RszList(nameof(solverSize))]
	public List<EFXMaterialExpression2> expressions = new();
	[RszFixedSizeArray(nameof(indexCount))] public uint[]? data2;

    public override int ExpressionCount => expressions.Count;
    public override IEnumerable<IExpressionObject> Expressions => expressions;

	// version not needed here yet, added for consistency with the ListWrapper1
    public EFXMaterialExpressionListWrapper2() { }
    public EFXMaterialExpressionListWrapper2(EfxVersion version) {  }

    protected override bool DoRead(FileHandler handler)
    {
		handler.Read(ref solverSize);
		handler.Read(ref indexCount);
		var end = handler.Tell() + solverSize;
		while (handler.Tell() < end) {
			var expr = new EFXMaterialExpression2();
			expr.Read(handler);
			expressions.Add(expr);
		}
		if (data2 == null || data2.Length != indexCount) data2 = new uint[indexCount];
		handler.ReadArray(data2);
		return true;
    }

    protected override bool DoWrite(FileHandler handler)
    {
		handler.Write(ref solverSize);
		handler.Write(data2?.Length ?? 0);
		expressions.Write(handler);
		handler.Write(Start, (uint)(handler.Tell() - Start));
		return true;
    }
}
