using DSAExperimentation.Benchmarks.ProblemSolutions;

namespace DSAExperimentation.Benchmarks.Tests.ProblemSolutions;

// Harness coverage for BestTimeToBuyAndSellStockIIBenchmarks (ARCHITECTURE 17.9): its two arms are
// competing strategies for the same question - the two-way buy/hold choice tree per day against the
// greedy ascent - so a harness whose arms disagree is timing two different problems. Setup draws
// the prices from one fixed seed, so the same Length must rebuild the same workload; otherwise two
// published numbers were never comparable in the first place.
public sealed partial class BestTimeToBuyAndSellStockIIBenchmarksTests
{
    private const int SmallestLength = 16;

    [Fact]
    public void Setup_SameLength_RebuildsTheSameWorkload() =>
        Assert.Equal(BuildHarness().BruteForce(), BuildHarness().BruteForce());

    [Fact]
    public void BruteForce_ChoiceTreePerDay_AgreesWithGreedyAscent()
    {
        var harness = BuildHarness();

        Assert.Equal(harness.GreedyAscent(), harness.BruteForce());
    }

    [Fact]
    public void GreedyAscent_SeededPriceRun_AgreesWithBruteForce()
    {
        var harness = BuildHarness();

        Assert.Equal(harness.BruteForce(), harness.GreedyAscent());
    }

    private static BestTimeToBuyAndSellStockIIBenchmarks BuildHarness()
    {
        var harness = new BestTimeToBuyAndSellStockIIBenchmarks { Length = SmallestLength };
        harness.Setup();

        return harness;
    }
}
