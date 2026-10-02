using DSAExperimentation.DataStructures.Graph.Engines.Dags.Trees;
using DSAExperimentation.LeetCode.BalancedBinaryTree;

namespace DSAExperimentation.Tests.LeetCodeCoverage.BalancedBinaryTree;

// Harness only. Both strategies live in BalancedBinaryTreeSolution - the
// height-or-unbalanced recursion and the top-down re-measuring check - so this file
// pins both to LeetCode's published examples plus the trivial empty-tree and
// single-node cases, and pins them to each other. BinaryTreeNode<int> is internal,
// so the trees stay out of a public TheoryData/[Theory] signature and the agreement
// check walks a private array instead.
public sealed partial class BalancedBinaryTreeTests
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

    [Fact]
    public void IsBalancedByTopDownHeightCheck_BalancedTree_ReturnsTrue() =>
        Assert.True(BalancedBinaryTreeSolution.IsBalancedByTopDownHeightCheck(
            new BinaryTreeNode<int>(3) { Left = new(9), Right = new(20) { Left = new(15), Right = new(7) } }));

    [Fact]
    public void IsBalancedByTopDownHeightCheck_UnbalancedTree_ReturnsFalse() =>
        Assert.False(BalancedBinaryTreeSolution.IsBalancedByTopDownHeightCheck(
            new BinaryTreeNode<int>(1) { Left = new(2) { Left = new(3) { Left = new(4) } } }));

    [Fact]
    public void IsBalancedByTopDownHeightCheck_EmptyTree_ReturnsTrue() =>
        Assert.True(BalancedBinaryTreeSolution.IsBalancedByTopDownHeightCheck(null));

    [Fact]
    public void IsBalancedByTopDownHeightCheck_SingleNode_ReturnsTrue() =>
        Assert.True(BalancedBinaryTreeSolution.IsBalancedByTopDownHeightCheck(new BinaryTreeNode<int>(1)));

    // The two arms are competing strategies for one question, so the property worth
    // pinning is that they reach the same verdict on every shape - not merely that each
    // agrees with the expectation beside it.
    [Fact]
    public void IsBalanced_AgreeOnEveryExample()
    {
        BinaryTreeNode<int>?[] trees =
        [
            new(3) { Left = new(9), Right = new(20) { Left = new(15), Right = new(7) } },
            new(1) { Left = new(2) { Left = new(3) { Left = new(4) } } },
            null,
            new(1),
        ];

        foreach (var tree in trees)
        {
            Assert.Equal(
                BalancedBinaryTreeSolution.IsBalancedByHeightRecursion(tree),
                BalancedBinaryTreeSolution.IsBalancedByTopDownHeightCheck(tree));
        }
    }
}
