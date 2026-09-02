using DSAExperimentation.DataStructures.Graph.Engines.Dags.Trees;
using DSAExperimentation.LeetCode.MinimumDepthOfBinaryTree;

namespace DSAExperimentation.Tests.LeetCodeCoverage.MinimumDepthOfBinaryTree;

// Harness only. The single recursive strategy lives in
// MinimumDepthOfBinaryTreeSolution and is asserted against LeetCode's published
// examples, the original test's right-skewed case, and the empty-tree edge case
// none of the pre-migration files exercised. BinaryTreeNode<int> is internal, so -
// as in SameTreeTests - it stays out of a public TheoryData/[Theory] signature and
// is only ever handed to the solution through private helpers.
public sealed class MinimumDepthOfBinaryTreeTests
{
    [Fact]
    public void MinDepthByRecursion_EmptyTree_ReturnsZero() =>
        Assert.Equal(0, MinimumDepthOfBinaryTreeSolution.MinDepthByRecursion(null));

    [Fact]
    public void MinDepthByRecursion_RightSkewedExample_ReturnsShortestLeafDepth() =>
        Assert.Equal(2, MinimumDepthOfBinaryTreeSolution.MinDepthByRecursion(RightSkewedExample()));

    [Fact]
    public void MinDepthByRecursion_BalancedExample_ReturnsShortestLeafDepth() =>
        Assert.Equal(2, MinimumDepthOfBinaryTreeSolution.MinDepthByRecursion(BalancedExample()));

    [Fact]
    public void MinDepthByRecursion_RightLeaningChainExample_ReturnsChainLength() =>
        Assert.Equal(5, MinimumDepthOfBinaryTreeSolution.MinDepthByRecursion(RightLeaningChainExample()));

    // LeetCode 111's own example: root = [1,2,3,null,null,4] -> 2.
    private static BinaryTreeNode<int> RightSkewedExample() =>
        new(1) { Left = new(2), Right = new(3) { Right = new(4) } };

    // LeetCode 111's own example: root = [3,9,20,null,null,15,7] -> 2.
    private static BinaryTreeNode<int> BalancedExample() =>
        new(3) { Left = new(9), Right = new(20) { Left = new(15), Right = new(7) } };

    // LeetCode 111's own example: root = [2,null,3,null,4,null,5,null,6] -> 5.
    // A leaf is only reachable by descending every right child, so the minimum
    // depth equals the maximum depth here.
    private static BinaryTreeNode<int> RightLeaningChainExample() =>
        new(2) { Right = new(3) { Right = new(4) { Right = new(5) { Right = new(6) } } } };
}
