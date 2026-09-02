using BenchmarkDotNet.Attributes;
using DSAExperimentation.DataStructures.Heap;

namespace DSAExperimentation.Benchmarks.ProblemSolutions;

// Minimum Interval to Include Each Query (LC 1851): PerQueryScan checks
// every interval against every query directly, O(n*q). HeapSweep processes
// queries in increasing order, pushing intervals as they open into this
// repo's own Heap<T,TOrder> keyed by interval size and lazily popping ones
// whose right endpoint has fallen behind the current query, O((n+q) log n).
[MemoryDiagnoser]
public class MinimumIntervalToIncludeEachQueryBenchmarks
{
    // LC problem number, used as the deterministic random seed.
    private const int RandomSeed = 1851;

    // Both interval endpoints and query values are drawn from the same [0, Count * this) space.
    private const int CoordinateSpaceMultiplier = 2;

    // Interval lengths are drawn from [0, Count / this) so intervals stay shorter than the full space.
    private const int MaxIntervalLengthDivisor = 2;

    [Params(200, 3_000)]
    public int Count;

    private int[][] _intervals = null!;
    private int[] _queries = null!;

    [GlobalSetup]
    public void Setup()
    {
        var random = new Random(RandomSeed);
        _intervals = Enumerable.Range(0, Count)
            .Select(_ =>
            {
                var left = random.Next(0, Count * CoordinateSpaceMultiplier);
                var right = left + random.Next(0, Count / MaxIntervalLengthDivisor);
                return new[] { left, right };
            })
            .ToArray();
        _queries = Enumerable.Range(0, Count).Select(_ => random.Next(0, Count * CoordinateSpaceMultiplier)).ToArray();
    }

    [Benchmark(Baseline = true)]
    public int PerQueryScan()
    {
        var total = 0;

        foreach (var query in _queries)
        {
            total += BestIntervalSizeForQuery(query);
        }

        return total;
    }

    private int BestIntervalSizeForQuery(int query)
    {
        var best = -1;

        foreach (var interval in _intervals)
        {
            if (interval[0] > query || interval[1] < query)
            {
                continue;
            }

            var size = interval[1] - interval[0] + 1;
            if (best == -1 || size < best)
            {
                best = size;
            }
        }

        return best;
    }

    [Benchmark]
    public int HeapSweep()
    {
        var sortedIntervals = _intervals.OrderBy(interval => interval[0]).ToArray();
        var queryOrder = Enumerable.Range(0, _queries.Length).OrderBy(i => _queries[i]).ToArray();

        var heap = new Heap<(int Size, int Right), BySizeOrder>();
        var result = new int[_queries.Length];
        var nextInterval = 0;

        foreach (var queryIndex in queryOrder)
        {
            var query = _queries[queryIndex];
            result[queryIndex] = SmallestCoveringSize(sortedIntervals, heap, query, ref nextInterval);
        }

        var total = 0;
        foreach (var value in result)
        {
            total += value;
        }

        return total;
    }

    private static int SmallestCoveringSize(
        int[][] sortedIntervals, Heap<(int Size, int Right), BySizeOrder> heap, int query, ref int nextInterval)
    {
        AdmitOpenIntervals(sortedIntervals, heap, query, ref nextInterval);

        while (heap.TryPeek(out var smallest) && smallest.Right < query)
        {
            heap.TryPop(out _);
        }

        return heap.TryPeek(out var best) ? best.Size : -1;
    }

    private static void AdmitOpenIntervals(
        int[][] sortedIntervals, Heap<(int Size, int Right), BySizeOrder> heap, int query, ref int nextInterval)
    {
        while (nextInterval < sortedIntervals.Length && sortedIntervals[nextInterval][0] <= query)
        {
            var left = sortedIntervals[nextInterval][0];
            var right = sortedIntervals[nextInterval][1];
            heap.Push((right - left + 1, right));
            nextInterval++;
        }
    }

    private readonly struct BySizeOrder : IHeapOrder<(int Size, int Right)>
    {
        public static bool HasPriority((int Size, int Right) candidate, (int Size, int Right) incumbent)
            => candidate.Size < incumbent.Size;
    }
}
