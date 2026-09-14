using DSAExperimentation.LeetCode.JumpGameVII;

namespace DSAExperimentation.Tests.LeetCodeCoverage.JumpGameVII;

// Harness only. Both strategies are JumpGameVIISolution's - LeetCode's published
// examples are stated once and replayed against each, so a failure names the
// strategy that broke rather than reporting a disagreement between an anonymous
// test helper and an anonymous benchmark arm. The unmemoized baseline was never
// asserted before this migration; every case here is small enough that its
// reconverging jump chains stay cheap.
public sealed class JumpGameVIITests
{
    public static TheoryData<string, int, int, bool> Examples =>
        new()
        {
            // LC example 1: 0 -> 3 -> 5.
            { "011010", 2, 3, true },

            // LC example 2: nothing in the jump window from 0 is a '0'.
            { "01101110", 2, 3, false },

            // The last index is reachable in one step, and is the start's only move.
            { "00", 1, 1, true },

            // The last index is a wall, so it can never be landed on.
            { "0001", 1, 2, false },

            // Every index is open and the window straddles the end.
            { "0000", 2, 3, true },

            // A wall inside a width-one window cuts the only chain at index 2.
            { "0010", 1, 1, false },
        };

    [Theory]
    [MemberData(nameof(Examples))]
    public void CanReachByUnmemoizedRecursion_LeetCodeExamples_ReportsWhetherTheLastIndexIsReachable(
        string positions, int minJump, int maxJump, bool expected) =>
        Assert.Equal(expected, JumpGameVIISolution.CanReachByUnmemoizedRecursion(positions, minJump, maxJump));

    [Theory]
    [MemberData(nameof(Examples))]
    public void CanReachByVisitedTrackingTraversal_LeetCodeExamples_ReportsWhetherTheLastIndexIsReachable(
        string positions, int minJump, int maxJump, bool expected) =>
        Assert.Equal(expected, JumpGameVIISolution.CanReachByVisitedTrackingTraversal(positions, minJump, maxJump));
}
