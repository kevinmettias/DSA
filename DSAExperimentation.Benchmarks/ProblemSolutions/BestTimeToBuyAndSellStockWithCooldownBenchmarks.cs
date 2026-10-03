using DSAExperimentation.Benchmarks.Fixtures;
using DSAExperimentation.LeetCode.BestTimeToBuyAndSellStockWithCooldown;

namespace DSAExperimentation.Benchmarks.ProblemSolutions;

// Harness only: both arms are BestTimeToBuyAndSellStockWithCooldownSolution's, the
// same methods BestTimeToBuyAndSellStockWithCooldownSolutionTests proves correct. Length is
// kept modest (<=28) specifically because the un-memoized baseline's blowup is real,
// the same reasoning FibonacciNumberBenchmarks already documents.
public class BestTimeToBuyAndSellStockWithCooldownBenchmarks
{
    private const int MaxPrice = 100;

    private int[] _prices = [];

    [Params(20, 28)]
    public int Length { get; set; }

    [GlobalSetup]
    public void Setup()
    {
        var random = new Random(1);
        _prices = SeededDraws.Values(Length, 0, MaxPrice, random);
    }

    [Benchmark(Baseline = true)]
    public int UnmemoizedRecursion() =>
        BestTimeToBuyAndSellStockWithCooldownSolution.MaxProfitByUnmemoizedRecursion(_prices);

    [Benchmark]
    public int MemoizedRecursion() =>
        BestTimeToBuyAndSellStockWithCooldownSolution.MaxProfitByMemoizedRecursion(_prices);
}
