using DSAExperimentation.Algorithms.Connectivity;
using DSAExperimentation.Algorithms.Reducing;
using DSAExperimentation.DataStructures.Graph.Grids;

namespace DSAExperimentation.LeetCode.MaxAreaOfIsland;

// LeetCode 695. Max Area of Island: flood-fill each unvisited land cell's
// connected component under 4-directional adjacency and keep the largest area
// seen. The baseline visits cells by zeroing them in place, so it clones the grid
// it is handed first, and a caller's own grid (or a benchmark fixture reused across
// iterations) is never left half-zeroed. The composed arm never writes to the grid:
// its walks share one visited set instead.
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

    // This repo's ConnectedComponents.ReduceEach over GridTopology: each island's walk
    // counts its own cells with NodeCountReduceAlgebra, which is its area.
    public static int MaxAreaByConnectedComponents(int[][] grid)
    {
        var land = Grid.Where(GridSize.Of(grid), new LandCell(grid)).PassableNodes();
        var areas = ConnectedComponents.ReduceEach<
            GridNode, GridTopology, GridChildren, DepthFirstReduceOrder<GridNode>, NodeCountReduceAlgebra<GridNode>, int>(land, 0);

        return areas.DefaultIfEmpty(0).Max();
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

    // The cells an island is made of.
    private readonly struct LandCell(int[][] grid) : IGridCellFilter
    {
        private readonly int[][] _grid = grid;

        public bool CanEnter(int row, int col) => _grid[row][col] == 1;
    }
}
