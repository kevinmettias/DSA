using BenchmarkDotNet.Attributes;

namespace DSAExperimentation.Benchmarks.ProblemSolutions;

// 01 Matrix (LC 542): an independent BFS per 1 cell (a freshly-allocated visited
// grid and BCL Queue re-walked outward from each cell until it hits a 0, O(rows*
// cols) work times rows*cols starting cells) vs. one shared multi-source BFS using
// this repo's own Queue<TElement>, seeded with every 0 cell at once - O(rows*cols)
// total, each cell discovered exactly once via its true nearest source.
[MemoryDiagnoser]
public class ZeroOneMatrixBenchmarks
{
    private static readonly (int DRow, int DCol)[] Directions = [(-1, 0), (1, 0), (0, -1), (0, 1)];

    [Params(10, 25)]
    public int Size;

    private int[][] _matrix = null!;

    [GlobalSetup]
    public void Setup()
    {
        var random = new Random(1);
        _matrix = Enumerable.Range(0, Size)
            .Select(_ => Enumerable.Range(0, Size).Select(_ => random.Next(0, 5) == 0 ? 0 : 1).ToArray())
            .ToArray();
        _matrix[0][0] = 0;
    }

    [Benchmark(Baseline = true)]
    public int[][] PerCellBfs()
    {
        var result = new int[Size][];

        for (var r = 0; r < Size; r++)
        {
            result[r] = new int[Size];

            for (var c = 0; c < Size; c++)
            {
                result[r][c] = NearestZeroDistance(r, c);
            }
        }

        return result;
    }

    private int NearestZeroDistance(int startRow, int startCol)
    {
        if (_matrix[startRow][startCol] == 0)
        {
            return 0;
        }

        var visited = new bool[Size, Size];
        var queue = new System.Collections.Generic.Queue<(int Row, int Col, int Dist)>();
        visited[startRow, startCol] = true;
        queue.Enqueue((startRow, startCol, 0));

        while (queue.Count > 0)
        {
            var (row, col, dist) = queue.Dequeue();

            foreach (var (dRow, dCol) in Directions)
            {
                var nextRow = row + dRow;
                var nextCol = col + dCol;

                if (nextRow < 0 || nextRow >= Size || nextCol < 0 || nextCol >= Size || visited[nextRow, nextCol])
                {
                    continue;
                }

                if (_matrix[nextRow][nextCol] == 0)
                {
                    return dist + 1;
                }

                visited[nextRow, nextCol] = true;
                queue.Enqueue((nextRow, nextCol, dist + 1));
            }
        }

        return -1;
    }

    [Benchmark]
    public int[][] MultiSourceBfs()
    {
        var distances = new int[Size][];
        var frontier = new DSAExperimentation.DataStructures.Queue.Queue<(int Row, int Col)>();

        for (var r = 0; r < Size; r++)
        {
            distances[r] = new int[Size];

            for (var c = 0; c < Size; c++)
            {
                distances[r][c] = _matrix[r][c] == 0 ? 0 : -1;

                if (_matrix[r][c] == 0)
                {
                    frontier.Enqueue((r, c));
                }
            }
        }

        while (frontier.TryDequeue(out var cell))
        {
            foreach (var (dRow, dCol) in Directions)
            {
                var nextRow = cell.Row + dRow;
                var nextCol = cell.Col + dCol;

                if (nextRow < 0 || nextRow >= Size || nextCol < 0 || nextCol >= Size || distances[nextRow][nextCol] != -1)
                {
                    continue;
                }

                distances[nextRow][nextCol] = distances[cell.Row][cell.Col] + 1;
                frontier.Enqueue((nextRow, nextCol));
            }
        }

        return distances;
    }
}
