using DSAExperimentation.LeetCode.Solutions.StoneGameIX;

namespace DSAExperimentation.LeetCode.Solutions.Tests.StoneGameIX;

// Harness only: both strategies live in StoneGameIXSolution and are asserted against
// the same cases, so the exhaustive game-tree search - which used to exist only as
// this problem's benchmark baseline, measured but never asserted - now has to agree
// with the closed-form parity rule on every case.
//
// Three cases pin the rule's branches: [1,1] (no remainder-2 stone at all), [1,1,2]
// (even remainder-0 count with both other buckets non-empty - Alice wins by opening
// with the remainder-2 stone), and [3,1,1,1] (odd remainder-0 count carried by an
// imbalance of exactly 3). LeetCode's published examples run in CatalogCaseTests.
public sealed partial class StoneGameIXSolutionTests
{
    public static TheoryData<RemainderGameCase> Cases =>
        new()
        {
            new RemainderGameCase([1, 1], AliceWins: false),
            new RemainderGameCase([1, 1, 2], AliceWins: true),
            new RemainderGameCase([3, 1, 1, 1], AliceWins: true),
        };

    [Theory]
    [MemberData(nameof(Cases))]
    public void CanAliceWinByGameTreeMinimax_Cases_MatchesExpectedOutcome(RemainderGameCase testCase) =>
        Assert.Equal(testCase.AliceWins, StoneGameIXSolution.CanAliceWinByGameTreeMinimax(testCase.Stones));

    [Theory]
    [MemberData(nameof(Cases))]
    public void CanAliceWinByClosedFormCounting_Cases_MatchesExpectedOutcome(RemainderGameCase testCase) =>
        Assert.Equal(testCase.AliceWins, StoneGameIXSolution.CanAliceWinByClosedFormCounting(testCase.Stones));

    // Nested because it is only ever used inside this test class and has no
    // independent identity: this harness's own vocabulary for one case.
    // The expected answer is a named field of the case rather than a bare `true` or
    // `false` sitting in the signature where only its position says what it means.
    public readonly record struct RemainderGameCase(int[] Stones, bool AliceWins);
}
