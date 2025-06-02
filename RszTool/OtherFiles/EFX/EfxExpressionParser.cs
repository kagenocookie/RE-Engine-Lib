using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using RszTool.Common;

namespace RszTool.Efx.Structs.Common;

public static partial class EfxExpressionStringParser
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

		var root = ParseExpression(ref ctx);
		var usedParams = new List<EFXExpressionParameterName>();
		StoreNewParameters(ref ctx, root, usedParams);

		return new EFXExpressionTree() {
			root = root,
			parameters = usedParams.Select(p => parameters.GetParameterByHash(p.parameterNameHash) ?? p).ToList(),
		};
	}

	private static void StoreNewParameters(ref ParseContext ctx, ExpressionAtom item, List<EFXExpressionParameterName> usedParams)
	{
		switch (item) {
			case ExpressionBinaryOperation bin:
				StoreNewParameters(ref ctx, bin.left, usedParams);
				StoreNewParameters(ref ctx, bin.right, usedParams);
				break;
			case ExpressionUnaryOperation unary:
				StoreNewParameters(ref ctx, unary.atom, usedParams);
				break;
			case ExpressionNegation neg:
				StoreNewParameters(ref ctx, neg.atom, usedParams);
				break;
			case ExpressionTernaryOperation tert:
				StoreNewParameters(ref ctx, tert.left, usedParams);
				StoreNewParameters(ref ctx, tert.arg2, usedParams);
				StoreNewParameters(ref ctx, tert.arg3, usedParams);
				break;
			case ExpressionParameter param:
				var existing = usedParams.GetParameterByHash(param.hash);
				if (existing == null) {
					usedParams.Add(new EFXExpressionParameterName() {
						source = param.source == ExpressionParameterSource.Unknown ? ExpressionParameterSource.External : param.source,
						parameterNameHash = param.hash,
					});
				}
				break;
		}
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
			// ctx.parameters.Add(new EFXExpressionParameterName);
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
		while (ch is ' ' or '\t' or '\r' or '\n') {
			if (ctx.position >= ctx.span.Length - 1) return TokenType.EOF;
			ch = ctx.span[++ctx.position];
		}

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
		while (ch is ' ' or '\t' or '\r' or '\n') {
			if (ctx.position >= ctx.span.Length - 1) throw new Exception("Expression reached end of file");
			ch = ctx.span[++ctx.position];
		}

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

public static class EfxExpressionTreeUtils
{
	public static readonly HashSet<uint> UnknownParameterHashes = new();
	private static readonly Dictionary<uint, string> KnownExternalHashes = new() {
		[4068760923] = "PI",
		[2715150528] = "speed",
		[4102950055] = "Speed",
		[3351123696] = "scale",
		[2589222962] = "TIMER",
		[124194757] = "alpha",
		[1124845316] = "Color",
		[2521345197] = "color",
		[1703575233] = "pitch",
		[3888230594] = "yaw",
		[2703807999] = "roll",

		[1484922460] = "ColorRange",
		[3710676630] = "Piece_Color",
		[3720841511] = "int_num",
		[480772615] = "Wide",
		[1042548107] = "LightShadowRatio",
		[4001584140] = "BackFaceLightRatio",
		[4053326860] = "SpawnNum",

		// still unresolved hashes:
		// [1737174073] = "???",
		// [2031344731] = "???",
		// [2451941081] = "???",
		// [3433402344] = "???",
		// [1351678141] = "???",
		// [1017435601] = "???",
		// [302732036] = "???",
	};

	public static void FlattenExpressions(List<EFXExpressionData> list, EFXExpressionTree tree, IExpressionParameterSource paramSource)
		=> FlattenExpression(list, tree.parameters, tree.root, tree, paramSource);

	private static void FlattenExpression(List<EFXExpressionData> components, List<EFXExpressionParameterName> parameters, ExpressionAtom item, EFXExpressionTree tree, IExpressionParameterSource paramSource)
	{
		if (item is ExpressionFloat flt) {
			components.Add(new EFXExpressionData(flt.value));
			return;
		}
		if (item is ExpressionParameter param) {
			components.Add(new EFXExpressionData(new EFXExpressionDataParameterHash() {
				parameterHash = param.hash,
			}));
			var existing = tree.parameters.GetParameterByHash(param.hash);
			if (existing == null) {
				var definition = paramSource.FindParameterByHash(param.hash);
				tree.parameters.Add(new EFXExpressionParameterName() {
					parameterNameHash = param.hash,
					source = definition != null ? ExpressionParameterSource.Parameter : ExpressionParameterSource.External,
				});
			}
			return;
		}
		if (item is ExpressionNegation neg) {
			FlattenExpression(components, parameters, neg.atom, tree, paramSource);
			components.Add(new EFXExpressionData(new EFXExpressionDataUnaryOperator()));
			return;
		}
		if (item is ExpressionUnaryOperation unary) {
			FlattenExpression(components, parameters, unary.atom, tree, paramSource);
			components.Add(new EFXExpressionData(new EFXExpressionDataFunction() { value = unary.func }));
			return;
		}
		if (item is ExpressionBinaryOperation binary) {
			FlattenExpression(components, parameters, binary.right, tree, paramSource);
			FlattenExpression(components, parameters, binary.left, tree, paramSource);
			components.Add(new EFXExpressionData(new EFXExpressionDataBinaryOperator() { value = binary.oper }));
			return;
		}
		if (item is ExpressionTernaryOperation ternary) {
			FlattenExpression(components, parameters, ternary.arg3, tree, paramSource);
			FlattenExpression(components, parameters, ternary.arg2, tree, paramSource);
			FlattenExpression(components, parameters, ternary.left, tree, paramSource);
			components.Add(new EFXExpressionData(new EFXExpressionDataFunction() { value = ternary.func }));
			return;
		}
		throw new ArgumentException("Unknown expression for reserialize: " + item.GetType().FullName);
	}

	public static List<EFXExpressionTree> ReconstructExpressionTreeList(IEnumerable<EFXExpressionObject> expressions, IExpressionParameterSource paramSource)
	{
		var list = new List<EFXExpressionTree>();
		foreach (var expr in expressions) {
			var tree = ReconstructExpressionTree(expr, paramSource);
			list.Add(tree);
		}
		return list;
	}

	public static EFXExpressionTree ReconstructExpressionTree(EFXExpressionObject expression, IExpressionParameterSource paramSource)
	{
		var tree = new EFXExpressionTree();
		var comps = expression.Components;
		if (comps.Count == 0) return tree;

		var i = comps.Count - 1;
		tree.root = UnflattenExpression(expression, paramSource, ref i);
		tree.parameters = expression.Parameters.ToList();
		if (i >= 0) {
			// two DMC5 files get caught here due to having an extra float at the start - Capcom user error?
			// maybe it's a feature where you can specify two values, and they get used as a min-max random range?
			// ignoring this for now since they're inconsequential
			var root2 = UnflattenExpression(expression, paramSource, ref i);
			if (i >= 0) {
				throw new Exception("Incomplete expression!!");
			}
		}

		return tree;
	}

	private static ExpressionAtom UnflattenExpression(EFXExpressionObject expression, IExpressionParameterSource paramSource, ref int index)
	{
		var comp = expression.Components[index--];
		if (comp.data is EFXExpressionDataParameterHash param) {
			var paramType = expression.parameters?.GetParameterByHash(param.parameterHash)?.source ?? ExpressionParameterSource.Unknown;
			string? name;
			switch (paramType) {
				case ExpressionParameterSource.External:
				case ExpressionParameterSource.Constant:
					if (KnownExternalHashes.TryGetValue(param.parameterHash, out name)) {
						return new ExpressionParameter() { hash = param.parameterHash, name = name, source = paramType };
					}
					UnknownParameterHashes.Add(param.parameterHash);
					return new ExpressionParameter() { hash = param.parameterHash, source = paramType };
				case ExpressionParameterSource.Unknown:
				case ExpressionParameterSource.Parameter:
					var exprParam = paramSource.FindParameterByHash(param.parameterHash);
					if (exprParam != null) {
						return new ExpressionParameter() { hash = param.parameterHash, name = exprParam.name, source = ExpressionParameterSource.Parameter };
					}
					var fieldParam = paramSource.FindParameterByHash(param.parameterHash);
					if (fieldParam != null) {
						return new ExpressionParameter() { hash = param.parameterHash, name = fieldParam.name, source = ExpressionParameterSource.Parameter };
					}
					break;
			}
			if (paramType == ExpressionParameterSource.Unknown) paramType = ExpressionParameterSource.External;
			if (KnownExternalHashes.TryGetValue(param.parameterHash, out name)) {
				return new ExpressionParameter() { hash = param.parameterHash, name = name };
			}
			UnknownParameterHashes.Add(param.parameterHash);
			return new ExpressionParameter() { hash = param.parameterHash };
		}
		if (comp.data is EFXExpressionDataBinaryOperator binary) {
			var item = new ExpressionBinaryOperation();
			item.oper = binary.value;
			item.left = UnflattenExpression(expression, paramSource, ref index);
			item.right = UnflattenExpression(expression, paramSource, ref index);
			return item;
		}
		if (comp.data is EFXExpressionDataFloat floatType) {
			return new ExpressionFloat() { value = floatType.value };
		}
		if (comp.data is EFXExpressionDataFunction intType) {
			if (intType.value is EfxExpressionFunction.Lerp or EfxExpressionFunction.InvLerp or EfxExpressionFunction.Clamp) {
				var item = new ExpressionTernaryOperation();
				item.func = intType.value;
				item.left = UnflattenExpression(expression, paramSource, ref index);
				item.arg2 = UnflattenExpression(expression, paramSource, ref index);
				item.arg3 = UnflattenExpression(expression, paramSource, ref index);
				return item;
			} else {
				// other found values: 0, 1, 2, 4, 5, 6, 7, 8, 9, 10
				var item = new ExpressionUnaryOperation();
				item.func = intType.value;
				item.atom = UnflattenExpression(expression, paramSource, ref index);
				return item;
			}
		}

		if (comp.data is EFXExpressionDataUnaryOperator unary) {
			if (unary.value != 0) {
				throw new Exception("Found non-0 type 2 efx expression data");
			}
			var item = new ExpressionNegation();
			item.atom = UnflattenExpression(expression, paramSource, ref index);
			return item;
		}

		throw new Exception("Found unsupported expression component");
	}

}