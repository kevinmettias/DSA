using DSAExperimentation.Benchmarks.ProblemSolutions;

namespace DSAExperimentation.Benchmarks.Tests.ProblemSolutions;

// Harness coverage for CountKReducibleNumbersLessThanNBenchmarks (ARCHITECTURE 17.9):
// BenchmarkArmsTests already holds the arms to agreeing, and to answering the same on a rebuilt
// harness and when called again on one, so this asserts what it cannot: a range for the answer
// that comes from the workload's shape rather than from either arm.
public sealed partial class CountKReducibleNumbersLessThanNBenchmarksTests
{
    private const int SmallestLength = 16;

    // Setup always sets the leading bit, so n has exactly SmallestLength bits and the count
    // ranges over [1, n): the answer can never leave [1, 2^SmallestLength - 1], because x = 1
    // needs no popcount steps at all and so is always k-reducible.
    [Fact]
    public void BruteForce_LeadingBitSet_CountsBetweenOneAndNMinusOne() =>
        Assert.InRange(BuildHarness().BruteForce(), 1, (1 << SmallestLength) - 1);

    private static CountKReducibleNumbersLessThanNBenchmarks BuildHarness()
    {
        var harness = new CountKReducibleNumbersLessThanNBenchmarks { Length = SmallestLength };
        harness.Setup();

        return harness;
    }
}
