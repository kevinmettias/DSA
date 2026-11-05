using DSAExperimentation.DataStructures.Graph.Engines.Dags.Trees;
using DSAExperimentation.DataStructures.SinglyLinkedList;

namespace DSAExperimentation.LeetCode.Solutions.ConvertSortedListToBinarySearchTree;

// LC 109 accepts any height-balanced binary search tree of the list's values. The catalog records one, so
// an answer stands when its values, read in order, are the list's, and every node's subtrees stand within
// one level of each other. Where a range has two middles, the dataset's reference took the right one and
// the midpoint recursion takes the left.
internal static class ConvertSortedListToBinarySearchTreeAnswerCheck
{
    public static bool IsAccepted(SinglyLinkedListNode<int>? head, BinaryTreeNode<int>? answer, BinaryTreeNode<int>? recorded)
    {
        var (values, _, isBalanced) = Measure(answer);
        return isBalanced && values.SequenceEqual(LeetCodeWireFormat.FromLinkedList(head));
    }

    // The subtree's values in order, its height, and whether every node in it is balanced, from its two
    // subtrees' own.
    private static (List<int> Values, int Height, bool IsBalanced) Measure(BinaryTreeNode<int>? node)
    {
        if (node is null)
        {
            return ([], 0, true);
        }

        var (left, right) = (Measure(node.Left), Measure(node.Right));
        var isBalanced = left.IsBalanced && right.IsBalanced && Math.Abs(left.Height - right.Height) <= 1;
        return ([.. left.Values, node.Value, .. right.Values], Math.Max(left.Height, right.Height) + 1, isBalanced);
    }
}
