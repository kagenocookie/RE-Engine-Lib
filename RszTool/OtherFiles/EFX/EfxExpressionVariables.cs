using System.Numerics;
using RszTool;

namespace RszTool.Efx.Structs;

public abstract partial class EFXExpressionDataBase : BaseModel
{
}

public enum EFXExpressionParamType
{

}

[RszGenerate, RszAutoReadWrite]
public partial class EFXExpressionDataTypeFloat : EFXExpressionDataBase
{
    public float value;
}

[RszGenerate, RszAutoReadWrite]
public partial class EFXExpressionDataType1 : EFXExpressionDataBase
{
    public uint value;
}
[RszGenerate, RszAutoReadWrite]
public partial class EFXExpressionDataType2 : EFXExpressionDataBase
{
    public uint value;
}

[RszGenerate, RszAutoReadWrite]
public partial class EFXExpressionDataType3 : EFXExpressionDataBase
{
    public uint unkn1;
}

[RszGenerate, RszAutoReadWrite]
public partial class EFXExpressionDataTypeParameterHash : EFXExpressionDataBase
{
    public uint parameterHash;
}

[RszGenerate, RszAutoReadWrite]
public partial class EFXExpressionData : BaseModel
{
	public uint type;
	[RszSwitch(
		"type == 0", typeof(EFXExpressionDataTypeFloat),
		"type == 1", typeof(EFXExpressionDataType1),
		"type == 2", typeof(EFXExpressionDataType2),
		"type == 3", typeof(EFXExpressionDataType3),
		"type == 4", typeof(EFXExpressionDataTypeParameterHash)
	)]
	public EFXExpressionDataBase data = null!;
}

public struct ExpressionStruct_Param1 {
	public uint parameterNameHash;
	public uint unkn1;
	public uint unkn2;
}

/// <summary>
/// EFX expression tested for RE4 (works for everything except TypeMeshExpression / ID: 30)
/// </summary>
[RszGenerate, RszAutoReadWrite]
public partial class EFXExpression : BaseModel
{
	public uint struct1Count;// input parameters?
	public uint struct2Count;
	[RszFixedSizeArray(nameof(struct1Count))] public ExpressionStruct_Param1[]? struct1;

	[RszClassInstance, RszList(nameof(struct2Count))]
	public List<EFXExpressionData> struct2 = new();
}

/// <summary>
/// EFX expression tested for DD2 (seems mostly same as EFXExpression except a 3rd integer/count that's usually 0) TODO find non-0 exp3 case
/// </summary>
[RszGenerate, RszAutoReadWrite]
public partial class EFXExpression3 : BaseModel
{
	public uint struct1Count;
	public uint struct2Count;
	public uint struct3Count;
	[RszFixedSizeArray(nameof(struct1Count))] public ExpressionStruct_Param1[]? struct1;

	[RszClassInstance, RszList(nameof(struct2Count))]
	public List<EFXExpressionData> struct2 = new();
}

[RszGenerate, RszAutoReadWrite]
public abstract partial class EFXExpressionListWrapperBase : BaseModel
{
	public uint solverSize;
}

public partial class EFXExpressionListWrapper : EFXExpressionListWrapperBase
{
	public List<EFXExpression> expressions = new();

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
		expressions.Write(handler);
		handler.Write(Start, (uint)(handler.Tell() - Start));
		return true;
    }
}

[RszGenerate]
public partial class EFXExpressionListWrapper2 : EFXExpressionListWrapperBase
{
	public uint indexCount;
	[RszFixedSizeArray(nameof(solverSize), "/4")] public uint[]? unknData;
	[RszFixedSizeArray(nameof(indexCount))] public uint[]? data2;

    protected override bool DoRead(FileHandler handler)
    {
		return base.DoRead(handler) && DefaultRead(handler);
    }

    protected override bool DoWrite(FileHandler handler)
    {
		return base.DoWrite(handler) && DefaultWrite(handler);
    }
}

public partial class EFXExpressionListWrapper3 : EFXExpressionListWrapperBase
{
	public List<EFXExpression3> expressions = new();

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
		expressions.Write(handler);
		handler.Write(Start, (uint)(handler.Tell() - Start));
		return true;
    }
}
