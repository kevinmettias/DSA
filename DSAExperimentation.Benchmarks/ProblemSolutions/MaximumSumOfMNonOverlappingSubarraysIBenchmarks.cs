using BenchmarkDotNet.Attributes;
using DSAExperimentation.LeetCode.MaximumSumOfMNonOverlappingSubarraysI;

namespace DSAExperimentation.Benchmarks.ProblemSolutions;

// Harness only: both arms are MaximumSumOfMNonOverlappingSubarraysISolution's,
// the same methods MaximumSumOfMNonOverlappingSubarraysITests proves correct.
// maxSubarrays and the [minLength, maxLength] window scale with Length so the
// O(n*m*(r-l+1)) baseline's length-window factor stays visible against the O(n*m)
// sliding-window arm.
[MemoryDiagnoser]
public class MaximumSumOfMNonOverlappingSubarraysIBenchmarks
{
    private const int Seed = 3956; private int[] _nums = [];

    private int _maxSubarrays;
    private int _minLength;
    private int _maxLength;
    // LC problem number

    [Params(100, 500)]
    public int Length { get; set; }

    [GlobalSetup]
    public void Setup()
    {
        var random = new Random(Seed);
        _nums = [.. Enumerable.Range(0, Length).Select(_ => random.Next(-50, 51))];
        _maxSubarrays = Math.Max(1, Length / 2);
        _minLength = 1;
        _maxLength = Math.Min(Length, 10);
    }

    [Benchmark(Baseline = true)]
    public long DynamicProgramming() =>
        MaximumSumOfMNonOverlappingSubarraysISolution.MaximumSumByDynamicProgramming(
            _nums, _maxSubarrays, _minLength, _maxLength);

    [Benchmark]
    public long SlidingWindowMaximum() =>
        MaximumSumOfMNonOverlappingSubarraysISolution.MaximumSumBySlidingWindowMaximum(
            _nums, _maxSubarrays, _minLength, _maxLength);
}
