using DSAExperimentation.LeetCode.StoneGameIV;

namespace DSAExperimentation.LeetCode.Tests.StoneGameIV;

// Harness only: both strategies live in StoneGameIVSolution and are asserted against
// the same examples. The un-memoized recursion is the definition and the table is the
// same recurrence settled bottom-up, so pinning both to one example set is the check
// that the table changed nothing.
//
// The first three rows are LeetCode's published examples. The rest are read by hand:
// the losing counts up to 20 are 0, 2, 5, 7, 10, 12, 15, 17 and 20 - each is losing
// because every square move from it lands on a winning count, and every other count
// wins by a square move onto one of them (3 - 1 = 2, 6 - 1 = 5, 8 - 1 = 7, 9 - 9 = 0).
// 17 loses because 16, 13 (13 - 1 = 12, a loss), 8 and 1 all win.
//
// UpperRangeExamples sit at LC 1510's n <= 10^5, where only the table can run: the
// recursion is exponential. Their answers come from a pull-style table written
// separately in Python, not from either strategy: 10^5 wins, and 99,919 is the largest
// losing count up to 10^5.
public sealed partial class StoneGameIVSolutionTests
{
    public static TheoryData<TakeAwaySquaresExample> Examples =>
        new()
        {
            new TakeAwaySquaresExample(1, AliceWins: true),
            new TakeAwaySquaresExample(2, AliceWins: false),
            new TakeAwaySquaresExample(4, AliceWins: true),
            new TakeAwaySquaresExample(3, AliceWins: true),
            new TakeAwaySquaresExample(5, AliceWins: false),
            new TakeAwaySquaresExample(6, AliceWins: true),
            new TakeAwaySquaresExample(7, AliceWins: false),
            new TakeAwaySquaresExample(8, AliceWins: true),
            new TakeAwaySquaresExample(9, AliceWins: true),
            new TakeAwaySquaresExample(10, AliceWins: false),
            new TakeAwaySquaresExample(17, AliceWins: false),
            new TakeAwaySquaresExample(20, AliceWins: false),
        };

    public static TheoryData<TakeAwaySquaresExample> UpperRangeExamples =>
        new()
        {
            new TakeAwaySquaresExample(100_000, AliceWins: true),
            new TakeAwaySquaresExample(99_919, AliceWins: false),
        };

    [Theory]
    [MemberData(nameof(Examples))]
    public void CanAliceWinByUnmemoizedRecursion_LeetCodeExamplesAndDeeperRecursion_MatchesExpectedOutcome(
        TakeAwaySquaresExample example) =>
        Assert.Equal(example.AliceWins, StoneGameIVSolution.CanAliceWinByUnmemoizedRecursion(example.N));

    [Theory]
    [MemberData(nameof(Examples))]
    public void CanAliceWinByBottomUpTable_LeetCodeExamplesAndDeeperRecursion_MatchesExpectedOutcome(
        TakeAwaySquaresExample example) =>
        Assert.Equal(example.AliceWins, StoneGameIVSolution.CanAliceWinByBottomUpTable(example.N));

    [Theory]
    [MemberData(nameof(UpperRangeExamples))]
    public void CanAliceWinByBottomUpTable_UpperRangeCounts_MatchesSeparatelyComputedOutcome(
        TakeAwaySquaresExample example) =>
        Assert.Equal(example.AliceWins, StoneGameIVSolution.CanAliceWinByBottomUpTable(example.N));

    // Nested because it is only ever used inside this test class and has no
    // independent identity: this harness's own vocabulary for one LeetCode example.
    // The expected answer is a named field of the case rather than a bare `true` or
    // `false` sitting in the signature where only its position says what it means.
    public readonly record struct TakeAwaySquaresExample(int N, bool AliceWins);
}
