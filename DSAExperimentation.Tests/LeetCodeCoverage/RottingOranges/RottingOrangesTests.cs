namespace DSAExperimentation.Tests.LeetCodeCoverage.RottingOranges;

// LeetCode 994. Rotting Oranges: multi-source BFS seeded with every already-rotten
// orange at once (minute 0), the same shape ZeroOneMatrixTests already uses for its
// multi-source distance sweep - just tracking minutes-to-rot per fresh cell instead
// of distance-to-nearest-zero, using this repo's own Queue<TElement> as the FIFO
// frontier.
public sealed partial class RottingOrangesTests
{
    [Fact]
    public void OrangesRotting_ClassicExample_ReturnsMinutesUntilAllRot()
    {
        int[][] grid = [[2, 1, 1], [1, 1, 0], [0, 1, 1]];

        Assert.Equal(4, OrangesRotting(grid));
    }

    [Fact]
    public void OrangesRotting_UnreachableFreshOrange_ReturnsNegativeOne()
    {
        int[][] grid = [[2, 1, 1], [0, 1, 1], [1, 0, 1]];

        Assert.Equal(-1, OrangesRotting(grid));
    }

    [Fact]
    public void OrangesRotting_NoFreshOranges_ReturnsZero()
    {
        int[][] grid = [[0, 2]];

        Assert.Equal(0, OrangesRotting(grid));
    }

    private static readonly (int DRow, int DCol)[] Directions = [(-1, 0), (1, 0), (0, -1), (0, 1)];

    private static int OrangesRotting(int[][] grid)
    {
        var minutesToRot = new int[grid.Length][];
        var (frontier, freshCount) = SeedFrontier(grid, minutesToRot);

        var state = new BfsState(grid, minutesToRot, frontier) { FreshCount = freshCount };
        RunBfs(state);

        return state.FreshCount == 0 ? state.MinutesElapsed : -1;
    }

    // Sizes minutesToRot to match the grid, enqueues every already-rotten cell as a
    // minute-0 BFS source, and counts the fresh oranges that still need reaching.
    private static (DSAExperimentation.DataStructures.Queue.Queue<(int Row, int Col)> Frontier, int FreshCount) SeedFrontier(
        int[][] grid, int[][] minutesToRot)
    {
        var rows = grid.Length;
        var cols = grid[0].Length;
        var frontier = new DSAExperimentation.DataStructures.Queue.Queue<(int Row, int Col)>();
        var freshCount = 0;

        for (var r = 0; r < rows; r++)
        {
            minutesToRot[r] = new int[cols];

            for (var c = 0; c < cols; c++)
            {
                freshCount += ClassifyCell(grid, frontier, r, c);
            }
        }

        return (frontier, freshCount);
    }

    // Enqueues an already-rotten cell as a minute-0 BFS source and returns 0;
    // returns 1 for a still-fresh cell so the caller can tally freshCount.
    private static int ClassifyCell(
        int[][] grid, DSAExperimentation.DataStructures.Queue.Queue<(int Row, int Col)> frontier, int r, int c)
    {
        if (grid[r][c] == 2)
        {
            frontier.Enqueue((r, c));
            return 0;
        }

        return grid[r][c] == 1 ? 1 : 0;
    }

    private static void RunBfs(BfsState state)
    {
        while (state.Frontier.TryDequeue(out var cell))
        {
            foreach (var (dRow, dCol) in Directions)
            {
                RotNeighborIfFresh(state, cell, dRow, dCol);
            }
        }
    }

    // One BFS relaxation step: rots the neighbor in direction (dRow, dCol) from
    // `cell` if it's still fresh and in bounds, updating the shared frontier/minute
    // bookkeeping in `state`.
    private static void RotNeighborIfFresh(BfsState state, (int Row, int Col) cell, int dRow, int dCol)
    {
        var nextRow = cell.Row + dRow;
        var nextCol = cell.Col + dCol;

        if (nextRow < 0 || nextRow >= state.Rows || nextCol < 0 || nextCol >= state.Cols
            || state.Grid[nextRow][nextCol] != 1)
        {
            return;
        }

        state.Grid[nextRow][nextCol] = 2;
        state.MinutesToRot[nextRow][nextCol] = state.MinutesToRot[cell.Row][cell.Col] + 1;
        state.FreshCount--;
        state.MinutesElapsed = Math.Max(state.MinutesElapsed, state.MinutesToRot[nextRow][nextCol]);
        state.Frontier.Enqueue((nextRow, nextCol));
    }

    // The grid/minute-map/frontier the BFS reads and writes, plus its running
    // fresh-orange count and elapsed-minutes bookkeeping - all mutated together
    // by RotNeighborIfFresh.
    private sealed class BfsState(
        int[][] grid, int[][] minutesToRot, DSAExperimentation.DataStructures.Queue.Queue<(int Row, int Col)> frontier)
    {
        public readonly int[][] Grid = grid;
        public readonly int[][] MinutesToRot = minutesToRot;
        public readonly DSAExperimentation.DataStructures.Queue.Queue<(int Row, int Col)> Frontier = frontier;
        public readonly int Rows = grid.Length;
        public readonly int Cols = grid[0].Length;
        public int FreshCount;
        public int MinutesElapsed;
    }
}
