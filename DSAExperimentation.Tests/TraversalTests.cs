using DSAExperimentation.Trees;

namespace DSAExperimentation.Tests;

public sealed class TraversalTests
{
    private struct BfsOrderMarker;
    private struct BfsNullMarker;
    private struct BfsPlainVisitMarker;
    private struct LevelGroupMarker;
    private struct PreOrderMarker;
    private struct PostOrderMarker;
    private struct DfsNullMarker;
    private struct PreAndPostOrderEnterMarker;
    private struct PreAndPostOrderExitMarker;

    [Fact]
    public void Bfs_VisitsInBreadthFirstOrderWithDepth()
    {
        var root = TestTrees.NArySample();

        DepthAwareBreadthFirstTraversal<
            TestNode,
            TestTopology,
            NaturalChildOrder<TestNode>,
            RecordingDepthAwareAction<BfsOrderMarker>>.Traverse(root);

        Assert.Equal(
            new[] { ("A", 0), ("B", 1), ("C", 1), ("D", 1), ("E", 2), ("F", 2), ("G", 2) },
            RecordingDepthAwareAction<BfsOrderMarker>.Visited);
    }

    [Fact]
    public void Bfs_PlainVisit_VisitsInBreadthFirstOrder()
    {
        var root = TestTrees.NArySample();

        NodeVisitBreadthFirstTraversal<
            TestNode,
            TestTopology,
            NaturalChildOrder<TestNode>,
            RecordingNodeAction<BfsPlainVisitMarker>>.Traverse(root);

        Assert.Equal(
            new[] { "A", "B", "C", "D", "E", "F", "G" },
            RecordingNodeAction<BfsPlainVisitMarker>.Visited);
    }

    [Fact]
    public void Bfs_NullRoot_NoVisits()
    {
        DepthAwareBreadthFirstTraversal<
            TestNode,
            TestTopology,
            NaturalChildOrder<TestNode>,
            RecordingDepthAwareAction<BfsNullMarker>>.Traverse(null);

        Assert.Empty(RecordingDepthAwareAction<BfsNullMarker>.Visited);
    }

    [Fact]
    public void LevelGrouped_GroupsNodesByLevel()
    {
        var root = TestTrees.NArySample();

        LevelGroupedBreadthFirstTraversal<
            TestNode,
            TestTopology,
            NaturalChildOrder<TestNode>,
            RecordingLevelAction<LevelGroupMarker>>.Traverse(root);

        var levels = RecordingLevelAction<LevelGroupMarker>.Levels;

        Assert.Equal(3, levels.Count);

        Assert.Equal(0, levels[0].Depth);
        Assert.Equal(new[] { "A" }, levels[0].Names);

        Assert.Equal(1, levels[1].Depth);
        Assert.Equal(new[] { "B", "C", "D" }, levels[1].Names);

        Assert.Equal(2, levels[2].Depth);
        Assert.Equal(new[] { "E", "F", "G" }, levels[2].Names);
    }

    [Fact]
    public void Dfs_PreOrder_VisitsParentBeforeChildren()
    {
        var root = TestTrees.NArySample();

        PreOrderDepthFirstTraversal<
            TestNode,
            TestTopology,
            NaturalChildOrder<TestNode>,
            RecordingNodeAction<PreOrderMarker>>.Traverse(root);

        Assert.Equal(
            new[] { "A", "B", "E", "F", "C", "D", "G" },
            RecordingNodeAction<PreOrderMarker>.Visited);
    }

    [Fact]
    public void Dfs_PostOrder_VisitsChildrenBeforeParent()
    {
        var root = TestTrees.NArySample();

        PostOrderDepthFirstTraversal<
            TestNode,
            TestTopology,
            NaturalChildOrder<TestNode>,
            RecordingNodeAction<PostOrderMarker>>.Traverse(root);

        Assert.Equal(
            new[] { "E", "F", "B", "C", "G", "D", "A" },
            RecordingNodeAction<PostOrderMarker>.Visited);
    }

    [Fact]
    public void Dfs_PreAndPostOrder_BothFireInOnePassAtCorrectOrders()
    {
        var root = TestTrees.NArySample();

        PreAndPostOrderDepthFirstTraversal<
            TestNode,
            TestTopology,
            NaturalChildOrder<TestNode>,
            RecordingNodeAction<PreAndPostOrderEnterMarker>,
            RecordingNodeAction<PreAndPostOrderExitMarker>>.Traverse(root);

        Assert.Equal(
            new[] { "A", "B", "E", "F", "C", "D", "G" },
            RecordingNodeAction<PreAndPostOrderEnterMarker>.Visited);
        Assert.Equal(
            new[] { "E", "F", "B", "C", "G", "D", "A" },
            RecordingNodeAction<PreAndPostOrderExitMarker>.Visited);
    }

    [Fact]
    public void Dfs_NullRoot_NoVisits()
    {
        PreOrderDepthFirstTraversal<
            TestNode,
            TestTopology,
            NaturalChildOrder<TestNode>,
            RecordingNodeAction<DfsNullMarker>>.Traverse(null);

        Assert.Empty(RecordingNodeAction<DfsNullMarker>.Visited);
    }
}
