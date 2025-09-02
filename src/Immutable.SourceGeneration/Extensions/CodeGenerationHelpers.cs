namespace Toarnbeike.Immutable.SourceGeneration.Extensions;

internal static class CodeGenerationHelpers
{
    /// <summary>
    /// Converts a property name (PascalCase) into a parameter name (camelCase).
    /// </summary>
    public static string ToParameterName(this string propertyName)
    {
        return propertyName.Length <= 1 
            ? propertyName.ToLowerInvariant() 
            : $"{char.ToLowerInvariant(propertyName[0])}{propertyName.Substring(1)}";
    }
    
    /// <summary>
    /// Converts a property name (PascalCase) into a private field name (_camelCase).
    /// </summary>
    public static string ToPrivateFieldName(this string propertyName)
    {
        return propertyName.Length <= 1 
            ? $"_{propertyName.ToLowerInvariant()}" 
            : $"_{char.ToLowerInvariant(propertyName[0])}{propertyName.Substring(1)}";
    }
    
    /// <summary>
    /// Generates a parameter list from a collection of <see cref="PropertyInfo" />.
    /// </summary>
    public static string ToParameterList(this IEnumerable<PropertyInfo> properties)
    {
        return string.Join(", ",
            properties.Select(p => $"{p.TypeName} {p.Name.ToParameterName()}"));
    }

    /// <summary>
    /// Generates an assignment code block for all properties in the <param name="properties" /> 
    /// </summary>
    public static string ToAssignments(this IEnumerable<PropertyInfo> properties)
    {
        return string.Join("\n",
            properties.Select(p => $"        {p.Name} = {p.Name.ToParameterName()};"));
    }
}