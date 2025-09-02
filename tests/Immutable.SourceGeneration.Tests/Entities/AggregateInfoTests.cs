using System.Collections.Immutable;
using Microsoft.CodeAnalysis;
using Toarnbeike.Immutable.SourceGeneration.Entities;
using Toarnbeike.Immutable.SourceGeneration.Tests.TestHelpers;

namespace Toarnbeike.Immutable.SourceGeneration.Tests.Entities;

public class AggregateInfoTests
{
    [Fact]
    public void Create_Should_ExtractKeyInfo()
    {
        const string source = """
                              namespace TestNamespace
                              {
                                  public interface IEntity<TKey> { }

                                  public sealed record TestAggregate : IEntity<TestEntityId>, IAggregate
                                  {
                                      public string Name { get; init; }
                                  }
                              }
                              """;

        var symbol = RoslynTestHelper.GetNamedTypeSymbol(source, "TestNamespace.TestAggregate");

        var aggregateInfo = AggregateInfo.Create(symbol);

        aggregateInfo.ShouldNotBeNull();
        aggregateInfo.Name.ShouldBe("TestAggregate");
        aggregateInfo.Namespace.ShouldBe("TestNamespace");
        aggregateInfo.Properties.Single().Name.ShouldBe("Name");

        aggregateInfo.EntityKeyInfo.ShouldNotBeNull();
        aggregateInfo.EntityKeyInfo.Name.ShouldBe("TestEntityId");
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