using DSAExperimentation.Algorithms.Traversal.DepthFirst;
using DSAExperimentation.DataStructures.Graph.Grids;

namespace DSAExperimentation.LeetCode.MaximumNumberOfFishInAGrid;

// LeetCode 2658. Maximum Number of Fish in a Grid: a fisher may start on any water
// cell and move 4-directionally between water cells, collecting every fish in that
// connected water component - so the answer is the largest per-component fish
// total. A water cell is any grid[r][c] > 0; land is exactly 0.
//
// That is the same border-agnostic flood fill MaxAreaOfIslandSolution runs for
// LC 695, over a value grid instead of a binary one: the component is found the
// same way, but the score is the sum of each visited cell's own fish count rather
// than the cell count. Both strategies visit cells by zeroing them in place, so
// each clones the grid it is handed first - a caller's own grid (or a benchmark
// fixture reused across iterations) is never left half-zeroed.
internal static class MaximumNumberOfFishInAGridSolution
{
    // The textbook answer: a hand-specialized recursive flood fill that adds each
    // cell's fish as it sinks it. Deliberately written without this repo's
    // traversal primitive - it is the arm the composed solution below has to
    // justify itself against.
    public static int MaxFishByNaiveFloodFill(int[][] grid)
    {
        var working = CloneGrid(grid);
        var bounds = new GridBounds(working.Length, working[0].Length);
        var best = 0;

        for (var r = 0; r < bounds.Rows; r++)
        {
            for (var c = 0; c < bounds.Cols; c++)
            {
                if (working[r][c] > 0)
                {
                    var floodedTotal = Flood(working, r, c, bounds);
                    best = Math.Max(best, floodedTotal);
                }
            }
        }

        return best;
    }

    // This repo's own DFS: DepthFirstSearch.Traverse returns one water component's
    // reachable cells from each unvisited water cell, and summing their fish counts
    // is the component's score. Sinking the component afterwards is what keeps the
    // outer sweep from re-walking it - the same visit-by-zeroing shape
    // MaxAreaOfIslandSolution already uses for LC 695.
    public static int MaxFishByDepthFirstSearch(int[][] grid)
    {
        var working = CloneGrid(grid);
        var rows = working.Length;
        var cols = working[0].Length;
        var best = 0;

        for (var r = 0; r < rows; r++)
        {
            for (var c = 0; c < cols; c++)
            {
                if (working[r][c] == 0)
                {
                    continue;
                }

                var componentTotal = SinkComponentFishTotal(working, r, c);
                best = Math.Max(best, componentTotal);
            }
        }

        return best;
    }

    private static int SinkComponentFishTotal(int[][] grid, int row, int col)
    {
        var size = GridSize.Of(grid);
        var water = new WaterCell(grid);
        var component = DepthFirstSearch.Traverse(
            (Row: row, Col: col), cell => GridNeighbors.Of(cell, size, GridDirections.Orthogonal, water));
        var total = component.Sum(cell => grid[cell.Row][cell.Col]);

        foreach (var (componentRow, componentCol) in component)
        {
            grid[componentRow][componentCol] = 0;
        }

        return total;
    }

    // The naive arm's recursive fill: sink the cell, bank its fish, recurse into
    // the four neighbours.
    private static int Flood(int[][] grid, int row, int col, GridBounds bounds)
    {
        var outsideGrid = row < 0 || row >= bounds.Rows || col < 0 || col >= bounds.Cols;

        if (outsideGrid || grid[row][col] == 0)
        {
            return 0;
        }

        var fish = grid[row][col];
        grid[row][col] = 0;

        return fish
            + Flood(grid, row + 1, col, bounds)
            + Flood(grid, row - 1, col, bounds)
            + Flood(grid, row, col + 1, bounds)
            + Flood(grid, row, col - 1, bounds);
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

    // The recursive fill's four bounds checks travel together on every frame, so
    // they ride as one parameter rather than two.
    private readonly record struct GridBounds(int Rows, int Cols);

    // The cells a component's walk may step into: water, which holds at least one fish.
    private readonly struct WaterCell(int[][] grid) : IGridCellFilter
    {
        public bool CanEnter(int row, int col) => grid[row][col] > 0;
    }
}
