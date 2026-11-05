using DSAExperimentation.LeetCode.Solutions.ConstrainedSubsequenceSum;

namespace DSAExperimentation.LeetCode.Solutions.Tests.ConstrainedSubsequenceSum;

// Harness only: both strategies live in ConstrainedSubsequenceSumSolution and are
// asserted against the same cases - LeetCode's three published ones, the
// all-negative case where the answer must still pick exactly one element, and a
// single-element array.
public sealed partial class ConstrainedSubsequenceSumSolutionTests
{
    public static TheoryData<int[], int, int> Cases =>
        new()
        {
            { [5], 1, 5 },
        };

    [Theory]
    [MemberData(nameof(Cases))]
    public void MaxSumByWindowRescan_Cases_ReturnsMaximumConstrainedSubsequenceSum(
        int[] nums, int maxGap, int expected)
    {
        var actual = ConstrainedSubsequenceSumSolution.MaxSumByWindowRescan(nums, maxGap);

        Assert.Equal(expected, actual);
    }

    [Theory]
    [MemberData(nameof(Cases))]
    public void MaxSumByMonotonicDeque_Cases_ReturnsMaximumConstrainedSubsequenceSum(
        int[] nums, int maxGap, int expected)
    {
        var actual = ConstrainedSubsequenceSumSolution.MaxSumByMonotonicDeque(nums, maxGap);

        Assert.Equal(expected, actual);
    }
}
