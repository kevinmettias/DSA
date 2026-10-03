using DSAExperimentation.Benchmarks.Fixtures;
using DSAExperimentation.LeetCode.BestTimeToBuyAndSellStockIII;

namespace DSAExperimentation.Benchmarks.ProblemSolutions;

// Harness only: both arms are BestTimeToBuyAndSellStockIIISolution's, the same
// methods BestTimeToBuyAndSellStockIIISolutionTests proves correct.
//
// Sizes are per arm. Brute force is a genuine, unmemoized two-way choice tree per
// day, so it stops at 20 days (BestTimeToBuyAndSellStockVBenchmarks' own precedent
// for "size the baseline can survive"); the memoized arm's whole point is that it
// only pays for the distinct (day, holding, transactions) states that actually occur,
// so it runs on to 1,000 days. LC 123 allows 10^5, but the memoized recursion is as
// deep as the price list is long, so 1,000 keeps its call stack shallow. The two are
// compared at the sizes both run.
public class BestTimeToBuyAndSellStockIIIBenchmarks
{
    private const int Seed = 123; // LC problem number
    private const int MaxPriceExclusive = 1_000;

    private Dictionary<int, int[]> _pricesByLength = [];

    public static IEnumerable<int> BaselineSizes => [16, 20];

    public static IEnumerable<int> MemoizedSizes => [.. BaselineSizes, 100, 1_000];

    // Every length any arm runs is drawn here, outside the timed region, each from its own
    // generator on the same seed; an arm looks its own up.
    [GlobalSetup]
    public void Setup() =>
        _pricesByLength = MemoizedSizes.ToDictionary(
            length => length,
            length => SeededDraws.Values(length, 1, MaxPriceExclusive, new Random(Seed)));

    [Benchmark(Baseline = true)]
    [ArgumentsSource(nameof(BaselineSizes))]
    public int BruteForce(int length) =>
        BestTimeToBuyAndSellStockIIISolution.MaxProfitByBruteForce(_pricesByLength[length]);

    [Benchmark]
    [ArgumentsSource(nameof(MemoizedSizes))]
    public int TransactionMemoization(int length) =>
        BestTimeToBuyAndSellStockIIISolution.MaxProfitByTransactionMemoization(_pricesByLength[length]);
}
