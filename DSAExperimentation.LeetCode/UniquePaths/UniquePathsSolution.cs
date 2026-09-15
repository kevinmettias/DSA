using DSAExperimentation.Algorithms.DynamicProgramming;

namespace DSAExperimentation.LeetCode.UniquePaths;

// LeetCode 62. Unique Paths: an m x n grid's top-left robot can only move right or
// down; count the distinct paths to the bottom-right corner.
//
// Both strategies count the same quantity - reaching the corner is always exactly
// m-1 downward steps and n-1 rightward steps in some order, so the answer is the
// binomial coefficient C(m+n-2, m-1). The combinatorics arm computes that closed
// form directly; the memoized recurrence rediscovers the same count by summing, at
// each cell, the paths from its right and down neighbors to the destination,
// caching each cell's result so it is computed once rather than exponentially
// many times.
internal static class UniquePathsSolution
{
    // Textbook baseline: the closed-form binomial coefficient
    // C(m+n-2, min(m,n)-1), built up incrementally so every intermediate product
    // stays an exact integer (no factorial overflow, no floating point).
    public static int CountPathsByCombinatorics(int m, int n)
    {
        var totalSteps = m + n - 2;
        var smaller = Math.Min(m, n) - 1;
        long result = 1;

        for (var i = 1; i <= smaller; i++)
        {
            result = result * (totalSteps - smaller + i) / i;
        }

        return (int)result;
    }

    // Memoizer caches the grid recurrence from each cell to the bottom-right
    // destination, so the exponential branching of "right or down" collapses to
    // one evaluation per cell.
    public static int CountPathsByMemoizedRecurrence(int m, int n) =>
        Memoizer.Memoize<(int Row, int Col), int>((0, 0), new PathsFromCellToCorner(m, n));

    // The recurrence, named: the destination cell is itself one path, and any other
    // cell adds up the paths from the neighbours it can still step to. The grid's
    // shape belongs to the caller and never varies during a run, so the two
    // dimensions travel in as constructor state rather than being re-supplied at
    // every level.
    private sealed class PathsFromCellToCorner(int rows, int columns)
        : IRecurrence<(int Row, int Col), int>
    {
        /// <inheritdoc/>
        public int Replay((int Row, int Col) state, IRecurrence<(int Row, int Col), int> rest)
        {
            var (row, col) = state;

            if (row == rows - 1 && col == columns - 1)
            {
                return 1;
            }

            var total = 0;

            if (row + 1 < rows)
            {
                total += rest.Replay((row + 1, col), rest);
            }

            if (col + 1 < columns)
            {
                total += rest.Replay((row, col + 1), rest);
            }

            return total;
        }
    }
}
