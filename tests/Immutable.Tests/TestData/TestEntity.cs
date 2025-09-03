using Toarnbeike.Immutable.Entities;
using Toarnbeike.Optional;

namespace Toarnbeike.Immutable.Tests.TestData;

[Aggregate]
public sealed partial record TestEntity : Entity<TestEntityId>, IAggregate
{
    public string Name { get; init; }
    public Option<string> Description { get; init; } = Option.None;
    public DateTime? DateOfBirth { get; init; } = null;
    public int YearOfEntry { get; init; } = 2000;
}