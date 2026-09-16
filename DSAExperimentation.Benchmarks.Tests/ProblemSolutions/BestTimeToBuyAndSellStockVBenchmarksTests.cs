using DSAExperimentation.Benchmarks.ProblemSolutions;

namespace DSAExperimentation.Benchmarks.Tests.ProblemSolutions;

// Harness coverage for BestTimeToBuyAndSellStockVBenchmarks (ARCHITECTURE 17.9): its two arms are
// competing strategies for the same question - the unmemoized three-way choice tree per day against
// the (day, used, position) memoization - so a harness whose arms disagree is timing two different
// problems. Both arms read the same private K, so the comparison also pins that the two strategies
// were handed the same transaction budget. Length stays small because the baseline's blowup is real.
public sealed partial class BestTimeToBuyAndSellStockVBenchmarksTests
{
    private const int SmallestLength = 10;

    [Fact]
    public void Setup_SameLength_RebuildsTheSameWorkload() =>
        Assert.Equal(BuildHarness().BruteForce(), BuildHarness().BruteForce());

    [Fact]
    public void BruteForce_ChoiceTreePerDay_AgreesWithTransactionMemoization()
    {
        var harness = BuildHarness();

        Assert.Equal(harness.TransactionMemoization(), harness.BruteForce());
    }

    [Fact]
    public void TransactionMemoization_SeededPriceRun_AgreesWithBruteForce()
    {
        var harness = BuildHarness();

        Assert.Equal(harness.BruteForce(), harness.TransactionMemoization());
    }

    private static BestTimeToBuyAndSellStockVBenchmarks BuildHarness()
    {
        var harness = new BestTimeToBuyAndSellStockVBenchmarks { Length = SmallestLength };
        harness.Setup();

        return harness;
    }
}
