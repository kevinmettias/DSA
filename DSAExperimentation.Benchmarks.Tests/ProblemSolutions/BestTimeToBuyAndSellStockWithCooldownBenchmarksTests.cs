using DSAExperimentation.Benchmarks.ProblemSolutions;

namespace DSAExperimentation.Benchmarks.Tests.ProblemSolutions;

// Harness coverage for BestTimeToBuyAndSellStockWithCooldownBenchmarks (ARCHITECTURE 17.9): its two
// arms are competing strategies for the same question - the unmemoized buy/sell/cooldown choice tree
// against the same recursion memoized - so a harness whose arms disagree is timing two different
// problems. Setup draws the prices from one fixed seed, so the same Length must rebuild the same
// workload; otherwise two published numbers were never comparable in the first place. Length stays
// small because the baseline's blowup is real, which is why the test pays for the smallest Params.
public sealed partial class BestTimeToBuyAndSellStockWithCooldownBenchmarksTests
{
    private const int SmallestLength = 20;

    [Fact]
    public void Setup_SameLength_RebuildsTheSameWorkload() =>
        Assert.Equal(BuildHarness().UnmemoizedRecursion(), BuildHarness().UnmemoizedRecursion());

    [Fact]
    public void UnmemoizedRecursion_CooldownDayAfterEachSale_AgreesWithMemoizedRecursion()
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

    private static BestTimeToBuyAndSellStockWithCooldownBenchmarks BuildHarness()
    {
        var harness = new BestTimeToBuyAndSellStockWithCooldownBenchmarks { Length = SmallestLength };
        harness.Setup();

        return harness;
    }
}
