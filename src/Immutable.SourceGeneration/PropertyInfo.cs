using Microsoft.CodeAnalysis;

namespace Toarnbeike.Immutable.SourceGeneration;

internal record PropertyInfo
{
    public string Name { get; }
    public string TypeName { get; }
    public bool IsReadOnly { get; }

    internal PropertyInfo(string name, string typeName, bool isReadOnly) =>
        (Name, TypeName, IsReadOnly) = (name, typeName, isReadOnly);

    public static PropertyInfo Create(IPropertySymbol propertySymbol)
    {
        var typeName = propertySymbol.Type.ToDisplayString(SymbolDisplayFormat.FullyQualifiedFormat);

        var isReadOnly = propertySymbol.SetMethod is null;

        return new PropertyInfo(propertySymbol.Name, typeName, isReadOnly);
    }
}