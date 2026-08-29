using DSAExperimentation.DataStructures.Graph.Engines.Dags.Trees;

namespace DSAExperimentation.Tests.LeetCodeCoverage.BinaryTreeMaximumPathSum;

public sealed partial class BinaryTreeMaximumPathSumTests
{
    [Fact] public void MaxPathSum_ClassicExample_ReturnsBestPath() => Assert.Equal(42, MaxPathSum(new BinaryTreeNode<int>(-10) { Left = new(9), Right = new(20) { Left = new(15), Right = new(7) } }));
    private static int MaxPathSum(BinaryTreeNode<int>? root) { var best = int.MinValue; Gain(root); return best; int Gain(BinaryTreeNode<int>? node) { if (node is null) return 0; var left = Math.Max(0, Gain(node.Left)); var right = Math.Max(0, Gain(node.Right)); best = Math.Max(best, node.Value + left + right); return node.Value + Math.Max(left, right); } }
}
