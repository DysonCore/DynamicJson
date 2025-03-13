using System.Collections.Immutable;
using System.Text;
using Microsoft.CodeAnalysis;

namespace DysonCore.DynamicJson.SourceGenerators
{
    /// <summary>
    /// Provides utility methods for working with assembly qualified names and compile time literals.
    /// </summary>
    internal static class TypeNameUtils
    {
        /// <summary>
        /// Escapes special characters in the input string for safe string usage in source generated code.
        /// </summary>
        /// <param name="input">The input string to be escaped.</param>
        /// <returns>A new string with special characters escaped.</returns>
        internal static string Escape(string input)
        {
            if (string.IsNullOrEmpty(input))
            {
                return string.Empty;
            }

            StringBuilder builder = new StringBuilder();

            foreach (char character in input)
            {
                switch (character)
                {
                    case '\\':
                        builder.Append("\\\\");
                        break;
                    case '\"':
                        builder.Append("\\\"");
                        break;
                    case '\r':
                        builder.Append("\\r");
                        break;
                    case '\n':
                        builder.Append("\\n");
                        break;
                    case '\t':
                        builder.Append("\\t");
                        break;
                    default:
                        if (char.IsControl(character))
                        {
                            builder.AppendFormat("\\u{0:X4}", (int)character);
                        }
                        else
                        {
                            builder.Append(character);
                        }

                        break;
                }
            }

            return builder.ToString();
        }


        /// <summary>
        /// Returns an assembly‑qualified name (AQN) for the provided symbol.
        /// For nested types, it uses '+' as the separator and removes the "global::" prefix.
        /// The result is escaped for safe inclusion in generated source code.
        /// </summary>
        internal static string GetAssemblyQualifiedName(INamedTypeSymbol symbol)
        {
            AssemblyIdentity assemblyIdentity = symbol.ContainingAssembly.Identity;
            
            string typeName = GetTypeName(symbol);
            
            string version = assemblyIdentity.Version.ToString();
            string publicKeyToken = GetPublicKeyTokenString(assemblyIdentity);
            string culture = string.IsNullOrEmpty(assemblyIdentity.CultureName) ? "neutral" : assemblyIdentity.CultureName; // Use "neutral" if no culture is specified.

            // Build the assembly part.
            // This produces a string like: "MyAssembly, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null"
            string assemblyPart = $"{assemblyIdentity.Name}, Version={version}, Culture={culture}, PublicKeyToken={publicKeyToken}";

            // Combine the type name and assembly part.
            return Escape($"{typeName}, {assemblyPart}");
        }

        /// <summary>
        /// Returns the compile-time literal representation of the provided symbol.
        /// </summary>
        internal static string GetCompileTimeLiteral(INamedTypeSymbol symbol)
        {
            return symbol.ToDisplayString(SymbolDisplayFormat.FullyQualifiedFormat);
        }

        /// <summary>
        /// Recursively builds the type's full name.
        /// For nested types, uses '+' as a separator.
        /// Also removes any "global::" prefix.
        /// </summary>
        private static string GetTypeName(INamedTypeSymbol symbol)
        {
            string name;
            
            if (symbol.ContainingType != null)
            {
                name = GetTypeName(symbol.ContainingType) + "+" + symbol.Name;
            }
            else
            {
                // Use the namespace if it's not global.
                string nameSpace = symbol.ContainingNamespace.IsGlobalNamespace ? "" : symbol.ContainingNamespace.ToDisplayString();
                name = string.IsNullOrEmpty(nameSpace) ? symbol.Name : $"{nameSpace}.{symbol.Name}";
            }

            // Remove "global::" if present.
            if (name.StartsWith("global::"))
            {
                name = name.Substring("global::".Length);
            }

            return name;
        }

        /// <summary>
        /// Converts the public key token of an assembly identity into a lowercase hexadecimal string.
        /// Returns "null" if no token is present.
        /// </summary>
        private static string GetPublicKeyTokenString(AssemblyIdentity identity)
        {
            ImmutableArray<byte> keyToken = identity.PublicKeyToken;
            
            if (keyToken == null || keyToken.Length == 0)
            {
                return "null";
            }
                
            StringBuilder builder = new StringBuilder();
            
            foreach (byte data in keyToken)
            {
                builder.Append(data.ToString("x2"));
            }

            return builder.ToString();
        }
    }
}