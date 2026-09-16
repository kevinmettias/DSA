using DSAExperimentation.Benchmarks.Fixtures;

namespace DSAExperimentation.Benchmarks.Tests.Fixtures;

// Harness coverage for PowerStateWorkloads (ARCHITECTURE 17.7). The reading depends on LC 3977's graph
// being forward-only - every node gets an edge from some earlier node, so the last node stays reachable
// from the first - with per-node costs small enough that a run crosses several edges before its budget
// would run out, which is what keeps the remaining-power dimension of the state space exercised.
public sealed partial class PowerStateWorkloadsTests
{
    private const int NodeCount = 100;
    private const int Seed = 3977; // LC problem number
    private const int WeightUpperBound = 1_000_000;
    private const int ExtraEdgesPerNode = 2;
    private const int MaxCost = 3;
    private const int EdgeFieldCount = 3; // FromNode, ToNode, Weight
    private const int WeightFieldIndex = 2;
    private const int FirstNode = 0;
    private const int LastNode = NodeCount - 1;

    [Fact]
    public void Build_NodeCount_ReturnsOneCostPerNode()
    {
        var (_, cost) = PowerStateWorkloads.Build(NodeCount, Seed);

        Assert.Equal(NodeCount, cost.Length);
    }

    [Fact]
    public void Build_EveryEdge_IsAThreeFieldForwardRowWithAWeightInsideTheDocumentedBand()
    {
        var (edges, _) = PowerStateWorkloads.Build(NodeCount, Seed);

        Assert.All(edges, edge => Assert.Equal(EdgeFieldCount, edge.Length));
        Assert.All(edges, edge => Assert.InRange(edge[0], FirstNode, LastNode));
        Assert.All(edges, edge => Assert.InRange(edge[1], FirstNode, LastNode));
        Assert.All(edges, edge => Assert.True(edge[0] < edge[1]));
        Assert.All(edges, edge => Assert.InRange(edge[WeightFieldIndex], 1, WeightUpperBound - 1));
    }

    [Fact]
    public void Build_EveryNodeBeyondTheFirst_HasAnIncomingEdgeFromAnEarlierNode()
    {
        var (edges, _) = PowerStateWorkloads.Build(NodeCount, Seed);

        foreach (var node in Enumerable.Range(1, NodeCount - 1))
        {
            Assert.Contains(edges, edge => edge[1] == node && edge[0] < node);
        }
    }

    [Fact]
    public void Build_EdgeCount_StaysBetweenTheSpanningBackboneAndTheDensityCap()
    {
        var (edges, _) = PowerStateWorkloads.Build(NodeCount, Seed);

        Assert.InRange(edges.Length, NodeCount - 1, (NodeCount - 1) + (NodeCount * ExtraEdgesPerNode));
    }

    [Fact]
    public void Build_EveryCost_StaysInsideTheDocumentedBand() =>
        Assert.All(
            PowerStateWorkloads.Build(NodeCount, Seed).Cost,
            value => Assert.InRange(value, 1, MaxCost));

    [Fact]
    public void Build_SameSeed_ReturnsTheSameWorkload()
    {
        var (edges, cost) = PowerStateWorkloads.Build(NodeCount, Seed);
        var (repeatEdges, repeatCost) = PowerStateWorkloads.Build(NodeCount, Seed);

        Assert.Equal(AnswerText.Of(edges), AnswerText.Of(repeatEdges));
        Assert.Equal(AnswerText.Of(cost), AnswerText.Of(repeatCost));
    }
}
