using DSAExperimentation.Algorithms.Traversal.BreadthFirst;
using DSAExperimentation.DataStructures.Graph.Contracts.Ordering;
using DSAExperimentation.DataStructures.Graph.Engines.Dags.Trees;
using DSAExperimentation.DataStructures.HashMap;

namespace DSAExperimentation.LeetCode.PopulatingNextRightPointersInEachNodeII;

// LeetCode 117. Populating Next Right Pointers in Each Node II: connect every node
// to the node immediately to its right at the same depth (null when none), for an
// arbitrary binary tree - not necessarily perfect, unlike #116.
//
// This repo's BinaryTreeNode<TValue> carries no Next slot of its own, so each
// strategy links the tree into a node -> next-node map, then reads the connected
// tree back the way LeetCode's judge does: level by level along the next pointers,
// '#' closing each level - [1,#,2,3,#,4,5,7,#] - with null standing for '#'. That
// readout is the answer every strategy returns, so the strategies differ only in
// how they link, not in what they report.
internal static class PopulatingNextRightPointersInEachNodeIISolution
{
    // The textbook answer: a manual queue-driven BFS, one level (queue snapshot
    // length) at a time, written without this repo's traversal engine.
    public static int?[] ConnectByManualQueueBfs(BinaryTreeNode<int>? root)
    {
        var next = LinkByManualQueueBfs(root);

        return ReadLevelsAlongNextPointers(root, new DictionaryNextPointers(next));
    }

    private static Dictionary<BinaryTreeNode<int>, BinaryTreeNode<int>?> LinkByManualQueueBfs(
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
            EnqueueChildren(queue, node);
        }

        next[previous!] = null;
    }

    // The next level's entries this node contributes, left before right.
    private static void EnqueueChildren(Queue<BinaryTreeNode<int>> queue, BinaryTreeNode<int> node)
    {
        if (node.Left is not null)
        {
            queue.Enqueue(node.Left);
        }

        if (node.Right is not null)
        {
            queue.Enqueue(node.Right);
        }
    }

    // LeetCode's readout of a connected tree: from each level's leftmost node, follow
    // the next pointers to the level's end, then close the level with null ('#'). The
    // next level starts at the first child met along the way, which a missing child
    // just pushes further right. The lookup is a struct type argument, so each
    // strategy's map is read through a direct call.
    internal static int?[] ReadLevelsAlongNextPointers<TNext>(BinaryTreeNode<int>? root, TNext next)
        where TNext : struct, INextPointerLookup
    {
        var readout = new List<int?>();
        var levelStart = root;

        while (levelStart is not null)
        {
            levelStart = ReadLevel(levelStart, next, readout);
        }

        return [.. readout];
    }

    // Reads one level into the readout and reports where the next level starts.
    private static BinaryTreeNode<int>? ReadLevel<TNext>(BinaryTreeNode<int> levelStart, TNext next, List<int?> readout)
        where TNext : struct, INextPointerLookup
    {
        BinaryTreeNode<int>? nextLevelStart = null;

        for (BinaryTreeNode<int>? node = levelStart; node is not null; node = next.NextOf(node))
        {
            readout.Add(node.Value);
            nextLevelStart ??= node.Left ?? node.Right;
        }

        readout.Add(null);

        return nextLevelStart;
    }

    // This repo's own LevelGroupedBreadthFirstTraversal already groups each depth
    // into its own left-to-right buffer - BinaryTreeChildren compacts away null
    // Left/Right slots before a level is grouped, so a missing sibling just means a
    // shorter buffer, with no separate code path needed for the general-tree case.
    // Linking a level is then just pairing each entry with the one after it.
    public static int?[] ConnectByLevelGroupedTraversal(BinaryTreeNode<int>? root)
    {
        var next = LinkByLevelGroupedTraversal(root);

        return ReadLevelsAlongNextPointers(root, new HashMapNextPointers(next));
    }

    private static HashMap<BinaryTreeNode<int>, BinaryTreeNode<int>?> LinkByLevelGroupedTraversal(
        BinaryTreeNode<int>? root)
    {
        var levels = new List<List<BinaryTreeNode<int>>>();

        LevelGroupedBreadthFirstTraversal.Walk<
            BinaryTreeNode<int>, BinaryTreeTopology<int>, BinaryTreeChildren<int>,
            LevelHooks>(root, new LevelHooks(levels));

        var next = new HashMap<BinaryTreeNode<int>, BinaryTreeNode<int>?>();

        foreach (var level in levels)
        {
            for (var i = 0; i < level.Count; i++)
            {
                var hasNextInLevel = i + 1 < level.Count;
                next.Set(level[i], hasNextInLevel ? NextInLevel(level, i) : null);
            }
        }

        return next;
    }

    // The level's next node, which the last node of a level has not got - hence the
    // guard at the call site.
    private static BinaryTreeNode<int>? NextInLevel(List<BinaryTreeNode<int>> level, int index) =>
        level[index + 1];

    private readonly struct LevelHooks(List<List<BinaryTreeNode<int>>> levels) : ILevelGroupedHooks<BinaryTreeNode<int>>
    {
        public void OnLevel(IReadOnlyList<BinaryTreeNode<int>> level, int depth) => levels.Add(level.ToList());
    }

    // A node's next pointer as one strategy's link map records it.
    internal interface INextPointerLookup
    {
        BinaryTreeNode<int>? NextOf(BinaryTreeNode<int> node);
    }

    // The manual-queue strategies' BCL map; every node of the tree has an entry.
    internal readonly struct DictionaryNextPointers(Dictionary<BinaryTreeNode<int>, BinaryTreeNode<int>?> next)
        : INextPointerLookup
    {
        public BinaryTreeNode<int>? NextOf(BinaryTreeNode<int> node) => next[node];
    }

    // The level-grouped strategies' map, on this repo's own HashMap.
    internal readonly struct HashMapNextPointers(HashMap<BinaryTreeNode<int>, BinaryTreeNode<int>?> next)
        : INextPointerLookup
    {
        public BinaryTreeNode<int>? NextOf(BinaryTreeNode<int> node) =>
            next.TryGetValue(node, out var nextNode) ? nextNode : null;
    }
}
