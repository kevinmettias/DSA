using DSAExperimentation.Benchmarks.ProblemSolutions;

namespace DSAExperimentation.Benchmarks.Tests.ProblemSolutions;

// Harness coverage for SecondMinimumTimeToReachDestinationBenchmarks (ARCHITECTURE 17.9): both arms
// are the same dual-distance BFS over the same prepared intersection network, differing only in the
// frontier they pop from, so a harness whose arms disagree is timing two different walks. Setup
// builds a single cycle over every intersection with no randomness, so the same NodeCount must
// rebuild the same network; neither arm mutates it, so one harness instance is safe to read twice in
// either order.
public sealed partial class SecondMinimumTimeToReachDestinationBenchmarksTests
{
    private const int SmallestNodeCount = 200;

    [Fact]
    public void Setup_SameNodeCount_RebuildsTheSameWorkload() =>
        Assert.Equal(BuildHarness().ListFrontierBfs(), BuildHarness().ListFrontierBfs());

    [Fact]
    public void ListFrontierBfs_SingleCycleGraph_AgreesWithQueueFrontierBfs()
    {
        var harness = BuildHarness();

        Assert.Equal(harness.QueueFrontierBfs(), harness.ListFrontierBfs());
    }

    [Fact]
    public void QueueFrontierBfs_SingleCycleGraph_AgreesWithListFrontierBfs()
    {
        var harness = BuildHarness();

        Assert.Equal(harness.ListFrontierBfs(), harness.QueueFrontierBfs());
    }

    private static SecondMinimumTimeToReachDestinationBenchmarks BuildHarness()
    {
        var harness = new SecondMinimumTimeToReachDestinationBenchmarks { NodeCount = SmallestNodeCount };
        harness.Setup();

        return harness;
    }
}
