using BenchmarkDotNet.Attributes;
using DSAExperimentation.Algorithms.Traversal.DepthFirst;
using DSAExperimentation.DataStructures.HashMap;
using DSAExperimentation.DataStructures.Set;

namespace DSAExperimentation.Benchmarks.ProblemSolutions;

// Making A Large Island (LC 827): repeatedly flipping each water cell and running
// a fresh recursive flood fill from scratch (the textbook brute force) vs. this
// repo's own DepthFirstSearch.Traverse labeling every island exactly once - the
// same primitive MaxAreaOfIslandBenchmarks uses for LC 695 - into a
// HashMap<int,int> of id -> area, then summing each water cell's already-known
// neighboring areas (deduped through this repo's own Set<int>) in a single pass.
[MemoryDiagnoser]
public class MakingALargeIslandBenchmarks
{
    private const int RandomSeed = 7; // LC problem number
    private const double LandProbability = 0.6;
    private const int FirstIslandId = 2;

    private static readonly (int DRow, int DCol)[] Directions = [(1, 0), (-1, 0), (0, 1), (0, -1)];

    [Params(20, 60)]
    public int Side;

    private int[][] _grid = null!;

    [GlobalSetup]
    public void Setup()
    {
        var random = new Random(RandomSeed);
        _grid = new int[Side][];

        for (var r = 0; r < Side; r++)
        {
            _grid[r] = new int[Side];

            for (var c = 0; c < Side; c++)
            {
                _grid[r][c] = random.NextDouble() < LandProbability ? 1 : 0;
            }
        }
    }

    [Benchmark(Baseline = true)]
    public int NaiveRepeatedFloodFill()
    {
        var grid = CloneGrid();
        var rows = grid.Length;
        var cols = grid[0].Length;
        var best = 0;

        for (var r = 0; r < rows; r++)
        {
            for (var c = 0; c < cols; c++)
            {
                if (grid[r][c] != 0)
                {
                    continue;
                }

                var floodedArea = FloodedAreaIfLand(grid, r, c);
                best = Math.Max(best, floodedArea);
            }
        }

        return best;
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

    [Benchmark]
    public int LabeledFloodFillWithAreaLookup()
    {
        var grid = CloneGrid();
        var rows = grid.Length;
        var cols = grid[0].Length;
        var areaById = new HashMap<int, int>();
        var context = new IslandGridContext(grid, areaById, rows, cols);

        LabelAllIslands(context);
        var best = MaxLabeledArea(areaById);

        return MaxMergedWaterArea(context, best);
    }

    private static void LabelAllIslands(IslandGridContext context)
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

    private static int MaxLabeledArea(HashMap<int, int> areaById)
    {
        var best = 0;

        foreach (var area in areaById.Values)
        {
            best = Math.Max(best, area);
        }

        return best;
    }

    private static int MaxMergedWaterArea(IslandGridContext context, int currentBest)
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

    private static IEnumerable<(int Row, int Col)> Neighbors(IslandGridContext context, (int Row, int Col) cell)
    {
        foreach (var (dRow, dCol) in Directions)
        {
            if (TryGetLandNeighbor(context, cell, (dRow, dCol), out var neighbor))
            {
                yield return neighbor;
            }
        }
    }

    private static bool TryGetLandNeighbor(IslandGridContext context, (int Row, int Col) cell, (int DRow, int DCol) direction, out (int Row, int Col) neighbor)
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

    private static int LabelIslandIfLand(IslandGridContext context, (int Row, int Col) cell, int nextId)
    {
        if (context.Grid[cell.Row][cell.Col] != 1)
        {
            return nextId;
        }

        var island = DepthFirstSearch.Traverse(cell, p => Neighbors(context, p));
        context.AreaById.Set(nextId, island.Count);

        foreach (var (row, col) in island)
        {
            context.Grid[row][col] = nextId;
        }

        return nextId + 1;
    }

    private static int MergedAreaForWaterCell(IslandGridContext context, (int Row, int Col) cell)
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

    private static int NeighborIslandArea(IslandGridContext context, (int Row, int Col) cell, (int DRow, int DCol) direction, Set<int> seenIslandIds)
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

    private int[][] CloneGrid()
    {
        var clone = new int[_grid.Length][];

        for (var r = 0; r < _grid.Length; r++)
        {
            clone[r] = (int[])_grid[r].Clone();
        }

        return clone;
    }

    private readonly record struct FloodFillGrid(int[][] Grid, bool[,] Visited, int Rows, int Cols);

    private readonly record struct IslandGridContext(int[][] Grid, HashMap<int, int> AreaById, int Rows, int Cols);
}
