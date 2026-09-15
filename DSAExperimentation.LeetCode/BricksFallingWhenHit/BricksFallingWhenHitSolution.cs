using DSAExperimentation.DataStructures.DisjointSet;

namespace DSAExperimentation.LeetCode.BricksFallingWhenHit;

// LeetCode 803. Bricks Falling When Hit: erase one brick per hit, in order, and
// report how many OTHER bricks lose their connection to the top row each time.
//
// The two strategies answer the same question from opposite ends of time. The
// baseline replays the hits forward and recounts roof-connected bricks by BFS
// after every one - O(hits * rows * cols) and deliberately BCL-only. The composed
// strategy runs time backwards: start from the fully-hit grid, then "un-hit"
// bricks back in from the last hit to the first, unioning each restored brick into
// any standing neighbour (and into a virtual roof node, for row 0) via this repo's
// own DisjointSet, which only ever merges components and so never has to undo one.
//
// A component's size is caller-side bookkeeping threaded beside Find/Union -
// DisjointSet itself has no size query, the same "own scratch state next to a
// composed primitive" shape TopologicalSort.cs's in-degree dictionary and
// ShortestPath.cs's Distances dictionary already establish - so no new production
// primitive is needed, only a size[] array local to this solution.
internal static class BricksFallingWhenHitSolution
{
    // The textbook answer: keep a presence grid, drop one brick at a time and
    // re-flood the roof from scratch after each drop. BCL Queue and arrays only -
    // it is the arm the reverse-time union strategy has to justify itself against.
    public static int[] HitBricksByForwardReplayBfs(int[][] grid, int[][] hits)
    {
        var rows = grid.Length;
        var cols = grid[0].Length;
        var standing = CopyBricks(grid, rows, cols);
        var fallen = new int[hits.Length];
        var connectedBefore = CountConnectedToRoof(standing, rows, cols);

        for (var i = 0; i < hits.Length; i++)
        {
            standing[hits[i][0], hits[i][1]] = false;
            var connectedAfter = CountConnectedToRoof(standing, rows, cols);
            var dropped = connectedBefore - connectedAfter - 1;

            fallen[i] = dropped > 0 ? dropped : 0;
            connectedBefore = connectedAfter;
        }

        return fallen;
    }

    private static bool[,] CopyBricks(int[][] grid, int rows, int cols)
    {
        var standing = new bool[rows, cols];

        for (var r = 0; r < rows; r++)
        {
            for (var c = 0; c < cols; c++)
            {
                standing[r, c] = grid[r][c] == 1;
            }
        }

        return standing;
    }

    private static int CountConnectedToRoof(bool[,] standing, int rows, int cols)
    {
        var visited = new bool[rows, cols];
        var frontier = new Queue<(int Row, int Col)>();

        for (var c = 0; c < cols; c++)
        {
            if (standing[0, c])
            {
                visited[0, c] = true;
                frontier.Enqueue((0, c));
            }
        }

        var roofGrid = new RoofGrid(standing, visited, rows, cols);
        var count = 0;

        while (frontier.Count > 0)
        {
            count += ProcessFrontierCell(roofGrid, frontier);
        }

        return count;
    }

    private static int ProcessFrontierCell(RoofGrid grid, Queue<(int Row, int Col)> frontier)
    {
        var (row, col) = frontier.Dequeue();

        (int Row, int Col)[] neighbors = [(row - 1, col), (row + 1, col), (row, col - 1), (row, col + 1)];

        foreach (var (neighborRow, neighborCol) in neighbors)
        {
            if (IsUnvisitedStandingBrick(grid, neighborRow, neighborCol))
            {
                grid.Visited[neighborRow, neighborCol] = true;
                frontier.Enqueue((neighborRow, neighborCol));
            }
        }

        return 1;
    }

    // A neighbour worth flooding into: on the board, still standing, and not yet
    // reached by the roof flood.
    private static bool IsUnvisitedStandingBrick(RoofGrid grid, int row, int col)
        => row >= 0 && row < grid.Rows && col >= 0 && col < grid.Cols
            && grid.Standing[row, col] && !grid.Visited[row, col];

    // This repo's own DisjointSet, driven backwards through the hit list: the roof
    // component's size before and after restoring a brick differs by exactly that
    // brick plus everything it re-attached, so the answer for that hit is the
    // growth minus the restored brick itself.
    public static int[] HitBricksByReverseTimeDisjointSet(int[][] grid, int[][] hits)
    {
        var rows = grid.Length;
        var cols = grid[0].Length;
        var roof = rows * cols;

        var present = BuildPresentGridAfterHits(grid, hits);
        var (components, size) = InitializeComponents(present, rows, cols, roof);
        var gridState = new GridState(components, size, present, rows, cols, roof);

        ConnectAllStandingBricks(gridState);

        var fallenReversed = new int[hits.Length];
        var hitContext = new HitProcessingContext(hits, grid, fallenReversed);
        ProcessHitsInReverse(gridState, hitContext);

        return fallenReversed;
    }

    private static bool[,] BuildPresentGridAfterHits(int[][] grid, int[][] hits)
    {
        var rows = grid.Length;
        var cols = grid[0].Length;
        var present = new bool[rows, cols];

        for (var r = 0; r < rows; r++)
        {
            for (var c = 0; c < cols; c++)
            {
                present[r, c] = grid[r][c] == 1;
            }
        }

        foreach (var hit in hits)
        {
            present[hit[0], hit[1]] = false;
        }

        return present;
    }

    private static (DisjointSet Components, int[] Size) InitializeComponents(bool[,] present, int rows, int cols, int roof)
    {
        var components = new DisjointSet(roof + 1);
        var size = new int[roof + 1];

        for (var r = 0; r < rows; r++)
        {
            for (var c = 0; c < cols; c++)
            {
                if (present[r, c])
                {
                    size[(r * cols) + c] = 1;
                }
            }
        }

        return (components, size);
    }

    private static void ConnectAllStandingBricks(GridState state)
    {
        for (var r = 0; r < state.Rows; r++)
        {
            for (var c = 0; c < state.Cols; c++)
            {
                if (state.Present[r, c])
                {
                    ConnectToStandingNeighbors(state, r, c);
                }
            }
        }
    }

    private static void ProcessHitsInReverse(GridState state, HitProcessingContext context)
    {
        for (var i = context.Hits.Length - 1; i >= 0; i--)
        {
            ProcessReverseHit(state, context, i);
        }
    }

    private static void ProcessReverseHit(GridState state, HitProcessingContext context, int i)
    {
        var row = context.Hits[i][0];
        var col = context.Hits[i][1];

        if (context.Grid[row][col] == 0)
        {
            return;
        }

        var beforeSize = state.Size[state.Components.Find(state.Roof)];
        state.Present[row, col] = true;
        state.Size[(row * state.Cols) + col] = 1;
        ConnectToStandingNeighbors(state, row, col);
        var afterSize = state.Size[state.Components.Find(state.Roof)];

        context.FallenReversed[i] = FallenBrickCount(afterSize, beforeSize);
    }

    // How many bricks this hit knocked down, read off the roof component's size before
    // and after the hit was undone: the growth less the restored brick itself, and none
    // at all when the roof ended no larger than it started.
    private static int FallenBrickCount(int afterSize, int beforeSize)
    {
        if (afterSize > beforeSize)
        {
            return afterSize - beforeSize - 1;
        }

        return 0;
    }

    private static void ConnectToStandingNeighbors(GridState state, int row, int col)
    {
        var cellId = (row * state.Cols) + col;

        if (row == 0)
        {
            Union(state.Components, state.Size, cellId, state.Roof);
        }

        (int Row, int Col)[] neighbors = [(row - 1, col), (row + 1, col), (row, col - 1), (row, col + 1)];

        foreach (var (neighborRow, neighborCol) in neighbors)
        {
            if (IsStandingBrick(state, neighborRow, neighborCol))
            {
                Union(state.Components, state.Size, cellId, (neighborRow * state.Cols) + neighborCol);
            }
        }
    }

    // A neighbour on the board that still has its brick.
    private static bool IsStandingBrick(GridState state, int row, int col)
        => row >= 0 && row < state.Rows && col >= 0 && col < state.Cols
            && state.Present[row, col];

    private static void Union(DisjointSet components, int[] size, int first, int second)
    {
        var firstRoot = components.Find(first);
        var secondRoot = components.Find(second);

        if (firstRoot == secondRoot)
        {
            return;
        }

        components.Union(first, second);
        var mergedRoot = components.Find(first);
        size[mergedRoot] = size[firstRoot] + size[secondRoot];
    }

    private readonly record struct RoofGrid(bool[,] Standing, bool[,] Visited, int Rows, int Cols);

    private readonly record struct GridState(
        DisjointSet Components, int[] Size, bool[,] Present, int Rows, int Cols, int Roof);

    private readonly record struct HitProcessingContext(int[][] Hits, int[][] Grid, int[] FallenReversed);
}
