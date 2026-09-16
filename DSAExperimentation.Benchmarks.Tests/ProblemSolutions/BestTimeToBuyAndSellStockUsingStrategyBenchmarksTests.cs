using DSAExperimentation.Benchmarks.ProblemSolutions;

namespace DSAExperimentation.Benchmarks.Tests.ProblemSolutions;

// Harness coverage for BestTimeToBuyAndSellStockUsingStrategyBenchmarks (ARCHITECTURE 17.9): its two
// arms are competing strategies for the same question - re-summing each window against sliding the
// running window sum - so a harness whose arms disagree is timing two different problems. Both arms
// read the same private window width, and the workload fixture draws prices and strategy from one
// fixed seed, so the same DayCount must rebuild the same workload.
public sealed partial class BestTimeToBuyAndSellStockUsingStrategyBenchmarksTests
{
    private const int SmallestDayCount = 1_000;

    [Fact]
    public void Setup_SameDayCount_RebuildsTheSameWorkload() =>
        Assert.Equal(BuildHarness().BruteForceWindowSum(), BuildHarness().BruteForceWindowSum());

    [Fact]
    public void BruteForceWindowSum_OverTheWholeRun_AgreesWithSlidingWindowSum()
    {
        var harness = BuildHarness();

        Assert.Equal(harness.SlidingWindowSum(), harness.BruteForceWindowSum());
    }

    [Fact]
    public void SlidingWindowSum_OverTheWholeRun_AgreesWithBruteForceWindowSum()
    {
        var harness = BuildHarness();

        Assert.Equal(harness.BruteForceWindowSum(), harness.SlidingWindowSum());
    }

    private static BestTimeToBuyAndSellStockUsingStrategyBenchmarks BuildHarness()
    {
        var harness = new BestTimeToBuyAndSellStockUsingStrategyBenchmarks { DayCount = SmallestDayCount };
        harness.Setup();

        return harness;
    }
}
