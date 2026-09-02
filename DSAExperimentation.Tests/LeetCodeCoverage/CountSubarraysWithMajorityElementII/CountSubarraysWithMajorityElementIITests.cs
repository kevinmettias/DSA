using DSAExperimentation.LeetCode.CountSubarraysWithMajorityElementII;

namespace DSAExperimentation.Tests.LeetCodeCoverage.CountSubarraysWithMajorityElementII;

// Harness only. Both strategies are
// CountSubarraysWithMajorityElementIISolution's - this file just pins them to
// LeetCode's published examples (the same three examples LC 3737's own
// CountSubarraysWithMajorityElementITests uses - LC 3739 restates the identical
// problem at a larger scale).
public sealed class CountSubarraysWithMajorityElementIITests
{
    public static TheoryData<int[], int, long> Examples =>
        new()
        {
            { [1, 2, 2, 3], 2, 5 },
            { [1, 1, 1, 1], 1, 10 },
            { [1, 2, 3], 4, 0 },
        };

    [Theory]
    [MemberData(nameof(Examples))]
    public void CountByBruteForce_LeetCodeExamples_ReturnsMajoritySubarrayCount(int[] nums, int target, long expected) =>
        Assert.Equal(expected, CountSubarraysWithMajorityElementIISolution.CountByBruteForce(nums, target));

    [Theory]
    [MemberData(nameof(Examples))]
    public void CountByFenwickPrefixSum_LeetCodeExamples_ReturnsMajoritySubarrayCount(int[] nums, int target, long expected) =>
        Assert.Equal(expected, CountSubarraysWithMajorityElementIISolution.CountByFenwickPrefixSum(nums, target));
}
