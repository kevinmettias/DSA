using DSAExperimentation.Benchmarks.ProblemSolutions;

namespace DSAExperimentation.Benchmarks.Tests.ProblemSolutions;

// Harness coverage for DeleteGreatestValueInEachRowBenchmarks (ARCHITECTURE 17.9): its two arms are
// competing strategies for the same question - repeated "find this row's current maximum" rounds
// against sorting each row once and taking a column-wise maximum pass - so a harness whose arms
// disagree is timing two different problems. Setup draws the grid from one fixed seed, and neither
// strategy writes to it, so every call sees the same grid. Each round removes one value per row and
// every value lies inside LC 2500's 1..100 range, so the reading's documented shape is between one
// smallest and one largest value per column; the same Columns must rebuild the same grid and with it
// the same total.
public sealed partial class DeleteGreatestValueInEachRowBenchmarksTests
{
    private const int SmallestColumns = 5;

    // [GlobalSetup] draws every cell from LC 2500's range.
    private const int LowestValue = 1;
    private const int HighestValue = 100;

    // One value is removed per column per round, so the total is one in-range value per column.
    private const int MinimumRemovedValueTotal = SmallestColumns * LowestValue;
    private const int MaximumRemovedValueTotal = SmallestColumns * HighestValue;

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
    public void RepeatedRowMaxScan_TwentyByFiveSeededGrid_AgreesWithMergeSortColumnMax()
    {
        var harness = BuildHarness();

        Assert.Equal(harness.MergeSortColumnMax(), harness.RepeatedRowMaxScan());
    }

    [Fact]
    public void MergeSortColumnMax_TwentyByFiveSeededGrid_AgreesWithRepeatedRowMaxScan()
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
