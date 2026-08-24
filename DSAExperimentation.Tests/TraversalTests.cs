using DSAExperimentation.Graph;

namespace DSAExperimentation.Tests;

public sealed class TraversalTests
{
    private struct PreOrderMarker;
    private struct PostOrderMarker;
    private struct EnterExitMarker;
    private struct DfsNullMarker;
    private struct ReverseOrderMarker;
    private struct BfsMarker;
    private struct BfsNullMarker;
    private struct LevelGroupMarker;

    [Fact]
    public void Dfs_Enter_FiresInPreOrder()
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
    public void Dfs_Exit_FiresInPostOrder()
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
    public void Dfs_EnterAndExit_BothFireInOnePassAtCorrectOrders()
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
    public void Dfs_NullRoot_NoVisits()
    {
        DepthFirstTraversal.Walk<
            TestNode,
            TestTopology,
            ListChildren<TestNode>,
            NaturalChildOrder<TestNode, ListChildren<TestNode>>,
            ListChildren<TestNode>,
            RecordingEnterHooks<DfsNullMarker>>(null);

        Assert.Empty(RecordingEnterHooks<DfsNullMarker>.Entered);
    }

    [Fact]
    public void ChildOrder_IsOrthogonalToVisitTiming()
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

    [Fact]
    public void Bfs_VisitsInBreadthFirstOrderWithDepth()
    {
        var root = TestTrees.NArySample();

        BreadthFirstTraversal.Walk<
            TestNode,
            TestTopology,
            ListChildren<TestNode>,
            NaturalChildOrder<TestNode, ListChildren<TestNode>>,
            ListChildren<TestNode>,
            RecordingVisitHooks<BfsMarker>>(root);

        Assert.Equal(
            new[] { ("A", 0), ("B", 1), ("C", 1), ("D", 1), ("E", 2), ("F", 2), ("G", 2) },
            RecordingVisitHooks<BfsMarker>.Visited);
    }

    [Fact]
    public void Bfs_NullRoot_NoVisits()
    {
        BreadthFirstTraversal.Walk<
            TestNode,
            TestTopology,
            ListChildren<TestNode>,
            NaturalChildOrder<TestNode, ListChildren<TestNode>>,
            ListChildren<TestNode>,
            RecordingVisitHooks<BfsNullMarker>>(null);

        Assert.Empty(RecordingVisitHooks<BfsNullMarker>.Visited);
    }

    [Fact]
    public void LevelGrouped_GroupsNodesByLevel()
    {
        var root = TestTrees.NArySample();

        LevelGroupedBreadthFirstTraversal.Walk<
            TestNode,
            TestTopology,
            ListChildren<TestNode>,
            NaturalChildOrder<TestNode, ListChildren<TestNode>>,
            ListChildren<TestNode>,
            RecordingLevelHooks<LevelGroupMarker>>(root);

        var levels = RecordingLevelHooks<LevelGroupMarker>.Levels;

        Assert.Equal(3, levels.Count);

        Assert.Equal(0, levels[0].Depth);
        Assert.Equal(new[] { "A" }, levels[0].Names);

        Assert.Equal(1, levels[1].Depth);
        Assert.Equal(new[] { "B", "C", "D" }, levels[1].Names);

        Assert.Equal(2, levels[2].Depth);
        Assert.Equal(new[] { "E", "F", "G" }, levels[2].Names);
    }
}
