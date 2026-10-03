using DSAExperimentation.Benchmarks.Fixtures;
using DSAExperimentation.LeetCode.BestTimeToBuyAndSellStockII;

namespace DSAExperimentation.Benchmarks.ProblemSolutions;

// Harness only: both arms are BestTimeToBuyAndSellStockIISolution's, the same
// methods BestTimeToBuyAndSellStockIISolutionTests proves correct. Brute force is a
// genuine, unmemoized two-way choice tree per day, so Length stays small enough
// for that arm to finish in reasonable time (BestTimeToBuyAndSellStockVBenchmarks'
// own precedent for "size the baseline can survive") - the greedy arm's whole
// point is that it never needs the exponential search at all.
public class BestTimeToBuyAndSellStockIIBenchmarks
{
    private const int Seed = 122; // LC problem number
    private const int MaxPriceExclusive = 1_000;

    private int[] _prices = [];

    [Params(16, 20)]
    public int Length { get; set; }

    [GlobalSetup]
    public void Setup()
    {
        var random = new Random(Seed);
        _prices = SeededDraws.Values(Length, 1, MaxPriceExclusive, random);
    }

    [Benchmark(Baseline = true)]
    public int BruteForce() => BestTimeToBuyAndSellStockIISolution.MaxProfitByBruteForce(_prices);

    [Benchmark]
    public int GreedyAscent() => BestTimeToBuyAndSellStockIISolution.MaxProfitByGreedyAscent(_prices);
}
