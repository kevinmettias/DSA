using BenchmarkDotNet.Attributes;
using DSAExperimentation.Algorithms.Traversal.DepthFirst;

namespace DSAExperimentation.Benchmarks.ProblemSolutions;

// Maximum Number of Fish in a Grid (LC 2658): a hand-specialized recursive flood
// fill (the textbook approach) vs. this repo's own DepthFirstSearch.Traverse
// walking each water component's reachable cells - the same border-flood-fill
// primitive MaxAreaOfIslandBenchmarks already uses for LC 695 - here summing each
// visited cell's own fish count instead of counting cells for area.
[MemoryDiagnoser]
public class MaximumNumberOfFishInAGridBenchmarks
{
    private static readonly (int DRow, int DCol)[] Directions = [(1, 0), (-1, 0), (0, 1), (0, -1)];

    private const int RandomSeed = 2658; // LC problem number
    private const double WaterProbability = 0.55;
    private const int MaxFishExclusive = 11;

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
                _grid[r][c] = random.NextDouble() < WaterProbability ? random.Next(1, MaxFishExclusive) : 0;
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
                if (grid[r][c] > 0)
                {
                    var floodedTotal = Flood(grid, r, c, bounds);
                    best = Math.Max(best, floodedTotal);
                }
            }
        }

        return best;
    }

    private static int Flood(int[][] grid, int row, int col, GridBounds bounds)
    {
        if (row < 0 || row >= bounds.Rows || col < 0 || col >= bounds.Cols || grid[row][col] == 0)
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
                if (grid[r][c] == 0)
                {
                    continue;
                }

                var floodedTotal = FloodFishTotal(grid, r, c);
                best = Math.Max(best, floodedTotal);
            }
        }

        return best;
    }

    private static int FloodFishTotal(int[][] grid, int r, int c)
    {
        var component = DepthFirstSearch.Traverse((Row: r, Col: c), p => WaterNeighbors(grid, p));
        var total = component.Sum(p => grid[p.Row][p.Col]);

        foreach (var (row, col) in component)
        {
            grid[row][col] = 0;
        }

        return total;
    }

    private static IEnumerable<(int Row, int Col)> WaterNeighbors(int[][] grid, (int Row, int Col) p)
    {
        foreach (var direction in Directions)
        {
            if (TryGetWaterNeighbor(grid, p, direction, out var neighbor))
            {
                yield return neighbor;
            }
        }
    }

    private static bool TryGetWaterNeighbor(
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

        return grid[nextRow][nextCol] > 0;
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
