using DSAExperimentation.Benchmarks.ProblemSolutions;

namespace DSAExperimentation.Benchmarks.Tests.ProblemSolutions;

// Harness coverage for MinimizeTheMaximumEdgeWeightOfGraphBenchmarks (ARCHITECTURE 17.9): its two arms are
// competing strategies for the same question - binary search over the weight threshold with a BFS
// feasibility check against reducing the graph to its qualifying edges and searching that - so a harness
// whose arms disagree is timing two different problems. Both arms return the minimum feasible maximum edge
// weight as an int, so they are compared directly, and both are handed the same prepared EdgeWeightGraph,
// so agreeing on the number also pins that neither arm re-derives a different adjacency from it. Neither
// arm mutates the graph, so one harness is safe to read twice in either order; Setup builds it from one
// fixed seed, so the same NodeCount must rebuild the same graph.
public sealed partial class MinimizeTheMaximumEdgeWeightOfGraphBenchmarksTests
{
    private const int SmallestNodeCount = 100;

    [Fact]
    public void Setup_SameNodeCount_RebuildsTheSameWorkload() =>
        Assert.Equal(BuildHarness().BinarySearchBfs(), BuildHarness().BinarySearchBfs());

    [Fact]
    public void BinarySearchBfs_SeededWeightedGraph_AgreesWithReduceGraphBinarySearch()
    {
        var harness = BuildHarness();

        Assert.Equal(harness.ReduceGraphBinarySearch(), harness.BinarySearchBfs());
    }

    [Fact]
    public void ReduceGraphBinarySearch_SeededWeightedGraph_AgreesWithBinarySearchBfs()
    {
        var harness = BuildHarness();

        Assert.Equal(harness.BinarySearchBfs(), harness.ReduceGraphBinarySearch());
    }

    private static MinimizeTheMaximumEdgeWeightOfGraphBenchmarks BuildHarness()
    {
        var harness = new MinimizeTheMaximumEdgeWeightOfGraphBenchmarks { NodeCount = SmallestNodeCount };
        harness.Setup();

        return harness;
    }
}
