using DSAExperimentation.DataStructures.Graph.Engines.Dags.Trees;

namespace DSAExperimentation.LeetCode.Solutions.ConvertSortedArrayToBinarySearchTree;

// LC 108 accepts any height-balanced binary search tree of the array's values. The catalog records one,
// so an answer stands when walking it in order gives the array back and no node's two subtrees differ in
// height by more than one. Where a range has two middles, the dataset's reference took the right one and
// the midpoint recursion takes the left.
internal static class ConvertSortedArrayToBinarySearchTreeAnswerCheck
{
    // A height no tree has, standing for a subtree whose own subtrees part by more than one.
    private const int Unbalanced = -1;

    public static bool IsAccepted(int[] nums, BinaryTreeNode<int>? answer, BinaryTreeNode<int>? recorded)
    {
        var values = new List<int>();
        CollectInOrder(answer, values);
        return values.SequenceEqual(nums) && HeightOf(answer) != Unbalanced;
    }

    private static void CollectInOrder(BinaryTreeNode<int>? node, List<int> values)
    {
        if (node is null)
        {
            return;
        }

        CollectInOrder(node.Left, values);
        values.Add(node.Value);
        CollectInOrder(node.Right, values);
    }

    // The subtree's height, 0 for an empty one, or Unbalanced when some node in it is out of balance.
    private static int HeightOf(BinaryTreeNode<int>? node)
    {
        if (node is null)
        {
            return 0;
        }

        var (left, right) = (HeightOf(node.Left), HeightOf(node.Right));
        var isBalanced = left != Unbalanced && right != Unbalanced && Math.Abs(left - right) <= 1;
        return isBalanced ? HeightAbove(left, right) : Unbalanced;
    }

    // The height of a node whose two subtrees have these heights.
    private static int HeightAbove(int left, int right) => Math.Max(left, right) + 1;
}
