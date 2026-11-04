using DSAExperimentation.DataStructures.Graph.Engines.Dags.Trees;

namespace DSAExperimentation.LeetCode.Solutions.ClosestNodesQueriesInABinarySearchTree;

// LC 2476's promise about its input: the tree is a binary search tree, so walked in order it lists
// its values in order. LeetCode's judge never poses an input that breaks it. The in-order binary
// search leans on that order, while the linear scan answers from whatever it finds. Generated cases
// do not always keep the promise, so a harness that runs them asks this first.
internal static class ClosestNodesQueriesInABinarySearchTreePromise
{
    public static bool IsKeptBy(BinaryTreeNode<int>? root, int[] queries) => IsInOrder(root);

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
