using BenchmarkDotNet.Attributes;
using DSAExperimentation.LeetCode.MaximumSubarraySumAfterAtMostKSwaps;

namespace DSAExperimentation.Benchmarks.ProblemSolutions;

// Harness only: both arms are MaximumSubarraySumAfterAtMostKSwapsSolution's,
// the same methods MaximumSubarraySumAfterAtMostKSwapsTests proves correct.
// BruteForce re-sorts each of the O(n^2) windows' inside/outside values from
// scratch (O(n^3 log n) overall), so Length stays well under LC's own n <=
// 1500 ceiling for this benchmark to finish in reasonable time.
[MemoryDiagnoser]
public class MaximumSubarraySumAfterAtMostKSwapsBenchmarks
{
    private const int Seed = 3962; private int[] _nums = [];

    private int _k;
    // LC problem number

    [Params(30, 120)]
    public int Length { get; set; }

    [GlobalSetup]
    public void Setup()
    {
        var random = new Random(Seed);
        _nums = [.. Enumerable.Range(0, Length).Select(_ => random.Next(-1000, 1001))];
        _k = Math.Max(1, Length / 3);
    }

    [Benchmark(Baseline = true)]
    public long BruteForce() => MaximumSubarraySumAfterAtMostKSwapsSolution.MaxSumByBruteForce(_nums, _k);

    [Benchmark]
    public long OrderStatisticsFenwick() =>
        MaximumSubarraySumAfterAtMostKSwapsSolution.MaxSumByOrderStatisticsFenwick(_nums, _k);
}
