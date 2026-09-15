using BenchmarkDotNet.Attributes;
using DSAExperimentation.LeetCode.MaximumProductDifferenceBetweenTwoPairs;

namespace DSAExperimentation.Benchmarks.ProblemSolutions;

// Harness only: both arms are MaximumProductDifferenceBetweenTwoPairsSolution's,
// the same methods MaximumProductDifferenceBetweenTwoPairsTests proves correct.
// [GlobalSetup] draws the random positive array so generation is charged to setup
// rather than to the O(n^2) pair scan and the O(n log n) sort being compared.
[MemoryDiagnoser]
public class MaximumProductDifferenceBetweenTwoPairsBenchmarks
{
    private const int RandomSeed = 1913; // LC 1913 problem number
    private const int MinValueInclusive = 1;
    private const int MaxValueExclusive = 10_000;

    private int[] _values = [];

    [Params(200, 5_000)]
    public int Length { get; set; }

    [GlobalSetup]
    public void Setup()
    {
        var random = new Random(RandomSeed);
        _values = Enumerable.Range(0, Length).Select(_ => random.Next(MinValueInclusive, MaxValueExclusive)).ToArray();
    }

    [Benchmark(Baseline = true)]
    public int BruteForcePairScan() =>
        MaximumProductDifferenceBetweenTwoPairsSolution.MaxProductDifferenceByBruteForcePairScan(_values);

    [Benchmark]
    public int MergeSortExtremes() =>
        MaximumProductDifferenceBetweenTwoPairsSolution.MaxProductDifferenceByMergeSortExtremes(_values);
}
