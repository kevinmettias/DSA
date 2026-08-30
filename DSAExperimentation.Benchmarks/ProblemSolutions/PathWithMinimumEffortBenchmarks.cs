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

    [Params(15, 40)]
    public int Size;

    private int[][] _heights = null!;

    [GlobalSetup]
    public void Setup()
    {
        var random = new Random(1631);
        _heights = new int[Size][];

        for (var r = 0; r < Size; r++)
        {
            _heights[r] = new int[Size];

            for (var c = 0; c < Size; c++)
            {
                _heights[r][c] = random.Next(0, 1_000_000);
            }
        }
    }

    [Benchmark(Baseline = true)]
    public int BinarySearchFloodFill()
    {
        var lo = 0;
        var hi = 1_000_000;

        while (lo < hi)
        {
            var mid = lo + ((hi - lo) / 2);

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

    private bool CanReachWithEffort(int effort)
    {
        var n = _heights.Length;
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

                if (nr < 0 || nr >= n || nc < 0 || nc >= n || visited[nr, nc])
                {
                    continue;
                }

                if (Math.Abs(_heights[nr][nc] - _heights[row][col]) > effort)
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
        var n = _heights.Length;
        var settled = new bool[n, n];
        var bestEffort = new int[n, n];

        for (var r = 0; r < n; r++)
        {
            for (var c = 0; c < n; c++)
            {
                bestEffort[r, c] = int.MaxValue;
            }
        }

        bestEffort[0, 0] = 0;
        var frontier = new Heap<((int Row, int Col) Node, int Priority), ByPriorityOrder<(int Row, int Col), int>>();
        frontier.Push(((0, 0), 0));

        while (frontier.TryPop(out var entry))
        {
            var (row, col) = entry.Node;

            if (settled[row, col])
            {
                continue;
            }

            settled[row, col] = true;
            var effort = entry.Priority;

            if (row == n - 1 && col == n - 1)
            {
                return effort;
            }

            foreach (var (dr, dc) in Directions)
            {
                var nr = row + dr;
                var nc = col + dc;

                if (nr < 0 || nr >= n || nc < 0 || nc >= n || settled[nr, nc])
                {
                    continue;
                }

                var candidate = Math.Max(effort, Math.Abs(_heights[nr][nc] - _heights[row][col]));

                if (candidate < bestEffort[nr, nc])
                {
                    bestEffort[nr, nc] = candidate;
                    frontier.Push(((nr, nc), candidate));
                }
            }
        }

        return -1;
    }
}
