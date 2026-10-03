using DSAExperimentation.DataStructures.Graph.Contracts.Ordering;
using DSAExperimentation.Algorithms.Traversal.BreadthFirst;
using DSAExperimentation.Tests.Algorithms.Traversal.BreadthFirst.Fixtures;
using DSAExperimentation.Tests.DataStructures.Graph.Fixtures;

namespace DSAExperimentation.Tests.Algorithms.Traversal.BreadthFirst;

public sealed partial class LevelGroupedBreadthFirstTraversalTests
{
    private struct LevelGroupMarker;
    private struct NullRootMarker;
    private struct SingleNodeMarker;

    // A -> [B, C, D], B -> [E, F], D -> [G] (TestTrees.NArySample)
    [Fact]
    public void Walk_GroupsNodesByLevel()
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

    [Fact]
    public void Walk_NullRoot_FiresNoLevels()
    {
        LevelGroupedBreadthFirstTraversal.Walk<
            TestNode,
            TestTopology,
            ListChildren<TestNode>,
            NaturalChildOrder<TestNode, ListChildren<TestNode>>,
            ListChildren<TestNode>,
            RecordingLevelHooks<NullRootMarker>>(null);

        Assert.Empty(RecordingLevelHooks<NullRootMarker>.Levels);
    }

    [Fact]
    public void Walk_SingleNode_FiresOneLevelAtDepthZero()
    {
        // The walk stops on the first empty frontier rather than reporting it as a level.
        LevelGroupedBreadthFirstTraversal.Walk<
            TestNode,
            TestTopology,
            ListChildren<TestNode>,
            NaturalChildOrder<TestNode, ListChildren<TestNode>>,
            ListChildren<TestNode>,
            RecordingLevelHooks<SingleNodeMarker>>(TestTrees.SingleNode());

        var level = Assert.Single(RecordingLevelHooks<SingleNodeMarker>.Levels);
        Assert.Equal(0, level.Depth);
        Assert.Equal(new[] { "A" }, level.Names);
    }
}
