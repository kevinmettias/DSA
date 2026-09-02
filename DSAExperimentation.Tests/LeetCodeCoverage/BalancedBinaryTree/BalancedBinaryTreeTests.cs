using DSAExperimentation.DataStructures.Graph.Engines.Dags.Trees;
using DSAExperimentation.LeetCode.BalancedBinaryTree;

namespace DSAExperimentation.Tests.LeetCodeCoverage.BalancedBinaryTree;

// Harness only. The height-or-unbalanced recursion is
// BalancedBinaryTreeSolution's; this file just pins it to LeetCode's
// published examples plus the trivial empty-tree and single-node cases.
public sealed class BalancedBinaryTreeTests
{
    [Fact]
    public void IsBalancedByHeightRecursion_BalancedTree_ReturnsTrue() =>
        Assert.True(BalancedBinaryTreeSolution.IsBalancedByHeightRecursion(
            new BinaryTreeNode<int>(3) { Left = new(9), Right = new(20) { Left = new(15), Right = new(7) } }));

    [Fact]
    public void IsBalancedByHeightRecursion_UnbalancedTree_ReturnsFalse() =>
        Assert.False(BalancedBinaryTreeSolution.IsBalancedByHeightRecursion(
            new BinaryTreeNode<int>(1) { Left = new(2) { Left = new(3) { Left = new(4) } } }));

    [Fact]
    public void IsBalancedByHeightRecursion_EmptyTree_ReturnsTrue() =>
        Assert.True(BalancedBinaryTreeSolution.IsBalancedByHeightRecursion(null));

    [Fact]
    public void IsBalancedByHeightRecursion_SingleNode_ReturnsTrue() =>
        Assert.True(BalancedBinaryTreeSolution.IsBalancedByHeightRecursion(new BinaryTreeNode<int>(1)));
}
