using DSAExperimentation.DataStructures.Graph.Engines.Dags.Trees;
using DSAExperimentation.LeetCode.Harness;
using DSAExperimentation.LeetCode.MaximumSumBSTInBinaryTree;

namespace DSAExperimentation.Tests.LeetCodeCoverage.MaximumSumBSTInBinaryTree;

// Harness only. Both the revalidate-per-node baseline and the single bottom-up
// scan are MaximumSumBSTInBinaryTreeSolution's - this file pins them to LeetCode's
// published examples, given in LeetCode's own level-order-with-null array shape
// (BinaryTreeNode<int> is internal, so it cannot appear in a public TheoryData
// signature; LeetCodeWireFormat.ToBinaryTree reconstructs it).
public sealed class MaximumSumBSTInBinaryTreeTests
{
    public static TheoryData<int?[], int> Examples =>
        new()
        {
            // LC 1373's own three examples: a tree whose best BST is a mid-tree
            // subtree, one whose best is a single leaf, and the all-negative tree
            // that reports the empty BST's zero.
            { [1, 4, 3, 2, 4, 2, 5, null, null, null, null, null, null, 4, 6], 20 },
            { [4, 3, null, 1, 2], 2 },
            { [-4, -2, -5], 0 },

            // The root already breaks BST order (8 > 5), so the whole tree is
            // invalid - but the subtree rooted at 8 is valid and beats every
            // smaller valid subtree's sum (10, 3, -2).
            { [5, 8, -2, 3, 10], 21 },

            // The entire tree is already a valid BST, so the answer is its total.
            { [2, 1, 3], 6 },

            // A single node is a trivially valid BST.
            { [7], 7 },
        };

    [Theory]
    [MemberData(nameof(Examples))]
    public void MaxSumBSTByRevalidatingEachNode_LeetCodeExamples_ReturnsBestValidSubtreeSum(
        int?[] values, int expected) =>
        Assert.Equal(expected, MaximumSumBSTInBinaryTreeSolution.MaxSumBSTByRevalidatingEachNode(LeetCodeWireFormat.ToBinaryTree(values)));

    [Theory]
    [MemberData(nameof(Examples))]
    public void MaxSumBSTByBottomUpScan_LeetCodeExamples_ReturnsBestValidSubtreeSum(
        int?[] values, int expected) =>
        Assert.Equal(expected, MaximumSumBSTInBinaryTreeSolution.MaxSumBSTByBottomUpScan(LeetCodeWireFormat.ToBinaryTree(values)));
}
