using DSAExperimentation.Algorithms.Traversal.TopDown;
using DSAExperimentation.DataStructures.Graph.Contracts.Ordering;
using DSAExperimentation.Tests.Algorithms.Traversal.TopDown.Fixtures;
using DSAExperimentation.Tests.Algorithms.Walking.Fixtures;
using DSAExperimentation.Tests.DataStructures.Graph.Fixtures;

namespace DSAExperimentation.Tests.Algorithms.Traversal.TopDown;

public sealed class TopDownTraversalTests
{
    private static List<(string Path, int Depth, NodePosition Position)> Walk(TestNode root)
    {
        var recorded = new List<(string, int, NodePosition)>();

        TopDownTraversal.Walk<
            TestNode, TestTopology, ListChildren<TestNode>,
            NaturalChildOrder<TestNode, ListChildren<TestNode>>, ListChildren<TestNode>,
            RecordingPathHooks, PathSoFar>(root, new PathSoFar([root.Name], recorded));

        return recorded;
    }

    [Fact]
    public void Walk_VisitsEveryNodeInDepthFirstOrder()
    {
        var visited = Walk(WalkGraphs.Tree());

        Assert.Equal(["A", "A/B", "A/B/D", "A/B/E", "A/C"], visited.Select(v => v.Path));
    }

    [Fact]
    public void Walk_ThreadsStateDownEachPathIndependentlyOfSiblings()
    {
        // "A/C" proves C's state came from A, not from its earlier sibling B.
        var visited = Walk(WalkGraphs.Tree());

        Assert.Contains(("A/C", 1, NodePosition.Leaf), visited.Select(v => (v.Path, v.Depth, v.Position)));
    }

    [Fact]
    public void Walk_ReportsDepthFromZeroAtTheRoot()
    {
        var visited = Walk(WalkGraphs.Tree());

        Assert.Equal([0, 1, 2, 2, 1], visited.Select(v => v.Depth));
    }

    [Fact]
    public void Walk_DistinguishesLeavesFromInteriorNodes()
    {
        var visited = Walk(WalkGraphs.Tree());

        Assert.Equal(
            [NodePosition.Interior, NodePosition.Interior, NodePosition.Leaf, NodePosition.Leaf, NodePosition.Leaf],
            visited.Select(v => v.Position));
    }

    [Fact]
    public void Walk_SingleNode_IsALeafAtDepthZero()
    {
        var visited = Walk(new TestNode("A"));

        Assert.Equal([("A", 0, NodePosition.Leaf)], visited);
    }

    [Fact]
    public void Walk_NullRoot_VisitsNothing()
    {
        var recorded = new List<(string, int, NodePosition)>();

        TopDownTraversal.Walk<
            TestNode, TestTopology, ListChildren<TestNode>,
            NaturalChildOrder<TestNode, ListChildren<TestNode>>, ListChildren<TestNode>,
            RecordingPathHooks, PathSoFar>(null, new PathSoFar([], recorded));

        Assert.Empty(recorded);
    }

    [Fact]
    public void WalkGraph_OnACycle_Terminates()
    {
        var root = WalkGraphs.Cycle();
        var recorded = new List<(string, int, NodePosition)>();

        TopDownTraversal.WalkGraph<
            TestNode, TestTopology, ListChildren<TestNode>,
            NaturalChildOrder<TestNode, ListChildren<TestNode>>, ListChildren<TestNode>,
            RecordingPathHooks, PathSoFar>(root, new PathSoFar([root.Name], recorded));

        Assert.Equal(["A", "A/B", "A/B/C"], recorded.Select(r => r.Item1));
    }

    [Fact]
    public void WalkGraph_SharedDescendant_IsVisitedOnce()
    {
        var (root, _) = WalkGraphs.DiamondShare();
        var recorded = new List<(string, int, NodePosition)>();

        TopDownTraversal.WalkGraph<
            TestNode, TestTopology, ListChildren<TestNode>,
            NaturalChildOrder<TestNode, ListChildren<TestNode>>, ListChildren<TestNode>,
            RecordingPathHooks, PathSoFar>(root, new PathSoFar([root.Name], recorded));

        Assert.Equal(["A", "A/B", "A/B/D", "A/C"], recorded.Select(r => r.Item1));
    }

    [Fact]
    public void WalkGraph_NullRoot_VisitsNothing()
    {
        var recorded = new List<(string, int, NodePosition)>();

        TopDownTraversal.WalkGraph<
            TestNode, TestTopology, ListChildren<TestNode>,
            NaturalChildOrder<TestNode, ListChildren<TestNode>>, ListChildren<TestNode>,
            RecordingPathHooks, PathSoFar>(null, new PathSoFar([], recorded));

        Assert.Empty(recorded);
    }
}
