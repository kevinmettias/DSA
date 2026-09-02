using BenchmarkDotNet.Attributes;
using DSAExperimentation.DataStructures.DisjointSet;

namespace DSAExperimentation.Benchmarks.ProblemSolutions;

// Detect Cycles in 2D Grid (LC 1559): an explicit-stack, parent-tracked DFS per
// unvisited component (baseline - the classic hand-rolled undirected-cycle check,
// visited/parent bookkeeping owned entirely by this method) vs. this repo's own
// DisjointSet unioning each cell with its same-character right/down neighbor and
// flagging a cycle the moment two cells are already connected - the same
// "hand-rolled traversal vs. repo Union-Find" contrast RegionsCutBySlashesBenchmarks
// already established for a different grid-cycle shape (triangle regions instead of
// same-character components).
[MemoryDiagnoser]
public class DetectCyclesIn2DGridBenchmarks
{
    // LC problem number, reused as the deterministic seed.
    private const int RandomSeed = 1559;

    private static readonly char[] Letters = ['a', 'b', 'c'];
    private static readonly (int DRow, int DCol)[] Directions = [(-1, 0), (1, 0), (0, -1), (0, 1)];

    [Params(30, 150)]
    public int GridSize;

    private char[][] _grid = null!;

    [GlobalSetup]
    public void Setup()
    {
        var random = new Random(RandomSeed);
        _grid = new char[GridSize][];

        for (var r = 0; r < GridSize; r++)
        {
            _grid[r] = new char[GridSize];
            for (var c = 0; c < GridSize; c++)
            {
                _grid[r][c] = Letters[random.Next(Letters.Length)];
            }
        }
    }

    [Benchmark(Baseline = true)]
    public bool ParentTrackedDepthFirstSearch()
    {
        var rows = _grid.Length;
        var cols = _grid[0].Length;
        var visited = new bool[rows, cols];

        for (var startRow = 0; startRow < rows; startRow++)
        {
            for (var startCol = 0; startCol < cols; startCol++)
            {
                if (!visited[startRow, startCol] && HasCycleFrom(startRow, startCol, visited))
                {
                    return true;
                }
            }
        }

        return false;
    }

    private bool HasCycleFrom(int startRow, int startCol, bool[,] visited)
    {
        var stack = new Stack<(int Row, int Col, int ParentRow, int ParentCol)>();
        stack.Push((startRow, startCol, -1, -1));
        visited[startRow, startCol] = true;

        while (stack.Count > 0)
        {
            var current = stack.Pop();

            foreach (var direction in Directions)
            {
                if (HasCycleAtNeighbor(current, direction, visited, stack))
                {
                    return true;
                }
            }
        }

        return false;
    }

    private bool HasCycleAtNeighbor(
        (int Row, int Col, int ParentRow, int ParentCol) current,
        (int DRow, int DCol) direction,
        bool[,] visited,
        Stack<(int Row, int Col, int ParentRow, int ParentCol)> stack)
    {
        var nextRow = current.Row + direction.DRow;
        var nextCol = current.Col + direction.DCol;

        if (nextRow < 0 || nextRow >= _grid.Length || nextCol < 0 || nextCol >= _grid[0].Length)
        {
            return false;
        }

        if (_grid[nextRow][nextCol] != _grid[current.Row][current.Col]
            || (nextRow == current.ParentRow && nextCol == current.ParentCol))
        {
            return false;
        }

        if (visited[nextRow, nextCol])
        {
            return true;
        }

        visited[nextRow, nextCol] = true;
        stack.Push((nextRow, nextCol, current.Row, current.Col));
        return false;
    }

    [Benchmark]
    public bool DisjointSetEdgeUnion()
    {
        var rows = _grid.Length;
        var cols = _grid[0].Length;
        var components = new DisjointSet(rows * cols);

        for (var row = 0; row < rows; row++)
        {
            for (var col = 0; col < cols; col++)
            {
                if (HasCycleThroughNeighbor(components, cols, (row, col), (row, col + 1)))
                {
                    return true;
                }

                if (HasCycleThroughNeighbor(components, cols, (row, col), (row + 1, col)))
                {
                    return true;
                }
            }
        }

        return false;
    }

    private bool HasCycleThroughNeighbor(
        DisjointSet components, int cols, (int Row, int Col) cell, (int Row, int Col) neighbor)
    {
        if (neighbor.Row >= _grid.Length || neighbor.Col >= cols
            || _grid[neighbor.Row][neighbor.Col] != _grid[cell.Row][cell.Col])
        {
            return false;
        }

        var id = (cell.Row * cols) + cell.Col;
        var neighborId = (neighbor.Row * cols) + neighbor.Col;

        if (components.IsConnected(id, neighborId))
        {
            return true;
        }

        components.Union(id, neighborId);
        return false;
    }
}
