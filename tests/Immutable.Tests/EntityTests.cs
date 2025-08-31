using Toarnbeike.Immutable.Entities;

namespace Toarnbeike.Immutable.Tests;

public class EntityTests
{
    private readonly record struct TestEntityId(Guid Value) : IEntityKey<TestEntityId>
    {
        public static TestEntityId New() => new(Guid.CreateVersion7());
    }

    private sealed record TestEntity : Entity<TestEntityId>
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
    
    [Fact]
    public void Constructor_Should_AddId_OfCorrectVersion()
    {
        var entity = new TestEntity("Test");
        entity.Id.Value.ShouldNotBe(Guid.Empty);
        entity.Id.Value.Version.ShouldBe(7);
    }

    [Fact]
    public void GetId_Should_ReturnGuid()
    {
        var entity = new TestEntity("Test");
        
        var result = entity.GetId();
        
        result.ShouldBe(entity.Id.Value);
        result.ShouldBeOfType<Guid>();
    }

    [Fact]
    public void Entity_Should_BeConstructableWithExistingId()
    {
        var existingId = TestEntityId.New();
        var entity = new TestEntity(existingId, "Test");

        entity.Id.ShouldBe(existingId);
        entity.Name.ShouldBe("Test");
    }
    
    [Fact]
    public void Entity_Should_BeUpdateable()
    {
        var entity = new TestEntity("Test");
        var newEntity = entity with { Name = "New" };
        
        newEntity.Name.ShouldBe("New");
        newEntity.Id.ShouldBe(entity.Id);
        
        entity.Name.ShouldBe("Test");
    }
}