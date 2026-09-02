using DSAExperimentation.LeetCode.CountSubarraysWithEvenOddRatioII;

namespace DSAExperimentation.Tests.LeetCodeCoverage.CountSubarraysWithEvenOddRatioII;

// Harness only. Both strategies are CountSubarraysWithEvenOddRatioIISolution's
// - this file just pins them to LeetCode's published examples (the same
// three LC 4011 publishes, since 4013 is the identical rule at a larger n).
public sealed class CountSubarraysWithEvenOddRatioIITests
{
    public static TheoryData<int[], int, int, long> Examples =>
        new()
        {
            { [1, 2, 1, 2], 3, 2, 7L },
            { [2, 2, 1], 2, 1, 3L },
            { [2, 2, 2], 1, 1, 0L },
        };

    [Theory]
    [MemberData(nameof(Examples))]
    public void CountByBruteForce_LeetCodeExamples_ReturnsValidSubarrayCount(
        int[] nums, int a, int b, long expected) =>
        Assert.Equal(expected, CountSubarraysWithEvenOddRatioIISolution.CountByBruteForce(nums, a, b));

    [Theory]
    [MemberData(nameof(Examples))]
    public void CountByFenwickPrefixSweep_LeetCodeExamples_ReturnsValidSubarrayCount(
        int[] nums, int a, int b, long expected) =>
        Assert.Equal(expected, CountSubarraysWithEvenOddRatioIISolution.CountByFenwickPrefixSweep(nums, a, b));
}
