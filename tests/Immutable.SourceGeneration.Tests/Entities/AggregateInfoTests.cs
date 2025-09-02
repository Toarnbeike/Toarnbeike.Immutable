using System.Collections.Immutable;
using Microsoft.CodeAnalysis;
using Toarnbeike.Immutable.SourceGeneration.Entities;

namespace Toarnbeike.Immutable.SourceGeneration.Tests.Entities;

public class AggregateInfoTests
{
    [Fact]
    public void Create_Should_ReturnInfo_WhenValid()
    {
        var symbol = Substitute.For<INamedTypeSymbol>();
        symbol.Name.Returns("TestAggregate");

        var ns = Substitute.For<INamespaceSymbol>();
        ns.ToDisplayString().Returns("TestNamespace");
        symbol.ContainingNamespace.Returns(ns);

        var aggregateInterface = Substitute.For<INamedTypeSymbol>();
        aggregateInterface.Name.Returns("IAggregate");
        aggregateInterface.TypeArguments.Returns(ImmutableArray<ITypeSymbol>.Empty);

        var typeArgument = Substitute.For<ITypeSymbol>();
        typeArgument.DeclaringSyntaxReferences.Returns(ImmutableArray<SyntaxReference>.Empty);
        
        var entityInterface = Substitute.For<INamedTypeSymbol>();
        entityInterface.Name.Returns("IEntity");
        entityInterface.TypeArguments.Returns([typeArgument]);

        symbol.AllInterfaces.Returns([aggregateInterface, entityInterface]);

        // Mock de properties
        var propertySymbol = Substitute.For<IPropertySymbol>();
        propertySymbol.Name.Returns("Id");
        propertySymbol.DeclaredAccessibility.Returns(Accessibility.Public);
        propertySymbol.Type.Returns(Substitute.For<ITypeSymbol>());
        propertySymbol.Type.ToDisplayString().Returns("System.Guid");

        symbol.GetMembers().Returns([propertySymbol]);

        var result = AggregateInfo.Create(symbol);

        result.ShouldNotBeNull();
        result!.Name.ShouldBe("TestAggregate");
        result.Namespace.ShouldBe("TestNamespace");
        result.Properties.Count.ShouldBe(1);
        result.Properties[0].Name.ShouldBe("Id");
    }

    [Fact]
    public void Create_Should_ReturnNull_WhenNoIAggregate()
    {
        var symbol = Substitute.For<INamedTypeSymbol>();
        symbol.Name.Returns("TestAggregate");

        var ns = Substitute.For<INamespaceSymbol>();
        ns.ToDisplayString().Returns("TestNamespace");
        symbol.ContainingNamespace.Returns(ns);

        var entityInterface = Substitute.For<INamedTypeSymbol>();
        entityInterface.Name.Returns("IEntity");
        entityInterface.TypeArguments.Returns([Substitute.For<ITypeSymbol>()]);

        // Only IEntity, no IAggregate
        symbol.AllInterfaces.Returns([entityInterface]);

        var result = AggregateInfo.Create(symbol);

        result.ShouldBeNull();
    }

    [Fact]
    public void Create_Should_ReturnNull_WhenNoIEntity()
    {
        var symbol = Substitute.For<INamedTypeSymbol>();
        symbol.Name.Returns("TestAggregate");

        var ns = Substitute.For<INamespaceSymbol>();
        ns.ToDisplayString().Returns("TestNamespace");
        symbol.ContainingNamespace.Returns(ns);

        var aggregateInterface = Substitute.For<INamedTypeSymbol>();
        aggregateInterface.Name.Returns("IAggregate");
        aggregateInterface.TypeArguments.Returns(ImmutableArray<ITypeSymbol>.Empty);

        // Only IAggregate, no IEntity
        symbol.AllInterfaces.Returns([aggregateInterface]);

        var result = AggregateInfo.Create(symbol);

        result.ShouldBeNull();
    }

    [Fact]
    public void Create_Should_ReturnNull_WhenNoNamespace()
    {
        var symbol = Substitute.For<INamedTypeSymbol>();
        symbol.Name.Returns("TestAggregate");

        // namespace is null
        symbol.ContainingNamespace.Returns((INamespaceSymbol?)null);

        var aggregateInterface = Substitute.For<INamedTypeSymbol>();
        aggregateInterface.Name.Returns("IAggregate");
        aggregateInterface.TypeArguments.Returns(ImmutableArray<ITypeSymbol>.Empty);

        var entityInterface = Substitute.For<INamedTypeSymbol>();
        entityInterface.Name.Returns("IEntity");
        entityInterface.TypeArguments.Returns([Substitute.For<ITypeSymbol>()]);

        symbol.AllInterfaces.Returns([aggregateInterface, entityInterface]);

        var result = AggregateInfo.Create(symbol);

        result.ShouldBeNull();
    }
}