using BenchmarkDotNet.Attributes;
using DSAExperimentation.DataStructures.Graph.ShortestPaths;
using DSAExperimentation.DataStructures.Heap;

namespace DSAExperimentation.Benchmarks.ProblemSolutions;

// Swim in Rising Water (LC 778): BinarySearchFloodFill repeatedly re-scans the
// whole grid with a fresh BFS per candidate time (the textbook "binary search
// the answer" approach) vs. HeapDijkstra, the single Dijkstra-shaped pass that
// composes this repo's own Heap<Element,TOrder> ordered by
// ByPriorityOrder<TNode,TWeight> - the exact frontier shape
// TrappingRainWaterIIBenchmarks' HeapFloodFill already uses, here keyed by the
// minimax (bottleneck) elevation crossed so far instead of a flood-fill water
// level. Grid values are a random permutation of 0..n*n-1, matching the
// problem's own constraint that every elevation from 0 to n^2-1 appears
// exactly once.
[MemoryDiagnoser]
public class SwimInRisingWaterBenchmarks
{
    private const int RandomSeed = 778;
    private const int MidpointDivisor = 2;

    [Params(15, 40)]
    public int Size;

    private int[][] _grid = null!;

    [GlobalSetup]
    public void Setup()
    {
        var random = new Random(RandomSeed);
        var values = Enumerable.Range(0, Size * Size).OrderBy(_ => random.Next()).ToArray();
        _grid = new int[Size][];

        for (var r = 0; r < Size; r++)
        {
            _grid[r] = new int[Size];

            for (var c = 0; c < Size; c++)
            {
                _grid[r][c] = values[(r * Size) + c];
            }
        }
    }

    [Benchmark(Baseline = true)]
    public int BinarySearchFloodFill()
    {
        var n = _grid.Length;
        var lo = 0;
        var hi = (n * n) - 1;

        while (lo < hi)
        {
            var mid = lo + ((hi - lo) / MidpointDivisor);

            if (CanReachAtTime(mid, n))
            {
                hi = mid;
            }
            else
            {
                lo = mid + 1;
            }
        }

        return lo;
    }

    private bool CanReachAtTime(int time, int n)
    {
        if (_grid[0][0] > time)
        {
            return false;
        }

        var context = InitializeFloodFill(n, time);
        return RunFloodFill(context);
    }

    private static FloodFillContext InitializeFloodFill(int n, int time)
    {
        var visited = new bool[n, n];
        var queue = new Queue<(int Row, int Col)>();
        queue.Enqueue((0, 0));
        visited[0, 0] = true;

        return new FloodFillContext(n, time, visited, queue);
    }

    private bool RunFloodFill(FloodFillContext context)
    {
        var n = context.N;

        while (context.Queue.Count > 0)
        {
            var current = context.Queue.Dequeue();

            if (current.Row == n - 1 && current.Col == n - 1)
            {
                return true;
            }

            EnqueueReachableNeighbors(current, context);
        }

        return false;
    }

    private void EnqueueReachableNeighbors((int Row, int Col) current, FloodFillContext context)
    {
        foreach (var (dr, dc) in Directions)
        {
            var nr = current.Row + dr;
            var nc = current.Col + dc;

            if (nr < 0 || nr >= context.N || nc < 0 || nc >= context.N || context.Visited[nr, nc] || _grid[nr][nc] > context.Time)
            {
                continue;
            }

            context.Visited[nr, nc] = true;
            context.Queue.Enqueue((nr, nc));
        }
    }

    private readonly record struct FloodFillContext(int N, int Time, bool[,] Visited, Queue<(int Row, int Col)> Queue);

    [Benchmark]
    public int HeapDijkstra()
    {
        var n = _grid.Length;
        var visited = new bool[n, n];
        var frontier = new Heap<((int Row, int Col) Node, int Priority), ByPriorityOrder<(int Row, int Col), int>>();

        frontier.Push(((0, 0), _grid[0][0]));
        visited[0, 0] = true;

        return RunDijkstra(new DijkstraContext(n, visited, frontier));
    }

    private int RunDijkstra(DijkstraContext context)
    {
        var n = context.N;

        while (context.Frontier.TryPop(out var entry))
        {
            var (row, col) = entry.Node;
            var time = entry.Priority;

            if (row == n - 1 && col == n - 1)
            {
                return time;
            }

            PushReachableNeighbors(entry.Node, time, context);
        }

        return -1;
    }

    private void PushReachableNeighbors((int Row, int Col) current, int time, DijkstraContext context)
    {
        foreach (var (dr, dc) in Directions)
        {
            var nr = current.Row + dr;
            var nc = current.Col + dc;

            if (nr < 0 || nr >= context.N || nc < 0 || nc >= context.N || context.Visited[nr, nc])
            {
                continue;
            }

            context.Visited[nr, nc] = true;
            context.Frontier.Push(((nr, nc), Math.Max(time, _grid[nr][nc])));
        }
    }

    private readonly record struct DijkstraContext(
        int N, bool[,] Visited, Heap<((int Row, int Col) Node, int Priority), ByPriorityOrder<(int Row, int Col), int>> Frontier);

    private static readonly (int Row, int Col)[] Directions = [(-1, 0), (1, 0), (0, -1), (0, 1)];
}
