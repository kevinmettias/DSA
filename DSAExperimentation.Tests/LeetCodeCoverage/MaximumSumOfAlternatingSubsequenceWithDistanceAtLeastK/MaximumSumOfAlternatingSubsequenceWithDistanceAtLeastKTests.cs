using DSAExperimentation.LeetCode.MaximumSumOfAlternatingSubsequenceWithDistanceAtLeastK;

namespace DSAExperimentation.Tests.LeetCodeCoverage.MaximumSumOfAlternatingSubsequenceWithDistanceAtLeastK;

// Harness only: both strategies live in
// MaximumSumOfAlternatingSubsequenceWithDistanceAtLeastKSolution. One test method
// per strategy over one shared set of LeetCode's own examples, so a failure names
// the strategy that broke.
public sealed class MaximumSumOfAlternatingSubsequenceWithDistanceAtLeastKTests
{
    public static TheoryData<int[], int, long> Examples =>
        new()
        {
            { [5, 4, 2], 2, 7 },
            { [3, 5, 4, 2, 4], 1, 14 },
            { [5], 1, 5 },
        };

    [Theory]
    [MemberData(nameof(Examples))]
    public void MaxAlternatingSumByBruteForce_LeetCodeExamples_ReturnsMaximumAlternatingSum(
        int[] nums, int minimumDistance, long expected)
    {
        var actual = MaximumSumOfAlternatingSubsequenceWithDistanceAtLeastKSolution
            .MaxAlternatingSumByBruteForce(nums, minimumDistance);

        Assert.Equal(expected, actual);
    }

    [Theory]
    [MemberData(nameof(Examples))]
    public void MaxAlternatingSumBySegmentTree_LeetCodeExamples_ReturnsMaximumAlternatingSum(
        int[] nums, int minimumDistance, long expected)
    {
        var actual = MaximumSumOfAlternatingSubsequenceWithDistanceAtLeastKSolution
            .MaxAlternatingSumBySegmentTree(nums, minimumDistance);

        Assert.Equal(expected, actual);
    }
}
