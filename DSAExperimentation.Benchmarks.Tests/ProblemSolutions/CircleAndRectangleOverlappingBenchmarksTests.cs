using DSAExperimentation.Benchmarks.ProblemSolutions;

namespace DSAExperimentation.Benchmarks.Tests.ProblemSolutions;

// Harness coverage for CircleAndRectangleOverlappingBenchmarks (ARCHITECTURE 17.9): its two arms are
// competing strategies for the same question - scanning every lattice point of the rectangle against
// the O(1) clamp-and-distance check - so a harness whose arms disagree is timing two different
// problems. Both arms answer with a bare bool, so agreement between them says the two strategies
// reached the same verdict on the same circle and rectangle.
public sealed partial class CircleAndRectangleOverlappingBenchmarksTests
{
    private const int SmallestSide = 60;

    // Setup centres the circle on the rectangle's far corner, (Side, Side), with radius 0, so the
    // corner point is the one place the two shapes meet and the answer is yes. The lattice scan
    // visits that corner last, its bounds being inclusive on both sides, and the clamped point is
    // that same corner - which is exactly the case the harness builds the workload for, since it
    // forces the scan through the whole rectangle rather than exiting on an early point. A rebuilt
    // workload therefore has to answer yes.
    private const bool ExpectedVerdict = true;

    [Fact]
    public void Setup_SameSide_RebuildsTheZeroRadiusCircleOnTheFarCorner()
    {
        var first = BuildHarness();
        var second = BuildHarness();

        Assert.Equal(ExpectedVerdict, first.HasOverlapByLatticePointScan());
        Assert.Equal(ExpectedVerdict, second.HasOverlapByClampedDistance());
    }

    [Fact]
    public void HasOverlapByLatticePointScan_ZeroRadiusCircleOnTheFarCorner_AgreesWithClampedDistance()
    {
        var harness = BuildHarness();

        Assert.Equal(harness.HasOverlapByClampedDistance(), harness.HasOverlapByLatticePointScan());
    }

    [Fact]
    public void HasOverlapByClampedDistance_ZeroRadiusCircleOnTheFarCorner_AgreesWithLatticePointScan()
    {
        var harness = BuildHarness();

        Assert.Equal(harness.HasOverlapByLatticePointScan(), harness.HasOverlapByClampedDistance());
    }

    private static CircleAndRectangleOverlappingBenchmarks BuildHarness()
    {
        var harness = new CircleAndRectangleOverlappingBenchmarks { Side = SmallestSide };
        harness.Setup();

        return harness;
    }
}
