using Toarnbeike.Immutable.Mutations;
using Toarnbeike.Optional.TestExtensions;
using Toarnbeike.Results;
using Toarnbeike.Results.TestHelpers;

namespace Toarnbeike.Immutable.Tests.Mutations;

public class MutationTests
{
    [Fact]
    public void Constructor_Should_SetPropertiesCorrectly()
    {
        var undoFunc = Substitute.For<Func<Result>>();
        var redoFunc = Substitute.For<Func<Result>>();
        const string description = "Test operation";

        var mutation = new Mutation(undoFunc, redoFunc, description);

        mutation.UndoFunction.ShouldBe(undoFunc);
        mutation.RedoFunction.ShouldBe(redoFunc);
        mutation.Description.ShouldBe(description);
        mutation.ChangeDescription.ShouldBeSomeWithValue(description);
    }

    [Fact]
    public void Constructor_Should_UseNoneOption_WhenDescriptionIsNull()
    {
        var undoFunc = Substitute.For<Func<Result>>();
        var redoFunc = Substitute.For<Func<Result>>();

        var mutation = new Mutation(undoFunc, redoFunc, null);

        mutation.Description.ShouldBeNull();
        mutation.ChangeDescription.ShouldBeNone();
    }
    
    [Fact]
    public void Undo_Should_Invoke_UndoFunction()
    {
        var undoFunc = Substitute.For<Func<Result>>();
        var redoFunc = Substitute.For<Func<Result>>();
        undoFunc().Returns(Result.Success());

        var mutation = new Mutation(undoFunc, redoFunc, "Test");

        var result = mutation.Undo();

        undoFunc.Received(1)();
        result.ShouldBeSuccess();
    }
    
    [Fact]
    public void Redo_Should_Invoke_RedoFunction()
    {
        var undoFunc = Substitute.For<Func<Result>>();
        var redoFunc = Substitute.For<Func<Result>>();
        redoFunc().Returns(Result.Success());

        var mutation = new Mutation(undoFunc, redoFunc, "Test");

        var result = mutation.Redo();

        redoFunc.Received(1)();
        result.ShouldBeSuccess();
    }
    
    // ReSharper disable AccessToModifiedClosure
    [Fact]
    public void Integration()
    {
        // Arrange - Simulate a counter increment/decrement operation
        var counter = 0;
        const int incrementAmount = 5;

        var mutation = new Mutation(UndoFunc, RedoFunc, $"Increment counter by {incrementAmount}");

        // Simulate the original operation that created this mutation
        counter += incrementAmount; // counter = 5

        // Act & Assert - Test undo
        var undoResult = mutation.Undo();
        undoResult.IsSuccess.ShouldBeTrue();
        counter.ShouldBe(0);

        // Act & Assert - Test redo
        var redoResult = mutation.Redo();
        redoResult.IsSuccess.ShouldBeTrue();
        counter.ShouldBe(5);

        return;

        Result UndoFunc()
        {
            counter -= incrementAmount;
            return Result.Success();
        }

        Result RedoFunc()
        {
            counter += incrementAmount;
            return Result.Success();
        }
    }
}