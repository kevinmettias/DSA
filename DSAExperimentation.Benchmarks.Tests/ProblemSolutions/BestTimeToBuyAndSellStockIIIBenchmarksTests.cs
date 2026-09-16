using DSAExperimentation.Benchmarks.ProblemSolutions;

namespace DSAExperimentation.Benchmarks.Tests.ProblemSolutions;

// Harness coverage for BestTimeToBuyAndSellStockIIIBenchmarks (ARCHITECTURE 17.9): its two arms are
// competing strategies for the same question - the unmemoized two-way choice tree per day against
// the (day, holding, transactions) memoization - so a harness whose arms disagree is timing two
// different problems. Setup draws the prices from one fixed seed, so the same Length must rebuild
// the same workload; otherwise two published numbers were never comparable in the first place.
public sealed partial class BestTimeToBuyAndSellStockIIIBenchmarksTests
{
    private const int SmallestLength = 16;

    [Fact]
    public void Setup_SameLength_RebuildsTheSameWorkload() =>
        Assert.Equal(BuildHarness().BruteForce(), BuildHarness().BruteForce());

    [Fact]
    public void BruteForce_AtMostTwoTransactions_AgreesWithTransactionMemoization()
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

    private static BestTimeToBuyAndSellStockIIIBenchmarks BuildHarness()
    {
        var harness = new BestTimeToBuyAndSellStockIIIBenchmarks { Length = SmallestLength };
        harness.Setup();

        return harness;
    }
}
