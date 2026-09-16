using DSAExperimentation.Benchmarks.ProblemSolutions;

namespace DSAExperimentation.Benchmarks.Tests.ProblemSolutions;

// Harness coverage for ShortestPathAlgorithmBenchmarks (ARCHITECTURE 17.9): all three arms answer the
// same single-source question - minutes to reach every node, or -1 if one is unreachable - on the same
// non-negative-weight graph, and FloydWarshall reads that answer off the source's own row of its
// all-pairs table. The class exists to show that the all-pairs algorithm is the wrong tool here, not
// that it answers a different question, so a harness whose arms disagree is timing two different
// graphs or two different questions. Setup builds the graph from one fixed seed and hands all three
// arms the same prepared vertices, so the same NodeCount must rebuild the same graph; no arm writes to
// it, so one harness instance is safe to call twice in either order.
public sealed partial class ShortestPathAlgorithmBenchmarksTests
{
    private const int SmallestNodeCount = 50;

    [Fact]
    public void Setup_SameNodeCount_RebuildsTheSameWorkload() =>
        Assert.Equal(BuildHarness().Dijkstra(), BuildHarness().Dijkstra());

    [Fact]
    public void Dijkstra_SeededWeightedGraph_AgreesWithBellmanFord()
    {
        var harness = BuildHarness();

        Assert.Equal(harness.BellmanFord(), harness.Dijkstra());
    }

    [Fact]
    public void BellmanFord_SeededWeightedGraph_AgreesWithFloydWarshall()
    {
        var harness = BuildHarness();

        Assert.Equal(harness.FloydWarshall(), harness.BellmanFord());
    }

    [Fact]
    public void FloydWarshall_SeededWeightedGraph_AgreesWithDijkstra()
    {
        var harness = BuildHarness();

        Assert.Equal(harness.Dijkstra(), harness.FloydWarshall());
    }

    private static ShortestPathAlgorithmBenchmarks BuildHarness()
    {
        var harness = new ShortestPathAlgorithmBenchmarks { NodeCount = SmallestNodeCount };
        harness.Setup();

        return harness;
    }
}
