using DSAExperimentation.Benchmarks.ProblemSolutions;

namespace DSAExperimentation.Benchmarks.Tests.ProblemSolutions;

// Harness coverage for FindMinimumTimeToReachLastRoomIBenchmarks (ARCHITECTURE 17.9): its two arms
// are competing strategies for the same question - the same wait-aware grid relaxation fronted by
// the BCL PriorityQueue against this repo's own Heap - so a harness whose arms disagree has relaxed
// two different arrival rules. Both answers are one int, so they are compared directly.
//
// Setup's grid gives the origin moveTime 0 (LeetCode's own guarantee) and every other cell a random
// wait, so the answer is the earliest arrival at the far corner and is positive on every run: the
// path to it is at least one move long, and each move costs a second.
public sealed partial class FindMinimumTimeToReachLastRoomIBenchmarksTests
{
    // The smaller of Setup's [Params(20, 60)] grid sides.
    private const int SmallestSize = 20;

    [Fact]
    public void Setup_SameSize_RebuildsTheSameMinimumTime() =>
        Assert.Equal(BuildHarness().BclPriorityQueue(), BuildHarness().BclPriorityQueue());

    [Fact]
    public void BclPriorityQueue_WaitCostGrid_AgreesWithHeap()
    {
        var harness = BuildHarness();

        Assert.True(harness.BclPriorityQueue() > 0);
        Assert.Equal(harness.Heap(), harness.BclPriorityQueue());
    }

    [Fact]
    public void Heap_WaitCostGrid_AgreesWithBclPriorityQueue()
    {
        var harness = BuildHarness();

        Assert.True(harness.Heap() > 0);
        Assert.Equal(harness.BclPriorityQueue(), harness.Heap());
    }

    private static FindMinimumTimeToReachLastRoomIBenchmarks BuildHarness()
    {
        var harness = new FindMinimumTimeToReachLastRoomIBenchmarks { Size = SmallestSize };
        harness.Setup();

        return harness;
    }
}
