using DSAExperimentation.Algorithms.Walking;
using DSAExperimentation.DataStructures.Graph.Contracts.Ordering;
using DSAExperimentation.Tests.Algorithms.Walking.Fixtures;
using DSAExperimentation.Tests.DataStructures.Graph.Fixtures;

namespace DSAExperimentation.Tests.Algorithms.Walking;

public sealed class BreadthFirstWalkTests
{
    private static List<(string Name, int Depth)> WalkTree(TestNode root) =>
        BreadthFirstWalk.Walk<
            TestNode, TestTopology, ListChildren<TestNode>,
            NaturalChildOrder<TestNode, ListChildren<TestNode>>, ListChildren<TestNode>,
            UnguardedVisit<TestNode>, RecordingWalkStep, List<(string, int)>>(
            root, [], new UnguardedVisit<TestNode>());

    private static List<(string Name, int Depth)> WalkGraph(TestNode root) =>
        BreadthFirstWalk.Walk<
            TestNode, TestTopology, ListChildren<TestNode>,
            NaturalChildOrder<TestNode, ListChildren<TestNode>>, ListChildren<TestNode>,
            TrackedVisitGuard<TestNode>, RecordingWalkStep, List<(string, int)>>(
            root, [], new TrackedVisitGuard<TestNode>([root]));

    [Fact]
    public void Walk_VisitsEveryNodeLevelByLevel()
    {
        var visited = WalkTree(WalkGraphs.Tree());

        Assert.Equal(["A", "B", "C", "D", "E"], visited.Select(v => v.Name));
    }

    [Fact]
    public void Walk_ReportsTheDepthOfEachNode()
    {
        var visited = WalkTree(WalkGraphs.Tree());

        Assert.Equal([0, 1, 1, 2, 2], visited.Select(v => v.Depth));
    }

    [Fact]
    public void Walk_SingleNode_VisitsOnlyTheRootAtDepthZero()
    {
        var visited = WalkTree(new TestNode("A"));

        Assert.Equal([("A", 0)], visited);
    }

    [Fact]
    public void Walk_TrackedGuardOnACycle_Terminates()
    {
        var visited = WalkGraph(WalkGraphs.Cycle());

        Assert.Equal(["A", "B", "C"], visited.Select(v => v.Name));
    }

    [Fact]
    public void Walk_TrackedGuardOnASharedDescendant_VisitsItOnce()
    {
        var (root, _) = WalkGraphs.DiamondShare();

        var visited = WalkGraph(root);

        Assert.Equal(["A", "B", "C", "D"], visited.Select(v => v.Name));
    }

    [Fact]
    public void NextLevel_ReturnsTheChildrenOfTheWholeFrontierInOrder()
    {
        var root = WalkGraphs.Tree();

        var next = BreadthFirstWalk.NextLevel<
            TestNode, TestTopology, ListChildren<TestNode>,
            NaturalChildOrder<TestNode, ListChildren<TestNode>>, ListChildren<TestNode>,
            UnguardedVisit<TestNode>>([root], new UnguardedVisit<TestNode>());

        Assert.Equal(["B", "C"], next.Select(n => n.Name));
    }

    [Fact]
    public void NextLevel_LeafFrontier_ReturnsEmpty()
    {
        var next = BreadthFirstWalk.NextLevel<
            TestNode, TestTopology, ListChildren<TestNode>,
            NaturalChildOrder<TestNode, ListChildren<TestNode>>, ListChildren<TestNode>,
            UnguardedVisit<TestNode>>([new TestNode("A")], new UnguardedVisit<TestNode>());

        Assert.Empty(next);
    }

    [Fact]
    public void NextLevel_AppliesTheGuardSoASharedChildAppearsOnce()
    {
        var (root, shared) = WalkGraphs.DiamondShare();
        var guard = new TrackedVisitGuard<TestNode>([root]);

        var level1 = BreadthFirstWalk.NextLevel<
            TestNode, TestTopology, ListChildren<TestNode>,
            NaturalChildOrder<TestNode, ListChildren<TestNode>>, ListChildren<TestNode>,
            TrackedVisitGuard<TestNode>>([root], guard);

        var level2 = BreadthFirstWalk.NextLevel<
            TestNode, TestTopology, ListChildren<TestNode>,
            NaturalChildOrder<TestNode, ListChildren<TestNode>>, ListChildren<TestNode>,
            TrackedVisitGuard<TestNode>>(level1, guard);

        Assert.Equal([shared], level2);
    }

    [Fact]
    public void NextLevel_RespectsTheChildOrderWitness()
    {
        var root = WalkGraphs.Tree();

        var next = BreadthFirstWalk.NextLevel<
            TestNode, TestTopology, ListChildren<TestNode>,
            ReverseChildOrder<TestNode, ListChildren<TestNode>>, ReversedChildren<TestNode, ListChildren<TestNode>>,
            UnguardedVisit<TestNode>>([root], new UnguardedVisit<TestNode>());

        Assert.Equal(["C", "B"], next.Select(n => n.Name));
    }
}
