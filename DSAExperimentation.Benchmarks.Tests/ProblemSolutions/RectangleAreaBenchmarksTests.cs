using DSAExperimentation.Benchmarks.ProblemSolutions;

namespace DSAExperimentation.Benchmarks.Tests.ProblemSolutions;

// Harness coverage for RectangleAreaBenchmarks (ARCHITECTURE 17.9): both arms are
// RectangleAreaSolution's, competing strategies for the same question - the unit-grid arm paints
// the bounding box and counts the covered cells, the closed-form arm derives the union of two
// rectangles arithmetically - so a harness whose arms disagree is timing two different questions.
// Setup derives rectangle 2 from rectangle 1's Side, so the same Side must rebuild the same pair.
public sealed partial class RectangleAreaBenchmarksTests
{
    private const int SmallestSide = 60;

    [Fact]
    public void Setup_SameSide_RebuildsTheSameWorkload() =>
        Assert.Equal(
            BuildHarness().UnitGridCoverageCount(),
            BuildHarness().UnitGridCoverageCount());

    [Fact]
    public void UnitGridCoverageCount_AgreesWithClosedFormOverlapArithmetic()
    {
        var harness = BuildHarness();

        Assert.Equal(harness.ClosedFormOverlapArithmetic(), harness.UnitGridCoverageCount());
    }

    [Fact]
    public void ClosedFormOverlapArithmetic_AgreesWithUnitGridCoverageCount()
    {
        var harness = BuildHarness();

        Assert.Equal(harness.UnitGridCoverageCount(), harness.ClosedFormOverlapArithmetic());
    }

    private static RectangleAreaBenchmarks BuildHarness()
    {
        var harness = new RectangleAreaBenchmarks { Side = SmallestSide };
        harness.Setup();

        return harness;
    }
}
