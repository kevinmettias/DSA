using BenchmarkDotNet.Attributes;
using DSAExperimentation.Benchmarks.Fixtures;
using DSAExperimentation.DataStructures.Graph.Engines.Dags.Trees;
using DSAExperimentation.LeetCode.MaximumGoodSubtreeScore;

namespace DSAExperimentation.Benchmarks.ProblemSolutions;

// Harness only: both arms are MaximumGoodSubtreeScoreSolution's, the same methods
// MaximumGoodSubtreeScoreTests proves correct. BruteForce is exponential in subtree
// size (2^n dominated by the root), so node counts stay small enough for it to
// finish - large enough to still show BitmaskTreeFold's per-node reuse paying off.
[MemoryDiagnoser]
public class MaximumGoodSubtreeScoreBenchmarks
{
    private const int TreeSeed = 3575;

    private int[] _vals = [];

    private int[] _par = [];
    private RootedTreeNode _root = null!;
    [Params(12, 20)]
    public int NodeCount { get; set; }

    [GlobalSetup]
    public void Setup()
    {
        (_vals, _par) = GoodSubtreeWorkloads.Build(NodeCount, TreeSeed);
        _root = ParentArrayTree.Build(_par)[0];
    }

    [Benchmark(Baseline = true)]
    public int BruteForce() => MaximumGoodSubtreeScoreSolution.GoodSubtreeScoreSumByBruteForce(_vals, _par);

    [Benchmark]
    public int BitmaskTreeFold() => MaximumGoodSubtreeScoreSolution.GoodSubtreeScoreSumByBitmaskTreeFold(_root, _vals);
}
