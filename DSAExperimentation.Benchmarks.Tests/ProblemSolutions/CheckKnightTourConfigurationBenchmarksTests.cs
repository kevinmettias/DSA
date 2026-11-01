using DSAExperimentation.Benchmarks.ProblemSolutions;

namespace DSAExperimentation.Benchmarks.Tests.ProblemSolutions;

// Harness coverage for CheckKnightTourConfigurationBenchmarks (ARCHITECTURE 17.9): BenchmarkArmsTests
// already holds the arms to agreeing on a rebuilt board, so this pins the baseline to the verdict
// the board itself decides, derived independently of either arm.
public sealed partial class CheckKnightTourConfigurationBenchmarksTests
{
    private const int SmallestBoardSize = 5;

    // The smallest board is the harness's own five-by-five tour literal, which the class documents as
    // a verified knight's tour that starts at the top-left cell: a permutation of 0..24 in which every
    // consecutive pair is a knight's move. Neither arm may therefore exit early on a bad pair, and the
    // workload has to answer yes.
    private const bool ExpectedVerdict = true;

    [Fact]
    public void IsValidGridByBoardRescan_FiveByFiveTour_ReturnsTrue() =>
        Assert.Equal(ExpectedVerdict, BuildHarness().IsValidGridByBoardRescan());

    private static CheckKnightTourConfigurationBenchmarks BuildHarness()
    {
        var harness = new CheckKnightTourConfigurationBenchmarks { BoardSize = SmallestBoardSize };
        harness.Setup();

        return harness;
    }
}
