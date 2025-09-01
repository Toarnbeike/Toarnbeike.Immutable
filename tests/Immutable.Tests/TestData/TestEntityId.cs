using Toarnbeike.Immutable.Entities;

namespace Toarnbeike.Immutable.Tests.TestData;

[EntityKey]
public readonly partial record struct TestEntityId(Guid Value) : IEntityKey;

// internal readonly partial record struct TestEntityId : IEntityKey<TestEntityId>
// {
//     public static TestEntityId New() => new(Guid.CreateVersion7());
//
// }