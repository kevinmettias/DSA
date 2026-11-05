using DSAExperimentation.LeetCode.Solutions.SumGame;

namespace DSAExperimentation.LeetCode.Solutions.Tests.SumGame;

// Harness only. The reduced game state is SumGameSolution's own SumGameState and
// all three strategies are its methods - this file pins them to four boards the
// original test did not cover: the smallest even board, a board whose single blank
// hands Alice the last move, a mirrored board Bob can always level, and a board where
// both blanks sit on one half so Bob never gets to answer them. The brute-force arm
// is asserted here for the first time; it used to live in the benchmark as an
// unasserted baseline. LeetCode's published examples run in CatalogCaseTests.
public sealed partial class SumGameSolutionTests
{
    public static TheoryData<SumGameCase> Cases =>
        new()
        {
            // Smallest even board: Alice writes a digit, Bob mirrors it.
            { new SumGameCase(Num: "??", AliceWins: false) },

            // One blank, so Alice moves last and any non-zero digit wins.
            { new SumGameCase(Num: "?0", AliceWins: true) },

            // Known digits already level and one blank per half: Bob mirrors again.
            { new SumGameCase(Num: "9??9", AliceWins: false) },

            // Both blanks on the left, so Bob has to answer on the same half Alice
            // just moved on and can never restore the balance.
            { new SumGameCase(Num: "??00", AliceWins: true) },
        };

    [Theory]
    [MemberData(nameof(Cases))]
    public void CanAliceWinByBruteForceRecursion_Cases_ReturnsOptimalPlayOutcome(
        SumGameCase testCase) =>
        Assert.Equal(testCase.AliceWins, SumGameSolution.CanAliceWinByBruteForceRecursion(testCase.Num));

    [Theory]
    [MemberData(nameof(Cases))]
    public void CanAliceWinByMemoizedRecursion_Cases_ReturnsOptimalPlayOutcome(
        SumGameCase testCase) =>
        Assert.Equal(testCase.AliceWins, SumGameSolution.CanAliceWinByMemoizedRecursion(testCase.Num));

    [Theory]
    [MemberData(nameof(Cases))]
    public void CanAliceWinByClosedForm_Cases_ReturnsOptimalPlayOutcome(
        SumGameCase testCase) =>
        Assert.Equal(testCase.AliceWins, SumGameSolution.CanAliceWinByClosedForm(testCase.Num));

    // One case: the board, and whether Alice wins optimal play on it.
    // Whether Alice wins is named at the row that states it, so a reader of `Cases`
    // never has to remember which position `true` sits in.
    public readonly record struct SumGameCase(string Num, bool AliceWins);
}
