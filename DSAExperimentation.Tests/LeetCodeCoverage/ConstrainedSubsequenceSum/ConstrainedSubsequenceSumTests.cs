using DSAExperimentation.LeetCode.ConstrainedSubsequenceSum;

namespace DSAExperimentation.Tests.LeetCodeCoverage.ConstrainedSubsequenceSum;

// Harness only: both strategies live in ConstrainedSubsequenceSumSolution and are
// asserted against the same examples - LeetCode's three published ones, the
// all-negative case where the answer must still pick exactly one element, and a
// single-element array.
public sealed class ConstrainedSubsequenceSumTests
{
    public static TheoryData<int[], int, int> Examples =>
        new()
        {
            { [10, 2, -10, 5, 20], 2, 37 },
            { [-1, -2, -3], 1, -1 },
            { [10, -2, -10, -5, 20], 2, 23 },
            { [5], 1, 5 },
        };

    [Theory]
    [MemberData(nameof(Examples))]
    public void MaxSumByWindowRescan_LeetCodeExamples_ReturnsMaximumConstrainedSubsequenceSum(
        int[] nums, int k, int expected)
    {
        var actual = ConstrainedSubsequenceSumSolution.MaxSumByWindowRescan(nums, k);

        Assert.Equal(expected, actual);
    }

    [Theory]
    [MemberData(nameof(Examples))]
    public void MaxSumByMonotonicDeque_LeetCodeExamples_ReturnsMaximumConstrainedSubsequenceSum(
        int[] nums, int k, int expected)
    {
        var actual = ConstrainedSubsequenceSumSolution.MaxSumByMonotonicDeque(nums, k);

        Assert.Equal(expected, actual);
    }
}
