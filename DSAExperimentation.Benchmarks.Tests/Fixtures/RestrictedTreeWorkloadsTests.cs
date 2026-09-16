using DSAExperimentation.Benchmarks.Fixtures;

namespace DSAExperimentation.Benchmarks.Tests.Fixtures;

// Harness coverage for RestrictedTreeWorkloads (ARCHITECTURE 17.7). The LC 2368 reading depends on
// the edges really describing a rooted tree over [0, nodeCount) - every node past the root hanging
// off an earlier one, which is what makes the flood fill walk a real branching structure - and on
// the restricted set fencing off a fraction of the non-root nodes, leaving both a substantial
// reachable component and a substantial pruned remainder.
public sealed partial class RestrictedTreeWorkloadsTests
{
    private const int NodeCount = 200;
    private const int Seed = 2368; // LC problem number
    private const int Root = 0;
    private const int EdgeFieldCount = 2; // Parent, Child
    private const int FewestRestrictedNodes = 1;

    [Fact]
    public void Build_NodeCount_ReturnsOneEdgePerNodeBeyondTheRoot()
    {
        var (edges, _) = RestrictedTreeWorkloads.Build(NodeCount, Seed);

        Assert.Equal(NodeCount - 1, edges.Length);
        Assert.All(edges, edge => Assert.Equal(EdgeFieldCount, edge.Length));
    }

    [Fact]
    public void Build_EveryNodeBeyondTheRoot_HangsOffAnEarlierNode()
    {
        var (edges, _) = RestrictedTreeWorkloads.Build(NodeCount, Seed);

        foreach (var node in Enumerable.Range(Root + 1, NodeCount - 1))
        {
            Assert.Contains(edges, edge => edge[1] == node && edge[0] < node);
        }
    }

    // Restricting the root would make every arm answer zero, so the fixture must never fence it
    // off; the rest of the set is the fraction the flood fill has to route around.
    [Fact]
    public void Build_Restricted_ExcludesTheRootAndLeavesBothPrunedAndReachableNodes()
    {
        var (_, restricted) = RestrictedTreeWorkloads.Build(NodeCount, Seed);

        Assert.DoesNotContain(Root, restricted);
        Assert.All(restricted, node => Assert.InRange(node, Root + 1, NodeCount - 1));
        Assert.InRange(restricted.Length, FewestRestrictedNodes, NodeCount - 1 - FewestRestrictedNodes);
        Assert.Equal(restricted.Length, restricted.Distinct().Count());
    }

    [Fact]
    public void Build_SameSeed_ReturnsTheSameWorkload()
    {
        var (edges, restricted) = RestrictedTreeWorkloads.Build(NodeCount, Seed);
        var (repeatEdges, repeatRestricted) = RestrictedTreeWorkloads.Build(NodeCount, Seed);

        Assert.Equal(AnswerText.Of(edges), AnswerText.Of(repeatEdges));
        Assert.Equal(restricted, repeatRestricted);
    }
}
