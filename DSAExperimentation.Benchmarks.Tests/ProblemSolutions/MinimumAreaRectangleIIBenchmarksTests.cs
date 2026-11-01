using DSAExperimentation.Benchmarks.ProblemSolutions;

namespace DSAExperimentation.Benchmarks.Tests.ProblemSolutions;

// Harness coverage for MinimumAreaRectangleIIBenchmarks (ARCHITECTURE 17.9): Setup draws the points
// from a grid barely larger than the point count, so rotated rectangles are plentiful and the answer
// cannot be the no-rectangle sentinel - a property of the workload the generic agreement check
// cannot supply, since two arms that both found nothing would still agree.
public sealed partial class MinimumAreaRectangleIIBenchmarksTests
{
    private const int SmallestLength = 12;

    // The no-rectangle sentinel both strategies report; the seeded dense grid yields real
    // rectangles, so this is only the fallback a broken workload would collapse onto.
    private const double NoRectangleArea = 0;

    [Fact]
    public void DiagonalGrouping_DenseGridPoints_FindsARectangle() =>
        Assert.NotEqual(NoRectangleArea, BuildHarness().DiagonalGrouping());

    private static MinimumAreaRectangleIIBenchmarks BuildHarness()
    {
        var harness = new MinimumAreaRectangleIIBenchmarks { Length = SmallestLength };
        harness.Setup();

        return harness;
    }
}
