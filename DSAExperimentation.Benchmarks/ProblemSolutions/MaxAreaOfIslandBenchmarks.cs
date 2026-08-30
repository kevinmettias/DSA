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

    [Params(30, 120)]
    public int Side;

    private int[][] _grid = null!;

    [GlobalSetup]
    public void Setup()
    {
        var random = new Random(3);
        _grid = new int[Side][];

        for (var r = 0; r < Side; r++)
        {
            _grid[r] = new int[Side];

            for (var c = 0; c < Side; c++)
            {
                _grid[r][c] = random.NextDouble() < 0.55 ? 1 : 0;
            }
        }
    }

    [Benchmark(Baseline = true)]
    public int NaiveRecursiveFloodFill()
    {
        var grid = CloneGrid();
        var rows = grid.Length;
        var cols = grid[0].Length;
        var best = 0;

        for (var r = 0; r < rows; r++)
        {
            for (var c = 0; c < cols; c++)
            {
                if (grid[r][c] == 1)
                {
                    best = Math.Max(best, Flood(grid, r, c, rows, cols));
                }
            }
        }

        return best;
    }

    private static int Flood(int[][] grid, int row, int col, int rows, int cols)
    {
        if (row < 0 || row >= rows || col < 0 || col >= cols || grid[row][col] != 1)
        {
            return 0;
        }

        grid[row][col] = 0;

        return 1
            + Flood(grid, row + 1, col, rows, cols)
            + Flood(grid, row - 1, col, rows, cols)
            + Flood(grid, row, col + 1, rows, cols)
            + Flood(grid, row, col - 1, rows, cols);
    }

    [Benchmark]
    public int DepthFirstSearchTraversal()
    {
        var grid = CloneGrid();
        var rows = grid.Length;
        var cols = grid[0].Length;
        var best = 0;

        for (var r = 0; r < rows; r++)
        {
            for (var c = 0; c < cols; c++)
            {
                if (grid[r][c] != 1)
                {
                    continue;
                }

                var island = DepthFirstSearch.Traverse((r, c), Neighbors);
                best = Math.Max(best, island.Count);

                foreach (var (row, col) in island)
                {
                    grid[row][col] = 0;
                }
            }
        }

        return best;

        IEnumerable<(int Row, int Col)> Neighbors((int Row, int Col) p)
        {
            foreach (var (dRow, dCol) in Directions)
            {
                var nextRow = p.Row + dRow;
                var nextCol = p.Col + dCol;

                if (nextRow < 0 || nextRow >= rows || nextCol < 0 || nextCol >= cols)
                {
                    continue;
                }

                if (grid[nextRow][nextCol] != 1)
                {
                    continue;
                }

                yield return (nextRow, nextCol);
            }
        }
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
