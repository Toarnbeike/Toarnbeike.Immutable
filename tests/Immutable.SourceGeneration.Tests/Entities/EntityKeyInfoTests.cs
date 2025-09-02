using System.Collections.Immutable;
using Microsoft.CodeAnalysis;
using Toarnbeike.Immutable.SourceGeneration.Entities;

namespace Toarnbeike.Immutable.SourceGeneration.Tests.Entities;

public class EntityKeyInfoTests
{
    [Fact]
    public void Create_Should_ReturnInfo_WhenValid()
    {
        var symbol = Substitute.For<INamedTypeSymbol>();
        symbol.Name.Returns("TestId");

        var ns = Substitute.For<INamespaceSymbol>();
        ns.ToDisplayString().Returns("TestNamespace");
        symbol.ContainingNamespace.Returns(ns);

        // No interfaces
        symbol.AllInterfaces.Returns(ImmutableArray<INamedTypeSymbol>.Empty);
        
        var result = EntityKeyInfo.Create(symbol);

        result.ShouldNotBeNull();
        result.Name.ShouldBe("TestId");
        result.Namespace.ShouldBe("TestNamespace");
    }

    [Fact]
    public void Create_Should_ReturnNull_WhenImplementsIEntityKey()
    {
        var symbol = Substitute.For<INamedTypeSymbol>();
        symbol.Name.Returns("TestId");

        var ns = Substitute.For<INamespaceSymbol>();
        ns.ToDisplayString().Returns("TestNamespace");
        symbol.ContainingNamespace.Returns(ns);

        // IEntityKey interface
        var @interface = Substitute.For<INamedTypeSymbol>();
        @interface.Name.Returns("IEntityKey");
        @interface.TypeArguments.Returns(ImmutableArray<ITypeSymbol>.Empty);
        symbol.AllInterfaces.Returns([@interface]);

        var result = EntityKeyInfo.Create(symbol);

        result.ShouldBeNull();
    }

    [Fact]
    public void Create_Should_ReturnNull_WhenImplementsGenericIEntityKey()
    {
        var symbol = Substitute.For<INamedTypeSymbol>();
        symbol.Name.Returns("TestId");

        var ns = Substitute.For<INamespaceSymbol>();
        ns.ToDisplayString().Returns("TestNamespace");
        symbol.ContainingNamespace.Returns(ns);
        
        // generic IEntityKey interface
        var @interface = Substitute.For<INamedTypeSymbol>();
        @interface.Name.Returns("IEntityKey");
        @interface.TypeArguments.Returns([Substitute.For<ITypeSymbol>()]);
        symbol.AllInterfaces.Returns([@interface]);
        
        var result = EntityKeyInfo.Create(symbol);

        result.ShouldBeNull();
    }

    [Fact]
    public void Create_Should_ReturnNull_WhenNoNamespace()
    {
        var symbol = Substitute.For<INamedTypeSymbol>();
        symbol.Name.Returns("TestId");

        // namespace is null
        symbol.ContainingNamespace.Returns((INamespaceSymbol?)null);

        var @interface = Substitute.For<INamedTypeSymbol>();
        @interface.Name.Returns("IEntityKey");
        @interface.TypeArguments.Returns(ImmutableArray<ITypeSymbol>.Empty);
        symbol.AllInterfaces.Returns([@interface]);
        
        var result = EntityKeyInfo.Create(symbol);

        result.ShouldBeNull();
    }
}