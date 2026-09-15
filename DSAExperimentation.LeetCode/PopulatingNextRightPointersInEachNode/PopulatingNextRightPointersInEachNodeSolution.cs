using DSAExperimentation.Algorithms.Traversal.BreadthFirst;
using DSAExperimentation.DataStructures.Graph.Contracts.Ordering;
using DSAExperimentation.DataStructures.Graph.Engines.Dags.Trees;
using DSAExperimentation.DataStructures.HashMap;

namespace DSAExperimentation.LeetCode.PopulatingNextRightPointersInEachNode;

// LeetCode 116. Populating Next Right Pointers in Each Node: given a perfect binary
// tree, connect each node to its next right neighbor at the same depth (null for
// the rightmost node of each level). BinaryTreeNode<int> has no Next field of its
// own, so "populate" is represented as a node -> next-node map built from a
// level-by-level BFS - the two strategies differ only in how a level's boundary is
// discovered, the same split BinaryTreeLevelOrderTraversalSolution uses for LC 102.
internal static class PopulatingNextRightPointersInEachNodeSolution
{
    // Textbook baseline: BCL Queue + Dictionary, snapshotting Count at the top of
    // each iteration to know how many nodes belong to the level being drained.
    public static Dictionary<BinaryTreeNode<int>, BinaryTreeNode<int>?> ConnectByManualQueueBfs(
        BinaryTreeNode<int> root)
    {
        var next = new Dictionary<BinaryTreeNode<int>, BinaryTreeNode<int>?>();
        var queue = new Queue<BinaryTreeNode<int>>();
        queue.Enqueue(root);

        while (queue.Count > 0)
        {
            DrainLevel(queue, next);
        }

        return next;
    }

    // One level of the walk: exactly the nodes already queued are drained, each linked
    // to its left-hand neighbor on the way, with that level's children queued up behind
    // them; the level then closes with a null right neighbor on its last node.
    private static void DrainLevel(Queue<BinaryTreeNode<int>> queue, Dictionary<BinaryTreeNode<int>, BinaryTreeNode<int>?> next)
    {
        var levelSize = queue.Count;
        BinaryTreeNode<int>? previous = null;

        for (var i = 0; i < levelSize; i++)
        {
            var node = queue.Dequeue();
            previous = VisitNode(node, queue, next, previous);
        }

        next[previous!] = null;
    }

    // One node of a level: linked to the left-hand neighbor the level has seen so far,
    // then handing its own children to the back of the queue - and returned, so the
    // caller can carry it as the next node's left-hand neighbor.
    private static BinaryTreeNode<int> VisitNode(
        BinaryTreeNode<int> node,
        Queue<BinaryTreeNode<int>> queue,
        Dictionary<BinaryTreeNode<int>, BinaryTreeNode<int>?> next,
        BinaryTreeNode<int>? previous)
    {
        if (previous is not null)
        {
            next[previous] = node;
        }

        if (node.Left is not null)
        {
            queue.Enqueue(node.Left);
        }

        if (node.Right is not null)
        {
            queue.Enqueue(node.Right);
        }

        return node;
    }

    // This repo's own level-grouped BFS: LevelGroupedBreadthFirstTraversal already
    // buffers a whole depth before firing, so the hook just links each buffered
    // level's nodes to their right neighbor, null-ing the last one.
    public static HashMap<BinaryTreeNode<int>, BinaryTreeNode<int>?> ConnectByLevelGroupedTraversal(
        BinaryTreeNode<int> root)
    {
        LevelHooks.BeginCapture();

        LevelGroupedBreadthFirstTraversal.Walk<
            BinaryTreeNode<int>, BinaryTreeTopology<int>, BinaryTreeChildren<int>,
            NaturalChildOrder<BinaryTreeNode<int>, BinaryTreeChildren<int>>, BinaryTreeChildren<int>,
            LevelHooks>(root);

        var next = new HashMap<BinaryTreeNode<int>, BinaryTreeNode<int>?>();

        foreach (var level in LevelHooks.CapturedLevels)
        {
            for (var i = 0; i < level.Count; i++)
            {
                var hasRightNeighbor = i + 1 < level.Count;
                next.Set(level[i], hasRightNeighbor ? RightNeighbor(level, i) : null);
            }
        }

        return next;
    }

    // The node immediately to the right of `index` within one level.
    private static BinaryTreeNode<int> RightNeighbor(List<BinaryTreeNode<int>> level, int index) =>
        level[index + 1];

    // A witness for this problem alone: buffers each depth's nodes so Connect can
    // link them left to right once a level is known complete.
    private readonly struct LevelHooks : ILevelGroupedHooks<BinaryTreeNode<int>>
    {
        // Static because ILevelGroupedHooks is static-abstract - the walk takes the hook as a
        // type argument and reaches it through the type, so no LevelHooks instance exists that
        // could own this buffer - and AsyncLocal is what keeps it safe: the list belongs to the
        // flow that started the Walk, so a traversal on another thread reads its own. Private,
        // with BeginCapture/CapturedLevels the only way in and out: the buffer exists for this
        // one caller's start-and-read pair, so nothing outside the hook needs to name it.
        private static readonly AsyncLocal<List<List<BinaryTreeNode<int>>>> Output = new();

        public static List<List<BinaryTreeNode<int>>> CapturedLevels => Output.Value!;

        public static void BeginCapture() => Output.Value = [];

        public static void OnLevel(IReadOnlyList<BinaryTreeNode<int>> level, int depth) =>
            Output.Value!.Add(level.ToList());
    }
}
