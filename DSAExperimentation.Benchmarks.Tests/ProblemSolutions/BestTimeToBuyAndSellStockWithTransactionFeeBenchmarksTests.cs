using DSAExperimentation.Benchmarks.ProblemSolutions;

namespace DSAExperimentation.Benchmarks.Tests.ProblemSolutions;

// Harness coverage for BestTimeToBuyAndSellStockWithTransactionFeeBenchmarks (ARCHITECTURE 17.9): its
// two arms are competing strategies for the same question - the unmemoized fee-charged choice tree
// against the same recursion memoized - so a harness whose arms disagree is timing two different
// problems. Both arms read the same private fee, so the comparison also pins that the two strategies
// were handed the same cost per transaction. Setup draws the prices from one fixed seed, so the same
// Length must rebuild the same workload.
public sealed partial class BestTimeToBuyAndSellStockWithTransactionFeeBenchmarksTests
{
    private const int SmallestLength = 20;

    [Fact]
    public void Setup_SameLength_RebuildsTheSameWorkload() =>
        Assert.Equal(BuildHarness().UnmemoizedRecursion(), BuildHarness().UnmemoizedRecursion());

    [Fact]
    public void UnmemoizedRecursion_FeeChargedPerSale_AgreesWithMemoizedRecursion()
    {
        var harness = BuildHarness();

        Assert.Equal(harness.MemoizedRecursion(), harness.UnmemoizedRecursion());
    }

    [Fact]
    public void MemoizedRecursion_SeededPriceRun_AgreesWithUnmemoizedRecursion()
    {
        var harness = BuildHarness();

        Assert.Equal(harness.UnmemoizedRecursion(), harness.MemoizedRecursion());
    }

    private static BestTimeToBuyAndSellStockWithTransactionFeeBenchmarks BuildHarness()
    {
        var harness = new BestTimeToBuyAndSellStockWithTransactionFeeBenchmarks { Length = SmallestLength };
        harness.Setup();

        return harness;
    }
}
