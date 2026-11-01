using DSAExperimentation.Benchmarks.ProblemSolutions;

namespace DSAExperimentation.Benchmarks.Tests.ProblemSolutions;

// Harness coverage for DiagonalTraverseBenchmarks (ARCHITECTURE 17.9): a property of the workload's
// shape. The matrix is Size x Size by construction, so the answer always holds exactly Size * Size
// entries.
public sealed partial class DiagonalTraverseBenchmarksTests
{
    private const int SmallestSize = 50;
    private const int ExpectedDiagonalOrderLength = SmallestSize * SmallestSize;

    [Fact]
    public void DirectionToggleWalk_SquareMatrix_HoldsOneEntryPerCell() =>
        Assert.Equal(ExpectedDiagonalOrderLength, BuildHarness().DirectionToggleWalk().Length);

    private static DiagonalTraverseBenchmarks BuildHarness()
    {
        var harness = new DiagonalTraverseBenchmarks { Size = SmallestSize };
        harness.Setup();

        return harness;
    }
}
