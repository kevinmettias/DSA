using DSAExperimentation.Benchmarks.ProblemSolutions;

namespace DSAExperimentation.Benchmarks.Tests.ProblemSolutions;

// Harness coverage for ShortestPathVisitingAllNodesBenchmarks (ARCHITECTURE 17.9): its two arms
// are competing strategies for the same question - one shared frontier seeded from every node
// at once against a full distance map per start over the prepared (node, mask) state graph -
// so a harness whose arms disagree is walking two different graphs. Setup draws the graph from
// a fixed seed through ShortestPathGraphs, so the same NodeCount must rebuild the same
// adjacency and the same state graph; otherwise two published numbers were never comparable.
//
// The composed arm is expected to do strictly more work and return the same number, so the
// agreement below is the whole of what these arms promise each other.
public sealed partial class ShortestPathVisitingAllNodesBenchmarksTests
{
    private const int SmallestNodeCount = 8;

    [Fact]
    public void Setup_SameNodeCount_RebuildsTheSameGraph() =>
        Assert.Equal(BuildHarness().MutationQueueBfs(), BuildHarness().MutationQueueBfs());

    [Fact]
    public void MutationQueueBfs_EightNodeConnectedGraph_AgreesWithReduceGraphBfs()
    {
        var harness = BuildHarness();

        Assert.Equal(harness.ReduceGraphBfs(), harness.MutationQueueBfs());
    }

    [Fact]
    public void ReduceGraphBfs_EightNodeConnectedGraph_AgreesWithMutationQueueBfs()
    {
        var harness = BuildHarness();

        Assert.Equal(harness.MutationQueueBfs(), harness.ReduceGraphBfs());
    }

    private static ShortestPathVisitingAllNodesBenchmarks BuildHarness()
    {
        var harness = new ShortestPathVisitingAllNodesBenchmarks { NodeCount = SmallestNodeCount };
        harness.Setup();

        return harness;
    }
}
