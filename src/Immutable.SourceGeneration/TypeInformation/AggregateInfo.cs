using Microsoft.CodeAnalysis;
using Toarnbeike.Immutable.Abstractions.Entities;
using Toarnbeike.Immutable.SourceGeneration.Extensions;

namespace Toarnbeike.Immutable.SourceGeneration.TypeInformation;

internal record AggregateInfo : TypeInfo
{
    public string PluralName { get; }
    public EntityKeyInfo EntityKeyInfo { get; }
    public IReadOnlyList<PropertyInfo> Properties { get; }
    
    private AggregateInfo(INamedTypeSymbol typeSymbol, EntityKeyInfo entityKeyInfo) : base(typeSymbol)
    {
        Name = typeSymbol.Name;
        Namespace = typeSymbol.ContainingNamespace!.ToDisplayString();
        PluralName = GetPluralName(typeSymbol);
        Properties = typeSymbol.GetOrderedProperties();
        EntityKeyInfo = entityKeyInfo;
    }

    public AggregateInfo(string name, string @namespace, EntityKeyInfo entityKeyInfo, string? pluralName = null, 
        params IReadOnlyList<PropertyInfo> properties) : base(name, @namespace)
    {
        PluralName = pluralName ?? name + "s";
        EntityKeyInfo = entityKeyInfo;
        Properties = properties;
    }
    
    /// <summary>
    /// Tries to create a new AggregateInfo from an INamedTypeSymbol.
    /// Will work only if a couple of checks are passed:
    /// - Implements the IAggregate interface
    /// - Implements IEntity{TKey}
    /// - Has proper namespace
    /// </summary>
    /// <returns>A new instance of the AggregateInfo if the typeSymbol is valid, otherwise null.</returns>
    public static AggregateInfo? Create(INamedTypeSymbol typeSymbol)
    {
        var baseTypes = typeSymbol.AllInterfaces;
        var hasIAggregate = baseTypes.Any(i => i.Name == "IAggregate" && i.TypeArguments.Length == 0);
        var hasNameSpace = typeSymbol.ContainingNamespace is not null && !typeSymbol.ContainingNamespace.IsGlobalNamespace;
        
        var entityInterface = baseTypes.FirstOrDefault(i => i.Name == "IEntity" && i.TypeArguments.Length == 1);
        var hasEntity = entityInterface is not null;
        var keyInfo = entityInterface?.TypeArguments[0] is INamedTypeSymbol keyType 
            ? EntityKeyInfo.Create(keyType) 
            : null;
        
        return hasIAggregate && hasEntity && hasNameSpace && keyInfo is not null
            ? new AggregateInfo(typeSymbol, keyInfo) 
            : null;
    }

    private static string GetPluralName(INamedTypeSymbol typeSymbol) =>
        typeSymbol.GetAttributeProperty<AggregateAttribute, string?>(attr => attr.PluralName) ??
            typeSymbol.Name + "s";
}