using BenchmarkDotNet.Attributes;
using DSAExperimentation.Algorithms.DynamicProgramming;

namespace DSAExperimentation.Benchmarks.ProblemSolutions;

// Number of Music Playlists (LC 920): plain bottom-up 2-D tabulation (dp[length,
// unique], filled in increasing length order) vs. this repo's Memoizer-based
// top-down recursion over the same (length, unique) state
// (CoinChangeIIBenchmarks precedent for tabulation-vs-Memoizer on a 2-D DP,
// DecodeWaysIITests precedent for the mod-reduced running total) - both compute the
// same count in O(Goal * N). k is fixed below Goal/N's params (LeetCode's own
// n/2 <= goal - k <= n constraint isn't required for either implementation to be
// correct, but staying inside it keeps every benchmarked case realistic).
[MemoryDiagnoser]
public class NumberOfMusicPlaylistsBenchmarks
{
    private const long Mod = 1_000_000_007;
    private const int K = 2;

    [Params(20, 100)]
    public int N;

    public int Goal => N;

    [Benchmark(Baseline = true)]
    public long Tabulation()
    {
        var dp = new long[Goal + 1, N + 1];
        dp[0, 0] = 1;

        for (var length = 1; length <= Goal; length++)
        {
            for (var unique = 1; unique <= N; unique++)
            {
                var total = dp[length - 1, unique - 1] * (N - unique + 1) % Mod;

                if (unique > K)
                {
                    total = (total + (dp[length - 1, unique] * (unique - K))) % Mod;
                }

                dp[length, unique] = total;
            }
        }

        return dp[Goal, N];
    }

    [Benchmark]
    public long Memoized()
    {
        return Memoizer.Memoize<(int Length, int Unique), long>((Goal, N), Ways);

        long Ways((int Length, int Unique) state, Func<(int Length, int Unique), long> ways)
        {
            var (length, unique) = state;

            if (length == 0)
            {
                return unique == 0 ? 1 : 0;
            }

            if (unique == 0)
            {
                return 0;
            }

            var total = ways((length - 1, unique - 1)) * (N - unique + 1) % Mod;

            if (unique > K)
            {
                total = (total + (ways((length - 1, unique)) * (unique - K))) % Mod;
            }

            return total;
        }
    }
}
