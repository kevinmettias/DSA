using DSAExperimentation.LeetCode.Solutions.JumpGameVII;

namespace DSAExperimentation.LeetCode.Solutions.Tests.JumpGameVII;

// Harness only. Both strategies are JumpGameVIISolution's - the cases are stated
// once and replayed against each, so a failure names the strategy that broke
// rather than reporting a disagreement between an anonymous test helper and an
// anonymous benchmark arm. The cases cover how a jump chain meets the last index:
// a one-step finish, a wall on the last index, a window that straddles the end,
// and a wall that cuts the only chain. The unmemoized baseline was never asserted
// before this migration; every case here is small enough that its reconverging
// jump chains stay cheap. LeetCode's published examples run in CatalogCaseTests.
public sealed partial class JumpGameVIISolutionTests
{
    public static TheoryData<JumpWindowCase> Cases =>
        new()
        {
            // The last index is reachable in one step, and is the start's only move.
            { new JumpWindowCase(Positions: "00", MinJump: 1, MaxJump: 1, Expected: true) },

            // The last index is a wall, so it can never be landed on.
            { new JumpWindowCase(Positions: "0001", MinJump: 1, MaxJump: 2, Expected: false) },

            // Every index is open and the window straddles the end.
            { new JumpWindowCase(Positions: "0000", MinJump: 2, MaxJump: 3, Expected: true) },

            // A wall inside a width-one window cuts the only chain at index 2.
            { new JumpWindowCase(Positions: "0010", MinJump: 1, MaxJump: 1, Expected: false) },
        };

    [Theory]
    [MemberData(nameof(Cases))]
    public void CanReachByUnmemoizedRecursion_Cases_ReportsWhetherTheLastIndexIsReachable(
        JumpWindowCase testCase)
    {
        var actual = JumpGameVIISolution.CanReachByUnmemoizedRecursion(
            testCase.Positions, testCase.MinJump, testCase.MaxJump);

        Assert.Equal(testCase.Expected, actual);
    }

    [Theory]
    [MemberData(nameof(Cases))]
    public void CanReachByVisitedTrackingTraversal_Cases_ReportsWhetherTheLastIndexIsReachable(
        JumpWindowCase testCase)
    {
        var actual = JumpGameVIISolution.CanReachByVisitedTrackingTraversal(
            testCase.Positions, testCase.MinJump, testCase.MaxJump);

        Assert.Equal(testCase.Expected, actual);
    }

    // One case: the '0'/'1' positions, the jump window's bounds, and whether
    // the last index can be reached. The `bool` is the expected answer rather than a
    // mode, so the row names it instead of leaving a bare `true` in a position to decode.
    public readonly record struct JumpWindowCase(
        string Positions, int MinJump, int MaxJump, bool Expected);
}
