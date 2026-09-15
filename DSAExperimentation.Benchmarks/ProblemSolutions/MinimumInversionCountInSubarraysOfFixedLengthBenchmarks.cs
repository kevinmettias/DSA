using BenchmarkDotNet.Attributes;
using DSAExperimentation.LeetCode.MinimumInversionCountInSubarraysOfFixedLength;

namespace DSAExperimentation.Benchmarks.ProblemSolutions;

// Harness only: both arms are
// MinimumInversionCountInSubarraysOfFixedLengthSolution's, the same methods
// MinimumInversionCountInSubarraysOfFixedLengthTests proves correct.
//
// WindowLength stays fixed and small so the O(n * k^2) brute force still
// finishes at both Length values; the Fenwick sliding window is O(n log n)
// regardless of k.
[MemoryDiagnoser]
public class MinimumInversionCountInSubarraysOfFixedLengthBenchmarks
{
    private const int RandomSeed = 3768; // LC problem number
    private const int WindowLength = 50;
    private const int MaxValue = 1_000_000;

    private int[] _nums = [];

    [Params(500, 5_000)]
    public int Length { get; set; }

    [GlobalSetup]
    public void Setup()
    {
        var random = new Random(RandomSeed);
        _nums = Enumerable.Range(0, Length).Select(_ => random.Next(1, MaxValue + 1)).ToArray();
    }

    [Benchmark(Baseline = true)]
    public long BruteForce() =>
        MinimumInversionCountInSubarraysOfFixedLengthSolution.MinInversionCountByBruteForce(_nums, WindowLength);

    [Benchmark]
    public long SlidingWindowFenwick() =>
        MinimumInversionCountInSubarraysOfFixedLengthSolution.MinInversionCountBySlidingWindowFenwick(_nums, WindowLength);
}
