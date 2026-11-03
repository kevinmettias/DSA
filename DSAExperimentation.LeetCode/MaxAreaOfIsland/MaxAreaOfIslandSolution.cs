using DSAExperimentation.Algorithms.Traversal.DepthFirst;
using DSAExperimentation.DataStructures.Graph.Grids;

namespace DSAExperimentation.LeetCode.MaxAreaOfIsland;

// LeetCode 695. Max Area of Island: flood-fill each unvisited land cell's
// connected component under 4-directional adjacency and keep the largest area
// seen. Both strategies visit cells by zeroing them in place, so each strategy
// clones the grid it is handed first - the same clone-before-mutating shape the
// original benchmark arms used, now made unconditional so a caller's own grid (or
// a benchmark fixture reused across iterations) is never left half-zeroed.
internal static class MaxAreaOfIslandSolution
{
    // The textbook answer: a hand-specialized recursive flood fill, BCL only.
    // Deliberately written without this repo's traversal primitive - it is the arm
    // the composed solution below has to justify itself against.
    public static int MaxAreaByNaiveFloodFill(int[][] grid)
    {
        var working = CloneGrid(grid);
        var rows = working.Length;
        var cols = working[0].Length;
        var best = 0;

        for (var r = 0; r < rows; r++)
        {
            for (var c = 0; c < cols; c++)
            {
                if (working[r][c] == 1)
                {
                    var area = Flood(working, r, c);
                    best = Math.Max(best, area);
                }
            }
        }

        return best;
    }

    // This repo's own DFS: DepthFirstSearch.Traverse walks one island's full land
    // component from each unvisited land cell, and the traversal's own reachable-
    // node count (instead of a separate reachability set) is the island's area -
    // the same border-flood-fill primitive PacificAtlanticWaterFlow/SurroundedRegions
    // already use for LC 417/130.
    public static int MaxAreaByDepthFirstSearch(int[][] grid)
    {
        var working = CloneGrid(grid);
        var rows = working.Length;
        var cols = working[0].Length;
        var best = 0;

        for (var r = 0; r < rows; r++)
        {
            for (var c = 0; c < cols; c++)
            {
                var area = IslandAreaAt(working, r, c);
                best = Math.Max(best, area);
            }
        }

        return best;
    }

    // One island's area: walk its whole land component from (rowIndex, columnIndex)
    // and zero it, so a cell that belongs to this island is not walked a second time.
    private static int IslandAreaAt(int[][] working, int rowIndex, int columnIndex)
    {
        if (working[rowIndex][columnIndex] != 1)
        {
            return 0;
        }

        var size = GridSize.Of(working);
        var land = new LandCell(working);
        var island = DepthFirstSearch.Traverse(
            (rowIndex, columnIndex), cell => GridNeighbors.Of(cell, size, GridDirections.Orthogonal, land));

        foreach (var (row, col) in island)
        {
            working[row][col] = 0;
        }

        return island.Count;
    }

    // Whether the cell lies on the board at all.
    private static bool IsOnBoard((int Row, int Col) cell, int rows, int cols)
        => cell.Row >= 0 && cell.Row < rows && cell.Col >= 0 && cell.Col < cols;

    // The grid being flooded already knows how far it reaches, so the bounds are read
    // back off it here rather than travelling down every recursive call.
    private static int Flood(int[][] grid, int row, int col)
    {
        var rows = grid.Length;
        var cols = grid[0].Length;

        if (!IsOnBoard((row, col), rows, cols) || grid[row][col] != 1)
        {
            return 0;
        }

        grid[row][col] = 0;

        return 1
            + Flood(grid, row + 1, col)
            + Flood(grid, row - 1, col)
            + Flood(grid, row, col + 1)
            + Flood(grid, row, col - 1);
    }

    private static int[][] CloneGrid(int[][] grid)
    {
        var clone = new int[grid.Length][];

        for (var r = 0; r < grid.Length; r++)
        {
            clone[r] = (int[])grid[r].Clone();
        }

        return clone;
    }

    // The cells the island's walk may step into: land not yet zeroed.
    private readonly struct LandCell(int[][] grid) : IGridCellFilter
    {
        public bool CanEnter(int row, int col) => grid[row][col] == 1;
    }
}
