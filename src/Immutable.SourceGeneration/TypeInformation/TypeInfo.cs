using Microsoft.CodeAnalysis;

namespace Toarnbeike.Immutable.SourceGeneration.TypeInformation;

/// <summary>
/// Base for any type that the source generator can find.
/// It contains the name and the namespace of the type.
/// Can be constructed either from strings or from the
/// <see cref="INamedTypeSymbol"/> that the compiler generates.
/// </summary>
internal record TypeInfo(string Name, string Namespace)
{
    public TypeInfo(INamedTypeSymbol symbol) : this(symbol.Name, symbol.ContainingNamespace.ToDisplayString()) { }
}