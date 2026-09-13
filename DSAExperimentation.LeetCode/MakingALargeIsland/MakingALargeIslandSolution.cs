using DSAExperimentation.Algorithms.Traversal.DepthFirst;
using DSAExperimentation.DataStructures.HashMap;
using DSAExperimentation.DataStructures.Set;

namespace DSAExperimentation.LeetCode.MakingALargeIsland;

// LeetCode 827. Making A Large Island: change at most one 0 to a 1 and report the
// largest 4-directionally connected island the result can contain.
//
// Both strategies visit cells by writing into the grid, so each clones the grid it
// is handed first - the same clone-before-mutating shape MaxAreaOfIslandSolution
// uses for LC 695, so a caller's own grid (or a benchmark fixture reused across
// iterations) is never left labeled.
internal static class MakingALargeIslandSolution
{
    private const int FirstIslandId = 2;

    private static readonly (int DRow, int DCol)[] Directions = [(1, 0), (-1, 0), (0, 1), (0, -1)];

    // The textbook answer: flip each water cell in turn and run a fresh recursive
    // flood fill from scratch, BCL only. Deliberately written without this repo's
    // traversal primitive - it is the arm the composed strategy below has to
    // justify itself against.
    //
    // "At most one" flip means a grid that is already entirely land has no water
    // cell to try, and its answer is the whole grid; every other grid has some
    // water cell adjacent to its largest island, so the best flip dominates.
    public static int LargestIslandByNaiveFloodFill(int[][] grid)
    {
        var working = CloneGrid(grid);
        var rows = working.Length;
        var cols = working[0].Length;
        var best = 0;
        var sawWater = false;

        for (var r = 0; r < rows; r++)
        {
            for (var c = 0; c < cols; c++)
            {
                if (working[r][c] != 0)
                {
                    continue;
                }

                sawWater = true;
                best = Math.Max(best, FloodedAreaIfLand(working, r, c));
            }
        }

        return sawWater ? best : rows * cols;
    }

    private static int FloodedAreaIfLand(int[][] grid, int row, int col)
    {
        var rows = grid.Length;
        var cols = grid[0].Length;
        grid[row][col] = 1;
        var visited = new bool[rows, cols];
        var floodedArea = FloodCount(new FloodFillGrid(grid, visited, rows, cols), row, col);
        grid[row][col] = 0;
        return floodedArea;
    }

    private static int FloodCount(FloodFillGrid state, int row, int col)
    {
        if (row < 0 || row >= state.Rows || col < 0 || col >= state.Cols || state.Visited[row, col] || state.Grid[row][col] != 1)
        {
            return 0;
        }

        state.Visited[row, col] = true;

        return 1
            + FloodCount(state, row + 1, col)
            + FloodCount(state, row - 1, col)
            + FloodCount(state, row, col + 1)
            + FloodCount(state, row, col - 1);
    }

    // This repo's own DFS: DepthFirstSearch.Traverse labels each island exactly
    // once - writing a distinct id >= 2 back into the grid in place of the
    // traversed 1s - while a HashMap<int,int> records that island's area by id.
    // For every water cell, its up-to-4 neighboring ids are deduped through this
    // repo's own Set<int> and their areas summed with the flipped cell itself.
    // Scanning the recorded areas directly covers the "already all land" case with
    // no separate branch.
    public static int LargestIslandByLabeledFloodFill(int[][] grid)
    {
        var working = CloneGrid(grid);
        var context = new IslandGrid(working, new HashMap<int, int>(), working.Length, working[0].Length);

        LabelAllIslands(context);

        return MaxMergedWaterArea(context, MaxLabeledArea(context.AreaById));
    }

    private static void LabelAllIslands(IslandGrid context)
    {
        var nextId = FirstIslandId;

        for (var r = 0; r < context.Rows; r++)
        {
            for (var c = 0; c < context.Cols; c++)
            {
                nextId = LabelIslandIfLand(context, (r, c), nextId);
            }
        }
    }

    private static int LabelIslandIfLand(IslandGrid context, (int Row, int Col) cell, int nextId)
    {
        if (context.Grid[cell.Row][cell.Col] != 1)
        {
            return nextId;
        }

        var island = DepthFirstSearch.Traverse(cell, p => LandNeighbors(context, p));
        context.AreaById.Set(nextId, island.Count);

        foreach (var (row, col) in island)
        {
            context.Grid[row][col] = nextId;
        }

        return nextId + 1;
    }

    private static IEnumerable<(int Row, int Col)> LandNeighbors(IslandGrid context, (int Row, int Col) cell)
    {
        foreach (var (dRow, dCol) in Directions)
        {
            if (TryGetLandNeighbor(context, cell, (dRow, dCol), out var neighbor))
            {
                yield return neighbor;
            }
        }
    }

    private static bool TryGetLandNeighbor(IslandGrid context, (int Row, int Col) cell, (int DRow, int DCol) direction, out (int Row, int Col) neighbor)
    {
        var nextRow = cell.Row + direction.DRow;
        var nextCol = cell.Col + direction.DCol;

        if (nextRow < 0 || nextRow >= context.Rows || nextCol < 0 || nextCol >= context.Cols || context.Grid[nextRow][nextCol] != 1)
        {
            neighbor = default;
            return false;
        }

        neighbor = (nextRow, nextCol);
        return true;
    }

    private static int MaxLabeledArea(HashMap<int, int> areaById)
    {
        var best = 0;

        foreach (var area in areaById.Values)
        {
            best = Math.Max(best, area);
        }

        return best;
    }

    private static int MaxMergedWaterArea(IslandGrid context, int currentBest)
    {
        var best = currentBest;

        for (var r = 0; r < context.Rows; r++)
        {
            for (var c = 0; c < context.Cols; c++)
            {
                var merged = MergedAreaForWaterCell(context, (r, c));
                best = Math.Max(best, merged);
            }
        }

        return best;
    }

    private static int MergedAreaForWaterCell(IslandGrid context, (int Row, int Col) cell)
    {
        if (context.Grid[cell.Row][cell.Col] != 0)
        {
            return 0;
        }

        var seenIslandIds = new Set<int>();
        var merged = 1;

        foreach (var (dRow, dCol) in Directions)
        {
            merged += NeighborIslandArea(context, cell, (dRow, dCol), seenIslandIds);
        }

        return merged;
    }

    private static int NeighborIslandArea(IslandGrid context, (int Row, int Col) cell, (int DRow, int DCol) direction, Set<int> seenIslandIds)
    {
        var nextRow = cell.Row + direction.DRow;
        var nextCol = cell.Col + direction.DCol;

        if (nextRow < 0 || nextRow >= context.Rows || nextCol < 0 || nextCol >= context.Cols)
        {
            return 0;
        }

        var neighborId = context.Grid[nextRow][nextCol];

        if (neighborId < FirstIslandId || !seenIslandIds.TryAdd(neighborId))
        {
            return 0;
        }

        return context.AreaById.TryGetValue(neighborId, out var area) ? area : 0;
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

    private readonly record struct FloodFillGrid(int[][] Grid, bool[,] Visited, int Rows, int Cols);

    private readonly record struct IslandGrid(int[][] Grid, HashMap<int, int> AreaById, int Rows, int Cols);
}
