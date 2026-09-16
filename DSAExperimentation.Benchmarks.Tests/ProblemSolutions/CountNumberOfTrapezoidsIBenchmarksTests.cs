using DSAExperimentation.Benchmarks.ProblemSolutions;

namespace DSAExperimentation.Benchmarks.Tests.ProblemSolutions;

// Harness coverage for CountNumberOfTrapezoidsIBenchmarks (ARCHITECTURE 17.9): its two arms are
// competing strategies for the same question - the C(n, 4) four-point scan against counting pairs of
// horizontal segments grouped by y - so a harness whose arms disagree is timing two different
// problems. Setup seeds the points, so the same PointCount must rebuild the same set.
public sealed partial class CountNumberOfTrapezoidsIBenchmarksTests
{
    private const int SmallestPointCount = 30;

    [Fact]
    public void Setup_SmallestPointCount_RebuildsTheSameWorkload()
    {
        var first = BuildHarness();
        var second = BuildHarness();

        // The documented shape: the y-coordinates come from a 20-value range, so the points pile
        // into a few rows. Pigeonhole alone puts at least ten of the thirty points in rows that
        // already hold another point, and two rows each holding a horizontal segment are a
        // trapezoid - so the count is never degenerate.
        Assert.InRange(first.BruteForce(), 1, int.MaxValue);
        Assert.Equal(first.BruteForce(), second.BruteForce());
    }

    [Fact]
    public void BruteForce_RepeatedYValues_AgreesWithHorizontalPairCounting()
    {
        var harness = BuildHarness();

        Assert.Equal(harness.HorizontalPairCounting(), harness.BruteForce());
    }

    [Fact]
    public void HorizontalPairCounting_RepeatedYValues_AgreesWithBruteForce()
    {
        var harness = BuildHarness();

        Assert.Equal(harness.BruteForce(), harness.HorizontalPairCounting());
    }

    private static CountNumberOfTrapezoidsIBenchmarks BuildHarness()
    {
        var harness = new CountNumberOfTrapezoidsIBenchmarks { PointCount = SmallestPointCount };
        harness.Setup();

        return harness;
    }
}
