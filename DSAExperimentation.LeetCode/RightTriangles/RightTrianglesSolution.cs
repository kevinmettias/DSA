using DSAExperimentation.DataStructures.HashMap;

namespace DSAExperimentation.LeetCode.RightTriangles;

// LeetCode 3128. Right Triangles: count triples of value-1 cells where one is in
// the same row as a second and the same column as a third. Every 1-cell can be the
// right-angle corner exactly once, contributing (ones elsewhere in its row) times
// (ones elsewhere in its column) triangles, so the answer is the sum of that
// product over every 1-cell.
//
// Both strategies agree on that identity; they differ only in whether each
// corner's row/column one-counts are freshly rescanned, or tallied once and looked
// up - the same "tally once, answer many queries" shape CountServersThatCommunicate
// (LC 1267) uses for its own row/column counts, composed here with this repo's own
// HashMap<int,int> in place of a BCL Dictionary.
internal static class RightTrianglesSolution
{
    // Textbook baseline: for every 1-cell, rescans its whole row and whole column
    // from scratch to count the other ones sharing it - no tallying, so every
    // corner repeats work every other corner in the same row or column already
    // did. The arm the tallied strategy below has to justify itself against.
    public static long CountByBruteForceRowColumnScan(int[][] grid)
    {
        var rows = grid.Length;
        var cols = grid[0].Length;
        var count = 0L;

        for (var r = 0; r < rows; r++)
        {
            for (var c = 0; c < cols; c++)
            {
                if (grid[r][c] == 0)
                {
                    continue;
                }

                count += (long)(RowOnes(grid, r, cols) - 1) * (ColumnOnes(grid, c, rows) - 1);
            }
        }

        return count;
    }

    private static int RowOnes(int[][] grid, int row, int cols)
    {
        var ones = 0;

        for (var c = 0; c < cols; c++)
        {
            ones += grid[row][c];
        }

        return ones;
    }

    private static int ColumnOnes(int[][] grid, int col, int rows)
    {
        var ones = 0;

        for (var r = 0; r < rows; r++)
        {
            ones += grid[r][col];
        }

        return ones;
    }

    // This repo's own HashMap<int,int>, one tallying ones per row and one per
    // column in a single pass; a second pass turns every 1-cell's
    // (row tally - 1) * (column tally - 1) into the answer, so no row or column is
    // ever rescanned.
    public static long CountByTalliedRowsAndColumns(int[][] grid)
    {
        var rowOnes = new HashMap<int, int>();
        var colOnes = new HashMap<int, int>();
        TallyOnes(grid, rowOnes, colOnes);

        return SumCorners(grid, rowOnes, colOnes);
    }

    private static void TallyOnes(int[][] grid, HashMap<int, int> rowOnes, HashMap<int, int> colOnes)
    {
        for (var r = 0; r < grid.Length; r++)
        {
            for (var c = 0; c < grid[0].Length; c++)
            {
                if (grid[r][c] == 1)
                {
                    Increment(rowOnes, r);
                    Increment(colOnes, c);
                }
            }
        }
    }

    private static void Increment(HashMap<int, int> counts, int key)
    {
        counts.TryGetValue(key, out var current);
        counts.Set(key, current + 1);
    }

    private static long SumCorners(int[][] grid, HashMap<int, int> rowOnes, HashMap<int, int> colOnes)
    {
        var count = 0L;

        for (var r = 0; r < grid.Length; r++)
        {
            for (var c = 0; c < grid[0].Length; c++)
            {
                if (grid[r][c] == 1)
                {
                    rowOnes.TryGetValue(r, out var rowCount);
                    colOnes.TryGetValue(c, out var colCount);
                    count += (long)(rowCount - 1) * (colCount - 1);
                }
            }
        }

        return count;
    }
}
