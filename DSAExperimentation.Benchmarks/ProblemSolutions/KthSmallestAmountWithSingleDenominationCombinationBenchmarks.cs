using BenchmarkDotNet.Attributes;
using DSAExperimentation.LeetCode.KthSmallestAmountWithSingleDenominationCombination;

namespace DSAExperimentation.Benchmarks.ProblemSolutions;

// Harness only: both arms are
// KthSmallestAmountWithSingleDenominationCombinationSolution's, the same
// methods KthSmallestAmountWithSingleDenominationCombinationTests proves
// correct. Coins stay fixed (this problem caps coins.Length at 15 regardless
// of k); K is the axis that grows, so the heap merge's O(k log n) cost is
// what the inclusion-exclusion search - whose own cost is independent of k -
// has to be measured against.
[MemoryDiagnoser]
public class KthSmallestAmountWithSingleDenominationCombinationBenchmarks
{
    private const int Seed = 3116; // LC problem number
    private const int CoinCount = 8;
    private const int MaxCoinValueExclusive = 26;

    [Params(200, 5_000)]
    public int K;

    private int[] _coins = null!;

    [GlobalSetup]
    public void Setup()
    {
        var random = new Random(Seed);
        _coins = Enumerable.Range(0, CoinCount).Select(_ => random.Next(1, MaxCoinValueExclusive)).ToArray();
    }

    [Benchmark(Baseline = true)]
    public long HeapMerge() =>
        KthSmallestAmountWithSingleDenominationCombinationSolution.KthSmallestAmountByHeapMerge(_coins, K);

    [Benchmark]
    public long InclusionExclusionSearch() =>
        KthSmallestAmountWithSingleDenominationCombinationSolution.KthSmallestAmountByInclusionExclusionSearch(
            _coins, K);
}
