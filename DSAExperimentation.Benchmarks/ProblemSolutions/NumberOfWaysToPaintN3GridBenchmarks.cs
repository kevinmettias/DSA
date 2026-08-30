using BenchmarkDotNet.Attributes;
using DSAExperimentation.Algorithms.DynamicProgramming;

namespace DSAExperimentation.Benchmarks.ProblemSolutions;

// Number of Ways to Paint N x 3 Grid (LC 1411): plain bottom-up tabulation
// over the (same, different) row-pattern-count pair vs. this repo's
// Memoizer-based top-down recursion over the same pair
// (NumberOfMusicPlaylistsBenchmarks precedent for tabulation-vs-Memoizer on a
// small-fixed-state DP) - both compute the same count in O(N).
[MemoryDiagnoser]
public class NumberOfWaysToPaintN3GridBenchmarks
{
    private const long Modulus = 1_000_000_007;

    [Params(1_000, 5_000)]
    public int N;

    [Benchmark(Baseline = true)]
    public long Tabulation()
    {
        long same = 6;
        long different = 6;

        for (var row = 2; row <= N; row++)
        {
            var nextSame = (3 * same + 2 * different) % Modulus;
            var nextDifferent = (2 * same + 2 * different) % Modulus;
            same = nextSame;
            different = nextDifferent;
        }

        return (same + different) % Modulus;
    }

    [Benchmark]
    public long Memoized()
    {
        var (same, different) = Memoizer.Memoize<int, (long Same, long Different)>(N, Ways);
        return (same + different) % Modulus;
    }

    private static (long Same, long Different) Ways(int row, Func<int, (long Same, long Different)> ways)
    {
        if (row == 1)
        {
            return (6, 6);
        }

        var (prevSame, prevDifferent) = ways(row - 1);

        return (
            (3 * prevSame + 2 * prevDifferent) % Modulus,
            (2 * prevSame + 2 * prevDifferent) % Modulus);
    }
}
