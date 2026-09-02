using BenchmarkDotNet.Attributes;
using DSAExperimentation.Algorithms.Traversal.DepthFirst;
using DSAExperimentation.DataStructures.Set;

namespace DSAExperimentation.Benchmarks.ProblemSolutions;

// Pacific Atlantic Water Flow (LC 417): a per-cell DFS re-scan (for every cell,
// independently DFS downhill toward each ocean's border with a freshly-allocated
// rows*cols visited grid, O(rows*cols) work times rows*cols starting cells) vs. a
// multi-source reverse-flow flood fill from every border cell using this repo's
// own DepthFirstSearch.Traverse and Set<(int,int)> - O(rows*cols) total, each cell
// visited at most once per ocean regardless of how many border cells it takes to
// reach it.
[MemoryDiagnoser]
public class PacificAtlanticWaterFlowBenchmarks
{
    private static readonly (int DRow, int DCol)[] Directions = [(1, 0), (-1, 0), (0, 1), (0, -1)];

    private const int MaxHeight = 1_000;

    [Params(10, 25)]
    public int Size;

    private int[][] _heights = null!;

    [GlobalSetup]
    public void Setup()
    {
        var random = new Random(1);
        _heights = Enumerable.Range(0, Size)
            .Select(_ => Enumerable.Range(0, Size).Select(_ => random.Next(0, MaxHeight)).ToArray())
            .ToArray();
    }

    [Benchmark(Baseline = true)]
    public int PerCellDfs()
    {
        var count = 0;

        for (var r = 0; r < Size; r++)
        {
            for (var c = 0; c < Size; c++)
            {
                if (CanReachBorder(r, c, pacific: true) && CanReachBorder(r, c, pacific: false))
                {
                    count++;
                }
            }
        }

        return count;
    }

    private bool CanReachBorder(int startRow, int startCol, bool pacific)
    {
        var visited = new bool[Size, Size];
        return Dfs(startRow, startCol, visited, pacific);
    }

    private bool Dfs(int row, int col, bool[,] visited, bool pacific)
    {
        if (visited[row, col])
        {
            return false;
        }

        visited[row, col] = true;

        if (IsBorderCell(row, col, pacific))
        {
            return true;
        }

        return TryReachAnyDirection(row, col, visited, pacific);
    }

    private bool IsBorderCell(int row, int col, bool pacific)
    {
        if (pacific)
        {
            return row == 0 || col == 0;
        }

        return row == Size - 1 || col == Size - 1;
    }

    private bool TryReachAnyDirection(int row, int col, bool[,] visited, bool pacific)
    {
        foreach (var direction in Directions)
        {
            if (TryReachViaDirection((row, col), direction, visited, pacific))
            {
                return true;
            }
        }

        return false;
    }

    private bool TryReachViaDirection(
        (int Row, int Col) from, (int DRow, int DCol) direction, bool[,] visited, bool pacific)
    {
        var nextRow = from.Row + direction.DRow;
        var nextCol = from.Col + direction.DCol;

        if (nextRow < 0 || nextRow >= Size || nextCol < 0 || nextCol >= Size || visited[nextRow, nextCol])
        {
            return false;
        }

        if (_heights[nextRow][nextCol] > _heights[from.Row][from.Col])
        {
            return false;
        }

        return Dfs(nextRow, nextCol, visited, pacific);
    }

    [Benchmark]
    public int MultiSourceFloodFill()
    {
        var pacific = new Set<(int Row, int Col)>();
        var atlantic = new Set<(int Row, int Col)>();

        FloodBorders(pacific, atlantic);

        return CountReachableFromBoth(pacific, atlantic);
    }

    private void FloodBorders(Set<(int Row, int Col)> pacific, Set<(int Row, int Col)> atlantic)
    {
        for (var r = 0; r < Size; r++)
        {
            FloodFrom((r, 0), pacific);
            FloodFrom((r, Size - 1), atlantic);
        }

        for (var c = 0; c < Size; c++)
        {
            FloodFrom((0, c), pacific);
            FloodFrom((Size - 1, c), atlantic);
        }
    }

    private int CountReachableFromBoth(Set<(int Row, int Col)> pacific, Set<(int Row, int Col)> atlantic)
    {
        var count = 0;

        for (var r = 0; r < Size; r++)
        {
            for (var c = 0; c < Size; c++)
            {
                if (pacific.Has((r, c)) && atlantic.Has((r, c)))
                {
                    count++;
                }
            }
        }

        return count;
    }

    private void FloodFrom((int Row, int Col) start, Set<(int Row, int Col)> reached)
    {
        if (reached.Has(start))
        {
            return;
        }

        foreach (var node in DepthFirstSearch.Traverse(start, Neighbors))
        {
            reached.TryAdd(node);
        }
    }

    private IEnumerable<(int Row, int Col)> Neighbors((int Row, int Col) p)
    {
        foreach (var (dRow, dCol) in Directions)
        {
            var nextRow = p.Row + dRow;
            var nextCol = p.Col + dCol;

            if (nextRow < 0 || nextRow >= Size || nextCol < 0 || nextCol >= Size)
            {
                continue;
            }

            if (_heights[nextRow][nextCol] < _heights[p.Row][p.Col])
            {
                continue;
            }

            yield return (nextRow, nextCol);
        }
    }
}
