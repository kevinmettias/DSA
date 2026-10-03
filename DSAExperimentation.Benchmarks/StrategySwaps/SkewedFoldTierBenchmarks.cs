using DSAExperimentation.Benchmarks.Fixtures;
using DSAExperimentation.DataStructures.Graph.Engines.Dags.Trees;

namespace DSAExperimentation.Benchmarks.StrategySwaps;

// FoldTierBenchmarks over a right-only chain instead of a balanced tree: recursion depth is now
// NodeCount, so every tier's per-call frame size shows up in the cost, not just its per-node
// bookkeeping. NodeCount stays well under a default thread stack's limit for the same reason
// SkewedTreeFoldBenchmarks' does - this measures the gap, not a StackOverflowException.
public class SkewedFoldTierBenchmarks
{
    private BinaryTreeNode<int> _root = null!;

    [Params(1_000, 3_000)]
    public int NodeCount { get; set; }

    [GlobalSetup]
    public void Setup() => _root = BinaryTrees.Skewed(NodeCount);

    [Benchmark(Baseline = true)]
    public int TreeTier() => FoldTierBenchmarkFixtures.TreeTier(_root);

    [Benchmark]
    public int DagTier() => FoldTierBenchmarkFixtures.DagTier(_root);

    [Benchmark]
    public int GraphTier() => FoldTierBenchmarkFixtures.GraphTier(_root);
}
