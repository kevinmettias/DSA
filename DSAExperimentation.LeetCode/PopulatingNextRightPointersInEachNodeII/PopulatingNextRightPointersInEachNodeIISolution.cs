using DSAExperimentation.Algorithms.Traversal.BreadthFirst;
using DSAExperimentation.DataStructures.Graph.Contracts.Ordering;
using DSAExperimentation.DataStructures.Graph.Engines.Dags.Trees;
using DSAExperimentation.DataStructures.HashMap;

namespace DSAExperimentation.LeetCode.PopulatingNextRightPointersInEachNodeII;

// LeetCode 117. Populating Next Right Pointers in Each Node II: connect every node
// to the node immediately to its right at the same depth (null when none), for an
// arbitrary binary tree - not necessarily perfect, unlike #116.
//
// This repo's BinaryTreeNode<TValue> carries no Next slot of its own, so both
// strategies report the answer as a node -> next-node map rather than mutating the
// tree in place. That is the only place the two strategies differ in shape; both
// walk the tree one level at a time and link each node to the one after it in that
// level's left-to-right order.
internal static class PopulatingNextRightPointersInEachNodeIISolution
{
    // The textbook answer: a manual queue-driven BFS, one level (queue snapshot
    // length) at a time, written without this repo's traversal engine.
    public static Dictionary<BinaryTreeNode<int>, BinaryTreeNode<int>?> ConnectByManualQueueBfs(
        BinaryTreeNode<int>? root)
    {
        var next = new Dictionary<BinaryTreeNode<int>, BinaryTreeNode<int>?>();

        if (root is null)
        {
            return next;
        }

        var queue = new Queue<BinaryTreeNode<int>>();
        queue.Enqueue(root);

        while (queue.Count > 0)
        {
            LinkLevel(queue, next);
        }

        return next;
    }

    private static void LinkLevel(
        Queue<BinaryTreeNode<int>> queue, Dictionary<BinaryTreeNode<int>, BinaryTreeNode<int>?> next)
    {
        var levelSize = queue.Count;
        BinaryTreeNode<int>? previous = null;

        for (var i = 0; i < levelSize; i++)
        {
            var node = queue.Dequeue();

            if (previous is not null)
            {
                next[previous] = node;
            }

            previous = node;

            if (node.Left is not null)
            {
                queue.Enqueue(node.Left);
            }

            if (node.Right is not null)
            {
                queue.Enqueue(node.Right);
            }
        }

        next[previous!] = null;
    }

    // This repo's own LevelGroupedBreadthFirstTraversal already groups each depth
    // into its own left-to-right buffer - BinaryTreeChildren compacts away null
    // Left/Right slots before a level is grouped, so a missing sibling just means a
    // shorter buffer, with no separate code path needed for the general-tree case.
    // Linking a level is then just pairing each entry with the one after it.
    public static HashMap<BinaryTreeNode<int>, BinaryTreeNode<int>?> ConnectByLevelGroupedTraversal(
        BinaryTreeNode<int>? root)
    {
        LevelHooks.Output.Value = [];

        LevelGroupedBreadthFirstTraversal.Walk<
            BinaryTreeNode<int>, BinaryTreeTopology<int>, BinaryTreeChildren<int>,
            NaturalChildOrder<BinaryTreeNode<int>, BinaryTreeChildren<int>>, BinaryTreeChildren<int>, LevelHooks>(root);

        var next = new HashMap<BinaryTreeNode<int>, BinaryTreeNode<int>?>();

        foreach (var level in LevelHooks.Output.Value!)
        {
            for (var i = 0; i < level.Count; i++)
            {
                next.Set(level[i], i + 1 < level.Count ? level[i + 1] : null);
            }
        }

        return next;
    }

    private readonly struct LevelHooks : ILevelGroupedHooks<BinaryTreeNode<int>>
    {
        public static readonly AsyncLocal<List<List<BinaryTreeNode<int>>>> Output = new();

        public static void OnLevel(IReadOnlyList<BinaryTreeNode<int>> level, int depth) => Output.Value!.Add(level.ToList());
    }
}
