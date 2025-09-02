using Microsoft.CodeAnalysis;
using Toarnbeike.Immutable.SourceGeneration.Extensions;

namespace Toarnbeike.Immutable.SourceGeneration.Entities;

internal record AggregateInfo
{
    public const string AggregateAttributeFqn = "Toarnbeike.Immutable.Entities.AggregateAttribute";
    
    public string Name { get; }
    public string Namespace { get; }

    public IReadOnlyList<PropertyInfo> Properties { get; }
    
    private AggregateInfo(INamedTypeSymbol typeSymbol)
    {
        Name = typeSymbol.Name;
        Namespace = typeSymbol.ContainingNamespace!.ToDisplayString();
        Properties = typeSymbol.GetOrderedProperties();
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
        var hasEntity = baseTypes.Any(i => i.Name == "IEntity" && i.TypeArguments.Length == 1);
        var hasNameSpace = typeSymbol.ContainingNamespace is not null;
        
        return hasIAggregate && hasEntity && hasNameSpace
            ? new AggregateInfo(typeSymbol) 
            : null;
    }
}