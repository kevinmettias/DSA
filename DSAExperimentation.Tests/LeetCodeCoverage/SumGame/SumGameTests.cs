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
    public static TheoryData<SumGameExample> Examples =>
        new()
        {
            // LC example 1: no blanks, halves already level, so Bob has won.
            { new SumGameExample(Num: "5023", AliceWins: false) },

            // LC example 2: two blanks on the right, left is 7 ahead, and Bob can
            // only level a two-blank half by 9.
            { new SumGameExample(Num: "25??", AliceWins: true) },

            // LC example 3: difference 9 with one blank left and three right -
            // exactly the margin Bob's replies are worth.
            { new SumGameExample(Num: "?3295???", AliceWins: false) },

            // Smallest even board: Alice writes a digit, Bob mirrors it.
            { new SumGameExample(Num: "??", AliceWins: false) },

            // One blank, so Alice moves last and any non-zero digit wins.
            { new SumGameExample(Num: "?0", AliceWins: true) },

            // Known digits already level and one blank per half: Bob mirrors again.
            { new SumGameExample(Num: "9??9", AliceWins: false) },

            // Both blanks on the left, so Bob has to answer on the same half Alice
            // just moved on and can never restore the balance.
            { new SumGameExample(Num: "??00", AliceWins: true) },
        };

    [Theory]
    [MemberData(nameof(Examples))]
    public void CanAliceWinByBruteForceRecursion_LeetCodeExamples_ReturnsOptimalPlayOutcome(
        SumGameExample example) =>
        Assert.Equal(example.AliceWins, SumGameSolution.CanAliceWinByBruteForceRecursion(example.Num));

    [Theory]
    [MemberData(nameof(Examples))]
    public void CanAliceWinByMemoizedRecursion_LeetCodeExamples_ReturnsOptimalPlayOutcome(
        SumGameExample example) =>
        Assert.Equal(example.AliceWins, SumGameSolution.CanAliceWinByMemoizedRecursion(example.Num));

    [Theory]
    [MemberData(nameof(Examples))]
    public void CanAliceWinByClosedForm_LeetCodeExamples_ReturnsOptimalPlayOutcome(
        SumGameExample example) =>
        Assert.Equal(example.AliceWins, SumGameSolution.CanAliceWinByClosedForm(example.Num));

    // One LeetCode example: the board, and whether Alice wins optimal play on it.
    // Whether Alice wins is named at the row that states it, so a reader of `Examples`
    // never has to remember which position `true` sits in.
    public readonly record struct SumGameExample(string Num, bool AliceWins);
}
