using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Toarnbeike.Immutable.SourceGeneration.TypeInformation;

namespace Toarnbeike.Immutable.SourceGeneration.Extensions;

internal static class NamedTypeSymbolExtensions
{
    /// <summary>
    /// Returns the public properties of the type in the same order
    /// as they are declared in the source code.
    /// Falls back to Roslyn's default order when syntax is not available.
    /// </summary>
    public static IReadOnlyList<PropertyInfo> GetOrderedProperties(this INamedTypeSymbol typeSymbol)
    {
        // Try to get the type declaration syntax for the type symbol
        var syntaxReferences = typeSymbol.DeclaringSyntaxReferences;
        var syntax = syntaxReferences.IsDefault ? null : syntaxReferences.FirstOrDefault()?.GetSyntax() as TypeDeclarationSyntax;

        // Read the property symbols
        var propertySymbols = typeSymbol
            .GetMembers()
            .OfType<IPropertySymbol>()
            .Where(p => p.DeclaredAccessibility == Accessibility.Public)
            .ToList();

        switch (syntax)
        {
            case null:
                // fallback: return the properties in roslyn order
                return propertySymbols
                    .Select(PropertyInfo.Create)
                    .ToList();
            default:
            {
                // Determine the order of the properties from the declaration syntax
                var propertyOrder = syntax.Members
                    .OfType<PropertyDeclarationSyntax>()
                    .Select((p, index) => (Name: p.Identifier.ValueText, Index: index))
                    .ToDictionary(x => x.Name, x => x.Index);

                // return the property symbols in order
                return propertySymbols
                    .OrderBy(p => propertyOrder.TryGetValue(p.Name, out var idx) ? idx : int.MaxValue)
                    .Select(PropertyInfo.Create)
                    .ToList();
            }
        }
    }
}