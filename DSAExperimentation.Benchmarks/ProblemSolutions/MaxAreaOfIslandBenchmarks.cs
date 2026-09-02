using BenchmarkDotNet.Attributes;
using DSAExperimentation.Algorithms.Traversal.DepthFirst;

namespace DSAExperimentation.Benchmarks.ProblemSolutions;

// Max Area of Island (LC 695): a hand-specialized recursive flood fill (the
// textbook approach) vs. this repo's own DepthFirstSearch.Traverse walking each
// island's reachable land cells - the same border-flood-fill primitive
// PacificAtlanticWaterFlowTests/SurroundedRegionsTests already use for LC 417/130 -
// here the traversal's own List<TNode>.Count doubles as the island's area instead
// of a separate reachability set.
[MemoryDiagnoser]
public class MaxAreaOfIslandBenchmarks
{
    private static readonly (int DRow, int DCol)[] Directions = [(1, 0), (-1, 0), (0, 1), (0, -1)];

    private const int RandomSeed = 3;
    private const double LandProbability = 0.55;

    [Params(30, 120)]
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

    private readonly record struct GridBounds(int Rows, int Cols);

    [Benchmark(Baseline = true)]
    public int NaiveRecursiveFloodFill()
    {
        var grid = CloneGrid();
        var bounds = new GridBounds(grid.Length, grid[0].Length);
        var best = 0;

        for (var r = 0; r < bounds.Rows; r++)
        {
            for (var c = 0; c < bounds.Cols; c++)
            {
                if (grid[r][c] == 1)
                {
                    var floodedArea = Flood(grid, r, c, bounds);
                    best = Math.Max(best, floodedArea);
                }
            }
        }

        return best;
    }

    private static int Flood(int[][] grid, int row, int col, GridBounds bounds)
    {
        if (row < 0 || row >= bounds.Rows || col < 0 || col >= bounds.Cols || grid[row][col] != 1)
        {
            return 0;
        }

        grid[row][col] = 0;

        return 1
            + Flood(grid, row + 1, col, bounds)
            + Flood(grid, row - 1, col, bounds)
            + Flood(grid, row, col + 1, bounds)
            + Flood(grid, row, col - 1, bounds);
    }

    [Benchmark]
    public int DepthFirstSearchTraversal()
    {
        var grid = CloneGrid();
        var bounds = new GridBounds(grid.Length, grid[0].Length);
        var best = 0;

        for (var r = 0; r < bounds.Rows; r++)
        {
            for (var c = 0; c < bounds.Cols; c++)
            {
                if (grid[r][c] != 1)
                {
                    continue;
                }

                var floodedArea = FloodIslandArea(grid, r, c);
                best = Math.Max(best, floodedArea);
            }
        }

        return best;
    }

    private static int FloodIslandArea(int[][] grid, int r, int c)
    {
        var island = DepthFirstSearch.Traverse((r, c), p => LandNeighbors(grid, p));

        foreach (var (row, col) in island)
        {
            grid[row][col] = 0;
        }

        return island.Count;
    }

    private static IEnumerable<(int Row, int Col)> LandNeighbors(int[][] grid, (int Row, int Col) p)
    {
        foreach (var direction in Directions)
        {
            if (TryGetLandNeighbor(grid, p, direction, out var neighbor))
            {
                yield return neighbor;
            }
        }
    }

    private static bool TryGetLandNeighbor(
        int[][] grid,
        (int Row, int Col) origin,
        (int DRow, int DCol) delta,
        out (int Row, int Col) neighbor)
    {
        var nextRow = origin.Row + delta.DRow;
        var nextCol = origin.Col + delta.DCol;
        neighbor = (nextRow, nextCol);

        if (nextRow < 0 || nextRow >= grid.Length || nextCol < 0 || nextCol >= grid[0].Length)
        {
            return false;
        }

        return grid[nextRow][nextCol] == 1;
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
}
