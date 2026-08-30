using BenchmarkDotNet.Attributes;

namespace DSAExperimentation.Benchmarks.ProblemSolutions;

// Rotting Oranges (LC 994): an independent BFS per fresh orange (a freshly
// allocated visited grid and BCL Queue re-walked outward from each fresh cell
// until it hits a rotten one, taking the max over all of them) vs. one shared
// multi-source BFS using this repo's own Queue<TElement>, seeded with every
// already-rotten orange at once - O(rows*cols) total instead of O(rows*cols) work
// times one BFS per fresh starting cell.
[MemoryDiagnoser]
public class RottingOrangesBenchmarks
{
    private static readonly (int DRow, int DCol)[] Directions = [(-1, 0), (1, 0), (0, -1), (0, 1)];

    [Params(10, 25)]
    public int Size;

    private int[][] _grid = null!;

    [GlobalSetup]
    public void Setup()
    {
        var random = new Random(1);
        _grid = Enumerable.Range(0, Size)
            .Select(_ => Enumerable.Range(0, Size).Select(_ => random.Next(0, 10) switch
            {
                0 => 2,
                1 => 0,
                _ => 1,
            }).ToArray())
            .ToArray();
        _grid[0][0] = 2;
    }

    [Benchmark(Baseline = true)]
    public int PerCellBfs()
    {
        var maxMinutes = 0;

        for (var r = 0; r < Size; r++)
        {
            for (var c = 0; c < Size; c++)
            {
                if (_grid[r][c] != 1)
                {
                    continue;
                }

                var minutes = NearestRottenDistance(r, c);

                if (minutes < 0)
                {
                    return -1;
                }

                maxMinutes = Math.Max(maxMinutes, minutes);
            }
        }

        return maxMinutes;
    }

    private int NearestRottenDistance(int startRow, int startCol)
    {
        var visited = new bool[Size, Size];
        var queue = new System.Collections.Generic.Queue<(int Row, int Col, int Minutes)>();
        visited[startRow, startCol] = true;
        queue.Enqueue((startRow, startCol, 0));

        while (queue.Count > 0)
        {
            var (row, col, minutes) = queue.Dequeue();

            foreach (var (dRow, dCol) in Directions)
            {
                var nextRow = row + dRow;
                var nextCol = col + dCol;

                if (nextRow < 0 || nextRow >= Size || nextCol < 0 || nextCol >= Size || visited[nextRow, nextCol])
                {
                    continue;
                }

                if (_grid[nextRow][nextCol] == 2)
                {
                    return minutes + 1;
                }

                if (_grid[nextRow][nextCol] == 1)
                {
                    visited[nextRow, nextCol] = true;
                    queue.Enqueue((nextRow, nextCol, minutes + 1));
                }
            }
        }

        return -1;
    }

    [Benchmark]
    public int MultiSourceBfs()
    {
        var minutesToRot = new int[Size][];
        var frontier = new DSAExperimentation.DataStructures.Queue.Queue<(int Row, int Col)>();
        var visited = new bool[Size, Size];
        var freshCount = 0;

        for (var r = 0; r < Size; r++)
        {
            minutesToRot[r] = new int[Size];

            for (var c = 0; c < Size; c++)
            {
                if (_grid[r][c] == 2)
                {
                    frontier.Enqueue((r, c));
                    visited[r, c] = true;
                }
                else if (_grid[r][c] == 1)
                {
                    freshCount++;
                }
            }
        }

        var minutesElapsed = 0;

        while (frontier.TryDequeue(out var cell))
        {
            foreach (var (dRow, dCol) in Directions)
            {
                var nextRow = cell.Row + dRow;
                var nextCol = cell.Col + dCol;

                if (nextRow < 0 || nextRow >= Size || nextCol < 0 || nextCol >= Size || visited[nextRow, nextCol] || _grid[nextRow][nextCol] != 1)
                {
                    continue;
                }

                visited[nextRow, nextCol] = true;
                minutesToRot[nextRow][nextCol] = minutesToRot[cell.Row][cell.Col] + 1;
                freshCount--;
                minutesElapsed = Math.Max(minutesElapsed, minutesToRot[nextRow][nextCol]);
                frontier.Enqueue((nextRow, nextCol));
            }
        }

        return freshCount == 0 ? minutesElapsed : -1;
    }
}
