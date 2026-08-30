using BenchmarkDotNet.Attributes;
using DSAExperimentation.Algorithms.DynamicProgramming;

namespace DSAExperimentation.Benchmarks.ProblemSolutions;

// Coin Change (LC 322): plain bottom-up array tabulation vs. this repo's
// Memoizer-based top-down recursion (DecodeWaysBenchmarks precedent) - both solve
// the same unbounded-knapsack recurrence in O(amount * coins.Length), just walking
// it from opposite directions.
[MemoryDiagnoser]
public class CoinChangeBenchmarks
{
    private const int Unreachable = int.MaxValue / 2;
    private static readonly int[] Coins = [1, 5, 10, 25];

    [Params(200, 2_000)]
    public int Amount;

    [Benchmark(Baseline = true)]
    public int Tabulation()
    {
        var dp = new int[Amount + 1];

        for (var a = 1; a <= Amount; a++)
        {
            var best = Unreachable;
            foreach (var coin in Coins)
            {
                if (coin <= a && dp[a - coin] < Unreachable)
                {
                    best = Math.Min(best, dp[a - coin] + 1);
                }
            }

            dp[a] = best;
        }

        return dp[Amount] >= Unreachable ? -1 : dp[Amount];
    }

    [Benchmark]
    public int Memoized()
    {
        var result = Memoizer.Memoize<int, int>(Amount, MinCoinsFor);
        return result >= Unreachable ? -1 : result;

        int MinCoinsFor(int remaining, Func<int, int> minCoins)
        {
            if (remaining == 0)
            {
                return 0;
            }

            if (remaining < 0)
            {
                return Unreachable;
            }

            var best = Unreachable;
            foreach (var coin in Coins)
            {
                var sub = minCoins(remaining - coin);
                if (sub < Unreachable)
                {
                    best = Math.Min(best, sub + 1);
                }
            }

            return best;
        }
    }
}
