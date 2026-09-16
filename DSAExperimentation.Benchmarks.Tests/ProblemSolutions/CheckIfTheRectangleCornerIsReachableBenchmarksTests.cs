using DSAExperimentation.Benchmarks.ProblemSolutions;

namespace DSAExperimentation.Benchmarks.Tests.ProblemSolutions;

// Harness coverage for CheckIfTheRectangleCornerIsReachableBenchmarks (ARCHITECTURE 17.9): its two
// arms are competing strategies for the same question - a flood fill over the rectangle's boundary
// against a disjoint-set sweep over the same circles - so a harness whose arms disagree is timing
// two different problems. Both arms answer with a bare bool, so agreement between them says the two
// strategies reached the same verdict on the same circle list.
public sealed partial class CheckIfTheRectangleCornerIsReachableBenchmarksTests
{
    private const int SmallestCircleCount = 50;

    [Fact]
    public void Setup_SameCircleCount_RebuildsTheSameCircles()
    {
        var first = BuildHarness();
        var second = BuildHarness();

        // The circle list is the fixture's seeded draw and stays private, and each arm reduces it to
        // one bit, so two harnesses built from the same CircleCount reporting the same verdict for
        // each strategy is the reading the rebuild can be pinned to: one seed means the same
        // circles, and the same circles mean the same verdict.
        Assert.Equal(first.IsReachableByBoundaryFloodFill(), second.IsReachableByBoundaryFloodFill());
        Assert.Equal(first.IsReachableByDisjointSet(), second.IsReachableByDisjointSet());
    }

    [Fact]
    public void IsReachableByBoundaryFloodFill_SeededCircleScatter_AgreesWithDisjointSet()
    {
        var harness = BuildHarness();

        Assert.Equal(harness.IsReachableByDisjointSet(), harness.IsReachableByBoundaryFloodFill());
    }

    [Fact]
    public void IsReachableByDisjointSet_SeededCircleScatter_AgreesWithBoundaryFloodFill()
    {
        var harness = BuildHarness();

        Assert.Equal(harness.IsReachableByBoundaryFloodFill(), harness.IsReachableByDisjointSet());
    }

    private static CheckIfTheRectangleCornerIsReachableBenchmarks BuildHarness()
    {
        var harness = new CheckIfTheRectangleCornerIsReachableBenchmarks { CircleCount = SmallestCircleCount };
        harness.Setup();

        return harness;
    }
}
