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
    [Params(20, 60)]
    public int Size;

    private int[][] _grid = null!;
    private int[][] _hits = null!;

    [GlobalSetup]
    public void Setup()
    {
        var random = new Random(803);
        _grid = new int[Size][];

        for (var r = 0; r < Size; r++)
        {
            _grid[r] = new int[Size];

            for (var c = 0; c < Size; c++)
            {
                _grid[r][c] = r == 0 || random.NextDouble() < 0.6 ? 1 : 0;
            }
        }

        var brickPositions = new List<(int Row, int Col)>();

        for (var r = 0; r < Size; r++)
        {
            for (var c = 0; c < Size; c++)
            {
                if (_grid[r][c] == 1)
                {
                    brickPositions.Add((r, c));
                }
            }
        }

        var hitCount = Math.Min(brickPositions.Count, (Size * Size) / 3);
        _hits = brickPositions
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

        var totalFallen = 0;
        var connectedBefore = CountConnectedToRoof(standing, rows, cols);

        foreach (var hit in _hits)
        {
            standing[hit[0], hit[1]] = false;
            var connectedAfter = CountConnectedToRoof(standing, rows, cols);
            var fallen = connectedBefore - connectedAfter - 1;
            totalFallen += fallen > 0 ? fallen : 0;
            connectedBefore = connectedAfter;
        }

        return totalFallen;
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

        var count = 0;

        while (frontier.Count > 0)
        {
            var (row, col) = frontier.Dequeue();
            count++;

            (int Row, int Col)[] neighbors = [(row - 1, col), (row + 1, col), (row, col - 1), (row, col + 1)];

            foreach (var (neighborRow, neighborCol) in neighbors)
            {
                if (neighborRow >= 0 && neighborRow < rows && neighborCol >= 0 && neighborCol < cols
                    && standing[neighborRow, neighborCol] && !visited[neighborRow, neighborCol])
                {
                    visited[neighborRow, neighborCol] = true;
                    frontier.Enqueue((neighborRow, neighborCol));
                }
            }
        }

        return count;
    }

    [Benchmark]
    public int ReverseTimeDisjointSet()
    {
        var rows = _grid.Length;
        var cols = _grid[0].Length;
        var roof = rows * cols;
        var present = new bool[rows, cols];

        for (var r = 0; r < rows; r++)
        {
            for (var c = 0; c < cols; c++)
            {
                present[r, c] = _grid[r][c] == 1;
            }
        }

        foreach (var hit in _hits)
        {
            present[hit[0], hit[1]] = false;
        }

        var components = new DisjointSet(roof + 1);
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

        for (var r = 0; r < rows; r++)
        {
            for (var c = 0; c < cols; c++)
            {
                if (present[r, c])
                {
                    ConnectToStandingNeighbors(components, size, present, rows, cols, r, c, roof);
                }
            }
        }

        var totalFallen = 0;

        for (var i = _hits.Length - 1; i >= 0; i--)
        {
            var row = _hits[i][0];
            var col = _hits[i][1];

            var beforeSize = size[components.Find(roof)];
            present[row, col] = true;
            size[(row * cols) + col] = 1;
            ConnectToStandingNeighbors(components, size, present, rows, cols, row, col, roof);
            var afterSize = size[components.Find(roof)];

            totalFallen += afterSize > beforeSize ? afterSize - beforeSize - 1 : 0;
        }

        return totalFallen;
    }

    private static void ConnectToStandingNeighbors(
        DisjointSet components, int[] size, bool[,] present, int rows, int cols, int row, int col, int roof)
    {
        var cellId = (row * cols) + col;

        if (row == 0)
        {
            Union(components, size, cellId, roof);
        }

        (int Row, int Col)[] neighbors = [(row - 1, col), (row + 1, col), (row, col - 1), (row, col + 1)];

        foreach (var (neighborRow, neighborCol) in neighbors)
        {
            if (neighborRow >= 0 && neighborRow < rows && neighborCol >= 0 && neighborCol < cols
                && present[neighborRow, neighborCol])
            {
                Union(components, size, cellId, (neighborRow * cols) + neighborCol);
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
}
