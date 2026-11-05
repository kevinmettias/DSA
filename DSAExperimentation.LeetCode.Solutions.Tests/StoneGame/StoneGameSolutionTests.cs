using DSAExperimentation.LeetCode.Solutions.StoneGame;

namespace DSAExperimentation.LeetCode.Solutions.Tests.StoneGame;

// Harness only. Both strategies are StoneGameSolution's - this file pins them to
// three even-length pile rows: a two-pile game in both orders and a six-pile game.
// LeetCode's published examples run in CatalogCaseTests.
public sealed partial class StoneGameSolutionTests
{
    public static TheoryData<PileGameCase> Cases =>
        new()
        {
            new PileGameCase([3, 2], AliceWins: true),
            new PileGameCase([2, 3], AliceWins: true),
            new PileGameCase([1, 100, 3, 4, 5, 6], AliceWins: true),
        };

    [Theory]
    [MemberData(nameof(Cases))]
    public void CanAliceWinByUnmemoizedRecursion_Cases_ReturnsWhetherAliceWins(PileGameCase testCase) =>
        Assert.Equal(testCase.AliceWins, StoneGameSolution.CanAliceWinByUnmemoizedRecursion(testCase.Piles));

    [Theory]
    [MemberData(nameof(Cases))]
    public void CanAliceWinByMemoizedRecursion_Cases_ReturnsWhetherAliceWins(PileGameCase testCase) =>
        Assert.Equal(testCase.AliceWins, StoneGameSolution.CanAliceWinByMemoizedRecursion(testCase.Piles));

    // Nested because it is only ever used inside this test class and has no
    // independent identity: this harness's own vocabulary for one case.
    // The expected answer is a named field of the case rather than a bare `true` or
    // `false` sitting in the signature where only its position says what it means.
    public readonly record struct PileGameCase(int[] Piles, bool AliceWins);
}
