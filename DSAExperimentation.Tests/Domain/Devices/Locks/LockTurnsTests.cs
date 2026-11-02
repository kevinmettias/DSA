using DSAExperimentation.DataStructures.Graph.States;
using DSAExperimentation.Domain.Devices.Locks;

namespace DSAExperimentation.Tests.Domain.Devices.Locks;

public sealed partial class LockTurnsTests
{
    [Fact]
    public void StateCount_IsEveryCombination() => Assert.Equal(10_000, new LockTurns<FourDigitLock>().StateCount);

    [Fact]
    public void MaxSuccessors_IsTwoTurnsPerWheel() => Assert.Equal(8, new LockTurns<FourDigitLock>().MaxSuccessors);

    [Fact]
    public void StateAt_IsTheNumberedCombination() => Assert.Equal("0042", new LockTurns<FourDigitLock>().StateAt(42));

    [Fact]
    public void WriteSuccessors_AreTheCombinationsOneTurnAway()
    {
        var successors = new string[8];

        var written = new LockTurns<FourDigitLock>().WriteSuccessors("0009", successors);

        Assert.Equal(8, written);
        Assert.Equal(["1009", "9009", "0109", "0909", "0019", "0099", "0000", "0008"], successors);
    }

    // Every turn can be undone, so the graph of the whole lock joins each combination to all eight
    // others one turn away, and each of them back.
    [Fact]
    public void StateGraph_OfTheWholeLock_JoinsEveryCombinationToEightOthersBothWays()
    {
        var graph = StateGraph<string>.Build(new LockTurns<FourDigitLock>(), new EveryState<string>());

        Assert.Equal(10_000, graph.Count);
        Assert.True(graph.TryGetNode("0000", out var start));
        Assert.Equal(8, start.Successors.Count);
        Assert.All(start.Successors, successor => Assert.Contains(start, successor.Successors));
    }
}
