using DSAExperimentation.Benchmarks.ProblemSolutions;

namespace DSAExperimentation.Benchmarks.Tests.ProblemSolutions;

// Harness coverage for MinimumNumberOfArrowsToBurstBalloonsBenchmarks (ARCHITECTURE 17.9): both
// arms are MinimumNumberOfArrowsToBurstBalloonsSolution's, the same methods
// MinimumNumberOfArrowsToBurstBalloonsTests proves correct, and both return the fewest arrows
// that burst every balloon. The brute-force rescan and the sort-ends-then-scan greedy are
// competing strategies for that one number, and both are handed the prepared (start, end) pairs
// rather than LeetCode's raw int[][], so they read the identical input.
public sealed partial class MinimumNumberOfArrowsToBurstBalloonsBenchmarksTests
{
    // The smallest declared [Params] value: the generated balloons are mostly non-overlapping,
    // so a shorter list already puts the brute force through its full rescan.
    private const int SmallestLength = 200;

    [Fact]
    public void Setup_SameParametersTwice_ProduceTheSameAnswer() =>
        Assert.Equal(
            BuildHarness().BruteForceRescan(),
            BuildHarness().BruteForceRescan());

    [Fact]
    public void BruteForceRescan_AgreesWithSortEndsThenGreedyScan()
    {
        var harness = BuildHarness();

        Assert.Equal(harness.SortEndsThenGreedyScan(), harness.BruteForceRescan());
    }

    [Fact]
    public void SortEndsThenGreedyScan_AgreesWithBruteForceRescan()
    {
        var harness = BuildHarness();

        Assert.Equal(harness.BruteForceRescan(), harness.SortEndsThenGreedyScan());
    }

    private static MinimumNumberOfArrowsToBurstBalloonsBenchmarks BuildHarness()
    {
        var harness = new MinimumNumberOfArrowsToBurstBalloonsBenchmarks { Length = SmallestLength };
        harness.Setup();

        return harness;
    }
}
