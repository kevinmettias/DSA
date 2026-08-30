using BenchmarkDotNet.Attributes;
using DSAExperimentation.DataStructures.DynamicArray;
using DSAExperimentation.DataStructures.Graph.Engines.Dags.Trees;

namespace DSAExperimentation.Benchmarks.ProblemSolutions;

// All Elements in Two Binary Search Trees (LC 1305): dumping every node from both
// trees into one list and sorting it - O((n+m) log(n+m)) - vs. this repo's own
// InOrderTraversal/IInOrderHooks collecting each tree's already-ascending sequence
// into a DynamicArray<int>, then a plain two-pointer merge of the two sorted
// sequences - O(n+m), the same composition AllElementsInTwoBinarySearchTreesTests
// uses. Both trees are built with this repo's own BinarySearchTree<int>.Insert over
// a shuffled permutation, so insertion order stays randomized (no adversarial
// ascending-order degeneration) the same way FindModeInBinarySearchTreeBenchmarks'
// fixture is shuffled.
[MemoryDiagnoser]
public class AllElementsInTwoBinarySearchTreesBenchmarks
{
    [Params(300, 5_000)]
    public int NodeCount;

    private BinaryTreeNode<int>? _root1;
    private BinaryTreeNode<int>? _root2;

    [GlobalSetup]
    public void Setup()
    {
        _root1 = BuildTree(seed: 1305);
        _root2 = BuildTree(seed: 1306);
    }

    [Benchmark(Baseline = true)]
    public int[] CollectAllThenSort()
    {
        var values = new List<int>(NodeCount * 2);
        CollectAll(_root1, values);
        CollectAll(_root2, values);

        var result = values.ToArray();
        Array.Sort(result);
        return result;
    }

    [Benchmark]
    public int[] InOrderTraversalMerge()
    {
        var first = CollectInOrder(_root1);
        var second = CollectInOrder(_root2);
        return MergeSortedLists(first, second);
    }

    private BinaryTreeNode<int>? BuildTree(int seed)
    {
        var values = Enumerable.Range(0, NodeCount).ToArray();
        var random = new Random(seed);

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

        return tree.Root;
    }

    private static void CollectAll(BinaryTreeNode<int>? node, List<int> values)
    {
        if (node is null)
        {
            return;
        }

        values.Add(node.Value);
        CollectAll(node.Left, values);
        CollectAll(node.Right, values);
    }

    private static DynamicArray<int> CollectInOrder(BinaryTreeNode<int>? root)
    {
        State.Values.Value = new DynamicArray<int>();
        InOrderTraversal.Walk<int, CollectHooks>(root);
        return State.Values.Value;
    }

    private static int[] MergeSortedLists(DynamicArray<int> first, DynamicArray<int> second)
    {
        var result = new int[first.Count + second.Count];
        var i = 0;
        var j = 0;
        var k = 0;

        while (i < first.Count && j < second.Count)
        {
            result[k++] = first.Get(i) <= second.Get(j) ? first.Get(i++) : second.Get(j++);
        }

        while (i < first.Count)
        {
            result[k++] = first.Get(i++);
        }

        while (j < second.Count)
        {
            result[k++] = second.Get(j++);
        }

        return result;
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
