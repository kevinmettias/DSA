using DSAExperimentation.Benchmarks.ProblemSolutions;

namespace DSAExperimentation.Benchmarks.Tests.ProblemSolutions;

// Harness coverage for BestTimeToBuyAndSellStockBenchmarks (ARCHITECTURE 17.9): the generic check holds the arms
// to each other, and this asserts the profit is non-negative: buying and selling on the same day is always
// available, so a negative answer could only come from an arm that paired the days the wrong way round.
public sealed partial class BestTimeToBuyAndSellStockBenchmarksTests
{
    // The smaller of Setup's [Params(200, 5_000)] price-history lengths.
    private const int SmallestLength = 200;

    [Fact]
    public void BruteForce_TwoHundredDayHistory_ReturnsANonNegativeProfit() =>
        Assert.True(BuildHarness().BruteForce() >= 0);

    private static BestTimeToBuyAndSellStockBenchmarks BuildHarness()
    {
        var harness = new BestTimeToBuyAndSellStockBenchmarks { Length = SmallestLength };
        harness.Setup();

        return harness;
    }
}
