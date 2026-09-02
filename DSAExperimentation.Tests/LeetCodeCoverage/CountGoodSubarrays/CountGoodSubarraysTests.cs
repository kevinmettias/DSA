using DSAExperimentation.LeetCode.CountGoodSubarrays;

namespace DSAExperimentation.Tests.LeetCodeCoverage.CountGoodSubarrays;

// Harness only. The OR == max characterization and the running-OR-groups
// compression both live in CountGoodSubarraysSolution - this file just pins
// both strategies to LeetCode's published examples.
public sealed class CountGoodSubarraysTests
{
    public static TheoryData<int[], long> Examples =>
        new()
        {
            { new[] { 4, 2, 3 }, 4L },
            { new[] { 1, 3, 1 }, 6L },
        };

    [Theory]
    [MemberData(nameof(Examples))]
    public void CountGoodSubarraysByBruteForce_LeetCodeExamples_ReturnsGoodSubarrayCount(int[] nums, long expected) =>
        Assert.Equal(expected, CountGoodSubarraysSolution.CountGoodSubarraysByBruteForce(nums));

    [Theory]
    [MemberData(nameof(Examples))]
    public void CountGoodSubarraysByRunningOrGroups_LeetCodeExamples_ReturnsGoodSubarrayCount(int[] nums, long expected) =>
        Assert.Equal(expected, CountGoodSubarraysSolution.CountGoodSubarraysByRunningOrGroups(nums));
}
