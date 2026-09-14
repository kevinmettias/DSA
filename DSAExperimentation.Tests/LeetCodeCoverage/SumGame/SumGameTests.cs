using DSAExperimentation.LeetCode.SumGame;

namespace DSAExperimentation.Tests.LeetCodeCoverage.SumGame;

// Harness only. The reduced game state is SumGameSolution's own SumGameState and
// all three strategies are its methods - this file pins them to LeetCode's three
// published examples plus four the original test did not cover: the smallest even
// board, a board whose single blank hands Alice the last move, a mirrored board
// Bob can always level, and a board where both blanks sit on one half so Bob never
// gets to answer them. The brute-force arm is asserted here for the first time; it
// used to live in the benchmark as an unasserted baseline.
public sealed class SumGameTests
{
    public static TheoryData<string, bool> Examples =>
        new()
        {
            // LC example 1: no blanks, halves already level, so Bob has won.
            { "5023", false },

            // LC example 2: two blanks on the right, left is 7 ahead, and Bob can
            // only level a two-blank half by 9.
            { "25??", true },

            // LC example 3: difference 9 with one blank left and three right -
            // exactly the margin Bob's replies are worth.
            { "?3295???", false },

            // Smallest even board: Alice writes a digit, Bob mirrors it.
            { "??", false },

            // One blank, so Alice moves last and any non-zero digit wins.
            { "?0", true },

            // Known digits already level and one blank per half: Bob mirrors again.
            { "9??9", false },

            // Both blanks on the left, so Bob has to answer on the same half Alice
            // just moved on and can never restore the balance.
            { "??00", true },
        };

    [Theory]
    [MemberData(nameof(Examples))]
    public void AliceWinsByBruteForceRecursion_LeetCodeExamples_ReturnsOptimalPlayOutcome(
        string num, bool expected) =>
        Assert.Equal(expected, SumGameSolution.AliceWinsByBruteForceRecursion(num));

    [Theory]
    [MemberData(nameof(Examples))]
    public void AliceWinsByMemoizedRecursion_LeetCodeExamples_ReturnsOptimalPlayOutcome(
        string num, bool expected) =>
        Assert.Equal(expected, SumGameSolution.AliceWinsByMemoizedRecursion(num));

    [Theory]
    [MemberData(nameof(Examples))]
    public void AliceWinsByClosedForm_LeetCodeExamples_ReturnsOptimalPlayOutcome(
        string num, bool expected) =>
        Assert.Equal(expected, SumGameSolution.AliceWinsByClosedForm(num));
}
