using BenchmarkDotNet.Attributes;
using DSAExperimentation.DataStructures.Graph.ShortestPaths;
using DSAExperimentation.DataStructures.Heap;

namespace DSAExperimentation.Benchmarks.ProblemSolutions;

// Minimum Cost to Make at Least One Valid Path in a Grid (LC 1368): the
// textbook O(V^2) Dijkstra (linear-scan the unsettled distance map for the
// current minimum every round, no priority queue) vs. this repo's own
// Heap<Element,TOrder> ordered by ByPriorityOrder<TNode,TWeight> - the same
// heap-backed frontier SwimInRisingWaterBenchmarks' HeapDijkstra and
// ShortestPathAlgorithmBenchmarks' Dijkstra already use - giving O(E log V).
// Grid cells hold a random arrow direction (1..4); the cost to cross an edge is
// 0 when it follows the source cell's own arrow, 1 otherwise.
[MemoryDiagnoser]
public class MinimumCostToMakeAtLeastOneValidPathInAGridBenchmarks
{
    private static readonly (int DRow, int DCol)[] Directions = [(0, 1), (0, -1), (1, 0), (-1, 0)];

    // LC problem number, reused as the deterministic benchmark seed.
    private const int RandomSeed = 1368;

    private const int ArrowDirectionUpperBound = 5;

    [Params(15, 40)]
    public int Size;

    private int[][] _grid = null!;

    [GlobalSetup]
    public void Setup()
    {
        var random = new Random(RandomSeed);
        _grid = new int[Size][];

        for (var row = 0; row < Size; row++)
        {
            _grid[row] = new int[Size];

            for (var col = 0; col < Size; col++)
            {
                _grid[row][col] = random.Next(1, ArrowDirectionUpperBound);
            }
        }
    }

    [Benchmark(Baseline = true)]
    public int NaiveDijkstra()
    {
        var n = Size;
        var distances = new int?[n, n];
        var settled = new bool[n, n];
        distances[0, 0] = 0;

        for (var round = 0; round < n * n; round++)
        {
            var cell = FindUnsettledMinimum(distances, settled, n);

            if (cell.Row < 0)
            {
                break;
            }

            if (SettleAndRelax(cell, n, distances, settled))
            {
                return distances[cell.Row, cell.Col]!.Value;
            }
        }

        return distances[n - 1, n - 1]!.Value;
    }

    private bool SettleAndRelax((int Row, int Col) cell, int n, int?[,] distances, bool[,] settled)
    {
        settled[cell.Row, cell.Col] = true;

        if (cell.Row == n - 1 && cell.Col == n - 1)
        {
            return true;
        }

        RelaxNeighbors(cell, n, distances, settled);
        return false;
    }

    private void RelaxNeighbors((int Row, int Col) cell, int n, int?[,] distances, bool[,] settled)
    {
        for (var direction = 0; direction < Directions.Length; direction++)
        {
            var (dRow, dCol) = Directions[direction];
            var (nextRow, nextCol) = (cell.Row + dRow, cell.Col + dCol);

            if (nextRow < 0 || nextRow >= n || nextCol < 0 || nextCol >= n || settled[nextRow, nextCol])
            {
                continue;
            }

            var weight = _grid[cell.Row][cell.Col] == direction + 1 ? 0 : 1;
            var candidate = distances[cell.Row, cell.Col]!.Value + weight;

            if (distances[nextRow, nextCol] is null || candidate < distances[nextRow, nextCol])
            {
                distances[nextRow, nextCol] = candidate;
            }
        }
    }

    private static (int Row, int Col) FindUnsettledMinimum(int?[,] distances, bool[,] settled, int n)
    {
        var best = int.MaxValue;
        var result = (-1, -1);

        for (var row = 0; row < n; row++)
        {
            for (var col = 0; col < n; col++)
            {
                if (!settled[row, col] && distances[row, col] is int distance && distance < best)
                {
                    best = distance;
                    result = (row, col);
                }
            }
        }

        return result;
    }

    [Benchmark]
    public int HeapDijkstra()
    {
        var n = Size;
        var distances = new Dictionary<(int Row, int Col), int> { [(0, 0)] = 0 };
        var settled = new HashSet<(int Row, int Col)>();
        var frontier = new Heap<((int Row, int Col) Node, int Priority), ByPriorityOrder<(int Row, int Col), int>>();
        frontier.Push(((0, 0), 0));

        while (frontier.TryPop(out var entry))
        {
            if (!settled.Add(entry.Node))
            {
                continue;
            }

            if (entry.Node.Row == n - 1 && entry.Node.Col == n - 1)
            {
                return entry.Priority;
            }

            RelaxNeighborsIntoFrontier(entry.Node, n, distances, frontier);
        }

        return distances[(n - 1, n - 1)];
    }

    private void RelaxNeighborsIntoFrontier(
        (int Row, int Col) cell,
        int n,
        Dictionary<(int Row, int Col), int> distances,
        Heap<((int Row, int Col) Node, int Priority), ByPriorityOrder<(int Row, int Col), int>> frontier)
    {
        for (var direction = 0; direction < Directions.Length; direction++)
        {
            var (dRow, dCol) = Directions[direction];
            var (nextRow, nextCol) = (cell.Row + dRow, cell.Col + dCol);

            if (nextRow < 0 || nextRow >= n || nextCol < 0 || nextCol >= n)
            {
                continue;
            }

            var weight = _grid[cell.Row][cell.Col] == direction + 1 ? 0 : 1;
            var candidate = distances[cell] + weight;

            if (!distances.TryGetValue((nextRow, nextCol), out var known) || candidate < known)
            {
                distances[(nextRow, nextCol)] = candidate;
                frontier.Push(((nextRow, nextCol), candidate));
            }
        }
    }
}
