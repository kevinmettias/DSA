using DSAExperimentation.Algorithms.Traversal.DepthFirst;

namespace DSAExperimentation.LeetCode.NumberOfClosedIslands;

// LeetCode 1254. Number of Closed Islands: count the 4-connected components of
// water ('0') cells that are entirely surrounded by land ('1') - a component
// counts only when none of its cells sit on the grid's own edge.
//
// This is the border-flood-fill shape NumberOfIslandsSolution and
// MaxAreaOfIslandSolution already use for LC 200/695, with the interior-vs-border
// distinction SurroundedRegionsSolution makes, counting components instead of
// relabelling them. Both strategies claim cells by filling them in place, so each
// clones the grid it is handed first - a caller's own grid (or a benchmark
// fixture reused across iterations) is never left half-filled.
internal static class NumberOfClosedIslandsSolution
{
    private const int Water = 0;

    private const int Land = 1;

    private static readonly (int DRow, int DCol)[] Directions = [(1, 0), (-1, 0), (0, 1), (0, -1)];

    // The textbook answer: a hand-specialized recursive flood fill, BCL only,
    // threading "this component reached the edge" out of the recursion by ref.
    // Deliberately written without this repo's traversal primitive - it is the
    // arm the composed solution below has to justify itself against.
    public static int CountClosedIslandsByNaiveFloodFill(int[][] grid)
    {
        var working = Clone(grid);
        var count = 0;

        for (var row = 0; row < working.Rows; row++)
        {
            for (var col = 0; col < working.Cols; col++)
            {
                if (IsClosedByFlood(working, row, col))
                {
                    count++;
                }
            }
        }

        return count;
    }

    private static bool IsClosedByFlood(LandGrid grid, int row, int col)
    {
        if (grid.Cells[row][col] != Water)
        {
            return false;
        }

        var contact = BorderContact.Enclosed;
        Flood(grid, row, col, ref contact);

        return contact == BorderContact.Enclosed;
    }

    // This repo's own DFS: DepthFirstSearch.Traverse already returns every cell
    // reachable from a start through an arbitrary successor function, so one
    // water component is a single traversal and "closed" is a predicate over the
    // cells it came back with rather than a flag threaded through recursion.
    public static int CountClosedIslandsByDepthFirstSearch(int[][] grid)
    {
        var working = Clone(grid);
        var count = 0;

        for (var row = 0; row < working.Rows; row++)
        {
            for (var col = 0; col < working.Cols; col++)
            {
                if (IsClosedByTraversal(working, row, col))
                {
                    count++;
                }
            }
        }

        return count;
    }

    private static bool IsClosedByTraversal(LandGrid grid, int row, int col)
    {
        if (grid.Cells[row][col] != Water)
        {
            return false;
        }

        var island = DepthFirstSearch.Traverse((row, col), cell => WaterNeighbors(grid, cell));
        var closed = true;

        foreach (var (islandRow, islandCol) in island)
        {
            if (IsOnBorder(grid, islandRow, islandCol))
            {
                closed = false;
            }

            grid.Cells[islandRow][islandCol] = Land;
        }

        return closed;
    }

    private static IEnumerable<(int Row, int Col)> WaterNeighbors(LandGrid grid, (int Row, int Col) cell)
    {
        foreach (var (dRow, dCol) in Directions)
        {
            var next = (Row: cell.Row + dRow, Col: cell.Col + dCol);

            if (!IsInside(grid, next.Row, next.Col))
            {
                continue;
            }

            if (grid.Cells[next.Row][next.Col] != Water)
            {
                continue;
            }

            yield return next;
        }
    }

    private static void Flood(LandGrid grid, int row, int col, ref BorderContact contact)
    {
        if (!IsInside(grid, row, col) || grid.Cells[row][col] != Water)
        {
            return;
        }

        grid.Cells[row][col] = Land;

        if (IsOnBorder(grid, row, col))
        {
            contact = BorderContact.TouchesBorder;
        }

        Flood(grid, row + 1, col, ref contact);
        Flood(grid, row - 1, col, ref contact);
        Flood(grid, row, col + 1, ref contact);
        Flood(grid, row, col - 1, ref contact);
    }

    // Both coordinates within the board is one idea, and two scans here ask it.
    private static bool IsInside(LandGrid grid, int row, int col) =>
        row >= 0 && row < grid.Rows && col >= 0 && col < grid.Cols;

    private static bool IsOnBorder(LandGrid grid, int row, int col) =>
        row == 0 || row == grid.Rows - 1 || col == 0 || col == grid.Cols - 1;

    private static LandGrid Clone(int[][] grid)
    {
        var cells = new int[grid.Length][];

        for (var row = 0; row < grid.Length; row++)
        {
            cells[row] = (int[])grid[row].Clone();
        }

        return new LandGrid(cells, cells.Length, cells[0].Length);
    }

    // The grid plus its own dimensions, so every walk below reads the bounds it
    // is checking against off one value instead of re-deriving them per call.
    private readonly record struct LandGrid(int[][] Cells, int Rows, int Cols);

    // What the flood has learned about the component it is walking, named where a
    // rewritten `true`/`false` at the call site said it only by position: Enclosed
    // until the first cell on the grid's edge, TouchesBorder from then on.
    private enum BorderContact
    {
        Enclosed,
        TouchesBorder,
    }
}
