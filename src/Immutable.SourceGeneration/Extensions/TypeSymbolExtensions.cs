using System.Linq.Expressions;
using Microsoft.CodeAnalysis;

namespace Toarnbeike.Immutable.SourceGeneration.Extensions;

public static class TypeSymbolExtensions
{
    /// <summary>
    /// Get the value of a property on an attribute that might be above the TypeSymbol.
    /// </summary>
    public static TProperty? GetAttributeProperty<TAttribute, TProperty>(
        this ITypeSymbol symbol,
        Expression<Func<TAttribute, TProperty>> propertyExpression)
        where TAttribute : Attribute
    {
        if (propertyExpression.Body is not MemberExpression memberExpr)
        {
            throw new ArgumentException("Expression must be a property access", nameof(propertyExpression));
        }

        var propertyName = memberExpr.Member.Name;

        // try to find attribute
        var attributeData = symbol.GetAttributes()
            .FirstOrDefault(a => a.AttributeClass?.ToDisplayString() == typeof(TAttribute).FullName);

        if (attributeData is null)
        {
            return default; // Attribute not found.
        }

        // Check named arguments
        var namedArg = attributeData.NamedArguments
            .FirstOrDefault(na => na.Key == propertyName);

        if (namedArg.Value.Value is not null)
        {
            return (TProperty?)namedArg.Value.Value; // Found as named argument
        }

        // Check constructor arguments
        var ctorParam = attributeData.AttributeConstructor?
            .Parameters
            .FirstOrDefault(p => p.Name == propertyName);

        if (ctorParam != null)
        {
            var index = attributeData.AttributeConstructor!.Parameters.IndexOf(ctorParam);
            var value = attributeData.ConstructorArguments[index].Value;
            return (TProperty?)value; // found as constructor argument
        }

        return default; // not found
    }
    
    public static bool ImplementsInterface(this ITypeSymbol type, string interfaceName) =>
        type.AllInterfaces.Any(i => i.ToDisplayString() == interfaceName);
    
    public static bool IsGenericType(this ITypeSymbol type, string genericTypeName) => 
        type is INamedTypeSymbol named &&
        named.ConstructedFrom.ToDisplayString() == genericTypeName;
}