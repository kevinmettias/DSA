using DSAExperimentation.Algorithms.Traversal.DepthFirst;
using DSAExperimentation.DataStructures.Set;

namespace DSAExperimentation.LeetCode.PacificAtlanticWaterFlow;

// LeetCode 417. Pacific Atlantic Water Flow: every cell that can flow downhill to
// BOTH the Pacific border (top row / left column) and the Atlantic border (bottom
// row / right column).
//
// The two strategies differ in direction and in how many times each cell is
// visited. The baseline walks forward from every interior cell, independently,
// re-scanning with a freshly allocated visited grid each time. The composed
// strategy walks backward instead: "can flow downhill from X to a border cell" is
// exactly the reverse of "can walk uphill-or-flat from that border cell to X", so
// one multi-source flood fill per ocean - this repo's own DepthFirstSearch.Traverse
// seeded at every border cell, deduped into a Set<(int,int)> - visits each cell at
// most once per ocean regardless of how many interior cells are queried.
internal static class PacificAtlanticWaterFlowSolution
{
    private static readonly (int DRow, int DCol)[] Directions = [(1, 0), (-1, 0), (0, 1), (0, -1)];

    // The textbook answer: for every cell, independently DFS downhill toward each
    // ocean's border with a freshly-allocated visited grid - O(rows*cols) work
    // times rows*cols starting cells. Deliberately written without this repo's
    // primitives - it is the arm the composed solution below has to justify
    // itself against.
    public static List<(int Row, int Col)> FindCellsByPerCellDfs(int[][] heights)
    {
        var rows = heights.Length;
        var cols = heights[0].Length;
        var result = new List<(int Row, int Col)>();

        for (var r = 0; r < rows; r++)
        {
            for (var c = 0; c < cols; c++)
            {
                if (CanReachBorder(r, c, Ocean.Pacific, heights) && CanReachBorder(r, c, Ocean.Atlantic, heights))
                {
                    result.Add((r, c));
                }
            }
        }

        return result;
    }

    private static bool CanReachBorder(int startRow, int startCol, Ocean ocean, int[][] heights)
    {
        var visited = new bool[heights.Length, heights[0].Length];
        var search = new DownhillSearch(heights, visited, ocean);

        return HasPathToBorder(startRow, startCol, search);
    }

    // This repo's own DepthFirstSearch.Traverse, run once per border cell walking
    // "uphill or flat" outward - the reverse direction of the baseline's downhill
    // walk - deduped into two Set<(int,int)> reachability sets instead of
    // SurroundedRegions' in-place board mutation (heights must stay untouched here).
    public static List<(int Row, int Col)> FindCellsByMultiSourceFloodFill(int[][] heights)
    {
        var rows = heights.Length;
        var cols = heights[0].Length;
        var pacific = new Set<(int Row, int Col)>();
        var atlantic = new Set<(int Row, int Col)>();

        for (var r = 0; r < rows; r++)
        {
            FloodFrom((r, 0), pacific, heights);
            FloodFrom((r, cols - 1), atlantic, heights);
        }

        for (var c = 0; c < cols; c++)
        {
            FloodFrom((0, c), pacific, heights);
            FloodFrom((rows - 1, c), atlantic, heights);
        }

        return CellsReachingBothOceans(heights, pacific, atlantic);
    }

    private static void FloodFrom((int Row, int Col) start, Set<(int Row, int Col)> reached, int[][] heights)
    {
        if (reached.Has(start))
        {
            return;
        }

        foreach (var node in DepthFirstSearch.Traverse(start, cell => Neighbors(cell, heights)))
        {
            reached.TryAdd(node);
        }
    }

    private static IEnumerable<(int Row, int Col)> Neighbors((int Row, int Col) cell, int[][] heights)
    {
        var rows = heights.Length;
        var cols = heights[0].Length;

        foreach (var (dRow, dCol) in Directions)
        {
            var next = (Row: cell.Row + dRow, Col: cell.Col + dCol);

            if (!IsInside(next.Row, next.Col, rows, cols))
            {
                continue;
            }

            if (heights[next.Row][next.Col] < heights[cell.Row][cell.Col])
            {
                continue;
            }

            yield return next;
        }
    }

    private static List<(int Row, int Col)> CellsReachingBothOceans(
        int[][] heights, Set<(int Row, int Col)> pacific, Set<(int Row, int Col)> atlantic)
    {
        var result = new List<(int Row, int Col)>();

        for (var r = 0; r < heights.Length; r++)
        {
            for (var c = 0; c < heights[0].Length; c++)
            {
                if (pacific.Has((r, c)) && atlantic.Has((r, c)))
                {
                    result.Add((r, c));
                }
            }
        }

        return result;
    }

    private static bool HasPathToBorder(int row, int col, DownhillSearch search)
    {
        if (search.Visited[row, col])
        {
            return false;
        }

        search.Visited[row, col] = true;

        if (IsBorderCell(row, col, search.Ocean, search.Heights))
        {
            return true;
        }

        return TryReachAnyDirection(row, col, search);
    }

    private static bool IsBorderCell(int row, int col, Ocean ocean, int[][] heights)
    {
        if (ocean == Ocean.Pacific)
        {
            return row == 0 || col == 0;
        }

        return row == heights.Length - 1 || col == heights[0].Length - 1;
    }

    private static bool TryReachAnyDirection(int row, int col, DownhillSearch search)
    {
        foreach (var direction in Directions)
        {
            if (TryReachViaDirection((row, col), direction, search))
            {
                return true;
            }
        }

        return false;
    }

    private static bool TryReachViaDirection(
        (int Row, int Col) from, (int DRow, int DCol) direction, DownhillSearch search)
    {
        var nextRow = from.Row + direction.DRow;
        var nextCol = from.Col + direction.DCol;

        if (!CanStepTo(search.Heights, from, (nextRow, nextCol), search.Visited))
        {
            return false;
        }

        return HasPathToBorder(nextRow, nextCol, search);
    }

    // A step is open when the cell one move away lies inside the grid, has not been
    // visited, and is no higher than the cell it came from - which is the reverse,
    // uphill-or-flat walk the flood fill makes. The bounds test comes first because the
    // visited grid cannot be indexed until it has passed.
    private static bool CanStepTo(
        int[][] heights, (int Row, int Col) from, (int Row, int Col) next, bool[,] visited)
    {
        if (!IsInside(next.Row, next.Col, heights.Length, heights[0].Length) || visited[next.Row, next.Col])
        {
            return false;
        }

        return heights[next.Row][next.Col] <= heights[from.Row][from.Col];
    }

    // Both coordinates within the grid is one idea, and both walks over neighbors
    // ask for its negation.
    private static bool IsInside(int row, int col, int rows, int cols)
        => row >= 0 && row < rows && col >= 0 && col < cols;

    // Which ocean's border the downhill walk is trying to reach, named where the
    // rewritten `true`/`false` at the call site said it only by position.
    private enum Ocean
    {
        Pacific,
        Atlantic,
    }

    // The three values every level of the downhill recursion threads: the read-only
    // height grid it walks, the visited marks one walk fills in, and the ocean whose
    // border that walk is trying to reach. They travel together, so they travel as
    // one value - which is also what keeps HasPathToBorder and its two helpers within the
    // parameter-count limit.
    private readonly record struct DownhillSearch(int[][] Heights, bool[,] Visited, Ocean Ocean);
}
