using DSAExperimentation.LeetCode.NumberOfLongestIncreasingSubsequence;

namespace DSAExperimentation.Tests.LeetCodeCoverage.NumberOfLongestIncreasingSubsequence;

// Harness only: the algorithm lives in NumberOfLongestIncreasingSubsequenceSolution.
public sealed class NumberOfLongestIncreasingSubsequenceTests
{
    public static TheoryData<int[], int> Examples =>
        new()
        {
            { [1, 3, 5, 4, 7], 2 },
            { [2, 2, 2, 2, 2], 5 },
            { [1], 1 },
            { [5, 4, 3, 2, 1], 5 },
        };

    [Theory]
    [MemberData(nameof(Examples))]
    public void FindNumberOfLisByBruteForce_LeetCodeExamples_ReturnsExpectedCount(int[] nums, int expected)
        => Assert.Equal(expected, NumberOfLongestIncreasingSubsequenceSolution.FindNumberOfLisByBruteForce(nums));

    [Theory]
    [MemberData(nameof(Examples))]
    public void FindNumberOfLisBySegmentTree_LeetCodeExamples_ReturnsExpectedCount(int[] nums, int expected)
        => Assert.Equal(expected, NumberOfLongestIncreasingSubsequenceSolution.FindNumberOfLisBySegmentTree(nums));
}
