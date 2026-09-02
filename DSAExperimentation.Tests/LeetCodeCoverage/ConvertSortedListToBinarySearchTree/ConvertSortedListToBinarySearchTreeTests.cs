using DSAExperimentation.DataStructures.Graph.Engines.Dags.Trees;
using DSAExperimentation.DataStructures.SinglyLinkedList;
using DSAExperimentation.LeetCode.ConvertSortedListToBinarySearchTree;

namespace DSAExperimentation.Tests.LeetCodeCoverage.ConvertSortedListToBinarySearchTree;

// Harness only: the midpoint-recursion build lives in
// ConvertSortedListToBinarySearchTreeSolution. LeetCode accepts any
// height-balanced BST whose in-order walk reproduces the sorted input, so
// each example is checked against those two properties rather than one
// specific tree shape - the same convention
// ConvertSortedArrayToBinarySearchTreeTests uses for LC 108.
public sealed class ConvertSortedListToBinarySearchTreeTests
{
    public static TheoryData<int[]> Examples =>
        new()
        {
            { [-10, -3, 0, 5, 9] },
            { [1, 3] },
            { [0] },
            { [] },
        };

    [Theory]
    [MemberData(nameof(Examples))]
    public void BuildByMidpointRecursion_LeetCodeExamples_ProducesHeightBalancedBstInSortedOrder(int[] sortedValues)
    {
        var head = BuildList(sortedValues);

        var root = ConvertSortedListToBinarySearchTreeSolution.BuildByMidpointRecursion(head);

        Assert.Equal(sortedValues, InOrder(root));
        Assert.True(IsHeightBalanced(root, out _));
    }

    private static SinglyLinkedListNode<int>? BuildList(int[] values)
    {
        SinglyLinkedListNode<int>? head = null;
        SinglyLinkedListNode<int>? tail = null;

        foreach (var value in values)
        {
            var node = new SinglyLinkedListNode<int>(value);

            if (tail is null)
            {
                head = node;
            }
            else
            {
                tail.Next = node;
            }

            tail = node;
        }

        return head;
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
