using DSAExperimentation.Benchmarks.ProblemSolutions;

namespace DSAExperimentation.Benchmarks.Tests.ProblemSolutions;

// Harness coverage for MinimumTimeVisitingAllPointsBenchmarks (ARCHITECTURE 17.9): its two arms are
// competing strategies for the same question - an open-coded max(|dx|, |dy|) per hop against
// Algorithms.ShortestPaths' ChebyshevHeuristic witness supplying the same distance - so a harness whose
// arms disagree is timing two different problems. Both arms return the total travel time as an int,
// computed from points they only read, so one harness instance is safe to call twice in either order.
// Setup draws those points from one fixed seed, so the same PointCount must rebuild the same point set;
// otherwise two published numbers were never comparable in the first place.
public sealed partial class MinimumTimeVisitingAllPointsBenchmarksTests
{
    private const int SmallestPointCount = 200;

    [Fact]
    public void Setup_SamePointCount_RebuildsTheSamePointSet() =>
        Assert.Equal(BuildHarness().InlineChebyshevSum(), BuildHarness().InlineChebyshevSum());

    [Fact]
    public void InlineChebyshevSum_SeededPointSet_AgreesWithPathHeuristicChebyshevSum()
    {
        var harness = BuildHarness();

        Assert.Equal(harness.PathHeuristicChebyshevSum(), harness.InlineChebyshevSum());
    }

    [Fact]
    public void PathHeuristicChebyshevSum_SeededPointSet_AgreesWithInlineChebyshevSum()
    {
        var harness = BuildHarness();

        Assert.Equal(harness.InlineChebyshevSum(), harness.PathHeuristicChebyshevSum());
    }

    private static MinimumTimeVisitingAllPointsBenchmarks BuildHarness()
    {
        var harness = new MinimumTimeVisitingAllPointsBenchmarks { PointCount = SmallestPointCount };
        harness.Setup();

        return harness;
    }
}
