using DSAExperimentation.LeetCode.Solutions.NonOverlappingIntervals;

namespace DSAExperimentation.LeetCode.Solutions.Tests.NonOverlappingIntervals;

// Harness only. Both strategies are NonOverlappingIntervalsSolution's - this
// file just pins them to three identical intervals, all but one of which must be
// removed. LeetCode's published examples run in CatalogCaseTests.
public sealed partial class NonOverlappingIntervalsSolutionTests
{
    public static TheoryData<int[][], int> Cases =>
        new()
        {
            { [[1, 100], [1, 100], [1, 100]], 2 },
        };

    [Theory]
    [MemberData(nameof(Cases))]
    public void EraseOverlapIntervalsByBruteForce_Cases_ReturnsMinimumRemovals(
        int[][] intervals, int expected) =>
        Assert.Equal(expected, NonOverlappingIntervalsSolution.EraseOverlapIntervalsByBruteForce(intervals));

    [Theory]
    [MemberData(nameof(Cases))]
    public void EraseOverlapIntervalsBySortThenGreedy_Cases_ReturnsMinimumRemovals(
        int[][] intervals, int expected) =>
        Assert.Equal(expected, NonOverlappingIntervalsSolution.EraseOverlapIntervalsBySortThenGreedy(intervals));
}
