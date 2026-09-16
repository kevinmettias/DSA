using DSAExperimentation.LeetCode.WaterAndJugProblem;

namespace DSAExperimentation.Tests.LeetCodeCoverage.WaterAndJugProblem;

// Harness only. All three strategies live in WaterAndJugProblemSolution; this
// file just pins them to LeetCode's published examples plus a boundary case
// where the target exceeds the combined capacity.
public sealed class WaterAndJugProblemTests
{
    public static TheoryData<MeasureWaterExample> Examples =>
        new()
        {
            { new MeasureWaterExample(JugX: 3, JugY: 5, Target: 4, Expected: true) },
            { new MeasureWaterExample(JugX: 2, JugY: 6, Target: 5, Expected: false) },
            { new MeasureWaterExample(JugX: 1, JugY: 2, Target: 3, Expected: true) },
            { new MeasureWaterExample(JugX: 2, JugY: 3, Target: 0, Expected: true) },
            { new MeasureWaterExample(JugX: 1, JugY: 2, Target: 4, Expected: false) },
        };

    [Theory]
    [MemberData(nameof(Examples))]
    public void CanMeasureWaterByStackSearch_LeetCodeExamples_MatchesExpectedReachability(MeasureWaterExample example)
    {
        var actual = WaterAndJugProblemSolution.CanMeasureWaterByStackSearch(
            example.JugX, example.JugY, example.Target);

        Assert.Equal(example.Expected, actual);
    }

    [Theory]
    [MemberData(nameof(Examples))]
    public void CanMeasureWaterByDepthFirstSearch_LeetCodeExamples_MatchesExpectedReachability(
        MeasureWaterExample example)
    {
        var actual = WaterAndJugProblemSolution.CanMeasureWaterByDepthFirstSearch(
            example.JugX, example.JugY, example.Target);

        Assert.Equal(example.Expected, actual);
    }

    [Theory]
    [MemberData(nameof(Examples))]
    public void CanMeasureWaterByGcdFormula_LeetCodeExamples_MatchesExpectedReachability(MeasureWaterExample example)
    {
        var actual = WaterAndJugProblemSolution.CanMeasureWaterByGcdFormula(
            example.JugX, example.JugY, example.Target);

        Assert.Equal(example.Expected, actual);
    }

    // One LeetCode example: the two jug capacities, the target volume, and whether it is
    // reachable. The row names every position - a bare `bool` argument would read as
    // "true" and say nothing about what is true.
    public readonly record struct MeasureWaterExample(int JugX, int JugY, int Target, bool Expected);
}
