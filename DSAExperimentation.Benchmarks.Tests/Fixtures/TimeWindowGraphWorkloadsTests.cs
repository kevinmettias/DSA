using DSAExperimentation.Benchmarks.Fixtures;

namespace DSAExperimentation.Benchmarks.Tests.Fixtures;

// Harness coverage for TimeWindowGraphWorkloads (ARCHITECTURE 17.7): LC 3604 schedules a journey along
// time-windowed edges, and the fixture's whole guarantee is that the last node stays reachable - a random
// forward-edge backbone under the extra random edges. Without it the strategies would be measured on
// instances with no journey at all, so reachability is the property this test derives independently.
public sealed partial class TimeWindowGraphWorkloadsTests
{
    private const int NodeCount = 32;
    private const int Seed = 3604; // LC problem number
    private const int EdgeWidth = 4; // FromNode, ToNode, WindowStart, WindowEnd
    private const int WindowStartFieldIndex = 2;
    private const int WindowEndFieldIndex = 3;
    private const int WindowStartCeilingExclusive = 50;
    private const int WindowLengthCeilingExclusive = 20;
    private const int ShortestWindowLength = 1;

    [Fact]
    public void Build_EveryEdge_NamesTwoDistinctNodesWithAForwardTimeWindow()
    {
        var edges = TimeWindowGraphWorkloads.Build(NodeCount, Seed);

        Assert.NotEmpty(edges);
        Assert.All(edges, edge => Assert.Equal(EdgeWidth, edge.Length));
        Assert.All(edges, edge => Assert.NotEqual(edge[0], edge[1]));
        Assert.All(edges, edge => Assert.InRange(edge[0], 0, NodeCount - 1));
        Assert.All(edges, edge => Assert.InRange(edge[1], 0, NodeCount - 1));
        Assert.All(edges, edge => Assert.InRange(edge[WindowStartFieldIndex], 0, WindowStartCeilingExclusive - 1));
        Assert.All(
            edges,
            edge => Assert.InRange(
                edge[WindowEndFieldIndex] - edge[WindowStartFieldIndex],
                ShortestWindowLength,
                WindowLengthCeilingExclusive - 1));
    }

    // The documented guarantee, derived rather than assumed: every node past the first is the target of
    // an edge from a strictly earlier node, so node 0 can reach the last node by induction.
    [Fact]
    public void Build_ForwardEdgeBackbone_ReachesEveryNodeFromTheFirst()
    {
        var edges = TimeWindowGraphWorkloads.Build(NodeCount, Seed);

        Assert.All(
            Enumerable.Range(1, NodeCount - 1),
            node => Assert.Contains(edges, edge => edge[1] == node && edge[0] < node));
    }

    [Fact]
    public void Build_SameSeed_ReturnsTheSameEdges() =>
        Assert.Equal(
            AnswerText.Of(TimeWindowGraphWorkloads.Build(NodeCount, Seed)),
            AnswerText.Of(TimeWindowGraphWorkloads.Build(NodeCount, Seed)));
}
