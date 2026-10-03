using DSAExperimentation.Benchmarks.Fixtures;
using DSAExperimentation.LeetCode.BestTimeToBuyAndSellStockWithTransactionFee;

namespace DSAExperimentation.Benchmarks.ProblemSolutions;

// Harness only: both arms are BestTimeToBuyAndSellStockWithTransactionFeeSolution's,
// the same methods BestTimeToBuyAndSellStockWithTransactionFeeSolutionTests proves correct.
// Length is kept modest (<=28) specifically because the un-memoized baseline's
// blowup is real, the same reasoning
// BestTimeToBuyAndSellStockWithCooldownBenchmarks.cs (LC 309) already documents for
// this identical state shape.
public class BestTimeToBuyAndSellStockWithTransactionFeeBenchmarks
{
    private const int Fee = 2;
    private const int MaxPrice = 100;

    private int[] _prices = [];

    [Params(20, 28)]
    public int Length { get; set; }

    [GlobalSetup]
    public void Setup()
    {
        var random = new Random(1);
        // LC 714's prices start at 1.
        _prices = SeededDraws.Values(Length, 1, MaxPrice, random);
    }

    [Benchmark(Baseline = true)]
    public int UnmemoizedRecursion() =>
        BestTimeToBuyAndSellStockWithTransactionFeeSolution.MaxProfitByUnmemoizedRecursion(_prices, Fee);

    [Benchmark]
    public int MemoizedRecursion() =>
        BestTimeToBuyAndSellStockWithTransactionFeeSolution.MaxProfitByMemoizedRecursion(_prices, Fee);
}
