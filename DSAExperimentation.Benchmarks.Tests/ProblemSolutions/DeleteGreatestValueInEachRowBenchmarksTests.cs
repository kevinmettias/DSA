using DSAExperimentation.Benchmarks.ProblemSolutions;

namespace DSAExperimentation.Benchmarks.Tests.ProblemSolutions;

// Harness coverage for DeleteGreatestValueInEachRowBenchmarks (ARCHITECTURE 17.9): the generic arm
// check compares the arms only with each other, so what stays here is a bound derived without either
// arm. Each round removes one value per row and every value lies inside LC 2500's 1..100 range, so the
// reading's documented shape is between one smallest and one largest value per column.
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
    public void RepeatedRowMaxScan_TwentyByFiveSeededGrid_RemovesOneInRangeValuePerColumn() =>
        Assert.InRange(
            BuildHarness().RepeatedRowMaxScan(),
            MinimumRemovedValueTotal,
            MaximumRemovedValueTotal);

    private static DeleteGreatestValueInEachRowBenchmarks BuildHarness()
    {
        var harness = new DeleteGreatestValueInEachRowBenchmarks { Columns = SmallestColumns };
        harness.Setup();

        return harness;
    }
}
