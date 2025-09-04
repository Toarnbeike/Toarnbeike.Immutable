using Microsoft.CodeAnalysis;

namespace Toarnbeike.Immutable.SourceGeneration.TypeInformation;

internal record EntityKeyInfo : TypeInfo
{
    public const string EntityKeyAttributeFqn = "Toarnbeike.Immutable.Entities.EntityKeyAttribute";
    
    public EntityKeyInfo(string name, string @namespace) : base(name, @namespace) { }
    private EntityKeyInfo(INamedTypeSymbol symbol) : base(symbol) { }

    /// <summary>
    /// Tries to create a new EntityKeyInfo from an INamedTypeSymbol.
    /// Will work only if a couple of checks are passed:
    /// - Does not implement the IEntityKey interface
    /// - Does not implement the IEntityKey{TSelf} interface
    /// - Has proper namespace
    /// </summary>
    /// <returns>A new instance of the EntityKeyInfo if the context is valid, otherwise null.</returns>
    public static EntityKeyInfo? Create(INamedTypeSymbol typeSymbol)
    {
        var baseTypes = typeSymbol.AllInterfaces;
        var hasIEntityKey = baseTypes.Any(i => i.Name == "IEntityKey" && i.TypeArguments.Length == 0);
        var hasGenericIEntityKey = baseTypes.Any(i => i.Name == "IEntityKey" && i.TypeArguments.Length == 1);
        var hasNameSpace = typeSymbol.ContainingNamespace is not null;
        
        return !hasIEntityKey && !hasGenericIEntityKey && hasNameSpace
            ? new EntityKeyInfo(typeSymbol) 
            : null;
    }
}