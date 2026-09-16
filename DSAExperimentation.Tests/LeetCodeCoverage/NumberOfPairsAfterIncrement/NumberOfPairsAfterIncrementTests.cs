using DSAExperimentation.LeetCode.NumberOfPairsAfterIncrement;

namespace DSAExperimentation.Tests.LeetCodeCoverage.NumberOfPairsAfterIncrement;

// Harness only. Both range-add/count strategies are
// NumberOfPairsAfterIncrementSolution's - this file just pins them to
// LeetCode's published examples.
public sealed class NumberOfPairsAfterIncrementTests
{
    public static TheoryData<int[], int[], int[][], int[]> Examples =>
        new()
        {
            {
                [1, 2], [3, 4],
                [[2, 5], [1, 0, 0, 2], [2, 5]],
                [2, 1]
            },
            {
                [1, 1], [2, 2, 3],
                [[2, 4], [1, 0, 1, 1], [2, 4]],
                [2, 6]
            },
            {
                [2, 5, 8, 4], [1, 3, 8],
                [[2, 9], [1, 1, 2, 1], [2, 10]],
                [1, 0]
            },
        };

    [Theory]
    [MemberData(nameof(Examples))]
    public void CountPairsByDirectArray_LeetCodeExamples_ReturnsPairCountsPerQuery(
        int[] nums1, int[] nums2, int[][] queries, int[] expected)
    {
        var actual = NumberOfPairsAfterIncrementSolution.CountPairsByDirectArray(nums1, nums2, queries);

        Assert.Equal(expected, actual);
    }

    [Theory]
    [MemberData(nameof(Examples))]
    public void CountPairsByRangeFenwickTree_LeetCodeExamples_ReturnsPairCountsPerQuery(
        int[] nums1, int[] nums2, int[][] queries, int[] expected)
    {
        var actual = NumberOfPairsAfterIncrementSolution.CountPairsByRangeFenwickTree(nums1, nums2, queries);

        Assert.Equal(expected, actual);
    }
}
