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

    private const int RandomSeed = 1020; // LeetCode problem number

    private const double LandDensity = 0.55;

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
                _grid[r][c] = random.NextDouble() < LandDensity ? 1 : 0;
            }
        }
    }

    private readonly record struct Grid(int[][] Cells, int Rows, int Cols);

    private delegate void SinkAt(Grid grid, int row, int col);

    [Benchmark(Baseline = true)]
    public int NaiveRecursiveFloodFill()
    {
        var cells = CloneGrid();
        var grid = new Grid(cells, cells.Length, cells[0].Length);

        SinkBorderIslands(grid, Flood);

        return CountLand(cells);
    }

    private static void Flood(Grid grid, int row, int col)
    {
        if (row < 0 || row >= grid.Rows || col < 0 || col >= grid.Cols || grid.Cells[row][col] != 1)
        {
            return;
        }

        grid.Cells[row][col] = 0;

        Flood(grid, row + 1, col);
        Flood(grid, row - 1, col);
        Flood(grid, row, col + 1);
        Flood(grid, row, col - 1);
    }

    [Benchmark]
    public int DepthFirstSearchTraversal()
    {
        var cells = CloneGrid();
        var grid = new Grid(cells, cells.Length, cells[0].Length);

        SinkBorderIslands(grid, SinkComponent);

        return CountLand(cells);
    }

    private static void SinkComponent(Grid grid, int startRow, int startCol)
    {
        if (grid.Cells[startRow][startCol] != 1)
        {
            return;
        }

        var component = DepthFirstSearch.Traverse((startRow, startCol), p => Neighbors(grid, p));

        foreach (var (row, col) in component)
        {
            grid.Cells[row][col] = 0;
        }
    }

    private static IEnumerable<(int Row, int Col)> Neighbors(Grid grid, (int Row, int Col) p)
    {
        foreach (var (dRow, dCol) in Directions)
        {
            var nextRow = p.Row + dRow;
            var nextCol = p.Col + dCol;

            if (nextRow < 0 || nextRow >= grid.Rows || nextCol < 0 || nextCol >= grid.Cols)
            {
                continue;
            }

            if (grid.Cells[nextRow][nextCol] != 1)
            {
                continue;
            }

            yield return (nextRow, nextCol);
        }
    }

    private static void SinkBorderIslands(Grid grid, SinkAt sink)
    {
        for (var r = 0; r < grid.Rows; r++)
        {
            for (var c = 0; c < grid.Cols; c++)
            {
                var onBorder = r == 0 || r == grid.Rows - 1 || c == 0 || c == grid.Cols - 1;

                if (onBorder)
                {
                    sink(grid, r, c);
                }
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
