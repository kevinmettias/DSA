using BenchmarkDotNet.Attributes;
using DSAExperimentation.LeetCode.BestTimeToBuyAndSellStock;

namespace DSAExperimentation.Benchmarks.ProblemSolutions;

// Harness only: both arms are BestTimeToBuyAndSellStockSolution's, the same
// methods BestTimeToBuyAndSellStockTests proves correct. Neither strategy has an
// early exit, so the random price distribution only affects the answer, not how
// much work either arm does.
[MemoryDiagnoser]
public class BestTimeToBuyAndSellStockBenchmarks
{
    private const int MaxPriceExclusive = 1_000;
    private const int Seed = 121;

    private int[] _prices = [];

    [Params(200, 5_000)]
    public int Length { get; set; }

    [GlobalSetup]
    public void Setup()
    {
        var random = new Random(Seed);
        _prices = Enumerable.Range(0, Length).Select(_ => random.Next(1, MaxPriceExclusive)).ToArray();
    }

    [Benchmark(Baseline = true)]
    public int BruteForce() => BestTimeToBuyAndSellStockSolution.MaxProfitByBruteForce(_prices);

    [Benchmark]
    public int OnePassMinTracking() => BestTimeToBuyAndSellStockSolution.MaxProfitByOnePassMinTracking(_prices);
}
