using DSAExperimentation.LeetCode.Solutions.JumpGameVI;

namespace DSAExperimentation.LeetCode.Solutions.Tests.JumpGameVI;

// Harness only. Both strategies are JumpGameVISolution's - MaxResultByWindowRescan
// (previously untested scaffolding inlined in the benchmark as its baseline arm) now
// gets the same cases as MaxResultByMonotonicDeque (previously this file's own
// private helper), so a failure names the strategy that broke.
public sealed partial class JumpGameVISolutionTests
{
    public static TheoryData<int[], int, int> Cases =>
        new()
        {
            { [1, -1, -2, 4, -7, 3], 1, -2 },
            { [1, -1, -2, 4, -7, 3], 5, 8 },
            { [10], 1, 10 },
        };

    [Theory]
    [MemberData(nameof(Cases))]
    public void MaxResultByWindowRescan_Cases_ReturnsBestReachableScore(
        int[] nums, int jumpLimit, int expected)
    {
        var actual = JumpGameVISolution.MaxResultByWindowRescan(nums, jumpLimit);

        Assert.Equal(expected, actual);
    }

    [Theory]
    [MemberData(nameof(Cases))]
    public void MaxResultByMonotonicDeque_Cases_ReturnsBestReachableScore(
        int[] nums, int jumpLimit, int expected)
    {
        var actual = JumpGameVISolution.MaxResultByMonotonicDeque(nums, jumpLimit);

        Assert.Equal(expected, actual);
    }
}
