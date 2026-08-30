using BenchmarkDotNet.Attributes;
using DSAExperimentation.Algorithms.Traversal.DepthFirst;

namespace DSAExperimentation.Benchmarks.ProblemSolutions;

// Minimum Number of Days to Disconnect Island (LC 1568): a hand-rolled recursive
// flood fill counting connected land components (the textbook approach, no repo
// primitive) vs. this repo's own DepthFirstSearch.Traverse doing the same count -
// the same primitive MakingALargeIslandBenchmarks/NumberOfIslandsTests already use
// for grid connectivity. Setup is a solid all-land square: a full rectangular grid
// graph has no articulation cell (removing any single land cell never disconnects
// it), so MinDays always returns 2 - forcing both approaches through their full
// O((Rows*Cols)^2) worst case (one connectivity check per candidate removal) rather
// than an early exit after the first cell tried.
[MemoryDiagnoser]
public class MinimumNumberOfDaysToDisconnectIslandBenchmarks
{
    private static readonly (int DRow, int DCol)[] Directions = [(1, 0), (-1, 0), (0, 1), (0, -1)];

    [Params(10, 20)]
    public int Side;

    private int[][] _grid = null!;

    [GlobalSetup]
    public void Setup()
    {
        _grid = new int[Side][];

        for (var r = 0; r < Side; r++)
        {
            _grid[r] = Enumerable.Repeat(1, Side).ToArray();
        }
    }

    [Benchmark(Baseline = true)]
    public int NaiveRecursiveFloodFill()
    {
        var rows = _grid.Length;
        var cols = _grid[0].Length;

        if (CountIslandsNaive(-1, -1, rows, cols) != 1)
        {
            return 0;
        }

        for (var r = 0; r < rows; r++)
        {
            for (var c = 0; c < cols; c++)
            {
                if (_grid[r][c] == 1 && CountIslandsNaive(r, c, rows, cols) != 1)
                {
                    return 1;
                }
            }
        }

        return 2;
    }

    private int CountIslandsNaive(int skipRow, int skipCol, int rows, int cols)
    {
        var visited = new bool[rows, cols];
        var count = 0;

        for (var r = 0; r < rows; r++)
        {
            for (var c = 0; c < cols; c++)
            {
                if (_grid[r][c] != 1 || visited[r, c] || (r == skipRow && c == skipCol))
                {
                    continue;
                }

                count++;
                FloodFill(r, c, visited, skipRow, skipCol, rows, cols);
            }
        }

        return count;
    }

    private void FloodFill(int row, int col, bool[,] visited, int skipRow, int skipCol, int rows, int cols)
    {
        if (row < 0 || row >= rows || col < 0 || col >= cols)
        {
            return;
        }

        if (visited[row, col] || _grid[row][col] != 1 || (row == skipRow && col == skipCol))
        {
            return;
        }

        visited[row, col] = true;

        foreach (var (dRow, dCol) in Directions)
        {
            FloodFill(row + dRow, col + dCol, visited, skipRow, skipCol, rows, cols);
        }
    }

    [Benchmark]
    public int PrimitiveComposed()
    {
        var rows = _grid.Length;
        var cols = _grid[0].Length;

        if (CountIslandsExcluding(-1, -1, rows, cols) != 1)
        {
            return 0;
        }

        for (var r = 0; r < rows; r++)
        {
            for (var c = 0; c < cols; c++)
            {
                if (_grid[r][c] == 1 && CountIslandsExcluding(r, c, rows, cols) != 1)
                {
                    return 1;
                }
            }
        }

        return 2;
    }

    private int CountIslandsExcluding(int skipRow, int skipCol, int rows, int cols)
    {
        var visited = new bool[rows, cols];
        var count = 0;

        for (var r = 0; r < rows; r++)
        {
            for (var c = 0; c < cols; c++)
            {
                if (_grid[r][c] != 1 || visited[r, c] || (r == skipRow && c == skipCol))
                {
                    continue;
                }

                count++;

                foreach (var (row, col) in DepthFirstSearch.Traverse((r, c), Neighbors))
                {
                    visited[row, col] = true;
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

                if (_grid[nextRow][nextCol] != 1 || (nextRow == skipRow && nextCol == skipCol))
                {
                    continue;
                }

                yield return (nextRow, nextCol);
            }
        }
    }
}
