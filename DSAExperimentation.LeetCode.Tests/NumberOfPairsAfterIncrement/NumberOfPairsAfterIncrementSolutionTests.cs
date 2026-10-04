using DSAExperimentation.LeetCode.NumberOfPairsAfterIncrement;

namespace DSAExperimentation.LeetCode.Tests.NumberOfPairsAfterIncrement;

// Harness only. All three range-add/count strategies are NumberOfPairsAfterIncrementSolution's.
// The first three rows are LeetCode's published examples. The last two are read by hand, with
// ranges that cut through the block decomposition's sqrt(n)-sized blocks (blocks of 3 over ten
// values, of 2 over seven):
// - nums2 = 1..10, nums1 = [1, 2]: tot 7 matches 6 and 5 (2 pairs); after +3 on 1..8, nums2 is
//   [1,5,6,7,8,9,10,11,12,10] and tot 11 matches 10 twice and 9 once (3); after +1 on all of it,
//   tot 12 matches 11 twice and 10 once (3).
// - seven 5s, nums1 = [3]: tot 8 matches all 7; after +1 on 2..4, four 5s and three 6s are left
//   (tot 8: 4, tot 9: 3); after +2 on all of it, four 7s and three 8s (tot 10: 4, tot 11: 3).
public sealed partial class NumberOfPairsAfterIncrementSolutionTests
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
            {
                [1, 2], [1, 2, 3, 4, 5, 6, 7, 8, 9, 10],
                [[2, 7], [1, 1, 8, 3], [2, 11], [1, 0, 9, 1], [2, 12]],
                [2, 3, 3]
            },
            {
                [3], [5, 5, 5, 5, 5, 5, 5],
                [[2, 8], [1, 2, 4, 1], [2, 8], [2, 9], [1, 0, 6, 2], [2, 10], [2, 11]],
                [7, 4, 3, 4, 3]
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
    public void CountPairsByValueBlocks_LeetCodeExamples_ReturnsPairCountsPerQuery(
        int[] nums1, int[] nums2, int[][] queries, int[] expected)
    {
        var actual = NumberOfPairsAfterIncrementSolution.CountPairsByValueBlocks(nums1, nums2, queries);

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
