using DSAExperimentation.Benchmarks.ProblemSolutions;

namespace DSAExperimentation.Benchmarks.Tests.ProblemSolutions;

// Harness coverage for CountNumberOfRectanglesContainingEachPointBenchmarks (ARCHITECTURE 17.9):
// BenchmarkArmsTests already holds the arms to agreeing, and to answering the same on a rebuilt
// harness and when called again on one, so this asserts what it cannot: the answer's length and
// per-point bound, which come from the workload's shape rather than from either arm.
public sealed partial class CountNumberOfRectanglesContainingEachPointBenchmarksTests
{
    private const int SmallestRectangleCount = 500;

    [Fact]
    public void BruteForce_AsManyPointsAsRectangles_CountsEachPointWithinTheRectangleCount()
    {
        var counts = BuildHarness().BruteForce();

        // The documented shape: Setup draws as many points as rectangles, and the answer is one
        // count per point - each no larger than the number of rectangles that could cover it.
        Assert.Equal(SmallestRectangleCount, counts.Length);
        Assert.All(counts, count => Assert.InRange(count, 0, SmallestRectangleCount));
    }

    private static CountNumberOfRectanglesContainingEachPointBenchmarks BuildHarness()
    {
        var harness = new CountNumberOfRectanglesContainingEachPointBenchmarks { RectangleCount = SmallestRectangleCount };
        harness.Setup();

        return harness;
    }
}
