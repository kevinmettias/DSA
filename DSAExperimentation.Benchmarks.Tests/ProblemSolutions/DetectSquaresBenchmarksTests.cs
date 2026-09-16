using DSAExperimentation.Benchmarks.ProblemSolutions;

namespace DSAExperimentation.Benchmarks.Tests.ProblemSolutions;

// Harness coverage for DetectSquaresBenchmarks (ARCHITECTURE 17.9): its two arms are competing
// strategies for the same question - rescanning the whole point list per candidate corner against
// this repo's own points-grouped-by-x map - so a harness whose arms disagree is timing two
// different problems. Setup builds a full lattice from GridDimension alone, so the same
// GridDimension must rebuild the same points, and that lattice has to be dense enough that the
// grouped-by-x arm's index is what the pairing is actually measuring.
public sealed partial class DetectSquaresBenchmarksTests
{
    private const int SmallestGridDimension = 5;

    private const int CornersPerSquare = 4;

    [Fact]
    public void Setup_SameGridDimension_RebuildsTheSameLattice()
    {
        // Every lattice point is added once and then queried once, so the summed answer counts
        // every axis-aligned square of the lattice once per corner - the only way a query can
        // report a square here, since every point it could name is present. A sparse or
        // mismatched point set would come in under that tally.
        Assert.Equal(ExpectedCornerTally(SmallestGridDimension), BuildHarness().ListBased());
        Assert.Equal(BuildHarness().ListBased(), BuildHarness().ListBased());
    }

    [Fact]
    public void ListBased_FullLatticeQueries_AgreesWithHashMapGroupedByX()
    {
        var harness = BuildHarness();

        Assert.Equal(harness.HashMapGroupedByX(), harness.ListBased());
    }

    [Fact]
    public void HashMapGroupedByX_FullLatticeQueries_AgreesWithListBased()
    {
        var harness = BuildHarness();

        Assert.Equal(harness.ListBased(), harness.HashMapGroupedByX());
    }

    private static DetectSquaresBenchmarks BuildHarness()
    {
        var harness = new DetectSquaresBenchmarks { GridDimension = SmallestGridDimension };
        harness.Setup();

        return harness;
    }

    // A d-by-d lattice holds the squares of every side s from 1 to d - 1, each in (d - s)^2
    // positions, and each square has CornersPerSquare corners - so querying every point in turn
    // reports that many times the square count.
    private static long ExpectedCornerTally(int dimension) =>
        CornersPerSquare * Enumerable.Range(1, dimension - 1).Sum(side => (long)side * side);
}
