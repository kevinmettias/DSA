using BenchmarkDotNet.Attributes;
using DSAExperimentation.Algorithms.DynamicProgramming;

namespace DSAExperimentation.Benchmarks.ProblemSolutions;

// Coin Change II (LC 518): plain bottom-up 1-D tabulation (outer loop over coins,
// inner loop over amount ascending - the standard trick that counts each coin
// combination exactly once, never a permutation of it) vs. this repo's Memoizer-based
// top-down recursion over a 2-D (coin index, remaining amount) state
// (CoinChangeBenchmarks/CombinationSumIVBenchmarks precedent for the memoized half,
// DistinctSubsequencesTests precedent for the 2-D tuple state) - both compute the same
// combination count in O(coins.Length * amount). The running counts can overflow a
// 32-bit int well before Amount's upper [Params] bound, same as
// CombinationSumIVBenchmarks - harmless here since both benchmarked methods overflow
// identically and this class measures wall-clock time, not the returned value.
[MemoryDiagnoser]
public class CoinChangeIIBenchmarks
{
    private static readonly int[] Coins = [1, 5, 10, 25, 50];

    [Params(200, 2_000)]
    public int Amount;

    [Benchmark(Baseline = true)]
    public int Tabulation()
    {
        var dp = new int[Amount + 1];
        dp[0] = 1;

        foreach (var coin in Coins)
        {
            for (var a = coin; a <= Amount; a++)
            {
                dp[a] += dp[a - coin];
            }
        }

        return dp[Amount];
    }

    [Benchmark]
    public int Memoized()
    {
        return Memoizer.Memoize<(int Index, int Remaining), int>((0, Amount), WaysFor);

        int WaysFor((int Index, int Remaining) state, Func<(int Index, int Remaining), int> ways)
        {
            var (index, remaining) = state;

            if (remaining == 0)
            {
                return 1;
            }

            if (index == Coins.Length)
            {
                return 0;
            }

            var total = ways((index + 1, remaining));
            if (Coins[index] <= remaining)
            {
                total += ways((index, remaining - Coins[index]));
            }

            return total;
        }
    }
}
