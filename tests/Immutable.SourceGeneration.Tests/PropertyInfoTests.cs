using Microsoft.CodeAnalysis;

namespace Toarnbeike.Immutable.SourceGeneration.Tests;

public class PropertyInfoTests
{
    [Fact]
    public void Create_Should_ReturnInfo_WhenValid()
    {
        var propertySymbol = Substitute.For<IPropertySymbol>();
        propertySymbol.Name.Returns("FirstName");

        var typeSymbol = Substitute.For<ITypeSymbol>();
        typeSymbol.ToDisplayString(SymbolDisplayFormat.FullyQualifiedFormat)
            .Returns("global::System.String");
        propertySymbol.Type.Returns(typeSymbol);

        var setMethod = Substitute.For<IMethodSymbol>();
        propertySymbol.SetMethod.Returns(setMethod);

        var result = PropertyInfo.Create(propertySymbol);

        result.ShouldNotBeNull();
        result.Name.ShouldBe("FirstName");
        result.TypeName.ShouldBe("global::System.String");
        result.IsReadOnly.ShouldBeFalse();
    }

    [Fact]
    public void Create_Should_MarkAsReadOnly_WhenNoSetter()
    {
        var propertySymbol = Substitute.For<IPropertySymbol>();
        propertySymbol.Name.Returns("Id");

        var typeSymbol = Substitute.For<ITypeSymbol>();
        typeSymbol.ToDisplayString(SymbolDisplayFormat.FullyQualifiedFormat)
            .Returns("global::System.Guid");
        propertySymbol.Type.Returns(typeSymbol);

        // Geen setter
        propertySymbol.SetMethod.Returns((IMethodSymbol?)null);

        var result = PropertyInfo.Create(propertySymbol);

        result.ShouldNotBeNull();
        result.Name.ShouldBe("Id");
        result.TypeName.ShouldBe("global::System.Guid");
        result.IsReadOnly.ShouldBeTrue();
    }
}