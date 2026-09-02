using DSAExperimentation.LeetCode.MaximumAndMinimumSumsOfAtMostSizeKSubarrays;

namespace DSAExperimentation.Tests.LeetCodeCoverage.MaximumAndMinimumSumsOfAtMostSizeKSubarrays;

// Harness only. Both strategies are
// MaximumAndMinimumSumsOfAtMostSizeKSubarraysSolution's - this file just pins
// them to LeetCode's published examples, including the negative-value case that
// exercises the monotonic-stack tie-break against a repeated value (the two 1s).
public sealed class MaximumAndMinimumSumsOfAtMostSizeKSubarraysTests
{
    public static TheoryData<int[], int, long> Examples =>
        new()
        {
            { [1, 2, 3], 2, 20 },
            { [1, -3, 1], 2, -6 },
        };

    [Theory]
    [MemberData(nameof(Examples))]
    public void SumByBruteForceWindow_LeetCodeExamples_ReturnsMaxPlusMinSum(int[] nums, int k, long expected) =>
        Assert.Equal(
            expected,
            MaximumAndMinimumSumsOfAtMostSizeKSubarraysSolution.SumByBruteForceWindow(nums, k));

    [Theory]
    [MemberData(nameof(Examples))]
    public void SumByMonotonicStackContribution_LeetCodeExamples_ReturnsMaxPlusMinSum(int[] nums, int k, long expected) =>
        Assert.Equal(
            expected,
            MaximumAndMinimumSumsOfAtMostSizeKSubarraysSolution.SumByMonotonicStackContribution(nums, k));
}
