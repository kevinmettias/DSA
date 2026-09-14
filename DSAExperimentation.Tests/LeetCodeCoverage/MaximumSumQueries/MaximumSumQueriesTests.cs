using DSAExperimentation.LeetCode.MaximumSumQueries;

namespace DSAExperimentation.Tests.LeetCodeCoverage.MaximumSumQueries;

// Harness only. Both strategies are MaximumSumQueriesSolution's - this file just pins
// them to LeetCode's published examples plus the boundary cases that decide whether the
// coordinate-compressed suffix query is inclusive: a threshold exactly equal to the only
// pair, and one a single unit past it in each coordinate.
public sealed class MaximumSumQueriesTests
{
    public static TheoryData<int[], int[], int[][], int[]> Examples =>
        new()
        {
            { [4, 3, 1, 2], [2, 4, 9, 5], [[4, 1], [1, 3], [2, 5]], [6, 10, 7] },
            { [3, 2, 5], [2, 3, 4], [[4, 4], [3, 2], [1, 1]], [9, 9, 9] },
            { [2, 1], [2, 3], [[3, 3]], [-1] },
            { [1, 1], [1, 1], [[5, 5]], [-1] },
            { [5], [5], [[5, 5], [6, 5], [5, 6]], [10, -1, -1] },
        };

    [Theory]
    [MemberData(nameof(Examples))]
    public void MaxSumsByBruteForceScan_LeetCodeExamples_ReturnsBestQualifyingSumPerQuery(
        int[] nums1, int[] nums2, int[][] queries, int[] expected) =>
        Assert.Equal(expected, MaximumSumQueriesSolution.MaxSumsByBruteForceScan(nums1, nums2, queries));

    [Theory]
    [MemberData(nameof(Examples))]
    public void MaxSumsBySweepWithSegmentTree_LeetCodeExamples_ReturnsBestQualifyingSumPerQuery(
        int[] nums1, int[] nums2, int[][] queries, int[] expected) =>
        Assert.Equal(expected, MaximumSumQueriesSolution.MaxSumsBySweepWithSegmentTree(nums1, nums2, queries));
}
