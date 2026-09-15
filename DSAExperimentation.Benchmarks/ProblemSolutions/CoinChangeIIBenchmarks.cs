using BenchmarkDotNet.Attributes;
using DSAExperimentation.LeetCode.CoinChangeII;

namespace DSAExperimentation.Benchmarks.ProblemSolutions;

// Harness only: both arms are CoinChangeIISolution's, the same methods
// CoinChangeIITests proves correct - plain bottom-up tabulation vs. this repo's
// Memoizer-based top-down recursion over a 2-D (coin index, remaining amount) state,
// both O(coins.Length * amount).
[MemoryDiagnoser]
public class CoinChangeIIBenchmarks
{
    private static readonly int[] Coins = [1, 5, 10, 25, 50];

    [Params(200, 2_000)]
    public int Amount { get; set; }

    [Benchmark(Baseline = true)]
    public int Tabulation() => CoinChangeIISolution.CountCombinationsByTabulation(Amount, Coins);

    [Benchmark]
    public int Memoized() => CoinChangeIISolution.CountCombinationsByMemoizedTopDown(Amount, Coins);
}
