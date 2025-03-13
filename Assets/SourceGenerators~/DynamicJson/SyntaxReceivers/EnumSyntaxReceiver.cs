using System.Collections.Generic;
using System.Linq;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace DysonCore.DynamicJson.SourceGenerators
{
    internal class EnumSyntaxReceiver : ISyntaxReceiver
    {
        internal List<EnumDeclarationSyntax> CandidateEnums { get; } = new ();

        public void OnVisitSyntaxNode(SyntaxNode syntaxNode)
        {
            if (syntaxNode is not EnumDeclarationSyntax enumDeclaration)
            {
                return;
            }

            // Look for enum declarations that have at least one attribute on one of their members.
            if (enumDeclaration.Members.Any(member => member.AttributeLists.Count > 0))
            {
                CandidateEnums.Add(enumDeclaration);
            }
        }
    }
}
