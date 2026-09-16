using DSAExperimentation.Benchmarks.Fixtures;

namespace DSAExperimentation.Benchmarks.Tests.Fixtures;

// Harness coverage for StabilityGraphWorkloads (ARCHITECTURE 17.7). The LC 3600 reading depends on
// the must-marked edges forming a spanning tree - a cycle among them would make the feasibility
// binary search short-circuit to -1 instead of doing real work - and on the optional edges layered
// over them giving the algorithm a genuine choice to make.
public sealed partial class StabilityGraphWorkloadsTests
{
    private const int NodeCount = 200;
    private const int Seed = 3600; // LC problem number
    private const int Root = 0;
    private const int EdgeFieldCount = 4; // FromNode, ToNode, Strength, MustFlag
    private const int MustEdgeFlag = 1;
    private const int OptionalEdgeFlag = 0;
    private const int MinStrength = 1;
    private const int MaxStrength = 99_999; // one below the fixture's own exclusive ceiling of 100_000
    private const int UpgradeBudgetDivisor = 4;

    [Fact]
    public void Build_EveryEdge_CarriesTheFourDocumentedFields()
    {
        var (edges, _) = StabilityGraphWorkloads.Build(NodeCount, Seed);

        Assert.All(edges, edge => Assert.Equal(EdgeFieldCount, edge.Length));
        Assert.All(edges, edge => Assert.True(edge[3] == MustEdgeFlag || edge[3] == OptionalEdgeFlag));
        Assert.All(edges, edge => Assert.InRange(edge[2], MinStrength, MaxStrength));
        Assert.All(edges, edge => Assert.InRange(edge[0], Root, NodeCount - 1));
        Assert.All(edges, edge => Assert.InRange(edge[1], Root, NodeCount - 1));
    }

    // One must edge per node past the root, each hanging off an earlier node: exactly the shape of a
    // spanning tree, which is what stops the must set from ever closing a cycle.
    [Fact]
    public void Build_MustEdges_FormASpanningTreeOverEveryNodeBeyondTheRoot()
    {
        var (edges, _) = StabilityGraphWorkloads.Build(NodeCount, Seed);
        var mustEdges = edges.Where(edge => edge[3] == MustEdgeFlag).ToList();

        Assert.Equal(NodeCount - 1, mustEdges.Count);
        Assert.All(mustEdges, edge => Assert.True(edge[0] < edge[1]));

        foreach (var node in Enumerable.Range(Root + 1, NodeCount - 1))
        {
            Assert.Contains(mustEdges, edge => edge[1] == node);
        }
    }

    [Fact]
    public void Build_OptionalEdges_LeaveTheAlgorithmAChoiceAndNeverAreSelfLoops()
    {
        var (edges, _) = StabilityGraphWorkloads.Build(NodeCount, Seed);
        var optionalEdges = edges.Where(edge => edge[3] == OptionalEdgeFlag).ToList();

        Assert.NotEmpty(optionalEdges);
        Assert.All(optionalEdges, edge => Assert.NotEqual(edge[0], edge[1]));
    }

    [Fact]
    public void Build_NodeCount_ReturnsTheMustEdgesPlusAtMostTwoOptionalEdgesPerNode()
    {
        var (edges, _) = StabilityGraphWorkloads.Build(NodeCount, Seed);
        var optionalPerNode = 2;

        Assert.InRange(edges.Length, NodeCount - 1, (NodeCount - 1) + (optionalPerNode * NodeCount));
    }

    [Fact]
    public void Build_UpgradeBudget_IsAFixedFractionOfTheNodeCount()
    {
        var (_, budget) = StabilityGraphWorkloads.Build(NodeCount, Seed);

        Assert.Equal(NodeCount / UpgradeBudgetDivisor, budget);
    }

    [Fact]
    public void Build_SameSeed_ReturnsTheSameWorkload()
    {
        var (edges, budget) = StabilityGraphWorkloads.Build(NodeCount, Seed);
        var (repeatEdges, repeatBudget) = StabilityGraphWorkloads.Build(NodeCount, Seed);

        Assert.Equal(AnswerText.Of(edges), AnswerText.Of(repeatEdges));
        Assert.Equal(budget, repeatBudget);
    }
}
