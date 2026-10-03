using DSAExperimentation.DataStructures.Graph.Contracts.Ordering;
using DSAExperimentation.Algorithms.Traversal.DepthFirst;
using DSAExperimentation.Tests.Algorithms.Traversal.DepthFirst.Fixtures;
using DSAExperimentation.Tests.DataStructures.Graph.Fixtures;

namespace DSAExperimentation.Tests.Algorithms.Traversal.DepthFirst;

public sealed partial class DepthFirstTraversalTests
{
    private struct PreOrderMarker;
    private struct PostOrderMarker;
    private struct EnterExitMarker;
    private struct NullRootMarker;
    private struct ReverseOrderMarker;
    private struct GraphCycleMarker;
    private struct GraphNullRootMarker;
    private struct GraphVisitedRootMarker;
    private struct GraphFirstCallMarker;
    private struct GraphSecondCallMarker;

    // A -> [B, C, D], B -> [E, F], D -> [G] (TestTrees.NArySample)
    [Fact]
    public void Walk_Enter_FiresInPreOrder()
    {
        var root = TestTrees.NArySample();

        DepthFirstTraversal.Walk<
            TestNode,
            TestTopology,
            ListChildren<TestNode>,
            NaturalChildOrder<TestNode, ListChildren<TestNode>>,
            ListChildren<TestNode>,
            RecordingEnterHooks<PreOrderMarker>>(root);

        Assert.Equal(
            new[] { "A", "B", "E", "F", "C", "D", "G" },
            RecordingEnterHooks<PreOrderMarker>.Entered.Select(v => v.Name));
    }

    [Fact]
    public void Walk_Exit_FiresInPostOrder()
    {
        var root = TestTrees.NArySample();

        DepthFirstTraversal.Walk<
            TestNode,
            TestTopology,
            ListChildren<TestNode>,
            NaturalChildOrder<TestNode, ListChildren<TestNode>>,
            ListChildren<TestNode>,
            RecordingExitHooks<PostOrderMarker>>(root);

        Assert.Equal(
            new[] { "E", "F", "B", "C", "G", "D", "A" },
            RecordingExitHooks<PostOrderMarker>.Exited.Select(v => v.Name));
    }

    [Fact]
    public void Walk_EnterAndExit_BothFireInOnePassAtCorrectOrders()
    {
        var root = TestTrees.NArySample();

        DepthFirstTraversal.Walk<
            TestNode,
            TestTopology,
            ListChildren<TestNode>,
            NaturalChildOrder<TestNode, ListChildren<TestNode>>,
            ListChildren<TestNode>,
            RecordingEnterExitHooks<EnterExitMarker>>(root);

        Assert.Equal(
            new[] { "A", "B", "E", "F", "C", "D", "G" },
            RecordingEnterExitHooks<EnterExitMarker>.Entered.Select(v => v.Name));
        Assert.Equal(
            new[] { "E", "F", "B", "C", "G", "D", "A" },
            RecordingEnterExitHooks<EnterExitMarker>.Exited.Select(v => v.Name));
    }

    [Fact]
    public void Walk_NullRoot_NoVisits()
    {
        DepthFirstTraversal.Walk<
            TestNode,
            TestTopology,
            ListChildren<TestNode>,
            NaturalChildOrder<TestNode, ListChildren<TestNode>>,
            ListChildren<TestNode>,
            RecordingEnterHooks<NullRootMarker>>(null);

        Assert.Empty(RecordingEnterHooks<NullRootMarker>.Entered);
    }

    [Fact]
    public void Walk_ChildOrderIsOrthogonalToVisitTiming()
    {
        // Same pre-order timing, but every sibling group is walked back-to-front -
        // child order and visit-timing are independent knobs.
        var root = TestTrees.NArySample();

        DepthFirstTraversal.Walk<
            TestNode,
            TestTopology,
            ListChildren<TestNode>,
            ReverseChildOrder<TestNode, ListChildren<TestNode>>,
            ReversedChildren<TestNode, ListChildren<TestNode>>,
            RecordingEnterHooks<ReverseOrderMarker>>(root);

        Assert.Equal(
            new[] { "A", "D", "G", "C", "B", "F", "E" },
            RecordingEnterHooks<ReverseOrderMarker>.Entered.Select(v => v.Name));
    }

    // A -> [B, D], B -> C, C -> A (TestGraphs.CycleWithLeaf)
    [Fact]
    public void WalkGraph_VisitsEachNodeOnceDespiteCycle()
    {
        var root = TestGraphs.CycleWithLeaf();

        DepthFirstTraversal.WalkGraph<
            TestNode, TestTopology, ListChildren<TestNode>,
            NaturalChildOrder<TestNode, ListChildren<TestNode>>, ListChildren<TestNode>,
            RecordingEnterExitHooks<GraphCycleMarker>>(root);

        Assert.Equal(
            new[] { ("A", 0), ("B", 1), ("C", 2), ("D", 1) },
            RecordingEnterExitHooks<GraphCycleMarker>.Entered);
        Assert.Equal(
            new[] { ("C", 2), ("B", 1), ("D", 1), ("A", 0) },
            RecordingEnterExitHooks<GraphCycleMarker>.Exited);
    }

    [Fact]
    public void WalkGraph_NullRoot_NoVisits()
    {
        DepthFirstTraversal.WalkGraph<
            TestNode, TestTopology, ListChildren<TestNode>,
            NaturalChildOrder<TestNode, ListChildren<TestNode>>, ListChildren<TestNode>,
            RecordingEnterHooks<GraphNullRootMarker>>(null);

        Assert.Empty(RecordingEnterHooks<GraphNullRootMarker>.Entered);
    }

    [Fact]
    public void WalkGraph_RootAlreadyInTheVisitedSet_NoVisits()
    {
        var root = TestGraphs.CycleWithLeaf();

        DepthFirstTraversal.WalkGraph<
            TestNode, TestTopology, ListChildren<TestNode>,
            NaturalChildOrder<TestNode, ListChildren<TestNode>>, ListChildren<TestNode>,
            RecordingEnterHooks<GraphVisitedRootMarker>>(root, [root]);

        Assert.Empty(RecordingEnterHooks<GraphVisitedRootMarker>.Entered);
    }

    [Fact]
    public void WalkGraph_VisitedSet_CarriesAcrossCalls()
    {
        // X points into the cycle an earlier call already walked; the shared set is what
        // keeps the second walk to X alone.
        var cycle = TestGraphs.CycleWithLeaf();
        HashSet<TestNode> visited = [];
        EnteredWalkingGraphFrom<GraphFirstCallMarker>(cycle, visited);

        var second = EnteredWalkingGraphFrom<GraphSecondCallMarker>(new TestNode("X") { Children = { cycle } }, visited);

        Assert.Equal(new[] { ("X", 0) }, second);
    }

    private static IReadOnlyList<(string Name, int Depth)> EnteredWalkingGraphFrom<TMarker>(
        TestNode root, HashSet<TestNode> visited)
        where TMarker : struct
    {
        DepthFirstTraversal.WalkGraph<
            TestNode, TestTopology, ListChildren<TestNode>,
            NaturalChildOrder<TestNode, ListChildren<TestNode>>, ListChildren<TestNode>,
            RecordingEnterHooks<TMarker>>(root, visited);

        return RecordingEnterHooks<TMarker>.Entered;
    }
}
