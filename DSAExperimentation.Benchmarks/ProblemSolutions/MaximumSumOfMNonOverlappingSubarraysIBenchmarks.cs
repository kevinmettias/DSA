using BenchmarkDotNet.Attributes;
using DSAExperimentation.LeetCode.MaximumSumOfMNonOverlappingSubarraysI;

namespace DSAExperimentation.Benchmarks.ProblemSolutions;

// Harness only: both arms are MaximumSumOfMNonOverlappingSubarraysISolution's,
// the same methods MaximumSumOfMNonOverlappingSubarraysITests proves correct.
// m and the [l, r] window scale with Length so the O(n*m*(r-l+1)) baseline's
// length-window factor stays visible against the O(n*m) sliding-window arm.
[MemoryDiagnoser]
public class MaximumSumOfMNonOverlappingSubarraysIBenchmarks
{
    private const int Seed = 3956; // LC problem number

    [Params(100, 500)]
    public int Length;

    private int[] _nums = null!;
    private int _m;
    private int _l;
    private int _r;

    [GlobalSetup]
    public void Setup()
    {
        var random = new Random(Seed);
        _nums = [.. Enumerable.Range(0, Length).Select(_ => random.Next(-50, 51))];
        _m = Math.Max(1, Length / 2);
        _l = 1;
        _r = Math.Min(Length, 10);
    }

    [Benchmark(Baseline = true)]
    public long DynamicProgramming() =>
        MaximumSumOfMNonOverlappingSubarraysISolution.MaximumSumByDynamicProgramming(_nums, _m, _l, _r);

    [Benchmark]
    public long SlidingWindowMaximum() =>
        MaximumSumOfMNonOverlappingSubarraysISolution.MaximumSumBySlidingWindowMaximum(_nums, _m, _l, _r);
}
