using DSAExperimentation.LeetCode.StoneGame;

namespace DSAExperimentation.Tests.LeetCodeCoverage.StoneGame;

// Harness only. Both strategies are StoneGameSolution's - this file just pins them
// to LeetCode's published examples plus a few more even-length pile rows.
public sealed class StoneGameTests
{
    public static TheoryData<int[], bool> Examples =>
        new()
        {
            { [5, 3, 4, 5], true },
            { [3, 7, 2, 3], true },
            { [3, 2], true },
            { [2, 3], true },
            { [1, 100, 3, 4, 5, 6], true },
        };

    [Theory]
    [MemberData(nameof(Examples))]
    public void AliceWinsByUnmemoizedRecursion_LeetCodeExamples_ReturnsWhetherAliceWins(int[] piles, bool expected) =>
        Assert.Equal(expected, StoneGameSolution.AliceWinsByUnmemoizedRecursion(piles));

    [Theory]
    [MemberData(nameof(Examples))]
    public void AliceWinsByMemoizedRecursion_LeetCodeExamples_ReturnsWhetherAliceWins(int[] piles, bool expected) =>
        Assert.Equal(expected, StoneGameSolution.AliceWinsByMemoizedRecursion(piles));
}
