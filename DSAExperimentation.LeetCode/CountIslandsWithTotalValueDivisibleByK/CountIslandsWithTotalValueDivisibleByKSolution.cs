using DSAExperimentation.Algorithms.Connectivity;
using DSAExperimentation.Algorithms.Reducing;
using DSAExperimentation.DataStructures.Graph.Grids;

namespace DSAExperimentation.LeetCode.CountIslandsWithTotalValueDivisibleByK;

// LeetCode 3619. Count Islands With Total Value Divisible by K: flood-fill every
// 4-directionally connected group of positive cells, sum its values, and count
// the groups whose sum is a multiple of divisor.
internal static class CountIslandsWithTotalValueDivisibleByKSolution
{
    private static readonly (int DeltaRow, int DeltaCol)[] Orthogonal = [(-1, 0), (1, 0), (0, -1), (0, 1)];

    // The textbook flood fill: a hand-rolled BCL Stack<T> and a visited bool[,],
    // walking one island at a time - the arm the component reduce below has to
    // justify itself against.
    public static int CountByFloodFillStack(int[][] grid, int divisor)
    {
        var rows = grid.Length;
        var visited = new bool[rows, grid[0].Length];
        var islands = 0;

        for (var row = 0; row < rows; row++)
        {
            for (var col = 0; col < grid[row].Length; col++)
            {
                if (grid[row][col] <= 0 || visited[row, col])
                {
                    continue;
                }

                if (FloodFillTotal(grid, visited, row, col) % divisor == 0)
                {
                    islands++;
                }
            }
        }

        return islands;
    }

    private static long FloodFillTotal(int[][] grid, bool[,] visited, int startRow, int startCol)
    {
        var pending = new Stack<(int Row, int Col)>();
        pending.Push((startRow, startCol));
        visited[startRow, startCol] = true;
        var total = 0L;

        while (pending.TryPop(out var cell))
        {
            total += grid[cell.Row][cell.Col];
            PushUnvisitedNeighbors(grid, visited, pending, cell);
        }

        return total;
    }

    // Claim every orthogonal land neighbor of `cell` that no island has claimed yet,
    // so the flood fill reaches it in its own time.
    private static void PushUnvisitedNeighbors(
        int[][] grid, bool[,] visited, Stack<(int Row, int Col)> pending, (int Row, int Col) cell)
    {
        foreach (var (deltaRow, deltaCol) in Orthogonal)
        {
            var nextRow = cell.Row + deltaRow;
            var nextCol = cell.Col + deltaCol;

            if (IsUnvisitedLand(grid, visited, nextRow, nextCol))
            {
                visited[nextRow, nextCol] = true;
                pending.Push((nextRow, nextCol));
            }
        }
    }

    private static bool IsUnvisitedLand(int[][] grid, bool[,] visited, int row, int col)
        => row >= 0 && row < grid.Length && col >= 0 && col < grid[row].Length &&
           !visited[row, col] && grid[row][col] > 0;

    // This repo's ConnectedComponents.ReduceEach over GridTopology: the positive cells
    // become a Grid, and each island's walk adds up its own cells' values with
    // IslandTotal. A GridNode carries only where a cell is, so the values are read from
    // the grid the state carries.
    public static int CountByConnectedComponents(int[][] grid, int divisor)
    {
        var land = Grid.Where(GridSize.Of(grid), new PositiveCell(grid)).PassableNodes();
        var islands = ConnectedComponents.ReduceEach<
            GridNode, GridTopology, GridChildren, DepthFirstReduceOrder<GridNode>, IslandTotal, IslandValue>(
            land, new IslandValue(grid, 0));

        return islands.Count(island => island.Total % divisor == 0);
    }

    // A cell of the island worth walking to: positive land rather than a zero or negative gap.
    private readonly struct PositiveCell(int[][] grid) : IGridCellFilter
    {
        private readonly int[][] _grid = grid;

        public bool CanEnter(int row, int col) => _grid[row][col] > 0;
    }

    // One island's total so far, with the grid its values are read from: a reduce algebra
    // is static, so what it reads has to travel in the state it threads.
    private readonly record struct IslandValue(int[][] Grid, long Total);

    // Adds each cell's value as the walk enters it.
    private readonly struct IslandTotal : IReduceAlgebra<GridNode, IslandValue>
    {
        public static IslandValue Seed => default;

        public static IslandValue Enter(IslandValue state, GridNode node, int depth)
            => state with { Total = state.Total + state.Grid[node.Row][node.Col] };
    }
}
