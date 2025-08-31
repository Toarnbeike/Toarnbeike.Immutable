using System.Runtime.CompilerServices;
using Toarnbeike.Results;
using Toarnbeike.Results.Failures;

namespace Toarnbeike.Immutable.Mutations;

/// <summary>
/// Dummy implementation of the <see cref="IMutationStore"/>
/// Does not store any mutations. Used when mutation tracking is explicitly disabled.
/// </summary>
internal sealed class NoOpMutationStore : IMutationStore
{
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void RegisterMutation(IMutation mutation) {}
    
    public bool CanUndo => false;
    public bool CanRedo => false;
    
    public Result Undo() => Result.Failure(new ExceptionFailure(new InvalidOperationException("Mutation tracking is disabled.")));
    public Result Redo() => Result.Failure(new ExceptionFailure(new InvalidOperationException("Mutation tracking is disabled.")));
}