using Toarnbeike.Immutable.Mutations;
using Toarnbeike.Immutable.Repositories;

namespace Toarnbeike.Immutable.Tests.TestData;

internal sealed class TestAggregateRepository : AggregateRepository<TestEntity, TestEntityId>
{
    public TestAggregateRepository(IMutationStore mutationStore)
        : base(mutationStore)
    {
        Entities = new Dictionary<TestEntityId, TestEntity>();
    }
}