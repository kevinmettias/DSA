using DSAExperimentation.Benchmarks.Fixtures;

namespace DSAExperimentation.Benchmarks.Tests.Fixtures;

// Harness coverage for MinimumWeightedSubgraphWorkloads (ARCHITECTURE 17.7). The reading depends on LC
// 2203's own int[][] edge shape over a graph node 0 reaches everyone in, plus this problem's own extra
// detail: node 1 is the second source, so it needs outgoing edges of its own rather than only whatever
// reach the back-edge chain grants it.
public sealed partial class MinimumWeightedSubgraphWorkloadsTests
{
    private const int NodeCount = 16;
    private const int ExtraEdgesPerNode = 3;
    private const int Seed = 2203; // LC problem number
    private const int EdgeFieldCount = 3; // FromNode, ToNode, Weight
    private const int MinEdgeWeight = 1;
    private const int MaxEdgeWeight = 49;
    private const int SecondSourceId = 1;
    private const int FirstSeedOfTheContractSweep = 2203;
    private const int SeedsInTheContractSweep = 16;

    [Fact]
    public void BuildEdges_EveryEdge_IsAThreeFieldRowWithinTheNodeRangeAndWeightBand()
    {
        var edges = MinimumWeightedSubgraphWorkloads.BuildEdges(NodeCount, ExtraEdgesPerNode, Seed);

        Assert.All(edges, edge => Assert.Equal(EdgeFieldCount, edge.Length));
        Assert.All(edges, edge => Assert.InRange(edge[0], 0, NodeCount - 1));
        Assert.All(edges, edge => Assert.InRange(edge[1], 0, NodeCount - 1));
        Assert.All(edges, edge => Assert.InRange(edge[2], MinEdgeWeight, MaxEdgeWeight));
    }

    // LC 2203's own constraint is that no edge's endpoints are the same node, so this has to hold for
    // every seed the generator is handed - it is the second-source spread's draws that are checked
    // here, since the other two loops already skip a self-target.
    [Fact]
    public void BuildEdges_EveryEdge_JoinsTwoDistinctNodesForEverySeed()
    {
        foreach (var seed in Enumerable.Range(FirstSeedOfTheContractSweep, SeedsInTheContractSweep))
        {
            var edges = MinimumWeightedSubgraphWorkloads.BuildEdges(NodeCount, ExtraEdgesPerNode, seed);

            Assert.All(edges, edge => Assert.NotEqual(edge[0], edge[1]));
        }
    }

    [Fact]
    public void BuildEdges_EveryNodeBeyondTheFirst_HasAnIncomingEdgeFromAnEarlierNode()
    {
        var edges = MinimumWeightedSubgraphWorkloads.BuildEdges(NodeCount, ExtraEdgesPerNode, Seed);

        foreach (var node in Enumerable.Range(1, NodeCount - 1))
        {
            Assert.Contains(edges, edge => edge[1] == node && edge[0] < node);
        }
    }

    // The back-edge spine only ever gives node 1 an incoming edge, so any outgoing edge it carries came
    // from the spread this problem's own shape adds on top of the chain.
    [Fact]
    public void BuildEdges_SecondSource_CarriesItsOwnOutgoingEdges()
    {
        var edges = MinimumWeightedSubgraphWorkloads.BuildEdges(NodeCount, ExtraEdgesPerNode, Seed);

        Assert.Contains(edges, edge => edge[0] == SecondSourceId);
    }

    [Fact]
    public void BuildEdges_SameSeed_ReturnsTheSameEdges() =>
        Assert.Equal(
            AnswerText.Of(MinimumWeightedSubgraphWorkloads.BuildEdges(NodeCount, ExtraEdgesPerNode, Seed)),
            AnswerText.Of(MinimumWeightedSubgraphWorkloads.BuildEdges(NodeCount, ExtraEdgesPerNode, Seed)));
}
