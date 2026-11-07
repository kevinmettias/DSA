using DSAExperimentation.Benchmarks.StrategySwaps;

namespace DSAExperimentation.Benchmarks.Tests.StrategySwaps;

// Harness coverage for MaxXorSetQueryBenchmarks (ARCHITECTURE 17.9): BenchmarkArmsTests already
// holds the two sets to agreeing, so this asserts what it cannot, a range for the answer that comes
// from the workload's shape rather than from either arm.
public sealed partial class MaxXorSetQueryBenchmarksTests
{
    private const int SmallestValueCount = 50;

    // Bit 30 is the highest bit a non-negative int sets. Values spread over every non-negative int,
    // so among 50 of them each query finds one whose bit 30 differs from its own: every maximum XOR
    // sets bit 30, and none sets the sign bit.
    private const long LeastMaxXor = 1L << 30;

    [Fact]
    public void TwoFields_ValuesOverEveryNonNegativeInt_SumsMaxXorsThatEachSetBitThirty() =>
        Assert.InRange(BuildHarness().TwoFields(), SmallestValueCount * LeastMaxXor, SmallestValueCount * (long)int.MaxValue);

    private static MaxXorSetQueryBenchmarks BuildHarness()
    {
        var harness = new MaxXorSetQueryBenchmarks { ValueCount = SmallestValueCount };
        harness.Setup();

        return harness;
    }
}
