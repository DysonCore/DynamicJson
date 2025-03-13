using Microsoft.CodeAnalysis;

namespace DysonCore.DynamicJson.SourceGenerators
{
    internal struct EnumEntry
    {
        public INamedTypeSymbol Symbol { get; private set; }
        public string MarkedMemer { get; private set; }

        internal EnumEntry(INamedTypeSymbol symbol, string markedMemer)
        {
            Symbol = symbol;
            MarkedMemer = markedMemer;
        }
    }
}