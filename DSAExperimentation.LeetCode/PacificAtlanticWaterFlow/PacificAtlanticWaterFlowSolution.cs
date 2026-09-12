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
                if (CanReachBorder(r, c, pacific: true, heights) && CanReachBorder(r, c, pacific: false, heights))
                {
                    result.Add((r, c));
                }
            }
        }

        return result;
    }

    private static bool CanReachBorder(int startRow, int startCol, bool pacific, int[][] heights)
    {
        var visited = new bool[heights.Length, heights[0].Length];
        return Dfs(startRow, startCol, visited, pacific, heights);
    }

    private static bool Dfs(int row, int col, bool[,] visited, bool pacific, int[][] heights)
    {
        if (visited[row, col])
        {
            return false;
        }

        visited[row, col] = true;

        if (IsBorderCell(row, col, pacific, heights))
        {
            return true;
        }

        return TryReachAnyDirection(row, col, visited, pacific, heights);
    }

    private static bool IsBorderCell(int row, int col, bool pacific, int[][] heights)
    {
        if (pacific)
        {
            return row == 0 || col == 0;
        }

        return row == heights.Length - 1 || col == heights[0].Length - 1;
    }

    private static bool TryReachAnyDirection(int row, int col, bool[,] visited, bool pacific, int[][] heights)
    {
        foreach (var direction in Directions)
        {
            if (TryReachViaDirection((row, col), direction, visited, pacific, heights))
            {
                return true;
            }
        }

        return false;
    }

    private static bool TryReachViaDirection(
        (int Row, int Col) from, (int DRow, int DCol) direction, bool[,] visited, bool pacific, int[][] heights)
    {
        var nextRow = from.Row + direction.DRow;
        var nextCol = from.Col + direction.DCol;
        var rows = heights.Length;
        var cols = heights[0].Length;

        if (nextRow < 0 || nextRow >= rows || nextCol < 0 || nextCol >= cols || visited[nextRow, nextCol])
        {
            return false;
        }

        if (heights[nextRow][nextCol] > heights[from.Row][from.Col])
        {
            return false;
        }

        return Dfs(nextRow, nextCol, visited, pacific, heights);
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

    private static void FloodFrom((int Row, int Col) start, Set<(int Row, int Col)> reached, int[][] heights)
    {
        if (reached.Has(start))
        {
            return;
        }

        foreach (var node in DepthFirstSearch.Traverse(start, p => Neighbors(p, heights)))
        {
            reached.TryAdd(node);
        }
    }

    private static IEnumerable<(int Row, int Col)> Neighbors((int Row, int Col) p, int[][] heights)
    {
        var rows = heights.Length;
        var cols = heights[0].Length;

        foreach (var (dRow, dCol) in Directions)
        {
            var next = (Row: p.Row + dRow, Col: p.Col + dCol);

            if (next.Row < 0 || next.Row >= rows || next.Col < 0 || next.Col >= cols)
            {
                continue;
            }

            if (heights[next.Row][next.Col] < heights[p.Row][p.Col])
            {
                continue;
            }

            yield return next;
        }
    }
}
