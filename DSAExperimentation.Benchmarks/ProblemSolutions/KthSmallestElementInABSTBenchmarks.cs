using BenchmarkDotNet.Attributes;
using DSAExperimentation.Benchmarks.Fixtures;
using DSAExperimentation.DataStructures.Graph.Engines.Dags.Trees;

namespace DSAExperimentation.Benchmarks.ProblemSolutions;

// Kth Smallest Element in a BST (LC 230): a hand-rolled recursive in-order walk
// (no repo primitive) vs. this repo's own InOrderTraversal/IInOrderHooks over
// BinaryTreeNode<int>. Neither early-exits once the kth value is found - Visit has
// no such signal (see InOrderTraversalHooks's own doc note) - so both are O(n);
// the comparison is genuinely-distinct walk mechanics, the same shape
// LongestValidParenthesesBenchmarks's DP-array-vs-stack pairing already uses, not
// an asymptotic win. The tree itself is built via this repo's own
// BinarySearchTree<int>, inserted in shuffled order so height stays close to
// O(log n) instead of the degenerate O(n) ascending-insertion case.
[MemoryDiagnoser]
public class KthSmallestElementInABSTBenchmarks
{
    private const int MedianDivisor = 2;

    private BinaryTreeNode<int>? _root;

    private int _k;
    private int _recursiveRemaining;
    private int? _recursiveResult;
    [Params(500, 20_000)]
    public int NodeCount { get; set; }

    [GlobalSetup]
    public void Setup()
    {
        var values = SeededSequences.ShuffledOneTo(NodeCount, seed: 1);

        var tree = new BinarySearchTree<int>();

        foreach (var value in values)
        {
            tree.Insert(value);
        }

        _root = tree.Root;
        _k = NodeCount / MedianDivisor;
    }

    [Benchmark(Baseline = true)]
    public int RecursiveInOrderWalk()
    {
        _recursiveRemaining = _k;
        _recursiveResult = null;
        Visit(_root);
        return _recursiveResult!.Value;
    }

    [Benchmark]
    public int InOrderTraversalHooks()
    {
        State.Remaining.Value = _k;
        State.Result.Value = null;
        InOrderTraversal.Walk<int, RankHooks>(_root);
        return State.Result.Value!.Value;
    }

    private void Visit(BinaryTreeNode<int>? node)
    {
        if (node is null)
        {
            return;
        }

        Visit(node.Left);

        if (_recursiveResult is null)
        {
            _recursiveRemaining--;

            if (_recursiveRemaining == 0)
            {
                _recursiveResult = node.Value;
            }
        }

        Visit(node.Right);
    }

    private readonly struct RankHooks : IInOrderHooks<int>
    {
        public static void Visit(BinaryTreeNode<int> node, int depth)
        {
            if (State.Result.Value is not null)
            {
                return;
            }

            State.Remaining.Value--;

            if (State.Remaining.Value == 0)
            {
                State.Result.Value = node.Value;
            }
        }
    }

    private static class State
    {
        public static readonly AsyncLocal<int> Remaining = new();
        public static readonly AsyncLocal<int?> Result = new();
    }
}
