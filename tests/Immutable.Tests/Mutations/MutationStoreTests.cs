using Toarnbeike.Immutable.Mutations;
using Toarnbeike.Results;
using Toarnbeike.Results.TestHelpers;

namespace Toarnbeike.Immutable.Tests.Mutations;

public class MutationStoreTests
{
    private readonly MutationStore _mutationStore = new();
    private readonly IMutation _mockMutation1 = Substitute.For<IMutation>();
    private readonly IMutation _mockMutation2 = Substitute.For<IMutation>();
    private readonly IMutation _mockMutation3 = Substitute.For<IMutation>();

    [Fact]
    public void RegisterMutation_Should_EnableUndo()
    {
        _mutationStore.RegisterMutation(_mockMutation1);
        _mutationStore.CanUndo.ShouldBeTrue();
    }

    [Fact]
    public void RegisterMutation_Should_DisableRedo()
    {
        _mockMutation1.Undo().Returns(Result.Success());
        _mutationStore.RegisterMutation(_mockMutation1);
        _mutationStore.Undo();
        _mutationStore.CanRedo.ShouldBeTrue(); // Verify redo is available
        
        _mutationStore.RegisterMutation(_mockMutation2);
        
        _mutationStore.CanRedo.ShouldBeFalse();
        _mutationStore.CanUndo.ShouldBeTrue();
    }
    
    [Fact]
    public void CanUndo_ShouldBeFalse_WhenNoMutationsAreAdded()
    {
        _mutationStore.CanUndo.ShouldBeFalse();
    }

    [Fact]
    public void Undo_Should_ReturnFailure_WhenNoMutationsAreAdded()
    {
        var result = _mutationStore.Undo();

        result.ShouldBeFailureWithCodeAndMessage("Undo.NotPossible","No undo information available.");
    }

    [Fact]
    public void Undo_Should_CallMutationUndoMethod()
    {
        _mockMutation1.Undo().Returns(Result.Success());
        _mutationStore.RegisterMutation(_mockMutation1);

        var result = _mutationStore.Undo();

        result.IsSuccess.ShouldBeTrue();
        _mockMutation1.Received(1).Undo();
    }
    
    [Fact]
    public void Undo_Should_EnableRedo()
    {
        _mockMutation1.Undo().Returns(Result.Success());
        _mutationStore.RegisterMutation(_mockMutation1);

        _mutationStore.Undo();

        _mutationStore.CanRedo.ShouldBeTrue();
        _mutationStore.CanUndo.ShouldBeFalse();
    }
    
    [Fact]
    public void Undo_Should_ProcessMultiple_InLIFO_Order()
    {
        _mockMutation1.Undo().Returns(Result.Success());
        _mockMutation2.Undo().Returns(Result.Success());
        _mockMutation3.Undo().Returns(Result.Success());
        
        _mutationStore.RegisterMutation(_mockMutation1);
        _mutationStore.RegisterMutation(_mockMutation2);
        _mutationStore.RegisterMutation(_mockMutation3);

        _mutationStore.Undo();
        _mutationStore.Undo();
        _mutationStore.Undo();

        Received.InOrder(() =>
        {
            _mockMutation3.Undo();
            _mockMutation2.Undo();
            _mockMutation1.Undo();
        });
    }
    
    [Fact]
    public void Undo_Should_ReturnFailure_WhenMutationFails()
    {
        var expectedFailure = new Failure("Test.Error", "Mutation failed");
        _mockMutation1.Undo().Returns(expectedFailure);
        _mutationStore.RegisterMutation(_mockMutation1);

        var result = _mutationStore.Undo();

        result.IsFailure.ShouldBeTrue();
        result.ShouldBeFailureThatSatisfiesPredicate(failure => failure == expectedFailure);
    }

    [Fact]
    public void Undo_Should_NotMoveToRedoStack_WhenMutationFails()
    {
        _mockMutation1.Undo().Returns(new Failure("Test.Error", "Mutation failed"));
        _mutationStore.RegisterMutation(_mockMutation1);

        _mutationStore.Undo();

        _mutationStore.CanRedo.ShouldBeFalse(); // Mutation can't be redone, no need to be in the redo stack
        _mutationStore.CanRedo.ShouldBeFalse(); // Should not be moved to redo stack
    }
    
    [Fact]
    public void CanRedo_ShouldBeFalse_WhenNoMutationsAreAdded()
    {
        _mutationStore.CanRedo.ShouldBeFalse();
    }
    
    [Fact]
    public void Redo_Should_ReturnFailure_WhenNoMutationsAreAdded()
    {
        var result = _mutationStore.Redo();

        result.ShouldBeFailureWithCodeAndMessage("Redo.NotPossible","No redo information available.");
    }
    
    [Fact]
    public void Redo_Should_CallMutationRedoMethod()
    {
        _mockMutation1.Undo().Returns(Result.Success());
        _mockMutation1.Redo().Returns(Result.Success());
        _mutationStore.RegisterMutation(_mockMutation1);
        _mutationStore.Undo();

        var result = _mutationStore.Redo();

        result.IsSuccess.ShouldBeTrue();
        _mockMutation1.Received(1).Redo();
    }

    [Fact]
    public void Redo_Should_MoveMutation_BackToUndoStack()
    {
        _mockMutation1.Undo().Returns(Result.Success());
        _mockMutation1.Redo().Returns(Result.Success());
        _mutationStore.RegisterMutation(_mockMutation1);
        _mutationStore.Undo();

        _mutationStore.Redo();

        _mutationStore.CanUndo.ShouldBeTrue();
        _mutationStore.CanRedo.ShouldBeFalse();
    }

    [Fact]
    public void Redo_Multiple_Should_Process_In_LIFO_Order()
    {
        _mockMutation1.Undo().Returns(Result.Success());
        _mockMutation2.Undo().Returns(Result.Success());
        _mockMutation3.Undo().Returns(Result.Success());
        _mockMutation1.Redo().Returns(Result.Success());
        _mockMutation2.Redo().Returns(Result.Success());
        _mockMutation3.Redo().Returns(Result.Success());
        
        _mutationStore.RegisterMutation(_mockMutation1);
        _mutationStore.RegisterMutation(_mockMutation2);
        _mutationStore.RegisterMutation(_mockMutation3);
        
        // Undo all
        _mutationStore.Undo(); // Undoes mutation3
        _mutationStore.Undo(); // Undoes mutation2
        _mutationStore.Undo(); // Undoes mutation1

        // Redo all
        _mutationStore.Redo(); // Should redo mutation1
        _mutationStore.Redo(); // Should redo mutation2
        _mutationStore.Redo(); // Should redo mutation3

        Received.InOrder(() =>
        {
            _mockMutation1.Redo(); // First undone, first redone
            _mockMutation2.Redo();
            _mockMutation3.Redo();
        });
    }

    [Fact]
    public void Redo_Should_ReturnFailure_WhenMutationFails()
    {
        var expectedFailure = new Failure("Test.Error", "Redo failed");
        _mockMutation1.Undo().Returns(Result.Success());
        _mockMutation1.Redo().Returns(Result.Failure(expectedFailure));
        _mutationStore.RegisterMutation(_mockMutation1);
        _mutationStore.Undo();

        var result = _mutationStore.Redo();

        result.IsFailure.ShouldBeTrue();
        result.ShouldBeFailureThatSatisfiesPredicate(failure => failure == expectedFailure);
    }

    [Fact]
    public void Redo_Should_NotMoveToUndoStack_WhenMutationFails()
    {
        _mockMutation1.Undo().Returns(Result.Success());
        _mockMutation1.Redo().Returns(new Failure("Test.Error", "Redo failed"));
        _mutationStore.RegisterMutation(_mockMutation1);
        _mutationStore.Undo();

        _mutationStore.Redo();

        _mutationStore.CanRedo.ShouldBeFalse(); // Mutation can't be redone, no need to be in the redo stack
        _mutationStore.CanUndo.ShouldBeFalse(); // Should not be moved to undo stack
    }
    
    [Fact]
    public void Undo_Redo_Combination_Should_WorkCorrectly()
    {
        // Arrange
        _mockMutation1.Undo().Returns(Result.Success());
        _mockMutation2.Undo().Returns(Result.Success());
        _mockMutation3.Undo().Returns(Result.Success());
        _mockMutation1.Redo().Returns(Result.Success());
        _mockMutation2.Redo().Returns(Result.Success());
        _mockMutation3.Redo().Returns(Result.Success());

        // Act - Build up mutation history
        _mutationStore.RegisterMutation(_mockMutation1);
        _mutationStore.RegisterMutation(_mockMutation2);
        
        // Undo one
        _mutationStore.Undo(); // Undo mutation2
        _mutationStore.CanUndo.ShouldBeTrue();  // mutation1 still available
        _mutationStore.CanRedo.ShouldBeTrue();  // mutation2 can be redone
        
        // Add new mutation (should clear redo stack)
        _mutationStore.RegisterMutation(_mockMutation3);
        _mutationStore.CanRedo.ShouldBeFalse(); // mutation2 should be gone
        
        // Undo twice
        _mutationStore.Undo(); // Undo mutation3
        _mutationStore.Undo(); // Undo mutation1
        _mutationStore.CanUndo.ShouldBeFalse();
        _mutationStore.CanRedo.ShouldBeTrue();
        
        // Redo once
        _mutationStore.Redo(); // Redo mutation1
        _mutationStore.CanUndo.ShouldBeTrue();
        _mutationStore.CanRedo.ShouldBeTrue(); // mutation3 still available for redo

        // Assert final verification
        _mockMutation1.Received(1).Undo();
        _mockMutation1.Received(1).Redo();
        _mockMutation2.Received(1).Undo();
        _mockMutation2.Received(0).Redo(); // Was cleared by new mutation
        _mockMutation3.Received(1).Undo();
        _mockMutation3.Received(0).Redo();
    }

    [Fact]
    public void Branching_Mutations_Should_ClearRedo()
    {
        // Arrange
        _mockMutation1.Undo().Returns(Result.Success());
        _mockMutation2.Undo().Returns(Result.Success());
        _mockMutation3.Undo().Returns(Result.Success());
        _mockMutation1.Redo().Returns(Result.Success());
        _mockMutation3.Redo().Returns(Result.Success());
        
        // Act - Create a branch in history
        _mutationStore.RegisterMutation(_mockMutation1);
        _mutationStore.RegisterMutation(_mockMutation2);
        
        // Undo to create redo history
        _mutationStore.Undo(); // mutation2 -> redo stack
        
        // Branch by adding new mutation
        _mutationStore.RegisterMutation(_mockMutation3); // Should clear mutation2 from redo
        
        // Verify branching worked
        _mutationStore.CanUndo.ShouldBeTrue();  // mutation3 and mutation1 available
        _mutationStore.CanRedo.ShouldBeFalse(); // mutation2 should be gone forever
        
        // Undo all remaining
        _mutationStore.Undo(); // Undo mutation3
        _mutationStore.Undo(); // Undo mutation1
        
        // Assert - only mutation3 and mutation1 should be redoable
        _mutationStore.Redo(); // Should redo mutation1
        _mutationStore.Redo(); // Should redo mutation3
        
        _mockMutation1.Received(1).Redo();
        _mockMutation2.Received(0).Redo(); // Should never be redone
        _mockMutation3.Received(1).Redo();
    }
}