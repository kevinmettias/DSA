using DSAExperimentation.LeetCode.Solutions.MaximumMedianSumOfSubsequencesOfSizeThree;

namespace DSAExperimentation.LeetCode.Solutions.Tests.MaximumMedianSumOfSubsequencesOfSizeThree;

// Harness only. The grouping-and-median-picking itself is
// MaximumMedianSumOfSubsequencesOfSizeThreeSolution's - this file just pins
// both strategies to the smallest possible
// case (one triple, so the median sum is just that triple's own median).
public sealed partial class MaximumMedianSumOfSubsequencesOfSizeThreeSolutionTests
{
    public static TheoryData<int[], long> Cases =>
        new()
        {
            { [1, 2, 3], 2L },
        };

    [Theory]
    [MemberData(nameof(Cases))]
    public void MaximumMedianSumByBruteForce_Cases_ReturnsMaximumMedianSum(
        int[] nums, long expected) =>
        Assert.Equal(expected, MaximumMedianSumOfSubsequencesOfSizeThreeSolution.MaximumMedianSumByBruteForce(nums));

    [Theory]
    [MemberData(nameof(Cases))]
    public void MaximumMedianSumBySortedGreedy_Cases_ReturnsMaximumMedianSum(
        int[] nums, long expected) =>
        Assert.Equal(expected, MaximumMedianSumOfSubsequencesOfSizeThreeSolution.MaximumMedianSumBySortedGreedy(nums));
}
