using BenchmarkDotNet.Attributes;
using DSAExperimentation.LeetCode.FindTheCountOfMonotonicPairsII;

namespace DSAExperimentation.Benchmarks.ProblemSolutions;

// Harness only: both arms are FindTheCountOfMonotonicPairsIISolution's, the same
// methods FindTheCountOfMonotonicPairsIITests proves correct. nums[i] is pushed
// to Part II's own constraint ceiling (<= 1000, twenty times Part I's) - exactly
// the regime where the brute-force row's extra maxValue factor stops being free
// and the prefix-sum arm's advantage should show. Length stays well under both
// parts' shared n <= 2000 cap so the O(n * maxValue^2) baseline still finishes
// in reasonable benchmark time.
[MemoryDiagnoser]
public class FindTheCountOfMonotonicPairsIIBenchmarks
{
    private const int MaxValue = 1_000;
    private const int Seed = 3251;

    [Params(100, 500)]
    public int Length;

    private int[] _nums = null!;

    [GlobalSetup]
    public void Setup()
    {
        var random = new Random(Seed);
        _nums = Enumerable.Range(0, Length).Select(_ => random.Next(1, MaxValue + 1)).ToArray();
    }

    [Benchmark(Baseline = true)]
    public long BruteForceDP() => FindTheCountOfMonotonicPairsIISolution.CountPairsByBruteForceDP(_nums);

    [Benchmark]
    public long PrefixSumDP() => FindTheCountOfMonotonicPairsIISolution.CountPairsByPrefixSumDP(_nums);
}
