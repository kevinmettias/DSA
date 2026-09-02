using DSAExperimentation.DataStructures.IntervalSet;

namespace DSAExperimentation.LeetCode.InsertInterval;

// LeetCode 57. Insert Interval: given a set of non-overlapping intervals already
// sorted by start, insert a new interval and merge as needed, returning the
// resulting disjoint intervals sorted by start.
internal static class InsertIntervalSolution
{
    // The textbook answer: append newInterval, sort once, then a single linear
    // merge pass - the "batch" shape, deliberately BCL-only. It does not exploit
    // the fact that intervals arrives already sorted; IntervalSetAdd below does.
    public static List<(int Start, int End)> InsertByListSortAndMerge(
        IEnumerable<(int Start, int End)> intervals, (int Start, int End) newInterval)
    {
        var all = intervals.ToList();
        all.Add(newInterval);
        all.Sort((a, b) => a.Start.CompareTo(b.Start));

        var merged = new List<(int Start, int End)>();

        foreach (var interval in all)
        {
            if (merged.Count == 0 || interval.Start > merged[^1].End)
            {
                merged.Add(interval);
            }
            else
            {
                merged[^1] = (merged[^1].Start, Math.Max(merged[^1].End, interval.End));
            }
        }

        return merged;
    }

    // This repo's own IntervalSet<TKey> already maintains the sorted, merged,
    // disjoint-interval invariant after every Add - inserting the existing
    // intervals and then the new one reproduces LC 57's answer directly.
    public static List<(int Start, int End)> InsertByIntervalSet(
        IEnumerable<(int Start, int End)> intervals, (int Start, int End) newInterval)
    {
        var set = new IntervalSet<int>();

        foreach (var (start, end) in intervals)
        {
            set.Add(start, end);
        }

        set.Add(newInterval.Start, newInterval.End);

        return Enumerable.Range(0, set.Count).Select(set.Get).ToList();
    }
}
