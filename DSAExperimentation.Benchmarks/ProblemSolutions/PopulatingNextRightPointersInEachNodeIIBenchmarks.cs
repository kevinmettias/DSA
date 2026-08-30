using BenchmarkDotNet.Attributes;
using DSAExperimentation.Algorithms.Traversal.BreadthFirst;
using DSAExperimentation.Benchmarks.Fixtures;
using DSAExperimentation.DataStructures.Graph.Contracts.Ordering;
using DSAExperimentation.DataStructures.Graph.Engines.Dags.Trees;
using DSAExperimentation.DataStructures.HashMap;

namespace DSAExperimentation.Benchmarks.ProblemSolutions;

// Populating Next Right Pointers in Each Node II (LC 117): identical connect
// contract to #116, but the input is only guaranteed to be an arbitrary binary
// tree, not a perfect one. BinaryTrees.Skewed (a right-only chain, one node per
// level) is the extreme non-perfect shape - proof that neither benchmarked
// strategy needs perfect-tree-specific code to stay correct here.
[MemoryDiagnoser]
public class PopulatingNextRightPointersInEachNodeIIBenchmarks
{
    [Params(200, 5_000)]
    public int NodeCount;

    private BinaryTreeNode<int> _root = null!;

    [GlobalSetup]
    public void Setup() => _root = BinaryTrees.Skewed(NodeCount);

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
