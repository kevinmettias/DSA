using DSAExperimentation.DataStructures.Graph.Engines.Dags.Trees;

namespace DSAExperimentation.LeetCode.Solutions.RangeSumOfBST;

// LC 938's promise about its input: the tree is a binary search tree of unique values. LeetCode's
// judge never poses an input that breaks it. The pruning search skips subtrees that a tree out of
// order still holds values of the range in. Generated cases do not always keep the promise, so a
// harness that runs them asks this first.
internal static class RangeSumOfBSTPromise
{
    public static bool IsKeptBy(BinaryTreeNode<int>? node, int low, int high) => IsInStrictOrder(node);

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
