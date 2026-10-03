using DSAExperimentation.Benchmarks.Fixtures;
using DSAExperimentation.LeetCode.SumOfSubarrayRanges;

namespace DSAExperimentation.Benchmarks.ProblemSolutions;

// Harness only: both arms are SumOfSubarrayRangesSolution's, the same methods
// SumOfSubarrayRangesSolutionTests proves correct. The workload is a uniformly random
// signed array, so the brute-force arm's inner loop never settles early on a run
// of equal extremes and each [Params] length measures the full O(n^2) walk against
// the O(n) contribution sweep. Length stops at LC 2104's 1,000.
public class SumOfSubarrayRangesBenchmarks
{
    private const int ValueMagnitude = 1_000;
    private const int RandomSeed = 1;

    private int[] _values = [];

    [Params(200, 1_000)]
    public int Length { get; set; }

    [GlobalSetup]
    public void Setup()
    {
        var random = new Random(RandomSeed);
        _values = SeededDraws.Values(Length, -ValueMagnitude, ValueMagnitude, random);
    }

    [Benchmark(Baseline = true)]
    public long BruteForce() => SumOfSubarrayRangesSolution.SubarrayRangesByBruteForce(_values);

    [Benchmark]
    public long MonotonicStack() => SumOfSubarrayRangesSolution.SubarrayRangesByMonotonicStack(_values);
}
