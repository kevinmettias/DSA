using DSAExperimentation.Benchmarks.ProblemSolutions;

namespace DSAExperimentation.Benchmarks.Tests.ProblemSolutions;

// Harness coverage for CountSubIslandsBenchmarks (ARCHITECTURE 17.9): its two arms are competing
// strategies for the same question - a recursive flood fill against the DFS-engine traversal - so a
// harness whose arms disagree is timing two different problems. Setup seeds both grids, so the same
// Side must rebuild the same pair.
public sealed partial class CountSubIslandsBenchmarksTests
{
    private const int SmallestSide = 30;

    // The answer counts islands of grid2, so it can never exceed its cells.
    private const int MostGridCells = SmallestSide * SmallestSide;

    [Fact]
    public void Setup_SmallestSide_RebuildsTheSameWorkload()
    {
        var first = BuildHarness();
        var second = BuildHarness();

        // The documented shape: grid1 is land-biased relative to grid2, so grid2's islands are a
        // real mix of ones grid1 covers completely and ones it does not - never an empty answer.
        Assert.InRange(first.RecursiveFloodFill(), 1, MostGridCells);
        Assert.Equal(first.RecursiveFloodFill(), second.RecursiveFloodFill());
    }

    [Fact]
    public void RecursiveFloodFill_LandBiasedGridPair_AgreesWithDepthFirstSearchTraverse()
    {
        var harness = BuildHarness();

        Assert.Equal(harness.DepthFirstSearchTraverse(), harness.RecursiveFloodFill());
    }

    [Fact]
    public void DepthFirstSearchTraverse_LandBiasedGridPair_AgreesWithRecursiveFloodFill()
    {
        var harness = BuildHarness();

        Assert.Equal(harness.RecursiveFloodFill(), harness.DepthFirstSearchTraverse());
    }

    private static CountSubIslandsBenchmarks BuildHarness()
    {
        var harness = new CountSubIslandsBenchmarks { Side = SmallestSide };
        harness.Setup();

        return harness;
    }
}
