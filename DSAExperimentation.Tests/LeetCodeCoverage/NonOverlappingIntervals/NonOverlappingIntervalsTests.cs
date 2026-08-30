using DSAExperimentation.Algorithms.Sorting;
using DSAExperimentation.DataStructures.Sequence;

namespace DSAExperimentation.Tests.LeetCodeCoverage.NonOverlappingIntervals;

// LeetCode 435. Non-overlapping Intervals: sort intervals by end coordinate with
// this repo's own MergeSort.Sort<Element,TSequence> over an ArrayIndexedSequence -
// the same custom-comparer shape MinimumNumberOfArrowsToBurstBalloonsTests
// exercises - then a single greedy pass counting how many intervals fit without
// overlapping; the removal count is simply the leftover. Unlike LC 452's
// touching-counts-as-overlap semantics, LC 435 treats touching endpoints as
// non-overlapping (Start >= previous End keeps both), so the greedy comparison
// flips from '>' to '>=' - this is NOT IntervalSet.Count, whose closed-interval
// merge semantics would wrongly treat [1,2],[2,3] as one overlapping group here.
public sealed partial class NonOverlappingIntervalsTests
{
    [Fact]
    public void EraseOverlapIntervals_ClassicExample_ReturnsOne()
    {
        int[][] intervals = [[1, 2], [2, 3], [3, 4], [1, 3]];

        Assert.Equal(1, EraseOverlapIntervals(intervals));
    }

    [Fact]
    public void EraseOverlapIntervals_AllOverlapping_ReturnsCountMinusOne()
    {
        int[][] intervals = [[1, 100], [1, 100], [1, 100]];

        Assert.Equal(2, EraseOverlapIntervals(intervals));
    }

    private static int EraseOverlapIntervals(int[][] intervals)
    {
        var items = intervals.Select(p => (Start: p[0], End: p[1])).ToArray();

        MergeSort.Sort<(int Start, int End), ArrayIndexedSequence<(int Start, int End)>>(
            new ArrayIndexedSequence<(int Start, int End)>(items),
            Comparer<(int Start, int End)>.Create((a, b) => a.End.CompareTo(b.End)));

        var kept = 1;
        var lastEnd = items[0].End;

        for (var i = 1; i < items.Length; i++)
        {
            if (items[i].Start >= lastEnd)
            {
                kept++;
                lastEnd = items[i].End;
            }
        }

        return items.Length - kept;
    }
}
