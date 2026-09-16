using DSAExperimentation.Benchmarks.ProblemSolutions;

namespace DSAExperimentation.Benchmarks.Tests.ProblemSolutions;

// Harness coverage for CountNumberOfTrapezoidsIIBenchmarks (ARCHITECTURE 17.9): its two arms are
// competing strategies for the same question - the C(n, 4) four-point scan against counting
// parallel non-collinear segment pairs grouped by slope - so a harness whose arms disagree is
// timing two different problems. Setup seeds the points, so the same PointCount must rebuild the
// same set.
public sealed partial class CountNumberOfTrapezoidsIIBenchmarksTests
{
    private const int SmallestPointCount = 30;

    [Fact]
    public void Setup_SmallestPointCount_RebuildsTheSameWorkload()
    {
        var first = BuildHarness();
        var second = BuildHarness();

        // The documented shape: points are drawn from a small coordinate range, so repeated slopes
        // - and therefore parallel sides - reliably occur, and the count is never degenerate.
        Assert.InRange(first.BruteForce(), 1, int.MaxValue);
        Assert.Equal(first.BruteForce(), second.BruteForce());
    }

    [Fact]
    public void BruteForce_RepeatedSlopes_AgreesWithParallelSegmentCounting()
    {
        var harness = BuildHarness();

        Assert.Equal(harness.ParallelSegmentCounting(), harness.BruteForce());
    }

    [Fact]
    public void ParallelSegmentCounting_RepeatedSlopes_AgreesWithBruteForce()
    {
        var harness = BuildHarness();

        Assert.Equal(harness.BruteForce(), harness.ParallelSegmentCounting());
    }

    private static CountNumberOfTrapezoidsIIBenchmarks BuildHarness()
    {
        var harness = new CountNumberOfTrapezoidsIIBenchmarks { PointCount = SmallestPointCount };
        harness.Setup();

        return harness;
    }
}
