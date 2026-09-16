using DSAExperimentation.Benchmarks.ProblemSolutions;

namespace DSAExperimentation.Benchmarks.Tests.ProblemSolutions;

// Harness coverage for MakingALargeIslandBenchmarks (ARCHITECTURE 17.9): its two arms are competing
// strategies for the same question - flipping each water cell and running a fresh recursive flood
// fill against labelling every island exactly once and summing the already-known areas around each
// water cell - so a harness whose arms disagree is timing two different problems. Both arms return
// the largest island area a single flip can reach. Each arm clones the shared grid before labelling
// it, so one harness is safe to call twice in either order and the answers are comparable per
// strategy as well as across the pair. Setup draws the cells from one fixed seed, so the same Side
// must rebuild the same grid.
public sealed partial class MakingALargeIslandBenchmarksTests
{
    private const int SmallestSide = 20;

    [Fact]
    public void Setup_SameSide_RebuildsTheSameWorkload()
    {
        Assert.Equal(BuildHarness().LargestIslandByNaiveFloodFill(), BuildHarness().LargestIslandByNaiveFloodFill());
        Assert.Equal(BuildHarness().LargestIslandByLabeledFloodFill(), BuildHarness().LargestIslandByLabeledFloodFill());
    }

    [Fact]
    public void LargestIslandByNaiveFloodFill_SeededLandScatter_AgreesWithLabeledFloodFill()
    {
        var harness = BuildHarness();

        Assert.Equal(harness.LargestIslandByLabeledFloodFill(), harness.LargestIslandByNaiveFloodFill());
    }

    [Fact]
    public void LargestIslandByLabeledFloodFill_SeededLandScatter_AgreesWithNaiveFloodFill()
    {
        var harness = BuildHarness();

        Assert.Equal(harness.LargestIslandByNaiveFloodFill(), harness.LargestIslandByLabeledFloodFill());
    }

    private static MakingALargeIslandBenchmarks BuildHarness()
    {
        var harness = new MakingALargeIslandBenchmarks { Side = SmallestSide };
        harness.Setup();

        return harness;
    }
}
