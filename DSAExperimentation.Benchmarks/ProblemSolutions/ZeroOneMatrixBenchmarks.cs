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
    private const int ZeroCellProbabilityDenominator = 5;

    [Params(10, 25)]
    public int Size;

    private int[][] _matrix = null!;

    [GlobalSetup]
    public void Setup()
    {
        var random = new Random(1);
        _matrix = Enumerable.Range(0, Size)
            .Select(_ => Enumerable.Range(0, Size).Select(_ => random.Next(0, ZeroCellProbabilityDenominator) == 0 ? 0 : 1).ToArray())
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

        var state = CreateSingleSourceBfsState(startRow, startCol);
        return RunSingleSourceBfs(state);
    }

    private SingleSourceBfsState CreateSingleSourceBfsState(int startRow, int startCol)
    {
        var visited = new bool[Size, Size];
        var queue = new System.Collections.Generic.Queue<(int Row, int Col, int Dist)>();
        visited[startRow, startCol] = true;
        queue.Enqueue((startRow, startCol, 0));
        return new SingleSourceBfsState(visited, queue);
    }

    private int RunSingleSourceBfs(SingleSourceBfsState state)
    {
        while (state.Queue.Count > 0)
        {
            var current = state.Queue.Dequeue();

            foreach (var direction in Directions)
            {
                var foundDistance = TryVisitNeighbor(current, direction, state);

                if (foundDistance is not null)
                {
                    return foundDistance.Value;
                }
            }
        }

        return -1;
    }

    private int? TryVisitNeighbor(
        (int Row, int Col, int Dist) current, (int DRow, int DCol) direction, SingleSourceBfsState state)
    {
        var nextRow = current.Row + direction.DRow;
        var nextCol = current.Col + direction.DCol;

        if (nextRow < 0 || nextRow >= Size || nextCol < 0 || nextCol >= Size || state.Visited[nextRow, nextCol])
        {
            return null;
        }

        if (_matrix[nextRow][nextCol] == 0)
        {
            return current.Dist + 1;
        }

        state.Visited[nextRow, nextCol] = true;
        state.Queue.Enqueue((nextRow, nextCol, current.Dist + 1));
        return null;
    }

    private readonly record struct SingleSourceBfsState(
        bool[,] Visited, System.Collections.Generic.Queue<(int Row, int Col, int Dist)> Queue);

    [Benchmark]
    public int[][] MultiSourceBfs()
    {
        var (distances, frontier) = InitializeMultiSourceFrontier();
        RunMultiSourceBfs(distances, frontier);
        return distances;
    }

    private (int[][] Distances, DSAExperimentation.DataStructures.Queue.Queue<(int Row, int Col)> Frontier)
        InitializeMultiSourceFrontier()
    {
        var distances = new int[Size][];
        var frontier = new DSAExperimentation.DataStructures.Queue.Queue<(int Row, int Col)>();

        for (var r = 0; r < Size; r++)
        {
            distances[r] = SeedRow(r, frontier);
        }

        return (distances, frontier);
    }

    private int[] SeedRow(int row, DSAExperimentation.DataStructures.Queue.Queue<(int Row, int Col)> frontier)
    {
        var distanceRow = new int[Size];

        for (var c = 0; c < Size; c++)
        {
            distanceRow[c] = _matrix[row][c] == 0 ? 0 : -1;

            if (_matrix[row][c] == 0)
            {
                frontier.Enqueue((row, c));
            }
        }

        return distanceRow;
    }

    private void RunMultiSourceBfs(
        int[][] distances, DSAExperimentation.DataStructures.Queue.Queue<(int Row, int Col)> frontier)
    {
        while (frontier.TryDequeue(out var cell))
        {
            foreach (var direction in Directions)
            {
                RelaxNeighbor(cell, direction, distances, frontier);
            }
        }
    }

    private void RelaxNeighbor(
        (int Row, int Col) cell,
        (int DRow, int DCol) direction,
        int[][] distances,
        DSAExperimentation.DataStructures.Queue.Queue<(int Row, int Col)> frontier)
    {
        var nextRow = cell.Row + direction.DRow;
        var nextCol = cell.Col + direction.DCol;

        if (nextRow < 0 || nextRow >= Size || nextCol < 0 || nextCol >= Size || distances[nextRow][nextCol] != -1)
        {
            return;
        }

        distances[nextRow][nextCol] = distances[cell.Row][cell.Col] + 1;
        frontier.Enqueue((nextRow, nextCol));
    }
}
