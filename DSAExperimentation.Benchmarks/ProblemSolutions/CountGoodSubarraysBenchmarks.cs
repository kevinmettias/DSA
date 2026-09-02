using BenchmarkDotNet.Attributes;
using DSAExperimentation.LeetCode.CountGoodSubarrays;

namespace DSAExperimentation.Benchmarks.ProblemSolutions;

// Harness only: both arms are CountGoodSubarraysSolution's, the same methods
// CountGoodSubarraysTests proves correct. Values are drawn from a wide range so
// the running-OR run list actually grows across several bit widths instead of
// collapsing to a single run immediately.
[MemoryDiagnoser]
public class CountGoodSubarraysBenchmarks
{
    private const int Seed = 3878; // LC problem number
    private const int MaxValueExclusive = 1 << 20;

    [Params(200, 5_000)]
    public int Length;

    private int[] _nums = null!;

    [GlobalSetup]
    public void Setup()
    {
        var random = new Random(Seed);
        _nums = Enumerable.Range(0, Length).Select(_ => random.Next(0, MaxValueExclusive)).ToArray();
    }

    [Benchmark(Baseline = true)]
    public long BruteForce() => CountGoodSubarraysSolution.CountGoodSubarraysByBruteForce(_nums);

    [Benchmark]
    public long RunningOrGroups() => CountGoodSubarraysSolution.CountGoodSubarraysByRunningOrGroups(_nums);
}
