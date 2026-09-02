using DSAExperimentation.DataStructures.Graph.Engines.Dags.Trees;
using DSAExperimentation.LeetCode.InvertBinaryTree;

namespace DSAExperimentation.Tests.LeetCodeCoverage.InvertBinaryTree;

// Harness only. The single strategy is InvertBinaryTreeSolution's; this file pins
// it to LeetCode's published examples plus the null/leaf edges the original test
// never exercised. BinaryTreeNode<int> is internal, so - as in SameTreeTests - it
// stays out of a public TheoryData/[Theory] signature and trees are built by a
// private factory instead.
public sealed class InvertBinaryTreeTests
{
    [Fact]
    public void InvertByRecursiveSwap_LeetCodeExampleOne_MirrorsEveryLevel()
    {
        // [4,2,7,1,3,6,9] -> [4,7,2,9,6,3,1]
        var root = Tree(4, Tree(2, Tree(1), Tree(3)), Tree(7, Tree(6), Tree(9)));

        var inverted = InvertBinaryTreeSolution.InvertByRecursiveSwap(root);

        Assert.Equal(4, inverted!.Value);
        Assert.Equal(7, inverted.Left!.Value);
        Assert.Equal(9, inverted.Left.Left!.Value);
        Assert.Equal(6, inverted.Left.Right!.Value);
        Assert.Equal(2, inverted.Right!.Value);
        Assert.Equal(3, inverted.Right.Left!.Value);
        Assert.Equal(1, inverted.Right.Right!.Value);
    }

    [Fact]
    public void InvertByRecursiveSwap_LeetCodeExampleTwo_SwapsTheOnlyPairOfChildren()
    {
        // [2,1,3] -> [2,3,1]
        var root = Tree(2, Tree(1), Tree(3));

        var inverted = InvertBinaryTreeSolution.InvertByRecursiveSwap(root);

        Assert.Equal(2, inverted!.Value);
        Assert.Equal(3, inverted.Left!.Value);
        Assert.Equal(1, inverted.Right!.Value);
    }

    [Fact]
    public void InvertByRecursiveSwap_EmptyTree_ReturnsNull() =>
        Assert.Null(InvertBinaryTreeSolution.InvertByRecursiveSwap(null));

    [Fact]
    public void InvertByRecursiveSwap_SingleNode_ReturnsItUnchanged()
    {
        var root = Tree(1);

        var inverted = InvertBinaryTreeSolution.InvertByRecursiveSwap(root);

        Assert.Equal(1, inverted!.Value);
        Assert.Null(inverted.Left);
        Assert.Null(inverted.Right);
    }

    [Fact]
    public void InvertByRecursiveSwap_LeftOnlyChain_BecomesARightOnlyChain()
    {
        var root = Tree(1, Tree(2, Tree(3)));

        var inverted = InvertBinaryTreeSolution.InvertByRecursiveSwap(root);

        Assert.Null(inverted!.Left);
        Assert.Equal(2, inverted.Right!.Value);
        Assert.Null(inverted.Right.Left);
        Assert.Equal(3, inverted.Right.Right!.Value);
    }

    private static BinaryTreeNode<int> Tree(int value, BinaryTreeNode<int>? left = null, BinaryTreeNode<int>? right = null) =>
        new(value) { Left = left, Right = right };
}
