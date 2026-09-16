using DSAExperimentation.Benchmarks.ProblemSolutions;

namespace DSAExperimentation.Benchmarks.Tests.ProblemSolutions;

// Harness coverage for MaximumNumberOfFishInAGridBenchmarks (ARCHITECTURE 17.9): both arms are
// MaximumNumberOfFishInAGridSolution's competing strategies for one question - a hand-specialized
// recursive flood fill against this repo's DepthFirstSearch.Traverse - so a harness whose arms
// disagree is timing two different problems. Both answer with the best single-component catch,
// compared directly.
//
// Both arms clone the shared grid internally before sinking cells, so one harness instance is safe
// to call twice in either order and a single harness per test is enough.
public sealed partial class MaximumNumberOfFishInAGridBenchmarksTests
{
    private const int SmallestSide = 30;

    [Fact]
    public void Setup_SameSide_RebuildsTheSameWaterGrid()
    {
        var first = BuildHarness();
        var second = BuildHarness();

        // The grid is private, so the rebuild is pinned through the catch it produces: the same
        // Side must draw the same seeded water cells and score them identically.
        Assert.Equal(first.NaiveRecursiveFloodFill(), second.NaiveRecursiveFloodFill());
        Assert.Equal(first.DepthFirstSearchTraversal(), second.DepthFirstSearchTraversal());
    }

    [Fact]
    public void NaiveRecursiveFloodFill_SeededWaterGrid_AgreesWithDepthFirstSearchTraversal()
    {
        var harness = BuildHarness();

        Assert.Equal(harness.DepthFirstSearchTraversal(), harness.NaiveRecursiveFloodFill());
    }

    [Fact]
    public void DepthFirstSearchTraversal_SeededWaterGrid_AgreesWithNaiveRecursiveFloodFill()
    {
        var harness = BuildHarness();

        Assert.Equal(harness.NaiveRecursiveFloodFill(), harness.DepthFirstSearchTraversal());
    }

    private static MaximumNumberOfFishInAGridBenchmarks BuildHarness()
    {
        var harness = new MaximumNumberOfFishInAGridBenchmarks { Side = SmallestSide };
        harness.Setup();

        return harness;
    }
}
