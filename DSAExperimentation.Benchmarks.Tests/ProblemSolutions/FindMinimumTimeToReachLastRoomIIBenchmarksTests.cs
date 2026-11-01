using DSAExperimentation.Benchmarks.ProblemSolutions;

namespace DSAExperimentation.Benchmarks.Tests.ProblemSolutions;

// Harness coverage for FindMinimumTimeToReachLastRoomIIBenchmarks (ARCHITECTURE 17.9): the generic
// arms check only compares the arms with each other, so this bounds the baseline's answer by the
// workload itself.
//
// Setup's grid gives the origin moveTime 0 and every other cell a random wait, so the answer is the
// earliest arrival at the far corner and is positive on every run: the path to it is at least one
// move long, and the first move alone costs a second.
public sealed partial class FindMinimumTimeToReachLastRoomIIBenchmarksTests
{
    // The smaller of Setup's [Params(20, 60)] grid sides.
    private const int SmallestSize = 20;

    [Fact]
    public void BclPriorityQueue_AlternatingCostGrid_ReturnsAPositiveTime() =>
        Assert.True(BuildHarness().BclPriorityQueue() > 0);

    private static FindMinimumTimeToReachLastRoomIIBenchmarks BuildHarness()
    {
        var harness = new FindMinimumTimeToReachLastRoomIIBenchmarks { Size = SmallestSize };
        harness.Setup();

        return harness;
    }
}
