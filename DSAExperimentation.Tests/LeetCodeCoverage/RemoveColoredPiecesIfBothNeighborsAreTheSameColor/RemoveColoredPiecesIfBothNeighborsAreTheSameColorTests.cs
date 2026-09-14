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
    public static TheoryData<string, bool> Examples =>
        new()
        {
            // LeetCode example 1: Alice's run of three yields one move, Bob's runs none.
            { "AAABABB", true },

            // LeetCode example 2: neither player has an interior piece to remove, and
            // Alice, moving first, is the one who is stuck.
            { "AA", false },

            // LeetCode example 3: Bob's run of seven buys him five moves to Alice's one.
            { "ABBBBBBBAAA", false },

            // The smallest winning position: one removable 'A' and nothing for Bob.
            { "AAA", true },

            // Its mirror - Alice cannot move at all on turn one.
            { "BBB", false },

            // Equal budgets: Alice runs out first, so a tie is a loss for her.
            { "AAABBB", false },

            // One more 'A' than that tips it.
            { "AAAABBB", true },

            // Two pieces of different colors: no interior position exists.
            { "AB", false },

            // Bob out-budgets Alice two to three.
            { "AAAABBBBB", false },

            // A single colour: Bob never gets a move.
            { "AAAAA", true },

            // Alternating pieces: every run has length one, so nobody can ever move.
            { "ABABABAB", false },
        };

    [Theory]
    [MemberData(nameof(Examples))]
    public void WinnerOfGameByGameSimulation_LeetCodeExamples_ReturnsWhetherAliceWins(
        string colors, bool expected) =>
        Assert.Equal(
            expected,
            RemoveColoredPiecesIfBothNeighborsAreTheSameColorSolution.WinnerOfGameByGameSimulation(colors));

    [Theory]
    [MemberData(nameof(Examples))]
    public void WinnerOfGameByRunLengthCounting_LeetCodeExamples_ReturnsWhetherAliceWins(
        string colors, bool expected) =>
        Assert.Equal(
            expected,
            RemoveColoredPiecesIfBothNeighborsAreTheSameColorSolution.WinnerOfGameByRunLengthCounting(colors));
}
