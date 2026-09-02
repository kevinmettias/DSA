using DSAExperimentation.Algorithms.Paths;
using DSAExperimentation.DataStructures.Graph.Engines.Dags.Trees;
using DSAExperimentation.DataStructures.Graph.Contracts.Ordering;

namespace DSAExperimentation.Tests.LeetCodeCoverage.PathSum;

public sealed partial class PathSumTests
{
    [Fact]
    public void HasPathSum_ClassicExample_ReturnsTrue()
    {
        var hasPathSum = HasPathSum(Tree(), 22);
        Assert.True(hasPathSum);
    }

    private static bool HasPathSum(BinaryTreeNode<int>? root, int target)
        => AllRootToLeafPaths.Find<BinaryTreeNode<int>, BinaryTreeTopology<int>, BinaryTreeChildren<int>, NaturalChildOrder<BinaryTreeNode<int>, BinaryTreeChildren<int>>, BinaryTreeChildren<int>>(root)
            .Any(path => path.Sum(node => node.Value) == target);

    private static BinaryTreeNode<int> Tree() => new(5) { Left = new(4) { Left = new(11) { Left = new(7), Right = new(2) } }, Right = new(8) { Left = new(13), Right = new(4) { Right = new(1) } } };
}
