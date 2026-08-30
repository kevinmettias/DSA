using BenchmarkDotNet.Attributes;
using DSAExperimentation.DataStructures.DynamicArray;
using DSAExperimentation.DataStructures.Graph.Engines.Dags.Trees;

namespace DSAExperimentation.Benchmarks.ProblemSolutions;

// Find Mode in Binary Search Tree (LC 501): the general "any binary tree" fix -
// a full traversal building a Dictionary<int,int> frequency count, then a second
// pass to find the max and collect ties (O(n) time, O(n) extra space) - vs. this
// repo's own InOrderTraversal/IInOrderHooks, which exploits the BST's sorted
// in-order walk to track a single running streak and needs no frequency map at
// all (O(n) time, O(1) extra space beyond the output), the same composition
// KthSmallestElementInABSTBenchmarks uses for its own InOrderTraversal-vs-hand-
// -rolled pairing. The tree is built with duplicates allowed (value <= node.Value
// goes left), which this repo's own BinarySearchTree<TValue>.Insert rejects as a
// no-op - LeetCode 501's BST explicitly permits repeated values, so the fixture
// builds BinaryTreeNode<int> nodes directly instead.
[MemoryDiagnoser]
public class FindModeInBinarySearchTreeBenchmarks
{
    [Params(500, 20_000)]
    public int NodeCount;

    private BinaryTreeNode<int>? _root;

    [GlobalSetup]
    public void Setup()
    {
        var distinctValues = Math.Max(1, NodeCount / 20);
        var values = new int[NodeCount];

        for (var i = 0; i < NodeCount; i++)
        {
            values[i] = i % distinctValues;
        }

        var random = new Random(501);
        for (var i = values.Length - 1; i > 0; i--)
        {
            var j = random.Next(i + 1);
            (values[i], values[j]) = (values[j], values[i]);
        }

        BinaryTreeNode<int>? root = null;
        foreach (var value in values)
        {
            InsertAllowingDuplicates(ref root, value);
        }

        _root = root;
    }

    [Benchmark(Baseline = true)]
    public int[] HashMapFrequencyCount()
    {
        var counts = new Dictionary<int, int>();
        CountFrequencies(_root, counts);

        var maxCount = 0;
        foreach (var count in counts.Values)
        {
            if (count > maxCount)
            {
                maxCount = count;
            }
        }

        var modes = new List<int>();
        foreach (var (value, count) in counts)
        {
            if (count == maxCount)
            {
                modes.Add(value);
            }
        }

        return modes.ToArray();
    }

    [Benchmark]
    public int[] InOrderTraversalStreak()
    {
        State.Modes.Value = new DynamicArray<int>();
        State.CurrentCount.Value = 0;
        State.MaxCount.Value = 0;

        InOrderTraversal.Walk<int, ModeHooks>(_root);

        var modes = State.Modes.Value;
        var result = new int[modes.Count];

        for (var i = 0; i < modes.Count; i++)
        {
            result[i] = modes.Get(i);
        }

        return result;
    }

    private static void CountFrequencies(BinaryTreeNode<int>? node, Dictionary<int, int> counts)
    {
        if (node is null)
        {
            return;
        }

        counts[node.Value] = counts.GetValueOrDefault(node.Value) + 1;
        CountFrequencies(node.Left, counts);
        CountFrequencies(node.Right, counts);
    }

    private static void InsertAllowingDuplicates(ref BinaryTreeNode<int>? root, int value)
    {
        if (root is null)
        {
            root = new BinaryTreeNode<int>(value);
            return;
        }

        var node = root;
        while (true)
        {
            if (value <= node.Value)
            {
                if (node.Left is null)
                {
                    node.Left = new BinaryTreeNode<int>(value);
                    return;
                }

                node = node.Left;
            }
            else
            {
                if (node.Right is null)
                {
                    node.Right = new BinaryTreeNode<int>(value);
                    return;
                }

                node = node.Right;
            }
        }
    }

    private readonly struct ModeHooks : IInOrderHooks<int>
    {
        public static void Visit(BinaryTreeNode<int> node, int depth)
        {
            if (State.CurrentCount.Value == 0 || node.Value != State.CurrentValue.Value)
            {
                State.CurrentValue.Value = node.Value;
                State.CurrentCount.Value = 0;
            }

            State.CurrentCount.Value++;

            if (State.CurrentCount.Value > State.MaxCount.Value)
            {
                State.MaxCount.Value = State.CurrentCount.Value;
                State.Modes.Value = new DynamicArray<int>();
                State.Modes.Value.Add(node.Value);
            }
            else if (State.CurrentCount.Value == State.MaxCount.Value)
            {
                State.Modes.Value!.Add(node.Value);
            }
        }
    }

    private static class State
    {
        public static readonly AsyncLocal<DynamicArray<int>> Modes = new();
        public static readonly AsyncLocal<int> CurrentValue = new();
        public static readonly AsyncLocal<int> CurrentCount = new();
        public static readonly AsyncLocal<int> MaxCount = new();
    }
}
