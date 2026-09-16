using DSAExperimentation.Benchmarks.ProblemSolutions;

namespace DSAExperimentation.Benchmarks.Tests.ProblemSolutions;

// Harness coverage for RectangleAreaIIBenchmarks (ARCHITECTURE 17.9): both arms are
// RectangleAreaIISolution's, competing strategies for the same question - coordinate compression
// checks every compressed cell against every rectangle, the sweep line merges each x-slab's active
// y-ranges - so a harness whose arms disagree is measuring two different areas. Setup draws the
// rectangles from one seeded Random, so the same RectangleCount must rebuild the same workload.
public sealed partial class RectangleAreaIIBenchmarksTests
{
    private const int SmallestRectangleCount = 20;

    [Fact]
    public void Setup_SameRectangleCount_RebuildsTheSameWorkload() =>
        Assert.Equal(
            BuildHarness().CoordinateCompressionCellCheck(),
            BuildHarness().CoordinateCompressionCellCheck());

    [Fact]
    public void CoordinateCompressionCellCheck_AgreesWithSweepLineWithIntervalSet()
    {
        var harness = BuildHarness();

        Assert.Equal(harness.SweepLineWithIntervalSet(), harness.CoordinateCompressionCellCheck());
    }

    [Fact]
    public void SweepLineWithIntervalSet_AgreesWithCoordinateCompressionCellCheck()
    {
        var harness = BuildHarness();

        Assert.Equal(harness.CoordinateCompressionCellCheck(), harness.SweepLineWithIntervalSet());
    }

    private static RectangleAreaIIBenchmarks BuildHarness()
    {
        var harness = new RectangleAreaIIBenchmarks { RectangleCount = SmallestRectangleCount };
        harness.Setup();

        return harness;
    }
}
