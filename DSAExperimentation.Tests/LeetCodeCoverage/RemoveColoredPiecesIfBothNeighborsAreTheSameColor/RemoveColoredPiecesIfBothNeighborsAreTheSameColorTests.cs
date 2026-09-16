using DSAExperimentation.LeetCode.RemoveColoredPiecesIfBothNeighborsAreTheSameColor;

namespace DSAExperimentation.Tests.LeetCodeCoverage.RemoveColoredPiecesIfBothNeighborsAreTheSameColor;

// Harness only. Both playing the game out and counting each player's fixed move
// budget are RemoveColoredPiecesIfBothNeighborsAreTheSameColorSolution's - this file
// pins them to LeetCode's three published examples plus the cases the examples never
// reach: a run short enough to yield no moves at all, equal budgets (which Alice
// loses, because she needs a STRICTLY larger one), and a string of one color only,
// where the losing player's budget is never touched.
public sealed class RemoveColoredPiecesIfBothNeighborsAreTheSameColorTests
{
    public static TheoryData<ColoredPiecesCase> Examples =>
        new()
        {
            // LeetCode example 1: Alice's run of three yields one move, Bob's runs none.
            { new ColoredPiecesCase(Colors: "AAABABB", AliceWins: true) },

            // LeetCode example 2: neither player has an interior piece to remove, and
            // Alice, moving first, is the one who is stuck.
            { new ColoredPiecesCase(Colors: "AA", AliceWins: false) },

            // LeetCode example 3: Bob's run of seven buys him five moves to Alice's one.
            { new ColoredPiecesCase(Colors: "ABBBBBBBAAA", AliceWins: false) },

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

            // A single colour: Bob never gets a move.
            { new ColoredPiecesCase(Colors: "AAAAA", AliceWins: true) },

            // Alternating pieces: every run has length one, so nobody can ever move.
            { new ColoredPiecesCase(Colors: "ABABABAB", AliceWins: false) },
        };

    [Theory]
    [MemberData(nameof(Examples))]
    public void WinnerOfGameByGameSimulation_LeetCodeExamples_ReturnsWhetherAliceWins(
        ColoredPiecesCase example)
    {
        var actual = RemoveColoredPiecesIfBothNeighborsAreTheSameColorSolution.WinnerOfGameByGameSimulation(
            example.Colors);

        Assert.Equal(example.AliceWins, actual);
    }

    [Theory]
    [MemberData(nameof(Examples))]
    public void WinnerOfGameByRunLengthCounting_LeetCodeExamples_ReturnsWhetherAliceWins(
        ColoredPiecesCase example)
    {
        var actual = RemoveColoredPiecesIfBothNeighborsAreTheSameColorSolution.WinnerOfGameByRunLengthCounting(
            example.Colors);

        Assert.Equal(example.AliceWins, actual);
    }

    // One LeetCode example: the piece-colour string and whether Alice, moving first
    // and needing a strictly larger move budget than Bob, wins the game. The outcome
    // is named at every construction site, so a row reads as the case it is rather
    // than as a bare `true` whose meaning is its position. Nested because it is only
    // ever used inside this test class - it is this harness's own vocabulary, not a
    // type another file would import.
    public readonly record struct ColoredPiecesCase(string Colors, bool AliceWins);
}
