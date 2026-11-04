using DSAExperimentation.DataStructures.Graph.Engines.Dags.Trees;

namespace DSAExperimentation.LeetCode.Solutions.AllElementsInTwoBinarySearchTrees;

// LC 1305's promise about its input: both trees are binary search trees, so each lists its values in
// order when walked in order. LeetCode's judge never poses an input that breaks it, and the in-order
// merge leans on it: given a tree out of order it merges an unsorted list, while collecting and sorting
// answers sorted regardless, so the two strategies part. Generated cases do not always keep the promise,
// so a harness that runs them asks this first.
internal static class AllElementsInTwoBinarySearchTreesPromise
{
    public static bool IsKeptBy(BinaryTreeNode<int>? root1, BinaryTreeNode<int>? root2)
        => IsInOrder(root1) && IsInOrder(root2);

    // Walked in order, no value is smaller than the one before it.
    private static bool IsInOrder(BinaryTreeNode<int>? root)
    {
        var pending = new Stack<BinaryTreeNode<int>>();
        PushLeftSpine(root, pending);
        int? previous = null;

        while (pending.Count > 0)
        {
            var node = pending.Pop();

            if (previous > node.Value)
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
