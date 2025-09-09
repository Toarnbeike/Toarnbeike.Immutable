using Toarnbeike.Immutable.Abstractions.Entities;
using Toarnbeike.Immutable.Entities;
using Toarnbeike.Optional;

namespace Toarnbeike.Immutable.Tests.TestData;

[Aggregate(PluralName = "TestEntities")]
public sealed partial record TestEntity : Entity<TestEntityId>
{
    public string Name { get; init; }
    public Option<string> Description { get; init; } = Option.None;
    public DateTime? DateOfBirth { get; init; } = null;
    public int YearOfEntry { get; init; } = 2000;
}