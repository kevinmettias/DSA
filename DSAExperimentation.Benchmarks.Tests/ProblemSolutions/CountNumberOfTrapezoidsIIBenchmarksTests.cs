using DSAExperimentation.Benchmarks.ProblemSolutions;

namespace DSAExperimentation.Benchmarks.Tests.ProblemSolutions;

// Harness coverage for CountNumberOfTrapezoidsIIBenchmarks (ARCHITECTURE 17.9): BenchmarkArmsTests
// already holds the arms to agreeing, and to answering the same on a rebuilt harness and when
// called again on one, so this asserts what it cannot: a lower bound on the answer that comes from
// the workload's shape rather than from either arm.
public sealed partial class CountNumberOfTrapezoidsIIBenchmarksTests
{
    private const int SmallestPointCount = 30;

    // The documented shape: points are drawn from a small coordinate range, so repeated slopes
    // - and therefore parallel sides - reliably occur, and the count is never degenerate.
    [Fact]
    public void BruteForce_SmallCoordinateRange_CountsAtLeastOneTrapezoid() =>
        Assert.InRange(BuildHarness().BruteForce(), 1, int.MaxValue);

    private static CountNumberOfTrapezoidsIIBenchmarks BuildHarness()
    {
        var harness = new CountNumberOfTrapezoidsIIBenchmarks { PointCount = SmallestPointCount };
        harness.Setup();

        return harness;
    }
}
