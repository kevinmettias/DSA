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
    [Params(15, 40)]
    public int Size;

    private int[][] _grid = null!;

    [GlobalSetup]
    public void Setup()
    {
        var random = new Random(778);
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
            var mid = lo + ((hi - lo) / 2);

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

        var visited = new bool[n, n];
        var queue = new Queue<(int Row, int Col)>();
        queue.Enqueue((0, 0));
        visited[0, 0] = true;

        while (queue.Count > 0)
        {
            var (row, col) = queue.Dequeue();

            if (row == n - 1 && col == n - 1)
            {
                return true;
            }

            foreach (var (dr, dc) in Directions)
            {
                var nr = row + dr;
                var nc = col + dc;

                if (nr < 0 || nr >= n || nc < 0 || nc >= n || visited[nr, nc] || _grid[nr][nc] > time)
                {
                    continue;
                }

                visited[nr, nc] = true;
                queue.Enqueue((nr, nc));
            }
        }

        return false;
    }

    [Benchmark]
    public int HeapDijkstra()
    {
        var n = _grid.Length;
        var visited = new bool[n, n];
        var frontier = new Heap<((int Row, int Col) Node, int Priority), ByPriorityOrder<(int Row, int Col), int>>();

        frontier.Push(((0, 0), _grid[0][0]));
        visited[0, 0] = true;

        while (frontier.TryPop(out var entry))
        {
            var (row, col) = entry.Node;
            var time = entry.Priority;

            if (row == n - 1 && col == n - 1)
            {
                return time;
            }

            foreach (var (dr, dc) in Directions)
            {
                var nr = row + dr;
                var nc = col + dc;

                if (nr < 0 || nr >= n || nc < 0 || nc >= n || visited[nr, nc])
                {
                    continue;
                }

                visited[nr, nc] = true;
                frontier.Push(((nr, nc), Math.Max(time, _grid[nr][nc])));
            }
        }

        return -1;
    }

    private static readonly (int Row, int Col)[] Directions = [(-1, 0), (1, 0), (0, -1), (0, 1)];
}
