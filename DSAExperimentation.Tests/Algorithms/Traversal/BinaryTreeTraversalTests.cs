using DSAExperimentation.DataStructures.Graph.Engines.Dags.Trees;
using DSAExperimentation.DataStructures.Graph.Contracts.Ordering;
using DSAExperimentation.Algorithms.Traversal.BreadthFirst;
using DSAExperimentation.Algorithms.Traversal.DepthFirst;
using DSAExperimentation.Tests.Algorithms.Traversal.BreadthFirst.Fixtures;
using DSAExperimentation.Tests.Algorithms.Traversal.DepthFirst.Fixtures;
using DSAExperimentation.Tests.DataStructures.Graph.Engines.Dags.Trees.Fixtures;

namespace DSAExperimentation.Tests.Algorithms.Traversal;

// Proves BinaryTreeTopology/BinaryTreeChildren correctly close the SAME generic
// engines DepthFirstTraversalTests, BreadthFirstTraversalTests and
// LevelGroupedBreadthFirstTraversalTests already exercise against
// TestTopology/ListChildren - same precedent as GridConnectedComponentsTests.cs
// sitting beside ConnectedComponentsTests.cs for a second concrete topology.
public sealed partial class BinaryTreeTraversalTests
{
    private struct LevelGroupMarker;

    [Fact]
    public void Dfs_Enter_FiresInPreOrder()
    {
        var entered = new List<(int Value, int Depth)>();

        DepthFirstTraversal.Walk<
            BinaryTreeNode<int>,
            BinaryTreeTopology<int>,
            BinaryTreeChildren<int>,
            NaturalChildOrder<BinaryTreeNode<int>, BinaryTreeChildren<int>>,
            BinaryTreeChildren<int>,
            RecordingBinaryTreeEnterHooks<int>>(
            BinaryTreeTrees.Sample(), new RecordingBinaryTreeEnterHooks<int>(entered));

        Assert.Equal(
            new[] { 4, 2, 1, 3, 6, 7 },
            entered.Select(v => v.Value));
    }

    [Fact]
    public void Dfs_Exit_FiresInPostOrder()
    {
        var exited = new List<(int Value, int Depth)>();

        DepthFirstTraversal.Walk<
            BinaryTreeNode<int>,
            BinaryTreeTopology<int>,
            BinaryTreeChildren<int>,
            NaturalChildOrder<BinaryTreeNode<int>, BinaryTreeChildren<int>>,
            BinaryTreeChildren<int>,
            RecordingBinaryTreeExitHooks<int>>(
            BinaryTreeTrees.Sample(), new RecordingBinaryTreeExitHooks<int>(exited));

        Assert.Equal(
            new[] { 1, 3, 2, 7, 6, 4 },
            exited.Select(v => v.Value));
    }

    [Fact]
    public void LevelGrouped_GroupsNodesByLevel()
    {
        var root = BinaryTreeTrees.Sample();

        LevelGroupedBreadthFirstTraversal.Walk<
            BinaryTreeNode<int>,
            BinaryTreeTopology<int>,
            BinaryTreeChildren<int>,
            NaturalChildOrder<BinaryTreeNode<int>, BinaryTreeChildren<int>>,
            BinaryTreeChildren<int>,
            RecordingBinaryTreeLevelHooks<int, LevelGroupMarker>>(root);

        var levels = RecordingBinaryTreeLevelHooks<int, LevelGroupMarker>.Levels;

        Assert.Equal(3, levels.Count);

        Assert.Equal(0, levels[0].Depth);
        Assert.Equal(new[] { 4 }, levels[0].Values);

        Assert.Equal(1, levels[1].Depth);
        Assert.Equal(new[] { 2, 6 }, levels[1].Values);

        Assert.Equal(2, levels[2].Depth);
        Assert.Equal(new[] { 1, 3, 7 }, levels[2].Values);
    }
}
