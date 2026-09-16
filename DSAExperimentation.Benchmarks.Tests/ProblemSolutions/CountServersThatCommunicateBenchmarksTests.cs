using DSAExperimentation.Benchmarks.ProblemSolutions;

namespace DSAExperimentation.Benchmarks.Tests.ProblemSolutions;

// Harness coverage for CountServersThatCommunicateBenchmarks (ARCHITECTURE 17.9): its two arms are
// competing strategies for the same question - rescanning each server's row and column against the
// two-pass HashMap tally - so a harness whose arms disagree is timing two different problems. Setup
// seeds the grid, so the same Size must rebuild the same grid.
public sealed partial class CountServersThatCommunicateBenchmarksTests
{
    private const int SmallestSize = 100;

    // The answer can never exceed the number of servers, itself bounded by the grid's own cells.
    private const int MostGridCells = SmallestSize * SmallestSize;

    [Fact]
    public void Setup_SmallestSize_RebuildsTheSameWorkload()
    {
        var first = BuildHarness();
        var second = BuildHarness();

        // The documented shape: a cell becomes a server with 1-in-4 odds, so every row of a hundred
        // holds servers and many of them share a row or column with another.
        Assert.InRange(first.BruteForceRowAndColumnRescan(), 1, MostGridCells);
        Assert.Equal(first.BruteForceRowAndColumnRescan(), second.BruteForceRowAndColumnRescan());
    }

    [Fact]
    public void BruteForceRowAndColumnRescan_QuarterDensityGrid_AgreesWithHashMapRowAndColumnCounts()
    {
        var harness = BuildHarness();

        Assert.Equal(harness.HashMapRowAndColumnCounts(), harness.BruteForceRowAndColumnRescan());
    }

    [Fact]
    public void HashMapRowAndColumnCounts_QuarterDensityGrid_AgreesWithBruteForceRowAndColumnRescan()
    {
        var harness = BuildHarness();

        Assert.Equal(harness.BruteForceRowAndColumnRescan(), harness.HashMapRowAndColumnCounts());
    }

    private static CountServersThatCommunicateBenchmarks BuildHarness()
    {
        var harness = new CountServersThatCommunicateBenchmarks { Size = SmallestSize };
        harness.Setup();

        return harness;
    }
}
