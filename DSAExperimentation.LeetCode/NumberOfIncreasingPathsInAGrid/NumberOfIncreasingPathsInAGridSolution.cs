using DSAExperimentation.Algorithms.DynamicProgramming;
using DSAExperimentation.Domain.Modular;

namespace DSAExperimentation.LeetCode.NumberOfIncreasingPathsInAGrid;

// LeetCode 2328. Number of Increasing Paths in a Grid: count every strictly
// increasing path in the grid - a single cell is itself a path of length one -
// reported modulo 1e9+7.
//
// This is LC 329's recurrence with the fold swapped: instead of taking the longest
// path out of a cell, sum the paths out of it (1 for the trivial single-cell path,
// plus the count from each strictly greater neighbor), then sum that over every cell
// as a candidate start. A strictly-increasing value relation can never come back to a
// cell already on the path, so the induced relation is a DAG and Memoizer's
// well-founded-state precondition never trips.
//
// The grid arrives as int[,] rather than LeetCode's own int[][] so that it matches
// LongestIncreasingPathInAMatrixSolution, the sibling this problem shares both its
// recurrence and its benchmark workload with.
internal static class NumberOfIncreasingPathsInAGridSolution
{
    private static readonly (int Row, int Col)[] Directions = [(-1, 0), (1, 0), (0, -1), (0, 1)];

    // The grid together with the bounds every neighbor test needs, so the recurrences
    // below carry one value instead of three.
    private readonly record struct PathsGrid(int[,] Values, int Rows, int Cols);

    // The textbook answer: plain recursion, re-walking every shared sub-path from
    // scratch per candidate start cell. Deliberately written without this repo's
    // primitives - it is the arm the memoized strategy below has to justify itself
    // against, and here the call count itself, not just the returned value, grows with
    // the number of increasing paths (central-Delannoy-number territory on a
    // row-major-increasing grid).
    public static int CountPathsByNaiveRecursion(int[,] grid)
    {
        var pathsGrid = ToPathsGrid(grid);
        var total = 0L;

        for (var row = 0; row < pathsGrid.Rows; row++)
        {
            for (var col = 0; col < pathsGrid.Cols; col++)
            {
                total = (total + PathsFromNaive(pathsGrid, (row, col))) % ModularArithmetic.Modulo;
            }
        }

        return (int)total;
    }

    // This repo's own Memoizer caches, per cell, the number of strictly increasing
    // paths starting there - the same (Row,Col)-state grid recurrence LC 329 uses -
    // collapsing the sub-paths shared within one start's search from exponential to
    // polynomial.
    public static int CountPathsByMemoizedRecurrence(int[,] grid)
    {
        var pathsGrid = ToPathsGrid(grid);
        var total = 0L;

        for (var row = 0; row < pathsGrid.Rows; row++)
        {
            for (var col = 0; col < pathsGrid.Cols; col++)
            {
                total += Memoizer.Memoize<(int Row, int Col), long>(
                    (row, col), (state, pathsFrom) => PathsFromMemoized(pathsGrid, state, pathsFrom));
                total %= ModularArithmetic.Modulo;
            }
        }

        return (int)total;
    }

    private static long PathsFromMemoized(
        PathsGrid grid, (int Row, int Col) cell, Func<(int Row, int Col), long> pathsFrom)
    {
        var count = 1L;

        foreach (var (rowOffset, colOffset) in Directions)
        {
            var next = (Row: cell.Row + rowOffset, Col: cell.Col + colOffset);

            if (IsIncreasingStep(grid, cell, next))
            {
                count += pathsFrom(next);
            }
        }

        return count % ModularArithmetic.Modulo;
    }

    private static long PathsFromNaive(PathsGrid grid, (int Row, int Col) cell)
    {
        var count = 1L;

        foreach (var (rowOffset, colOffset) in Directions)
        {
            var next = (Row: cell.Row + rowOffset, Col: cell.Col + colOffset);

            if (IsIncreasingStep(grid, cell, next))
            {
                count += PathsFromNaive(grid, next);
            }
        }

        return count % ModularArithmetic.Modulo;
    }

    private static bool IsIncreasingStep(PathsGrid grid, (int Row, int Col) from, (int Row, int Col) to) =>
        to.Row >= 0 && to.Row < grid.Rows && to.Col >= 0 && to.Col < grid.Cols
        && grid.Values[to.Row, to.Col] > grid.Values[from.Row, from.Col];

    private static PathsGrid ToPathsGrid(int[,] grid) =>
        new(grid, grid.GetLength(0), grid.GetLength(1));
}
