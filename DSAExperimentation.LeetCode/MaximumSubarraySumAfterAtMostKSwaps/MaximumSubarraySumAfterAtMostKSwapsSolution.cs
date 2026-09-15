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
                var windowBest = BestForBruteForceWindow(nums, start, end, k);
                best = Math.Max(best, windowBest);
            }
        }

        return best;
    }

    // One window's worth of the brute-force arm: the two pools sorted, then the
    // swap arithmetic.
    private static long BestForBruteForceWindow(int[] nums, int start, int end, int k)
    {
        var (window, outside) = WindowAndComplement(nums, start, end);

        Array.Sort(window);
        Array.Sort(outside);

        return BestAfterSwaps(window, outside, k);
    }

    // The window's own values and everything outside it, each in array order and
    // widened to the longs the swap arithmetic works in. They are one pair: what
    // one swap trades away comes from the window and what it gains comes from
    // the complement, so the complement's length is also what caps the swap count.
    private static (long[] Window, long[] Outside) WindowAndComplement(int[] nums, int start, int end)
    {
        var window = new long[end - start + 1];
        var outside = new long[nums.Length - window.Length];
        var outsideWrite = 0;

        for (var i = 0; i < window.Length; i++)
        {
            window[i] = nums[start + i];
        }

        for (var i = 0; i < start; i++)
        {
            outside[outsideWrite++] = nums[i];
        }

        for (var i = end + 1; i < nums.Length; i++)
        {
            outside[outsideWrite++] = nums[i];
        }

        return (window, outside);
    }

    // The best window sum over every allowed swap count: spending one more swap
    // removes the next-smallest value inside and adds the next-largest outside,
    // so one running pair of partial sums scores every count without re-sorting.
    private static long BestAfterSwaps(long[] window, long[] outside, int swapBudget)
    {
        var windowSum = Total(window);
        var outsideCap = Math.Min(swapBudget, outside.Length);
        var cap = Math.Min(window.Length, outsideCap);
        var best = windowSum;
        var removedSmallest = 0L;
        var addedLargest = 0L;

        for (var swaps = 1; swaps <= cap; swaps++)
        {
            removedSmallest += window[swaps - 1];
            addedLargest += outside[outside.Length - swaps];
            best = Math.Max(best, windowSum - removedSmallest + addedLargest);
        }

        return best;
    }

    private static long Total(long[] values)
    {
        var sum = 0L;

        foreach (var value in values)
        {
            sum += value;
        }

        return sum;
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
        var values = ValuesAsLongs(nums);
        var rankByAscendingValue = RankPositions(n, ValueOrder.Ascending, nums);
        var rankByDescendingValue = RankPositions(n, ValueOrder.Descending, nums);
        var prefix = PrefixSumsOf(values);
        var source = (n, values, prefix, rankByAscendingValue, rankByDescendingValue);

        return BestAcrossWindowLengths(k, source);
    }

    private static long[] ValuesAsLongs(int[] nums)
    {
        var values = new long[nums.Length];

        for (var i = 0; i < nums.Length; i++)
        {
            values[i] = nums[i];
        }

        return values;
    }

    // rankByValue[position] = where that array position falls in the array
    // sorted by value, ascending or descending. Every rank is occupied by
    // exactly one position, so it is a bijection on [0, n) regardless of value
    // ties - the property that lets WindowLedger's Fenwick pairs stay simple
    // present/absent counts instead of needing per-value remainder math.
    private static int[] RankPositions(int n, ValueOrder order, int[] nums)
    {
        var positionAtRank = Enumerable.Range(0, n).OrderBy(i => SortKey(nums, i, order)).ToArray();
        var rank = new int[n];

        for (var r = 0; r < n; r++)
        {
            rank[positionAtRank[r]] = r;
        }

        return rank;
    }

    // The key a position sorts on: its own value ascending, its negated value
    // descending, so one ordering pass reads the array in either direction.
    private static int SortKey(int[] nums, int position, ValueOrder order)
    {
        if (order == ValueOrder.Ascending)
        {
            return nums[position];
        }

        return -nums[position];
    }

    // prefix[i] is the total of the first i values, so any window sum is one
    // subtraction of two entries.
    private static long[] PrefixSumsOf(long[] values)
    {
        var prefix = new long[values.Length + 1];

        for (var i = 0; i < values.Length; i++)
        {
            prefix[i + 1] = prefix[i] + values[i];
        }

        return prefix;
    }

    // Every window length in turn, each on its own ledger seeded with the initial
    // window and then slid across every later start position.
    private static long BestAcrossWindowLengths(
        int swapBudget,
        (int N, long[] Values, long[] Prefix, int[] RankAscending, int[] RankDescending) source)
    {
        var ranks = (Ascending: source.RankAscending, Descending: source.RankDescending);
        var best = long.MinValue;

        for (var windowLength = 1; windowLength <= source.N; windowLength++)
        {
            var ledger = SeedLedger(source.N, source.Values, windowLength, ranks);
            var lengthBest = BestForLength(ledger, source, windowLength, swapBudget);
            best = Math.Max(best, lengthBest);
        }

        return best;
    }

    // A ledger holding the initial window [0, length) as the inside pool and
    // every later position as the outside one.
    private static WindowLedger SeedLedger(
        int elementCount, long[] values, int length, (int[] Ascending, int[] Descending) ranks)
    {
        var ledger = new WindowLedger(elementCount);

        for (var index = 0; index < length; index++)
        {
            ledger.MarkInside(index, values[index], ranks.Ascending);
        }

        for (var index = length; index < elementCount; index++)
        {
            ledger.MarkOutside(index, values[index], ranks.Descending);
        }

        return ledger;
    }

    // One window length's whole sweep: score the window at start 0, then slide it
    // one position at a time to every later start, keeping the best window seen.
    private static long BestForLength(
        WindowLedger ledger,
        (int N, long[] Values, long[] Prefix, int[] RankAscending, int[] RankDescending) source,
        int windowLength,
        int swapBudget)
    {
        var best = BestForWindow(ledger, source.Prefix, (Start: 0, Length: windowLength), swapBudget);

        for (var start = 0; start + windowLength < source.N; start++)
        {
            var leaving = start;
            var entering = start + windowLength;

            ledger.MoveInsideToOutside(leaving, source.Values[leaving], source.RankAscending, source.RankDescending);
            ledger.MoveOutsideToInside(entering, source.Values[entering], source.RankAscending, source.RankDescending);

            var windowBest = BestForWindow(
                ledger, source.Prefix, (Start: start + 1, Length: windowLength), swapBudget);
            best = Math.Max(best, windowBest);
        }

        return best;
    }

    // The window is one range - a start and a length chosen together and never
    // passed apart - and n is prefix.Length - 1, the element count the prefix sums
    // were built over, so the caller need not say it twice.
    private static long BestForWindow(
        WindowLedger ledger, long[] prefix, (int Start, int Length) window, int k)
    {
        var n = prefix.Length - 1;
        var windowSum = prefix[window.Start + window.Length] - prefix[window.Start];
        var outsideCap = Math.Min(k, n - window.Length);
        var cap = Math.Min(window.Length, outsideCap);
        var (lo, hi) = NarrowToPeak(cap, windowSum, ledger);

        return BestInBracket(lo, hi, windowSum, ledger);
    }

    // h(s), the window sum once s swaps are spent, is concave in s: each further
    // swap removes a value at least as large as the last and adds one at most as
    // large as the last. A ternary search therefore brackets its peak into
    // whichever two-thirds of [low, high] must contain it, three points wide.
    private static (int Low, int High) NarrowToPeak(int cap, long windowSum, WindowLedger ledger)
    {
        var lo = 0;
        var hi = cap;

        while (hi - lo > 2)
        {
            var mid1 = lo + ((hi - lo) / 3);
            var mid2 = hi - ((hi - lo) / 3);

            if (SumAfterSwaps(windowSum, ledger, mid1) < SumAfterSwaps(windowSum, ledger, mid2))
            {
                lo = mid1 + 1;
            }
            else
            {
                hi = mid2 - 1;
            }
        }

        return (lo, hi);
    }

    // The three candidates the search leaves are the peak's bracket, so each is
    // scored outright.
    private static long BestInBracket(int low, int high, long windowSum, WindowLedger ledger)
    {
        var best = long.MinValue;

        for (var swaps = low; swaps <= high; swaps++)
        {
            var candidate = SumAfterSwaps(windowSum, ledger, swaps);
            best = Math.Max(best, candidate);
        }

        return best;
    }

    // The window sum once s swaps have been spent: the s smallest values inside the
    // window are swapped for the s largest values outside it, so the sum gains the
    // difference between the two pools.
    private static long SumAfterSwaps(long windowSum, WindowLedger ledger, int swaps)
        => windowSum - ledger.SumOfSmallestInside(swaps) + ledger.SumOfLargestOutside(swaps);

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

    // Which way a ranking pass reads the window's values: Ascending puts the
    // smallest value at rank 0 - the inside pool's "smallest first" - and
    // Descending puts the largest there, for the outside pool's "largest first".
    private enum ValueOrder
    {
        Ascending,
        Descending,
    }
}
