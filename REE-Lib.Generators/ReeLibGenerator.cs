namespace ReeLib.Generators;

using System.Runtime.InteropServices;
using System.Text;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

[Generator(LanguageNames.CSharp)]
public class ReeLibGenerator : IIncrementalGenerator
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

        context.RegisterSourceOutput(classes, static (ctx, source) => ExecuteRszSerializers(ctx, source));

        var gameNameEnum = context.SyntaxProvider
            .CreateSyntaxProvider(
                predicate: static (s, _) => s is EnumDeclarationSyntax enumDecl && (enumDecl.Identifier.Text == "GameName" || enumDecl.Identifier.Text == "KnownFileFormats"),
                transform: static (ctx, asd) => (EnumDeclarationSyntax)ctx.Node
            );
        context.RegisterSourceOutput(gameNameEnum, static (ctx, enumDecl) => {
            GenerateEnumHashes(ctx, enumDecl);
            var name = enumDecl.Identifier.Text;
            if (name == "GameName") {
                var sb = new StringBuilder();
                AppendParentClasses(sb, enumDecl, out var indent);
                var indentStr = new string('\t', indent - 1);
                sb.Append(indentStr).AppendLine($"public readonly partial struct GameIdentifier");
                sb.Append(indentStr).AppendLine("{");
                foreach (var val in enumDecl.Members) {
                    var valName = val.Identifier.Text;
                    if (valName == "unknown") continue;
                    sb.Append(indentStr).AppendLine($"\tpublic static readonly GameIdentifier {valName} = new GameIdentifier(\"{valName}\", GameNameHash.{valName});");
                }

                CloseIndents(sb, indent);
                ctx.AddSource($"GameIdentifier.GameNames", sb.ToString());
            }
            if (name == "KnownFileFormats") {
                var sb1 = new StringBuilder();
                var sb2 = new StringBuilder();
                AppendParentClasses(sb1, enumDecl, out var indent);
                var indentStr = new string('\t', indent - 1);
                var indentStr2 = new string('\t', indent);
                var indentStr3 = new string('\t', indent + 1);
                sb1.Append(indentStr).AppendLine($"public static class FileFormatExtensions");
                sb1.Append(indentStr).AppendLine("{");
                sb1.Append(indentStr2).AppendLine($"public static {name} ExtensionToEnum(string extension) => extension switch {{");
                sb2.Append(indentStr2).AppendLine($"public static {name} ExtensionHashToEnum(int hash) => ExtensionHashToEnum((uint)hash);");
                sb2.Append(indentStr2).AppendLine($"public static {name} ExtensionHashToEnum(uint hash) => hash switch {{");
                foreach (var val in enumDecl.Members) {
                    var valName = val.Identifier.Text;
                    if (val.HasLeadingTrivia) {
                        var comp = val.GetLeadingTrivia().ToFullString()
                            .Replace("\n", "").Replace("\r", "").Replace("<summary>", "").Replace("</summary>", "").Replace("///", "").Replace(" ", "").Replace(".", "").Trim();
                        if (!string.IsNullOrWhiteSpace(comp)) {
                            var extensionList = comp!.Split(',');
                            foreach (var ext in extensionList) {
                                sb1.Append(indentStr3).AppendLine($"\"{ext}\" => {name}.{valName},");
                                sb2.Append(indentStr3).AppendLine($"{MurMur3Hash(ext)} => {name}.{valName},");
                            }
                        } else {
                            sb1.Append(indentStr3).AppendLine("//unhandled: " + valName);
                        }
                    } else {
                        sb1.Append(indentStr3).AppendLine("//unhandled: " + valName);
                    }
                }

                sb1.Append(indentStr3).AppendLine($"_ => {name}.Unknown");
                sb1.Append(indentStr2).AppendLine("};");

                sb1.AppendLine().Append(sb2);
                sb1.Append(indentStr3).AppendLine($"_ => {name}.Unknown");
                sb1.Append(indentStr2).AppendLine("};");

                CloseIndents(sb1, indent);

                ctx.AddSource($"{name}.extMapping", sb1.ToString());
            }
        });
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
            ExecuteRszSerializers(source, context);
        } catch (Exception e) {
            source.ReportDiagnostic(Diagnostic.Create(Diagnostics.UnhandledFailure, context.ClassDecl.Identifier.GetLocation(), e.Message));
        }
    }

    private static void GenerateEnumHashes(SourceProductionContext source, EnumDeclarationSyntax enumDecl)
    {
        var sb = new StringBuilder();
        var name = enumDecl.Identifier.Text;
        AppendParentClasses(sb, enumDecl, out var indent);
        var indentStr = new string('\t', indent - 1);
        sb.Append(indentStr).AppendLine($"public enum {name}Hash");
        sb.Append(indentStr).AppendLine("{");
        foreach (var val in enumDecl.Members) {
            var valName = val.Identifier.Text;
            sb.Append(indentStr).AppendLine($"\t{valName} = {(int)MurMur3Hash(valName)},");
        }

        // sb.Append(indentStr).AppendLine("}");
        CloseIndents(sb, indent);

        source.AddSource($"{name}.hashes", sb.ToString());
    }

    public static void ExecuteRszSerializers(SourceProductionContext source, RszFileHandlerContext context)
    {
        if (!context.ClassDecl.Modifiers.Any(mod => mod.Text == "partial")) {
            source.ReportDiagnostic(Diagnostic.Create(Diagnostics.NonPartialClass, context.ClassDecl.Identifier.GetLocation(), context.ClassDecl.Identifier.Text));
            return;
        }

        var sb = new StringBuilder();
        var buildCtx = new ClassBuildContext(sb, context, source);

        var ns = context.ClassDecl.GetFullNamespace();
        AppendParentClasses(sb, context.ClassDecl, out var indent);
        AppendClassDefinition(sb, context.ClassDecl, indent);
        // if (ns != null) sb.Append("namespace ").Append(ns).Append(';').AppendLine();
        // sb.AppendLine();

        // var usings = context.ClassDecl.SyntaxTree.GetRoot().ChildNodes().OfType<UsingDirectiveSyntax>();
        // if (usings != null) foreach (var uu in usings) {
        //     sb.AppendLine(uu.GetText().ToString().TrimEnd());
        // }
        // sb.AppendLine();


        // var parents = context.ClassDecl.Ancestors().OfType<ClassDeclarationSyntax>().Reverse();
        // foreach (var parent in parents) {
        //     sb.Append(string.Join(" ", parent.Modifiers.Select(m => m.Text))).Append(" class ").AppendLine(parent.Identifier.Text).AppendLine("{");
        // }

        var classIndent = new string('\t', indent - 1);
        var memberIndent = new string('\t', indent);
        var methodBodyIndent = new string('\t', indent + 1);
        buildCtx.indent = methodBodyIndent;

        // sb.Append(classIndent).Append(string.Join(" ", context.ClassDecl.Modifiers.Select(m => m.Text))).Append(" class ").Append(context.ClassDecl.Identifier.Text);
        // sb.Append(context.ClassDecl.TypeParameterList?.ToString() ?? string.Empty);
        // sb.Append(context.ClassDecl.BaseList?.ToString() ?? string.Empty);
        // sb.AppendLine();
        // sb.Append(classIndent).AppendLine("{");

        string? versionString = null;
        if (context.ClassDecl.TryGetAttribute("RszAssignVersion", out var classAttr)) {
            versionString = EvaluateExpressionIdentifier(buildCtx, classAttr.ArgumentList?.Arguments.Skip(1).FirstOrDefault()?.Expression)
                ?? "var Version = handler.FileVersion;";
            if (!versionString.EndsWith(";")) versionString += ';';
        }

        sb.Append(memberIndent).AppendLine("[System.Runtime.CompilerServices.MethodImpl(System.Runtime.CompilerServices.MethodImplOptions.AggressiveInlining)]");
        sb.Append(memberIndent).AppendLine("private bool DefaultRead(FileHandler handler)");
        sb.Append(memberIndent).AppendLine("{");
        if (versionString != null) buildCtx.Indent().AppendLine(versionString);
        WriteReader(buildCtx);
        sb.Append(methodBodyIndent).AppendLine("return true;");
        sb.Append(memberIndent).AppendLine("}");

        sb.Append(memberIndent).AppendLine("[System.Runtime.CompilerServices.MethodImpl(System.Runtime.CompilerServices.MethodImplOptions.AggressiveInlining)]");
        sb.Append(memberIndent).AppendLine("private bool DefaultWrite(FileHandler handler)");
        sb.Append(memberIndent).AppendLine("{");
        if (versionString != null) buildCtx.Indent().AppendLine(versionString);
        WriteWriter(buildCtx);
        sb.Append(methodBodyIndent).AppendLine("return true;");
        sb.Append(memberIndent).AppendLine("}");

        var fieldListMethod = context.ClassDecl.Members.Any(m => m is MethodDeclarationSyntax meth && meth.Identifier.Text == "GetFieldList")
            ? "GetFieldListDefault"
            : "GetFieldList";
        var shouldNew = !context.ClassDecl.IsSubclassOf("BaseModel", "EFXAttribute", "EFXExpressionDataBase")
            ? "new "
            : "";
        if (context.ClassDecl.TryGetAttribute("RszVersionedObject", out classAttr)) {
            var versionType = (classAttr.ArgumentList?.Arguments.FirstOrDefault()?.Expression as TypeOfExpressionSyntax)?.Type.GetElementTypeName();
            if (versionType != null) {
                buildCtx.versionParam = EvaluateExpressionIdentifier(buildCtx, classAttr.ArgumentList?.Arguments.Skip(1).FirstOrDefault()?.Expression)
                    ?? "Version";
                sb.Append(memberIndent).AppendLine($"public static {shouldNew} IEnumerable<(string name, Type type)> {fieldListMethod}({versionType} {buildCtx.versionParam})");
                sb.Append(memberIndent).AppendLine("{");
                WriteFieldList(buildCtx);
                buildCtx.Indent().AppendLine("yield break;");
                sb.Append(memberIndent).AppendLine("}");
            }
        } else {
            sb.Append(memberIndent).AppendLine($"public static {shouldNew} IEnumerable<(string name, Type type)> {fieldListMethod}()");
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

        // sb.Append(classIndent).AppendLine("}");
        CloseIndents(sb, indent);

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
        if (field.IsConstOrStatic() || field.HasAttribute("RszIgnore")) return;

        var name = field.GetFieldName();
        if (name == null) return;

        foreach (var conditionAttr in field.GetAttributesWhere(attr => attr.Name.ToString() == "RszConditional" || attr.Name.ToString() == "RszVersion" || attr.Name.ToString() == "RszVersionExact")) {
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
            } else if (conditionAttr.Name.ToString() == "RszVersionExact") {
                var argCount = positional.Count();
                condition = string.Join(" || ", positional.Select(p => $"{ctx.versionParam} == {p}"));
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
                ctx.Indent().AppendLine($"if ({name} == 0) {name} = global::ReeLib.Common.MurMur3HashUtils.GetHash({str});");
            }
            if (EvaluateExpressionString(ctx, field.GetAttribute("RszStringAsciiHash")?.ArgumentList?.Arguments.FirstOrDefault()?.Expression) is string str2) {
                ctx.Indent().AppendLine($"if ({name} == 0) {name} = global::ReeLib.Common.MurMur3HashUtils.GetAsciiHash({str2});");
            }
            if (EvaluateExpressionString(ctx, field.GetAttribute("RszStringUTF8Hash")?.ArgumentList?.Arguments.FirstOrDefault()?.Expression) is string str3) {
                ctx.Indent().AppendLine($"if ({name} == 0) {name} = global::ReeLib.Common.MurMur3HashUtils.GetUTF8Hash({str3});");
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
                    ctx.Indent().AppendLine($"{name}?.Write(handler);");
                } else if (isString || isWString) {
                    var strMethod = isString ? "WriteAsciiString" : "WriteWString";
                    ctx.Indent().AppendLine($"{name} ??= Array.Empty<string>();");
                    ctx.Indent().AppendLine($"foreach (var str in {name}) handler.{strMethod}(str);");
                } else {
                    if (!field.IsReadonly()) ctx.Indent().AppendLine($"{name} ??= new();");
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
                        ctx.Indent().AppendLine($"for (int i = 0; i < ({size}); ++i) {{ var item = new {fieldType}({constructor}); item.Read(handler); {name}.Add(item); }}");
                    }
                } else if (isString || isWString) {
                    ctx.Indent().AppendLine($"{name} = new string[{size}];");
                    var strMethod = isString ? "ReadAsciiString" : "ReadWString";
                    ctx.Indent().AppendLine($"for (int i = 0; i < ({size}); ++i) {name}[i] = handler.{strMethod}(-1, -1, false);");
                } else {
                    var fieldType = field.GetFieldType()?.GetArrayElementType();
                    if (!field.IsReadonly()) ctx.Indent().AppendLine($"{name} ??= new();");
                    if (string.IsNullOrEmpty(size)) {
                        ctx.Indent().AppendLine($"var len_{name} = handler.Read<int>();");
                        size = $"len_{name}";
                    }
                    ctx.Indent().AppendLine($"for (int i = 0; i < ({size}); ++i) {name}.Add(handler.Read<{fieldType}>());");
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
                ctx.Indent().AppendLine($"{name}?.Write(handler);");
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

    private static void AppendParentClasses(StringBuilder sb, BaseTypeDeclarationSyntax classDecl, out int indentCount)
    {
        var ns = classDecl.GetFullNamespace();
        if (ns != null) sb.Append("namespace ").Append(ns).Append(';').AppendLine();
        sb.AppendLine();

        var usings = classDecl.SyntaxTree.GetRoot().ChildNodes().OfType<UsingDirectiveSyntax>();
        if (usings != null) foreach (var uu in usings) {
            sb.AppendLine(uu.GetText().ToString().TrimEnd());
        }
        sb.AppendLine();

        var parents = classDecl.Ancestors().OfType<BaseTypeDeclarationSyntax>().Reverse();
        foreach (var parent in parents) {
            sb.Append(string.Join(" ", parent.Modifiers.Select(m => m.Text))).Append(" class ").AppendLine(parent.Identifier.Text).AppendLine("{");
        }

        indentCount = parents.Count() + 1;
    }

    private static void AppendClassDefinition(StringBuilder sb, ClassDeclarationSyntax classDecl, int indentCount)
    {
        var classIndent = new string('\t', indentCount - 1);
        sb.Append(classIndent).Append(string.Join(" ", classDecl.Modifiers.Select(m => m.Text))).Append(" class ").Append(classDecl.Identifier.Text);
        sb.Append(classDecl.TypeParameterList?.ToString() ?? string.Empty);
        sb.Append(classDecl.BaseList?.ToString() ?? string.Empty);
        sb.AppendLine();
        sb.Append(classIndent).AppendLine("{");
    }
    private static void CloseIndents(StringBuilder sb, int indents)
    {
        for (int i = indents - 1; i >= 0; --i) sb.Append(new string('\t', indents - 1)).AppendLine("}");
    }


    private static uint MurMur3Hash(string str) => MurMur3Hash(MemoryMarshal.AsBytes(str.AsSpan()));
    private static uint MurMur3Hash(ReadOnlySpan<byte> bytes)
    {
        const uint c1 = 0xcc9e2d51;
        const uint c2 = 0x1b873593;
        const uint seed = 0xffffffff;

        uint h1 = seed;
        uint k1;

        for (int i = 0; i < bytes.Length; i += 4)
        {
            int chunkLength = Math.Min(4, bytes.Length - i);
            k1 = chunkLength switch
            {
                4 => (uint)(bytes[i] | bytes[i + 1] << 8 | bytes[i + 2] << 16 | bytes[i + 3] << 24),
                3 => (uint)(bytes[i] | bytes[i + 1] << 8 | bytes[i + 2] << 16),
                2 => (uint)(bytes[i] | bytes[i + 1] << 8),
                1 => bytes[i],
                _ => 0
            };
            k1 *= c1;
            k1 = Rotl32(k1, 15);
            k1 *= c2;
            h1 ^= k1;
            if (chunkLength == 4)
            {
                h1 = Rotl32(h1, 13);
                h1 = h1 * 5 + 0xe6546b64;
            }
        }

        h1 ^= (uint)bytes.Length;
        h1 = Fmix(h1);

        return h1;

        static uint Rotl32(uint x, byte r)
        {
            return (x << r) | (x >> (32 - r));
        }

        static uint Fmix(uint h)
        {
            h ^= h >> 16;
            h *= 0x85ebca6b;
            h ^= h >> 13;
            h *= 0xc2b2ae35;
            h ^= h >> 16;
            return h;
        }
    }
}

public class RszFileHandlerContext
{
    public ClassDeclarationSyntax ClassDecl = null!;
    public List<FieldDeclarationSyntax> Fields = new();
}
