using Toarnbeike.Immutable.Abstractions.Entities;
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

public partial interface IDataContext
{
    Dictionary<TestEntityId, TestEntity> TestEntitys { get; }
}

public sealed partial class DataContext : IDataContext
{
    public Dictionary<TestEntityId, TestEntity> TestEntitys { get; private set; } = new();
}