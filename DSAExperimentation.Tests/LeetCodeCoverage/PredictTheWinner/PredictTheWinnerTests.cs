using DSAExperimentation.LeetCode.PredictTheWinner;

namespace DSAExperimentation.Tests.LeetCodeCoverage.PredictTheWinner;

// Harness only. Both strategies are PredictTheWinnerSolution's - this file just pins
// them to LeetCode's published examples.
public sealed class PredictTheWinnerTests
{
    public static TheoryData<int[], bool> Examples =>
        new()
        {
            { [1, 5, 2], false },
            { [1, 5, 233, 7], true },
            { [1], true },
        };

    [Theory]
    [MemberData(nameof(Examples))]
    public void CanWinByUnmemoizedRecursion_LeetCodeExamples_ReturnsWhetherPlayerOneCanWinOrTie(int[] nums, bool expected) =>
        Assert.Equal(expected, PredictTheWinnerSolution.CanWinByUnmemoizedRecursion(nums));

    [Theory]
    [MemberData(nameof(Examples))]
    public void CanWinByMemoizedRecursion_LeetCodeExamples_ReturnsWhetherPlayerOneCanWinOrTie(int[] nums, bool expected) =>
        Assert.Equal(expected, PredictTheWinnerSolution.CanWinByMemoizedRecursion(nums));
}
