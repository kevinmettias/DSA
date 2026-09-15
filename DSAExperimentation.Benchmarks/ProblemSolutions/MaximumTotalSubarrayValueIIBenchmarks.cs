using BenchmarkDotNet.Attributes;
using DSAExperimentation.Benchmarks.Fixtures;
using DSAExperimentation.DataStructures.SegmentTree;
using DSAExperimentation.LeetCode.MaximumTotalSubarrayValueII;

namespace DSAExperimentation.Benchmarks.ProblemSolutions;

// Harness only: both arms are MaximumTotalSubarrayValueIISolution's, the same
// methods MaximumTotalSubarrayValueIITests proves correct. The composed arm is
// handed its two prebuilt SegmentTrees so tree construction is charged to
// [GlobalSetup] rather than to the search being measured, mirroring
// OpenTheLockBenchmarks' LockGraph hoist.
[MemoryDiagnoser]
public class MaximumTotalSubarrayValueIIBenchmarks
{
    private const int Seed = 3691;
    private const int K = 2_000;

    private int[] _nums = [];

    private SegmentTree<int, MaxOperation<int>> _maxTree = null!;
    private SegmentTree<int, MinOperation<int>> _minTree = null!;
    [Params(100, 500)]
    public int Size { get; set; }

    [GlobalSetup]
    public void Setup()
    {
        _nums = MaximumTotalSubarrayValueIIWorkloads.BuildNums(Size, Seed);
        _maxTree = new SegmentTree<int, MaxOperation<int>>(_nums);
        _minTree = new SegmentTree<int, MinOperation<int>>(_nums);
    }

    [Benchmark(Baseline = true)]
    public long BruteForce() => MaximumTotalSubarrayValueIISolution.MaxTotalValueByBruteForce(_nums, K);

    [Benchmark]
    public long SegmentTreeHeap() =>
        MaximumTotalSubarrayValueIISolution.MaxTotalValueBySegmentTreeHeap(_maxTree, _minTree, K);
}
