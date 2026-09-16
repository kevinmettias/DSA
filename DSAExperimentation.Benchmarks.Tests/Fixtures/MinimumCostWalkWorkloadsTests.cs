using DSAExperimentation.Benchmarks.Fixtures;

namespace DSAExperimentation.Benchmarks.Tests.Fixtures;

// Harness coverage for MinimumCostWalkWorkloads (ARCHITECTURE 17.7). The reading depends on LC 3108's
// graph being connected - every node i > 0 reached from an earlier one - with a batch of queries over
// it, so both strategies do genuine walk-cost work instead of mostly reporting -1.
public sealed partial class MinimumCostWalkWorkloadsTests
{
    private const int NodeCount = 32;
    private const int QueryCount = 64;
    private const int Seed = 3108; // LC problem number
    private const int EdgeFieldCount = 3; // FromNode, ToNode, Weight
    private const int QueryFieldCount = 2; // Source, Target
    private const int MinWeight = 0;
    private const int MaxWeightExclusive = 100_001;

    [Fact]
    public void Build_NodeCount_ReturnsOneEdgeRowPerEdgeAndOneQueryPerRequestedQuery()
    {
        var (edges, query) = MinimumCostWalkWorkloads.Build(NodeCount, QueryCount, Seed);

        Assert.All(edges, edge => Assert.Equal(EdgeFieldCount, edge.Length));
        Assert.Equal(QueryCount, query.Length);
    }

    [Fact]
    public void Build_EveryEdge_StaysWithinTheNodeRangeAndWeightBand()
    {
        var (edges, _) = MinimumCostWalkWorkloads.Build(NodeCount, QueryCount, Seed);

        Assert.All(edges, edge => Assert.InRange(edge[0], 0, NodeCount - 1));
        Assert.All(edges, edge => Assert.InRange(edge[1], 0, NodeCount - 1));
        Assert.All(edges, edge => Assert.InRange(edge[2], MinWeight, MaxWeightExclusive - 1));
    }

    [Fact]
    public void Build_EveryNodeBeyondTheFirst_HasAnIncomingEdgeFromAnEarlierNode()
    {
        var (edges, _) = MinimumCostWalkWorkloads.Build(NodeCount, QueryCount, Seed);

        foreach (var node in Enumerable.Range(1, NodeCount - 1))
        {
            Assert.Contains(edges, edge => edge[1] == node && edge[0] < node);
        }
    }

    // The query loop redraws until the two ends differ, so no query may ask for a node's cost to
    // itself - the degenerate pair neither strategy does work for.
    [Fact]
    public void Build_EveryQuery_NamesTwoDistinctNodesInRange()
    {
        var (_, query) = MinimumCostWalkWorkloads.Build(NodeCount, QueryCount, Seed);

        Assert.All(query, pair => Assert.Equal(QueryFieldCount, pair.Length));
        Assert.All(query, pair => Assert.InRange(pair[0], 0, NodeCount - 1));
        Assert.All(query, pair => Assert.InRange(pair[1], 0, NodeCount - 1));
        Assert.All(query, pair => Assert.NotEqual(pair[0], pair[1]));
    }

    [Fact]
    public void Build_SameSeed_ReturnsTheSameWorkload()
    {
        var (edges, query) = MinimumCostWalkWorkloads.Build(NodeCount, QueryCount, Seed);
        var (repeatEdges, repeatQuery) = MinimumCostWalkWorkloads.Build(NodeCount, QueryCount, Seed);

        Assert.Equal(AnswerText.Of(edges), AnswerText.Of(repeatEdges));
        Assert.Equal(AnswerText.Of(query), AnswerText.Of(repeatQuery));
    }
}
