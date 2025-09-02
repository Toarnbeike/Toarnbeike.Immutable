using Toarnbeike.Immutable.Tests.TestData;

namespace Toarnbeike.Immutable.Tests.Entities;

public class EntityTests
{
    [Fact]
    public void Constructor_Should_AddId_OfCorrectVersion()
    {
        var entity = TestEntity.CreateNew("Test");
        entity.Id.Value.ShouldNotBe(Guid.Empty);
        entity.Id.Value.Version.ShouldBe(7);
    }

    [Fact]
    public void GetId_Should_ReturnGuid()
    {
        var entity = TestEntity.CreateNew("Test");
        
        var result = entity.GetId();
        
        result.ShouldBe(entity.Id.Value);
        result.ShouldBeOfType<Guid>();
    }

    [Fact]
    public void Entity_Should_BeConstructableWithExistingId()
    {
        var existingId = TestEntityId.New();
        var entity = TestEntity.CreateExisting(existingId, "Test");

        entity.Id.ShouldBe(existingId);
        entity.Name.ShouldBe("Test");
    }
    
    [Fact]
    public void Entity_Should_BeUpdateable()
    {
        var entity = TestEntity.CreateNew("Test");
        var newEntity = entity with { Name = "New" };
        
        newEntity.Name.ShouldBe("New");
        newEntity.Id.ShouldBe(entity.Id);
        
        entity.Name.ShouldBe("Test");
    }
}