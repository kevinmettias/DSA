using DSAExperimentation.DataStructures.Graph.Engines.Dags.Trees;
using DSAExperimentation.LeetCode.BalanceABinarySearchTree;
using DSAExperimentation.LeetCode.Harness;

namespace DSAExperimentation.Tests.LeetCodeCoverage.BalanceABinarySearchTree;

// Harness only: both strategies live in BalanceABinarySearchTreeSolution. Examples
// are stated as LeetCode's own level-order arrays - BinaryTreeNode<int> is
// internal, so it cannot appear in a public TheoryData member; LeetCodeWireFormat.ToBinaryTree
// reconstructs it inside each test method instead, the same shape
// DiameterOfBinaryTreeTests uses. LeetCode accepts any height-balanced BST whose
// in-order walk reproduces the input's values, so each example is checked against
// those two properties rather than one specific tree shape. The pre-migration test
// only exercised the InOrderTraversal composition; BalanceByRepeatedKthSmallest
// (previously untested scaffolding inlined in BalanceABinarySearchTreeBenchmarks as
// its [Benchmark(Baseline = true)] arm) gets the identical assertions here for the
// first time.
public sealed partial class BalanceABinarySearchTreeTests
{
    public static TheoryData<int?[], int[]> Examples =>
        new()
        {
            // LeetCode's published example 1: a right-only chain, height 4.
            { [1, null, 2, null, 3, null, 4], [1, 2, 3, 4] },

            // LeetCode's published example 2: already balanced.
            { [2, 1, 3], [1, 2, 3] },

            // A single node is its own balanced tree.
            { [9], [9] },

            // A left-only chain - the mirror image of example 1.
            { [4, 3, null, 2, null, 1], [1, 2, 3, 4] },

            // A larger, lopsided-but-ordered tree: the left subtree is a chain, the
            // right subtree is already balanced.
            { [5, 3, 8, 2, null, 7, 9, 1], [1, 2, 3, 5, 7, 8, 9] },
        };

    [Theory]
    [MemberData(nameof(Examples))]
    public void BalanceByInOrderTraversal_LeetCodeExamples_ProducesHeightBalancedBstWithSameValues(
        int?[] levelOrder, int[] expectedSorted)
    {
        var balanced = BalanceABinarySearchTreeSolution.BalanceByInOrderTraversal(LeetCodeWireFormat.ToBinaryTree(levelOrder)!);

        Assert.Equal(expectedSorted, InOrder(balanced));
        Assert.True(IsHeightBalanced(balanced).IsBalanced);
    }

    [Theory]
    [MemberData(nameof(Examples))]
    public void BalanceByRepeatedKthSmallest_LeetCodeExamples_ProducesHeightBalancedBstWithSameValues(
        int?[] levelOrder, int[] expectedSorted)
    {
        var balanced = BalanceABinarySearchTreeSolution.BalanceByRepeatedKthSmallest(LeetCodeWireFormat.ToBinaryTree(levelOrder)!);

        Assert.Equal(expectedSorted, InOrder(balanced));
        Assert.True(IsHeightBalanced(balanced).IsBalanced);
    }

    private static int[] InOrder(BinaryTreeNode<int>? node)
    {
        if (node is null)
        {
            return [];
        }

        return [.. InOrder(node.Left), node.Value, .. InOrder(node.Right)];
    }

    private static (bool IsBalanced, int Height) IsHeightBalanced(BinaryTreeNode<int>? node)
    {
        if (node is null)
        {
            return (true, 0);
        }

        var (leftBalanced, leftHeight) = IsHeightBalanced(node.Left);

        if (!leftBalanced)
        {
            return (false, 0);
        }

        var (rightBalanced, rightHeight) = IsHeightBalanced(node.Right);

        if (!rightBalanced)
        {
            return (false, 0);
        }

        return (Math.Abs(leftHeight - rightHeight) <= 1, 1 + Math.Max(leftHeight, rightHeight));
    }
}
