using DSAExperimentation.LeetCode.Solutions.RemoveColoredPiecesIfBothNeighborsAreTheSameColor;

namespace DSAExperimentation.LeetCode.Solutions.Tests.RemoveColoredPiecesIfBothNeighborsAreTheSameColor;

// Harness only. Both playing the game out and counting each player's fixed move
// budget are RemoveColoredPiecesIfBothNeighborsAreTheSameColorSolution's - this file
// pins them to cases LeetCode's published examples never reach: a run short enough
// to yield no moves at all, equal budgets (which Alice loses, because she needs a
// STRICTLY larger one), and a string of one color only, where the losing player's
// budget is never touched. LeetCode's published examples run in CatalogCaseTests.
public sealed partial class RemoveColoredPiecesIfBothNeighborsAreTheSameColorSolutionTests
{
    public static TheoryData<ColoredPiecesCase> Cases =>
        new()
        {
            // The smallest winning position: one removable 'A' and nothing for Bob.
            { new ColoredPiecesCase(Colors: "AAA", AliceWins: true) },

            // Its mirror - Alice cannot move at all on turn one.
            { new ColoredPiecesCase(Colors: "BBB", AliceWins: false) },

            // Equal budgets: Alice runs out first, so a tie is a loss for her.
            { new ColoredPiecesCase(Colors: "AAABBB", AliceWins: false) },

            // One more 'A' than that tips it.
            { new ColoredPiecesCase(Colors: "AAAABBB", AliceWins: true) },

            // Two pieces of different colors: no interior position exists.
            { new ColoredPiecesCase(Colors: "AB", AliceWins: false) },

            // Bob out-budgets Alice two to three.
            { new ColoredPiecesCase(Colors: "AAAABBBBB", AliceWins: false) },
        };

    [Theory]
    [MemberData(nameof(Cases))]
    public void CanAliceWinByGameSimulation_Cases_ReturnsWhetherAliceWins(
        ColoredPiecesCase testCase)
    {
        var actual = RemoveColoredPiecesIfBothNeighborsAreTheSameColorSolution.CanAliceWinByGameSimulation(
            testCase.Colors);

        Assert.Equal(testCase.AliceWins, actual);
    }

    [Theory]
    [MemberData(nameof(Cases))]
    public void CanAliceWinByRunLengthCounting_Cases_ReturnsWhetherAliceWins(
        ColoredPiecesCase testCase)
    {
        var actual = RemoveColoredPiecesIfBothNeighborsAreTheSameColorSolution.CanAliceWinByRunLengthCounting(
            testCase.Colors);

        Assert.Equal(testCase.AliceWins, actual);
    }

    // One case: the piece-colour string and whether Alice, moving first
    // and needing a strictly larger move budget than Bob, wins the game. The outcome
    // is named at every construction site, so a row reads as the case it is rather
    // than as a bare `true` whose meaning is its position. Nested because it is only
    // ever used inside this test class - it is this harness's own vocabulary, not a
    // type another file would import.
    public readonly record struct ColoredPiecesCase(string Colors, bool AliceWins);
}
