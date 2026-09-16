using DSAExperimentation.LeetCode.JumpGameVI;

namespace DSAExperimentation.Tests.LeetCodeCoverage.JumpGameVI;

// Harness only. Both strategies are JumpGameVISolution's - MaxResultByWindowRescan
// (previously untested scaffolding inlined in the benchmark as its baseline arm) now
// gets the same examples as MaxResultByMonotonicDeque (previously this file's own
// private helper), so a failure names the strategy that broke.
public sealed partial class JumpGameVITests
{
    public static TheoryData<int[], int, int> Examples =>
        new()
        {
            { [1, -1, -2, 4, -7, 3], 2, 7 },
            { [10, -5, -2, 4, 0, 3], 3, 17 },
            { [1, -5, -20, 4, -1, 3, -6, -3], 2, 0 },
            { [1, -1, -2, 4, -7, 3], 1, -2 },
            { [1, -1, -2, 4, -7, 3], 5, 8 },
            { [10], 1, 10 },
        };

    [Theory]
    [MemberData(nameof(Examples))]
    public void MaxResultByWindowRescan_LeetCodeExamples_ReturnsBestReachableScore(
        int[] nums, int jumpLimit, int expected)
    {
        var actual = JumpGameVISolution.MaxResultByWindowRescan(nums, jumpLimit);

        Assert.Equal(expected, actual);
    }

    [Theory]
    [MemberData(nameof(Examples))]
    public void MaxResultByMonotonicDeque_LeetCodeExamples_ReturnsBestReachableScore(
        int[] nums, int jumpLimit, int expected)
    {
        var actual = JumpGameVISolution.MaxResultByMonotonicDeque(nums, jumpLimit);

        Assert.Equal(expected, actual);
    }
}
