using DSAExperimentation.Benchmarks.StrategySwaps;

namespace DSAExperimentation.Benchmarks.Tests.StrategySwaps;

// Harness coverage for XorMultisetBenchmarks (ARCHITECTURE 17.9): BenchmarkArmsTests already holds
// the two multisets to agreeing, so this asserts what it cannot, a range for the answer that comes
// from the workload's shape rather than from either arm.
public sealed partial class XorMultisetBenchmarksTests
{
    private const int SmallestValueCount = 500;

    // The values are drawn below 2^20 and the window holds a quarter of them.
    private const long ValueBound = 1L << 20;
    private const int WindowShare = 4;

    // Once the window holds 64 values, a value whose bit 19 matches all 64 others turns up once in
    // 2^64 draws: from then on every largest XOR sets bit 19. Each step also counts at least the
    // new value itself below the limit, its XOR with itself being 0.
    private const int StepsBeforeBit19IsCertain = 64;
    private const long Bit19 = 1L << 19;

    [Fact]
    public void PooledArrays_ValuesBelowTwoToTheTwenty_SumsWithinTheWindowsBounds() =>
        Assert.InRange(
            BuildHarness().PooledArrays(),
            ((SmallestValueCount - StepsBeforeBit19IsCertain) * Bit19) + SmallestValueCount,
            (SmallestValueCount * (ValueBound - 1))
                + (SmallestValueCount * (SmallestValueCount / WindowShare)));

    private static XorMultisetBenchmarks BuildHarness()
    {
        var harness = new XorMultisetBenchmarks { ValueCount = SmallestValueCount };
        harness.Setup();

        return harness;
    }
}
