using DSAExperimentation.Benchmarks.Fixtures;

namespace DSAExperimentation.Benchmarks.Tests.Fixtures;

// Harness coverage for MaxProfitWorkloads (ARCHITECTURE 17.7). The reading depends on LC 3530's graph
// staying acyclic by construction - every edge drawn from a lower index to a higher one, so no
// separate cycle check is needed - at a density that constrains valid orderings without collapsing the
// DAG to a single chain.
public sealed partial class MaxProfitWorkloadsTests
{
    private const int NodeCount = 32;
    private const int Seed = 3530; // LC problem number
    private const int EdgeFieldCount = 2; // FromNode, ToNode
    private const int MinScore = 1;
    private const int MaxScoreExclusive = 100_001;
    private const int UnorderedPairDivisor = 2; // a complete graph holds n(n-1)/2 distinct edges

    [Fact]
    public void Build_NodeCount_ReturnsOneScorePerNode() =>
        Assert.Equal(NodeCount, MaxProfitWorkloads.Build(NodeCount, Seed).Score.Length);

    [Fact]
    public void Build_EveryScore_StaysWithinTheDocumentedBand() =>
        Assert.All(
            MaxProfitWorkloads.Build(NodeCount, Seed).Score,
            score => Assert.InRange(score, MinScore, MaxScoreExclusive - 1));

    [Fact]
    public void Build_EveryEdge_IsATwoNodeRowPointingForward()
    {
        var (edges, _) = MaxProfitWorkloads.Build(NodeCount, Seed);

        Assert.All(edges, edge => Assert.Equal(EdgeFieldCount, edge.Length));
        Assert.All(edges, edge => Assert.InRange(edge[0], 0, NodeCount - 1));
        Assert.All(edges, edge => Assert.InRange(edge[1], 0, NodeCount - 1));
        Assert.All(edges, edge => Assert.True(edge[0] < edge[1]));
    }

    // An empty edge list would leave every ordering valid and the strategies measuring nothing, so
    // the density band starts above the single possible ordering.
    [Fact]
    public void Build_EdgeCount_StaysWithinTheDensityBand()
    {
        var (edges, _) = MaxProfitWorkloads.Build(NodeCount, Seed);

        Assert.InRange(edges.Length, 1, NodeCount * (NodeCount - 1) / UnorderedPairDivisor);
    }

    [Fact]
    public void Build_SameSeed_ReturnsTheSameWorkload()
    {
        var (edges, score) = MaxProfitWorkloads.Build(NodeCount, Seed);
        var (repeatEdges, repeatScore) = MaxProfitWorkloads.Build(NodeCount, Seed);

        Assert.Equal(AnswerText.Of(edges), AnswerText.Of(repeatEdges));
        Assert.Equal(score, repeatScore);
    }
}
