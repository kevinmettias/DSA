using BenchmarkDotNet.Attributes;
using DSAExperimentation.LeetCode.CoinChange;

namespace DSAExperimentation.Benchmarks.ProblemSolutions;

// Harness only: both arms are CoinChangeSolution's, the same methods
// CoinChangeTests proves correct.
[MemoryDiagnoser]
public class CoinChangeBenchmarks
{
    private static readonly int[] Coins = [1, 5, 10, 25];

    [Params(200, 2_000)]
    public int Amount;

    [Benchmark(Baseline = true)]
    public int Tabulation() => CoinChangeSolution.FewestCoinsByTabulation(Coins, Amount);

    [Benchmark]
    public int Memoized() => CoinChangeSolution.FewestCoinsByMemoization(Coins, Amount);
}
