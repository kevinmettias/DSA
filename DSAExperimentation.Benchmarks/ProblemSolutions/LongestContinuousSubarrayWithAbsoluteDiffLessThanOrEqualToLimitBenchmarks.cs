using BenchmarkDotNet.Attributes;
using DSAExperimentation.LeetCode.LongestContinuousSubarrayWithAbsoluteDiffLessThanOrEqualToLimit;

namespace DSAExperimentation.Benchmarks.ProblemSolutions;

// Harness only: both arms are
// LongestContinuousSubarrayWithAbsoluteDiffLessThanOrEqualToLimitSolution's, the
// same methods the coverage test proves correct. Values are random over a narrow
// range relative to Limit so windows run long enough for the baseline's O(n^2) cost
// to actually show, rather than every start immediately violating the limit.
[MemoryDiagnoser]
public class LongestContinuousSubarrayWithAbsoluteDiffLessThanOrEqualToLimitBenchmarks
{
    private const int Limit = 100;
    private const int ValueUpperBound = 2_000;
    private const int RandomSeed = 1;

    [Params(500, 4_000)]
    public int Length;

    private int[] _values = null!;

    [GlobalSetup]
    public void Setup()
    {
        var random = new Random(RandomSeed);

        _values = [.. Enumerable.Range(0, Length).Select(_ => random.Next(0, ValueUpperBound))];
    }

    [Benchmark(Baseline = true)]
    public int BruteForceAllStartingPoints() =>
        LongestContinuousSubarrayWithAbsoluteDiffLessThanOrEqualToLimitSolution
            .LongestSubarrayByBruteForceWindows(_values, Limit);

    [Benchmark]
    public int DoubleMonotonicDeque() =>
        LongestContinuousSubarrayWithAbsoluteDiffLessThanOrEqualToLimitSolution
            .LongestSubarrayByMonotonicDeques(_values, Limit);
}
