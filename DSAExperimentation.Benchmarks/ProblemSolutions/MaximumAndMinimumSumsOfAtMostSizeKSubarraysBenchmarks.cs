using BenchmarkDotNet.Attributes;
using DSAExperimentation.LeetCode.MaximumAndMinimumSumsOfAtMostSizeKSubarrays;

namespace DSAExperimentation.Benchmarks.ProblemSolutions;

// Harness only: both arms are MaximumAndMinimumSumsOfAtMostSizeKSubarraysSolution's,
// the same methods MaximumAndMinimumSumsOfAtMostSizeKSubarraysTests proves
// correct. K is pinned to the full array length, the constraint's own upper
// bound, so SumByBruteForceWindow pays its worst-case O(n*k) = O(n^2) while
// SumByMonotonicStackContribution stays O(n) - the gap the closed-form
// contribution counting exists to open.
[MemoryDiagnoser]
public class MaximumAndMinimumSumsOfAtMostSizeKSubarraysBenchmarks
{
    private const int Seed = 3430;

    private int[] _nums = [];

    [Params(200, 2_000)]
    public int Length { get; set; }

    [GlobalSetup]
    public void Setup()
    {
        var random = new Random(Seed);
        _nums = [.. Enumerable.Range(0, Length).Select(_ => random.Next(-1_000_000, 1_000_000))];
    }

    [Benchmark(Baseline = true)]
    public long BruteForceWindow() =>
        MaximumAndMinimumSumsOfAtMostSizeKSubarraysSolution.SumByBruteForceWindow(_nums, Length);

    [Benchmark]
    public long MonotonicStackContribution() =>
        MaximumAndMinimumSumsOfAtMostSizeKSubarraysSolution.SumByMonotonicStackContribution(_nums, Length);
}
