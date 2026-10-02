using DSAExperimentation.Benchmarks.ProblemSolutions;

namespace DSAExperimentation.Benchmarks.Tests.ProblemSolutions;

// Harness coverage for HouseRobberBenchmarks (ARCHITECTURE 17.9). Its two arms are competing
// strategies for the same question - the memoized recurrence and the rolling-totals pass - so a
// harness whose arms disagree is timing two different problems: both must report the same haul.
// The workload is LeetCode 198's own example street, whose best non-adjacent haul is 2 + 9 + 1.
// The class carries no [Params] and no [GlobalSetup]: the fixed five-house literal is the whole
// workload, so the same street must always report the same haul, stated literally rather than read
// back out of an arm.
public sealed partial class HouseRobberBenchmarksTests
{
    private const int ExpectedMaximumHaul = 12;

    [Fact]
    public void MemoizedRecursion_ExampleStreet_ReturnsTheBestNonAdjacentHaul()
    {
        var harness = BuildHarness();

        Assert.Equal(ExpectedMaximumHaul, harness.MemoizedRecursion());
    }

    [Fact]
    public void IterativeRollingTotals_ExampleStreet_ReturnsTheBestNonAdjacentHaul()
    {
        var harness = BuildHarness();

        Assert.Equal(ExpectedMaximumHaul, harness.IterativeRollingTotals());
    }

    [Fact]
    public void IterativeRollingTotals_AgreesWithMemoizedRecursion()
    {
        var harness = BuildHarness();

        Assert.Equal(harness.MemoizedRecursion(), harness.IterativeRollingTotals());
    }

    private static HouseRobberBenchmarks BuildHarness() => new();
}
