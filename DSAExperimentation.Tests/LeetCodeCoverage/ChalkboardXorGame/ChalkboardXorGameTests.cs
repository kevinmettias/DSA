using DSAExperimentation.LeetCode.ChalkboardXorGame;

namespace DSAExperimentation.Tests.LeetCodeCoverage.ChalkboardXorGame;

// Harness only. All three strategies are ChalkboardXorGameSolution's - LeetCode's
// published examples (plus single-element and already-zero-xor boundaries) are
// stated once and replayed against each, so a failure names the strategy that broke
// rather than reporting a disagreement between an anonymous test helper and an
// anonymous benchmark arm. The closed form in particular was never asserted before:
// it existed only as a benchmark arm.
public sealed class ChalkboardXorGameTests
{
    public static TheoryData<int[], bool> Examples =>
        new()
        {
            { [1, 1, 2], false },
            { [0, 1], true },
            { [1, 2, 3], true },
            { [0], true },
            { [2], false },
            { [1, 2], true },
            { [3, 3, 3], false },
            { [1, 2, 3, 4], true },
        };

    [Theory]
    [MemberData(nameof(Examples))]
    public void AliceWinsByBruteForceRecursion_LeetCodeExamples_ReturnsWhetherAliceWins(int[] nums, bool expected) =>
        Assert.Equal(expected, ChalkboardXorGameSolution.AliceWinsByBruteForceRecursion(nums));

    [Theory]
    [MemberData(nameof(Examples))]
    public void AliceWinsByMemoizedRecursion_LeetCodeExamples_ReturnsWhetherAliceWins(int[] nums, bool expected) =>
        Assert.Equal(expected, ChalkboardXorGameSolution.AliceWinsByMemoizedRecursion(nums));

    [Theory]
    [MemberData(nameof(Examples))]
    public void AliceWinsByXorParityFormula_LeetCodeExamples_ReturnsWhetherAliceWins(int[] nums, bool expected) =>
        Assert.Equal(expected, ChalkboardXorGameSolution.AliceWinsByXorParityFormula(nums));
}
