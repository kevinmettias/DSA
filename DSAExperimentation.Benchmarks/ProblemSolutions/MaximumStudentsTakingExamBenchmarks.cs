using BenchmarkDotNet.Attributes;
using DSAExperimentation.Algorithms.DynamicProgramming;

namespace DSAExperimentation.Benchmarks.ProblemSolutions;

// Maximum Students Taking Exam (LC 1349): the textbook unmemoized bitmask recursion
// over "which seats in this row are occupied" (re-explores the identical (Row,
// PrevMask) subtree once per distinct prior-row mask sequence that happens to land on
// the same PrevMask) vs. the same recursion routed through this repo's own Memoizer,
// keyed on (Row, PrevMask) - CanIWinBenchmarks'/SmallestSufficientTeamBenchmarks'
// precedent, applied to a 2D tuple state instead of a 1D one. Every seat is left open
// (no broken seats), maximizing both the per-row branching factor and how often
// different row-by-row paths converge on the same PrevMask - the worst case for an
// unmemoized walk and the best case for memoization. RowCount stays modest, matching
// LongestIncreasingPathInAMatrixBenchmarks' "kept modest for exactly that reason",
// since the unmemoized side is exponential in RowCount.
[MemoryDiagnoser]
public class MaximumStudentsTakingExamBenchmarks
{
    private const int ColumnCount = 6;

    [Params(3, 5)]
    public int RowCount;

    private int _fullMask;

    [GlobalSetup]
    public void Setup() => _fullMask = (1 << ColumnCount) - 1;

    [Benchmark(Baseline = true)]
    public int BruteForceRecursion() => BestFromBruteForce(0, 0);

    private int BestFromBruteForce(int row, int prevMask)
    {
        if (row == RowCount)
        {
            return 0;
        }

        var best = 0;
        for (var mask = 0; mask <= _fullMask; mask++)
        {
            if ((mask & (mask << 1)) != 0
                || (mask & (prevMask << 1)) != 0
                || (mask & (prevMask >> 1)) != 0)
            {
                continue;
            }

            best = Math.Max(best, PopCount(mask) + BestFromBruteForce(row + 1, mask));
        }

        return best;
    }

    [Benchmark]
    public int MemoizedRecursion() => Memoizer.Memoize<(int Row, int PrevMask), int>((0, 0), BestFrom);

    private int BestFrom((int Row, int PrevMask) state, Func<(int Row, int PrevMask), int> bestFrom)
    {
        var (row, prevMask) = state;
        if (row == RowCount)
        {
            return 0;
        }

        var best = 0;
        for (var mask = 0; mask <= _fullMask; mask++)
        {
            if ((mask & (mask << 1)) != 0
                || (mask & (prevMask << 1)) != 0
                || (mask & (prevMask >> 1)) != 0)
            {
                continue;
            }

            best = Math.Max(best, PopCount(mask) + bestFrom((row + 1, mask)));
        }

        return best;
    }

    private static int PopCount(int mask)
    {
        var count = 0;
        while (mask != 0)
        {
            mask &= mask - 1;
            count++;
        }

        return count;
    }
}
