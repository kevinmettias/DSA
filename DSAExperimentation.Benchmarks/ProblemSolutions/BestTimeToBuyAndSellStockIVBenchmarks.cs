using DSAExperimentation.Benchmarks.Fixtures;
using DSAExperimentation.LeetCode.BestTimeToBuyAndSellStockIV;

namespace DSAExperimentation.Benchmarks.ProblemSolutions;

// Harness only: both arms are BestTimeToBuyAndSellStockIVSolution's, the same
// methods BestTimeToBuyAndSellStockIVSolutionTests proves correct. Pre-migration this
// class was an untested compile-smoke placeholder (Baseline() => 1,
// PrimitiveComposed() => 1) rather than a second strategy to reconcile.
//
// Sizes are per arm. Brute force is a genuine, unmemoized two-way choice tree per
// day, so it stops at 20 days (BestTimeToBuyAndSellStockIIIBenchmarks' own precedent
// for the same DP family); the memoized arm's whole point is that it only pays for
// the distinct (day, holding, transactions) states that actually occur, so it runs on
// to LC 188's own bound of 1,000 days, and the two are compared at the sizes both run.
public class BestTimeToBuyAndSellStockIVBenchmarks
{
    private const int Seed = 188; // LC problem number
    private const int MaxPriceExclusive = 1_000;
    private const int K = 2;

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
        BestTimeToBuyAndSellStockIVSolution.MaxProfitByBruteForce(_pricesByLength[length], K);

    [Benchmark]
    [ArgumentsSource(nameof(MemoizedSizes))]
    public int TransactionMemoization(int length) =>
        BestTimeToBuyAndSellStockIVSolution.MaxProfitByTransactionMemoization(_pricesByLength[length], K);
}
