using BenchmarkDotNet.Attributes;
using DSAExperimentation.DataStructures.Graph.Engines.Dags.Trees;

namespace DSAExperimentation.Benchmarks.ProblemSolutions;

// Increasing Order Search Tree (LC 897): a hand-rolled recursive in-order walk (no
// repo primitive) threading a running tail node through recursive parameters and
// return values, vs. this repo's own InOrderTraversal/IInOrderHooks doing the
// identical Left=null/Right=tail relink through AsyncLocal-threaded state - the same
// genuinely-distinct-walk-mechanics comparison KthSmallestElementInABSTBenchmarks
// already makes for LC 230, not an asymptotic win (both are O(n)). Each [Benchmark]
// rebuilds a fresh, shuffle-inserted BST from the same source values every
// invocation via this repo's own BinarySearchTree<int>, since both walks mutate the
// tree's Left/Right pointers in place and would otherwise corrupt a later iteration.
[MemoryDiagnoser]
public class IncreasingOrderSearchTreeBenchmarks
{
    [Params(500, 20_000)]
    public int NodeCount;

    private int[] _shuffledValues = null!;

    [GlobalSetup]
    public void Setup()
    {
        var values = Enumerable.Range(1, NodeCount).ToArray();
        var random = new Random(1);

        for (var i = values.Length - 1; i > 0; i--)
        {
            var j = random.Next(i + 1);
            (values[i], values[j]) = (values[j], values[i]);
        }

        _shuffledValues = values;
    }

    [Benchmark(Baseline = true)]
    public int RecursiveRelink()
    {
        var dummy = new BinaryTreeNode<int>(0);
        Visit(BuildTree(), dummy);
        return dummy.Right!.Value;
    }

    [Benchmark]
    public int InOrderTraversalHooks()
    {
        var dummy = new BinaryTreeNode<int>(0);
        State.Tail.Value = dummy;
        InOrderTraversal.Walk<int, RelinkHooks>(BuildTree());
        return dummy.Right!.Value;
    }

    private BinaryTreeNode<int> BuildTree()
    {
        var tree = new BinarySearchTree<int>();

        foreach (var value in _shuffledValues)
        {
            tree.Insert(value);
        }

        return tree.Root!;
    }

    private static BinaryTreeNode<int> Visit(BinaryTreeNode<int>? node, BinaryTreeNode<int> tail)
    {
        if (node is null)
        {
            return tail;
        }

        tail = Visit(node.Left, tail);

        var right = node.Right;
        node.Left = null;
        tail.Right = node;

        return Visit(right, node);
    }

    private readonly struct RelinkHooks : IInOrderHooks<int>
    {
        public static void Visit(BinaryTreeNode<int> node, int depth)
        {
            node.Left = null;
            State.Tail.Value!.Right = node;
            State.Tail.Value = node;
        }
    }

    private static class State
    {
        public static readonly AsyncLocal<BinaryTreeNode<int>?> Tail = new();
    }
}
