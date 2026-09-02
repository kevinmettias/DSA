using BenchmarkDotNet.Attributes;
using DSAExperimentation.DataStructures.DisjointSet;

namespace DSAExperimentation.Benchmarks.ProblemSolutions;

// Bricks Falling When Hit (LC 803): the textbook "replay forward, recompute
// roof-connectivity by BFS after every single hit" approach - O(hits * rows * cols)
// - against this repo's own DisjointSet driving the reverse-time union trick: start
// from the fully-hit grid, then "un-hit" bricks back in from the last hit to the
// first, unioning each into any standing neighbor's component. Row 0 is kept fully
// bricked and every other row filled at a fixed density, with hits drawn only from
// real brick positions, so both strategies do real connectivity work on every hit
// instead of mostly no-op ones.
[MemoryDiagnoser]
public class BricksFallingWhenHitBenchmarks
{
    private const int RandomSeed = 803; // LC problem number
    private const double BrickDensity = 0.6;
    private const int HitFractionDivisor = 3;

    [Params(20, 60)]
    public int Size;

    private int[][] _grid = null!;
    private int[][] _hits = null!;

    [GlobalSetup]
    public void Setup()
    {
        var random = new Random(RandomSeed);
        _grid = BuildGrid(random, Size);
        var brickPositions = CollectBrickPositions(_grid, Size);
        _hits = SelectHits(random, brickPositions, Size);
    }

    private static int[][] BuildGrid(Random random, int size)
    {
        var grid = new int[size][];

        for (var r = 0; r < size; r++)
        {
            grid[r] = new int[size];

            for (var c = 0; c < size; c++)
            {
                grid[r][c] = r == 0 || random.NextDouble() < BrickDensity ? 1 : 0;
            }
        }

        return grid;
    }

    private static List<(int Row, int Col)> CollectBrickPositions(int[][] grid, int size)
    {
        var brickPositions = new List<(int Row, int Col)>();

        for (var r = 0; r < size; r++)
        {
            for (var c = 0; c < size; c++)
            {
                if (grid[r][c] == 1)
                {
                    brickPositions.Add((r, c));
                }
            }
        }

        return brickPositions;
    }

    private static int[][] SelectHits(Random random, List<(int Row, int Col)> brickPositions, int size)
    {
        var hitCount = Math.Min(brickPositions.Count, (size * size) / HitFractionDivisor);
        return brickPositions
            .OrderBy(_ => random.Next())
            .Take(hitCount)
            .Select(p => new[] { p.Row, p.Col })
            .ToArray();
    }

    [Benchmark(Baseline = true)]
    public int ReplayForwardWithBfs()
    {
        var rows = _grid.Length;
        var cols = _grid[0].Length;
        var standing = new bool[rows, cols];

        for (var r = 0; r < rows; r++)
        {
            for (var c = 0; c < cols; c++)
            {
                standing[r, c] = _grid[r][c] == 1;
            }
        }

        var state = new ReplayState(0, CountConnectedToRoof(standing, rows, cols));

        foreach (var hit in _hits)
        {
            state = ApplyHit(standing, hit, state);
        }

        return state.TotalFallen;
    }

    private static ReplayState ApplyHit(bool[,] standing, int[] hit, ReplayState state)
    {
        var rows = standing.GetLength(0);
        var cols = standing.GetLength(1);
        standing[hit[0], hit[1]] = false;
        var connectedAfter = CountConnectedToRoof(standing, rows, cols);
        var fallen = state.ConnectedBefore - connectedAfter - 1;
        var totalFallen = state.TotalFallen + (fallen > 0 ? fallen : 0);
        return new ReplayState(totalFallen, connectedAfter);
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

        var grid = new RoofGrid(standing, visited, rows, cols);
        var count = 0;

        while (frontier.Count > 0)
        {
            count += ProcessFrontierCell(grid, frontier);
        }

        return count;
    }

    private static int ProcessFrontierCell(RoofGrid grid, Queue<(int Row, int Col)> frontier)
    {
        var (row, col) = frontier.Dequeue();

        (int Row, int Col)[] neighbors = [(row - 1, col), (row + 1, col), (row, col - 1), (row, col + 1)];

        foreach (var (neighborRow, neighborCol) in neighbors)
        {
            if (neighborRow >= 0 && neighborRow < grid.Rows && neighborCol >= 0 && neighborCol < grid.Cols
                && grid.Standing[neighborRow, neighborCol] && !grid.Visited[neighborRow, neighborCol])
            {
                grid.Visited[neighborRow, neighborCol] = true;
                frontier.Enqueue((neighborRow, neighborCol));
            }
        }

        return 1;
    }

    [Benchmark]
    public int ReverseTimeDisjointSet()
    {
        var rows = _grid.Length;
        var cols = _grid[0].Length;
        var roof = rows * cols;
        var present = BuildPresenceGrid(_grid, rows, cols, _hits);
        var components = new DisjointSet(roof + 1);
        var size = InitializeSizes(present, rows, cols, roof);
        var grid = new BrickGrid(components, size, present, rows, cols, roof);

        ConnectAllStandingCells(grid);

        return ReplayHitsBackward(grid, _hits);
    }

    private static bool[,] BuildPresenceGrid(int[][] sourceGrid, int rows, int cols, int[][] hits)
    {
        var present = new bool[rows, cols];

        for (var r = 0; r < rows; r++)
        {
            for (var c = 0; c < cols; c++)
            {
                present[r, c] = sourceGrid[r][c] == 1;
            }
        }

        foreach (var hit in hits)
        {
            present[hit[0], hit[1]] = false;
        }

        return present;
    }

    private static int[] InitializeSizes(bool[,] present, int rows, int cols, int roof)
    {
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

        return size;
    }

    private static void ConnectAllStandingCells(BrickGrid grid)
    {
        for (var r = 0; r < grid.Rows; r++)
        {
            for (var c = 0; c < grid.Cols; c++)
            {
                if (grid.Present[r, c])
                {
                    ConnectToStandingNeighbors(grid, r, c);
                }
            }
        }
    }

    private static int ReplayHitsBackward(BrickGrid grid, int[][] hits)
    {
        var totalFallen = 0;

        for (var i = hits.Length - 1; i >= 0; i--)
        {
            totalFallen += UndoHit(grid, hits[i]);
        }

        return totalFallen;
    }

    private static int UndoHit(BrickGrid grid, int[] hit)
    {
        var row = hit[0];
        var col = hit[1];

        var beforeSize = grid.Size[grid.Components.Find(grid.Roof)];
        grid.Present[row, col] = true;
        grid.Size[(row * grid.Cols) + col] = 1;
        ConnectToStandingNeighbors(grid, row, col);
        var afterSize = grid.Size[grid.Components.Find(grid.Roof)];

        return afterSize > beforeSize ? afterSize - beforeSize - 1 : 0;
    }

    private static void ConnectToStandingNeighbors(BrickGrid grid, int row, int col)
    {
        var cellId = (row * grid.Cols) + col;

        if (row == 0)
        {
            Union(grid.Components, grid.Size, cellId, grid.Roof);
        }

        (int Row, int Col)[] neighbors = [(row - 1, col), (row + 1, col), (row, col - 1), (row, col + 1)];

        foreach (var (neighborRow, neighborCol) in neighbors)
        {
            if (neighborRow >= 0 && neighborRow < grid.Rows && neighborCol >= 0 && neighborCol < grid.Cols
                && grid.Present[neighborRow, neighborCol])
            {
                Union(grid.Components, grid.Size, cellId, (neighborRow * grid.Cols) + neighborCol);
            }
        }
    }

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

    private readonly record struct ReplayState(int TotalFallen, int ConnectedBefore);

    private readonly record struct RoofGrid(bool[,] Standing, bool[,] Visited, int Rows, int Cols);

    private readonly record struct BrickGrid(DisjointSet Components, int[] Size, bool[,] Present, int Rows, int Cols, int Roof);
}
