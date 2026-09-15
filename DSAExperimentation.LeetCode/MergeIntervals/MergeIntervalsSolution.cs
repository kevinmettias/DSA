using DSAExperimentation.DataStructures.IntervalSet;

namespace DSAExperimentation.LeetCode.MergeIntervals;

// LeetCode 56. Merge Intervals: given a collection of intervals, merge every pair
// that overlaps and return the resulting disjoint intervals sorted by start.
//
// Both strategies answer the same question - build the actual merged interval list,
// not just count it - so IntervalSetTests already covers IntervalSet<TKey>'s own
// merge invariant; this class only supplies the two ways of reaching LC 56's answer.
internal static class MergeIntervalsSolution
{
    // The textbook answer: sort once, O(n log n), then a single linear merge pass.
    // Deliberately BCL-only - the arm the composed IntervalSet strategy below has to
    // justify itself against.
    public static List<(int Start, int End)> MergeByBatchSortAndMerge(IEnumerable<(int Start, int End)> intervals)
    {
        var sorted = intervals.OrderBy(interval => interval.Start).ToList();

        if (sorted.Count == 0)
        {
            return sorted;
        }

        return MergeSorted(sorted);
    }

    // The linear merge pass over an already start-sorted list: each interval either
    // extends the interval last emitted or opens a fresh one after it.
    private static List<(int Start, int End)> MergeSorted(List<(int Start, int End)> sorted)
    {
        var merged = new List<(int Start, int End)> { sorted[0] };

        for (var i = 1; i < sorted.Count; i++)
        {
            var last = merged[^1];
            var (start, end) = sorted[i];

            if (start <= last.End)
            {
                merged[^1] = (last.Start, Math.Max(last.End, end));
            }
            else
            {
                merged.Add((start, end));
            }
        }

        return merged;
    }

    // This repo's own IntervalSet<TKey> already maintains a merged, sorted set of
    // disjoint intervals as each one is added (see IntervalSet.cs's own doc comment)
    // - inserting one at a time reproduces LC 56's answer as a side effect of
    // IntervalSet's own invariant, at the cost of O(n) work per insertion.
    public static List<(int Start, int End)> MergeByIntervalSet(IEnumerable<(int Start, int End)> intervals)
    {
        var set = new IntervalSet<int>();

        foreach (var (start, end) in intervals)
        {
            set.Add(start, end);
        }

        return Enumerable.Range(0, set.Count).Select(set.Get).ToList();
    }
}
