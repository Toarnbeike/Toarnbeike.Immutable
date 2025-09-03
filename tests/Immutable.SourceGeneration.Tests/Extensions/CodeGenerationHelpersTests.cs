using Toarnbeike.Immutable.SourceGeneration.Extensions;

namespace Toarnbeike.Immutable.SourceGeneration.Tests.Extensions;

public class CodeGenerationHelpersTests
{
    [Fact]
    public void ToParameterName_Should_Transform()
    {
        var actual = "Name".ToParameterName();
        actual.ShouldBe("name");
    }

    [Fact]
    public void ToParameterName_Should_Transform_ForSingleLetterParameter()
    {
        var actual = "A".ToParameterName();
        actual.ShouldBe("a");
    }

    [Fact]
    public void ToPrivateFieldName_Should_Transform()
    {
        var actual = "Name".ToPrivateFieldName();
        actual.ShouldBe("_name");
    }

    [Fact]
    public void ToPrivateFieldName_Should_Transform_ForSingleLetterParameter()
    {
        var actual = "A".ToPrivateFieldName();
        actual.ShouldBe("_a");
    }

    // [Fact]
    // public void ToParameterList_Should_Transform()
    // {
    //     var property1 = new PropertyInfo("Name", "string", false);
    //     var property2 = new PropertyInfo("Age", "int", false);
    //     IEnumerable<PropertyInfo> properties = [property1, property2];
    //     
    //     var actual = properties.ToParameterList();
    //     
    //     actual.ShouldBe("string name, int age");
    // }
    //
    // [Fact]
    // public void ToAssignments_Should_Transform()
    // {
    //     var property1 = new PropertyInfo("Name", "string", false);
    //     var property2 = new PropertyInfo("Age", "int", false);
    //     IEnumerable<PropertyInfo> properties = [property1, property2];
    //     
    //     var actual = properties.ToAssignments();
    //
    //     actual.ShouldContain("Name = name;");
    //     actual.ShouldContain("Age = age;");
    // }
}