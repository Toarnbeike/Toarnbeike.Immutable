using Toarnbeike.Immutable.Mutations;
using Toarnbeike.Results.Failures;
using Toarnbeike.Results.TestHelpers;

namespace Toarnbeike.Immutable.Tests.Mutations;

public class NoOpMutationStoreTests
{
    private readonly IMutation _testMutation = Substitute.For<IMutation>();
    private readonly NoOpMutationStore _mutationStore = new();
    
    [Fact]
    public void RegisterMutation_Should_DoNothing()
    {
        Should.NotThrow(() => _mutationStore.RegisterMutation(_testMutation));
        _testMutation.ReceivedCalls().Count().ShouldBe(0);
    }

    [Fact]
    public void CanUndo_ShouldBeFalse()
    {
        _mutationStore.CanUndo.ShouldBeFalse();
    }

    [Fact]
    public void CanRedo_ShouldBeFalse()
    {
        _mutationStore.CanRedo.ShouldBeFalse();
    }

    [Fact]
    public void Undo_Should_ReturnFailureResult()
    {
        var result = _mutationStore.Undo();
        var exceptionFailure = result.ShouldBeFailureOfType<ExceptionFailure>();
        exceptionFailure.Message.ShouldBe("Mutation tracking is disabled.");
    }
    
    [Fact]
    public void Redo_Should_ReturnFailureResult()
    {
        var result = _mutationStore.Redo();
        var exceptionFailure = result.ShouldBeFailureOfType<ExceptionFailure>();
        exceptionFailure.Message.ShouldBe("Mutation tracking is disabled.");
    }
}