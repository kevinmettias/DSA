using DSAExperimentation.LeetCode.LongestIncreasingSubsequence;

namespace DSAExperimentation.Tests.LeetCodeCoverage.LongestIncreasingSubsequence;

// Harness only. Both strategies are LongestIncreasingSubsequenceSolution's - this file
// just pins them to LeetCode's published examples.
public sealed class LongestIncreasingSubsequenceTests
{
    public static TheoryData<int[], int> Examples =>
        new()
        {
            { [10, 9, 2, 5, 3, 7, 101, 18], 4 },
            { [0, 1, 0, 3, 2, 3], 4 },
            { [7, 7, 7, 7, 7, 7, 7], 1 },
        };

    [Theory]
    [MemberData(nameof(Examples))]
    public void LengthOfLisByDynamicProgramming_LeetCodeExamples_ReturnsExpectedLength(
        int[] nums, int expected) =>
        Assert.Equal(expected, LongestIncreasingSubsequenceSolution.LengthOfLisByDynamicProgramming(nums));

    [Theory]
    [MemberData(nameof(Examples))]
    public void LengthOfLisByPatienceSortingBinarySearch_LeetCodeExamples_ReturnsExpectedLength(
        int[] nums, int expected) =>
        Assert.Equal(
            expected,
            LongestIncreasingSubsequenceSolution.LengthOfLisByPatienceSortingBinarySearch(nums));
}
