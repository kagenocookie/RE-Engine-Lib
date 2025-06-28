namespace ReeLib.Generators;

using Microsoft.CodeAnalysis;

internal static class Diagnostics
{
    public static readonly DiagnosticDescriptor UnhandledFailure = new DiagnosticDescriptor("RSZT001", "Failed to execute ReeLib generator", "There was an unexpected error: {0}", "Design", DiagnosticSeverity.Error, true);
    public static readonly DiagnosticDescriptor MemberGenerateFailure = new DiagnosticDescriptor("RSZT002", "Failed to generate member", "There was an unexpected error while generating the member: {0}", "Design", DiagnosticSeverity.Error, true);
    public static readonly DiagnosticDescriptor NonPartialClass = new DiagnosticDescriptor("RSZT003", "Class annotated with RszGenerate must be partial", "Change class {0} access to partial", "Design", DiagnosticSeverity.Error, true);
    public static readonly DiagnosticDescriptor UnsupportedExpression = new DiagnosticDescriptor("RSZT004", "Invalid or unsupported expression", "Invalid or unsupported expression", "Design", DiagnosticSeverity.Error, true);
    public static readonly DiagnosticDescriptor InvalidFieldIdentifier = new DiagnosticDescriptor("RSZT005", "Invalid end field name for conditional", "The conditional EndAt parameter must be a valid field", "Design", DiagnosticSeverity.Error, true);
}
