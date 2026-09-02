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
    private const long Row1PatternCount = 6;
    private const int FirstDpRowIndex = 2;
    private const long SamePatternWeight = 3;
    private const long DifferentPatternWeight = 2;

    [Params(1_000, 5_000)]
    public int N;

    [Benchmark(Baseline = true)]
    public long Tabulation()
    {
        long same = Row1PatternCount;
        long different = Row1PatternCount;

        for (var row = FirstDpRowIndex; row <= N; row++)
        {
            var nextSame = (SamePatternWeight * same + DifferentPatternWeight * different) % Modulus;
            var nextDifferent = (DifferentPatternWeight * same + DifferentPatternWeight * different) % Modulus;
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
            return (Row1PatternCount, Row1PatternCount);
        }

        var (prevSame, prevDifferent) = ways(row - 1);

        return (
            (SamePatternWeight * prevSame + DifferentPatternWeight * prevDifferent) % Modulus,
            (DifferentPatternWeight * prevSame + DifferentPatternWeight * prevDifferent) % Modulus);
    }
}
