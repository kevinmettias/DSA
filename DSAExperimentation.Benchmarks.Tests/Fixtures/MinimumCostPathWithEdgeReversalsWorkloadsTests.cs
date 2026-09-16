using DSAExperimentation.Benchmarks.Fixtures;

namespace DSAExperimentation.Benchmarks.Tests.Fixtures;

// Harness coverage for MinimumCostPathWithEdgeReversalsWorkloads (ARCHITECTURE 17.7). The reading
// depends on LC 3650's own int[][] edge shape over a directed graph node 0 can reach every node of
// without any reversal, on top of which the extra edges give a reversal something to shorten.
public sealed partial class MinimumCostPathWithEdgeReversalsWorkloadsTests
{
    private const int NodeCount = 16;
    private const int ExtraEdgesPerNode = 3;
    private const int Seed = 3650; // LC problem number
    private const int EdgeFieldCount = 3; // FromNode, ToNode, Weight
    private const int MinEdgeWeight = 1;
    private const int MaxEdgeWeight = 49;

    [Fact]
    public void BuildEdges_EveryEdge_IsAThreeFieldRowWithinTheNodeRangeAndWeightBand()
    {
        var edges = MinimumCostPathWithEdgeReversalsWorkloads.BuildEdges(NodeCount, ExtraEdgesPerNode, Seed);

        Assert.All(edges, edge => Assert.Equal(EdgeFieldCount, edge.Length));
        Assert.All(edges, edge => Assert.InRange(edge[0], 0, NodeCount - 1));
        Assert.All(edges, edge => Assert.InRange(edge[1], 0, NodeCount - 1));
        Assert.All(edges, edge => Assert.InRange(edge[2], MinEdgeWeight, MaxEdgeWeight));
    }

    // The generator writes its spanning edge as { earlier node, later node, weight }, and it is that
    // forward direction which reaches every node - including the last - before any reversal is paid for.
    [Fact]
    public void BuildEdges_EveryNodeBeyondTheFirst_HasAnIncomingEdgeFromAnEarlierNode()
    {
        var edges = MinimumCostPathWithEdgeReversalsWorkloads.BuildEdges(NodeCount, ExtraEdgesPerNode, Seed);

        foreach (var node in Enumerable.Range(1, NodeCount - 1))
        {
            Assert.Contains(edges, edge => edge[1] == node && edge[0] < node);
        }
    }

    [Fact]
    public void BuildEdges_EdgeCount_StaysBetweenTheSpanningBackboneAndTheDensityCap()
    {
        var edges = MinimumCostPathWithEdgeReversalsWorkloads.BuildEdges(NodeCount, ExtraEdgesPerNode, Seed);

        Assert.InRange(edges.Length, NodeCount - 1, NodeCount - 1 + (NodeCount * ExtraEdgesPerNode));
    }

    [Fact]
    public void BuildEdges_SameSeed_ReturnsTheSameEdges() =>
        Assert.Equal(
            AnswerText.Of(MinimumCostPathWithEdgeReversalsWorkloads.BuildEdges(NodeCount, ExtraEdgesPerNode, Seed)),
            AnswerText.Of(MinimumCostPathWithEdgeReversalsWorkloads.BuildEdges(NodeCount, ExtraEdgesPerNode, Seed)));
}
