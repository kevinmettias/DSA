using BenchmarkDotNet.Attributes;
using DSAExperimentation.Benchmarks.Fixtures;
using DSAExperimentation.DataStructures.Graph.Engines.Dags.Trees;
using DSAExperimentation.LeetCode.SubtreeOfAnotherTree;

namespace DSAExperimentation.Benchmarks.ProblemSolutions;

// Harness only: both arms are SubtreeOfAnotherTreeSolution's, the same methods
// SubtreeOfAnotherTreeTests proves correct. Both trees are left-skewed chains
// sharing the same filler value so the naive approach can't short-circuit on an
// early mismatch - it has to walk deep into every candidate start before
// failing - and subRoot's deepest node carries a value that never appears in
// root, so neither strategy ever finds a real match and both run to
// completion.
[MemoryDiagnoser]
public class SubtreeOfAnotherTreeBenchmarks
{
    [Params(200, 2_000)]
    public int NodeCount;

    private BinaryTreeNode<int> _root = null!;
    private BinaryTreeNode<int> _subRoot = null!;

    [GlobalSetup]
    public void Setup()
    {
        _root = SubtreeOfAnotherTreeWorkloads.BuildLeftChain(NodeCount, lastValue: 1);
        _subRoot = SubtreeOfAnotherTreeWorkloads.BuildLeftChain(
            NodeCount / SubtreeOfAnotherTreeWorkloads.SubRootSizeDivisor, lastValue: -1);
    }

    [Benchmark(Baseline = true)]
    public bool RecursiveCompareAtEveryNode() =>
        SubtreeOfAnotherTreeSolution.IsSubtreeByRecursiveCompareAtEveryNode(_root, _subRoot);

    [Benchmark]
    public bool SerializeThenKmpSearch() =>
        SubtreeOfAnotherTreeSolution.IsSubtreeBySerializeThenKmpSearch(_root, _subRoot);
}
