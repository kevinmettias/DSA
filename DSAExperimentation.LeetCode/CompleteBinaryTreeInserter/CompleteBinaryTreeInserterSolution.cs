using DSAExperimentation.Algorithms.Traversal.BreadthFirst;
using DSAExperimentation.DataStructures.Graph.Contracts.Ordering;
using DSAExperimentation.DataStructures.Graph.Engines.Dags.Trees;
using RepoQueue = DSAExperimentation.DataStructures.Queue.Queue<
    DSAExperimentation.DataStructures.Graph.Engines.Dags.Trees.BinaryTreeNode<int>>;

namespace DSAExperimentation.LeetCode.CompleteBinaryTreeInserter;

// LeetCode 919. Complete Binary Tree Inserter: a design problem - LeetCode's own
// shape is a stateful object seeded with the root of a complete binary tree,
// exposing an Insert operation that appends a value in the next left-to-right slot
// and returns the value of the node it was attached to, plus a Root accessor - the
// same design-problem shape KthLargestElementInAStreamSolution uses for LC 703.
//
// ICompleteBinaryTreeInserter is bespoke to this problem alone - no other LeetCode
// entry shares an "attach into the next open slot, return the parent" contract - so
// it stays here rather than in DataStructures/.
internal static class CompleteBinaryTreeInserterSolution
{
    // The textbook baseline this composition has to justify itself against: keep no
    // incremental state at all and re-walk the whole tree from the root on every
    // single Insert (a hand-rolled BCL Queue BFS, O(current size) per call).
    // Deliberately written without this repo's primitives.
    public static ICompleteBinaryTreeInserter CreateByBfsRescan(BinaryTreeNode<int> root) =>
        new BfsRescanInserter(root);

    // The composed answer: seed a candidate queue of "nodes with room for one more
    // child" once, by walking the existing tree level order via this repo's own
    // LevelGroupedBreadthFirstTraversal (PopulatingNextRightPointersInEachNode's own
    // precedent for that walk) and filtering to nodes missing a Left or Right child.
    // Every Insert is then served off that queue in amortized O(1) with no re-walk,
    // held in this repo's own Queue<T> (aliased per ARCHITECTURE.md §10.3, since a
    // bare "Queue" reference resolves to the enclosing namespace segment before any
    // using directive).
    public static ICompleteBinaryTreeInserter CreateByIncompleteQueue(BinaryTreeNode<int> root) =>
        new IncompleteQueueInserter(root);

    private sealed class BfsRescanInserter(BinaryTreeNode<int> root) : ICompleteBinaryTreeInserter
    {
        public BinaryTreeNode<int> Root { get; } = root;

        public int Insert(int value)
        {
            var node = new BinaryTreeNode<int>(value);
            var queue = new Queue<BinaryTreeNode<int>>();
            queue.Enqueue(Root);

            while (queue.Count > 0)
            {
                var current = queue.Dequeue();

                if (current.Left is null)
                {
                    current.Left = node;
                    return current.Value;
                }

                if (current.Right is null)
                {
                    current.Right = node;
                    return current.Value;
                }

                queue.Enqueue(current.Left);
                queue.Enqueue(current.Right);
            }

            return Root.Value;
        }
    }

    private sealed class IncompleteQueueInserter : ICompleteBinaryTreeInserter
    {
        private readonly RepoQueue _incomplete = new();

        public BinaryTreeNode<int> Root { get; }

        public IncompleteQueueInserter(BinaryTreeNode<int> root)
        {
            Root = root;

            LevelHooks.Output.Value = [];
            LevelGroupedBreadthFirstTraversal.Walk<
                BinaryTreeNode<int>, BinaryTreeTopology<int>, BinaryTreeChildren<int>,
                NaturalChildOrder<BinaryTreeNode<int>, BinaryTreeChildren<int>>, BinaryTreeChildren<int>,
                LevelHooks>(root);

            foreach (var level in LevelHooks.Output.Value!)
            {
                foreach (var node in level)
                {
                    if (node.Left is null || node.Right is null)
                    {
                        _incomplete.Enqueue(node);
                    }
                }
            }
        }

        // The new node has two open slots of its own, so it joins the back of the
        // candidate queue; the parent leaves the front once both of its slots are
        // filled.
        public int Insert(int value)
        {
            var node = new BinaryTreeNode<int>(value);
            _incomplete.TryPeek(out var parent);

            if (parent.Left is null)
            {
                parent.Left = node;
            }
            else
            {
                parent.Right = node;
                _incomplete.TryDequeue(out _);
            }

            _incomplete.Enqueue(node);
            return parent.Value;
        }
    }

    // A witness for this problem alone: buffers each depth's nodes so the seeding
    // pass can scan them in level order for open child slots.
    private readonly struct LevelHooks : ILevelGroupedHooks<BinaryTreeNode<int>>
    {
        public static readonly AsyncLocal<List<List<BinaryTreeNode<int>>>> Output = new();

        public static void OnLevel(IReadOnlyList<BinaryTreeNode<int>> level, int depth) =>
            Output.Value!.Add(level.ToList());
    }
}

// The Insert/Root contract every strategy above implements. Bespoke to this problem:
// no other LeetCode entry shares this shape, so it stays here rather than in
// DataStructures/.
internal interface ICompleteBinaryTreeInserter
{
    BinaryTreeNode<int> Root { get; }

    int Insert(int value);
}
