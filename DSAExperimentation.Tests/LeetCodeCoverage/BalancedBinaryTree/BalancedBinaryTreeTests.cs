using DSAExperimentation.DataStructures.Graph.Engines.Dags.Trees;

namespace DSAExperimentation.Tests.LeetCodeCoverage.BalancedBinaryTree;

public sealed partial class BalancedBinaryTreeTests
{
    [Fact] public void IsBalanced_BalancedTree_ReturnsTrue() => Assert.True(IsBalanced(new BinaryTreeNode<int>(3) { Left = new(9), Right = new(20) { Left = new(15), Right = new(7) } }));
    [Fact] public void IsBalanced_UnbalancedTree_ReturnsFalse() => Assert.False(IsBalanced(new BinaryTreeNode<int>(1) { Left = new(2) { Left = new(3) { Left = new(4) } } }));
    private static bool IsBalanced(BinaryTreeNode<int>? root) => HeightOrUnbalanced(root) >= 0;
    private static int HeightOrUnbalanced(BinaryTreeNode<int>? node) { if (node is null) return 0; var left = HeightOrUnbalanced(node.Left); var right = HeightOrUnbalanced(node.Right); if (left < 0 || right < 0 || Math.Abs(left - right) > 1) return -1; return 1 + Math.Max(left, right); }
}
