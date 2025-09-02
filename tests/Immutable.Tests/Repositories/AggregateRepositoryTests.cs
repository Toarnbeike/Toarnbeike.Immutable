using Toarnbeike.Immutable.Mutations;
using Toarnbeike.Immutable.Tests.TestData;
using Toarnbeike.Optional.TestExtensions;
using Toarnbeike.Results.TestHelpers;

namespace Toarnbeike.Immutable.Tests.Repositories;

public class AggregateRepositoryTests
{
    private readonly IMutationStore _mockMutationStore = Substitute.For<IMutationStore>();
    private readonly ITestEntityRepository _repo;

    public AggregateRepositoryTests()
    {
        _repo = new TestEntityRepository(_mockMutationStore);
    }

    [Fact]
    public void Add_Should_AddEntityToCollection()
    {
        var entity = TestEntity.CreateNew("Alice");
        var result = _repo.Add(entity);
        
        result.ShouldBeSuccess();
        _repo.GetById(entity.Id).ShouldBeSuccessWithValue(entity);
    }

    [Fact]
    public void Add_Should_RegisterMutation()
    {
        var entity = TestEntity.CreateNew("Alice");
        var result = _repo.Add(entity);
        
        result.ShouldBeSuccess();
        _mockMutationStore.Received(1).RegisterMutation(Arg.Any<Mutation>());
    }
    
    [Fact]
    public void Add_Should_ReturnFailure_WhenIdAlreadyExists()
    {
        var id = TestEntityId.New();
        var entity1 = TestEntity.CreateExisting(id, "Alice");
        var entity2 = TestEntity.CreateExisting(id, "Bob");
        
        _repo.Add(entity1);
        
        var result = _repo.Add(entity2);
        result.ShouldBeFailureWithCodeAndMessage(nameof(TestEntity), $"Entity with id {id} already exists.");
        
        _mockMutationStore.Received(1).RegisterMutation(Arg.Any<Mutation>()); // once for adding Alice, none for adding Bob.
    }

    [Fact]
    public void Update_Should_UpdateEntity()
    {
        var entity = TestEntity.CreateNew("Alice");
        _repo.Add(entity);
        
        var updated = entity with { Name = "Bob" };
        var result = _repo.Update(updated);
        
        result.ShouldBeSuccess();
        _repo.GetById(entity.Id).ShouldBeSuccessWithValue(updated);
    }

    [Fact]
    public void Update_Should_RegisterMutation()
    {
        var entity = TestEntity.CreateNew("Alice");
        _repo.Add(entity);
        
        var updated = entity with { Name = "Bob" };
        var result = _repo.Update(updated);
        
        result.ShouldBeSuccess();
        // once for adding Alice, once for updating to Bob
        _mockMutationStore.Received(2).RegisterMutation(Arg.Any<Mutation>());
    }
    
    [Fact]
    public void Update_Should_ReturnFailure_WhenIdDoesNotExist()
    {
        var entity = TestEntity.CreateNew("Alice");
        
        var result =  _repo.Update(entity);
        
        result.ShouldBeFailureWithCodeAndMessage(nameof(TestEntity), $"Entity with id {entity.Id} not found.");
        _mockMutationStore.Received(0).RegisterMutation(Arg.Any<Mutation>());
    }

    [Fact]
    public void Remove_Should_RemoveEntityFromCollection()
    {
        var entity = TestEntity.CreateNew("Alice");
        _repo.Add(entity);

        var result = _repo.Remove(entity.Id);
        result.ShouldBeSuccess();
        
        _repo.GetById(entity.Id).ShouldBeFailure();
    }

    [Fact]
    public void Remove_Should_RegisterMutation()
    {
        var entity = TestEntity.CreateNew("Alice");
        _repo.Add(entity);
        
        var result = _repo.Remove(entity.Id);
        result.ShouldBeSuccess();
        
        // once for adding Alice, once for removing Alice.
        _mockMutationStore.Received(2).RegisterMutation(Arg.Any<Mutation>());
    }

    [Fact]
    public void Remove_Should_ReturnFailure_WhenIdDoesNotExist()
    {
        var entity = TestEntity.CreateNew("Alice");
        
        var result = _repo.Remove(entity.Id);
        
        result.ShouldBeFailureWithCodeAndMessage(nameof(TestEntity),  $"Entity with id {entity.Id} not found.");
    }
    
    [Fact]
    public void GetById_Should_ReturnEntity()
    {
        var entity = TestEntity.CreateNew("Alice");
        _repo.Add(entity);
        
        var result = _repo.GetById(entity.Id);
        result.ShouldBeSuccessWithValue(entity);
    }

    [Fact]
    public void GetById_Should_ReturnFailure_WhenIdDoesNotExist()
    {
        var id = TestEntityId.Empty;
        
        var result =  _repo.GetById(id);
        
        result.ShouldBeFailureWithCodeAndMessage(nameof(TestEntity),  $"Entity with id {id} not found.");
    }

    [Fact]
    public void FirstOrNone_Should_ReturnEntity()
    {
        var entity = TestEntity.CreateNew("Alice");
        _repo.Add(entity);

        var result = _repo.FirstOrNone(e => e.Name == "Alice");

        result.ShouldBeSomeWithValue(entity);
    }

    [Fact]
    public void FirstOrNone_Should_ReturnNone_WhenNoEntityIsFound()
    {
        var entity = TestEntity.CreateNew("Alice");
        _repo.Add(entity);
        
        var result = _repo.FirstOrNone(e => e.Name == "Bob");
        result.ShouldBeNone();
    }

    [Fact]
    public void FirstOrNone_Should_ReturnFirstInstance_WhenMultipleAreFound()
    {
        _repo.Add(TestEntity.CreateNew("Alice"));
        _repo.Add(TestEntity.CreateNew("Amanda"));
        
        var result = _repo.FirstOrNone(e => e.Name.StartsWith('A'));
        
        result.ShouldBeSomeThatMatches(entity => entity.Name.StartsWith('A'));
    }

    [Fact]
    public void GetAll_Should_ReturnAllEntities()
    {
        _repo.Add(TestEntity.CreateNew("Alice"));
        _repo.Add(TestEntity.CreateNew("Bob"));
        _repo.Add(TestEntity.CreateNew("Charlie"));
        
        var result = _repo.GetAll();
        
        result.Count().ShouldBe(3);
    }

    [Fact]
    public void GetAll_Should_ReturnEmpty_WhenCollectionIsEmpty()
    {
        var result = _repo.GetAll();
        result.ShouldBeEmpty();
    }
    
    [Fact]
    public void GetWhere_Should_ReturnAllEntities_ThatMatchPredicate()
    {
        _repo.Add(TestEntity.CreateNew("Alice"));
        _repo.Add(TestEntity.CreateNew("Amanda"));
        _repo.Add(TestEntity.CreateNew("Bob"));
        
        var result = _repo.Where(e => e.Name.StartsWith('A'));
        result.Count().ShouldBe(2);
    }

    [Fact]
    public void GetWhere_Should_ReturnEmpty_WhenNoEntitiesAreFound()
    {
        _repo.Add(TestEntity.CreateNew("Alice"));
        _repo.Add(TestEntity.CreateNew("Amanda"));
        _repo.Add(TestEntity.CreateNew("Bob"));
        
        var result = _repo.Where(e => e.Name.StartsWith('C'));
        result.ShouldBeEmpty();
    }
    
    [Fact]
    public void GetWhere_Should_ReturnEmpty_WhenCollectionIsEmpty()
    {
        var result = _repo.Where(e => e.Name.StartsWith('C'));
        result.ShouldBeEmpty();
    }
}