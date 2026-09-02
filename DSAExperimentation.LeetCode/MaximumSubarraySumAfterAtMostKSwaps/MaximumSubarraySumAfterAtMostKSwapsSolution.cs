using DSAExperimentation.Algorithms.Searching;
using DSAExperimentation.DataStructures.FenwickTree;
using DSAExperimentation.DataStructures.Sequence;

namespace DSAExperimentation.LeetCode.MaximumSubarraySumAfterAtMostKSwaps;

// LeetCode 3962. Maximum Subarray Sum After at Most K Swaps: pick any two
// indices and swap them, up to k times total, then report the best contiguous
// subarray sum achievable.
//
// A swap only ever matters through where the two values end up, and Kadane's
// maximum-subarray answer never depends on the ORDER of values inside the
// window that realizes it - only on the multiset of values that lands there.
// So for one fixed candidate window [i, i+w), the question is: with at most
// min(w, k, n-w) elements exchanged between "inside" and "outside", how much
// can the window's sum improve? Swapping the window's smallest element for the
// largest element outside is always at least as good as any other pairing (an
// exchange argument: any better outside value for a worse inside one strictly
// helps), so the best use of exactly s swaps replaces the s smallest inside
// values with the s largest outside ones:
//
//   h(s) = windowSum - sum(s smallest inside) + sum(s largest outside)
//
// and the answer for that window is max over s in [0, cap] of h(s). h is
// concave in s (removing the s-th smallest costs a non-decreasing amount as s
// grows, and adding the s-th largest gains a non-increasing amount), so its
// max is found by evaluating h at O(log cap) points via ternary search rather
// than every s. Both strategies below try every window; they differ only in
// how "sum of the s smallest/largest present values" is answered as the
// window slides.
internal static class MaximumSubarraySumAfterAtMostKSwapsSolution
{
    // Re-sorts the window and its complement from scratch for every one of the
    // O(n^2) windows, then walks every s in [0, cap] directly instead of
    // ternary-searching for the concave peak - the O(n^3 log n) arm the
    // Fenwick-backed order-statistics strategy below has to beat.
    public static long MaxSumByBruteForce(int[] nums, int k)
    {
        var n = nums.Length;
        var best = long.MinValue;

        for (var start = 0; start < n; start++)
        {
            for (var end = start; end < n; end++)
            {
                best = Math.Max(best, BestForBruteForceWindow(nums, start, end, k));
            }
        }

        return best;
    }

    private static long BestForBruteForceWindow(int[] nums, int start, int end, int k)
    {
        var windowLength = end - start + 1;
        var window = new long[windowLength];

        for (var i = 0; i < windowLength; i++)
        {
            window[i] = nums[start + i];
        }

        var outsideLength = nums.Length - windowLength;
        var outside = new long[outsideLength];
        var outsideIndex = 0;

        for (var i = 0; i < start; i++)
        {
            outside[outsideIndex++] = nums[i];
        }

        for (var i = end + 1; i < nums.Length; i++)
        {
            outside[outsideIndex++] = nums[i];
        }

        Array.Sort(window);
        Array.Sort(outside);

        var windowSum = 0L;

        foreach (var value in window)
        {
            windowSum += value;
        }

        var cap = Math.Min(windowLength, Math.Min(k, outsideLength));
        var best = windowSum;
        var removedSmallest = 0L;
        var addedLargest = 0L;

        for (var s = 1; s <= cap; s++)
        {
            removedSmallest += window[s - 1];
            addedLargest += outside[outsideLength - s];
            best = Math.Max(best, windowSum - removedSmallest + addedLargest);
        }

        return best;
    }

    // For each window length, keeps two present/absent Fenwick pairs - one
    // over ranks sorted by value ascending (for "inside, smallest first"), one
    // over ranks sorted by value descending (for "outside, largest first") -
    // updating them by one toggle per element as the window slides across
    // every start position at that length, and answering "sum of the s
    // smallest/largest present values" via BinarySearch.LowerBound over a view
    // of the count tree's PrefixQuery, the same order-statistics composition
    // FindingMKAverageBenchmarks' FenwickCountSequence already uses (there
    // indexed by value; here indexed by rank, since every element occupies its
    // own rank exactly once, so no same-value remainder bookkeeping is
    // needed).
    public static long MaxSumByOrderStatisticsFenwick(int[] nums, int k)
    {
        var n = nums.Length;
        var values = new long[n];

        for (var i = 0; i < n; i++)
        {
            values[i] = nums[i];
        }

        var rankByAscendingValue = RankPositions(n, ascending: true, nums);
        var rankByDescendingValue = RankPositions(n, ascending: false, nums);
        var prefix = new long[n + 1];

        for (var i = 0; i < n; i++)
        {
            prefix[i + 1] = prefix[i] + values[i];
        }

        var best = long.MinValue;

        for (var windowLength = 1; windowLength <= n; windowLength++)
        {
            var ledger = new WindowLedger(n);

            for (var index = 0; index < windowLength; index++)
            {
                ledger.MarkInside(index, values[index], rankByAscendingValue);
            }

            for (var index = windowLength; index < n; index++)
            {
                ledger.MarkOutside(index, values[index], rankByDescendingValue);
            }

            best = Math.Max(best, BestForWindow(ledger, prefix, 0, windowLength, k, n));

            for (var start = 0; start + windowLength < n; start++)
            {
                var leaving = start;
                var entering = start + windowLength;

                ledger.MoveInsideToOutside(leaving, values[leaving], rankByAscendingValue, rankByDescendingValue);
                ledger.MoveOutsideToInside(entering, values[entering], rankByAscendingValue, rankByDescendingValue);

                best = Math.Max(best, BestForWindow(ledger, prefix, start + 1, windowLength, k, n));
            }
        }

        return best;
    }

    private static long BestForWindow(WindowLedger ledger, long[] prefix, int start, int windowLength, int k, int n)
    {
        var windowSum = prefix[start + windowLength] - prefix[start];
        var cap = Math.Min(windowLength, Math.Min(k, n - windowLength));

        long H(int s) => s == 0
            ? windowSum
            : windowSum - ledger.SumOfSmallestInside(s) + ledger.SumOfLargestOutside(s);

        var lo = 0;
        var hi = cap;

        while (hi - lo > 2)
        {
            var mid1 = lo + ((hi - lo) / 3);
            var mid2 = hi - ((hi - lo) / 3);

            if (H(mid1) < H(mid2))
            {
                lo = mid1 + 1;
            }
            else
            {
                hi = mid2 - 1;
            }
        }

        var best = long.MinValue;

        for (var s = lo; s <= hi; s++)
        {
            best = Math.Max(best, H(s));
        }

        return best;
    }

    // rankByValue[position] = where that array position falls in the array
    // sorted by value, ascending or descending. Every rank is occupied by
    // exactly one position, so it is a bijection on [0, n) regardless of value
    // ties - the property that lets WindowLedger's Fenwick pairs stay simple
    // present/absent counts instead of needing per-value remainder math.
    private static int[] RankPositions(int n, bool ascending, int[] nums)
    {
        var order = Enumerable.Range(0, n).OrderBy(i => ascending ? nums[i] : -nums[i]).ToArray();
        var rank = new int[n];

        for (var r = 0; r < n; r++)
        {
            rank[order[r]] = r;
        }

        return rank;
    }

    private readonly struct FenwickPrefixCountSequence(FenwickTree<long, SumOperation<long>> counts)
        : IRandomAccessSequence<long>
    {
        public int Length => counts.Count;

        public long Get(int index) => counts.PrefixQuery(index);
    }

    // The four running Fenwick trees behind one window length's sweep: counts
    // and sums, each kept twice over - once ranked ascending (inside) and once
    // ranked descending (outside) - so "s smallest inside" and "s largest
    // outside" are both answered by the same SumOfSmallestByRank shape.
    private sealed class WindowLedger(int n)
    {
        private readonly FenwickTree<long, SumOperation<long>> _insideCount = new(n);
        private readonly FenwickTree<long, SumOperation<long>> _insideSum = new(n);
        private readonly FenwickTree<long, SumOperation<long>> _outsideCount = new(n);
        private readonly FenwickTree<long, SumOperation<long>> _outsideSum = new(n);

        public void MarkInside(int position, long value, int[] rankAscending)
        {
            _insideCount.Add(rankAscending[position], 1);
            _insideSum.Add(rankAscending[position], value);
        }

        public void MarkOutside(int position, long value, int[] rankDescending)
        {
            _outsideCount.Add(rankDescending[position], 1);
            _outsideSum.Add(rankDescending[position], value);
        }

        public void MoveInsideToOutside(int position, long value, int[] rankAscending, int[] rankDescending)
        {
            _insideCount.Add(rankAscending[position], -1);
            _insideSum.Add(rankAscending[position], -value);
            _outsideCount.Add(rankDescending[position], 1);
            _outsideSum.Add(rankDescending[position], value);
        }

        public void MoveOutsideToInside(int position, long value, int[] rankAscending, int[] rankDescending)
        {
            _outsideCount.Add(rankDescending[position], -1);
            _outsideSum.Add(rankDescending[position], -value);
            _insideCount.Add(rankAscending[position], 1);
            _insideSum.Add(rankAscending[position], value);
        }

        public long SumOfSmallestInside(int s) => SumOfSmallestByRank(_insideCount, _insideSum, s);

        public long SumOfLargestOutside(int s) => SumOfSmallestByRank(_outsideCount, _outsideSum, s);

        // Every rank holds count 0 or 1, so the rank where the cumulative
        // count first reaches s is exactly the s-th present rank - unlike a
        // value-indexed Fenwick (FindingMKAverageBenchmarks' SumOfSmallest),
        // no partial remainder at the boundary rank is possible.
        private static long SumOfSmallestByRank(
            FenwickTree<long, SumOperation<long>> count, FenwickTree<long, SumOperation<long>> sum, int s)
        {
            if (s == 0)
            {
                return 0;
            }

            var sequence = new FenwickPrefixCountSequence(count);
            var index = BinarySearch.LowerBound<long, FenwickPrefixCountSequence>(sequence, s);

            return sum.PrefixQuery(index);
        }
    }
}
