using DSAExperimentation.Benchmarks.Fixtures;

namespace DSAExperimentation.Benchmarks.Tests.Fixtures;

// Harness coverage for ReachableNodesInSubdividedGraphWorkloads (ARCHITECTURE 17.7). The reading depends
// on LC 882's edge list carrying LeetCode's own {u, v, cnt} rows over a graph whose backbone reaches
// node 0 from everywhere, with subdivision counts low enough that the subdivided graph stays comparable
// in size to the weighted one SubdividedGraph.Build would produce from the same weights.
public sealed partial class ReachableNodesInSubdividedGraphWorkloadsTests
{
    private const int NodeCount = 30;
    private const int ExtraEdgesPerNode = 2;
    private const int Seed = 882; // LC problem number
    private const int EdgeFieldCount = 3;
    private const int MinSubdivisionCount = 0;
    private const int EdgeWeightUpperBound = 50;

    [Fact]
    public void BuildEdges_EveryEdge_IsAThreeFieldRowInsideTheNodeAndSubdivisionBands()
    {
        var edges = ReachableNodesInSubdividedGraphWorkloads.BuildEdges(NodeCount, ExtraEdgesPerNode, Seed);

        Assert.All(edges, edge => Assert.Equal(EdgeFieldCount, edge.Length));
        Assert.All(edges, edge => Assert.InRange(edge[0], 0, NodeCount - 1));
        Assert.All(edges, edge => Assert.InRange(edge[1], 0, NodeCount - 1));
        Assert.All(edges, edge => Assert.NotEqual(edge[0], edge[1]));
        Assert.All(
            edges,
            edge => Assert.InRange(edge[2], MinSubdivisionCount, EdgeWeightUpperBound - 2));
    }

    // The backbone is what guarantees reachability from node 0, so every node beyond the first must have
    // an edge to a lower-numbered node however the density edges are drawn.
    [Fact]
    public void BuildEdges_EveryNodeBeyondTheFirst_HasABackEdgeFromAnEarlierNode()
    {
        var edges = ReachableNodesInSubdividedGraphWorkloads.BuildEdges(NodeCount, ExtraEdgesPerNode, Seed);

        foreach (var node in Enumerable.Range(1, NodeCount - 1))
        {
            Assert.Contains(edges, edge => edge[1] == node && edge[0] < node);
        }
    }

    [Fact]
    public void BuildEdges_EdgeCount_StaysBetweenTheSpanningBackboneAndTheDensityCap()
    {
        var edges = ReachableNodesInSubdividedGraphWorkloads.BuildEdges(NodeCount, ExtraEdgesPerNode, Seed);

        Assert.InRange(
            edges.Length,
            NodeCount - 1,
            (NodeCount - 1) + (NodeCount * ExtraEdgesPerNode));
    }

    [Fact]
    public void BuildEdges_SameSeed_ReturnsTheSameEdges() =>
        Assert.Equal(
            AnswerText.Of(ReachableNodesInSubdividedGraphWorkloads.BuildEdges(NodeCount, ExtraEdgesPerNode, Seed)),
            AnswerText.Of(ReachableNodesInSubdividedGraphWorkloads.BuildEdges(NodeCount, ExtraEdgesPerNode, Seed)));
}
