using DSAExperimentation.Algorithms.Traversal.BreadthFirst;
using DSAExperimentation.DataStructures.Graph.Contracts.Ordering;
using DSAExperimentation.DataStructures.Graph.Engines.Dags.Trees;

namespace DSAExperimentation.LeetCode.BinaryTreeLevelOrderTraversalII;

// LeetCode 107. Binary Tree Level Order Traversal II: return the tree's node
// values grouped by level, bottom level first.
//
// Both strategies walk the tree level by level and reverse the buffered result;
// they differ only in how the level grouping itself is produced - a hand-rolled
// BCL Queue loop, or this repo's own LevelGroupedBreadthFirstTraversal engine.
internal static class BinaryTreeLevelOrderTraversalIISolution
{
    // The textbook answer: a BCL Queue, draining exactly one level per iteration
    // (its size read before any of that level's children are enqueued), then
    // reversing the buffered levels. Deliberately written without this repo's
    // traversal primitives - it is the arm the composed solution below has to
    // justify itself against.
    public static List<List<int>> LevelOrderBottomByQueue(BinaryTreeNode<int>? root)
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
            var level = new List<int>();

            for (var remaining = queue.Count; remaining > 0; remaining--)
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

        levels.Reverse();
        return levels;
    }

    // This repo's own level-grouped BFS: LevelGroupedBreadthFirstTraversal already
    // buffers one whole depth before firing, so the only thing this strategy adds
    // is collecting each level's values and reversing the buffered result.
    public static List<List<int>> LevelOrderBottomByLevelGroupedTraversal(BinaryTreeNode<int>? root)
    {
        LevelHooks.Output.Value = [];
        LevelGroupedBreadthFirstTraversal.Walk<
            BinaryTreeNode<int>, BinaryTreeTopology<int>, BinaryTreeChildren<int>,
            NaturalChildOrder<BinaryTreeNode<int>, BinaryTreeChildren<int>>, BinaryTreeChildren<int>,
            LevelHooks>(root);

        var levels = LevelHooks.Output.Value!;
        levels.Reverse();
        return levels;
    }

    // Answers this problem alone - collects each buffered level's values into the
    // shape LeetCode expects, nothing LevelGroupedBreadthFirstTraversal's other
    // callers would want.
    private readonly struct LevelHooks : ILevelGroupedHooks<BinaryTreeNode<int>>
    {
        public static readonly AsyncLocal<List<List<int>>> Output = new();

        public static void OnLevel(IReadOnlyList<BinaryTreeNode<int>> level, int depth) =>
            Output.Value!.Add(level.Select(n => n.Value).ToList());
    }
}
