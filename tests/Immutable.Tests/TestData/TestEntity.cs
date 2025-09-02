using Toarnbeike.Immutable.Entities;

namespace Toarnbeike.Immutable.Tests.TestData;

[Aggregate]
public sealed partial record TestEntity : Entity<TestEntityId>, IAggregate
{
    public string Name { get; init; }
}