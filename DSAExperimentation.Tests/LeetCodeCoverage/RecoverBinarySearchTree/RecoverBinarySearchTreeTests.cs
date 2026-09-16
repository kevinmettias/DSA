using DSAExperimentation.DataStructures.Graph.Engines.Dags.Trees;
using DSAExperimentation.LeetCode.RecoverBinarySearchTree;

namespace DSAExperimentation.Tests.LeetCodeCoverage.RecoverBinarySearchTree;

// Harness only. Both strategies are RecoverBinarySearchTreeSolution's; this file
// pins them to LeetCode's published examples. BinaryTreeNode<int> is internal, so -
// as in ValidateBinarySearchTreeTests - it stays out of a public TheoryData/[Theory]
// signature; each example is a private factory rebuilt fresh per [Fact], since
// recovery mutates its tree in place.
public sealed partial class RecoverBinarySearchTreeTests
{
    [Fact]
    public void RecoverByManualRecursiveScan_AdjacentSwapAtTheRoot_RestoresBstOrdering()
    {
        var root = BuildAdjacentSwapAtTheRootTree();

        RecoverBinarySearchTreeSolution.RecoverByManualRecursiveScan(root);

        AssertAdjacentSwapAtTheRootRestored(root);
    }

    [Fact]
    public void RecoverByInOrderHooks_AdjacentSwapAtTheRoot_RestoresBstOrdering()
    {
        var root = BuildAdjacentSwapAtTheRootTree();

        RecoverBinarySearchTreeSolution.RecoverByInOrderHooks(root);

        AssertAdjacentSwapAtTheRootRestored(root);
    }

    [Fact]
    public void RecoverByManualRecursiveScan_NonAdjacentSwapAcrossTheTree_RestoresBstOrdering()
    {
        var root = BuildNonAdjacentSwapTree();

        RecoverBinarySearchTreeSolution.RecoverByManualRecursiveScan(root);

        AssertNonAdjacentSwapRestored(root);
    }

    [Fact]
    public void RecoverByInOrderHooks_NonAdjacentSwapAcrossTheTree_RestoresBstOrdering()
    {
        var root = BuildNonAdjacentSwapTree();

        RecoverBinarySearchTreeSolution.RecoverByInOrderHooks(root);

        AssertNonAdjacentSwapRestored(root);
    }

    // Every tree this file reads back is one it built itself, with the child already
    // in place: each Build* helper below names Left/Right on the node it attaches to.
    // Both recovery strategies then rewrite Values in place - neither detaches,
    // relinks or rebuilds a child - so each child an assertion walks is the one its
    // initializer set.
    private static BinaryTreeNode<int> Child(BinaryTreeNode<int>? child) =>
        child ?? throw new InvalidOperationException(
            "every tree here is built by hand with that child set, and recovery swaps Values without relinking");

    // [1,3,null,null,2] -> expected [3,1,null,null,2]: the two nodes to swap are the root
    // and its left child, so both Values have to move and the child keeps its own right
    // subtree.
    private static BinaryTreeNode<int> BuildAdjacentSwapAtTheRootTree() =>
        new BinaryTreeNode<int>(1) { Left = new(3) { Right = new(2) } };

    private static void AssertAdjacentSwapAtTheRootRestored(BinaryTreeNode<int> root)
    {
        var left = Child(root.Left);

        Assert.Equal(3, root.Value);
        Assert.Equal(1, left.Value);
        Assert.Equal(2, Child(left.Right).Value);
    }

    // [3,1,4,null,null,2] -> expected [2,1,4,null,null,3]: the two nodes to swap are
    // not adjacent in the traversal, so the second mismatch must not be read as a
    // repeat of the first.
    private static BinaryTreeNode<int> BuildNonAdjacentSwapTree() =>
        new BinaryTreeNode<int>(3) { Left = new(1), Right = new(4) { Left = new(2) } };

    private static void AssertNonAdjacentSwapRestored(BinaryTreeNode<int> root)
    {
        var right = Child(root.Right);

        Assert.Equal(2, root.Value);
        Assert.Equal(1, Child(root.Left).Value);
        Assert.Equal(4, right.Value);
        Assert.Equal(3, Child(right.Left).Value);
    }
}
