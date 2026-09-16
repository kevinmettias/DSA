using DSAExperimentation.LeetCode.JumpGame;

namespace DSAExperimentation.Tests.LeetCodeCoverage.JumpGame;

// Harness only: both strategies live in JumpGameSolution and are asserted
// against the same examples - the greedy farthest-reach scan this file's
// original helper computed, and the O(n^2) forward-reachability DP that used
// to be untested benchmark scaffolding.
public sealed class JumpGameTests
{
    public static TheoryData<JumpReachExample> Examples =>
        new()
        {
            { new JumpReachExample(Nums: [2, 3, 1, 1, 4], Expected: true) },
            { new JumpReachExample(Nums: [3, 2, 1, 0, 4], Expected: false) },
            { new JumpReachExample(Nums: [0], Expected: true) },
            { new JumpReachExample(Nums: [2, 0, 0], Expected: true) },
            { new JumpReachExample(Nums: [1, 0, 1, 0], Expected: false) },
        };

    [Theory]
    [MemberData(nameof(Examples))]
    public void CanJumpByGreedyFarthestReach_LeetCodeExamples_ReturnsReachability(JumpReachExample example)
    {
        var actual = JumpGameSolution.CanJumpByGreedyFarthestReach(example.Nums);

        Assert.Equal(example.Expected, actual);
    }

    [Theory]
    [MemberData(nameof(Examples))]
    public void CanJumpByForwardReachabilityDp_LeetCodeExamples_ReturnsReachability(JumpReachExample example)
    {
        var actual = JumpGameSolution.CanJumpByForwardReachabilityDp(example.Nums);

        Assert.Equal(example.Expected, actual);
    }

    // One LeetCode example: the jump lengths, and whether the last index is reachable.
    // The `bool` is the expected answer rather than a mode, so the row names it instead
    // of leaving a bare `true` in a position the reader has to decode.
    public readonly record struct JumpReachExample(int[] Nums, bool Expected);
}
