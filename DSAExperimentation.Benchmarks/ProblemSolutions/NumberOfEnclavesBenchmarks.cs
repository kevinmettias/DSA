using BenchmarkDotNet.Attributes;
using DSAExperimentation.Algorithms.Traversal.DepthFirst;

namespace DSAExperimentation.Benchmarks.ProblemSolutions;

// Number of Enclaves (LC 1020): a hand-specialized recursive border flood fill
// (the textbook approach) vs. this repo's own DepthFirstSearch.Traverse walking
// each border-connected land component - the same flood-fill primitive
// MaxAreaOfIslandBenchmarks/NumberOfIslandsBenchmarks already use, applied here
// to sink every border-touching island instead of measuring one island's area.
[MemoryDiagnoser]
public class NumberOfEnclavesBenchmarks
{
    private static readonly (int DRow, int DCol)[] Directions = [(1, 0), (-1, 0), (0, 1), (0, -1)];

    [Params(30, 120)]
    public int Side;

    private int[][] _grid = null!;

    [GlobalSetup]
    public void Setup()
    {
        var random = new Random(1020);
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

        for (var r = 0; r < rows; r++)
        {
            for (var c = 0; c < cols; c++)
            {
                var onBorder = r == 0 || r == rows - 1 || c == 0 || c == cols - 1;

                if (onBorder)
                {
                    Flood(grid, r, c, rows, cols);
                }
            }
        }

        return CountLand(grid);
    }

    private static void Flood(int[][] grid, int row, int col, int rows, int cols)
    {
        if (row < 0 || row >= rows || col < 0 || col >= cols || grid[row][col] != 1)
        {
            return;
        }

        grid[row][col] = 0;

        Flood(grid, row + 1, col, rows, cols);
        Flood(grid, row - 1, col, rows, cols);
        Flood(grid, row, col + 1, rows, cols);
        Flood(grid, row, col - 1, rows, cols);
    }

    [Benchmark]
    public int DepthFirstSearchTraversal()
    {
        var grid = CloneGrid();
        var rows = grid.Length;
        var cols = grid[0].Length;

        for (var r = 0; r < rows; r++)
        {
            for (var c = 0; c < cols; c++)
            {
                var onBorder = r == 0 || r == rows - 1 || c == 0 || c == cols - 1;

                if (onBorder)
                {
                    SinkComponent(grid, r, c, rows, cols);
                }
            }
        }

        return CountLand(grid);
    }

    private static void SinkComponent(int[][] grid, int startRow, int startCol, int rows, int cols)
    {
        if (grid[startRow][startCol] != 1)
        {
            return;
        }

        var component = DepthFirstSearch.Traverse((startRow, startCol), Neighbors);

        foreach (var (row, col) in component)
        {
            grid[row][col] = 0;
        }

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

    private static int CountLand(int[][] grid)
    {
        var count = 0;

        foreach (var row in grid)
        {
            foreach (var cell in row)
            {
                if (cell == 1)
                {
                    count++;
                }
            }
        }

        return count;
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
