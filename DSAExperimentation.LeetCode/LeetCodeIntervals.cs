using DSAExperimentation.Algorithms.Sorting;
using DSAExperimentation.DataStructures.Sequence;

namespace DSAExperimentation.LeetCode;

// LeetCode states an interval as a two-element int[] read as [start, end] - the same
// shape whether the problem calls them intervals, balloons or ranges - and every
// problem taking that shape unpacks the same two coordinates out of it. That unpack
// is fixed by the problem set rather than by any algorithm, which is why it is
// declared once here at the root of the LeetCode tier, next to LeetCodeAnswer and
// LeetCodeAdjacency, rather than restated per problem folder.
//
// The pairs' end order is stated here too. Taking the interval that finishes first is
// never worse than taking any other still-compatible one - it leaves at least as much
// room for everything after it - so every earliest-finish greedy over these pairs
// sweeps them by end, LC 435 and LC 452 among them. What each sweep then does is its
// own: LC 435 keeps every interval starting at or after the last kept end (>=), and
// LC 452 fires an arrow at the earliest end still uncovered (>), which is exactly the
// touching-endpoints difference between the two problems.
internal static class LeetCodeIntervals
{
    private static readonly Comparer<(int Start, int End)> ByEnd =
        Comparer<(int Start, int End)>.Create((first, second) => first.End.CompareTo(second.End));

    // Each row of LC's interval shape as the (Start, End) pair the rest of the
    // problem's code reads. The rows are read, never written back, so nothing here
    // keeps a reference to the caller's own array.
    public static (int Start, int End)[] AsPairs(int[][] intervals) =>
        intervals.Select(interval => (Start: interval[0], End: interval[1])).ToArray();

    // A sorted copy, so a caller that reuses one workload across iterations still
    // starts each one unsorted. MergeSort is stable, so intervals that end together
    // keep the order they were given in.
    public static (int Start, int End)[] SortedByEnd((int Start, int End)[] intervals)
    {
        var sorted = ((int Start, int End)[])intervals.Clone();

        MergeSort.Sort<(int Start, int End), ArrayIndexedSequence<(int Start, int End)>>(
            new ArrayIndexedSequence<(int Start, int End)>(sorted), ByEnd);

        return sorted;
    }
}
