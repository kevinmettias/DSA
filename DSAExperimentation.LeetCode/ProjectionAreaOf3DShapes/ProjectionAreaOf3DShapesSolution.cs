namespace DSAExperimentation.LeetCode.ProjectionAreaOf3DShapes;

// LeetCode 883. Projection Area of 3D Shapes: grid[r][c] is the height of the stack
// of unit cubes at cell (r, c). Sum the areas of the three orthographic projections -
// top (one unit per occupied cell), front (each ROW's tallest stack), side (each
// COLUMN's tallest stack).
//
// No repo Representation/Operations primitive applies - the same "nothing to compose"
// precedent Spiral Matrix / Spiral Matrix II already establish - so every strategy
// here is a plain grid reduction and the only thing that varies between them is how
// many times the grid is walked. All three are O(rows * cols); the difference they
// isolate is redundant-pass overhead, not an algorithm class.
internal static class ProjectionAreaOf3DShapesSolution
{
    // Textbook baseline: one independent full-grid pass per projection - count the
    // nonzero cells, then a per-row pass for front, then a per-column pass for side.
    // Three walks over the same data, which is the arm the fused strategies below have
    // to justify themselves against.
    public static int ProjectionAreaByThreeSeparatePasses(int[][] grid)
    {
        var rows = grid.Length;
        var cols = grid[0].Length;

        var top = CountTopView(grid, rows, cols);
        var front = SumFrontView(grid, rows, cols);
        var side = SumSideView(grid, rows, cols);

        return top + front + side;
    }

    private static int CountTopView(int[][] grid, int rows, int cols)
    {
        var top = 0;

        for (var r = 0; r < rows; r++)
        {
            for (var c = 0; c < cols; c++)
            {
                if (grid[r][c] > 0)
                {
                    top++;
                }
            }
        }

        return top;
    }

    private static int SumFrontView(int[][] grid, int rows, int cols)
    {
        var front = 0;

        for (var r = 0; r < rows; r++)
        {
            var rowMax = 0;

            for (var c = 0; c < cols; c++)
            {
                rowMax = Math.Max(rowMax, grid[r][c]);
            }

            front += rowMax;
        }

        return front;
    }

    // Two passes: top and front share the row-major walk (both are decided by the
    // cells of one row), while side keeps its own column-major walk. No scratch array,
    // but the grid is still read twice - the middle ground between the baseline's
    // three walks and the fused single walk below.
    public static int ProjectionAreaByRowAndColumnPasses(int[][] grid)
    {
        var rows = grid.Length;
        var cols = grid[0].Length;
        var (top, front) = SumTopAndFrontView(grid, rows, cols);
        var side = SumSideView(grid, rows, cols);

        return top + front + side;
    }

    private static (int Top, int Front) SumTopAndFrontView(int[][] grid, int rows, int cols)
    {
        var top = 0;
        var front = 0;

        for (var r = 0; r < rows; r++)
        {
            var rowMax = 0;

            for (var c = 0; c < cols; c++)
            {
                if (grid[r][c] > 0)
                {
                    top++;
                }

                rowMax = Math.Max(rowMax, grid[r][c]);
            }

            front += rowMax;
        }

        return (top, front);
    }

    // One pass: the column maxima the side view needs are accumulated into a scratch
    // colMax[] as the row-major walk goes, so every cell is read exactly once and the
    // only extra work is a final sweep over cols entries.
    public static int ProjectionAreaBySingleCombinedPass(int[][] grid)
    {
        var rows = grid.Length;
        var cols = grid[0].Length;
        var colMax = new int[cols];
        var top = 0;
        var front = 0;

        for (var r = 0; r < rows; r++)
        {
            var rowState = ScanRow(grid, r, colMax, top);
            top = rowState.Top;
            front += rowState.RowMax;
        }

        return top + front + SumColumnMaxima(colMax);
    }

    // cols is the caller's own grid[0].Length - every row of the projection grid has
    // the same width - so the scan reads it off the grid rather than taking it again.
    private static RowScanState ScanRow(int[][] grid, int r, int[] colMax, int top)
    {
        var cols = grid[0].Length;
        var rowState = new RowScanState(top, 0);

        for (var c = 0; c < cols; c++)
        {
            rowState = ScanCell(grid, (Row: r, Col: c), colMax, rowState);
        }

        return rowState;
    }

    // A cell is one coordinate: neither index is ever supplied without the other.
    private static RowScanState ScanCell(
        int[][] grid, (int Row, int Col) cell, int[] colMax, RowScanState state)
    {
        var value = grid[cell.Row][cell.Col];
        var top = state.Top + (value > 0 ? 1 : 0);
        var rowMax = Math.Max(state.RowMax, value);
        colMax[cell.Col] = Math.Max(colMax[cell.Col], value);

        return new RowScanState(top, rowMax);
    }

    private static int SumColumnMaxima(int[] colMax)
    {
        var side = 0;

        foreach (var max in colMax)
        {
            side += max;
        }

        return side;
    }

    private static int SumSideView(int[][] grid, int rows, int cols)
    {
        var side = 0;

        for (var c = 0; c < cols; c++)
        {
            var colMax = 0;

            for (var r = 0; r < rows; r++)
            {
                colMax = Math.Max(colMax, grid[r][c]);
            }

            side += colMax;
        }

        return side;
    }

    // The two running totals a row-major scan carries forward: the occupied-cell count
    // so far, and the tallest stack seen in the row being scanned.
    private readonly record struct RowScanState(int Top, int RowMax);
}
