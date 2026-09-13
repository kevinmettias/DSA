using BenchmarkDotNet.Attributes;
using DSAExperimentation.Benchmarks.Fixtures;
using DSAExperimentation.DataStructures.Graph.Engines.Dags.Trees;
using DSAExperimentation.LeetCode.BalanceABinarySearchTree;

namespace DSAExperimentation.Benchmarks.ProblemSolutions;

// Harness only: both arms are BalanceABinarySearchTreeSolution's, the same methods
// BalanceABinarySearchTreeTests proves correct. Re-deriving the k-th smallest value
// from scratch for every rank k = 1..n (each call its own O(n) in-order walk,
// O(n^2) overall) vs. this repo's own InOrderTraversal/IInOrderHooks, which visits
// every node exactly once. Both then rebuild by the identical midpoint-split
// recursion. Fixtures.BinaryTrees.Skewed gives a maximally unbalanced (but
// already-BST-ordered) input tree, built in [GlobalSetup] so its construction is
// not charged to the measured balance. Each arm returns LeetCode's real answer -
// the rebuilt root - and the harness walks it for a height, so the rebuild cannot
// be eliminated as dead code.
[MemoryDiagnoser]
public class BalanceABinarySearchTreeBenchmarks
{
    [Params(200, 2_000)]
    public int NodeCount;

    private BinaryTreeNode<int> _root = null!;

    [GlobalSetup]
    public void Setup() => _root = BinaryTrees.Skewed(NodeCount);

    [Benchmark(Baseline = true)]
    public int RepeatedKthSmallestScan() =>
        Height(BalanceABinarySearchTreeSolution.BalanceByRepeatedKthSmallest(_root));

    [Benchmark]
    public int InOrderTraversalCollectAndRebuild() =>
        Height(BalanceABinarySearchTreeSolution.BalanceByInOrderTraversal(_root));

    private static int Height(BinaryTreeNode<int>? node)
        => node is null ? 0 : 1 + Math.Max(Height(node.Left), Height(node.Right));
}
