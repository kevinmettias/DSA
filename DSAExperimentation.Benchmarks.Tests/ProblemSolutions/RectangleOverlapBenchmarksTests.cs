using DSAExperimentation.Benchmarks.ProblemSolutions;

namespace DSAExperimentation.Benchmarks.Tests.ProblemSolutions;

// Harness coverage for RectangleOverlapBenchmarks (ARCHITECTURE 17.9): the baseline's verdict against
// the one the workload's shape fixes, which the generic check cannot supply - it holds the closed-form
// arm to the baseline, and two bools agreeing would witness only that both arms said the same thing.
// Setup offsets rectangle 2 by half of rectangle 1's side on both axes, so the pair always overlaps.
public sealed partial class RectangleOverlapBenchmarksTests
{
    private const int SmallestSide = 60;

    [Fact]
    public void IsOverlappingByUnitGridIntersectionScan_HalfSideOffsetPair_ReportsOverlap() =>
        Assert.True(BuildHarness().IsOverlappingByUnitGridIntersectionScan());

    private static RectangleOverlapBenchmarks BuildHarness()
    {
        var harness = new RectangleOverlapBenchmarks { Side = SmallestSide };
        harness.Setup();

        return harness;
    }
}
