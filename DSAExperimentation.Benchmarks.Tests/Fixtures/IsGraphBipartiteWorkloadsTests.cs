using DSAExperimentation.Benchmarks.Fixtures;

namespace DSAExperimentation.Benchmarks.Tests.Fixtures;

// Harness coverage for IsGraphBipartiteWorkloads (ARCHITECTURE 17.7). The reading depends on the graph
// being bipartite by construction - every edge crossing the A/B split - and on staying inside LC 785's
// contract: no self-loops, every edge listed from both ends, and no neighbor list repeating a node.
// At this size the density draws do repeat edges, so the distinctness assertion is load-bearing.
public sealed partial class IsGraphBipartiteWorkloadsTests
{
    private const int NodeCount = 100; // LC 785's cap, the benchmark's largest size
    private const int Seed = 1;
    private const int PartitionCount = 2;

    [Fact]
    public void BuildAdjacency_NodeCount_ReturnsOneNeighborListPerNode() =>
        Assert.Equal(NodeCount, Build().Length);

    [Fact]
    public void BuildAdjacency_EveryNeighborList_HoldsDistinctNodes() =>
        Assert.All(Build(), neighbors => Assert.Equal(neighbors.Length, neighbors.Distinct().Count()));

    [Fact]
    public void BuildAdjacency_EveryEdge_CrossesTheSplitAndIsListedFromBothEnds()
    {
        var adjacency = Build();
        var half = NodeCount / PartitionCount;

        for (var node = 0; node < NodeCount; node++)
        {
            var nodeInA = node < half;

            foreach (var neighbor in adjacency[node])
            {
                Assert.NotEqual(nodeInA, neighbor < half);
                Assert.Contains(node, adjacency[neighbor]);
            }
        }
    }

    [Fact]
    public void BuildAdjacency_SameSeed_ReturnsTheSameGraph() =>
        Assert.Equal(Build(), Build());

    private static int[][] Build() => IsGraphBipartiteWorkloads.BuildAdjacency(NodeCount, Seed);
}
