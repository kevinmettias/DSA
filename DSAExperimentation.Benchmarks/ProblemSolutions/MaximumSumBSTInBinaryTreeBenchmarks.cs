using BenchmarkDotNet.Attributes;
using DSAExperimentation.Benchmarks.Fixtures;
using DSAExperimentation.DataStructures.Graph.Engines.Dags.Trees;
using DSAExperimentation.LeetCode.MaximumSumBSTInBinaryTree;

namespace DSAExperimentation.Benchmarks.ProblemSolutions;

// Harness only: both arms are MaximumSumBSTInBinaryTreeSolution's, the same methods
// MaximumSumBSTInBinaryTreeTests proves correct. The baseline revalidates and re-sums
// each node's whole subtree independently (O(n) work at every one of n nodes, so
// O(n^2)) against the single bottom-up pass (O(n)). Fixtures.BinaryTrees.Skewed's
// strictly increasing right-only chain is itself a valid BST end to end, the same
// reason DiameterOfBinaryTreeBenchmarks reuses a skewed shape - it keeps the naive
// baseline's cost real instead of hidden behind O(log n) depth.
[MemoryDiagnoser]
public class MaximumSumBSTInBinaryTreeBenchmarks
{
    [Params(200, 2_000)]
    public int NodeCount;

    private BinaryTreeNode<int> _root = null!;

    [GlobalSetup]
    public void Setup() => _root = BinaryTrees.Skewed(NodeCount);

    [Benchmark(Baseline = true)]
    public int RevalidatePerNode() =>
        MaximumSumBSTInBinaryTreeSolution.MaxSumBSTByRevalidatingEachNode(_root);

    [Benchmark]
    public int OnePassBottomUpScan() =>
        MaximumSumBSTInBinaryTreeSolution.MaxSumBSTByBottomUpScan(_root);
}
