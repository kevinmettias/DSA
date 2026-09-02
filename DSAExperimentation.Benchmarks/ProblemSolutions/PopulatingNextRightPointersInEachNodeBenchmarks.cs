using BenchmarkDotNet.Attributes;
using DSAExperimentation.Benchmarks.Fixtures;
using DSAExperimentation.DataStructures.Graph.Engines.Dags.Trees;
using DSAExperimentation.LeetCode.PopulatingNextRightPointersInEachNode;

namespace DSAExperimentation.Benchmarks.ProblemSolutions;

// Harness only: both arms are PopulatingNextRightPointersInEachNodeSolution's, the
// same methods PopulatingNextRightPointersInEachNodeTests proves correct. Fixture
// sizes are 2^k-1 so BinaryTrees.Balanced is a genuinely perfect tree, matching
// this problem's guarantee.
//
// Each arm takes .Count of the real next-pointer map rather than returning the map
// itself - BinaryTreeNode<int> is internal, and a public [Benchmark] method (a
// BenchmarkDotNet requirement) cannot return a type built over an internal one
// (CS0050), the same constraint §17.8 already resolved for WordLadderII by taking
// .Count of the real answer instead of a weaker one.
[MemoryDiagnoser]
public class PopulatingNextRightPointersInEachNodeBenchmarks
{
    [Params(63, 1_023)]
    public int NodeCount;

    private BinaryTreeNode<int> _root = null!;

    [GlobalSetup]
    public void Setup() => _root = BinaryTrees.Balanced(NodeCount);

    [Benchmark(Baseline = true)]
    public int ManualQueueBfs() => PopulatingNextRightPointersInEachNodeSolution.ConnectByManualQueueBfs(_root).Count;

    [Benchmark]
    public int LevelGroupedTraversal() =>
        PopulatingNextRightPointersInEachNodeSolution.ConnectByLevelGroupedTraversal(_root).Count;
}
