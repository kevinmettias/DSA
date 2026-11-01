using DSAExperimentation.Benchmarks.StrategySwaps;

namespace DSAExperimentation.Benchmarks.Tests.StrategySwaps;

// Harness coverage for BufferCapacityBenchmarks (ARCHITECTURE 17.9): step i pops back and peeks the i
// it just pushed at each end, so the baseline must answer 2 * (0 + 1 + ... + n - 1) = n(n - 1), and
// the generic check's arm agreement carries that to the bounded arm - a bound the churn never reaches
// may only cost time, never refuse a push.
public sealed partial class BufferCapacityBenchmarksTests
{
    private const int SmallestOperationCount = 1_000;

    [Fact]
    public void Unbounded_Churn_SumsEveryStepTwice() =>
        Assert.Equal((long)SmallestOperationCount * (SmallestOperationCount - 1), BuildHarness().Unbounded());

    private static BufferCapacityBenchmarks BuildHarness() => new() { OperationCount = SmallestOperationCount };
}
