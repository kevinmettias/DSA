using DSAExperimentation.Benchmarks.ProblemSolutions;

namespace DSAExperimentation.Benchmarks.Tests.ProblemSolutions;

// Harness coverage for SeparateSquaresIIBenchmarks (ARCHITECTURE 17.9): both arms answer the same
// separating-line question about the same squares, so a harness whose arms disagree is timing two
// different sets of squares. Setup draws the squares from one fixed seed over a coordinate range
// small relative to SquareCount, so real overlaps occur and the same SquareCount must rebuild the
// same squares; neither arm mutates them, so one harness instance is safe to read twice in either
// order. Both arms answer with a coordinate, a double, so the comparison needs a tolerance rather
// than exact equality.
public sealed partial class SeparateSquaresIIBenchmarksTests
{
    private const int SmallestSquareCount = 20;
    private const double RelativeTolerance = 1E-09;

    [Fact]
    public void Setup_SameSquareCount_RebuildsTheSameWorkload() =>
        Assert.Equal(
            BuildHarness().EventSweep(),
            BuildHarness().EventSweep(),
            RelativeTolerance);

    [Fact]
    public void EventSweep_OverlappingSquares_AgreesWithIntervalSetSweep()
    {
        var harness = BuildHarness();

        Assert.Equal(harness.IntervalSetSweep(), harness.EventSweep(), RelativeTolerance);
    }

    [Fact]
    public void IntervalSetSweep_OverlappingSquares_AgreesWithEventSweep()
    {
        var harness = BuildHarness();

        Assert.Equal(harness.EventSweep(), harness.IntervalSetSweep(), RelativeTolerance);
    }

    private static SeparateSquaresIIBenchmarks BuildHarness()
    {
        var harness = new SeparateSquaresIIBenchmarks { SquareCount = SmallestSquareCount };
        harness.Setup();

        return harness;
    }
}
