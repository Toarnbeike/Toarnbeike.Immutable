using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace Toarnbeike.Immutable.SourceGeneration.TypeInformation;

internal record PropertyInfo
{
    public string Name { get; }
    public string TypeName { get; }
    public bool IsReadOnly { get; }
    public bool HasDefaultValue { get; }
    public bool IsValueType { get; }

    internal PropertyInfo(string name, string typeName, bool isReadOnly = false, bool hasDefaultValue = false, bool isValueType = false) =>
        (Name, TypeName, IsReadOnly, HasDefaultValue, IsValueType) = (name, typeName, isReadOnly, hasDefaultValue, isValueType);

    public static PropertyInfo Create(IPropertySymbol propertySymbol)
    {
        var typeName = propertySymbol.Type.ToDisplayString(SymbolDisplayFormat.FullyQualifiedFormat);
        var isReadOnly = propertySymbol.SetMethod is null;
        var hasDefaultValue = propertySymbol.DeclaringSyntaxReferences
            .Select(r => r.GetSyntax())
            .OfType<PropertyDeclarationSyntax>()
            .Any(p => p.Initializer != null);
        var isValueType = propertySymbol.Type.IsValueType;

        return new PropertyInfo(propertySymbol.Name, typeName, isReadOnly, hasDefaultValue, isValueType);
    }
}