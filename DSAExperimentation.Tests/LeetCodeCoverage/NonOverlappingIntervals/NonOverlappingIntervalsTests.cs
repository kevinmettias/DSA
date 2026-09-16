using DSAExperimentation.LeetCode.NonOverlappingIntervals;

namespace DSAExperimentation.Tests.LeetCodeCoverage.NonOverlappingIntervals;

// Harness only. Both strategies are NonOverlappingIntervalsSolution's - this
// file just pins them to LeetCode's published examples.
public sealed partial class NonOverlappingIntervalsTests
{
    public static TheoryData<int[][], int> Examples =>
        new()
        {
            { [[1, 2], [2, 3], [3, 4], [1, 3]], 1 },
            { [[1, 100], [1, 100], [1, 100]], 2 },
        };

    [Theory]
    [MemberData(nameof(Examples))]
    public void EraseOverlapIntervalsByBruteForce_LeetCodeExamples_ReturnsMinimumRemovals(
        int[][] intervals, int expected) =>
        Assert.Equal(expected, NonOverlappingIntervalsSolution.EraseOverlapIntervalsByBruteForce(intervals));

    [Theory]
    [MemberData(nameof(Examples))]
    public void EraseOverlapIntervalsBySortThenGreedy_LeetCodeExamples_ReturnsMinimumRemovals(
        int[][] intervals, int expected) =>
        Assert.Equal(expected, NonOverlappingIntervalsSolution.EraseOverlapIntervalsBySortThenGreedy(intervals));
}
