using DSAExperimentation.DataStructures.Graph.Contracts.Ordering;
using DSAExperimentation.Algorithms.Traversal.BreadthFirst;
using DSAExperimentation.Tests.Algorithms.Traversal.BreadthFirst.Fixtures;
using DSAExperimentation.Tests.DataStructures.Graph.Fixtures;

namespace DSAExperimentation.Tests.Algorithms.Traversal.BreadthFirst;

public sealed partial class LevelGroupedBreadthFirstTraversalTests
{
    // TestTrees.NArySample's depth count: A, then B C D, then E F G.
    private const int NArySampleLevelCount = 3;

    // A -> [B, C, D], B -> [E, F], D -> [G] (TestTrees.NArySample)
    [Fact]
    public void Walk_GroupsNodesByLevel()
    {
        var levels = Recorded(TestTrees.NArySample());

        Assert.Equal(NArySampleLevelCount, levels.Count);

        Assert.Equal(0, levels[0].Depth);
        Assert.Equal(new[] { "A" }, levels[0].Names);

        Assert.Equal(1, levels[1].Depth);
        Assert.Equal(new[] { "B", "C", "D" }, levels[1].Names);

        Assert.Equal(2, levels[2].Depth);
        Assert.Equal(new[] { "E", "F", "G" }, levels[2].Names);
    }

    [Fact]
    public void Walk_NullRoot_FiresNoLevels() => Assert.Empty(Recorded(null));

    [Fact]
    public void Walk_SingleNode_FiresOneLevelAtDepthZero()
    {
        // The walk stops on the first empty frontier rather than reporting it as a level.
        var level = Assert.Single(Recorded(TestTrees.SingleNode()));
        Assert.Equal(0, level.Depth);
        Assert.Equal(new[] { "A" }, level.Names);
    }

    // A hook held by value comes back with what the walk did to it, not as it went in.
    [Fact]
    public void Walk_ReturnsTheHookValueTheWalkFinishedWith()
    {
        var hooks = LevelGroupedBreadthFirstTraversal.Walk<
            TestNode, TestTopology, ListChildren<TestNode>,
            NaturalChildOrder<TestNode, ListChildren<TestNode>>, ListChildren<TestNode>,
            CountingLevelHooks>(TestTrees.NArySample(), new CountingLevelHooks());

        Assert.Equal(NArySampleLevelCount, hooks.Levels);
    }

    private static List<(int Depth, List<string> Names)> Recorded(TestNode? root)
    {
        var levels = new List<(int Depth, List<string> Names)>();

        LevelGroupedBreadthFirstTraversal.Walk<
            TestNode, TestTopology, ListChildren<TestNode>,
            NaturalChildOrder<TestNode, ListChildren<TestNode>>, ListChildren<TestNode>,
            RecordingLevelHooks>(root, new RecordingLevelHooks(levels));

        return levels;
    }
}
