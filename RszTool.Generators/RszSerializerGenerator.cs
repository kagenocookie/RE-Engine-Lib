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
        public StringBuilder staticBuffer;
        public List<ConditionContext> OpenConditions = new();
        public string indent = string.Empty;
        public RszFileHandlerContext context;
        public readonly SourceProductionContext source;
        public string versionParam = "Version";

        public void AddIndent() => indent += "\t";
        public void ReduceIndent() => indent = indent.Length == 0 ? "" : indent.Substring(0, indent.Length - 1);
        public StringBuilder Indent() => builder.Append(indent);

        public ClassBuildContext(StringBuilder builder, RszFileHandlerContext context, SourceProductionContext source)
        {
            this.builder = builder;
            this.context = context;
            this.source = source;
            staticBuffer = new StringBuilder();
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

        var ns = context.ClassDecl.GetFullNamespace();
        if (ns != null) sb.Append("namespace ").Append(ns).Append(';').AppendLine();
        sb.AppendLine();

        var usings = context.ClassDecl.SyntaxTree.GetRoot().ChildNodes().OfType<UsingDirectiveSyntax>();
        if (usings != null) foreach (var uu in usings) {
            sb.AppendLine(uu.GetText().ToString().TrimEnd());
        }
        sb.AppendLine();


        var parents = context.ClassDecl.Ancestors().OfType<ClassDeclarationSyntax>().Reverse();
        foreach (var parent in parents) {
            sb.Append(string.Join(" ", parent.Modifiers.Select(m => m.Text))).Append(" class ").AppendLine(parent.Identifier.Text).AppendLine("{");
        }

        var classIndent = new string('\t', parents.Count());
        var memberIndent = new string('\t', parents.Count() + 1);
        var methodBodyIndent = new string('\t', parents.Count() + 2);
        buildCtx.indent = methodBodyIndent;

        sb.Append(classIndent).Append(string.Join(" ", context.ClassDecl.Modifiers.Select(m => m.Text))).Append(" class ").Append(context.ClassDecl.Identifier.Text);
        sb.Append(context.ClassDecl.TypeParameterList?.ToString() ?? string.Empty);
        sb.Append(context.ClassDecl.BaseList?.ToString() ?? string.Empty);
        sb.AppendLine();
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

        var fieldListMethod = context.ClassDecl.Members.Any(m => m is MethodDeclarationSyntax meth && meth.Identifier.Text == "GetFieldList")
            ? "GetFieldListDefault"
            : "GetFieldList";
        if (context.ClassDecl.TryGetAttribute("RszVersionedObject", out var vvo)) {
            var versionType = (vvo.ArgumentList?.Arguments.FirstOrDefault()?.Expression as TypeOfExpressionSyntax)?.Type.GetElementTypeName();
            if (versionType != null) {
                buildCtx.versionParam = EvaluateExpressionIdentifier(buildCtx, vvo.ArgumentList?.Arguments.Skip(1).FirstOrDefault()?.Expression)
                    ?? "Version";
                sb.Append(memberIndent).AppendLine($"public static IEnumerable<(string name, Type type)> {fieldListMethod}({versionType} {buildCtx.versionParam})");
                sb.Append(memberIndent).AppendLine("{");
                WriteFieldList(buildCtx);
                buildCtx.Indent().AppendLine("yield break;");
                sb.Append(memberIndent).AppendLine("}");
            }
        } else {
            sb.Append(memberIndent).AppendLine($"public static IEnumerable<(string name, Type type)> {fieldListMethod}()");
            sb.Append(memberIndent).AppendLine("{");
            WriteFieldList(buildCtx);
            buildCtx.Indent().AppendLine("yield break;");
            sb.Append(memberIndent).AppendLine("}");
        }

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
        source.AddSource($"{ns}_{context.ClassDecl.Identifier.Text}.rsz", sb.ToString());
    }

    private static void WriteReader(ClassBuildContext ctx)
    {
        foreach (var field in ctx.context.Fields) {
            try {
                HandleMember(HandleType.Read, field, ctx);
            } catch (Exception e) {
                ctx.source.ReportDiagnostic(Diagnostic.Create(Diagnostics.MemberGenerateFailure, field.GetLocation(), e.Message));
            }
        }
    }

    private static void WriteWriter(ClassBuildContext ctx)
    {
        foreach (var field in ctx.context.Fields) {
            HandleMember(HandleType.Write, field, ctx);
        }
    }

    private static void WriteFieldList(ClassBuildContext ctx)
    {
        foreach (var field in ctx.context.Fields) {
            HandleMember(HandleType.FieldList, field, ctx);
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

    private static string? EvaluateAttributeExpressionList(ClassBuildContext ctx, IEnumerable<AttributeArgumentSyntax>? expr)
    {
        if (expr == null) return null;
        return string.Join(" ", expr.Select(p => EvaluateExpressionString(ctx, p.Expression)));
    }
    private static string? EvaluateAttributeExpressionList(ClassBuildContext ctx, AttributeSyntax? expr)
    {
        if (expr?.ArgumentList == null) return null;
        return EvaluateAttributeExpressionList(ctx, expr.ArgumentList.Arguments);
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
        var fields = ctx.context.ClassDecl.GetFields();

        var field = EvaluateExpressionIdentifier(ctx, expr);
        if (field == null) {
            ctx.source.ReportDiagnostic(Diagnostic.Create(Diagnostics.UnsupportedExpression, expr.GetLocation()));
        } else if (!fields.Any(f => f.GetFieldName() == field)) {
            ctx.source.ReportDiagnostic(Diagnostic.Create(Diagnostics.InvalidFieldIdentifier, expr.GetLocation()));
        } else {
            return field;
        }

        return null;
    }

    private enum HandleType { Read, Write, FieldList }

    private static void HandleMember(HandleType handle, FieldDeclarationSyntax field, ClassBuildContext ctx)
    {
        if (field.HasAttribute("RszIgnore")) return;

        var name = field.GetFieldName();
        if (name == null) return;

        foreach (var conditionAttr in field.GetAttributesWhere(attr => attr.Name.ToString() == "RszConditional" || attr.Name.ToString() == "RszVersion")) {
            var args = conditionAttr.ArgumentList!.Arguments;
            var positional = conditionAttr.GetPositionalArguments();
            var optional = conditionAttr.GetOptionalArguments();

            var condition = EvaluateAttributeExpressionList(ctx, positional) ?? string.Empty;
            if (conditionAttr.Name.ToString() == "RszVersion") {
                var argCount = positional.Count();
                if (argCount == 1) {
                    condition = $"{ctx.versionParam} >= {condition}";
                } else if (argCount == 2 && (condition.StartsWith("<") || condition.StartsWith(">") || condition.StartsWith("=") || condition.StartsWith("!="))) {
                    condition = $"{ctx.versionParam} {condition}";
                }
            } else if (handle == HandleType.FieldList) {
                continue;
            }

            var endAt = EvaluateExpressionFieldIdentifier(ctx, optional.FirstOrDefault()?.Expression);
            if (!string.IsNullOrEmpty(condition)) {
                ctx.OpenConditions.Add(new ConditionContext(condition) { endAt = endAt ?? name });
                ctx.Indent().AppendLine($"if ({condition}) {{ // end at: {endAt ?? name}");
                ctx.AddIndent();
            }
        }

        if (handle == HandleType.Write) {
            if (EvaluateExpressionString(ctx, field.GetAttribute("RszStringHash")?.ArgumentList?.Arguments.FirstOrDefault()?.Expression) is string str) {
                ctx.Indent().AppendLine($"if ({name} == 0) {name} = global::RszTool.Common.MurMur3HashUtils.GetHash({str});");
            }
            if (EvaluateExpressionString(ctx, field.GetAttribute("RszStringAsciiHash")?.ArgumentList?.Arguments.FirstOrDefault()?.Expression) is string str2) {
                ctx.Indent().AppendLine($"if ({name} == 0) {name} = global::RszTool.Common.MurMur3HashUtils.GetAsciiHash({str2});");
            }
            if (EvaluateExpressionString(ctx, field.GetAttribute("RszStringUTF8Hash")?.ArgumentList?.Arguments.FirstOrDefault()?.Expression) is string str3) {
                ctx.Indent().AppendLine($"if ({name} == 0) {name} = global::RszTool.Common.MurMur3HashUtils.GetUTF8Hash({str3});");
            }
            if (EvaluateExpressionString(ctx, field.GetAttribute("RszStringLengthField")?.ArgumentList?.Arguments.FirstOrDefault()?.Expression) is string str4) {
                ctx.Indent().AppendLine($"{name} = {str4}?.Length ?? 0;");
            }
            if (EvaluateExpressionString(ctx, field.GetAttribute("RszArraySizeField")?.ArgumentList?.Arguments.FirstOrDefault()?.Expression) is string str5) {
                var targetField = ctx.context.ClassDecl.GetFields().FirstOrDefault(f => f.GetFieldName() == str5);
                if (targetField != null && targetField.GetFieldType()?.GetElementType() is GenericNameSyntax generic) {
                    ctx.Indent().AppendLine($"{name} = {str5}?.Count ?? 0;");
                } else {
                    ctx.Indent().AppendLine($"{name} = {str5}?.Length ?? 0;");
                }
            }
        }

        AttributeSyntax mainAttr;
        if (field.TryGetAttribute("RszList", out mainAttr) || (handle == HandleType.FieldList && field.TryGetAttribute("RszFixedSizeArray", out mainAttr))) {
            var isClass = field.HasAttribute("RszClassInstance");
            var isString = !isClass && field.HasAttribute("RszInlineString");
            var isWString = !isClass && !isString && field.HasAttribute("RszInlineWString");
            if (handle == HandleType.Write) {
                if (isClass) {
                    ctx.Indent().AppendLine($"{name}.Write(handler);");
                } else if (isString || isWString) {
                    var strMethod = isString ? "WriteAsciiString" : "WriteWString";
                    ctx.Indent().AppendLine($"{name} ??= Array.Empty<string>();");
                    ctx.Indent().AppendLine($"foreach (var str in {name}) handler.{strMethod}(str);");
                } else {
                    ctx.Indent().AppendLine($"{name} ??= new();");
                    ctx.Indent().AppendLine($"foreach (var val in {name}) handler.Write(val);");
                }
            } else if (handle == HandleType.Read) {
                var size = EvaluateAttributeExpressionList(ctx, mainAttr.GetPositionalArguments());
                if (isClass) {
                    if (string.IsNullOrEmpty(size)) size = "handler.Read<int>()";
                    var constructor = EvaluateAttributeExpressionList(ctx, field.GetAttribute("RszConstructorParams"));
                    if (string.IsNullOrEmpty(constructor)) {
                        ctx.Indent().AppendLine($"{name} ??= new();");
                        ctx.Indent().AppendLine($"{name}.Read(handler, (int)({size}));");
                    } else {
                        var fieldType = field.GetFieldType()?.GetArrayElementType();
                        ctx.Indent().AppendLine($"{name} ??= new();");
                        ctx.Indent().AppendLine($"for (int i = 0; i < {size}; ++i) {{ var item = new {fieldType}({constructor}); item.Read(handler); {name}.Add(item); }}");
                    }
                } else if (isString || isWString) {
                    ctx.Indent().AppendLine($"{name} = new string[{size}];");
                    var strMethod = isString ? "ReadAsciiString" : "ReadWString";
                    ctx.Indent().AppendLine($"for (int i = 0; i < {size}; ++i) {name}[i] = handler.{strMethod}(-1, -1, false);");
                } else {
                    var fieldType = field.GetFieldType()?.GetArrayElementType();
                    ctx.Indent().AppendLine($"{name} ??= new();");
                    ctx.Indent().AppendLine($"for (int i = 0; i < {size}; ++i) {name}.Add(handler.Read<{fieldType}>());");
                    // ctx.source.ReportDiagnostic(Diagnostic.Create(Diagnostics.UnhandledFailure, field.GetLocation(), "unsupported list type"));
                }
            } else if (handle == HandleType.FieldList) {
                var size = EvaluateAttributeExpressionList(ctx, mainAttr.GetPositionalArguments());
                if (!int.TryParse(size, out var count)) {
                    count = 999;
                }
                var fieldType = field.GetFieldType()!;
                ctx.Indent().AppendLine($"yield return (nameof({name}), typeof({fieldType.GetElementTypeName()}));");
            }
        } else if (field.TryGetAttribute("RszInlineWString", out mainAttr) || field.TryGetAttribute("RszInlineString", out mainAttr)) {
            var stringType = mainAttr.Name.ToString().Contains("WString") ? "WString" : "AsciiString";

            var size = EvaluateAttributeExpressionList(ctx, mainAttr.GetPositionalArguments());
            if (!string.IsNullOrEmpty(size)) size = $"(int)({size})";

            if (handle == HandleType.Write) {
                ctx.Indent().AppendLine($"{name} ??= string.Empty;");
                if (string.IsNullOrEmpty(size)) {
                    ctx.Indent().AppendLine($"handler.Write<int>({name}.Length);");
                }
                ctx.Indent().AppendLine($"handler.Write{stringType}({name});");
            } else if (handle == HandleType.Read) {
                if (string.IsNullOrEmpty(size)) {
                    ctx.Indent().AppendLine($"var len_{name} = handler.Read<int>();");
                    ctx.Indent().AppendLine($"{name} = handler.Read{stringType}(-1, len_{name}, false);");
                } else {
                    ctx.Indent().AppendLine($"{name} = handler.Read{stringType}(-1, {size}, false);");
                }
            } else if (handle == HandleType.FieldList) {
                if (string.IsNullOrEmpty(size)) {
                    ctx.Indent().AppendLine($"yield return (\"len_{name}\", typeof(uint));");
                }
                ctx.Indent().AppendLine($"yield return (nameof({name}), typeof({field.GetFieldType()?.GetArrayElementType()}));");
            }
        } else if (field.TryGetAttribute("RszOffsetWString", out mainAttr) || field.TryGetAttribute("RszOffsetString", out mainAttr)) {
            var stringType = mainAttr.Name.ToString().Contains("WString") ? "WString" : "AsciiString";

            if (handle == HandleType.Write) {
                ctx.Indent().AppendLine($"{name} ??= string.Empty;");
                ctx.Indent().AppendLine($"handler.WriteOffset{stringType}({name});");
            } else if (handle == HandleType.Read) {
                ctx.Indent().AppendLine($"{name} = handler.ReadOffset{stringType}();");
            } else if (handle == HandleType.FieldList) {
                ctx.Indent().AppendLine($"yield return (nameof({name}), typeof({field.GetFieldType()?.GetArrayElementType()}));");
            }
        } else if (field.TryGetAttribute("RszFixedSizeArray", out mainAttr)) {
            var fieldType = field.GetFieldType();
            var elementType = fieldType?.GetArrayElementType();
            var size = EvaluateAttributeExpressionList(ctx, mainAttr.GetPositionalArguments());
            if (handle == HandleType.Write) {
                if (!field.IsReadonly()) {
                    ctx.Indent().AppendLine($"{name} ??= new {elementType}[{size}];");
                    ctx.Indent().AppendLine($"if ({name}.Length != ({size})) {{");
                    ctx.AddIndent();
                    ctx.Indent().AppendLine($"var tmpArray_{name} = new {elementType}[{size}];");
                    ctx.Indent().AppendLine($"Array.Copy({name}, tmpArray_{name}, Math.Min({size}, {name}.Length));");
                    ctx.Indent().AppendLine($"{name} = tmpArray_{name};");
                    ctx.ReduceIndent();
                    ctx.Indent().AppendLine($"}}");
                }
                ctx.Indent().AppendLine($"handler.WriteArray({name});");
            } else if (handle == HandleType.Read) {
                if (field.IsReadonly()) {
                    ctx.Indent().AppendLine($"handler.ReadArray({name});");
                } else {
                    ctx.Indent().AppendLine($"{name} = handler.ReadArray<{elementType}>((int)({size}));");
                }
            }
        } else if (handle == HandleType.FieldList) {
            ctx.Indent().AppendLine($"yield return (nameof({name}), typeof({field.GetFieldType()?.GetElementTypeName()}));");
        } else if (field.TryGetAttribute("RszSwitch", out mainAttr)) {
            var args = mainAttr.GetPositionalArguments().ToList();
            if (handle == HandleType.Write) {
                ctx.Indent().AppendLine($"{name}.Write(handler);");
            } else if (handle == HandleType.Read) {
                var constructor = EvaluateAttributeExpressionList(ctx, field.GetAttribute("RszConstructorParams"));
                List<string> caseArgs = new();
                var addedArgs = 0;
                for (var i = 0; i < args.Count; i++) {
                    if (args[i].Expression is not TypeOfExpressionSyntax typeofSyntax) {
                        caseArgs.Add(EvaluateExpressionString(ctx, args[i].Expression)!);
                        continue;
                    }

                    var classname = typeofSyntax.Type.GetElementTypeName();
                    var condition = string.Join(" ", caseArgs);
                    caseArgs.Clear();
                    if (addedArgs++ == 0) {
                        ctx.Indent().AppendLine($"if ({condition}) {{");
                    } else if (string.IsNullOrEmpty(condition)) {
                        ctx.Indent().AppendLine("} else {");
                    } else {
                        ctx.Indent().AppendLine($"}} else if ({condition}) {{");
                    }
                    ctx.AddIndent();
                    ctx.Indent().AppendLine($"{name} = new {classname}({constructor});");
                    ctx.ReduceIndent();
                    if (i == args.Count - 1) {
                        if (!string.IsNullOrEmpty(condition)) {
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
            if (handle == HandleType.Write) {
                ctx.Indent().AppendLine($"{name}?.Write(handler);");
            } else if (handle == HandleType.Read) {
                var constructor = EvaluateAttributeExpressionList(ctx, field.GetAttribute("RszConstructorParams"));
                if (!field.IsReadonly()) {
                    ctx.Indent().AppendLine($"{name} ??= new({constructor});");
                } else if (constructor != null) {
                    ctx.Indent().AppendLine($"{name}.{constructor} = {constructor};");
                }
                ctx.Indent().AppendLine($"{name}.Read(handler);");
            }
        } else {
            ctx.Indent().Append(handle == HandleType.Write ? "handler.Write(ref " : "handler.Read(ref ").Append(name).AppendLine(");");
        }
        if (handle != HandleType.FieldList && field.TryGetAttribute("RszPaddingAfter", out mainAttr)) {
            var positional = mainAttr.GetPositionalArguments();
            var padding = EvaluateExpressionString(ctx, positional.FirstOrDefault()?.Expression)
                ?? mainAttr.ArgumentList?.ToString();

            if (padding != null) {
                var conditions = positional.Skip(1);
                if (conditions.Any()) {
                    var paddingCondition = EvaluateAttributeExpressionList(ctx, conditions);
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
