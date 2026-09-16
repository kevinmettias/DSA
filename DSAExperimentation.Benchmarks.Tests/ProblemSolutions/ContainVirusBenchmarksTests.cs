using DSAExperimentation.Benchmarks.ProblemSolutions;

namespace DSAExperimentation.Benchmarks.Tests.ProblemSolutions;

// Harness coverage for ContainVirusBenchmarks (ARCHITECTURE 17.9): its two arms are competing
// strategies for the same question - a naive recursive flood fill against the depth-first traversal
// - so a harness whose arms disagree is timing two different problems. Setup draws the grid from one
// fixed seed, so the same Side must rebuild the same grid; otherwise two published numbers were
// never comparable in the first place.
//
// The grid is private, so the workload's documented shape is asserted through the walls each arm
// reports: LC 749's answer counts walls built around quarantined regions, so it is never negative
// and never more than one wall per side of every cell in the grid.
public sealed partial class ContainVirusBenchmarksTests
{
    private const int SmallestSide = 10;
    private const int WallsPerCell = 4;

    [Fact]
    public void Setup_SameSide_RebuildsTheSameInfectionGrid()
    {
        Assert.InRange(
            BuildHarness().NaiveRecursiveFloodFill(),
            0,
            SmallestSide * SmallestSide * WallsPerCell);

        Assert.Equal(BuildHarness().NaiveRecursiveFloodFill(), BuildHarness().NaiveRecursiveFloodFill());
    }

    [Fact]
    public void NaiveRecursiveFloodFill_SeededInfectionGrid_AgreesWithDepthFirstSearchTraversal()
    {
        var harness = BuildHarness();

        Assert.Equal(harness.DepthFirstSearchTraversal(), harness.NaiveRecursiveFloodFill());
    }

    [Fact]
    public void DepthFirstSearchTraversal_SeededInfectionGrid_AgreesWithNaiveRecursiveFloodFill()
    {
        var harness = BuildHarness();

        Assert.Equal(harness.NaiveRecursiveFloodFill(), harness.DepthFirstSearchTraversal());
    }

    private static ContainVirusBenchmarks BuildHarness()
    {
        var harness = new ContainVirusBenchmarks { Side = SmallestSide };
        harness.Setup();

        return harness;
    }
}
