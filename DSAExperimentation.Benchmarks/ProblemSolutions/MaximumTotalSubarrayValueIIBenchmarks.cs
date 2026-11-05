using DSAExperimentation.Benchmarks.Fixtures;
using DSAExperimentation.Domain.Mathematics.Algebra;
using DSAExperimentation.DataStructures.SegmentTree;
using DSAExperimentation.LeetCode.Solutions.MaximumTotalSubarrayValueII;

namespace DSAExperimentation.Benchmarks.ProblemSolutions;

// Harness only: both arms are MaximumTotalSubarrayValueIISolution's, the same
// methods CatalogCaseTests proves correct. The composed arm is
// handed its two prebuilt SegmentTrees so tree construction is charged to
// [GlobalSetup] rather than to the search being measured, mirroring
// OpenTheLockBenchmarks' lock-graph hoist.
public class MaximumTotalSubarrayValueIIBenchmarks
{
    private const int Seed = 3691;
    private const int K = 2_000;

    private int[] _nums = [];

    private SegmentTree<int, Maximum<int>> _maxTree = null!;
    private SegmentTree<int, Minimum<int>> _minTree = null!;
    [Params(100, 500)]
    public int Size { get; set; }

    [GlobalSetup]
    public void Setup()
    {
        _nums = MaximumTotalSubarrayValueIIWorkloads.BuildNums(Size, Seed);
        _maxTree = new SegmentTree<int, Maximum<int>>(_nums);
        _minTree = new SegmentTree<int, Minimum<int>>(_nums);
    }

    [Benchmark(Baseline = true)]
    public long BruteForce() => MaximumTotalSubarrayValueIISolution.MaxTotalValueByBruteForce(_nums, K);

    [Benchmark]
    public long SegmentTreeHeap() =>
        MaximumTotalSubarrayValueIISolution.MaxTotalValueBySegmentTreeHeap(_maxTree, _minTree, K);
}
