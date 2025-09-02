using Toarnbeike.Immutable.Entities;

namespace Toarnbeike.Immutable.Tests.TestData;

[Aggregate]
public sealed partial record TestEntity : Entity<TestEntityId>, IAggregate
{
    public string Name { get; init; }
}

// // partial source generated
// public partial record TestEntity
// {
//     /// <summary>
//     /// Create a new instance of the TestEntity.
//     /// </summary>
//     public static TestEntity CreateNew(string name) => 
//         new(name);
//     
//     /// <summary>
//     /// Recreate an existing instance of the TestEntity.
//     /// </summary>
//     public static TestEntity CreateExisting(TestEntityId id, string name) =>
//         new(id, name);
//     
//     private TestEntity(TestEntityId id, string name) : base(id)
//     {
//         Name = name;
//     }
//
//     private TestEntity(string name)
//     {
//         Name = name;
//     }
// }