using BenchmarkDotNet.Attributes;
using DSAExperimentation.LeetCode.BestTimeToBuyAndSellStockIII;

namespace DSAExperimentation.Benchmarks.ProblemSolutions;

// Harness only: both arms are BestTimeToBuyAndSellStockIIISolution's, the same
// methods BestTimeToBuyAndSellStockIIITests proves correct. Brute force is a
// genuine, unmemoized two-way choice tree per day, so Length stays small enough
// for that arm to finish in reasonable time (BestTimeToBuyAndSellStockVBenchmarks'
// own precedent for "size the baseline can survive") - the memoized arm's whole
// point is that it only pays for the distinct (day, holding, transactions) states
// that actually occur.
[MemoryDiagnoser]
public class BestTimeToBuyAndSellStockIIIBenchmarks
{
    private const int Seed = 123; // LC problem number
    private const int MaxPriceExclusive = 1_000;

    private int[] _prices = [];

    [Params(16, 20)]
    public int Length { get; set; }

    [GlobalSetup]
    public void Setup()
    {
        var random = new Random(Seed);
        _prices = Enumerable.Range(0, Length).Select(_ => random.Next(1, MaxPriceExclusive)).ToArray();
    }

    [Benchmark(Baseline = true)]
    public int BruteForce() => BestTimeToBuyAndSellStockIIISolution.MaxProfitByBruteForce(_prices);

    [Benchmark]
    public int TransactionMemoization() =>
        BestTimeToBuyAndSellStockIIISolution.MaxProfitByTransactionMemoization(_prices);
}
