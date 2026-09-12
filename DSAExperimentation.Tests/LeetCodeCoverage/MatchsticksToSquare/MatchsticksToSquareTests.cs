using DSAExperimentation.LeetCode.MatchsticksToSquare;

namespace DSAExperimentation.Tests.LeetCodeCoverage.MatchsticksToSquare;

// Harness only. Both strategies are MatchsticksToSquareSolution's - this file
// just pins them to LeetCode's published examples.
public sealed class MatchsticksToSquareTests
{
    public static TheoryData<int[], bool> Examples =>
        new()
        {
            { [1, 1, 2, 2, 2], true },
            { [3, 3, 3, 3, 4], false },
            { [1, 1, 1], false },
        };

    [Theory]
    [MemberData(nameof(Examples))]
    public void CanMakeSquareByNaiveBacktracking_LeetCodeExamples_ReturnsWhetherFourEqualSidesExist(
        int[] matchsticks, bool expected) =>
        Assert.Equal(expected, MatchsticksToSquareSolution.CanMakeSquareByNaiveBacktracking(matchsticks));

    [Theory]
    [MemberData(nameof(Examples))]
    public void CanMakeSquareByGenericBacktrack_LeetCodeExamples_ReturnsWhetherFourEqualSidesExist(
        int[] matchsticks, bool expected) =>
        Assert.Equal(expected, MatchsticksToSquareSolution.CanMakeSquareByGenericBacktrack(matchsticks));
}
