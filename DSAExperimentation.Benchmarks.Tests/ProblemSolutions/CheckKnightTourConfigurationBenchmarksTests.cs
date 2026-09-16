using DSAExperimentation.Benchmarks.ProblemSolutions;

namespace DSAExperimentation.Benchmarks.Tests.ProblemSolutions;

// Harness coverage for CheckKnightTourConfigurationBenchmarks (ARCHITECTURE 17.9): its two arms are
// competing strategies for the same question - rescanning the whole board to locate each move's cell
// against one lookup table built in a single pass - so a harness whose arms disagree is timing two
// different problems. Both arms answer with a bare bool, so agreement between them says the two
// strategies reached the same verdict on the same board.
public sealed partial class CheckKnightTourConfigurationBenchmarksTests
{
    private const int SmallestBoardSize = 5;

    // The smallest board is the harness's own five-by-five tour literal, which the class documents as
    // a verified knight's tour that starts at the top-left cell: a permutation of 0..24 in which every
    // consecutive pair is a knight's move. Neither arm may therefore exit early on a bad pair, and a
    // rebuilt workload has to answer yes.
    private const bool ExpectedVerdict = true;

    [Fact]
    public void Setup_SameBoardSize_RebuildsTheFiveByFiveTour()
    {
        var first = BuildHarness();
        var second = BuildHarness();

        Assert.Equal(ExpectedVerdict, first.IsValidGridByBoardRescan());
        Assert.Equal(ExpectedVerdict, second.IsValidGridByPositionLookup());
    }

    [Fact]
    public void IsValidGridByBoardRescan_FiveByFiveTour_AgreesWithPositionLookup()
    {
        var harness = BuildHarness();

        Assert.Equal(harness.IsValidGridByPositionLookup(), harness.IsValidGridByBoardRescan());
    }

    [Fact]
    public void IsValidGridByPositionLookup_FiveByFiveTour_AgreesWithBoardRescan()
    {
        var harness = BuildHarness();

        Assert.Equal(harness.IsValidGridByBoardRescan(), harness.IsValidGridByPositionLookup());
    }

    private static CheckKnightTourConfigurationBenchmarks BuildHarness()
    {
        var harness = new CheckKnightTourConfigurationBenchmarks { BoardSize = SmallestBoardSize };
        harness.Setup();

        return harness;
    }
}
