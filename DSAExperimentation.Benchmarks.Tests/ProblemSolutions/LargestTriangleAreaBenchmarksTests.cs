using DSAExperimentation.Benchmarks.ProblemSolutions;

namespace DSAExperimentation.Benchmarks.Tests.ProblemSolutions;

// Harness coverage for LargestTriangleAreaBenchmarks (ARCHITECTURE 17.9): its two arms are competing
// strategies for the same question - every triple against the convex hull's own vertices, reduced
// from the same point set - so a harness whose arms disagree is timing two different problems.
// Setup draws distinct integer points from one fixed seed, so the same Length must rebuild the same
// point set; otherwise two published numbers were never comparable in the first place.
//
// Both arms return a double, so the two areas are compared under a named relative tolerance rather
// than for bit equality: the hull arm evaluates the same maximum over a subset of the same triples,
// but the two are free to reach it through different float arithmetic.
public sealed partial class LargestTriangleAreaBenchmarksTests
{
    private const int SmallestLength = 60;
    private const double RelativeTolerance = 1e-9;

    // Areas at this coordinate bound are in the hundreds of thousands, but a degenerate fixture
    // could return 0; the floor keeps the tolerance band a real number rather than a bare zero.
    private const double AreaScaleFloor = 1.0;

    [Fact]
    public void Setup_SameLength_RebuildsTheSamePointSet() =>
        Assert.Equal(
            BuildHarness().BruteForceAllTriples(),
            BuildHarness().BruteForceAllTriples(),
            RelativeTolerance * AreaScaleFloor);

    [Fact]
    public void BruteForceAllTriples_DistinctIntegerPoints_AgreesWithConvexHullReduction()
    {
        var harness = BuildHarness();
        var bruteForce = harness.BruteForceAllTriples();

        Assert.Equal(
            bruteForce,
            harness.ConvexHullReduction(),
            RelativeTolerance * Math.Max(bruteForce, AreaScaleFloor));
    }

    [Fact]
    public void ConvexHullReduction_DistinctIntegerPoints_AgreesWithBruteForceAllTriples()
    {
        var harness = BuildHarness();
        var hullReduction = harness.ConvexHullReduction();

        Assert.Equal(
            hullReduction,
            harness.BruteForceAllTriples(),
            RelativeTolerance * Math.Max(hullReduction, AreaScaleFloor));
    }

    private static LargestTriangleAreaBenchmarks BuildHarness()
    {
        var harness = new LargestTriangleAreaBenchmarks { Length = SmallestLength };
        harness.Setup();

        return harness;
    }
}
