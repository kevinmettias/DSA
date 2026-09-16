namespace DSAExperimentation.LeetCode;

// LeetCode states an interval as a two-element int[] read as [start, end] - the same
// shape whether the problem calls them intervals, balloons or ranges - and every
// problem taking that shape unpacks the same two coordinates out of it. That unpack
// is fixed by the problem set rather than by any algorithm, which is why it is
// declared once here at the root of the LeetCode tier, next to LeetCodeAnswer and
// LeetCodeAdjacency, rather than restated per problem folder.
//
// What a problem then does with the pairs is its own: the end order its greedy reads
// is a step inside an algorithm, so it lives in the folder of the problem that
// states it (NonOverlappingIntervals' IntervalEndOrder, for LC 435 and LC 452).
internal static class LeetCodeIntervals
{
    // Each row of LC's interval shape as the (Start, End) pair the rest of the
    // problem's code reads. The rows are read, never written back, so nothing here
    // keeps a reference to the caller's own array.
    public static (int Start, int End)[] AsPairs(int[][] intervals) =>
        intervals.Select(interval => (Start: interval[0], End: interval[1])).ToArray();
}
