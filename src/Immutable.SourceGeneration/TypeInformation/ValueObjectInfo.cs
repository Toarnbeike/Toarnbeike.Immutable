using Microsoft.CodeAnalysis;
using Toarnbeike.Immutable.Abstractions.Entities;
using Toarnbeike.Immutable.SourceGeneration.Extensions;

namespace Toarnbeike.Immutable.SourceGeneration.TypeInformation;

internal record ValueObjectInfo : TypeInfo
{
    public string Prefix { get; }
    public IReadOnlyList<PropertyInfo> Properties { get; }
    
    private ValueObjectInfo(INamedTypeSymbol typeSymbol) : base(typeSymbol)
    {
        Name = typeSymbol.Name;
        Namespace = typeSymbol.ContainingNamespace!.ToDisplayString();
        Prefix = GetPrefix(typeSymbol) + "_";
        Properties = typeSymbol.GetOrderedProperties();
    }

    public ValueObjectInfo(string name, string @namespace, string? prefix = null, 
        params IReadOnlyList<PropertyInfo> properties) : base(name, @namespace)
    {
        Prefix = (prefix ?? name) + "_";
        Properties = properties;
    }
    
    /// <summary>
    /// Tries to create a new AggregateInfo from an INamedTypeSymbol.
    /// Will work only if a couple of checks are passed:
    /// - Implements IEntity{TKey}
    /// - Has proper namespace
    /// </summary>
    /// <returns>A new instance of the AggregateInfo if the typeSymbol is valid, otherwise null.</returns>
    public static ValueObjectInfo? Create(INamedTypeSymbol typeSymbol)
    {
        return typeSymbol.ContainingNamespace is not null && !typeSymbol.ContainingNamespace.IsGlobalNamespace
            ? new ValueObjectInfo(typeSymbol) 
            : null;
    }

    private static string GetPrefix(INamedTypeSymbol typeSymbol) =>
        typeSymbol.GetAttributeProperty<ValueObjectAttribute, string?>(attr => attr.Prefix) ??
        typeSymbol.Name;
}