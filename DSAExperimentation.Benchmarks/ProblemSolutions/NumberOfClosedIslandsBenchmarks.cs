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

    [Params(30, 120)]
    public int Side;

    private int[][] _grid = null!;

    [GlobalSetup]
    public void Setup()
    {
        var random = new Random(7);
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
        var count = 0;

        for (var r = 0; r < rows; r++)
        {
            for (var c = 0; c < cols; c++)
            {
                if (grid[r][c] != 0)
                {
                    continue;
                }

                var touchesBorder = false;
                Flood(grid, r, c, rows, cols, ref touchesBorder);

                if (!touchesBorder)
                {
                    count++;
                }
            }
        }

        return count;
    }

    private static void Flood(int[][] grid, int row, int col, int rows, int cols, ref bool touchesBorder)
    {
        if (row < 0 || row >= rows || col < 0 || col >= cols || grid[row][col] != 0)
        {
            return;
        }

        grid[row][col] = 1;

        if (row == 0 || row == rows - 1 || col == 0 || col == cols - 1)
        {
            touchesBorder = true;
        }

        Flood(grid, row + 1, col, rows, cols, ref touchesBorder);
        Flood(grid, row - 1, col, rows, cols, ref touchesBorder);
        Flood(grid, row, col + 1, rows, cols, ref touchesBorder);
        Flood(grid, row, col - 1, rows, cols, ref touchesBorder);
    }

    [Benchmark]
    public int DepthFirstSearchTraversal()
    {
        var grid = CloneGrid();
        var rows = grid.Length;
        var cols = grid[0].Length;
        var count = 0;

        for (var r = 0; r < rows; r++)
        {
            for (var c = 0; c < cols; c++)
            {
                if (grid[r][c] != 0)
                {
                    continue;
                }

                var island = DepthFirstSearch.Traverse((r, c), Neighbors);
                var closed = true;

                foreach (var (row, col) in island)
                {
                    if (row == 0 || row == rows - 1 || col == 0 || col == cols - 1)
                    {
                        closed = false;
                    }

                    grid[row][col] = 1;
                }

                if (closed)
                {
                    count++;
                }
            }
        }

        return count;

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

                if (grid[nextRow][nextCol] != 0)
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
