using DSAExperimentation.Benchmarks.ProblemSolutions;

namespace DSAExperimentation.Benchmarks.Tests.ProblemSolutions;

// Harness coverage for CountGoodSubarraysBenchmarks (ARCHITECTURE 17.9): BenchmarkArmsTests already
// holds the arms to agreeing on a rebuilt workload, so this asserts only a bound that comes from the
// problem rather than from either arm: every single-element subarray has OR == max == its own value,
// which the problem's predicate admits, so no admissible answer can fall below the number of starting
// points.
public sealed partial class CountGoodSubarraysBenchmarksTests
{
    private const int SmallestLength = 200;

    [Fact]
    public void RunningOrGroups_TwoHundredValues_CountsAtLeastEveryStartingPoint() =>
        Assert.True(BuildHarness().RunningOrGroups() >= SmallestLength);

    private static CountGoodSubarraysBenchmarks BuildHarness()
    {
        var harness = new CountGoodSubarraysBenchmarks { Length = SmallestLength };
        harness.Setup();

        return harness;
    }
}
