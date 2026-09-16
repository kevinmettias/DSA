using DSAExperimentation.Benchmarks.ProblemSolutions;

namespace DSAExperimentation.Benchmarks.Tests.ProblemSolutions;

// Harness coverage for HouseRobberIIBenchmarks (ARCHITECTURE 17.9): the class has a single arm, so
// there is no second strategy to reconcile it against and the assertion has to come from what the
// class comment makes decisive instead - the workload is LeetCode 213's own example, a circle of
// four houses that can only be robbed from one side of the wrap-around, for a best haul of
// 1 + 3. The class carries no [Params] and no [GlobalSetup]: the fixed four-house literal is the
// whole workload, so the same circle must always report the same haul, and the test states that
// haul rather than reading it back out of the arm.
public sealed partial class HouseRobberIIBenchmarksTests
{
    private const int ExpectedMaximumHaul = 4;

    [Fact]
    public void MemoizedRecursion_ExampleCircle_ReturnsTheBestNonAdjacentHaul()
    {
        var harness = BuildHarness();

        Assert.Equal(ExpectedMaximumHaul, harness.MemoizedRecursion());
    }

    private static HouseRobberIIBenchmarks BuildHarness() => new();
}
