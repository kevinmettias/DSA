using DSAExperimentation.LeetCode.JumpGame;

namespace DSAExperimentation.Tests.LeetCodeCoverage.JumpGame;

// Harness only: both strategies live in JumpGameSolution and are asserted
// against the same examples - the greedy farthest-reach scan this file's
// original helper computed, and the O(n^2) forward-reachability DP that used
// to be untested benchmark scaffolding.
public sealed class JumpGameTests
{
    public static TheoryData<int[], bool> Examples =>
        new()
        {
            { [2, 3, 1, 1, 4], true },
            { [3, 2, 1, 0, 4], false },
            { [0], true },
            { [2, 0, 0], true },
            { [1, 0, 1, 0], false },
        };

    [Theory]
    [MemberData(nameof(Examples))]
    public void CanJumpByGreedyFarthestReach_LeetCodeExamples_ReturnsReachability(int[] nums, bool expected) =>
        Assert.Equal(expected, JumpGameSolution.CanJumpByGreedyFarthestReach(nums));

    [Theory]
    [MemberData(nameof(Examples))]
    public void CanJumpByForwardReachabilityDp_LeetCodeExamples_ReturnsReachability(int[] nums, bool expected) =>
        Assert.Equal(expected, JumpGameSolution.CanJumpByForwardReachabilityDp(nums));
}
