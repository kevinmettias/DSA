using BenchmarkDotNet.Attributes;
using DSAExperimentation.DataStructures.Graph.ShortestPaths;
using DSAExperimentation.DataStructures.Heap;

namespace DSAExperimentation.Benchmarks.ProblemSolutions;

// Path With Minimum Effort (LC 1631): BinarySearchFloodFill repeatedly
// re-scans the whole grid with a fresh BFS per candidate effort - the
// textbook "binary search the answer" approach, the same baseline shape
// SwimInRisingWaterBenchmarks already uses for its own grid problem - vs.
// HeapDijkstra, the single minimax-Dijkstra pass that composes this repo's
// own Heap<Element,TOrder> ordered by ByPriorityOrder<TNode,TWeight>, keyed
// here by the bottleneck absolute height difference crossed so far instead
// of a flood-fill water level, and settled at pop time (not push time)
// since, unlike Swim in Rising Water's per-cell elevation, this edge weight
// depends on both endpoints. Heights are random in [0, 10^6), matching
// LeetCode's own constraint range.
[MemoryDiagnoser]
public class PathWithMinimumEffortBenchmarks
{
    private static readonly (int Row, int Col)[] Directions = [(-1, 0), (1, 0), (0, -1), (0, 1)];

    private const int RandomSeed = 1631; // LeetCode problem number

    private const int HeightUpperBoundExclusive = 1_000_000;

    private const int BinarySearchMidDivisor = 2;

    [Params(15, 40)]
    public int Size;

    private int[][] _heights = null!;

    [GlobalSetup]
    public void Setup()
    {
        var random = new Random(RandomSeed);
        _heights = new int[Size][];

        for (var r = 0; r < Size; r++)
        {
            _heights[r] = new int[Size];

            for (var c = 0; c < Size; c++)
            {
                _heights[r][c] = random.Next(0, HeightUpperBoundExclusive);
            }
        }
    }

    [Benchmark(Baseline = true)]
    public int BinarySearchFloodFill()
    {
        var lo = 0;
        var hi = HeightUpperBoundExclusive;

        while (lo < hi)
        {
            var mid = lo + ((hi - lo) / BinarySearchMidDivisor);

            if (CanReachWithEffort(mid))
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

    private readonly record struct ReachabilitySearch(int N, int Effort, bool[,] Visited, Queue<(int Row, int Col)> Queue);

    private bool CanReachWithEffort(int effort)
    {
        var search = CreateSearch(effort);
        return RunSearch(search);
    }

    private ReachabilitySearch CreateSearch(int effort)
    {
        var n = _heights.Length;
        var visited = new bool[n, n];
        var queue = new Queue<(int Row, int Col)>();
        queue.Enqueue((0, 0));
        visited[0, 0] = true;

        return new ReachabilitySearch(n, effort, visited, queue);
    }

    private bool RunSearch(ReachabilitySearch search)
    {
        while (search.Queue.Count > 0)
        {
            var current = search.Queue.Dequeue();

            if (current.Row == search.N - 1 && current.Col == search.N - 1)
            {
                return true;
            }

            foreach (var direction in Directions)
            {
                TryEnqueueNeighbor(current, direction, search);
            }
        }

        return false;
    }

    private void TryEnqueueNeighbor((int Row, int Col) current, (int Row, int Col) direction, ReachabilitySearch search)
    {
        var nr = current.Row + direction.Row;
        var nc = current.Col + direction.Col;

        if (nr < 0 || nr >= search.N || nc < 0 || nc >= search.N || search.Visited[nr, nc])
        {
            return;
        }

        if (Math.Abs(_heights[nr][nc] - _heights[current.Row][current.Col]) > search.Effort)
        {
            return;
        }

        search.Visited[nr, nc] = true;
        search.Queue.Enqueue((nr, nc));
    }

    private readonly record struct DijkstraState(
        int N, bool[,] Settled, int[,] BestEffort, Heap<((int Row, int Col) Node, int Priority), ByPriorityOrder<(int Row, int Col), int>> Frontier);

    [Benchmark]
    public int HeapDijkstra()
    {
        var n = _heights.Length;
        var settled = new bool[n, n];
        var bestEffort = InitializeBestEffort(n);
        var frontier = new Heap<((int Row, int Col) Node, int Priority), ByPriorityOrder<(int Row, int Col), int>>();
        frontier.Push(((0, 0), 0));
        var state = new DijkstraState(n, settled, bestEffort, frontier);

        while (frontier.TryPop(out var entry))
        {
            var result = ProcessDijkstraEntry(entry, state);

            if (result is { } effort)
            {
                return effort;
            }
        }

        return -1;
    }

    private static int[,] InitializeBestEffort(int n)
    {
        var bestEffort = new int[n, n];

        for (var r = 0; r < n; r++)
        {
            for (var c = 0; c < n; c++)
            {
                bestEffort[r, c] = int.MaxValue;
            }
        }

        bestEffort[0, 0] = 0;

        return bestEffort;
    }

    private int? ProcessDijkstraEntry(((int Row, int Col) Node, int Priority) entry, DijkstraState state)
    {
        var (row, col) = entry.Node;

        if (state.Settled[row, col])
        {
            return null;
        }

        state.Settled[row, col] = true;
        var effort = entry.Priority;

        if (row == state.N - 1 && col == state.N - 1)
        {
            return effort;
        }

        foreach (var direction in Directions)
        {
            TryRelaxNeighbor((row, col), direction, effort, state);
        }

        return null;
    }

    private void TryRelaxNeighbor((int Row, int Col) current, (int Row, int Col) direction, int effort, DijkstraState state)
    {
        var nr = current.Row + direction.Row;
        var nc = current.Col + direction.Col;

        if (nr < 0 || nr >= state.N || nc < 0 || nc >= state.N || state.Settled[nr, nc])
        {
            return;
        }

        var candidate = Math.Max(effort, Math.Abs(_heights[nr][nc] - _heights[current.Row][current.Col]));

        if (candidate < state.BestEffort[nr, nc])
        {
            state.BestEffort[nr, nc] = candidate;
            state.Frontier.Push(((nr, nc), candidate));
        }
    }
}
