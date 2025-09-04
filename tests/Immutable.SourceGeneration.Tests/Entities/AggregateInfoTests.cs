using System.Collections.Immutable;
using Microsoft.CodeAnalysis;
using Toarnbeike.Immutable.SourceGeneration.Tests.TestHelpers;
using Toarnbeike.Immutable.SourceGeneration.TypeInformation;

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

        var result = AggregateInfo.Create(symbol);

        result.ShouldNotBeNull();
        result.Name.ShouldBe("TestAggregate");
        result.Namespace.ShouldBe("TestNamespace");
        result.Properties.Single().Name.ShouldBe("Name");

        result.EntityKeyInfo.ShouldNotBeNull();
        result.EntityKeyInfo.Name.ShouldBe("TestEntityId");
    }
    
    [Fact]
    public void Create_Should_ReturnNull_WhenNoIAggregate()
    {
        const string source = """
                              namespace TestNamespace
                              {
                                  public interface IEntity<TKey> { }

                                  public sealed record TestAggregate : IEntity<TestEntityId>
                                  {
                                      public string Name { get; init; }
                                  }
                              }
                              """;

        var symbol = RoslynTestHelper.GetNamedTypeSymbol(source, "TestNamespace.TestAggregate");

        var result = AggregateInfo.Create(symbol);
        result.ShouldBeNull();
    }

    [Fact]
    public void Create_Should_ReturnNull_WhenNoIEntity()
    {
        const string source = """
                              namespace TestNamespace
                              {
                                  public interface IEntity<TKey> { }

                                  public sealed record TestAggregate : IAggregate
                                  {
                                      public string Name { get; init; }
                                  }
                              }
                              """;

        var symbol = RoslynTestHelper.GetNamedTypeSymbol(source, "TestNamespace.TestAggregate");

        var result = AggregateInfo.Create(symbol);
        result.ShouldBeNull();
    }

    [Fact]
    public void Create_Should_ReturnNull_WhenNoNamespace()
    {
        const string source = """
                              public interface IEntity<TKey> { }

                              public sealed record TestAggregate : IEntity<TestEntityId>, IAggregate
                              {
                                  public string Name { get; init; }
                              }
                              """;

        var symbol = RoslynTestHelper.GetNamedTypeSymbol(source, "TestAggregate");

        var result = AggregateInfo.Create(symbol);
        result.ShouldBeNull();
    }
}