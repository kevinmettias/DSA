using BenchmarkDotNet.Attributes;
using DSAExperimentation.LeetCode.BestTimeToBuyAndSellStockV;

namespace DSAExperimentation.Benchmarks.ProblemSolutions;

// Harness only: both arms are BestTimeToBuyAndSellStockVSolution's, the same
// methods BestTimeToBuyAndSellStockVTests proves correct. Brute force is a
// genuine, unmemoized 3-way choice tree per day, so Length stays small enough
// for that arm to finish in reasonable time
// (FindTheNumberOfSubsequencesWithEqualGcdBenchmarks' own precedent for "size
// the baseline can survive") - the memoized arm's whole point is that it only
// pays for the distinct (day, used, position) states that actually occur.
[MemoryDiagnoser]
public class BestTimeToBuyAndSellStockVBenchmarks
{
    private const int Seed = 3573; // LC problem number
    private const int MaxValueExclusive = 1_000;
    private const int K = 3;

    private int[] _prices = [];

    [Params(10, 14)]
    public int Length { get; set; }

    [GlobalSetup]
    public void Setup()
    {
        var random = new Random(Seed);
        _prices = Enumerable.Range(0, Length).Select(_ => random.Next(1, MaxValueExclusive)).ToArray();
    }

    [Benchmark(Baseline = true)]
    public long BruteForce() => BestTimeToBuyAndSellStockVSolution.MaxProfitByBruteForce(_prices, K);

    [Benchmark]
    public long TransactionMemoization() =>
        BestTimeToBuyAndSellStockVSolution.MaxProfitByTransactionMemoization(_prices, K);
}
