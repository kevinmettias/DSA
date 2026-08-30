using BenchmarkDotNet.Attributes;
using DSAExperimentation.Algorithms.Traversal.DepthFirst;

namespace DSAExperimentation.Benchmarks.ProblemSolutions;

// Check if There is a Valid Path in a Grid (LC 1391): a hand-rolled recursive DFS
// over a bool[,] visited array vs. this repo's own DepthFirstSearch.Traverse over
// a bespoke street-opening successor function (the same implicit-graph shape
// PacificAtlanticWaterFlowBenchmarks uses), checking whether the bottom-right
// corner appears in the reachable set from (0,0). Both walk the identical
// street-compatibility rule, so the comparison isolates the traversal
// machinery (explicit stack + HashSet vs. recursion + array) rather than the
// per-cell logic.
[MemoryDiagnoser]
public class CheckIfThereIsAValidPathInAGridBenchmarks
{
    private static readonly Dictionary<int, (int DRow, int DCol)[]> Openings = new()
    {
        [1] = [(0, -1), (0, 1)],
        [2] = [(-1, 0), (1, 0)],
        [3] = [(0, -1), (1, 0)],
        [4] = [(0, 1), (1, 0)],
        [5] = [(0, -1), (-1, 0)],
        [6] = [(0, 1), (-1, 0)],
    };

    [Params(10, 30)]
    public int Size;

    private int[][] _grid = null!;

    [GlobalSetup]
    public void Setup()
    {
        var random = new Random(1391);
        _grid = Enumerable.Range(0, Size)
            .Select(_ => Enumerable.Range(0, Size).Select(_ => random.Next(1, 7)).ToArray())
            .ToArray();
    }

    [Benchmark(Baseline = true)]
    public bool RecursiveDfs()
    {
        var visited = new bool[Size, Size];
        return Dfs(0, 0, visited);
    }

    private bool Dfs(int row, int col, bool[,] visited)
    {
        if (visited[row, col])
        {
            return false;
        }

        visited[row, col] = true;

        if (row == Size - 1 && col == Size - 1)
        {
            return true;
        }

        foreach (var (dRow, dCol) in Openings[_grid[row][col]])
        {
            var nextRow = row + dRow;
            var nextCol = col + dCol;

            if (nextRow < 0 || nextRow >= Size || nextCol < 0 || nextCol >= Size || visited[nextRow, nextCol])
            {
                continue;
            }

            if (Array.IndexOf(Openings[_grid[nextRow][nextCol]], (-dRow, -dCol)) < 0)
            {
                continue;
            }

            if (Dfs(nextRow, nextCol, visited))
            {
                return true;
            }
        }

        return false;
    }

    [Benchmark]
    public bool DepthFirstSearchTraverse()
    {
        var target = (Row: Size - 1, Col: Size - 1);
        return DepthFirstSearch.Traverse((Row: 0, Col: 0), Neighbors).Contains(target);
    }

    private IEnumerable<(int Row, int Col)> Neighbors((int Row, int Col) cell)
    {
        foreach (var (dRow, dCol) in Openings[_grid[cell.Row][cell.Col]])
        {
            var next = (Row: cell.Row + dRow, Col: cell.Col + dCol);

            if (next.Row < 0 || next.Row >= Size || next.Col < 0 || next.Col >= Size)
            {
                continue;
            }

            if (Array.IndexOf(Openings[_grid[next.Row][next.Col]], (-dRow, -dCol)) < 0)
            {
                continue;
            }

            yield return next;
        }
    }
}
