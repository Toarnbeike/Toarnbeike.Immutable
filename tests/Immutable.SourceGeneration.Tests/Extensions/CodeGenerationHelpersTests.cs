using Toarnbeike.Immutable.SourceGeneration.Extensions;
using Toarnbeike.Immutable.SourceGeneration.TypeInformation;

namespace Toarnbeike.Immutable.SourceGeneration.Tests.Extensions;

public class CodeGenerationHelpersTests
{
    [Fact]
    public void ToParameterName_Should_Transform()
    {
        var property = new PropertyInfo("Name", "string");
        var actual = property.ToParameterName();
        actual.ShouldBe("name");
    }

    [Fact]
    public void ToParameterName_Should_Transform_ForSingleLetterParameter()
    {
        var property = new PropertyInfo("A", "string");
        var actual = property.ToParameterName();
        actual.ShouldBe("a");
    }

    [Fact]
    public void ToPrivateFieldName_Should_Transform()
    {
        var property = new PropertyInfo("Name", "string");
        var actual = property.ToPrivateFieldName();
        actual.ShouldBe("_name");
    }

    [Fact]
    public void ToPrivateFieldName_Should_Transform_ForSingleLetterParameter()
    {        
        var property = new PropertyInfo("A", "string");
        var actual = property.ToPrivateFieldName();
        actual.ShouldBe("_a");
    }

    [Fact]
    public void ToNullableTypeName_Should_Transform()
    {
        var property = new PropertyInfo("Name", "string");
        var actual = property.ToNullableTypeName();
        actual.ShouldBe("string?");
    }

    [Fact]
    public void ToNullableTypeName_Should_Transform_WhenTypeIsNullable()
    {
        var property = new PropertyInfo("Name", "string?");
        var actual = property.ToNullableTypeName();
        actual.ShouldBe("string?");
    }

    [Fact]
    public void ToNullableTypeName_Should_Transform_WhenTypeIsOption()
    {
        var property = new PropertyInfo("Name", "global::Toarnbeike.Optional.Option<string>");
        var actual = property.ToNullableTypeName();
        actual.ShouldBe("string?");
    }
    
    [Fact]
    public void ToParameterList_Should_Transform()
    {
        var property1 = new PropertyInfo("Name", "string");
        var property2 = new PropertyInfo("Age", "int");
        IReadOnlyList<PropertyInfo> properties = [property1, property2];
        
        var actual = properties.ToParameterList();
        
        actual.ShouldBe("string name, int age");
    }

    [Fact]
    public void ToNotNullAssignments_Should_Transform()
    {
        var property = new PropertyInfo("Name", "string", hasDefaultValue: true);
        List<PropertyInfo> properties = [property];
        var result = properties.ToNotNullAssignments("entity");
        result.ShouldContain("if (name is not null)");
        result.ShouldContain("entity = entity with { Name = name };");
    }
    
    [Fact]
    public void ToNotNullAssignments_Should_Transform_ValueType()
    {
        var property = new PropertyInfo("Age", "int", hasDefaultValue: true, isValueType: true);
        List<PropertyInfo> properties = [property];
        var result = properties.ToNotNullAssignments("entity");
        result.ShouldContain("if (age is not null)");
        result.ShouldContain("entity = entity with { Age = age.Value };");
    }

    [Fact]
    public void ToNotNullAssignments_Should_NotTransform_WhenTypeHasNoDefaultValue()
    {
        var property = new PropertyInfo("Age", "int", hasDefaultValue: false);
        List<PropertyInfo> properties = [property];
        var result = properties.ToNotNullAssignments("entity");
        result.ShouldBeEmpty();
    }

    [Fact]
    public void ToNotNullAssignments_Should_NotTransform_WhenTypeIsReadOnly()
    {
        var property = new PropertyInfo("Age", "int", hasDefaultValue: true, isReadOnly: true);
        List<PropertyInfo> properties = [property];
        var result = properties.ToNotNullAssignments("entity");
        result.ShouldBeEmpty();
    }
    
    [Fact]
    public void ToAssignments_Should_Transform()
    {
        var property1 = new PropertyInfo("Name", "string", false, false, false);
        var property2 = new PropertyInfo("Age", "int", false, false, true);
        IReadOnlyList<PropertyInfo> properties = [property1, property2];

        
        var actual = properties.ToAssignments();
    
        actual.ShouldContain("Name = name;");
        actual.ShouldContain("Age = age;");
    }
}