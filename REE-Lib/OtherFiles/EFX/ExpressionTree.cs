using System.Globalization;
using System.Text;

namespace ReeLib.Efx.Structs.Common;

public enum UnaryExpressionOperator
{
	Negation = 0,
}

/// <summary>
/// Am not 100% sure on the exact operators for 1-4 but they seem reasonable.
/// </summary>
public enum BinaryExpressionOperator
{
	Max = 0,
	Add = 1,
	Sub = 2,
	Mul = 3,
	Div = 4,
	Min = 5,
}

public enum EfxExpressionFunction
{
	// unary potential candidates: sin/cos/tan/atan2/inverse/reciprocial/sqrt/pow2/pow3/root3/exp/abs/ceil/floor/clamp01/log
	// When changing any of these names, also add the previous name to EfxExpressionParser.functionArgCount for compatiblity
	Unary0 = 0,
	Unary1 = 1,
	Unary2 = 2,
	Unary4 = 4,
	Unary5 = 5,
	Unary6 = 6,
	Unary7 = 7,
	Unary8 = 8,
	Unary9 = 9,
	Unary10 = 10,
	Lerp = 15,
	InvLerp = 16,
	Clamp = 17,
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
	public List<EFXExpressionParameterName> parameters = new();

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

    private sealed class ExpressionNull : ExpressionAtom
	{
        public override string ToString() => "";
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

    public override string ToString()
	{
		var str = value.ToString("F6", CultureInfo.InvariantCulture).TrimEnd('0');
		if (str.EndsWith('.')) return str.Substring(0, str.Length - 1);
		return str;
	}

    internal override void AppendString(StringBuilder sb) => sb.Append(ToString());
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

    public override string ToString() => oper switch {
        BinaryExpressionOperator.Add => $"({left} + {right})",
        BinaryExpressionOperator.Sub => $"({left} - {right})",
        BinaryExpressionOperator.Mul => $"({left} * {right})",
        BinaryExpressionOperator.Div => $"({left} / {right})",
        _ => $"{oper}({left}, {right})",
    };

	public static int GetPrecedence(BinaryExpressionOperator op)
	{
		return op switch {
			BinaryExpressionOperator.Add => 1,
			BinaryExpressionOperator.Sub => 1,
			BinaryExpressionOperator.Mul => 2,
			BinaryExpressionOperator.Div => 2,
			BinaryExpressionOperator.Max => 3,
			BinaryExpressionOperator.Min => 3,
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
				sb.Append(oper switch {
					BinaryExpressionOperator.Add => "+",
					BinaryExpressionOperator.Sub => "-",
					BinaryExpressionOperator.Mul => "*",
					BinaryExpressionOperator.Div => "/",
					_ => "?"
				});
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
			case BinaryExpressionOperator.Min:
			case BinaryExpressionOperator.Max:
				sb.Append(oper);
				sb.Append('(');
				left.AppendString(sb);
				sb.Append(", ");
				right.AppendString(sb);
				sb.Append(')');
				break;
			default:
				throw new Exception("Unsupported binary operator " + oper);
		}
    }
}

public class ExpressionUnaryOperation : ExpressionAtom
{
	public EfxExpressionFunction func;
	public ExpressionAtom atom = ExpressionAtom.Null;

    public override string ToString() => $"{func}({atom})";

    internal override void AppendString(StringBuilder sb)
    {
		sb.Append(func).Append('(');
		atom.AppendString(sb);
		sb.Append(')');
    }
}
public class ExpressionTernaryOperation : ExpressionAtom
{
	public EfxExpressionFunction func;
	public ExpressionAtom left = ExpressionAtom.Null;
	public ExpressionAtom arg2 = ExpressionAtom.Null;
	public ExpressionAtom arg3 = ExpressionAtom.Null;

    public override string ToString() => $"{func}({left}, {arg2}, {arg3})";

    internal override void AppendString(StringBuilder sb)
    {
		sb.Append($"{func}(");
		left.AppendString(sb);
		sb.Append(", ");
		arg2.AppendString(sb);
		sb.Append(", ");
		arg3.AppendString(sb);
		sb.Append(')');
    }
}
