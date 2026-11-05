using DSAExperimentation.LeetCode.Solutions.FindTheNumberOfSubsequencesWithEqualGcd;

namespace DSAExperimentation.LeetCode.Solutions.Tests.FindTheNumberOfSubsequencesWithEqualGcd;

// Harness only: both strategies live in
// FindTheNumberOfSubsequencesWithEqualGcdSolution - this file just pins them to
// a small hand-verified duplicate-value case
// (TwoSumTests precedent).
public sealed partial class FindTheNumberOfSubsequencesWithEqualGcdSolutionTests
{
    public static TheoryData<int[], int> Cases =>
        new()
        {
            { [1, 1], 2 },
        };

    [Theory]
    [MemberData(nameof(Cases))]
    public void CountPairsByBruteForce_Cases_ReturnsPairCountModuloLargePrime(int[] nums, int expected) =>
        Assert.Equal(expected, FindTheNumberOfSubsequencesWithEqualGcdSolution.CountPairsByBruteForce(nums));

    [Theory]
    [MemberData(nameof(Cases))]
    public void CountPairsByGcdMemoization_Cases_ReturnsPairCountModuloLargePrime(int[] nums, int expected) =>
        Assert.Equal(expected, FindTheNumberOfSubsequencesWithEqualGcdSolution.CountPairsByGcdMemoization(nums));
}
