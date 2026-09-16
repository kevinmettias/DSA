using DSAExperimentation.Benchmarks.Fixtures;

namespace DSAExperimentation.Benchmarks.Tests.Fixtures;

// Harness coverage for EdgeWeightGraphWorkloads (ARCHITECTURE 17.7). The reading depends on LC
// 3419's own int[][] edge shape over a weighted directed graph in which every node can reach node
// 0, so the binary search's feasibility checks filter real edges instead of walking a bare
// spanning path.
public sealed partial class EdgeWeightGraphWorkloadsTests
{
    private const int NodeCount = 16;
    private const int ExtraEdgesPerNode = 3;
    private const int Seed = 3419; // LC problem number
    private const int EdgeFieldCount = 3; // FromNode, ToNode, Weight
    private const int MinEdgeWeight = 1;
    private const int MaxEdgeWeight = 999_999;

    [Fact]
    public void BuildEdges_EveryEdge_IsAThreeFieldRowWithinTheNodeRangeAndWeightBand()
    {
        var edges = EdgeWeightGraphWorkloads.BuildEdges(NodeCount, ExtraEdgesPerNode, Seed);

        Assert.All(edges, edge => Assert.Equal(EdgeFieldCount, edge.Length));
        Assert.All(edges, edge => Assert.InRange(edge[0], 0, NodeCount - 1));
        Assert.All(edges, edge => Assert.InRange(edge[1], 0, NodeCount - 1));
        Assert.All(edges, edge => Assert.InRange(edge[2], MinEdgeWeight, MaxEdgeWeight));
    }

    // LC 3419 is directed, and the generator writes its spanning edge as { later node, earlier
    // node, weight }, so here the direction is the claim itself: each node's edge to a
    // lower-numbered node is what lets node 0 be reached from every node.
    [Fact]
    public void BuildEdges_EveryNodeBeyondTheFirst_PointsAtAnEarlierNode()
    {
        var edges = EdgeWeightGraphWorkloads.BuildEdges(NodeCount, ExtraEdgesPerNode, Seed);

        foreach (var node in Enumerable.Range(1, NodeCount - 1))
        {
            Assert.Contains(edges, edge => edge[0] == node && edge[1] < node);
        }
    }

    [Fact]
    public void BuildEdges_EdgeCount_StaysBetweenTheSpanningBackboneAndTheDensityCap()
    {
        var edges = EdgeWeightGraphWorkloads.BuildEdges(NodeCount, ExtraEdgesPerNode, Seed);

        Assert.InRange(edges.Length, NodeCount - 1, NodeCount - 1 + (NodeCount * ExtraEdgesPerNode));
    }

    [Fact]
    public void BuildEdges_SameSeed_ReturnsTheSameEdges() =>
        Assert.Equal(
            AnswerText.Of(EdgeWeightGraphWorkloads.BuildEdges(NodeCount, ExtraEdgesPerNode, Seed)),
            AnswerText.Of(EdgeWeightGraphWorkloads.BuildEdges(NodeCount, ExtraEdgesPerNode, Seed)));
}
