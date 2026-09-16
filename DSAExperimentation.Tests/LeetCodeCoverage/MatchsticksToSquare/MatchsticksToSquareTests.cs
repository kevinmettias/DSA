using DSAExperimentation.LeetCode.MatchsticksToSquare;

namespace DSAExperimentation.Tests.LeetCodeCoverage.MatchsticksToSquare;

// Harness only. Both strategies are MatchsticksToSquareSolution's - this file
// just pins them to LeetCode's published examples.
public sealed partial class MatchsticksToSquareTests
{
    public static TheoryData<SquareExample> Examples =>
        new()
        {
            { new SquareExample(Matchsticks: [1, 1, 2, 2, 2], Expected: true) },
            { new SquareExample(Matchsticks: [3, 3, 3, 3, 4], Expected: false) },
            { new SquareExample(Matchsticks: [1, 1, 1], Expected: false) },
        };

    [Theory]
    [MemberData(nameof(Examples))]
    public void CanMakeSquareByNaiveBacktracking_LeetCodeExamples_ReturnsWhetherFourEqualSidesExist(
        SquareExample example) =>
        Assert.Equal(example.Expected, MatchsticksToSquareSolution.CanMakeSquareByNaiveBacktracking(example.Matchsticks));

    [Theory]
    [MemberData(nameof(Examples))]
    public void CanMakeSquareByGenericBacktrack_LeetCodeExamples_ReturnsWhetherFourEqualSidesExist(
        SquareExample example) =>
        Assert.Equal(example.Expected, MatchsticksToSquareSolution.CanMakeSquareByGenericBacktrack(example.Matchsticks));

    // One example as one argument. The expected answer is a bool, and a bare `true` or
    // `false` sitting second in a row does not say what it is a verdict on; naming the
    // field at each row below does.
    public readonly record struct SquareExample(int[] Matchsticks, bool Expected);
}
