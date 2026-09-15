using BenchmarkDotNet.Attributes;
using DSAExperimentation.Algorithms.Folding;
using DSAExperimentation.Benchmarks.Fixtures;
using DSAExperimentation.DataStructures.Graph.Engines.Dags.Trees;

namespace DSAExperimentation.Benchmarks.StrategySwaps;

// Same fold, same algebra as BalancedTreeFoldBenchmarks, but over a degenerate
// right-only chain - the exact case TreeFold's own doc comment warns about
// ("safe to override TStrategy with IterativeFoldEvaluation only when TAlgebra is
// pure") and IterativeFoldEvaluation's doc comment names as its reason to exist:
// RecursiveFoldEvaluation's call stack grows with NodeCount here, not log(NodeCount).
// NodeCount is kept well under a default 1MB thread stack's limit deliberately -
// this demonstrates the cost gap, not a StackOverflowException (uncatchable, would
// crash the whole benchmark run).
[MemoryDiagnoser]
public class SkewedTreeFoldBenchmarks
{
    private BinaryTreeNode<int> _root = null!;

    [Params(1_000, 3_000)]
    public int NodeCount { get; set; }

    [GlobalSetup]
    public void Setup() => _root = BinaryTrees.Skewed(NodeCount);

    [Benchmark(Baseline = true)]
    public int Recursive() => TreeFoldBenchmarkHelpers.Fold<RecursiveFoldEvaluation<BinaryTreeNode<int>>>(_root);

    [Benchmark]
    public int Iterative() => TreeFoldBenchmarkHelpers.Fold<IterativeFoldEvaluation<BinaryTreeNode<int>>>(_root);
}
