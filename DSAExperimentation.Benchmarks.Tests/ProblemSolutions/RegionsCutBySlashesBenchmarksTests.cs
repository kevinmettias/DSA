using DSAExperimentation.Benchmarks.ProblemSolutions;

namespace DSAExperimentation.Benchmarks.Tests.ProblemSolutions;

// Harness coverage for RegionsCutBySlashesBenchmarks (ARCHITECTURE 17.9): both arms are
// RegionsCutBySlashesSolution's, competing strategies for the same question - the 3x3 subgrid
// expansion flood-fills a 9n^2-cell grid, the composed arm unions four triangles per cell and counts
// distinct roots - so a harness whose arms disagree counts two different numbers of regions. Setup
// builds the grid from one seeded fixture, so the same GridSize must rebuild the same slashes.
public sealed partial class RegionsCutBySlashesBenchmarksTests
{
    private const int SmallestGridSize = 30;

    [Fact]
    public void Setup_SameGridSize_RebuildsTheSameWorkload() =>
        Assert.Equal(
            BuildHarness().ThreeByThreeExpansionFloodFill(),
            BuildHarness().ThreeByThreeExpansionFloodFill());

    [Fact]
    public void ThreeByThreeExpansionFloodFill_AgreesWithDisjointSetTriangleUnion()
    {
        var harness = BuildHarness();

        Assert.Equal(harness.DisjointSetTriangleUnion(), harness.ThreeByThreeExpansionFloodFill());
    }

    [Fact]
    public void DisjointSetTriangleUnion_AgreesWithThreeByThreeExpansionFloodFill()
    {
        var harness = BuildHarness();

        Assert.Equal(harness.ThreeByThreeExpansionFloodFill(), harness.DisjointSetTriangleUnion());
    }

    private static RegionsCutBySlashesBenchmarks BuildHarness()
    {
        var harness = new RegionsCutBySlashesBenchmarks { GridSize = SmallestGridSize };
        harness.Setup();

        return harness;
    }
}
