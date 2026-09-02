using DSAExperimentation.Algorithms.Traversal.BreadthFirst;
using DSAExperimentation.DataStructures.Graph.Contracts.Ordering;
using DSAExperimentation.DataStructures.Graph.Engines.Dags.Trees;

namespace DSAExperimentation.LeetCode.BinaryTreeLevelOrderTraversal;

// LeetCode 102. Binary Tree Level Order Traversal: the node values grouped by
// depth, top level first.
//
// The two strategies differ only in how a level's boundary is discovered - a
// hand-rolled BCL Queue that snapshots Count before draining a level, or this
// repo's own LevelGroupedBreadthFirstTraversal, which already buffers exactly
// one level before firing its hook.
internal static class BinaryTreeLevelOrderTraversalSolution
{
    // Textbook baseline: BCL Queue, snapshotting Count at the top of each
    // iteration to know how many nodes belong to the level being drained.
    public static List<List<int>> LevelOrderByQueueLevels(BinaryTreeNode<int>? root)
    {
        var levels = new List<List<int>>();

        if (root is null)
        {
            return levels;
        }

        var queue = new Queue<BinaryTreeNode<int>>();
        queue.Enqueue(root);

        while (queue.Count > 0)
        {
            var levelSize = queue.Count;
            var level = new List<int>(levelSize);

            for (var i = 0; i < levelSize; i++)
            {
                var node = queue.Dequeue();
                level.Add(node.Value);

                if (node.Left is not null)
                {
                    queue.Enqueue(node.Left);
                }

                if (node.Right is not null)
                {
                    queue.Enqueue(node.Right);
                }
            }

            levels.Add(level);
        }

        return levels;
    }

    // This repo's own level-grouped BFS: LevelGroupedBreadthFirstTraversal
    // already buffers a whole depth before firing, so the hook just projects
    // each buffered level onto its node values.
    public static List<List<int>> LevelOrderByLevelGroupedTraversal(BinaryTreeNode<int>? root)
    {
        LevelHooks.Output.Value = [];
        LevelGroupedBreadthFirstTraversal.Walk<
            BinaryTreeNode<int>, BinaryTreeTopology<int>, BinaryTreeChildren<int>,
            NaturalChildOrder<BinaryTreeNode<int>, BinaryTreeChildren<int>>, BinaryTreeChildren<int>,
            LevelHooks>(root);
        return LevelHooks.Output.Value!;
    }

    private readonly struct LevelHooks : ILevelGroupedHooks<BinaryTreeNode<int>>
    {
        public static readonly AsyncLocal<List<List<int>>> Output = new();

        public static void OnLevel(IReadOnlyList<BinaryTreeNode<int>> level, int depth) =>
            Output.Value!.Add(level.Select(n => n.Value).ToList());
    }
}
