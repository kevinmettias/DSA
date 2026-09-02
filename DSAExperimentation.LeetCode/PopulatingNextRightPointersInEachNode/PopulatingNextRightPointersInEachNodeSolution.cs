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

        return next;
    }

    // This repo's own level-grouped BFS: LevelGroupedBreadthFirstTraversal already
    // buffers a whole depth before firing, so the hook just links each buffered
    // level's nodes to their right neighbor, null-ing the last one.
    public static HashMap<BinaryTreeNode<int>, BinaryTreeNode<int>?> ConnectByLevelGroupedTraversal(
        BinaryTreeNode<int> root)
    {
        LevelHooks.Output.Value = [];

        LevelGroupedBreadthFirstTraversal.Walk<
            BinaryTreeNode<int>, BinaryTreeTopology<int>, BinaryTreeChildren<int>,
            NaturalChildOrder<BinaryTreeNode<int>, BinaryTreeChildren<int>>, BinaryTreeChildren<int>,
            LevelHooks>(root);

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

    // A witness for this problem alone: buffers each depth's nodes so Connect can
    // link them left to right once a level is known complete.
    private readonly struct LevelHooks : ILevelGroupedHooks<BinaryTreeNode<int>>
    {
        public static readonly AsyncLocal<List<List<BinaryTreeNode<int>>>> Output = new();

        public static void OnLevel(IReadOnlyList<BinaryTreeNode<int>> level, int depth) =>
            Output.Value!.Add(level.ToList());
    }
}
