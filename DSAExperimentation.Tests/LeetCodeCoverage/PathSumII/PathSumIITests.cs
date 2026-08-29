using DSAExperimentation.Algorithms.Paths;
using DSAExperimentation.DataStructures.Graph.Engines.Dags.Trees;
using DSAExperimentation.DataStructures.Graph.Contracts.Ordering;

namespace DSAExperimentation.Tests.LeetCodeCoverage.PathSumII;

public sealed partial class PathSumIITests
{
    [Fact]
    public void PathSum_ClassicExample_ReturnsMatchingRootToLeafPaths()
    {
        var paths = PathSum(Tree(), 22);
        Assert.Contains(paths, p => p.SequenceEqual([5, 4, 11, 2]));
        Assert.Contains(paths, p => p.SequenceEqual([5, 8, 4, 5]));
    }

    private static List<List<int>> PathSum(BinaryTreeNode<int>? root, int target)
        => AllRootToLeafPaths.Find<BinaryTreeNode<int>, BinaryTreeTopology<int>, BinaryTreeChildren<int>, NaturalChildOrder<BinaryTreeNode<int>, BinaryTreeChildren<int>>, BinaryTreeChildren<int>>(root)
            .Where(path => path.Sum(node => node.Value) == target)
            .Select(path => path.Select(node => node.Value).ToList())
            .ToList();

    private static BinaryTreeNode<int> Tree() => new(5) { Left = new(4) { Left = new(11) { Left = new(7), Right = new(2) } }, Right = new(8) { Left = new(13), Right = new(4) { Left = new(5), Right = new(1) } } };
}
