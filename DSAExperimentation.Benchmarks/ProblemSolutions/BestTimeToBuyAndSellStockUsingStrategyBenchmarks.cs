using BenchmarkDotNet.Attributes;
using DSAExperimentation.Benchmarks.Fixtures;
using DSAExperimentation.LeetCode.BestTimeToBuyAndSellStockUsingStrategy;

namespace DSAExperimentation.Benchmarks.ProblemSolutions;

// Harness only: both arms are BestTimeToBuyAndSellStockUsingStrategySolution's, the
// same methods BestTimeToBuyAndSellStockUsingStrategyTests proves correct. Prices and
// strategy are built once in [GlobalSetup]; both arms take LeetCode's own array shape
// directly, so there is nothing further to hoist.
[MemoryDiagnoser]
public class BestTimeToBuyAndSellStockUsingStrategyBenchmarks
{
    private const int Seed = 3652;
    private const int WindowSize = 100;

    private int[] _prices = [];

    private int[] _strategy = [];
    [Params(1_000, 20_000)]
    public int DayCount { get; set; }

    [GlobalSetup]
    public void Setup() => (_prices, _strategy) = BestTimeToBuyAndSellStockUsingStrategyWorkloads.Build(DayCount, seed: Seed);

    [Benchmark(Baseline = true)]
    public long BruteForceWindowSum() =>
        BestTimeToBuyAndSellStockUsingStrategySolution.MaxProfitByBruteForceWindowSum(_prices, _strategy, WindowSize);

    [Benchmark]
    public long SlidingWindowSum() =>
        BestTimeToBuyAndSellStockUsingStrategySolution.MaxProfitBySlidingWindowSum(_prices, _strategy, WindowSize);
}
