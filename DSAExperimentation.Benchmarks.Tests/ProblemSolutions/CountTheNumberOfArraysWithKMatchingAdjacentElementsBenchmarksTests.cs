using DSAExperimentation.Benchmarks.ProblemSolutions;

namespace DSAExperimentation.Benchmarks.Tests.ProblemSolutions;

// Harness coverage for CountTheNumberOfArraysWithKMatchingAdjacentElementsBenchmarks
// (ARCHITECTURE 17.9): BenchmarkArmsTests already holds the arms to agreeing on a rebuilt workload,
// so this asserts only a bound that comes from the workload's shape rather than from either arm.
//
// The match count is private and the array count is the only thing either arm reports, so the
// documented shape is asserted through that: the answer counts length-n arrays over a 2-value
// alphabet, and there are only 2^n such arrays in total.
public sealed partial class CountTheNumberOfArraysWithKMatchingAdjacentElementsBenchmarksTests
{
    private const int SmallestArrayLength = 10;

    private const long ArrayCount = 1L << SmallestArrayLength;

    [Fact]
    public void BruteForce_SmallestArrayLength_StaysWithinTheArrayCount() =>
        Assert.InRange(BuildHarness().BruteForce(SmallestArrayLength), 0, ArrayCount);

    private static CountTheNumberOfArraysWithKMatchingAdjacentElementsBenchmarks BuildHarness()
    {
        var harness = new CountTheNumberOfArraysWithKMatchingAdjacentElementsBenchmarks();
        harness.Setup();

        return harness;
    }
}
