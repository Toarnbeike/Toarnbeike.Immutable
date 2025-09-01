using Toarnbeike.Immutable.Entities;

namespace Toarnbeike.Immutable.Tests.TestData;

internal readonly record struct TestEntityId(Guid Value) : IEntityKey<TestEntityId>
{
    public static TestEntityId New() => new(Guid.CreateVersion7());
}