using Toarnbeike.Immutable.Mutations;
using Toarnbeike.Immutable.Tests.TestData;
using Toarnbeike.Results.TestHelpers;

namespace Toarnbeike.Immutable.Tests.Repositories;

/// <summary>
/// Tests specifically to verify that the provided Undo and Redo methods in the mutations are correct.
/// </summary>
public class AggregateRepositoryUndoRedoTests
{
    private readonly MutationStore _mutationStore = new();
    private readonly TestAggregateRepository _repo;

    public AggregateRepositoryUndoRedoTests()
    {
        _repo = new TestAggregateRepository(_mutationStore);
    }

    [Fact]
    public void Undo_Should_RevertAdd()
    {
        var entity = TestEntity.CreateNew("Alice");
        _repo.Add(entity);
        
        var result = _mutationStore.Undo();
        result.ShouldBeSuccess();
        
        _repo.GetById(entity.Id).ShouldBeFailure();
    }

    [Fact]
    public void Redo_Should_ReapplyAdd_AfterUndo()
    {
        var entity = TestEntity.CreateNew("Alice");
        _repo.Add(entity);
        _mutationStore.Undo();
        
        var result = _mutationStore.Redo();
        result.ShouldBeSuccess();
        
        _repo.GetById(entity.Id).ShouldBeSuccess();
    }

    [Fact]
    public void Undo_Should_RevertUpdate()
    {
        var original = TestEntity.CreateNew("Alice");
        _repo.Add(original);
        
        var updated = original with { Name = "Bob" };
        _repo.Update(updated);
        
        var result = _mutationStore.Undo();
        result.ShouldBeSuccess();
        
        _repo.GetById(original.Id).ShouldBeSuccessWithValue(original);
    }

    [Fact]
    public void Redo_Should_ReapplyUpdate_AfterUndo()
    {
        var original = TestEntity.CreateNew("Alice");
        _repo.Add(original);
        
        var updated =  original with { Name = "Bob" };
        _repo.Update(updated);
        
        _mutationStore.Undo();
        var result = _mutationStore.Redo();
        
        result.ShouldBeSuccess();
        _repo.GetById(original.Id).ShouldBeSuccessWithValue(updated);
    }

    [Fact]
    public void Undo_Should_RevertRemove()
    {
        var entity = TestEntity.CreateNew("Alice");
        _repo.Add(entity);
        _repo.Remove(entity.Id);
        
        var result = _mutationStore.Undo();
        result.ShouldBeSuccess();
        
        _repo.GetById(entity.Id).ShouldBeSuccess();
    }

    [Fact]
    public void Redo_Should_RevertRemove_AfterUndo()
    {
        var entity = TestEntity.CreateNew("Alice");
        _repo.Add(entity);
        _repo.Remove(entity.Id);
        
        _mutationStore.Undo();
        var result = _mutationStore.Redo();
        
        result.ShouldBeSuccess();
        _repo.GetById(entity.Id).ShouldBeFailure();
    }
}