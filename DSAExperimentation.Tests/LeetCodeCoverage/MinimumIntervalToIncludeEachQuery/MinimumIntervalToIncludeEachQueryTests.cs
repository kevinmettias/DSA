using DSAExperimentation.LeetCode.MinimumIntervalToIncludeEachQuery;

namespace DSAExperimentation.Tests.LeetCodeCoverage.MinimumIntervalToIncludeEachQuery;

// Harness only. Both the per-query scan baseline and the offline heap sweep are
// MinimumIntervalToIncludeEachQuerySolution's - this file just pins them to LeetCode's
// published examples plus the uncovered-query, equal-size and descending-query cases
// that exercise the sweep's discard loop and its write-back through the original query
// order.
public sealed partial class MinimumIntervalToIncludeEachQueryTests
{
    public static TheoryData<int[][], int[], int[]> Examples =>
        new()
        {
            { [[1, 4], [2, 4], [3, 6], [4, 4]], [2, 3, 4, 5], [3, 3, 1, 4] },
            { [[2, 3], [2, 5], [1, 8], [20, 25]], [2, 19, 5, 22], [2, -1, 4, 6] },
            { [[1, 4], [2, 4], [3, 6], [4, 4]], [5, 4, 3, 2], [4, 1, 3, 3] },
            { [[1, 3], [5, 7]], [1, 4, 6], [3, -1, 3] },
            { [[1, 2]], [3], [-1] },
            { [[4, 4]], [4], [1] },
        };

    [Theory]
    [MemberData(nameof(Examples))]
    public void MinIntervalsByPerQueryScan_LeetCodeExamples_ReturnsSmallestCoveringSizePerQuery(
        int[][] intervals, int[] queries, int[] expected)
    {
        var actual = MinimumIntervalToIncludeEachQuerySolution.MinIntervalsByPerQueryScan(intervals, queries);

        Assert.Equal(expected, actual);
    }

    [Theory]
    [MemberData(nameof(Examples))]
    public void MinIntervalsByHeapSweep_LeetCodeExamples_ReturnsSmallestCoveringSizePerQuery(
        int[][] intervals, int[] queries, int[] expected)
    {
        var actual = MinimumIntervalToIncludeEachQuerySolution.MinIntervalsByHeapSweep(intervals, queries);

        Assert.Equal(expected, actual);
    }
}
