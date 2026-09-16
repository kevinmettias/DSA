using System.Numerics;

namespace DSAExperimentation.LeetCode.NumberOfIntegersWithPopcountDepthEqualToKII;

// LeetCode 3624. Number of Integers With Popcount-Depth Equal to K II: the
// same popcount-depth definition Part I counts over a static [1, n], but now
// nums is a mutable array and queries interleave point updates
// ([2, idx, val]) with range-depth counts ([1, l, r, k]) instead of asking
// one fixed range once. Both strategies answer the same question with the
// same signature (TwoSumSolution precedent).
internal static class NumberOfIntegersWithPopcountDepthEqualToKIISolution
{
    private const int UpdateQuery = 2;

    // Textbook baseline: mutate a plain copy of nums directly and rescan
    // [l, r], simulating each value's popcount chain on every range query -
    // correct, but O(range length) per query, unusable at the real problem's
    // n and query count each up to 1e5 - the arm the Fenwick-bucket strategy
    // below has to beat.
    public static int[] PopcountDepthByBruteForce(long[] nums, long[][] queries)
    {
        var current = (long[])nums.Clone();
        var results = new List<int>();

        foreach (var query in queries)
        {
            if (query[0] == UpdateQuery)
            {
                current[(int)query[1]] = query[2];
            }
            else
            {
                var (left, right, depth) = ((int)query[1], (int)query[2], (int)query[3]);
                var count = CountWithDepth(current, left, right, depth);
                results.Add(count);
            }
        }

        return [.. results];
    }

    // One range rescan: the values in [left, right] whose popcount depth is the queried
    // one. Named so the query loop above reads as dispatch and this reads as the scan the
    // Fenwick-bucket strategy exists to replace.
    private static int CountWithDepth(long[] values, int left, int right, int depth)
    {
        var count = 0;

        for (var j = left; j <= right; j++)
        {
            if (Depth(values[j]) == depth)
            {
                count++;
            }
        }

        return count;
    }

    private static int Depth(long value)
    {
        var depth = 0;

        while (value != 1)
        {
            value = BitOperations.PopCount((ulong)value);
            depth++;
        }

        return depth;
    }

    // This problem's own PopcountDepthFenwickIndex (see its own doc comment)
    // keeps one point-update/range-sum FenwickTree per depth value, so a
    // range-count query is O(log n) instead of rescanning the range, and an
    // update touches at most two trees instead of nothing being cached at
    // all - the same "index the query dimension the naive scan rescans every
    // time" move OpenTheLockSolution's Reduce.Graph arm makes for shortest
    // turns.
    public static int[] PopcountDepthByFenwickBuckets(long[] nums, long[][] queries)
    {
        var index = PopcountDepthFenwickIndex.Build(nums);

        return PopcountDepthByFenwickBuckets(index, queries);
    }

    public static int[] PopcountDepthByFenwickBuckets(PopcountDepthFenwickIndex index, long[][] queries)
    {
        var results = new List<int>();

        foreach (var query in queries)
        {
            if (query[0] == UpdateQuery)
            {
                index.Update((int)query[1], query[2]);
            }
            else
            {
                var (left, right, depth) = ((int)query[1], (int)query[2], (int)query[3]);
                var count = index.Count(left, right, depth);
                results.Add(count);
            }
        }

        return [.. results];
    }
}
