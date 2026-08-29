using DSAExperimentation.Algorithms.Traversal.BreadthFirst;
using DSAExperimentation.DataStructures.Graph.Engines.Dags.Trees;
using DSAExperimentation.DataStructures.Graph.Contracts.Ordering;

namespace DSAExperimentation.Tests.LeetCodeCoverage.BinaryTreeLevelOrderTraversal;

public sealed partial class BinaryTreeLevelOrderTraversalTests
{
    [Fact]
    public void LevelOrder_ClassicExample_ReturnsLevelsTopDown()
        => Assert.Equal([[3], [9, 20], [15, 7]], LevelOrder(Tree()));

    private static List<List<int>> LevelOrder(BinaryTreeNode<int>? root)
    {
        LevelHooks.Output.Value = [];
        LevelGroupedBreadthFirstTraversal.Walk<BinaryTreeNode<int>, BinaryTreeTopology<int>, BinaryTreeChildren<int>, NaturalChildOrder<BinaryTreeNode<int>, BinaryTreeChildren<int>>, BinaryTreeChildren<int>, LevelHooks>(root);
        return LevelHooks.Output.Value!;
    }

    private readonly struct LevelHooks : ILevelGroupedHooks<BinaryTreeNode<int>>
    {
        public static readonly AsyncLocal<List<List<int>>> Output = new();
        public static void OnLevel(IReadOnlyList<BinaryTreeNode<int>> level, int depth) => Output.Value!.Add(level.Select(n => n.Value).ToList());
    }

    private static BinaryTreeNode<int> Tree() => new(3) { Left = new(9), Right = new(20) { Left = new(15), Right = new(7) } };
}
