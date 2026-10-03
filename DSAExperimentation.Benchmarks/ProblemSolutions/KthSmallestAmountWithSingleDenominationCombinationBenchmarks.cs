using DSAExperimentation.Benchmarks.Fixtures;
using DSAExperimentation.LeetCode.KthSmallestAmountWithSingleDenominationCombination;

namespace DSAExperimentation.Benchmarks.ProblemSolutions;

// Harness only: both arms are
// KthSmallestAmountWithSingleDenominationCombinationSolution's, the same
// methods KthSmallestAmountWithSingleDenominationCombinationSolutionTests proves
// correct. Coins stay fixed (this problem caps coins.Length at 15 regardless
// of rank); Rank is the axis that grows, so the heap merge's
// O(rank log coins.Length) cost is what the inclusion-exclusion search - whose
// own cost is independent of rank - has to be measured against.
//
// The coins are the first CoinCount values of a seeded shuffle of 1..25, LC 3116's
// coin range, so they are pairwise distinct as the problem promises.
public class KthSmallestAmountWithSingleDenominationCombinationBenchmarks
{
    private const int Seed = 3116; // LC problem number
    private const int CoinCount = 8;
    private const int MaxCoinValue = 25;

    private int[] _coins = [];

    [Params(200, 5_000)]
    public int Rank { get; set; }

    [GlobalSetup]
    public void Setup() => _coins = SeededSequences.ShuffledOneTo(MaxCoinValue, Seed)[..CoinCount];

    [Benchmark(Baseline = true)]
    public long HeapMerge() =>
        KthSmallestAmountWithSingleDenominationCombinationSolution.KthSmallestAmountByHeapMerge(_coins, Rank);

    [Benchmark]
    public long InclusionExclusionSearch() =>
        KthSmallestAmountWithSingleDenominationCombinationSolution.KthSmallestAmountByInclusionExclusionSearch(
            _coins, Rank);
}
