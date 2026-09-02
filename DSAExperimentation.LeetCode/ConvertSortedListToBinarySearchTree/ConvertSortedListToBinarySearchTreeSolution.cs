using DSAExperimentation.DataStructures.Graph.Engines.Dags.Trees;
using DSAExperimentation.DataStructures.SinglyLinkedList;

namespace DSAExperimentation.LeetCode.ConvertSortedListToBinarySearchTree;

// LeetCode 109. Convert Sorted List to Binary Search Tree: build a
// height-balanced BST from a singly linked list already sorted in ascending
// order.
//
// There is only one strategy here: the original test's private helper and
// the (unimplemented, placeholder) original benchmark agreed on nothing, so
// this is the real algorithm - materialize the list once into an array, then
// pick the midpoint of each remaining [low, high] range as the subtree root,
// exactly the recurrence ConvertSortedArrayToBinarySearchTreeSolution uses
// for LC 108 once the list has been flattened.
internal static class ConvertSortedListToBinarySearchTreeSolution
{
    private const int MidpointDivisor = 2;

    public static BinaryTreeNode<int>? BuildByMidpointRecursion(SinglyLinkedListNode<int>? head)
    {
        var values = new List<int>();

        for (var node = head; node is not null; node = node.Next)
        {
            values.Add(node.Value);
        }

        return BuildByMidpointRecursion(values, 0, values.Count - 1);
    }

    private static BinaryTreeNode<int>? BuildByMidpointRecursion(List<int> values, int low, int high)
    {
        if (low > high)
        {
            return null;
        }

        var mid = low + ((high - low) / MidpointDivisor);

        return new BinaryTreeNode<int>(values[mid])
        {
            Left = BuildByMidpointRecursion(values, low, mid - 1),
            Right = BuildByMidpointRecursion(values, mid + 1, high),
        };
    }
}
