using DSAExperimentation.Algorithms.Sorting;
using DSAExperimentation.DataStructures.Sequence;

namespace DSAExperimentation.LeetCode.NonOverlappingIntervals;

// Intervals by end coordinate, ascending. Taking the interval that finishes first is
// never worse than taking any other still-compatible one - it leaves at least as much
// room for everything that follows - so one pass over this order settles both
// problems that read it: LC 435 keeps every interval starting at or after the last
// kept end, and LC 452 fires one arrow at the earliest end still uncovered.
//
// The order is stated once, here in LC 435's own folder, because keeping the largest
// set of pairwise non-overlapping intervals is the problem this order is the whole
// answer to, and LC 452 calls in for it rather than sorting by end a second way - the
// arrangement SqrtX's SquareExceedsSequence has with FourDivisors, ClosestDivisors
// and ThreeDivisors. The sweeps themselves are not shared: LC 435 stops at ">=" and
// LC 452 at ">", which is exactly the touching-endpoints difference between the two
// problems.
internal static class IntervalEndOrder
{
    // A sorted copy, so a caller that reuses one workload across iterations still
    // starts each one unsorted.
    public static (int Start, int End)[] SortedByEnd((int Start, int End)[] intervals)
    {
        var sorted = ((int Start, int End)[])intervals.Clone();

        MergeSort.Sort<(int Start, int End), ArrayIndexedSequence<(int Start, int End)>>(
            new ArrayIndexedSequence<(int Start, int End)>(sorted),
            Comparer<(int Start, int End)>.Create((a, b) => a.End.CompareTo(b.End)));

        return sorted;
    }
}
