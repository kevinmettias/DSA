using DSAExperimentation.Benchmarks.ProblemSolutions;

namespace DSAExperimentation.Benchmarks.Tests.ProblemSolutions;

// Harness coverage for FindMinimumTimeToReachLastRoomIBenchmarks (ARCHITECTURE 17.9): the generic
// arms check only compares the arms with each other, so this bounds the baseline's answer by the
// workload itself.
//
// Setup's grid gives the origin moveTime 0 (LeetCode's own guarantee) and every other cell a random
// wait, so the answer is the earliest arrival at the far corner and is positive on every run: the
// path to it is at least one move long, and each move costs a second.
public sealed partial class FindMinimumTimeToReachLastRoomIBenchmarksTests
{
    // The smaller of Setup's [Params(20, 50)] grid sides.
    private const int SmallestSize = 20;

    [Fact]
    public void BclPriorityQueue_WaitCostGrid_ReturnsAPositiveTime() =>
        Assert.True(BuildHarness().BclPriorityQueue() > 0);

    private static FindMinimumTimeToReachLastRoomIBenchmarks BuildHarness()
    {
        var harness = new FindMinimumTimeToReachLastRoomIBenchmarks { Size = SmallestSize };
        harness.Setup();

        return harness;
    }
}
