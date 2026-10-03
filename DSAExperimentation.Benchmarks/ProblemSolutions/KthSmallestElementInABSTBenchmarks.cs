using BenchmarkDotNet.Attributes;
using DSAExperimentation.Benchmarks.Fixtures;
using DSAExperimentation.DataStructures.Graph.Engines.Dags.Trees;
using DSAExperimentation.LeetCode.KthSmallestElementInABST;

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

    private int _targetRank;

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
        _targetRank = NodeCount / MedianDivisor;
    }

    [Benchmark(Baseline = true)]
    public int RecursiveInOrderWalk() => KthSmallestElementInABSTSolution.KthSmallestByRecursiveWalk(_root, _targetRank);

    [Benchmark]
    public int InOrderTraversalHooks() => KthSmallestElementInABSTSolution.KthSmallestByInOrderTraversal(_root, _targetRank);
}
