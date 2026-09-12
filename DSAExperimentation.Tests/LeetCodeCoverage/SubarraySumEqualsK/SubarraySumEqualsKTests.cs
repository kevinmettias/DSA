using DSAExperimentation.LeetCode.SubarraySumEqualsK;

namespace DSAExperimentation.Tests.LeetCodeCoverage.SubarraySumEqualsK;

// LeetCode 560. Subarray Sum Equals K: harness only. Both strategies are
// SubarraySumEqualsKSolution's; this file pins them to the same examples.
public sealed class SubarraySumEqualsKTests
{
    public static TheoryData<int[], int, int> Examples =>
        new()
        {
            { [1, 1, 1], 2, 2 },
            { [1, 2, 1, 2, 1], 3, 4 },
            { [1, -1, 0], 0, 3 },
        };

    [Theory]
    [MemberData(nameof(Examples))]
    public void CountByBruteForce_LeetCodeExamples_ReturnsExpectedCount(int[] nums, int k, int expected) =>
        Assert.Equal(expected, SubarraySumEqualsKSolution.CountByBruteForce(nums, k));

    [Theory]
    [MemberData(nameof(Examples))]
    public void CountByPrefixSumHashMap_LeetCodeExamples_ReturnsExpectedCount(int[] nums, int k, int expected) =>
        Assert.Equal(expected, SubarraySumEqualsKSolution.CountByPrefixSumHashMap(nums, k));
}
