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

    // LC 1568: a rectangular all-land grid has no articulation cell, so MinDays
    // always bottoms out at "already disconnected in 2 or fewer removals".
    private const int NoArticulationCellFound = 2;

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
        return MinDaysUsing(CountIslandsNaive, rows, cols);
    }

    // Shared by both benchmarks: try removing no cell, then every single land
    // cell in turn, and report how many removals (if any) it takes to break
    // the grid into more than one island. `countIslands` supplies the two
    // competing ways to count connected components while skipping one cell.
    private int MinDaysUsing(Func<int, int, int, int, int> countIslands, int rows, int cols)
    {
        if (countIslands(-1, -1, rows, cols) != 1)
        {
            return 0;
        }

        for (var r = 0; r < rows; r++)
        {
            for (var c = 0; c < cols; c++)
            {
                if (_grid[r][c] == 1 && countIslands(r, c, rows, cols) != 1)
                {
                    return 1;
                }
            }
        }

        return NoArticulationCellFound;
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
                FloodFill(new GridPosition(r, c), visited, new GridPosition(skipRow, skipCol), new GridBounds(rows, cols));
            }
        }

        return count;
    }

    private readonly record struct GridPosition(int Row, int Col);

    private readonly record struct GridBounds(int Rows, int Cols);

    private void FloodFill(GridPosition position, bool[,] visited, GridPosition skip, GridBounds bounds)
    {
        if (position.Row < 0 || position.Row >= bounds.Rows || position.Col < 0 || position.Col >= bounds.Cols)
        {
            return;
        }

        if (visited[position.Row, position.Col] || _grid[position.Row][position.Col] != 1 || position == skip)
        {
            return;
        }

        visited[position.Row, position.Col] = true;

        foreach (var (dRow, dCol) in Directions)
        {
            FloodFill(new GridPosition(position.Row + dRow, position.Col + dCol), visited, skip, bounds);
        }
    }

    [Benchmark]
    public int PrimitiveComposed()
    {
        var rows = _grid.Length;
        var cols = _grid[0].Length;
        return MinDaysUsing(CountIslandsExcluding, rows, cols);
    }

    private int CountIslandsExcluding(int skipRow, int skipCol, int rows, int cols)
    {
        var visited = new bool[rows, cols];
        var count = 0;
        var bounds = new GridBounds(rows, cols);
        var skip = new GridPosition(skipRow, skipCol);

        for (var r = 0; r < rows; r++)
        {
            for (var c = 0; c < cols; c++)
            {
                if (VisitIslandAt(new GridPosition(r, c), visited, bounds, skip))
                {
                    count++;
                }
            }
        }

        return count;
    }

    private bool VisitIslandAt(GridPosition cell, bool[,] visited, GridBounds bounds, GridPosition skip)
    {
        if (_grid[cell.Row][cell.Col] != 1 || visited[cell.Row, cell.Col] || cell == skip)
        {
            return false;
        }

        foreach (var (row, col) in DepthFirstSearch.Traverse((cell.Row, cell.Col), p => Neighbors(p, bounds, skip)))
        {
            visited[row, col] = true;
        }

        return true;
    }

    private IEnumerable<(int Row, int Col)> Neighbors((int Row, int Col) p, GridBounds bounds, GridPosition skip)
    {
        foreach (var (dRow, dCol) in Directions)
        {
            var next = new GridPosition(p.Row + dRow, p.Col + dCol);

            if (!IsVisitableNeighbor(next, bounds, skip))
            {
                continue;
            }

            yield return (next.Row, next.Col);
        }
    }

    private bool IsVisitableNeighbor(GridPosition next, GridBounds bounds, GridPosition skip)
    {
        if (next.Row < 0 || next.Row >= bounds.Rows || next.Col < 0 || next.Col >= bounds.Cols)
        {
            return false;
        }

        return _grid[next.Row][next.Col] == 1 && next != skip;
    }
}
