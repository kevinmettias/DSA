using DSAExperimentation.Benchmarks.ProblemSolutions;

namespace DSAExperimentation.Benchmarks.Tests.ProblemSolutions;

// Harness coverage for HouseRobberBenchmarks (ARCHITECTURE 17.9): the class has a single arm, so
// there is no second strategy to reconcile it against and the assertion has to come from what the
// class comment makes decisive instead - the workload is LeetCode 198's own example street, whose
// best non-adjacent haul is 2 + 9 + 1. The class carries no [Params] and no [GlobalSetup]: the
// fixed five-house literal is the whole workload, so the same street must always report the same
// haul, and the test states that haul rather than reading it back out of the arm.
public sealed partial class HouseRobberBenchmarksTests
{
    private const int ExpectedMaximumHaul = 12;

    [Fact]
    public void MemoizedRecursion_ExampleStreet_ReturnsTheBestNonAdjacentHaul()
    {
        var harness = BuildHarness();

        Assert.Equal(ExpectedMaximumHaul, harness.MemoizedRecursion());
    }

    private static HouseRobberBenchmarks BuildHarness() => new();
}
