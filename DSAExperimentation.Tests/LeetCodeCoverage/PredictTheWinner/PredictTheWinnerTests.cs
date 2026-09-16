using DSAExperimentation.LeetCode.PredictTheWinner;

namespace DSAExperimentation.Tests.LeetCodeCoverage.PredictTheWinner;

// Harness only. Both strategies are PredictTheWinnerSolution's - this file just pins
// them to LeetCode's published examples.
public sealed class PredictTheWinnerTests
{
    public static TheoryData<PredictTheWinnerCase> Examples =>
        new()
        {
            { new PredictTheWinnerCase(Nums: [1, 5, 2], Expected: false) },
            { new PredictTheWinnerCase(Nums: [1, 5, 233, 7], Expected: true) },
            { new PredictTheWinnerCase(Nums: [1], Expected: true) },
        };

    [Theory]
    [MemberData(nameof(Examples))]
    public void CanWinByUnmemoizedRecursion_LeetCodeExamples_ReturnsWhetherPlayerOneCanWinOrTie(
        PredictTheWinnerCase example) =>
        Assert.Equal(example.Expected, PredictTheWinnerSolution.CanWinByUnmemoizedRecursion(example.Nums));

    [Theory]
    [MemberData(nameof(Examples))]
    public void CanWinByMemoizedRecursion_LeetCodeExamples_ReturnsWhetherPlayerOneCanWinOrTie(
        PredictTheWinnerCase example) =>
        Assert.Equal(example.Expected, PredictTheWinnerSolution.CanWinByMemoizedRecursion(example.Nums));

    // One LeetCode example: the score values and whether player one can win or tie.
    // The expected value is named at every construction site, so a row reads as the
    // case it is rather than as a bare `true` whose meaning is its position. Nested
    // because it is only ever used inside this test class - it is this harness's own
    // vocabulary, not a type another file would import.
    public readonly record struct PredictTheWinnerCase(int[] Nums, bool Expected);
}
