using DSAExperimentation.LeetCode.FindSubarrayWithBitwiseORClosestToK;

namespace DSAExperimentation.Tests.LeetCodeCoverage.FindSubarrayWithBitwiseORClosestToK;

// Harness only. Both strategies are FindSubarrayWithBitwiseORClosestToKSolution's -
// this file just pins them to LeetCode's published examples.
public sealed class FindSubarrayWithBitwiseORClosestToKTests
{
    public static TheoryData<int[], int, int> Examples =>
        new()
        {
            { [1, 2, 4, 5], 3, 0 },
            { [1, 3, 1, 3], 2, 1 },
            { [1], 10, 9 },
        };

    [Theory]
    [MemberData(nameof(Examples))]
    public void MinimumDifferenceByBruteForce_LeetCodeExamples_ReturnsClosestOrDifference(
        int[] nums, int k, int expected) =>
        Assert.Equal(expected, FindSubarrayWithBitwiseORClosestToKSolution.MinimumDifferenceByBruteForce(nums, k));

    [Theory]
    [MemberData(nameof(Examples))]
    public void MinimumDifferenceByOrCompression_LeetCodeExamples_ReturnsClosestOrDifference(
        int[] nums, int k, int expected) =>
        Assert.Equal(expected, FindSubarrayWithBitwiseORClosestToKSolution.MinimumDifferenceByOrCompression(nums, k));
}
