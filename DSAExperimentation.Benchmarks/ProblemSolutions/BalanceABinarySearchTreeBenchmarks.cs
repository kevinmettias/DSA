using DSAExperimentation.DataStructures.Graph.Engines.Dags.Trees;
using DSAExperimentation.LeetCode.BalanceABinarySearchTree;

namespace DSAExperimentation.Benchmarks.ProblemSolutions;

// Harness only: both arms are BalanceABinarySearchTreeSolution's, the same methods
// BalanceABinarySearchTreeSolutionTests proves correct. Re-deriving the k-th smallest value
// from scratch for every rank k = 1..n (each call its own O(n) in-order walk,
// O(n^2) overall) vs. this repo's own InOrderTraversal/IInOrderHooks, which visits
// every node exactly once. Both then rebuild by the identical midpoint-split
// recursion. The input is a maximally unbalanced (but already-BST-ordered) right-only
// chain, built in [GlobalSetup] so its construction is not charged to the measured
// balance. It holds 1..n rather than Fixtures.BinaryTrees.Skewed's 0..n-1 because
// LC 1382's node values start at 1. Each arm returns LeetCode's real answer -
// the rebuilt root - as object?, since a public [Benchmark] method cannot name the
// internal BinaryTreeNode<int> (CS0050).
public class BalanceABinarySearchTreeBenchmarks
{
    private BinaryTreeNode<int> _root = null!;

    [Params(200, 2_000)]
    public int NodeCount { get; set; }

    [GlobalSetup]
    public void Setup() => _root = AscendingRightChain(NodeCount);

    // A right-only chain holding 1, 2, ..., nodeCount from the root down.
    private static BinaryTreeNode<int> AscendingRightChain(int nodeCount)
    {
        var root = new BinaryTreeNode<int>(1);
        var current = root;
        for (var value = 2; value <= nodeCount; value++)
        {
            current.Right = new BinaryTreeNode<int>(value);
            current = current.Right;
        }

        return root;
    }

    [Benchmark(Baseline = true)]
    public object? RepeatedKthSmallestScan() =>
        BalanceABinarySearchTreeSolution.BalanceByRepeatedKthSmallest(_root);

    [Benchmark]
    public object? InOrderTraversalCollectAndRebuild() =>
        BalanceABinarySearchTreeSolution.BalanceByInOrderTraversal(_root);
}
