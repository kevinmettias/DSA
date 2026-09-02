using DSAExperimentation.DataStructures.Heap;

namespace DSAExperimentation.Tests.LeetCodeCoverage.MinimumIntervalToIncludeEachQuery;

// LeetCode 1851. Minimum Interval to Include Each Query: sweep queries in
// increasing order, pushing every interval that has opened (left <= query)
// into this repo's own Heap<T,TOrder> ordered by interval size, then lazily
// popping intervals whose right endpoint has already fallen behind the
// current query - the standard offline-sweep-plus-min-heap approach,
// O((n+q) log n) instead of an O(n*q) per-query scan.
public sealed partial class MinimumIntervalToIncludeEachQueryTests
{
    [Fact]
    public void MinInterval_LeetCodeExampleOne_ReturnsSmallestCoveringSizePerQuery()
    {
        int[][] intervals = [[1, 4], [2, 4], [3, 6], [4, 4]];
        int[] queries = [2, 3, 4, 5];

        var result = MinInterval(intervals, queries);

        Assert.Equal([3, 3, 1, 4], result);
    }

    [Fact]
    public void MinInterval_LeetCodeExampleTwo_ReturnsNegativeOneWhenUncovered()
    {
        int[][] intervals = [[2, 3], [2, 5], [1, 8], [20, 25]];
        int[] queries = [2, 19, 5, 22];

        var result = MinInterval(intervals, queries);

        Assert.Equal([2, -1, 4, 6], result);
    }

    private static int[] MinInterval(int[][] intervals, int[] queries)
    {
        var sortedIntervals = intervals.OrderBy(interval => interval[0]).ToArray();
        var queryOrder = Enumerable.Range(0, queries.Length).OrderBy(i => queries[i]).ToArray();

        var heap = new Heap<(int Size, int Right), BySizeOrder>();
        var result = new int[queries.Length];
        var nextInterval = 0;

        foreach (var queryIndex in queryOrder)
        {
            var query = queries[queryIndex];
            result[queryIndex] = ResolveSmallestIntervalSize(query, sortedIntervals, heap, ref nextInterval);
        }

        return result;
    }

    private static int ResolveSmallestIntervalSize(
        int query, int[][] sortedIntervals, Heap<(int Size, int Right), BySizeOrder> heap, ref int nextInterval)
    {
        while (nextInterval < sortedIntervals.Length && sortedIntervals[nextInterval][0] <= query)
        {
            var left = sortedIntervals[nextInterval][0];
            var right = sortedIntervals[nextInterval][1];
            heap.Push((right - left + 1, right));
            nextInterval++;
        }

        while (heap.TryPeek(out var smallest) && smallest.Right < query)
        {
            heap.TryPop(out _);
        }

        return heap.TryPeek(out var best) ? best.Size : -1;
    }

    private readonly struct BySizeOrder : IHeapOrder<(int Size, int Right)>
    {
        public static bool HasPriority((int Size, int Right) candidate, (int Size, int Right) incumbent)
            => candidate.Size < incumbent.Size;
    }
}
