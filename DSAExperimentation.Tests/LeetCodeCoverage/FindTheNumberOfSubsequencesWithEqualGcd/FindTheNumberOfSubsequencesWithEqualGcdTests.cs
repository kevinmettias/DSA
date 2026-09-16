using DSAExperimentation.LeetCode.FindTheNumberOfSubsequencesWithEqualGcd;

namespace DSAExperimentation.Tests.LeetCodeCoverage.FindTheNumberOfSubsequencesWithEqualGcd;

// Harness only: both strategies live in
// FindTheNumberOfSubsequencesWithEqualGcdSolution - this file just pins them to
// LeetCode's published examples plus a small hand-verified duplicate-value case
// (TwoSumTests precedent).
public sealed partial class FindTheNumberOfSubsequencesWithEqualGcdTests
{
    public static TheoryData<int[], int> Examples =>
        new()
        {
            { [1, 2, 3, 4], 10 },
            { [10, 20, 30], 2 },
            { [1, 1], 2 },
        };

    [Theory]
    [MemberData(nameof(Examples))]
    public void CountPairsByBruteForce_LeetCodeExamples_ReturnsPairCountModuloLargePrime(int[] nums, int expected) =>
        Assert.Equal(expected, FindTheNumberOfSubsequencesWithEqualGcdSolution.CountPairsByBruteForce(nums));

    [Theory]
    [MemberData(nameof(Examples))]
    public void CountPairsByGcdMemoization_LeetCodeExamples_ReturnsPairCountModuloLargePrime(int[] nums, int expected) =>
        Assert.Equal(expected, FindTheNumberOfSubsequencesWithEqualGcdSolution.CountPairsByGcdMemoization(nums));
}
