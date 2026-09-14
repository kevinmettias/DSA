using BenchmarkDotNet.Attributes;
using DSAExperimentation.LeetCode.SumOfSubarrayRanges;

namespace DSAExperimentation.Benchmarks.ProblemSolutions;

// Harness only: both arms are SumOfSubarrayRangesSolution's, the same methods
// SumOfSubarrayRangesTests proves correct. The workload is a uniformly random
// signed array, so the brute-force arm's inner loop never settles early on a run
// of equal extremes and each [Params] length measures the full O(n^2) walk against
// the O(n) contribution sweep.
[MemoryDiagnoser]
public class SumOfSubarrayRangesBenchmarks
{
    private const int ValueMagnitude = 1_000;
    private const int RandomSeed = 1;

    [Params(200, 2_000)]
    public int Length;

    private int[] _values = null!;

    [GlobalSetup]
    public void Setup()
    {
        var random = new Random(RandomSeed);
        _values = Enumerable.Range(0, Length).Select(_ => random.Next(-ValueMagnitude, ValueMagnitude)).ToArray();
    }

    [Benchmark(Baseline = true)]
    public long BruteForce() => SumOfSubarrayRangesSolution.SubarrayRangesByBruteForce(_values);

    [Benchmark]
    public long MonotonicStack() => SumOfSubarrayRangesSolution.SubarrayRangesByMonotonicStack(_values);
}
