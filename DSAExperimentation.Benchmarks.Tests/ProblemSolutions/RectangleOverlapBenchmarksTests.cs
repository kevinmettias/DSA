using DSAExperimentation.Benchmarks.ProblemSolutions;

namespace DSAExperimentation.Benchmarks.Tests.ProblemSolutions;

// Harness coverage for RectangleOverlapBenchmarks (ARCHITECTURE 17.9): both arms are
// RectangleOverlapSolution's, competing strategies for the same question - the unit-grid arm paints
// the bounding box before it can answer, the closed-form arm compares the axis intervals - so a
// harness whose arms disagree answers two different overlap questions. Setup offsets rectangle 2 by
// half of rectangle 1's side on both axes, so the pair always overlaps and the verdict is fixed by
// the fixture rather than by either arm; that decisive verdict is asserted beside the agreement,
// because two bools agreeing on `true` would otherwise witness only that both arms said something.
public sealed partial class RectangleOverlapBenchmarksTests
{
    private const int SmallestSide = 60;

    [Fact]
    public void Setup_SameSide_RebuildsTheSameWorkload() =>
        Assert.Equal(
            BuildHarness().IsOverlappingByUnitGridIntersectionScan(),
            BuildHarness().IsOverlappingByUnitGridIntersectionScan());

    [Fact]
    public void IsOverlappingByUnitGridIntersectionScan_HalfSideOffsetPair_ReportsOverlap()
    {
        var harness = BuildHarness();
        var unitGridVerdict = harness.IsOverlappingByUnitGridIntersectionScan();

        Assert.True(unitGridVerdict);
        Assert.Equal(harness.IsOverlappingByClosedFormAxisIntervals(), unitGridVerdict);
    }

    [Fact]
    public void IsOverlappingByClosedFormAxisIntervals_HalfSideOffsetPair_ReportsOverlap()
    {
        var harness = BuildHarness();
        var closedFormVerdict = harness.IsOverlappingByClosedFormAxisIntervals();

        Assert.True(closedFormVerdict);
        Assert.Equal(harness.IsOverlappingByUnitGridIntersectionScan(), closedFormVerdict);
    }

    private static RectangleOverlapBenchmarks BuildHarness()
    {
        var harness = new RectangleOverlapBenchmarks { Side = SmallestSide };
        harness.Setup();

        return harness;
    }
}
