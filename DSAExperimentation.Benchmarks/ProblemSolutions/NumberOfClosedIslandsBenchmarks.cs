using BenchmarkDotNet.Attributes;
using DSAExperimentation.Algorithms.Traversal.DepthFirst;

namespace DSAExperimentation.Benchmarks.ProblemSolutions;

// Number of Closed Islands (LC 1254): a hand-specialized recursive flood fill
// (the textbook approach, threading a "touched border" flag out of the
// recursion by ref) vs. this repo's own DepthFirstSearch.Traverse - the same
// border-flood-fill primitive NumberOfIslandsBenchmarks/MaxAreaOfIslandBenchmarks
// already use for LC 200/695, here checking whether any visited cell's own
// coordinates land on the grid's edge instead of counting components or area.
[MemoryDiagnoser]
public class NumberOfClosedIslandsBenchmarks
{
    private static readonly (int DRow, int DCol)[] Directions = [(1, 0), (-1, 0), (0, 1), (0, -1)];

    private const int RandomSeed = 7;

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

    [Benchmark(Baseline = true)]
    public int NaiveRecursiveFloodFill()
    {
        var cells = CloneGrid();
        var grid = new Grid(cells, cells.Length, cells[0].Length);
        var count = 0;

        for (var r = 0; r < grid.Rows; r++)
        {
            for (var c = 0; c < grid.Cols; c++)
            {
                if (TryCountClosedIsland(grid, r, c))
                {
                    count++;
                }
            }
        }

        return count;
    }

    private static bool TryCountClosedIsland(Grid grid, int row, int col)
    {
        if (grid.Cells[row][col] != 0)
        {
            return false;
        }

        var touchesBorder = false;
        Flood(grid, row, col, ref touchesBorder);

        return !touchesBorder;
    }

    private static void Flood(Grid grid, int row, int col, ref bool touchesBorder)
    {
        if (row < 0 || row >= grid.Rows || col < 0 || col >= grid.Cols || grid.Cells[row][col] != 0)
        {
            return;
        }

        grid.Cells[row][col] = 1;

        if (row == 0 || row == grid.Rows - 1 || col == 0 || col == grid.Cols - 1)
        {
            touchesBorder = true;
        }

        Flood(grid, row + 1, col, ref touchesBorder);
        Flood(grid, row - 1, col, ref touchesBorder);
        Flood(grid, row, col + 1, ref touchesBorder);
        Flood(grid, row, col - 1, ref touchesBorder);
    }

    [Benchmark]
    public int DepthFirstSearchTraversal()
    {
        var cells = CloneGrid();
        var grid = new Grid(cells, cells.Length, cells[0].Length);
        var count = 0;

        for (var r = 0; r < grid.Rows; r++)
        {
            for (var c = 0; c < grid.Cols; c++)
            {
                if (TryCountClosedIslandViaTraversal(grid, r, c))
                {
                    count++;
                }
            }
        }

        return count;
    }

    private static bool TryCountClosedIslandViaTraversal(Grid grid, int row, int col)
    {
        if (grid.Cells[row][col] != 0)
        {
            return false;
        }

        var island = DepthFirstSearch.Traverse((row, col), p => Neighbors(grid, p));
        var closed = true;

        foreach (var (r, c) in island)
        {
            if (r == 0 || r == grid.Rows - 1 || c == 0 || c == grid.Cols - 1)
            {
                closed = false;
            }

            grid.Cells[r][c] = 1;
        }

        return closed;
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

            if (grid.Cells[nextRow][nextCol] != 0)
            {
                continue;
            }

            yield return (nextRow, nextCol);
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
