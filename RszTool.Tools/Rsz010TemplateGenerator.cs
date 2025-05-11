namespace RszTool.Generators;

using System.Text;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;

// NOTE: code should be kept in sync with any meaningful changes to RszSerializerGenerator

public class Rsz010TemplateGenerator : IIncrementalGenerator
{
    public class TemplateGeneratorContext : RszFileHandlerContext
    {
    }

    public void Initialize(IncrementalGeneratorInitializationContext context)
    {
        var classes = context.SyntaxProvider
            .CreateSyntaxProvider<TemplateGeneratorContext>(
                predicate: static (s, _) => s is ClassDeclarationSyntax classDecl && classDecl.HasAttribute("RszGenerate"),
                transform: static (ctx, _) => new TemplateGeneratorContext() {
                    ClassDecl = (ClassDeclarationSyntax)ctx.Node,
                    Fields = ctx.Node.ChildNodes().OfType<FieldDeclarationSyntax>().Where(field => !field.HasAttribute("RszIgnore")).ToList(),
                }
            )
            .Where(ctx => ctx is not null);

        var structs = context.SyntaxProvider
            .CreateSyntaxProvider<StructDeclarationSyntax>(
                predicate: static (s, _) => s is StructDeclarationSyntax && s.Parent is not ClassDeclarationSyntax,
                transform: static (ctx, _) => (StructDeclarationSyntax)ctx.Node
            );

        var enums = context.SyntaxProvider
            .CreateSyntaxProvider<EnumDeclarationSyntax>(
                predicate: static (s, _) => s is EnumDeclarationSyntax,
                transform: static (ctx, _) => (EnumDeclarationSyntax)ctx.Node
            );

        context.RegisterSourceOutput(structs, static (ctx, decl) => {
            var buildCtx = new ClassBuildContext(new StringBuilder(), null!, ctx);
            HandleStruct(decl, buildCtx);
            ctx.AddSource(decl.Identifier.Text, buildCtx.builder.ToString());
        });
        context.RegisterSourceOutput(classes, static (ctx, source) => Execute(ctx, source));

        context.RegisterSourceOutput(enums, static (ctx, enumDecl) => {
            var sb = new StringBuilder();
            var backing = enumDecl.BaseList?.Types.FirstOrDefault()?.GetText().ToString().Trim() ?? "int";
            sb.AppendLine($$"""
            typedef enum <{{backing}}>
            {
            """);
            int lastValue = 0;
            var name = enumDecl.Identifier.Text;
            foreach (var member in enumDecl.Members) {
                // member.Identifier.Text
                if (member.EqualsValue != null) {
                    lastValue = int.Parse(member.EqualsValue.Value.ToString());
                } else {
                    lastValue++;
                }
                sb.Append('\t').Append(name).Append('_').Append(member.Identifier.Text).Append(" = ").Append(lastValue).AppendLine(",");
            }
            sb.AppendLine($"}} {name};");

            ctx.AddSource("Enum_" + name, sb.ToString());
        });
    }

    private sealed class ClassBuildContext
    {
        public StringBuilder builder;
        public List<ConditionContext> OpenConditions = new();
        public string indent = string.Empty;
        public string versionParam = string.Empty;
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

    public static void ExecuteSafe(SourceProductionContext source, TemplateGeneratorContext context)
    {
        try {
            Execute(source, context);
        } catch (Exception) {
            // ignore
        }
    }

    private static Dictionary<string, string> typeRemaps = new() {
        { "Vector2", "Vec2" },
        { "Vector3", "Vec3" },
        { "Vector4", "Vec4" },
        { "via.Int3", "Int3" },
        { "via.Int2", "Int2" },
        { "via.RangeI", "Int2" },
        { "via.Color", "Color" },
        { "bool", "byte" },
        { "byte", "ubyte" },
        { "sbyte", "byte" },
    };

    private static string? RemapType(string? type)
    {
        return type != null && typeRemaps.TryGetValue(type, out var re) ? re : type;
    }

    public static void Execute(SourceProductionContext source, TemplateGeneratorContext context)
    {
        if (!context.ClassDecl.Modifiers.Any(mod => mod.Text == "partial")) {
            // source.ReportDiagnostic(Diagnostic.Create(Diagnostics.NonPartialClass, context.ClassDecl.Identifier.GetLocation(), context.ClassDecl.Identifier.Text));
            return;
        }

        var sb = new StringBuilder();
        var buildCtx = new ClassBuildContext(sb, context, source);

        if (context.ClassDecl.TryGetAttribute("RszVersionedObject", out var vvo)) {
            var versionType = (vvo.ArgumentList?.Arguments.FirstOrDefault()?.Expression as TypeOfExpressionSyntax)?.Type.GetElementTypeName();
            if (versionType != null) {
                buildCtx.versionParam = EvaluateExpressionIdentifier(buildCtx, vvo.ArgumentList?.Arguments.Skip(1).FirstOrDefault()?.Expression)
                    ?? "Version";
            }
        }

        var classIndent = "";
        var memberIndent = "\t";
        buildCtx.indent = memberIndent;

        var structName = context.ClassDecl.Identifier.Text.Replace("EFXAttribute", "");
        sb.Append(classIndent).Append("typedef struct ").AppendLine(structName);
        sb.Append(classIndent).AppendLine("{");

        foreach (var nested in context.ClassDecl.ChildNodes().OfType<StructDeclarationSyntax>()) {
            HandleStruct(nested, buildCtx);
        }

        Write010Members(buildCtx, buildCtx.context.ClassDecl, buildCtx.context.Fields);

        sb.Append(classIndent).AppendLine("};");
        var name = context.ClassDecl.Identifier.Text;
        var efxAttr = context.ClassDecl.GetAttribute("EfxStruct");
        if (efxAttr == null) {
            source.AddSource(name, sb.ToString());
        } else {
            var efxType = (efxAttr.ArgumentList?.Arguments.FirstOrDefault()?.Expression as MemberAccessExpressionSyntax)?.Name;
            var names = efxAttr.ArgumentList?.Arguments.Skip(1).Select(arg => (arg.Expression as MemberAccessExpressionSyntax)?.Name);
            if (efxType != null && names != null) {
                name = $"{name}__{efxType}__{(string.Join("_", names))}";
            }
            source.AddSource(name, sb.ToString());
        }
    }

    private static void Write010Members(ClassBuildContext ctx, ClassDeclarationSyntax classDecl, IEnumerable<FieldDeclarationSyntax> fields)
    {
        var baseClass = classDecl.BaseList?.Types.FirstOrDefault();
        if (baseClass != null) {
            var baseName = (baseClass.Type as SimpleNameSyntax)?.Identifier.Text;
            if (baseName != null) {
                var baseDecl = classDecl.Parent?.ChildNodes().FirstOrDefault(ch => ch is ClassDeclarationSyntax cls && cls.Identifier.Text == baseName);
                if (baseDecl != null && baseDecl is ClassDeclarationSyntax cls && cls.HasAttribute("RszGenerate")) {
                    var baseFields = cls.ChildNodes().OfType<FieldDeclarationSyntax>().Where(field => !field.HasAttribute("RszIgnore")).ToList();
                    Write010Members(ctx, cls, baseFields);
                }
            }
        }
        foreach (var field in fields) {
            HandleMember(field, ctx);
        }
    }

    private static string? EvaluateExpressionString(ClassBuildContext ctx, ExpressionSyntax? expr)
    {
        if (expr == null) return null;
        if (expr is InvocationExpressionSyntax invo) {
            if (invo.Expression is IdentifierNameSyntax id && id.Identifier.Text == "nameof") {
                return ((invo.ArgumentList.Arguments.First() as ArgumentSyntax).Expression.GetText().ToString());
            } else {
                // ctx.source.ReportDiagnostic(Diagnostic.Create(Diagnostic_UnsupportedExpression, invo.GetLocation()));
                return null;
            }
        }
        if (expr is MemberAccessExpressionSyntax member) {
            // assuming enum
            return (member.Expression as IdentifierNameSyntax)!.Identifier.Text + "_" + member.Name.Identifier.Text;
        }

        if (expr is LiteralExpressionSyntax lit) {
            return lit.Token.ValueText;
        }

        return expr.ToString();
    }

    private static string? EvaluateExpressionIdentifier(ClassBuildContext ctx, ExpressionSyntax? expr)
    {
        if (expr == null) return null;

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
        var fields = expr.FindParentNode<ClassDeclarationSyntax>()?.ChildNodes().OfType<FieldDeclarationSyntax>();

        return EvaluateExpressionIdentifier(ctx, expr);
    }

    private static void HandleStruct(StructDeclarationSyntax data, ClassBuildContext ctx)
    {
        ctx.Indent().AppendLine("typedef struct " + data.Identifier.Text + " {");
        ctx.AddIndent();
        foreach (var field in data.Members.OfType<FieldDeclarationSyntax>()) {
            var type = RemapType(field.GetFieldType()?.GetElementTypeName());
            var name = field.GetFieldName();
            ctx.Indent().AppendLine($"{type} {name};");
        }
        ctx.ReduceIndent();
        ctx.Indent().AppendLine("};");
    }

    private static void HandleMember(FieldDeclarationSyntax field, ClassBuildContext ctx)
    {
        if (field.HasAttribute("RszIgnore")) return;

        var name = field.GetFieldName();
        if (name == null) return;

        foreach (var conditionAttr in field.GetAttributesWhere(attr => attr.Name.ToString() == "RszConditional" || attr.Name.ToString() == "RszVersion")) {
            var args = conditionAttr.ArgumentList!.Arguments;
            var positional = conditionAttr.GetPositionalArguments();
            var optional = conditionAttr.GetOptionalArguments();

            var condition = string.Join(" ", positional.Select(p => EvaluateExpressionString(ctx, p.Expression)));
            if (conditionAttr.Name.ToString() == "RszVersion") {
                var argCount = positional.Count();
                if (argCount == 1) {
                    condition = $"{ctx.versionParam} >= {condition}";
                } else if (argCount == 2 && (condition.StartsWith('<') || condition.StartsWith('>') || condition.StartsWith('=') || condition.StartsWith("!="))) {
                    condition = $"{ctx.versionParam} {condition}";
                }
            }

            var endAt = EvaluateExpressionFieldIdentifier(ctx, optional.FirstOrDefault()?.Expression);
            if (!string.IsNullOrEmpty(condition)) {
                ctx.OpenConditions.Add(new ConditionContext(condition) { endAt = endAt ?? name });
                ctx.Indent().AppendLine($"if ({condition}) {{ // end at: {endAt ?? name}");
                ctx.AddIndent();
            }
        }

        AttributeSyntax mainAttr;
        if (field.TryGetAttribute("RszFixedSizeArray", out mainAttr) || field.TryGetAttribute("RszList", out mainAttr)) {
            var isClass = field.HasAttribute("RszClassInstance");
            var stringAttr = !isClass ? field.GetAttribute("RszInlineString") : null;
            var wstringAttr = !isClass && stringAttr == null ? field.GetAttribute("RszInlineWString") : null;

            var size = string.Join(" ", mainAttr.GetPositionalArguments().Select(p => EvaluateExpressionString(ctx, p.Expression)));
            if (string.IsNullOrEmpty(size)) {
                size = $"len_{name}";
                ctx.Indent().AppendLine($"uint {size};");
            }
            if (stringAttr != null || wstringAttr != null) {
                var fieldType = stringAttr != null ? "struct { string str; }" : "struct { wstring str; }";

                if (int.TryParse(size, out _)) {
                    ctx.Indent().AppendLine($"{fieldType} {name}[{size}] <read=str,optimize=false>;");
                } else {
                    ctx.Indent().AppendLine($"if ({size} > 0) {fieldType} {name}[{size}] <read=str,optimize=false>;");
                }
            } else {
                var meta = isClass ? " <optimize=false>" : "";
                var fieldType = RemapType(field.GetFieldType()?.GetArrayElementType(true));
                if (int.TryParse(size, out _)) {
                    ctx.Indent().AppendLine($"{fieldType} {name}[{size}]{meta};");
                } else {
                    ctx.Indent().AppendLine($"if ({size} > 0) {fieldType} {name}[{size}]{meta};");
                }
            }
        } else if (field.TryGetAttribute("RszInlineWString", out mainAttr) || field.TryGetAttribute("RszInlineString", out mainAttr)) {
            var stringType = mainAttr.Name.ToString().Contains("WString") ? "wstring" : "string";
            var size = string.Join(" ", mainAttr.GetPositionalArguments().Select(p => EvaluateExpressionString(ctx, p?.Expression)));
            if (string.IsNullOrEmpty(size)) {
                ctx.Indent().AppendLine($"int len_{name};");
                ctx.Indent().AppendLine($"{stringType} {name};");
            } else {
                ctx.Indent().AppendLine($"{stringType} {name};");
            }
        } else if (field.TryGetAttribute("RszOffsetWString", out mainAttr) || field.TryGetAttribute("RszOffsetString", out mainAttr)) {
            var stringType = mainAttr.Name.ToString().Contains("WString") ? "wstring" : "string";

            ctx.Indent().AppendLine($"ulong offset_{name};");
            ctx.Indent().AppendLine($"local int pos_{name} = FTell(); FSeek(offset_{name}); {stringType} {name}; FSeek(pos_{name});");
        } else if (field.TryGetAttribute("RszSwitch", out mainAttr)) {
            var args = mainAttr.GetPositionalArguments().ToList();
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
                ctx.Indent().AppendLine($"{classname} {name};");
                ctx.ReduceIndent();
                if (i == args.Count - 1) {
                    if (!string.IsNullOrEmpty(condition)) {
                        ctx.Indent().AppendLine("} else {");
                        ctx.AddIndent();
                        ctx.Indent().AppendLine("Warning(\"Unhandled case, template stopping\");");
                        ctx.Indent().AppendLine("Exit(1);");
                        ctx.ReduceIndent();
                    }
                    ctx.Indent().AppendLine($"}}");
                }
            }
        } else {
            ctx.Indent().Append(RemapType(field.GetFieldType()?.GetElementTypeName())).Append(' ').Append(name).AppendLine(";");
        }

        if (field.TryGetAttribute("RszPaddingAfter", out mainAttr)) {
            var positional = mainAttr.GetPositionalArguments();
            var padding = EvaluateExpressionString(ctx, positional.FirstOrDefault()?.Expression)
                ?? mainAttr.ArgumentList?.ToString();

            if (padding != null) {
                var conditions = positional.Skip(1);
                if (conditions.Any()) {
                    var paddingCondition = string.Join(" ", conditions.Select(p => EvaluateExpressionString(ctx, p.Expression)));
                    ctx.Indent().AppendLine($"if ({paddingCondition}) FSkip({padding});");
                } else {
                    ctx.Indent().AppendLine($"FSkip({padding});");
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
