using DSAExperimentation.Benchmarks.ProblemSolutions;

namespace DSAExperimentation.Benchmarks.Tests.ProblemSolutions;

// Harness coverage for ReachableNodesInSubdividedGraphBenchmarks (ARCHITECTURE 17.9): its two arms
// are competing strategies for the same question - how many nodes are reachable within the move
// budget - so a harness whose arms disagree is timing two different problems. Both return the count
// as a scalar, and both are handed the pre-bounded move budget [GlobalSetup] computes, so one
// harness is safe to call twice in either order. Setup builds the edge list from one fixed seed and
// the composed arm's graph from that same list, so the same NodeCount must rebuild both.
public sealed partial class ReachableNodesInSubdividedGraphBenchmarksTests
{
    private const int SmallestNodeCount = 30;

    [Fact]
    public void Setup_SameNodeCount_RebuildsTheSameWorkload() =>
        Assert.Equal(BuildHarness().MaterializedBfs(), BuildHarness().MaterializedBfs());

    [Fact]
    public void MaterializedBfs_SeededSubdividedGraph_AgreesWithDijkstra()
    {
        var harness = BuildHarness();

        Assert.Equal(harness.MaterializedBfs(), harness.Dijkstra());
    }

    [Fact]
    public void Dijkstra_SeededSubdividedGraph_AgreesWithTheComposedArm()
    {
        var harness = BuildHarness();

        Assert.Equal(harness.Dijkstra(), harness.MaterializedBfs());
    }

    private static ReachableNodesInSubdividedGraphBenchmarks BuildHarness()
    {
        var harness = new ReachableNodesInSubdividedGraphBenchmarks { NodeCount = SmallestNodeCount };
        harness.Setup();

        return harness;
    }
}
