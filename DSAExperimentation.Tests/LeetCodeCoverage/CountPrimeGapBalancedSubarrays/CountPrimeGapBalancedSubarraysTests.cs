using DSAExperimentation.LeetCode.CountPrimeGapBalancedSubarrays;

namespace DSAExperimentation.Tests.LeetCodeCoverage.CountPrimeGapBalancedSubarrays;

// Harness only: both strategies are CountPrimeGapBalancedSubarraysSolution's -
// this file just pins them to LeetCode's published examples (OpenTheLockTests
// precedent).
public sealed class CountPrimeGapBalancedSubarraysTests
{
    public static TheoryData<int[], int, long> Examples =>
        new()
        {
            { [1, 2, 3], 1, 2 },
            { [2, 3, 5, 7], 3, 4 },
        };

    [Theory]
    [MemberData(nameof(Examples))]
    public void CountByBruteForce_LeetCodeExamples_ReturnsPrimeGapBalancedSubarrayCount(
        int[] nums, int k, long expected) =>
        Assert.Equal(expected, CountPrimeGapBalancedSubarraysSolution.CountByBruteForce(nums, k));

    [Theory]
    [MemberData(nameof(Examples))]
    public void CountByPrimeWindowDeque_LeetCodeExamples_ReturnsPrimeGapBalancedSubarrayCount(
        int[] nums, int k, long expected) =>
        Assert.Equal(expected, CountPrimeGapBalancedSubarraysSolution.CountByPrimeWindowDeque(nums, k));
}
