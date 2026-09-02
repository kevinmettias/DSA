using BenchmarkDotNet.Attributes;
using DSAExperimentation.LeetCode.BestTimeToBuyAndSellStockII;

namespace DSAExperimentation.Benchmarks.ProblemSolutions;

// Harness only: both arms are BestTimeToBuyAndSellStockIISolution's, the same
// methods BestTimeToBuyAndSellStockIITests proves correct. Brute force is a
// genuine, unmemoized two-way choice tree per day, so Length stays small enough
// for that arm to finish in reasonable time (BestTimeToBuyAndSellStockVBenchmarks'
// own precedent for "size the baseline can survive") - the greedy arm's whole
// point is that it never needs the exponential search at all.
[MemoryDiagnoser]
public class BestTimeToBuyAndSellStockIIBenchmarks
{
    private const int Seed = 122; // LC problem number
    private const int MaxPriceExclusive = 1_000;

    [Params(16, 20)]
    public int Length;

    private int[] _prices = null!;

    [GlobalSetup]
    public void Setup()
    {
        var random = new Random(Seed);
        _prices = Enumerable.Range(0, Length).Select(_ => random.Next(1, MaxPriceExclusive)).ToArray();
    }

    [Benchmark(Baseline = true)]
    public int BruteForce() => BestTimeToBuyAndSellStockIISolution.MaxProfitByBruteForce(_prices);

    [Benchmark]
    public int GreedyAscent() => BestTimeToBuyAndSellStockIISolution.MaxProfitByGreedyAscent(_prices);
}
