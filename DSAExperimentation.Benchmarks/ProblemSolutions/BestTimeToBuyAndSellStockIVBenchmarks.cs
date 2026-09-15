using BenchmarkDotNet.Attributes;
using DSAExperimentation.LeetCode.BestTimeToBuyAndSellStockIV;

namespace DSAExperimentation.Benchmarks.ProblemSolutions;

// Harness only: both arms are BestTimeToBuyAndSellStockIVSolution's, the same
// methods BestTimeToBuyAndSellStockIVTests proves correct. Pre-migration this
// class was an untested compile-smoke placeholder (Baseline() => 1,
// PrimitiveComposed() => 1) rather than a second strategy to reconcile. Brute
// force is a genuine, unmemoized two-way choice tree per day, so Length stays
// small enough for that arm to finish in reasonable time
// (BestTimeToBuyAndSellStockIIIBenchmarks' own precedent for the same DP
// family) - the memoized arm's whole point is that it only pays for the
// distinct (day, holding, transactions) states that actually occur.
[MemoryDiagnoser]
public class BestTimeToBuyAndSellStockIVBenchmarks
{
    private const int Seed = 188; // LC problem number
    private const int MaxPriceExclusive = 1_000;
    private const int K = 2;

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
    public int BruteForce() => BestTimeToBuyAndSellStockIVSolution.MaxProfitByBruteForce(_prices, K);

    [Benchmark]
    public int TransactionMemoization() =>
        BestTimeToBuyAndSellStockIVSolution.MaxProfitByTransactionMemoization(_prices, K);
}
