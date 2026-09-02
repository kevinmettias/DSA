using DSAExperimentation.DataStructures.Graph.Engines.Dags.Trees;
using DSAExperimentation.LeetCode.ConvertSortedArrayToBinarySearchTree;

namespace DSAExperimentation.Tests.LeetCodeCoverage.ConvertSortedArrayToBinarySearchTree;

// Harness only: the recursive midpoint build lives in
// ConvertSortedArrayToBinarySearchTreeSolution. LeetCode accepts any
// height-balanced BST whose in-order walk reproduces the sorted input, so each
// example is checked against those two properties rather than one specific
// tree shape.
public sealed class ConvertSortedArrayToBinarySearchTreeTests
{
    public static TheoryData<int[]> Examples =>
        new()
        {
            { [-10, -3, 0, 5, 9] },
            { [1, 3] },
            { [5] },
            { [] },
        };

    [Theory]
    [MemberData(nameof(Examples))]
    public void BuildByMidpointRecursion_LeetCodeExamples_ProducesHeightBalancedBstInSortedOrder(int[] nums)
    {
        var root = ConvertSortedArrayToBinarySearchTreeSolution.BuildByMidpointRecursion(nums);

        Assert.Equal(nums, InOrder(root));
        Assert.True(IsHeightBalanced(root, out _));
    }

    private static int[] InOrder(BinaryTreeNode<int>? node) =>
        node is null ? [] : [.. InOrder(node.Left), node.Value, .. InOrder(node.Right)];

    private static bool IsHeightBalanced(BinaryTreeNode<int>? node, out int height)
    {
        if (node is null)
        {
            height = 0;
            return true;
        }

        if (!IsHeightBalanced(node.Left, out var leftHeight) ||
            !IsHeightBalanced(node.Right, out var rightHeight))
        {
            height = 0;
            return false;
        }

        height = 1 + Math.Max(leftHeight, rightHeight);
        return Math.Abs(leftHeight - rightHeight) <= 1;
    }
}
