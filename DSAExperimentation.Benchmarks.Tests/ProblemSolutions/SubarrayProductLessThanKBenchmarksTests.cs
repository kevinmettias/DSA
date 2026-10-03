using DSAExperimentation.Benchmarks.ProblemSolutions;

namespace DSAExperimentation.Benchmarks.Tests.ProblemSolutions;

// Harness coverage for SubarrayProductLessThanKBenchmarks (ARCHITECTURE 17.9). Arm agreement is
// BenchmarkArmsTests' job; this pins the count the workload is built to have. The fixture's values
// are all 1 and K is 2, so every subarray qualifies: the count lands on a value the fixture's own
// structure fixes, not on whatever the arms happen to say.
public sealed partial class SubarrayProductLessThanKBenchmarksTests
{
    private const int SmallestLength = 200;

    // Every one of the SmallestLength * (SmallestLength + 1) / 2 contiguous subarrays has
    // product 1, which is below K, so all of them are counted.
    private const int ExpectedSubarrayCount = SmallestLength * (SmallestLength + 1) / 2;

    [Fact]
    public void BruteForce_AllOnesUnderTwo_CountsEverySubarray() =>
        Assert.Equal(ExpectedSubarrayCount, BuildHarness().BruteForce());

    [Fact]
    public void SlidingWindow_AllOnesUnderTwo_CountsEverySubarray() =>
        Assert.Equal(ExpectedSubarrayCount, BuildHarness().SlidingWindow());

    private static SubarrayProductLessThanKBenchmarks BuildHarness()
    {
        var harness = new SubarrayProductLessThanKBenchmarks { Length = SmallestLength };
        harness.Setup();

        return harness;
    }
}
