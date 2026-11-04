using DSAExperimentation.DataStructures.Graph.Engines.Dags.Trees;

namespace DSAExperimentation.LeetCode.Solutions.SearchInABinarySearchTree;

// LC 700's promise about its input: the tree is a binary search tree of unique values. LeetCode's
// judge never poses an input that breaks it. The descent goes one way at each node, which a tree
// out of order may hide the value from. Generated cases do not always keep the promise, so a
// harness that runs them asks this first.
internal static class SearchInABinarySearchTreePromise
{
    public static bool IsKeptBy(BinaryTreeNode<int>? root, int val) => IsInStrictOrder(root);

    // Walked in order, each value is larger than the one before it.
    private static bool IsInStrictOrder(BinaryTreeNode<int>? root)
    {
        var pending = new Stack<BinaryTreeNode<int>>();
        PushLeftSpine(root, pending);
        int? previous = null;

        while (pending.Count > 0)
        {
            var node = pending.Pop();

            if (previous >= node.Value)
            {
                return false;
            }

            previous = node.Value;
            PushLeftSpine(node.Right, pending);
        }

        return true;
    }

    // The node and its left descendants, deepest last, so the stack hands them back smallest first.
    private static void PushLeftSpine(BinaryTreeNode<int>? node, Stack<BinaryTreeNode<int>> pending)
    {
        for (var at = node; at is not null; at = at.Left)
        {
            pending.Push(at);
        }
    }
}
