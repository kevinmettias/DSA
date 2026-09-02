using BenchmarkDotNet.Attributes;
using DSAExperimentation.Algorithms.Searching;
using DSAExperimentation.DataStructures.DynamicArray;
using DSAExperimentation.DataStructures.Graph.Engines.Dags.Trees;
using DSAExperimentation.DataStructures.Sequence;

namespace DSAExperimentation.Benchmarks.ProblemSolutions;

// Closest Nodes Queries in a Binary Search Tree (LC 2476): an O(n) per-query
// recursive scan of every node's value (the "ignore the BST invariant entirely"
// baseline) vs. this repo's own InOrderTraversal/IInOrderHooks collecting every
// value into a sorted DynamicArray<int> exactly once - the same composition
// FindModeInBinarySearchTreeBenchmarks/KthSmallestElementInABSTBenchmarks already
// use - then answering each query with BinarySearch.LowerBound's insertion point,
// the same floor/ceiling-from-an-insertion-point move ClosestRoomBenchmarks
// already proves out. O(n * queries) vs. O(n + queries * log n). The tree itself
// is built with this repo's own BinarySearchTree<int>.Insert over a shuffled
// distinct-value permutation, the same random-insertion-order precedent
// FindModeInBinarySearchTreeBenchmarks uses for a realistically shaped tree.
[MemoryDiagnoser]
public class ClosestNodesQueriesInABinarySearchTreeBenchmarks
{
    private const int RandomSeed = 2476; // LeetCode problem number

    private const int MaxQueryOffset = 5;

    [Params(500, 20_000)]
    public int NodeCount;

    private BinaryTreeNode<int>? _root;
    private int[] _queries = null!;

    [GlobalSetup]
    public void Setup()
    {
        var values = Enumerable.Range(0, NodeCount).ToArray();
        var random = new Random(RandomSeed);

        for (var i = values.Length - 1; i > 0; i--)
        {
            var j = random.Next(i + 1);
            (values[i], values[j]) = (values[j], values[i]);
        }

        var tree = new BinarySearchTree<int>();

        foreach (var value in values)
        {
            tree.Insert(value);
        }

        _root = tree.Root;
        _queries = Enumerable.Range(0, NodeCount)
            .Select(_ => random.Next(-MaxQueryOffset, NodeCount + MaxQueryOffset))
            .ToArray();
    }

    [Benchmark(Baseline = true)]
    public long LinearScanPerQuery()
    {
        var total = 0L;

        foreach (var query in _queries)
        {
            var (floor, ceiling) = LinearFloorAndCeiling(_root, query);
            total += floor + ceiling;
        }

        return total;
    }

    private static (int Floor, int Ceiling) LinearFloorAndCeiling(BinaryTreeNode<int>? node, int query)
    {
        var floor = -1;
        var ceiling = -1;
        Walk(node, query, ref floor, ref ceiling);
        return (floor, ceiling);
    }

    private static void Walk(BinaryTreeNode<int>? node, int query, ref int floor, ref int ceiling)
    {
        if (node is null)
        {
            return;
        }

        if (node.Value <= query && (floor == -1 || node.Value > floor))
        {
            floor = node.Value;
        }

        if (node.Value >= query && (ceiling == -1 || node.Value < ceiling))
        {
            ceiling = node.Value;
        }

        Walk(node.Left, query, ref floor, ref ceiling);
        Walk(node.Right, query, ref floor, ref ceiling);
    }

    [Benchmark]
    public long SortedInOrderWithBinarySearch()
    {
        State.Values.Value = new DynamicArray<int>();
        InOrderTraversal.Walk<int, CollectHooks>(_root);
        var values = State.Values.Value;
        var total = 0L;

        foreach (var query in _queries)
        {
            var (floor, ceiling) = FloorAndCeiling(values, query);
            total += floor + ceiling;
        }

        return total;
    }

    private static (int Floor, int Ceiling) FloorAndCeiling(DynamicArray<int> values, int query)
    {
        var index = BinarySearch.LowerBound(new DynamicArraySequence<int>(values), query);

        if (index < values.Count && values.Get(index) == query)
        {
            return (query, query);
        }

        var floor = index > 0 ? values.Get(index - 1) : -1;
        var ceiling = index < values.Count ? values.Get(index) : -1;
        return (floor, ceiling);
    }

    private readonly struct CollectHooks : IInOrderHooks<int>
    {
        public static void Visit(BinaryTreeNode<int> node, int depth) => State.Values.Value!.Add(node.Value);
    }

    private static class State
    {
        public static readonly AsyncLocal<DynamicArray<int>> Values = new();
    }
}
