using DSAExperimentation.Algorithms.Traversal.DepthFirst;

namespace DSAExperimentation.LeetCode.MinimumNumberOfDaysToDisconnectIsland;

// LeetCode 1568. Minimum Number of Days to Disconnect Island: a grid is connected
// when it holds exactly one 4-connected island, and one day removes one land cell.
//
// The answer is always 0, 1, or 2: already disconnected (zero islands, or two or
// more), disconnectable by removing a single articulation cell, or otherwise 2 -
// remove any land cell on the island's boundary, then a land cell adjacent to what
// is left, which always peels a corner off. So every strategy is the same three-step
// decision driven by one primitive, "how many islands are there if this one cell is
// treated as water?", and the two below differ only in how that count is taken.
internal static class MinimumNumberOfDaysToDisconnectIslandSolution
{
    // LC 1568 encodes a land cell as 1 ...
    private const int Land = 1;

    // ... and calls the grid connected when it holds exactly one island. The two are
    // separate subjects that happen to share a value: one is a cell's contents, the
    // other an island count.
    private const int Connected = 1;

    // Two removals always suffice once no single cell is an articulation cell.
    private const int NoArticulationCellFound = 2;

    // A count taken with no cell removed passes this as the skipped coordinate.
    private static readonly (int Row, int Col) NoSkippedCell = (-1, -1);

    private static readonly (int DRow, int DCol)[] Directions = [(1, 0), (-1, 0), (0, 1), (0, -1)];

    // Both counts are stateless, so one instance each serves every call and neither
    // benchmark arm allocates anything to pick one.
    private static readonly IConnectedComponentCount NaiveCount = new NaiveFloodFillCount();
    private static readonly IConnectedComponentCount TraversalCount = new DepthFirstSearchCount();

    // The one query the two strategies answer differently: how many islands the grid
    // holds once a single cell is treated as water. Naming it puts that skipped cell in
    // the contract, where a bare grid-to-int callable had nowhere to say it; what counts
    // as connected stays with the caller that compares the count.
    private interface IConnectedComponentCount
    {
        int Count(IslandScan scan);
    }

    // The textbook answer: a hand-rolled recursive flood fill over a bool[,] visited
    // map, BCL only. Deliberately written without this repo's traversal primitive -
    // it is the arm the composed strategy below has to justify itself against.
    public static int MinDaysByNaiveFloodFill(int[][] grid)
        => MinDaysUsing(grid, NaiveCount);

    private sealed class NaiveFloodFillCount : IConnectedComponentCount
    {
        public int Count(IslandScan scan)
        {
            var visited = new bool[scan.Rows, scan.Cols];
            var count = 0;

            for (var row = 0; row < scan.Rows; row++)
            {
                for (var col = 0; col < scan.Cols; col++)
                {
                    if (!IsNewIslandStart(scan, visited, row, col))
                    {
                        continue;
                    }

                    count++;
                    FloodFill(scan, visited, (row, col));
                }
            }

            return count;
        }
    }

    // This repo's own DepthFirstSearch.Traverse takes the successor function that
    // treats the skipped cell as water and returns the island reachable from a start,
    // so the component count is one scan claiming each island exactly once - the same
    // composition NumberOfIslandsSolution (LC 200) and MakingALargeIslandSolution
    // (LC 827) already use for grid connectivity.
    public static int MinDaysByDepthFirstSearch(int[][] grid)
        => MinDaysUsing(grid, TraversalCount);

    private sealed class DepthFirstSearchCount : IConnectedComponentCount
    {
        public int Count(IslandScan scan)
        {
            var visited = new bool[scan.Rows, scan.Cols];
            var count = 0;

            for (var row = 0; row < scan.Rows; row++)
            {
                for (var col = 0; col < scan.Cols; col++)
                {
                    if (!IsNewIslandStart(scan, visited, row, col))
                    {
                        continue;
                    }

                    count++;
                    MarkIslandByTraversal(scan, visited, (row, col));
                }
            }

            return count;
        }
    }

    // Claim every cell of the island reachable from `start`, handing the successor
    // function that treats the skipped cell as water to this repo's own traversal
    // primitive - the one step the two counting strategies do not share.
    private static void MarkIslandByTraversal(IslandScan scan, bool[,] visited, (int Row, int Col) start)
    {
        foreach (var (islandRow, islandCol) in
                 DepthFirstSearch.Traverse(start, cell => LandNeighbors(scan, cell)))
        {
            visited[islandRow, islandCol] = true;
        }
    }

    private static IEnumerable<(int Row, int Col)> LandNeighbors(IslandScan scan, (int Row, int Col) cell)
    {
        foreach (var (dRow, dCol) in Directions)
        {
            var neighbor = (Row: cell.Row + dRow, Col: cell.Col + dCol);

            if (IsLandInside(scan, neighbor))
            {
                yield return neighbor;
            }
        }
    }

    // Both scans claim a cell as a new island's start only when it is land that is
    // neither already claimed nor the cell the scan is pretending is water.
    private static bool IsNewIslandStart(IslandScan scan, bool[,] visited, int row, int col)
        => scan.Grid[row][col] == Land && !visited[row, col] && (row, col) != scan.Skip;

    private static void FloodFill(IslandScan scan, bool[,] visited, (int Row, int Col) cell)
    {
        if (!IsLandInside(scan, cell) || visited[cell.Row, cell.Col])
        {
            return;
        }

        visited[cell.Row, cell.Col] = true;

        foreach (var (dRow, dCol) in Directions)
        {
            FloodFill(scan, visited, (cell.Row + dRow, cell.Col + dCol));
        }
    }

    // Zero days if the grid is already disconnected, one if some land cell is an
    // articulation cell, two otherwise. `countIslands` supplies the competing ways to
    // count connected components while treating one cell as water, and is the only
    // thing the two strategies differ in.
    private static int MinDaysUsing(int[][] grid, IConnectedComponentCount countIslands)
    {
        var rows = grid.Length;
        var cols = grid[0].Length;

        if (countIslands.Count(new IslandScan(grid, rows, cols, NoSkippedCell)) != Connected)
        {
            return 0;
        }

        for (var row = 0; row < rows; row++)
        {
            for (var col = 0; col < cols; col++)
            {
                if (grid[row][col] == Land &&
                    countIslands.Count(new IslandScan(grid, rows, cols, (row, col))) != Connected)
                {
                    return 1;
                }
            }
        }

        return NoArticulationCellFound;
    }

    private static bool IsLandInside(IslandScan scan, (int Row, int Col) cell)
        => cell.Row >= 0 && cell.Row < scan.Rows &&
           cell.Col >= 0 && cell.Col < scan.Cols &&
           scan.Grid[cell.Row][cell.Col] == Land &&
           cell != scan.Skip;

    // One connectivity query: the grid, its bounds, and the single cell this query
    // pretends has already been removed.
    private readonly record struct IslandScan(int[][] Grid, int Rows, int Cols, (int Row, int Col) Skip);
}
