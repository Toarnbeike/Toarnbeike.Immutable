using Toarnbeike.Optional;
using Toarnbeike.Optional.Extensions;
using Toarnbeike.Results;

namespace Toarnbeike.Immutable.Mutations;

/// <summary>
/// Represents an operation that can be undone and redone.
/// </summary>
/// <param name="UndoFunction">Function to invoke in order to undo the operation.</param>
/// <param name="RedoFunction">Function to invoke in order to redo the operation.</param>
/// <param name="Description">Optional: description of the change.</param>
public record Mutation(Func<Result> UndoFunction, Func<Result> RedoFunction, string? Description) : IMutation
{
    public Option<string> ChangeDescription => Description.AsOption();
    public Result Undo() => UndoFunction();
    public Result Redo() => RedoFunction();
}