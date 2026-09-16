using DSAExperimentation.LeetCode.CountIntegersInIntervals;

namespace DSAExperimentation.Tests.LeetCodeCoverage.CountIntegersInIntervals;

// Harness only. Both strategies are CountIntegersInIntervalsSolution's; this file just
// replays LeetCode's published add() call script against each and asserts count() after
// EVERY add rather than only at the script's own count() positions, so a merge that goes
// wrong is caught at the call that caused it - the same running-snapshot shape
// DataStreamAsDisjointIntervalsTests uses for LC 352. The pre-migration test only
// exercised the IntervalSet composition; CreateByHashSetPerInteger's baseline (previously
// untested scaffolding inlined in the benchmark) gets that same coverage here for the
// first time.
public sealed partial class CountIntegersInIntervalsTests
{
    public static TheoryData<(int Left, int Right)[], int[]> Examples =>
        new()
        {
            // LeetCode example 1: add(2,3), add(7,10), add(5,8) - the last add bridges
            // [7,10] and reports 8, the union {2,3} u {5..10}.
            { [(2, 3), (7, 10), (5, 8)], [2, 6, 8] },

            // Two disjoint ranges, then a range that bridges both into a single union.
            { [(1, 3), (5, 7), (3, 5)], [3, 6, 7] },

            // A single integer, added as a degenerate range.
            { [(1, 1)], [1] },

            // Re-adding a range already fully contained changes nothing.
            { [(1, 10), (3, 4)], [10, 10] },

            // The same degenerate range twice: still one integer.
            { [(5, 5), (5, 5)], [1, 1] },

            // Ranges that abut without sharing an integer stay two intervals, and the
            // total is still the plain count of distinct integers.
            { [(1, 2), (4, 5)], [2, 4] },

            // A wide range added last swallows several earlier disjoint ones at once,
            // which is where a running-total field (rather than re-summing the intervals
            // actually held) would over-count.
            { [(1, 2), (5, 6), (9, 10), (0, 20)], [2, 4, 6, 21] },

            // Negative and zero-crossing bounds.
            { [(-5, -3), (-4, 0)], [3, 6] },
        };

    [Theory]
    [MemberData(nameof(Examples))]
    public void CreateByIntervalSetMerge_LeetCodeExamples_CountTracksUnionSizeAsRangesMerge(
        (int Left, int Right)[] ranges, int[] expectedAfterEachAdd) =>
        AssertSequence(
            CountIntegersInIntervalsSolution.CreateByIntervalSetMerge(), ranges, expectedAfterEachAdd);

    [Theory]
    [MemberData(nameof(Examples))]
    public void CreateByHashSetPerInteger_LeetCodeExamples_CountTracksUnionSizeAsRangesMerge(
        (int Left, int Right)[] ranges, int[] expectedAfterEachAdd) =>
        AssertSequence(
            CountIntegersInIntervalsSolution.CreateByHashSetPerInteger(), ranges, expectedAfterEachAdd);

    private static void AssertSequence(
        CountIntegersInIntervalsSolution.ICountIntervals countIntervals,
        (int Left, int Right)[] ranges,
        int[] expectedAfterEachAdd)
    {
        for (var i = 0; i < ranges.Length; i++)
        {
            countIntervals.Add(ranges[i].Left, ranges[i].Right);
            Assert.Equal(expectedAfterEachAdd[i], countIntervals.Count());
        }
    }
}
