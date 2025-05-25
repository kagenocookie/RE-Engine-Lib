using System.Globalization;
using System.Text;

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

public enum ExpressionParameterSource
{
	Unknown = -1,
	Parameter = 0,
	Constant = 1,
	External = 2,
}

public class EFXExpressionTree
{
	public ExpressionAtom root = ExpressionAtom.Null;

	public override string ToString()
	{
		var sb = new StringBuilder();
		root.AppendString(sb);
		return sb.ToString();
	}
}

public abstract class ExpressionAtom
{
	public static readonly ExpressionAtom Null = new ExpressionNull();

    internal virtual void AppendString(StringBuilder sb)
    {
        sb.Append(ToString());
    }

    private class ExpressionNull : ExpressionAtom
	{
        public override string ToString() => "---";
	}
}

public class ExpressionParameter : ExpressionAtom
{
	public uint hash;
	public ExpressionParameterSource source;
	public string? name;

    public override string ToString()
	{
		switch (source) {
			case ExpressionParameterSource.Parameter:
        		return name == null ? $"p:{hash}" : name;
			case ExpressionParameterSource.External:
        		return name == null ? "ext:" + hash : name;
			case ExpressionParameterSource.Constant:
        		return name == null ? "const:" + hash : name;
			default:
        		return name == null ? "ukn:" + hash : name;
		}
	}

    internal override void AppendString(StringBuilder sb) => sb.Append(ToString());
}

public class ExpressionFloat : ExpressionAtom
{
	public float value;

    public override string ToString() => value.ToString(CultureInfo.InvariantCulture);

    internal override void AppendString(StringBuilder sb) => sb.Append(value);
}
public class ExpressionUnaryOperation : ExpressionAtom
{
	public int oper;
	public ExpressionAtom atom = ExpressionAtom.Null;

    public override string ToString() => $"Unary{oper}({atom})";

    internal override void AppendString(StringBuilder sb)
    {
		sb.Append("Unary").Append(oper).Append('(');
		atom.AppendString(sb);
		sb.Append(')');
    }
}
public class ExpressionNegation : ExpressionAtom
{
	public ExpressionAtom atom = ExpressionAtom.Null;

    public override string ToString() => atom is ExpressionFloat or ExpressionParameter ? $"-{atom}" : $"-({atom})";

    internal override void AppendString(StringBuilder sb)
    {
		if (atom is ExpressionFloat or ExpressionParameter) {
			sb.Append('-');
			atom.AppendString(sb);
		} else {
			sb.Append("-(");
			atom.AppendString(sb);
			sb.Append(')');
		}
    }
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

	public static int GetPrecedence(BinaryExpressionOperator op)
	{
		return op switch {
			BinaryExpressionOperator.Add => 1,
			BinaryExpressionOperator.Sub => 1,
			BinaryExpressionOperator.Mul => 2,
			BinaryExpressionOperator.Div => 2,
			BinaryExpressionOperator.ClampMax => 3,
			_ => 4,
		};
	}

    internal override void AppendString(StringBuilder sb)
    {
		switch (oper) {
			case BinaryExpressionOperator.Add:
			case BinaryExpressionOperator.Sub:
			case BinaryExpressionOperator.Div:
			case BinaryExpressionOperator.Mul:
				var p0 = GetPrecedence(oper);
				if (left is ExpressionBinaryOperation l) {
					var p1 = GetPrecedence(l.oper);
					if (p1 >= p0) {
						l.AppendString(sb);
					} else {
						sb.Append('(');
						l.AppendString(sb);
						sb.Append(')');
					}
				} else {
					left.AppendString(sb);
				}
				sb.Append(' ');
				sb.Append(OperatorToSign(oper));
				sb.Append(' ');
				if (right is ExpressionBinaryOperation r) {
					var p2 = GetPrecedence(r.oper);
					if (p2 >= p0) {
						r.AppendString(sb);
					} else {
						sb.Append('(');
						r.AppendString(sb);
						sb.Append(')');
					}
				} else {
					right.AppendString(sb);
				}

				break;
			case BinaryExpressionOperator.ClampMax:
				sb.Append("Min(");
				left.AppendString(sb);
				sb.Append(", ");
				right.AppendString(sb);
				sb.Append(')');
				break;
			default:
				sb.Append($"Binary{(int)oper}(");
				left.AppendString(sb);
				sb.Append(", ");
				right.AppendString(sb);
				sb.Append(')');
				break;
		}
    }
}

public class ExpressionTernaryOperation : ExpressionAtom
{
	public EfxExpressionFunction oper;
	public ExpressionAtom left = ExpressionAtom.Null;
	public ExpressionAtom arg2 = ExpressionAtom.Null;
	public ExpressionAtom arg3 = ExpressionAtom.Null;

    public override string ToString() => $"Func{(int)oper}({left}, {arg2}, {arg3})";

    internal override void AppendString(StringBuilder sb)
    {
		sb.Append($"Func{(int)oper}(");
		left.AppendString(sb);
		sb.Append(", ");
		arg2.AppendString(sb);
		sb.Append(", ");
		arg3.AppendString(sb);
		sb.Append(')');
    }
}
