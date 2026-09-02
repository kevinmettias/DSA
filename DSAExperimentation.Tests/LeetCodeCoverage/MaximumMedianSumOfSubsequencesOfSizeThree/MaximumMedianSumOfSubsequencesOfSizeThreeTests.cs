using DSAExperimentation.LeetCode.MaximumMedianSumOfSubsequencesOfSizeThree;

namespace DSAExperimentation.Tests.LeetCodeCoverage.MaximumMedianSumOfSubsequencesOfSizeThree;

// Harness only. The grouping-and-median-picking itself is
// MaximumMedianSumOfSubsequencesOfSizeThreeSolution's - this file just pins
// both strategies to LeetCode's published examples plus the smallest possible
// case (one triple, so the median sum is just that triple's own median).
public sealed class MaximumMedianSumOfSubsequencesOfSizeThreeTests
{
    public static TheoryData<int[], long> Examples =>
        new()
        {
            { [2, 1, 3, 2, 1, 3], 5L },
            { [1, 1, 10, 10, 10, 10], 20L },
            { [1, 2, 3], 2L },
        };

    [Theory]
    [MemberData(nameof(Examples))]
    public void MaximumMedianSumByBruteForce_LeetCodeExamples_ReturnsMaximumMedianSum(
        int[] nums, long expected) =>
        Assert.Equal(expected, MaximumMedianSumOfSubsequencesOfSizeThreeSolution.MaximumMedianSumByBruteForce(nums));

    [Theory]
    [MemberData(nameof(Examples))]
    public void MaximumMedianSumBySortedGreedy_LeetCodeExamples_ReturnsMaximumMedianSum(
        int[] nums, long expected) =>
        Assert.Equal(expected, MaximumMedianSumOfSubsequencesOfSizeThreeSolution.MaximumMedianSumBySortedGreedy(nums));
}
