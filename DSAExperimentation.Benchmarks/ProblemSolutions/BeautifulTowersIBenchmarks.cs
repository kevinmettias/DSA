using BenchmarkDotNet.Attributes;
using DSAExperimentation.LeetCode.BeautifulTowersI;

namespace DSAExperimentation.Benchmarks.ProblemSolutions;

// Harness only: both arms are BeautifulTowersISolution's, the same methods
// BeautifulTowersITests proves correct - the O(n^2) per-peak clamped walk baseline
// vs. the O(n) monotonic-stack sweep over this repo's own Stack<int>. [Params]
// stays at LC 2865's own n <= 1000 constraint.
[MemoryDiagnoser]
public class BeautifulTowersIBenchmarks
{
    private const int MaxHeight = 1_000_000_000;

    private int[] _maxHeights = [];

    [Params(200, 1_000)]
    public int Length { get; set; }

    [GlobalSetup]
    public void Setup()
    {
        var random = new Random(1);
        _maxHeights = Enumerable.Range(0, Length).Select(_ => random.Next(1, MaxHeight)).ToArray();
    }

    [Benchmark(Baseline = true)]
    public long BruteForce() => BeautifulTowersISolution.MaximumSumOfHeightsByBruteForce(_maxHeights);

    [Benchmark]
    public long MonotonicStack() => BeautifulTowersISolution.MaximumSumOfHeightsByMonotonicStack(_maxHeights);
}
