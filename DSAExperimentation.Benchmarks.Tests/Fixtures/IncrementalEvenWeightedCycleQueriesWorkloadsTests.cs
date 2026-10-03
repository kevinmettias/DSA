using DSAExperimentation.Benchmarks.Fixtures;

namespace DSAExperimentation.Benchmarks.Tests.Fixtures;

// Harness coverage for IncrementalEvenWeightedCycleQueriesWorkloads (ARCHITECTURE 17.7). LC 3887
// promises every edge is distinct and ordered u < v, with a 0/1 weight; a stream that repeated a pair
// would hand both strategies a two-edge cycle the problem never poses. At this size and seed - the
// benchmark's own smallest - an unchecked draw does repeat a pair, so the distinctness assertion is
// load-bearing.
public sealed partial class IncrementalEvenWeightedCycleQueriesWorkloadsTests
{
    private const int EdgeCount = 500;
    private const int NodeCount = EdgeCount;
    private const int Seed = 3887; // LC problem number
    private const int MaxWeight = 1;
    private const int TripleLength = 3;
    private const int WeightPosition = 2;

    [Fact]
    public void BuildEdges_EdgeCount_ReturnsOneTriplePerEdge()
    {
        var edges = Build();

        Assert.Equal(EdgeCount, edges.Length);
        Assert.All(edges, edge => Assert.Equal(TripleLength, edge.Length));
    }

    [Fact]
    public void BuildEdges_EveryEdge_JoinsTwoNodesInAscendingOrder() =>
        Assert.All(Build(), edge => Assert.InRange(edge[0], 0, edge[1] - 1));

    [Fact]
    public void BuildEdges_EveryEdge_StaysInsideTheNodePoolWithAZeroOrOneWeight() =>
        Assert.All(
            Build(),
            edge =>
            {
                Assert.InRange(edge[1], 1, NodeCount - 1);
                Assert.InRange(edge[WeightPosition], 0, MaxWeight);
            });

    [Fact]
    public void BuildEdges_EveryPair_IsJoinedOnce()
    {
        var edges = Build();

        Assert.Equal(EdgeCount, edges.Select(edge => (edge[0], edge[1])).Distinct().Count());
    }

    [Fact]
    public void BuildEdges_SameSeed_ReturnsTheSameEdges() =>
        Assert.Equal(Build(), Build());

    private static int[][] Build() =>
        IncrementalEvenWeightedCycleQueriesWorkloads.BuildEdges(NodeCount, EdgeCount, Seed);
}
