using DSAExperimentation.LeetCode.MinimumInversionCountInSubarraysOfFixedLength;

namespace DSAExperimentation.Tests.LeetCodeCoverage.MinimumInversionCountInSubarraysOfFixedLength;

// Harness only. Both strategies are
// MinimumInversionCountInSubarraysOfFixedLengthSolution's - this file just
// pins them to LeetCode's published examples.
public sealed class MinimumInversionCountInSubarraysOfFixedLengthTests
{
    public static TheoryData<int[], int, long> Examples =>
        new()
        {
            { [3, 1, 2, 5, 4], 3, 0 },
            { [5, 3, 2, 1], 4, 6 },
            { [2, 1], 1, 0 },
        };

    [Theory]
    [MemberData(nameof(Examples))]
    public void MinInversionCountByBruteForce_LeetCodeExamples_ReturnsMinimumInversionCount(
        int[] nums, int k, long expected) =>
        Assert.Equal(
            expected,
            MinimumInversionCountInSubarraysOfFixedLengthSolution.MinInversionCountByBruteForce(nums, k));

    [Theory]
    [MemberData(nameof(Examples))]
    public void MinInversionCountBySlidingWindowFenwick_LeetCodeExamples_ReturnsMinimumInversionCount(
        int[] nums, int k, long expected) =>
        Assert.Equal(
            expected,
            MinimumInversionCountInSubarraysOfFixedLengthSolution.MinInversionCountBySlidingWindowFenwick(nums, k));
}
