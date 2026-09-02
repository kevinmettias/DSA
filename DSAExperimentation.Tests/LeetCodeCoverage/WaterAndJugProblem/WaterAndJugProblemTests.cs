using DSAExperimentation.LeetCode.WaterAndJugProblem;

namespace DSAExperimentation.Tests.LeetCodeCoverage.WaterAndJugProblem;

// Harness only. All three strategies live in WaterAndJugProblemSolution; this
// file just pins them to LeetCode's published examples plus a boundary case
// where the target exceeds the combined capacity.
public sealed class WaterAndJugProblemTests
{
    public static TheoryData<int, int, int, bool> Examples =>
        new()
        {
            { 3, 5, 4, true },
            { 2, 6, 5, false },
            { 1, 2, 3, true },
            { 2, 3, 0, true },
            { 1, 2, 4, false },
        };

    [Theory]
    [MemberData(nameof(Examples))]
    public void CanMeasureWaterByStackSearch_LeetCodeExamples_MatchesExpectedReachability(
        int jugX, int jugY, int target, bool expected) =>
        Assert.Equal(expected, WaterAndJugProblemSolution.CanMeasureWaterByStackSearch(jugX, jugY, target));

    [Theory]
    [MemberData(nameof(Examples))]
    public void CanMeasureWaterByDepthFirstSearch_LeetCodeExamples_MatchesExpectedReachability(
        int jugX, int jugY, int target, bool expected) =>
        Assert.Equal(expected, WaterAndJugProblemSolution.CanMeasureWaterByDepthFirstSearch(jugX, jugY, target));

    [Theory]
    [MemberData(nameof(Examples))]
    public void CanMeasureWaterByGcdFormula_LeetCodeExamples_MatchesExpectedReachability(
        int jugX, int jugY, int target, bool expected) =>
        Assert.Equal(expected, WaterAndJugProblemSolution.CanMeasureWaterByGcdFormula(jugX, jugY, target));
}
