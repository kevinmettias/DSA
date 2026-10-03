using DSAExperimentation.Benchmarks.Fixtures;
using DSAExperimentation.LeetCode.FinalPricesWithASpecialDiscountInAShop;

namespace DSAExperimentation.Benchmarks.ProblemSolutions;

// Harness only: both arms are FinalPricesWithASpecialDiscountInAShopSolution's,
// the same methods FinalPricesWithASpecialDiscountInAShopSolutionTests proves correct -
// the O(n^2) forward scan against the O(n) monotonic-stack pass over this repo's
// own Stack<int>, the same brute-force-vs-primitive shape TwoSumBenchmarks makes.
// Prices are random with no forced worst case, matching the distribution
// LeetCode's own constraints describe; the price array is LeetCode's own argument
// shape, so [GlobalSetup] hands it to both arms directly. LC 1475 caps the prices at
// 500, so the larger Length is that cap.
public class FinalPricesWithASpecialDiscountInAShopBenchmarks
{
    private const int MaxPrice = 1_000;

    // Fixed seed so the measured price distribution is identical run to run;
    // the value carries over unchanged from the pre-migration benchmark.
    private const int PriceSeed = 1;

    private int[] _prices = [];

    [Params(200, 500)]
    public int Length { get; set; }

    [GlobalSetup]
    public void Setup()
    {
        var random = new Random(PriceSeed);
        _prices = SeededDraws.Values(Length, 1, MaxPrice, random);
    }

    [Benchmark(Baseline = true)]
    public int[] BruteForce() =>
        FinalPricesWithASpecialDiscountInAShopSolution.FinalPricesByBruteForce(_prices);

    [Benchmark]
    public int[] MonotonicStack() =>
        FinalPricesWithASpecialDiscountInAShopSolution.FinalPricesByMonotonicStack(_prices);
}
