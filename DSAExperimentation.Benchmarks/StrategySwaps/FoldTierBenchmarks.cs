using DSAExperimentation.Benchmarks.Fixtures;
using DSAExperimentation.DataStructures.Graph.Engines.Dags.Trees;

namespace DSAExperimentation.Benchmarks.StrategySwaps;

// What each fold tier's defence costs on a balanced tree, where recursion depth is O(log n): the
// README's claim that a fold "pays only for the defence its tier doesn't already rule out", made a
// number. TreeTier is the baseline because it pays for nothing; DagTier adds a memo per node and
// GraphTier adds an in-progress set on top. See SkewedFoldTierBenchmarks for the deep-chain case.
public class FoldTierBenchmarks
{
    private BinaryTreeNode<int> _root = null!;

    [Params(10_000, 200_000)]
    public int NodeCount { get; set; }

    [GlobalSetup]
    public void Setup() => _root = BinaryTrees.Balanced(NodeCount);

    [Benchmark(Baseline = true)]
    public int TreeTier() => FoldTierBenchmarkFixtures.TreeTier(_root);

    [Benchmark]
    public int DagTier() => FoldTierBenchmarkFixtures.DagTier(_root);

    [Benchmark]
    public int GraphTier() => FoldTierBenchmarkFixtures.GraphTier(_root);
}
