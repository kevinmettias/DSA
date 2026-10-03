using DSAExperimentation.DataStructures.Graph.Contracts.Ordering;
using DSAExperimentation.Algorithms.Traversal.BreadthFirst;
using DSAExperimentation.Tests.Algorithms.Traversal.BreadthFirst.Fixtures;
using DSAExperimentation.Tests.DataStructures.Graph.Fixtures;

namespace DSAExperimentation.Tests.Algorithms.Traversal.BreadthFirst;

public sealed partial class BreadthFirstTraversalTests
{
    private struct TreeMarker;
    private struct NullRootMarker;
    private struct GraphCycleMarker;
    private struct GraphNullRootMarker;
    private struct GraphVisitedRootMarker;
    private struct GraphFirstCallMarker;
    private struct GraphSecondCallMarker;

    // A -> [B, C, D], B -> [E, F], D -> [G] (TestTrees.NArySample)
    [Fact]
    public void Walk_VisitsInBreadthFirstOrderWithDepth()
    {
        var root = TestTrees.NArySample();

        BreadthFirstTraversal.Walk<
            TestNode,
            TestTopology,
            ListChildren<TestNode>,
            NaturalChildOrder<TestNode, ListChildren<TestNode>>,
            ListChildren<TestNode>,
            RecordingVisitHooks<TreeMarker>>(root);

        Assert.Equal(
            new[] { ("A", 0), ("B", 1), ("C", 1), ("D", 1), ("E", 2), ("F", 2), ("G", 2) },
            RecordingVisitHooks<TreeMarker>.Visited);
    }

    [Fact]
    public void Walk_NullRoot_NoVisits()
    {
        BreadthFirstTraversal.Walk<
            TestNode,
            TestTopology,
            ListChildren<TestNode>,
            NaturalChildOrder<TestNode, ListChildren<TestNode>>,
            ListChildren<TestNode>,
            RecordingVisitHooks<NullRootMarker>>(null);

        Assert.Empty(RecordingVisitHooks<NullRootMarker>.Visited);
    }

    // A -> [B, D], B -> C, C -> A (TestGraphs.CycleWithLeaf)
    [Fact]
    public void WalkGraph_VisitsEachNodeOnceDespiteCycle()
    {
        // C's edge back to A is dropped by the guard; D, on A's level-one frontier, is
        // visited before C on level two.
        var root = TestGraphs.CycleWithLeaf();

        BreadthFirstTraversal.WalkGraph<
            TestNode, TestTopology, ListChildren<TestNode>,
            NaturalChildOrder<TestNode, ListChildren<TestNode>>, ListChildren<TestNode>,
            RecordingVisitHooks<GraphCycleMarker>>(root);

        Assert.Equal(
            new[] { ("A", 0), ("B", 1), ("D", 1), ("C", 2) },
            RecordingVisitHooks<GraphCycleMarker>.Visited);
    }

    [Fact]
    public void WalkGraph_NullRoot_NoVisits()
    {
        BreadthFirstTraversal.WalkGraph<
            TestNode, TestTopology, ListChildren<TestNode>,
            NaturalChildOrder<TestNode, ListChildren<TestNode>>, ListChildren<TestNode>,
            RecordingVisitHooks<GraphNullRootMarker>>(null);

        Assert.Empty(RecordingVisitHooks<GraphNullRootMarker>.Visited);
    }

    [Fact]
    public void WalkGraph_RootAlreadyInTheVisitedSet_NoVisits()
    {
        var root = TestGraphs.CycleWithLeaf();

        BreadthFirstTraversal.WalkGraph<
            TestNode, TestTopology, ListChildren<TestNode>,
            NaturalChildOrder<TestNode, ListChildren<TestNode>>, ListChildren<TestNode>,
            RecordingVisitHooks<GraphVisitedRootMarker>>(root, [root]);

        Assert.Empty(RecordingVisitHooks<GraphVisitedRootMarker>.Visited);
    }

    [Fact]
    public void WalkGraph_VisitedSet_CarriesAcrossCalls()
    {
        // X points into the cycle an earlier call already walked; the shared set is what
        // keeps the second walk to X alone.
        var cycle = TestGraphs.CycleWithLeaf();
        HashSet<TestNode> visited = [];
        VisitedWalkingGraphFrom<GraphFirstCallMarker>(cycle, visited);

        var second = VisitedWalkingGraphFrom<GraphSecondCallMarker>(new TestNode("X") { Children = { cycle } }, visited);

        Assert.Equal(new[] { ("X", 0) }, second);
    }

    private static IReadOnlyList<(string Name, int Depth)> VisitedWalkingGraphFrom<TMarker>(
        TestNode root, HashSet<TestNode> visited)
        where TMarker : struct
    {
        BreadthFirstTraversal.WalkGraph<
            TestNode, TestTopology, ListChildren<TestNode>,
            NaturalChildOrder<TestNode, ListChildren<TestNode>>, ListChildren<TestNode>,
            RecordingVisitHooks<TMarker>>(root, visited);

        return RecordingVisitHooks<TMarker>.Visited;
    }
}
