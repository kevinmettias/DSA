using DSAExperimentation.Benchmarks.ProblemSolutions;

namespace DSAExperimentation.Benchmarks.Tests.ProblemSolutions;

// Harness coverage for BestTimeToBuyAndSellStockIVBenchmarks (ARCHITECTURE 17.9): its two arms are
// competing strategies for the same question - the unmemoized two-way choice tree per day against
// the (day, holding, transactions) memoization - so a harness whose arms disagree is timing two
// different problems. Both arms read the same private K, so the comparison also pins that the two
// strategies were handed the same transaction budget. Setup draws the prices from one fixed seed,
// so the same Length must rebuild the same workload.
public sealed partial class BestTimeToBuyAndSellStockIVBenchmarksTests
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

    private static BestTimeToBuyAndSellStockIVBenchmarks BuildHarness()
    {
        var harness = new BestTimeToBuyAndSellStockIVBenchmarks { Length = SmallestLength };
        harness.Setup();

        return harness;
    }
}
