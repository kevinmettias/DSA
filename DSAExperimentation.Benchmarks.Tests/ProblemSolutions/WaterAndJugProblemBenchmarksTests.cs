using DSAExperimentation.Benchmarks.ProblemSolutions;

namespace DSAExperimentation.Benchmarks.Tests.ProblemSolutions;

// Harness coverage for WaterAndJugProblemBenchmarks (ARCHITECTURE 17.9): all three arms are
// competing strategies for the same question, so a harness whose arms disagree is timing three
// different problems. Setup picks the target unreachable by construction (the class comment names
// jugX + jugY - 1 as never a multiple of the jugs' gcd), which is what makes the expected answer a
// decisive value rather than a re-reading of whichever arm answered first.
public sealed partial class WaterAndJugProblemBenchmarksTests
{
    private const int SmallestCapacity = 40;

    [Fact]
    public void Setup_SameCapacity_RebuildsTheSameWorkload() =>
        Assert.Equal(
            AnswerText.Of(BuildHarness().CanMeasureWaterByGcdFormula()),
            AnswerText.Of(BuildHarness().CanMeasureWaterByGcdFormula()));

    [Fact]
    public void CanMeasureWaterByStackSearch_AgreesWithDepthFirstSearch()
    {
        var harness = BuildHarness();

        Assert.Equal(harness.CanMeasureWaterByStackSearch(), harness.CanMeasureWaterByDepthFirstSearch());
    }

    [Fact]
    public void CanMeasureWaterByDepthFirstSearch_AgreesWithGcdFormula()
    {
        var harness = BuildHarness();

        Assert.Equal(harness.CanMeasureWaterByDepthFirstSearch(), harness.CanMeasureWaterByGcdFormula());
    }

    // The decisive value: an unreachable target is unmeasurable, whichever strategy is asked.
    [Fact]
    public void CanMeasureWaterByGcdFormula_UnreachableTarget_ReportsImmeasurable()
    {
        var harness = BuildHarness();

        Assert.False(harness.CanMeasureWaterByGcdFormula());
        Assert.False(harness.CanMeasureWaterByStackSearch());
        Assert.False(harness.CanMeasureWaterByDepthFirstSearch());
    }

    private static WaterAndJugProblemBenchmarks BuildHarness()
    {
        var harness = new WaterAndJugProblemBenchmarks { Capacity = SmallestCapacity };
        harness.Setup();

        return harness;
    }
}
