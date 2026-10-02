using DSAExperimentation.Benchmarks.ProblemSolutions;

namespace DSAExperimentation.Benchmarks.Tests.ProblemSolutions;

// Harness coverage for HouseRobberIIBenchmarks (ARCHITECTURE 17.9). Its two arms are competing
// strategies for the same question - the memoized circular split and the iterative two-pass split -
// so a harness whose arms disagree is timing two different problems: both must report the same
// haul. The workload is LeetCode 213's own example, a circle of four houses that can only be robbed
// from one side of the wrap-around, for a best haul of 1 + 3. The class carries no [Params] and no
// [GlobalSetup]: the fixed four-house literal is the whole workload, so the same circle must always
// report the same haul, stated literally rather than read back out of an arm.
public sealed partial class HouseRobberIIBenchmarksTests
{
    private const int ExpectedMaximumHaul = 4;

    [Fact]
    public void MemoizedRecursion_ExampleCircle_ReturnsTheBestNonAdjacentHaul()
    {
        var harness = BuildHarness();

        Assert.Equal(ExpectedMaximumHaul, harness.MemoizedRecursion());
    }

    [Fact]
    public void IterativeTwoPass_ExampleCircle_ReturnsTheBestNonAdjacentHaul()
    {
        var harness = BuildHarness();

        Assert.Equal(ExpectedMaximumHaul, harness.IterativeTwoPass());
    }

    [Fact]
    public void IterativeTwoPass_AgreesWithMemoizedRecursion()
    {
        var harness = BuildHarness();

        Assert.Equal(harness.MemoizedRecursion(), harness.IterativeTwoPass());
    }

    private static HouseRobberIIBenchmarks BuildHarness() => new();
}
