using DSAExperimentation.Benchmarks.ProblemSolutions;

namespace DSAExperimentation.Benchmarks.Tests.ProblemSolutions;

// Harness coverage for BestTimeToBuyAndSellStockBenchmarks (ARCHITECTURE 17.9): its two arms are
// BestTimeToBuyAndSellStockSolution's competing strategies for the same question - the every-pair scan against one
// pass tracking the running minimum - so a harness whose arms disagree has priced two different histories. Both
// return the one int profit, so they are compared directly rather than through a rendering, and the profit is
// asserted non-negative as well: buying and selling on the same day is always available, so a negative answer
// could only come from an arm that paired the days the wrong way round. Setup draws the prices from one seed, so
// the same Length must rebuild the same history.
public sealed partial class BestTimeToBuyAndSellStockBenchmarksTests
{
    // The smaller of Setup's [Params(200, 5_000)] price-history lengths.
    private const int SmallestLength = 200;

    [Fact]
    public void Setup_SameLength_RebuildsTheSameWorkload() =>
        Assert.Equal(BuildHarness().BruteForce(), BuildHarness().BruteForce());

    [Fact]
    public void BruteForce_TwoHundredDayHistory_AgreesWithOnePassMinTracking()
    {
        var harness = BuildHarness();

        Assert.True(harness.BruteForce() >= 0);
        Assert.Equal(harness.OnePassMinTracking(), harness.BruteForce());
    }

    [Fact]
    public void OnePassMinTracking_TwoHundredDayHistory_AgreesWithBruteForce()
    {
        var harness = BuildHarness();

        Assert.True(harness.OnePassMinTracking() >= 0);
        Assert.Equal(harness.BruteForce(), harness.OnePassMinTracking());
    }

    private static BestTimeToBuyAndSellStockBenchmarks BuildHarness()
    {
        var harness = new BestTimeToBuyAndSellStockBenchmarks { Length = SmallestLength };
        harness.Setup();

        return harness;
    }
}
