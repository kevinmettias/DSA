using BenchmarkDotNet.Attributes;
using DSAExperimentation.LeetCode.CountSubarraysWithFixedBounds;

namespace DSAExperimentation.Benchmarks.ProblemSolutions;

// Harness only: both arms are CountSubarraysWithFixedBoundsSolution's, the same
// methods CountSubarraysWithFixedBoundsTests proves correct. The baseline rescans
// each subarray from scratch to recompute its own min/max, the composed arm builds
// this repo's Min/Max SegmentTree pair once and answers each subarray in O(log n) -
// so the SegmentTree build stays inside the measured arm deliberately, since paying
// for it is exactly what that strategy is trading against the rescan.
[MemoryDiagnoser]
public class CountSubarraysWithFixedBoundsBenchmarks
{
    private const int RandomSeed = 2444; // LC problem number
    private const int MinK = 2;
    private const int MaxK = 8;
    private const int MaxValueExclusive = 10;

    [Params(20, 100)]
    public int Length;

    private int[] _nums = null!;

    [GlobalSetup]
    public void Setup()
    {
        var random = new Random(RandomSeed);
        _nums = Enumerable.Range(0, Length).Select(_ => random.Next(1, MaxValueExclusive)).ToArray();
    }

    [Benchmark(Baseline = true)]
    public long RescanEachSubarray() =>
        CountSubarraysWithFixedBoundsSolution.CountFixedBoundSubarraysByRescan(_nums, MinK, MaxK);

    [Benchmark]
    public long SegmentTreeRangeQueries() =>
        CountSubarraysWithFixedBoundsSolution.CountFixedBoundSubarraysBySegmentTreeQueries(_nums, MinK, MaxK);
}
