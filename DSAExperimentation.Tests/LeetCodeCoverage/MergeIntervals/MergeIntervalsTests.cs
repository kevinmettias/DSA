using DSAExperimentation.LeetCode.MergeIntervals;

namespace DSAExperimentation.Tests.LeetCodeCoverage.MergeIntervals;

// Harness only. Both strategies are MergeIntervalsSolution's; IntervalSet<TKey>'s
// own merge invariant is covered directly by
// DSAExperimentation.Tests/DataStructures/IntervalSet/IntervalSetTests.cs - this
// file just pins the two strategies to LeetCode's published examples.
public sealed class MergeIntervalsTests
{
    public static TheoryData<(int Start, int End)[], (int Start, int End)[]> Examples =>
        new()
        {
            { [(1, 3), (2, 6), (8, 10), (15, 18)], [(1, 6), (8, 10), (15, 18)] },
            { [(1, 4), (4, 5)], [(1, 5)] },
        };

    [Theory]
    [MemberData(nameof(Examples))]
    public void MergeByBatchSortAndMerge_LeetCodeExamples_ReturnsMergedDisjointIntervals(
        (int Start, int End)[] intervals, (int Start, int End)[] expected) =>
        Assert.Equal(expected, MergeIntervalsSolution.MergeByBatchSortAndMerge(intervals));

    [Theory]
    [MemberData(nameof(Examples))]
    public void MergeByIntervalSet_LeetCodeExamples_ReturnsMergedDisjointIntervals(
        (int Start, int End)[] intervals, (int Start, int End)[] expected) =>
        Assert.Equal(expected, MergeIntervalsSolution.MergeByIntervalSet(intervals));
}
