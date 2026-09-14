using DSAExperimentation.LeetCode.StoneGameIX;

namespace DSAExperimentation.Tests.LeetCodeCoverage.StoneGameIX;

// Harness only: both strategies live in StoneGameIXSolution and are asserted against
// the same examples, so the exhaustive game-tree search - which used to exist only as
// this problem's benchmark baseline, measured but never asserted - now has to agree
// with the closed-form parity rule on every case.
//
// LeetCode's three published examples are joined by four that pin the rule's branches:
// [1,1] (no remainder-2 stone at all), [3,3,3] (nothing but remainder-0 stones, so
// Alice busts on her first move), [1,1,2] (even remainder-0 count with both other
// buckets non-empty - Alice wins by opening with the remainder-2 stone), and [3,1,1,1]
// (odd remainder-0 count carried by an imbalance of exactly 3).
public sealed class StoneGameIXTests
{
    public static TheoryData<int[], bool> Examples =>
        new()
        {
            { [2, 1], true },
            { [2], false },
            { [5, 1, 2, 4, 3], false },
            { [1, 1], false },
            { [3, 3, 3], false },
            { [1, 1, 2], true },
            { [3, 1, 1, 1], true },
        };

    [Theory]
    [MemberData(nameof(Examples))]
    public void AliceWinsByGameTreeMinimax_LeetCodeExamples_MatchesExpectedOutcome(
        int[] stones, bool expected) =>
        Assert.Equal(expected, StoneGameIXSolution.AliceWinsByGameTreeMinimax(stones));

    [Theory]
    [MemberData(nameof(Examples))]
    public void AliceWinsByClosedFormCounting_LeetCodeExamples_MatchesExpectedOutcome(
        int[] stones, bool expected) =>
        Assert.Equal(expected, StoneGameIXSolution.AliceWinsByClosedFormCounting(stones));
}
