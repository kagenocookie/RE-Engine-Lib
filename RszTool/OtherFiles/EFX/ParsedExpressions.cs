using System.Globalization;
using System.Numerics;
using RszTool;
using RszTool.InternalAttributes;

namespace RszTool.Efx.Structs.Common;

/// <summary>
/// Am not 100% sure on the exact operators for 1-4 but they seem reasonable.
/// </summary>
public enum BinaryExpressionOperator
{
	Add = 1,
	Sub = 2,
	Mul = 3,
	Div = 4,
	ClampMax = 5,
	UknSix = 6,
}

public enum EfxExpressionFunction
{
	Unary0 = 0, // unary potential candidates: sin/cos/tan/atan2/inverse/reciprocial
	Unary1 = 1,
	Unary2 = 2,
	Unary4 = 4,
	Unary5 = 5,
	Unary6 = 6,
	Unary7 = 7,
	Unary8 = 8,
	Unary9 = 9,
	Unary10 = 10,
	Ternary15 = 15, // ternary candidates: lerp, clamp
	Ternary16 = 16,
	Ternary17 = 17,
}

public class EFXExpressionTree
{
	public ExpressionAtom root = ExpressionAtom.Null;

    public override string ToString() => root.ToString() ?? "~~~";
}

public abstract class ExpressionAtom
{
	public static readonly ExpressionAtom Null = new ExpressionNull();

	private class ExpressionNull : ExpressionAtom
	{
        public override string ToString() => "---";
	}
}

public class ExpressionParameter : ExpressionAtom
{
	public uint hash;
	public string? name;

    public override string ToString() => name == null ? $"[ext:{hash}]" : name;
}
public class ExpressionFloat : ExpressionAtom
{
	public float value;

    public override string ToString() => value.ToString(CultureInfo.InvariantCulture);
}
public class ExpressionInt : ExpressionAtom
{
	public int value;

    public override string ToString() => value.ToString(CultureInfo.InvariantCulture);
}
public class ExpressionUnaryOperation : ExpressionAtom
{
	public int oper;
	public ExpressionAtom atom = ExpressionAtom.Null;

    public override string ToString() => $"Unary{oper}({atom})";
}
public class ExpressionNegation : ExpressionAtom
{
	public ExpressionAtom atom = ExpressionAtom.Null;

    public override string ToString() => $"(-({atom}))";
}
public class ExpressionBinaryOperation : ExpressionAtom
{
	public BinaryExpressionOperator oper;
	public ExpressionAtom left = ExpressionAtom.Null;
	public ExpressionAtom right = ExpressionAtom.Null;

	public static string OperatorToSign(BinaryExpressionOperator op) => op switch {
		BinaryExpressionOperator.Add => "+",
		BinaryExpressionOperator.Sub => "-",
		BinaryExpressionOperator.Mul => "*",
		BinaryExpressionOperator.Div => "/",
		_ => "??"
	};

    public override string ToString() => oper switch {
        BinaryExpressionOperator.Add => $"({left} + {right})",
        BinaryExpressionOperator.Sub => $"({left} - {right})",
        BinaryExpressionOperator.Mul => $"({left} * {right})",
        BinaryExpressionOperator.Div => $"({left} / {right})",
        _ => $"Min({left}, {right})",
    };
}
public class ExpressionMultiOperation : ExpressionAtom
{
	public int oper;
	public List<ExpressionAtom> args = new();

    public override string ToString() => $"({string.Join(" ", args)})";
}

public class ExpressionTernaryOperation : ExpressionAtom
{
	public EfxExpressionFunction oper;
	public ExpressionAtom left = ExpressionAtom.Null;
	public ExpressionAtom arg2 = ExpressionAtom.Null;
	public ExpressionAtom arg3 = ExpressionAtom.Null;

    public override string ToString() => $"Func{(int)oper}({left}, {arg2}, {arg3})";
}
