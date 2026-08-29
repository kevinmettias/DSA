using DSAExperimentation.Algorithms.Traversal.BreadthFirst;
using DSAExperimentation.DataStructures.Graph.Engines.Dags.Trees;
using DSAExperimentation.DataStructures.Graph.Contracts.Ordering;

namespace DSAExperimentation.Tests.LeetCodeCoverage.BinaryTreeLevelOrderTraversalII;

public sealed partial class BinaryTreeLevelOrderTraversalIITests
{
    [Fact]
    public void LevelOrderBottom_ClassicExample_ReturnsLevelsBottomUp()
        => Assert.Equal([[15, 7], [9, 20], [3]], LevelOrderBottom(Tree()));

    private static List<List<int>> LevelOrderBottom(BinaryTreeNode<int>? root)
    {
        LevelHooks.Output.Value = [];
        LevelGroupedBreadthFirstTraversal.Walk<BinaryTreeNode<int>, BinaryTreeTopology<int>, BinaryTreeChildren<int>, NaturalChildOrder<BinaryTreeNode<int>, BinaryTreeChildren<int>>, BinaryTreeChildren<int>, LevelHooks>(root);
        var levels = LevelHooks.Output.Value!;
        levels.Reverse();
        return levels;
    }

    private readonly struct LevelHooks : ILevelGroupedHooks<BinaryTreeNode<int>>
    {
        public static readonly AsyncLocal<List<List<int>>> Output = new();
        public static void OnLevel(IReadOnlyList<BinaryTreeNode<int>> level, int depth) => Output.Value!.Add(level.Select(n => n.Value).ToList());
    }

    private static BinaryTreeNode<int> Tree() => new(3) { Left = new(9), Right = new(20) { Left = new(15), Right = new(7) } };
}
