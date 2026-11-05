using DSAExperimentation.LeetCode.Solutions.MaximumSumQueries;

namespace DSAExperimentation.LeetCode.Solutions.Tests.MaximumSumQueries;

// Harness only. Both strategies are MaximumSumQueriesSolution's - this file just pins
// them to the boundary cases that decide whether the
// coordinate-compressed suffix query is inclusive: a threshold exactly equal to the only
// pair, and one a single unit past it in each coordinate.
public sealed partial class MaximumSumQueriesSolutionTests
{
    public static TheoryData<int[], int[], int[][], int[]> Cases =>
        new()
        {
            { [1, 1], [1, 1], [[5, 5]], [-1] },
            { [5], [5], [[5, 5], [6, 5], [5, 6]], [10, -1, -1] },
        };

    [Theory]
    [MemberData(nameof(Cases))]
    public void MaxSumsByBruteForceScan_Cases_ReturnsBestQualifyingSumPerQuery(
        int[] nums1, int[] nums2, int[][] queries, int[] expected)
    {
        var actual = MaximumSumQueriesSolution.MaxSumsByBruteForceScan(nums1, nums2, queries);

        Assert.Equal(expected, actual);
    }

    [Theory]
    [MemberData(nameof(Cases))]
    public void MaxSumsBySweepWithSegmentTree_Cases_ReturnsBestQualifyingSumPerQuery(
        int[] nums1, int[] nums2, int[][] queries, int[] expected)
    {
        var actual = MaximumSumQueriesSolution.MaxSumsBySweepWithSegmentTree(nums1, nums2, queries);

        Assert.Equal(expected, actual);
    }
}
