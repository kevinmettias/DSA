using DSAExperimentation.Benchmarks.Fixtures;
using DSAExperimentation.LeetCode.ContinuousSubarrays;

namespace DSAExperimentation.Benchmarks.ProblemSolutions;

// Harness only: both arms are ContinuousSubarraysSolution's, the same methods
// ContinuousSubarraysSolutionTests proves correct - the O(n^2) rescan from every starting
// index against the two monotonic Deque<int> windows at O(n) total, the same contrast
// LongestContinuousSubarrayWithAbsoluteDiffLessThanOrEqualToLimitBenchmarks draws for
// LC 1438. [GlobalSetup] draws values from a range narrow enough relative to the
// problem's fixed limit of 2 that windows run long, so the baseline's quadratic cost
// actually shows instead of every start immediately violating the limit.
public class ContinuousSubarraysBenchmarks
{
    private const int Seed = 1;
    private const int ValueUpperBound = 40;

    private int[] _values = [];

    [Params(500, 4_000)]
    public int Length { get; set; }

    [GlobalSetup]
    public void Setup()
    {
        var random = new Random(Seed);
        // LC 2762's values start at 1; the draw spans the same 40 values either way.
        _values = SeededDraws.Values(Length, 1, ValueUpperBound + 1, random);
    }

    [Benchmark(Baseline = true)]
    public long BruteForceAllStartingPoints() =>
        ContinuousSubarraysSolution.CountContinuousSubarraysByBruteForceWindows(_values);

    [Benchmark]
    public long DoubleMonotonicDeque() =>
        ContinuousSubarraysSolution.CountContinuousSubarraysByMonotonicDeques(_values);
}
