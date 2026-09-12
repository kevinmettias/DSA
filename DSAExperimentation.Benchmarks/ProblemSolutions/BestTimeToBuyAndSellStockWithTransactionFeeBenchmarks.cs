using BenchmarkDotNet.Attributes;
using DSAExperimentation.LeetCode.BestTimeToBuyAndSellStockWithTransactionFee;

namespace DSAExperimentation.Benchmarks.ProblemSolutions;

// Harness only: both arms are BestTimeToBuyAndSellStockWithTransactionFeeSolution's,
// the same methods BestTimeToBuyAndSellStockWithTransactionFeeTests proves correct.
// Length is kept modest (<=28) specifically because the un-memoized baseline's
// blowup is real, the same reasoning
// BestTimeToBuyAndSellStockWithCooldownBenchmarks.cs (LC 309) already documents for
// this identical state shape.
[MemoryDiagnoser]
public class BestTimeToBuyAndSellStockWithTransactionFeeBenchmarks
{
    private const int Fee = 2;
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
        BestTimeToBuyAndSellStockWithTransactionFeeSolution.MaxProfitByUnmemoizedRecursion(_prices, Fee);

    [Benchmark]
    public int MemoizedRecursion() =>
        BestTimeToBuyAndSellStockWithTransactionFeeSolution.MaxProfitByMemoizedRecursion(_prices, Fee);
}
