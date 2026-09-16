using BenchmarkDotNet.Attributes;
using DSAExperimentation.Algorithms.Folding;
using DSAExperimentation.Algorithms.Folding.Dags.Trees;
using DSAExperimentation.Algorithms.Metrics;
using DSAExperimentation.Benchmarks.Fixtures;
using DSAExperimentation.DataStructures.Graph.Engines.Dags.Trees;
using DSAExperimentation.DataStructures.Graph.Contracts.Ordering;

namespace DSAExperimentation.Benchmarks.StrategySwaps;

// Compares this repo's own IFoldEvaluationStrategy implementations
// (RecursiveFoldEvaluation vs IterativeFoldEvaluation) on a balanced tree, where
// recursion depth is only O(log n) - the easy case, where the two should perform
// comparably. See SkewedTreeFoldBenchmarks for the case IterativeFoldEvaluation's
// own doc comment names as its reason to exist. Same result either way: SizeAlgebra
// is pure, per IFoldAlgebra's purity caveat.
[MemoryDiagnoser]
public class BalancedTreeFoldBenchmarks
{
    private BinaryTreeNode<int> _root = null!;

    [Params(10_000, 200_000)]
    public int NodeCount { get; set; }

    [GlobalSetup]
    public void Setup() => _root = BinaryTrees.Balanced(NodeCount);

    [Benchmark(Baseline = true)]
    public int Recursive() => TreeFoldBenchmarkFixtures.Fold<RecursiveFoldEvaluation<BinaryTreeNode<int>>>(_root);

    [Benchmark]
    public int Iterative() => TreeFoldBenchmarkFixtures.Fold<IterativeFoldEvaluation<BinaryTreeNode<int>>>(_root);
}
