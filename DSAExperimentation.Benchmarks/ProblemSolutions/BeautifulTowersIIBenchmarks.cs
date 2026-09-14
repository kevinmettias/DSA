using BenchmarkDotNet.Attributes;
using DSAExperimentation.LeetCode.BeautifulTowersII;

namespace DSAExperimentation.Benchmarks.ProblemSolutions;

// Harness only: both arms are BeautifulTowersIISolution's, the same methods
// BeautifulTowersIITests proves correct. Identical algorithm to Beautiful Towers I
// (see BeautifulTowersIBenchmarks) at [Params] scaled up toward II's much larger
// official n <= 1e5 bound - large enough that the baseline's O(n^2) cost visibly
// dominates while staying inside a reasonable benchmark run.
[MemoryDiagnoser]
public class BeautifulTowersIIBenchmarks
{
    private const int MaxHeight = 1_000_000_000;

    [Params(1_000, 8_000)]
    public int Length;

    private int[] _maxHeights = null!;

    [GlobalSetup]
    public void Setup()
    {
        var random = new Random(1);
        _maxHeights = Enumerable.Range(0, Length).Select(_ => random.Next(1, MaxHeight)).ToArray();
    }

    [Benchmark(Baseline = true)]
    public long BruteForce() => BeautifulTowersIISolution.MaximumSumOfHeightsByBruteForce(_maxHeights);

    [Benchmark]
    public long MonotonicStack() => BeautifulTowersIISolution.MaximumSumOfHeightsByMonotonicStack(_maxHeights);
}
