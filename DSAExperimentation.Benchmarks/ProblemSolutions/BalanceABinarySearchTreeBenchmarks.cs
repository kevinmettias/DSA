using DSAExperimentation.Benchmarks.Fixtures;
using DSAExperimentation.DataStructures.Graph.Engines.Dags.Trees;
using DSAExperimentation.LeetCode.BalanceABinarySearchTree;

namespace DSAExperimentation.Benchmarks.ProblemSolutions;

// Harness only: both arms are BalanceABinarySearchTreeSolution's, the same methods
// BalanceABinarySearchTreeSolutionTests proves correct. Re-deriving the k-th smallest value
// from scratch for every rank k = 1..n (each call its own O(n) in-order walk,
// O(n^2) overall) vs. this repo's own InOrderTraversal/IInOrderHooks, which visits
// every node exactly once. Both then rebuild by the identical midpoint-split
// recursion. Fixtures.BinaryTrees.Skewed gives a maximally unbalanced (but
// already-BST-ordered) input tree, built in [GlobalSetup] so its construction is
// not charged to the measured balance. Each arm returns LeetCode's real answer -
// the rebuilt root - as object?, since a public [Benchmark] method cannot name the
// internal BinaryTreeNode<int> (CS0050).
public class BalanceABinarySearchTreeBenchmarks
{
    private BinaryTreeNode<int> _root = null!;

    [Params(200, 2_000)]
    public int NodeCount { get; set; }

    [GlobalSetup]
    public void Setup() => _root = BinaryTrees.Skewed(NodeCount);

    [Benchmark(Baseline = true)]
    public object? RepeatedKthSmallestScan() =>
        BalanceABinarySearchTreeSolution.BalanceByRepeatedKthSmallest(_root);

    [Benchmark]
    public object? InOrderTraversalCollectAndRebuild() =>
        BalanceABinarySearchTreeSolution.BalanceByInOrderTraversal(_root);
}
