using DSAExperimentation.LeetCode.Solutions.MinimumIntervalToIncludeEachQuery;

namespace DSAExperimentation.LeetCode.Solutions.Tests.MinimumIntervalToIncludeEachQuery;

// Harness only. Both the per-query scan baseline and the offline heap sweep are
// MinimumIntervalToIncludeEachQuerySolution's - this file just pins them to the
// uncovered-query, equal-size, single-point and descending-query cases that exercise
// the sweep's discard loop and its write-back through the original query order.
// LeetCode's published examples run in CatalogCaseTests.
public sealed partial class MinimumIntervalToIncludeEachQuerySolutionTests
{
    public static TheoryData<int[][], int[], int[]> Cases =>
        new()
        {
            { [[1, 4], [2, 4], [3, 6], [4, 4]], [5, 4, 3, 2], [4, 1, 3, 3] },
            { [[1, 3], [5, 7]], [1, 4, 6], [3, -1, 3] },
            { [[1, 2]], [3], [-1] },
            { [[4, 4]], [4], [1] },
        };

    [Theory]
    [MemberData(nameof(Cases))]
    public void MinIntervalsByPerQueryScan_Cases_ReturnsSmallestCoveringSizePerQuery(
        int[][] intervals, int[] queries, int[] expected)
    {
        var actual = MinimumIntervalToIncludeEachQuerySolution.MinIntervalsByPerQueryScan(intervals, queries);

        Assert.Equal(expected, actual);
    }

    [Theory]
    [MemberData(nameof(Cases))]
    public void MinIntervalsByHeapSweep_Cases_ReturnsSmallestCoveringSizePerQuery(
        int[][] intervals, int[] queries, int[] expected)
    {
        var actual = MinimumIntervalToIncludeEachQuerySolution.MinIntervalsByHeapSweep(intervals, queries);

        Assert.Equal(expected, actual);
    }
}
