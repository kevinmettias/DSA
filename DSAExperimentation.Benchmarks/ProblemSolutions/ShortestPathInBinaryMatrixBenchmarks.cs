using BenchmarkDotNet.Attributes;

namespace DSAExperimentation.Benchmarks.ProblemSolutions;

// Shortest Path in Binary Matrix (LC 1091): single-source 8-directional BFS from the
// top-left cell using the BCL's own Queue<T> as the frontier vs. the identical walk
// using this repo's own Queue<TElement> - the same baseline-vs-repo-primitive
// comparison RottingOrangesBenchmarks/ZeroOneMatrixBenchmarks already run for their
// 4-directional grid BFS sweeps, just with all 8 king-move directions here since
// diagonal steps are allowed. Grid cells are blocked with low enough probability that
// a clear path from corner to corner almost always exists, so both strategies do
// comparable real BFS work instead of failing fast.
[MemoryDiagnoser]
public class ShortestPathInBinaryMatrixBenchmarks
{
    private static readonly (int DRow, int DCol)[] Directions =
    [
        (-1, -1), (-1, 0), (-1, 1),
        (0, -1), (0, 1),
        (1, -1), (1, 0), (1, 1),
    ];

    [Params(10, 25)]
    public int Size;

    private int[][] _grid = null!;

    [GlobalSetup]
    public void Setup()
    {
        var random = new Random(1);
        _grid = Enumerable.Range(0, Size)
            .Select(_ => Enumerable.Range(0, Size).Select(_ => random.Next(0, 10) == 0 ? 1 : 0).ToArray())
            .ToArray();
        _grid[0][0] = 0;
        _grid[Size - 1][Size - 1] = 0;
    }

    [Benchmark(Baseline = true)]
    public int BclQueueBfs()
    {
        if (_grid[0][0] != 0 || _grid[Size - 1][Size - 1] != 0)
        {
            return -1;
        }

        var distance = new int[Size, Size];
        for (var r = 0; r < Size; r++)
        {
            for (var c = 0; c < Size; c++)
            {
                distance[r, c] = -1;
            }
        }

        var frontier = new System.Collections.Generic.Queue<(int Row, int Col)>();
        distance[0, 0] = 1;
        frontier.Enqueue((0, 0));

        while (frontier.Count > 0)
        {
            var cell = frontier.Dequeue();

            if (cell.Row == Size - 1 && cell.Col == Size - 1)
            {
                return distance[cell.Row, cell.Col];
            }

            foreach (var (dRow, dCol) in Directions)
            {
                var nextRow = cell.Row + dRow;
                var nextCol = cell.Col + dCol;

                if (nextRow < 0 || nextRow >= Size || nextCol < 0 || nextCol >= Size
                    || _grid[nextRow][nextCol] != 0 || distance[nextRow, nextCol] != -1)
                {
                    continue;
                }

                distance[nextRow, nextCol] = distance[cell.Row, cell.Col] + 1;
                frontier.Enqueue((nextRow, nextCol));
            }
        }

        return -1;
    }

    [Benchmark]
    public int RepoQueueBfs()
    {
        if (_grid[0][0] != 0 || _grid[Size - 1][Size - 1] != 0)
        {
            return -1;
        }

        var distance = new int[Size, Size];
        for (var r = 0; r < Size; r++)
        {
            for (var c = 0; c < Size; c++)
            {
                distance[r, c] = -1;
            }
        }

        var frontier = new DSAExperimentation.DataStructures.Queue.Queue<(int Row, int Col)>();
        distance[0, 0] = 1;
        frontier.Enqueue((0, 0));

        while (frontier.TryDequeue(out var cell))
        {
            if (cell.Row == Size - 1 && cell.Col == Size - 1)
            {
                return distance[cell.Row, cell.Col];
            }

            foreach (var (dRow, dCol) in Directions)
            {
                var nextRow = cell.Row + dRow;
                var nextCol = cell.Col + dCol;

                if (nextRow < 0 || nextRow >= Size || nextCol < 0 || nextCol >= Size
                    || _grid[nextRow][nextCol] != 0 || distance[nextRow, nextCol] != -1)
                {
                    continue;
                }

                distance[nextRow, nextCol] = distance[cell.Row, cell.Col] + 1;
                frontier.Enqueue((nextRow, nextCol));
            }
        }

        return -1;
    }
}
