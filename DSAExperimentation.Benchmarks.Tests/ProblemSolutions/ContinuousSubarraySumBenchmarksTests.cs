using DSAExperimentation.Benchmarks.ProblemSolutions;

namespace DSAExperimentation.Benchmarks.Tests.ProblemSolutions;

// Harness coverage for ContinuousSubarraySumBenchmarks (ARCHITECTURE 17.9): BenchmarkArmsTests
// already holds the arms to agreeing on a rebuilt workload, so this asserts only the verdict the
// workload's documented shape decides: the divisor exceeds any possible total sum of the generated
// values, so no subarray can be a multiple of it, which is what forces both strategies through
// their full worst-case scan.
public sealed partial class ContinuousSubarraySumBenchmarksTests
{
    private const int SmallestLength = 200;

    [Fact]
    public void HasSubarraySumMultipleOfKByHashMapPrefixRemainder_ValuesBelowTheDivisor_ReturnsFalse() =>
        Assert.False(BuildHarness().HasSubarraySumMultipleOfKByHashMapPrefixRemainder());

    private static ContinuousSubarraySumBenchmarks BuildHarness()
    {
        var harness = new ContinuousSubarraySumBenchmarks { Length = SmallestLength };
        harness.Setup();

        return harness;
    }
}
