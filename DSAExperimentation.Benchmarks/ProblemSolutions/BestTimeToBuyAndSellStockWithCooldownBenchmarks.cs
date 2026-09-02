using BenchmarkDotNet.Attributes;
using DSAExperimentation.LeetCode.BestTimeToBuyAndSellStockWithCooldown;

namespace DSAExperimentation.Benchmarks.ProblemSolutions;

// Harness only: both arms are BestTimeToBuyAndSellStockWithCooldownSolution's, the
// same methods BestTimeToBuyAndSellStockWithCooldownTests proves correct. Length is
// kept modest (<=28) specifically because the un-memoized baseline's blowup is real,
// the same reasoning FibonacciBenchmarks.cs's NaiveRecursive already documents.
[MemoryDiagnoser]
public class BestTimeToBuyAndSellStockWithCooldownBenchmarks
{
    private const int MaxPrice = 100;

    [Params(20, 28)]
    public int Length;

    private int[] _prices = null!;

    [GlobalSetup]
    public void Setup()
    {
        var random = new Random(1);
        _prices = Enumerable.Range(0, Length).Select(_ => random.Next(0, MaxPrice)).ToArray();
    }

    [Benchmark(Baseline = true)]
    public int UnmemoizedRecursion() =>
        BestTimeToBuyAndSellStockWithCooldownSolution.MaxProfitByUnmemoizedRecursion(_prices);

    [Benchmark]
    public int MemoizedRecursion() =>
        BestTimeToBuyAndSellStockWithCooldownSolution.MaxProfitByMemoizedRecursion(_prices);
}
