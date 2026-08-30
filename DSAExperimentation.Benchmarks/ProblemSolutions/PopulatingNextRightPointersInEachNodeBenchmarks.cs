using BenchmarkDotNet.Attributes;
using DSAExperimentation.Algorithms.Traversal.BreadthFirst;
using DSAExperimentation.Benchmarks.Fixtures;
using DSAExperimentation.DataStructures.Graph.Contracts.Ordering;
using DSAExperimentation.DataStructures.Graph.Engines.Dags.Trees;
using DSAExperimentation.DataStructures.HashMap;

namespace DSAExperimentation.Benchmarks.ProblemSolutions;

// Populating Next Right Pointers in Each Node (LC 116): a hand-rolled BCL-Queue
// level-by-level BFS baseline vs. this repo's LevelGroupedBreadthFirstTraversal +
// HashMap<TKey,TValue> (BinaryTreeNode<int> has no Next field of its own, so "connect"
// is represented as a node -> next-node map, same shape the coverage test asserts
// against). Fixture sizes are 2^k-1 so BinaryTrees.Balanced is a genuinely perfect
// tree, matching this problem's guarantee.
[MemoryDiagnoser]
public class PopulatingNextRightPointersInEachNodeBenchmarks
{
    [Params(63, 1_023)]
    public int NodeCount;

    private BinaryTreeNode<int> _root = null!;

    [GlobalSetup]
    public void Setup() => _root = BinaryTrees.Balanced(NodeCount);

    [Benchmark(Baseline = true)]
    public int ManualQueueBfs()
    {
        var next = new Dictionary<BinaryTreeNode<int>, BinaryTreeNode<int>?>();
        var queue = new Queue<BinaryTreeNode<int>>();
        queue.Enqueue(_root);
        var linked = 0;

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
                    linked++;
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

        return linked;
    }

    [Benchmark]
    public int LevelGroupedTraversal()
    {
        LevelHooks.Output.Value = [];

        LevelGroupedBreadthFirstTraversal.Walk<
            BinaryTreeNode<int>, BinaryTreeTopology<int>, BinaryTreeChildren<int>,
            NaturalChildOrder<BinaryTreeNode<int>, BinaryTreeChildren<int>>, BinaryTreeChildren<int>, LevelHooks>(_root);

        var next = new HashMap<BinaryTreeNode<int>, BinaryTreeNode<int>?>();
        var linked = 0;

        foreach (var level in LevelHooks.Output.Value!)
        {
            for (var i = 0; i < level.Count; i++)
            {
                var nextNode = i + 1 < level.Count ? level[i + 1] : null;
                next.Set(level[i], nextNode);

                if (nextNode is not null)
                {
                    linked++;
                }
            }
        }

        return linked;
    }

    private readonly struct LevelHooks : ILevelGroupedHooks<BinaryTreeNode<int>>
    {
        public static readonly AsyncLocal<List<List<BinaryTreeNode<int>>>> Output = new();

        public static void OnLevel(IReadOnlyList<BinaryTreeNode<int>> level, int depth) => Output.Value!.Add(level.ToList());
    }
}
