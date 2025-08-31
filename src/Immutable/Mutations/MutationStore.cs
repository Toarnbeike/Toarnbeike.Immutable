using Toarnbeike.Results;
using Toarnbeike.Results.Extensions;

namespace Toarnbeike.Immutable.Mutations;

/// <summary>
/// Default in-memory implementation of <see cref="IMutationStore"/>.  
/// Uses two stacks to maintain undo and redo history.
/// </summary>
internal sealed class MutationStore : IMutationStore
{
    private readonly Stack<IMutation> _undoStack = new();
    private readonly Stack<IMutation> _redoStack = new();

    /// <inheritdoc/>
    public void RegisterMutation(IMutation mutation)
    {
        _undoStack.Push(mutation);
        _redoStack.Clear();
    }
    
    /// <inheritdoc/>
    public bool CanUndo => _undoStack.Count > 0;

    /// <inheritdoc/>
    public bool CanRedo => _redoStack.Count > 0;

    /// <inheritdoc/>
    public Result Undo()
    {
        return Result.Success(CanUndo)
            .Check(can => can, () => new Failure("Undo.NotPossible", "No undo information available."))
            .Map(_ => _undoStack.Pop())
            .Verify(mutation => mutation.Undo())
            .Tap(mutation => _redoStack.Push(mutation));
    }

    /// <inheritdoc/>
    public Result Redo()
    {
        return Result.Success(CanRedo)
            .Check(can => can , () => new Failure("Redo.NotPossible", "No redo information available."))
            .Map(_ => _redoStack.Pop())
            .Verify(mutation => mutation.Redo())
            .Tap(mutation => _undoStack.Push(mutation));
    }
}