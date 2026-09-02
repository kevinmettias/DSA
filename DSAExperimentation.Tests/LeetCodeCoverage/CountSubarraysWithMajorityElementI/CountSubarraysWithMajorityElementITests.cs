using DSAExperimentation.LeetCode.CountSubarraysWithMajorityElementI;

namespace DSAExperimentation.Tests.LeetCodeCoverage.CountSubarraysWithMajorityElementI;

// Harness only. Both strategies are
// CountSubarraysWithMajorityElementISolution's - this file just pins them to
// LeetCode's published examples.
public sealed class CountSubarraysWithMajorityElementITests
{
    public static TheoryData<int[], int, int> Examples =>
        new()
        {
            { [1, 2, 2, 3], 2, 5 },
            { [1, 1, 1, 1], 1, 10 },
            { [1, 2, 3], 4, 0 },
        };

    [Theory]
    [MemberData(nameof(Examples))]
    public void CountByBruteForce_LeetCodeExamples_ReturnsMajoritySubarrayCount(int[] nums, int target, int expected) =>
        Assert.Equal(expected, CountSubarraysWithMajorityElementISolution.CountByBruteForce(nums, target));

    [Theory]
    [MemberData(nameof(Examples))]
    public void CountByFenwickPrefixSum_LeetCodeExamples_ReturnsMajoritySubarrayCount(int[] nums, int target, int expected) =>
        Assert.Equal(expected, CountSubarraysWithMajorityElementISolution.CountByFenwickPrefixSum(nums, target));
}
