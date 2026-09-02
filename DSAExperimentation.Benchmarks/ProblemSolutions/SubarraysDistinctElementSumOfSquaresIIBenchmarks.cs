using BenchmarkDotNet.Attributes;
using DSAExperimentation.LeetCode.SubarraysDistinctElementSumOfSquaresII;

namespace DSAExperimentation.Benchmarks.ProblemSolutions;

// Harness only: both arms are SubarraysDistinctElementSumOfSquaresIISolution's, the
// same methods SubarraysDistinctElementSumOfSquaresIITests proves correct. Values
// are drawn from a small bound so distinct-count churn (and therefore the Fenwick
// tree's range-update work) stays high across the whole array, the case that best
// separates the O(n^2) brute force from the O(n log n) range-Fenwick sweep.
[MemoryDiagnoser]
public class SubarraysDistinctElementSumOfSquaresIIBenchmarks
{
    private const int RandomSeed = 2916; // LeetCode problem number
    private const int MaxValueExclusive = 50;

    [Params(200, 2_000)]
    public int Length;

    private int[] _nums = null!;

    [GlobalSetup]
    public void Setup()
    {
        var random = new Random(RandomSeed);
        _nums = Enumerable.Range(0, Length).Select(_ => random.Next(1, MaxValueExclusive)).ToArray();
    }

    [Benchmark(Baseline = true)]
    public long BruteForce() => SubarraysDistinctElementSumOfSquaresIISolution.SumOfSquaresByBruteForce(_nums);

    [Benchmark]
    public long RangeFenwickTreeSweep() => SubarraysDistinctElementSumOfSquaresIISolution.SumOfSquaresByRangeFenwickTree(_nums);
}
