using DSAExperimentation.Benchmarks.ProblemSolutions;

namespace DSAExperimentation.Benchmarks.Tests.ProblemSolutions;

// Harness coverage for MinimumTimeToReachDestinationInDirectedGraphBenchmarks (ARCHITECTURE 17.9): its
// two arms are competing strategies for the same question - the earliest arrival time at the last node
// under the edges' time windows - so a harness whose arms disagree is timing two different problems.
// Both arms are handed the TimeWindowAdjacency [GlobalSetup] already built, so the comparison also
// pins that this repo's Heap-backed frontier and the BCL priority queue relax the same adjacency list
// in a way that converges on the same arrival times: a front end that dropped or duplicated an edge
// would surface here. Setup builds that graph from one fixed seed, so the same NodeCount must rebuild
// the same time windows.
public sealed partial class MinimumTimeToReachDestinationInDirectedGraphBenchmarksTests
{
    private const int SmallestNodeCount = 200;

    [Fact]
    public void Setup_SameNodeCount_RebuildsTheSameWorkload() =>
        Assert.Equal(BuildHarness().BclPriorityQueue(), BuildHarness().BclPriorityQueue());

    [Fact]
    public void BclPriorityQueue_SameTimeWindowGraph_AgreesWithHeap()
    {
        var harness = BuildHarness();

        Assert.Equal(harness.Heap(), harness.BclPriorityQueue());
    }

    [Fact]
    public void Heap_SameTimeWindowGraph_AgreesWithBclPriorityQueue()
    {
        var harness = BuildHarness();

        Assert.Equal(harness.BclPriorityQueue(), harness.Heap());
    }

    private static MinimumTimeToReachDestinationInDirectedGraphBenchmarks BuildHarness()
    {
        var harness = new MinimumTimeToReachDestinationInDirectedGraphBenchmarks { NodeCount = SmallestNodeCount };
        harness.Setup();

        return harness;
    }
}
