using DSAExperimentation.Benchmarks.ProblemSolutions;

namespace DSAExperimentation.Benchmarks.Tests.ProblemSolutions;

// Harness coverage for DetectSquaresBenchmarks (ARCHITECTURE 17.9), for what BenchmarkArmsTests cannot pin: how many
// squares the queries find, known from Setup's construction rather than from either arm. Setup builds a full lattice
// from GridDimension alone, and each arm adds every lattice point once, then queries every point once, returning
// each query's count.
public sealed partial class DetectSquaresBenchmarksTests
{
    private const int SmallestGridDimension = 5;

    private const int CornersPerSquare = 4;

    [Fact]
    public void ListBased_FullLatticeQueries_FindsEverySquareOncePerCorner() =>
        Assert.Equal(ExpectedCornerTally(SmallestGridDimension), BuildHarness().ListBased().Sum(count => (long)count));

    [Fact]
    public void HashMapGroupedByX_FullLatticeQueries_FindsEverySquareOncePerCorner() =>
        Assert.Equal(ExpectedCornerTally(SmallestGridDimension), BuildHarness().HashMapGroupedByX().Sum(count => (long)count));

    private static DetectSquaresBenchmarks BuildHarness()
    {
        var harness = new DetectSquaresBenchmarks { GridDimension = SmallestGridDimension };
        harness.Setup();

        return harness;
    }

    // Every lattice point is added once and then queried once, so the counts together tally every
    // axis-aligned square of the lattice once per corner - the only way a query can report a square
    // here, since every point it could name is present; a sparse or mismatched point set would come
    // in under that tally. A d-by-d lattice holds the squares of every side s from 1 to d - 1, each in
    // (d - s)^2 positions, and each square has CornersPerSquare corners.
    private static long ExpectedCornerTally(int dimension) =>
        CornersPerSquare * Enumerable.Range(1, dimension - 1).Sum(side => (long)side * side);
}
