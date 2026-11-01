using DSAExperimentation.Benchmarks.ProblemSolutions;

namespace DSAExperimentation.Benchmarks.Tests.ProblemSolutions;

// Harness coverage for CountSubIslandsBenchmarks (ARCHITECTURE 17.9): BenchmarkArmsTests already
// holds the arms to agreeing on a fresh harness, so this asserts what it cannot: a range for the
// answer that comes from the workload's shape rather than from either arm, and that a harness which
// has already answered once answers as a rebuild does, since BenchmarkDotNet calls an arm many times
// on one harness while the generic check builds a fresh harness for every call. Setup seeds both
// grids, so the same Side must rebuild the same pair.
public sealed partial class CountSubIslandsBenchmarksTests
{
    private const int SmallestSide = 30;

    // The answer counts islands of grid2, so it can never exceed its cells.
    private const int MostGridCells = SmallestSide * SmallestSide;

    [Fact]
    public void Setup_SmallestSide_RebuildsTheSameWorkload()
    {
        var first = BuildHarness();
        var second = BuildHarness();

        // The documented shape: grid1 is land-biased relative to grid2, so grid2's islands are a
        // real mix of ones grid1 covers completely and ones it does not - never an empty answer.
        Assert.InRange(first.RecursiveFloodFill(), 1, MostGridCells);
        Assert.Equal(first.RecursiveFloodFill(), second.RecursiveFloodFill());
    }

    private static CountSubIslandsBenchmarks BuildHarness()
    {
        var harness = new CountSubIslandsBenchmarks { Side = SmallestSide };
        harness.Setup();

        return harness;
    }
}
