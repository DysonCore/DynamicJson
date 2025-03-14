using Microsoft.CodeAnalysis;

namespace DysonCore.DynamicJson.SourceGenerators
{
    internal class DiagnosticUtils
    {
        internal static void SendDiagnostics(DiagnosticDescriptor descriptor, GeneratorExecutionContext context)
        {
            Diagnostic diag = Diagnostic.Create(descriptor, Location.None);
            context.ReportDiagnostic(diag);
        }
    }
}

