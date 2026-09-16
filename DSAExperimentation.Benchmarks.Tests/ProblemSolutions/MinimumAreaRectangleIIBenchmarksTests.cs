using DSAExperimentation.Benchmarks.ProblemSolutions;

namespace DSAExperimentation.Benchmarks.Tests.ProblemSolutions;

// Harness coverage for MinimumAreaRectangleIIBenchmarks (ARCHITECTURE 17.9): its two arms are
// competing strategies for the same question - an O(n^4) scan of every ordered quadruple against
// grouping candidate diagonals by midpoint and length - so a harness whose arms disagree is timing
// two different problems. Both arms answer with a double, so they are compared under a named
// relative tolerance rather than by exact equality; the two strategies combine coordinates in
// different orders, and a last-bit difference in those combinations is not what the comparison is
// about. Setup draws the points from a grid barely larger than the point count, so rotated
// rectangles are plentiful.
public sealed partial class MinimumAreaRectangleIIBenchmarksTests
{
    private const int SmallestLength = 12;

    // Both arms compute the same area by the same multiplications over the same coordinates, so
    // they agree far inside this; the constant exists so the two doubles are compared as
    // measurements rather than for exact equality.
    private const double RelativeTolerance = 1E-09;

    // The no-rectangle sentinel both strategies report; the seeded dense grid yields real
    // rectangles, so this is only the fallback a broken workload would collapse onto.
    private const double NoRectangleArea = 0;

    [Fact]
    public void Setup_SameLength_RebuildsTheSamePointSet()
    {
        Assert.Equal(
            BuildHarness().BruteForceQuadruples(),
            BuildHarness().BruteForceQuadruples(),
            RelativeTolerance);
    }

    [Fact]
    public void BruteForceQuadruples_DenseGridPoints_AgreesWithDiagonalGrouping()
    {
        var harness = BuildHarness();

        Assert.Equal(harness.DiagonalGrouping(), harness.BruteForceQuadruples(), RelativeTolerance);
    }

    [Fact]
    public void DiagonalGrouping_DenseGridPoints_AgreesWithBruteForceQuadruples()
    {
        var harness = BuildHarness();

        Assert.NotEqual(NoRectangleArea, harness.DiagonalGrouping());
        Assert.Equal(harness.BruteForceQuadruples(), harness.DiagonalGrouping(), RelativeTolerance);
    }

    private static MinimumAreaRectangleIIBenchmarks BuildHarness()
    {
        var harness = new MinimumAreaRectangleIIBenchmarks { Length = SmallestLength };
        harness.Setup();

        return harness;
    }
}
