using DSAExperimentation.Algorithms.GameSearch;
using DSAExperimentation.Tests.Algorithms.GameSearch.Fixtures;

namespace DSAExperimentation.Tests.Algorithms.GameSearch;

// Companion of ExcludedOptions, the recurrence SpragueGrundy hands Memoizer: a position's number is
// the minimum excludant of its options' numbers. It is private to SpragueGrundy, so it is driven
// through SpragueGrundy.Number.
public sealed partial class ExcludedOptionsTests
{
    [Fact]
    public void Replay_PositionWithNoMoves_IsZero() => Assert.Equal(0, SpragueGrundy.Number(new NimHeap(), 0));

    // A heap of two moves to heaps of one and none, numbered 1 and 0, so its own number is 2.
    [Fact]
    public void Replay_Position_IsTheMinimumExcludantOfItsOptions() =>
        Assert.Equal(2, SpragueGrundy.Number(new NimHeap(), 2));
}
