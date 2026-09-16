using DSAExperimentation.Benchmarks.ProblemSolutions;

namespace DSAExperimentation.Benchmarks.Tests.ProblemSolutions;

// Harness coverage for DeleteGreatestValueInEachRowBenchmarks (ARCHITECTURE 17.9): its two arms are
// competing strategies for the same question - repeated "find this row's current maximum" rounds
// against sorting each row once and taking a column-wise maximum pass - so a harness whose arms
// disagree is timing two different problems. Setup draws the grid from one fixed seed, and neither
// strategy writes to it, so every call sees the same grid. Each round removes one value per row and
// every value lies inside the seeded bound, so the reading's documented shape is at most one bound's
// worth of value per column; the same Columns must rebuild the same grid and with it the same total.
public sealed partial class DeleteGreatestValueInEachRowBenchmarksTests
{
    private const int SmallestColumns = 50;

    // [GlobalSetup] fixes the row count and the exclusive value bound.
    private const int Rows = 20;
    private const int ValueBound = 100_000;

    // One value is removed per column per round, so the total is one bounded value per column at most.
    private const int MinimumRemovedValueTotal = 0;
    private const int MaximumRemovedValueTotal = SmallestColumns * ValueBound;

    [Fact]
    public void Setup_SameColumns_RebuildsTheSameWorkload()
    {
        Assert.InRange(
            BuildHarness().RepeatedRowMaxScan(),
            MinimumRemovedValueTotal,
            MaximumRemovedValueTotal);
        Assert.Equal(BuildHarness().RepeatedRowMaxScan(), BuildHarness().RepeatedRowMaxScan());
    }

    [Fact]
    public void RepeatedRowMaxScan_TwentyByFiftySeededGrid_AgreesWithMergeSortColumnMax()
    {
        var harness = BuildHarness();

        Assert.Equal(harness.MergeSortColumnMax(), harness.RepeatedRowMaxScan());
    }

    [Fact]
    public void MergeSortColumnMax_TwentyByFiftySeededGrid_AgreesWithRepeatedRowMaxScan()
    {
        var harness = BuildHarness();

        Assert.Equal(harness.RepeatedRowMaxScan(), harness.MergeSortColumnMax());
    }

    private static DeleteGreatestValueInEachRowBenchmarks BuildHarness()
    {
        var harness = new DeleteGreatestValueInEachRowBenchmarks { Columns = SmallestColumns };
        harness.Setup();

        return harness;
    }
}
