using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using RszTool.Common;

namespace RszTool.Efx.Structs.Common;

public static partial class EfxExpressionParser
{
	private ref struct ParseContext
	{
		public int position;
		public ReadOnlySpan<char> span;
		public TokenType nextToken;
		public IEnumerable<EFXExpressionParameterName> parameters;

		public ReadOnlySpan<char> GetSpan(in Token token) => span[token.start..token.end];
	}

	private readonly record struct Token(int start, int end, TokenType type);

	public static EFXExpressionTree Parse(ReadOnlySpan<char> expression, IEnumerable<EFXExpressionParameterName> parameters)
	{
		var ctx = new ParseContext() { span = expression, parameters = parameters };
		if (PredictToken(ref ctx) == TokenType.EOF) return new EFXExpressionTree();
		return new EFXExpressionTree() {
			root = ParseExpression(ref ctx),
			parameters = parameters.ToList(),
		};
	}

	private static ExpressionAtom ParseExpression(ref ParseContext ctx)
	{
		var next = ctx.nextToken = PredictToken(ref ctx);
		switch (next) {
			case TokenType.Float:
			case TokenType.Identifier:
			case TokenType.OpSub:
			case TokenType.ParenOpen:
				return ParseBinaryOperationAddSub(ref ctx);
			default:
				ThrowTokenError(ref ctx, next);
				return ExpressionAtom.Null;
		}
	}

    private static ExpressionAtom ParseBinaryOperationAddSub(ref ParseContext ctx)
    {
		var left = ParseBinaryOperationMulDiv(ref ctx);
        var op = ctx.nextToken;
		if (op is TokenType.OpAdd or TokenType.OpSub) {
			SkipToken(ref ctx);
        	var right = ParseBinaryOperationAddSub(ref ctx);
			return new ExpressionBinaryOperation() {
				left = left,
				oper = op switch {
					TokenType.OpAdd => BinaryExpressionOperator.Add,
					TokenType.OpSub => BinaryExpressionOperator.Sub,
					_ => throw new Exception("Unhandled token type " + op)
				},
				right = right,
			};
		} else {
			return left;
		}
    }

    private static ExpressionAtom ParseBinaryOperationMulDiv(ref ParseContext ctx)
    {
		var left = ParseUnary(ref ctx);
        var op = ctx.nextToken;
		if (op is TokenType.OpMul or TokenType.OpDiv) {
			SkipToken(ref ctx);
        	var right = ParseBinaryOperationMulDiv(ref ctx);
			return new ExpressionBinaryOperation() {
				left = left,
				oper = op switch {
					TokenType.OpMul => BinaryExpressionOperator.Mul,
					TokenType.OpDiv => BinaryExpressionOperator.Div,
					_ => throw new Exception("Unhandled token type " + op)
				},
				right = right,
			};
		} else {
			return left;
		}
    }

	private static ExpressionAtom ParseUnary(ref ParseContext ctx)
	{
		var tok = ctx.nextToken;
		if (tok == TokenType.OpSub) {
			SkipToken(ref ctx);
			return new ExpressionNegation() { atom = ParseValueAtom(ref ctx) };
		}
		return ParseValueAtom(ref ctx);
	}
	private static ExpressionAtom ParseValueAtom(ref ParseContext ctx)
	{
		var token = ReadTokenAndPredict(ref ctx);
		switch (token.type) {
			case TokenType.Float:
				return new ExpressionFloat() { value = float.Parse(ctx.GetSpan(token), CultureInfo.InvariantCulture) };
			case TokenType.Identifier:
				if (ctx.nextToken == TokenType.ParenOpen) {
					return ParseFunction(ref ctx, token);
				}
				var id = ParseIdentifier(ref ctx, token);
				return id;
			case TokenType.ParenOpen:
				var inner = ParseExpression(ref ctx);
				SkipToken(ref ctx, TokenType.ParenClosed);
				return inner;
		}
		ThrowTokenError(ref ctx, token.type, TokenType.Identifier, TokenType.Float);
		return ExpressionAtom.Null;
	}

    [DoesNotReturn]
	private static void ThrowTokenError(ref ParseContext ctx, TokenType token) {
		throw new Exception($"Found unexpected token {token} at position {ctx.position}");
	}

	[DoesNotReturn]
	private static void ThrowTokenError(ref ParseContext ctx, TokenType token, params TokenType[] expected) {
		throw new Exception($"Found unexpected token {token} at position {ctx.position}. Expected: {string.Join(", ", expected)}");
	}

	[DoesNotReturn]
	private static ExpressionAtom ThrowTokenError(in Token token, params TokenType[] expected) {
		throw new Exception($"Found unexpected token {token.type} at position {token.start}. Expected: {string.Join(", ", expected)}");
	}

	[DoesNotReturn]
	private static ExpressionAtom ThrowPositionedError(in Token token, string message) {
		throw new Exception($"{message} at position {token.start}");
	}

	private static void SkipToken(ref ParseContext ctx) => ReadTokenAndPredict(ref ctx);
	private static void SkipToken(ref ParseContext ctx, TokenType expected) {
		var token = ReadTokenAndPredict(ref ctx);
		if (token.type != expected) {
			ThrowTokenError(token, expected);
		}
	}

	private static ExpressionAtom ParseFloatOrIdentifier(ref ParseContext ctx, in Token token)
	{
		return token.type switch {
			TokenType.Float => new ExpressionFloat() { value = float.Parse(ctx.GetSpan(token)) },
			TokenType.Identifier => ParseIdentifier(ref ctx, token),
			_ => ThrowTokenError(token, TokenType.Float, TokenType.Identifier),
		};
	}

	private static ExpressionParameter ParseIdentifier(ref ParseContext ctx, in Token token)
	{
		var span = ctx.GetSpan(token);
		ExpressionParameterSource source;
		if (span.StartsWith("ext:")) {
			source = ExpressionParameterSource.External;
			span = span.Slice(4);
		} else if (span.StartsWith("const:")) {
			source = ExpressionParameterSource.Constant;
			span = span.Slice(6);
		} else if (span.StartsWith("p:")) {
			source = ExpressionParameterSource.Parameter;
			span = span.Slice(2);
		} else {
			source = ExpressionParameterSource.Unknown;
		}

		if (char.IsAsciiDigit(span[0])) {
			var param = new ExpressionParameter() { hash = uint.Parse(span), source = source };
			if (source == ExpressionParameterSource.Unknown) {
				param.source = ctx.parameters.GetParameterByHash(param.hash)?.source ?? throw new Exception("Unknown expression parameter hash " + param.hash);
			}
			return param;
		}

		var name = span.ToString();
		return new ExpressionParameter() {
			name = name,
			hash = MurMur3HashUtils.GetAsciiHash(name),
			source = source
		};
	}

	private static readonly Dictionary<int, int> functionArgCount = new() {
		[GetSpanHash(nameof(EfxExpressionFunction.Unary0))] = 1,
		[GetSpanHash(nameof(EfxExpressionFunction.Unary1))] = 1,
		[GetSpanHash(nameof(EfxExpressionFunction.Unary2))] = 1,
		[GetSpanHash(nameof(EfxExpressionFunction.Unary4))] = 1,
		[GetSpanHash(nameof(EfxExpressionFunction.Unary5))] = 1,
		[GetSpanHash(nameof(EfxExpressionFunction.Unary6))] = 1,
		[GetSpanHash(nameof(EfxExpressionFunction.Unary7))] = 1,
		[GetSpanHash(nameof(EfxExpressionFunction.Unary8))] = 1,
		[GetSpanHash(nameof(EfxExpressionFunction.Unary9))] = 1,
		[GetSpanHash(nameof(EfxExpressionFunction.Unary10))] = 1,

		[GetSpanHash(nameof(BinaryExpressionOperator.Min))] = 2,
		[GetSpanHash(nameof(BinaryExpressionOperator.Max))] = 2,

		[GetSpanHash(nameof(EfxExpressionFunction.Lerp))] = 3,
		[GetSpanHash(nameof(EfxExpressionFunction.InvLerp))] = 3,
		[GetSpanHash(nameof(EfxExpressionFunction.Clamp))] = 3,
	};
    private static int GetSpanHash(ReadOnlySpan<char> span)
    {
        return CultureInfo.InvariantCulture.CompareInfo.GetHashCode(span, CompareOptions.Ordinal);
    }

	private static ExpressionAtom ParseFunction(ref ParseContext ctx, in Token idToken)
	{
		SkipToken(ref ctx, TokenType.ParenOpen);

		var nameSpan = ctx.GetSpan(idToken);
		var nameHash = GetSpanHash(nameSpan);
		if (!functionArgCount.TryGetValue(nameHash, out var args)) {
			ThrowPositionedError(idToken, "Unknown EFX function " + nameSpan.ToString());
			return ExpressionAtom.Null;
		}

		ExpressionAtom? result = null;
		if (args == 1) {
			result = new ExpressionUnaryOperation() {
				func = EnumHelper.Parse<EfxExpressionFunction>(nameSpan, true),
				atom = ParseExpression(ref ctx),
			};
		} else if (args == 2) {
			var bin = new ExpressionBinaryOperation() { oper = EnumHelper.Parse<BinaryExpressionOperator>(nameSpan, true) };
			bin.left = ParseExpression(ref ctx);
			SkipToken(ref ctx, TokenType.Comma);
			bin.right = ParseExpression(ref ctx);
			result = bin;
		} else if (args == 3) {
			var ter = new ExpressionTernaryOperation() { func = EnumHelper.Parse<EfxExpressionFunction>(nameSpan, true) };
			ter.left = ParseExpression(ref ctx);
			SkipToken(ref ctx, TokenType.Comma);
			ter.arg2 = ParseExpression(ref ctx);
			SkipToken(ref ctx, TokenType.Comma);
			ter.arg3 = ParseExpression(ref ctx);
			result = ter;
		}

		if (result == null) {
			ThrowPositionedError(idToken, "Unknown EFX function " + nameSpan.ToString());
		}

		SkipToken(ref ctx, TokenType.ParenClosed);
		return result;
	}

	private static TokenType PredictToken(ref ParseContext ctx)
	{
		if (ctx.position >= ctx.span.Length) return TokenType.EOF;

		var ch = ctx.span[ctx.position];
		// skip whitespace
		while (ch is ' ' or '\t' or '\r' or '\n') ch = ctx.span[++ctx.position];

		switch (ch) {
			case '(': return TokenType.ParenOpen;
			case ')': return TokenType.ParenClosed;
			case '+': return TokenType.OpAdd;
			case '-': return TokenType.OpSub;
			case '/': return TokenType.OpDiv;
			case '*': return TokenType.OpMul;
			case ',': return TokenType.Comma;
			case '0':
			case '1':
			case '2':
			case '3':
			case '4':
			case '5':
			case '6':
			case '7':
			case '8':
			case '9': return TokenType.Float;
			default: return TokenType.Identifier;
		}
	}

	private static Token ReadTokenAndPredict(ref ParseContext ctx)
	{
		var tok = ReadToken(ref ctx);
		ctx.nextToken = PredictToken(ref ctx);
		return tok;
	}

	private static Token ReadToken(ref ParseContext ctx)
	{
		if (ctx.position >= ctx.span.Length) throw new Exception("Expression reached end of file");

		var ch = ctx.span[ctx.position];
		// skip whitespace
		while (ch is ' ' or '\t' or '\r' or '\n') ch = ctx.span[++ctx.position];

		switch (ch) {
			case '(': return new Token(ctx.position, ++ctx.position, TokenType.ParenOpen);
			case ')': return new Token(ctx.position, ++ctx.position, TokenType.ParenClosed);
			case '+': return new Token(ctx.position, ++ctx.position, TokenType.OpAdd);
			case '-': return new Token(ctx.position, ++ctx.position, TokenType.OpSub);
			case '/': return new Token(ctx.position, ++ctx.position, TokenType.OpDiv);
			case '*': return new Token(ctx.position, ++ctx.position, TokenType.OpMul);
			case ',': return new Token(ctx.position, ++ctx.position, TokenType.Comma);
			case '0':
			case '1':
			case '2':
			case '3':
			case '4':
			case '5':
			case '6':
			case '7':
			case '8':
			case '9': return ReadFloat(ref ctx);
			default: return ReadIdentifier(ref ctx);
		}
	}

	private static Token ReadFloat(ref ParseContext ctx)
	{
		var start = ctx.position;
		var end = start;
		var ch = ctx.span[start];
		var foundDot = false;
		do {
			foundDot = foundDot || ch == '.';
			end = end + 1;
			if (end >= ctx.span.Length) break;
			ch = ctx.span[end];
		} while (ch >= '0' && ch <= '9' || !foundDot && ch == '.');

		ctx.position = end;
		return new Token(start, end, TokenType.Float);
	}

	private static Token ReadIdentifier(ref ParseContext ctx)
	{
		var start = ctx.position;
		var end = start;
		var ch = ctx.span[start];

		do {
			end = end + 1;
			if (end >= ctx.span.Length) break;
			ch = ctx.span[end];
		} while (ch >= '0' && ch <= '9' || ch >= 'a' && ch <= 'z' || ch >= 'A' && ch <= 'Z' || ch == '_' || ch == ':');

		ctx.position = end;
		return new Token(start, end, TokenType.Identifier);
	}

	private enum TokenType
	{
		EOF,
		Float,
		Identifier,
		OpAdd,
		OpSub,
		OpMul,
		OpDiv,
		ParenOpen,
		ParenClosed,
		Comma,
	}
}
