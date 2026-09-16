using DSAExperimentation.LeetCode.ChalkboardXorGame;

namespace DSAExperimentation.Tests.LeetCodeCoverage.ChalkboardXorGame;

// Harness only. All three strategies are ChalkboardXorGameSolution's - LeetCode's
// published examples (plus single-element and already-zero-xor boundaries) are
// stated once and replayed against each, so a failure names the strategy that broke
// rather than reporting a disagreement between an anonymous test helper and an
// anonymous benchmark arm. The closed form in particular was never asserted before:
// it existed only as a benchmark arm.
public sealed partial class ChalkboardXorGameTests
{
    public static TheoryData<GameExample> Examples =>
        new()
        {
            { new GameExample(Nums: [1, 1, 2], AliceWins: false) },
            { new GameExample(Nums: [0, 1], AliceWins: true) },
            { new GameExample(Nums: [1, 2, 3], AliceWins: true) },
            { new GameExample(Nums: [0], AliceWins: true) },
            { new GameExample(Nums: [2], AliceWins: false) },
            { new GameExample(Nums: [1, 2], AliceWins: true) },
            { new GameExample(Nums: [3, 3, 3], AliceWins: false) },
            { new GameExample(Nums: [1, 2, 3, 4], AliceWins: true) },
        };

    [Theory]
    [MemberData(nameof(Examples))]
    public void CanAliceWinByBruteForceRecursion_LeetCodeExamples_ReturnsWhetherAliceWins(GameExample example) =>
        Assert.Equal(example.AliceWins, ChalkboardXorGameSolution.CanAliceWinByBruteForceRecursion(example.Nums));

    [Theory]
    [MemberData(nameof(Examples))]
    public void CanAliceWinByMemoizedRecursion_LeetCodeExamples_ReturnsWhetherAliceWins(GameExample example) =>
        Assert.Equal(example.AliceWins, ChalkboardXorGameSolution.CanAliceWinByMemoizedRecursion(example.Nums));

    [Theory]
    [MemberData(nameof(Examples))]
    public void CanAliceWinByXorParityFormula_LeetCodeExamples_ReturnsWhetherAliceWins(GameExample example) =>
        Assert.Equal(example.AliceWins, ChalkboardXorGameSolution.CanAliceWinByXorParityFormula(example.Nums));

    // One LeetCode example: the chalkboard and whether Alice wins from it. The win
    // flag is named at the row that states it, so a reader of `Examples` never has to
    // remember which position `false` sits in.
    public readonly record struct GameExample(int[] Nums, bool AliceWins);
}
