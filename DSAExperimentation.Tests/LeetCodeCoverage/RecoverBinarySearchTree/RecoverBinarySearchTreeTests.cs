using DSAExperimentation.DataStructures.Graph.Engines.Dags.Trees;
using DSAExperimentation.LeetCode.RecoverBinarySearchTree;

namespace DSAExperimentation.Tests.LeetCodeCoverage.RecoverBinarySearchTree;

// Harness only. Both strategies are RecoverBinarySearchTreeSolution's; this file
// pins them to LeetCode's published examples. BinaryTreeNode<int> is internal, so -
// as in ValidateBinarySearchTreeTests - it stays out of a public TheoryData/[Theory]
// signature; each example is a private factory rebuilt fresh per [Fact], since
// recovery mutates its tree in place.
public sealed class RecoverBinarySearchTreeTests
{
    [Fact]
    public void RecoverByManualRecursiveScan_AdjacentSwapAtTheRoot_RestoresBstOrdering()
    {
        // [1,3,null,null,2] -> expected [3,1,null,null,2]
        var root = new BinaryTreeNode<int>(1) { Left = new(3) { Right = new(2) } };

        RecoverBinarySearchTreeSolution.RecoverByManualRecursiveScan(root);

        Assert.Equal(3, root.Value);
        Assert.Equal(1, root.Left!.Value);
        Assert.Equal(2, root.Left.Right!.Value);
    }

    [Fact]
    public void RecoverByInOrderHooks_AdjacentSwapAtTheRoot_RestoresBstOrdering()
    {
        var root = new BinaryTreeNode<int>(1) { Left = new(3) { Right = new(2) } };

        RecoverBinarySearchTreeSolution.RecoverByInOrderHooks(root);

        Assert.Equal(3, root.Value);
        Assert.Equal(1, root.Left!.Value);
        Assert.Equal(2, root.Left.Right!.Value);
    }

    [Fact]
    public void RecoverByManualRecursiveScan_NonAdjacentSwapAcrossTheTree_RestoresBstOrdering()
    {
        // [3,1,4,null,null,2] -> expected [2,1,4,null,null,3]
        var root = new BinaryTreeNode<int>(3) { Left = new(1), Right = new(4) { Left = new(2) } };

        RecoverBinarySearchTreeSolution.RecoverByManualRecursiveScan(root);

        Assert.Equal(2, root.Value);
        Assert.Equal(1, root.Left!.Value);
        Assert.Equal(4, root.Right!.Value);
        Assert.Equal(3, root.Right.Left!.Value);
    }

    [Fact]
    public void RecoverByInOrderHooks_NonAdjacentSwapAcrossTheTree_RestoresBstOrdering()
    {
        var root = new BinaryTreeNode<int>(3) { Left = new(1), Right = new(4) { Left = new(2) } };

        RecoverBinarySearchTreeSolution.RecoverByInOrderHooks(root);

        Assert.Equal(2, root.Value);
        Assert.Equal(1, root.Left!.Value);
        Assert.Equal(4, root.Right!.Value);
        Assert.Equal(3, root.Right.Left!.Value);
    }
}
