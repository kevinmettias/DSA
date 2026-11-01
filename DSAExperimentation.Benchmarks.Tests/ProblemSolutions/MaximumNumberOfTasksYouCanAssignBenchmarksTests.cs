using DSAExperimentation.Benchmarks.ProblemSolutions;

namespace DSAExperimentation.Benchmarks.Tests.ProblemSolutions;

// Harness coverage for MaximumNumberOfTasksYouCanAssignBenchmarks (ARCHITECTURE 17.9): the answer
// the workload forces is known from Setup's draw ranges alone, an expected value the generic
// agreement check, which only compares arms with each other, cannot supply. The baseline answers
// with a single assignment count, compared directly.
public sealed partial class MaximumNumberOfTasksYouCanAssignBenchmarksTests
{
    private const int SmallestLength = 2_000;

    // Setup draws every task requirement from [500_000, 1_000_000) while the strongest worker
    // reaches only 99 + 5 pills * strength 100 = 599, so no worker can ever take a task and the
    // whole workload is unassignable. This is deliberately the answer the harness forces - it is
    // what makes the linear scan walk its full range before stopping - so it is asserted rather
    // than left implicit.
    private const int ExpectedAssignableTasks = 0;

    [Fact]
    public void LinearScan_UnassignableWorkload_AssignsNoTask() =>
        Assert.Equal(ExpectedAssignableTasks, BuildHarness().LinearScan());

    private static MaximumNumberOfTasksYouCanAssignBenchmarks BuildHarness()
    {
        var harness = new MaximumNumberOfTasksYouCanAssignBenchmarks { Length = SmallestLength };
        harness.Setup();

        return harness;
    }
}
