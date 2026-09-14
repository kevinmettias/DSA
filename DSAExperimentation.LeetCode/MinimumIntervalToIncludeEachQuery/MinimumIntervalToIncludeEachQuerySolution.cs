using DSAExperimentation.DataStructures.Heap;

namespace DSAExperimentation.LeetCode.MinimumIntervalToIncludeEachQuery;

// LeetCode 1851. Minimum Interval to Include Each Query: for every query, the size
// of the smallest interval [left, right] with left <= query <= right, or -1 when no
// interval covers it. An interval's size is right - left + 1.
//
// Both strategies answer the same question - the per-query minimum covering size, in
// the queries' original order. They differ only in whether each query re-examines
// every interval or inherits the work the previous (smaller) query already did.
internal static class MinimumIntervalToIncludeEachQuerySolution
{
    // The two endpoints' slots in LeetCode's own two-element interval array.
    private const int LeftEndpoint = 0;
    private const int RightEndpoint = 1;

    // The textbook answer: for each query, walk the whole interval list and keep the
    // smallest one that covers it. Nothing is remembered between queries, so an
    // interval is re-tested once per query - O(n*q). Deliberately plain arrays and
    // loops; it is the arm the sweep below has to justify itself against.
    public static int[] MinIntervalsByPerQueryScan(int[][] intervals, int[] queries)
    {
        var answer = new int[queries.Length];

        for (var i = 0; i < queries.Length; i++)
        {
            answer[i] = SmallestCoveringSize(intervals, queries[i]);
        }

        return answer;
    }

    private static int SmallestCoveringSize(int[][] intervals, int query)
    {
        var best = LeetCodeAnswer.None;

        foreach (var interval in intervals)
        {
            if (interval[LeftEndpoint] > query || interval[RightEndpoint] < query)
            {
                continue;
            }

            var size = interval[RightEndpoint] - interval[LeftEndpoint] + 1;

            if (best == LeetCodeAnswer.None || size < best)
            {
                best = size;
            }
        }

        return best;
    }

    // The offline sweep: answer the queries in increasing order so an interval is
    // admitted once, when it opens, and discarded once, when its right endpoint falls
    // behind the sweep. This repo's own Heap<Element,TOrder> in MinHeapOrder over
    // (Size, Right) keeps the smallest live interval at the root, so each query is a
    // peek - O((n + q) log n) overall. Results are written back through the original
    // query index, so the caller still gets them in LeetCode's order.
    public static int[] MinIntervalsByHeapSweep(int[][] intervals, int[] queries)
    {
        var sortedIntervals = intervals.OrderBy(interval => interval[LeftEndpoint]).ToArray();
        var queryOrder = Enumerable.Range(0, queries.Length).OrderBy(index => queries[index]).ToArray();
        var sweep = new IntervalSweep(sortedIntervals);
        var answer = new int[queries.Length];

        foreach (var queryIndex in queryOrder)
        {
            answer[queryIndex] = sweep.SmallestCoveringSize(queries[queryIndex]);
        }

        return answer;
    }

    // The sweep's position over the left-sorted intervals, plus the min-heap of the
    // ones currently open. Kept as one object rather than a heap and a cursor threaded
    // through ref parameters: the cursor only ever advances, so it is state belonging
    // to the sweep and not to any single query.
    //
    // Ordering by the whole (Size, Right) tuple rather than by Size alone only breaks
    // ties among equal-sized intervals; the answer is the root's Size either way.
    private sealed class IntervalSweep(int[][] sortedIntervals)
    {
        private readonly Heap<(int Size, int Right), MinHeapOrder<(int, int)>> _open = new();
        private int _nextInterval;

        // Queries arrive in increasing order, so an interval discarded here is dead for
        // every later query too - the discard loop is amortized, not per-query work.
        public int SmallestCoveringSize(int query)
        {
            AdmitOpenedIntervals(query);
            DiscardExpiredIntervals(query);

            return _open.TryPeek(out var best) ? best.Size : LeetCodeAnswer.None;
        }

        private void AdmitOpenedIntervals(int query)
        {
            while (_nextInterval < sortedIntervals.Length
                && sortedIntervals[_nextInterval][LeftEndpoint] <= query)
            {
                var left = sortedIntervals[_nextInterval][LeftEndpoint];
                var right = sortedIntervals[_nextInterval][RightEndpoint];
                _open.Push((right - left + 1, right));
                _nextInterval++;
            }
        }

        private void DiscardExpiredIntervals(int query)
        {
            while (_open.TryPeek(out var smallest) && smallest.Right < query)
            {
                _open.TryPop(out _);
            }
        }
    }
}
