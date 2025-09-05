using System.Text;
using Toarnbeike.Immutable.SourceGeneration.TypeInformation;

namespace Toarnbeike.Immutable.SourceGeneration.Extensions;

internal static class CodeGenerationHelpers
{
    /// <summary>
    /// Converts a property name (PascalCase) into a parameter name (camelCase).
    /// </summary>
    public static string ToParameterName(this PropertyInfo property)
    {
        return property.Name.Length <= 1 
            ? property.Name.ToLowerInvariant() 
            : $"{char.ToLowerInvariant(property.Name[0])}{property.Name.Substring(1)}";
    }
    
    /// <summary>
    /// Converts a property name (PascalCase) into a private field name (_camelCase).
    /// </summary>
    public static string ToPrivateFieldName(this PropertyInfo property)
    {
        return property.Name.Length <= 1 
            ? $"_{property.Name.ToLowerInvariant()}" 
            : $"_{char.ToLowerInvariant(property.Name[0])}{property.Name.Substring(1)}";
    }

    public static string ToNullableTypeName(this PropertyInfo property)
    {
        if (property.TypeName.EndsWith("?"))
        {
            return property.TypeName;
        }

        if (property.TypeName.StartsWith("global::Toarnbeike.Optional.Option<"))
        {
            return property.TypeName.Substring(35, property.TypeName.Length - 36) + "?";
        }

        return property.TypeName + "?";
    }
    /// <summary>
    /// Generates a parameter list from a collection of <see cref="PropertyInfo" />.
    /// Filters readonly properties away.
    /// Optionally also include properties with default values.
    /// When included, they become nullable with default value null.
    /// </summary>
    public static string ToParameterList(this IReadOnlyList<PropertyInfo> properties, string? prefix = null, bool includeDefault = false)
    {
        var segments = new List<string>();

        if (!string.IsNullOrEmpty(prefix))
        {
            segments.Add(prefix!);
        }
        
        segments.AddRange(properties
            .Where(p => !p.IsReadOnly && !p.HasDefaultValue)
            .Select(p => $"{p.TypeName} {p.ToParameterName()}"));

        if (includeDefault)
        {
            segments.AddRange(properties
                .Where(p => !p.IsReadOnly && p.HasDefaultValue)
                .Select(p => $"{p.ToNullableTypeName()} {p.ToParameterName()} = null"));
        }

        return string.Join(", ", segments);
    }

    public static string ToNotNullAssignments(this IReadOnlyList<PropertyInfo> properties, string intermediateResult)
    {
        var sb = new StringBuilder();
        foreach (var p in properties.Where(p => p.HasDefaultValue && !p.IsReadOnly))
        {
            sb.AppendLine(p.ToNotNullAssignment(intermediateResult));
        }
        return sb.ToString();
    }

    private static string ToNotNullAssignment(this PropertyInfo property, string intermediateResult) =>
        $$"""
                 if ({{property.ToParameterName()}} is not null)
                 {
                    {{intermediateResult}} = {{intermediateResult}} with { {{property.Name}} = {{property.ToParameterName()}}{{(property.IsValueType ? ".Value" : "")}} };
                 }
         """;
    
    /// <summary>
    /// Generates an assignment code block for all properties in the <param name="properties" /> 
    /// </summary>
    public static string ToAssignments(this IEnumerable<PropertyInfo> properties)
    {
        return string.Join("\n        ",
            properties.Where(p => !p.IsReadOnly && !p.HasDefaultValue)
                .Select(p => $"{p.Name} = {p.ToParameterName()};"));
    }
}