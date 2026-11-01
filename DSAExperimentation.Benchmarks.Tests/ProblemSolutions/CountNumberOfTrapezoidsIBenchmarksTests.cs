using DSAExperimentation.Benchmarks.ProblemSolutions;

namespace DSAExperimentation.Benchmarks.Tests.ProblemSolutions;

// Harness coverage for CountNumberOfTrapezoidsIBenchmarks (ARCHITECTURE 17.9): BenchmarkArmsTests
// already holds the arms to agreeing on a fresh harness, so this asserts what it cannot: a lower
// bound on the answer that comes from the workload's shape rather than from either arm, and that a
// harness which has already answered once answers as a rebuild does, since BenchmarkDotNet calls an
// arm many times on one harness while the generic check builds a fresh harness for every call.
// Setup seeds the points, so the same PointCount must rebuild the same set.
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

    private static CountNumberOfTrapezoidsIBenchmarks BuildHarness()
    {
        var harness = new CountNumberOfTrapezoidsIBenchmarks { PointCount = SmallestPointCount };
        harness.Setup();

        return harness;
    }
}
