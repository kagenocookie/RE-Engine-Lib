using System.Globalization;
using System.Text;

namespace ReeLib.Efx.Structs.Common;

public enum UnaryExpressionOperator
{
	Negation = 0,
}

public enum BinaryExpressionOperator
{
	Pow = 0,
	Mul = 1,
	Div = 2,
	Mod = 3,
	Add = 4,
	Sub = 5,
}

public enum EfxExpressionFunction
{
	Sin = 0,
	Cos = 1,
	Asin = 2,
	Acos = 3,
	Floor = 4,
	Ceil = 5,
	Log = 6,
	Log10 = 7,
	Exp = 8,
	Abs = 9,
	Saturate = 10,
	SinDeg = 11,
	CosDeg = 12,
	Lerp = 15,
	Clamp = 16,
	SmoothStep = 17,
	Min = 18,
	Max = 19,
	Pow = 20,
	Remap = 21,
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
			BinaryExpressionOperator.Mod => 2,
			BinaryExpressionOperator.Div => 3,
			BinaryExpressionOperator.Mul => 3,
			BinaryExpressionOperator.Pow => 4,
			_ => 5,
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
			case BinaryExpressionOperator.Pow:
			case BinaryExpressionOperator.Mod:
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

public class ExpressionFuncOperation : ExpressionAtom
{
	public EfxExpressionFunction func;
	public ExpressionAtom[] args = [];

    public override string ToString() => $"{func}({string.Join(", ", args.Select(x => x.ToString()))})";

    internal override void AppendString(StringBuilder sb)
    {
		sb.Append($"{func}(");
		for (int i = 0; i < args.Length; i++) {
			if (i != 0) sb.Append(", ");
			sb.Append(args[i]);
		}
		sb.Append(')');
    }
}

public class ExpressionRootValueOption : ExpressionAtom
{
	public ExpressionAtom value1 = ExpressionAtom.Null;
	public ExpressionAtom value2 = ExpressionAtom.Null;

    public ExpressionRootValueOption(ExpressionAtom value1, ExpressionAtom value2)
    {
        this.value1 = value1;
        this.value2 = value2;
    }

    public override string ToString() => value1 + "  |  " + value2;

    internal override void AppendString(StringBuilder sb) => sb.Append(ToString());
}