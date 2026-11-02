using DSAExperimentation.Benchmarks.ProblemSolutions;

namespace DSAExperimentation.Benchmarks.Tests.ProblemSolutions;

// Harness coverage for CountSubIslandsBenchmarks (ARCHITECTURE 17.9): BenchmarkArmsTests already
// holds the arms to agreeing, and to answering the same on a rebuilt harness and when called again
// on one, so this asserts what it cannot: a range for the answer that comes from the workload's
// shape rather than from either arm.
public sealed partial class CountSubIslandsBenchmarksTests
{
    private const int SmallestSide = 30;

    // The answer counts islands of grid2, so it can never exceed its cells.
    private const int MostGridCells = SmallestSide * SmallestSide;

    // The documented shape: grid1 is land-biased relative to grid2, so grid2's islands are a
    // real mix of ones grid1 covers completely and ones it does not - never an empty answer.
    [Fact]
    public void RecursiveFloodFill_LandBiasedGrid1_CountsWithinTheGridCells() =>
        Assert.InRange(BuildHarness().RecursiveFloodFill(), 1, MostGridCells);

    private static CountSubIslandsBenchmarks BuildHarness()
    {
        var harness = new CountSubIslandsBenchmarks { Side = SmallestSide };
        harness.Setup();

        return harness;
    }
}
