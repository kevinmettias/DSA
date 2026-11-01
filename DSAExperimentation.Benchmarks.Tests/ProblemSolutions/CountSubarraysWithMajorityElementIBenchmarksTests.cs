using DSAExperimentation.Benchmarks.ProblemSolutions;

namespace DSAExperimentation.Benchmarks.Tests.ProblemSolutions;

// Harness coverage for CountSubarraysWithMajorityElementIBenchmarks (ARCHITECTURE 17.9):
// BenchmarkArmsTests already holds the arms to agreeing on a rebuilt workload, so this asserts only
// a bound that comes from the workload's shape rather than from either arm.
//
// The generated array is private and the subarray count is the only thing either arm reports, so the
// documented shape is asserted through that: the answer counts subarrays of a Length-long array, so
// it can never exceed Length * (Length + 1) / 2.
public sealed partial class CountSubarraysWithMajorityElementIBenchmarksTests
{
    private const int SmallestLength = 200;

    private const int SubarrayCount = SmallestLength * (SmallestLength + 1) / 2;

    [Fact]
    public void BruteForce_SmallestLength_StaysWithinTheSubarrayCount() =>
        Assert.InRange(BuildHarness().BruteForce(), 0, SubarrayCount);

    private static CountSubarraysWithMajorityElementIBenchmarks BuildHarness()
    {
        var harness = new CountSubarraysWithMajorityElementIBenchmarks { Length = SmallestLength };
        harness.Setup();

        return harness;
    }
}
