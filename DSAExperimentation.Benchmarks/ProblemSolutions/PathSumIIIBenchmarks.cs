using BenchmarkDotNet.Attributes;
using DSAExperimentation.Benchmarks.Fixtures;
using DSAExperimentation.DataStructures.Graph.Engines.Dags.Trees;
using DSAExperimentation.LeetCode.PathSumIII;

namespace DSAExperimentation.Benchmarks.ProblemSolutions;

// Harness only: both arms are PathSumIIISolution's, the same methods
// PathSumIIITests proves correct. _root is BinaryTrees.Balanced, whose node
// values are all non-negative, so Target is deliberately unreachable - both
// strategies are forced through their full traversal instead of an early match.
[MemoryDiagnoser]
public class PathSumIIIBenchmarks
{
    private const int Target = -1;

    private BinaryTreeNode<int> _root = null!;

    [Params(200, 5_000)]
    public int NodeCount { get; set; }

    [GlobalSetup]
    public void Setup() => _root = BinaryTrees.Balanced(NodeCount);

    [Benchmark(Baseline = true)]
    public int DoubleDfs() => PathSumIIISolution.PathSumByDoubleDfs(_root, Target);

    [Benchmark]
    public int PrefixSumHashMap() => PathSumIIISolution.PathSumByPrefixSumHashMap(_root, Target);
}
