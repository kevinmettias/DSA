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
public sealed partial class ConvertSortedListToBinarySearchTreeTests
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
        Assert.True(IsHeightBalanced(root).IsBalanced);
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

    // Both arms are values: a call that names the empty walk, and a call that names the
    // non-empty one. The non-empty arm stays a call rather than a hoisted local because
    // the condition guards it - a local above the expression would run it every time.
    private static int[] InOrder(BinaryTreeNode<int>? node) =>
        node is null ? Array.Empty<int>() : InOrderNonNull(node);

    private static int[] InOrderNonNull(BinaryTreeNode<int> node) =>
        [.. InOrder(node.Left), node.Value, .. InOrder(node.Right)];

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
