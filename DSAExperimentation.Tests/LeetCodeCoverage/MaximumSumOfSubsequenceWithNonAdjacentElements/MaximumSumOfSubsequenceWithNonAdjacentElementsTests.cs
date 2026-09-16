using DSAExperimentation.LeetCode.MaximumSumOfSubsequenceWithNonAdjacentElements;

namespace DSAExperimentation.Tests.LeetCodeCoverage.MaximumSumOfSubsequenceWithNonAdjacentElements;

// Harness only. Both strategies are
// MaximumSumOfSubsequenceWithNonAdjacentElementsSolution's - this file just
// pins them to LeetCode's published examples.
public sealed partial class MaximumSumOfSubsequenceWithNonAdjacentElementsTests
{
    public static TheoryData<int[], int[][], int> Examples =>
        new()
        {
            { [3, 5, 9], [[1, -2], [0, -3]], 21 },
            { [0, -1], [[0, -5]], 0 },
        };

    [Theory]
    [MemberData(nameof(Examples))]
    public void MaximumSumByRecomputeDP_LeetCodeExamples_ReturnsSumOfAnswersModulo(
        int[] nums, int[][] queries, int expected)
    {
        var actual = MaximumSumOfSubsequenceWithNonAdjacentElementsSolution.MaximumSumByRecomputeDP(nums, queries);

        Assert.Equal(expected, actual);
    }

    [Theory]
    [MemberData(nameof(Examples))]
    public void MaximumSumBySegmentTreeMerge_LeetCodeExamples_ReturnsSumOfAnswersModulo(
        int[] nums, int[][] queries, int expected)
    {
        var actual = MaximumSumOfSubsequenceWithNonAdjacentElementsSolution.MaximumSumBySegmentTreeMerge(nums, queries);

        Assert.Equal(expected, actual);
    }
}
