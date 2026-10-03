using DSAExperimentation.Benchmarks.Fixtures;
using DSAExperimentation.LeetCode.BestTimeToBuyAndSellStockV;

namespace DSAExperimentation.Benchmarks.ProblemSolutions;

// Harness only: both arms are BestTimeToBuyAndSellStockVSolution's, the same
// methods BestTimeToBuyAndSellStockVSolutionTests proves correct.
//
// Sizes are per arm. Brute force is a genuine, unmemoized 3-way choice tree per day,
// so it stops at 14 days (FindTheNumberOfSubsequencesWithEqualGcdBenchmarks' own
// precedent for "size the baseline can survive"); the memoized arm's whole point is
// that it only pays for the distinct (day, used, position) states that actually
// occur, so it runs on to LC 3573's own bound of 1,000 days, and the two are compared
// at the sizes both run.
public class BestTimeToBuyAndSellStockVBenchmarks
{
    private const int Seed = 3573; // LC problem number
    private const int MaxValueExclusive = 1_000;
    private const int K = 3;

    private Dictionary<int, int[]> _pricesByLength = [];

    public static IEnumerable<int> BaselineSizes => [10, 14];

    public static IEnumerable<int> MemoizedSizes => [.. BaselineSizes, 100, 1_000];

    // Every length any arm runs is drawn here, outside the timed region, each from its own
    // generator on the same seed; an arm looks its own up.
    [GlobalSetup]
    public void Setup() =>
        _pricesByLength = MemoizedSizes.ToDictionary(
            length => length,
            length => SeededDraws.Values(length, 1, MaxValueExclusive, new Random(Seed)));

    [Benchmark(Baseline = true)]
    [ArgumentsSource(nameof(BaselineSizes))]
    public long BruteForce(int length) =>
        BestTimeToBuyAndSellStockVSolution.MaxProfitByBruteForce(_pricesByLength[length], K);

    [Benchmark]
    [ArgumentsSource(nameof(MemoizedSizes))]
    public long TransactionMemoization(int length) =>
        BestTimeToBuyAndSellStockVSolution.MaxProfitByTransactionMemoization(_pricesByLength[length], K);
}
