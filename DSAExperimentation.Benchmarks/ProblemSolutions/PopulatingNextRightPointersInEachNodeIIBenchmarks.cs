using BenchmarkDotNet.Attributes;
using DSAExperimentation.Benchmarks.Fixtures;
using DSAExperimentation.DataStructures.Graph.Engines.Dags.Trees;
using DSAExperimentation.LeetCode.PopulatingNextRightPointersInEachNodeII;

namespace DSAExperimentation.Benchmarks.ProblemSolutions;

// Harness only: both arms are PopulatingNextRightPointersInEachNodeIISolution's, the
// same methods PopulatingNextRightPointersInEachNodeIITests proves correct.
// BinaryTrees.Skewed (a right-only chain, one node per level) is the extreme
// non-perfect shape - proof that neither strategy needs perfect-tree-specific code
// to stay correct here.
[MemoryDiagnoser]
public class PopulatingNextRightPointersInEachNodeIIBenchmarks
{
    [Params(200, 5_000)]
    public int NodeCount;

    private BinaryTreeNode<int> _root = null!;

    [GlobalSetup]
    public void Setup() => _root = BinaryTrees.Skewed(NodeCount);

    [Benchmark(Baseline = true)]
    public int ManualQueueBfs() =>
        PopulatingNextRightPointersInEachNodeIISolution.ConnectByManualQueueBfs(_root).Count;

    [Benchmark]
    public int LevelGroupedTraversal() =>
        PopulatingNextRightPointersInEachNodeIISolution.ConnectByLevelGroupedTraversal(_root).Count;
}
