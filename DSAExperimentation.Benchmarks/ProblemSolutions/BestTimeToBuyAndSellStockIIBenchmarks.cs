using DSAExperimentation.Benchmarks.Fixtures;
using DSAExperimentation.LeetCode.BestTimeToBuyAndSellStockII;

namespace DSAExperimentation.Benchmarks.ProblemSolutions;

// Harness only: both arms are BestTimeToBuyAndSellStockIISolution's, the same
// methods BestTimeToBuyAndSellStockIISolutionTests proves correct.
//
// Sizes are per arm. Brute force is a genuine, unmemoized two-way choice tree per
// day, so it stops at 20 days (BestTimeToBuyAndSellStockVBenchmarks' own precedent
// for "size the baseline can survive"); the greedy arm's whole point is that it never
// needs the exponential search at all, so its single pass runs on to LC 122's own
// bound of 30,000 days, and the two are compared at the sizes both run.
public class BestTimeToBuyAndSellStockIIBenchmarks
{
    private const int Seed = 122; // LC problem number
    private const int MaxPriceExclusive = 1_000;

    private Dictionary<int, int[]> _pricesByLength = [];

    public static IEnumerable<int> BaselineSizes => [16, 20];

    public static IEnumerable<int> GreedySizes => [.. BaselineSizes, 1_000, 30_000];

    // Every length any arm runs is drawn here, outside the timed region, each from its own
    // generator on the same seed; an arm looks its own up.
    [GlobalSetup]
    public void Setup() =>
        _pricesByLength = GreedySizes.ToDictionary(
            length => length,
            length => SeededDraws.Values(length, 1, MaxPriceExclusive, new Random(Seed)));

    [Benchmark(Baseline = true)]
    [ArgumentsSource(nameof(BaselineSizes))]
    public int BruteForce(int length) =>
        BestTimeToBuyAndSellStockIISolution.MaxProfitByBruteForce(_pricesByLength[length]);

    [Benchmark]
    [ArgumentsSource(nameof(GreedySizes))]
    public int GreedyAscent(int length) =>
        BestTimeToBuyAndSellStockIISolution.MaxProfitByGreedyAscent(_pricesByLength[length]);
}
