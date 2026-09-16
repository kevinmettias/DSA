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
public sealed partial class StoneGameIXTests
{
    public static TheoryData<RemainderGameExample> Examples =>
        new()
        {
            new RemainderGameExample([2, 1], AliceWins: true),
            new RemainderGameExample([2], AliceWins: false),
            new RemainderGameExample([5, 1, 2, 4, 3], AliceWins: false),
            new RemainderGameExample([1, 1], AliceWins: false),
            new RemainderGameExample([3, 3, 3], AliceWins: false),
            new RemainderGameExample([1, 1, 2], AliceWins: true),
            new RemainderGameExample([3, 1, 1, 1], AliceWins: true),
        };

    [Theory]
    [MemberData(nameof(Examples))]
    public void CanAliceWinByGameTreeMinimax_LeetCodeExamples_MatchesExpectedOutcome(RemainderGameExample example) =>
        Assert.Equal(example.AliceWins, StoneGameIXSolution.CanAliceWinByGameTreeMinimax(example.Stones));

    [Theory]
    [MemberData(nameof(Examples))]
    public void CanAliceWinByClosedFormCounting_LeetCodeExamples_MatchesExpectedOutcome(RemainderGameExample example) =>
        Assert.Equal(example.AliceWins, StoneGameIXSolution.CanAliceWinByClosedFormCounting(example.Stones));

    // Nested because it is only ever used inside this test class and has no
    // independent identity: this harness's own vocabulary for one LeetCode example.
    // The expected answer is a named field of the case rather than a bare `true` or
    // `false` sitting in the signature where only its position says what it means.
    public readonly record struct RemainderGameExample(int[] Stones, bool AliceWins);
}
