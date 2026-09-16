using DSAExperimentation.LeetCode.FindTheWinningPlayerInCoinGame;

namespace DSAExperimentation.Tests.LeetCodeCoverage.FindTheWinningPlayerInCoinGame;

// Harness only. Both strategies are FindTheWinningPlayerInCoinGameSolution's -
// this file pins them to LeetCode's published examples.
public sealed class FindTheWinningPlayerInCoinGameTests
{
    public static TheoryData<int, int, string> Examples =>
        new()
        {
            { 2, 7, "Alice" },
            { 4, 11, "Bob" },
        };

    [Theory]
    [MemberData(nameof(Examples))]
    public void WinningPlayerBySimulation_LeetCodeExamples_ReturnsPlayerWhoTakesTheLastTurn(
        int x, int y, string expected)
    {
        var actual = FindTheWinningPlayerInCoinGameSolution.WinningPlayerBySimulation(x, y);

        Assert.Equal(expected, actual);
    }

    [Theory]
    [MemberData(nameof(Examples))]
    public void WinningPlayerByTurnParity_LeetCodeExamples_ReturnsPlayerWhoTakesTheLastTurn(
        int x, int y, string expected)
    {
        var actual = FindTheWinningPlayerInCoinGameSolution.WinningPlayerByTurnParity(x, y);

        Assert.Equal(expected, actual);
    }
}
