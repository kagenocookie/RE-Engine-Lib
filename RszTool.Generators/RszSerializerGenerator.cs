namespace RszTool.Generators;

using System.Text;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;

[Generator(LanguageNames.CSharp)]
public class RszSerializerGenerator : IIncrementalGenerator
{
    public void Initialize(IncrementalGeneratorInitializationContext context)
    {
        var classes = context.SyntaxProvider
            .CreateSyntaxProvider<RszFileHandlerContext>(
                predicate: static (s, _) => s is ClassDeclarationSyntax classDecl && classDecl.HasAttribute("RszGenerate"),
                transform: static (ctx, _) => new RszFileHandlerContext() {
                    ClassDecl = (ClassDeclarationSyntax)ctx.Node,
                    Fields = ctx.Node.ChildNodes().OfType<FieldDeclarationSyntax>().Where(field => !field.HasAttribute("RszIgnore")).ToList(),
                }
            )
            .Where(ctx => ctx is not null);

        context.RegisterSourceOutput(classes, static (ctx, source) => Execute(ctx, source));
    }

    private sealed class ClassBuildContext
    {
        public StringBuilder builder;
        public List<ConditionContext> OpenConditions = new();
        public string indent = string.Empty;
        public RszFileHandlerContext context;
        public readonly SourceProductionContext source;

        public void AddIndent() => indent += "\t";
        public void ReduceIndent() => indent = indent.Length == 0 ? "" : indent.Substring(0, indent.Length - 1);
        public StringBuilder Indent() => builder.Append(indent);

        public ClassBuildContext(StringBuilder builder, RszFileHandlerContext context, SourceProductionContext source)
        {
            this.builder = builder;
            this.context = context;
            this.source = source;
        }
    }

    private sealed class ConditionContext
    {
        public string condition;
        public string? endAt;

        public ConditionContext(string condition)
        {
            this.condition = condition;
        }
    }

    public static void ExecuteSafe(SourceProductionContext source, RszFileHandlerContext context)
    {
        try {
            Execute(source, context);
        } catch (Exception e) {
            source.ReportDiagnostic(Diagnostic.Create(Diagnostics.UnhandledFailure, context.ClassDecl.Identifier.GetLocation(), e.Message));
        }
    }

    public static void Execute(SourceProductionContext source, RszFileHandlerContext context)
    {
        if (!context.ClassDecl.Modifiers.Any(mod => mod.Text == "partial")) {
            source.ReportDiagnostic(Diagnostic.Create(Diagnostics.NonPartialClass, context.ClassDecl.Identifier.GetLocation(), context.ClassDecl.Identifier.Text));
            return;
        }

        var sb = new StringBuilder();
        var buildCtx = new ClassBuildContext(sb, context, source);

        sb.AppendLine("using RszTool;");
        var ns = context.ClassDecl.GetFullNamespace();
        if (ns != null) sb.Append("namespace ").Append(ns).Append(';').AppendLine();
        sb.AppendLine();

        var parents = context.ClassDecl.Ancestors().OfType<ClassDeclarationSyntax>().Reverse();
        foreach (var parent in parents) {
            sb.Append(string.Join(" ", parent.Modifiers.Select(m => m.Text))).Append(" class ").Append(parent.Identifier.Text);
        }

        var classIndent = new string('\t', parents.Count());
        var memberIndent = new string('\t', parents.Count() + 1);
        var methodBodyIndent = new string('\t', parents.Count() + 2);
        buildCtx.indent = methodBodyIndent;

        sb.Append(classIndent).Append(string.Join(" ", context.ClassDecl.Modifiers.Select(m => m.Text))).Append(" class ").AppendLine(context.ClassDecl.Identifier.Text);
        sb.Append(classIndent).AppendLine("{");

        sb.Append(memberIndent).AppendLine("[System.Runtime.CompilerServices.MethodImpl(System.Runtime.CompilerServices.MethodImplOptions.AggressiveInlining)]");
        sb.Append(memberIndent).AppendLine("private bool DefaultRead(FileHandler handler)");
        sb.Append(memberIndent).AppendLine("{");
        WriteReader(buildCtx);
        sb.Append(methodBodyIndent).AppendLine("return true;");
        sb.Append(memberIndent).AppendLine("}");

        sb.Append(memberIndent).AppendLine("[System.Runtime.CompilerServices.MethodImpl(System.Runtime.CompilerServices.MethodImplOptions.AggressiveInlining)]");
        sb.Append(memberIndent).AppendLine("private bool DefaultWrite(FileHandler handler)");
        sb.Append(memberIndent).AppendLine("{");
        WriteWriter(buildCtx);
        sb.Append(methodBodyIndent).AppendLine("return true;");
        sb.Append(memberIndent).AppendLine("}");

        if (context.ClassDecl.HasAttribute("RszAutoReadWrite") || context.ClassDecl.HasAttribute("RszAutoRead")) {
            sb.Append(memberIndent).AppendLine("protected override bool DoRead(FileHandler handler) => DefaultRead(handler);");
        }

        if (context.ClassDecl.HasAttribute("RszAutoReadWrite") || context.ClassDecl.HasAttribute("RszAutoWrite")) {
            sb.Append(memberIndent).AppendLine("protected override bool DoWrite(FileHandler handler) => DefaultWrite(handler);");
        }

        sb.Append(classIndent).AppendLine("}");
        foreach (var parent in parents) {
            sb.AppendLine("}");
        }
        source.AddSource($"{context.ClassDecl.Identifier.Text}.rsz", sb.ToString());
    }

    private static void WriteReader(ClassBuildContext ctx)
    {
        foreach (var field in ctx.context.Fields) {
            try {
                HandleMember(false, field, ctx);
            } catch (Exception e) {
                ctx.source.ReportDiagnostic(Diagnostic.Create(Diagnostics.MemberGenerateFailure, field.GetLocation(), e.Message));
            }
        }
    }

    private static void WriteWriter(ClassBuildContext ctx)
    {
        foreach (var field in ctx.context.Fields) {
            HandleMember(true, field, ctx);
        }
    }

    private static string? EvaluateExpressionString(ClassBuildContext ctx, ExpressionSyntax? expr)
    {
        if (expr == null) return null;
        if (expr is InvocationExpressionSyntax invo) {
            if (invo.Expression is IdentifierNameSyntax id && id.Identifier.Text == "nameof") {
                return ((invo.ArgumentList.Arguments.First() as ArgumentSyntax).Expression.GetText().ToString());
            } else {
                ctx.source.ReportDiagnostic(Diagnostic.Create(Diagnostics.UnsupportedExpression, invo.GetLocation()));
                return null;
            }
        }

        if (expr is LiteralExpressionSyntax lit) {
            return lit.Token.ValueText;
        }

        return expr.ToString();
    }


    private static string? EvaluateExpressionIdentifier(ClassBuildContext ctx, ExpressionSyntax? expr)
    {
        if (expr == null) return null;
        var fields = ctx.context.ClassDecl.ChildNodes().OfType<FieldDeclarationSyntax>();

        string? field = null;
        if (expr is InvocationExpressionSyntax invo) {
            if (invo.Expression is IdentifierNameSyntax id && id.Identifier.Text == "nameof") {
                field = (invo.ArgumentList.Arguments.First() as ArgumentSyntax).Expression.GetText().ToString();
            } else {
                return null;
            }
        } else if (expr is LiteralExpressionSyntax lit) {
            field = lit.Token.ValueText;
        }

        return field;
    }

    private static string? EvaluateExpressionFieldIdentifier(ClassBuildContext ctx, ExpressionSyntax? expr)
    {
        if (expr == null) return null;
        var fields = ctx.context.ClassDecl.ChildNodes().OfType<FieldDeclarationSyntax>();

        var field = EvaluateExpressionIdentifier(ctx, expr);
        if (field == null) {
            // ctx.source.ReportDiagnostic(Diagnostic.Create(Diagnostic_UnsupportedExpression, expr.GetLocation()));
        } else if (!fields.Any(f => f.GetFieldName() == field)) {
            // ctx.source.ReportDiagnostic(Diagnostic.Create(Diagnostic_InvalidFieldIdentifier, expr.GetLocation()));
        } else {
            return field;
        }

        return null;
    }

    private static void HandleMember(bool isWrite, FieldDeclarationSyntax field, ClassBuildContext ctx)
    {
        if (field.HasAttribute("RszIgnore")) return;

        var name = field.GetFieldName();
        if (name == null) return;

        var conditionAttr = field.GetAttribute("RszConditional");
        if (conditionAttr != null) {
            var args = conditionAttr.ArgumentList!.Arguments;
            var positional = conditionAttr.GetPositionalArguments();
            var optional = conditionAttr.GetOptionalArguments();

            var condition = string.Join(" ", positional.Select(p => EvaluateExpressionString(ctx, p.Expression)));
            var endAt = EvaluateExpressionIdentifier(ctx, optional.FirstOrDefault()?.Expression);
            if (!string.IsNullOrEmpty(condition)) {
                ctx.OpenConditions.Add(new ConditionContext(condition) { endAt = endAt ?? name });
                ctx.Indent().AppendLine($"if ({condition}) {{ // end at: {endAt ?? name}");
                ctx.AddIndent();
            }
        }

        if (isWrite) {
            if (EvaluateExpressionString(ctx, field.GetAttribute("RszStringHash")?.ArgumentList?.Arguments.FirstOrDefault()?.Expression) is string str) {
                ctx.Indent().AppendLine($"if ({name} == 0) {name} = global::RszTool.Common.MurMur3HashUtils.GetHash({str});");
            }
            if (EvaluateExpressionString(ctx, field.GetAttribute("RszStringAsciiHash")?.ArgumentList?.Arguments.FirstOrDefault()?.Expression) is string str2) {
                ctx.Indent().AppendLine($"if ({name} == 0) {name} = global::RszTool.Common.MurMur3HashUtils.GetAsciiHash({str2});");
            }
        }

        if (field.TryGetAttribute("RszInlineWString", out var mainAttr) || field.TryGetAttribute("RszInlineString", out mainAttr)) {
            var stringType = mainAttr.Name.ToString().Contains("WString") ? "WString" : "AsciiString";

            var size = string.Join(" ", mainAttr.GetPositionalArguments().Select(p => EvaluateExpressionString(ctx, p?.Expression)));
            if (!string.IsNullOrEmpty(size)) size = $"(int)({size})";

            if (isWrite) {
                ctx.Indent().AppendLine($"{name} ??= string.Empty;");
                if (string.IsNullOrEmpty(size)) {
                    ctx.Indent().AppendLine($"handler.Write<int>({name}.Length);");
                }
                ctx.Indent().AppendLine($"handler.Write{stringType}({name});");
            } else {
                if (string.IsNullOrEmpty(size)) {
                    ctx.Indent().AppendLine($"var len_{name} = handler.Read<int>();");
                    ctx.Indent().AppendLine($"{name} = handler.Read{stringType}(-1, len_{name}, false);");
                } else {
                    ctx.Indent().AppendLine($"{name} = handler.Read{stringType}(-1, {size}, false);");
                }
            }
        } else if (field.TryGetAttribute("RszOffsetWString", out mainAttr) || field.TryGetAttribute("RszOffsetString", out mainAttr)) {
            var stringType = mainAttr.Name.ToString().Contains("WString") ? "WString" : "AsciiString";

            if (isWrite) {
                ctx.Indent().AppendLine($"{name} ??= string.Empty;");
                ctx.Indent().AppendLine($"handler.WriteOffset{stringType}({name});");
            } else {
                ctx.Indent().AppendLine($"{name} = handler.ReadOffset{stringType}();");
            }
        } else if (field.TryGetAttribute("RszFixedSizeArray", out mainAttr)) {
            var fieldType = field.GetFieldType();
            var elementType = fieldType?.GetArrayElementType();
            var size = string.Join(" ", mainAttr.GetPositionalArguments().Select(p => EvaluateExpressionString(ctx, p.Expression)));
            if (isWrite) {
                ctx.Indent().AppendLine($"if ({name} == null || {name}.Length != ({size}))");
                ctx.AddIndent();
                ctx.Indent().AppendLine($"{name} = new {elementType}[{size}];");
                ctx.ReduceIndent();
                ctx.Indent().AppendLine($"handler.WriteArray({name});");
            } else {
                ctx.Indent().AppendLine($"{name} = handler.ReadArray<{elementType}>((int)({size}));");
            }
        } else if (field.TryGetAttribute("RszClassList", out mainAttr)) {
            if (isWrite) {
                ctx.Indent().AppendLine($"{name}.Write(handler);");
            } else {
                var size = string.Join(" ", mainAttr.GetPositionalArguments().Select(p => EvaluateExpressionString(ctx, p.Expression)));
                if (string.IsNullOrEmpty(size)) size = "handler.Read<int>()";
                ctx.Indent().AppendLine($"{name}.Read(handler, (int)({size}));");
            }
        } else if (field.TryGetAttribute("RszSwitch", out mainAttr)) {
            var args = mainAttr.GetPositionalArguments().ToList();
            var isValid = args.Count % 2 == 0;
            if (!isValid || args.Count == 0) {
                ctx.Indent().AppendLine("// ERROR: Missing switch cases");
            } else if (isWrite) {
                ctx.Indent().AppendLine($"{name}.Write(handler);");
            } else {
                for (var i = 0; i < args.Count; i += 2) {
                    var condition = EvaluateExpressionString(ctx, args[i].Expression);
                    if (condition == "null") condition = "true";
                    var classname = EvaluateExpressionIdentifier(ctx, args[i + 1].Expression);
                    if (i == 0) {
                        ctx.Indent().AppendLine($"if ({condition}) {{");
                    } else {
                        ctx.Indent().AppendLine($"}} else if ({condition}) {{");
                    }
                    ctx.AddIndent();
                    ctx.Indent().AppendLine($"{name} = new {classname}();");
                    ctx.ReduceIndent();
                    if (i == args.Count - 2) {
                        if (condition != "true") {
                            ctx.Indent().AppendLine("} else {");
                            ctx.AddIndent();
                            ctx.Indent().AppendLine("throw new NotImplementedException(\"Unhandled switch case\");");
                            ctx.ReduceIndent();
                        }
                        ctx.Indent().AppendLine($"}}");
                    }
                }
                ctx.Indent().AppendLine($"{name}?.Read(handler);");
            }
        } else if (field.HasAttribute("RszClassInstance")) {
            if (isWrite) {
                ctx.Indent().AppendLine($"{name}?.Write(handler);");
            } else {
                ctx.Indent().AppendLine($"{name} ??= new();");
                ctx.Indent().AppendLine($"{name}.Read(handler);");
            }
        } else {
            ctx.Indent().Append(isWrite ? "handler.Write(ref " : "handler.Read(ref ").Append(name).AppendLine(");");
        }
        if (field.TryGetAttribute("RszPaddingAfter", out mainAttr)) {
            var positional = mainAttr.GetPositionalArguments();
            var padding = EvaluateExpressionString(ctx, positional.FirstOrDefault()?.Expression)
                ?? mainAttr.ArgumentList?.ToString();

            if (padding != null) {
                var conditions = positional.Skip(1);
                if (conditions.Any()) {
                    var paddingCondition = string.Join(" ", conditions.Select(p => EvaluateExpressionString(ctx, p.Expression)));
                    ctx.Indent().AppendLine($"if ({paddingCondition}) handler.Skip({padding});");
                } else {
                    ctx.Indent().AppendLine($"handler.Skip({padding});");
                }
            }
        }

        var anyEnded = false;
        foreach (var ended in ctx.OpenConditions.Where(cond => cond.endAt == name).ToList()) {
            ctx.ReduceIndent();
            ctx.Indent().AppendLine("}");
            ctx.OpenConditions.Remove(ended);
            anyEnded = true;
        }
        if (anyEnded) ctx.builder.AppendLine();
    }

}

public class RszFileHandlerContext
{
    public ClassDeclarationSyntax ClassDecl = null!;
    public List<FieldDeclarationSyntax> Fields = new();
}
