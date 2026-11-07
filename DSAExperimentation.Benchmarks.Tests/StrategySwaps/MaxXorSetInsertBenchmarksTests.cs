using DSAExperimentation.Benchmarks.StrategySwaps;

namespace DSAExperimentation.Benchmarks.Tests.StrategySwaps;

// Harness coverage for MaxXorSetInsertBenchmarks (ARCHITECTURE 17.9): the values are drawn
// distinct, so the baseline must hold every one of them, and the generic check's arm agreement
// carries that count to the one trie's set - a strategy for the layer may only cost time, never
// lose a value.
public sealed partial class MaxXorSetInsertBenchmarksTests
{
    private const int SmallestValueCount = 50;

    [Fact]
    public void TwoFields_DistinctValues_HoldsEveryOne() =>
        Assert.Equal(SmallestValueCount, BuildHarness().TwoFields());

    private static MaxXorSetInsertBenchmarks BuildHarness()
    {
        var harness = new MaxXorSetInsertBenchmarks { ValueCount = SmallestValueCount };
        harness.Setup();

        return harness;
    }
}
