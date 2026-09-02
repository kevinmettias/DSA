using DSAExperimentation.Algorithms.Walking;
using DSAExperimentation.DataStructures.Graph.Contracts.Ordering;
using DSAExperimentation.Tests.Algorithms.Walking.Fixtures;
using DSAExperimentation.Tests.DataStructures.Graph.Fixtures;

namespace DSAExperimentation.Tests.Algorithms.Walking;

public sealed class DepthFirstWalkTests
{
    private static List<(string Name, int Depth)> WalkTree(TestNode root) =>
        DepthFirstWalk.Walk<
            TestNode, TestTopology, ListChildren<TestNode>,
            NaturalChildOrder<TestNode, ListChildren<TestNode>>, ListChildren<TestNode>,
            UnguardedVisit<TestNode>, RecordingWalkStep, List<(string, int)>>(
            root, 0, [], new UnguardedVisit<TestNode>());

    private static List<(string Name, int Depth)> WalkGraph(TestNode root) =>
        DepthFirstWalk.Walk<
            TestNode, TestTopology, ListChildren<TestNode>,
            NaturalChildOrder<TestNode, ListChildren<TestNode>>, ListChildren<TestNode>,
            TrackedVisitGuard<TestNode>, RecordingWalkStep, List<(string, int)>>(
            root, 0, [], new TrackedVisitGuard<TestNode>([root]));

    [Fact]
    public void Walk_DescendsEachBranchFullyBeforeTheNext()
    {
        var visited = WalkTree(WalkGraphs.Tree());

        Assert.Equal(["A", "B", "D", "E", "C"], visited.Select(v => v.Name));
    }

    [Fact]
    public void Walk_ReportsDepthRelativeToTheStartingDepth()
    {
        var visited = WalkTree(WalkGraphs.Tree());

        Assert.Equal([0, 1, 2, 2, 1], visited.Select(v => v.Depth));
    }

    [Fact]
    public void Walk_StartingDepthIsHonoured()
    {
        var visited = DepthFirstWalk.Walk<
            TestNode, TestTopology, ListChildren<TestNode>,
            NaturalChildOrder<TestNode, ListChildren<TestNode>>, ListChildren<TestNode>,
            UnguardedVisit<TestNode>, RecordingWalkStep, List<(string, int)>>(
            new TestNode("A"), 7, [], new UnguardedVisit<TestNode>());

        Assert.Equal([("A", 7)], visited);
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

        Assert.Equal(["A", "B", "D", "C"], visited.Select(v => v.Name));
    }

    [Fact]
    public void Walk_UnguardedOnASharedDescendant_VisitsItOncePerPath()
    {
        // Not a defect: UnguardedVisit is only valid under ITreeTopology's unique
        // ancestry promise, and this pins what breaking that promise costs.
        var (root, _) = WalkGraphs.DiamondShare();

        var visited = WalkTree(root);

        Assert.Equal(["A", "B", "D", "C", "D"], visited.Select(v => v.Name));
    }

    [Fact]
    public void Walk_RespectsTheChildOrderWitness()
    {
        var visited = DepthFirstWalk.Walk<
            TestNode, TestTopology, ListChildren<TestNode>,
            ReverseChildOrder<TestNode, ListChildren<TestNode>>, ReversedChildren<TestNode, ListChildren<TestNode>>,
            UnguardedVisit<TestNode>, RecordingWalkStep, List<(string Name, int Depth)>>(
            WalkGraphs.Tree(), 0, [], new UnguardedVisit<TestNode>());

        Assert.Equal(["A", "C", "B", "E", "D"], visited.Select(v => v.Name));
    }
}
