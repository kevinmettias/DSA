using DSAExperimentation.LeetCode.CountSubarraysWithCostLessThanOrEqualToK;

namespace DSAExperimentation.Tests.LeetCodeCoverage.CountSubarraysWithCostLessThanOrEqualToK;

// Harness only. Both strategies are
// CountSubarraysWithCostLessThanOrEqualToKSolution's - this file just pins them
// to LeetCode's published examples, including the all-equal array (every
// subarray costs 0 regardless of length) and the k = 0 case (only single-element
// subarrays ever qualify).
public sealed class CountSubarraysWithCostLessThanOrEqualToKTests
{
    public static TheoryData<int[], long, long> Examples =>
        new()
        {
            { [1, 3, 2], 4, 5 },
            { [5, 5, 5, 5], 0, 10 },
            { [1, 2, 3], 0, 3 },
        };

    [Theory]
    [MemberData(nameof(Examples))]
    public void CountByBruteForce_LeetCodeExamples_ReturnsMatchingSubarrayCount(int[] nums, long k, long expected)
    {
        var actual = CountSubarraysWithCostLessThanOrEqualToKSolution.CountByBruteForce(nums, k);
        Assert.Equal(expected, actual);
    }

    [Theory]
    [MemberData(nameof(Examples))]
    public void CountByMonotonicDeques_LeetCodeExamples_ReturnsMatchingSubarrayCount(int[] nums, long k, long expected)
    {
        var actual = CountSubarraysWithCostLessThanOrEqualToKSolution.CountByMonotonicDeques(nums, k);
        Assert.Equal(expected, actual);
    }
}
