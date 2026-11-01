using DSAExperimentation.Benchmarks.ProblemSolutions;

namespace DSAExperimentation.Benchmarks.Tests.ProblemSolutions;

// Harness coverage for FallingSquaresBenchmarks (ARCHITECTURE 17.9): a property of the workload's
// shape. LeetCode asks for the running stack height after each drop, so the answer holds one height
// per square Setup built.
public sealed partial class FallingSquaresBenchmarksTests
{
    private const int SmallestSquareCount = 100;

    [Fact]
    public void BruteForceOverlapScan_OverlappingSquares_ReportsOneHeightPerSquare() =>
        Assert.Equal(SmallestSquareCount, BuildHarness().BruteForceOverlapScan().Count);

    private static FallingSquaresBenchmarks BuildHarness()
    {
        var harness = new FallingSquaresBenchmarks { SquareCount = SmallestSquareCount };
        harness.Setup();

        return harness;
    }
}
