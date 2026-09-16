using DSAExperimentation.Benchmarks.ProblemSolutions;

namespace DSAExperimentation.Benchmarks.Tests.ProblemSolutions;

// Harness coverage for RightTrianglesBenchmarks (ARCHITECTURE 17.9): its two arms are competing
// strategies for the same question - scanning every row/column pair per occupied cell against a
// sweep that tallies each row's and each column's ones up front - so a harness whose arms disagree
// is timing two different problems. Setup asks the fixture for the grid at a fixed seed and size, so
// the same Size must rebuild the same grid; otherwise two published numbers were never comparable in
// the first place.
//
// Neither arm writes to the hoisted grid, so one harness is safe to call twice in either order and
// the single-harness rule holds. The fixture's one-density grid is not a shape whose triangle count
// can be derived without walking it, so the arms are reconciled against each other rather than
// against a re-derivation of the count.
public sealed partial class RightTrianglesBenchmarksTests
{
    private const int SmallestSize = 20;

    [Fact]
    public void Setup_SameSize_RebuildsTheSameGrid() =>
        Assert.Equal(BuildHarness().BruteForceRowColumnScan(), BuildHarness().BruteForceRowColumnScan());

    [Fact]
    public void BruteForceRowColumnScan_SeededGrid_AgreesWithTalliedRowsAndColumns()
    {
        var harness = BuildHarness();

        Assert.Equal(harness.TalliedRowsAndColumns(), harness.BruteForceRowColumnScan());
    }

    [Fact]
    public void TalliedRowsAndColumns_SeededGrid_AgreesWithBruteForceRowColumnScan()
    {
        var harness = BuildHarness();

        Assert.Equal(harness.BruteForceRowColumnScan(), harness.TalliedRowsAndColumns());
    }

    private static RightTrianglesBenchmarks BuildHarness()
    {
        var harness = new RightTrianglesBenchmarks { Size = SmallestSize };
        harness.Setup();

        return harness;
    }
}
