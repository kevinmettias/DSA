using DSAExperimentation.LeetCode.JumpGameVII;

namespace DSAExperimentation.Tests.LeetCodeCoverage.JumpGameVII;

// Harness only. Both strategies are JumpGameVIISolution's - LeetCode's published
// examples are stated once and replayed against each, so a failure names the
// strategy that broke rather than reporting a disagreement between an anonymous
// test helper and an anonymous benchmark arm. The unmemoized baseline was never
// asserted before this migration; every case here is small enough that its
// reconverging jump chains stay cheap.
public sealed partial class JumpGameVIITests
{
    public static TheoryData<JumpWindowExample> Examples =>
        new()
        {
            // LC example 1: 0 -> 3 -> 5.
            { new JumpWindowExample(Positions: "011010", MinJump: 2, MaxJump: 3, Expected: true) },

            // LC example 2: nothing in the jump window from 0 is a '0'.
            { new JumpWindowExample(Positions: "01101110", MinJump: 2, MaxJump: 3, Expected: false) },

            // The last index is reachable in one step, and is the start's only move.
            { new JumpWindowExample(Positions: "00", MinJump: 1, MaxJump: 1, Expected: true) },

            // The last index is a wall, so it can never be landed on.
            { new JumpWindowExample(Positions: "0001", MinJump: 1, MaxJump: 2, Expected: false) },

            // Every index is open and the window straddles the end.
            { new JumpWindowExample(Positions: "0000", MinJump: 2, MaxJump: 3, Expected: true) },

            // A wall inside a width-one window cuts the only chain at index 2.
            { new JumpWindowExample(Positions: "0010", MinJump: 1, MaxJump: 1, Expected: false) },
        };

    [Theory]
    [MemberData(nameof(Examples))]
    public void CanReachByUnmemoizedRecursion_LeetCodeExamples_ReportsWhetherTheLastIndexIsReachable(
        JumpWindowExample example)
    {
        var actual = JumpGameVIISolution.CanReachByUnmemoizedRecursion(
            example.Positions, example.MinJump, example.MaxJump);

        Assert.Equal(example.Expected, actual);
    }

    [Theory]
    [MemberData(nameof(Examples))]
    public void CanReachByVisitedTrackingTraversal_LeetCodeExamples_ReportsWhetherTheLastIndexIsReachable(
        JumpWindowExample example)
    {
        var actual = JumpGameVIISolution.CanReachByVisitedTrackingTraversal(
            example.Positions, example.MinJump, example.MaxJump);

        Assert.Equal(example.Expected, actual);
    }

    // One LeetCode example: the '0'/'1' positions, the jump window's bounds, and whether
    // the last index can be reached. The `bool` is the expected answer rather than a
    // mode, so the row names it instead of leaving a bare `true` in a position to decode.
    public readonly record struct JumpWindowExample(
        string Positions, int MinJump, int MaxJump, bool Expected);
}
