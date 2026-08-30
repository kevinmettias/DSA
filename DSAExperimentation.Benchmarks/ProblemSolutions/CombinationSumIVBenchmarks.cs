using BenchmarkDotNet.Attributes;
using DSAExperimentation.Algorithms.DynamicProgramming;

namespace DSAExperimentation.Benchmarks.ProblemSolutions;

// Combination Sum IV (LC 377): plain bottom-up array tabulation vs. this repo's
// Memoizer-based top-down recursion (CoinChangeBenchmarks precedent) - both solve the
// same unbounded-knapsack counting recurrence in O(target * nums.Length), just walking
// it from opposite directions. The running counts overflow a 32-bit int well before
// Target's upper [Params] bound (the real LeetCode judge only guarantees an int-sized
// answer for its own, much smaller constraints) - harmless here since both benchmarked
// methods overflow identically and this class measures wall-clock time, not the
// returned value.
[MemoryDiagnoser]
public class CombinationSumIVBenchmarks
{
    private static readonly int[] Nums = [1, 2, 3, 5, 10];

    [Params(200, 2_000)]
    public int Target;

    [Benchmark(Baseline = true)]
    public int Tabulation()
    {
        var dp = new int[Target + 1];
        dp[0] = 1;

        for (var t = 1; t <= Target; t++)
        {
            var total = 0;
            foreach (var num in Nums)
            {
                if (num <= t)
                {
                    total += dp[t - num];
                }
            }

            dp[t] = total;
        }

        return dp[Target];
    }

    [Benchmark]
    public int Memoized()
    {
        return Memoizer.Memoize<int, int>(Target, WaysFor);

        int WaysFor(int remaining, Func<int, int> ways)
        {
            if (remaining == 0)
            {
                return 1;
            }

            var total = 0;
            foreach (var num in Nums)
            {
                if (num <= remaining)
                {
                    total += ways(remaining - num);
                }
            }

            return total;
        }
    }
}
