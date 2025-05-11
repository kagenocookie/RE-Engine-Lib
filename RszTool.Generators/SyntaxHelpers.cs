using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

public static class SyntaxHelpers
{
    public static string? GetFullNamespace(this SyntaxNode node)
    {
        // NOTE: this probably only handles the lowest level nested NS?
        return node.Ancestors().OfType<BaseNamespaceDeclarationSyntax>().FirstOrDefault()?.Name.ToFullString().Trim();
    }

    public static string GetFullName(this ClassDeclarationSyntax cls)
    {
        // TODO: this probably only handles the lowest level nested NS?
        // return node.Ancestors().OfType<BaseNamespaceDeclarationSyntax>().Select(ns => ns.Name.ToString());
        var ns = cls.GetFullNamespace();
        var parentPath = string.Join(".", cls.Ancestors().OfType<ClassDeclarationSyntax>().Select(c => c.Identifier.Text));
        if (string.IsNullOrEmpty(parentPath)) {
            return ns == null ? cls.Identifier.Text : $"{ns}.{cls.Identifier.Text}";
        } else {
            return ns == null ? $"{parentPath}.{cls.Identifier.Text}" : $"{ns}.{parentPath}.{cls.Identifier.Text}";
        }
    }

    public static bool HasAttribute(this MemberDeclarationSyntax node, string name)
    {
        return node.AttributeLists.Any(al => al.Attributes.Any(a => a.Name.ToString() == name));
    }

    public static IEnumerable<AttributeSyntax> GetAttributesWhere(this MemberDeclarationSyntax node, Func<AttributeSyntax, bool> filter)
    {
        return node.AttributeLists.SelectMany(al => al.Attributes.Where(filter));
    }

    public static IEnumerable<AttributeSyntax> GetAttributes(this MemberDeclarationSyntax node, string name)
    {
        return node.AttributeLists.SelectMany(al => al.Attributes.Where(a => a.Name.ToString() == name));
    }

    public static AttributeSyntax? GetAttribute(this MemberDeclarationSyntax node, string name)
        => node.GetAttributes(name).FirstOrDefault();

    public static bool TryGetAttribute(this MemberDeclarationSyntax node, string name, out AttributeSyntax attr)
    {
        attr = node.GetAttribute(name)!;
        return attr != null;
    }

    public static bool HasAttribute(this TypeDeclarationSyntax node, string name)
    {
        return node.AttributeLists.Any(al => al.Attributes.Any(a => a.Name.ToString() == name));
    }

    public static bool TryGetAttribute(this TypeDeclarationSyntax node, string name, out AttributeSyntax attr)
    {
        attr = node.GetAttribute(name)!;
        return attr != null;
    }

    public static IEnumerable<AttributeArgumentSyntax> GetPositionalArguments(this AttributeSyntax attribute)
    {
        return attribute.ArgumentList?.Arguments.Where(args => args.NameEquals == null) ?? Array.Empty<AttributeArgumentSyntax>();
    }

    public static IEnumerable<AttributeArgumentSyntax> GetOptionalArguments(this AttributeSyntax attribute)
    {
        return attribute.ArgumentList?.Arguments.Where(args => args.NameEquals != null) ?? Array.Empty<AttributeArgumentSyntax>();;
    }

    public static string? GetFieldName(this FieldDeclarationSyntax field)
    {
        return field.DescendantNodes().OfType<VariableDeclaratorSyntax>().FirstOrDefault()?.Identifier.Text;
    }

    public static TypeSyntax? GetFieldType(this FieldDeclarationSyntax field)
    {
        return field.DescendantNodes().OfType<VariableDeclarationSyntax>().FirstOrDefault()?.Type;
    }

    public static string? GetArrayElementType(this TypeSyntax type, bool ignoreNamespace = false)
    {
        if (type is GenericNameSyntax generic) {
            type = generic.TypeArgumentList.Arguments.First();
        }

        if (type is NullableTypeSyntax nullable) {
            type = nullable.ElementType;
        }

        if (type is ArrayTypeSyntax array) {
            type = array.ElementType;
        }

        if (ignoreNamespace && type is QualifiedNameSyntax colon) {
            type = colon.Right;
        }

        return type.ToString();
    }
    public static string? GetElementTypeName(this TypeSyntax type)
    {
        if (type is NullableTypeSyntax nullable) {
            type = nullable.ElementType;
        }

        return type.ToString();
    }
    public static T? FindParentNode<T>(this SyntaxNode? node) where T : SyntaxNode
    {
        while (node?.Parent != null) {
            if (node.Parent is T target) return target;
            node = node.Parent;
        }

        return null;
    }

    public static IEnumerable<FieldDeclarationSyntax> GetFields(this ClassDeclarationSyntax classDecl)
    {
        return classDecl.ChildNodes().OfType<FieldDeclarationSyntax>();
    }
}