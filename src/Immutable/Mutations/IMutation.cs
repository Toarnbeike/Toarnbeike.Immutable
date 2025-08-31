using Toarnbeike.Optional;
using Toarnbeike.Results;

namespace Toarnbeike.Immutable.Mutations;

/// <summary>
/// Represents an operation that can be undone and redone.
/// </summary>
public interface IMutation
{
    /// <summary>
    /// Optional description of the operation, for display purposes.
    /// </summary>
    Option<string> ChangeDescription { get; }
    
    /// <summary>
    /// Reverts the operation that was previously applied.
    /// </summary>
    Result Undo();

    /// <summary>
    /// Re-applies the operation in case it was undone.
    /// </summary>
    Result Redo();
}