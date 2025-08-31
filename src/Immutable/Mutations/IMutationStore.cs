using Toarnbeike.Results;

namespace Toarnbeike.Immutable.Mutations;

/// <summary>
/// Provides storage and control for applied mutations,
/// allowing undo and redo functionality.
/// </summary>
public interface IMutationStore
{
    /// <summary>
    /// Registers a new mutation in the store.  
    /// This clears the redo stack, as redo history is only valid until a new mutation is introduced.
    /// </summary>
    /// <param name="mutation">The mutation to register.</param>
    void RegisterMutation(IMutation mutation);
    
    /// <summary>
    /// Gets a value indicating whether there is a mutation available to undo.
    /// </summary>
    bool CanUndo { get; }
    
    /// <summary>
    /// Gets a value indicating whether there is a mutation available to redo.
    /// </summary>
    bool CanRedo { get; }

    /// <summary>
    /// Undoes the most recently applied mutation, if available.
    /// </summary>
    Result Undo();

    /// <summary>
    /// Redoes the most recently undone mutation, if available.
    /// </summary>
    Result Redo();
}