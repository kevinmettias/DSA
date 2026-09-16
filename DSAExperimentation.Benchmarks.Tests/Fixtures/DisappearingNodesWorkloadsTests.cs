using DSAExperimentation.Benchmarks.Fixtures;

namespace DSAExperimentation.Benchmarks.Tests.Fixtures;

// Harness coverage for DisappearingNodesWorkloads (ARCHITECTURE 17.7). The reading depends on a
// connected graph, so both strategies run genuine full-graph Dijkstra work rather than mostly
// reporting unreachable nodes, and on every deadline sitting inside the documented ceiling.
public sealed partial class DisappearingNodesWorkloadsTests
{
    private const int NodeCount = 16;
    private const int Seed = 3112; // LC problem number
    private const int EdgeFieldCount = 3; // FromNode, ToNode, Length
    private const int LengthFieldIndex = 2;
    private const int MinEdgeLength = 1;
    private const int MaxEdgeLength = 100;
    private const int MinDisappearTime = 1;
    private const int MaxDisappearTime = 1_000_000;

    [Fact]
    public void Build_EveryEdge_JoinsTwoDistinctNodesWithALengthInsideTheBand()
    {
        var (edges, _) = DisappearingNodesWorkloads.Build(NodeCount, Seed);

        Assert.All(edges, edge => Assert.Equal(EdgeFieldCount, edge.Length));
        Assert.All(edges, edge => Assert.NotEqual(edge[0], edge[1]));
        Assert.All(edges, edge => Assert.InRange(edge[0], 0, NodeCount - 1));
        Assert.All(edges, edge => Assert.InRange(edge[1], 0, NodeCount - 1));
        Assert.All(edges, edge => Assert.InRange(edge[LengthFieldIndex], MinEdgeLength, MaxEdgeLength));
    }

    [Fact]
    public void Build_EveryNodeBeyondTheFirst_ReachesAnEarlierNode()
    {
        var (edges, _) = DisappearingNodesWorkloads.Build(NodeCount, Seed);

        foreach (var node in Enumerable.Range(1, NodeCount - 1))
        {
            Assert.Contains(edges, edge => edge[1] == node && edge[0] < node);
        }
    }

    [Fact]
    public void Build_EveryDeadline_FallsWithinTheDocumentedCeiling()
    {
        var (_, disappear) = DisappearingNodesWorkloads.Build(NodeCount, Seed);

        Assert.Equal(NodeCount, disappear.Length);
        Assert.All(disappear, deadline => Assert.InRange(deadline, MinDisappearTime, MaxDisappearTime));
    }

    [Fact]
    public void Build_SameSeed_ReturnsTheSameWorkload()
    {
        var (edges, disappear) = DisappearingNodesWorkloads.Build(NodeCount, Seed);
        var (repeatEdges, repeatDisappear) = DisappearingNodesWorkloads.Build(NodeCount, Seed);

        Assert.Equal(AnswerText.Of(edges), AnswerText.Of(repeatEdges));
        Assert.Equal(disappear, repeatDisappear);
    }
}
