using DSAExperimentation.Benchmarks.ProblemSolutions;

namespace DSAExperimentation.Benchmarks.Tests.ProblemSolutions;

// Harness coverage for NumberOfClosedIslandsBenchmarks (ARCHITECTURE 17.9): its two arms are competing
// strategies for the same question - a hand-specialized flood fill threading a "touched border" flag
// out by ref against this repo's own DepthFirstSearch.Traverse checking each returned cell against the
// grid's edge - so a harness whose arms disagree is counting two different grids. Setup fills the
// seeded side x side grid from the fixed land density, so the same Side must rebuild the same cells.
//
// Each strategy clones the shared grid internally before filling it, so neither arm can leave the
// workload altered for the other and one harness serves both calls in either order. Both arms return
// an int, so they are compared directly.
public sealed partial class NumberOfClosedIslandsBenchmarksTests
{
    private const int SmallestSide = 30;

    [Fact]
    public void Setup_SameSide_RebuildsTheSameWorkload() =>
        Assert.Equal(BuildHarness().CountClosedIslandsByNaiveFloodFill(), BuildHarness().CountClosedIslandsByNaiveFloodFill());

    [Fact]
    public void CountClosedIslandsByDepthFirstSearch_AgreesWithCountClosedIslandsByNaiveFloodFill()
    {
        var harness = BuildHarness();

        Assert.Equal(
            harness.CountClosedIslandsByNaiveFloodFill(),
            harness.CountClosedIslandsByDepthFirstSearch());
    }

    [Fact]
    public void CountClosedIslandsByNaiveFloodFill_AgreesWithCountClosedIslandsByDepthFirstSearch()
    {
        var harness = BuildHarness();

        Assert.Equal(
            harness.CountClosedIslandsByDepthFirstSearch(),
            harness.CountClosedIslandsByNaiveFloodFill());
    }

    private static NumberOfClosedIslandsBenchmarks BuildHarness()
    {
        var harness = new NumberOfClosedIslandsBenchmarks { Side = SmallestSide };
        harness.Setup();

        return harness;
    }
}
