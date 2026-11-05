using DSAExperimentation.LeetCode.Solutions.WaterAndJugProblem;

namespace DSAExperimentation.LeetCode.Solutions.Tests.WaterAndJugProblem;

// Harness only. All three strategies live in WaterAndJugProblemSolution; this
// file just pins them to a boundary case
// where the target exceeds the combined capacity.
public sealed partial class WaterAndJugProblemSolutionTests
{
    public static TheoryData<MeasureWaterCase> Cases =>
        new()
        {
            { new MeasureWaterCase(JugX: 3, JugY: 5, Target: 4, Expected: true) },
            { new MeasureWaterCase(JugX: 2, JugY: 3, Target: 0, Expected: true) },
            { new MeasureWaterCase(JugX: 1, JugY: 2, Target: 4, Expected: false) },
        };

    [Theory]
    [MemberData(nameof(Cases))]
    public void CanMeasureWaterByStackSearch_Cases_MatchesExpectedReachability(MeasureWaterCase testCase)
    {
        var actual = WaterAndJugProblemSolution.CanMeasureWaterByStackSearch(
            testCase.JugX, testCase.JugY, testCase.Target);

        Assert.Equal(testCase.Expected, actual);
    }

    [Theory]
    [MemberData(nameof(Cases))]
    public void CanMeasureWaterByDepthFirstSearch_Cases_MatchesExpectedReachability(
        MeasureWaterCase testCase)
    {
        var actual = WaterAndJugProblemSolution.CanMeasureWaterByDepthFirstSearch(
            testCase.JugX, testCase.JugY, testCase.Target);

        Assert.Equal(testCase.Expected, actual);
    }

    [Theory]
    [MemberData(nameof(Cases))]
    public void CanMeasureWaterByGcdFormula_Cases_MatchesExpectedReachability(MeasureWaterCase testCase)
    {
        var actual = WaterAndJugProblemSolution.CanMeasureWaterByGcdFormula(
            testCase.JugX, testCase.JugY, testCase.Target);

        Assert.Equal(testCase.Expected, actual);
    }

    // One case: the two jug capacities, the target volume, and whether it is
    // reachable. The row names every position - a bare `bool` argument would read as
    // "true" and say nothing about what is true.
    public readonly record struct MeasureWaterCase(int JugX, int JugY, int Target, bool Expected);
}
