using DSAExperimentation.Benchmarks.ProblemSolutions;

namespace DSAExperimentation.Benchmarks.Tests.ProblemSolutions;

// Harness coverage for MaxAreaOfIslandBenchmarks (ARCHITECTURE 17.9): its two arms are competing
// strategies for the same question - the hand-specialized recursive flood fill against this repo's
// own DepthFirstSearch traversal - so a harness whose arms disagree is timing two different
// problems. Both arms return the largest island's area. Each arm clones the shared grid before
// mutating it, so one harness is safe to call twice in either order and each strategy's answer is
// stable across setups as well as comparable across the pair. Setup draws the cells from one fixed
// seed, so the same Side must rebuild the same grid.
public sealed partial class MaxAreaOfIslandBenchmarksTests
{
    private const int SmallestSide = 30;

    [Fact]
    public void Setup_SameSide_RebuildsTheSameWorkload()
    {
        Assert.Equal(BuildHarness().ByNaiveFloodFill(), BuildHarness().ByNaiveFloodFill());
        Assert.Equal(BuildHarness().ByDepthFirstSearch(), BuildHarness().ByDepthFirstSearch());
    }

    [Fact]
    public void ByNaiveFloodFill_SeededLandScatter_AgreesWithDepthFirstSearch()
    {
        var harness = BuildHarness();

        Assert.Equal(harness.ByDepthFirstSearch(), harness.ByNaiveFloodFill());
    }

    [Fact]
    public void ByDepthFirstSearch_SeededLandScatter_AgreesWithNaiveFloodFill()
    {
        var harness = BuildHarness();

        Assert.Equal(harness.ByNaiveFloodFill(), harness.ByDepthFirstSearch());
    }

    private static MaxAreaOfIslandBenchmarks BuildHarness()
    {
        var harness = new MaxAreaOfIslandBenchmarks { Side = SmallestSide };
        harness.Setup();

        return harness;
    }
}
