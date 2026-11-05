using DSAExperimentation.LeetCode.Solutions.StoneGameIV;

namespace DSAExperimentation.LeetCode.Solutions.Tests.StoneGameIV;

// Harness only: both strategies live in StoneGameIVSolution and are asserted against
// the same cases. The un-memoized recursion is the definition and the table is the
// same recurrence settled bottom-up, so pinning both to one set of cases is the check
// that the table changed nothing.
//
// The rows are read by hand: the losing counts up to 20 are 0, 2, 5, 7, 10, 12, 15, 17
// and 20 - each is losing because every square move from it lands on a winning count,
// and every other count wins by a square move onto one of them (3 - 1 = 2, 6 - 1 = 5,
// 8 - 1 = 7, 9 - 9 = 0).
//
// UpperRangeExamples sit at LC 1510's n <= 10^5, where only the table can run: the
// recursion is exponential. Their answers come from a pull-style table written
// separately in Python, not from either strategy: 10^5 wins, and 99,919 is the largest
// losing count up to 10^5. LeetCode's published examples run in CatalogCaseTests.
public sealed partial class StoneGameIVSolutionTests
{
    public static TheoryData<TakeAwaySquaresCase> Cases =>
        new()
        {
            new TakeAwaySquaresCase(6, AliceWins: true),
            new TakeAwaySquaresCase(8, AliceWins: true),
            new TakeAwaySquaresCase(9, AliceWins: true),
        };

    public static TheoryData<TakeAwaySquaresCase> UpperRangeExamples =>
        new()
        {
            new TakeAwaySquaresCase(100_000, AliceWins: true),
            new TakeAwaySquaresCase(99_919, AliceWins: false),
        };

    [Theory]
    [MemberData(nameof(Cases))]
    public void CanAliceWinByUnmemoizedRecursion_DeeperRecursion_MatchesExpectedOutcome(
        TakeAwaySquaresCase testCase) =>
        Assert.Equal(testCase.AliceWins, StoneGameIVSolution.CanAliceWinByUnmemoizedRecursion(testCase.N));

    [Theory]
    [MemberData(nameof(Cases))]
    public void CanAliceWinByBottomUpTable_DeeperRecursion_MatchesExpectedOutcome(
        TakeAwaySquaresCase testCase) =>
        Assert.Equal(testCase.AliceWins, StoneGameIVSolution.CanAliceWinByBottomUpTable(testCase.N));

    [Theory]
    [MemberData(nameof(UpperRangeExamples))]
    public void CanAliceWinByBottomUpTable_UpperRangeCounts_MatchesSeparatelyComputedOutcome(
        TakeAwaySquaresCase testCase) =>
        Assert.Equal(testCase.AliceWins, StoneGameIVSolution.CanAliceWinByBottomUpTable(testCase.N));

    // Nested because it is only ever used inside this test class and has no
    // independent identity: this harness's own vocabulary for one case.
    // The expected answer is a named field of the case rather than a bare `true` or
    // `false` sitting in the signature where only its position says what it means.
    public readonly record struct TakeAwaySquaresCase(int N, bool AliceWins);
}
