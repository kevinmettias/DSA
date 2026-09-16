using DSAExperimentation.Benchmarks.ProblemSolutions;

namespace DSAExperimentation.Benchmarks.Tests.ProblemSolutions;

// Harness coverage for NetworkRecoveryPathwaysBenchmarks (ARCHITECTURE 17.9): its two arms are
// competing strategies for the same question - a hand-rolled BCL Dijkstra rebuilt per binary-search
// probe against this repo's own ShortestPath engine over the same RecoveryNetwork - so a harness
// whose arms disagree is searching two different graphs. Setup builds that network from the seeded
// NetworkRecoveryWorkloads DAG, so the same NodeCount must rebuild the same edges and online flags.
//
// One harness serves both arms here: the composed arm's Rebuild(threshold) rewrites each node's
// outgoing edge list, but it clears and repopulates that list from the same OnlineEdges on every
// call, and the baseline arm reads OnlineEdges/Nodes/MaxCost rather than those per-node lists - so
// neither arm can see the other's fingerprints and the call order does not matter.
public sealed partial class NetworkRecoveryPathwaysBenchmarksTests
{
    private const int SmallestNodeCount = 300;

    [Fact]
    public void Setup_SameNodeCount_RebuildsTheSameWorkload() =>
        Assert.Equal(BuildHarness().BruteForceDijkstra(), BuildHarness().BruteForceDijkstra());

    [Fact]
    public void BruteForceDijkstra_AgreesWithReduceGraph()
    {
        var harness = BuildHarness();

        Assert.Equal(harness.ReduceGraph(), harness.BruteForceDijkstra());
    }

    [Fact]
    public void ReduceGraph_AgreesWithBruteForceDijkstra()
    {
        var harness = BuildHarness();

        Assert.Equal(harness.BruteForceDijkstra(), harness.ReduceGraph());
    }

    private static NetworkRecoveryPathwaysBenchmarks BuildHarness()
    {
        var harness = new NetworkRecoveryPathwaysBenchmarks { NodeCount = SmallestNodeCount };
        harness.Setup();

        return harness;
    }
}
