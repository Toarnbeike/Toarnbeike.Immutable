using Toarnbeike.Immutable.Entities;

namespace Toarnbeike.Immutable.Tests.TestData;

internal sealed record TestEntity : Entity<TestEntityId>, IAggregate<TestEntityId>
{
    public string Name { get; init; }
        
    // for now hardcoded, in the future generated.
    public TestEntity(TestEntityId id, string name) : base(id)
    {
        Name = name;
    }

    // for now hardcoded, in the future generated.
    public TestEntity(string name)
    {
        Name = name;
    }
}