using DSAExperimentation.Benchmarks.Fixtures;

namespace DSAExperimentation.Benchmarks.Tests.Fixtures;

// Harness coverage for FindEdgesInShortestPathsWorkloads (ARCHITECTURE 17.7). The reading depends
// on LC 3123's own int[][] edge shape over a weighted directed graph node 0 can reach every node
// of, so the shortest-path DAG the answer walks is never a partial one.
public sealed partial class FindEdgesInShortestPathsWorkloadsTests
{
    private const int NodeCount = 16;
    private const int ExtraEdgesPerNode = 3;
    private const int Seed = 3123; // LC problem number
    private const int EdgeFieldCount = 3; // FromNode, ToNode, Weight
    private const int MinEdgeWeight = 1;
    private const int MaxEdgeWeight = 49;

    [Fact]
    public void BuildEdges_EveryEdge_IsAThreeFieldRowWithinTheNodeRangeAndWeightBand()
    {
        var edges = FindEdgesInShortestPathsWorkloads.BuildEdges(NodeCount, ExtraEdgesPerNode, Seed);

        Assert.All(edges, edge => Assert.Equal(EdgeFieldCount, edge.Length));
        Assert.All(edges, edge => Assert.InRange(edge[0], 0, NodeCount - 1));
        Assert.All(edges, edge => Assert.InRange(edge[1], 0, NodeCount - 1));
        Assert.All(edges, edge => Assert.InRange(edge[2], MinEdgeWeight, MaxEdgeWeight));
    }

    // LC 3123 is directed and the generator writes its spanning edge as { earlier node, later
    // node, weight }, so the direction is asserted exactly: it is the edge pointing forward that
    // makes node 0 reach every node.
    [Fact]
    public void BuildEdges_EveryNodeBeyondTheFirst_HasAnIncomingEdgeFromAnEarlierNode()
    {
        var edges = FindEdgesInShortestPathsWorkloads.BuildEdges(NodeCount, ExtraEdgesPerNode, Seed);

        foreach (var node in Enumerable.Range(1, NodeCount - 1))
        {
            Assert.Contains(edges, edge => edge[1] == node && edge[0] < node);
        }
    }

    [Fact]
    public void BuildEdges_EdgeCount_StaysBetweenTheSpanningBackboneAndTheDensityCap()
    {
        var edges = FindEdgesInShortestPathsWorkloads.BuildEdges(NodeCount, ExtraEdgesPerNode, Seed);

        Assert.InRange(edges.Length, NodeCount - 1, NodeCount - 1 + (NodeCount * ExtraEdgesPerNode));
    }

    [Fact]
    public void BuildEdges_SameSeed_ReturnsTheSameEdges() =>
        Assert.Equal(
            AnswerText.Of(FindEdgesInShortestPathsWorkloads.BuildEdges(NodeCount, ExtraEdgesPerNode, Seed)),
            AnswerText.Of(FindEdgesInShortestPathsWorkloads.BuildEdges(NodeCount, ExtraEdgesPerNode, Seed)));
}
