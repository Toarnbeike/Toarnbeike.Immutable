using Toarnbeike.Immutable.Tests.TestData;

namespace Toarnbeike.Immutable.Tests.Entities;

public class EntityKeyTests
{
    [Fact]
    public void New_Should_CreateNonEmptyGuid()
    {
        var actual = TestEntityId.New();
        actual.Value.ShouldNotBe(Guid.Empty);
    }

    [Fact]
    public void New_Should_CreateDifferentGuids()
    {
        var first = TestEntityId.New();
        var second = TestEntityId.New();
        first.Value.ShouldNotBe(second.Value);
    }
    
    [Fact]
    public void New_Should_CreateGuidVersion7()
    {
        var actual = TestEntityId.New();
        actual.Value.Version.ShouldBe(7);
    }
    
    [Fact]
    public void Empty_Should_CreateEmptyGuid()
    {
        var actual = TestEntityId.Empty;
        actual.Value.ShouldBe(Guid.Empty);
    }

    [Fact]
    public void Value_Should_BeSettable()
    {
        var existing = TestEntityId.New();
        var actual = new TestEntityId(existing.Value);
        
        actual.Value.ShouldBe(existing.Value);
    }

    [Fact]
    public void Value_Should_BeOverrideable()
    {
        var existing = TestEntityId.New();
        var actual = TestEntityId.New() with { Value = existing.Value };
        
        actual.Value.ShouldBe(existing.Value);
    }
}