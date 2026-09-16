using DSAExperimentation.Benchmarks.ProblemSolutions;

namespace DSAExperimentation.Benchmarks.Tests.ProblemSolutions;

// Harness coverage for PrisonCellsAfterNDaysBenchmarks (ARCHITECTURE 17.9): its two arms are competing
// strategies for one question - the cell configuration after Days transitions - so a harness whose
// arms disagree is timing two different problems. Days is the only [Params] axis and Setup pins the
// eight-cell configuration both arms start from, so the same Days must rebuild the same start state.
public sealed partial class PrisonCellsAfterNDaysBenchmarksTests
{
    private const int SmallestDays = 10_000;

    [Fact]
    public void Setup_SameCellConfiguration_RebuildsTheSameCells() =>
        Assert.Equal(
            AnswerText.Of(BuildHarness().DailySimulation()),
            AnswerText.Of(BuildHarness().DailySimulation()));

    [Fact]
    public void DailySimulation_AgreesWithCycleDetectionViaHashMap()
    {
        var harness = BuildHarness();

        Assert.Equal(
            AnswerText.Of(harness.CycleDetectionViaHashMap()),
            AnswerText.Of(harness.DailySimulation()));
    }

    [Fact]
    public void CycleDetectionViaHashMap_AgreesWithDailySimulation()
    {
        var harness = BuildHarness();

        Assert.Equal(
            AnswerText.Of(harness.DailySimulation()),
            AnswerText.Of(harness.CycleDetectionViaHashMap()));
    }

    private static PrisonCellsAfterNDaysBenchmarks BuildHarness()
    {
        var harness = new PrisonCellsAfterNDaysBenchmarks { Days = SmallestDays };
        harness.Setup();

        return harness;
    }
}
