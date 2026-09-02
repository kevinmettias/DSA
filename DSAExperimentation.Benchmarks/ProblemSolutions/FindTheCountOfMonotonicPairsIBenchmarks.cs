using BenchmarkDotNet.Attributes;
using DSAExperimentation.LeetCode.FindTheCountOfMonotonicPairsI;

namespace DSAExperimentation.Benchmarks.ProblemSolutions;

// Harness only: both arms are FindTheCountOfMonotonicPairsISolution's, the same
// methods FindTheCountOfMonotonicPairsITests proves correct. nums[i] stays
// within Part I's own constraint (<= 50), the range where the O(n * maxValue^2)
// baseline is still meant to be viable.
[MemoryDiagnoser]
public class FindTheCountOfMonotonicPairsIBenchmarks
{
    private const int MaxValue = 50;
    private const int Seed = 3250;

    [Params(500, 2_000)]
    public int Length;

    private int[] _nums = null!;

    [GlobalSetup]
    public void Setup()
    {
        var random = new Random(Seed);
        _nums = Enumerable.Range(0, Length).Select(_ => random.Next(1, MaxValue + 1)).ToArray();
    }

    [Benchmark(Baseline = true)]
    public long BruteForceDP() => FindTheCountOfMonotonicPairsISolution.CountPairsByBruteForceDP(_nums);

    [Benchmark]
    public long PrefixSumDP() => FindTheCountOfMonotonicPairsISolution.CountPairsByPrefixSumDP(_nums);
}
