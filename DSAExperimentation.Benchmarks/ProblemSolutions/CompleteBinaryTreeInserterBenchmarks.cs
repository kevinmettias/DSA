using BenchmarkDotNet.Attributes;
using DSAExperimentation.Algorithms.Traversal.BreadthFirst;
using DSAExperimentation.Benchmarks.Fixtures;
using DSAExperimentation.DataStructures.Graph.Contracts.Ordering;
using DSAExperimentation.DataStructures.Graph.Engines.Dags.Trees;
using RepoQueue = DSAExperimentation.DataStructures.Queue.Queue<DSAExperimentation.DataStructures.Graph.Engines.Dags.Trees.BinaryTreeNode<int>>;

namespace DSAExperimentation.Benchmarks.ProblemSolutions;

// Complete Binary Tree Inserter (LC 919): NaiveRescanPerInsert re-walks the whole
// tree from the root on every single Insert to find the first node with an open
// child slot (a hand-rolled BCL-Queue BFS, O(current size) per call, the textbook
// approach absent any incremental state) vs. QueueTrackedInserter, which seeds a
// candidate queue once via this repo's own LevelGroupedBreadthFirstTraversal
// (PopulatingNextRightPointersInEachNodeBenchmarks' own precedent for that walk)
// and this repo's own Queue<T> (aliased per ARCHITECTURE.md §10.3, same as the
// coverage test), then serves every Insert off that queue in amortized O(1) with
// no re-walk. Each [Benchmark] rebuilds a fresh perfect tree (BinaryTrees.Balanced,
// 2^k-1 nodes) and replays the same InsertCount insertions, so both strategies pay
// for the full sequence, not just one call.
[MemoryDiagnoser]
public class CompleteBinaryTreeInserterBenchmarks
{
    private const int InsertCount = 200;

    [Params(63, 1_023)]
    public int NodeCount;

    private int[] _insertValues = null!;

    [GlobalSetup]
    public void Setup() => _insertValues = Enumerable.Range(NodeCount, InsertCount).ToArray();

    [Benchmark(Baseline = true)]
    public int NaiveRescanPerInsert()
    {
        var root = BinaryTrees.Balanced(NodeCount);

        foreach (var value in _insertValues)
        {
            var node = new BinaryTreeNode<int>(value);
            InsertViaBfsRescan(root, node);
        }

        return root.Value;
    }

    // Re-walks the tree from the root via BFS on every call to find the first
    // node with an open child slot - no incremental state is kept between calls.
    private static void InsertViaBfsRescan(BinaryTreeNode<int> root, BinaryTreeNode<int> node)
    {
        var queue = new Queue<BinaryTreeNode<int>>();
        queue.Enqueue(root);

        while (queue.Count > 0)
        {
            var current = queue.Dequeue();

            if (current.Left is null)
            {
                current.Left = node;
                break;
            }

            if (current.Right is null)
            {
                current.Right = node;
                break;
            }

            queue.Enqueue(current.Left);
            queue.Enqueue(current.Right);
        }
    }

    [Benchmark]
    public int QueueTrackedInserter()
    {
        var root = BinaryTrees.Balanced(NodeCount);
        var incomplete = SeedIncompleteQueue(root);

        foreach (var value in _insertValues)
        {
            InsertUsingIncompleteQueue(incomplete, value);
        }

        return root.Value;
    }

    // One BFS pass (via this repo's own LevelGroupedBreadthFirstTraversal) finds
    // every node with an open child slot; InsertUsingIncompleteQueue then serves
    // off this queue with no further re-walks.
    private static RepoQueue SeedIncompleteQueue(BinaryTreeNode<int> root)
    {
        var incomplete = new RepoQueue();

        LevelHooks.Output.Value = [];
        LevelGroupedBreadthFirstTraversal.Walk<
            BinaryTreeNode<int>, BinaryTreeTopology<int>, BinaryTreeChildren<int>,
            NaturalChildOrder<BinaryTreeNode<int>, BinaryTreeChildren<int>>, BinaryTreeChildren<int>, LevelHooks>(root);

        foreach (var level in LevelHooks.Output.Value!)
        {
            foreach (var node in level)
            {
                if (node.Left is null || node.Right is null)
                {
                    incomplete.Enqueue(node);
                }
            }
        }

        return incomplete;
    }

    // Serves an insert off the pre-seeded incomplete-node queue in amortized
    // O(1), re-enqueuing the new node (it will have open slots of its own) and
    // dequeuing the parent once both of its slots are filled.
    private static void InsertUsingIncompleteQueue(RepoQueue incomplete, int value)
    {
        var node = new BinaryTreeNode<int>(value);
        incomplete.TryPeek(out var parent);

        if (parent.Left is null)
        {
            parent.Left = node;
        }
        else
        {
            parent.Right = node;
            incomplete.TryDequeue(out _);
        }

        incomplete.Enqueue(node);
    }

    private readonly struct LevelHooks : ILevelGroupedHooks<BinaryTreeNode<int>>
    {
        public static readonly AsyncLocal<List<List<BinaryTreeNode<int>>>> Output = new();

        public static void OnLevel(IReadOnlyList<BinaryTreeNode<int>> level, int depth) => Output.Value!.Add(level.ToList());
    }
}
