using DSAExperimentation.Algorithms.DynamicProgramming;

namespace DSAExperimentation.LeetCode.SelectCellsInGridWithMaximumScore;

// LeetCode 3276. Select Cells in Grid With Maximum Score: pick at most one cell
// per row so every selected value is globally unique, maximizing their sum. This
// is maximum-weight bipartite matching between rows and distinct values (an edge
// row-to-value exists, weighted by the value itself, whenever that value appears
// somewhere in that row) - grid.length, grid[i].length <= 10 keeps a bitmask over
// ROWS (not values, which can run to 100) small enough to memoize over.
//
// The naive recursion below can't be memoized as written: its state is
// (row, set-of-used-values), and the used-value set has no small closed range to
// bound it to. The composed strategy's insight is re-indexing the very same
// choice by DISTINCT VALUE instead of by row, which shrinks the "already
// committed" half of the state from an unbounded value-set down to a <= 10-bit
// row mask - exactly the shape this repo's own Memoizer can cache.
internal static class SelectCellsInGridWithMaximumScoreSolution
{
    // Textbook baseline: walk the grid row by row, either skipping the row or
    // trying each of its distinct not-yet-used values, backtracking through a
    // plain BCL HashSet<int>. No memoization is possible over this state shape
    // (see the class comment), so this genuinely revisits the same row/remaining-
    // rows situation once per distinct used-value set that reaches it.
    public static int MaxScoreByBruteForceRecursion(int[][] grid) => SearchRow(grid, row: 0, usedValues: []);

    private static int SearchRow(int[][] grid, int row, HashSet<int> usedValues)
    {
        if (row == grid.Length)
        {
            return 0;
        }

        var best = SearchRow(grid, row + 1, usedValues);

        foreach (var value in new HashSet<int>(grid[row]))
        {
            if (usedValues.Add(value))
            {
                best = Math.Max(best, value + SearchRow(grid, row + 1, usedValues));
                usedValues.Remove(value);
            }
        }

        return best;
    }

    // This repo's own Memoizer over (ValueIndex, RowMask): every distinct value in
    // the grid either contributes to the score (claiming one of the rows it
    // appears in, if that row is still free) or is skipped entirely.
    public static int MaxScoreByBitmaskMemoization(int[][] grid) => MaxScoreByBitmaskMemoization(GroupRowsByValue(grid));

    public static int MaxScoreByBitmaskMemoization(Dictionary<int, List<int>> rowsByValue)
    {
        var values = rowsByValue.Keys.ToArray();

        int Recurrence((int ValueIndex, int RowMask) state, Func<(int, int), int> best)
        {
            var (valueIndex, rowMask) = state;

            if (valueIndex == values.Length)
            {
                return 0;
            }

            var skip = best((valueIndex + 1, rowMask));
            var value = values[valueIndex];
            var claimed = skip;

            foreach (var row in rowsByValue[value])
            {
                var bit = 1 << row;

                if ((rowMask & bit) == 0)
                {
                    claimed = Math.Max(claimed, value + best((valueIndex + 1, rowMask | bit)));
                }
            }

            return claimed;
        }

        return Memoizer.Memoize<(int ValueIndex, int RowMask), int>((0, 0), Recurrence);
    }

    // Every distinct value mapped to the rows it appears in - the input the
    // bitmask strategy's recursion actually walks, so a benchmark's
    // [GlobalSetup] can charge this grouping to setup instead of the measured DP.
    public static Dictionary<int, List<int>> GroupRowsByValue(int[][] grid)
    {
        var rowsByValue = new Dictionary<int, List<int>>();

        for (var row = 0; row < grid.Length; row++)
        {
            foreach (var value in new HashSet<int>(grid[row]))
            {
                if (!rowsByValue.TryGetValue(value, out var rows))
                {
                    rows = [];
                    rowsByValue[value] = rows;
                }

                rows.Add(row);
            }
        }

        return rowsByValue;
    }
}
