using DSAExperimentation.Benchmarks.ProblemSolutions;

namespace DSAExperimentation.Benchmarks.Tests.ProblemSolutions;

// Harness coverage for CountServersThatCommunicateBenchmarks (ARCHITECTURE 17.9): BenchmarkArmsTests
// already holds the arms to agreeing, and to answering the same on a rebuilt harness and when
// called again on one, so this asserts what it cannot: a range for the answer that comes from the
// workload's shape rather than from either arm.
public sealed partial class CountServersThatCommunicateBenchmarksTests
{
    private const int SmallestSize = 100;

    // The answer can never exceed the number of servers, itself bounded by the grid's own cells.
    private const int MostGridCells = SmallestSize * SmallestSize;

    // The documented shape: a cell becomes a server with 1-in-4 odds, so every row of a hundred
    // holds servers and many of them share a row or column with another.
    [Fact]
    public void BruteForceRowAndColumnRescan_OneInFourServers_CountsWithinTheGridCells() =>
        Assert.InRange(BuildHarness().BruteForceRowAndColumnRescan(), 1, MostGridCells);

    private static CountServersThatCommunicateBenchmarks BuildHarness()
    {
        var harness = new CountServersThatCommunicateBenchmarks { Size = SmallestSize };
        harness.Setup();

        return harness;
    }
}
