using DSAExperimentation.Benchmarks.ProblemSolutions;

namespace DSAExperimentation.Benchmarks.Tests.ProblemSolutions;

// Harness coverage for RottingOrangesBenchmarks (ARCHITECTURE 17.9): its two arms are competing
// strategies for the same question - an independent BFS restarted from every fresh orange against
// one shared multi-source frontier - so a harness whose arms disagree is timing two different
// problems. Setup asks the fixture for the grid at a fixed seed and size, so the same Size must
// rebuild the same grid; otherwise two published numbers were never comparable in the first place.
//
// Neither arm writes to the hoisted grid, so one harness is safe to call twice in either order and
// the single-harness rule holds. The fixture forces the top-left corner rotten but draws the rest of
// the grid, so how many minutes the rot takes is a property of that draw rather than something the
// fixture's documented shape decides; the arms are reconciled against each other.
public sealed partial class RottingOrangesBenchmarksTests
{
    private const int SmallestSize = 10;

    [Fact]
    public void Setup_SameSize_RebuildsTheSameGrid() =>
        Assert.Equal(BuildHarness().PerCellBfs(), BuildHarness().PerCellBfs());

    [Fact]
    public void PerCellBfs_SeededGrid_AgreesWithMultiSourceBfs()
    {
        var harness = BuildHarness();

        Assert.Equal(harness.MultiSourceBfs(), harness.PerCellBfs());
    }

    [Fact]
    public void MultiSourceBfs_SeededGrid_AgreesWithPerCellBfs()
    {
        var harness = BuildHarness();

        Assert.Equal(harness.PerCellBfs(), harness.MultiSourceBfs());
    }

    private static RottingOrangesBenchmarks BuildHarness()
    {
        var harness = new RottingOrangesBenchmarks { Size = SmallestSize };
        harness.Setup();

        return harness;
    }
}
