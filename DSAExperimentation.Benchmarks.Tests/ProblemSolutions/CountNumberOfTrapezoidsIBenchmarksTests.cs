using DSAExperimentation.Benchmarks.ProblemSolutions;

namespace DSAExperimentation.Benchmarks.Tests.ProblemSolutions;

// Harness coverage for CountNumberOfTrapezoidsIBenchmarks (ARCHITECTURE 17.9): BenchmarkArmsTests
// already holds the arms to agreeing, and to answering the same on a rebuilt harness and when
// called again on one, so this asserts what it cannot: a lower bound on the answer that comes from
// the workload's shape rather than from either arm.
public sealed partial class CountNumberOfTrapezoidsIBenchmarksTests
{
    private const int SmallestPointCount = 30;

    // The documented shape: the y-coordinates come from a 20-value range, so the points pile
    // into a few rows. Pigeonhole alone puts at least ten of the thirty points in rows that
    // already hold another point, and two rows each holding a horizontal segment are a
    // trapezoid - so the count is never degenerate.
    [Fact]
    public void BruteForce_ThirtyPointsOnTwentyRows_CountsAtLeastOneTrapezoid() =>
        Assert.InRange(BuildHarness().BruteForce(), 1, int.MaxValue);

    private static CountNumberOfTrapezoidsIBenchmarks BuildHarness()
    {
        var harness = new CountNumberOfTrapezoidsIBenchmarks { PointCount = SmallestPointCount };
        harness.Setup();

        return harness;
    }
}
