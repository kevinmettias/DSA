using DSAExperimentation.Benchmarks.ProblemSolutions;

namespace DSAExperimentation.Benchmarks.Tests.ProblemSolutions;

// Harness coverage for CircleAndRectangleOverlappingBenchmarks (ARCHITECTURE 17.9):
// BenchmarkArmsTests already holds the arms to agreeing on a rebuilt workload, so this pins the
// baseline to the verdict the circle's placement decides, derived independently of either arm.
public sealed partial class CircleAndRectangleOverlappingBenchmarksTests
{
    private const int SmallestSide = 60;

    // Setup centres a radius-1 circle at (Side + 1, Side), one unit past the rectangle's far corner
    // (Side, Side), so that corner is the one lattice point within reach and the answer is yes:
    // every other point of the rectangle is at least the square root of two away. The lattice scan
    // visits that corner last, its bounds being inclusive on both sides, which is exactly the case
    // the harness builds the workload for, since it forces the scan through the whole rectangle
    // rather than exiting on an early point.
    private const bool ExpectedVerdict = true;

    [Fact]
    public void HasOverlapByLatticePointScan_UnitCircleTouchingTheFarCorner_ReturnsTrue() =>
        Assert.Equal(ExpectedVerdict, BuildHarness().HasOverlapByLatticePointScan());

    private static CircleAndRectangleOverlappingBenchmarks BuildHarness()
    {
        var harness = new CircleAndRectangleOverlappingBenchmarks { Side = SmallestSide };
        harness.Setup();

        return harness;
    }
}
