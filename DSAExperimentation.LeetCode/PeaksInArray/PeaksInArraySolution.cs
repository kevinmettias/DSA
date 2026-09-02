using DSAExperimentation.DataStructures.FenwickTree;

namespace DSAExperimentation.LeetCode.PeaksInArray;

// LeetCode 3187. Peaks in Array: index i (0 < i < nums.Length - 1) is a peak
// when nums[i] is strictly greater than both neighbors. queries[q] is either
// [1, l, r] - count peaks strictly inside (l, r) - or [2, index, val] -
// nums[index] = val. Answer every type-1 query, in order.
//
// Both strategies answer the same question with the same signature, so the
// test harness can assert they agree and the benchmark harness can time them
// against each other without either restating the algorithm.
internal static class PeaksInArraySolution
{
    // The textbook answer: keep a mutable copy of nums and rescan the queried
    // range for peaks from scratch every time. O(n) per type-1 query,
    // deliberately BCL-only - the arm the Fenwick-tree strategy below has to
    // justify itself against.
    public static List<int> CountPeaksByBruteForce(int[] nums, int[][] queries)
    {
        var working = (int[])nums.Clone();
        var answer = new List<int>();

        foreach (var query in queries)
        {
            if (query[0] == 1)
            {
                answer.Add(CountPeaksInRange(working, query[1], query[2]));
            }
            else
            {
                working[query[1]] = query[2];
            }
        }

        return answer;
    }

    private static int CountPeaksInRange(int[] nums, int left, int right)
    {
        var count = 0;

        for (var i = left + 1; i < right; i++)
        {
            if (IsPeak(nums, i))
            {
                count++;
            }
        }

        return count;
    }

    // Composed: a point-update/range-query FenwickTree<int, SumOperation<int>>
    // (DataStructures/FenwickTree) stores a 1 at every peak index and 0
    // elsewhere, so a type-1 query is one Query(l+1, r-1) instead of a rescan. A
    // type-2 update can only change peak status at index-1, index and index+1,
    // so it removes the stale indicators there, writes the value, and re-adds
    // whatever indicators now hold - three or fewer O(log n) Fenwick edits
    // instead of an O(n) rescan.
    public static List<int> CountPeaksByFenwickTree(int[] nums, int[][] queries)
    {
        var working = (int[])nums.Clone();
        var peaks = new FenwickTree<int, SumOperation<int>>(working.Length);

        for (var i = 1; i < working.Length - 1; i++)
        {
            if (IsPeak(working, i))
            {
                peaks.Add(i, 1);
            }
        }

        var answer = new List<int>();

        foreach (var query in queries)
        {
            if (query[0] == 1)
            {
                answer.Add(CountPeaks(peaks, query[1], query[2]));
            }
            else
            {
                ApplyUpdate(working, peaks, query[1], query[2]);
            }
        }

        return answer;
    }

    private static int CountPeaks(FenwickTree<int, SumOperation<int>> peaks, int left, int right)
    {
        var lo = left + 1;
        var hi = right - 1;

        return lo > hi ? 0 : peaks.Query(lo, hi);
    }

    private static void ApplyUpdate(int[] working, FenwickTree<int, SumOperation<int>> peaks, int index, int value)
    {
        foreach (var position in AffectedPositions(index, working.Length))
        {
            peaks.Add(position, -IndicatorOf(working, position));
        }

        working[index] = value;

        foreach (var position in AffectedPositions(index, working.Length))
        {
            peaks.Add(position, IndicatorOf(working, position));
        }
    }

    private static IEnumerable<int> AffectedPositions(int index, int length)
    {
        for (var position = index - 1; position <= index + 1; position++)
        {
            if (position >= 1 && position <= length - 2)
            {
                yield return position;
            }
        }
    }

    private static int IndicatorOf(int[] nums, int index) => IsPeak(nums, index) ? 1 : 0;

    private static bool IsPeak(int[] nums, int index) =>
        index > 0 && index < nums.Length - 1 && nums[index] > nums[index - 1] && nums[index] > nums[index + 1];
}
