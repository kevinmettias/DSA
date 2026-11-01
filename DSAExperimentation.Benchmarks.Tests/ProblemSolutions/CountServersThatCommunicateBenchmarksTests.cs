using DSAExperimentation.Benchmarks.ProblemSolutions;

namespace DSAExperimentation.Benchmarks.Tests.ProblemSolutions;

// Harness coverage for CountServersThatCommunicateBenchmarks (ARCHITECTURE 17.9): BenchmarkArmsTests
// already holds the arms to agreeing on a fresh harness, so this asserts what it cannot: a range for
// the answer that comes from the workload's shape rather than from either arm, and that a harness
// which has already answered once answers as a rebuild does, since BenchmarkDotNet calls an arm many
// times on one harness while the generic check builds a fresh harness for every call. Setup seeds
// the grid, so the same Size must rebuild the same grid.
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

    private static CountServersThatCommunicateBenchmarks BuildHarness()
    {
        var harness = new CountServersThatCommunicateBenchmarks { Size = SmallestSize };
        harness.Setup();

        return harness;
    }
}
