using DSAExperimentation.Algorithms.Connectivity;
using DSAExperimentation.Algorithms.Reducing;
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
// than the cell count. The baseline visits cells by zeroing them in place, so it
// clones the grid it is handed first, and a caller's own grid (or a benchmark
// fixture reused across iterations) is never left half-zeroed. The composed arm
// never writes to the grid: its walks share one visited set instead.
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

    // This repo's ConnectedComponents.ReduceEach over GridTopology: each water
    // component's walk adds up its own cells' fish with FishTotal, the component's score.
    public static int MaxFishByConnectedComponents(int[][] grid)
    {
        var water = Grid.Where(GridSize.Of(grid), new WaterCell(grid)).PassableNodes();
        var catches = ConnectedComponents.ReduceEach<
            GridNode, GridTopology, GridChildren, DepthFirstReduceOrder<GridNode>, FishTotal, FishCatch>(
            water, new FishCatch(grid, 0));

        return catches.Select(fishCatch => fishCatch.Total).DefaultIfEmpty(0).Max();
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

    // The cells a component is made of: water, which holds at least one fish.
    private readonly struct WaterCell(int[][] grid) : IGridCellFilter
    {
        private readonly int[][] _grid = grid;

        public bool CanEnter(int row, int col) => _grid[row][col] > 0;
    }

    // One component's catch so far, with the grid its fish are read from: a reduce algebra
    // is static, so what it reads has to travel in the state it threads.
    private readonly record struct FishCatch(int[][] Grid, int Total);

    // Adds each cell's fish as the walk enters it.
    private readonly struct FishTotal : IReduceAlgebra<GridNode, FishCatch>
    {
        public static FishCatch Seed => default;

        public static FishCatch Enter(FishCatch state, GridNode node, int depth)
            => state with { Total = state.Total + state.Grid[node.Row][node.Col] };
    }
}
